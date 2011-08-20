using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace TVRename.db_access.repository
{
    public abstract class EntityRepository<TEntity, TDoc> : IEntityRepository<TEntity, TDoc> where TEntity : IEntity<TDoc>
    {
        protected IDocumentSession documentSession;

        public EntityRepository(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public TEntity Load(string id)
        {
            if (id == null)
                return default(TEntity);

            return Create(this.documentSession.Load<TDoc>(id));
        }

        public void Add(TEntity entity)
        {
            this.documentSession.Store(entity.GetInnerDocument());
        }

        public void Remove(TEntity entity)
        {
            this.documentSession.Delete(entity.GetInnerDocument());
        }

        protected abstract TEntity Create(TDoc doc);
    }
}
