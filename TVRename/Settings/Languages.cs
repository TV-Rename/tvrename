using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    [Serializable]
    [System.ComponentModel.DesignerCategoryAttribute("code")]

    [XmlRoot("Languages", Namespace = "")]
    public class Languages : List<Language>
    {
        [XmlIgnoreAttribute] private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static Languages Load()
        {
            string fn = PathManager.LanguagesFile.FullName;

            return !File.Exists(fn) ? new Languages() : LoadFrom(fn);
        }

        public void Save()
        {
            SaveToFile(PathManager.LanguagesFile.FullName);
        }

        private static Languages LoadFrom(string filename)
        {
            if (!File.Exists(filename))
                return null;

            XmlReaderSettings settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
            Languages sc;

            try
            {
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Languages));
                    sc = (Languages)xs.Deserialize(reader);
                    System.Diagnostics.Debug.Assert(sc != null);
                }
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                return new Languages();
            }

            return sc;
        }

        private void SaveToFile(string toFile)
        {
            System.IO.DirectoryInfo di = new System.IO.FileInfo(toFile).Directory;
            if (!di.Exists)
                di.Create();

            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, NewLineOnAttributes = true };
            using (XmlWriter writer = XmlWriter.Create(toFile, settings))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Languages));
                xs.Serialize(writer, this);
            }
        }

        public Language GetLanguageFromCode(string languageAbbreviation)
        {
            foreach (Language l in this)
            {
                if (l.Abbreviation == languageAbbreviation) return l;
            }
            return null;
        }

        public Language GetLanguageFromLocalName(string language)
        {
            foreach (Language l in this)
            {
                if (l.Name == language) return l;
            }
            return null;
        }

        public Language GetLanguageFromId(int languageId)
        {
            foreach (Language l in this)
            {
                if (l.Id == languageId) return l;
            }
            return null;
        }
    }

    public class Language
    {
        public Language() {
        }

        public Language(int id, string abbreviation, string name, string englishName)
        {
            Id = id;
            Abbreviation = abbreviation;
            Name = name;
            EnglishName = englishName;
        }

        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }
}
