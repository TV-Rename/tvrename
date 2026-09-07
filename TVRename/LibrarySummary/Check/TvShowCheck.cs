using System.Threading.Tasks;

namespace TVRename;

internal abstract class TvShowCheck(ShowConfiguration show, TVDoc doc) : SettingsCheck(doc)
{
    public readonly ShowConfiguration Show = show;

    protected override async void MarkMediaDirtyAsync()
    {
        if (Show.CachedShow != null)
        {
            Show.CachedShow.Dirty = true;
            await Doc.TvAddedOrEditedAsync(false, true, true, null, Show);
        }
    }

    public override MediaConfiguration.MediaType Type() => MediaConfiguration.MediaType.tv;

    public override string MediaName => Show.ShowName;
}
