using System;
using System.Threading;

namespace TVRename.Forms.Tools;

public class FixIssuesNotifier : TaskNotifier
{
    public FixIssuesNotifier(LongOperation operation,CancellationTokenSource cts) : base("Fix Issues",cts)
    {
        var progressHandler = new Progress<TaskProgress>(UpdateProgress);

        task = operation.StartAsync(progressHandler, cts.Token);
    }
}
