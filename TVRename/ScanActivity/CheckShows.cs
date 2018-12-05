// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

namespace TVRename
{
    internal class CheckShows : ScanActivity
    {
        public CheckShows(TVDoc doc) : base(doc) {}

        protected override void Check(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            MDoc.TheActionList = new ItemList();

            if (TVSettings.Instance.RenameCheck)
                MDoc.Stats().RenameChecksDone++;

            if (TVSettings.Instance.MissingCheck)
                MDoc.Stats().MissingChecksDone++;

            if (settings.Type == TVSettings.ScanType.Full)
            {
                // only do episode count if we're doing all shows and seasons
                MDoc.CurrentStats.NS_NumberOfEpisodes = 0;
                showList = MDoc.Library.Values;
            }

            DirFilesCache dfc = new DirFilesCache();

            int c = 0;
            UpdateStatus(c,showList.Count, "Checking shows");
            foreach (ShowItem si in showList)
            {
                UpdateStatus(c++ ,showList.Count, si.ShowName);
                if (settings.Token.IsCancellationRequested)
                    return;

                LOGGER.Info("Rename and missing check: " + si.ShowName);

                new CheckAllFoldersExist(MDoc).CheckIfActive(si, dfc, settings);
                new MergeLibraryEpisodes(MDoc).CheckIfActive(si, dfc, settings);
                new RenameAndMissingCheck(MDoc).CheckIfActive(si, dfc, settings);
            } // for each show

            MDoc.RemoveIgnored();
        }

        public override bool Active() => TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck;
    }
}
