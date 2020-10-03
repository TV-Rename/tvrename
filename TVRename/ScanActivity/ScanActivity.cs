// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace TVRename
{
    public abstract class ScanActivity
    {
        protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        protected readonly TVDoc MDoc;
        private SetProgressDelegate? progressDelegate;
        private int startPosition;
        private int endPosition;

        protected ScanActivity(TVDoc doc) => MDoc = doc;

        protected abstract string CheckName();
        public abstract bool Active();
        protected abstract void DoCheck(SetProgressDelegate prog, ICollection<ShowConfiguration> showList, TVDoc.ScanSettings settings);

        public void Check(SetProgressDelegate prog, List<ShowConfiguration> showList, TVDoc.ScanSettings settings) =>
            Check(prog, 0, 100, showList, settings);

        public void Check(SetProgressDelegate prog, int startpct, int totPct, ICollection<ShowConfiguration> showList,
            TVDoc.ScanSettings settings)
        {
            startPosition = startpct;
            endPosition = totPct;
            progressDelegate = prog;
            progressDelegate.Invoke(startPosition, string.Empty);
            try
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                if (!Active())
                {
                    return;
                }

                DoCheck(prog, showList, settings);
                LogActionListSummary();
            }
            catch(TVRenameOperationInterruptedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LOGGER.Fatal(e, $"Failed to run Scan for {CheckName()}");
            }
            finally
            {
                progressDelegate?.Invoke(endPosition, string.Empty);
            }
        }

        protected void UpdateStatus(int recordNumber,int totalRecords, string message)
        {
            int position = (endPosition - startPosition) * recordNumber / (totalRecords+1);
            progressDelegate?.Invoke(startPosition + position, message);
        }

        private void LogActionListSummary()
        {
            try
            {
                LOGGER.Info($"Summary of known actions after check: {CheckName()}");
                LOGGER.Info($"   Missing Items: {MDoc.TheActionList.Missing.ToList().Count}");
                LOGGER.Info($"   Copy/Move Items: {MDoc.TheActionList.CopyMoveRename.ToList().Count}");
                LOGGER.Info($"   Total Actions: {MDoc.TheActionList.Actions.ToList().Count}");
            }
            catch (InvalidOperationException)
            {
                //someties get this if enumeration updates
            }
        }
    }
}
