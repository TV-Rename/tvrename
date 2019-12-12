// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using NLog;

namespace TVRename
{
    public abstract class PostScanActivity
    {
        protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        protected readonly TVDoc MDoc;
        private SetProgressDelegate progressDelegate;
        private int startPosition;
        private int endPosition;

        protected PostScanActivity(TVDoc doc) => MDoc = doc;

        protected abstract string Checkname();
        public abstract bool Active();
        protected abstract void DoCheck(SetProgressDelegate prog);

        public void Check(SetProgressDelegate prog) =>
            Check(prog, 0, 100);

        public void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            startPosition = startpct;
            endPosition = totPct;
            progressDelegate = prog;
            progressDelegate?.Invoke(startPosition, string.Empty);
            try
            {
                if (!Active())
                {
                    return;
                }

                DoCheck(prog);
                LogActionListSummary();
            }
            catch(TVRenameOperationInterruptedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LOGGER.Fatal(e, $"Failed to run Scan for {Checkname()}");
            }
            finally
            {
                progressDelegate?.Invoke(endPosition, string.Empty);
            }
        }

        protected void UpdateStatus(int recordNumber,int totalRecords, string message)
        {
            progressDelegate?.Invoke(startPosition + ((endPosition - startPosition) * recordNumber / (totalRecords+1)), message);
        }

        private void LogActionListSummary()
        {
            LOGGER.Info($"Completed after activity: {Checkname()}");
        }
    }
}
