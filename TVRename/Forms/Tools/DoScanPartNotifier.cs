using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename.Forms.Tools;

public class DoScanPartNotifier : TaskNotifier
{
    private readonly PostScanActivity activity;

    public DoScanPartNotifier(PostScanActivity activity, CancellationTokenSource cancellationToken) :base (activity.ActivityName(), cancellationToken)
    {
        this.activity = activity;
    }

    protected async Task DoAsync(IProgress<TaskProgress> sender, CancellationTokenSource source)
    {
        await activity.CheckAsync(sender, source.Token);

        Close();
    }
}

