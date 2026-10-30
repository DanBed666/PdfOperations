import hashlib
from contextlib import contextmanager
import io
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tarfile
import tempfile
import unittest
import uuid
from unittest.mock import Mock, patch
import zipfile

HERE = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(HERE))
from source_inputs import digest, download, extract, inventory, normalize_patch_line_endings
from compare_converter import contents
from check_build_backend import check_source_metadata, check_windows_library_paths
from build_converter import command_environment, restore_pdf2docx_version, validate_pdf2docx_wheel


@contextmanager
def temporary_directory():
    # Avoid owner-only ACLs from Python 3.14 tempfile under restricted Windows tokens.
    root = Path(os.environ.get('RUNNER_TEMP', tempfile.gettempdir())).resolve()
    directory = root / ('native-tests-' + uuid.uuid4().hex)
    directory.mkdir()
    try:
        yield str(directory)
    finally:
        if directory.resolve().parent != root:
            raise RuntimeError('Unexpected test cleanup path')
        shutil.rmtree(directory)


class CommandEncodingTests(unittest.TestCase):
    def test_inherits_environment_without_mutating_it(self):
        with patch.dict(os.environ, {'BUILD_TEST': 'preserved', 'PYTHONUTF8': '0'}, clear=True):
            environment = command_environment()
            self.assertEqual(environment['BUILD_TEST'], 'preserved')
            self.assertEqual(environment['PYTHONUTF8'], '1')
            self.assertEqual(environment['PYTHONIOENCODING'], 'utf-8')
            self.assertEqual(os.environ['PYTHONUTF8'], '0')
            self.assertNotIn('PYTHONIOENCODING', os.environ)

    def test_preserves_custom_build_environment(self):
        original = {'PYMUPDF_SETUP_MUPDF_BUILD': 'local-source', 'WDEV_VS_YEAR': '2022',
                    'PYTHONIOENCODING': 'cp1252', 'PYTHONUTF8': '0'}
        environment = command_environment(original)
        self.assertEqual(environment['PYMUPDF_SETUP_MUPDF_BUILD'], 'local-source')
        self.assertEqual(environment['WDEV_VS_YEAR'], '2022')
        self.assertEqual(environment['PYTHONIOENCODING'], 'utf-8')
        self.assertEqual(original['PYTHONIOENCODING'], 'cp1252')

    def test_redirected_unicode_output_reproduces_failure_and_passes_with_utf8(self):
        legacy = dict(os.environ, PYTHONUTF8='0', PYTHONIOENCODING='cp1252')
        command = [sys.executable, '-c', "print(chr(0x1f44b) + ' ' + chr(0x142))"]
        with temporary_directory() as folder:
            logfile = Path(folder) / 'command.log'
            with logfile.open('w', encoding='utf-8') as output:
                failed = subprocess.run(command, env=legacy, stdout=output,
                                        stderr=subprocess.PIPE, timeout=30)
            self.assertNotEqual(failed.returncode, 0)
            self.assertIn(b'UnicodeEncodeError', failed.stderr)
            with logfile.open('w', encoding='utf-8') as output:
                subprocess.run(command, env=command_environment(legacy), stdout=output,
                               stderr=subprocess.PIPE, timeout=30, check=True)
            self.assertEqual(logfile.read_text(encoding='utf-8').strip(), chr(0x1f44b) + ' ' + chr(0x142))


