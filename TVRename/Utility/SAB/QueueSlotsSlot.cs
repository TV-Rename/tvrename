namespace TVRename.SAB
{
    public class QueueSlotsSlot : IDownloadInformation
    {
        public string? Status { get; set; }
        public string? Mb { get; set; }
        public string? Filename { get; set; }
        public string? SizeLeft { get; set; }
        public string? TimeLeft { get; set; }

        string? IDownloadInformation.FileIdentifier => Filename;
        string? IDownloadInformation.Destination => Filename;

        string IDownloadInformation.RemainingText
        {
            get
            {
                string txt = $"{Status}, {SizeLeft}% Complete";
                if (Status == "Downloading")
                {
                    txt += $", {TimeLeft} left";
                }
                return txt;
            }
        }
    }
}
