using System;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualBasic.ApplicationServices;
using TVRename.Forms;
using TVRename.Ipc;

namespace TVRename.App
{
    /// <summary>
    /// Provides the primary form bootstrap including a splash screen.
    /// </summary>
    /// <seealso cref="WindowsFormsApplicationBase" />
    /// <inheritdoc />
    internal class ApplicationBase : WindowsFormsApplicationBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes the splash screen.
        /// </summary>
        /// <inheritdoc />
        protected override void OnCreateSplashScreen()
        {
            this.SplashScreen = new SplashScreen();
        }

        /// <summary>
        /// Configures the splash screen and initializes the main application form
        /// This runs once the splash screen is visible.
        /// </summary>
        /// <inheritdoc />
        protected override void OnCreateMainForm()
        {
            // Update splash screen
            this.SplashScreen.Invoke(new MethodInvoker(() => ((SplashScreen)this.SplashScreen).Status = "Initializing"));

            // Update RegVersion to bring the WebBrowser up to speed
            RegistryHelper.UpdateBrowserEmulationVersion();

            CommandLineArgs clargs = new CommandLineArgs(this.CommandLineArgs);

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

                    Logger.Error(ex, $"Error while setting the User-Defined File Path - EXITING: {clargs.UserFilePath}");

                    Environment.Exit(1);
                }
            }

            FileInfo tvdbFile = PathManager.TVDBFile;
            FileInfo settingsFile = PathManager.SettingsFile;
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
                TheTVDB.Instance.setup(tvdbFile, PathManager.TVDBFile, clargs);

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

            // Show user interface
            UI ui = new UI(doc, (SplashScreen)this.SplashScreen);

            // Bind IPC actions to the form, this allows another instance to trigger form actions
            RemoteClient.Bind(ui, doc);

            this.MainForm = ui;
        }
    }
}
