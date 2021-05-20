// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    internal class SeriesBanners
    {
        //All Banners
        public Dictionary<int, Banner> AllBanners; // All Banners linked by bannerId.

        //Collections of Posters and Banners per season
        private Dictionary<int, Banner> seasonBanners; // e.g. Dictionary of the best posters per cachedSeries.
        private Dictionary<int, Banner> seasonLangBanners; // e.g. Dictionary of the best posters per cachedSeries in the correct language.
        private Dictionary<int, Banner> seasonWideBanners; // e.g. Dictionary of the best wide banners per cachedSeries.
        private Dictionary<int, Banner> seasonLangWideBanners; // e.g. Dictionary of the best wide banners per cachedSeries in the correct language.

        //best Banner, Poster and Fanart loaded from the images files (in any language)
        private int bestSeriesPosterId;
        private int bestSeriesBannerId;
        private int bestSeriesFanartId;

        //best Banner, Poster and Fanart loaded from the images files (in our language)
        private int bestSeriesLangPosterId;
        private int bestSeriesLangBannerId;
        private int bestSeriesLangFanartId;

        private readonly CachedSeriesInfo cachedSeries;

        // ReSharper disable once NotNullMemberIsNotInitialized
        public SeriesBanners(CachedSeriesInfo s)
        {
            cachedSeries = s;
            ResetBanners();
        }

        public void ResetBanners()
        {
            AllBanners = new Dictionary<int, Banner>();
            seasonBanners = new Dictionary<int, Banner>();
            seasonLangBanners = new Dictionary<int, Banner>();
            seasonWideBanners = new Dictionary<int, Banner>();
            seasonLangWideBanners = new Dictionary<int, Banner>();

            bestSeriesPosterId = -1;
            bestSeriesBannerId = -1;
            bestSeriesFanartId = -1;
            bestSeriesLangPosterId = -1;
            bestSeriesLangBannerId = -1;
            bestSeriesLangFanartId = -1;
        }

        public void MergeBanners(SeriesBanners? o)
        {
            if (o is null)
            {
                return;
            }

            if (WorthUpdating(o.seasonBanners))
            {
                seasonBanners = o.seasonBanners;
            }

            if (WorthUpdating(o.seasonLangBanners))
            {
                seasonLangBanners = o.seasonLangBanners;
            }

            if (WorthUpdating(o.seasonLangWideBanners))
            {
                seasonLangWideBanners = o.seasonLangWideBanners;
            }

            if (WorthUpdating(o.seasonWideBanners))
            {
                seasonWideBanners = o.seasonWideBanners;
            }

            if (WorthUpdating(o.AllBanners))
            {
                AllBanners = o.AllBanners;
            }

            if (o.bestSeriesPosterId != -1)
            {
                bestSeriesPosterId = o.bestSeriesPosterId;
            }

            if (o.bestSeriesBannerId != -1)
            {
                bestSeriesBannerId = o.bestSeriesBannerId;
            }

            if (o.bestSeriesFanartId != -1)
            {
                bestSeriesFanartId = o.bestSeriesFanartId;
            }

            if (o.bestSeriesLangPosterId != -1)
            {
                bestSeriesLangPosterId = o.bestSeriesLangPosterId;
            }

            if (o.bestSeriesLangBannerId != -1)
            {
                bestSeriesLangBannerId = o.bestSeriesLangBannerId;
            }

            if (o.bestSeriesLangFanartId != -1)
            {
                bestSeriesLangFanartId = o.bestSeriesLangFanartId;
            }
        }

        private static bool WorthUpdating(Dictionary<int, Banner>? b) => b != null && b.Count > 0;

        public string? GetSeasonBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            System.Diagnostics.Debug.Assert(cachedSeries.BannersLoaded);

            if (seasonLangBanners.ContainsKey(snum))
            {
                return seasonLangBanners[snum].BannerPath;
            }

            if (seasonBanners.ContainsKey(snum))
            {
                return seasonBanners[snum].BannerPath;
            }

            //if there is a problem then return the non-season specific poster by default
            return GetSeriesPosterPath();
        }

        public string? GetSeriesWideBannerPath()
        {
            //ry the best one we've found with the correct language
            if (bestSeriesLangBannerId != -1 && AllBanners.ContainsKey(bestSeriesLangBannerId))
            {
                return AllBanners[bestSeriesLangBannerId].BannerPath;
            }

            //if there are none with the right language then try one from another language
            if (bestSeriesBannerId != -1 && AllBanners.ContainsKey(bestSeriesBannerId))
            {
                return AllBanners[bestSeriesBannerId].BannerPath;
            }

            //then choose the one the TVDB recommended _LOWERED IN PRIORITY AFTER LEVERAGE issue - https://github.com/TV-Rename/tvrename/issues/285
            if (!string.IsNullOrEmpty(cachedSeries.BannerString))
            {
                return cachedSeries.BannerString;
            }

            //give up
            return string.Empty;
        }

        [NotNull]
        public string GetSeriesPosterPath()
        {
            //then try the best one we've found with the correct language
            if (bestSeriesLangPosterId != -1 && AllBanners.ContainsKey(bestSeriesLangPosterId))
            {
                return AllBanners[bestSeriesLangPosterId]?.BannerPath ?? string.Empty;
            }

            //if there are none with the righ tlanguage then try one from another language
            if (bestSeriesPosterId != -1 && AllBanners.ContainsKey(bestSeriesPosterId))
            {
                return AllBanners[bestSeriesPosterId]?.BannerPath ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(cachedSeries.PosterUrl))
            {
                return cachedSeries.PosterUrl;
            }
            //give up
            return string.Empty;
        }

    [NotNull]
    public string GetSeriesFanartPath()
        {
            //then try the best one we've found with the correct language
            if (bestSeriesLangFanartId != -1 && AllBanners.ContainsKey(bestSeriesLangFanartId))
            {
                return AllBanners[bestSeriesLangFanartId]?.BannerPath?? string.Empty;
            }

            //if there are none with the righ tlanguage then try one from another language
            if (bestSeriesFanartId != -1 && AllBanners.ContainsKey(bestSeriesFanartId))
            {
                return AllBanners[bestSeriesFanartId]?.BannerPath??string.Empty;
            }

            //give up
            return string.Empty;
        }

        public string? GetSeasonWideBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            System.Diagnostics.Debug.Assert(cachedSeries.BannersLoaded);

            if (seasonLangWideBanners.ContainsKey(snum))
            {
                return seasonLangWideBanners[snum].BannerPath;
            }

            if (seasonWideBanners.ContainsKey(snum))
            {
                return seasonWideBanners[snum].BannerPath;
            }

            //if there is a problem then return the non-season specific poster by default
            return GetSeriesWideBannerPath();
        }

        public void AddOrUpdateBanner([NotNull] Banner banner)
        {
            if (AllBanners.ContainsKey(banner.BannerId))
            {
                AllBanners[banner.BannerId] = banner;
            }
            else
            {
                AllBanners.Add(banner.BannerId, banner);
            }

            if (banner.IsSeasonPoster())
            {
                AddOrUpdateSeasonPoster(banner);
            }

            if (banner.IsSeasonBanner())
            {
                AddOrUpdateWideSeason(banner);
            }

            if (banner.IsSeriesPoster())
            {
                bestSeriesPosterId = GetBestBannerId(banner, bestSeriesPosterId);
            }

            if (banner.IsSeriesBanner())
            {
                bestSeriesBannerId = GetBestBannerId(banner, bestSeriesBannerId);
            }

            if (banner.IsFanart())
            {
                bestSeriesFanartId = GetBestBannerId(banner, bestSeriesFanartId);
            }

            if (banner.LanguageId == cachedSeries.ActualLocale.PreferredLanguage.TVDBId)//todo - make this properly generic
            {
                if (banner.IsSeriesPoster())
                {
                    bestSeriesLangPosterId = GetBestBannerId(banner, bestSeriesLangPosterId);
                }

                if (banner.IsSeriesBanner())
                {
                    bestSeriesLangBannerId = GetBestBannerId(banner, bestSeriesLangBannerId);
                }

                if (banner.IsFanart())
                {
                    bestSeriesLangFanartId = GetBestBannerId(banner, bestSeriesLangFanartId);
                }
            }
        }

        private int GetBestBannerId([NotNull] Banner selectedBanner, int bestBannerId)
        {
            if (bestBannerId == -1)
            {
                return selectedBanner.BannerId;
            }

            if (!AllBanners.ContainsKey(bestBannerId))
            {
                return selectedBanner.BannerId;
            }

            if (AllBanners[bestBannerId].Rating < selectedBanner.Rating)
            {
                //update banner - we have found a better one
                return selectedBanner.BannerId;
            }

            return bestBannerId;
        }

        private void AddOrUpdateSeasonPoster([NotNull] Banner banner)
        {
            AddUpdateIntoCollections(banner, seasonBanners, seasonLangBanners);
        }

        private void AddOrUpdateWideSeason([NotNull] Banner banner)
        {
            AddUpdateIntoCollections(banner, seasonWideBanners, seasonLangWideBanners);
        }

        private void AddUpdateIntoCollections([NotNull] Banner banner, [NotNull] IDictionary<int, Banner> coll, IDictionary<int, Banner> langColl)
        {
            //update language specific cache if appropriate
            if (banner.LanguageId == cachedSeries.ActualLocale.PreferredLanguage.TVDBId)
            {
                AddUpdateIntoCollection(banner, langColl);
            }
            //Now do the same for the all banners dictionary
            AddUpdateIntoCollection(banner, coll);
        }

        private static void AddUpdateIntoCollection([NotNull] Banner banner, [NotNull] IDictionary<int, Banner> coll)
        {
            int seasonOfNewBanner = banner.SeasonId;

            if (coll.ContainsKey(seasonOfNewBanner))
            {
                //it already contains a season of the approprite type - see which is better
                if (coll[seasonOfNewBanner].Rating < banner.Rating)
                {
                    //update banner - we have found a better one
                    coll[seasonOfNewBanner] = banner;
                }
            }
            else
            {
                coll.Add(seasonOfNewBanner, banner);
            }
        }

        public string? GetImage(TVSettings.FolderJpgIsType type)
        {
            return type switch
            {
                TVSettings.FolderJpgIsType.Banner => GetSeriesWideBannerPath(),
                TVSettings.FolderJpgIsType.FanArt => GetSeriesFanartPath(),
                TVSettings.FolderJpgIsType.SeasonPoster => GetSeriesPosterPath(),
                _ => GetSeriesPosterPath()
            };
        }

        public void Remove(int removeBannerId)
        {
            AllBanners.Remove(removeBannerId);

            if (bestSeriesPosterId == removeBannerId)
            {
                if (AllBanners.Any(pair => pair.Value.IsSeriesPoster()))
                {
                    double maxRating = AllBanners.Where(pair => pair.Value.IsSeriesPoster())
                        .Select(pair => pair.Value.Rating).Max();

                    bestSeriesPosterId = AllBanners.Where(pair => pair.Value.IsSeriesPoster())
                        .First(pair => Math.Abs(pair.Value.Rating - maxRating) < 0.001).Value.BannerId;
                }
                else
                {
                    bestSeriesPosterId = -1;
                }
            }

            if (bestSeriesBannerId == removeBannerId)
            {
                if (AllBanners.Any(pair => pair.Value.IsSeriesBanner()))
                {
                    double maxRating = AllBanners.Where(pair => pair.Value.IsSeriesBanner())
                    .Select(pair => pair.Value.Rating).Max();

                bestSeriesBannerId = AllBanners.Where(pair => pair.Value.IsSeriesBanner())
                    .First(pair => Math.Abs(pair.Value.Rating - maxRating) < 0.001).Value.BannerId;
                }
                else
                {
                    bestSeriesBannerId = -1;
                }
            }

            if (bestSeriesFanartId == removeBannerId)
            {
                if (AllBanners.Any(pair => pair.Value.IsFanart()))
                {
                    double maxRating = AllBanners.Where(pair => pair.Value.IsFanart())
                    .Select(pair => pair.Value.Rating).Max();

                bestSeriesFanartId = AllBanners.Where(pair => pair.Value.IsFanart())
                    .First(pair => Math.Abs(pair.Value.Rating - maxRating) < 0.001).Value.BannerId;
                }
                else
                {
                    bestSeriesFanartId = -1;
                }
            }
        }
    }
}
