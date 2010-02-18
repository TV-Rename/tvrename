using Microsoft.Win32;
using System;
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

namespace TVRename
{

	////////////////////////////////////////////////////////////////////////////
	// Timezone magic

//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(push,1)
	public class SYSTEMTIME
	{
		public short wYear;
		public short wMonth;
		public short wDayOfWeek;
		public short wDay;
		public short wHour;
		public short wMinute;
		public short wSecond;
		public short wMilliseconds;
	}
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(pop)

//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(push,1)
	public class TZI
	{
		public int bias;
		public int standardBias;
		public int daylightBias;
		public SYSTEMTIME standardDate = new SYSTEMTIME();
		public SYSTEMTIME daylightDate = new SYSTEMTIME();
	}
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(pop)



	public class TZMagic
	{
	private TZMagic()
			 {
			 }
		public static Byte[] GetTZ(string name)
		{
			// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones\Eastern Standard Time
			RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\" + name);

			if (rk == null)
				return null;
			else
				return (Byte[])rk.GetValue("TZI");
		}

		public static string DefaultTZ()
		{
			return "Eastern Standard Time";
		}

		public static bool BytesToTZI(Byte[] mTZIBytes, TZI tz)
		{
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged.
			char *p = (char)(tz);
			if (mTZIBytes.Length != sizeof(TZI))
				return false;

			for (int i =0;i<sizeof(TZI);i++)
				*p++= mTZIBytes[i];

			return true;
		}


		public static DateTime AdjustTZTimeToOurs(DateTime dt, TZI tz) // set tz to 0 to not correct for timezone
		{
					if ((tz == 0) || (dt == null))
						return dt;

					// take a DateTime in a foreign timezone (tz), and return it in ours
					// e.g. 3PM,NZST -> 1PM (if this computer's local time is in AEST)
					int thisYear = DateTime.Now.Year;

					// TimeSpan ^ourUTCdiff = TimeZone::CurrentTimeZone->GetUtcOffset(DateTime::Now);

					// some timezones don't observe any DST.  the changeover dates seem to be all zeroes in that case.
					bool theyHaveDST = ! ((tz.daylightDate.wMonth == 0) && (tz.daylightDate.wDay == 0) && (tz.daylightDate.wHour == 0) && (tz.daylightDate.wMinute == 0) && (tz.daylightDate.wSecond == 0) && (tz.daylightDate.wMilliseconds == 0) && (tz.standardDate.wMonth == 0) && (tz.standardDate.wDay == 0) && (tz.standardDate.wHour == 0) && (tz.standardDate.wMinute == 0) && (tz.standardDate.wSecond == 0) && (tz.standardDate.wMilliseconds == 0));

					DateTime themNow = DateTime.UtcNow.AddMinutes(-tz.bias); // tz->bias in minutes. +300 = 5 hours _behind_ UTC

					if (theyHaveDST)
					{
						DateTime theirDSTStart = new DateTime(thisYear,tz.daylightDate.wMonth,tz.daylightDate.wDay, tz.daylightDate.wHour,tz.daylightDate.wMinute,tz.daylightDate.wSecond);
						DateTime theirDSTEnd = new DateTime(thisYear,tz.standardDate.wMonth,tz.standardDate.wDay, tz.standardDate.wHour,tz.standardDate.wMinute,tz.standardDate.wSecond);

						if (theirDSTEnd.CompareTo(theirDSTStart) < 0)
							theirDSTStart -= TimeSpan(365,0,0,0,0);

						if (themNow.CompareTo(theirDSTStart) > 0)
							themNow = themNow.AddMinutes(-tz.daylightBias);
						if (themNow.CompareTo(theirDSTEnd) > 0)
							themNow = themNow.AddMinutes(tz.daylightBias);
					}

					TimeSpan tweakTime = DateTime.Now.Subtract(themNow);

					return dt.Add(tweakTime);
					;
		}


		public static uint Epoch()
		{
			return (uint)(DateTime.UtcNow.Subtract(DateTime(1970,1,1,0,0,0,0)).TotalSeconds);
		}
		public static uint Epoch(DateTime dt)
		{
			DateTime uni = dt.ToUniversalTime();
			uint r = (uint)(uni.Subtract(DateTime(1970,1,1,0,0,0,0)).TotalSeconds);
			return r;
		}

	}



}