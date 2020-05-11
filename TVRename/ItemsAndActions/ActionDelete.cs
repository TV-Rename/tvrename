// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using JetBrains.Annotations;

namespace TVRename
{
    public abstract class ActionDelete : ActionFileOperation
    {
        [NotNull]
        public override string Name => "Delete";
        public override long SizeOfWork => 100;
        public override int IconNumber => 9;
        [NotNull]
        public override string ScanListViewGroup => "lvgActionDelete";

        public override string DestinationFolder => TargetFolder;
        public override string DestinationFile => ProgressText;
    }
}
