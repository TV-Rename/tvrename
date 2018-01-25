using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;

namespace TVRename.Core.Metadata.Identifiers
{
    public class KodiIdentifier : TextIdentifier
    {
        public override string TextFormat => "Kodi XML";

        public override TargetTypes SupportedTypes => TargetTypes.Show | TargetTypes.Season | TargetTypes.Episode;

        protected override IAction ProcessShow(ProcessedShow show, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            return new KodiAction(show, file);
        }

        protected override IAction ProcessSeason(ProcessedShow show, ProcessedSeason season, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            return new KodiAction(show, file);
        }

        protected override IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            return new KodiAction(show, file);
        }
    }
}
