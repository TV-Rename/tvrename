// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

namespace TVRename
{
    internal class MoviesTxt : MoviesExporter
    {
        public MoviesTxt(List<MovieConfiguration> shows) : base(shows)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportMoviesTXT;
        protected override string Location() => TVSettings.Instance.ExportMoviesTXTTo;

        protected override void Do()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
            {
                foreach (MovieConfiguration si in Shows)
                {
                    file.WriteLine(si.ShowName);
                }
            }
        }
    }
}
