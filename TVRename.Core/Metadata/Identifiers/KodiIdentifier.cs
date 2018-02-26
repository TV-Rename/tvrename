using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;

namespace TVRename.Core.Metadata.Identifiers
{
    public class KodiIdentifier : TextIdentifier
    {
        public override string TextFormat => "Kodi XML";

        public override TargetTypes SupportedTypes => TargetTypes.Show | TargetTypes.Episode;

        protected override IAction ProcessShow(ProcessedShow show, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            return new KodiShowAction(show, file);
        }
        
        protected override IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            return new KodiEpisodeAction(episode , file);
        }
    }
}
