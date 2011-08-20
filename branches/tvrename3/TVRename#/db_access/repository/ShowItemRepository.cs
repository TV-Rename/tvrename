using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using TVRename.db_access.documents;
using TVRename.Shows;

namespace TVRename.db_access.repository
{
    public class ShowItemRepository : EntityRepository<ShowItem, ShowItemDocument>, IShowItemRepository
    {

        public ShowItemRepository(IDocumentSession documentSession) : base(documentSession) { }

        public List<ShowItem> getShowItems()
        {
            return documentSession.Query<ShowItem>().ToList<ShowItem>();
        }

        protected override ShowItem Create(ShowItemDocument doc)
        {
            return new ShowItem(doc);
        }
    }
}