class Pdf2DocxVersionTests(unittest.TestCase):
    def test_restores_version_from_pinned_source_metadata(self):
        with temporary_directory() as folder:
            root = Path(folder)
            (root / 'PKG-INFO').write_bytes(b'Name: pdf2docx\nVersion: 0.5.13\n')
            restore_pdf2docx_version(root)
            restore_pdf2docx_version(root)
            self.assertEqual((root / 'version.txt').read_text().strip(), '0.5.13')

    def test_rejects_other_source_version(self):
        with temporary_directory() as folder:
            root = Path(folder)
            (root / 'PKG-INFO').write_bytes(b'Name: pdf2docx\nVersion: 0.5.6a1\n')
            with self.assertRaisesRegex(ValueError, 'source metadata'):
                restore_pdf2docx_version(root)
            self.assertFalse((root / 'version.txt').exists())

    def test_keeps_conflicting_version_file(self):
        with temporary_directory() as folder:
            root = Path(folder)
            (root / 'PKG-INFO').write_bytes(b'Name: pdf2docx\nVersion: 0.5.13\n')
            (root / 'version.txt').write_bytes(b'0.5.6a1\n')
            with self.assertRaisesRegex(ValueError, 'Conflicting'):
                restore_pdf2docx_version(root)
            self.assertEqual((root / 'version.txt').read_bytes(), b'0.5.6a1\n')

    def test_checks_built_wheel_metadata_not_just_filename(self):
        with temporary_directory() as folder:
            wheel = Path(folder) / 'pdf2docx-0.5.13-py3-none-any.whl'
            for version in ('0.5.13', '0.5.6a1'):
                with zipfile.ZipFile(wheel, 'w') as archive:
                    archive.writestr('pdf2docx.dist-info/METADATA', f'Name: pdf2docx\nVersion: {version}\n')
                if version == '0.5.13':
                    validate_pdf2docx_wheel(wheel)
                else:
                    with self.assertRaisesRegex(ValueError, 'rebuilt pdf2docx version'):
                        validate_pdf2docx_wheel(wheel)


