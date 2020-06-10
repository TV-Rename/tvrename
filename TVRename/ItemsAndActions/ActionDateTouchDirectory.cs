using System;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class ActionDateTouchDirectory : ActionDateTouch
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
}
