// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows.Forms;
using TVRename;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


// Check the mutex that we're not already running, start the main UI, pass in commandline arguments
// If we are running, send arguments via IPC to already running process

public static class GlobalMembersTVRename
{

    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    [STAThread]
    private static int Main(string[] args)
    {
        logger.Info("TV Rename Started with args: {0}",args);
        // Enabling Windows XP visual effects before any controls are created
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        //Update RegVersion to bring the WebBrowser up to speed
        RegistryHelper.UpdateBrowserEmulationVersion();

        // Sort out the command line arguments
        CommandLineArgs clargs = new CommandLineArgs(args);

        // see if we're already running
        bool createdNew = false;
        System.Threading.Mutex mutex = new System.Threading.Mutex(true, "TVRenameMutex", out createdNew);

        if (!createdNew)
        {
            // we're already running
            logger.Warn("TV Rename is alrady running");

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
            bool renameBefore = ipc.RenameBehaviour;

            if (clargs.RenameCheck == false)
             {
              // Temporarily override behaviour for missing folders
              ipc.RenameBehaviour = false;
            }

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

            ipc.RenameBehaviour = renameBefore;
            ipc.MissingBehaviour = before;

        return 0;
        }

#if !DEBUG
		try
		{
#endif

        // Starting TVRename...

        new  TVRenameProgram().Run(args);
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

class TVRenameProgram : WindowsFormsApplicationBase
{
    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    protected override void OnCreateSplashScreen()
    {
        SplashScreen = new TVRenameSplash();
    }

    void updateSplashStatus(String text)
    {
        SplashScreen.Invoke((System.Action)delegate
        {
            ((TVRenameSplash)SplashScreen).UpdateStatus(text);
        });
    }
    protected override void OnCreateMainForm()
    {
        updateSplashStatus("Initalising");

        // Check arguments for forced recover
        bool ok = true;
        string recoverText = "";
        // Sort out the command line arguments
        CommandLineArgs clargs = new CommandLineArgs(CommandLineArgs);


        if (clargs.ForceRecover)
        {
            ok = false; // force recover dialog
            recoverText = "Recover manually requested.";
        }

        // Load settings files
        try
        {
            if (!string.IsNullOrEmpty(clargs.UserFilePath))
                PathManager.SetUserDefinedBasePath(clargs.UserFilePath);
        }
        catch (Exception ex)
        {
            if ((!clargs.Unattended) && (!clargs.Hide)) MessageBox.Show("Error while setting the User-Defined File Path:" + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            logger.Error("Error while setting the User-Defined File Path - EXITING: {0}", clargs.UserFilePath);
            logger.Error(ex);

            Environment.Exit(1);
        }

        FileInfo tvdbFile = PathManager.TVDBFile;
        FileInfo settingsFile = PathManager.TVDocSettingsFile;
        TVDoc doc = null;

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
                    //TODO : throw an error to be caught
                    return;
            }

            // try loading using current settings files, and set up the main
            // classes
            TheTVDB.Instance.setup(tvdbFile, PathManager.TVDBFile, clargs);

            doc = new TVDoc(settingsFile, clargs);

            if (!ok)
                doc.SetDirty();

            ok = doc.LoadOK;

            if (!ok)
            {
                recoverText = "";
                if (!doc.LoadOK && !String.IsNullOrEmpty(doc.LoadErr))
                    recoverText += doc.LoadErr;
                if (!TheTVDB.Instance.LoadOK && !String.IsNullOrEmpty(TheTVDB.Instance.LoadErr))
                    recoverText += "\r\n" + TheTVDB.Instance.LoadErr;
            }
        } while (!ok);


        // Do your time consuming stuff here...
        UI ui = new UI(doc,(TVRenameSplash)SplashScreen);

        // Show user interface
        MainForm = ui;
        
        
    }
}
