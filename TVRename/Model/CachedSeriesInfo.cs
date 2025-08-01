//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename;

public class CachedSeriesInfo : CachedMediaInfo
{
    public DateTime? AirsTime;
    public string? AirsDay;
    public string? SeriesType;
    public ProcessedSeason.SeasonType SeasonOrderType;

    private ConcurrentDictionary<int, Episode> sourceEpisodes;

    public ICollection<Episode> Episodes => sourceEpisodes.Values;

    public void ClearEpisodes() => sourceEpisodes.Clear();

    private ShowImages images = [];

    public int? MinYear =>
        Episodes
            .Where(e => !e.IsSpecial(SeasonOrderType))
            .Select(e => e.Year)
            .Where(adt => adt.HasValue)
            .MinOrNull(adt => adt!.Value)
            ;

    public int? MaxYear =>
        Episodes
            .Where(e => !e.IsSpecial(SeasonOrderType))
            .Select(e => e.Year)
            .Where(adt => adt.HasValue)
            .MaxOrNull(adt => adt!.Value)
            ;

    public string Year => FirstAired?.Year.ToString() ?? $"{MinYear}";

    public IEnumerable<Season> Seasons => seasons;

    // note: "SeriesID" in a <Series> is the tv.com code,
    // "seriesid" in an <Episode> is the tvdb code!

    private CachedSeriesInfo(TVDoc.ProviderType source) : base(source)
    {
        sourceEpisodes = new ConcurrentDictionary<int, Episode>();
        AirsTime = null;
    }

    public CachedSeriesInfo(int tvdb, int tvmaze, int tmdb, Locale langCode, TVDoc.ProviderType source) : base(tvdb, tvmaze, tmdb, langCode, source)
    {
        sourceEpisodes = new ConcurrentDictionary<int, Episode>();
        AirsTime = null;
    }

    public CachedSeriesInfo(Locale locale, TVDoc.ProviderType source) : base(locale, source)
    {
        sourceEpisodes = new ConcurrentDictionary<int, Episode>();
        AirsTime = null;
    }

    public CachedSeriesInfo(XElement seriesXml, TVDoc.ProviderType source) : this(source)
    {
        LoadXml(seriesXml);
        IsSearchResultOnly = false;
    }

    public CachedSeriesInfo(JObject json, Locale locale, bool searchResult, TVDoc.ProviderType source) : this(locale, source)
    {
        LoadJson(json);
        IsSearchResultOnly = searchResult;

        if (string.IsNullOrEmpty(Name))
        {
            LOGGER.Warn("Issue with cachedSeries " + this);
            LOGGER.Warn(json.ToString());
        }

        if (SrvLastUpdated == 0 && !searchResult)
        {
            LOGGER.Warn("Issue with cachedSeries (update time is 0) " + this);
            LOGGER.Warn(json.ToString());
            SrvLastUpdated = 100;
        }
    }

    public CachedSeriesInfo(JObject json, JObject jsonInDefaultLang, Locale locale, TVDoc.ProviderType source) : this(locale, source)
    {
        LoadJson(json, jsonInDefaultLang);
        IsSearchResultOnly = false;
        if (string.IsNullOrEmpty(Name))
        {
            LOGGER.Warn("Issue with cachedSeries " + this);
            LOGGER.Warn(json.ToString());
            LOGGER.Info(jsonInDefaultLang.ToString());
        }

        if (SrvLastUpdated == 0)
        {
            LOGGER.Warn("Issue with cachedSeries (update time is 0) " + this);
            LOGGER.Warn(json.ToString());
            LOGGER.Info(jsonInDefaultLang.ToString());
            SrvLastUpdated = 100;
        }
    }

