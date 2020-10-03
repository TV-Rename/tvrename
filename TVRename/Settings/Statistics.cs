// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using Alphaleonis.Win32.Filesystem;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;

// ReSharper disable RedundantDefaultMemberInitializer

// Keeps count of some statistics.

namespace TVRename
{
    [Serializable]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    
    [XmlRootAttribute("Statistics", Namespace = "")]
    // ReSharper disable once InconsistentNaming
    public class TVRenameStats
    {
        public int AutoAddedShows = 0;
        public int AutoAddedMovies = 0;
        public int FilesCopied = 0;
        public int FilesMoved = 0;
        public int FilesRenamed = 0;
        public int FindAndOrganisesDone = 0;
        public int MissingChecksDone = 0;
        public int RenameChecksDone = 0;
        public int TorrentsMatched = 0;

        // The following aren't saved, but are calculated when we do a scan
        [XmlIgnoreAttribute] public int NsNumberOfEpisodes = -1; // -1 = unknown
        [XmlIgnoreAttribute] public int NsNumberOfEpisodesExpected = 0;
        [XmlIgnoreAttribute] public int NsNumberOfSeasons = 0;
        [XmlIgnoreAttribute] public int NsNumberOfShows = 0;
        [XmlIgnoreAttribute] public int NsNumberOfMovies = 0;
        [XmlIgnoreAttribute] private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static TVRenameStats? Load()
        {
            string fn = PathManager.StatisticsFile.FullName;
            return !File.Exists(fn) ? new TVRenameStats() : LoadFrom(fn);
        }

        public void Save()
        {
            SaveToFile(PathManager.StatisticsFile.FullName);
        }

        private static TVRenameStats? LoadFrom(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            XmlReaderSettings settings = new XmlReaderSettings {IgnoreComments = true, IgnoreWhitespace = true};
            TVRenameStats sc;

            try
            {
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(TVRenameStats));
                    sc = (TVRenameStats) xs.Deserialize(reader);
                }

                System.Diagnostics.Debug.Assert(sc != null);
            }
            catch (Exception e)
            {
               Logger.Fatal(e);
               return new TVRenameStats(); 
            }

            return sc;
        }

        private void SaveToFile([NotNull] string toFile)
        {
            System.IO.DirectoryInfo di = new System.IO.FileInfo(toFile).Directory;
            if (di is null)
            {
                Logger.Error($"Failed to save Statistics XML to {toFile}");
                return;
            }
            if (!di.Exists)
            {
                di.Create();
            }

            XmlWriterSettings settings = new XmlWriterSettings {Indent = true, NewLineOnAttributes = true};
            using (XmlWriter writer = XmlWriter.Create(toFile, settings))
            {
                XmlSerializer xs = new XmlSerializer(typeof(TVRenameStats));
                xs.Serialize(writer, this);
            }
        }
    }
}
