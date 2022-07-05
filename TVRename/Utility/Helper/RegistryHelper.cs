//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using Microsoft.Win32;
using NLog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace TVRename;

public static class RegistryHelper
{
    //From https://www.cyotek.com/blog/configuring-the-emulation-mode-of-an-internet-explorer-webbrowser-control THANKS
    //Needed to ensure webBrowser renders HTML 5 content

    private const string INTERNET_EXPLORER_ROOT_KEY = @"Software\Microsoft\Internet Explorer";
    private const string BROWSER_EMULATION_KEY = INTERNET_EXPLORER_ROOT_KEY + @"\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage("ReSharper", "InconsistentNaming")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    private enum BrowserEmulationVersion
    {
        Default = 0,
        Version7 = 7000,
        Version8 = 8000,
        Version9 = 9000,
        Version10 = 10000,
        Version11 = 11000
    }

    private static int GetInternetExplorerMajorVersion()
    {
        try
        {
            RegistryKey? key = Registry.LocalMachine.OpenSubKey(INTERNET_EXPLORER_ROOT_KEY);

            if (key != null)
            {
                object? value = key.GetValue("svcVersion", null) ?? key.GetValue("Version", null);
                string? version = value?.ToString();

                if (version != null)
                {
                    int separator = version.IndexOf('.');
                    if (separator != -1)
                    {
                        int.TryParse(version.AsSpan(0, separator), out int result);
                        return result;
                    }
                }
            }
        }
        catch (SecurityException se)
        {
            // The user does not have the permissions required to read from the registry key.
            Logger.Error(se);
        }
        catch (UnauthorizedAccessException uae)
        {
            // The user does not have the necessary registry rights.
            Logger.Error(uae);
        }

        return 0;
    }

    private static BrowserEmulationVersion GetBrowserEmulationVersion()
    {
        try
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey(BROWSER_EMULATION_KEY, true);
            if (key != null)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                object? value = key.GetValue(programName, null);

                if (value != null)
                {
                    return (BrowserEmulationVersion)Convert.ToInt32(value);
                }
            }
        }
        catch (SecurityException se)
        {
            // The user does not have the permissions required to read from the registry key.
            Logger.Error(se);
        }
        catch (UnauthorizedAccessException uae)
        {
            // The user does not have the necessary registry rights.
            Logger.Error(uae);
        }

        return BrowserEmulationVersion.Default;
    }

    private static bool IsBrowserEmulationSet() => GetBrowserEmulationVersion() != BrowserEmulationVersion.Default;

    private static bool UpgradeBrowserEmulationRequired() => GetBrowserEmulationVersion() != GetInternetExplorerVersion();

    private static bool SetBrowserEmulationVersion(BrowserEmulationVersion browserEmulationVersion)
    {
        try
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(BROWSER_EMULATION_KEY, true);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (key != null)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

                if (browserEmulationVersion != BrowserEmulationVersion.Default)
                {
                    // if it's a valid value, update or create the value
                    key.SetValue(programName, (int)browserEmulationVersion, RegistryValueKind.DWord);
                    Logger.Warn($"SETTING REGISTRY:{key.Name}-{programName}-{(int)browserEmulationVersion}-{RegistryValueKind.DWord}");
                }
                else
                {
                    // otherwise, remove the existing value
                    key.DeleteValue(programName, false);
                    Logger.Warn($"DELETING REGISTRY KEY:{key.Name}-{programName}");
                }

                return true;
            }

            Logger.Warn($"Could not access {BROWSER_EMULATION_KEY}");
        }
        catch (SecurityException se)
        {
            // The user does not have the permissions required to read from the registry key.
            Logger.Error(se);
        }
        catch (UnauthorizedAccessException uae)
        {
            // The user does not have the necessary registry rights.
            Logger.Error(uae);
        }

        return false;
    }

    private static bool SetBrowserEmulationVersion() => SetBrowserEmulationVersion(GetInternetExplorerVersion());

    private static BrowserEmulationVersion GetInternetExplorerVersion()
    {
        int ieVersion = GetInternetExplorerMajorVersion();
        Logger.Info($"IE Version {ieVersion} is identified");

        if (ieVersion >= 11)
        {
            return BrowserEmulationVersion.Version11;
        }

        return ieVersion switch
        {
            10 => BrowserEmulationVersion.Version10,
            9 => BrowserEmulationVersion.Version9,
            8 => BrowserEmulationVersion.Version8,
            _ => BrowserEmulationVersion.Version7
        };
    }

    public static void UpdateBrowserEmulationVersion()
    {
        if (!IsBrowserEmulationSet() || UpgradeBrowserEmulationRequired())
        {
            Logger.Warn("Updating the registry to ensure that the latest browser version is used");
            if (!SetBrowserEmulationVersion())
            {
                Logger.Error("Failed to update the browser emulation version");
            }
        }
    }
}
