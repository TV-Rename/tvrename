using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace TVRename
{
    public static class JsonHelper {
        public static string Flatten(JToken ja,string delimiter = ",")
        {
            if (ja == null) return "";

            if (ja.Type != JTokenType.Array) return "";

            JArray ja2 = (JArray)ja;
            string[] values = ja2.ToObject<string[]>();
            return string.Join(delimiter, values);
        }

        internal static JArray ObtainArray(string url)
        {
            string responseText;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseText = reader.ReadToEnd();
            }

            return JArray.Parse(responseText);
        }

        internal static JToken ObtainToken(string url)
        {
            string responseText;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseText = reader.ReadToEnd();
            }

            return JToken.Parse(responseText);
        }

    }
}
