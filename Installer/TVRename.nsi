!define PRODUCTNAME "TV Rename"

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


SetCompressor /solid lzma

!define ADDREMOVECP_NAME "TV Rename"
!define DEF_INSTALL_DIR "TVRename"
!define STARTMENUFOLDER "TV Rename"
!define REGUNINSTKEY TVRename

Name "${PRODUCTNAME}"

BrandingText /TRIMLEFT " "
!define MUI_ICON "..\TVRename#\App\app.ico"
!define MUI_UNICON "uninstall.ico"

!include "MUI.nsh"
!include "DotNet4Client.nsh"

;Var MUI_TEMP
Var STARTMENU_FOLDER

;Remember the Start Menu Folder
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\${STARTMENUFOLDER}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"

;General
OutFile "TVRename2.3.x.exe"

;Folder selection page
InstallDir "$PROGRAMFILES\${DEF_INSTALL_DIR}"

  CRCCheck On


  !define MUI_HEADERIMAGE
;  !define MUI_HEADERIMAGE_BITMAP "..\InstallerExtras\header.bmp"

; Don't do this from the installer, as it will run as Administrator!
; TODO: Find out how to run as the original user.
;  !define MUI_FINISHPAGE_RUN "$INSTDIR\TVRename.exe"

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_STARTMENU Application $STARTMENU_FOLDER
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_PAGE_FINISH
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES 

  !insertmacro MUI_LANGUAGE "English"
  LangString DESC_SecTVRename ${LANG_ENGLISH} "Install TV Rename software"


!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
!insertmacro MUI_RESERVEFILE_LANGDLL

  InstType "Full Installation"

Section "TVRename" SecTVRename
  SectionIn 1

  Call DotNet4Client_CheckAndInstall

  SetOutPath "$INSTDIR"
  File "..\TVRename#\bin\Release\TVRename.exe"
  File "..\TVRename#\bin\Release\Ionic.Utils.Zip.dll"
  File "..\TVRename#\bin\Release\Newtonsoft.Json.dll"
  File "..\TVRename#\bin\Release\SourceGrid.dll"
  File "..\TVRename#\bin\Release\AlphaFS.dll"
  
  CreateDirectory "$SMPROGRAMS\$STARTMENU_FOLDER"
  CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\TV Rename.lnk" "$INSTDIR\TVRename.exe"
  CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\TV Rename (Recover).lnk" "$INSTDIR\TVRename.exe" /recover
  CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe"

  WriteUninstaller "$INSTDIR\Uninstall.exe"

  ; Create uninstall registry entries for add/remove on control panel
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${REGUNINSTKEY}" "DisplayName" "${ADDREMOVECP_NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${REGUNINSTKEY}" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${REGUNINSTKEY}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${REGUNINSTKEY}" "NoRepair" 1

SectionEnd

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
;--------------------------------------------------------------------------------
;                    UNINSTALLER SECTION

Section "Uninstall"

  Delete "$INSTDIR\TVRename.exe"
  Delete "$INSTDIR\Ionic.Utils.Zip.dll"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\SourceGrid.dll"
  Delete "$INSTDIR\AlphaFS.dll"
  Delete "$INSTDIR\Uninstall.exe"
  RmDir "$INSTDIR"
  Delete "$SMPROGRAMS\$STARTMENU_FOLDER\TV Rename.lnk"
  Delete "$SMPROGRAMS\$STARTMENU_FOLDER\TV Rename (Recover).lnk"
  Delete "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall.lnk"
  RmDir "$SMPROGRAMS\$STARTMENU_FOLDER"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${REGUNINSTKEY}"

  MessageBox MB_YESNO|MB_ICONEXCLAMATION "Do you wish to remove your personal settings, etc., as well?" IDNO nothanks

  ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "AppData"
  RmDir /r "$R0\TVRename"

nothanks:

SectionEnd


!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SECDOTNET} $(DESC_LONGDOTNET)
!insertmacro MUI_FUNCTION_DESCRIPTION_END


;--------------------------------
; .NET Stuff

	!include WordFunc.nsh
	!insertmacro VersionCompare
	!include LogicLib.nsh

	Function .onInit
		Call GetDotNETVersion
		Pop $0
		${If} $0 == "not found"
			Call InstallDotNET
		${Else}		
			StrCpy $0 $0 "" 1 # skip "v"
		
			${VersionCompare} $0 "2.0" $1
			${If} $1 == 2
			      Call InstallDotNET
			${EndIf}
		${EndIf}
	FunctionEnd

	Function GetDotNETVersion
		Push $0
		Push $1
		
		System::Call "mscoree::GetCORVersion(w .r0, i ${NSIS_MAX_STRLEN}, *i) i .r1"
		StrCmp $1 "error" 0 +2
		StrCpy $0 "not found"
		
		Pop $1
		Exch $0
	FunctionEnd

	Function InstallDotNET
		MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION "TV Rename needs the .NET runtime library, version 2.0, to be installed before it can be run.$\rPress OK to go to Microsoft's website and download it, or Cancel to continue installing anyway." IDCANCEL skipdl

		ExecShell Open "http://www.microsoft.com/downloads/details.aspx?familyid=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5&displaylang=en"
		skipdl:
	FunctionEnd
