using System;
using Humanizer;

namespace TVRename
{
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
            SetTimes(DateTime.UtcNow.ToUnixTime());
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
        public DateTime LastSuccessfulServerUpdateDateTime() => Helpers.FromUnixTime(srvTime).ToLocalTime();
        public DateTime ProposedServerUpdateDateTime() => Helpers.FromUnixTime(newSrvTime).ToLocalTime();

        public void Load(string? time)
        {
            long newTime = time is null ? 0 : long.Parse(time);
            if (newTime > DateTime.UtcNow.ToUnixTime() + 1.Days().TotalSeconds)
            {
                Logger.Error($"Asked to update time to: {newTime} by parsing {time}");
                newTime = DateTime.UtcNow.ToUnixTime();
            }

            SetTimes(newTime);
        }

        public void RegisterServerUpdate(long maxUpdateTime)
        {
            if (maxUpdateTime > DateTime.UtcNow.ToUnixTime() + 1.Days().TotalSeconds)
            {
                Logger.Error($"Asked to update time to: {maxUpdateTime}");
                newSrvTime = DateTime.UtcNow.ToUnixTime();
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
}
