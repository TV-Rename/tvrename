// TVRename.cpp : main project file.

#include "stdafx.h"
#include "UI.h"

#ifndef _DEBUG
#include "ShowException.h"
#endif

//#define NOMUTEX

using namespace TVRename;
using namespace System::Threading;
using namespace System::Security::AccessControl;
using namespace System::Security::Permissions;


[STAThreadAttribute]
int main(array<System::String ^> ^args)
{
	// Enabling Windows XP visual effects before any controls are created
	Application::EnableVisualStyles();
	Application::SetCompatibleTextRenderingDefault(false); 

#ifndef NOMUTEX
    // see if we're already running
    String ^mutexName = "TVRenameMutex";

    System::Threading::Mutex ^mutex = nullptr;

    bool requestInitialOwnership = true;
    bool createdNew = false;
    mutex = gcnew System::Threading::Mutex(requestInitialOwnership, mutexName, createdNew);
    if (!createdNew)
    {
      // we're already running
     return 0;
    }
#endif

	// Create the main window and run it

#ifndef _DEBUG
	try 
    {
#endif
      Application::Run(gcnew UI());
#ifndef NOMUTEX
	  GC::KeepAlive(mutex);
#endif

#ifndef _DEBUG
	} 
    catch (Exception ^e)
    {
      ShowException ^se = gcnew ShowException(e);
      se->ShowDialog();
    }
#endif


    return 0;
}