// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TVRename
{
    internal class FindNewShowsInLibrary : ScanActivity
    {
        public FindNewShowsInLibrary(TVDoc doc) : base(doc)
        {
        }

        protected override void Check(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            BulkAddManager bam = new BulkAddManager(MDoc);
            bam.CheckFolders(settings.Token, prog, false);
            AskUserAboutShows(settings, bam);

            if (!bam.AddItems.Any(s => s.CodeKnown)) return;

            List<int> idsToAdd = bam.AddItems.Where(s => s.CodeKnown).Select(folder => folder.TVDBCode).ToList();
            
            bam.AddAllToMyShows();

            MDoc.SetDirty();
            MDoc.DoDownloadsFG();

            List<ShowItem> addedShows = idsToAdd.Select(s => MDoc.Library.ShowItem(s)).ToList();

            //add each new show into the shows being scanned
            foreach (ShowItem si in addedShows)
            {
                showList.Add(si);
            }
            LOGGER.Info("Added new shows called: {0}", string.Join(",", addedShows.Select(si => si.ShowName)));

            MDoc.DoWhenToWatch(true);

            MDoc.WriteUpcoming();
            MDoc.WriteRecent();
        }

        private void AskUserAboutShows(TVDoc.ScanSettings settings, BulkAddManager bam)
        {
            foreach (FoundFolder folder in bam.AddItems)
            {
                if (settings.Token.IsCancellationRequested)
                    break;
                AskUserAboutShow(folder);
            }
        }

        private void AskUserAboutShow(FoundFolder folder)
        {
            if (folder.CodeKnown)
                return;

            BulkAddManager.GuessShowItem(folder, MDoc.Library);

            if (folder.CodeKnown)
                return;

            FolderMonitorEdit ed = new FolderMonitorEdit(folder);
            if ((ed.ShowDialog() != DialogResult.OK) || (ed.Code == -1))
                return;

            folder.TVDBCode = ed.Code;
        }

        public override bool Active() => TVSettings.Instance.DoBulkAddInScan;
    }
}
