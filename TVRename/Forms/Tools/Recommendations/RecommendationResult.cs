//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename.Forms;

public class RecommendationResult
{
    internal int Key;
    internal bool Trending;
    internal bool TopRated;
    internal readonly List<MediaConfiguration> Related = [];
    internal readonly List<MediaConfiguration> Similar = [];

    public double GetScore(int trendingWeight, int topWeight, int relatedWeight, int similarWeight, int maxRelated, int maxSimilar)
    {
        return ((Trending ? trendingWeight : 0)
                  + (TopRated ? topWeight : 0)
                  + (1.0 * relatedWeight * Related.Count / maxRelated)
                  + (1.0 * similarWeight * Similar.Count / maxSimilar))
               / (trendingWeight + topWeight + similarWeight + relatedWeight);
    }
}
