// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace TVRename
{
    public static class JsonHelper
    {
        public static string Flatten(JToken ja) => Flatten(ja, ",");

        public static string Flatten(JToken ja,string delimiter)
        {
            if (ja == null) return "";

            if (ja.Type != JTokenType.Array) return "";

            JArray ja2 = (JArray)ja;
            string[] values = ja2.ToObject<string[]>();
            return string.Join(delimiter, values);
        }

        public static string Obtain(string url)
        {
            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
                if (stream != null)
                    using (StreamReader reader = new StreamReader(stream))
                        {
                            responseText = reader.ReadToEnd();
                        }

            return responseText;
        }
    }
}
