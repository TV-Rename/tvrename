using System;
using System.Collections.Generic;

namespace TVRename;

public class ProcessedEpisode : Episode
{
    public int
        EpNum2; // if we are a concatenation of episodes, this is the last one in the cachedSeries. Otherwise, same as EpNum

    public bool Ignore;
    public bool NextToAir;
    public int OverallNumber;
    public readonly ShowConfiguration Show;
    public readonly ProcessedEpisodeType Type;
    public readonly List<Episode> SourceEpisodes;
    public ProcessedSeason TheAiredProcessedSeason;
    public ProcessedSeason TheDvdProcessedSeason;

    // ReSharper disable once InconsistentNaming
    public string TVDBWebsiteUrl => TheTVDB.API.WebsiteEpisodeUrl(this);

    public enum ProcessedEpisodeType
    {
        single,
        split,
        merged
    }

    public ProcessedEpisode(ProcessedEpisode o)
        : base(o)
    {
        NextToAir = o.NextToAir;
        EpNum2 = o.EpNum2;
        Ignore = o.Ignore;
        Show = o.Show;
        OverallNumber = o.OverallNumber;
        Type = o.Type;
        TheAiredProcessedSeason = o.TheAiredProcessedSeason;
        TheDvdProcessedSeason = o.TheDvdProcessedSeason;
        SourceEpisodes = [];
    }

    public ProcessedEpisode(Episode e, ShowConfiguration si)
        : base(e)
    {
        OverallNumber = -1;
        NextToAir = false;
        Ignore = false;
        Show = si;
        EpNum2 = Show.Order == ProcessedSeason.SeasonType.dvd ? DvdEpNum : AiredEpNum;
        Type = ProcessedEpisodeType.single;
        TheAiredProcessedSeason = si.GetOrAddAiredSeason(e.AiredSeasonNumber, e.SeasonId);
        TheDvdProcessedSeason = si.GetOrAddDvdSeason(e.DvdSeasonNumber, e.SeasonId);
        SourceEpisodes = [];
    }

    public ProcessedEpisode(ProcessedEpisode e, ShowConfiguration si, ProcessedEpisodeType t)
        : base(e)
    {
        OverallNumber = -1;
        NextToAir = false;
        Show = si;
        EpNum2 = Show.Order == ProcessedSeason.SeasonType.dvd ? DvdEpNum : AiredEpNum;
        Ignore = false;
        Type = t;
        TheAiredProcessedSeason = e.TheAiredProcessedSeason;
        TheDvdProcessedSeason = e.TheDvdProcessedSeason;
        SourceEpisodes = [];
    }

    public ProcessedEpisode(ProcessedEpisode e, ShowConfiguration si, List<Episode> episodes)
        : base(e)
    {
        OverallNumber = -1;
        NextToAir = false;
        Show = si;
        EpNum2 = Show.Order == ProcessedSeason.SeasonType.dvd ? DvdEpNum : AiredEpNum;
        Ignore = false;
        SourceEpisodes = episodes;
        Type = ProcessedEpisodeType.merged;
        TheAiredProcessedSeason = e.TheAiredProcessedSeason;
        TheDvdProcessedSeason = e.TheDvdProcessedSeason;
    }

    public ProcessedEpisode(ProcessedEpisode pe, ShowConfiguration si, string name, int airedEpNum, int dvdEpNum, int epNum2)
        : base(pe)
    {
        //This is used when a new episode is inserted

        EpisodeId = -1;
        SrvLastUpdated = 0;
        Overview = string.Empty;
        Runtime = null;
        LinkUrl = null;
        EpisodeRating = null;
        EpisodeGuestStars = null;
        EpisodeDirector = null;
        Writer = null;
        Dirty = false;
        DvdChapter = null;
        DvdDiscId = null;
        AirsBeforeEpisode = null;
        AirsBeforeSeason = null;
        AirsAfterSeason = null;
        SiteRatingCount = null;
        AbsoluteNumber = null;
        ProductionCode = null;
        ImdbCode = null;
        Filename = null;
        AirStamp = null;
        AirTime = null;

        NextToAir = false;
        OverallNumber = -1;
        Ignore = false;
        Name = name;
        AiredEpNum = airedEpNum;
        DvdEpNum = dvdEpNum;
        EpNum2 = epNum2;
        Show = si;
        Type = ProcessedEpisodeType.single;
        TheAiredProcessedSeason = pe.TheAiredProcessedSeason;
        TheDvdProcessedSeason = pe.TheDvdProcessedSeason;
        SourceEpisodes = [];
    }