class OcrRecipePatchTests(unittest.TestCase):
    def apply_source_patch(self, root, relative_path, original, patch_name):
        target = root / relative_path
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(original.encode('utf-8'))
        subprocess.run(['git', 'init', '--quiet', str(root)], check=True, capture_output=True)
        patch_file = root / patch_name
        shutil.copy2(HERE / 'patches' / patch_name, patch_file)
        normalize_patch_line_endings(patch_file)
        for check in (True, False):
            subprocess.run(['git', 'apply', *(['--check'] if check else []), str(patch_file)],
                           cwd=root, check=True, capture_output=True)
        return target.read_text()

    def test_cairo_patch_guards_operators_without_removing_fallback(self):
        original = (
            'typedef DWRITE_COLOR_GLYPH_RUN1 DWRITE_COLOR_GLYPH_RUN1_WORKAROUND;\n'
            '#endif\n\n'
            'DEFINE_ENUM_FLAG_OPERATORS(DWRITE_GLYPH_IMAGE_FORMATS);\n\n'
            '#endif /* DWRITE_EXTRA_H */\n'
        )
        with temporary_directory() as folder:
            patched = self.apply_source_patch(Path(folder), 'src/win32/dw-extra.h',
                                              original, 'cairo-mingw-enum.patch')
            self.assertIn(
                '#if !defined(__MINGW64_VERSION_MAJOR) || __MINGW64_VERSION_MAJOR < 11\n'
                'DEFINE_ENUM_FLAG_OPERATORS(DWRITE_GLYPH_IMAGE_FORMATS);\n#endif', patched)
            self.assertEqual(patched.count('DEFINE_ENUM_FLAG_OPERATORS('), 1)

    def test_glib_launcher_supports_current_and_legacy_setuptools(self):
        import ast
        from types import SimpleNamespace

        original = (
            'import sys\nimport re\nimport os\n'
            'from setuptools.command.easy_install import get_win_launcher\n\n'
            'path = sys.argv[1]\n'
        )
        with temporary_directory() as folder:
            patched = self.apply_source_patch(Path(folder), 'pyscript2exe.py',
                                              original, 'glib2-setuptools-launcher.patch')
            block = next(node for node in ast.parse(patched).body if isinstance(node, ast.Try))
            code = compile(ast.Module(body=[block], type_ignores=[]), '<launcher imports>', 'exec')
            launcher = Mock(return_value=b'MZ-test-launcher')
            for legacy in (False, True):
                with self.subTest(legacy=legacy):
                    calls = []

                    def import_helper(name, *args, **kwargs):
                        calls.append(name)
                        if name == 'setuptools._scripts' and legacy:
                            raise ImportError('old setuptools')
                        return SimpleNamespace(get_win_launcher=launcher)

                    namespace = {'__builtins__': dict(vars(__import__('builtins')), __import__=import_helper)}
                    exec(code, namespace)
                    self.assertIs(namespace['get_win_launcher'], launcher)
                    self.assertEqual(calls, ['setuptools._scripts'] +
                                     (['setuptools.command.easy_install'] if legacy else []))

    def test_cairo_and_glib_recipes_include_checked_source_patches(self):
        for name, prepare_line in (
            ('cairo-mingw-enum', '+  patch -p1 -i "${srcdir}/cairo-mingw-enum.patch"'),
            ('glib2-setuptools-launcher',
             '+  (cd "${srcdir}/launcher-helper" && patch -Np1 -i "${srcdir}/glib2-setuptools-launcher.patch")'),
        ):
            with self.subTest(name=name):
                data = (HERE / 'patches' / (name + '.patch')).read_bytes().replace(b'\r\n', b'\n')
                recipe = (HERE / 'patches' / (name + '-recipe.patch')).read_text()
                self.assertIn('+source+=(' + name + '.patch)', recipe)
                self.assertIn("+sha256sums+=('" + hashlib.sha256(data).hexdigest() + "')", recipe)
                self.assertIn(prepare_line, recipe)
                if name == 'glib2-setuptools-launcher':
                    self.assertIn('+  cp "${srcdir}/pyscript2exe.py" "${srcdir}/launcher-helper/"', recipe)
                    self.assertIn('+      "${srcdir}/launcher-helper/pyscript2exe.py"', recipe)

    def test_gettext_patch_updates_both_environment_arrays(self):
        original = '''      char **env = _environ;
      wchar_t **wenv = _wenviron;
      if (env != NULL)
        for (char *s = env; *s != NULL; s++)
          if (s[0] == 'T' && s[1] == 'Z' && s[2] == '=')
            s[0] = '$';
      if (wenv != NULL)
        for (wchar_t *ws = wenv; *ws != NULL; ws++)
          if (ws[0] == L'T' && ws[1] == L'Z' && ws[2] == L'=')
            ws[0] = L'$';
    }
#endif

'''
        with temporary_directory() as folder:
            root = Path(folder)
            target = root / 'gettext-tools/gnulib-lib/localtime.c'
            target.parent.mkdir(parents=True)
            target.write_bytes(original.encode('utf-8'))
            subprocess.run(['git', 'init', '--quiet', str(root)], check=True, capture_output=True)
            patch_file = root / 'gettext-localtime.patch'
            shutil.copy2(HERE / 'patches/gettext-localtime.patch', patch_file)
            normalize_patch_line_endings(patch_file)
            for check in (True, False):
                subprocess.run(['git', 'apply', *(['--check'] if check else []), str(patch_file)],
                               cwd=root, check=True, capture_output=True)
            patched = target.read_text()
            self.assertIn('for (char **ep = env;', patched)
            self.assertIn('char *s = *ep;', patched)
            self.assertIn('for (wchar_t **wep = wenv;', patched)
            self.assertIn('wchar_t *ws = *wep;', patched)
            self.assertNotIn('for (char *s = env;', patched)
            self.assertNotIn('for (wchar_t *ws = wenv;', patched)

    def test_gettext_recipe_keeps_source_patch_checksum(self):
        data = (HERE / 'patches/gettext-localtime.patch').read_bytes().replace(b'\r\n', b'\n')
        recipe_patch = (HERE / 'patches/gettext-localtime-recipe.patch').read_text()
        self.assertIn("+sha256sums+=('" + hashlib.sha256(data).hexdigest() + "')", recipe_patch)
        self.assertIn('+source+=(gettext-localtime.patch)', recipe_patch)
        self.assertIn('+  apply_patch_with_msg gettext-localtime.patch', recipe_patch)

    def test_libiconv_patch_preserves_version_and_existing_flags(self):
        original = (
            'pkgver=1.17\npkgrel=4\n'
            'build() {\n'
            '  [[ -d "${srcdir}/build-${MSYSTEM}" ]] && rm -rf "${srcdir}/build-${MSYSTEM}"\n'
            '  mkdir -p "${srcdir}/build-${MSYSTEM}" && cd "${srcdir}/build-${MSYSTEM}"\n'
            '}\n'
        )
        with temporary_directory() as folder:
            root = Path(folder)
            recipe = root / 'PKGBUILD'
            recipe.write_bytes(original.encode('utf-8'))
            subprocess.run(['git', 'init', '--quiet', str(root)], check=True, capture_output=True)
            patch_file = root / 'libiconv.patch'
            shutil.copy2(HERE / 'patches/libiconv-gnu17.patch', patch_file)
            normalize_patch_line_endings(patch_file)
            for check in (True, False):
                subprocess.run(['git', 'apply', *(['--check'] if check else []), str(patch_file)],
                               cwd=root, check=True, capture_output=True)
            patched = recipe.read_text()
            self.assertIn('pkgver=1.17\npkgrel=4\n', patched)
            self.assertEqual(patched.count('-std=gnu17'), 1)
            self.assertIn('export CFLAGS="${CFLAGS:-} -std=gnu17"', patched)


