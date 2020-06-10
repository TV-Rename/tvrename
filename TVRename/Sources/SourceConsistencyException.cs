using System;
using JetBrains.Annotations;

namespace TVRename
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class SourceConsistencyException : Exception
    {
        // Thrown if an error occurs in the XML when reading TheTVDB.xml
        public SourceConsistencyException(string message,ShowItem.ProviderType provider)
            : base(DataProviderName(provider)+": "+message)
        {
        }

        [NotNull]
        private static string DataProviderName(ShowItem.ProviderType provider)
        {
            return provider switch
            {
                ShowItem.ProviderType.TVmaze => "TVmaze",
                ShowItem.ProviderType.TheTVDB => "TheTVDB",
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }
    }
}
