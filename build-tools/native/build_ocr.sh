#!/usr/bin/env bash
set -euo pipefail
package=$1
root=$2
case "$package" in libiconv|libunistring|libidn2|gettext|glib2|cairo|pango) ;; *) exit 2 ;; esac
test "${MSYSTEM:-}" = MINGW64
test -f "$root/inputs/PKGBUILD"
mkdir -p "$root/logs" "$root/work" "$root/packages"
exec > >(tee "$root/logs/build.log") 2>&1
collect_diagnostics() {
  status=$?
  trap - EXIT
  set +e
  pacman -Q > "$root/logs/environment-after.txt"
  for logfile in "$root/work/"*.log; do
    if test -f "$logfile"; then cp "$logfile" "$root/logs/"; fi
  done
  exit "$status"
}
trap collect_diagnostics EXIT
cp -a "$root/inputs/." "$root/work/"
cd "$root/work"
export MINGW_ARCH=mingw64
export PKGDEST="$root/packages"
export SRCDEST="$root/inputs"
export SRCPKGDEST="$root/packages"
export GNUPGHOME="$root/gnupg"
# This disposable keyring contains public verification keys, never private keys.
# Do not request POSIX chmod on the Windows runner's NTFS temporary directory.
mkdir -p -- "$GNUPGHOME"
# The recipe is downloaded from a pinned commit and SHA-256 checked first.
source ./PKGBUILD
if declare -p validpgpkeys &>/dev/null; then
  gpg --batch --keyserver hkps://keyserver.ubuntu.com --recv-keys "${validpgpkeys[@]}"
fi
pacman -Q > "$root/logs/environment-before.txt"
printf '%s\n' 'Preparing source archive (checksums and signatures remain enabled)...'
makepkg-mingw --allsource --noconfirm
printf '%s\n' 'Building binary packages...'
makepkg-mingw --syncdeps --noconfirm --cleanbuild --log
shopt -s nullglob
binaries=("$root/packages/"*.pkg.tar.*)
sources=("$root/packages/"*.src.tar.*)
test "${#binaries[@]}" -gt 0
test "${#sources[@]}" -gt 0
mkdir -p "$root/artifacts"
cp "${binaries[@]}" "${sources[@]}" "$root/artifacts/"
cp "$root/logs/source-inputs.json" "$root/logs/build_ocr.sh" "$root/logs/sources.json" "$root/artifacts/"
pacman -Q > "$root/artifacts/build-environment.txt"
printf '%s\n' 'CANDIDATE ONLY: not a tested drop-in replacement for the OCR bundle.' \
  'Built from recorded sources with the current MINGW64 build environment.' \
  'Do not replace original DLLs before checking imports, runtime dependencies and OCR regressions.' \
  'Build environment packages are not automatically licensed or sourced by this archive.' > "$root/artifacts/READ-ME-FIRST.txt"
(cd "$root/artifacts" && sha256sum -- *.pkg.tar.* *.src.tar.* > SHA256SUMS)
