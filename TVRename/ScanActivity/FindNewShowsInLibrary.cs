// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    internal class FindNewShowsInLibrary : ScanActivity
    {
        public FindNewShowsInLibrary(TVDoc doc) : base(doc)
        {
        }
        [NotNull]
        protected override string Checkname() => "Looked in the library for any new shows to be added (bulk add)";

        protected override void DoCheck([NotNull] SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            BulkAddManager bam = new BulkAddManager(MDoc);
            bam.CheckFolders(settings.Token, prog, false,!settings.Unattended);
            AskUserAboutShows(settings, bam);

            if (!bam.AddItems.Any(s => s.CodeKnown))
            {
                return;
            }

            List<int> idsToAdd = bam.AddItems.Where(s => s.CodeKnown).Select(folder => folder.TVDBCode).ToList();
            
            bam.AddAllToMyShows();

            MDoc.SetDirty();
            MDoc.DoDownloadsFG(settings.Unattended,settings.Hidden);

            List<ShowItem> addedShows = idsToAdd.Select(s => MDoc.Library.ShowItem(s)).ToList();

            //add each new show into the shows being scanned
            foreach (ShowItem si in addedShows)
            {
                showList.Add(si);
            }
            LOGGER.Info("Added new shows called: {0}", addedShows.Select(si => si?.ShowName).ToCsv());

            MDoc.DoWhenToWatch(true,settings.Unattended,settings.Hidden);

            MDoc.WriteUpcoming();
            MDoc.WriteRecent();
        }

        private void AskUserAboutShows(TVDoc.ScanSettings settings, [NotNull] BulkAddManager bam)
        {
            foreach (FoundFolder folder in bam.AddItems)
            {
                if (settings.Token.IsCancellationRequested)
                {
                    break;
                }

                AskUserAboutShow(folder);
            }
        }

        private void AskUserAboutShow([NotNull] FoundFolder folder)
        {
            if (folder.CodeKnown)
            {
                return;
            }

            BulkAddManager.GuessShowItem(folder, MDoc.Library,true);

            if (folder.CodeKnown)
            {
                return;
            }

            FolderMonitorEdit ed = new FolderMonitorEdit(folder);
            if (ed.ShowDialog() != DialogResult.OK || ed.Code == -1)
            {
                return;
            }

            folder.TVDBCode = ed.Code;
        }

        public override bool Active() => TVSettings.Instance.DoBulkAddInScan;
    }
}
