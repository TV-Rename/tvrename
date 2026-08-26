    //
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename;

public class PossibleMergedEpisode(ProcessedEpisode episodeOne, ProcessedEpisode episodeTwo, int season, bool airDatesMatch, bool similarNames, bool oneFound, bool largeFileSize)
{
    private readonly ProcessedEpisode episodeOne = episodeOne;
    private readonly ProcessedEpisode episodeTwo = episodeTwo;
    public readonly int SeasonNumber = season;
    public readonly bool AirDatesMatch = airDatesMatch;
    public readonly bool SimilarNames = similarNames;
    public readonly bool OneFound = oneFound;
    public readonly bool LargeFileSize = largeFileSize;

    public ListViewItem PresentationView
    {
        get
        {
            ListViewItem lvi = new()
            {
                Text = episodeOne.Show.ShowName
            };

            lvi.SubItems.Add(episodeOne.AppropriateSeasonNumber.ToString());
            lvi.SubItems.Add(episodeOne.EpisodeNumbersAsText + " & " + episodeTwo.EpisodeNumbersAsText);
            lvi.SubItems.Add(episodeOne.GetAirDateDt().PrettyPrint());
            lvi.SubItems.Add(episodeOne.Name + " & " + episodeTwo.Name);

            List<string> names = [episodeOne.Name, episodeTwo.Name];
            string combinedName = ShowLibrary.GetBestNameFor(names, "");
            lvi.SubItems.Add(combinedName);

            lvi.Tag = this;

            return lvi;
        }
    }

    public ShowConfiguration ShowConfiguration => episodeTwo.Show;
    public ProcessedEpisode Episode => episodeOne;

    public ShowRule GenerateRule()
    {
        return new()
        {
            DoWhatNow = RuleAction.kMerge,
            First = episodeOne.AppropriateEpNum,
            Second = episodeTwo.AppropriateEpNum
        };
    }
}
