// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.IO;
using System.Net;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace TVRename
{
    public static class JsonHelper
    {
        [NotNull]
        public static string Flatten([CanBeNull]this JToken ja,string delimiter)
        {
            if (ja is null)
            {
                return string.Empty;
            }

            if (ja.Type != JTokenType.Array)
            {
                return string.Empty;
            }

            JArray ja2 = (JArray)ja;
            string[] values = ja2.ToObject<string[]>();
            return string.Join(delimiter, values);
        }

        [NotNull]
        public static string Obtain([NotNull] string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
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

        public static int ExtractStringToInt([NotNull] this JObject r, [NotNull] string key)
        {
            string valueAsString = (string)r[key];

            if (valueAsString.IsNullOrWhitespace())
            {
                return 0;
            }

            if (!int.TryParse(valueAsString, out int returnValue))
            {
                return 0;
            }

            return returnValue;
        }
    }
}
