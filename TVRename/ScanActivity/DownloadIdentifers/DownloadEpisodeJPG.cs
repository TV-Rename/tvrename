using System.Collections.Generic;
using System.IO;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename;

internal class DownloadEpisodeJpg : DownloadIdentifier
{
    private const string DEFAULT_EXTENSION = ".jpg";

    public override DownloadType GetDownloadType() => DownloadType.downloadImage;
    private List<string> doneJpg = [];

    public override void NotifyComplete(FileInfo file)
    {
        doneJpg.Add(file.FullName);
        base.NotifyComplete(file);
    }
    public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
    {
        if (!TVSettings.Instance.EpJPGs)
        {
            return null;
        }

        string? ban = episode.Filename;
        if (string.IsNullOrEmpty(ban))
        {
            return null;
        }

        string basefn = file.RemoveExtension();

        try
        {
            FileInfo imgjpg = FileHelper.FileInFolder(file.Directory, basefn + DEFAULT_EXTENSION);
            if (doneJpg.Contains(imgjpg.FullName))
            {
                return null;
            }

            if (!forceRefresh && imgjpg.Exists)
            {
                return null;
            }

            return [new ActionDownloadImage(episode.Show, episode, imgjpg, ban, TVSettings.Instance.ShrinkLargeMede8erImages)];
        }
        catch (DirectoryNotFoundException ex)
        {
            LOGGER.Warn(ex,"Failed to find directory to look for images for episode");
            return null;
        }
    }

    public sealed override void Reset()
    {
        doneJpg = [];
    }
}
