// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace TVRename
{
    public class TvDbTokenProvider
    {
        public static readonly string TVDB_API_URL = "https://api.thetvdb.com";
        public static readonly string TVDB_API_KEY = "5FEC454623154441";

        private string lastKnownToken = string.Empty;
        private DateTime lastRefreshTime = DateTime.MinValue;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string GetToken()
        {
            if (IsTokenAquired() && TokenIsValid())
            {
                if (ShouldRefreshToken())
                {
                    logger.Info("Refreshing TheTVDB token... ");
                    RefreshToken();
                }

                return lastKnownToken;
            }

            AcquireToken();
            return lastKnownToken;
        }

        private void AcquireToken()
        {
            var request = new JObject(new JProperty("apikey", TVDB_API_KEY));
            var jsonResponse = HTTPHelper.JsonHTTPPOSTRequest($"{TVDB_API_URL}/login", request);

            UpdateToken((string)jsonResponse["token"]);
            logger.Info("Performed login at " + System.DateTime.UtcNow);
            logger.Info("New Token " + lastKnownToken);
        }

        private void RefreshToken()
        {
            var jsonResponse = HTTPHelper.JsonHTTPGETRequest($"{TVDB_API_URL}/refresh_token", null, lastKnownToken);

            UpdateToken((string)jsonResponse["token"]);
            logger.Info("refreshed token at " + System.DateTime.UtcNow);
            logger.Info("New Token " + lastKnownToken);
        }

        private bool ShouldRefreshToken()
        {
            return (DateTime.Now - lastRefreshTime) >= TimeSpan.FromHours(23);
        }

        private void UpdateToken(string token)
        {
            lastKnownToken = token;
            lastRefreshTime = DateTime.Now;
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
