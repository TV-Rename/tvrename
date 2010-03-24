namespace TVRename
{
    public class ExtraEp
    {
        public int SeriesID;
        public int EpisodeID;
        public bool Done;

        public ExtraEp(int series, int episode)
        {
            SeriesID = series;
            EpisodeID = episode;
            Done = false;
        }
    }
}