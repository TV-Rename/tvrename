using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVRename.db_access.documents;

namespace TVRename.Settings
{
    class ConfigManager
    {
        public ConfigDocument loadConfigSettings()
        {
            return new ConfigDocument();
        }
    }
}
