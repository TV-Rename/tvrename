using System;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public class PathManager
    {
        /// <summary>
        /// The default base path for application data.
        /// Used if a user path is not specified.
        /// </summary>
        /// <see cref="userBasePath"/>
        public static readonly string DefaultBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TV Rename");

        /// <summary>
        /// The user base path if specifed.
        /// If not specified then the default base path will be used.
        /// </summary>
        /// <see cref="DefaultBasePath"/>
        private static string userBasePath;

        /// <summary>
        /// The file name for the settings file.
        /// </summary>
        public const string SettingsFileName = "Settings.xml";

        /// <summary>
        /// The file name for the statistics file.
        /// </summary>
        public const string StatisticsFileName = "Statistics.xml";

        /// <summary>
        /// The file name for the layout configuration file.
        /// </summary>
        public const string LayoutFileName = "Layout.xml";

        /// <summary>
        /// The file name for the TheTVDB cache file.
        /// </summary>
        public const string TVDBFileName = "TheTVDB.xml";

        /// <summary>
        /// Gets file information for the settings file.
        /// </summary>
        /// <value>
        /// The settings file information.
        /// </value>
        /// <see cref="SettingsFileName"/>
        public static FileInfo SettingsFile => GetFileInfo("config", SettingsFileName);

        /// <summary>
        /// Gets file information for the statistics file.
        /// </summary>
        /// <value>
        /// The statistics file information.
        /// </value>
        /// <see cref="StatisticsFileName"/>
        public static FileInfo StatisticsFile => GetFileInfo("config", StatisticsFileName);

        /// <summary>
        /// Gets file information for the layout configuration file.
        /// </summary>
        /// <value>
        /// The layout file information.
        /// </value>
        /// <see cref="LayoutFileName"/>
        public static FileInfo LayoutFile => GetFileInfo("config", LayoutFileName);

        /// <summary>
        /// Gets file information for the TheTVDB cache file.
        /// </summary>
        /// <value>
        /// The TVDB cache file information.
        /// </value>
        /// <see cref="TVDBFileName"/>
        public static FileInfo TVDBFile => GetFileInfo("cache", TVDBFileName);

        /// <summary>
        /// Gets file information for a specified path and file relative to the base path.
        /// </summary>
        /// <param name="relativePath">The path relative to the base path.</param>
        /// <param name="file">The file to read.</param>
        /// <returns>File information for specifed file.</returns>
        private static FileInfo GetFileInfo(string relativePath, string file)
        {
            var path = Path.Combine(userBasePath ?? DefaultBasePath, relativePath);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return new FileInfo(Path.Combine(path, file));
        }

        /// <summary>
        /// Sets the base path to a user defined directory, creating it if it does not exist.
        /// </summary>
        /// <param name="path">The base path.</param>
        /// <exception cref="ArgumentNullException">Path is null.</exception>
        public static void SetUserDefinedBasePath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            path = Path.GetFullPath(path); // Resolve relative path, depending on Environment.CurrentDirectory

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            userBasePath = path;
        }
    }
}
