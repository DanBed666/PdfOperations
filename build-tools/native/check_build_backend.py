"""Check source-archive metadata support before spending time compiling MuPDF."""
import argparse
import ast
import importlib.metadata
from pathlib import Path


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
