using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TVRename
{
    internal static class MergedEpisodeFinderController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [NotNull]
        internal static List<PossibleMergedEpisode> FindDoubleEps(TVDoc doc, [NotNull] BackgroundWorker worker)
        {
            int total = doc.TvLibrary.Count;
            int current = 0;

            doc.PreventAutoScan("Find Double Episodes");
            StringBuilder output = new StringBuilder();
            List<PossibleMergedEpisode> returnValue = new List<PossibleMergedEpisode>();

            output.AppendLine("");
            output.AppendLine("##################################################");
            output.AppendLine("MERGED EPISODES FINDER - Start");
            output.AppendLine("##################################################");

            DirFilesCache dfc = new DirFilesCache();
            foreach (ShowConfiguration si in doc.TvLibrary.GetSortedShowItems())
            {
                worker.ReportProgress(100 * current++ / total, si.ShowName);

                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.ActiveSeasons)
                {
                    //Ignore seasons that all aired on same date
                    DateTime? seasonMinAirDate = (from pep in kvp.Value select pep.FirstAired).Min();
                    DateTime? seasonMaxAirDate = (from pep in kvp.Value select pep.FirstAired).Max();
                    if (seasonMaxAirDate.HasValue && seasonMinAirDate.HasValue &&
                        seasonMaxAirDate == seasonMinAirDate)
                    {
                        continue;
                    }

                    //Search through each pair of episodes for the same season
                    foreach (ProcessedEpisode pep in kvp.Value)
                    {
                        SearchForDuplicates(pep, output, si, kvp.Key, kvp.Value, dfc, returnValue);
                    }
                }
            }

            output.AppendLine("##################################################");
            output.AppendLine("MERGED EPISODES FINDER - End");
            output.AppendLine("##################################################");

            Logger.Info(output.ToString());
            doc.AllowAutoScan();
            return returnValue;
        }

        private static void SearchForDuplicates([NotNull] ProcessedEpisode pep, StringBuilder output, ShowConfiguration si, int seasonId, [NotNull] IEnumerable<ProcessedEpisode> seasonEpisodes, DirFilesCache dfc, List<PossibleMergedEpisode> returnValue)
        {
            if (pep.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
            {
                output.AppendLine(si.ShowName + " - Season: " + seasonId + " - " + pep.EpNumsAsString() +
                                  " - " + pep.Name + " is:");

                foreach (Episode sourceEpisode in pep.SourceEpisodes)
                {
                    output.AppendLine("                      - " + sourceEpisode.AiredEpNum + " - " +
                                      sourceEpisode.Name);
                }
            }

            foreach (ProcessedEpisode comparePep in seasonEpisodes.Where(comparePep => EpisodesMatch(pep, comparePep)))
            {
                // Tell user about this possibility
                output.AppendLine($"{si.ShowName} - Season: {seasonId} - {pep.FirstAired.ToString()} - {pep.AiredEpNum}({pep.Name}) - {comparePep.AiredEpNum}({comparePep.Name})");

                //do the 'name' test
                string root = StringExtensions.GetCommonStartString(pep.Name, comparePep.Name);
                bool sameLength = pep.Name.Length == comparePep.Name.Length;
                bool sameName = !root.Trim().Equals("Episode") && sameLength && root.Length > 3 && root.Length > pep.Name.Length / 2;

                bool oneFound = false;
                bool largerFileSize = false;
                if (sameName)
                {
                    oneFound = IsOneFound(output, dfc, pep, comparePep, ref largerFileSize);
                }

                returnValue.Add(new PossibleMergedEpisode(pep, comparePep, seasonId, true, sameName, oneFound, largerFileSize));
            }
        }

        private static bool EpisodesMatch([NotNull] Episode pep, Episode comparePep)
        {
            return pep.FirstAired.HasValue && comparePep.FirstAired.HasValue &&
                   pep.FirstAired == comparePep.FirstAired && pep.EpisodeId < comparePep.EpisodeId;
        }

        private static bool IsOneFound([NotNull] StringBuilder output, DirFilesCache dfc, [NotNull] ProcessedEpisode pep, [NotNull] ProcessedEpisode comparePep, ref bool largerFileSize)
        {
            output.AppendLine("####### POSSIBLE MERGED FILE DUE TO NAME##########");

            //Do the missing Test (ie is one missing and not the other)
            bool pepFound = dfc.FindEpOnDisk(pep).Any();
            bool comparePepFound = dfc.FindEpOnDisk(comparePep).Any();
            bool oneFound = pepFound ^ comparePepFound;
            if (oneFound)
            {
                output.AppendLine(
                    "####### POSSIBLE MERGED FILE DUE TO ONE MISSING AND ONE FOUND ##########");

                ProcessedEpisode possibleDupEpisode = pepFound ? pep : comparePep;
                //Test the file sizes in the season
                //More than 40% longer
                FileInfo possibleDupFile = dfc.FindEpOnDisk(possibleDupEpisode)[0];
                int dupMovieLength = possibleDupFile.GetFilmLength();
                List<int> otherMovieLengths = new List<int>();
                foreach (FileInfo file in possibleDupFile.Directory.EnumerateFiles())
                {
                    if (file.IsMovieFile())
                    {
                        otherMovieLengths.Add(file.GetFilmLength());
                    }
                }

                int averageMovieLength = otherMovieLengths.Count == 1
                    ? otherMovieLengths.Sum()
                    : (otherMovieLengths.Sum() - dupMovieLength) / (otherMovieLengths.Count - 1);

                largerFileSize = dupMovieLength > averageMovieLength * 1.4;
                if (largerFileSize)
                {
                    {
                        output.AppendLine(
                            "######################################################################");

                        output.AppendLine(
                            "####### SURELY WE HAVE ONE NOW                              ##########");

                        output.AppendLine(
                            $"####### {possibleDupEpisode.AiredEpNum}({possibleDupEpisode.Name}) has length {dupMovieLength} greater than the average in the directory of {averageMovieLength}");

                        output.AppendLine(
                            "######################################################################");
                    }
                }
            }

            return oneFound;
        }
    }
}