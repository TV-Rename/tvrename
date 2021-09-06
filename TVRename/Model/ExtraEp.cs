//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
namespace TVRename
{
    public class ExtraEp
    {
        public bool Done;
        public readonly int EpisodeId;
        public readonly int SeriesId;
        public readonly ProcessedSeason.SeasonType Order;

        public ExtraEp(int series, int episode, ProcessedSeason.SeasonType st)
        {
            SeriesId = series;
            EpisodeId = episode;
            Done = false;
            Order = st;
        }
    }
}
