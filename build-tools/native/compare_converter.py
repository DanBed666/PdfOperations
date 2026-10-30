import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import sys
import xml.etree.ElementTree as ET
import zipfile


def contents(path):
    result = {}
    with zipfile.ZipFile(path) as source:
        document = ET.fromstring(source.read('word/document.xml'))
        if not any(x.text and x.text.strip() for x in document.iter('{http://schemas.openxmlformats.org/wordprocessingml/2006/main}t')):
            raise AssertionError('Empty output document')
        for name in source.namelist():
            data = source.read(name)
            if name == 'docProps/core.xml':
                node = ET.fromstring(data)
                for child in list(node):
                    if child.tag in ('{http://purl.org/dc/terms/}created', '{http://purl.org/dc/terms/}modified'):
                        node.remove(child)
                data = ET.tostring(node)
            result[name] = hashlib.sha256(data).hexdigest()
    return result


def compare(candidate, output):
    import pymupdf
    from PIL import Image, ImageDraw

    output.mkdir(parents=True, exist_ok=False)
    image = Image.new('RGB', (320, 100), 'white')
    ImageDraw.Draw(image).text((20, 35), 'IMAGE TEST 123', fill='black', font_size=30)
    image.save(output / 'image.png')
    for kind in ('text', 'pages', 'image'):
        doc = pymupdf.open()
        for index in range(3 if kind == 'pages' else 1):
            page = doc.new_page()
            page.insert_text((50, 50), f'Conversion check: {kind}, page {index + 1}', fontsize=18)
            page.insert_text((50, 90), 'Alpha 12345. Beta 67890. Example only.', fontsize=12)
            if kind == 'image':
                page.insert_image(pymupdf.Rect(50, 130, 370, 230), filename=str(output / 'image.png'))
        doc.save(output / f'{kind}.pdf')
        doc.close()
    results = []
    wrapper = Path(__file__).with_name('pdf2docx_convert.py')
    for kind in ('text', 'pages', 'image'):
        input_file = output / f'{kind}.pdf'
        for label, command in [('reference', [sys.executable, str(wrapper)]), ('candidate', [str(candidate)])]:
            target = output / f'{kind}-{label}.docx'
            with (output / f'{kind}-{label}.log').open('wb') as log:
                subprocess.run([*command, str(input_file), str(target)], check=True,
                               stdout=log, stderr=subprocess.STDOUT, timeout=120)
        if contents(output / f'{kind}-reference.docx') != contents(output / f'{kind}-candidate.docx'):
            raise AssertionError(f'Conversion differs: {kind}; do not replace the original converter')
        results.append({'case': kind, 'equalExceptTimestamps': True})
    (output / 'results.json').write_text(json.dumps(results, indent=2))


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--candidate', required=True, type=Path)
    parser.add_argument('--output', required=True, type=Path)
    args = parser.parse_args()
    compare(args.candidate.resolve(), args.output.resolve())
