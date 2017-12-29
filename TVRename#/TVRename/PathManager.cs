using System;
using Alphaleonis.Win32.Filesystem;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    public class PathManager
    {
        public const string TVDBFileName = "TheTVDB.xml";
        public const string SettingsFileName = "TVRenameSettings.xml";
        const string LayoutFileName = "TVRenameLayout.dat";
        const string UiLayoutFileName = "Layout.xml";
        const string StatisticsFileName = "Statistics.xml";

        private static string _userDefinedBasePath;

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
            _userDefinedBasePath = path;
        }

        protected static FileInfo GetFileInfo(string path, string file)
        {
            return new FileInfo(System.IO.Path.Combine(path, file));
        }

        public static FileInfo StatisticsFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_userDefinedBasePath))
                {
                    return GetFileInfo(_userDefinedBasePath, StatisticsFileName);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), StatisticsFileName);
                }
            }
        }

        public static FileInfo LayoutFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_userDefinedBasePath))
                {
                    return GetFileInfo(_userDefinedBasePath, LayoutFileName);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), LayoutFileName);
                }
            }
        }

        public static FileInfo UiLayoutFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_userDefinedBasePath))
                {
                    return GetFileInfo(_userDefinedBasePath, UiLayoutFileName);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), UiLayoutFileName);
                }
            }
        }

        public static FileInfo TVDBFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_userDefinedBasePath))
                {
                    return GetFileInfo(_userDefinedBasePath, TVDBFileName);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), TVDBFileName);
                }
            }
        }

        public static FileInfo TVDocSettingsFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_userDefinedBasePath))
                {
                    return GetFileInfo(_userDefinedBasePath, SettingsFileName);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), SettingsFileName);
                }
            }
        }

    }
}
