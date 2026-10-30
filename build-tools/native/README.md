# Native Build Candidates

Manual preparation only. These scripts do not build or edit the C# application,
replace its tools, commit changes, or create a GitHub release. No secrets are
required. Workflow permissions are read-only; action revisions are pinned.

## Run

1. Commit `.github/workflows/native-candidates.yml` and `build-tools/native/`.
2. Put the workflow on the repository's default branch (`master`) before using
   the GitHub **Run workflow** button. This can be a separate infrastructure
   commit; merging the entire GUI branch is not required just to run this build.
3. Open **Actions > Build native candidates > Run workflow**.
4. Start with `pdf2docx`. Download its candidate, source inputs and build logs
   from the completed run's **Artifacts** section.
5. Run `ocr-dependencies` separately for the seven historical native packages.
   At most two jobs run concurrently. Each job has a 90-minute timeout.
6. Keep source artifacts with the candidate binaries. Artifacts expire after
   30 days (diagnostic logs after 14); they are not permanent source hosting.

GitHub-hosted runner usage/storage may count toward your account's limits.
Nothing runs on push or pull request. Nothing is automatically published.

## PDF-to-DOCX

- Uses Python 3.14.4, pdf2docx 0.5.13 and PyMuPDF 1.28.0.
- Restores pdf2docx's missing `version.txt` from the verified 0.5.13 source
  metadata and checks the rebuilt wheel's version. Without it the upstream
  setup script incorrectly labels this source as 0.5.6a1.
- Builds MuPDF from the hash-pinned official 1.28.0 source archive plus the two
  recorded public patches (version header and release-test script).
- Applies the included `patches/mupdf-vs-upgrade-libdir.patch` so linking uses
  the same Visual Studio output directory as compilation. The first hosted run
  built the library in `win32-vs-upgrade` but tried to link it from `win32`.
  This build-path-only patch is retained in the source-input artifact.
- Applies `patches/pymupdf-vs-upgrade-packaging.patch` so wheel packaging finds
  both Windows import libraries in that same directory. A preflight executes
  the actual source's path helper in 18 configurations before compilation.
- Builds PyMuPDF against that explicit local MuPDF source tree, never a
  silently downloaded prebuilt candidate wheel.
- Uses pipcl 10 to handle Git metadata absent from source archives. A metadata
  preflight runs before compilation; pipcl 9 failed here after compiling MuPDF.
- Uses UTF-8 for the build process and its Python subprocesses, including
  redirected dependency reports containing non-ASCII package descriptions.
- Keeps the converter wrapper unchanged. Runtime Python versions are pinned
  to the inspected bundle. Build-tool requirements are recorded separately.
- Compares three synthetic documents against the audited official PyMuPDF
  Windows wheel. DOCX contents, including images, must match except timestamps.
- Omits the separate `MSVCP140.dll` copied from the runner. End users need an
  appropriate Microsoft Visual C++ v14 x64 runtime installed from Microsoft.
- The output includes metadata/license material, source inputs, patches,
  generated MuPDF config, build instructions, dependency downloads and logs.

This comparison is not proof of identical behavior for every PDF. Before
replacement, test your real files, invalid inputs, cancellation and timeouts,
and test on clean Windows with the documented runtime prerequisites. No
application binaries are replaced by this workflow, even after passing tests.

## OCR Dependencies

Builds the recorded recipes from MSYS2 commit
`dda6ad369e9f663920436ece2c03e0bfbcddbd25`:

| Package | Version |
| --- | --- |
| libiconv | 1.17-4 |
| libunistring | 1.2-1 |
| libidn2 | 2.3.7-2 |
| gettext | 0.22.5-2 |
| GLib | 2.82.2-1 |
| cairo | 1.18.2-1 |
| Pango | 1.54.0-1 |

Recipe/source checksums are pinned; `makepkg` retains its source and signature
checks. Binary packages include build metadata, and `--allsource` includes
upstream inputs and patches. The compiler/build dependencies come from the
runner's current MINGW64 repository and are recorded with `pacman -Q`.

These are individual package candidates, NOT a reconstructed, tested Tesseract
distribution. They must not be blindly extracted over the existing OCR folder.
Check DLL imports and runtime compatibility together, then compare OCR results,
Polish characters, PDF/image search, cancellation and timeout behavior. New
runtime dependencies would require their own notices/source review.

The temporary public-key keyring uses inherited Windows permissions; the script
does not request POSIX chmod on the runner's NTFS temporary directory. Signature
checks are unchanged. Before compilation, orchestration tests use mock package
builders to check success and failure paths. They are not native build tests.
Source archives are prepared first. On failure, a separate `ocr-*-partial-*`
artifact preserves any packages already created, inputs and diagnostics. Partial
artifacts are not approved binaries; logs and exit status must be reviewed first.

MINGW64 is deprecated upstream but is intentionally kept to match the existing
bundle's C runtime. If a build dependency disappears or an old recipe does not
build with current tools, the job fails. Do not change it to UCRT64 or suppress
checks just to obtain a green result. Diagnose the saved log first.

## Limits

Successful builds establish recorded inputs for NEW candidate binaries; they
do not retroactively establish the provenance of the old DLLs. This is not
legal clearance for the complete release, and MIT does not replace third-party
licenses. Source artifacts cover these jobs, not every tool in the application.

Local checks do not replace a complete successful hosted build. Candidate
packaging and comparisons must pass before the resulting binaries are usable.

References:
- https://docs.github.com/en/actions/how-tos/manage-workflow-runs/manually-run-a-workflow
- https://pymupdf.readthedocs.io/en/latest/installation.html
- https://www.msys2.org/news/#2026-10-24-mingw64-deprecation-update
