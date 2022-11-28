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
using TVRename.Forms.Tools;

namespace TVRename;

public abstract class PostScanActivity :LongOperation
{
    protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    protected readonly TVDoc MDoc;
    private SetProgressDelegate? progressDelegate;
    private int startPosition;
    private int endPosition;

    protected delegate void PostScanProgressDelegate(int percent,int total, string message,string lastUpdate);
    protected PostScanActivity(TVDoc doc)
    {
        MDoc = doc;
        startPosition = 0;
        endPosition = 100;
    }

    public abstract string ActivityName();

    protected abstract bool Active();

    protected abstract void DoCheck( PostScanProgressDelegate progress, CancellationToken token);

    public override void Start(SetProgressDelegate? progress, CancellationToken sourceToken)
        => Check(progress, sourceToken);

    public void Check(SetProgressDelegate? progress, CancellationToken token)
        => Check(progress, 0, 100, token);

    private void Check(SetProgressDelegate? progress, int startpct, int totPct, CancellationToken token)
    {
        startPosition = startpct;
        endPosition = totPct;
        progressDelegate = progress;
        progressDelegate?.Invoke(startpct, string.Empty,string.Empty);
        try
        {
            if (!Active())
            {
                return;
            }

            DoCheck(UpdateStatus, token);
            LogActionListSummary();
        }
        catch (TVRenameOperationInterruptedException)
        {
            throw;
        }
        catch (System.Threading.Tasks.TaskCanceledException tce)
        {
            LOGGER.Warn($"Failed to run Scan for {ActivityName()} : {tce.Message}");
        }
        catch (Exception e)
        {
            LOGGER.Fatal(e, $"Failed to run Scan for {ActivityName()}");
        }
        finally
        {
            progressDelegate?.Invoke(totPct, string.Empty, string.Empty);
        }
    }

    private void UpdateStatus(int recordNumber, int totalRecords, string message,string lastAction)
    {
        int position = (endPosition - startPosition) * recordNumber / (totalRecords + 1);
        progressDelegate?.Invoke(startPosition + position, message, lastAction);
    }

    private void LogActionListSummary()
    {
        LOGGER.Info($"Completed after activity: {ActivityName()}");
    }
}
