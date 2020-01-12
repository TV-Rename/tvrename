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

!define MUI_TEXT_LICENSE_TITLE "End-User License Agreement"
!define MUI_TEXT_LICENSE_SUBTITLE "Please read the following license agreement carefully"
!define MUI_LICENSEPAGE_TEXT_TOP "Please read the following license agreement carefully"
!define MUI_LICENSEPAGE_BUTTON Accept
!define MUI_LICENSEPAGE_CHECKBOX

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "Licence.rtf"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_STARTMENU Application $STARTMENU_FOLDER
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Section "Install"
    SetOutPath "$INSTDIR"

    !insertmacro CheckNetFramework 472

    Delete "$INSTDIR\Ionic.Utils.Zip.dll" ; Remove old dependency

    File "TVRename\bin\Release\TVRename.exe"
    File "TVRename\bin\Release\AlphaFS.dll"
    File "TVRename\bin\Release\CloudFlareUtilities.dll"
    File "TVRename\bin\Release\Humanizer.dll"
    File "TVRename\bin\Release\Ical.Net.dll"
    File "TVRename\bin\Release\JetBrains.Annotations.dll"
    File "TVRename\bin\Release\MediaInfo.Wrapper.dll"
    File "TVRename\bin\Release\Microsoft.Win32.Primitives.dll"
    File "TVRename\bin\Release\Microsoft.WindowsAPICodePack.dll"
    File "TVRename\bin\Release\Microsoft.WindowsAPICodePack.Shell.dll"
    File "TVRename\bin\Release\Microsoft.WindowsAPICodePack.ShellExtensions.dll"
    File "TVRename\bin\Release\netstandard.dll"
    File "TVRename\bin\Release\Newtonsoft.Json.dll"
    File "TVRename\bin\Release\NLog.dll"
    File "TVRename\bin\Release\NLog.Targets.Syslog.dll"
    File "TVRename\bin\Release\NLog.Windows.Forms.dll"
    File "TVRename\bin\Release\NodaTime.dll"
    File "TVRename\bin\Release\SourceGrid.dll"
    File "TVRename\bin\Release\System.ValueTuple.dll"
    File "TVRename\bin\Release\System.Net.Http.dll"
    File "TVRename\bin\Release\System.Threading.Tasks.dll"
    File "TVRename\bin\Release\TimeZoneConverter.dll"
    File "TVRename\bin\Release\System.Runtime.InteropServices.RuntimeInformation.dll"

    File "TVRename\bin\Release\NLog.config"
    File "TVRename\bin\Release\TVRename.exe.config"

    File "TVRename\bin\Release\x64\libcrypto-1_1-x64.dll"
    File "TVRename\bin\Release\x64\libcurl.dll"
    File "TVRename\bin\Release\x64\libssl-1_1-x64.dll"
    File "TVRename\bin\Release\x64\MediaInfo.dll"
    
    File "TVRename\bin\Release\x86\libcrypto-1_1.dll"
    File "TVRename\bin\Release\x86\libcurl.dll"
    File "TVRename\bin\Release\x86\libssl-1_1.dll"
    File "TVRename\bin\Release\x86\MediaInfo.dll"
    
    
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
    Delete "$INSTDIR\NLog.Windows.Forms.dll"
    Delete "$INSTDIR\NLog.Targets.Syslog.dll"
    Delete "$INSTDIR\SourceGrid.dll"
    Delete "$INSTDIR\Humanizer.dll"
    Delete "$INSTDIR\Ical.Net.dll"
    Delete "$INSTDIR\NodaTime.dll"
    Delete "$INSTDIR\TimeZoneConverter.dll"
    Delete "$INSTDIR\Microsoft.Toolkit.Win32.UI.Controls.dll"
    Delete "$INSTDIR\Microsoft.WindowsAPICodePack.dll"
    Delete "$INSTDIR\Microsoft.WindowsAPICodePack.Shell.dll"
    Delete "$INSTDIR\Microsoft.WindowsAPICodePack.ShellExtensions.dll"
    Delete "$INSTDIR\Uninstall.exe"
    Delete "$INSTDIR\MediaInfo.Wrapper.dll"
    Delete "$INSTDIR\x64\MediaInfo.dll"
    Delete "$INSTDIR\x86\MediaInfo.dll"
    Delete "$INSTDIR\CloudFlareUtilities.dll"
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