    // ReSharper disable once FunctionComplexityOverflow
    public void Merge(CachedSeriesInfo o)
    {
        if (o.IsSearchResultOnly && !IsSearchResultOnly)
        {
            return;
        }

        if (o.TvdbCode != TvdbCode && o.TvMazeCode != TvMazeCode && o.TmdbCode != TmdbCode)
        {
            return; // that's not us!
        }

        if (o.TvMazeCode != -1 && TvMazeCode != o.TvMazeCode)
        {
            TvMazeCode = o.TvMazeCode;
        }
        if (o.TmdbCode != -1 && TmdbCode != o.TmdbCode)
        {
            TmdbCode = o.TmdbCode;
        }
        if (o.TvdbCode != -1 && TvdbCode != o.TvdbCode)
        {
            TvdbCode = o.TvdbCode;
        }

        if (o.SrvLastUpdated != 0 && o.SrvLastUpdated < SrvLastUpdated)
        {
            return; // older!?
        }

        if (!o.IsSearchResultOnly)
        {
            IsSearchResultOnly = false;
        }

        bool currentLanguageNotSet = ActualLocale is null;
        //            Language optimaLanguage = config o.ActualLocale ?? TVSettings.Instance.PreferredTVDBLanguage;
        // bool newLanguageOptimal = o.ActualLocale.PreferredLanguage == optimaLanguage;
        bool useNewDataOverOld = currentLanguageNotSet || o.SrvLastUpdated >= SrvLastUpdated; //TODO - work out cached language and see what's best || newLanguageOptimal;

        SrvLastUpdated = o.SrvLastUpdated;

        MergeCommon(o, useNewDataOverOld);

        AirsDay = ChooseBetter(AirsDay, useNewDataOverOld, o.AirsDay);
        SeriesType = ChooseBetter(SeriesType, useNewDataOverOld, o.SeriesType);
        SeasonOrderType = o.SeasonOrderType;

        bool useNewSeasons = o.seasons.HasAny() && useNewDataOverOld;
        if (!seasons.HasAny() || useNewSeasons)
        {
            seasons = o.seasons;
        }

        if (o.AirsTime != null)
        {
            AirsTime = o.AirsTime;
        }

        if (!o.sourceEpisodes.IsEmpty)
        {
            sourceEpisodes = o.sourceEpisodes;
        }

        images.MergeImages(o.images);
    }
    private void LoadXml(XElement seriesXml)
    {
        LoadCommonXml(seriesXml);

        try
        {
            string airsTimeString = seriesXml.ExtractStringOrNull("Airs_Time") ?? seriesXml.ExtractString("airsTime");
            AirsTime = JsonHelper.ParseAirTime(airsTimeString);

            AirsDay = seriesXml.ExtractStringOrNull("airsDayOfWeek") ?? seriesXml.ExtractString("Airs_DayOfWeek");
            BannerString = seriesXml.ExtractStringOrNull("banner") ?? seriesXml.ExtractString("Banner");
            SeriesType = seriesXml.ExtractString("Type");
            SeasonOrderType = seriesXml.ExtractEnum("SeasonOrderType", ProcessedSeason.SeasonType.aired);
            LoadSeasons(seriesXml);
            LoadImages(seriesXml);
        }
        catch (SourceConsistencyException e)
        {
            LOGGER.Error(e, GenerateErrorMessage());
            // ReSharper disable once PossibleIntendedRethrow
            throw;
        }
    }

    internal void AddBanners(IEnumerable<ShowImage> enumerable)
    {
        foreach (ShowImage s in enumerable)
        {
            images.Add(s);
        }
    }

    private void LoadSeasons(XElement seriesXml)
    {
        seasons = [.. seriesXml.Descendants("Seasons").Descendants("Season").Select(xml => new Season(xml))];
    }

    private void LoadImages(XElement seriesXml)
    {
        images = [.. seriesXml.Descendants("Images").Descendants("ShowImage").Select(xml => new ShowImage(Source, xml))];
    }

