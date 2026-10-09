//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class MovieFolderCheck(MovieConfiguration movie, TVDoc doc) : MovieCheck(movie, doc)
{
    protected override string MovieCheckName => "Use either manual or automatic folders";

    public override bool Check() => !Movie.UseAutomaticFolders && !Movie.UseManualLocations;

    public override string Explain() => $"{Movie.Name} does not use automated nor manual folders";

    /// <exception cref="FixCheckException">Condition.</exception>
    protected override void FixInternal()
    {
        if (!TVSettings.Instance.DefMovieUseAutomaticFolders)
        {
            throw new FixCheckException($"Please manually assign automatic/manual directory for {Movie.Name}");
        }

        Movie.UseAutomaticFolders = true;
    }
}
