// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    public static class TimeZoneHelper
    {
        public static IEnumerable<string> ZoneNames() => TimeZoneInfo.GetSystemTimeZones().Select(x=>x.StandardName);

        public static string DefaultTimeZone() => "Eastern Standard Time";

        public static string TimeZoneForNetwork(string network)
        {
            string[] uktv = { "Sky Atlantic (UK)", "BBC One", "Sky1", "BBC Two", "ITV", "Nick Jr.", "BBC Three", "Channel 4", "CBeebies", "Sky Box Office", "Watch", "ITV2", "National Geographic (UK)", "V", "ITV Encore", "ITV1", "BBC", "E4", "Channel 5 (UK)", "BBC Four", "ITVBe" };
            string[] ausTv = { "ABC4Kids", "Stan", "Showcase (AU)", "PBS Kids Sprout", "SBS (AU)", "Nine Network", "ABC1", "ABC (AU)" };
            if (string.IsNullOrWhiteSpace(network)) return DefaultTimeZone();
            if (uktv.Contains(network)) return "GMT Standard Time";
            if (ausTv.Contains(network)) return "AUS Eastern Standard Time";

            return DefaultTimeZone();
        }

        public static DateTime AdjustTzTimeToLocalTime(DateTime theirDateTime, TimeZoneInfo theirTimeZone)
        {
            return theirTimeZone == null ? theirDateTime : TimeZoneInfo.ConvertTime(theirDateTime,theirTimeZone,TimeZoneInfo.Local);
        }

        public static double Epoch(DateTime dt)
        {
            return dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }
    }
}
