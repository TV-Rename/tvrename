// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Net;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace TVRename
{
    public static class JsonHelper
    {
        [NotNull]
        public static string Flatten(JToken ja) => Flatten(ja, ",");

        [NotNull]
        public static string Flatten([CanBeNull] JToken ja,string delimiter)
        {
            if (ja is null)
            {
                return "";
            }

            if (ja.Type != JTokenType.Array)
            {
                return "";
            }

            JArray ja2 = (JArray)ja;
            string[] values = ja2.ToObject<string[]>();
            return string.Join(delimiter, values);
        }

        [NotNull]
        public static string Obtain([NotNull] string url)
        {
            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (System.IO.Stream stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        responseText = reader.ReadToEnd();
                    }
                }
            }

            return responseText;
        }
    }
}
