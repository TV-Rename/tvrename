// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;
    using System.Windows.Forms;
    using System.Xml;


    public class ActionMede8erViewXML : ActionWriteMetadata
    {
        public int Snum;

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si) : base(nfo, si)
        {
            Snum = -1;
        }

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si, int snum) : base(nfo,si)
        {
            Snum = snum;
        }

        #region Action Members

        public override  string Name => "Write Mede8er View Data";

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Where.FullName, settings))
                {

                    writer.WriteStartElement("FolderTag");
                    // is it a show or season folder
                    if (Snum >= 0)
                    {
                        // if episode thumbnails are generated, use ViewMode Photo, otherwise use List
                        XmlHelper.WriteElementToXml(writer, "ViewMode", TVSettings.Instance.EpJPGs ? "Photo" : "List");
                        XmlHelper.WriteElementToXml(writer, "ViewType", "Video");
                    }
                    else
                    {
                        XmlHelper.WriteElementToXml(writer, "ViewMode", "Preview");
                    }

                    writer.WriteEndElement();

                }
            }

            catch (Exception e)
            {
                Error = true;
                ErrorText = e.Message;
                Done = true;
                return false;

            }

            Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ActionMede8erViewXML xml) && (xml.Where == Where);
        }

        public override int Compare(Item o)
        {
            ActionMede8erViewXML nfo = o as ActionMede8erViewXML;

            return (Where.FullName).CompareTo(nfo.Where.FullName);
        }

        #endregion

        #region Item Members

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = SelectedShow.ShowName;
                lvi.SubItems.Add(Snum > 0 ? Snum.ToString() : "");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                lvi.SubItems.Add(Where.DirectoryName);
                lvi.SubItems.Add(Where.Name);

                lvi.Tag = this;

                return lvi;
            }
        }

        #endregion

    }
}
