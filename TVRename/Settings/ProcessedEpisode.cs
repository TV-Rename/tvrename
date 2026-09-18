using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TVRename;

public class ProcessedEpisode : Episode, IComparable<ProcessedEpisode>, INotifyPropertyChanged, IEquatable<ProcessedEpisode>
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

    public virtual string SeriesName => Forms.UI.GenerateShowUIName(this);
    public virtual DateTime? AirDate => GetAirDateDt();
    public virtual string AirDateString => GetAirDateDt()?.ToShortDateString() ?? string.Empty;
    public virtual string Time => GetAirDateDt()?.ToString("t") ?? string.Empty;
    public virtual string Day => GetAirDateDt()?.ToString("ddd") ?? string.Empty;
    public virtual string Length => HowLong();
    public virtual string Network => TheCachedSeries.Networks.FirstOrDefault() ?? string.Empty;
    public virtual int ImageTypeName
    {
        get
        {
            if (TVSettings.Instance.IgnorePreviouslySeen && PreviouslySeen && !(_airedStatus==FoundStatus.OnDisk))
            {
                _airedStatus = FoundStatus.PreviouslySeen;
                NotifyPropertyChanged("ImageTypeName");
            }
            if (!HasAired() && (_airedStatus == FoundStatus.Unknown || _airedStatus == FoundStatus.Missing))
            {
                _airedStatus = FoundStatus.Future;
                NotifyPropertyChanged("ImageTypeName");
            }

            return Forms.UI.ChooseWtwIcon(_airedStatus);
        }
    }

    private FoundStatus _airedStatus = FoundStatus.Unknown;
    public enum FoundStatus
    {
        OnDisk,
        PreviouslySeen,
        Missing,
        Future,
        Unknown
    }

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
        TheAiredProcessedSeason = si.GetOrAddSeason(e.AiredSeasonNumber, e.SeasonId, ProcessedSeason.SeasonType.aired);
        TheDvdProcessedSeason = si.GetOrAddSeason(e.DvdSeasonNumber, e.SeasonId, ProcessedSeason.SeasonType.dvd);
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

    public event PropertyChangedEventHandler? PropertyChanged;

    // This method is called by the Set accessor of each property.
    // The CallerMemberName attribute that is applied to the optional propertyName
    // parameter causes the property name of the caller to be substituted as an argument.
    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    internal async Task UpdateSeenStatusAsync(DirFilesCache dfc)
    {
        List<FileInfo> fl = await dfc.FindEpOnDiskAsync(this);
        bool appropriateFileNameFound = !TVSettings.Instance.RenameCheck
                                        || !Show.DoRename
                                        || fl.All(file => file.Name.StartsWith(TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(this)), StringComparison.OrdinalIgnoreCase));

        if (fl.Any() && appropriateFileNameFound)
        {
            _airedStatus = ProcessedEpisode.FoundStatus.OnDisk;
            NotifyPropertyChanged("ImageTypeName");
            return;
        }

        if (TVSettings.Instance.IgnorePreviouslySeen && PreviouslySeen)
        {
            _airedStatus = ProcessedEpisode.FoundStatus.PreviouslySeen;
            NotifyPropertyChanged("ImageTypeName");
            return;

        }

        if (HasAired())
        {
            if (Show.DoMissingCheck)
            {
                _airedStatus = ProcessedEpisode.FoundStatus.Missing;
                NotifyPropertyChanged("ImageTypeName");
                return;

            }
        }

        _airedStatus = ProcessedEpisode.FoundStatus.Future;
        NotifyPropertyChanged("ImageTypeName");
    }

    bool IEquatable<ProcessedEpisode>.Equals(ProcessedEpisode? other)
    {
        throw new NotImplementedException();
    }

    int IComparable<ProcessedEpisode>.CompareTo(ProcessedEpisode? other)
    {
        throw new NotImplementedException();
    }
}
