//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using TVRename.Forms.Tools;

namespace TVRename;

public abstract class PostScanActivity(TVDoc doc) : LongOperation
{
    protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    protected readonly TVDoc MDoc = doc;
    private IProgress<TaskProgress>? progressDelegate;
    private int startPosition = 0;
    private int endPosition = 100;

    public abstract string ActivityName();

    protected abstract bool Active();

    protected abstract Task DoCheckAsync(CancellationToken token);

    public override async Task StartAsync(IProgress<TaskProgress> progress, CancellationToken sourceToken)
        => await CheckAsync(progress, sourceToken);

    public async Task CheckAsync(IProgress<TaskProgress> progress, CancellationToken token)
        => await CheckAsync(progress, 0, 100, token);

    private async Task CheckAsync(IProgress<TaskProgress> progress, int startpct, int totPct, CancellationToken token)
    {
        startPosition = startpct;
        endPosition = totPct;
        progressDelegate = progress;
        progressDelegate.Report(new TaskProgress(startpct, string.Empty, string.Empty));
        try
        {
            if (!Active())
            {
                return;
            }

            await DoCheckAsync(token);
            LogActionListSummary();
        }
        catch (TVRenameOperationInterruptedException)
        {
            throw;
        }
        catch (System.Threading.Tasks.TaskCanceledException tce)
        {
            LOGGER.Warn($"Failed to run Scan for {ActivityName()} : {tce.ErrorText()}");
        }
        catch (Exception e)
        {
            LOGGER.Fatal(e, $"Failed to run Scan for {ActivityName()}");
        }
        finally
        {
            progressDelegate.Report(new TaskProgress(totPct, string.Empty, string.Empty));
        }
    }

    protected void UpdateStatus(int recordNumber, int totalRecords, string message, string lastAction)
    {
        int position = (endPosition - startPosition) * recordNumber / (totalRecords + 1);
        progressDelegate?.Report(new TaskProgress(startPosition + position, message, lastAction));
    }

    private void LogActionListSummary()
    {
        LOGGER.Info($"Completed after activity: {ActivityName()}");
    }
}
