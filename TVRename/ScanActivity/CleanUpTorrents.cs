using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    class CleanUpTorrents : ScanActivity
    {
        private readonly List<IDownloadProvider> sources;
        private ProcessedEpisode? lastFoundEpisode;
        private TorrentEntry? lastFoundEntry;
        public CleanUpTorrents([NotNull] TVDoc doc) : base(doc)
        {
            sources = new List<IDownloadProvider> {new qBitTorrent(), new uTorrent()};
        }

        protected override string CheckName() => "Cleaned up completed TV Torrents";
        public override bool Active() => true;//todo move to settings

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            DirFilesCache dfc = new DirFilesCache();
            foreach (IDownloadProvider source in sources)
            {
                List<TorrentEntry>? downloads = source.GetTorrentDownloads();
                if (downloads is null)
                {
                    continue;
                }

                var keys = downloads.GroupBy(entry => entry.DownloadingTo);

                foreach (IGrouping<string, TorrentEntry> torrentKey in keys)
                {
                    if(torrentKey.All(entry => CanRemove(entry,dfc)))
                    {
                        if (lastFoundEntry != null && lastFoundEpisode != null)
                        {
                            MDoc.TheActionList.Add(new ActionTRemove(source, lastFoundEntry, lastFoundEpisode));
                        }
                    }

                    lastFoundEntry = null;
                    lastFoundEpisode = null;
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
            FileInfo x = new FileInfo(download.DownloadingTo);
            if (!x.IsMovieFile())
            {
                return false;
            }
            List<ProcessedEpisode>? pes = MatchEpisodes(x);
            if (pes is null || pes.Count==0)
            {
                //File does not match any episodes
                return false;
            }
            if (!pes.All(episode => IsFound(dfc,episode)))
            {
                //Some Episodes have not been copied yet - wait until they have
                return false;
            }

            lastFoundEntry = download;
            lastFoundEpisode = pes.First();
            return true;
        }

        private bool IsFound(DirFilesCache dfc,ProcessedEpisode episode)
        {
            List<FileInfo> fl = dfc.FindEpOnDisk(episode);
            return fl.Any();
        }

        private List<ProcessedEpisode>? MatchEpisodes(FileInfo droppedFile)
        {
            ShowItem? bestShow = FinderHelper.FindBestMatchingShow(droppedFile, MDoc.Library.Shows);

            if (bestShow is null)
            {
                return null;
            }

            if (!FinderHelper.FindSeasEp(droppedFile, out int seasonNum, out int episodeNum, out int _, bestShow,
                out TVSettings.FilenameProcessorRE _))
            {
                return null;
            }

            ProcessedEpisode episode = bestShow.GetEpisode(seasonNum, episodeNum);

            return new List<ProcessedEpisode>() {episode};
        }
    }
}
