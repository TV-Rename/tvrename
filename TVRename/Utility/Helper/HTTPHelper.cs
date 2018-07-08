using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename
{
    public static class HttpHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string HttpRequest(string method, string url,string json, string contentType,string authToken = "", string lang = "") {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = method;
            if (authToken != "")
            {
                httpWebRequest.Headers.Add("Authorization", "Bearer " + authToken);
            }
            if (lang != "")
            {
                httpWebRequest.Headers.Add("Accept-Language",lang);
            }

            Logger.Trace("Obtaining {0}", url);

            if (method == "POST") { 
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
            }

            string result;
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            Logger.Trace("Returned {0}", result);
            return result;
        }

        public static JObject JsonHttpPostRequest( string url, JObject request)
        {
            string response = HttpRequest("POST",url, request.ToString(), "application/json");
            return JObject.Parse(response);
        }

        public static JObject JsonHttpGetRequest(string url, Dictionary<string, string> parameters, string authToken, string lang="")
        {
            string response = HttpRequest("GET", url + GetHttpParameters(parameters), null, "application/json", authToken,lang);
            return JObject.Parse(response);
        }

        private static string GetHttpParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("?");

            foreach (KeyValuePair<string,string>  item in parameters)
            {
                sb.Append($"{item.Key}={item.Value}&");
            }
            string finalUrl = sb.ToString();
            return finalUrl.Remove(finalUrl.LastIndexOf("&", StringComparison.Ordinal));
        }
    }
}
