namespace TVRename
{
    public class Season
    {
        public SeriesInfo TheSeries;
        public System.Collections.Generic.List<Episode> Episodes;
        public int SeasonNumber;
        public int SeasonID;

        public Season(SeriesInfo theSeries, int number, int seasonid)
        {
            TheSeries = theSeries;
            SeasonNumber = number;
            SeasonID = seasonid;
            Episodes = new System.Collections.Generic.List<Episode>();
        }

    }
}