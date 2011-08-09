using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using TVRename.db_access.documents;

namespace TVRename.db_access.repository
{
    class ShowItemRepository
    {
        private IDocumentSession session;

        public ShowItemRepository(IDocumentSession session)
        {
            this.session = session;
        }
    }
}
