using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVRename.db_access.documents;
using Raven.Client;
using TVRename.Settings;

namespace TVRename.db_access.repository
{
    public class ConfigRepository : EntityRepository<Config, ConfigDocument>, IConfigRepository
    {
        public ConfigRepository(IDocumentSession documentSession) : base(documentSession) { }

        public ConfigDocument loadConfigSettings()
        {
            IEnumerable<ConfigDocument> iter = GetConfigDocuments();
            foreach (ConfigDocument c in iter){
                return c;
            }
            return null;
        }

        //Get all ConfigDocument
        public IEnumerable<ConfigDocument> GetConfigDocuments()
        {
            var categories = documentSession.Advanced.LuceneQuery<ConfigDocument>()
                .ToArray();
            return categories;
        }

        //Insert/Update ConfigDocument
        public void Save(ConfigDocument category)
        {
            //store ConfigDocument object into session
            documentSession.Store(category);
            //save changes 
            documentSession.SaveChanges();
        }

        protected override Config Create(ConfigDocument doc)
        {
            return new Config(doc);
        }
    }
}
