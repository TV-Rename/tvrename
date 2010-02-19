//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

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
