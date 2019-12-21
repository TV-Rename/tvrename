// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    using System;

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

        [CanBeNull]
        public override string Produces => whereFile?.FullName?? whereDirectory?.FullName;

        #region Action Members

        [NotNull]
        public override string Name => "Update Timestamp";

        [CanBeNull]
        public override string ProgressText => whereFile?.Name??whereDirectory?.Name;

        public override long SizeOfWork => 100;

        public override bool Go(TVRenameStats stats)
        {
            try
            {
                if (whereFile != null)
                {
                    ProcessFile(whereFile, updateTime);
                }

                if (whereDirectory != null)
                {
                    System.IO.Directory.SetLastWriteTimeUtc(whereDirectory.FullName, updateTime);
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                ErrorText = uae.Message;
                LastError = null;
                Error = true;
                Done = true;
                return false;
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                LastError = e;
                Error = true;
                Done = true;
                return false;
            }

            Done = true;
            return true;
        }

        private static void ProcessFile([NotNull] FileInfo whereFile, DateTime updateTime)
        {
            bool priorFileReadonly = whereFile.IsReadOnly;
            if (priorFileReadonly)
            {
                whereFile.IsReadOnly = false;
            }

            File.SetLastWriteTimeUtc(whereFile.FullName, updateTime);
            if (priorFileReadonly)
            {
                whereFile.IsReadOnly = true;
            }
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

            if (Episode is null)
            {
                return 1;
            }

            if (nfo?.Episode is null)
            {
                return -1;
            }

            if (whereFile != null)
            {
                return string.Compare((whereFile.FullName + Episode.Name), nfo.whereFile.FullName + nfo.Episode.Name, StringComparison.Ordinal);
            }

            return string.Compare((whereDirectory.FullName + Episode.Name), nfo.whereDirectory.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        [CanBeNull]
        public override IgnoreItem Ignore => whereFile is null ? null : new IgnoreItem(whereFile.FullName);

        protected override string SeriesName => (Episode != null) ? Episode.Show.ShowName :
            (season != null) ? season.TheSeries.Name : show.ShowName;
        protected override string SeasonNumber => (Episode != null) ? Episode.AppropriateSeasonNumber.ToString() :
            (season != null) ? season.SeasonNumber.ToString() : string.Empty;
        protected override string AirDate =>
            (updateTime.CompareTo(DateTime.MaxValue)) != 0 ? updateTime.ToShortDateString() : "";
        [CanBeNull]
        protected override string DestinationFolder => whereFile?.DirectoryName ?? whereDirectory?.FullName;
        [CanBeNull]
        protected override string DestinationFile => whereFile?.Name ?? whereDirectory?.Name;
        [CanBeNull]
        public override string TargetFolder => whereFile?.DirectoryName??whereDirectory?.Name;
        [NotNull]
        public override string ScanListViewGroup => "lvgUpdateFileDates";
        public override int IconNumber => 7;

        #endregion
    }
}
