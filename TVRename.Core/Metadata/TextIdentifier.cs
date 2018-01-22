using Newtonsoft.Json;

namespace TVRename.Core.Metadata
{
    public abstract class TextIdentifier : Identifier
    {
        public override FileType FileType => FileType.Text;

        [JsonIgnore]
        public abstract string TextFormat { get; }
    }
}
