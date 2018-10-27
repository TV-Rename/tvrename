// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    public abstract class ActionDelete : ActionFileOperation
    {
        public override string Name => "Delete";
        public override long SizeOfWork => 100;
        public override int IconNumber => 9;
        public override string ScanListViewGroup => "lvgActionDelete";

        protected override string DestinationFolder => TargetFolder;
        protected override string DestinationFile => ProgressText;
    }
}
