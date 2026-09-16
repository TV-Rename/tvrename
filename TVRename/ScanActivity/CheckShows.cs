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
using System.Threading;
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

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4 // Only 4 tasks will run concurrently at any given time
        };

        ThreadSafeCounter c = new();

        UpdateStatus(0, Settings.Shows.Count, "Checking shows");
        IEnumerable<(ShowConfiguration si, DirFilesCache dfc, ThreadSafeCounter c)> shows = Settings.Shows.OrderBy(item => item.ShowName).Select(si => (si, dfc, c));
        await Parallel.ForEachAsync(shows, options, async (show, cancellationToken) =>
        {
            await DoCheckForShowAsync(show.dfc, show.c, show.si, settings.Token);
        });// for each show

        c.Reset();
        UpdateStatus(0, Settings.Movies.Count, "Checking movies");
        IEnumerable<(MovieConfiguration si, DirFilesCache dfc, ThreadSafeCounter c)> movies = Settings.Movies.OrderBy(item => item.ShowName).Select(si => (si, dfc, c));
        await Parallel.ForEachAsync(movies, options, async (movie, cancellationToken) =>
        {
            await DoCheckMovieAsync(movie.dfc, movie.c, movie.si, settings.Token);
        }); // for each movie

        MDoc.RemoveIgnored();
    }

    private async Task DoCheckMovieAsync(DirFilesCache dfc, ThreadSafeCounter c, MovieConfiguration si, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }
        UpdateStatus(c.Value, Settings.Movies.Count, si.ShowName);

        LOGGER.Info("Rename and missing check: " + si.ShowName);
        try
        {
            await new CheckAllMovieFoldersExist(MDoc).CheckIfActiveAsync(si, dfc, Settings);
            await new RenameAndMissingMovieCheck(MDoc).CheckIfActiveAsync(si, dfc, Settings);
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
        finally
        {
            c.Increment();
        }
    }

    private async Task DoCheckForShowAsync(DirFilesCache dfc, ThreadSafeCounter c, ShowConfiguration si, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }
        UpdateStatus(c.Value, Settings.Shows.Count, si.ShowName);

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
        finally
        {
            c.Increment();
        }
    }

    public override bool Active() => TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck;
}
