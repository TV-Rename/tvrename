using System;
using JetBrains.Annotations;

namespace TVRename
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class SourceConsistencyException : Exception
    {
        // Thrown if an error occurs in the XML when reading TheTVDB.xml
        public SourceConsistencyException(string message,TVDoc.ProviderType provider)
            : base(DataProviderName(provider)+": "+message)
        {
        }

        [NotNull]
        private static string DataProviderName(TVDoc.ProviderType provider)
        {
            return provider switch
            {
                TVDoc.ProviderType.TVmaze => "TVmaze",
                TVDoc.ProviderType.TheTVDB => "TheTVDB",
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }
    }
}
