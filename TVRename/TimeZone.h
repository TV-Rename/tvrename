//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

namespace TVRename
{
using namespace Microsoft::Win32;
using namespace System;

	////////////////////////////////////////////////////////////////////////////
	// Timezone magic

#pragma pack(push,1)
	struct SYSTEMTIME {
		short wYear;
		short wMonth;
		short wDayOfWeek;
		short wDay;
		short wHour;
		short wMinute;
		short wSecond;
		short wMilliseconds;
	};
#pragma pack(pop)

#pragma pack(push,1)
	struct TZI
	{
	public:
		int bias;
		int standardBias;
		int daylightBias;
		SYSTEMTIME standardDate;
		SYSTEMTIME daylightDate;
	};
#pragma pack(pop)

	typedef array<Byte>^ TZIBytes;


	public ref class TZMagic
	{
	private: TZMagic()
			 {
			 }
	public:
		static array<Byte>^ GetTZ(String ^name)
		{
			// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones\Eastern Standard Time
			RegistryKey ^rk = Registry::LocalMachine->OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\" + name);

			if (rk == nullptr)
				return nullptr;
			else
				return (array<Byte>^)rk->GetValue("TZI");
		}
		
		static String ^DefaultTZ()
		{
			return "Eastern Standard Time";
		}

		static bool BytesToTZI(TZIBytes mTZIBytes, TZI *tz)
		{		
			char *p = (char *)(tz);
			if (mTZIBytes->Length != sizeof(TZI))
				return false;

			for (int i=0;i<sizeof(TZI);i++)
				*p++ = mTZIBytes[i];

			return true;
		}


		static DateTime ^AdjustTZTimeToOurs(DateTime ^dt, const TZI *tz) // set tz to 0 to not correct for timezone
		{
                    if ((tz == 0) || (dt == nullptr))
                        return dt;

                    // take a DateTime in a foreign timezone (tz), and return it in ours
                    // e.g. 3PM,NZST -> 1PM (if this computer's local time is in AEST)
                    int thisYear = DateTime::Now.Year;

                    // TimeSpan ^ourUTCdiff = TimeZone::CurrentTimeZone->GetUtcOffset(DateTime::Now);

                    // some timezones don't observe any DST.  the changeover dates seem to be all zeroes in that case.
                    bool theyHaveDST = ! (
                        (tz->daylightDate.wMonth == 0) && 
                        (tz->daylightDate.wDay == 0) &&
                        (tz->daylightDate.wHour == 0) && 
                        (tz->daylightDate.wMinute == 0) &&
                        (tz->daylightDate.wSecond == 0) &&
                        (tz->daylightDate.wMilliseconds == 0) &&
                        (tz->standardDate.wMonth == 0) && 
                        (tz->standardDate.wDay == 0) &&
                        (tz->standardDate.wHour == 0) && 
                        (tz->standardDate.wMinute == 0) &&
                        (tz->standardDate.wSecond == 0) &&
                        (tz->standardDate.wMilliseconds == 0)
                        );

                    DateTime themNow = DateTime::UtcNow.AddMinutes(-tz->bias); // tz->bias in minutes.  +300 = 5 hours _behind_ UTC

                    if (theyHaveDST)
                    {
                        DateTime theirDSTStart(thisYear,tz->daylightDate.wMonth,tz->daylightDate.wDay,
                            tz->daylightDate.wHour,tz->daylightDate.wMinute,tz->daylightDate.wSecond);
                        DateTime theirDSTEnd(thisYear,tz->standardDate.wMonth,tz->standardDate.wDay,
                            tz->standardDate.wHour,tz->standardDate.wMinute,tz->standardDate.wSecond);

                        if (theirDSTEnd.CompareTo(theirDSTStart) < 0)
                            theirDSTStart -= TimeSpan(365,0,0,0,0);

                        if (themNow.CompareTo(theirDSTStart) > 0)
                            themNow = themNow.AddMinutes(-tz->daylightBias);
                        if (themNow.CompareTo(theirDSTEnd) > 0)
                            themNow = themNow.AddMinutes(tz->daylightBias);
                    }

                    TimeSpan tweakTime = DateTime::Now.Subtract(themNow);

                    return dt->Add(tweakTime);;
		}


		static unsigned long Epoch()
		{
			return (unsigned long)(DateTime::UtcNow.Subtract(DateTime(1970,1,1,0,0,0,0)).TotalSeconds);
		}		
		static unsigned long Epoch(DateTime ^dt)
		{
			DateTime ^uni = dt->ToUniversalTime();
			unsigned long r = (unsigned long)(uni->Subtract(DateTime(1970,1,1,0,0,0,0)).TotalSeconds);
			return r;
		}		

	};

	

}
