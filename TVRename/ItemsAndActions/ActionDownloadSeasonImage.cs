using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public class ActionDownloadSeasonImage(ShowConfiguration si, int snum, FileInfo dest, string path, bool shrink) : ActionDownloadImage(si, null, dest, path, shrink)
{
    private readonly int seasonNumber = snum;
    public ActionDownloadSeasonImage(ShowConfiguration si, int snum, FileInfo dest, string path)
        : this(si, snum, dest, path, false)
    {
    }

    public override string SeasonNumber => seasonNumber != 0 ? seasonNumber.ToString() : TVSettings.SpecialsListViewName;
    public override int? SeasonNumberAsInt => seasonNumber;
    public override ShowConfiguration? Series => Si as ShowConfiguration;
}