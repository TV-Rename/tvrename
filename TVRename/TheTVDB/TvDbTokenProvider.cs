// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Newtonsoft.Json.Linq;
using System;
using JetBrains.Annotations;

namespace TVRename
{
    public class TvDbTokenProvider
    {
        [NotNull]
        // ReSharper disable once InconsistentNaming
        public static string TVDB_API_URL
        {
            get
            {
                switch (TheTVDB.VERS)
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    case ApiVersion.v2:
                        return "https://api.thetvdb.com";

                    // ReSharper disable once HeuristicUnreachableCode
                    // ReSharper disable once HeuristicUnreachableCode
                    case ApiVersion.v3:
                        // ReSharper disable once HeuristicUnreachableCode
                        return "https://api-dev.thetvdb.com";

                    default:
                        throw new NotSupportedException();
                }
            }
        }

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
            if (!TokenIsValid())
            {
                AcquireToken();
            }
            //If we have logged on and have a valid token that is nearing its use-by date then refresh
            if (ShouldRefreshToken())
            {
                 RefreshToken();
            }

            return lastKnownToken;
        }

        public void EnsureValid()
        {
            GetToken();
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
            JObject jsonResponse = HttpHelper.JsonHttpGetRequest($"{TVDB_API_URL}/refresh_token", lastKnownToken);

            UpdateToken((string)jsonResponse["token"]);
            Logger.Info("refreshed token at " + DateTime.UtcNow);
            Logger.Info("New Token " + lastKnownToken);
        }

        private void UpdateToken(string token)
        {
            lastKnownToken = token;
            lastRefreshTime = DateTime.Now;
        }

        private bool ShouldRefreshToken() => DateTime.Now - lastRefreshTime >= TimeSpan.FromHours(23);

        private bool TokenIsValid() => DateTime.Now - lastRefreshTime < TimeSpan.FromDays(1) - TimeSpan.FromMinutes(1);

        private bool IsTokenAquired() => lastKnownToken != string.Empty;
    }
}
