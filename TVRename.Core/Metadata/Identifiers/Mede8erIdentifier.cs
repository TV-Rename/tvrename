using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;

namespace TVRename.Core.Metadata.Identifiers
{
    // ReSharper disable once InconsistentNaming
    public class Mede8erIdentifier : TextIdentifier
    {
        public override string TextFormat => "Mede8er XML";

        public override TargetTypes SupportedTypes => TargetTypes.Show | TargetTypes.Episode;

        protected override IAction ProcessShow(ProcessedShow show, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            return new Mede8erAction(show, file);
        }
        
        protected override IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;
            
            return null; // TODO
        }
    }
}
