using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using Microsoft.VisualBasic.ApplicationServices;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets.Syslog;
using NLog.Targets.Syslog.Settings;
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows.Forms;
using TVRename.Ipc;
using System.Net;

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

            CommandLineArgs clargs = new(CommandLineArgs);
            if (clargs.Hide || !Environment.UserInteractive)
            {
                SplashScreen.Visible = false;
            }
        }

        /// <summary>
        /// Configures the splash screen and initializes the main application form
        /// This runs once the splash screen is visible.
        /// </summary>
        protected override void OnCreateMainForm()
        {
            CommandLineArgs parameters = new(CommandLineArgs);
            if (parameters.Hide || !Environment.UserInteractive)
            {
                SplashScreen.SafeInvoke(
                    () => ((TVRenameSplash)SplashScreen).Visible = false, true);
            }

            // Update splash screen
            SplashScreen.SafeInvoke(
                () => ((TVRenameSplash)SplashScreen).UpdateStatus("Initializing"), true);

            // Update RegVersion to bring the WebBrowser up to speed
            RegistryHelper.UpdateBrowserEmulationVersion();

            TVDoc doc = LoadSettings(parameters);

            if (TVSettings.Instance.mode == TVSettings.BetaMode.BetaToo || TVSettings.Instance.ShareLogs)
            {
                SetupLogging();
            }

            // Show user interface
            UI ui = new(doc, (TVRenameSplash)SplashScreen, !parameters.Unattended && !parameters.Hide && Environment.UserInteractive);
            ui.Text = ui.Text + " " + Helpers.DisplayVersion;

            // Bind IPC actions to the form, this allows another instance to trigger form actions
            try
            {
                RemoteClient.Bind(ui);
            }
            catch (RemotingException ex)
            {
                Logger.Warn(
                    $"Could not create IPC Port: {ex.Message} : TV Rename will not be able to accept incoming commands");
            }

            MainForm = ui;
        }

        [NotNull]
        private static TVDoc LoadSettings([NotNull] CommandLineArgs clargs)
        {
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
            FileInfo tvmazeFile = PathManager.TVmazeFile;
            FileInfo tmdbFile = PathManager.TmdbFile;
            FileInfo settingsFile = PathManager.TVDocSettingsFile;
            TVDoc doc;

            do // Loop until files correctly load
            {
                if (recover) // Recovery required, prompt user
                {
                    RecoverXml recoveryForm = new(recoverText);

                    if (recoveryForm.ShowDialog() == DialogResult.OK)
                    {
                        tvdbFile = recoveryForm.TvDbFile;
                        tvmazeFile = recoveryForm.TvMazeFile;
                        tmdbFile = recoveryForm.TmdbFile;
                        settingsFile = recoveryForm.SettingsFile;
                    }
                    else
                    {
                        Logger.Error("User requested no recovery");
                        throw new TVRenameOperationInterruptedException();
                    }
                }

                // Try loading settings file
                doc = new TVDoc(settingsFile, clargs);

                // Try loading TheTVDB cache file
                bool showIssues = !clargs.Unattended && !clargs.Hide;
                TheTVDB.LocalCache.Instance.Setup(tvdbFile, PathManager.TVDBFile, showIssues);
                TVmaze.LocalCache.Instance.Setup(tvmazeFile, PathManager.TVmazeFile, showIssues);
                TMDB.LocalCache.Instance.Setup(tmdbFile, PathManager.TmdbFile, showIssues);

                if (recover)
                {
                    doc.SetDirty();
                }

                recover = !doc.LoadOk || !(TheTVDB.LocalCache.Instance.LoadOk && TMDB.LocalCache.Instance.LoadOk && TVmaze.LocalCache.Instance.LoadOk);

                // Continue if correctly loaded
                if (!recover)
                {
                    continue;
                }

                // Set recover message
                recoverText = string.Empty;
                if (!doc.LoadOk && !string.IsNullOrEmpty(doc.LoadErr))
                {
                    recoverText = doc.LoadErr;
                }

                bool oneOfTheCacheFailedToLoad =
                    !TheTVDB.LocalCache.Instance.LoadOk || !TVmaze.LocalCache.Instance.LoadOk;

                if (oneOfTheCacheFailedToLoad && !string.IsNullOrEmpty(CachePersistor.LoadErr))
                {
                    recoverText += $"{Environment.NewLine}{CachePersistor.LoadErr}";
                }
            } while (recover);

            return doc;
        }

        private static void SetupCustomSettings([NotNull] CommandLineArgs clargs)
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
                    if (!clargs.Unattended && !clargs.Hide && Environment.UserInteractive)
                    {
                        MessageBox.Show($"Error while setting the User-Defined File Path:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Logger.Error(ex, $"Error while setting the User-Defined File Path - EXITING: {clargs.UserFilePath}");

                    Environment.Exit(1);
                }
            }
        }

        private void SetupLogging()
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(Assembly.Load("NLog.Targets.Syslog"));
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(Assembly.Load("Timber.io.NLog"));

            SetupPapertrailLogging();
            SetupSemaTextLogging();

            Logger.Fatal($"TV Rename {Helpers.DisplayVersion} logging started on {Environment.OSVersion}, {(Environment.Is64BitOperatingSystem ? "64 Bit OS" : "")}, {(Environment.Is64BitProcess ? "64 Bit Process" : "")} {Environment.Version} {(Environment.UserInteractive ? "Interactive" : "")} with args: {string.Join(" ", CommandLineArgs)}");
            Logger.Info($"Copyright (C) {DateTime.Now.Year} TV Rename");
            Logger.Info("This program comes with ABSOLUTELY NO WARRANTY; This is free software, and you are welcome to redistribute it under certain conditions");
        }

        private static void SetupSemaTextLogging()
        {
            try
            {
                LoggingConfiguration config = LogManager.Configuration;
                SyslogTarget sematext = new()
                {
                    MessageCreation =
                    {
                        Rfc5424 =
                        {
                            AppName = "0dcb3012-fa85-47c5-b6ca-cfd33609ac33"
                        }
                    },
                    MessageSend =
                    {
                        Protocol = ProtocolType.Tcp,
                        Tcp = {Server = "logsene-syslog-receiver.sematext.com", Port = 514}
                    }
                };

                config.AddTarget("sema", sematext);
                JsonLayout jsonLayout = new()
                {
                    Attributes =
                    {
                        new JsonAttribute("exceptionType", "${exception:format=Type}"),
                        new JsonAttribute("exceptionDetails", "${exception:format=toString,Data}"),
                        new JsonAttribute("details", "${message}"),
                        new JsonAttribute("exceptionMessage", "${exception:format=Message}"),
                        new JsonAttribute("level", "${level:uppercase=true}"),
                        new JsonAttribute("appVersion",Helpers.DisplayVersion),
                        new JsonAttribute("innerException", new JsonLayout
                            {
                                Attributes =
                                {
                                    new JsonAttribute("type", "${exception:format=:innerFormat=Type:MaxInnerExceptionLevel=1:InnerExceptionSeparator=}"),
                                    new JsonAttribute("innerMessage", "${exception:format=:innerFormat=Message:MaxInnerExceptionLevel=1:InnerExceptionSeparator=}")
                                }
                            },
                            //don't escape layout
                            false)
                    }
                };
                sematext.Layout = jsonLayout;

                LoggingRule semaRule = new("*", LogLevel.Warn, sematext);
                config.LoggingRules.Add(semaRule);
                LogManager.Configuration = config;
            }
            catch
            {
                Logger.Error("Failed to setup logging with sema");
            }
        }

        private static void SetupPapertrailLogging()
        {
            try
            {
                LoggingConfiguration config = LogManager.Configuration;
                SyslogTarget papertrail = new()
                {
                    MessageCreation = { Facility = Facility.Local7 },
                    MessageSend =
                    {
                        Protocol = ProtocolType.Tcp,
                        Tcp = {Server = "logs7.papertrailapp.com", Port = 13236, Tls = {Enabled = true}}
                    }
                };

                config.AddTarget("papertrail", papertrail);

                papertrail.Layout = "| " + Helpers.DisplayVersion +
                                    " |${level:uppercase=true}| ${message} ${exception:format=toString,Data}";

                LoggingRule rule = new("*", LogLevel.Error, papertrail);
                config.LoggingRules.Add(rule);
                LogManager.Configuration = config;
            }
            catch
            {
                Logger.Error("Failed to setup logging with papertrail");
            }
        }
    }
}
