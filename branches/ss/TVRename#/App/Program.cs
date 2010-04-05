// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
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
        
        // Sort out the command line arguments
        CommandLineArgs clargs = new CommandLineArgs(args);

        // see if we're already running
        bool createdNew = false;
        System.Threading.Mutex mutex = new System.Threading.Mutex(true, "TVRenameMutex", out createdNew);

        if (!createdNew)
        {
            // we're already running

            // tell the already running copy to come to the foreground
            IpcClientChannel clientChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(clientChannel, true);

            RemotingConfiguration.RegisterWellKnownClientType(typeof (IPCMethods), "ipc://TVRenameChannel/IPCMethods");

            IPCMethods ipc = new IPCMethods();

            // if we were already running, and no command line arguments, then bring application to the foreground
            // and we're done.
            if (args.Length == 0)
            {
                ipc.BringToForeground();
                return 0;
            }

            // Send command-line arguments to already running TVRename via IPC

            CommandLineArgs.MissingFolderBehaviour before = ipc.MissingBehaviour;

            if (clargs.MissingFolder != CommandLineArgs.MissingFolderBehaviour.Ask)
             {
              // Temporarily override behaviour for missing folders
              ipc.MissingBehaviour = clargs.MissingFolder;
            }

            // TODO: Unify command line handling between here and in UI.cs (ProcessArgs).  Just send in clargs via IPC?

            if (clargs.Scan || clargs.DoAll) // doall implies scan
                ipc.Scan();

            if (clargs.DoAll)
                ipc.DoAll();

            if (clargs.Quit)
            {
                ipc.Quit();
                return 0;
            }

            ipc.MissingBehaviour = before;

        return 0;
        }

#if !DEBUG
		try
		{
#endif

        // Starting TVRename...

        // Check arguments for forced recover
        bool ok = true;
        string recoverText = "";

        if (clargs.ForceRecover)
        {
            ok = false; // force recover dialog
            recoverText = "Recover manually requested.";
        }

        // Load settings files
        TVDoc doc = null;

        FileInfo tvdbFile = TVDoc.TVDBFile();
        FileInfo settingsFile = TVDoc.TVDocSettingsFile();

        do // loop until no problems loading settings & tvdb cache files
        {
            if (!ok) // something went wrong last time around, ask the user what to do
            {
                RecoverXML rec = new RecoverXML(recoverText);
                if (rec.ShowDialog() == DialogResult.OK)
                {
                    tvdbFile = rec.DBFile;
                    settingsFile = rec.SettingsFile;
                }
                else
                    return 1;
            }

            // try loading using current settings files, and set up the main
            // classes
            TheTVDB tvdb = new TheTVDB(tvdbFile, TVDoc.TVDBFile());
            doc = new TVDoc(settingsFile, tvdb, clargs);

            if (!ok)
                doc.SetDirty();

            ok = doc.LoadOK;

            if (!ok)
            {
                recoverText = "";
                if (!doc.LoadOK && !String.IsNullOrEmpty(doc.LoadErr))
                    recoverText += doc.LoadErr;
                if (!tvdb.LoadOK && !String.IsNullOrEmpty(tvdb.LoadErr))
                    recoverText += "\r\n" + tvdb.LoadErr;
            }
        } while (!ok);

        // Show user interface
        UI theUI = new UI(doc);
        Application.Run(theUI);
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