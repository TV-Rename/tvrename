using Alphaleonis.Win32.Filesystem;
using CefSharp;
using CefSharp.WinForms;
using Microsoft.Win32;
using MscVersion;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TVRename;

public class CefWrapper
{
    //We are using the singleton design pattern
    //http://msdn.microsoft.com/en-au/library/ff650316.aspx

    private static volatile CefWrapper? InternalInstance;
    private static readonly object SyncRoot = new();

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
            using CefSettings settings = new();

            settings.CachePath = PathManager.CefCachePath();
            settings.UserDataPath = PathManager.CefCachePath();
            settings.LogFile = PathManager.CefLogFile();

            if (!Helpers.InDebug())
            {
                SetArchitecturePaths(settings);
            }
            Cef.Initialize(settings);
        }
        catch (System.IO.FileNotFoundException fex)
        {
            Logger.Warn(fex,
                $"Can't initialise CEF with settings {PathManager.CefCachePath()}, {PathManager.CefLogFile()}, {architectureSpecificBrowserPath}, {architectureSpecificLocalesDirPath}, {architectureSpecificResourcesDirPath}");

            Logger.Warn("C++ Version (Installers): " + Vc2015Installed().ToCsv());
            Logger.Warn("C++ Version (Git Library): " + VcRuntime.GetInstalled(_ => true).Select(VersionToString).ToCsv());
            Logger.Warn("If C++ 2019 is not installed visit: https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-160 and install the latest appropriate version");

            MessageBox.Show("TV Rename needs Microsoft Visual C++ 2015-2019 Redistributable to be present. Downloading installer now.", "Missing Dependencies");
            string urlToDownload = Environment.Is64BitProcess ? "vc_redist.x64.exe" : "vc_redist.x86.exe";
            $"https://aka.ms/vs/17/release/{urlToDownload}".OpenUrlInBrowser();
        }
        CheckForBrowserDependencies(false);

        Cef.EnableHighDPISupport();
    }

    public static void Shutdown()
    {
        Cef.Shutdown();
    }

    private void SetArchitecturePaths(CefSettings settings)
    {
        architectureSpecificBrowserPath = Path.Combine(
            AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "runtimes",
            "win-" + (Environment.Is64BitProcess ? "x64" : "x86"), "native",
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
            AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "runtimes",
            "win-" + (Environment.Is64BitProcess ? "x64" : "x86"), "native",
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
            AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "runtimes",
            "win-" + (Environment.Is64BitProcess ? "x64" : "x86"), "native");

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

    public void CheckForBrowserDependencies(bool showUi)
    {
        try
        {
            if (!Helpers.InDebug())
            {
                Logger.Info($"Checking for CEF dependencies {architectureSpecificResourcesDirPath}");
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
            Logger.Warn(a, "Missing Cef Dependencies");
            if (showUi)
            {
                MessageBox.Show("Dependencies missing - see log for more details", "Browser Capability Test");
            }
        }
    }

    private static string VersionToString(VcRuntimeVersion arg) => $"{arg.MscVer}-{arg.Architecture}-{arg.Version}";
    private static IEnumerable<string> Vc2015Installed()
    {
        const string DEPENDENCIES_PATH = @"SOFTWARE\Classes\Installer\Dependencies";
        List<string> returnValue = [];

        using (RegistryKey? dependencies = Registry.LocalMachine.OpenSubKey(DEPENDENCIES_PATH))
        {
            if (dependencies == null)
            {
                return returnValue;
            }

            foreach (string subKeyName in dependencies.GetSubKeyNames().Where(n => !n.ToLower().Contains("dotnet") && !n.ToLower().Contains("microsoft")))
            {
                using RegistryKey? subDir = Registry.LocalMachine.OpenSubKey(DEPENDENCIES_PATH + "\\" + subKeyName);
                string? value = subDir?.GetValue("DisplayName")?.ToString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                if (Regex.IsMatch(value, @"C\+\+"))
                {
                    returnValue.Add(value);
                }
            }
        }
        return returnValue;
    }
}
