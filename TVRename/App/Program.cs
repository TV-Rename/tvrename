//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;

namespace TVRename.App;

/// <summary>
/// Application entry point.
/// </summary>
public static class Program
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static ApplicationBase? TvRename;

    // Set to start TV Rename again once it has closed (to use a new theme)
    private static bool restartWhenClosed;
    private const string RESTARTED_FROM = "TVRENAME_RESTARTED_FROM";

    internal static void RestartWhenClosed() => restartWhenClosed = true;

    // Set when this copy was started by a restart, so the startup scan and update popup are not done again
    internal static bool StartedByRestart { get; private set; }

    /// <summary>
    /// Defines the entry point of the application.
    /// Checks if the application is already running and if so, performs actions via IPC and exits.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    [STAThread]
    private static void Main(string[] args)
    {
        Logger.Info($"TV Rename {Helpers.DisplayVersion} started with args: {string.Join(" ", args)}");
        Logger.Info($"Copyright (C) {TimeHelpers.LocalNow().Year} TV Rename");
        Logger.Info("This program comes with ABSOLUTELY NO WARRANTY; This is free software, and you are welcome to redistribute it under certain conditions");

        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.SetCompatibleTextRenderingDefault(false);

        try
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;
            Application.ThreadException += delegate (object _, ThreadExceptionEventArgs eventArgs)
            {
                Exception e = eventArgs.Exception;
                if (e is HttpRequestException hre)
                {
                    Logger.Fatal(e, $"UNHANDLED ERROR - Application.ThreadException ({hre.TargetSite}-{hre.InnerException})");
                    if (hre.InnerException is { } ie)
                    {
                        Logger.Fatal(ie, $"UNHANDLED ERROR - Application.ThreadException ({ie.TargetSite}-{ie.InnerException})");
                    }
                }
                else
                {
                    Logger.Fatal(e, "UNHANDLED ERROR - Application.ThreadException");
                }

                Environment.Exit(1);
            };
        }
        catch (Exception e)
        {
            Logger.Fatal(e);
        }

        if (args.Contains("/?", StringComparer.OrdinalIgnoreCase))
        {
            Logger.Info(CommandLineArgs.Helptext());

            //redirect console output to parent process
            //must be before any calls to Console.WriteLine()
            //MS: Never got this to work quite right - seems there is no simple way to output
            //to the command line console if you are a winforms app
            if (NativeMethods.AttachParentConsole())
            {
                Console.WriteLine(CommandLineArgs.Helptext());
            }
            else
            {
                Logger.Info("Could not attach to console");
            }
            return;
        }

        CloseIfPreviousCopyStillRunning();

        Mutex? mutex = null;
        try
        {
            SingleInstanceService singleInstanceService = new(OnArgumentsReceived);

            // Check if an application instance is already running
            mutex = new Mutex(true, "TVRename", out bool newInstance);

            if (!singleInstanceService.IsFirstInstance() || !newInstance)
            {
                // Already running
                Logger.Warn("An instance is already running, exiting");
                SingleInstanceService.SendArgumentsToExistingInstance();
                Environment.Exit(1);
            }

            Logger.Info("Starting new instance");

            TvRename = new ApplicationBase();

            TvRename.Run(args);

            GC.KeepAlive(mutex);
        }
        catch (TVRenameOperationInterruptedException)
        {
            Logger.Warn("USER REQUESTED End: Application exiting with error");
            Environment.Exit(1);
        }
        catch (ObjectDisposedException)
        {
            Logger.Warn("ObjectDisposedError: Application exiting with error");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Application exiting with error");

            new ShowException(ex).ShowDialog();

            Environment.Exit(1);
        }
        finally
        {
            mutex?.Dispose();
            Logger.Info("Application exiting");
        }

        StartAgainIfAsked(args);
    }

    // Only the settings that stay in force are passed on, so one off tasks (/scan, /doall, /quit...) are not done again
    private static readonly string[] KeptArgs = ["/userfilepath:", "/createmissing", "/ignoremissing", "/norenamecheck", "/hide", "/unattended"];

    // The new copy waits for this one to finish, as only one copy of TV Rename can run at a time
    private static void StartAgainIfAsked(string[] args)
    {
        if (!restartWhenClosed)
        {
            return;
        }

        try
        {
            List<string> keptArgs = [.. args.Where(a => KeptArgs.Any(k => a.StartsWith(k, StringComparison.OrdinalIgnoreCase)))];
            ProcessStartInfo startInfo = new(Environment.ProcessPath!) { UseShellExecute = false, WorkingDirectory = Environment.CurrentDirectory };
            foreach (string arg in keptArgs)
            {
                startInfo.ArgumentList.Add(arg);
            }

            startInfo.Environment[RESTARTED_FROM] = Environment.ProcessId.ToString();
            Logger.Info($"Restarting with args: {string.Join(" ", keptArgs)}");
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Could not restart TV Rename");
        }
    }

    // Leave the old copy alone (it may still be saving) and do not pass it any arguments
    private static void CloseIfPreviousCopyStillRunning()
    {
        if (!WaitForPreviousCopyToClose())
        {
            Logger.Warn("The previous copy of TV Rename is still running 60 seconds after a restart, so this copy is closing");
            Environment.Exit(1);
        }
    }

    // Waits for the process, not the mutex: the old copy closes the mutex in Main's finally, but its
    // single instance semaphore and argument listener are only released when the process itself ends
    private static bool WaitForPreviousCopyToClose()
    {
        string? previous = Environment.GetEnvironmentVariable(RESTARTED_FROM);
        if (previous is null || !int.TryParse(previous, out int processId))
        {
            return true;
        }

        Environment.SetEnvironmentVariable(RESTARTED_FROM, null);
        StartedByRestart = true;
        try
        {
            using Process old = Process.GetProcessById(processId);

            // Process ids get reused, but the copy that asked for the restart started before this one
            if (old.StartTime >= Process.GetCurrentProcess().StartTime)
            {
                return true;
            }

            return old.WaitForExit(TimeSpan.FromSeconds(60));
        }
        catch (ArgumentException)
        {
            // It has already closed
            return true;
        }
        catch (InvalidOperationException)
        {
            // It closed while being checked
            return true;
        }
        catch (Win32Exception)
        {
            // A process this copy cannot look at, so not the copy that asked for the restart
            return true;
        }
    }

    private static void OnArgumentsReceived(string[] args)
    {
        if (TvRename is null)
        {
            Logger.Warn($"Cannot pass {args.ToCsv()} to running instance ApplicationBase 's' is not created yet.");
        }
        else
        {
            Logger.Info($"Received {args.ToCsv()}, sending to the application.");
            TvRename.ProcessReceivedArgs(args);
        }
    }

    private static Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        if (args.Name.StartsWith("CefSharp", StringComparison.Ordinal))
        {
            string assemblyName = args.Name.Split([','], 2)[0] + ".dll";
            string architectureSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? "x64" : "x86",
                assemblyName);

            if (File.Exists(architectureSpecificPath))
            {
                Logger.Warn($"Updated path for Assembly: {architectureSpecificPath}");
                return Assembly.LoadFile(architectureSpecificPath);
            }
        }

        return null;
    }
    private static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs args)
    {
        Exception e = (Exception)args.ExceptionObject;
        Logger.Fatal(e, "UNHANDLED ERROR - GlobalExceptionHandler");
        Environment.Exit(1);
    }
}
