using System.ComponentModel;
using System.Threading;

namespace TVRename.Forms.Tools;

public class DoScanPartNotifier : Notifier
{
    private readonly PostScanActivity activity;

    public DoScanPartNotifier(PostScanActivity activity)
    {
        this.activity = activity;
        Start();
    }

    internal override void Do(BackgroundWorker backgroundWorker, CancellationTokenSource source)
    {
        activity.Check((percent, message, lastUpdate) => ReportProgress(source, percent, message, lastUpdate), source.Token);
    }

    internal override string ActionName() => activity.ActivityName();
}
