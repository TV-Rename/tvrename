namespace TVRename.Core.Models.Cache
{
    /// <summary>
    /// Represents TheTVDB user rating of a television show.
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// Gets or sets the average score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public decimal Score { get; set; }

        /// <summary>
        /// Gets or sets the number of votes.
        /// </summary>
        /// <value>
        /// The votes.
        /// </value>
        public int Votes { get; set; }
    }
}
