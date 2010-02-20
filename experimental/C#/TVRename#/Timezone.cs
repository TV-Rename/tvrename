//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

using Microsoft.Win32; // for RegistryKey
using System;

// If we were targeting .NET 3.5 or later, then we'd use System.Core.TimeZoneInfo

namespace TVRename
{
    public class TZI
    {
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

            public SysTime(short y, short mon, short dow, short d, short h, short min, short sec, short msec)
            {
                wYear = y;
                wMonth = mon;
                wDayOfWeek = dow;
                wDay = d;
                wHour = h;
                wMinute = min;
                wSecond = sec;
                wMilliseconds = msec;
            }
        }

        public int Bias;
        public int StandardBias;
        public int DaylightBias;
        public SysTime StandardDate;
        public SysTime DaylightDate;

        SysTime SystimeFromBytesStartingAt(Byte[] bytes, int pos)
        {
            int y = bytes[pos + 0] + ((short)bytes[pos + 1] << 8);
            int m = bytes[pos + 2] + ((short)bytes[pos + 3] << 8);
            int dow = bytes[pos + 4] + ((short)bytes[pos + 5] << 8);
            int day = bytes[pos + 6] + ((short)bytes[pos + 7] << 8);
            int hr = bytes[pos + 8] + ((short)bytes[pos + 9] << 8);
            int min = bytes[pos + 10] + ((short)bytes[pos + 11] << 8);
            int sec = bytes[pos + 12] + ((short)bytes[pos + 13] << 8);
            int msec = bytes[pos + 14] + ((short)bytes[pos + 15] << 8);
            return new SysTime((short)y, (short)m, (short)dow, (short)day, (short)hr, (short)min, (short)sec, (short)msec);
        }
        public TZI(Byte[] bytes)
        {
            System.Diagnostics.Debug.Assert(bytes.Length == 44);
            // decode bytes into a TZI
            // they are normally a packed struct

            Bias = bytes[0] + ((int)bytes[1] << 8) + ((int)bytes[2] << 16) + ((int)bytes[3] << 24);
            StandardBias = bytes[4] + ((int)bytes[5] << 8) + ((int)bytes[6] << 16) + ((int)bytes[7] << 24);
            DaylightBias = bytes[8] + ((int)bytes[9] << 8) + ((int)bytes[10] << 16) + ((int)bytes[11] << 24);
            StandardDate = SystimeFromBytesStartingAt(bytes, 12); // uses 16 bytes
            DaylightDate = SystimeFromBytesStartingAt(bytes, 28); // uses 16 bytes
            // 28 + 16 = 44.  yay!
        }
    }

    public static class TZMagic
    {
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

        public static TZI GetTZI(string name)
        {
            if (Version.OnMono())
            {
                // need a mono version of this function
                return null;
            }
            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones\Eastern Standard Time
            RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\" + name);

            if (rk == null)
                return null;
            else
                return new TZI((Byte[])rk.GetValue("TZI"));
        }

        public static string DefaultTZ()
        {
            return "Eastern Standard Time";
        }

        public static DateTime? AdjustTZTimeToLocalTime(DateTime? dt, TZI theirTZI) // set tz to 0 to not correct for timezone
        {
            if ((theirTZI == null) || (dt == null))
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
            bool theyHaveDST = !((theirTZI.DaylightDate.wMonth == 0) && (theirTZI.DaylightDate.wDay == 0) && (theirTZI.DaylightDate.wHour == 0) && (theirTZI.DaylightDate.wMinute == 0) && (theirTZI.DaylightDate.wSecond == 0) && (theirTZI.DaylightDate.wMilliseconds == 0) && (theirTZI.StandardDate.wMonth == 0) && (theirTZI.StandardDate.wDay == 0) && (theirTZI.StandardDate.wHour == 0) && (theirTZI.StandardDate.wMinute == 0) && (theirTZI.StandardDate.wSecond == 0) && (theirTZI.StandardDate.wMilliseconds == 0));

            DateTime themNow = DateTime.UtcNow.AddMinutes(-theirTZI.Bias); // tz->bias in minutes. +300 = 5 hours _behind_ UTC

            if (theyHaveDST)
            {
                DateTime theirDSTStart = new DateTime(thisYear, theirTZI.DaylightDate.wMonth, theirTZI.DaylightDate.wDay, theirTZI.DaylightDate.wHour, theirTZI.DaylightDate.wMinute, theirTZI.DaylightDate.wSecond);
                DateTime theirDSTEnd = new DateTime(thisYear, theirTZI.StandardDate.wMonth, theirTZI.StandardDate.wDay, theirTZI.StandardDate.wHour, theirTZI.StandardDate.wMinute, theirTZI.StandardDate.wSecond);

                if (theirDSTEnd.CompareTo(theirDSTStart) < 0)
                    theirDSTStart -= new TimeSpan(365, 0, 0, 0, 0);

                if (themNow.CompareTo(theirDSTStart) > 0)
                    themNow = themNow.AddMinutes(-theirTZI.DaylightBias);
                if (themNow.CompareTo(theirDSTEnd) > 0)
                    themNow = themNow.AddMinutes(theirTZI.DaylightBias);
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