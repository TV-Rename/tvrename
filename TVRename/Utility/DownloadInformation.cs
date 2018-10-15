namespace TVRename
{
    public interface DownloadInformation
    {
        string FileIdentifier { get; }
        string Destination { get; }
        string RemainingText { get; }
    }
}
