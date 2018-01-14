using JetBrains.Annotations;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class Language
    {
        public string Abbreviation { get; set; }

        public string EnglishName { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
