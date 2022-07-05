//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

public class Episode
{
    public bool Dirty;
    public int AiredEpNum;
    public int DvdEpNum;
    public int? DvdChapter;
    public string? DvdDiscId;
    public int EpisodeId;
    public DateTime? FirstAired;
    public string? Overview;
    public string? LinkUrl;
    public string? Runtime;
    public string? EpisodeRating;
    public string? EpisodeGuestStars;
    public string? EpisodeDirector;
    public string? Writer;
    public int? AirsBeforeSeason;
    public int? AirsBeforeEpisode;
    public int? AirsAfterSeason;
    public int? SiteRatingCount;
    public int? AbsoluteNumber;

    public string? ProductionCode;
    public string? ImdbCode;
    public string? ShowUrl;
    public string? Filename;

    protected internal int ReadAiredSeasonNum; // only use after loading to attach to the correct season!
    public int ReadDvdSeasonNum; // only use after loading to attach to the correct season!
    public int SeasonId;
    public int SeriesId;
    public long SrvLastUpdated;

    private CachedSeriesInfo? internalSeries;

    internal string? MName;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public DateTime? AirStamp;
    public DateTime? AirTime;

    // ReSharper disable once FunctionComplexityOverflow
    protected Episode(Episode o)
    {
        EpisodeId = o.EpisodeId;
        SeriesId = o.SeriesId;
        AiredEpNum = o.AiredEpNum;
        DvdEpNum = o.DvdEpNum;
        FirstAired = o.FirstAired;
        SrvLastUpdated = o.SrvLastUpdated;
        Overview = o.Overview;
        Runtime = o.Runtime;
        LinkUrl = o.LinkUrl;
        EpisodeRating = o.EpisodeRating;
        EpisodeGuestStars = o.EpisodeGuestStars;
        EpisodeDirector = o.EpisodeDirector;
        Writer = o.Writer;
        MName = o.MName;
        internalSeries = o.TheCachedSeries;
        SeasonId = o.SeasonId;
        Dirty = o.Dirty;
        DvdChapter = o.DvdChapter;
        DvdDiscId = o.DvdDiscId;
        AirsBeforeEpisode = o.AirsBeforeEpisode;
        AirsBeforeSeason = o.AirsBeforeSeason;
        AirsAfterSeason = o.AirsAfterSeason;
        SiteRatingCount = o.SiteRatingCount;
        AbsoluteNumber = o.AbsoluteNumber;
        ProductionCode = o.ProductionCode;
        ImdbCode = o.ImdbCode;
        ShowUrl = o.ShowUrl;
        Filename = o.Filename;
        ReadAiredSeasonNum = o.ReadAiredSeasonNum;
        ReadDvdSeasonNum = o.ReadDvdSeasonNum;
        AirStamp = o.AirStamp;
        AirTime = o.AirTime;
    }

    public LocalDateTime? GetAirDateDt()
    {
        if (FirstAired is null || internalSeries is null)
        {
            return null;
        }

        DateTime fa = (DateTime)FirstAired;
        DateTime? airs = internalSeries.AirsTime;

        return new LocalDateTime(fa.Year, fa.Month, fa.Day, airs?.Hour ?? 20, airs?.Minute ?? 0);
    }

    public DateTime? GetAirDateDt(DateTimeZone tz)
    {
        LocalDateTime? dt = GetAirDateDt();
        if (dt is null)
        {
            return null;
        }

        return TimeZoneHelper.AdjustTzTimeToLocalTime(dt.Value, tz);
    }

    public Episode(int seriesId, CachedSeriesInfo si) :this()
    {
        internalSeries = si;
        SeriesId = seriesId;
    }

