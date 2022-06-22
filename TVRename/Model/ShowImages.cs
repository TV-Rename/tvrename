using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    public class ShowImages : SafeList<ShowImage>
    {
        internal ShowImage? GetSeasonWideBanner(int snum, Language lang)
        {
            return GetImage(snum, lang, MediaImage.ImageType.wideBanner);
        }

        internal ShowImage? GetSeasonBanner(int snum, Language lang)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            return GetImage(snum, lang, MediaImage.ImageType.poster);
        }

        internal ShowImage? GetImage(int snum, Language lang, MediaImage.ImageType type)
        {
            return GetBestSeasonLanguage(snum, lang, type)
                   ?? GetBestSeason(snum, type)
                   ?? GetShowImage(lang, type);
        }

        internal ShowImage? GetShowImage(Language lang, MediaImage.ImageType type)
        {
            return GetSeriesLangImage(lang, type)
                   ?? GetSeriesImage(type);
        }

        private ShowImage? GetBestSeason(int snum, MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> validImages = this.Where(i => i.ImageStyle == type && i.SeasonNumber == snum);
            return BestFrom(validImages);
        }

        private ShowImage? GetSeriesLangImage(Language l, MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> validImages = this.Where(i => i.ImageStyle == type && i.Subject == MediaImage.ImageSubject.show && (i.LanguageCode == l.ThreeAbbreviation || i.LanguageCode ==l.Abbreviation));
            return BestFrom(validImages);
        }

        private ShowImage? GetBestSeasonLanguage(int snum, Language l, MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> validImages = this.Where(i => i.ImageStyle == type && i.SeasonNumber == snum && (i.LanguageCode == l.ThreeAbbreviation || i.LanguageCode == l.Abbreviation));
            return BestFrom(validImages);
        }

        private ShowImage? GetSeriesImage(MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> validImages = this.Where(i => i.ImageStyle == type && i.Subject == MediaImage.ImageSubject.show);
            return BestFrom(validImages);
        }

        private static ShowImage? BestFrom(IEnumerable<ShowImage> validImages)
        {
            List<ShowImage> showImages = validImages.ToList();
            if (!showImages.Any())
            {
                return null;
            }
            double maxRating = showImages.Select(pair => pair.Rating).Max();

            return showImages.First(pair => Math.Abs(pair.Rating - maxRating) < 0.001);
        }

        public ShowImage? GetImage(TVSettings.FolderJpgIsType type, Language lang)
        {
            return type switch
            {
                TVSettings.FolderJpgIsType.Banner => GetShowImage(lang, MediaImage.ImageType.wideBanner),
                TVSettings.FolderJpgIsType.FanArt => GetShowImage(lang, MediaImage.ImageType.background),
                TVSettings.FolderJpgIsType.SeasonPoster => GetShowImage(lang, MediaImage.ImageType.poster),
                _ => GetShowImage(lang, MediaImage.ImageType.poster)
            };
        }

        internal void MergeImages(ShowImages images)
        {
            if (!this.Any())
            {
                Clear();
                AddRange(images);
                return;
            }
            foreach (ShowImage i in images)
            {
                if (this.All(si => si.Id != i.Id))
                {
                    Add(i);
                }
            }
        }
    }
}
