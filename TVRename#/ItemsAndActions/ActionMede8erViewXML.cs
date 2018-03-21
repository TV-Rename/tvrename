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
        public FileInfo Where;
        public ShowItem SI; // if for an entire show, rather than specific episode
        public int snum;

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si)
        {
            this.SI = si;
            this.Where = nfo;
            this.snum = -1;
        }

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si, int snum)
        {
            this.SI = si;
            this.Where = nfo;
            this.snum = snum;
        }

        public override string produces => this.Where.FullName;

        #region Action Members

        public override  string Name => "Write Mede8er View Data";

        public override string ProgressText => this.Where.Name;

        public double PercentDone => this.Done ? 100 : 0;

        public override long SizeOfWork => 10000;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };
            // "try" and silently fail.  eg. when file is use by other...
            XmlWriter writer;
            try
            {
                writer = XmlWriter.Create(this.Where.FullName, settings);
                if (writer == null)
                    return false;
            }
            catch (Exception)
            {
                this.Done = true;
                return true;
            }

            writer.WriteStartElement("FolderTag");
            // is it a show or season folder
            if (snum >= 0)
            {
                // if episode thumbnails are generated, use ViewMode Photo, otherwise use List
                if (TVSettings.Instance.EpJPGs)
                {
                    XMLHelper.WriteElementToXML(writer, "ViewMode", "Photo");
                }
                else
                {
                    XMLHelper.WriteElementToXML(writer, "ViewMode", "List");
                }
                XMLHelper.WriteElementToXML(writer, "ViewType", "Video");
            }
            else
            {
                XMLHelper.WriteElementToXML(writer, "ViewMode", "Preview");
            }
            writer.WriteEndElement();

            writer.Close();
            this.Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ActionMede8erViewXML) && ((o as ActionMede8erViewXML).Where == this.Where);
        }

        public override int Compare(Item o)
        {
            ActionMede8erViewXML nfo = o as ActionMede8erViewXML;

            return (this.Where.FullName).CompareTo(nfo.Where.FullName);
        }

        #endregion

        #region Item Members

        public override  IgnoreItem Ignore
        {
            get
            {
                if (this.Where == null)
                    return null;
                return new IgnoreItem(this.Where.FullName);
            }
        }

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = this.SI.ShowName;
                lvi.SubItems.Add(this.snum > 0 ? this.snum.ToString() : "");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                lvi.SubItems.Add(this.Where.DirectoryName);
                lvi.SubItems.Add(this.Where.Name);

                lvi.Tag = this;

                return lvi;
            }
        }

        public override string TargetFolder => this.Where == null ? null : this.Where.DirectoryName;

        public override string ScanListViewGroup => "lvgActionMeta";

        public override int IconNumber => 7;

        #endregion

    }
}
