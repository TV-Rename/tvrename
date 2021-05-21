using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NLog;
using System;

namespace TVRename.TheTVDB
{
    internal class TokenProvider
    {
        [NotNull]
        // ReSharper disable once InconsistentNaming
        public static string TVDB_API_URL
        {
            get
            {
                return TVSettings.Instance.TvdbVersion switch
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    ApiVersion.v2 => "https://api.thetvdb.com",
                    // ReSharper disable once HeuristicUnreachableCode
                    // ReSharper disable once HeuristicUnreachableCode
                    ApiVersion.v3 => "https://api.thetvdb.com",
                    // ReSharper disable once HeuristicUnreachableCode
                    ApiVersion.v4 => "https://api4.thetvdb.com/v4",
                    _ => throw new NotSupportedException()
                };
            }
        }

        // ReSharper disable once InconsistentNaming
        public static string TVDB_API_KEY
        {
            get
            {
                return TVSettings.Instance.TvdbVersion switch
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    ApiVersion.v2 => "5FEC454623154441",
                    // ReSharper disable once HeuristicUnreachableCode
                    // ReSharper disable once HeuristicUnreachableCode
                    ApiVersion.v3 => "5FEC454623154441",
                    // ReSharper disable once HeuristicUnreachableCode
                    //ApiVersion.v4 => "b6bcc474-b211-4c8d-ac8c-2cfccab56e9b",
                    ApiVersion.v4 => "51020266-18f7-4382-81fc-75a4014fa59f",
                    _ => throw new NotSupportedException()
                };
            }
        }

        private string lastKnownToken = string.Empty;
        private DateTime lastRefreshTime = DateTime.MinValue;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Reset()
        {
            lastKnownToken = string.Empty;
            lastRefreshTime = DateTime.MinValue;
        }

        public string GetToken()
        {
            //If we have not logged on at all then logon
            if (!IsTokenAcquired())
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
            JObject request = new JObject(new JProperty("apikey", TVDB_API_KEY), new JProperty("pin", "TVDB_API_KEY"));
            JObject jsonResponse = HttpHelper.JsonHttpPostRequest($"{TVDB_API_URL}/login", request, true);

            string newToken = (TVSettings.Instance.TvdbVersion == ApiVersion.v4) ? (string)jsonResponse["data"]["token"] : (string)jsonResponse["token"];
            if (newToken == null)
            {
                Logger.Error("Could not refresh Token");
                return;
            }
            UpdateToken(newToken);

            Logger.Info("Performed login at " + DateTime.UtcNow);
            Logger.Info("New Token " + lastKnownToken);
        }

        private void RefreshToken()
        {
            Logger.Info("Refreshing TheTVDB token... ");
            JObject jsonResponse = HttpHelper.JsonHttpGetRequest($"{TVDB_API_URL}/refresh_token", lastKnownToken);

            string newToken = (string)jsonResponse["token"];
            if (newToken == null)
            {
                Logger.Error("Could not refresh Token");
                return;
            }
            UpdateToken(newToken);

            Logger.Info("Refreshed token at " + DateTime.UtcNow);
            Logger.Info("New Token " + lastKnownToken);
        }

        private void UpdateToken(string token)
        {
            lastKnownToken = token;
            lastRefreshTime = DateTime.Now;
        }

        private bool ShouldRefreshToken() => DateTime.Now - lastRefreshTime >= TimeSpan.FromHours(23);

        private bool TokenIsValid() => DateTime.Now - lastRefreshTime < TimeSpan.FromDays(1) - TimeSpan.FromMinutes(1);

        private bool IsTokenAcquired() => lastKnownToken != string.Empty;
    }
}
