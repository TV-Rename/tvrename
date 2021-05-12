using System.Collections.Concurrent;

namespace TVRename.Forms
{
    public class Recomendations : ConcurrentDictionary<int,RecommendationResult>
    {
        private RecommendationResult Enrich(int key)
        {
            if (TryGetValue(key, out RecommendationResult movieRec))
            {
                return movieRec;
            }

            RecommendationResult x = new RecommendationResult {Key = key};
            TryAdd(key, x);
            return x;
        }

        public void AddTopRated(int key)
        {
            Enrich(key).TopRated = true;
        }

        public void AddTrending(int key)
        {
            Enrich(key).Trending = true;
        }

        public void AddRelated(int key, MediaConfiguration sourceId)
        {
            Enrich(key).Related.Add(sourceId);
        }

        public void AddSimilar(int key, MediaConfiguration sourceId)
        {
            Enrich(key).Similar.Add(sourceId);
        }
    }
}