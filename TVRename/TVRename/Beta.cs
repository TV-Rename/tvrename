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
using System.Text;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal static class Beta
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal static void LogShowEpisodeSizes([NotNull] TVDoc doc)
        {
            doc.PreventAutoScan("Show File Sizes");
            StringBuilder output = new StringBuilder();

            output.AppendLine("");
            output.AppendLine("##################################################");
            output.AppendLine("File Quailty FINDER - Start");
            output.AppendLine("##################################################");
            Logger.Info(output.ToString());

            DirFilesCache dfc = new DirFilesCache();
            foreach (ShowItem si in doc.Library.Values)
            {
                foreach (List<ProcessedEpisode> episodes in si.SeasonEpisodes.Values.ToList())
                {
                    foreach (ProcessedEpisode pep in episodes)
                    {
                        List<FileInfo> files = dfc.FindEpOnDisk(pep);
                        foreach (FileInfo file in files)
                        {
                            int width = file.GetFrameWidth();
                            int height = file.GetFrameHeight();
                            int length = file.GetFilmLength();
                            Logger.Info($"{width,-10}   {height,-10}   {length,-10}    {pep.Show.ShowName,-50}  {file.Name}");
                        }
                    }
                }
            }

            output.Clear();
            output.AppendLine("##################################################");
            output.AppendLine("File Quailty FINDER - End");
            output.AppendLine("##################################################");

            Logger.Info(output.ToString());
            doc.AllowAutoScan();
        }

        [NotNull]
        internal static List<PossibleDuplicateEpisode> FindDoubleEps([NotNull] TVDoc doc)
        {
            doc.PreventAutoScan("Find Double Episodes");
            StringBuilder output = new StringBuilder();
            List<PossibleDuplicateEpisode> returnValue = new List<PossibleDuplicateEpisode>();

            output.AppendLine("");
            output.AppendLine("##################################################");
            output.AppendLine("DUPLICATE FINDER - Start");
            output.AppendLine("##################################################");

            DirFilesCache dfc = new DirFilesCache();
            foreach (ShowItem si in doc.Library.Values)
            {
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
            output.AppendLine("DUPLICATE FINDER - End");
            output.AppendLine("##################################################");

            Logger.Info(output.ToString());
            doc.AllowAutoScan();
            return returnValue;
        }

        private static void SearchForDuplicates([NotNull] ProcessedEpisode pep, StringBuilder output, ShowItem si, int seasonId, [NotNull] IEnumerable<ProcessedEpisode> seasonEpisodes, DirFilesCache dfc, List<PossibleDuplicateEpisode> returnValue)
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

                returnValue.Add(new PossibleDuplicateEpisode(pep, comparePep, seasonId, true, sameName, oneFound, largerFileSize));
            }
        }

        private static bool EpisodesMatch([NotNull] ProcessedEpisode pep, ProcessedEpisode comparePep)
        {
            return pep.FirstAired.HasValue && comparePep.FirstAired.HasValue &&
                   pep.FirstAired == comparePep.FirstAired && pep.EpisodeId < comparePep.EpisodeId;
        }

        private static bool IsOneFound([NotNull] StringBuilder output, DirFilesCache dfc, [NotNull] ProcessedEpisode pep, [NotNull] ProcessedEpisode comparePep, ref bool largerFileSize)
        {
            output.AppendLine("####### POSSIBLE DUPLICATE DUE TO NAME##########");

            //Do the missing Test (ie is one missing and not the other)
            bool pepFound = dfc.FindEpOnDisk(pep).Any();
            bool comparePepFound = dfc.FindEpOnDisk(comparePep).Any();
            bool oneFound = pepFound ^ comparePepFound;
            if (oneFound)
            {
                output.AppendLine(
                    "####### POSSIBLE DUPLICATE DUE TO ONE MISSING AND ONE FOUND ##########");

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

                int averageMovieLength =otherMovieLengths.Count ==1
                    ?otherMovieLengths.Sum()
                    :(otherMovieLengths.Sum() - dupMovieLength) / (otherMovieLengths.Count - 1);

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
