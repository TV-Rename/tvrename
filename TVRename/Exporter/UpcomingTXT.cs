using System;
using System.Collections.Generic;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class UpcomingTXT : UpcomingExporter
    {
        public UpcomingTXT(TVDoc i) : base(i)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportWTWTXT;

        protected override string Location() => TVSettings.Instance.ExportWTWTXTTo;
        protected override string Name() => "Upcoming TXT Exporter";

        protected override bool Generate(System.IO.Stream str, IEnumerable<ProcessedEpisode> elist)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(str))
                {
                    file.WriteLine(HeaderLine());
                    foreach (ProcessedEpisode processedEpisode in elist)
                    {
                        file.WriteLine(ConvertToLine(processedEpisode));
                    }
                }
                return true;
            } // try
            catch (Exception e)
            {
                LOGGER.Error(e);
                return false;
            }
        }

        private string HeaderLine() => FormattedLine("Show", "Network", "Day", "Time");

        private string ConvertToLine(ProcessedEpisode ei)
        {
            DateTime? stTime = ei.GetAirDateDt(true);

            if (!stTime.HasValue)
            {
                return string.Empty;
            }

            string niceName = TVSettings.Instance.NamingStyle.NameFor(ei);
            DateTime startTime = stTime.Value;

            return FormattedLine(niceName, ei.TheCachedSeries.Network ?? string.Empty, startTime.ToString("ddd, d MMM"), startTime.ToString("HH:mm"));
        }

        private string FormattedLine(string niceName, string network, string day, string time)
        {
            return $"{day,-15} {time,-10} {network.First(15),-15} {niceName.First(80),-80}";
        }
    }
}
