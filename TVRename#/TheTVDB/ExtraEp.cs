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
        public int EpisodeID;
        public int SeriesID;

        public ExtraEp(int series, int episode)
        {
            SeriesID = series;
            EpisodeID = episode;
            Done = false;
        }
    }
}
