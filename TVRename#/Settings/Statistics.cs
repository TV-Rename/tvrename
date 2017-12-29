// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using Alphaleonis.Win32.Filesystem;
using System.Xml;
using System.Xml.Serialization;

// Keeps count of some statistics.

namespace TVRename
{
    [Serializable]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    
    [XmlRootAttribute("Statistics", Namespace = "")]
    public class TVRenameStats
    {
        public int AutoAddedShows = 0;
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
        [XmlIgnoreAttribute] private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static TVRenameStats Load()
        {
            String fn = PathManager.StatisticsFile.FullName;
            if (!File.Exists(fn))
                return new TVRenameStats();
            return LoadFrom(fn);
        }

        public void Save()
        {
            SaveToFile(PathManager.StatisticsFile.FullName);
        }

        private static TVRenameStats LoadFrom(String filename)
        {
            if (!File.Exists(filename))
                return null;

            XmlReaderSettings settings = new XmlReaderSettings {IgnoreComments = true, IgnoreWhitespace = true};
            TVRenameStats sc;

            try
            {
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    XmlSerializer xs = new XmlSerializer(typeof (TVRenameStats));
                    sc = (TVRenameStats) xs.Deserialize(reader);
                    System.Diagnostics.Debug.Assert(sc != null);
                    reader.Close();
                }
            }
            catch (Exception e)
            {
               _logger.Fatal(e);
               return new TVRenameStats(); 
            }

            return sc;
        }

        private void SaveToFile(String toFile)
        {
            System.IO.DirectoryInfo di = new System.IO.FileInfo(toFile).Directory;
            if (!di.Exists)
                di.Create();

            XmlWriterSettings settings = new XmlWriterSettings {Indent = true, NewLineOnAttributes = true};
            using (XmlWriter writer = XmlWriter.Create(toFile, settings))
            {
                XmlSerializer xs = new XmlSerializer(typeof (TVRenameStats));
                xs.Serialize(writer, this);
                writer.Close();
            }
        }
    }
}
