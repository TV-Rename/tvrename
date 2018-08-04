!include "MUI.nsh"
!include "FileFunc.nsh"
!include "DotNetChecker.nsh" ; https://github.com/ReVolly/NsisDotNetChecker

!define APPNAME "TV Rename"

CRCCheck On
Unicode true
SetCompressor /solid lzma

Name "${APPNAME}"
Caption "${APPNAME} ${VERSION} Setup"
BrandingText " "

InstallDir "$PROGRAMFILES\TVRename"

OutFile "TVRename-${TAG}.exe"

!define MUI_ICON "TVRename\App\app.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\win-uninstall.ico"

Var STARTMENU_FOLDER
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKLM"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\TVRename"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"

!define MUI_ABORTWARNING

!define MUI_FINISHPAGE_RUN "$INSTDIR\TVRename.exe"
!define MUI_FINISHPAGE_LINK "Visit the TV Rename site for the latest news and support"
!define MUI_FINISHPAGE_LINK_LOCATION "http://www.tvrename.com/"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_STARTMENU Application $STARTMENU_FOLDER
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Section "Install"
    SetOutPath "$INSTDIR"

    !insertmacro CheckNetFramework 462

    Delete "$INSTDIR\Ionic.Utils.Zip.dll" ; Remove old dependency

    File "TVRename\bin\Release\TVRename.exe"
    File "TVRename\bin\Release\NLog.config"
    File "TVRename\bin\Release\AlphaFS.dll"
    File "TVRename\bin\Release\Newtonsoft.Json.dll"
    File "TVRename\bin\Release\NLog.dll"
    File "TVRename\bin\Release\SourceGrid.dll"
    File "TVRename\bin\Release\Humanizer.dll"
    File "TVRename\bin\Release\Microsoft.Toolkit.Win32.UI.Controls.dll"
    File "TVRename\bin\Release\Microsoft.WindowsAPICodePack.dll"
    File "TVRename\bin\Release\Microsoft.WindowsAPICodePack.Shell.dll"
    File "TVRename\bin\Release\Microsoft.WindowsAPICodePack.ShellExtensions.dll"

    WriteUninstaller "$INSTDIR\Uninstall.exe"

    !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    CreateDirectory "$SMPROGRAMS\$STARTMENU_FOLDER"
    CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\${APPNAME}.lnk" "$INSTDIR\TVRename.exe"
    CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\${APPNAME} (Recover).lnk" "$INSTDIR\TVRename.exe" /recover
    CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
    !insertmacro MUI_STARTMENU_WRITE_END

    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "DisplayName" "${APPNAME}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "DisplayIcon" "$INSTDIR\TVRename.exe"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "DisplayVersion" "${VERSION}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "Publisher" "${APPNAME}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "UninstallString" "$INSTDIR\Uninstall.exe"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "NoRepair" 1
    
    ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
    IntFmt $0 "0x%08X" $0
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "EstimatedSize" "$0"

SectionEnd

Section "Uninstall"
    Delete "$INSTDIR\TVRename.exe"
    Delete "$INSTDIR\NLog.config"
    Delete "$INSTDIR\AlphaFS.dll"
    Delete "$INSTDIR\Newtonsoft.Json.dll"
    Delete "$INSTDIR\NLog.dll"
    Delete "$INSTDIR\SourceGrid.dll"
    Delete "$INSTDIR\Humanizer.dll"
    Delete "$INSTDIR\Microsoft.Toolkit.Win32.UI.Controls.dll"
    Delete "$INSTDIR\Microsoft.WindowsAPICodePack.dll"
    Delete "$INSTDIR\Microsoft.WindowsAPICodePack.Shell.dll"
    Delete "$INSTDIR\Microsoft.WindowsAPICodePack.ShellExtensions.dll"
    Delete "$INSTDIR\Uninstall.exe"
    RmDir "$INSTDIR"

    !insertmacro MUI_STARTMENU_GETFOLDER Application $STARTMENU_FOLDER
    Delete "$SMPROGRAMS\$STARTMENU_FOLDER\${APPNAME}.lnk"
    Delete "$SMPROGRAMS\$STARTMENU_FOLDER\${APPNAME} (Recover).lnk"
    Delete "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall.lnk"
    RmDir "$SMPROGRAMS\$STARTMENU_FOLDER"

    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename"

    MessageBox MB_YESNO|MB_ICONEXCLAMATION "Do you wish to remove your ${APPNAME} settings as well?" IDNO done
    ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "AppData"
    RmDir /r "$R0\TVRename"

done:

SectionEnd
