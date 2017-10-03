// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

    public class ActionMede8erViewXML : Item, Action, ScanListItem, ActionWriteMetadata
    {
        public FileInfo Where;
        public ShowItem SI; // if for an entire show, rather than specific episode
        public int snum;

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si)
        {
            this.SI = si;
            this.Where = nfo;
        }

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si, int snum)
        {
            this.SI = si;
            this.Where = nfo;
            this.snum = snum;
        }

        public string produces
        {
            get { return this.Where.FullName; }
        }

        #region Action Members

        public string Name
        {
            get { return "Write Mede8er View Data"; }
        }

        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; set; }

        public string ProgressText
        {
            get { return this.Where.Name; }
        }

        public double PercentDone
        {
            get { return this.Done ? 100 : 0; }
        }

        public long SizeOfWork
        {
            get { return 10000; }
        }

        public bool Go(ref bool pause, TVRenameStats stats)
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
                //                XmlWriter writer = XmlWriter.Create(this.Where.FullName, settings);
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
            XMLHelper.WriteElementToXML(writer, "ViewMode", "Movie");
            writer.WriteEndElement();

            writer.Close();
            this.Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ActionMede8erViewXML) && ((o as ActionMede8erViewXML).Where == this.Where);
        }

        public int Compare(Item o)
        {
            ActionMede8erViewXML nfo = o as ActionMede8erViewXML;

            return (this.Where.FullName ).CompareTo(nfo.Where.FullName );
        }

        #endregion

        #region ScanListItem Members

        public IgnoreItem Ignore
        {
            get
            {
                if (this.Where == null)
                    return null;
                return new IgnoreItem(this.Where.FullName);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = this.SI.ShowName;
                if (snum > 0) { lvi.SubItems.Add(snum.ToString()); } else { lvi.SubItems.Add(""); }
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                lvi.SubItems.Add(this.Where.DirectoryName);
                lvi.SubItems.Add(this.Where.Name);

                lvi.Tag = this;

                return lvi;
            }
        }

        string ScanListItem.TargetFolder
        {
            get
            {
                if (this.Where == null)
                    return null;
                return this.Where.DirectoryName;
            }
        }

        public string ScanListViewGroup
        {
            get { return "lvgActionMeta"; }
        }

        public int IconNumber
        {
            get { return 7; }
        }

        public ProcessedEpisode Episode { get; private set; }

        #endregion

    }
}
