using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename
{
    public class PossibleDuplicateEpisode

    {
        public ProcessedEpisode episodeOne;
        public ProcessedEpisode episodeTwo;
        internal int SeasonNumber;
        public bool AirDatesMatch;
        public bool SimilarNames;
        public bool OneFound;
        public bool LargeFileSize;

        public PossibleDuplicateEpisode(ProcessedEpisode episodeOne, ProcessedEpisode episodeTwo, int season,
            bool AirDatesMatch, bool SimilarNames, bool OneFound, bool LargeFileSize)
        {
            this.episodeTwo = episodeTwo;
            this.episodeOne = episodeOne;
            SeasonNumber = season;
            this.AirDatesMatch = AirDatesMatch;
            this.SimilarNames = SimilarNames;
            this.OneFound = OneFound;
            this.LargeFileSize = LargeFileSize;
        }


        public ListViewItem PresentationView
        {
            get
            {
                ListViewItem lvi = new ListViewItem
                {
                    Text = episodeOne.SI.ShowName
                };

                lvi.SubItems.Add(episodeOne.AppropriateSeasonNumber.ToString());
                lvi.SubItems.Add(episodeOne.NumsAsString() + " & " + episodeTwo.NumsAsString());

                DateTime? dt = episodeOne.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(episodeOne.Name + " & " + episodeTwo.Name);
                
                List<string> names = new List<string> {episodeOne.Name, episodeTwo.Name};
                string combinedName = ShowLibrary.GetBestNameFor(names, "");
                lvi.SubItems.Add(combinedName);

                lvi.Tag = this;

                return lvi;
            }
        }

        public ShowItem ShowItem => episodeTwo.SI;
        public ProcessedEpisode Episode => episodeOne;
    }

}
