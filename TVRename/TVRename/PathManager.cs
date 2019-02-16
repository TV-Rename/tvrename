// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
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
        private const string LANGUAGES_FILE_NAME = "Languages.xml";

        // =========================================================================================================================================
        private const string SHOWS_FILE_NAME = "TVRenameShows.xml";
        private const string SHOWS_COLLECTION_FILE_NAME = "TVRenameColls.xml";
        private const string SHOWS_DEFAULT_COLLECTION = "2.1";

        private static string UserDefinedBasePath;

        // =========================================================================================================================================
        private static string SHOWS_COLLECTION = "";

        public static FileInfo[] GetPossibleShowsHistory()
        {
            return new DirectoryInfo(System.IO.Path.GetDirectoryName(TVDocShowsFile.FullName)).GetFiles(SHOWS_FILE_NAME + "*");
        }
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

        // =========================================================================================================================================
        public static string ShowCollection
        {
            get
            {
                return SHOWS_COLLECTION;
            }
            set
            {
                if (value != SHOWS_DEFAULT_COLLECTION)
                {
                    SHOWS_COLLECTION = value;
                }
                else
                {
                    SHOWS_COLLECTION = "";
                }
            }
        }

        public static FileInfo ShowCollectionFile
        {
            get
            {
                if (!string.IsNullOrEmpty(UserDefinedBasePath))
                {
                    return GetFileInfo(UserDefinedBasePath, SHOWS_COLLECTION_FILE_NAME);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename"), SHOWS_COLLECTION_FILE_NAME);
                }
            }
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
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", (!string.IsNullOrEmpty(SHOWS_COLLECTION) ? SHOWS_COLLECTION : SHOWS_DEFAULT_COLLECTION)), STATISTICS_FILE_NAME);
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
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", (!string.IsNullOrEmpty(SHOWS_COLLECTION) ? "" : SHOWS_DEFAULT_COLLECTION)), UI_LAYOUT_FILE_NAME);
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
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", (!string.IsNullOrEmpty(SHOWS_COLLECTION) ? SHOWS_COLLECTION : SHOWS_DEFAULT_COLLECTION)), TVDB_FILE_NAME);
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
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", (!string.IsNullOrEmpty(SHOWS_COLLECTION) ? "" : SHOWS_DEFAULT_COLLECTION)), SETTINGS_FILE_NAME);
                }
            }
        }

        public static FileInfo LanguagesFile
        {
            get
            {
                if (!string.IsNullOrEmpty(UserDefinedBasePath))
                {
                    return GetFileInfo(UserDefinedBasePath, LANGUAGES_FILE_NAME);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", (!string.IsNullOrEmpty(SHOWS_COLLECTION) ? "" : SHOWS_DEFAULT_COLLECTION)), LANGUAGES_FILE_NAME);
                }
            }
        }

        // =========================================================================================================================================
        public static FileInfo TVDocShowsFile
        {
            get
            {
                if (!string.IsNullOrEmpty(UserDefinedBasePath))
                {
                    return GetFileInfo(UserDefinedBasePath, SHOWS_FILE_NAME);
                }
                else
                {
                    return GetFileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", (!string.IsNullOrEmpty(SHOWS_COLLECTION) ? SHOWS_COLLECTION : SHOWS_DEFAULT_COLLECTION)), (!string.IsNullOrEmpty(SHOWS_COLLECTION) ? SHOWS_FILE_NAME : SETTINGS_FILE_NAME));
                }
            }
        }
    }
}
