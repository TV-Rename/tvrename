//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TVRename;

public abstract class ScanActivity
{
    protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    protected readonly TVDoc MDoc;
    private SetProgressDelegate? progressDelegate;
    private int startPosition;
    private int endPosition;
    protected readonly TVDoc.ScanSettings Settings;

    protected ScanActivity(TVDoc doc, TVDoc.ScanSettings settings)
    {
        MDoc = doc;
        Settings = settings;
    }

    protected abstract string CheckName();

    public abstract bool Active();

    protected abstract void DoCheck(SetProgressDelegate progress);

    public void Check(SetProgressDelegate prog) =>
        Check(prog, 0, 100);

    public void Check(SetProgressDelegate prog, int startpct, int totPct)
    {
        startPosition = startpct;
        endPosition = totPct;
        progressDelegate = prog;
        progressDelegate?.Invoke(startPosition, string.Empty, string.Empty);

        try
        {
            if (Settings.Token.IsCancellationRequested)
            {
                return;
            }

            if (!Active())
            {
                return;
            }

            DoCheck(prog);
            LogActionListSummary();
        }
        catch (TVRenameOperationInterruptedException)
        {
            throw;
        }
        catch (TaskCanceledException sce)
        {
            LOGGER.Warn($"Failed to run Scan for {CheckName()} : {sce.Message}");
        }
        catch (Exception e)
        {
            LOGGER.Fatal(e, $"Failed to run Scan for {CheckName()}");
        }
        finally
        {
            progressDelegate?.Invoke(endPosition, string.Empty, string.Empty);
        }
    }

    protected void UpdateStatus(int recordNumber, int totalRecords, string message)
    {
        int position = (endPosition - startPosition) * recordNumber / (totalRecords + 1);
        progressDelegate?.Invoke(startPosition + position, message, string.Empty);
    }

    private void LogActionListSummary()
    {
        try
        {
            LOGGER.Info($"Summary of known actions after check: {CheckName()}");
            LOGGER.Info($"   Total Items: {MDoc.TheActionList.ToList().Count}");
            LOGGER.Info($"   Missing Items: {MDoc.TheActionList.Missing.ToList().Count}");
            LOGGER.Info($"   Copy/Move Items: {MDoc.TheActionList.CopyMoveRename.ToList().Count}");
            LOGGER.Info($"   Downloading Items: {MDoc.TheActionList.Downloading.ToList().Count}");
            LOGGER.Info($"   Total Actions: {MDoc.TheActionList.Actions.ToList().Count}");
        }
        catch (InvalidOperationException)
        {
            //sometimes get this if enumeration updates
        }
    }
}
