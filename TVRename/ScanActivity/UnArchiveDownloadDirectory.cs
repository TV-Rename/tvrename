using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TVRename;

internal class UnArchiveDownloadDirectory(TVDoc doc, TVDoc.ScanSettings settings) : ScanActivity(doc, settings)
{
    public override bool Active() => TVSettings.Instance.UnArchiveFilesInDownloadDirectory;
    protected override string CheckName() => "Unarchived files in download directory";

    protected override async Task DoCheckAsync(SetProgressDelegate progress)
    {
        int totalDownloadFolders = TVSettings.Instance.DownloadFolders.Count;
        ThreadSafeCounter c = new();

        foreach (string dirPath in TVSettings.Instance.DownloadFolders.ToList())
        {
            UpdateStatus(c.Increment(), totalDownloadFolders, dirPath);

            if (!Directory.Exists(dirPath) || Settings.Token.IsCancellationRequested)
            {
                continue;
            }

            await ReviewFilesInDownloadDirectoryAsync(dirPath);
        }
    }

    private async Task ReviewFilesInDownloadDirectoryAsync(string dirPath)
    {
        try
        {
            foreach (string filePath in Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories).Where(File.Exists))
            {
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                FileInfo fi = new(filePath);

                if (!fi.IsArchiveFile())
                {
                    continue;
                }

                await ReviewArchiveAsync(fi);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            LOGGER.Warn(ex, $"Could not access files in {dirPath}");
        }
        catch (System.IO.DirectoryNotFoundException ex)
        {
            LOGGER.Warn(ex, $"Could not access files in {dirPath}");
        }
        catch (System.IO.IOException ex)
        {
            LOGGER.Warn(ex, $"Could not access files in {dirPath}");
        }
    }

    private async Task ReviewArchiveAsync(FileInfo fi)
    {
        List<ShowConfiguration> matchingShowsAll = [.. MDoc.TvLibrary.GetSortedShows().Where(si => si.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders))];
        List<ShowConfiguration> matchingShows = FinderHelper.RemoveShortShows(matchingShowsAll);
        List<MovieConfiguration> matchingMoviesAll = [.. MDoc.FilmLibrary.GetSortedMovies().Where(mi => mi.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders))];
        List<MovieConfiguration> matchingMovies = FinderHelper.RemoveShortShows(matchingMoviesAll);

        List<MovieConfiguration> matchingMoviesNoShows =
            FinderHelper.RemoveShortMedia(matchingMovies, matchingShows);
        List<ShowConfiguration> matchingShowsNoMovies =
            FinderHelper.RemoveShortMedia(matchingShows, matchingMovies);

        if (matchingShowsNoMovies.IsAny())
        {
            MDoc.TheActionList.Add(new ActionUnArchive(fi, matchingShowsNoMovies.First()));
            return;
        }

        if (await matchingMoviesNoShows.AnyAsync(x => HasMissingAsync(x, fi)))
        {
            MDoc.TheActionList.Add(new ActionUnArchive(fi, await matchingMoviesNoShows.FirstAsync(x => HasMissingAsync(x, fi))));
        }
    }

    private static async Task<bool> HasMissingAsync(MovieConfiguration x, FileInfo fi) => await FinderHelper.FileNeededAsync(fi, x, new DirFilesCache());
}
