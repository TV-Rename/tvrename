//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class UpcomingiCAL : UpcomingExporter
    {
        public UpcomingiCAL(TVDoc i) : base(i)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportWTWICAL;

        protected override string Location() => TVSettings.Instance.ExportWTWICALTo;
        [NotNull]
        protected override string Name() => "Upcoming iCal Exporter";
        protected override bool Generate(System.IO.Stream str, IEnumerable<ProcessedEpisode>? episodes)
        {
            if (episodes is null)
            {
                return false;
            }

            try
            {
                Calendar calendar = new() { ProductId = "Upcoming Shows Exported by TV Rename http://www.tvrename.com" };

                foreach (CalendarEvent ev in episodes.Select(CreateEvent).Where(ev => ev is not null))
                {
                    calendar.Events.Add(ev);
                }

                CalendarSerializer serializer = new();
                serializer.Serialize(calendar, str, Encoding.ASCII);

                return true;
            } // try
            catch (Exception e)
            {
                LOGGER.Error(e);
                return false;
            }
        }

        private static CalendarEvent? CreateEvent([NotNull] ProcessedEpisode ei)
        {
            string niceName = TVSettings.Instance.NamingStyle.NameFor(ei);
            try
            {
                DateTime? stTime = ei.GetAirDateDt(true);

                if (!stTime.HasValue)
                {
                    return null;
                }

                DateTime startTime = stTime.Value;
                string s = ei.Show.CachedShow?.Runtime;
                DateTime endTime = stTime.Value.AddMinutes(string.IsNullOrWhiteSpace(s) ? 0 : int.Parse(s!));

                return new CalendarEvent
                {
                    Start = new CalDateTime(startTime),
                    End = new CalDateTime(endTime),
                    Description = ei.Overview,
                    Comments = new List<string> { ei.Overview },
                    Summary = niceName,
                    Location = ei.TheCachedSeries.Networks.ToCsv(),
                    Url = new Uri(ei.ProviderWebUrl()),
                    Uid = ei.EpisodeId.ToString()
                };
            }
            catch (Exception e)
            {
                LOGGER.Error(e, $"Failed to create ics record for {niceName}");
                return null;
            }
        }
    }
}
