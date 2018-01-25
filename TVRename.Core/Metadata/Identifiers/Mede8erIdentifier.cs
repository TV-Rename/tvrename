using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;

namespace TVRename.Core.Metadata.Identifiers
{
    // ReSharper disable once InconsistentNaming
    public class Mede8erIdentifier : TextIdentifier
    {
        public override string TextFormat => "Mede8er XML";

        protected override IAction ProcessShow(ProcessedShow show, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            return new Mede8erAction(show, file);
        }

        protected override IAction ProcessSeason(ProcessedShow show, ProcessedSeason season, FileInfo file, bool force = false)
        {
            return null;
        }

        protected override IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false)
        {
            return null;
        }
    }
}
