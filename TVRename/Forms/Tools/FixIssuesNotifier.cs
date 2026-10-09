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
    public FixIssuesNotifier(LongOperation operation, CancellationTokenSource cts) : base("Fix Issues", cts)
    {
        var progressHandler = new Progress<TaskProgress>(UpdateProgress);

        task = operation.StartAsync(progressHandler, cts.Token);
    }
}
