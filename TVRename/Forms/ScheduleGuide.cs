//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Web;
using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;

namespace TVRename.Forms;

// The Schedule's Guide tab: the list's episodes as weeks of day columns, one tile for each row of the list.
// Every week is in the page, so paging happens in the page; the details panel has a fixed height so the columns never move.
public class ScheduleGuide : UserControl
{
    private const int DAYS_SHOWN = 7;
    private const string DATE_FORMAT = "yyyy-MM-dd";
    private const int FONT_SIZE = 12;
    private const string DIV_END = "</div>";
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // How much each tile shows; the show name and episode number are always shown
    public enum TileLayout
    {
        Full,
        NoBanners,
        Compact
    }

    private static readonly (TileLayout layout, string text)[] TileChoices =
    [
        (TileLayout.Full, "Full tiles"),
        (TileLayout.NoBanners, "No banners"),
        (TileLayout.Compact, "Compact (show and episode only)")
    ];

    private static Font? boldMenuFont;

    private TileLayout tiles = TileLayout.Full;
    private int hiddenByFilter;

    private readonly ChromiumWebBrowser browser;
    private List<ProcessedEpisode> episodes = [];
    private ProcessedEpisode? selected;
    private DateTime? pickedDay;
    private int week; // 0 is the week starting yesterday
    private int pageNumber; // a click on an older page must not act on a newer episode list
    private int readyPage; // the page whose script is running; calls made while a page loads would be lost
    private int fileLookup; // only the latest file lookup updates the page
    private bool needsRender = true;
    private bool syncWhenReady; // the selection or calendar day changed while the page was loading
    private string renderedSignature = string.Empty; // what the page shows; reloading redraws every picture, so only reload for a change
    private string iconCss = string.Empty;

    public event EventHandler<ProcessedEpisode>? EpisodeSelected;
    public event EventHandler<ProcessedEpisode>? EpisodeOpened;
    public event EventHandler<ProcessedEpisode>? EpisodeRightClicked;
    public event EventHandler<ProcessedEpisode>? EpisodeSearched;
    public event EventHandler? FilterCleared;
    public event EventHandler? MenuDismissed; // the page runs on the browser's thread, so an open menu never sees its clicks

    // The list's icons, so a tile shows the same status as its row
    [DefaultValue(null)]
    public ImageList? StatusIcons { get; set; }

    private bool PageReady => !IsDisposed && browser.IsBrowserInitialized && readyPage == pageNumber;

    private bool PageLoading => !IsDisposed && browser.IsBrowserInitialized && !needsRender && readyPage != pageNumber;

    // Stored in Layout.xml by name, for example "NoBanners"
    public string TilesText() => tiles.ToString();

    public void SetTiles(string? text)
    {
        tiles = Enum.TryParse(text, out TileLayout parsed) && Enum.IsDefined(parsed) ? parsed : TileLayout.Full;
        RenderWhenVisible();
    }

    // The current choice in the Guide's menus is bold; one font serves them all
    internal static Font BoldMenuFont(Font menuFont) => boldMenuFont ??= new Font(menuFont, FontStyle.Bold);

    // A plain list like the search menu
    public void FillTilesMenu(ToolStripItemCollection items)
    {
        items.Clear();
        foreach ((TileLayout layout, string text) in TileChoices)
        {
            ToolStripMenuItem item = new(text);
            items.Add(item);
            if (layout == tiles)
            {
                item.Font = BoldMenuFont(item.Font);
            }
            item.Click += (_, _) =>
            {
                tiles = layout;
                RenderWhenVisible();
            };
        }
    }

    public ScheduleGuide()
    {
        browser = new ChromiumWebBrowser { Dock = DockStyle.Fill };
        browser.RequestHandler = new BrowserRequestHandler();
        browser.JavascriptMessageReceived += Browser_JavascriptMessageReceived;
        // Pages are only loaded once the browser is ready: a load asked for earlier is queued and could land after a newer one
        browser.IsBrowserInitializedChanged += (_, _) => this.SafeInvoke(() =>
        {
            if (Visible && needsRender)
            {
                Render();
            }
        }, false);
        Controls.Add(browser);
    }

    // hidden is how many of the schedule's episodes the list's filters leave out
    public void SetEpisodes(IEnumerable<ProcessedEpisode> eps, ProcessedEpisode? selectedEpisode, int hidden)
    {
        hiddenByFilter = hidden;
        episodes = [.. eps.Where(ep => ep.GetAirDateDt().HasValue).OrderBy(ep => ep.GetAirDateDt()).ThenBy(ep => ep.SeriesName).ThenBy(ep => ep.AppropriateEpNum)];

        // The list's selection, or else the episode already shown, found again in the new episode list
        ProcessedEpisode? wanted = episodes.FirstOrDefault(ep => ep.Equals(selectedEpisode)) ?? episodes.FirstOrDefault(ep => ep.Equals(selected));

        // Same episodes as the page shows or is loading: at most the selection moved
        bool samePage = Signature() == renderedSignature && (PageReady || PageLoading);
        if (samePage && wanted is not null && !wanted.Equals(selected))
        {
            SelectTile(episodes.IndexOf(wanted));
        }

        selected = wanted;
        if (!samePage)
        {
            RenderWhenVisible();
        }
    }

