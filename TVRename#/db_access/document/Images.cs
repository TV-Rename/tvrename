using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename.db_access.document
{
    class Images
    {
        public enum ImageType
        {
            banner,
            fanart,
            poster
        }

        public ImageType type { get; set; }
        public string path { get; set; }
    }
}
