using System;
using System.Security;
using Alphaleonis.Win32.Filesystem;
using Microsoft.Win32;
using NLog;

namespace TVRename
{
    public static class RegistryHelper {
        //From https://www.cyotek.com/blog/configuring-the-emulation-mode-of-an-internet-explorer-webbrowser-control THANKS
        //Needed to ensure webBrowser renders HTML 5 content

        private const string InternetExplorerRootKey = @"Software\Microsoft\Internet Explorer";
        private const string BrowserEmulationKey = InternetExplorerRootKey + @"\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private enum BrowserEmulationVersion
        {
            Default = 0,
            Version7 = 7000,
            Version8 = 8000,
            Version8Standards = 8888,
            Version9 = 9000,
            Version9Standards = 9999,
            Version10 = 10000,
            Version10Standards = 10001,
            Version11 = 11000,
            Version11Edge = 11001
        }

        private static int GetInternetExplorerMajorVersion()
        {
            int result;

            result = 0;

            try
            {
                RegistryKey key;

                key = Registry.LocalMachine.OpenSubKey(InternetExplorerRootKey);

                if (key != null)
                {
                    object value;

                    value = key.GetValue("svcVersion", null) ?? key.GetValue("Version", null);

                    if (value != null)
                    {
                        string version;
                        int separator;

                        version = value.ToString();
                        separator = version.IndexOf('.');
                        if (separator != -1)
                        {
                            int.TryParse(version.Substring(0, separator), out result);
                        }
                    }
                }
            }
            catch (SecurityException se)
            {
                // The user does not have the permissions required to read from the registry key.
                logger.Error(se);
            }
            catch (UnauthorizedAccessException uae)
            {
                // The user does not have the necessary registry rights.
                logger.Error(uae);
            }

            return result;
        }
        
        private static BrowserEmulationVersion GetBrowserEmulationVersion()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);
                if (key != null)
                {
                    string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                    object value = key.GetValue(programName, null);

                    if (value != null)
                    {
                        return (BrowserEmulationVersion)Convert.ToInt32(value);
                    }
                }
            }
            catch (SecurityException se)
            {
                // The user does not have the permissions required to read from the registry key.
                logger.Error(se);
            }
            catch (UnauthorizedAccessException uae)
            {
                // The user does not have the necessary registry rights.
                logger.Error(uae);
            }

            return BrowserEmulationVersion.Default;
        }

        private static bool IsBrowserEmulationSet()
        {
            return GetBrowserEmulationVersion() != BrowserEmulationVersion.Default;
        }

        private static bool SetBrowserEmulationVersion(BrowserEmulationVersion browserEmulationVersion)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(BrowserEmulationKey,true);

                if (key != null)
                {
                    string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

                    if (browserEmulationVersion != BrowserEmulationVersion.Default)
                    {
                        // if it's a valid value, update or create the value
                        key.SetValue(programName, (int)browserEmulationVersion, RegistryValueKind.DWord);
                        logger.Warn("SETTING REGISTRY:{0}-{1}-{2}-{3}",key.Name,programName, (int)browserEmulationVersion, RegistryValueKind.DWord.ToString());
                    }
                    else
                    {
                        // otherwise, remove the existing value
                        key.DeleteValue(programName, false);
                        logger.Warn("DELETING REGISTRY KEY:{0}-{1}", key.Name, programName);
                    }

                    return true;
                }
            }
            catch (SecurityException se)
            {
                // The user does not have the permissions required to read from the registry key.
                logger.Error(se);
            }
            catch (UnauthorizedAccessException uae)
            {
                // The user does not have the necessary registry rights.
                logger.Error(uae);
            }

            return false;
        }

        private static bool SetBrowserEmulationVersion()
        {
            BrowserEmulationVersion emulationCode;

            int ieVersion = GetInternetExplorerMajorVersion();
            logger.Warn("IE Version {0} is identified",ieVersion );

            if (ieVersion >= 11)
            {
                emulationCode = BrowserEmulationVersion.Version11;
            }
            else
            {
                switch (ieVersion)
                {
                    case 10:
                        emulationCode = BrowserEmulationVersion.Version10;
                        break;
                    case 9:
                        emulationCode = BrowserEmulationVersion.Version9;
                        break;
                    case 8:
                        emulationCode = BrowserEmulationVersion.Version8;
                        break;
                    default:
                        emulationCode = BrowserEmulationVersion.Version7;
                        break;
                }
            }

            return SetBrowserEmulationVersion(emulationCode);
        }

        public static void UpdateBrowserEmulationVersion()
        {
            if (IsBrowserEmulationSet()) return;
            logger.Warn("Updating the registry to ensure that the latest browser version is used");
            if (!SetBrowserEmulationVersion())
            {
                logger.Error("Failed to update the browser emulation version");
            }
        }
    }
}
