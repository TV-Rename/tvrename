using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public class CachedMediaInfo
    {
        public string Name;
        public string? Overview;
        public string? Runtime;
        public string? ContentRating;
        public float SiteRating;
        public int SiteRatingVotes;
        public string? Imdb;
        public int TvdbCode;
        public int TvMazeCode;
        public int TvRageCode;
        public int TmdbCode;
        public string? WebUrl;
        public string? OfficialUrl;
        public string? ShowLanguage;
        public string? PosterUrl;
        public string? TwitterId;
        public string? InstagramId;
        public string? FacebookId;
        public string? TagLine;
        public double? Popularity;

        public bool IsSearchResultOnly; // set to true if local info is known to be just certain fields found from search results. Do not need to be saved

        protected List<Actor> Actors;
        protected List<Crew> Crew;
        public List<string> Genres;
        protected List<string> Aliases;

        public bool Dirty; // set to true if local info is known to be older than whats on the server
        public long SrvLastUpdated;


        private protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        protected CachedMediaInfo()
        {
            Actors = new List<Actor>();
            Crew = new List<Crew>();
            Aliases = new List<string>();
            Genres = new List<string>();

            Dirty = false;
            Name = string.Empty;

            TvdbCode = -1;
            TvMazeCode = -1;
            TvRageCode = 0;
            TmdbCode = -1;
        }

        public IEnumerable<string> GetAliases() => Aliases;

        public IEnumerable<Actor> GetActors() => Actors;

        [NotNull]
        public IEnumerable<string> GetActorNames() => GetActors().Select(x => x.ActorName);

        public void ClearActors()
        {
            Actors = new List<Actor>();
        }

        public void AddActor(Actor actor)
        {
            Actors.Add(actor);
        }

        public IEnumerable<Crew> GetCrew() => Crew;


        [NotNull]
        public IEnumerable<string> GetCrewNames() => GetCrew().Select(x => x.Name);


        public void ClearCrew()
        {
            Crew = new List<Crew>();
        }

        public void AddCrew(Crew crew)
        {
            Crew.Add(crew);
        }


        [NotNull]
        public string GetImdbNumber() =>
            Imdb is null ? string.Empty
            : Imdb.StartsWith("tt", StringComparison.Ordinal) ? Imdb.RemoveFirst(2)
            : Imdb;


        public void AddAlias(string s)
        {
            Aliases.Add(s);
        }

        public override string ToString() => $"TMDB:{TmdbCode}/TVDB:{TvdbCode}/Maze:{TvMazeCode}/{Name}";
        public void UpgradeSearchResultToDirty()
        {
            if (IsSearchResultOnly)
            {
                Dirty = true;
                IsSearchResultOnly = false;
            }
        }

        protected static string ChooseBetter(string? encumbant, bool betterLanguage, string? newValue)
        {
            if (string.IsNullOrEmpty(encumbant))
            {
                return newValue?.Trim() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(newValue))
            {
                return encumbant.Trim();
            }

            return betterLanguage ? newValue.Trim() : encumbant.Trim();
        }

        [NotNull]
        protected static string ChooseBetterStatus(string? encumbant, bool betterLanguage, string? newValue)
        {
            if (string.IsNullOrEmpty(encumbant) || encumbant.Equals("Unknown"))
            {
                return newValue?.Trim() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(newValue) || newValue.Equals("Unknown"))
            {
                return encumbant.Trim();
            }

            return betterLanguage ? newValue.Trim() : encumbant.Trim();
        }
    }
}
