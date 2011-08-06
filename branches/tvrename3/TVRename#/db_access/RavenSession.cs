using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Embedded;

namespace TVRename.db_access
{
    public class RavenSession
    {
        private static RavenSession instance;

        private RavenSession() {
            var documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = "App_Data\\tvrename\\RavenDB"
                //,
                //UseEmbeddedHttpServer = true
            };
            documentStore.Initialize();
        }

        public static RavenSession Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RavenSession();
                }
                return instance;
            }
        }

    }
}