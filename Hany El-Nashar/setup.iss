; -- WebViewApplication.iss --
#pragma include __INCLUDE__ + ";" + ReadReg(HKLM, "Software\Mitrich Software\Inno Download Plugin", "InstallDir")
#include <idp.iss>

#define MyAppName "WebViewApplication"
#define MyAppVersion "1.0.0.0"
#define MyAppPublisher "Original"
#define MyAppExeName "WebViewApplication.exe"

; IMPORTANT: Update these paths to match your actual project structure
#define BuildOutputPath "bin\Release\net8.0-windows\"  ; Relative to script location
#define SolutionDir "..\"  ; Relative to script location if script is in project folder
#define AppIcon "app.ico"  ; Icon file name (place in same folder as script)

[Setup]
AppId={{DC04F8EB-95B9-4E8A-80F1-2FF97714AAAF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir={#SourcePath}\Output
OutputBaseFilename=WebViewApplication_Setup
SetupIconFile={#SourcePath}\{#AppIcon}
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; Main application files
Source: "{#SolutionDir}{#BuildOutputPath}{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SolutionDir}{#BuildOutputPath}*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall

[Code]
function InitializeSetup(): Boolean;
begin
  // Add prerequisite downloads
  idpAddFile('https://go.microsoft.com/fwlink/?LinkId=2085155', ExpandConstant('{tmp}\dotnet48.exe'));
  idpAddFile('https://go.microsoft.com/fwlink/p/?LinkId=2124703', ExpandConstant('{tmp}\webview2.exe'));
  idpDownloadAfter(wpReady);
  Result := True;
end;

function IsDotNetInstalled(): Boolean;
var
  success: Boolean;
  release: Cardinal;
begin
  success := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', release);
  Result := success and (release >= 528040); // .NET 4.8
end;

function IsWebView2Installed(): Boolean;
begin
  Result := RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}');
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  // Install .NET 4.8 if needed
  if not IsDotNetInstalled() then
  begin
    if not Exec(ExpandConstant('{tmp}\dotnet48.exe'), '/q /norestart', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      Result := 'Failed to install .NET Framework 4.8';
      Exit;
    end;
  end;
  
  // Install WebView2 if needed
  if not IsWebView2Installed() then
  begin
    if not Exec(ExpandConstant('{tmp}\webview2.exe'), '/silent /install', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      Result := 'Failed to install WebView2 Runtime';
      Exit;
    end;
  end;
end;