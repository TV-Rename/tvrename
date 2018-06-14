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

    public class ActionDateTouch : ActionFileMetaData
    {
        public ShowItem SI; // if for an entire show, rather than specific episode
        public Season SN; // if for an entire show, rather than specific episode
        public FileInfo WhereFile;
        public DirectoryInfo WhereDirectory;
        private readonly DateTime updateTime;

        public ActionDateTouch(FileInfo f, ProcessedEpisode pe, DateTime date)
        {
            Episode = pe;
            WhereFile = f;
            updateTime = date;
        }

        public ActionDateTouch(DirectoryInfo dir, Season sn, DateTime date)
        {
            SN = sn;
            WhereDirectory = dir;
            updateTime = date;

        }

        public ActionDateTouch(DirectoryInfo dir, ShowItem si, DateTime date)
        {
            SI = si;
            WhereDirectory = dir;
            updateTime = date;

        }


        public override string Produces => WhereFile?.FullName?? WhereDirectory?.FullName;

        #region Action Members

        public override string Name => "Update Timestamp";

        public override string ProgressText => WhereFile?.Name??WhereDirectory?.Name;

        public override long SizeOfWork => 100;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (WhereFile != null)
                {
                    bool priorFileReadonly = WhereFile.IsReadOnly;
                    if (priorFileReadonly) WhereFile.IsReadOnly = false;
                    System.IO.File.SetLastWriteTimeUtc(WhereFile.FullName, updateTime);
                    if (priorFileReadonly) WhereFile.IsReadOnly = true;

                }
                if (WhereDirectory != null)
                {
                    System.IO.Directory.SetLastWriteTimeUtc(WhereDirectory.FullName, updateTime );
                }
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                Error = true;
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
            return (o is ActionDateTouch) && ((o as ActionDateTouch).WhereFile == WhereFile) && ((o as ActionDateTouch).WhereDirectory == WhereDirectory);
        }

        public override int Compare(Item o)
        {
            ActionDateTouch nfo = o as ActionDateTouch;

            if (Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            if (WhereFile != null)
                return String.Compare((WhereFile.FullName + Episode.Name), nfo.WhereFile.FullName + nfo.Episode.Name, StringComparison.Ordinal);
            return String.Compare((WhereDirectory.FullName + Episode.Name), nfo.WhereDirectory.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        public override IgnoreItem Ignore
        {
            get
            {
                if (WhereFile == null)
                    return null;
                return new IgnoreItem(WhereFile.FullName);
            }
        }

        public override ListViewItem ScanListViewItem
        {
            get
            {

                ListViewItem lvi = new ListViewItem();

                if (Episode != null)
                {
                    lvi.Text = Episode.SI.ShowName;
                    lvi.SubItems.Add(Episode.AppropriateSeasonNumber.ToString());
                    lvi.SubItems.Add(Episode.NumsAsString());

                }
                else if (SN != null)
                {
                    lvi.Text = SN.TheSeries.Name;
                    lvi.SubItems.Add(SN.SeasonNumber.ToString());
                    lvi.SubItems.Add("");

                }
                else if (SI != null)
                {
                    lvi.Text = SI.ShowName;
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");

                }

                DateTime dt = updateTime;

                if ((dt.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(WhereFile?.DirectoryName??WhereDirectory?.FullName);
                lvi.SubItems.Add(WhereFile?.Name??WhereDirectory?.Name);

                lvi.Tag = this;

                //lv->Items->Add(lvi);
                return lvi;
            }
        }

        public override string TargetFolder => WhereFile?.DirectoryName??WhereDirectory?.Name;

        public override string ScanListViewGroup => "lvgUpdateFileDates";

        public override int IconNumber => 7;

        #endregion

    }
}
