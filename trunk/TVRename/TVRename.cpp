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
      Application::Run(gcnew UI(args));
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

#ifdef MONOSTUFF

#pragma warning(disable:4483)

// Also: Go to project properties, Linker, Input, Ignore all default libraries: On
//
//
// TODO: Write code for functions that MoMA points out
//
//
// TODO: Figure out what should be in the bodies of these fuctions, OR figure out
// how to avoid them being called.
//
//

void __clrcall __identifier(".cctor")()
{
}

int __clrcall ___CxxExceptionFilter(void *, void *, int, void *)
{
	return 0;
}

int __clrcall ___CxxRegisterExceptionObject(void *, void *)
{
	return 0;
}

int __clrcall ___CxxDetectRethrow(void *)
{
	return 0;
}

int __clrcall ___CxxQueryExceptionSize(void)
{
	return 0;
}

void __clrcall ___CxxUnregisterExceptionObject(void *,int)
{
}

#endif
