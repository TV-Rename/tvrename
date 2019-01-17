using System;
using System.Reflection;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualBasic.ApplicationServices;
using NLog;
using NLog.Config;
using NLog.Targets.Syslog;
using NLog.Targets.Syslog.Settings;
using TVRename.Ipc;

namespace TVRename.App
{
    /// <summary>
    /// Provides the primary form bootstrap including a splash screen.
    /// </summary>
    /// <seealso cref="WindowsFormsApplicationBase" />
    internal class ApplicationBase : WindowsFormsApplicationBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                    () => ((TVRenameSplash)SplashScreen).Visible = false, true);

            // Update splash screen
            SplashScreen.SafeInvoke(
                () => ((TVRenameSplash)SplashScreen).UpdateStatus("Initializing"), true);

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

            SetupCustomSettings(clargs);

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
                        tvdbFile = recoveryForm.DbFile;
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
                recover = !doc.LoadOk;

                // Continue if correctly loaded
                if (!recover) continue;

                // Set recover message
                recoverText = string.Empty;
                if (!doc.LoadOk && !string.IsNullOrEmpty(doc.LoadErr)) recoverText = doc.LoadErr;
                if (!TheTVDB.Instance.LoadOk && !string.IsNullOrEmpty(TheTVDB.Instance.LoadErr)) recoverText += $"{Environment.NewLine}{TheTVDB.Instance.LoadErr}";
            } while (recover);

            if (TVSettings.Instance.mode == TVSettings.BetaMode.BetaToo || TVSettings.Instance.ShareLogs) SetupLogging();

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
            //it should only do anything the first time it is run and then be entirely benign
            //can be removed after 1/1/19

            foreach (ShowItem si in doc.Library.GetShowItems())
            {
                string newTimeZone = tvdb.GetSeries(si.TvdbCode)?.TempTimeZone;

                if (string.IsNullOrWhiteSpace(newTimeZone)) continue;
                if ( newTimeZone == TimeZoneHelper.DefaultTimeZone() ) continue;
                if (si.ShowTimeZone != TimeZoneHelper.DefaultTimeZone()) continue;

                si.ShowTimeZone = newTimeZone;
                doc.SetDirty();
                Logger.Info("Copied timezone:{0} onto series {1}", newTimeZone, si.ShowName);
            }
        }

        private void SetupLogging()
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(Assembly.Load("NLog.Targets.Syslog"));

            LoggingConfiguration config = LogManager.Configuration;

            SyslogTarget syslog = new SyslogTarget
            {
                MessageCreation = { Facility = Facility.Local7 },
                MessageSend =
                {
                    Protocol = ProtocolType.Tcp,
                    Tcp = {Server = "logs7.papertrailapp.com", Port = 13236, Tls = {Enabled = true}}
                }
            };

            config.AddTarget("syslog", syslog);

            syslog.Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} | " + Helpers.DisplayVersion + " |${level:uppercase=true}| ${message} ${exception:format=toString,Data}";

            LoggingRule rule = new LoggingRule("*", LogLevel.Error, syslog);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;

            Logger.Info($"TV Rename {Helpers.DisplayVersion} logging started with args: {string.Join(" ", CommandLineArgs)}");
        }
    }
}