class SourceTests(unittest.TestCase):
    def test_manifest_has_pinned_sources(self):
        manifest = json.loads((HERE / 'sources.json').read_text())
        self.assertEqual(len(manifest['ocr']), 7)
        groups = [manifest['converter'], *(x['files'] for x in manifest['ocr'].values())]
        for group in groups:
            names = [x['name'] for x in group]
            self.assertEqual(len(names), len(set(names)))
            for item in group:
                self.assertRegex(item['sha256'], r'^[0-9a-f]{64}$')
                self.assertTrue(item['url'].startswith('https://'))
                self.assertEqual(Path(item['name']).name, item['name'])

    def test_valid_download(self):
        with temporary_directory() as folder:
            item = {'name': 'source.bin', 'url': 'https://example.invalid/source',
                    'sha256': hashlib.sha256(b'source').hexdigest()}
            with patch('urllib.request.urlopen', return_value=io.BytesIO(b'source')):
                path = download(item, Path(folder))
            self.assertEqual(digest(path), item['sha256'])

    def test_bad_download_is_not_promoted(self):
        with temporary_directory() as folder:
            item = {'name': 'source.bin', 'url': 'https://example.invalid/source', 'sha256': '0' * 64}
            with patch('urllib.request.urlopen', return_value=io.BytesIO(b'wrong')):
                with self.assertRaises(ValueError):
                    download(item, Path(folder))
            self.assertEqual(list(Path(folder).iterdir()), [])

    def test_existing_download_is_rechecked(self):
        with temporary_directory() as folder:
            (Path(folder) / 'source.bin').write_bytes(b'wrong')
            with self.assertRaises(ValueError):
                download({'name': 'source.bin', 'sha256': '0' * 64}, Path(folder))

    def test_download_rejects_path_traversal(self):
        with self.assertRaises(ValueError):
            download({'name': '../outside'}, Path('.'))

    def test_extract_rejects_traversal(self):
        with temporary_directory() as folder:
            archive = Path(folder) / 'bad.tar'
            with tarfile.open(archive, 'w') as target:
                item = tarfile.TarInfo('../outside.txt')
                item.size = 3
                target.addfile(item, io.BytesIO(b'bad'))
            with self.assertRaises(tarfile.FilterError):
                extract(archive, Path(folder) / 'extracted')
            self.assertFalse((Path(folder) / 'outside.txt').exists())

    def test_inventory_is_content_based(self):
        with temporary_directory() as folder:
            root = Path(folder)
            (root / 'input').write_bytes(b'abc')
            self.assertEqual(inventory(root), [{'path': 'input', 'bytes': 3,
                              'sha256': hashlib.sha256(b'abc').hexdigest()}])


