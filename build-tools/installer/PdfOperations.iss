#define AppVersion "1.0.0"
#define TermsRevision "vc-runtime-v1"
#if FileExists(AddBackslash(SourcePath) + "payload-dotnet\tools\pdf2docx\_internal\MSVCP140.dll")
  #define BundlesVcRuntime
#endif

[Setup]
AppId=DanBed666.PdfOperations
AppName=PdfOperations
AppVersion={#AppVersion}
AppPublisher=DanBed666
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
DefaultDirName={localappdata}\Programs\PdfOperations
DisableDirPage=no
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\PdfOperations.Gui.exe
OutputDir=output-dotnet
OutputBaseFilename=PdfOperations-{#AppVersion}-win-x64-setup
Compression=lzma2
SolidCompression=yes

[Languages]
#ifdef BundlesVcRuntime
Name: "english"; MessagesFile: "compiler:Default.isl"; LicenseFile: "payload-dotnet\terms\{#TermsRevision}\en.txt"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"; LicenseFile: "payload-dotnet\terms\{#TermsRevision}\pl.txt"
#else
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
#endif

[Files]
Source: "payload-dotnet\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; Flags:unchecked

[Icons]
Name: "{userprograms}\PdfOperations"; Filename: "{app}\PdfOperations.Gui.exe"; WorkingDir: "{app}"
Name: "{userdesktop}\PdfOperations"; Filename: "{app}\PdfOperations.Gui.exe"; WorkingDir: "{app}"; Tasks: desktopicon

#ifdef BundlesVcRuntime
[Registry]
Root: HKCU; Subkey: "Software\DanBed666\PdfOperations\Terms"; ValueType: dword; ValueName: "{#TermsRevision}"; ValueData: "1"; Check: AcceptedInWizard

[Code]
function AcceptedInWizard: Boolean;
begin
  Result := not WizardSilent and WizardForm.LicenseAcceptedRadio.Checked;
end;

function InitializeSetup: Boolean;
var
  Accepted: Cardinal;
begin
  Result := True;
  if WizardSilent then
  begin
    Result := RegQueryDWordValue(HKCU, 'Software\DanBed666\PdfOperations\Terms', '{#TermsRevision}', Accepted) and (Accepted = 1);
    if not Result then
      Log('Interactive installation required to accept the Microsoft Visual C++ library terms.');
  end;
end;
#endif
