using CloudFlareUtilities;
using Humanizer;
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
        if (!url.IsWebLink())
        {
            //lets try this
            url = "https://" + url;
        }
        if (useCloudflareProtection)
        {
            try
            {
                // Create a HttpClient that uses the handler to bypass CloudFlare's JavaScript challange.
                using HttpClient client = new(new ClearanceHandler());

                // Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
                return TaskObtainStringFromUrl(url, client);
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
            Client.DefaultRequestHeaders.UserAgent.Clear();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(TVSettings.USER_AGENT);
            return TaskObtainStringFromUrl(url, Client);
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
            Client.DefaultRequestHeaders.UserAgent.Clear();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(TVSettings.USER_AGENT);
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
        using HttpClient newClient = new();
        Uri newClientBaseAddress = new(url);
        newClient.BaseAddress = newClientBaseAddress;
        if (!contentType.IsNullOrWhitespace())
        {
            newClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(contentType));
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

        Logger.Trace($"Obtaining {url}");

        if (method == "POST" && postContent != null)
        {
            StringContent content = new(postContent, Encoding.UTF8, "application/json");

            //POST the object to the specified URI
            HttpResponseMessage response = newClient.PostAsync(newClientBaseAddress, content).Result;

            //Read back the answer from server
            return response.Content.ReadAsStringAsync().Result;
        }

        return TaskObtainStringFromUrl(url, newClient);
    }

    private static string TaskObtainStringFromUrl(string url, HttpClient client)
        => Task.Run(ObtainStringFromUrlFunc(url, client)).Result;

    private static Func<Task<string>> ObtainStringFromUrlFunc(string url, HttpClient client)
    {
        try
        {
            return async () => await client.GetStringAsync(url);
        }
        catch (HttpRequestException hre)
        {
            Logger.Fatal(hre, $"Could not obtain {url}");
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException hre)
        {
            Logger.Fatal(hre, $"Could not obtain {url}");
        }

        return () => Task.FromResult(string.Empty);
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
    public static void LogTaskCanceledException(this Logger l, string message, TaskCanceledException wex)
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

    public static bool Is404(this HttpRequestException ex) => ex.StatusCode is HttpStatusCode.NotFound;

    public static byte[] Download(string url)
    {
        using HttpClient wc = new();
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
    public static string LoggableDetails(this TaskCanceledException ex)
    {
        return ex.InnerException != null
            ? $"TaskCanceledException obtained. {ex.Message}. Further details: {ex.InnerException.Message}"
            : $"TaskCanceledException obtained. {ex.Message}";
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
        string? response = null;

        void Operation()
        {
            response = HttpRequest("POST", url, request.ToString(), "application/json", string.Empty);
        }

        if (retry)
        {
            RetryOnException(3, 2.Seconds(), url, _ => true, Operation, null);
        }
        else
        {
            Operation();
        }

        return JObject.Parse(response ?? string.Empty);
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
                try
                {
                    updateOperation?.Invoke();
                }
                catch (Exception refreshException)
                {
                    Logger.Error($"Could not complete the update operation: {refreshException.Message}");
                }
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
        RetryOnException(times, secondsGap.Seconds(), fullUrl,
            RetryableWebException()
            , () => { response = JsonHttpGetRequest(fullUrl, null); }
            , null);

        return response!;
    }

    private static Func<Exception, bool> RetryableWebException()
    {
        return exception => exception.IsRetryable();
    }

    private static bool IsRetryable(this Exception e) => e is TaskCanceledException
                                                  || (e is WebException wex && !wex.Is404())
                                                  || e is System.IO.IOException
                                                  || (e is HttpRequestException hre && !hre.Is404())
                                                  || (e is AggregateException ae && ae.InnerException != null && ae.InnerException.IsRetryable());

    public static JArray HttpGetArrayRequestWithRetry(string fullUrl, int times, int secondsGap)
    {
        JArray? response = null;
        RetryOnException(times, secondsGap.Seconds(), fullUrl,
            RetryableWebException()
            , () => { response = JsonListHttpGetRequest(fullUrl, null); }
            , null);

        return response!;
    }
}
