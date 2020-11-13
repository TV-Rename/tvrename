// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Linq;

namespace TVRename
{
    public class FolderMonitorEntryList : System.Collections.Generic.List<PossibleNewTvShow>
    {
    }
    public class PossibleNewMovies : System.Collections.Generic.List<PossibleNewMovie>
    {
        public void AddIfNew(PossibleNewMovie ai)
        {
            if (this.Any(m => m.MovieStub.Equals(ai.MovieStub,StringComparison.CurrentCultureIgnoreCase)))
            {
                return;
            }
            Add(ai);
        }
    }
}
