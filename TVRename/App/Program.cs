// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CefSharp;
using JetBrains.Annotations;
using TVRename.Ipc;

namespace TVRename.App
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    public static class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Defines the entry point of the application.
        /// Checks if the application is already running and if so, performs actions via IPC and exits.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        [STAThread]
        private static void Main([NotNull] string[] args)
        {
            Logger.Info($"TV Rename {Helpers.DisplayVersion} started with args: {string.Join(" ", args)}");
            Logger.Info($"Copyright (C) {DateTime.Now.Year} TV Rename");
            Logger.Info("This program comes with ABSOLUTELY NO WARRANTY; This is free software, and you are welcome to redistribute it under certain conditions");

            try
            {
                //DependencyChecker.AssertAllDependenciesPresent();
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += delegate(object sender, ThreadExceptionEventArgs eventArgs) {
                Exception e = eventArgs.Exception;
                Logger.Fatal(e, "UNHANDLED ERROR - Application.ThreadException");
                Environment.Exit(1);
            };
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;

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
            // Check if an application instance is already running
            Mutex mutex = new Mutex(true, "TVRename", out bool newInstance);

            if (!newInstance)
            {
                // Already running
                Logger.Warn("An instance is already running");

                // Create an IPC channel to the existing instance
                RemoteClient.Proxy();

                // Transparent proxy to the existing instance
                RemoteClient ipc = new RemoteClient();

                // If already running and no command line arguments then bring instance to the foreground and quit
                if (args.Length == 0)
                {
                    ipc.FocusWindow();
                }
                else
                {
                    Logger.Warn($"Sending {args.ToCsv()} to the running instance.");
                    ipc.SendArgs(args);
                }

                return;
            }

            try
            {
                Logger.Info("Starting new instance");

                ApplicationBase s = new ApplicationBase();

                s.Run(args);

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

            Logger.Info("Application exiting");
        }

        private static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs args)
    {
        Exception e = (Exception) args.ExceptionObject;
        Logger.Fatal(e,"UNHANDLED ERROR - GlobalExceptionHandler");
        Environment.Exit(1);
        }
    }
}
