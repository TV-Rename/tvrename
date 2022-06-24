//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Globalization;

namespace TVRename;

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
            Logger.Error($"Failed to parse time: {theTime}");
        }
        return DateTime.Parse("20:00");
    }

    public static bool ContainsTyped<T>(this JArray arr, T? item)
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

    public static string Flatten(this JToken? ja, string delimiter)
    {
        if (ja?.Type != JTokenType.Array)
        {
            return string.Empty;
        }

        if (ja is not JArray ja2)
        {
            return string.Empty;
        }
        if (ja2.Count == 0)
        {
            return string.Empty;
        }

        try
        {
            string[]? values = ja2.ToObject<string[]>();
            if (values is null)
            {
                return string.Empty;
            }

            return string.Join(delimiter, values);
        }
        catch (NullReferenceException nre)
        {
            Logger.Error(nre,$"Problem flattening {ja}");
            return string.Empty;
        }
    }

    public static int ExtractStringToInt(this JObject r, string key)
    {
        string? valueAsString = (string?)r[key];

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

    public static int GetMandatoryInt(this JToken r, string key, TVDoc.ProviderType type)
    {
        return (int?)r[key] ?? throw new SourceConsistencyException($"Could not get data element '{key}' from {r}", type);
    }
    public static long GetMandatoryLong(this JToken r, string key, TVDoc.ProviderType type)
    {
        return (long?)r[key] ?? throw new SourceConsistencyException($"Could not get data element '{key}' from {r}", type);
    }
}