    // Revert View: back to this week with nothing picked, on full tiles; drawn by the SetEpisodes that follows the list's revert
    public void Revert()
    {
        tiles = TileLayout.Full;
        week = 0;
        pickedDay = null;
        selected = null;
        renderedSignature = string.Empty;
        needsRender = true;
    }

    private void SelectTile(int index)
    {
        if (PageReady)
        {
            browser.ExecuteScriptAsync("selectTile", index);
        }
        else
        {
            syncWhenReady = true;
        }
    }

    // Everything the page draws from, so an unchanged schedule (a periodic refresh, switching views) keeps the page as it is
    private string Signature()
    {
        StringBuilder sb = new();
        sb.Append(DateTime.Today.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)).Append(ThemeManager.ActiveTheme?.Key)
          .Append(TVSettings.Instance.UseColoursOnWtw).Append(tiles).Append(hiddenByFilter);
        foreach (ProcessedEpisode ep in episodes)
        {
            sb.Append(';').Append(ep.SeriesName).Append('|').Append(ep.AppropriateSeasonNumber).Append('|').Append(ep.EpisodeNumbersAsText)
              .Append('|').Append(ep.GetAirDateDt()?.ToString("O")).Append('|').Append(ep.AiredStatus).Append('|').Append(ep.Name)
              .Append('|').Append(ep.HowLong()).Append('|').Append(ep.Network).Append('|').Append(ep.Show.WideBannerUrl());
        }
        return sb.ToString();
    }

    // Used by the calendar: show the week holding that day and outline the day
    public void ShowDay(DateTime day)
    {
        pickedDay = day.Date;
        week = WeekOf(day);
        if (PageReady)
        {
            browser.ExecuteScriptAsync("showDay", day.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));
            return;
        }
        if (PageLoading)
        {
            syncWhenReady = true;
            return;
        }
        RenderWhenVisible();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Visible && needsRender && browser.IsBrowserInitialized)
        {
            Render();
        }
    }

    private static DateTime FirstDay => DateTime.Today.AddDays(-1);

    private static int WeekOf(DateTime day) => (int)Math.Floor((day.Date - FirstDay).TotalDays / DAYS_SHOWN);

    private static DateTime AirTime(ProcessedEpisode ep) => ep.GetAirDateDt() ?? DateTime.MaxValue;

    private void RenderWhenVisible()
    {
        needsRender = true;
        if (Visible && browser.IsBrowserInitialized)
        {
            Render();
        }
    }

    private void Render()
    {
        needsRender = false;
        syncWhenReady = false;
        pageNumber++;
        renderedSignature = Signature();
        browser.SetHtmlBody(GuideHtml());
    }

    private void Browser_JavascriptMessageReceived(object? sender, JavascriptMessageReceivedEventArgs e)
    {
        // Messages are "page|action|value"
        if ((e.Message as string ?? string.Empty).Split('|') is not [var pageText, var action, var valueText]
            || !int.TryParse(pageText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int page)
            || !int.TryParse(valueText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
        {
            return;
        }

        this.SafeInvoke(() => _ = ActAsync(page, action, value), false);
    }

    private async Task ActAsync(int page, string action, int value)
    {
        if (page != pageNumber)
        {
            return;
        }

        try
        {
            await ActOnPageAsync(page, action, value);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Schedule guide could not handle '{action}'");
        }
    }

    private async Task ActOnPageAsync(int page, string action, int value)
    {
        switch (action)
        {
            case "ready":
                readyPage = page;
                SyncPage();
                await ShowNetworkLogosAsync();
                break;

            case "week":
                week = value;
                break;

            case "paged":
                week = value;
                pickedDay = null;
                break;

            case "clearfilter":
                FilterCleared?.Invoke(this, EventArgs.Empty);
                break;

            case "dismiss":
                MenuDismissed?.Invoke(this, EventArgs.Empty);
                break;

            default:
                if (value >= 0 && value < episodes.Count)
                {
                    await ActOnEpisodeAsync(episodes[value], action);
                }
                break;
        }
    }

    private async Task ActOnEpisodeAsync(ProcessedEpisode ep, string action)
    {
        if (action != "look")
        {
            selected = ep;
        }

        switch (action)
        {
            case "look":
                await ShowFileAsync(ep);
                break;

            case "select":
                EpisodeSelected?.Invoke(this, ep);
                await ShowFileAsync(ep);
                break;

            case "open":
                EpisodeOpened?.Invoke(this, ep);
                break;

            case "menu":
                EpisodeRightClicked?.Invoke(this, ep);
                await ShowFileAsync(ep);
                break;

            case "search":
                EpisodeSearched?.Invoke(this, ep);
                break;

            default:
                // Not an episode action
                break;
        }
    }

    // The file is looked up only for the episode in the details panel, so drawing the page reads nothing from disk
    private async Task ShowFileAsync(ProcessedEpisode ep)
    {
        int lookup = ++fileLookup;
        int page = pageNumber;
        FileInfo? file = (await FinderHelper.FindEpOnDiskAsync(null, ep)).FirstOrDefault();
        (string size, string dimensions) = file is null ? (string.Empty, string.Empty) : await FileDetailsAsync(file);

        if (lookup != fileLookup || page != pageNumber || !browser.IsBrowserInitialized)
        {
            return;
        }

        string link = file is null ? string.Empty : HttpUtility.UrlEncode(file.FullName);
        browser.ExecuteScriptAsync("setFile", page, file?.Name ?? string.Empty, size, dimensions,
            file is null ? string.Empty : UI.WATCH_PROXY + link,
            file is null ? string.Empty : UI.EXPLORE_PROXY + link);
    }

    private static async Task<(string size, string dimensions)> FileDetailsAsync(FileInfo file)
    {
        string size = string.Empty;
        try
        {
            size = file.Length.GBMB();
            (int? height, int? width, _) = await file.GetMovieDetailsAsync();
            return (size, width.HasValue && height.HasValue ? $"{width}x{height}" : string.Empty);
        }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
        {
            // The file went away or could not be read; still show its name
            return (size, string.Empty);
        }
    }

    // What changed while the page was loading
    private void SyncPage()
    {
        if (!syncWhenReady)
        {
            return;
        }

        syncWhenReady = false;
        if (selected is not null && episodes.Contains(selected))
        {
            browser.ExecuteScriptAsync("selectTile", episodes.IndexOf(selected));
        }
        if (pickedDay.HasValue)
        {
            browser.ExecuteScriptAsync("showDay", pickedDay.Value.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));
        }
    }

    // Logos come from TMDB once the page is ready; until then, and if TMDB has none, the network's name is shown
    private async Task ShowNetworkLogosAsync()
    {
        List<ProcessedEpisode> shows = [.. episodes.Where(ep => ep.Show.TmdbCode > 0 && ep.Network.HasValue()).DistinctBy(ep => ep.Show.TmdbCode)];
        ConcurrentDictionary<int, string> found = new();
        await Parallel.ForEachAsync(shows, new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, async (ep, token) =>
        {
            int id = ep.Show.TmdbCode!.Value;
            found[id] = await TMDB.LocalCache.GetNetworkLogoAsync(id, ep.Network, token);
        });

        // Back on the window's thread
        foreach ((int id, string logo) in found)
        {
            SetLogo(id, logo);
        }
    }

    private void SetLogo(int tmdbId, string logo)
    {
        if (logo.HasValue() && !IsDisposed && PageReady)
        {
            browser.ExecuteScriptAsync("setLogo", pageNumber, tmdbId, logo);
        }
    }

    private static string Network(ShowConfiguration show) => show.CachedShow?.Networks.FirstOrDefault() ?? string.Empty;

    private static string KnownLogo(ShowConfiguration show) =>
        show.TmdbCode is { } id ? TMDB.LocalCache.KnownNetworkLogo(id, Network(show)) : string.Empty;

    private string GuideHtml()
    {
        ProcessedEpisode? upNext = episodes.FirstOrDefault(ep => AirTime(ep) >= TimeHelpers.LocalNow());
        ProcessedEpisode? featured = selected ?? upNext;
        int firstWeek = Math.Min(0, episodes.Count == 0 ? 0 : WeekOf(AirTime(episodes[0])));
        int lastWeek = Math.Max(0, episodes.Count == 0 ? 0 : WeekOf(AirTime(episodes[^1])));
        List<ShowConfiguration> shows = [.. episodes.Select(ep => ep.Show).Distinct()];

        StringBuilder sb = new();
        sb.AppendLine(ShowHtmlHelper.HTMLHeader(FONT_SIZE));
        sb.AppendLine($"<style>{LayoutCss()}{ColourCss()}{IconCss()}</style>");

        sb.AppendLine("<div class=\"guide-top\">");
        AppendDetails(sb);
        AppendPaging(sb);
        sb.AppendLine(DIV_END);

        for (int w = firstWeek; w <= lastWeek; w++)
        {
            AppendWeek(sb, w, shows);
        }

        int featuredIndex = featured is null ? -1 : episodes.IndexOf(featured);
        string upNextLabel = featured is not null && featured.Equals(upNext) && selected is null ? "Up next" : string.Empty;
        // Invariant, as some cultures write a minus sign that the script can't read
        string picked = pickedDay?.ToString(DATE_FORMAT, CultureInfo.InvariantCulture) ?? string.Empty;
        sb.AppendLine(string.Create(CultureInfo.InvariantCulture,
            $"<script>var page={pageNumber},days={DAYS_SHOWN},firstWeek={firstWeek},lastWeek={lastWeek},startWeek={Math.Clamp(week, firstWeek, lastWeek)}"
            + $",firstDay='{FirstDay.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)}',picked='{picked}'"
            + $",featured={featuredIndex},featuredLabel='{upNextLabel}',shows={ShowsJson(shows)};</script>"));
        sb.AppendLine($"<script>{DetailsScript()}{NavigationScript()}</script>");
        sb.AppendLine(ShowHtmlHelper.HTMLFooter());
        return sb.ToString();
    }

    // What every tile of a show shares, written once so a long schedule keeps the page small
    private static string ShowsJson(List<ShowConfiguration> shows) =>
        JsonConvert.SerializeObject(shows.Select(show => new
        {
            poster = show.PosterUrl(),
            summary = show.CachedShow?.Overview?.ToUiVersion() ?? string.Empty,
            network = Network(show),
            tmdb = show.TmdbCode ?? 0,
            logo = KnownLogo(show)
        }), new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });

    // Filled in by the page script from the selected tile, or the next one to air
    private static void AppendDetails(StringBuilder sb)
    {
        sb.AppendLine("<div class=\"details\">");
        sb.AppendLine("<img id=\"dPoster\" class=\"poster\" alt=\"\" onerror=\"this.style.visibility='hidden'\" onload=\"this.style.visibility='visible'\">");
        sb.AppendLine("<div class=\"details-text\">");
        sb.AppendLine("<div class=\"details-title\"><span id=\"dLabel\" class=\"badge bg-secondary\"></span> <span id=\"dShow\">Nothing on the schedule</span> <span id=\"dStatus\"></span></div>");
        sb.AppendLine("<div id=\"dEpisode\" class=\"details-episode\"></div>");
        sb.AppendLine("<div class=\"details-airs\"><span id=\"dAirs\"></span> <img id=\"dLogo\" class=\"logo logo-lg\" alt=\"\" onerror=\"noLogo(current)\">"
                      + "<span id=\"dNetwork\" class=\"badge bg-primary\"></span> <span id=\"dHowLong\" class=\"text-muted\"></span></div>");
        sb.AppendLine("<div class=\"show-summary text-muted\"><span class=\"label\">About the show: </span><span id=\"dShowSummary\"></span></div>");
        sb.AppendLine("<div class=\"file-line\"><span id=\"dFile\" class=\"text-muted small\"></span>");
        sb.AppendLine("<a id=\"dWatch\" class=\"btn btn-sm btn-outline-secondary\"><i class=\"far fa-eye\"></i> Watch</a>");
        sb.AppendLine("<a id=\"dExplore\" class=\"btn btn-sm btn-outline-secondary\"><i class=\"far fa-folder-open\"></i> Open folder</a>");
        sb.AppendLine("<button id=\"dSearch\" class=\"btn btn-sm btn-outline-secondary\" onclick=\"send('search',current)\"><i class=\"fas fa-search\"></i> Search</button>");
        sb.AppendLine("</div></div></div>");
    }

    private void AppendPaging(StringBuilder sb)
    {
        sb.AppendLine("<div class=\"paging\">");
        sb.AppendLine("<div class=\"paging-buttons\">");
        sb.AppendLine("<button id=\"bEarlier\" class=\"btn btn-sm btn-outline-secondary\" onclick=\"showWeek(shown-1,'',true)\"></button>");
        sb.AppendLine("<button class=\"btn btn-sm btn-outline-secondary\" onclick=\"showWeek(0,'',true)\">Today</button>");
        sb.AppendLine("<button id=\"bLater\" class=\"btn btn-sm btn-outline-secondary\" onclick=\"showWeek(shown+1,'',true)\"></button>");
        sb.AppendLine(DIV_END);
        sb.AppendLine("<div id=\"range\" class=\"text-muted small mt-1\"></div>");
        if (hiddenByFilter > 0)
        {
            // A filter set in either view hides episodes here too, so say so
            string hiddenEpisodes = hiddenByFilter == 1 ? "episode" : "episodes";
            sb.AppendLine($"<div class=\"filter-note small mt-1\"><span class=\"badge bg-warning\" style=\"color:#212529\">Filtered</span> {hiddenByFilter} {hiddenEpisodes} hidden"
                          + " · <a href=\"#\" onclick=\"send('clearfilter',0);return false;\">Clear</a></div>");
        }
        sb.AppendLine(DIV_END);
    }

    private void AppendWeek(StringBuilder sb, int w, List<ShowConfiguration> shows)
    {
        DateTime first = FirstDay.AddDays(w * DAYS_SHOWN);
        DateTime end = first.AddDays(DAYS_SHOWN);
        int earlier = episodes.Count(ep => AirTime(ep) < first);
        int later = episodes.Count(ep => AirTime(ep) >= end);

        sb.AppendLine($"<div class=\"week\" id=\"w{w}\" data-range=\"{Encode(first.ToString("M"))} to {Encode(end.AddDays(-1).ToString("M"))}\" data-earlier=\"{earlier}\" data-later=\"{later}\">");
        sb.AppendLine("<div class=\"days day-heads\">");
        for (int d = 0; d < DAYS_SHOWN; d++)
        {
            DateTime day = first.AddDays(d);
            sb.AppendLine($"<div class=\"day-head {DayClass(day)}\" data-date=\"{day.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)}\">"
                          + $"<div class=\"day-name\">{Encode(DayName(day))}</div><div class=\"day-date\">{Encode(day.ToString("M"))}</div></div>");
        }
        sb.AppendLine(DIV_END);

        sb.AppendLine("<div class=\"days\">");
        for (int d = 0; d < DAYS_SHOWN; d++)
        {
            DateTime day = first.AddDays(d);
            List<int> onDay = [.. Enumerable.Range(0, episodes.Count).Where(i => AirTime(episodes[i]).Date == day)];
            sb.AppendLine("<div class=\"day\">");
            if (onDay.Count == 0)
            {
                sb.AppendLine("<div class=\"nothing text-muted small\">Nothing airing</div>");
            }
            foreach (int i in onDay)
            {
                AppendTile(sb, i, shows.IndexOf(episodes[i].Show));
            }
            sb.AppendLine(DIV_END);
        }
        sb.AppendLine("</div></div>");
    }

    private static string DayClass(DateTime day)
    {
        if (day == DateTime.Today)
        {
            return "today";
        }
        return day < DateTime.Today ? "past" : "future";
    }

    private void AppendTile(StringBuilder sb, int index, int showIndex)
    {
        ProcessedEpisode ep = episodes[index];
        string banner = ep.Show.WideBannerUrl();
        ProcessedEpisode.FoundStatus status = ep.AiredStatus;

        sb.AppendLine($"<div class=\"tile card{(ep.Equals(selected) ? " sel" : string.Empty)}\" data-i=\"{index}\" data-s=\"{showIndex}\"{TileData(ep, status)}>");
        if (banner.HasValue() && tiles == TileLayout.Full)
        {
            sb.AppendLine($"<img class=\"banner\" loading=\"lazy\" src=\"{Encode(banner)}\" alt=\"{Encode(ep.SeriesName)}\" onerror=\"this.remove()\">");
        }
        sb.AppendLine("<div class=\"tile-body\">");
        sb.AppendLine($"<div class=\"tile-show\">{StatusHtml(status, false)}{Encode(ep.SeriesName)}</div>");
        if (tiles == TileLayout.Compact)
        {
            sb.AppendLine($"<div class=\"tile-episode\">{Encode(EpisodeNumbers(ep))}</div>");
        }
        else
        {
            sb.AppendLine($"<div class=\"tile-episode\">{Encode(EpisodeTitle(ep))}</div>");
            sb.AppendLine($"<div class=\"tile-meta text-muted\">{TileMeta(ep)}</div>");
        }
        sb.AppendLine("</div></div>");
    }

    // What the details panel shows for this episode
    private string TileData(ProcessedEpisode ep, ProcessedEpisode.FoundStatus status)
    {
        DateTime airs = AirTime(ep);
        bool aired = airs <= TimeHelpers.LocalNow();
        string howLong = aired ? string.Empty : ep.HowLong();
        string airsText = $"{(aired ? "Aired" : "Airs")} {airs:ddd d MMM} at {ep.Time}{(ep.Network.HasValue() ? " on" : string.Empty)}";

        return $" data-show=\"{Encode(ep.SeriesName)}\" data-episode=\"{Encode(EpisodeTitle(ep))}\""
               + $" data-status=\"{Encode(StatusHtml(status, true))}\" data-airs=\"{Encode(airsText)}\""
               + $" data-howlong=\"{Encode(howLong.HasValue() ? "in " + howLong : string.Empty)}\""
               + $" data-aired=\"{(aired ? 1 : 0)}\" data-ondisk=\"{(status == ProcessedEpisode.FoundStatus.OnDisk ? 1 : 0)}\"";
    }

    private static string DayName(DateTime day)
    {
        if (day == DateTime.Today)
        {
            return "Today";
        }
        if (day == DateTime.Today.AddDays(-1))
        {
            return "Yesterday";
        }
        return day == DateTime.Today.AddDays(1) ? "Tomorrow" : day.ToString("dddd");
    }

    // "S6 E3 · Name"; a multipart episode shows its numbers as the list does, "S6 E3-4"
    private static string EpisodeTitle(ProcessedEpisode ep) => $"{EpisodeNumbers(ep)} · {ep.Name}";

    private static string EpisodeNumbers(ProcessedEpisode ep) =>
        ep.AppropriateSeasonNumber == 0 ? $"Special {ep.EpisodeNumbersAsText}" : $"S{ep.SeasonNumberAsText} E{ep.EpisodeNumbersAsText}";

    // The time on the left and the network on the right, its logo in place of its name once it is known
    private static string TileMeta(ProcessedEpisode ep)
    {
        string time = $"<span class=\"time\">{Encode(ep.Time)}</span>";
        if (!ep.Network.HasValue())
        {
            return time;
        }

        string logo = KnownLogo(ep.Show);
        string network = logo.HasValue()
            ? $"<img class=\"logo\" src=\"{Encode(logo)}\" alt=\"{Encode(ep.Network)}\" title=\"{Encode(ep.Network)}\" onerror=\"noLogo(this.closest('.tile').dataset.i)\">"
            : Encode(ep.Network);
        return $"{time}<span class=\"net\">{network}</span>";
    }

    private static string StatusText(ProcessedEpisode.FoundStatus status) => status switch
    {
        ProcessedEpisode.FoundStatus.OnDisk => "On disk",
        ProcessedEpisode.FoundStatus.PreviouslySeen => "Previously seen",
        ProcessedEpisode.FoundStatus.Missing => "Missing",
        _ => string.Empty
    };

    // The list's icon for the status; the details panel also names it
    private string StatusHtml(ProcessedEpisode.FoundStatus status, bool withText)
    {
        string text = StatusText(status);
        if (!text.HasValue() || StatusIcons is null)
        {
            return string.Empty;
        }

        string icon = $"<span class=\"status st{UI.ChooseWtwIcon(status)}\" title=\"{text}\"></span>";
        return withText ? $"{icon}<span class=\"status-text text-muted\">{text}</span>" : icon;
    }

    // The icons as page images, made once
    private string IconCss()
    {
        if (iconCss.HasValue() || StatusIcons is null)
        {
            return iconCss;
        }

        StringBuilder css = new();
        foreach (ProcessedEpisode.FoundStatus status in new[] { ProcessedEpisode.FoundStatus.OnDisk, ProcessedEpisode.FoundStatus.PreviouslySeen, ProcessedEpisode.FoundStatus.Missing })
        {
            int index = UI.ChooseWtwIcon(status);
            if (index < StatusIcons.Images.Count)
            {
                using Image image = StatusIcons.Images[index];
                using System.IO.MemoryStream png = new();
                image.Save(png, System.Drawing.Imaging.ImageFormat.Png);
                css.Append($".st{index}{{background-image:url(data:image/png;base64,{Convert.ToBase64String(png.ToArray())})}}");
            }
        }
        iconCss = css.ToString();
        return iconCss;
    }

    private static string Encode(string? text) => WebUtility.HtmlEncode(text ?? string.Empty);

    // Fixed heights and single lines: whatever is selected, the panel and the day columns below it stay put
    private static string LayoutCss() =>
        "body{margin:0;padding:0 10px 8px}.col-sm-12{width:100%}"
        + ".guide-top{position:sticky;top:0;z-index:3;padding:8px 0 6px;display:grid;grid-template-columns:1fr auto;gap:16px}"
        + ".details{display:flex;gap:16px;height:200px;overflow:hidden}.details-text{max-width:1100px;min-width:0;flex:1}"
        + ".poster{height:200px;width:134px;object-fit:cover;border-radius:4px;flex:none}"
        + ".details-title,.details-episode,.details-airs{white-space:nowrap;overflow:hidden;text-overflow:ellipsis}"
        + ".details-title{font-size:1.25rem;font-weight:500;line-height:1.3}.details-episode{font-weight:500}"
        + ".details-airs{line-height:24px}#dLabel:empty{display:none}"
        + ".show-summary{display:-webkit-box;-webkit-box-orient:vertical;overflow:hidden;-webkit-line-clamp:4;font-size:.9rem;line-height:1.35;height:5.4em}"
        + ".file-line{margin-top:3px;height:31px;display:flex;gap:6px;align-items:center;overflow:hidden}"
        + ".paging{text-align:right;white-space:nowrap}.paging-buttons{display:flex;gap:4px;justify-content:flex-end}"
        + ".show-summary .label{font-weight:500}"
        + $".days{{display:grid;grid-template-columns:repeat({DAYS_SHOWN},minmax(0,1fr));gap:8px;align-items:start}}"
        + ".day-heads{position:sticky;top:var(--top,0);z-index:2;padding-bottom:6px}"
        + ".day-name{font-weight:500}.day-date{font-size:.8rem;opacity:.8}"
        + ".nothing{padding:4px 8px}.week{display:none}"
        + ".banner{display:block;width:100%;aspect-ratio:758/140;object-fit:cover}"
        + ".tile-body{padding:6px 8px}.tile-show{font-weight:500;line-height:1.2}"
        + ".tile-episode{font-size:.85rem;line-height:1.25;margin:2px 0}"
        + ".tile-meta{font-size:.8rem;line-height:22px;display:flex;justify-content:space-between;align-items:center;gap:6px}"
        + ".tile-meta .time{white-space:nowrap}.tile-meta .net{min-width:0;overflow:hidden;text-overflow:ellipsis;white-space:nowrap}"
        + ".badge{font-weight:500;vertical-align:middle}"
        + ".status{display:inline-block;width:16px;height:16px;margin-right:5px;vertical-align:-2px;background:no-repeat center/16px 16px}"
        + ".status-text{font-size:.9rem;font-weight:400}"
        // Network logos are made for a light background, so they sit on one in every theme
        + ".logo{height:18px;max-width:72px;object-fit:contain;padding:2px 4px;border-radius:3px;background:#fff;box-shadow:0 0 0 1px rgba(0,0,0,.15);vertical-align:middle}"
        + ".logo-lg{height:22px;max-width:110px}";

    // Day headings use the Schedule row colours, so the Guide matches the list in every theme
    private static string ColourCss()
    {
        Theme? t = ThemeManager.ActiveTheme;
        string link = (t?.WebLink ?? Color.FromArgb(0x0D, 0x6E, 0xFD)).HexColour();
        string shade = t is { IsLightBase: false } ? "255,255,255" : "0,0,0";

        StringBuilder css = new();
        css.Append($".day-head{{padding:4px 8px;border-radius:4px;background:rgba({shade},.06)}}");
        if (TVSettings.Instance.UseColoursOnWtw)
        {
            string text = (t?.ScheduleText ?? Color.Black).HexColour();
            css.Append($".day-head.today{{background:{(t?.ScheduleToday ?? Color.LightYellow).HexColour()};color:{text}}}");
            css.Append($".day-head.future{{background:{(t?.ScheduleFuture ?? Color.LightGray).HexColour()};color:{text}}}");
            css.Append($".day-head.past{{background:{(t?.SchedulePast ?? Color.LightBlue).HexColour()};color:{text}}}");
        }
        else
        {
            css.Append($".day-head.today{{border-bottom:3px solid {link}}}");
        }
        css.Append($".day-head.picked{{outline:2px solid {link}}}");
        // Bootstrap cards are white; a see-through shade keeps tiles readable on every theme's page colour
        css.Append($".tile.card{{margin-bottom:8px;cursor:pointer;overflow:hidden;user-select:none;background-color:rgba({shade},.04)!important}}");
        css.Append($".tile.card:hover{{background-color:rgba({shade},.09)!important}}");
        css.Append($".tile.sel{{outline:2px solid {link};outline-offset:-2px}}");
        return css.ToString();
    }

    // Fills the details panel and the network logos
    private static string DetailsScript()
    {
        return """
            var shown = 0, current = -1;
            function send(action, value) { CefSharp.PostMessage(page + '|' + action + '|' + value); }
            function el(id) { return document.getElementById(id); }
            function tileAt(i) { return document.querySelector('.tile[data-i="' + i + '"]'); }
            function showNetwork(s) {
              var logo = el('dLogo'), name = el('dNetwork');
              if (s.logo) {
                logo.src = s.logo; logo.alt = s.network; logo.title = s.network; logo.style.display = '';
                name.style.display = 'none';
              } else {
                logo.removeAttribute('src'); logo.style.display = 'none';
                name.textContent = s.network; name.style.display = s.network ? '' : 'none';
              }
            }
            function logoImage(s) {
              var img = document.createElement('img');
              img.className = 'logo'; img.src = s.logo; img.alt = s.network; img.title = s.network;
              img.onerror = function () { noLogo(img.closest('.tile').dataset.i); };
              return img;
            }
            // A logo that will not load is dropped for the show; its name is shown instead
            function noLogo(i) {
              var tile = tileAt(i);
              if (!tile) { return; }
              var s = shows[tile.dataset.s];
              s.logo = '';
              document.querySelectorAll('.tile[data-s="' + tile.dataset.s + '"] .net').forEach(function (net) { net.textContent = s.network; });
              if (current >= 0 && tileAt(current).dataset.s === tile.dataset.s) { showNetwork(s); }
            }
            function setLogo(p, tmdb, url) {
              if (p !== page) { return; }
              shows.forEach(function (s, i) {
                if (s.tmdb !== tmdb || s.logo === url) { return; }
                s.logo = url;
                document.querySelectorAll('.tile[data-s="' + i + '"] .net').forEach(function (net) {
                  net.textContent = ''; net.appendChild(logoImage(s));
                });
                if (current >= 0 && Number(tileAt(current).dataset.s) === i) { showNetwork(s); }
              });
            }
            function fill(tile, label) {
              var d = tile.dataset, s = shows[d.s];
              current = Number(d.i);
              var poster = el('dPoster');
              if (s.poster) { poster.src = s.poster; } else { poster.removeAttribute('src'); poster.style.visibility = 'hidden'; }
              el('dLabel').textContent = label;
              el('dShow').textContent = d.show;
              el('dStatus').innerHTML = d.status;
              el('dEpisode').textContent = d.episode;
              el('dAirs').textContent = d.airs;
              showNetwork(s);
              el('dHowLong').textContent = d.howlong;
              el('dShowSummary').textContent = s.summary;
              el('dShowSummary').parentNode.title = s.summary;
              el('dShowSummary').parentNode.style.visibility = s.summary ? 'visible' : 'hidden';
              setFile(page, '', '', '', '', '');
              el('dFile').textContent = d.ondisk === '1' ? 'Looking for the file...' : '';
              el('dSearch').style.display = d.aired === '1' && d.ondisk !== '1' ? '' : 'none';
            }
            function setFile(p, name, size, dims, watch, explore) {
              if (p !== page) { return; }
              el('dFile').textContent = [name, size, dims].filter(function (s) { return s; }).join(' · ');
              el('dWatch').href = watch; el('dWatch').style.display = watch ? '' : 'none';
              el('dExplore').href = explore; el('dExplore').style.display = explore ? '' : 'none';
            }
            """;
    }

    // Weeks, tiles and the messages back to the window
    private static string NavigationScript()
    {
        return """
            function pick(tile) {
              document.querySelectorAll('.tile.sel').forEach(function (t) { t.classList.remove('sel'); });
              tile.classList.add('sel');
              fill(tile, '');
            }
            // paged is Earlier / Today / Later, which leave the day picked on the calendar
            function showWeek(w, day, paged) {
              w = Math.max(firstWeek, Math.min(lastWeek, w));
              document.querySelectorAll('.week').forEach(function (x) { x.style.display = 'none'; });
              var week = el('w' + w);
              week.style.display = 'block';
              shown = w;
              el('range').textContent = week.dataset.range;
              el('bEarlier').innerHTML = '&#9664; Earlier' + (week.dataset.earlier !== '0' ? ' (' + week.dataset.earlier + ')' : '');
              el('bLater').innerHTML = 'Later' + (week.dataset.later !== '0' ? ' (' + week.dataset.later + ')' : '') + ' &#9654;';
              el('bEarlier').disabled = w === firstWeek;
              el('bLater').disabled = w === lastWeek;
              document.querySelectorAll('.day-head.picked').forEach(function (h) { h.classList.remove('picked'); });
              var head = day ? week.querySelector('.day-head[data-date="' + day + '"]') : null;
              if (head) { head.classList.add('picked'); }
              window.scrollTo(0, 0);
              send(paged ? 'paged' : 'week', w);
            }
            function selectTile(i) {
              var tile = tileAt(i);
              if (!tile) { return; }
              showWeek(Number(tile.closest('.week').id.substring(1)), '', false);
              pick(tile);
              send('look', i);
            }
            function showDay(day) {
              var w = Math.floor((Date.parse(day) - Date.parse(firstDay)) / (days * 86400000));
              showWeek(w, day, false);
            }
            document.querySelectorAll('.tile').forEach(function (tile) {
              tile.addEventListener('click', function () { pick(tile); send('select', tile.dataset.i); });
              tile.addEventListener('dblclick', function () { send('open', tile.dataset.i); });
              tile.addEventListener('contextmenu', function (e) { e.preventDefault(); pick(tile); send('menu', tile.dataset.i); });
            });
            document.addEventListener('contextmenu', function (e) { e.preventDefault(); });
            document.addEventListener('mousedown', function () { send('dismiss', 0); });
            document.addEventListener('keydown', function (e) { if (e.key === 'Escape') { send('dismiss', 0); } });
            var guideTop = document.querySelector('.guide-top');
            guideTop.style.backgroundColor = getComputedStyle(document.body).backgroundColor;
            new ResizeObserver(function () { document.documentElement.style.setProperty('--top', guideTop.offsetHeight + 'px'); }).observe(guideTop);
            showWeek(startWeek, picked, false);
            var start = featured >= 0 ? tileAt(featured) : null;
            if (start) { fill(start, featuredLabel); send('look', featured); }
            else { setFile(page, '', '', '', '', ''); el('dSearch').style.display = 'none'; el('dNetwork').style.display = 'none'; el('dLogo').style.display = 'none'; }
            send('ready', 0);
            """;
    }
}
