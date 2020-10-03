using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public class CachedMediaInfo
    {
        public bool Dirty; // set to true if local info is known to be older than whats on the server

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
        public long SrvLastUpdated;
        public string? TwitterId;
        public string? InstagramId;
        public string? FacebookId;
        public string? TagLine;

        public bool IsSearchResultOnly; // set to true if local info is known to be just certain fields found from search results. Do not need to be saved

        protected List<Actor> Actors;
        public List<string> Genres;
        protected List<string> Aliases;

        private protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();


        public IEnumerable<Actor> GetActors() => Actors;

        public IEnumerable<string> GetAliases() => Aliases;

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

    }
}
