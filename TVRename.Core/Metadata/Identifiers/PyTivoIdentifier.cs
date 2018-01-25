using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;

namespace TVRename.Core.Metadata.Identifiers
{
    public class PyTivoIdentifier : TextIdentifier
    {
        public override string TextFormat => "pyTivo";

        public override TargetTypes SupportedTypes => TargetTypes.Episode;

        protected override IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            return new PyTivoAction(show, season, episode, file);
        }
    }
}