    public int AppropriateSeasonNumber => Show.Order == ProcessedSeason.SeasonType.dvd ? DvdSeasonNumber : AiredSeasonNumber;
    public int AppropriateSeasonIndex => Show.GetSeasonIndex(AppropriateSeasonNumber);
    public ProcessedSeason AppropriateProcessedSeason => Show.Order == ProcessedSeason.SeasonType.dvd ? TheDvdProcessedSeason : TheAiredProcessedSeason;
    public int AppropriateEpNum => Show.Order == ProcessedSeason.SeasonType.dvd ? DvdEpNum : AiredEpNum;

    public bool PreviouslySeen => TVSettings.Instance.PreviouslySeenEpisodes.Contains(EpisodeId);

    public string SeasonNumberAsText => TVSettings.SeasonNameFor(AppropriateSeasonNumber);

    public string? WebsiteUrl => Show.Provider == TVDoc.ProviderType.TheTVDB ? TVDBWebsiteUrl : LinkUrl;

    public string EpisodeNumbersAsText
    {
        get
        {
            if (AppropriateEpNum > 0 && EpNum2 != AppropriateEpNum && EpNum2 > 0)
            {
                return AppropriateEpNum + "-" + EpNum2;
            }

            return AppropriateEpNum > 0 ? AppropriateEpNum.ToString() : string.Empty;
        }
    }

    public static int EpNumberSorter(ProcessedEpisode e1, ProcessedEpisode e2)
    {
        int ep1 = e1.AiredEpNum;
        int ep2 = e2.AiredEpNum;

        return ep1 - ep2;
    }

    // ReSharper disable once InconsistentNaming
    public static int DVDOrderSorter(ProcessedEpisode e1, ProcessedEpisode e2)
    {
        int ep1 = e1.DvdEpNum;
        int ep2 = e2.DvdEpNum;

        return ep1 - ep2;
    }

    public DateTime? GetAirDateDt() =>
        // do timezone adjustment
        GetAirDateDt(Show.GetTimeZone());

    public string HowLong()
    {
        DateTime? airsdt = GetAirDateDt();
        if (airsdt is null)
        {
            return string.Empty;
        }

        TimeSpan ts = airsdt.Value.Subtract(TimeHelpers.LocalNow()); // how long...
        if (ts.TotalHours < 0)
        {
            return "Aired";
        }

        int h = ts.Hours;
        if (ts.TotalHours >= 1)
        {
            if (ts.Minutes >= 30)
            {
                h += 1;
            }

            return ts.Days + "d " + h + "h";
        }

        return Math.Round(ts.TotalMinutes) + "min";
    }

    public string DayOfWeek()
    {
        DateTime? dt = GetAirDateDt();
        return dt != null ? dt.Value.ToString("ddd") : "-";
    }

    public string TimeOfDay()
    {
        DateTime? dt = GetAirDateDt();
        return dt != null ? dt.Value.ToString("t") : "-";
    }

    public new bool HasAired()
    {
        DateTime? airsdt = GetAirDateDt();
        TimeSpan? ts = airsdt?.Subtract(TimeHelpers.LocalNow()); // how long...
        return ts?.TotalHours < 0;
    }

    public bool WithinLastDays(int days)
    {
        DateTime? dt = GetAirDateDt();
        if (dt is null || dt.Value.CompareTo(DateTime.MaxValue) == 0)
        {
            return false;
        }

        DateTime now = TimeHelpers.LocalNow();
        DateTime limit = now.AddDays(-days);
        
        return limit <= dt && dt <= now;
    }

    public bool IsInFuture(bool def)
    {
        DateTime? airsdt = GetAirDateDt();
        if (airsdt is null)
        {
            return def;
        }

        DateTime dt = (DateTime)airsdt;

        TimeSpan ts = dt.Subtract(TimeHelpers.LocalNow()); // how long...
        return ts.TotalHours > 0;
    }

    public override string ToString() => $"S{AppropriateSeasonNumber:D2}E{AppropriateEpNum:D2} - {Name}";

    public void SetEpisodeNumbers(int startEpisodeNum, int endEpisodeNum)
    {
        if (Show.Order == ProcessedSeason.SeasonType.dvd)
        {
            DvdEpNum = startEpisodeNum;
        }
        else
        {
            AiredEpNum = startEpisodeNum;
        }

        EpNum2 = endEpisodeNum;
    }

    public bool NotOnDvd()
    {
        return DvdEpNum == 0 && DvdChapter is null && string.IsNullOrWhiteSpace(DvdDiscId) && DvdSeasonNumber == 0;
    }

    public bool HasAiredDate()
    {
        DateTime? dt = GetAirDateDt();
        return dt != null && dt.Value.CompareTo(DateTime.MaxValue) != 0;
    }
}
