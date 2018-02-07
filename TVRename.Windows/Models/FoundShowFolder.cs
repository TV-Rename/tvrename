namespace TVRename.Windows.Models
{
    public class FoundShowFolder
    {
        public string Name { get; set; }

        public int TvdbId { get; set; }

        public string Location { get; set; }

        public FolderStructure Structure { get; set; }

        public enum FolderStructure
        {
            Flat,
            Seasons
        }
    }
}
