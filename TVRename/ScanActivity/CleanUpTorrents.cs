using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class CleanUpTorrents : ScanActivity
{
    private readonly List<IDownloadProvider> sources;
    private ProcessedEpisode? lastFoundEpisode;
    private MovieConfiguration? lastFoundMovie;
    private TorrentEntry? lastFoundEntry;

    public CleanUpTorrents(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
        sources = [new qBitTorrent(), new uTorrent()];
    }

    protected override string CheckName() => "Cleaned up completed TV Torrents";

    public override bool Active() => TVSettings.Instance.RemoveCompletedTorrents;

    protected override void DoCheck(SetProgressDelegate progress)
    {
        DirFilesCache dfc = new();
        foreach (IDownloadProvider source in sources)
        {
            List<TorrentEntry>? downloads = source.GetTorrentDownloads();
            if (downloads is null)
            {
                continue;
            }

            IEnumerable<IGrouping<string, TorrentEntry>> keys = downloads.GroupBy(entry => entry.DownloadingTo);

            foreach (IGrouping<string, TorrentEntry> torrentKey in keys)
            {
                if (torrentKey.All(entry => CanRemove(entry, dfc)))
                {
                    if (lastFoundEntry != null && lastFoundEpisode != null)
                    {
                        MDoc.TheActionList.Add(new ActionTRemove(source, lastFoundEntry, lastFoundEpisode));
                    }
                    if (lastFoundEntry != null && lastFoundMovie != null)
                    {
                        MDoc.TheActionList.Add(new ActionTRemove(source, lastFoundEntry, lastFoundMovie));
                    }
                }

                lastFoundEntry = null;
                lastFoundEpisode = null;
                lastFoundMovie = null;
            }
        }
    }

    private bool CanRemove(TorrentEntry download, DirFilesCache dfc)
    {
        if (download.PercentDone < 100)
        {
            return false;
        }
        if (!download.Finished)
        {
            return false;
        }
        FileInfo x = new(download.DownloadingTo);
        if (!x.IsMovieFile())
        {
            return false;
        }

        List<ProcessedEpisode>? pes = MatchEpisodes(x);

        List<MovieConfiguration>? movies = MatchMovies(x);

        bool matchesSomeMovies = !(movies is null || movies.Count == 0);
        bool matchesSomeShows = !(pes is null || pes.Count == 0);

        if (!matchesSomeMovies && !matchesSomeShows)
        {
            //File does not match any movies or shows
            return false;
        }

        if (matchesSomeMovies && !movies!.All(movie => IsFound(dfc, movie)))
        {
            //Some Movies have not been copied yet - wait until they have
            return false;
        }

        if (matchesSomeShows && !pes!.All(episode => IsFound(dfc, episode)))
        {
            //Some Episodes have not been copied yet - wait until they have
            return false;
        }

        if (matchesSomeShows)
        {
            lastFoundEntry = download;
            lastFoundEpisode = pes!.First();
            lastFoundMovie = null;
            return true;
        }

        lastFoundEntry = download;
        lastFoundEpisode = null;
        lastFoundMovie = movies!.First();
        return true;
    }

    private List<MovieConfiguration>? MatchMovies(FileInfo droppedFile)
    {
        MovieConfiguration? bestShow = FinderHelper.FindBestMatchingMovie(droppedFile, MDoc.FilmLibrary.Movies);

        if (bestShow is null)
        {
            return null;
        }

        return [bestShow];
    }

    private static bool IsFound(DirFilesCache dfc, ProcessedEpisode episode)
    {
        List<FileInfo> fl = dfc.FindEpOnDisk(episode);
        return fl.Any();
    }

    private static bool IsFound(DirFilesCache dfc, MovieConfiguration movie) => dfc.FindMovieOnDisk(movie).Any();

    private List<ProcessedEpisode>? MatchEpisodes(FileInfo droppedFile)
    {
        ShowConfiguration? bestShow = FinderHelper.FindBestMatchingShow(droppedFile, MDoc.TvLibrary.Shows);

        if (bestShow is null)
        {
            return null;
        }

        if (!FinderHelper.FindSeasEp(droppedFile, out int seasonNum, out int episodeNum, out int _, bestShow,
                out TVSettings.FilenameProcessorRE? _))
        {
            return null;
        }

        try
        {
            ProcessedEpisode episode = bestShow.GetEpisode(seasonNum, episodeNum);

            return [episode];
        }
        catch (EpisodeNotFoundException)
        {
            return null;
        }
    }
}
