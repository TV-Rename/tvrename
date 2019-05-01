// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System;
using System.Collections.Generic;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using JetBrains.Annotations;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class UpcomingiCAL :UpcomingExporter
    {
        public UpcomingiCAL(TVDoc i) : base(i) { }
        public override bool Active() =>TVSettings.Instance.ExportWTWICAL;
        protected override string Location() => TVSettings.Instance.ExportWTWICALTo;

        protected override bool Generate(System.IO.Stream str, [CanBeNull] List<ProcessedEpisode> elist)
        {
            if (elist is null)
            {
                return false;
            }

            try
            {
                Calendar calendar = new Calendar {ProductId = "Upcoming Shows Exported by TV Rename http://www.tvrename.com"};
                
                foreach (ProcessedEpisode ei in elist)
                {
                    string niceName = TVSettings.Instance.NamingStyle.NameFor(ei);
                    DateTime? stTime = ei.GetAirDateDt(true);

                    if (!stTime.HasValue)
                    {
                        continue;
                    }

                    DateTime startTime = stTime.Value;
                    string s = ei.Show.TheSeries()?.Runtime;
                    DateTime endTime = stTime.Value.AddMinutes(s==null?0:int.Parse(s));

                    CalendarEvent e = new CalendarEvent
                    {
                        Start = new CalDateTime(startTime),
                        End = new CalDateTime(endTime),
                        Description = ei.Overview,
                        Comments = new List<string>{ei.Overview},
                        Summary = niceName,
                        Location=ei.TheSeries.Network,
                        Url = new Uri(TheTVDB.Instance.WebsiteUrl(ei.TheSeries.TvdbCode, ei.SeasonId, false)),
                        Uid = ei.EpisodeId.ToString()
                    };
                    calendar.Events.Add(e);
                }

                CalendarSerializer serializer = new CalendarSerializer();
                serializer.Serialize(calendar,str,Encoding.ASCII);

                return true;
            } // try
            catch (Exception e)
            {
                LOGGER.Error(e);
                return false;
            }
        }
    }
}
