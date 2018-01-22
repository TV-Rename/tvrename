using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;

namespace TVRename.Core.Metadata.Identifiers
{
    // ReSharper disable once InconsistentNaming
    public class Mede8erViewIdentifier : TextIdentifier
    {
        public override string TextFormat => "Mede8er View";

        public override Target Target { get; set; }

        public override string Location { get; set; }

        public override string FileName { get; set; }

        protected override IAction ProcessShow(ProcessedShow show, FileInfo file, bool force = false)
        {
            return new DownloadAction(file, $@"{this.Location}\{this.FileName}"); // TODO: Mede8erViewAction
        }

        protected override IAction ProcessSeason(ProcessedShow show, ProcessedSeason season, FileInfo file, bool force = false)
        {
            return new DownloadAction(file, $@"{this.Location}\{this.FileName}"); // TODO: Mede8erViewAction
        }

        protected override IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false)
        {
            return new DownloadAction(file, $@"{this.Location}\{this.FileName}"); // TODO: Mede8erViewAction
        }
    }
}
