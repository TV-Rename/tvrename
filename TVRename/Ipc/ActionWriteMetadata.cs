// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public abstract class ActionWriteMetadata : ActionDownload
    {
        protected readonly FileInfo Where;
        protected readonly ShowItem SelectedShow; // if for an entire show, rather than specific episode

        protected ActionWriteMetadata(FileInfo where, ShowItem sI)
        {
            Where = where;
            SelectedShow = sI;
        }

        public override string Produces => Where.FullName;

        public override string ProgressText => Where.Name;

        public override long SizeOfWork => 10000;

        public override string TargetFolder => Where == null ? null : Where.DirectoryName;

        public override IgnoreItem Ignore => Where == null ? null : new IgnoreItem(Where.FullName);

        public override string ScanListViewGroup => "lvgActionMeta";

        public override int IconNumber => 7;

        protected override string SeriesName => Episode?.Show?.ShowName ?? SelectedShow.ShowName;
        protected override string DestinationFolder => Where.DirectoryName;
        protected override string DestinationFile => Where.Name;
    }
}
