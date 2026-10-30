#!/usr/bin/env bash
# Exercise orchestration only: mocked package builds never produce usable binaries.
set -euo pipefail
root=$1
script=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/build_ocr.sh
mkdir -p "$root"
root=$(cd "$root" && pwd)
export MSYSTEM=MINGW64

mkdir() {
  for argument in "$@"; do
    case "$argument" in -m|--mode|--mode=*|-m[0-9]*)
      printf '%s\n' 'Simulated Windows chmod failure' >&2
      return 77
      ;;
    esac
  done
  command mkdir "$@"
}
gpg() {
  test -d "$GNUPGHOME"
  test "$*" = '--batch --keyserver hkps://keyserver.ubuntu.com --recv-keys TEST-PUBLIC-KEY'
  printf '%s\n' "$*" > "$GNUPGHOME/verification-call.txt"
}
pacman() {
  test "$*" = '-Q'
  printf '%s\n' 'mock-environment 1.0'
}
makepkg-mingw() {
  printf '%s\n' "$*" >> "$PKGDEST/calls.txt"
  case "$*" in
    '--allsource --noconfirm')
      if test "$OCR_TEST_MODE" = source-failure; then return 42; fi
      printf '%s\n' 'TEST FIXTURE, NOT A SOURCE PACKAGE' > "$SRCPKGDEST/mock.src.tar.gz"
      ;;
    '--syncdeps --noconfirm --cleanbuild --log')
      printf '%s\n' 'compiler diagnostic fixture' > mock-build.log
      printf '%s\n' 'TEST FIXTURE, NOT A BINARY PACKAGE' > "$PKGDEST/mock.pkg.tar.zst"
      if test "$OCR_TEST_MODE" = build-failure; then return 43; fi
      ;;
    *) return 99 ;;
  esac
}
export -f mkdir gpg pacman makepkg-mingw

for mode in success source-failure build-failure; do
  testroot="$root/$mode"
  mkdir -p "$testroot/inputs" "$testroot/logs"
  printf '%s\n' "validpgpkeys=('TEST-PUBLIC-KEY')" \
    'build() {' \
    '  [[ -d "${srcdir}/build-${MSYSTEM}" ]] && rm -rf "${srcdir}/build-${MSYSTEM}"' \
    '  mkdir -p "${srcdir}/build-${MSYSTEM}" && cd "${srcdir}/build-${MSYSTEM}"' \
    '}' > "$testroot/inputs/PKGBUILD"
  for file in source-inputs.json sources.json; do
    printf '{}\n' > "$testroot/logs/$file"
  done
  cp "$script" "$testroot/logs/build_ocr.sh"
  export OCR_TEST_MODE="$mode"
  status=0
  bash "$script" libiconv "$testroot" || status=$?
  test -f "$testroot/gnupg/verification-call.txt"
  test -f "$testroot/logs/environment-after.txt"
  grep -q -- '-std=gnu17' "$testroot/work/PKGBUILD"
  case "$mode" in
    success)
      test "$status" = 0
      test -f "$testroot/artifacts/mock.pkg.tar.zst"
      test -f "$testroot/artifacts/mock.src.tar.gz"
      test -f "$testroot/artifacts/patches/libiconv-gnu17.patch"
      cmp "$testroot/work/PKGBUILD" "$testroot/artifacts/PKGBUILD"
      (cd "$testroot/artifacts" && sha256sum -c SHA256SUMS)
      test -f "$testroot/logs/mock-build.log"
      ;;
    source-failure)
      test "$status" = 42
      test ! -d "$testroot/artifacts"
      test ! -f "$testroot/packages/mock.pkg.tar.zst"
      ;;
    build-failure)
      test "$status" = 43
      test ! -d "$testroot/artifacts"
      test -f "$testroot/packages/mock.src.tar.gz"
      test -f "$testroot/packages/mock.pkg.tar.zst"
      test -f "$testroot/logs/mock-build.log"
      ;;
  esac
  printf 'OCR orchestration check passed: %s\n' "$mode"
done
