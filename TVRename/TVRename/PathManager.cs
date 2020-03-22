// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    public static class PathManager
    {
        private const string TVDB_FILE_NAME = "TheTVDB.xml";
        private const string TVMAZE_FILE_NAME = "TVmaze.xml";
        private const string SETTINGS_FILE_NAME = "TVRenameSettings.xml";
        private const string UI_LAYOUT_FILE_NAME = "Layout.xml";
        private const string STATISTICS_FILE_NAME = "Statistics.xml";
        private const string LANGUAGES_FILE_NAME = "Languages.xml";

        private static string UserDefinedBasePath;

        public static FileInfo[] GetPossibleSettingsHistory() => new DirectoryInfo(System.IO.Path.GetDirectoryName(TVDocSettingsFile.FullName)).GetFiles(SETTINGS_FILE_NAME + "*");

        public static FileInfo[] GetPossibleTvdbHistory() => new DirectoryInfo(System.IO.Path.GetDirectoryName(TVDocSettingsFile.FullName)).GetFiles(TVDB_FILE_NAME + "*");
        public static FileInfo[] GetPossibleTvMazeHistory() => new DirectoryInfo(System.IO.Path.GetDirectoryName(TVDocSettingsFile.FullName)).GetFiles(TVMAZE_FILE_NAME + "*");

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

        [NotNull]
        private static FileInfo GetFileInfo([NotNull] string file)
        {
            string path = UserDefinedBasePath.HasValue()
                ? UserDefinedBasePath
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename",
                    "TVRename", "2.1");
            Directory.CreateDirectory(path);
            return new FileInfo(System.IO.Path.Combine(path, file));
        }

        [NotNull]
        public static FileInfo StatisticsFile => GetFileInfo(STATISTICS_FILE_NAME);
        // ReSharper disable once InconsistentNaming
        [NotNull]
        public static FileInfo UILayoutFile => GetFileInfo(UI_LAYOUT_FILE_NAME);
        // ReSharper disable once InconsistentNaming
        [NotNull]
        public static FileInfo TVDBFile => GetFileInfo(TVDB_FILE_NAME);
        // ReSharper disable once InconsistentNaming
        [NotNull]
        public static FileInfo TVmazeFile=> GetFileInfo(TVMAZE_FILE_NAME);
        // ReSharper disable once InconsistentNaming
        [NotNull]
        public static FileInfo TVDocSettingsFile => GetFileInfo(SETTINGS_FILE_NAME);
        [NotNull]
        public static FileInfo LanguagesFile => GetFileInfo(LANGUAGES_FILE_NAME);
    }
}
