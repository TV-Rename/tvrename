using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;

namespace TVRename.Core.Metadata.Identifiers
{
    public class KodiIdentifier : TextIdentifier
    {
        public override string TextFormat => "Kodi XML";

        public override Target Target { get; set; }

        public override string Location { get; set; }

        public override string FileName { get; set; }

        protected override IAction ProcessShow(ProcessedShow show, FileInfo file, bool force = false)
        {
            return new DownloadAction(file, $@"{this.Location}\{this.FileName}"); // TODO: KodiAction
        }

        protected override IAction ProcessSeason(ProcessedShow show, ProcessedSeason season, FileInfo file, bool force = false)
        {
            return new DownloadAction(file, $@"{this.Location}\{this.FileName}"); // TODO: KodiAction
        }

        protected override IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false)
        {
            return new DownloadAction(file, $@"{this.Location}\{this.FileName}"); // TODO: KodiAction
        }
    }
}
