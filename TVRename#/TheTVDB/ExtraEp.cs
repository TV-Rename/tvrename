// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    public class ExtraEp
    {
        public bool Done;
        public int EpisodeId;
        public int SeriesId;

        public ExtraEp(int series, int episode)
        {
            SeriesId = series;
            EpisodeId = episode;
            Done = false;
        }
    }
}
