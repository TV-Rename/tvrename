// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Newtonsoft.Json.Linq;
using System;

namespace TVRename
{
    public class TvDbTokenProvider
    {
        //public static readonly string TVDB_API_URL = "https://api-dev.thetvdb.com";
        public static readonly string TVDB_API_URL = "https://api.thetvdb.com";
        public static readonly string TVDB_API_KEY = "5FEC454623154441";

        private string lastKnownToken = string.Empty;
        private DateTime lastRefreshTime = DateTime.MinValue;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string GetToken()
        {
            //If we have not logged on at all then logon
            if (!IsTokenAquired() )
            {
                AcquireToken();
            }
            //If we have logged in but the token has expired so logon again
            else if (!TokenIsValid())
            {
                AcquireToken();
            }
            //If we have logged on and have a valid token that is nearing its use-by date then refresh
            else if (ShouldRefreshToken())
            {
                 RefreshToken();
            }

            return lastKnownToken;
        }

        private void AcquireToken()
        {
            Logger.Info("Acquire a TheTVDB token... ");
            JObject request = new JObject(new JProperty("apikey", TVDB_API_KEY));
            JObject jsonResponse = HttpHelper.JsonHttpPostRequest($"{TVDB_API_URL}/login", request,true);

            UpdateToken((string)jsonResponse["token"]);
            Logger.Info("Performed login at " + DateTime.UtcNow);
            Logger.Info("New Token " + lastKnownToken);
        }

        private void RefreshToken()
        {
            Logger.Info("Refreshing TheTVDB token... ");
            JObject jsonResponse = HttpHelper.JsonHttpGetRequest($"{TVDB_API_URL}/refresh_token", null, lastKnownToken,true);

            UpdateToken((string)jsonResponse["token"]);
            Logger.Info("refreshed token at " + DateTime.UtcNow);
            Logger.Info("New Token " + lastKnownToken);
        }

        private void UpdateToken(string token)
        {
            lastKnownToken = token;
            lastRefreshTime = DateTime.Now;
        }

        private bool ShouldRefreshToken()
        {
            return (DateTime.Now - lastRefreshTime) >= TimeSpan.FromHours(23);
        }

        private bool TokenIsValid()
        {
            return (DateTime.Now - lastRefreshTime) < (TimeSpan.FromDays(1) - TimeSpan.FromMinutes(1));
        }

        private bool IsTokenAquired()
        {
            return lastKnownToken != string.Empty;
        }
    }
}
