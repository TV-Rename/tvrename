using Alphaleonis.Win32.Filesystem;
using System.Globalization;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using TVRename.Forms;

namespace TVRename;

internal class ActionNfoShow : ActionNfo
{
    // Produce a file based on specification at https://kodi.wiki/view/NFO_files/TV_shows
    public ActionNfoShow(FileInfo where, ShowConfiguration mc) : base(where, mc)
    {
        Episode = null;
    }

    public override string Name => "Write KODI Metadata (Show)";

    protected override long? UpdateTime() => SelectedShow?.CachedShow?.SrvLastUpdated;

    protected override string RootName() => "tvshow";

    protected override ActionOutcome UpdateFile()
    {
        XDocument doc = XDocument.Load(Where.FullName);
        XElement? root = doc.Root;

        if (root is null)
        {
            return new ActionOutcome($"Could not load {Where.FullName}");
        }

        CachedSeriesInfo? cachedSeries = SelectedShow!.CachedShow;
        root.UpdateElement("title", SelectedShow.ShowName);

        float? showRating = cachedSeries?.SiteRating;
        if (showRating.HasValue)
        {
            UpdateRatings(root, showRating.Value.ToString(CultureInfo.InvariantCulture), cachedSeries!.SiteRatingVotes);
        }

        if (cachedSeries is not null)
        {
            root.UpdateElement("originaltitle", SelectedShow!.ShowName);
            root.UpdateElement("sorttitle", UI.GenerateShowUiName(SelectedShow));
            root.ReplaceElements("studio", cachedSeries.Networks);
            root.UpdateElement("id", cachedSeries.TvdbCode);
            root.UpdateElement("episodeguide", GenerateEpisdeGuideJson(cachedSeries));
            root.UpdateElement("runtime", cachedSeries.Runtime, true);
            root.UpdateElement("mpaa", cachedSeries.ContentRating, true);
            root.UpdateElement("premiered", cachedSeries.FirstAired);
            root.UpdateElement("year", cachedSeries.Year);
            root.UpdateElement("status", cachedSeries.Status);
            root.UpdateElement("plot", cachedSeries.Overview);
            root.UpdateElement("trailer", cachedSeries.TrailerUrl);

            UpdateId(root, "tvdb", SelectedShow.Provider == TVDoc.ProviderType.TheTVDB ? "true" : "false", SelectedShow.TvdbCode);
            UpdateId(root, "tmdb", SelectedShow.Provider == TVDoc.ProviderType.TMDB ? "true" : "false", SelectedShow.TmdbCode);
            UpdateId(root, "tvmaze", SelectedShow.Provider == TVDoc.ProviderType.TVmaze ? "true" : "false", SelectedShow.TvMazeId);
            UpdateId(root, "imdb", "false", SelectedShow.ImdbCode);

            root.ReplaceElements("genre", SelectedShow.Genres);

            ReplaceActors(root, SelectedShow.Actors);

            ReplaceThumbs(root, "poster", cachedSeries.Images(MediaImage.ImageType.poster));
            ReplaceThumbs(root, "banner", cachedSeries.Images(MediaImage.ImageType.wideBanner));
            ReplaceThumbs(root, "keyart", cachedSeries.Images(MediaImage.ImageType.clearArt));
            ReplaceThumbs(root, "clearlogo", cachedSeries.Images(MediaImage.ImageType.clearLogo));

            ReplaceFanart(root, cachedSeries.Images(MediaImage.ImageType.background));
        }
        doc.Save(Where.FullName);
        return ActionOutcome.Success();
    }

    private string GenerateEpisdeGuideJson(CachedSeriesInfo cachedSeries)
    {
        JObject ids = new();
        AddId(ids, "tvmaze", cachedSeries.TvMazeCode);
        AddId(ids, "tvrage", cachedSeries.TvRageCode);
        AddId(ids, "tvdb", cachedSeries.TvdbCode);
        AddId(ids,"tmdb",cachedSeries.TmdbCode);
        AddId(ids, "imdb", cachedSeries.ImdbCode);
        return ids.ToString();
    }

    private void AddId(JObject json, string key, string? id)
    {
        if (id is null || !id.HasValue())
        {
            return;
        }
        json.Add(key,id);
    }

    private void AddId(JObject json, string key, int? id)
    {
        if (id is null or 0)
        {
            return;
        }
        AddId(json,key,id.ToString());
    }
}
