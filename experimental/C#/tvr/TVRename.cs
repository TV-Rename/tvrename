//#define NOMUTEX

using TVRename;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Permissions;
using System;
using System.Windows.Forms;

public static class GlobalMembersTVRename
{


    [STAThread]
	static int Main(string[] args)
	{
		// Enabling Windows XP visual effects before any controls are created
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

	#if ! NOMUTEX
		// see if we're already running
		string mutexName = "TVRenameMutex";

		System.Threading.Mutex mutex = null;

		bool requestInitialOwnership = true;
		bool createdNew = false;
		mutex = new System.Threading.Mutex(requestInitialOwnership, mutexName, out createdNew);
		if (!createdNew)
		{
		  // we're already running
		 return 0;
		}
	#endif

		// Create the main window and run it

	#if ! _DEBUG
		try
		{
	#endif

			Application.Run(new UI(args));

	#if ! NOMUTEX
		  GC.KeepAlive(mutex);
	#endif

	#if ! _DEBUG
		}
		catch (Exception e)
		{
		  ShowException se = new ShowException(e);
		  se.ShowDialog();
		}
	#endif


		return 0;
	}

	#if MONOSTUFF

	//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
	//#pragma warning(disable:4483)

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

	public static void __clrcall @".cctor"()
	{
	}

	public static int __clrcall ___CxxExceptionFilter(IntPtr UnnamedParameter1, IntPtr UnnamedParameter2, int UnnamedParameter3, IntPtr UnnamedParameter4)
	{
		return 0;
	}

	public static int __clrcall ___CxxRegisterExceptionObject(IntPtr UnnamedParameter1, IntPtr UnnamedParameter2)
	{
		return 0;
	}

	public static int __clrcall ___CxxDetectRethrow(IntPtr UnnamedParameter1)
	{
		return 0;
	}

	public static int __clrcall ___CxxQueryExceptionSize()
	{
		return 0;
	}

	public static void __clrcall ___CxxUnregisterExceptionObject(IntPtr UnnamedParameter1, int UnnamedParameter2)
	{
	}

	#endif
}
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
// TVRename.cpp : main project file.


#if ! _DEBUG
#endif
