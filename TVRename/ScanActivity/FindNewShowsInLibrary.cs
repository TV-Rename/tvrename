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
        protected override string CheckName() => "Looked in the library for any new shows to be added (bulk add)";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowConfiguration> showList, TVDoc.ScanSettings settings)
        {
            BulkAddSeriesManager bam = new BulkAddSeriesManager(MDoc);
            bam.CheckFolders(settings.Token, prog, false,!settings.Unattended);
            AskUserAboutShows(settings, bam);

            if (!bam.AddItems.Any(s => s.CodeKnown))
            {
                return;
            }

            List<int> idsToAdd = bam.AddItems.Where(s => s.CodeKnown).Select(folder => folder.TVDBCode).ToList();
            
            bam.AddAllToMyShows();

            MDoc.SetDirty();
            MDoc.DoDownloadsFG(settings.Unattended,settings.Hidden,settings.Owner);

            List<ShowConfiguration> addedShows = idsToAdd.Select(s => MDoc.TvLibrary.GetShowItem(s)).ToList();

            //add each new show into the shows being scanned
            foreach (ShowConfiguration si in addedShows)
            {
                showList.Add(si);
            }
            LOGGER.Info("Added new shows called: {0}", addedShows.Select(si => si.ShowName).ToCsv());

            MDoc.DoWhenToWatch(true,settings.Unattended,settings.Hidden, settings.Owner);

            MDoc.WriteUpcoming();
            MDoc.WriteRecent();
        }

        private void AskUserAboutShows(TVDoc.ScanSettings settings, [NotNull] BulkAddSeriesManager bam)
        {
            foreach (PossibleNewTvShow folder in bam.AddItems)
            {
                if (settings.Token.IsCancellationRequested)
                {
                    break;
                }

                AskUserAboutShow(folder,settings.Owner);
            }
        }

        private void AskUserAboutShow([NotNull] PossibleNewTvShow folder, IDialogParent owner)
        {
            if (folder.CodeKnown)
            {
                return;
            }

            BulkAddSeriesManager.GuessShowItem(folder, MDoc.TvLibrary,true);

            if (folder.CodeKnown)
            {
                return;
            }

            FolderMonitorEdit ed = new FolderMonitorEdit(folder);

            owner.ShowChildDialog(ed);
            DialogResult x = ed.DialogResult;
            int code = ed.Code;
            ed.Dispose();

            if (x != DialogResult.OK || code == -1)
            {
                return;
            }

            folder.TVDBCode = code;
        }

        public override bool Active() => TVSettings.Instance.DoBulkAddInScan;
    }
}
