// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    public class ExtraEp
    {
        public bool Done;
        public int EpisodeID;
        public int SeriesID;

        public ExtraEp(int series, int episode)
        {
            this.SeriesID = series;
            this.EpisodeID = episode;
            this.Done = false;
        }
    }
}