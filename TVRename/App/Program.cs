//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace TVRename.App;

/// <summary>
/// Application entry point.
/// </summary>
public static class Program
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static ApplicationBase? TvRename;

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
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
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
                return;
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
        }

        Logger.Info("Application exiting");
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
            string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
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
