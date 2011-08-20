using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename.db_access.repository
{
    public interface IEntityRepository<TEntity, TDocument> where TEntity : IEntity<TDocument>
    {
        TEntity Load(string id);
        void Add(TEntity entity);
        void Remove(TEntity entity);
    }
}
