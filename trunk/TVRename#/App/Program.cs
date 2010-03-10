//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

// Check the mutex that we're not already running, start the main UI, pass in commandline arguments

// TODO: mutex stops you running a (future) command-line version if main UI version is already running

using System;
using System.Windows.Forms;
using TVRename;

public static class GlobalMembersTVRename
{
    [STAThread]
    static int Main(string[] args)
    {
        // Enabling Windows XP visual effects before any controls are created
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

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