    private void LoadJson(JObject r)
    {
        AirsDay = ((string?)r["airsDayOfWeek"])?.Trim();
        string? airsTimeString = (string?)r["airsTime"];
        AirsTime = JsonHelper.ParseAirTime(airsTimeString);
        Aliases = (r["aliases"] ?? throw new SourceConsistencyException($"Can't find aliases in Series JSON: {r}", TVDoc.ProviderType.TheTVDB)).Select(x => x.Value<string>()).OfType<string>().ToSafeList();
        BannerString = (string?)r["banner"];
        FirstAired = JsonHelper.ParseFirstAired((string?)r["firstAired"]);

        if (r.TryGetValue("genre", out JToken? value))
        {
            Genres = value.Select(x => x.Value<string>()?.Trim()).OfType<string>().Distinct().ToSafeList();
        }

        TvdbCode = r.GetMandatoryInt("id", TVDoc.ProviderType.TheTVDB);
        Imdb = ((string?)r["imdbId"])?.Trim();
        Network = ((string?)r["network"])?.Trim();
        Slug = ((string?)r["slug"])?.Trim();
        Overview = System.Web.HttpUtility.HtmlDecode((string?)r["overview"])?.Trim();
        ContentRating = ((string?)r["rating"])?.Trim();
        Runtime = ((string?)r["runtime"])?.Trim();
        SeriesId = (string?)r["seriesId"];
        string? s = (string?)r["seriesName"];
        if (s != null)
        {
            Name = System.Web.HttpUtility.HtmlDecode(s).Trim();
        }
        Status = (string?)r["status"];

        SrvLastUpdated = long.TryParse((string?)r["lastUpdated"], out long updateTime) ? updateTime : 0;

        string? siteRatingString = (string?)r["siteRating"];
        float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRating);

