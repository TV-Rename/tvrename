//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class MovieImages : SafeList<MovieImage>
{
    public void MergeImages(MovieImages images)
    {
        if (!this.IsAny())
        {
            Clear();
            AddRange(images);
            return;
        }
        foreach (MovieImage i in images)
        {
            if (this.All(si => si.Id != i.Id))
            {
                Add(i);
            }
        }
    }
}
