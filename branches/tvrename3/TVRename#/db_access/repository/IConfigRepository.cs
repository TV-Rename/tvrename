using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVRename.db_access.documents;
using TVRename.Settings;

namespace TVRename.db_access.repository
{
    public interface IConfigRepository : IEntityRepository<Config, ConfigDocument>
    {

    }
}
