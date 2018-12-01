using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    internal class CheckShows : ScanActivity
    {
        public CheckShows(TVDoc doc) : base(doc) {}

        public override void Check(SetProgressDelegate prog, int startpct, int totPct, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            MDoc.TheActionList = new ItemList();
            bool fullScan = (showList.Count == MDoc.Library.Shows.Count());

            if (TVSettings.Instance.RenameCheck)
                MDoc.Stats().RenameChecksDone++;

            if (TVSettings.Instance.MissingCheck)
                MDoc.Stats().MissingChecksDone++;

            if (fullScan)
            {
                // only do episode count if we're doing all shows and seasons
                MDoc.CurrentStats.NS_NumberOfEpisodes = 0;
                showList = MDoc.Library.Values;
            }

            DirFilesCache dfc = new DirFilesCache();

            int c = startpct;
            prog.Invoke(c, "Checking shows");
            foreach (ShowItem si in showList)
            {
                prog.Invoke(startpct + ((totPct-startpct) * c++ / showList.Count), si.ShowName);
                if (settings.Token.IsCancellationRequested)
                    return;

                LOGGER.Info("Rename and missing check: " + si.ShowName);

                new CheckAllFoldersExist(MDoc).CheckIfActive(si, dfc, settings);
                new MergeLibraryEpisodes(MDoc).CheckIfActive(si, dfc, settings);
                new RenameAndMissingCheck(MDoc).CheckIfActive(si, dfc, settings);
            } // for each show

            MDoc.RemoveIgnored();
            prog.Invoke(totPct, string.Empty);
        }

        public override bool Active() => TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck;
    }
}
