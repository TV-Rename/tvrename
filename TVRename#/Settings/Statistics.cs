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
        [XmlIgnoreAttribute] public int NS_NumberOfEpisodes = -1; // -1 = unknown
        [XmlIgnoreAttribute] public int NS_NumberOfEpisodesExpected = 0;
        [XmlIgnoreAttribute] public int NS_NumberOfSeasons = 0;
        [XmlIgnoreAttribute] public int NS_NumberOfShows = 0;

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
            TVRenameStats sc = null;

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
                System.Diagnostics.Debug.Print(e.Message + " " + e.StackTrace);
                return null;
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