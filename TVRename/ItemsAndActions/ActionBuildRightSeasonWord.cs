// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;

namespace TVRename.ItemsAndActions
{
    class ActionBuildRightSeasonWord
    {
        public string SeasonName { get; }

        public ActionBuildRightSeasonWord ( int season )
        {
            if (TVSettings.Instance.defaultSeasonWord.Length > 1)
            {
                this.SeasonName = TVSettings.Instance.defaultSeasonWord + " " + season;
            }
            else
            {
                bool leadingZero = TVSettings.Instance.LeadingZeroOnSeason;
                if (leadingZero == true)
                {
                    this.SeasonName = TVSettings.Instance.defaultSeasonWord + season.ToString("00");
                }
                else
                {
                    this.SeasonName = TVSettings.Instance.defaultSeasonWord + season.ToString();
                }
            }
        }

        ~ActionBuildRightSeasonWord()
        {
        }

    }
}
