//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

internal abstract class TvShowCheck(ShowConfiguration show, TVDoc doc) : SettingsCheck(doc)
{
    public readonly ShowConfiguration Show = show;

    protected override async Task MarkMediaDirtyAsync()
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
