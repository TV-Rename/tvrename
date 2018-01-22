using System.Drawing.Imaging;
using Newtonsoft.Json;

namespace TVRename.Core.Metadata
{
    public abstract class ImageIdentifier : Identifier
    {
        [JsonIgnore]
        public abstract override Target Target { get; set; }

        public override FileType FileType => FileType.Image;
        
        public abstract ImageType ImageType { get; set; }

        public abstract ImageFormat ImageFormat { get; set; }
    }
}
