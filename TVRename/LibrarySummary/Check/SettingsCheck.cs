using NLog;
using System;

namespace TVRename;

internal abstract class SettingsCheck
{
    public readonly TVDoc Doc;

    public abstract bool Check();

    // ReSharper disable once UnusedMember.Global - Property is referred to by the ObjectListView
    // ReSharper disable once MemberCanBeProtected.Global
    public abstract string Explain();

    // ReSharper disable once MemberCanBePrivate.Global- Property is referred to by the ObjectListView
    // ReSharper disable once UnusedAutoPropertyAccessor.Global- Property is referred to by the ObjectListView
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? ErrorText { get; private set; }

    public bool IsError { get; private set; }

    public void Fix()
    {
        try
        {
            IsError = false;
            ErrorText = string.Empty;
            FixInternal();
            MarkMediaDirty();
            Doc.SetDirty();
        }
        catch (FixCheckException e)
        {
            IsError = true;
            ErrorText = e.Message;
            LOGGER.Warn($"Error occurred fixing {Explain()}, error was {e.Message} for {MediaName}");
        }
        catch (Exception exception)
        {
            IsError = true;
            ErrorText = exception.Message;
            LOGGER.Error(exception,$"Error occurred fixing {Explain()}, for {MediaName}, error was {exception.Message}");
        }
    }

    protected abstract void FixInternal();

    protected abstract void MarkMediaDirty();

    // ReSharper disable once UnusedMember.Global- Property is referred to by the ObjectListView
    public abstract MediaConfiguration.MediaType Type();

    public abstract string MediaName { get; }

    public abstract string CheckName { get; }
    protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

    protected SettingsCheck(TVDoc doc)
    {
        Doc = doc;
    }
}
