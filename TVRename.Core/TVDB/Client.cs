using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using TVRename.Core.Extensions;
using TVRename.Core.Models.TVDB;

namespace TVRename.Core.TVDB
{
    /// <summary>
    /// TheTVDB API client.
    /// </summary>
    public class Client
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The TheTVDB API base URL.
        /// </summary>
        public const string ApiUrl = "https://api.thetvdb.com/";

        /// <summary>
        /// The TheTVDB image base URL.
        /// </summary>
        public const string ImageUrl = "https://www.thetvdb.com/banners/";

        /// <summary>
        /// The TheTVDB API version.
        /// </summary>
        public const string ApiVersion = "2.1.2";

        /// <summary>
        /// The TheTVDB API key.
        /// </summary>
        private const string ApiKey = "5FEC454623154441";

        /// <summary>
        /// The active TheTVDB API token, if any.
        /// </summary>
        private string token;

        /// <summary>
        /// Gets or sets the default TheTVDB API language.
        /// </summary>
        /// <value>
        /// The TheTVDB API language.
        /// </value>
        public string Language { get; set; } = "en"; // TODO

        /// <summary>
        /// Gets a value indicating whether this <see cref="Client"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected => !string.IsNullOrEmpty(this.token);

        /// <summary>
        /// Makes a TheTVDB API request the specified HTTP method and optional JSON payload.
        /// Optionally connects to the API before the request.
        /// </summary>
        /// <param name="method">The HTTP method to use.</param>
        /// <param name="endpoint">The TheTVDB API endpoint.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <param name="json">The JSON payload. Ignored unless <c>method</c> is <c>HttpMethod.Post</c></param>
        /// <param name="connect">If set to <c>true</c> connect to TheTVDB API.</param>
        /// <returns>The TheTVDB API reponse parsed as JSON.</returns>
        /// <exception cref="System.Exception">Failed to connect to TheTVDB API</exception>
        /// <exception cref="HttpException">TheTVDB API exception</exception>
        private async Task<JObject> Request(HttpMethod method, string endpoint, CancellationToken ct, JObject json = null, bool connect = true)
        {
            // Connect if needed and requested
            if (!this.Connected && connect)
            {
                if (!await Login(ct)) throw new Exception("Failed to connect");
            }

            using (HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(ApiUrl)
            })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue($"application/vnd.thetvdb.v{ApiVersion}"));

                if (!string.IsNullOrEmpty(this.token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

                if (!string.IsNullOrEmpty(this.Language)) client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(this.Language));

                using (HttpRequestMessage request = new HttpRequestMessage(method, endpoint))
                {
                    if (method == HttpMethod.Post)
                    {
                        request.Content = new StringContent(json?.ToString(), Encoding.UTF8, "application/json");
                    }

                    Logger.Debug($"[API] {request.Method} {request.RequestUri}");

                    using (HttpResponseMessage response = await client.SendAsync(request, ct))
                    {
                        if (!response.IsSuccessStatusCode) throw new HttpException((int)response.StatusCode, await response.Content.ReadAsStringAsync());

                        ct.ThrowIfCancellationRequested();

                        return JObject.Parse(await response.Content.ReadAsStringAsync());
                    }
                }
            }
        }

        /// <summary>
        /// Logs in to the TheTVDB API with the API key, obtaining a bearer token.
        /// </summary>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns><c>true</c> if successful.</returns>
        public async Task<bool> Login(CancellationToken ct)
        {
            JObject json = new JObject(new JProperty("apikey", ApiKey));

            try
            {
                this.token = null;

                JObject response = await Request(HttpMethod.Post, "/login", ct, json, false);

                this.token = response.SelectToken("token").ToString();

                //KeepTVDBAliveTimer.Start(); // TODO: Automatic refresh

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                this.token = null;

                return false;
            }
        }

        /// <summary>
        /// Refresh the TheTVDB API bearer token.
        /// </summary>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns><c>true</c> if successful.</returns>
        public async Task<bool> RefreshToken(CancellationToken ct)
        {
            if (!this.Connected) return false;

            try
            {
                JObject response = await Request(HttpMethod.Get, "/refresh_token", ct);

                this.token = response.SelectToken("token").ToString();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                this.token = null;

                return false;
            }
        }

        /// <summary>
        /// Gets the supported TheTVDB languages.
        /// </summary>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns>List of supported languages.</returns>
        public async Task<IEnumerable<Language>> GetLanguages(CancellationToken ct)
        {
            JObject response = await Request(HttpMethod.Get, "/languages", ct);

            return response.SelectToken("data").ToObject<List<Language>>().OrderBy(l => l.Id);
        }

        /// <summary>
        /// Searches TheTVDB for series with the specified text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns>List of matching series.</returns>
        public async Task<List<PartialSeries>> Search(string text, CancellationToken ct)
        {
            text = Uri.EscapeDataString(text);
            //text = Helpers.RemoveDiacritics(text); // TODO: Remove special chars?

            try
            {
                JObject response = await Request(HttpMethod.Get, $"/search/series?name={text}", ct);

                return response.SelectToken("data").ToObject<List<PartialSeries>>();
            }
            catch (HttpException ex) when (ex.GetHttpCode() == 404)
            {
                Logger.Error(ex);

                return new List<PartialSeries>();
            }
        }

        /// <summary>
        /// Gets a series from TheTVDB API by series ID.
        /// </summary>
        /// <param name="id">TheTVDB series identifier.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <param name="episodes">If set to <c>true</c> also get series episodes.</param>
        /// <returns>The series.</returns>
        public async Task<Series> GetSeries(int id, CancellationToken ct, bool episodes)
        {
            JObject response = await Request(HttpMethod.Get, $"/series/{id}", ct);
            Series show = response.SelectToken("data").ToObject<Series>();

            ct.ThrowIfCancellationRequested();

            response = await Request(HttpMethod.Get, $"/series/{id}/actors", ct);
            show.Actors.AddRange(response.SelectToken("data").ToObject<List<Actor>>() ?? new List<Actor>());

            ct.ThrowIfCancellationRequested();

            response = await Request(HttpMethod.Get, $"/series/{id}/images", ct);
            List<string> imageTypes = response.SelectToken("data").ToObject<Dictionary<string, int>>().Where(d => d.Value > 0).Select(d => d.Key).ToList();

            ct.ThrowIfCancellationRequested();

            foreach (string imageType in imageTypes)
            {
                ct.ThrowIfCancellationRequested();

                response = await Request(HttpMethod.Get, $"/series/{id}/images/query?keyType={imageType}", ct);
                List<Image> a = response.SelectToken("data").ToObject<List<Image>>();
                show.Images.AddRange(a);
            }

            ct.ThrowIfCancellationRequested();

            if (episodes) show.Episodes = await GetEpisodes(id, ct);

            return show;
        }

        /// <summary>
        /// Gets all the partial episodes for a series from TheTVDB API by series ID.
        /// </summary>
        /// <param name="id">TheTVDB series identifier.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns>List of partial episodes.</returns>
        public async Task<List<PartialEpisode>> GetPartialEpisodes(int id, CancellationToken ct)
        {
            List<PartialEpisode> episodes = new List<PartialEpisode>();

            int page = 1;
            bool morePages = true;

            while (morePages)
            {
                JObject response = await Request(HttpMethod.Get, $"/series/{id}/episodes?page={page++}", ct);

                ct.ThrowIfCancellationRequested();

                Dictionary<string, int?> links = response.SelectToken("links").ToObject<Dictionary<string, int?>>();
                List<PartialEpisode> newEpisodes = response.SelectToken("data").ToObject<List<PartialEpisode>>();

                episodes.AddRange(newEpisodes);

                if (!links["next"].HasValue) morePages = false;
            }

            return episodes;
        }

        /// <summary>
        /// Gets all the episodes for a series from TheTVDB API by series ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns>List of episodes.</returns>
        public async Task<List<Episode>> GetEpisodes(int id, CancellationToken ct) // TODO: Parallel
        {
            List<PartialEpisode> partialEpisodes = await GetPartialEpisodes(id, ct);

            ct.ThrowIfCancellationRequested();

            List<Task<Episode>> tasks = partialEpisodes.Select(e => GetEpisode(e.Id, ct)).ToList();

            Episode[] results = await Task.WhenAll(tasks);

            return results.ToList();

            //return partialEpisodes.Select(e =>
            //{
            //    return GetEpisode(e.Id, ct);
            //}).Select(t =>
            //{
            //    return t.Result;
            //}).ToList();
        }

        /// <summary>
        /// Gets a episode from TheTVDB API by episode ID.
        /// </summary>
        /// <param name="id">TheTVDB episode identifier.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns>The episode.</returns>
        public async Task<Episode> GetEpisode(int id, CancellationToken ct)
        {
            JObject response = await Request(HttpMethod.Get, $"/episodes/{id}", ct);

            return response.SelectToken("data").ToObject<Episode>();
        }

        /// <summary>
        /// Gets the list of updates from TheTVDB API starting from a date.
        /// TheTVDB returns a maximum of seven days of updates.
        /// </summary>
        /// <param name="from">Timestamp to get updates from.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns>List of updates.</returns>
        public async Task<List<Update>> GetUpdates(DateTime from, CancellationToken ct)
        {
            JObject response = await Request(HttpMethod.Get, $"/updated/query?fromTime={from.ToUniversalTime().ToUnixTime()}", ct);

            return response.SelectToken("data").ToObject<List<Update>>() ?? new List<Update>();
        }
    }
}
