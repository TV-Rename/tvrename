using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename
{
    public static class HttpHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string HttpRequest(string method, string url, string json, string contentType,
            TvDbTokenProvider authToken, string lang = "")
            => HttpRequest(method, url, json, contentType, authToken?.GetToken(), lang);

        private static string HttpRequest(string method, string url, string json, string contentType, string token, string lang = "")
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

            if (method == "POST") { 
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
            }

            string result;
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                result = streamReader.ReadToEnd();
            }
            Logger.Trace("Returned {0}", result);
            return result;
        }

        public static JObject JsonHttpGetRequest(string url, Dictionary<string, string> parameters, TvDbTokenProvider  authToken, bool retry) =>
            JsonHttpGetRequest(url, parameters, authToken, string.Empty,retry);

        public static JObject JsonHttpGetRequest(string url, string authToken) =>
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
                    authToken.AcquireToken);
            }
            else
            {
                response = HttpRequest("GET", fullUrl, null, "application/json", authToken, lang);
            }

            return JObject.Parse(response);
        }

        private static string GetHttpParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("?");

            foreach (KeyValuePair<string,string>  item in parameters)
            {
                sb.Append($"{item.Key}={WebUtility.UrlEncode(item.Value)}&");
            }
            string finalUrl = sb.ToString();
            return finalUrl.Remove(finalUrl.LastIndexOf("&", StringComparison.Ordinal));
        }

        private static void RetryOnException(int times,TimeSpan delay,string url, [NotNull] System.Action operation, System.Action updateOperation)
        {
            if (times <= 0)
                throw new ArgumentOutOfRangeException(nameof(times));

            if (operation == null) throw new ArgumentNullException(nameof(operation));

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

        public static async Task RetryOnExceptionAsync<TException>(int times, TimeSpan delay, string url, Func<Task> operation) where TException : Exception
        {
            if (times <= 0)
                throw new ArgumentOutOfRangeException(nameof(times));

            if (operation == null) throw new ArgumentNullException(nameof(operation));

            int attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    await operation();
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

                    await Task.Delay(delay);
                }
            } while (true);
        }
    }
}
