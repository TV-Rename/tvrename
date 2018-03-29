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
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes the splash screen.
        /// </summary>
        protected override void OnCreateSplashScreen()
        {
            this.SplashScreen = new TVRenameSplash();

            CommandLineArgs clargs = new CommandLineArgs(this.CommandLineArgs);
            if ((clargs.Unattended) || (clargs.Hide)) this.SplashScreen.Visible  = false;
                
        }

        /// <summary>
        /// Configures the splash screen and initializes the main application form
        /// This runs once the splash screen is visible.
        /// </summary>
        protected override void OnCreateMainForm()
        {
            CommandLineArgs clargs = new CommandLineArgs(this.CommandLineArgs);
            if ((clargs.Unattended) || (clargs.Hide))
                this.SplashScreen.SafeInvoke(
                    () => ((TVRenameSplash)this.SplashScreen).Visible = false,true);

            // Update splash screen
            this.SplashScreen.SafeInvoke(
                () => ((TVRenameSplash) this.SplashScreen).UpdateStatus("Initializing"), true);

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

                    logger.Error(ex, $"Error while setting the User-Defined File Path - EXITING: {clargs.UserFilePath}");

                    Environment.Exit(1);
                }
            }

            FileInfo tvdbFile = PathManager.TVDBFile;
            FileInfo settingsFile = PathManager.TVDocSettingsFile;
            TVDoc doc;

            do // Loop until files correctly load
            {
                if (recover) // Recovery required, prompt user
                {
                    RecoverXML recoveryForm = new RecoverXML(recoverText);

                    if (recoveryForm.ShowDialog() == DialogResult.OK)
                    {
                        tvdbFile = recoveryForm.DBFile;
                        settingsFile = recoveryForm.SettingsFile;
                    }
                    else
                    {
                        // TODO: Throw an error
                        return;
                    }
                }

                // Try loading TheTVDB cache file
                TheTVDB.Instance.Setup(tvdbFile, PathManager.TVDBFile, clargs);

                // Try loading settings file
                doc = new TVDoc(settingsFile, clargs);

                if (recover) doc.SetDirty();
                recover = !doc.LoadOK;

                // Continue if correctly loaded
                if (!recover) continue;

                // Set recover message
                recoverText = string.Empty;
                if (!doc.LoadOK && !string.IsNullOrEmpty(doc.LoadErr)) recoverText = doc.LoadErr;
                if (!TheTVDB.Instance.LoadOK && !string.IsNullOrEmpty(TheTVDB.Instance.LoadErr)) recoverText += $"{Environment.NewLine}{TheTVDB.Instance.LoadErr}";
            } while (recover);

            convertSeriesTimeZones(doc, TheTVDB.Instance);

            // Show user interface
            UI ui = new UI(doc, (TVRenameSplash)this.SplashScreen, !clargs.Unattended && !clargs.Hide);

            // Bind IPC actions to the form, this allows another instance to trigger form actions
            RemoteClient.Bind(ui, doc);

            this.MainForm = ui;
        }

        private void convertSeriesTimeZones(TVDoc doc, TheTVDB tvdb)
        {
            //this is just to convert timezones in the TheTVDB into the TVDOC where they should be:
            //itshould only do anything the first time it is run and then be entirely begign
            //can be removed after 1/1/19

            foreach (ShowItem si in doc.ShowItems)
            {
                string newTimeZone = tvdb.GetSeries(si.TVDBCode)?.tempTimeZone;
                if ((si.ShowTimeZone == TimeZone.DefaultTimeZone()) && newTimeZone != TimeZone.DefaultTimeZone() && !string.IsNullOrWhiteSpace(newTimeZone))
                {
                    si.ShowTimeZone = newTimeZone;
                    doc.SetDirty();
                    logger.Info("Copied timezone:{0} onto series {1}", newTimeZone, si.ShowName);
                }
            }

        }
    }
}
