using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVRename.db_access.documents;
using Raven.Client;

namespace TVRename.db_access.repository
{
    class ConfigRepository
    {
        private IDocumentSession session;

        public ConfigRepository(IDocumentSession session)
        {
            this.session = session;
        }

        public ConfigDocument loadConfigSettings()
        {
            IEnumerable<ConfigDocument> iter = GetConfigDocuments();
            foreach (ConfigDocument c in iter){
                return c;
            }
            return null;
        }

        //Load ConfigDocument based on Id
        public ConfigDocument Load(string id)
        {
            return session.Load<ConfigDocument>(id);
        }

        //Get all ConfigDocument
        public IEnumerable<ConfigDocument> GetConfigDocuments()
        {
            var categories = session.Advanced.LuceneQuery<ConfigDocument>()
                .ToArray();
            return categories;
        }

        //Insert/Update ConfigDocument
        public void Save(ConfigDocument category)
        {
            //store ConfigDocument object into session
            session.Store(category);
            //save changes 
            session.SaveChanges();
        }

        //delete a ConfigDocument
        public void Delete(string id)
        {
            var category = Load(id);
            session.Delete<ConfigDocument>(category);
            session.SaveChanges();
        }
    }
}
