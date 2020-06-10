using System;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class ActionDateTouchEpisode : ActionDateTouch
    {
        public ActionDateTouchEpisode(FileInfo f, ProcessedEpisode pe, DateTime date):base(date)
        {
            Episode = pe;
            whereFile = f;
        }
        private readonly FileInfo whereFile;
        public override string Produces => whereFile.FullName;
        public override string ProgressText => whereFile.Name;
        public override IgnoreItem Ignore => new IgnoreItem(whereFile.FullName);
        public override string SeriesName => Episode != null ? Episode.Show.ShowName :string.Empty;
        public override string SeasonNumber => Episode != null ? Episode.AppropriateSeasonNumber.ToString() : string.Empty;
        public override string? DestinationFolder => whereFile.DirectoryName;
        public override string? DestinationFile => whereFile.Name;
        public override string? TargetFolder => whereFile.DirectoryName;

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
