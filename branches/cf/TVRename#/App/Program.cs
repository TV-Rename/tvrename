// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Threading;
using System.Windows.Forms;

using TVRename;

// Check the mutex that we're not already running, start the main UI, pass in commandline arguments

// TODO: mutex stops you running a (future) command-line version if main UI version is already running

public static class GlobalMembersTVRename
{
    [STAThread]
    private static int Main(string[] args)
    {
        // Enabling Windows XP visual effects before any controls are created
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // see if we're already running
        const string mutexName = "TVRenameMutex";

        const bool requestInitialOwnership = true;
        bool createdNew;

        Mutex mutex = new Mutex(requestInitialOwnership, mutexName, out createdNew);

        if (!createdNew)
        {
            // we're already running
            return 0;
        }

        // Create the main window and run it

#if !DEBUG
		try
		{
#endif

        Application.Run(new UI(args));

        GC.KeepAlive(mutex);

#if !DEBUG
		}
		catch (Exception e)
		{
		  ShowException se = new ShowException(e);
		  se.ShowDialog();
		}
#endif

        return 0;
    }
}