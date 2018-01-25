using System.Drawing.Imaging;
using Newtonsoft.Json;

namespace TVRename.Core.Metadata
{
    public abstract class ImageIdentifier : Identifier
    {
        [JsonIgnore]
        public abstract override Target Target { get; set; }

        public override FileType FileType => FileType.Image;
        
        public ImageType ImageType { get; set; }

        public ImageFormat ImageFormat { get; set; }
    }
}
