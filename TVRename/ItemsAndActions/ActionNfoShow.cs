using System.Globalization;
using System.Xml.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class ActionNfoShow : ActionNfo
    {
        public ActionNfoShow([NotNull] FileInfo where, [NotNull] ShowConfiguration mc) : base(where, mc)
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

            CachedSeriesInfo cachedSeries = SelectedShow!.CachedShow;
            root.UpdateElement("title", SelectedShow.ShowName);

            float? showRating = cachedSeries?.SiteRating;
            if (showRating.HasValue)
            {
                UpdateRatings(root, showRating.Value.ToString(CultureInfo.InvariantCulture), cachedSeries.SiteRatingVotes);
            }

            string lang = TVSettings.Instance.PreferredLanguageCode;

            if (SelectedShow.UseCustomLanguage && SelectedShow.PreferredLanguage != null)
            {
                lang = SelectedShow.PreferredLanguage.Abbreviation;
            }

            //https://forum.kodi.tv/showthread.php?tid=323588
            //says that we need a format like this:
            //<episodeguide><url post="yes" cache="auth.json">https://api.thetvdb.com/login?{&quot;apikey&quot;:&quot;((API-KEY))&quot;,&quot;id&quot;:((ID))}|Content-Type=application/json</url></episodeguide>

            XElement episodeGuideNode = root.GetOrCreateElement("episodeguide");
            XElement urlNode = episodeGuideNode.GetOrCreateElement("url");
            urlNode.UpdateAttribute("post", "yes");
            urlNode.UpdateAttribute("cache", "auth.json");
            urlNode.SetValue(TheTVDB.API.BuildUrl(SelectedShow.TvdbCode, lang));

            if (!(cachedSeries is null))
            {
                root.UpdateElement("originaltitle", SelectedShow.ShowName);
                UpdateAmongstElements(root, "studio", cachedSeries.Network);
                root.UpdateElement("id", cachedSeries.TvdbCode);
                root.UpdateElement("runtime", cachedSeries.Runtime, true);
                root.UpdateElement("mpaa", cachedSeries.ContentRating, true);
                root.UpdateElement("premiered", cachedSeries.FirstAired);
                root.UpdateElement("year", cachedSeries.Year);
                root.UpdateElement("status", cachedSeries.Status);
                root.UpdateElement("plot", cachedSeries.Overview);

                UpdateId(root, "tvdb", "true", cachedSeries.TvdbCode);
                UpdateId(root, "imdb", "false", cachedSeries.Imdb);
            }

            root.ReplaceElements("genre", SelectedShow.Genres);

            ReplaceActors(root, SelectedShow.Actors);

            doc.Save(Where.FullName);
            return ActionOutcome.Success();
        }
    }
}
