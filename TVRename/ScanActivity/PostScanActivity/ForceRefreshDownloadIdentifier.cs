using System.Linq;
using System.Threading;

namespace TVRename;

internal class ForceRefreshDownloadIdentifier : PostScanActivity
{
    private readonly DownloadIdentifiersController cx;
    private readonly string name;

    public ForceRefreshDownloadIdentifier(DownloadIdentifier action, TVDoc doc, string name) : base(doc)
    {
        cx = new DownloadIdentifiersController(action);
        this.name = name;
    }

    public override string ActivityName() => name;

    protected override bool Active() => true;

    protected override void DoCheck(PostScanProgressDelegate progress, CancellationToken token)
    {
        int totalRecords = MDoc.TvLibrary.Count;
        int currentRecord = 1;
        foreach (ShowConfiguration si in MDoc.TvLibrary.GetSortedShowItems())
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (!si.AutoAddFolderBase.HasValue() || !si.AllExistngFolderLocations().Any())
            {
                continue;
            }

            MDoc.TheActionList.AddNullableRange(cx.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadMetaData, si));

            progress(currentRecord++, totalRecords, si.Name ?? string.Empty, string.Empty);
        }
    }
}
