using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TVRename.TVDB;

namespace TVRename.Tests.TVDB
{
    [TestClass]
    public class ClientTests
    {
        private Client client;

        [TestInitialize]
        public void Initialize()
        {
            this.client = new Client();
        }

        [TestMethod]
        public async Task GetLanguagesTest()
        {
            IEnumerable<Models.TVDB.Language> results = await this.client.GetLanguages();

            List<Models.TVDB.Language> languages = results.ToList();

            // Check multiple languages were returned
            Assert.IsTrue(languages.Count > 1);

            // Check results are ordered
            Assert.IsTrue(languages[0].Id < languages[1].Id);

            Models.TVDB.Language en = languages.First(l => l.Abbreviation == "en");

            // Check English exists
            Assert.IsNotNull(en);

            // Check fields were populted
            Assert.IsTrue(en.Name == "English");
            Assert.IsTrue(en.EnglishName == "English");
        }
    }
}
