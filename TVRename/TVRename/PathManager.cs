using System;
using Alphaleonis.Win32.Filesystem;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    public static class PathManager
    {
        private const string TVDB_FILE_NAME = "TheTVDB.xml";
        private const string SETTINGS_FILE_NAME = "TVRenameSettings.xml";
        private const string UI_LAYOUT_FILE_NAME = "Layout.xml";
        private const string STATISTICS_FILE_NAME = "Statistics.xml";

        private static string UserDefinedBasePath;

        public static FileInfo[] GetPossibleSettingsHistory()
        {
            return new DirectoryInfo(System.IO.Path.GetDirectoryName(TVDocSettingsFile.FullName)).GetFiles(SETTINGS_FILE_NAME + "*");
        }
        public static FileInfo[] GetPossibleTvdbHistory()
        {
            return new DirectoryInfo(System.IO.Path.GetDirectoryName(TVDocSettingsFile.FullName)).GetFiles(TVDB_FILE_NAME + "*");
        }

        public static void SetUserDefinedBasePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
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
            UserDefinedBasePath = path;
        }

        private static FileInfo GetFileInfo(string path, string file)
        {
            Directory.CreateDirectory(path);

            return new FileInfo(System.IO.Path.Combine(path, file));
        }

        public static FileInfo StatisticsFile
        {
            get
            {
                if (!string.IsNullOrEmpty(UserDefinedBasePath))
                {
                    return GetFileInfo(UserDefinedBasePath, STATISTICS_FILE_NAME);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), STATISTICS_FILE_NAME);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public static FileInfo UILayoutFile
        {
            get
            {
                if (!string.IsNullOrEmpty(UserDefinedBasePath))
                {
                    return GetFileInfo(UserDefinedBasePath, UI_LAYOUT_FILE_NAME);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), UI_LAYOUT_FILE_NAME);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public static FileInfo TVDBFile
        {
            get
            {
                if (!string.IsNullOrEmpty(UserDefinedBasePath))
                {
                    return GetFileInfo(UserDefinedBasePath, TVDB_FILE_NAME);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), TVDB_FILE_NAME);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public static FileInfo TVDocSettingsFile
        {
            get
            {
                if (!string.IsNullOrEmpty(UserDefinedBasePath))
                {
                    return GetFileInfo(UserDefinedBasePath, SETTINGS_FILE_NAME);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1"), SETTINGS_FILE_NAME);
                }
            }
        }

    }
}
