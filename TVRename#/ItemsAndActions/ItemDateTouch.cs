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
    using System.IO;
    using Directory = Alphaleonis.Win32.Filesystem.Directory;
    using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

    public class ItemDateTouch : Item, Action, ScanListItem, ActionWriteMetadata
    {
        public ShowItem SI; // if for an entire show, rather than specific episode
        public FileInfo WhereFile;
        public DirectoryInfo WhereDirectory;
        private DateTime updateTime;

        public ItemDateTouch(FileInfo f, ProcessedEpisode pe, DateTime date)
        {
            this.Episode = pe;
            this.WhereFile = f;
            this.updateTime = date;
        }

        public ItemDateTouch(DirectoryInfo dir, ShowItem si, DateTime date)
        {
            this.SI = si;
            this.WhereDirectory = dir;
            this.updateTime = date;

        }

        public string produces
        {
            get { return this.WhereFile?.FullName?? this.WhereDirectory?.FullName; }
        }

        #region Action Members

        public string Name
        {
            get { return "Touch Update Time"; }
        }

        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; set; }

        public string ProgressText
        {
            get { return this.WhereFile?.Name??this.WhereDirectory?.Name; }
        }

        public double PercentDone
        {
            get { return this.Done ? 100 : 0; }
        }

        public long SizeOfWork
        {
            get { return 100; }
        }

        public bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (WhereFile != null)
                {
                    System.IO.File.SetLastWriteTimeUtc(WhereFile.FullName, updateTime);
                }
                if (WhereDirectory != null)
                {
                    //TODO maybe this should be a new field on the series that is the max aired time before now
                    //TODO confirm timezones etc
                    System.IO.Directory.SetLastWriteTimeUtc(WhereDirectory.FullName, updateTime );
                }
            }
            catch (Exception e)
            {
                this.ErrorText = e.Message;
                this.Error = true;
                this.Done = true;
                return false;
            }

            this.Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ItemDateTouch) && ((o as ItemDateTouch).WhereFile == this.WhereFile) && ((o as ItemDateTouch).WhereDirectory == this.WhereDirectory);
        }

        public int Compare(Item o)
        {
            ItemDateTouch nfo = o as ItemDateTouch;

            if (this.Episode == null)
                return 1;
            if (nfo == null || nfo.Episode == null)
                return -1;
            if (this.WhereFile != null)
                return (this.WhereFile.FullName + this.Episode.Name).CompareTo(nfo.WhereFile.FullName + nfo.Episode.Name);
            return (this.WhereDirectory.FullName + this.Episode.Name).CompareTo(nfo.WhereDirectory.FullName + nfo.Episode.Name);
        }

        #endregion

        #region ScanListItem Members

        public IgnoreItem Ignore
        {
            get
            {
                if (this.WhereFile == null)
                    return null;
                return new IgnoreItem(this.WhereFile.FullName);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = this.Episode.SI.ShowName;
                lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
                lvi.SubItems.Add(this.Episode.NumsAsString());
                DateTime? dt = this.Episode.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(this.WhereFile?.DirectoryName??this.WhereDirectory?.Name);
                lvi.SubItems.Add(this.WhereFile?.Name??this.WhereDirectory?.Name);

                lvi.Tag = this;

                //lv->Items->Add(lvi);
                return lvi;
            }
        }

        string ScanListItem.TargetFolder
        {
            get
            {
                return this.WhereFile?.DirectoryName??this.WhereDirectory?.Name;
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
