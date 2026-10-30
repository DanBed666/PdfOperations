"""Check source metadata and packaging paths before compiling MuPDF."""
import argparse
import ast
import importlib.metadata
from pathlib import Path
from types import SimpleNamespace


def check_windows_library_paths(source):
    setup = source / 'setup.py'
    tree = ast.parse(setup.read_text(encoding='utf-8'))
    functions = [node for node in tree.body
                 if isinstance(node, ast.FunctionDef) and node.name == '_windows_lib_directory']
    if len(functions) != 1:
        raise ValueError('Missing or ambiguous PyMuPDF Windows library directory helper')
    # Execute the actual path helper without importing setup.py or invoking a compiler.
    code = compile(ast.Module(body=functions, type_ignores=[]), str(setup), 'exec')
    for upgrade in (None, '0', '1'):
        environment = {} if upgrade is None else {'PYMUPDF_SETUP_MUPDF_VS_UPGRADE': upgrade}
        infix = 'win32-vs-upgrade' if upgrade == '1' else 'win32'
        for cpu in ('', 'x64/', 'arm64/'):
            namespace = {
                'os': SimpleNamespace(environ=environment),
                'pipcl': SimpleNamespace(wdev=SimpleNamespace(
                    WindowsCpu=lambda: SimpleNamespace(windows_subdir=cpu))),
            }
            exec(code, namespace)
            for mode, directory in [('release', 'Release'), ('debug', 'Debug')]:
                actual = namespace['_windows_lib_directory']('mupdf-source', mode)
                expected = f'mupdf-source/platform/{infix}/{cpu}{directory}/'
                if actual != expected:
                    raise ValueError(f'PyMuPDF packaging path mismatch: {actual!r}; expected {expected!r}')
    print('Windows packaging paths passed (18 configurations; both .lib files use this directory)')


def check_source_metadata(backend, source):
    text = backend.git_info_py(str(source), check=0, prefix='pymupdf_git_')
    tree = ast.parse(text)
    values = {}
    for statement in tree.body:
        if not isinstance(statement, ast.Assign) or len(statement.targets) != 1:
            raise ValueError('Unexpected build metadata statement')
        target = statement.targets[0]
        if not isinstance(target, ast.Name):
            raise ValueError('Unexpected build metadata target')
        values[target.id] = ast.literal_eval(statement.value)
    required = {'pymupdf_git_sha', 'pymupdf_git_comment', 'pymupdf_git_branch', 'pymupdf_git_diff'}
    if not required.issubset(values):
        raise ValueError('Incomplete build metadata')
    print('Source metadata check passed; Git revision:', values['pymupdf_git_sha'])


if __name__ == '__main__':
    import pipcl

    parser = argparse.ArgumentParser()
    parser.add_argument('source', type=Path)
    args = parser.parse_args()
    print('Build backend pipcl:', importlib.metadata.version('pipcl'))
    check_source_metadata(pipcl, args.source.resolve())
    check_windows_library_paths(args.source.resolve())
