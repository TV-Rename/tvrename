namespace TVRename;

public interface IDownloadInformation
{
    string FileIdentifier { get; }
    string Destination { get; }
    string RemainingText { get; }
}