class ComparisonTests(unittest.TestCase):
    def docx(self, root, name, text, timestamp):
        path = root / name
        with zipfile.ZipFile(path, 'w') as target:
            target.writestr('word/document.xml', '<w:document xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"><w:t>' + text + '</w:t></w:document>')
            target.writestr('docProps/core.xml', '<core xmlns:d="http://purl.org/dc/terms/"><d:created>' + timestamp + '</d:created></core>')
        return path

    def test_ignores_only_timestamp(self):
        with temporary_directory() as folder:
            root = Path(folder)
            left = self.docx(root, 'left.docx', 'Same text', '2025')
            right = self.docx(root, 'right.docx', 'Same text', '2026')
            self.assertEqual(contents(left), contents(right))

    def test_detects_changed_text(self):
        with temporary_directory() as folder:
            root = Path(folder)
            left = self.docx(root, 'left.docx', 'First text', '2026')
            right = self.docx(root, 'right.docx', 'Second text', '2026')
            self.assertNotEqual(contents(left), contents(right))

    def test_rejects_empty_output(self):
        with temporary_directory() as folder:
            path = self.docx(Path(folder), 'empty.docx', '', '2026')
            with self.assertRaises(AssertionError):
                contents(path)


class BuildMetadataTests(unittest.TestCase):
    def test_accepts_archive_without_git_metadata(self):
        backend = Mock()
        backend.git_info_py.return_value = '\n'.join(
            f'pymupdf_git_{name} = None' for name in ('sha', 'comment', 'branch', 'diff'))
        check_source_metadata(backend, Path('source-archive'))
        backend.git_info_py.assert_called_once_with('source-archive', check=0, prefix='pymupdf_git_')

    def test_rejects_incomplete_metadata(self):
        backend = Mock()
        backend.git_info_py.return_value = 'pymupdf_git_sha = None'
        with self.assertRaises(ValueError):
            check_source_metadata(backend, Path('source-archive'))

    def test_surfaces_backend_failure(self):
        backend = Mock()
        backend.git_info_py.side_effect = TypeError("object of type 'NoneType' has no len()")
        with self.assertRaises(TypeError):
            check_source_metadata(backend, Path('source-archive'))


class PackagingPathTests(unittest.TestCase):
    original = (
        'def _windows_lib_directory(mupdf_local, build_type):\n'
        '    wc = pipcl.wdev.WindowsCpu()\n'
        "    ret = f'{mupdf_local}/platform/win32/{wc.windows_subdir}'\n"
        "    if build_type == 'release':\n"
        "        ret += 'Release/'\n"
        "    elif build_type == 'debug':\n"
        "        ret += 'Debug/'\n"
        '    else:\n'
        "        assert 0, f'Unrecognised {build_type=}.'\n"
        '    return ret\n'
    )

    def test_preflight_detects_original_packaging_bug(self):
        with temporary_directory() as folder:
            root = Path(folder)
            (root / 'setup.py').write_bytes(self.original.encode('utf-8'))
            with self.assertRaisesRegex(ValueError, 'packaging path mismatch'):
                check_windows_library_paths(root)

    def test_patch_fixes_packaging_with_and_without_vs_upgrade(self):
        with temporary_directory() as folder:
            root = Path(folder)
            # The preflight must not execute setup.py's top-level build actions.
            (root / 'setup.py').write_bytes(
                ("raise RuntimeError('Do not import setup.py')\n\n" + self.original).encode('utf-8'))
            subprocess.run(['git', 'init', '--quiet', str(root)], check=True, capture_output=True)
            patch_file = root / 'packaging.patch'
            data = (HERE / 'patches/pymupdf-vs-upgrade-packaging.patch').read_bytes().replace(b'\r\n', b'\n')
            patch_file.write_bytes(data.replace(b'\n', b'\r\n'))
            normalize_patch_line_endings(patch_file)
            for check in (True, False):
                subprocess.run(['git', 'apply', *(['--check'] if check else []), str(patch_file)],
                               cwd=root, check=True, capture_output=True)
            check_windows_library_paths(root)

    def test_preflight_rejects_missing_helper(self):
        with temporary_directory() as folder:
            root = Path(folder)
            (root / 'setup.py').write_bytes(b'')
            with self.assertRaisesRegex(ValueError, 'Missing or ambiguous'):
                check_windows_library_paths(root)

    def test_preflight_rejects_duplicate_helper(self):
        with temporary_directory() as folder:
            root = Path(folder)
            (root / 'setup.py').write_bytes((self.original * 2).encode('utf-8'))
            with self.assertRaisesRegex(ValueError, 'Missing or ambiguous'):
                check_windows_library_paths(root)


