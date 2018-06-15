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
        private readonly ShowItem show; // if for an entire show, rather than specific episode
        private readonly Season season; // if for an entire show, rather than specific episode
        private readonly FileInfo whereFile;
        private readonly DirectoryInfo whereDirectory;
        private readonly DateTime updateTime;

        public ActionDateTouch(FileInfo f, ProcessedEpisode pe, DateTime date)
        {
            Episode = pe;
            whereFile = f;
            updateTime = date;
        }

        public ActionDateTouch(DirectoryInfo dir, Season sn, DateTime date)
        {
            season = sn;
            whereDirectory = dir;
            updateTime = date;
        }

        public ActionDateTouch(DirectoryInfo dir, ShowItem si, DateTime date)
        {
            show = si;
            whereDirectory = dir;
            updateTime = date;
        }

        public override string Produces => whereFile?.FullName?? whereDirectory?.FullName;

        #region Action Members

        public override string Name => "Update Timestamp";

        public override string ProgressText => whereFile?.Name??whereDirectory?.Name;

        public override long SizeOfWork => 100;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (whereFile != null)
                {
                    bool priorFileReadonly = whereFile.IsReadOnly;
                    if (priorFileReadonly) whereFile.IsReadOnly = false;
                    System.IO.File.SetLastWriteTimeUtc(whereFile.FullName, updateTime);
                    if (priorFileReadonly) whereFile.IsReadOnly = true;
                }
                if (whereDirectory != null)
                {
                    System.IO.Directory.SetLastWriteTimeUtc(whereDirectory.FullName, updateTime );
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
            return (o is ActionDateTouch touch) && (touch.whereFile == whereFile) && (touch.whereDirectory == whereDirectory);
        }

        public override int Compare(Item o)
        {
            ActionDateTouch nfo = o as ActionDateTouch;

            if (Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            if (whereFile != null)
                return string.Compare((whereFile.FullName + Episode.Name), nfo.whereFile.FullName + nfo.Episode.Name, StringComparison.Ordinal);
            return string.Compare((whereDirectory.FullName + Episode.Name), nfo.whereDirectory.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        public override IgnoreItem Ignore => whereFile == null ? null : new IgnoreItem(whereFile.FullName);

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
                else if (season != null)
                {
                    lvi.Text = season.TheSeries.Name;
                    lvi.SubItems.Add(season.SeasonNumber.ToString());
                    lvi.SubItems.Add("");

                }
                else if (show != null)
                {
                    lvi.Text = show.ShowName;
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                }

                DateTime dt = updateTime;

                lvi.SubItems.Add((dt.CompareTo(DateTime.MaxValue)) != 0 ? dt.ToShortDateString() : "");

                lvi.SubItems.Add(whereFile?.DirectoryName??whereDirectory?.FullName);
                lvi.SubItems.Add(whereFile?.Name??whereDirectory?.Name);

                lvi.Tag = this;

                return lvi;
            }
        }

        public override string TargetFolder => whereFile?.DirectoryName??whereDirectory?.Name;

        public override string ScanListViewGroup => "lvgUpdateFileDates";

        public override int IconNumber => 7;

        #endregion
    }
}
