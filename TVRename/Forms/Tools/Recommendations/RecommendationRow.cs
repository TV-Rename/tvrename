using System;
using System.Linq;

namespace TVRename.Forms;

public class RecommendationRow
{
    private readonly RecommendationResult result;
    private readonly MediaConfiguration.MediaType type;
    public readonly CachedSeriesInfo? Series;
    public readonly CachedMovieInfo? Movie;
    private readonly CachedMediaInfo? cachedMediaInfo;
    private readonly int trendingWeight, topWeight, relatedWeight, similarWeight, maxRelated, maxSimilar;

    /// <exception cref="ArgumentOutOfRangeException">MediaType.</exception>
    public RecommendationRow(RecommendationResult x, MediaConfiguration.MediaType t, int trendingWeight, int topWeight, int relatedWeight, int similarWeight, int maxRelated, int maxSimilar)
    {
        result = x;
        type = t;
        switch (t)
        {
            case MediaConfiguration.MediaType.tv:
                Series = TMDB.LocalCache.Instance.GetSeries(x.Key);
                cachedMediaInfo = Series;
                break;

            case MediaConfiguration.MediaType.movie:
                Movie = TMDB.LocalCache.Instance.GetMovie(x.Key);
                cachedMediaInfo = Movie;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(t), t, null);
        }

        this.trendingWeight = trendingWeight;
        this.topWeight = topWeight;
        this.relatedWeight = relatedWeight;
        this.similarWeight = similarWeight;
        this.maxRelated = maxRelated;
        this.maxSimilar = maxSimilar;
    }

    public int Key => result.Key;
    public string? Name => cachedMediaInfo?.Name;
    public string? Overview => cachedMediaInfo?.Overview;

    public string? Year => type == MediaConfiguration.MediaType.movie
        ? Movie?.Year.ToString()
        : Series?.Year;

    public bool TopRated => result.TopRated;
    public bool Trending => result.Trending;
    public string? Language => cachedMediaInfo?.ShowLanguage;

    //Star score is out of 5 stars, we produce a 'normlised' result by adding a top mark 10/10 and a bottom mark 1/10 and recalculating
    //this is to stop a show with one 10/10 vote looking too good, this normalises it back if the number of votes is small
    public float StarScore => NormalisedScore(cachedMediaInfo?.SiteRatingVotes, cachedMediaInfo?.SiteRating);

    private static float NormalisedScore(int? votes, float? score)
    {
        float totalScore = ((votes ?? 1) * (score ?? 5)) + 10 + 1;
        int totalVotes = (votes ?? 1) + 2;
        return totalScore / totalVotes;
    }

    public double RecommendationScore => result.GetScore(trendingWeight, topWeight, relatedWeight, similarWeight, maxRelated, maxSimilar);

    public string Reason => result.Similar.Select(configuration => configuration.ShowName).ToCsv() + "-" + result.Related.Select(configuration => configuration.ShowName).ToCsv();
    public string Similar => result.Similar.Select(configuration => configuration.ShowName).ToCsv();
    public string Related => result.Related.Select(configuration => configuration.ShowName).ToCsv();
}
