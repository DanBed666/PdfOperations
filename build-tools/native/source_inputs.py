import argparse
import hashlib
import json
from pathlib import Path
import shutil
import tarfile
import urllib.request

HERE = Path(__file__).resolve().parent


def normalize_patch_line_endings(path):
    data = path.read_bytes()
    normalized = data.replace(b'\r\n', b'\n')
    if data != normalized:
        path.write_bytes(normalized)


def digest(path):
    with Path(path).open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def download(item, directory):
    name = item['name']
    if Path(name).name != name or '/' in name or '\\' in name:
        raise ValueError('Expected a filename, not a path')
    directory.mkdir(parents=True, exist_ok=True)
    target = directory / name
    if not target.exists():
        partial = target.with_name(target.name + '.partial')
        try:
            with urllib.request.urlopen(item['url'], timeout=120) as response, partial.open('wb') as output:
                shutil.copyfileobj(response, output)
            if digest(partial) != item['sha256']:
                raise ValueError(f'Checksum mismatch: {name}')
            partial.replace(target)
        finally:
            partial.unlink(missing_ok=True)
    if digest(target) != item['sha256']:
        raise ValueError(f'Checksum mismatch: {name}')
    return target


def extract(archive, destination):
    destination.mkdir(parents=True, exist_ok=False)
    with tarfile.open(archive) as source:
        source.extractall(destination, filter='data')
    children = list(destination.iterdir())
    if len(children) != 1 or not children[0].is_dir():
        raise ValueError(f'Expected one source root: {archive}')
    return children[0]


def inventory(directory):
    return [{'path': x.relative_to(directory).as_posix(), 'bytes': x.stat().st_size, 'sha256': digest(x)}
            for x in sorted(directory.rglob('*')) if x.is_file()]


def prepare_ocr(package, output):
    manifest = json.loads((HERE / 'sources.json').read_text())
    spec = manifest['ocr'][package]
    if output.exists():
        raise FileExistsError(output)
    inputs = output / 'inputs'
    inputs.mkdir(parents=True)
    (output / 'logs').mkdir()
    for item in spec['files']:
        download(item, inputs)
    (output / 'logs/source-inputs.json').write_text(json.dumps(spec, indent=2))
    shutil.copy2(HERE / 'build_ocr.sh', output / 'logs/build_ocr.sh')
    shutil.copy2(HERE / 'sources.json', output / 'logs/sources.json')


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--package', required=True,
                        choices=list(json.loads((HERE / 'sources.json').read_text())['ocr']))
    parser.add_argument('--output', required=True, type=Path)
    args = parser.parse_args()
    prepare_ocr(args.package, args.output.resolve())
