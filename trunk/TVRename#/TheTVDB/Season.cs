// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    public class Season
    {
        public System.Collections.Generic.List<Episode> Episodes;
        public int SeasonID;
        public int SeasonNumber;
        public SeriesInfo TheSeries;

        public Season(SeriesInfo theSeries, int number, int seasonid)
        {
            this.TheSeries = theSeries;
            this.SeasonNumber = number;
            this.SeasonID = seasonid;
            this.Episodes = new System.Collections.Generic.List<Episode>();
        }
    }
}