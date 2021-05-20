//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;

namespace TVRename
{
    public abstract class PostScanActivity
    {
        protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        protected readonly TVDoc MDoc;
        private SetProgressDelegate? progressDelegate;
        private int startPosition;
        private int endPosition;

        protected PostScanActivity(TVDoc doc)
        {
            MDoc = doc;
            startPosition = 0;
            endPosition = 100;
        }

        protected abstract string ActivityName();

        protected abstract bool Active();

        protected abstract void DoCheck(SetProgressDelegate? progress);

        public void Check(SetProgressDelegate? progress) =>
            Check(progress, 0, 100);

        private void Check(SetProgressDelegate? progress, int startpct, int totPct)
        {
            startPosition = startpct;
            endPosition = totPct;
            progressDelegate = progress;
            progressDelegate?.Invoke(startpct, string.Empty);
            try
            {
                if (!Active())
                {
                    return;
                }

                DoCheck(progress);
                LogActionListSummary();
            }
            catch (TVRenameOperationInterruptedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LOGGER.Fatal(e, $"Failed to run Scan for {ActivityName()}");
            }
            finally
            {
                progressDelegate?.Invoke(totPct, string.Empty);
            }
        }

        protected void UpdateStatus(int recordNumber, int totalRecords, string message)
        {
            int position = (endPosition - startPosition) * recordNumber / (totalRecords + 1);
            progressDelegate?.Invoke(startPosition + position, message);
        }

        private void LogActionListSummary()
        {
            LOGGER.Info($"Completed after activity: {ActivityName()}");
        }
    }
}