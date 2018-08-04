using System;
using System.Collections.Generic;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class UpcomingiCAL :UpcomingExporter
    {
        public UpcomingiCAL(TVDoc i) : base(i) { }
        public override bool Active() =>TVSettings.Instance.ExportWTWICAL;
        protected override string Location() => TVSettings.Instance.ExportWTWICALTo;

        protected override bool Generate(System.IO.Stream str, List<ProcessedEpisode> elist)
        {
            if (elist == null)
                return false;

            try
            {
                Calendar calendar = new Calendar {ProductId = "Upcoming Shows Exported by TV Rename http://www.tvrename.com"};
                
                foreach (ProcessedEpisode ei in elist)
                {
                    string niceName = TVSettings.Instance.NamingStyle.NameFor(ei);
                    DateTime? stTime = ei.GetAirDateDT(true);

                    if (!stTime.HasValue) continue;

                    DateTime startTime = stTime.Value;
                    String s = ei.SI.TheSeries().GetRuntime();
                    DateTime endTime = stTime.Value.AddMinutes(int.Parse(s));

                    CalendarEvent e = new CalendarEvent
                    {
                        Start = new CalDateTime(startTime),
                        End = new CalDateTime(endTime),
                        Description = ei.Overview,
                        Comments = new List<string>{ei.Overview},
                        Summary = niceName,
                        Location=ei.TheSeries.GetNetwork(),
                        Url = new Uri(TheTVDB.Instance.WebsiteUrl(ei.TheSeries.TVDBCode, ei.SeasonId, false)),
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
                Logger.Error(e);
                return false;
            }
        }
    }
}
