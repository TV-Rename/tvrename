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

namespace TVRename;

internal class FindNewShowsInLibrary : ScanActivity
{
    public FindNewShowsInLibrary(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
    }

    protected override string CheckName() => "Looked in the library for any new shows to be added (bulk add)";

    protected override void DoCheck(SetProgressDelegate progress)
    {
        BulkAddSeriesManager bam = new(MDoc);
        bam.CheckFolders(progress, false, !Settings.Unattended, Settings.Token);
        AskUserAboutShows(bam);

        if (!bam.AddItems.Any(s => s.CodeKnown))
        {
            return;
        }

        var idsToAdd = bam.AddItems.Where(s => s.CodeKnown).Select(folder => new { Code = folder.ProviderCode, folder.Provider }).ToList();

        bam.AddAllToMyShows(Settings.Owner);
        List<ShowConfiguration> addedShows = [.. idsToAdd.Select(s => MDoc.TvLibrary.GetShowItem(s.Code, s.Provider)).OfType<ShowConfiguration>()];

        //add each new show into the shows being scanned
        foreach (ShowConfiguration si in addedShows)
        {
            Settings.Shows.Add(si);
        }
        LOGGER.Info($"Added new shows called: {addedShows.Select(si => si.ShowName).ToCsv()}");

        MDoc.TvAddedOrEdited(true, Settings.Unattended, Settings.Hidden, Settings.Owner, addedShows);
    }

    private void AskUserAboutShows(BulkAddSeriesManager bam)
    {
        foreach (PossibleNewTvShow folder in bam.AddItems)
        {
            if (Settings.Token.IsCancellationRequested)
            {
                break;
            }

            AskUserAboutShow(folder, Settings.Owner);
        }
    }

    private void AskUserAboutShow(PossibleNewTvShow folder, IDialogParent owner)
    {
        if (folder.CodeKnown)
        {
            return;
        }

        BulkAddSeriesManager.GuessShowItem(folder, MDoc.TvLibrary, true);

        if (folder.CodeKnown)
        {
            return;
        }

        using BulkAddEditShow ed = new(folder);

        owner.ShowChildDialog(ed);
        DialogResult x = ed.DialogResult;
        int code = ed.Code;

        if (x != DialogResult.OK || code == -1)
        {
            return;
        }

        folder.UpdateId(code, ed.ProviderType);
    }

    public override bool Active() => TVSettings.Instance.DoBulkAddInScan;
}
