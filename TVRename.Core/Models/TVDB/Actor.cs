using System;
using JetBrains.Annotations;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class Actor
    {
        public int Id { get; set; }

        public string Image { get; set; }

        public string ImageAdded { get; set; }

        public int? ImageAuthor { get; set; }

        public DateTime LastUpdated { get; set; } // TODO: Safe?

        public string Name { get; set; }

        public string Role { get; set; }

        public int SeriesId { get; set; } // TODO: Nullable?

        public int SortOrder { get; set; } // TODO: Nullable?
    }
}
