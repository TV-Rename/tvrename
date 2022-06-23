using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public class ActionDownloadSeasonImage : ActionDownloadImage
{
    private readonly int seasonNumber;
    public ActionDownloadSeasonImage(ShowConfiguration si, int snum, FileInfo dest, string path)
        : this(si, snum, dest, path, false)
    {
    }

    public ActionDownloadSeasonImage(ShowConfiguration si, int snum, FileInfo dest, string path, bool shrink)
        : base(si, null, dest, path, shrink)
    {
        seasonNumber = snum;
    }

    public override string SeasonNumber => seasonNumber != 0 ? seasonNumber.ToString() : TVSettings.SpecialsListViewName;
    public override int? SeasonNumberAsInt => seasonNumber;
    public override ShowConfiguration? Series => Si as ShowConfiguration;
}