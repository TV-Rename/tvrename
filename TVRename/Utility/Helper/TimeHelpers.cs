using System;
using System.Net.Sockets;
using GuerrillaNtp;
using Humanizer;
using NLog;

namespace TVRename;

public static class TimeHelpers
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static NtpClock? ClockInstance;

    private static NtpClock GetClock()
    {
        try
        {
            if (ClockInstance is null)
            {
                Logger.Info($"Connected to NTP Server at UTC (based on local clock) {DateTime.UtcNow}");
                ClockInstance = NtpClient.Default.Query();
                Logger.Info($"Finished connecting to NTP Server at UTC (based on local clock) {DateTime.UtcNow}");
                if (ClockInstance.CorrectionOffset > 20.Seconds())
                {
                    Logger.Warn($"Discrepancy for systemtime of {DateTime.UtcNow} to {ClockInstance.UtcNow.UtcDateTime}");
                }
            }
        }
        catch (NtpException)
        {
            return NtpClock.LocalFallback;
        }
        catch (SocketException)
        {
            return NtpClock.LocalFallback;
        }
        return ClockInstance;
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
