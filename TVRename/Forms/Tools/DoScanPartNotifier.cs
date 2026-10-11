//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
namespace TVRename.Forms.Tools;

public class DoScanPartNotifier : TaskNotifier
{
    private readonly PostScanActivity activity;

    public DoScanPartNotifier(PostScanActivity activity, CancellationTokenSource cancellationToken) : base(activity.ActivityName(), cancellationToken)
    {
        this.activity = activity;
    }

    public async Task DoAsync()
    {
        var progressHandler = new Progress<TaskProgress>(UpdateProgress);

        task = activity.CheckAsync(progressHandler, cancellationToken.Token);
        Start(task);
        await task;
        Close();
    }
}

