using System.Collections.Generic;

namespace TVRename.Forms
{
    public class RecommendationResult
    {
        internal int Key;
        internal bool Trending;
        internal bool TopRated;
        internal readonly List<MediaConfiguration> Related = new List<MediaConfiguration>();
        internal readonly List<MediaConfiguration> Similar = new List<MediaConfiguration>();

        public double GetScore(int trendingWeight, int topWeight, int relatedWeight, int similarWeight,int maxRelated, int maxSimilar)
        {
            return (  (Trending ? trendingWeight : 0)
                   + (TopRated ? topWeight : 0)
                   + (1.0 * relatedWeight * Related.Count / maxRelated)
                   + (1.0 * similarWeight * Similar.Count / maxSimilar))
                   /(trendingWeight+topWeight+similarWeight+relatedWeight);
        }
    }
}
