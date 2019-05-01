// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public static class TimeZoneHelper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [NotNull]
        public static IEnumerable<string> ZoneNames() => TimeZoneInfo.GetSystemTimeZones().Select(x=>x.StandardName);

        [NotNull]
        public static string DefaultTimeZone() => "Eastern Standard Time";

        [NotNull]
        public static string TimeZoneForNetwork([CanBeNull] string network)
        {
            string[] uktv = { "Sky Atlantic (UK)", "BBC One", "Sky1", "BBC Two", "ITV", "Nick Jr.", "BBC Three", "Channel 4", "CBeebies", "Sky Box Office", "Watch", "ITV2", "National Geographic (UK)", "V", "ITV Encore", "ITV1", "BBC", "E4", "Channel 5 (UK)", "BBC Four", "ITVBe" };
            string[] ausTv = { "ABC4Kids", "Stan", "Showcase (AU)", "PBS Kids Sprout", "SBS (AU)", "Nine Network", "ABC1", "ABC (AU)" };
            if (string.IsNullOrWhiteSpace(network))
            {
                return DefaultTimeZone();
            }

            if (uktv.Contains(network))
            {
                return "GMT Standard Time";
            }

            if (ausTv.Contains(network))
            {
                return "AUS Eastern Standard Time";
            }

            return DefaultTimeZone();
        }

        public static DateTime AdjustTzTimeToLocalTime(DateTime theirDateTime, [CanBeNull] TimeZoneInfo theirTimeZone)
        {
            try
            {
                return theirTimeZone is null
                    ? theirDateTime
                    : TimeZoneInfo.ConvertTime(theirDateTime, theirTimeZone, TimeZoneInfo.Local);
            }
            catch (ArgumentException)
            {
                try
                {
                    Debug.Assert(theirTimeZone != null, nameof(theirTimeZone) + " != null");
                    DateTime returnValue = TimeZoneInfo.ConvertTime(theirDateTime.AddHours(1), theirTimeZone, TimeZoneInfo.Local);
                    Logger.Warn($"Could not convert {theirDateTime.ToShortDateString()} {theirDateTime.ToShortTimeString()} {theirDateTime.Kind} in {theirTimeZone.StandardName} into {TimeZoneInfo.Local.StandardName} in TimeZoneHelper.AdjustTzTimeToLocalTime (added one hour and it worked ok to account for daylight savings)");
                    return returnValue;
                }
                catch (ArgumentException ae)
                {
                    Logger.Error($"Could not convert {theirDateTime.ToShortDateString()} {theirDateTime.ToShortTimeString()} {theirDateTime.Kind} in {theirTimeZone?.StandardName} into {TimeZoneInfo.Local.StandardName} in TimeZoneHelper.AdjustTzTimeToLocalTime (tried adding one hour too so that we account for daylight saving): {ae.Message}");
                    return theirDateTime;
                }
            }
        }

        public static double Epoch(DateTime dt)
        {
            return dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }
    }
}