        string? siteRatingVotesString = (string?)r["siteRatingCount"];
        int.TryParse(siteRatingVotesString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRatingVotes);
    }

    /// <exception cref="EpisodeNotFoundException">Condition.</exception>
    internal Episode GetEpisode(int epId)
    {
        if (sourceEpisodes.TryGetValue(epId, out Episode? returnValue))
        {
            return returnValue;
        }
        throw new EpisodeNotFoundException();
    }

    private void LoadJson(JObject bestLanguageR, JObject backupLanguageR)
    {
        //Here we have two pieces of JSON. One in local language and one in the default language (English).
        //We will populate with the best language frst and then fill in any gaps with the backup Language
        LoadJson(bestLanguageR);

        //backupLanguageR should be a cachedSeries of name/value pairs (ie a JArray of JPropertes)
        //TVDB asserts that name and overview are the fields that are localised

        string? s = (string?)backupLanguageR["seriesName"];
        if (string.IsNullOrWhiteSpace(Name) && s != null)
        {
            Name = System.Web.HttpUtility.HtmlDecode(s);
        }

        string? o = (string?)backupLanguageR["overview"];
        if (string.IsNullOrWhiteSpace(Overview) && o != null)
        {
            Overview = System.Web.HttpUtility.HtmlDecode(o);
        }

        //Looking at the data then the aliases, banner and runtime are also different by language

        if (!Aliases.HasAny())
        {
            JToken aliasesToken = backupLanguageR["aliases"] ?? throw new SourceConsistencyException($"Can not find aliases in {backupLanguageR}", TVDoc.ProviderType.TheTVDB);

            Aliases = aliasesToken.Select(x => x.Value<string>()).OfType<string>().ToSafeList();
        }

        if (string.IsNullOrWhiteSpace(Runtime))
        {
            Runtime = (string?)backupLanguageR["runtime"];
        }

        if (string.IsNullOrWhiteSpace(BannerString))
        {
            BannerString = (string?)backupLanguageR["banner"];
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Series");
        WriteCommonFields(writer);

        writer.WriteElement("airsDayOfWeek", AirsDay);
        writer.WriteElement("Airs_Time", AirsTime?.ToString("HH:mm"), true);
        writer.WriteElement("Type", SeriesType, true);
        writer.WriteElement("SeasonOrderType", (int)SeasonOrderType);

        writer.WriteStartElement("Seasons");
        foreach (Season a in seasons)
        {
            a.WriteXml(writer);
        }
        writer.WriteEndElement(); //Seasons

        writer.WriteStartElement("Episodes");
        foreach (Episode e in Episodes)
        {
            writer.WriteEpisodeXml(e);
        }
        writer.WriteEndElement(); //Episodes

        writer.WriteStartElement("Images");
        foreach (ShowImage i in images)
        {
            i.WriteXml(writer);
        }
        writer.WriteEndElement(); //Images

        writer.WriteEndElement(); // cachedSeries
    }

    public string? GetSeriesFanartPath() => FanartUrl.HasValue() ? FanartUrl : images.GetShowImage(TargetLocale.LanguageToUse(Source), MediaImage.ImageType.background)?.ImageUrl;

    public string? GetSeriesPosterPath() => PosterUrl.HasValue() ? PosterUrl : images.GetShowImage(TargetLocale.LanguageToUse(Source), MediaImage.ImageType.poster)?.ImageUrl;

    public string? GetImage(TVSettings.FolderJpgIsType itemForFolderJpg) => images.GetImage(itemForFolderJpg, TargetLocale.LanguageToUse(Source))?.ImageUrl;

    public string? GetSeasonBannerPath(int snum) => images.GetSeasonBanner(snum, TargetLocale.LanguageToUse(Source))?.ImageUrl;

    public string? GetSeriesWideBannerPath() => images.GetShowImage(TargetLocale.LanguageToUse(Source), MediaImage.ImageType.wideBanner)?.ImageUrl;

    public string? GetSeasonWideBannerPath(int snum) => images.GetSeasonWideBanner(snum, TargetLocale.LanguageToUse(Source))?.ImageUrl;

    public void UpdateBanners(List<int> latestBannerIds)
    {
        List<int> bannersToRemove = [];
        foreach (ShowImage currentImage in images)
        {
            if (latestBannerIds.Contains(currentImage.Id))
            {
                continue;
            }

            bannersToRemove.Add(currentImage.Id);
        }

        foreach (int removeBanner in bannersToRemove)
        {
            images.RemoveAll(x => x.Id == removeBanner);
        }
    }

    public void AddEpisode(Episode episode)
    {
        sourceEpisodes.AddOrUpdate(episode.EpisodeId, episode, (_, _) => episode);
        episode.SetSeriesSeason(this);
    }

    public void RemoveEpisode(int episodeId)
    {
        sourceEpisodes.TryRemove(episodeId, out Episode? _);
    }

    protected override MediaConfiguration.MediaType MediaType() => MediaConfiguration.MediaType.tv;

    public override string ToString() => $"TVDB:{TvdbCode}/Maze:{TvMazeCode}/TMDB:{TmdbCode}/{Name}";

    public override ProcessedSeason.SeasonType SeasonOrder => SeasonOrderType;

    private List<Season> seasons = [];

    public void AddSeason(Season generateSeason)
    {
        seasons.Add(generateSeason);
    }

    public Season? Season(int sSeasonNumber)
    {
        return seasons.FirstOrDefault(season => season.SeasonNumber == sSeasonNumber);
    }

    public bool IsCacheFor(ShowConfiguration show) => show.TmdbCode == TmdbCode || show.TvdbId == TvdbCode || show.TvMazeId == TvMazeCode;

    public void AddOrUpdateImage(ShowImage showImage)
    {
        images.RemoveAll(s => s.Id == showImage.Id);
        images.Add(showImage);
    }

    public IEnumerable<ShowImage> Images(MediaImage.ImageType type)
    {
        Language languageToUse = TargetLocale.LanguageToUse(Source);
        return images
            .Where(x => x.ImageStyle == type)
            .Where(x => x.LocationMatches(languageToUse));
    }

    public IEnumerable<ShowImage> Images(MediaImage.ImageType type, MediaImage.ImageSubject subject)
    {
        return Images(type).Where(x => x.Subject == subject);
    }

    internal IEnumerable<ShowImage> Images(MediaImage.ImageType type, MediaImage.ImageSubject subject, int seasonNumber)
    {
        return Images(type, subject).Where(x => x.SeasonNumber == seasonNumber);
    }
}
