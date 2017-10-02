using System;
using System.IO;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    public class PathManager
    {
        public const string TVDBFileName = "TheTVDB.xml";
        public const string SettingsFileName = "TVRenameSettings.xml";
        const string LayoutFileName = "TVRenameLayout.dat";
        const string UILayoutFileName = "Layout.xml";
        const string StatisticsFileName = "Statistics.xml";

        private static string userDefinedBasePath;

        public static void SetUserDefinedBasePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            if (System.IO.File.Exists(path))
            {
                throw new ArgumentException("path");
            }
            path = System.IO.Path.GetFullPath(path); // Get absolute path, in case the given path was a relative one. This will make the Path absolute depending on the Environment.CurrentDirectory.
            // Why are we getting a absolute path here ? Simply because it is not guaranteed that the Environment.CurrentDirectory will not change a some point during runtime and then all bets are off were the Files are going to be saved, which would be fatal to the data integrity.(Saved changes might go to some file nobody even knew about )
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            userDefinedBasePath = path;
        }

        protected static FileInfo GetFileInfo(string path, string file)
        {
            return new FileInfo(System.IO.Path.Combine(path, file));
        }

        public static FileInfo StatisticsFile
        {
            get
            {
                if (!string.IsNullOrEmpty(userDefinedBasePath))
                {
                    return GetFileInfo(userDefinedBasePath, StatisticsFileName);
                }
                else
                {
                    return GetFileInfo(System.Windows.Forms.Application.UserAppDataPath, StatisticsFileName);
                }
            }
        }

        public static FileInfo LayoutFile
        {
            get
            {
                if (!string.IsNullOrEmpty(userDefinedBasePath))
                {
                    return GetFileInfo(userDefinedBasePath, LayoutFileName);
                }
                else
                {
                    return GetFileInfo(System.Windows.Forms.Application.UserAppDataPath, LayoutFileName);
                }
            }
        }

        public static FileInfo UILayoutFile
        {
            get
            {
                if (!string.IsNullOrEmpty(userDefinedBasePath))
                {
                    return GetFileInfo(userDefinedBasePath, UILayoutFileName);
                }
                else
                {
                    return GetFileInfo(System.Windows.Forms.Application.UserAppDataPath, UILayoutFileName);
                }
            }
        }

        public static FileInfo TVDBFile
        {
            get
            {
                if (!string.IsNullOrEmpty(userDefinedBasePath))
                {
                    return GetFileInfo(userDefinedBasePath, TVDBFileName);
                }
                else
                {
                    return GetFileInfo(System.Windows.Forms.Application.UserAppDataPath, TVDBFileName);
                }
            }
        }

        public static FileInfo TVDocSettingsFile
        {
            get
            {
                if (!string.IsNullOrEmpty(userDefinedBasePath))
                {
                    return GetFileInfo(userDefinedBasePath, SettingsFileName);
                }
                else
                {
                    return GetFileInfo(System.Windows.Forms.Application.UserAppDataPath, SettingsFileName);
                }
            }
        }

    }
}
