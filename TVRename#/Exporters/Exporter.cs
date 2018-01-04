namespace TVRename.Exporters
{
    /// <summary>
    /// Base exporter for saving data.
    /// </summary>
    internal abstract class Exporter
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets a value indicating whether this <see cref="Exporter"/> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        public abstract bool Active { get; }

        /// <summary>
        /// Gets the file location to export data to.
        /// </summary>
        /// <value>
        /// The file location.
        /// </value>
        protected abstract string Location { get; }
    }
}
