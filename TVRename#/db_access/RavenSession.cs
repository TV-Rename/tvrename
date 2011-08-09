using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Embedded;
using Raven.Client;
using Raven.Http;

namespace TVRename.db_access
{
    public class RavenSession
    {
        private static RavenSession instance;
        private EmbeddableDocumentStore docStore;

        private RavenSession()
        {
            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080); 
           
            var documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = "RavenDB"
                //, UseEmbeddedHttpServer = true
            };
            documentStore.Initialize();
            docStore = documentStore;
        }

        public static IDocumentSession SessionInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RavenSession();
                }
                return instance.docStore.OpenSession();
            }
        }

    }
}