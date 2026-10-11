//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NodaTime;
using System.Globalization;

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

    public Episode(XElement r) 
    {
        // </Episode>
        //  blah blah
        //  <id>...</id>
        // <Episode>
        SeriesId = r.ExtractInt("seriesid", -1); // key to the cachedSeries
        EpisodeId = r.ExtractInt("id", -1);
        SeasonId = r.ExtractInt("airedSeasonID") ?? r.ExtractInt("seasonid", -1);
        AiredEpNum = r.ExtractInt("airedEpisodeNumber") ?? r.ExtractInt("EpisodeNumber", -1);
        DvdEpNum = ExtractAndParse(r, "dvdEpisodeNumber");
        ReadAiredSeasonNum = ExtractAndParse(r, "SeasonNumber");
        ReadDvdSeasonNum = ExtractAndParse(r, "dvdSeason");
        SrvLastUpdated = r.ExtractLong("lastupdated", -1);
        Overview = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Overview")));
        LinkUrl = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("LinkURL"));
        Runtime = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Runtime"));
        EpisodeRating = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Rating"));
        EpisodeGuestStars = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("GuestStars"));
        EpisodeDirector = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("EpisodeDirector"));
        Writer = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Writer"));
        MName = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("EpisodeName")));

        AirStamp = r.ExtractDateTime("AirStamp");
        AirTime = JsonHelper.ParseAirTime(r.ExtractString("Airs_Time"));

        DvdDiscId = r.ExtractString("DvdDiscId");
        Filename = r.ExtractStringOrNull("Filename") ?? r.ExtractString("filename");
        ShowUrl = r.ExtractStringOrNull("ShowUrl") ?? r.ExtractString("showUrl");
        ImdbCode = r.ExtractStringOrNull("ImdbCode") ?? r.ExtractStringOrNull("IMDB_ID") ?? r.ExtractString("imdbId");
        ProductionCode = r.ExtractStringOrNull("ProductionCode") ?? r.ExtractString("productionCode");

        DvdChapter = r.ExtractInt("DvdChapter") ?? r.ExtractInt("dvdChapter");
        AirsBeforeSeason = r.ExtractInt("AirsBeforeSeason") ?? r.ExtractInt("airsBeforeSeason") ?? r.ExtractInt("airsbefore_season");
        AirsBeforeEpisode = r.ExtractInt("AirsBeforeEpisode") ?? r.ExtractInt("airsBeforeEpisode") ?? r.ExtractInt("airsbefore_episode");
        AirsAfterSeason = r.ExtractInt("AirsAfterSeason");
        SiteRatingCount = r.ExtractInt("SiteRatingCount") ?? r.ExtractInt("siteRatingCount");
        AbsoluteNumber = r.ExtractInt("AbsoluteNumber");
        FirstAired = JsonHelper.ParseFirstAired(r.ExtractString("FirstAired"));
    }


    public LocalDateTime? LocalAirTime()
    {
        DateTime? fa = FirstAired;

        if (fa is null)
        {
            return null;
        }
        DateTime? airs = internalSeries?.AirsTime ?? AirStamp;

        int defaultHour = (internalSeries?.Network).IsStreamingService()
            ? 0
            : 20;

        return new LocalDateTime(fa.Value.Year, fa.Value.Month, fa.Value.Day, airs?.Hour ?? defaultHour, airs?.Minute ?? 0);
    }

    public DateTime? GetAirDateDt(DateTimeZone tz)
    {
        LocalDateTime? dt = LocalAirTime();
        if (dt is null)
        {
            return null;
        }

        return TimeZoneHelper.AdjustTzTimeToLocalTime(dt.Value, tz);
    }

    public Episode(int seriesId, CachedSeriesInfo si) : this()
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

    public IEnumerable<string> GuestStars => EpisodeGuestStars.FromPsv();

    public IEnumerable<string> Writers => Writer.FromPsv();

    public IEnumerable<string> Directors => EpisodeDirector.FromPsv();

    /// <exception cref="InvalidOperationException" accessor="get">Attempt to access Series for an Episode that has yet to be set</exception>
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

    public int? Year => LocalAirTime()?.Year;
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
        List<Actor> returnValue = [.. si.GetActors()];
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
            Writer = writers.ToPsv();
        }
    }

    public void SetDirectors(List<string> directors)
    {
        if (directors.HasAny())
        {
            EpisodeDirector = directors.ToPsv();
        }
    }

    public bool HasAired()
    {
        LocalDateTime? dateTime = LocalAirTime();
        return dateTime.HasValue && dateTime.Value.ToDateTimeUnspecified().IsInPast();
    }

    public bool IsSpecial(ProcessedSeason.SeasonType seasonOrderType)
    {
        return seasonOrderType switch
        {
            ProcessedSeason.SeasonType.dvd => DvdSeasonNumber == 0,
            ProcessedSeason.SeasonType.aired => AiredSeasonNumber == 0,
            ProcessedSeason.SeasonType.absolute => AiredSeasonNumber == 0,
            ProcessedSeason.SeasonType.alternate => AiredSeasonNumber == 0,
            _ => throw new ArgumentOutOfRangeException(nameof(seasonOrderType), seasonOrderType, null)
        };
    }

    internal XElement GenerateEpisodeXElement()
    {
        return new XElement("Episode",
            new XElement("id", EpisodeId),
            new XElement("seriesid", SeriesId),
            new XElement("airedSeasonID", SeasonId),
            new XElement("airedEpisodeNumber", AiredEpNum),
            new XElement("SeasonNumber", AiredSeasonNumber),
            XmlHelper.GenerateOptionalXElement("dvdEpisodeNumber", DvdEpNum),
            XmlHelper.GenerateOptionalXElement("dvdSeason", DvdSeasonNumber),
            new XElement("lastupdated", SrvLastUpdated),
            XmlHelper.GenerateOptionalXElement("Overview", Overview?.Trim()),
            new XElement("LinkURL", LinkUrl?.Trim()),
            new XElement("Runtime", Runtime?.Trim()),
            new XElement("Rating", EpisodeRating),
            XmlHelper.GenerateOptionalXElement("GuestStars", EpisodeGuestStars),
            XmlHelper.GenerateOptionalXElement("EpisodeDirector", EpisodeDirector),
            XmlHelper.GenerateOptionalXElement("Writer", Writer),
            new XElement("EpisodeName", MName),
            XmlHelper.GenerateOptionalXElement("FirstAired", FirstAired?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
            XmlHelper.GenerateOptionalXElement("AirTime", AirTime?.ToString("HH:mm")),
            XmlHelper.GenerateOptionalXElement("AirStamp", AirStamp),
            XmlHelper.GenerateOptionalXElement("DvdChapter", DvdChapter),
            XmlHelper.GenerateOptionalXElement("DvdDiscId", DvdDiscId),
            XmlHelper.GenerateOptionalXElement("AirsBeforeSeason", AirsBeforeSeason),
            XmlHelper.GenerateOptionalXElement("AirsBeforeEpisode", AirsBeforeEpisode),
            XmlHelper.GenerateOptionalXElement("AirsAfterSeason", AirsAfterSeason),
            XmlHelper.GenerateOptionalXElement("SiteRatingCount", SiteRatingCount),
            XmlHelper.GenerateOptionalXElement("AbsoluteNumber", AbsoluteNumber),
            XmlHelper.GenerateOptionalXElement("ProductionCode", ProductionCode),
            XmlHelper.GenerateOptionalXElement("ImdbCode", ImdbCode),
            XmlHelper.GenerateOptionalXElement("ShowUrl", ShowUrl),
            XmlHelper.GenerateOptionalXElement("Filename", Filename)
        );
    }

    private static int ExtractAndParse(XElement r, string key)
    {
        string value = r.ExtractString(key);
        return int.TryParse(value, out int intValue) ? intValue : -1;
    }
}
