using System;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualBasic.ApplicationServices;
using TVRename.Ipc;

namespace TVRename.App
{
    /// <summary>
    /// Provides the primary form bootstrap including a splash screen.
    /// </summary>
    /// <seealso cref="WindowsFormsApplicationBase" />
    internal class ApplicationBase : WindowsFormsApplicationBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes the splash screen.
        /// </summary>
        protected override void OnCreateSplashScreen()
        {
            SplashScreen = new TVRenameSplash();

            CommandLineArgs clargs = new CommandLineArgs(CommandLineArgs);
            if (clargs.Hide) SplashScreen.Visible  = false;
                
        }

        /// <summary>
        /// Configures the splash screen and initializes the main application form
        /// This runs once the splash screen is visible.
        /// </summary>
        protected override void OnCreateMainForm()
        {
            CommandLineArgs clargs = new CommandLineArgs(CommandLineArgs);
            if (clargs.Hide)
                SplashScreen.SafeInvoke(
                    () => ((TVRenameSplash)SplashScreen).Visible = false,true);

            // Update splash screen
            SplashScreen.SafeInvoke(
                () => ((TVRenameSplash) SplashScreen).UpdateStatus("Initializing"), true);

            // Update RegVersion to bring the WebBrowser up to speed
            RegistryHelper.UpdateBrowserEmulationVersion();

            bool recover = false;
            string recoverText = string.Empty;

            // Check arguments for forced recover
            if (clargs.ForceRecover)
            {
                recover = true;
                recoverText = "Recover manually requested.";
            }

            PathManager.ShowCollection = "";
            SetupCustomSettings(clargs);

            TVDoc doc;

            do // Loop until files correctly load
            {
                // Try loading settings file
                doc = new TVDoc(clargs);

                FileInfo tvdbFile  = PathManager.TVDBFile;
                FileInfo showsFile = PathManager.TVDocShowsFile;

                if (recover) doc.SetDirty();
                recover = !doc.LoadOk;

                // Continue if correctly loaded
                if (!recover) continue;

                // Set recover message
                recoverText = string.Empty;
                if (!doc.LoadOk && !string.IsNullOrEmpty(doc.LoadErr)) recoverText = doc.LoadErr;
                if (!TheTVDB.Instance.LoadOk && !string.IsNullOrEmpty(TheTVDB.Instance.LoadErr)) recoverText += $"{Environment.NewLine}{TheTVDB.Instance.LoadErr}";
                if (recover) // Recovery required, prompt user
                {
                    RecoverXML recoveryForm = new RecoverXML(recoverText);

                    if (recoveryForm.ShowDialog() == DialogResult.OK)
                    {
                        tvdbFile = recoveryForm.DbFile;
                        showsFile = recoveryForm.SettingsFile;
                    }
                    else
                    {
                        // TODO: Throw an error
                        return;
                    }
                }
            } while (recover);

            ConvertSeriesTimeZones(doc, TheTVDB.Instance);

            // Show user interface
            UI ui = new UI(doc, (TVRenameSplash)SplashScreen, !clargs.Unattended && !clargs.Hide);
            ui.Text = ui.Text + " " + Helpers.DisplayVersion;

            // Bind IPC actions to the form, this allows another instance to trigger form actions
            RemoteClient.Bind(ui, doc);

            MainForm = ui;
        }

        private static void SetupCustomSettings(CommandLineArgs clargs)
        {
            // Check arguments for custom settings path
            if (!string.IsNullOrEmpty(clargs.UserFilePath))
            {
                try
                {
                    PathManager.SetUserDefinedBasePath(clargs.UserFilePath);
                }
                catch (Exception ex)
                {
                    if (!clargs.Unattended && !clargs.Hide) MessageBox.Show($"Error while setting the User-Defined File Path:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Logger.Error(ex, $"Error while setting the User-Defined File Path - EXITING: {clargs.UserFilePath}");

                    Environment.Exit(1);
                }
            }
        }

        private static void ConvertSeriesTimeZones(TVDoc doc, TheTVDB tvdb)
        {
            //this is just to convert timezones in the TheTVDB into the TVDOC where they should be:
            //itshould only do anything the first time it is run and then be entirely begign
            //can be removed after 1/1/19

            foreach (ShowItem si in doc.Library.GetShowItems())
            {
                string newTimeZone = tvdb.GetSeries(si.TvdbCode)?.TempTimeZone;

                if (string.IsNullOrWhiteSpace(newTimeZone)) continue;
                if ( newTimeZone == TimeZone.DefaultTimeZone() ) continue;
                if (si.ShowTimeZone != TimeZone.DefaultTimeZone()) continue;

                si.ShowTimeZone = newTimeZone;
                doc.SetDirty();
                Logger.Info("Copied timezone:{0} onto series {1}", newTimeZone, si.ShowName);
            }

        }
    }
}
