//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class CustomNameMovieCheck(MovieConfiguration movie, TVDoc doc) : CustomMovieCheck(movie, doc)
{
    protected override void FixInternal()
    {
        Movie.UseCustomShowName = false;
    }

    protected override string FieldName => "Use Custom Name";

    protected override bool Field => Movie.UseCustomShowName;

    protected override string? CustomFieldValue => Movie.CustomShowName;

    protected override string? DefaultFieldValue => Movie.CachedMovie?.Name;
}
