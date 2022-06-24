using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualBasic.ApplicationServices;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets.Syslog;
using NLog.Targets.Syslog.Settings;
using System;
using System.Reflection;
//using System.Runtime.Remoting;
using System.Windows.Forms;
using Microsoft.Win32;
//using TVRename.Ipc;

namespace TVRename.App;

/// <summary>
/// Provides the primary form bootstrap including a splash screen.
/// </summary>
/// <seealso cref="WindowsFormsApplicationBase" />
internal class ApplicationBase : WindowsFormsApplicationBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private TVDoc? doc;

    /// <summary>
    /// Initializes the splash screen.
    /// </summary>
    protected override void OnCreateSplashScreen()
    {
        SplashScreen = new TVRenameSplash();

        CommandLineArgs commandLineArgs = new(CommandLineArgs);
        if (commandLineArgs.Hide || !Environment.UserInteractive)
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

        doc = LoadSettings(parameters);

        if (TVSettings.Instance.mode == TVSettings.BetaMode.BetaToo || TVSettings.Instance.ShareLogs)
        {
            SetupLogging();
        }

        RegisterForSystemEvents();

        // Show user interface
        UI ui = new(doc, (TVRenameSplash)SplashScreen, !parameters.Unattended && !parameters.Hide && Environment.UserInteractive);
        ui.Text = ui.Text + " " + Helpers.DisplayVersion;

        // Bind IPC actions to the form, this allows another instance to trigger form actions
        /*
        try
        {
            RemoteClient.Bind(ui);
        }
        catch (RemotingException ex)
        {
            Logger.Warn(
                $"Could not create IPC Port: {ex.Message} : TV Rename will not be able to accept incoming commands");
        }
*/
        MainForm = ui;
    }

    private void RegisterForSystemEvents()
    {
        //Always get the final notification when the event thread is shutting down
        //so we can unregister.

        SystemEvents.EventsThreadShutdown += OnEventsThreadShutdown;
        SystemEvents.SessionEnded += OnSessionEnded;
    }
    private void UnregisterFromSystemEvents()
    {
        SystemEvents.EventsThreadShutdown -= OnEventsThreadShutdown;
        SystemEvents.SessionEnded -= OnSessionEnded;
    }
    /* Notifies you when the thread that is distributing the events from the SystemEvents class is
     * shutting down so that we can unregister events on the SystemEvents class
     */
    private void OnEventsThreadShutdown(object? sender, EventArgs e)
    {
        //Unregister all our events as the notification thread is going away
        UnregisterFromSystemEvents();
    }

    /*  Triggered when the user is actually logging off or shutting down the system
     */
    private void OnSessionEnded(object sender, SessionEndedEventArgs e )
    {
        if (doc?.Dirty() ?? false)
        {
            doc?.WriteXMLSettings();
        }

        doc?.Closing();
    }

    private static TVDoc LoadSettings(CommandLineArgs commandLineArgs)
    {
        bool recover = false;
        string recoverText = string.Empty;

        // Check arguments for forced recover
        if (commandLineArgs.ForceRecover)
        {
            recover = true;
            recoverText = "Recover manually requested.";
        }

        SetupCustomSettings(commandLineArgs);

        FileInfo? tvdbFile = PathManager.TVDBFile;
        FileInfo? tvMazeFile = PathManager.TVmazeFile;
        FileInfo? tmdbFile = PathManager.TmdbFile;
        FileInfo? settingsFile = PathManager.TVDocSettingsFile;
        TVDoc doc;

        do // Loop until files correctly load
        {
            if (recover) // Recovery required, prompt user
            {
                RecoverXml recoveryForm = new(recoverText);

                if (recoveryForm.ShowDialog() == DialogResult.OK)
                {
                    tvdbFile = recoveryForm.TvDbFile;
                    tvMazeFile = recoveryForm.TvMazeFile;
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
            doc = new TVDoc(settingsFile, commandLineArgs);

            // Try loading TheTVDB cache file
            bool showIssues = !commandLineArgs.Unattended && !commandLineArgs.Hide;
            TheTVDB.LocalCache.Instance.Setup(tvdbFile, PathManager.TVDBFile, showIssues);
            TVmaze.LocalCache.Instance.Setup(tvMazeFile, PathManager.TVmazeFile, showIssues);
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

    private static void SetupCustomSettings(CommandLineArgs commandLineArgs)
    {
        // Check arguments for custom settings path
        string? userFilePath = commandLineArgs.UserFilePath;
        if (!string.IsNullOrEmpty(userFilePath))
        {
            try
            {
                PathManager.SetUserDefinedBasePath(userFilePath);
            }
            catch (Exception ex)
            {
                if (!commandLineArgs.Unattended && !commandLineArgs.Hide && Environment.UserInteractive)
                {
                    MessageBox.Show($"Error while setting the User-Defined File Path:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Logger.Error(ex, $"Error while setting the User-Defined File Path - EXITING: {userFilePath}");

                Environment.Exit(1);
            }
        }
    }

    private void SetupLogging()
    {
        ConfigurationItemFactory.Default.RegisterItemsFromAssembly(Assembly.Load("NLog.Targets.Syslog"));
        ConfigurationItemFactory.Default.RegisterItemsFromAssembly(Assembly.Load("Timber.io.NLog"));

        SetupPapertrailLogging();
        //SetupSemaTextLogging();

        Logger.Fatal($"TV Rename {Helpers.DisplayVersion} logging started on {Environment.OSVersion}, {(Environment.Is64BitOperatingSystem ? "64 Bit OS" : "")}, {(Environment.Is64BitProcess ? "64 Bit Process" : "")} {Environment.Version} {(Environment.UserInteractive ? "Interactive" : "")} with args: {string.Join(" ", CommandLineArgs)}");
        Logger.Info($"Copyright (C) {DateTime.Now.Year} TV Rename");
        Logger.Info("This program comes with ABSOLUTELY NO WARRANTY; This is free software, and you are welcome to redistribute it under certain conditions");
    }

    // ReSharper disable once UnusedMember.Local
    private static void SetupSemaTextLogging()
    {
        try
        {
            LoggingConfiguration config = LogManager.Configuration;
            SyslogTarget semaText = new()
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

            config.AddTarget("sema", semaText);
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
            semaText.Layout = jsonLayout;

            LoggingRule semaRule = new("*", LogLevel.Warn, semaText);
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
