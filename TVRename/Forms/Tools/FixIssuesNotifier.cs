using System.ComponentModel;
using System.Threading;

namespace TVRename.Forms.Tools;

public class FixIssuesNotifier : Notifier
{
    private readonly LongOperation operation;

    public FixIssuesNotifier(LongOperation operation)
    {
        this.operation = operation;
        Start();
    }
    internal override void Do(BackgroundWorker backgroundWorker, CancellationTokenSource source)
    {
        operation.Start(source.Token,
            (percent, message, lastUpdate) => ReportProgress(source,percent,message,lastUpdate)
        );
    }

    internal override string ActionName() => "Fix Issues";
}