class MuPdfPatchTests(unittest.TestCase):
    def test_normalizes_windows_patch(self):
        with temporary_directory() as folder:
            path = Path(folder) / 'fix.patch'
            path.write_bytes(b'--- original\r\n+++ changed\r\n')
            normalize_patch_line_endings(path)
            self.assertEqual(path.read_bytes(), b'--- original\n+++ changed\n')

    def test_keeps_unix_patch(self):
        with temporary_directory() as folder:
            path = Path(folder) / 'fix.patch'
            data = b'--- original\n+++ changed\n'
            path.write_bytes(data)
            normalize_patch_line_endings(path)
            self.assertEqual(path.read_bytes(), data)

    def test_linker_uses_selected_vs_directory(self):
        import ast
        from types import SimpleNamespace

        original = (
            '                if state.state_.windows:\n'
            '\n'
            "                    libdir = f'{build_dirs.dir_mupdf}/platform/win32/'\n"
            "                    mupdfcpp_lib = f'{build_dirs.dir_mupdf}/platform/win32/'\n"
            '                    if build_dirs.cpu.bits == 64:\n'
            "                        libdir += 'x64/'\n"
            "                    libdir += 'Debug/' if debug else 'Memento/' if memento else 'Release/'\n"
            '                    libs = list()\n'
        )
        with temporary_directory() as folder:
            root = Path(folder)
            target = root / 'scripts/wrap/__main__.py'
            target.parent.mkdir(parents=True)
            target.write_bytes(original.encode('utf-8'))
            subprocess.run(['git', 'init', '--quiet', str(root)], check=True, capture_output=True)
            patch_file = root / 'windows-checkout.patch'
            patch_data = (HERE / 'patches/mupdf-vs-upgrade-libdir.patch').read_bytes().replace(b'\r\n', b'\n')
            patch_file.write_bytes(patch_data.replace(b'\n', b'\r\n'))
            normalize_patch_line_endings(patch_file)
            for check in (True, False):
                subprocess.run(['git', 'apply', *(['--check'] if check else []), str(patch_file)],
                               cwd=root, check=True, capture_output=True)
            # Execute only the patched path-selection block, not MuPDF or a compiler.
            code = '\n'.join(line[16:] if line.startswith(' ' * 16) else line
                             for line in target.read_text().splitlines())
            parsed = compile(ast.parse(code), '<patched linker path>', 'exec')
            for upgrade, directory in [(False, 'win32'), (True, 'win32-vs-upgrade')]:
                namespace = {
                    'state': SimpleNamespace(state_=SimpleNamespace(windows=True)),
                    'build_dirs': SimpleNamespace(dir_mupdf='source', cpu=SimpleNamespace(bits=64)),
                    'vs_upgrade': upgrade, 'devenv': 'compiler', 'debug': False, 'memento': False,
                    '_windows_vs_upgrade': lambda enabled, *_: 'win32-vs-upgrade' if enabled else 'win32',
                }
                exec(parsed, namespace)
                self.assertEqual(namespace['libdir'], f'source/platform/{directory}/x64/Release/')


if __name__ == '__main__':
    unittest.main()
