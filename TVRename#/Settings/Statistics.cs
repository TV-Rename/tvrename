using TVRename.Utility;

namespace TVRename.Settings
{
    /// <summary>
    /// Stores and represents application statistics.
    /// See <see cref="JsonSettings{T}"/>.
    /// </summary>
    /// <seealso cref="JsonSettings{Statistics}" />
    /// <inheritdoc />
    public class Statistics : JsonSettings<Statistics>
    {
        public int AutoAddedShows { get; set; }

        public int FilesCopied { get; set; }

        public int FilesMoved { get; set; }

        public int FilesRenamed { get; set; }

        public int FindAndOrganisesDone { get; set; }

        public int MissingChecksDone { get; set; }

        public int RenameChecksDone { get; set; }

        public int TorrentsMatched { get; set; }
    }
}
