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
using System.Text.RegularExpressions;
using System.Web;
using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;

namespace TVRename.Forms;

// The Schedule's Guide tab: the list's episodes as weeks of day columns, a double episode sharing one slot.
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

    // Network logos by TMDB show id, found once a session; empty when TMDB has none for the show's network
    private static readonly Dictionary<int, string> NetworkLogos = [];
    private static readonly HashSet<int> LogosAsked = [];
    private static Font? boldMenuFont;

    private TileLayout tiles = TileLayout.Full;
    private int hiddenByFilter;

    private readonly ChromiumWebBrowser browser;
    private List<List<ProcessedEpisode>> slots = [];
    private List<ProcessedEpisode>? selected;
    private DateTime? pickedDay;
    private int week; // 0 is the week starting yesterday
    private int pageNumber; // a click on an older page must not act on a newer episode list
    private int readyPage; // the page whose script is running; calls made while a page loads would be lost
    private int fileLookup; // only the latest file lookup updates the page
    private bool needsRender = true;
    private bool syncWhenReady; // the selection or calendar day changed while the page was loading
    private string renderedSignature = string.Empty; // what the page shows; reloading redraws every picture, so only reload for a change
    private string iconCss = string.Empty;

    public event EventHandler<List<ProcessedEpisode>>? EpisodesSelected;
    public event EventHandler<List<ProcessedEpisode>>? EpisodesOpened;
    public event EventHandler<List<ProcessedEpisode>>? EpisodesRightClicked;
    public event EventHandler<List<ProcessedEpisode>>? EpisodesSearched;
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
        browser.IsBrowserInitializedChanged += (_, _) => OnUiThread(() =>
        {
            if (Visible && needsRender)
            {
                Render();
            }
        });
        Controls.Add(browser);
    }

    // The browser calls back on its own thread, possibly while the window is closing
    private void OnUiThread(System.Action action)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        try
        {
            BeginInvoke(action);
        }
        catch (InvalidOperationException)
        {
            // Closed in the meantime
        }
    }

    // hidden is how many of the schedule's episodes the list's filters leave out
    public void SetEpisodes(IEnumerable<ProcessedEpisode> eps, ProcessedEpisode? selectedEpisode, int hidden)
    {
        hiddenByFilter = hidden;
        slots = Slots(eps);

        // The list's selection, or else the slot already shown, found again in the new episode list
        List<ProcessedEpisode>? wanted = selectedEpisode is null ? null : slots.FirstOrDefault(slot => slot.Contains(selectedEpisode));
        wanted ??= slots.FirstOrDefault(slot => SameSlot(slot, selected));

        // Same episodes as the page shows or is loading: at most the selection moved
        bool samePage = Signature() == renderedSignature && (PageReady || PageLoading);
        if (samePage && wanted is not null && !SameSlot(wanted, selected))
        {
            SelectTile(slots.IndexOf(wanted));
        }

        selected = wanted;
        if (!samePage)
        {
            RenderWhenVisible();
        }
    }

    private static List<List<ProcessedEpisode>> Slots(IEnumerable<ProcessedEpisode> eps)
    {
        List<List<ProcessedEpisode>> found = [];
        foreach (ProcessedEpisode ep in eps.Where(ep => ep.GetAirDateDt().HasValue).OrderBy(ep => ep.GetAirDateDt()).ThenBy(ep => ep.SeriesName).ThenBy(ep => ep.AppropriateEpNum))
        {
            List<ProcessedEpisode>? last = found.LastOrDefault();
            if (last is not null && last[0].Show == ep.Show && last[0].GetAirDateDt() == ep.GetAirDateDt())
            {
                last.Add(ep);
            }
            else
            {
                found.Add([ep]);
            }
        }
        return found;
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

    private static bool SameSlot(List<ProcessedEpisode> a, List<ProcessedEpisode>? b) =>
        b is not null && a.Count == b.Count && a.Zip(b).All(pair => pair.First.Equals(pair.Second));

    // Everything the page draws from, so an unchanged schedule (a periodic refresh, switching views) keeps the page as it is
    private string Signature()
    {
        StringBuilder sb = new();
        sb.Append(DateTime.Today.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)).Append(ThemeManager.ActiveTheme?.Key)
          .Append(TVSettings.Instance.UseColoursOnWtw).Append(tiles).Append(hiddenByFilter);
        foreach (ProcessedEpisode ep in slots.SelectMany(slot => slot))
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

    private static DateTime AirTime(List<ProcessedEpisode> slot) => slot[0].GetAirDateDt() ?? DateTime.MaxValue;

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

        OnUiThread(() => _ = ActAsync(page, action, value));
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
                if (value >= 0 && value < slots.Count)
                {
                    await ActOnSlotAsync(slots[value], action);
                }
                break;
        }
    }

    private async Task ActOnSlotAsync(List<ProcessedEpisode> slot, string action)
    {
        if (action != "look")
        {
            selected = slot;
        }

        switch (action)
        {
            case "look":
                await ShowFileAsync(slot);
                break;

            case "select":
                EpisodesSelected?.Invoke(this, slot);
                await ShowFileAsync(slot);
                break;

            case "open":
                EpisodesOpened?.Invoke(this, slot);
                break;

            case "menu":
                EpisodesRightClicked?.Invoke(this, slot);
                await ShowFileAsync(slot);
                break;

            case "search":
                EpisodesSearched?.Invoke(this, slot);
                break;

            default:
                // Not a slot action
                break;
        }
    }

    // The file is looked up only for the slot in the details panel, so drawing the page reads nothing from disk
    private async Task ShowFileAsync(List<ProcessedEpisode> slot)
    {
        int lookup = ++fileLookup;
        int page = pageNumber;
        FileInfo? file = (await FinderHelper.FindEpOnDiskAsync(null, slot[0])).FirstOrDefault();
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
        if (selected is not null && slots.Contains(selected))
        {
            browser.ExecuteScriptAsync("selectTile", slots.IndexOf(selected));
        }
        if (pickedDay.HasValue)
        {
            browser.ExecuteScriptAsync("showDay", pickedDay.Value.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));
        }
    }

    // Logos come from TMDB when a page is ready; until then, and if TMDB has none, the network's name is shown
    private async Task ShowNetworkLogosAsync()
    {
        List<ProcessedEpisode> shows = [.. slots.Select(slot => slot[0]).Where(ep => ep.Show.TmdbCode > 0 && ep.Network.HasValue()).DistinctBy(ep => ep.Show.TmdbCode)];

        // Found while this page was loading
        foreach (ProcessedEpisode ep in shows)
        {
            SetLogo(ep.Show.TmdbCode!.Value, KnownLogo(ep.Show));
        }

        if (TVSettings.Instance.OfflineMode)
        {
            return;
        }

        List<ProcessedEpisode> toFind = [.. shows.Where(ep => LogosAsked.Add(ep.Show.TmdbCode!.Value))];
        ConcurrentDictionary<int, string> found = new();
        await Parallel.ForEachAsync(toFind, new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, async (ep, token) =>
        {
            int id = ep.Show.TmdbCode!.Value;
            Dictionary<string, string> logos = await TMDB.LocalCache.GetNetworkLogosAsync(id, token);
            found[id] = logos.FirstOrDefault(pair => NetworkKey(pair.Key) == NetworkKey(ep.Network)).Value ?? string.Empty;
        });

        // Back on the window's thread
        foreach ((int id, string logo) in found)
        {
            NetworkLogos[id] = logo;
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

    // So "ABC (US)" matches TMDB's "ABC" and "Apple TV" matches "Apple TV+"
    private static string NetworkKey(string name) =>
        new([.. Regex.Replace(name, @"\([^)]*\)", string.Empty).Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant)]);

    private static string KnownLogo(ShowConfiguration show) =>
        show.TmdbCode is { } id && NetworkLogos.TryGetValue(id, out string? logo) ? logo : string.Empty;

    private string GuideHtml()
    {
        List<ProcessedEpisode>? upNext = slots.FirstOrDefault(slot => AirTime(slot) >= TimeHelpers.LocalNow());
        List<ProcessedEpisode>? featured = selected ?? upNext;
        int firstWeek = Math.Min(0, slots.Count == 0 ? 0 : WeekOf(AirTime(slots[0])));
        int lastWeek = Math.Max(0, slots.Count == 0 ? 0 : WeekOf(AirTime(slots[^1])));
        List<ShowConfiguration> shows = [.. slots.Select(slot => slot[0].Show).Distinct()];

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

        int featuredIndex = featured is null ? -1 : slots.IndexOf(featured);
        string upNextLabel = featured is not null && featured == upNext && selected is null ? "Up next" : string.Empty;
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
            network = show.CachedShow?.Networks.FirstOrDefault() ?? string.Empty,
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
            string episodes = hiddenByFilter == 1 ? "episode" : "episodes";
            sb.AppendLine($"<div class=\"filter-note small mt-1\"><span class=\"badge bg-warning\" style=\"color:#212529\">Filtered</span> {hiddenByFilter} {episodes} hidden"
                          + " · <a href=\"#\" onclick=\"send('clearfilter',0);return false;\">Clear</a></div>");
        }
        sb.AppendLine(DIV_END);
    }

    private void AppendWeek(StringBuilder sb, int w, List<ShowConfiguration> shows)
    {
        DateTime first = FirstDay.AddDays(w * DAYS_SHOWN);
        DateTime end = first.AddDays(DAYS_SHOWN);
        int earlier = slots.Sum(slot => AirTime(slot) < first ? slot.Count : 0);
        int later = slots.Sum(slot => AirTime(slot) >= end ? slot.Count : 0);

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
            List<int> onDay = [.. Enumerable.Range(0, slots.Count).Where(i => AirTime(slots[i]).Date == day)];
            sb.AppendLine("<div class=\"day\">");
            if (onDay.Count == 0)
            {
                sb.AppendLine("<div class=\"nothing text-muted small\">Nothing airing</div>");
            }
            foreach (int i in onDay)
            {
                AppendTile(sb, i, shows.IndexOf(slots[i][0].Show));
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
        List<ProcessedEpisode> slot = slots[index];
        ProcessedEpisode ep = slot[0];
        string banner = ep.Show.WideBannerUrl();
        ProcessedEpisode.FoundStatus status = SlotStatus(slot);

        sb.AppendLine($"<div class=\"tile card{(slot == selected ? " sel" : string.Empty)}\" data-i=\"{index}\" data-s=\"{showIndex}\"{TileData(slot, status)}>");
        if (banner.HasValue() && tiles == TileLayout.Full)
        {
            sb.AppendLine($"<img class=\"banner\" loading=\"lazy\" src=\"{Encode(banner)}\" alt=\"{Encode(ep.SeriesName)}\" onerror=\"this.remove()\">");
        }
        sb.AppendLine("<div class=\"tile-body\">");
        sb.AppendLine($"<div class=\"tile-show\">{StatusHtml(status, false)}{Encode(ep.SeriesName)}</div>");
        if (tiles == TileLayout.Compact)
        {
            sb.AppendLine($"<div class=\"tile-episode\">{Encode(EpisodeNumbers(slot))}</div>");
        }
        else
        {
            sb.AppendLine($"<div class=\"tile-episode\">{Encode(EpisodeTitle(slot, false))}</div>");
            sb.AppendLine($"<div class=\"tile-meta text-muted\">{TileMeta(ep)}</div>");
        }
        sb.AppendLine("</div></div>");
    }

    // What the details panel shows for this slot
    private string TileData(List<ProcessedEpisode> slot, ProcessedEpisode.FoundStatus status)
    {
        ProcessedEpisode ep = slot[0];
        DateTime airs = AirTime(slot);
        bool aired = airs <= TimeHelpers.LocalNow();
        string howLong = aired ? string.Empty : ep.HowLong();
        string airsText = $"{(aired ? "Aired" : "Airs")} {airs:ddd d MMM} at {ep.Time}{(ep.Network.HasValue() ? " on" : string.Empty)}";

        return $" data-show=\"{Encode(ep.SeriesName)}\" data-episode=\"{Encode(EpisodeTitle(slot, true))}\""
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

    // "S6 E3 · Name" for one episode; "S6 E3-E4 · Name" for a double episode, with every name in the details panel
    private static string EpisodeTitle(List<ProcessedEpisode> slot, bool allNames)
    {
        string names = allNames ? string.Join(" / ", slot.Select(e => e.Name)) : slot[0].Name;
        return $"{EpisodeNumbers(slot)} · {names}";
    }

    private static string EpisodeNumbers(List<ProcessedEpisode> slot)
    {
        ProcessedEpisode ep = slot[0];
        ProcessedEpisode last = slot[^1];
        string numbers = slot.Count == 1 ? ep.EpisodeNumbersAsText : $"{ep.EpisodeNumbersAsText}-E{last.EpisodeNumbersAsText}";
        return ep.AppropriateSeasonNumber == 0 ? $"Special {numbers}" : $"S{ep.SeasonNumberAsText} E{numbers}";
    }

    // "9:00 PM · CBS", with the network's logo in place of its name once it is known
    private static string TileMeta(ProcessedEpisode ep)
    {
        if (!ep.Network.HasValue())
        {
            return Encode(ep.Time);
        }

        string logo = KnownLogo(ep.Show);
        string network = logo.HasValue()
            ? $"<img class=\"logo\" src=\"{Encode(logo)}\" alt=\"{Encode(ep.Network)}\" title=\"{Encode(ep.Network)}\" onerror=\"noLogo(this.closest('.tile').dataset.i)\">"
            : Encode(ep.Network);
        return $"{Encode(ep.Time)} · <span class=\"net\">{network}</span>";
    }

    // A slot is as good as its worst episode: one missing part of a double episode makes it Missing
    private static ProcessedEpisode.FoundStatus SlotStatus(List<ProcessedEpisode> slot)
    {
        List<ProcessedEpisode.FoundStatus> found = [.. slot.Select(ep => ep.AiredStatus)];
        if (found.Contains(ProcessedEpisode.FoundStatus.Missing))
        {
            return ProcessedEpisode.FoundStatus.Missing;
        }
        if (found.All(s => s == ProcessedEpisode.FoundStatus.OnDisk))
        {
            return ProcessedEpisode.FoundStatus.OnDisk;
        }
        return found.All(s => s is ProcessedEpisode.FoundStatus.OnDisk or ProcessedEpisode.FoundStatus.PreviouslySeen)
            ? ProcessedEpisode.FoundStatus.PreviouslySeen
            : ProcessedEpisode.FoundStatus.Future;
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
        + ".details{display:flex;gap:16px;height:150px;overflow:hidden}.details-text{max-width:1100px;min-width:0;flex:1}"
        + ".poster{height:150px;width:100px;object-fit:cover;border-radius:4px;flex:none}"
        + ".details-title,.details-episode,.details-airs{white-space:nowrap;overflow:hidden;text-overflow:ellipsis}"
        + ".details-title{font-size:1.25rem;font-weight:500;line-height:1.3}.details-episode{font-weight:500}"
        + ".details-airs{line-height:24px}#dLabel:empty{display:none}"
        + ".show-summary{display:-webkit-box;-webkit-box-orient:vertical;overflow:hidden;-webkit-line-clamp:2;font-size:.9rem;line-height:1.35;height:2.7em}"
        + ".file-line{margin-top:3px;height:31px;display:flex;gap:6px;align-items:center;overflow:hidden}"
        + ".paging{text-align:right;white-space:nowrap}.paging-buttons{display:flex;gap:4px;justify-content:flex-end}"
        + ".show-summary .label{font-weight:500}"
        + $".days{{display:grid;grid-template-columns:repeat({DAYS_SHOWN},minmax(0,1fr));gap:8px;align-items:start}}"
        + ".day-heads{position:sticky;top:var(--top,0);z-index:2;padding-bottom:6px}"
        + ".day-name{font-weight:500}.day-date{font-size:.8rem;opacity:.8}"
        + ".nothing{padding:4px 8px}.week{display:none}"
        + ".banner{display:block;width:100%;aspect-ratio:758/140;object-fit:cover}"
        + ".tile-body{padding:6px 8px}.tile-show{font-weight:500;line-height:1.2}"
        + ".tile-episode{font-size:.85rem;line-height:1.25;margin:2px 0}.tile-meta{font-size:.8rem;line-height:20px}"
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
