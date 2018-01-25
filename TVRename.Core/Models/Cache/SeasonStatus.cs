namespace TVRename.Core.Models.Cache
{
    /// <summary>
    /// Describes a season episode airing status of a television show.
    /// </summary>
    public enum SeasonStatus
    {
        /// <summary>
        /// The season has fully aired.
        /// </summary>
        Aired,

        /// <summary>
        /// The season has partially aired.
        /// </summary>
        PartiallyAired,

        /// <summary>
        /// The season has not aired.
        /// </summary>
        NoneAired,

        /// <summary>
        /// The season has no episodes.
        /// </summary>
        NoEpisodes
    }
}
