using System.Collections.Generic;
using JetBrains.Annotations;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class Image
    {
        public int Id { get; set; } // TODO: Nullable?

        public string FileName { get; set; }

        public string KeyType { get; set; }

        public int? LanguageId { get; set; } // TODO: Nullable?

        public Dictionary<string, double> RatingsInfo { get; set; } = new Dictionary<string, double>(); // TODO: Abstract type

        public string Resolution { get; set; }

        public string SubKey { get; set; }

        public string Thumbnail { get; set; }
    }
}
