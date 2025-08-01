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

namespace TVRename;

public class ShowFilter
{
    public List<string> Genres { get; } = [];
    public string? ShowName { get; set; }
    public string? ShowStatus { get; set; }
    public bool ShowStatusInclude { get; set; }
    public string? ShowNetwork { get; set; }
    public bool ShowNetworkInclude { get; set; }
    public string? ShowRating { get; set; }
    public bool ShowRatingInclude { get; set; }
    public bool IncludeBlankFields { get; set; }
    public bool IsEnabled =>
        ShowName.HasValue() ||
        ShowStatus.HasValue() ||
        ShowNetwork.HasValue() ||
        ShowRating.HasValue() ||
        Genres.Any();

    public bool Filter(ShowConfiguration show)
    {
        bool IsNetworkOk(ShowConfiguration showItem)
        {
            List<string>? seriesInfoNetwork = showItem.CachedShow?.Networks.ToList();
            if (seriesInfoNetwork is null || !seriesInfoNetwork.HasAny())
            {
                return IncludeBlankFields;
            }

            return ShowNetworkInclude
                ? seriesInfoNetwork.Contains(ShowNetwork)
                : !seriesInfoNetwork.Contains(ShowNetwork);
        }

        bool IsRatingOk(ShowConfiguration showItem)
        {
            string? seriesInfoContentRating = showItem.CachedShow?.ContentRating;
            if (seriesInfoContentRating is null)
            {
                return IncludeBlankFields;
            }

            return ShowRatingInclude
                ? seriesInfoContentRating.Equals(ShowRating)
                : !seriesInfoContentRating.Equals(ShowRating);
        }

        bool IsStatusOk(ShowConfiguration showItem)
        {
            string? seriesInfoStatus = showItem.CachedShow?.Status;
            if (seriesInfoStatus is null)
            {
                return IncludeBlankFields;
            }

            return ShowStatusInclude
                ? showItem.ShowStatus.Equals(ShowStatus)
                : !showItem.ShowStatus.Equals(ShowStatus);
        }

        //Filter on show name
        bool isNameOk = ShowName is null || show.ShowName.Contains(ShowName, StringComparison.OrdinalIgnoreCase);

        //Filter on show status
        bool isStatusOk = ShowStatus is null || IsStatusOk(show);

        //Filter on show network
        bool isNetworkOk = ShowNetwork is null || IsNetworkOk(show);

        //Filter on show rating
        bool isRatingOk = ShowRating is null || IsRatingOk(show);

        //Filter on show genres
        bool areGenresIgnored = Genres.Count == 0;
        bool doAnyGenresMatch = FindMatchingGenres(show);

        return isNameOk && isStatusOk && isNetworkOk && isRatingOk && (areGenresIgnored || doAnyGenresMatch);
    }

    private bool FindMatchingGenres(ShowConfiguration show)
    {
        return show.Genres.Any(showGenre => Genres.Contains(showGenre));
    }
}
