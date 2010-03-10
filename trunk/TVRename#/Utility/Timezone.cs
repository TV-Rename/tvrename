//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

// Do conversions between timezones, handling daylight savings time (summer time) at both ends.
// Standard DateTime and DateTimeOffset classes in .NET 2.0 can't do this.
//
// If we were targeting .NET 3.5 or later, then we'd use System.Core.TimeZoneInfo and get rid of 
// all this.

using System;
using Microsoft.Win32; // for RegistryKey

namespace TVRename
{
    // TimeZone was initially based on the Win32 structure "TIMEZONEINFO"
    // and, through it's constructor, will take binary data for the Win32 structure
    // (which comes from the registry) and pull it apart to initialise itself.

    public class TimeZone
    {
        // SysTime is based on the Win32 structure SYSTIME
        public class SysTime
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;

            public SysTime(Byte[] bytes, int pos)
            {
                int y = bytes[pos + 0] + ((short)bytes[pos + 1] << 8);
                int m = bytes[pos + 2] + ((short)bytes[pos + 3] << 8);
                int dow = bytes[pos + 4] + ((short)bytes[pos + 5] << 8);
                int day = bytes[pos + 6] + ((short)bytes[pos + 7] << 8);
                int hr = bytes[pos + 8] + ((short)bytes[pos + 9] << 8);
                int min = bytes[pos + 10] + ((short)bytes[pos + 11] << 8);
                int sec = bytes[pos + 12] + ((short)bytes[pos + 13] << 8);
                int msec = bytes[pos + 14] + ((short)bytes[pos + 15] << 8);

                wYear = (short)y;
                wMonth = (short)m;
                wDayOfWeek = (short)dow;
                wDay = (short)day;
                wHour = (short)hr;
                wMinute = (short)min;
                wSecond = (short)sec;
                wMilliseconds = (short)msec;
            }
        }

        public int Bias;
        public int StandardBias;
        public int DaylightBias;
        public SysTime StandardDate;
        public SysTime DaylightDate;

        public TimeZone(Byte[] bytes)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(bytes.Length == 44);
#endif
            // decode bytes from a Win32 TIMEZONEINFO struct

            Bias = bytes[0] + ((int)bytes[1] << 8) + ((int)bytes[2] << 16) + ((int)bytes[3] << 24);
            StandardBias = bytes[4] + ((int)bytes[5] << 8) + ((int)bytes[6] << 16) + ((int)bytes[7] << 24);
            DaylightBias = bytes[8] + ((int)bytes[9] << 8) + ((int)bytes[10] << 16) + ((int)bytes[11] << 24);
            StandardDate = new SysTime(bytes, 12); // uses 16 bytes
            DaylightDate = new SysTime(bytes, 28); // uses 16 bytes
            // 28 + 16 = 44.  yay!
        }
    
        public static string[] ZoneNames()
        {
            if (Version.OnMono())
            {
                // need a mono version of this function
                return new string[0];
            }

            RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones");
            return rk.GetSubKeyNames();
        }

        public static TimeZone TimeZoneFor(string name)
        {
            if (Version.OnMono())
            {
#warning Need a mono version of the TimeZone class.  Does mono have System.Core.TimeZoneInfo?
                return null;
            }
            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones\Eastern Standard Time
            RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\" + name);

            if (rk == null)
                return null;
            else
                return new TimeZone((Byte[])rk.GetValue("TZI"));
        }

        public static string DefaultTimeZone()
        {
            return "Eastern Standard Time";
        }

        public static DateTime? AdjustTZTimeToLocalTime(DateTime? dt, TimeZone theirTimeZone) // set tz to 0 to not correct for timezone
        {
            if ((theirTimeZone == null) || (dt == null))
                return dt;

            if (Version.OnMono())
            {
                // need a mono version of this function
                return dt;
            }

            // take a DateTime in a foreign timezone (tz), and return it in ours
            // e.g. 3PM,NZST -> 1PM (if this computer's local time is in AEST)
            int thisYear = DateTime.Now.Year;

            // some timezones don't observe any DST.  the changeover dates seem to be all zeroes in that case.
            bool theyHaveDST = !((theirTimeZone.DaylightDate.wMonth == 0) && (theirTimeZone.DaylightDate.wDay == 0) && (theirTimeZone.DaylightDate.wHour == 0) && (theirTimeZone.DaylightDate.wMinute == 0) && (theirTimeZone.DaylightDate.wSecond == 0) && (theirTimeZone.DaylightDate.wMilliseconds == 0) && (theirTimeZone.StandardDate.wMonth == 0) && (theirTimeZone.StandardDate.wDay == 0) && (theirTimeZone.StandardDate.wHour == 0) && (theirTimeZone.StandardDate.wMinute == 0) && (theirTimeZone.StandardDate.wSecond == 0) && (theirTimeZone.StandardDate.wMilliseconds == 0));

            DateTime themNow = DateTime.UtcNow.AddMinutes(-theirTimeZone.Bias); // tz->bias in minutes. +300 = 5 hours _behind_ UTC

            if (theyHaveDST)
            {
                DateTime theirDSTStart = new DateTime(thisYear, theirTimeZone.DaylightDate.wMonth, theirTimeZone.DaylightDate.wDay, theirTimeZone.DaylightDate.wHour, theirTimeZone.DaylightDate.wMinute, theirTimeZone.DaylightDate.wSecond);
                DateTime theirDSTEnd = new DateTime(thisYear, theirTimeZone.StandardDate.wMonth, theirTimeZone.StandardDate.wDay, theirTimeZone.StandardDate.wHour, theirTimeZone.StandardDate.wMinute, theirTimeZone.StandardDate.wSecond);

                if (theirDSTEnd.CompareTo(theirDSTStart) < 0)
                    theirDSTStart -= new TimeSpan(365, 0, 0, 0, 0);

                if (themNow.CompareTo(theirDSTStart) > 0)
                    themNow = themNow.AddMinutes(-theirTimeZone.DaylightBias);
                if (themNow.CompareTo(theirDSTEnd) > 0)
                    themNow = themNow.AddMinutes(theirTimeZone.DaylightBias);
            }

            TimeSpan tweakTime = DateTime.Now.Subtract(themNow);

            return dt.Value.Add(tweakTime);
        }

        public static uint Epoch() // unix epoch time for now (seconds since midnight 1 jan 1970 UTC)
        {
            return (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }
        public static uint Epoch(DateTime dt)
        {
            DateTime uni = dt.ToUniversalTime();
            uint r = (uint)(uni.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            return r;
        }
    }
}