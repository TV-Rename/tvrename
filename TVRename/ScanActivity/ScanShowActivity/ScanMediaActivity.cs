namespace TVRename;

public abstract class ScanMediaActivity(TVDoc doc)
{
    protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
    protected readonly TVDoc Doc = doc;

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
            //sometimes get this if enumeration updates
        }
    }

    protected abstract string ActivityName();
}
