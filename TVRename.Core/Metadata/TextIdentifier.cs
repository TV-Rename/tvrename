using Newtonsoft.Json;

namespace TVRename.Core.Metadata
{
    public abstract class TextIdentifier : Identifier
    {
        public override FileType FileType => FileType.Text;

        public override Target Target { get; set; }

        [JsonIgnore]
        public abstract string TextFormat { get; }

        public override string ToString()
        {
            return this.TextFormat;
        }
    }
}
