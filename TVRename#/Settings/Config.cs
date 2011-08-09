using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVRename.db_access;
using TVRename.db_access.documents;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using TVRename.db_access.repository;

namespace TVRename.Settings
{
    public class Config : IEntity<ConfigDocument>
    {
        public ConfigDocument innerDocument;
        private ConfigRepository repo;

        public Config() {
            repo = new ConfigRepository(RavenSession.SessionInstance);
            //if (this.innerDocument == null) innerDocument = new ConfigDocument();
            //this.SetToDefaults();
            this.innerDocument = repo.loadConfigSettings();

            // check that the configurations are not null.  if they
            // are create a new object, set it to defaults, and save it
            if (this.innerDocument == null)
            {
                this.innerDocument = DefaultObjectFactory.getDefaultConfigDocument();
                saveConfig();
            }
        }

        public static int TabNumberFromName(string n)
        {
            int r = 0;
            if (!string.IsNullOrEmpty(n))
                r = Array.IndexOf(TabNames(), n);
            if (r < 0)
                r = 0;
            return r;
        }

        public void saveConfig()
        {
            repo.Save(this.innerDocument);
        }

        ConfigDocument IEntity<ConfigDocument>.GetInnerDocument()
        {
            return this.innerDocument;
        }

        public static string[] TabNames()
        {
            return new string[] { "MyShows", "Scan", "WTW" };
        }

        public static string TabNameForNumber(int n)
        {
            if ((n >= 0) && (n < TabNames().Length))
                return TabNames()[n];
            return "";
        }

        public bool UsefulExtension(string sn, bool otherExtensionsToo)
        {
            foreach (string s in this.innerDocument.VideoExtensionsArray)
            {
                if (sn.ToLower() == s)
                    return true;
            }
            if (otherExtensionsToo)
            {
                foreach (string s in this.innerDocument.OtherExtensionsArray)
                {
                    if (sn.ToLower() == s)
                        return true;
                }
            }

            return false;
        }

        public string FilenameFriendly(string fn)
        {
            foreach (Replacement R in this.innerDocument.Replacements)
            {
                if (R.CaseInsensitive)
                    fn = Regex.Replace(fn, Regex.Escape(R.This), Regex.Escape(R.That), RegexOptions.IgnoreCase);
                else
                    fn = fn.Replace(R.This, R.That);
            }
            if (this.innerDocument.ForceLowercaseFilenames)
                fn = fn.ToLower();
            return fn;
        }
    }
}
