using System;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using JetBrains.Annotations;
using NLog;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace TVRename
{
    public class CefWrapper{
        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile CefWrapper? InternalInstance;
        private static readonly object SyncRoot = new();

        [NotNull]
        public static CefWrapper Instance
        {
            get
            {
                if (InternalInstance is null)
                {
                    lock (SyncRoot)
                    {
                        // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                        if (InternalInstance is null)
                        {
                            InternalInstance = new CefWrapper();
                        }
                    }
                }

                return InternalInstance;
            }
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string? architectureSpecificBrowserPath;
        private string? architectureSpecificLocalesDirPath;
        private string? architectureSpecificResourcesDirPath;
        public void InitialiseBrowserFramework()
        {
            try
            {
                CefSettings settings = new()
                {
                    CachePath = PathManager.CefCachePath,
                    UserDataPath = PathManager.CefCachePath,
                    LogFile = PathManager.CefLogFile,
                };

                if (!Helpers.InDebug())
                {
                    SetArchitecturePaths(settings);
                }

                Cef.Initialize(settings);
            }
            catch (FileNotFoundException fex)
            {
                Logger.Fatal(fex,$"Can't initialise CEF {PathManager.CefCachePath}, {PathManager.CefLogFile}, {architectureSpecificBrowserPath}, {architectureSpecificLocalesDirPath}, {architectureSpecificResourcesDirPath}");
            }

            CheckForBroswerDependencies(false);

            //Cef.EnableHighDPISupport(); todo - reinstate when we support high DPI
        }

        private void SetArchitecturePaths(CefSettings settings)
        {
            architectureSpecificBrowserPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? "x64" : "x86",
                "CefSharp.BrowserSubprocess.exe");

            if (File.Exists(architectureSpecificBrowserPath))
            {
                Logger.Info($"Updated path for BrowserSubprocess: {architectureSpecificBrowserPath}");
                settings.BrowserSubprocessPath = architectureSpecificBrowserPath;
            }
            else
            {
                Logger.Error($"Could not update path for CEF BrowserSubprocess: {architectureSpecificBrowserPath}");
            }

            architectureSpecificLocalesDirPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? "x64" : "x86",
                "locales");

            if (Directory.Exists(architectureSpecificLocalesDirPath))
            {
                Logger.Info($"Updated path for LocalesDirPath: {architectureSpecificLocalesDirPath}");
                settings.LocalesDirPath = architectureSpecificLocalesDirPath;
            }
            else
            {
                Logger.Error($"Could not update path for CEF LocalesDirPath: {architectureSpecificLocalesDirPath}");
            }

            architectureSpecificResourcesDirPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? "x64" : "x86");

            if (Directory.Exists(architectureSpecificResourcesDirPath))
            {
                Logger.Info($"Updated path for ResourcesDirPath: {architectureSpecificResourcesDirPath}");
                settings.ResourcesDirPath = architectureSpecificResourcesDirPath;
            }
            else
            {
                Logger.Error(
                    $"Could not update path for CEF ResourcesDirPath: {architectureSpecificResourcesDirPath}");
            }
        }

        public void CheckForBroswerDependencies(bool showUi)
        {
            try
            {
                if (Helpers.InDebug())
                {
                    DependencyChecker.AssertAllDependenciesPresent();
                }
                else
                {
                    DependencyChecker.AssertAllDependenciesPresent(
                        browserSubProcessPath: architectureSpecificBrowserPath,
                        localesDirPath: architectureSpecificLocalesDirPath,
                        resourcesDirPath: architectureSpecificResourcesDirPath
                    );
                }
                Logger.Info("Dependencies all found");
                if (showUi)
                {
                    MessageBox.Show("Dependencies all found", "Browser Capability Test");
                }
            }
            catch (Exception a)
            {
                Logger.Error(a, "Missing Cef Dependencies");
                if (showUi)
                {
                    MessageBox.Show("Dependencies missing - see log for more details", "Browser Capability Test");
                }
            }
        }
    }
}
