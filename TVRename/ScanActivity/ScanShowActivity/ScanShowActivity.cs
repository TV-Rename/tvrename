// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using JetBrains.Annotations;

namespace TVRename
{
    public abstract class ScanShowActivity : ScanMediaActivity
    {
        protected abstract void Check(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings);

        public void CheckIfActive(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (Active())
            {
                Check(si,dfc,settings);
                LogActionListSummary();
            }
        }

        protected ScanShowActivity([NotNull] TVDoc doc) : base(doc)
        {
        }
    }

    public abstract class ScanMovieActivity : ScanMediaActivity
    {
        protected abstract void Check(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings);

        public void CheckIfActive(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (Active())
            {
                Check(si, dfc, settings);
                LogActionListSummary();
            }
        }

        protected ScanMovieActivity([NotNull] TVDoc doc) : base(doc)
        {
        }
    }

    public abstract class ScanMediaActivity
    {
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
        protected readonly TVDoc Doc;

        protected ScanMediaActivity(TVDoc doc) => Doc = doc;
        protected abstract bool Active();
        protected void LogActionListSummary()
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
