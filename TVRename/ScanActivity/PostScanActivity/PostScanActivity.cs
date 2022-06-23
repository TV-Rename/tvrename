//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.Threading.Tasks;

namespace TVRename;

public abstract class PostScanActivity
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

    protected abstract void DoCheck(System.Threading.CancellationToken token, PostScanProgressDelegate progress);

    public void Check(System.Threading.CancellationToken token, SetProgressDelegate? progress) =>
        Check(token, progress, 0, 100);

    private void Check(System.Threading.CancellationToken token, SetProgressDelegate? progress, int startpct, int totPct)
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

            DoCheck(token, UpdateStatus);
            LogActionListSummary();
        }
        catch (TVRenameOperationInterruptedException)
        {
            throw;
        }
        catch (TaskCanceledException tce)
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
