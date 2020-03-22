// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.IO;
using System.Net;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename
{
    public static class JsonHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static DateTime? ParseAirTime([CanBeNull] string theTime)
        {
            try
            {
                if (!string.IsNullOrEmpty(theTime))
                {
                    if (DateTime.TryParse(theTime, out DateTime airsTime))
                    {
                        return airsTime;
                    }

                    if (DateTime.TryParse(theTime.Replace('.', ':'), out airsTime))
                    {
                        return airsTime;
                    }
                }
            }
            catch (FormatException)
            {
                Logger.Error("Failed to parse time: {0} ", theTime);
            }
            return DateTime.Parse("20:00");
        }

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
