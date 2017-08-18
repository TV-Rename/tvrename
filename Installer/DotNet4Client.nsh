; Uses inetc plugin from http://nsis.sourceforge.net/Inetc_plug-in
; Uses registry plugin from http://nsis.sourceforge.net/Registry_plug-in

!include WordFunc.nsh
!include LogicLib.nsh
!include "MSI3.1.nsh"

var LocalPath
var DeleteAfter
var dllink
var val
var execResult

Function DotNet4Client_CheckAndInstall
  ; Following: http://msdn.microsoft.com/en-us/kb/kbarticle.aspx?id=318785

  ReadRegStr $val HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" "Install"

  ${If} $val == "1"
     DetailPrint ".NET 4.0 is installed"
     goto done     ; is installed
  ${EndIf}

  DetailPrint ".NET 4.0 needs to be installed"
  StrCpy $DeleteAfter "No"

  StrCpy $LocalPath ".\dotNetFx40_Full_x86_x64.exe"
  IfFileExists $LocalPath confirmlocal
  StrCpy $LocalPath "Runtime\dotNetFx40_Full_x86_x64.exe"
  IfFileExists $LocalPath confirmlocal
  StrCpy $LocalPath "Runtimes\dotNetFx40_Full_x86_x64.exe"
  IfFileExists $LocalPath confirmlocal

  StrCpy $LocalPath ".\dotnetfx35setup.exe"
  IfFileExists $LocalPath confirmlocal
  StrCpy $LocalPath "Runtime\dotnetfx35setup.exe"
  IfFileExists $LocalPath confirmlocal
  StrCpy $LocalPath "Runtimes\dotnetfx35setup.exe"
  IfFileExists $LocalPath confirmlocal

  ; not local, need to download it

  StrCpy $LocalPath "$TEMP\dotNetFx40_Full_x86_x64.exe"
  StrCpy $dllink "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe" ; Full installer (~48MB) 

  goto dodownload

dodownload:
  ; need to download it
  StrCpy $DeleteAfter "Yes"

  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION "${PRODUCTNAME} needs the .NET runtime library, version 4.0 Client, to be installed before it can be run.$\r$\rPress OK to download and install it from Microsoft's website, or Cancel to continue without it." IDCANCEL done

  SetDetailsView hide

  inetc::get /caption "Downloading .NET Framework 4.0 Client" /canceltext "Cancel" $dllink "$LocalPath" /end
  Pop $1
	
  ${If} $1 != "OK"
    Delete "$LocalPath"
    Abort "Installation cancelled."
  ${EndIf}

  SetDetailsView show
  goto havelocal

confirmlocal:
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION "${PRODUCTNAME} needs the .NET runtime library, 4.0 Client to be installed before it can be run.$\r$\rPress OK to install it now (it will take a few minutes), or Cancel to continue without it." IDCANCEL done

havelocal:
 ; we have the dot net installer at "LocalPath"

  ; need to check/update MSI installer before continuing to install .NET 4.0
  StrCpy $MSISkipConfirmation "Yes"
  Call UpdateMSIVersion31

  DetailPrint "Installing .NET Framework 4.0 Client.  This may take several minutes..."
  ExecWait '$LocalPath /qb /norestart' $execResult

  ${If} $DeleteAfter == "Yes"
    Delete "$LocalPath"
  ${EndIf}

  ${If} $execResult == 3010
    ; need reboot
    SetRebootFlag true
  ${EndIf}	
done:

FunctionEnd