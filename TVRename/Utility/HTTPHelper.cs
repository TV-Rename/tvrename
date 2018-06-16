using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename
{
    public static class HTTPHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static string HTTPRequest(string method, string url,string json, string contentType,string authToken = "", string lang = "") {
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

            logger.Trace("Obtaining {0}", url);

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
            logger.Trace("Returned {0}", result);
            return result;
        }

        public static JObject JsonHTTPPOSTRequest( string url, JObject request)
        {
            string response = HTTPRequest("POST",url, request.ToString(), "application/json");
            return JObject.Parse(response);
        }

        public static JObject JsonHTTPGETRequest(string url, Dictionary<string, string> parameters, string authToken, string lang="")
        {
            string response = HTTPRequest("GET", url + getHTTPParameters(parameters), null, "application/json", authToken,lang);
            return JObject.Parse(response);
        }

        public static string getHTTPParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("?");

            foreach (KeyValuePair<string,string>  item in parameters)
            {
                sb.Append($"{item.Key}={item.Value}&");
            }
            string finalUrl = sb.ToString();
            return finalUrl.Remove(finalUrl.LastIndexOf("&"));
        }
    }
}
