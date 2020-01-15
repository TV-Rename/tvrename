using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CloudFlareUtilities;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NLog;
using System.IO;

namespace TVRename
{
    public static class HttpHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [NotNull]
        internal static MultipartFormDataContent AddFile([NotNull] this MultipartFormDataContent @this,
            string name,
            [NotNull] string path,
            string contentType = "application/octet-stream")
        {
            FileStream stream = File.OpenRead(path);
            string fileName = Path.GetFileName(path);
            StreamContent content = new StreamContent(stream)
            {
                Headers = { ContentType = new MediaTypeHeaderValue(contentType) }
            };
            @this.Add(content, name, fileName);

            return @this;
        }

        public static string GetUrl(string url, bool useCloudflareProtection)
        {
            if (useCloudflareProtection)
            {
                try
                {
                    // Create a HttpClient that uses the handler to bypass CloudFlare's JavaScript challange.
                    HttpClient client = new HttpClient(new ClearanceHandler());

                    // Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
                    Task<string> task = Task.Run(async () => await client.GetStringAsync(url));
                    return task.Result;
                }
                catch (AggregateException ex) when (ex.InnerException is CloudFlareClearanceException)
                {
                    // After all retries, clearance still failed.
                }
                catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
                {
                    // Looks like we ran into a timeout. Too many clearance attempts?
                    // Maybe you should increase client.Timeout as each attempt will take about five seconds.
                }
            }
            else
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", TVSettings.Instance.USER_AGENT);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return client.DownloadString(url);
            }

            return string.Empty;
        }

        public static byte[] GetUrlBytes(string url, bool useCloudflareProtection)
        {
            if (useCloudflareProtection)
            {
                try
                {
                    // Create a HttpClient that uses the handler to bypass CloudFlare's JavaScript challange.
                    HttpClient client = new HttpClient(new ClearanceHandler());

                    // Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
                    Task<byte[]> task = Task.Run(async () => await client.GetByteArrayAsync(url));
                    return task.Result;
                }
                catch (AggregateException ex) when (ex.InnerException is CloudFlareClearanceException)
                {
                    // After all retries, clearance still failed.
                }
                catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
                {
                    // Looks like we ran into a timeout. Too many clearance attempts?
                    // Maybe you should increase client.Timeout as each attempt will take about five seconds.
                }
            }
            else
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", TVSettings.Instance.USER_AGENT);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return client.DownloadData(url);
            }

            return new byte[]{};
        }

        [NotNull]
        private static string HttpRequest([NotNull] string method, [NotNull] string url, string json, string contentType,
            [CanBeNull] TvDbTokenProvider authToken, string lang = "")
            => HttpRequest(method, url, json, contentType, authToken?.GetToken(), lang);

        [NotNull]
        private static string HttpRequest([NotNull] string method, [NotNull] string url, string json, string contentType, [CanBeNull] string token, string lang = "")
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = method;
            if (!string.IsNullOrWhiteSpace(token))
            {
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
            }
            if (lang != "")
            {
                httpWebRequest.Headers.Add("Accept-Language",lang);
            }

            Logger.Trace("Obtaining {0}", url);

            if (method == "POST")
            {
                using (StreamWriter streamWriter =
                    new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
            }

            string result;
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                result = streamReader.ReadToEnd();
            }
            Logger.Trace("Returned {0}", result);
            return result;
        }

        public static bool IsUnimportant(this WebException ex)
        {
            if (ex.Status == WebExceptionStatus.Timeout) return true;
            if (ex.Status == WebExceptionStatus.NameResolutionFailure) return true;
            return false;
        }
        public static string LoggableDetails(this WebException ex)
        {
            StringBuilder s = new StringBuilder();
            s.Append($"WebException {ex.Status} obtained. {ex.Message}");
            if (ex.Response == null)
            {
                s.Append(" with no response");
            }
            else
            {
                s.Append($" from {ex.Response.ResponseUri.OriginalString}");
                if (((HttpWebResponse)ex.Response)?.StatusCode != null)
                {
                    s.Append($" {((HttpWebResponse)ex.Response)?.StatusCode}");
                }
            }
            if (ex.InnerException != null)
            {
                s.Append($". Further details: {ex.InnerException.Message}");
            }
            return s.ToString();
        }

        public static JObject JsonHttpGetRequest(string url, Dictionary<string, string> parameters, TvDbTokenProvider  authToken, bool retry) =>
            JsonHttpGetRequest(url, parameters, authToken, string.Empty,retry);

        public static JObject JsonHttpGetRequest([NotNull] string url, string authToken) =>
            JObject.Parse(HttpRequest("GET",url, null, "application/json", authToken,string.Empty));

        public static JObject JsonHttpPostRequest( string url, JObject request, bool retry)
        {
            TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);

            string response=null;
            if (retry)
            {
                RetryOnException(3, pauseBetweenFailures, url,
                    () => { response = HttpRequest("POST", url, request.ToString(), "application/json",string.Empty); },
                    null);
            }
            else
            {
                response = HttpRequest("POST", url, request.ToString(), "application/json", string.Empty);
            }

            return JObject.Parse(response);
        }

        public static JObject JsonHttpGetRequest(string url, Dictionary<string, string> parameters, TvDbTokenProvider authToken, string lang, bool retry)
        {
            TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);
            string fullUrl = url + GetHttpParameters(parameters);

            string response = null;

            if (retry)
            {
                RetryOnException(3, pauseBetweenFailures, fullUrl,
                    () => { response = HttpRequest("GET", fullUrl, null, "application/json", authToken, lang); },
                    authToken.EnsureValid);
            }
            else
            {
                response = HttpRequest("GET", fullUrl, null, "application/json", authToken, lang);
            }

            return JObject.Parse(response);
        }

        [NotNull]
        private static string GetHttpParameters([CanBeNull] Dictionary<string, string> parameters)
        {
            if (parameters is null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("?");

            foreach (KeyValuePair<string,string>  item in parameters)
            {
                sb.Append($"{item.Key}={WebUtility.UrlEncode(item.Value)}&");
            }
            string finalUrl = sb.ToString();
            return finalUrl.Remove(finalUrl.LastIndexOf("&", StringComparison.Ordinal));
        }

        private static void RetryOnException(int times,TimeSpan delay,string url, [NotNull] System.Action operation, [CanBeNull] System.Action updateOperation)
        {
            if (times <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times));
            }

            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            int attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    operation();
                    break; // Success! Lets exit the loop!
                }
                catch (Exception ex)
                {
                    if (attempts == times)
                    {
                        Logger.Warn($"Exception caught on attempt {attempts} of {times} to get {url} - cancelling: {ex.Message}");
                        throw;
                    }

                    Logger.Warn($"Exception caught on attempt {attempts} of {times} to get {url} - will retry after delay {delay}: {ex.Message}");

                    Task.Delay(delay).Wait();

                    updateOperation?.Invoke();
                }
            } while (true);
        }

        public static async Task RetryOnExceptionAsync<TException>(int times, TimeSpan delay, string url, [NotNull] Func<Task> operation) where TException : Exception
        {
            if (times <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times));
            }

            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            int attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    await operation().ConfigureAwait(false);
                    break;
                }
                catch (TException ex)
                {
                    if (attempts == times)
                    {
                        Logger.Warn($"Exception caught on attempt {attempts} of {times} to get {url} - cancelling: {ex.Message}");
                        throw;
                    }

                    Logger.Warn($"Exception caught on attempt {attempts} of {times} to get {url} - will retry after delay {delay}: {ex.Message}");

                    await Task.Delay(delay).ConfigureAwait(false);
                }
            } while (true);
        }
    }
}
