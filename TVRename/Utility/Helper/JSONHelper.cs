//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Globalization;

namespace TVRename
{
    public static class JsonHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static DateTime? ParseAirTime(string? theTime)
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

        public static bool ContainsTyped<T>(this JArray arr, T item)
        {
            return System.Linq.Enumerable.Any(arr, it =>
             {
                 try
                 {
                     return it.ToObject<T>()?.Equals(item) ?? false;
                 }
                 catch (Newtonsoft.Json.JsonException e)
                 {
                     Console.WriteLine("Couldn't parse array item {0} as type {1}: {2}", it, typeof(T), e);
                     return false;
                 }
             });
        }

        [NotNull]
        public static string Flatten(this JToken? ja, string delimiter)
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
            if (values != null)
            {
                return string.Join(delimiter, values);
            }

            return string.Empty;
        }

        public static int ExtractStringToInt([NotNull] this JObject r, [NotNull] string key)
        {
            string valueAsString = (string)r[key];

            if (!valueAsString.HasValue())
            {
                return 0;
            }

            if (!int.TryParse(valueAsString, out int returnValue))
            {
                return 0;
            }

            return returnValue;
        }

        public static DateTime? ParseFirstAired(string? theDate)
        {
            if (DateTime.TryParseExact(theDate, new[] { "yyyy-MM-dd", "yyyy-MM-d" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime output))
            {
                return output;
            }

            return null;
        }
    }
}
