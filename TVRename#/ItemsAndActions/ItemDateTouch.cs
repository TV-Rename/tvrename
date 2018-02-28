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
    using System.Windows.Forms;
    using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

    public class ItemDateTouch : Item, Action, ScanListItem
    {
        public ShowItem SI; // if for an entire show, rather than specific episode
        public Season SN; // if for an entire show, rather than specific episode
        public FileInfo WhereFile;
        public DirectoryInfo WhereDirectory;
        private readonly DateTime updateTime;

        public ItemDateTouch(FileInfo f, ProcessedEpisode pe, DateTime date)
        {
            this.Episode = pe;
            this.WhereFile = f;
            this.updateTime = date;
        }

        public ItemDateTouch(DirectoryInfo dir, Season sn, DateTime date)
        {
            this.SN = sn;
            this.WhereDirectory = dir;
            this.updateTime = date;

        }

        public ItemDateTouch(DirectoryInfo dir, ShowItem si, DateTime date)
        {
            this.SI = si;
            this.WhereDirectory = dir;
            this.updateTime = date;

        }


        public string produces => this.WhereFile?.FullName?? this.WhereDirectory?.FullName;

        #region Action Members

        public string Name => "Touch Update Time";

        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; set; }

        public string ProgressText => this.WhereFile?.Name??this.WhereDirectory?.Name;

        public double PercentDone => this.Done ? 100 : 0;

        public long SizeOfWork => 100;

        public bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (this.WhereFile != null)
                {
                    System.IO.File.SetLastWriteTimeUtc(this.WhereFile.FullName, this.updateTime);
                }
                if (this.WhereDirectory != null)
                {
                    System.IO.Directory.SetLastWriteTimeUtc(this.WhereDirectory.FullName, this.updateTime );
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
            if (nfo?.Episode == null)
                return -1;
            if (this.WhereFile != null)
                return String.Compare((this.WhereFile.FullName + this.Episode.Name), nfo.WhereFile.FullName + nfo.Episode.Name, StringComparison.Ordinal);
            return String.Compare((this.WhereDirectory.FullName + this.Episode.Name), nfo.WhereDirectory.FullName + nfo.Episode.Name, StringComparison.Ordinal);
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

                if (this.Episode != null)
                {
                    lvi.Text = this.Episode.SI.ShowName;
                    lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
                    lvi.SubItems.Add(this.Episode.NumsAsString());

                }
                else if (this.SN != null)
                {
                    lvi.Text = this.SN.TheSeries.Name;
                    lvi.SubItems.Add(this.SN.SeasonNumber.ToString());
                    lvi.SubItems.Add("");

                }
                else if (this.SI != null)
                {
                    lvi.Text = this.SI.ShowName;
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");

                }

                DateTime dt = this.updateTime;

                if ((dt.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(this.WhereFile?.DirectoryName??this.WhereDirectory?.FullName);
                lvi.SubItems.Add(this.WhereFile?.Name??this.WhereDirectory?.Name);

                lvi.Tag = this;

                //lv->Items->Add(lvi);
                return lvi;
            }
        }

        string ScanListItem.TargetFolder => this.WhereFile?.DirectoryName??this.WhereDirectory?.Name;

        public string ScanListViewGroup => "lvgUpdateFileDates";

        public int IconNumber => 7;

        public ProcessedEpisode Episode { get; }

        #endregion

    }
}
