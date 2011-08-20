using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVRename.Shows;
using TVRename.db_access.documents;

namespace TVRename.db_access.repository
{
    public interface IShowItemRepository : IEntityRepository<ShowItem, ShowItemDocument>
    {

    }
}
