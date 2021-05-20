namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class RSSItem
    {
        public readonly int Episode;
        public readonly int Season;
        public readonly string ShowName;
        public readonly string Title;

        // ReSharper disable once InconsistentNaming
        public readonly string URL;

        public readonly int Seeders;
        public readonly long Bytes;
        public readonly string UpstreamSource;

        public RSSItem(string url, string title, int season, int episode, string showName, int seeders, long bytes,
            string source)
        {
            URL = url;
            Season = season;
            Episode = episode;
            Title = title;
            ShowName = showName;
            Seeders = seeders;
            Bytes = bytes;
            UpstreamSource = source;
        }
    }
}