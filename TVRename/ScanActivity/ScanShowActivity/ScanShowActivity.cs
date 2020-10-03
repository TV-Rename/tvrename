// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    public abstract class ScanShowActivity
    {
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
        protected readonly TVDoc Doc;

        protected ScanShowActivity(TVDoc doc) => Doc = doc;

        protected abstract void Check(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings);

        protected abstract bool Active();

        public void CheckIfActive(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (Active())
            {
                Check(si,dfc,settings);
                LogActionListSummary();
            }
        }

        private void LogActionListSummary()
        {
            try
            {
                LOGGER.Debug($"   Summary of known actions after check: {ActivityName()}");
                LOGGER.Debug($"      Missing Items: {Doc.TheActionList.Missing.Count}");
                LOGGER.Debug($"      Copy/Move Items: {Doc.TheActionList.CopyMoveRename.Count}");
                LOGGER.Debug($"      Total Actions: {Doc.TheActionList.Actions.Count}");
            }
            catch (System.InvalidOperationException)
            {
                //someties get this if enumeration updates
            }
        }

        protected abstract string ActivityName();
    }
}
