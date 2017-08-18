;*********************************************************************
; UpdateMSIVersion
;
; This function will check the version of the installed Windows
; Installer. This is done by checking the version of the
; $SYSDIR\MSI.dll (recommended method by the installer team).
;
; Usage
; Call UpdateMSIVersion
; Pop $MSI_UPDATE ; If $MSI_UPDATE is 1, install latest MSI.
;*********************************************************************


var MSILocalPath
var MSIDeleteAfter
var MSIDLLink
var MSISkipConfirmation

Function UpdateMSIVersion31
  GetDllVersion "$SYSDIR\MSI.dll" $R0 $R1
  IntOp $R2 $R0 / 0x00010000
  IntOp $R3 $R0 & 0x0000FFFF
 
  IntCmp $R2 3 0 InstallMSI RightMSI
  IntCmp $R3 1 RightMSI InstallMSI RightMSI
 
  RightMSI:
    Goto ExitFunction
 
  InstallMSI:
      StrCpy $MSILocalPath ".\WindowsInstaller-KB893803-v2-x86.exe"
      IfFileExists $MSILocalPath confirmlocal
      StrCpy $MSILocalPath "Runtime\WindowsInstaller-KB893803-v2-x86.exe"
      IfFileExists $MSILocalPath confirmlocal
      StrCpy $MSILocalPath "Runtimes\WindowsInstaller-KB893803-v2-x86.exe"
      IfFileExists $MSILocalPath confirmlocal

      ; not local, need to download it
      StrCpy $MSILocalPath "$TEMP\WindowsInstaller-KB893803-v2-x86.exe"
      StrCpy $MSIDLLink "http://download.microsoft.com/download/1/4/7/147ded26-931c-4daf-9095-ec7baf996f46/WindowsInstaller-KB893803-v2-x86.exe"
      goto dodownload

dodownload:
      ; need to download it
      StrCpy $MSIDeleteAfter "Yes"

      ${If} $MSISkipConfirmation != "Yes"
        MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION "${PRODUCTNAME} needs the Windows Installer 3.1 to be installed.$\rPress OK to download and install it from Microsoft's website, or Cancel to try to continue without it." IDCANCEL done
      ${EndIf}
  
      SetDetailsView hide
    
      inetc::get /caption "Downloading Windows Installer 3.1" /canceltext "Cancel" $MSIDLLink "$MSILocalPath" /end
      Pop $1
    	
      ${If} $1 != "OK"
        Delete "$MSILocalPath"
        Abort "Installation cancelled."
      ${EndIf}
    
      SetDetailsView show
      goto havelocal
    
confirmlocal:
      ${If} $MSISkipConfirmation != "Yes"
         MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION "${PRODUCTNAME} needs the Windows Installer 3.1 to be installed.$\rPress OK to install it now, or Cancel to try to continue without it." IDCANCEL done
      ${EndIf}

havelocal:
     ; we have the installer at "MSILocalPath"
      DetailPrint "Installing the Windows 3.1 Installer..."
      ExecWait '$MSILocalPath /passive /norestart'
    
      ${If} $MSIDeleteAfter == "Yes"
        Delete "$MSILocalPath"
      ${EndIf}
    	
      Goto ExitFunction
     
done:
ExitFunction:
 
FunctionEnd
