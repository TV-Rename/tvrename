using System;
using System.Collections.Generic;
using System.Globalization;
using Alphaleonis.Win32.Filesystem;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Xml;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using System.Text;
using NodaTime.Extensions;

namespace TVRename
{
    public class ShowCollection
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public string Path { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ShowCollection (string Pa)
        {
            Path = Pa;
        }

        public ShowCollection (XmlReader reader)
        {
            LoadXMLCollectionItem(reader);
        }

        public void WriteXMLCollectionItem (XmlWriter writer)
        {
            writer.WriteStartElement("CollectionItem");
            XmlHelper.WriteElementToXml(writer, "Path", Path);
            XmlHelper.WriteElementToXml(writer, "Name", Name);
            XmlHelper.WriteElementToXml(writer, "Description", Description);
            writer.WriteEndElement();
        }

        private void LoadXMLCollectionItem (XmlReader reader)
        {
            reader.Read();
            while (!reader.EOF)
            {
                if ((reader.Name == "CollectionItem") && (!reader.IsStartElement()))
                    break;

                if (reader.Name == "Path")
                {
                    Path = reader.ReadElementContentAsString();
                }
                if (reader.Name == "Name")
                {
                    Name = reader.ReadElementContentAsString();
                }
                if (reader.Name == "Description")
                {
                    Description = reader.ReadElementContentAsString();
                }
                else
                {
                    reader.ReadOuterXml();
                }
            }
        }
    }
}
