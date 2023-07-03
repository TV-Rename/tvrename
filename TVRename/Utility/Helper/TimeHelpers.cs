using GuerrillaNtp;
using Humanizer;
using NLog;
using System;
using System.Net.Sockets;

namespace TVRename;

public static class TimeHelpers
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static NtpClock? ClockInstance;
    private static bool AlreadyAlerted;

    private static NtpClock GetClock()
    {
        if (ClockInstance is not null)
        {
            return ClockInstance;
        }

        if (AlreadyAlerted && TVSettings.Instance.OfflineMode)
        {
            return NtpClock.LocalFallback;
        }

        try
        {
            Logger.Info($"Connected to NTP Server at UTC (based on local clock) {DateTime.UtcNow}");
            ClockInstance = NtpClient.Default.Query();
            Logger.Info($"Finished connecting to NTP Server at UTC (based on local clock) {DateTime.UtcNow} - Offset = {ClockInstance.CorrectionOffset}");
            if (ClockInstance.CorrectionOffset > 20.Seconds())
            {
                Logger.Warn($"Discrepancy for systemtime of {DateTime.UtcNow} to {ClockInstance.UtcNow.UtcDateTime}");
            }
            return ClockInstance;
        }
        catch (NtpException e)
        {
            LogNtpConnectionIssue(e);
            return NtpClock.LocalFallback;
        }
        catch (SocketException e)
        {
            LogNtpConnectionIssue(e);
            return NtpClock.LocalFallback;
        }
    }

    public static DateTime GetRequestedTime(this long updateFromEpochTime)
    {
        try
        {
            return updateFromEpochTime.FromUnixTime().ToUniversalTime();
        }
        catch (Exception ex)
        {
            Logger.Error(ex,
                $"Could not convert {updateFromEpochTime} to DateTime.");
        }

        //Have to do something!!
        return DateTime.UnixEpoch.ToUniversalTime();
    }
    public static bool EqualsUpToSeconds(this DateTime dt1, DateTime dt2)
    {
        return dt1.Year == dt2.Year
               && dt1.Month == dt2.Month
               && dt1.Day == dt2.Day
               && dt1.Hour == dt2.Hour
               && dt1.Minute == dt2.Minute
               && dt1.Second == dt2.Second;
    }
    private static void LogNtpConnectionIssue(Exception e)
    {
        if (AlreadyAlerted)
        {
            Logger.Warn($"Could not connect to NTP clock: {e.ErrorText()}");
        }
        else
        {
            Logger.Error($"Could not connect to NTP clock: {e.ErrorText()}");
            AlreadyAlerted = true;
        }
    }

    public static DateTime UtcNow() => GetClock().UtcNow.UtcDateTime;
    public static long UnixUtcNow() => UtcNow().ToUnixTime();

    public static DateTime LocalNow() => GetClock().Now.LocalDateTime;

    public static string PrettyPrint(this DateTime? dt)
    {
        try
        {
            if (dt != null && dt.Value.CompareTo(DateTime.MaxValue) != 0)
            {
                return dt.Value.ToShortDateString();
            }
        }
        catch (ArgumentOutOfRangeException)
        {
        }

        return string.Empty;
    }
}
