//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class TvShowEpisodeNameCheck(ShowConfiguration show, TVDoc doc) : TvShowCheck(show, doc)
{
    public override bool Check() => Show.UseCustomNamingFormat;

    public override string Explain() => $"TV Show does not use the standard episode naming format {TVSettings.Instance.NamingStyle.StyleString}, it uses {Show.CustomNamingFormat}";

    protected override void FixInternal()
    {
        Show.UseCustomNamingFormat = false;
    }

    public override string CheckName => "[TV] Use Custom Folder Name Format";
}