    public Episode()
    {
        Overview = string.Empty;
        EpisodeRating = string.Empty;
        EpisodeGuestStars = string.Empty;
        EpisodeDirector = string.Empty;
        Writer = string.Empty;
        MName = string.Empty;
        EpisodeId = -1;
        ReadAiredSeasonNum = -1;
        ReadDvdSeasonNum = -1;
        AiredEpNum = -1;
        DvdEpNum = -1;
        FirstAired = null;
        SrvLastUpdated = -1;
        Dirty = false;
    }

    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(MName))
            {
                return "Aired Episode " + AiredEpNum;
            }

            return MName;
        }
        set => MName = System.Web.HttpUtility.HtmlDecode(value);
    }

    public int AiredSeasonNumber
    {
        get => ReadAiredSeasonNum;
        set => ReadAiredSeasonNum = value;
    }

    public int DvdSeasonNumber => ReadDvdSeasonNum;

    public bool SameAs(Episode o) => EpisodeId == o.EpisodeId;

    public IEnumerable<string> GuestStars => string.IsNullOrEmpty(EpisodeGuestStars) ? Array.Empty<string>() : EpisodeGuestStars.Split('|').Where(s => s.HasValue());

    public IEnumerable<string> Writers => string.IsNullOrEmpty(Writer) ? Array.Empty<string>() : Writer.Split('|').Where(s => s.HasValue());

    public IEnumerable<string> Directors => string.IsNullOrEmpty(EpisodeDirector) ? Array.Empty<string>() : EpisodeDirector.Split('|').Where(s => s.HasValue());

    public CachedSeriesInfo TheCachedSeries
    {
        get
        {
            if (internalSeries != null)
            {
                return internalSeries;
            }

            throw new InvalidOperationException("Attempt to access Series for an Episode that has yet to be set");
        }
    }

    public bool Ok()
    {
        bool returnVal = EpisodeId != -1 && AiredEpNum != -1 && SeasonId != -1 && ReadAiredSeasonNum != -1;

        if (!returnVal)
        {
            Logger.Error("Issue with episode " + EpisodeId + " for cachedSeries " + SeriesId + " for EpNum " + AiredEpNum +
                         " for SeasonID " + SeasonId + " for ReadSeasonNum " + ReadAiredSeasonNum +
                         " for DVDSeasonNum " + ReadDvdSeasonNum);
        }

        return returnVal;
    }

    public void SetSeriesSeason(CachedSeriesInfo ser)
    {
        internalSeries = ser;
    }

    public int GetSeasonNumber(ProcessedSeason.SeasonType order)
    {
        return order switch
        {
            ProcessedSeason.SeasonType.dvd => DvdSeasonNumber,
            ProcessedSeason.SeasonType.aired => AiredSeasonNumber,
            ProcessedSeason.SeasonType.alternate => AiredSeasonNumber,
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
        };
    }

    public int GetEpisodeNumber(ProcessedSeason.SeasonType order)
    {
        return order switch
        {
            ProcessedSeason.SeasonType.dvd => DvdEpNum,
            ProcessedSeason.SeasonType.aired => AiredEpNum,
            ProcessedSeason.SeasonType.alternate => AiredEpNum,
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
        };
    }

    public IEnumerable<Actor> AllActors(CachedSeriesInfo si)
    {
        List<Actor> returnValue = si.GetActors().ToList();
        foreach (string star in GuestStars)
        {
            if (si.GetActors().All(actor => actor.ActorName != star))
            {
                returnValue.Add(new Actor(star));
            }
        }

        return returnValue;
    }

    public void SetWriters(List<string> writers)
    {
        if (writers.HasAny())
        {
            Writer = string.Join("|", writers);
        }
    }

    public void SetDirectors(List<string> directors)
    {
        if (directors.HasAny())
        {
            EpisodeDirector = string.Join("|", directors);
        }
    }

    public bool HasAired()
    {
        LocalDateTime? dateTime = GetAirDateDt();
        return dateTime.HasValue && dateTime.Value.InUtc().ToInstant().CompareTo(SystemClock.Instance.GetCurrentInstant()) < 0;
    }

    public bool IsSpecial(ProcessedSeason.SeasonType seasonOrderType)
    {
        return seasonOrderType switch
        {
            ProcessedSeason.SeasonType.dvd => 0 == DvdSeasonNumber,
            ProcessedSeason.SeasonType.aired => 0 == AiredSeasonNumber,
            ProcessedSeason.SeasonType.absolute => 0 == AiredSeasonNumber,
            ProcessedSeason.SeasonType.alternate => 0 == AiredSeasonNumber,
            _ => throw new ArgumentOutOfRangeException(nameof(seasonOrderType), seasonOrderType, null)
        };
    }
}
