using CloudFlareUtilities;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TVRename;

public static class HttpHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    // HttpClient is intended to be instantiated once per application, rather than per-use.
    private static readonly HttpClient Client = new();

    internal static MultipartFormDataContent AddFile(this MultipartFormDataContent @this,
        string name,
        string path,
        string contentType = "application/octet-stream")
    {
        System.IO.FileStream stream = System.IO.File.OpenRead(path);
        string fileName = System.IO.Path.GetFileName(path);
        StreamContent content = new(stream)
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
                HttpClient client = new(new ClearanceHandler());

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
            Client.DefaultRequestHeaders.Add("user-agent", TVSettings.USER_AGENT);
            HttpResponseMessage response = Client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            Task<string> task = Task.Run(async () => await Client.GetStringAsync(url));
            return task.Result;
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
                HttpClient cloudflareclient = new(new ClearanceHandler());

                // Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
                Task<byte[]> task = Task.Run(async () => await cloudflareclient.GetByteArrayAsync(url));
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
            Client.DefaultRequestHeaders.Add("user-agent", TVSettings.USER_AGENT);
            Task<byte[]> task = Task.Run(async () => await Client.GetByteArrayAsync(url));
            return task.Result;
        }

        return Array.Empty<byte>();
    }

    private static string HttpRequest(string method, string url, string json, string contentType, string? token)
    {
        return HttpRequest(method, url, json, contentType, token, string.Empty);
    }

    public static string HttpRequest(string method, string url, string? postContent,
        string? contentType, string? token, string? lang)
    {
        HttpClient newClient = new();
        Uri newClientBaseAddress = new(url);
        newClient.BaseAddress = newClientBaseAddress;
        if (!contentType.IsNullOrWhitespace())
        {
            newClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(contentType!));
        }

        if (!token.IsNullOrWhitespace())
        {
            newClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            newClient.DefaultRequestHeaders.Add("x-api-key", token);
        }
        if (lang.HasValue())
        {
            newClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(lang));
        }

        Logger.Trace($"Obtaining {url}" );

        if (method == "POST" && postContent !=null)
        {
            StringContent content = new(postContent, Encoding.UTF8, "application/json");

            //POST the object to the specified URI
            HttpResponseMessage response = newClient.PostAsync(newClientBaseAddress, content).Result;

            //Read back the answer from server
            return response.Content.ReadAsStringAsync().Result;
        }

        Task<string> task = Task.Run(async () => await newClient.GetStringAsync(url));
        return task.Result;
    }

    public static string Obtain(string url) => GetUrl(url, false);

    public static void LogWebException(this Logger l, string message, WebException wex)
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

    public static void LogIoException(this Logger l, string message, System.IO.IOException wex)
    {
        l.Warn(message + " " + wex.LoggableDetails());
    }

    public static void LogHttpRequestException(this Logger l, string message, HttpRequestException wex)
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

    private static bool IsUnimportant(this WebException ex)
    {
        return ex.Status switch
        {
            WebExceptionStatus.Timeout => true,
            WebExceptionStatus.NameResolutionFailure => true,
            WebExceptionStatus.ConnectFailure => true,
            WebExceptionStatus.ProtocolError => true,
            WebExceptionStatus.TrustFailure => true,
            WebExceptionStatus.RequestCanceled => true,
            WebExceptionStatus.PipelineFailure => true,
            WebExceptionStatus.ConnectionClosed => true,
            WebExceptionStatus.ReceiveFailure => true,
            WebExceptionStatus.SendFailure => true,
            WebExceptionStatus.SecureChannelFailure => true,
            _ => false
        };
    }

    private static bool IsUnimportant(this HttpRequestException ex)
    {
        if (ex.InnerException is WebException wex)
        {
            return wex.IsUnimportant();
        }
        return true;
    }

    public static bool Is404(this WebException ex)
    {
        if (ex.Status != WebExceptionStatus.ProtocolError)
        {
            return false;
        }

        if (ex.Response is not HttpWebResponse resp)
        {
            return false;
        }

        return resp.StatusCode == HttpStatusCode.NotFound;
    }

    public static byte[] Download(string url, bool forceReload)
    {
        HttpClient wc = new();

        if (forceReload)
        {
            //TODO wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
        }

        return wc.GetByteArrayAsync(url).Result;
    }

    public static string LoggableDetails(this System.IO.IOException ex)
    {
        StringBuilder s = new();
        s.Append($"IOException obtained. {ex.Message}");
        if (ex.InnerException != null)
        {
            s.Append($". Further details: {ex.InnerException.Message}");
        }
        return s.ToString();
    }

    public static string LoggableDetails(this WebException ex)
    {
        StringBuilder s = new();
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

    public static string LoggableDetails(this HttpRequestException ex)
    {
        StringBuilder s = new();
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

    public static JObject JsonHttpGetRequest(string url, string? authToken) =>
        JObject.Parse(HttpRequest("GET", url, null, "application/json", authToken, string.Empty));

    public static JArray JsonListHttpGetRequest(string url, string? authToken) =>
        JArray.Parse(HttpRequest("GET", url, null, "application/json", authToken, string.Empty));

    public static JObject JsonHttpPostRequest(string url, JObject request, bool retry)
    {
        TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);

        string? response = null;
        if (retry)
        {
            RetryOnException(3, pauseBetweenFailures, url, _ => true,
                () => { response = HttpRequest("POST", url, request.ToString(), "application/json", string.Empty); },
                null);
        }
        else
        {
            response = HttpRequest("POST", url, request.ToString(), "application/json", string.Empty);
        }

        return JObject.Parse(response??string.Empty);
    }

    public static string GetHttpParameters(Dictionary<string, string?>? parameters)
    {
        if (parameters is null)
        {
            return string.Empty;
        }

        StringBuilder sb = new();
        sb.Append('?');

        foreach (KeyValuePair<string, string?> item in parameters)
        {
            sb.Append($"{item.Key}={WebUtility.UrlEncode(item.Value)}&");
        }
        string finalUrl = sb.ToString();
        return finalUrl.Remove(finalUrl.LastIndexOf("&", StringComparison.Ordinal));
    }

    public static void RetryOnException(int times, TimeSpan delay, string url, Func<Exception, bool> retryableException, System.Action operation, System.Action? updateOperation)
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

    public static async Task RetryOnExceptionAsync<TException>(int times, TimeSpan delay, string url, Func<Task> operation) where TException : Exception
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

    public static JObject HttpGetRequestWithRetry(string fullUrl, int times, int secondsGap)
    {
        JObject? response = null;
        TimeSpan gap = TimeSpan.FromSeconds(secondsGap);
        RetryOnException(times, gap, fullUrl,
            exception => (exception is WebException wex && !wex.Is404()) || exception is System.IO.IOException
            , () => { response = JsonHttpGetRequest(fullUrl, null); }
            , null);

        return response!;
    }

    public static JArray HttpGetArrayRequestWithRetry(string fullUrl, int times, int secondsGap)
    {
        JArray? response = null;
        TimeSpan gap = TimeSpan.FromSeconds(secondsGap);
        RetryOnException(times, gap, fullUrl,
            exception => (exception is WebException wex && !wex.Is404()) || exception is System.IO.IOException
            , () => { response = JsonListHttpGetRequest(fullUrl, null); }
            , null);

        return response!;
    }
}
