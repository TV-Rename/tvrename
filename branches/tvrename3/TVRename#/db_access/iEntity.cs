using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename.db_access
{
    public interface IEntity<TDoc>
    {
        TDoc GetInnerDocument();
    }
}
