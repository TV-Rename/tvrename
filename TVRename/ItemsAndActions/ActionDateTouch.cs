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

    public abstract class ActionDateTouch : ActionFileMetaData
    {
        protected readonly DateTime UpdateTime;

        protected ActionDateTouch(DateTime time)
        {
            UpdateTime = time;
        }

        #region Action Members

        public override string Name => "Update Timestamp";
        public override long SizeOfWork => 100;

        #endregion

        public override string ScanListViewGroup => "lvgUpdateFileDates";
        public override int IconNumber => 7;
        public override string AirDateString =>
            UpdateTime.CompareTo(DateTime.MaxValue) != 0 ? UpdateTime.ToShortDateString() : "";
    }

    internal class  ActionDateTouchDirectory : ActionDateTouch
    {
        private readonly DirectoryInfo whereDirectory;

        protected ActionDateTouchDirectory(DirectoryInfo dir, DateTime date) : base(date)
        {
            whereDirectory = dir;
        }

        public override string Produces => whereDirectory.FullName;
        public override string ProgressText => whereDirectory.Name;

        public override bool SameAs(Item o)
        {
            return o is ActionDateTouchDirectory touch && touch.whereDirectory == whereDirectory;
        }

        public override ActionOutcome Go(TVRenameStats stats)
        {
            try
            {
                    System.IO.Directory.SetLastWriteTimeUtc(whereDirectory.FullName, UpdateTime);
            }
            catch (UnauthorizedAccessException uae)
            {
                return new ActionOutcome(uae);
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }

            return ActionOutcome.Success();
        }

        public override IgnoreItem? Ignore => null;

        public override string? DestinationFolder => whereDirectory.FullName;
        public override string? DestinationFile => whereDirectory.Name;
        public override string? TargetFolder => whereDirectory.Name;


        public override int CompareTo(object o)
        {
            ActionDateTouchDirectory nfo = o as ActionDateTouchDirectory;

            if (Episode is null)
            {
                return 1;
            }

            if (nfo?.Episode is null)
            {
                return -1;
            }

            return string.Compare(whereDirectory.FullName + Episode.Name,
                nfo.whereDirectory.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
    }

    internal class ActionDateTouchShow : ActionDateTouchDirectory
    {
        private readonly ShowItem show; // if for an entire show, rather than specific episode
        public ActionDateTouchShow(DirectoryInfo dir, ShowItem si, DateTime date) :base(dir,date)
        {
            show = si;
        }
        public override string SeriesName => show.ShowName;
        public override string SeasonNumber => string.Empty;
    }

    internal class ActionDateTouchSeason : ActionDateTouchDirectory
    {
        private readonly ProcessedSeason processedSeason; // if for an entire show, rather than specific episode
        public ActionDateTouchSeason(DirectoryInfo dir, ProcessedSeason sn, DateTime date) : base(dir, date)
        {
            processedSeason = sn;
        }
        public override string SeriesName => processedSeason.Show.ShowName;
        public override string SeasonNumber => processedSeason.SeasonNumber.ToString();
    }

    internal class ActionDateTouchEpisode : ActionDateTouch
    {
        public ActionDateTouchEpisode(FileInfo f, ProcessedEpisode pe, DateTime date):base(date)
        {
            Episode = pe;
            whereFile = f;
        }
        private readonly FileInfo whereFile;
        public override string Produces => whereFile.FullName;
        public override string ProgressText => whereFile.Name ;
        public override IgnoreItem Ignore => new IgnoreItem(whereFile.FullName);

        public override string SeriesName => Episode != null ? Episode.Show.ShowName :string.Empty;
        public override string SeasonNumber => Episode != null ? Episode.AppropriateSeasonNumber.ToString() : string.Empty;

        public override string? DestinationFolder => whereFile.DirectoryName ;
        public override string? DestinationFile => whereFile.Name ;
        public override string? TargetFolder => whereFile.DirectoryName ;

        public override bool SameAs(Item o)
        {
            return o is ActionDateTouchEpisode touch && touch.whereFile == whereFile;
        }

        public override ActionOutcome Go(TVRenameStats stats)
        {
            try
            {
                ProcessFile(whereFile, UpdateTime);
            }
            catch (UnauthorizedAccessException uae)
            {
                return new ActionOutcome(uae);
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }

            return ActionOutcome.Success();
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


        public override int CompareTo(object o)
        {
            ActionDateTouchEpisode nfo = o as ActionDateTouchEpisode;

            if (Episode is null)
            {
                return 1;
            }

            if (nfo?.Episode is null)
            {
                return -1;
            }

            return string.Compare(whereFile.FullName + Episode.Name, nfo.whereFile.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
    }
}
