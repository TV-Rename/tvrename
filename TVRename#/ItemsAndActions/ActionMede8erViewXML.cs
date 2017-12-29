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
    using Alphaleonis.Win32.Filesystem;
    using System.Windows.Forms;
    using System.Xml;


    public class ActionMede8erViewXML : Item, Action, ScanListItem, ActionWriteMetadata
    {
        public FileInfo Where;
        public ShowItem SI; // if for an entire show, rather than specific episode
        public int snum;

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si)
        {
            SI = si;
            Where = nfo;
            snum = -1;
        }

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si, int snum)
        {
            SI = si;
            Where = nfo;
            this.snum = snum;
        }

        public string produces
        {
            get { return Where.FullName; }
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
            get { return Where.Name; }
        }

        public double PercentDone
        {
            get { return Done ? 100 : 0; }
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
                writer = XmlWriter.Create(Where.FullName, settings);
                if (writer == null)
                    return false;
            }
            catch (Exception)
            {
                Done = true;
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
            Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ActionMede8erViewXML) && ((o as ActionMede8erViewXML).Where == Where);
        }

        public int Compare(Item o)
        {
            ActionMede8erViewXML nfo = o as ActionMede8erViewXML;

            return (Where.FullName).CompareTo(nfo.Where.FullName);
        }

        #endregion

        #region ScanListItem Members

        public IgnoreItem Ignore
        {
            get
            {
                if (Where == null)
                    return null;
                return new IgnoreItem(Where.FullName);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = SI.ShowName;
                if (snum > 0) { lvi.SubItems.Add(snum.ToString()); } else { lvi.SubItems.Add(""); }
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                lvi.SubItems.Add(Where.DirectoryName);
                lvi.SubItems.Add(Where.Name);

                lvi.Tag = this;

                return lvi;
            }
        }

        string ScanListItem.TargetFolder
        {
            get
            {
                if (Where == null)
                    return null;
                return Where.DirectoryName;
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
