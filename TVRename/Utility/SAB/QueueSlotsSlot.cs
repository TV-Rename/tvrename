namespace TVRename.SAB
{
    public class QueueSlotsSlot : IDownloadInformation
    {
        public string? Status { get; set; }
        public string? Mb { get; set; }
        public string? Filename { get; set; }
        public string? Sizeleft { get; set; }
        public string? Timeleft { get; set; }

        string? IDownloadInformation.FileIdentifier => Filename;
        string? IDownloadInformation.Destination => Filename;

        string IDownloadInformation.RemainingText
        {
            get
            {
                string txt = $"{Status}, {Sizeleft}% Complete";
                if (Status == "Downloading")
                {
                    txt += $", {Timeleft} left";
                }
                return txt;
            }
        }
    }
}