import argparse
from email.parser import BytesParser
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import zipfile

from source_inputs import HERE, digest, download, extract, inventory, normalize_patch_line_endings


def command_environment(env=None):
    environment = dict(os.environ if env is None else env)
    # A UTF-8 log file alone does not set the encoding used by child processes.
    environment.update(PYTHONUTF8='1', PYTHONIOENCODING='utf-8')
    return environment


def restore_pdf2docx_version(source):
    metadata = BytesParser().parsebytes((source / 'PKG-INFO').read_bytes(), headersonly=True)
    if metadata.get('Name') != 'pdf2docx' or metadata.get('Version') != '0.5.13':
        raise ValueError('Unexpected pdf2docx source metadata')
    version_file = source / 'version.txt'
    if version_file.exists() and version_file.read_text(encoding='utf-8').strip() != '0.5.13':
        raise ValueError('Conflicting pdf2docx version.txt')
    # The published sdist omits this generated file; setup.py otherwise uses 0.5.6a1.
    version_file.write_text('0.5.13\n', encoding='utf-8')


def validate_pdf2docx_wheel(wheel):
    with zipfile.ZipFile(wheel) as archive:
        names = [name for name in archive.namelist() if name.endswith('.dist-info/METADATA')]
        if len(names) != 1:
            raise ValueError('Expected one pdf2docx wheel metadata file')
        metadata = BytesParser().parsebytes(archive.read(names[0]), headersonly=True)
    if metadata.get('Name') != 'pdf2docx' or metadata.get('Version') != '0.5.13':
        raise ValueError('Unexpected rebuilt pdf2docx version')


