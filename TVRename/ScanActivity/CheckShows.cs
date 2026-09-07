//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TVRename;

internal class CheckShows(TVDoc doc, TVDoc.ScanSettings settings) : ScanActivity(doc, settings)
{
    protected override string CheckName() => "Looked in the library to find missing files";

    /// <exception cref="TVRenameOperationInterruptedException">Condition.</exception>
    protected override async Task DoCheckAsync(SetProgressDelegate progress)
    {
        if (TVSettings.Instance.RenameCheck)
        {
            MDoc.Stats().RenameChecksDone++;
        }

        if (TVSettings.Instance.MissingCheck)
        {
            MDoc.Stats().MissingChecksDone++;
        }

        DirFilesCache dfc = new();

        if (Settings.Type == TVSettings.ScanType.Full && Settings.Shows.Any())
        {
            // only do episode count if we're doing all shows and seasons
            MDoc.CurrentStats.NsNumberOfEpisodes = 0;
        }

        //TODO - All these can happen in parallel

        int c = 0;
        UpdateStatus(c, Settings.Shows.Count, "Checking shows");
        foreach (ShowConfiguration si in Settings.Shows.OrderBy(item => item.ShowName))
        {
            if (Settings.Token.IsCancellationRequested)
            {
                return;
            }

            DoCheckForShow(dfc, c, si);
            c++;
        } // for each show

        c = 0;
        UpdateStatus(c, Settings.Movies.Count, "Checking movies");
        foreach (MovieConfiguration si in Settings.Movies.OrderBy(item => item.ShowName))
        {
            if (Settings.Token.IsCancellationRequested)
            {
                return;
            }

            DoCheckMovie(dfc, c, si);
            c++;
        } // for each movie

        MDoc.RemoveIgnored();
    }

    private void DoCheckMovie(DirFilesCache dfc, int c, MovieConfiguration si)
    {
        UpdateStatus(c, Settings.Movies.Count, si.ShowName);

        LOGGER.Info("Rename and missing check: " + si.ShowName);
        try
        {
            new CheckAllMovieFoldersExist(MDoc).CheckIfActive(si, dfc, Settings);
            new RenameAndMissingMovieCheck(MDoc).CheckIfActive(si, dfc, Settings);
        }
        catch (TVRenameOperationInterruptedException)
        {
            throw;
        }
        catch (FileNotFoundException e)
        {
            LOGGER.Warn(e, $"Failed to scan {si.ShowName}. Please double check settings for this movie: {si.Code}: {si}");
        }
        catch (Exception e)
        {
            LOGGER.Error(e, $"Failed to scan {si.ShowName}. Please double check settings for this movie: {si.Code}: {si}");
        }
    }

    private void DoCheckForShow(DirFilesCache dfc, int c, ShowConfiguration si)
    {
        UpdateStatus(c, Settings.Shows.Count, si.ShowName);

        LOGGER.Info("Rename and missing check: " + si.ShowName);
        try
        {
            new CheckAllFoldersExist(MDoc).CheckIfActive(si, dfc, Settings);
            new MergeLibraryEpisodes(MDoc).CheckIfActive(si, dfc, Settings);
            new RenameAndMissingCheck(MDoc).CheckIfActive(si, dfc, Settings);
        }
        catch (TVRenameOperationInterruptedException)
        {
            throw;
        }
        catch (Exception e)
        {
            LOGGER.Error(e, $"Failed to scan {si.ShowName}. Please double check settings for this show: {si}: {si.AutoAddFolderBase}");
        }
    }

    public override bool Active() => TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck;
}
