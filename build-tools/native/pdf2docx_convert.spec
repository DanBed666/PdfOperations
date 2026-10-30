a = Analysis(['pdf2docx_convert.py'], pathex=[], binaries=[], datas=[],
             hiddenimports=[], hookspath=[], hooksconfig={}, runtime_hooks=[],
             excludes=[], noarchive=False, optimize=0)
# Use the user's installed Microsoft runtime, not a copy from the build host.
a.binaries = [entry for entry in a.binaries if entry[0].replace('\\', '/').split('/')[-1].lower() != 'msvcp140.dll']
pyz = PYZ(a.pure)
exe = EXE(pyz, a.scripts, [], exclude_binaries=True, name='pdf2docx_convert',
          debug=False, bootloader_ignore_signals=False, strip=False, upx=False,
          console=True, disable_windowed_traceback=False)
coll = COLLECT(exe, a.binaries, a.datas, strip=False, upx=False, name='pdf2docx_convert')