def build(root):
    if sys.platform != 'win32' or sys.version_info[:3] != (3, 14, 4):
        raise RuntimeError('Use the workflow Windows x64 Python 3.14.4 environment')
    root.mkdir(parents=True, exist_ok=False)
    logs = root / 'logs'
    logs.mkdir()
    number = 0

    def run(*command, cwd=None, env=None, timeout=3600):
        nonlocal number
        number += 1
        logfile = logs / f'{number:02d}-{Path(str(command[0])).stem}.log'
        print(f'Running {command[0]} (log: {logfile.name})', flush=True)
        with logfile.open('w', encoding='utf-8') as output:
            output.write(json.dumps(list(map(str, command))) + '\n')
            output.flush()
            try:
                subprocess.run(list(map(str, command)), cwd=cwd, env=command_environment(env), check=True,
                               stdout=output, stderr=subprocess.STDOUT, timeout=timeout)
            except (subprocess.CalledProcessError, subprocess.TimeoutExpired):
                output.flush()
                print(f'Failed command; last log lines from {logfile.name}:', flush=True)
                print('\n'.join(logfile.read_text(encoding='utf-8', errors='replace').splitlines()[-80:]), flush=True)
                raise

    inputs = root / 'source-inputs'
    spec = json.loads((HERE / 'sources.json').read_text())
    for item in spec['converter']:
        download(item, inputs)
    shutil.copytree(HERE, inputs / 'build-tools', ignore=shutil.ignore_patterns('__pycache__'))
    workflow = HERE.parents[1] / '.github/workflows/native-candidates.yml'
    shutil.copy2(workflow, inputs / 'native-candidates.yml')
    mupdf = extract(inputs / 'mupdf-1.28.0-source.tar.gz', root / 'mupdf')
    pymupdf = extract(inputs / 'pymupdf-1.28.0.tar.gz', root / 'pymupdf')
    pdf2docx = extract(inputs / 'pdf2docx-0.5.13.tar.gz', root / 'pdf2docx')
    restore_pdf2docx_version(pdf2docx)
    for item in spec['converter']:
        if item['name'].endswith('.patch'):
            patch = inputs / item['name']
            run('git', 'apply', '--check', patch, cwd=mupdf)
            run('git', 'apply', patch, cwd=mupdf)
    for source, name in [
        (mupdf, 'mupdf-vs-upgrade-libdir.patch'),
        (pymupdf, 'pymupdf-vs-upgrade-packaging.patch'),
    ]:
        local_patch = inputs / 'build-tools/patches' / name
        normalize_patch_line_endings(local_patch)
        run('git', 'apply', '--check', local_patch, cwd=source)
        run('git', 'apply', local_patch, cwd=source)
    run(sys.executable, '-m', 'venv', root / 'builder')
    python = root / 'builder/Scripts/python.exe'
    run(python, '-m', 'pip', 'install', '-r', HERE / 'build-requirements.txt')
    run(python, HERE / 'check_build_backend.py', pymupdf)
    run(python, '-m', 'pip', 'download', '--no-deps', '-r', HERE / 'runtime-requirements.txt',
        '--dest', root / 'runtime-downloads')
    old_wheels = list((root / 'runtime-downloads').glob('pymupdf-*.whl'))
    expected = 'e01e90fd86abfeb37ceb921eddb951f988a11d45ff6ce6b7664f2039849068ec'
    if len(old_wheels) != 1 or digest(old_wheels[0]) != expected:
        raise RuntimeError('Reference PyMuPDF wheel differs from the audited Windows wheel')
    wheels = root / 'reference-wheels'
    run(python, '-m', 'pip', 'wheel', '--no-deps', '--no-build-isolation', '--no-index',
        '--find-links', root / 'runtime-downloads', '-r', HERE / 'runtime-requirements.txt', '-w', wheels)
    run(sys.executable, '-m', 'venv', root / 'reference')
    reference = root / 'reference/Scripts/python.exe'
    for target in [reference, python]:
        run(target, '-m', 'pip', 'install', '--no-index', '--no-deps', '--find-links', wheels,
            '-r', HERE / 'runtime-requirements.txt')
        run(target, '-m', 'pip', 'check')
    env = os.environ.copy()
    for key in list(env):
        if key.startswith('PYMUPDF_SETUP_'):
            del env[key]
    env.update(PYMUPDF_SETUP_MUPDF_BUILD=str(mupdf), PYMUPDF_SETUP_MUPDF_VS_UPGRADE='1',
               PIPCL_SHOW_ENV='0', WDEV_VS_YEAR='2022')
    new_wheels = root / 'rebuilt-wheels'
    run(python, '-m', 'pip', 'wheel', '--no-deps', '--no-build-isolation', pymupdf, '-w', new_wheels, env=env)
    run(python, '-m', 'pip', 'wheel', '--no-deps', '--no-build-isolation', pdf2docx, '-w', new_wheels)
    validate_pdf2docx_wheel(new_wheels / 'pdf2docx-0.5.13-py3-none-any.whl')
    rebuilt = list(new_wheels.glob('*.whl'))
    if len(rebuilt) != 2:
        raise RuntimeError('Expected rebuilt PyMuPDF and pdf2docx wheels')
    run(python, '-m', 'pip', 'install', '--no-deps', '--force-reinstall', *rebuilt)
    run(python, '-m', 'pip', 'check')
    shutil.copy2(mupdf / 'include/mupdf/fitz/config.h', inputs / 'mupdf-config-used.h')
    run(python, '-m', 'pip', 'freeze', '--all')
    run(python, '-m', 'pip', 'inspect')
    # Build from the unchanged wrapper; packaging alone omits System32's MSVCP140.
    run(python, '-m', 'PyInstaller', '--noconfirm', '--clean', '--distpath', root / 'dist',
        '--workpath', root / 'pyinstaller-work', HERE / 'pdf2docx_convert.spec', cwd=HERE)
    exe = root / 'dist/pdf2docx_convert/pdf2docx_convert.exe'
    if not exe.is_file() or (exe.parent / '_internal/MSVCP140.dll').exists():
        raise RuntimeError('Unexpected frozen converter layout/runtime')
    run(reference, HERE / 'compare_converter.py', '--candidate', exe, '--output', root / 'comparison', timeout=600)
    # Preserve package metadata/notices, including Python's own Windows terms.
    licenses = exe.parent / 'licenses'
    licenses.mkdir()
    for metadata in (python.parents[1] / 'Lib/site-packages').glob('*.dist-info'):
        shutil.copytree(metadata, licenses / metadata.name)
    shutil.copy2(Path(sys.base_prefix) / 'LICENSE.txt', licenses / 'CPython-LICENSE.txt')
    shutil.copy2(mupdf / 'COPYING', licenses / 'MuPDF-COPYING')
    # Include archived inputs, recipes and logs, never the runner's whole workdir.
    artifacts = root / 'artifacts'
    artifacts.mkdir()
    (exe.parent / 'CANDIDATE.txt').write_text(
        'Candidate only. Requires system Visual C++ v14 x64 runtime.\n'
        'Synthetic conversion comparisons passed; test the real application before replacement.\n'
        'This does not certify the licensing of the entire application bundle.\n')
    shutil.make_archive(str(artifacts / 'pdf2docx-candidate'), 'zip', exe.parent)
    shutil.make_archive(str(artifacts / 'converter-source-inputs'), 'zip', inputs)
    shutil.make_archive(str(artifacts / 'runtime-downloads'), 'zip', root / 'runtime-downloads')
    shutil.copytree(new_wheels, artifacts / 'rebuilt-wheels')
    shutil.copytree(root / 'comparison', artifacts / 'comparison')
    shutil.copytree(logs, artifacts / 'build-logs')
    (artifacts / 'BUILD.json').write_text(json.dumps({
        'commit': os.environ.get('GITHUB_SHA'), 'run': os.environ.get('GITHUB_RUN_ID'),
        'runnerImage': os.environ.get('ImageVersion'), 'python': sys.version,
        'mupdfBuildDirectory': str(mupdf), 'files': inventory(artifacts),
        'status': 'Candidate only; not automatically copied to the application'
    }, indent=2))


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--output', required=True, type=Path)
    build(parser.parse_args().output.resolve())
