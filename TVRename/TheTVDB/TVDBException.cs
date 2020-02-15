using System;

namespace TVRename.TheTVDB
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class TVDBException : Exception
    {
        // Thrown if an error occurs in the XML when reading TheTVDB.xml
        public TVDBException(string message)
            : base(message)
        {
        }
    }
}