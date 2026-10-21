//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
namespace TVRename.Forms.Tools;

public class FixIssuesNotifier : TaskNotifier
{
    Progress<TaskProgress> progressHandler;
    public FixIssuesNotifier(CancellationTokenSource cts) : base("Fix Issues", cts)
    {
        progressHandler = new Progress<TaskProgress>(UpdateProgress);
    }
    public async Task Run(LongOperation operation)
    {
        task = operation.StartAsync(progressHandler, cancellationToken.Token);
        await Task.Yield();

        await task;
        Close();
    }
}
