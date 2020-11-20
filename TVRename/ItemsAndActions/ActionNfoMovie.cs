using System.Globalization;
using System.Xml.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    class ActionNfoMovie : ActionNfo
    {
        public ActionNfoMovie([NotNull] FileInfo where, [NotNull] MovieConfiguration mc) : base(where, mc)
        {
            Episode = null;
        }

        public override string Name => "Write KODI Metadata (Movie)";

        protected override long? UpdateTime() => Movie?.CachedMovie?.SrvLastUpdated;

        protected override string RootName() => "movie";

        protected override ActionOutcome UpdateFile()
        {
            XDocument doc = XDocument.Load(Where.FullName);
            XElement? root = doc.Root;

            if (root is null)
            {
                return new ActionOutcome($"Could not load {Where.FullName}");
            }

            CachedMovieInfo cachedSeries = Movie!.CachedMovie;
            root.UpdateElement("title", Movie.ShowName);

            float? showRating = cachedSeries?.SiteRating;
            if (showRating.HasValue)
            {
                UpdateRatings(root, showRating.Value.ToString(CultureInfo.InvariantCulture), cachedSeries.SiteRatingVotes);
            }

            string lang = TVSettings.Instance.PreferredLanguageCode;

            if (Movie.UseCustomLanguage && Movie.PreferredLanguage != null)
            {
                lang = Movie.PreferredLanguage.Abbreviation;
            }

            //https://forum.kodi.tv/showthread.php?tid=323588
            //says that we need a format like this:
            //<episodeguide><url post="yes" cache="auth.json">https://api.thetvdb.com/login?{&quot;apikey&quot;:&quot;((API-KEY))&quot;,&quot;id&quot;:((ID))}|Content-Type=application/json</url></episodeguide>

            XElement episodeGuideNode = root.GetOrCreateElement("episodeguide");
            XElement urlNode = episodeGuideNode.GetOrCreateElement("url");
            urlNode.UpdateAttribute("post", "yes");
            urlNode.UpdateAttribute("cache", "auth.json");
            urlNode.SetValue(TheTVDB.API.BuildUrl(Movie.TvdbCode, lang));

            if (!(cachedSeries is null))
            {
                root.UpdateElement("originaltitle", Movie.ShowName);
                UpdateAmongstElements(root, "studio", cachedSeries.Network);
                root.UpdateElement("id", Movie.Code);
                root.UpdateElement("runtime", cachedSeries.Runtime, true);
                root.UpdateElement("mpaa", cachedSeries.ContentRating, true);
                root.UpdateElement("premiered", cachedSeries.FirstAired);
                if (cachedSeries.Year.HasValue)
                {
                    root.UpdateElement("year", cachedSeries.Year.Value);
                }
                root.UpdateElement("status", cachedSeries.Status);
                root.UpdateElement("plot", cachedSeries.Overview);
                root.UpdateElement("outline", cachedSeries.Overview);
                root.UpdateElement("tagline", cachedSeries.TagLine);
                root.UpdateElement("set", cachedSeries.CollectionName);
                root.UpdateElement("trailer", cachedSeries.TrailerUrl);

                UpdateId(root, "tvdb", "false", cachedSeries.TvdbCode);
                UpdateId(root, "imdb", "false", cachedSeries.Imdb);
                UpdateId(root, "tmdb", "true", cachedSeries.TmdbCode);
            }

            root.ReplaceElements("genre", Movie.Genres);

            ReplaceActors(root, Movie.Actors);

            doc.Save(Where.FullName);
            return ActionOutcome.Success();
        }
    }
}
