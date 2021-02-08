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
using System.Net.Cache;

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
                client.Headers.Add("user-agent", TVSettings.USER_AGENT);
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
                client.Headers.Add("user-agent", TVSettings.USER_AGENT);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return client.DownloadData(url);
            }

            return new byte[]{};
        }

        [NotNull]
        public static string HttpRequest([NotNull] string method, [NotNull] string url, string json, string contentType, string? token)
        {
            return HttpRequest(method, url, json, contentType, token, string.Empty );
        }

        [NotNull]
        public static string HttpRequest([NotNull] string method, [NotNull] string url, string? postContent,
            string? contentType, string? token, string? lang)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            if (!contentType.IsNullOrWhitespace())
            {
                httpWebRequest.ContentType = contentType;
            }
            httpWebRequest.Method = method;
            if (!token.IsNullOrWhitespace())
            {
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.Headers.Add("x-api-key", token);
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
                    streamWriter.Write(postContent);
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

        [NotNull]
        public static string Obtain([NotNull] string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream? stream = response.GetResponseStream())
            {
                if (stream == null)
                {
                    return string.Empty;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void LogWebException([NotNull] this Logger l,string message, [NotNull] WebException wex)
        {
            if (wex.IsUnimportant())
            {
                l.Warn(message+" "+wex.LoggableDetails());
            }
            else
            {
                l.Error(message + " " + wex.LoggableDetails());
            }
        }

        public static void LogIoException([NotNull] this Logger l, string message, [NotNull] IOException wex)
        {
            l.Warn(message + " " + wex.LoggableDetails());
        }

        public static void LogHttpRequestException([NotNull] this Logger l, string message, [NotNull] HttpRequestException wex)
        {
            if (wex.IsUnimportant())
            {
                l.Warn(message + " " + wex.LoggableDetails());
            }
            else
            {
                l.Error(message + " " + wex.LoggableDetails());
            }
        }

        private static bool IsUnimportant([NotNull] this WebException ex)
        {
            switch (ex.Status)
            {
                case WebExceptionStatus.Timeout:
                case WebExceptionStatus.NameResolutionFailure:
                case WebExceptionStatus.ConnectFailure:
                case WebExceptionStatus.ProtocolError:
                case WebExceptionStatus.TrustFailure:
                case WebExceptionStatus.RequestCanceled:
                case WebExceptionStatus.PipelineFailure:
                case WebExceptionStatus.ConnectionClosed:
                case WebExceptionStatus.ReceiveFailure:
                case WebExceptionStatus.SendFailure:
                case WebExceptionStatus.SecureChannelFailure:
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsUnimportant([NotNull] this HttpRequestException ex)
        {
            if (ex.InnerException is WebException wex)
            {
                return wex.IsUnimportant();
            }
            return true;
        }

        public static bool Is404([NotNull] this WebException ex)
        {
            if (ex.Status != WebExceptionStatus.ProtocolError)
            {
                return false;
            }

            if (!(ex.Response is HttpWebResponse resp))
            {
                return false;
            }

            return resp.StatusCode == HttpStatusCode.NotFound;
        }

        public static byte[] Download([NotNull] string url, bool forceReload)
        {
            WebClient wc = new WebClient();

            if (forceReload)
            {
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
            }

            return wc.DownloadData(url);
        }

        public static string LoggableDetails([NotNull] this IOException ex)
        {
            StringBuilder s = new StringBuilder();
            s.Append($"IOException obtained. {ex.Message}");
            if (ex.InnerException != null)
            {
                s.Append($". Further details: {ex.InnerException.Message}");
            }
            return s.ToString();
        }

        [NotNull]
        public static string LoggableDetails([NotNull] this WebException ex)
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
                if (ex.Response is HttpWebResponse webResponse)
                {
                    s.Append($" {webResponse.StatusCode}");
                }
            }
            if (ex.InnerException != null)
            {
                s.Append($". Further details: {ex.InnerException.Message}");
            }
            return s.ToString();
        }

        [NotNull]
        public static string LoggableDetails([NotNull] this HttpRequestException ex)
        {
            StringBuilder s = new StringBuilder();
            s.Append($"HttpRequestException obtained. {ex.Message}");
            {
                s.Append($" from {ex.TargetSite}");
            }
            if (ex.InnerException != null)
            {
                if (ex.InnerException is WebException wex)
                {
                    s.Append(wex.LoggableDetails());
                }
                else
                {
                    s.Append($". Further details: {ex.InnerException.Message}");
                }
            }
            return s.ToString();
        }

        [NotNull]
        public static JObject JsonHttpGetRequest([NotNull] string url, string? authToken) =>
            JObject.Parse(HttpRequest("GET",url, null, "application/json", authToken,string.Empty));

        [NotNull]
        public static JObject JsonHttpPostRequest( string url, JObject request, bool retry)
        {
            TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);

            string response=null;
            if (retry)
            {
                RetryOnException(3, pauseBetweenFailures, url, exception => true,
                    () => { response = HttpRequest("POST", url, request.ToString(), "application/json",string.Empty); },
                    null);
            }
            else
            {
                response = HttpRequest("POST", url, request.ToString(), "application/json", string.Empty);
            }

            return JObject.Parse(response);
        }

        [NotNull]
        public static string GetHttpParameters(Dictionary<string, string>? parameters)
        {
            if (parameters is null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("?");

            foreach (KeyValuePair<string, string> item in parameters)
            {
                sb.Append($"{item.Key}={WebUtility.UrlEncode(item.Value)}&");
            }
            string finalUrl = sb.ToString();
            return finalUrl.Remove(finalUrl.LastIndexOf("&", StringComparison.Ordinal));
        }

        public static void RetryOnException(int times,TimeSpan delay,string url, Func<Exception, bool> retryableException,[NotNull] System.Action operation, System.Action? updateOperation)
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
                    if (attempts == times || !retryableException(ex))
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

        public static JObject  HttpGetRequestWithRetry(string fullUrl, int times,int secondsGap) 
        {
            JObject response = null;
            TimeSpan gap = TimeSpan.FromSeconds(secondsGap);
            RetryOnException(times, gap, fullUrl,
                exception => exception is WebException wex && !wex.Is404()
                    ,() => { response = JsonHttpGetRequest(fullUrl, null); }
                    ,null);

            return response;
        }
    }
}
