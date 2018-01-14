using System.Collections.Generic;
using JetBrains.Annotations;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class PartialSeries
    {
        public List<string> Aliases { get; set; } = new List<string>();

        public string Banner { get; set; }

        public string FirstAired { get; set; }

        public int Id { get; set; }

        public string Network { get; set; }

        public string Overview { get; set; }

        public string SeriesName { get; set; }

        public Status Status { get; set; }
    }
}
