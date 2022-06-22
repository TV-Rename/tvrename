using Alphaleonis.Win32.Filesystem;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace TVRename
{
    internal class ActionNfoMovie : ActionNfo
    {
        public ActionNfoMovie(FileInfo where, MovieConfiguration mc) : base(where, mc)
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

            //https://forum.kodi.tv/showthread.php?tid=323588
            //says that we need a format like this:
            //<episodeguide><url post="yes" cache="auth.json">https://api.thetvdb.com/login?{&quot;apikey&quot;:&quot;((API-KEY))&quot;,&quot;id&quot;:((ID))}|Content-Type=application/json</url></episodeguide>

            if (cachedSeries is not null)
            {
                root.UpdateElement("originaltitle", Movie.ShowName);
                root.UpdateElement("sorttitle", UI.GenerateShowUiName(Movie));
                root.ReplaceElements("studio", cachedSeries.Networks);
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

                root.ReplaceElements("genre", Movie.Genres);
                root.ReplaceElements("credits", cachedSeries.GetCrew().Where(c=>c.Department=="Writing").Select(c=>c.Name));
                root.ReplaceElements("director", cachedSeries.GetCrew().Where(c => c.Department == "Directing").Select(c => c.Name));

                ReplaceActors(root, Movie.Actors);

                ReplaceThumbs(root, "poster", cachedSeries.Images(MediaImage.ImageType.poster));
                ReplaceThumbs(root, "banner", cachedSeries.Images(MediaImage.ImageType.wideBanner));
                ReplaceThumbs(root, "keyart", cachedSeries.Images(MediaImage.ImageType.clearArt));
                ReplaceThumbs(root, "clearlogo", cachedSeries.Images(MediaImage.ImageType.clearLogo));

                ReplaceFanart(root, cachedSeries.Images(MediaImage.ImageType.background));
            }
            doc.Save(Where.FullName);
            return ActionOutcome.Success();
        }
    }
}
