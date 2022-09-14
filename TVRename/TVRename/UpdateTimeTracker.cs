using Humanizer;
using System;

namespace TVRename;

public class UpdateTimeTracker
{
    public UpdateTimeTracker()
    {
        SetTimes(0);
    }

    private long newSrvTime; //tme from the latest update
    private long srvTime; // only update this after a 100% successful download
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public bool HasIncreased => srvTime < newSrvTime;

    public void Reset()
    {
        SetTimes(TimeHelpers.UnixUtcNow());
    }

    private void SetTimes(long newTime)
    {
        newSrvTime = newTime;
        srvTime = newSrvTime;
    }

    public override string ToString() =>
        $"System is up to date from: {srvTime} to {newSrvTime}. ie {LastSuccessfulServerUpdateDateTime()} to {ProposedServerUpdateDateTime()}";

    public void RecordSuccessfulUpdate()
    {
        srvTime = newSrvTime;
    }

    public long LastSuccessfulServerUpdateTimecode() => srvTime;

    public DateTime LastSuccessfulServerUpdateDateTime() => srvTime.FromUnixTime().ToLocalTime();

    public DateTime ProposedServerUpdateDateTime() => newSrvTime.FromUnixTime().ToLocalTime();

    public void Load(string? time)
    {
        long newTime = time is null ? 0 : long.Parse(time);
        if (newTime > TimeHelpers.UnixUtcNow() + 1.Days().TotalSeconds)
        {
            Logger.Error($"Asked to update time to: {newTime} by parsing {time}");
            newTime = TimeHelpers.UnixUtcNow();
        }

        SetTimes(newTime);
    }

    public void RegisterServerUpdate(long maxUpdateTime)
    {
        if (maxUpdateTime > TimeHelpers.UnixUtcNow() + 1.Days().TotalSeconds)
        {
            Logger.Error($"Asked to update time to: {maxUpdateTime}");
            newSrvTime = TimeHelpers.UnixUtcNow();
        }
        else
        {
            newSrvTime =
                Math.Max(newSrvTime,
                    Math.Max(maxUpdateTime,
                        srvTime)); // just in case the new update time is no better than the prior one
        }
    }
}
