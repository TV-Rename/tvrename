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

InstallDir "$PROGRAMFILES\${APPNAME}"

OutFile "TVRename-${TAG}.exe"

!define MUI_ICON "TVRename#\App\app.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\win-uninstall.ico"

Var STARTMENU_FOLDER
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKLM"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\${APPNAME}"
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

Function .onInit ; Remove v2 first
    ReadRegStr $R0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "UninstallString"
    StrCmp $R0 "" done

    MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION "A previous version of ${APPNAME} is installed.$\n$\nUninstall previous version before installing the new version?$\nYour settings will be imported into the new version." IDOK upgrade
    Abort

upgrade:
    ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "AppData"

    CreateDirectory "$R0\${APPNAME}\cache"
    CreateDirectory "$R0\${APPNAME}\config"
    CopyFiles "$R0\TVRename\TVRename\2.1\TheTVDB.xml" "$R0\${APPNAME}\cache"
    CopyFiles "$R0\TVRename\TVRename\2.1\Layout.xml" "$R0\${APPNAME}\config"
    CopyFiles "$R0\TVRename\TVRename\2.1\Statistics.xml" "$R0\${APPNAME}\config"
    CopyFiles "$R0\TVRename\TVRename\2.1\TVRenameSettings.xml" "$R0\${APPNAME}\config"

    ReadRegStr $R0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TVRename" "UninstallString"

    ClearErrors
    ExecWait '"$R0" _?=$INSTDIR' ; Do not copy the uninstaller to a temp file

done:

FunctionEnd

Section "Install"
    SetOutPath "$INSTDIR"

    !insertmacro CheckNetFramework 40Client

    File "TVRename#\bin\Release\TVRename.exe"
    File "TVRename#\bin\Release\TVRename.Core.dll"
    File "TVRename#\bin\Release\NLog.config"
    File "TVRename#\bin\Release\AlphaFS.dll"
    File "TVRename#\bin\Release\Newtonsoft.Json.dll"
    File "TVRename#\bin\Release\NLog.dll"
    File "TVRename#\bin\Release\SourceGrid.dll"

    WriteUninstaller "$INSTDIR\Uninstall.exe"

    !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    CreateDirectory "$SMPROGRAMS\$STARTMENU_FOLDER"
    CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\${APPNAME}.lnk" "$INSTDIR\TVRename.exe"
    CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\${APPNAME} (Recover).lnk" "$INSTDIR\TVRename.exe" /recover
    CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
    !insertmacro MUI_STARTMENU_WRITE_END

    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayIcon" "$INSTDIR\TVRename.exe"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayVersion" "${VERSION}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "Publisher" "${APPNAME}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\Uninstall.exe"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1

    ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
    IntFmt $0 "0x%08X" $0
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "EstimatedSize" "$0"

SectionEnd

Section "Uninstall"
    Delete "$INSTDIR\TVRename.exe"
    Delete "$INSTDIR\TVRename.Core.dll"
    Delete "$INSTDIR\NLog.config"
    Delete "$INSTDIR\AlphaFS.dll"
    Delete "$INSTDIR\Newtonsoft.Json.dll"
    Delete "$INSTDIR\NLog.dll"
    Delete "$INSTDIR\SourceGrid.dll"
    Delete "$INSTDIR\Uninstall.exe"
    RmDir "$INSTDIR"

    !insertmacro MUI_STARTMENU_GETFOLDER Application $STARTMENU_FOLDER
    Delete "$SMPROGRAMS\$STARTMENU_FOLDER\${APPNAME}.lnk"
    Delete "$SMPROGRAMS\$STARTMENU_FOLDER\${APPNAME} (Recover).lnk"
    Delete "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall.lnk"
    RmDir "$SMPROGRAMS\$STARTMENU_FOLDER"

    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

    MessageBox MB_YESNO|MB_ICONEXCLAMATION "Do you wish to remove your ${APPNAME} settings as well?" IDNO done
    ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "AppData"
    RmDir /r "$R0\${APPNAME}"

done:

SectionEnd
