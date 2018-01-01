using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Invokes multiple <see cref="DownloadIdentifier"/> instances returning results from all of them.
    /// </summary>
    internal class DownloadIdentifiersController
    {
        private readonly List<DownloadIdentifier> identifiers;

        public DownloadIdentifiersController()
        {
            this.identifiers = new List<DownloadIdentifier>
            {
                new DownloadFolderImage(),
                new DownloadSeriesImage(),
                new DownloadFanartImage(),
                new DownloadEpisodeImage(),
                new DownloadMede8erMetadata(),
                new DownloadpyTivoMetadata(),
                new DownloadKodiMetadata(),
                new DownloadKodiImages()
            };
        }

        public IEnumerable<Item> ProcessShow(ShowItem show)
        {
            return this.identifiers.SelectMany(i => i.ProcessShow(show));
        }

        public IEnumerable<Item> ProcessSeason(ShowItem show, string folder, int season)
        {
            return this.identifiers.SelectMany(i => i.ProcessSeason(show, folder, season));
        }

        public IEnumerable<Item> ProcessEpisode(ProcessedEpisode episode, FileInfo file)
        {
            return this.identifiers.SelectMany(i => i.ProcessEpisode(episode, file));
        }

        public IEnumerable<Item> ForceUpdateShow(DownloadType type, ShowItem show)
        {
            return this.identifiers.Where(i => i.Type == type).SelectMany(i => i.ProcessShow(show, true));
        }

        public IEnumerable<Item> ForceUpdateSeason(DownloadType type, ShowItem show, string folder, int season)
        {
            return this.identifiers.Where(i => i.Type == type).SelectMany(i => i.ProcessSeason(show, folder, season, true));
        }

        public IEnumerable<Item> ForceUpdateEpisode(DownloadType type, ProcessedEpisode episode, FileInfo file)
        {
            return this.identifiers.Where(i => i.Type == type).SelectMany(i => i.ProcessEpisode(episode, file, true));
        }

        public void MarkProcessed(FileInfo file)
        {
            foreach (DownloadIdentifier i in this.identifiers) i.MarkProcessed(file);
        }

        public void Reset()
        {
            foreach (DownloadIdentifier i in this.identifiers) i.Reset();
        }
    }
}
