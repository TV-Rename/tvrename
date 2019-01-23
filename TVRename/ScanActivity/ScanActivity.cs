// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private SetProgressDelegate progressDelegate;
        private int startPosition;
        private int endPosition;

        protected ScanActivity(TVDoc doc) => MDoc = doc;

        protected abstract string Checkname();
        public abstract bool Active();
        protected abstract void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings);

        public void Check(SetProgressDelegate prog, List<ShowItem> showList, TVDoc.ScanSettings settings) =>
            Check(prog, 0, 100, showList, settings);

        public void Check(SetProgressDelegate prog, int startpct, int totPct, ICollection<ShowItem> showList,
            TVDoc.ScanSettings settings)
        {
            startPosition = startpct;
            endPosition = totPct;
            progressDelegate = prog;
            progressDelegate.Invoke(startPosition, string.Empty);
            try
            {
                if (settings.Token.IsCancellationRequested) return;
                if (!Active()) return;

                DoCheck(prog, showList, settings);
                LogActionListSummary();
            }
            catch(TVRenameOperationInteruptedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LOGGER.Fatal(e, $"Failed to run Scan for {Checkname()}");
            }
            finally
            {
                progressDelegate.Invoke(endPosition, string.Empty);
            }
        }

        protected void UpdateStatus(int recordNumber,int totalRecords, string message)
        {
            progressDelegate.Invoke(startPosition + ((endPosition - startPosition) * recordNumber / (totalRecords+1)), message);
        }

        private void LogActionListSummary()
        {
            LOGGER.Info($"Summary of known actions after check: {Checkname()}");
            LOGGER.Info($"   Missing Items: {MDoc.TheActionList.MissingItems().Count()}");
            LOGGER.Info($"   Copy/Move Items: {MDoc.TheActionList.CopyMoveItems().Count()}");
            LOGGER.Info($"   Total Actions: {MDoc.TheActionList.Actions().Count()}");
        }
    }
}
