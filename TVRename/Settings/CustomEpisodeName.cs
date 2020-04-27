// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using NLog;

// This builds the filenames to rename to, for any given episode (or multi-episode episode)

namespace TVRename
{
    public class CustomEpisodeName
    {
        public string StyleString;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CustomEpisodeName(string s)
        {
            StyleString = s;
        }

        public CustomEpisodeName()
        {
            StyleString = DefaultStyle();
        }

        private static string DefaultStyle() => PRESETS[1];

        public static string OldNStyle(int n)
        {
            // for now, this maps onto the presets
            if (n >= 0 && n < 9)
            {
                return PRESETS[n];
            }

            return DefaultStyle();
        }

        protected internal static readonly List<string> PRESETS = new List<string>
                                                        {
                                                            "{ShowName} - {Season}x{Episode}[-{Season}x{Episode2}] - {EpisodeName}",
                                                            "{ShowName} - S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
                                                            "{ShowName} S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
                                                            "{Season}{Episode}[-{Season}{Episode2}] - {EpisodeName}",
                                                            "{Season}x{Episode}[-{Season}x{Episode2}] - {EpisodeName}",
                                                            "S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
                                                            "E{Episode}[-E{Episode2}] - {EpisodeName}",
                                                            "{Episode}[-{Episode2}] - {ShowName} - 3 - {EpisodeName}",
                                                            "{Episode}[-{Episode2}] - {EpisodeName}",
                                                            "{ShowName} - S{Season:2}{AllEpisodes} - {EpisodeName}"
                                                        };

        protected internal static readonly List<string> TAGS = new List<string>
        {
            "{ShowName}",
            "{ShowNameInitial}",
            "{ShowNameLower}",
            "{Season}",
            "{Season:2}",
            "{SeasonNumber}",
            "{SeasonNumber:2}",
            "{Episode}",
            "{Episode2}",
            "{EpisodeName}",
            "{Number}",
            "{Number:2}",
            "{Number:3}",
            "{ShortDate}",
            "{LongDate}",
            "{YMDDate}",
            "{AllEpisodes}",
            "{Year}",
            "{SeasonYear}",
            "{Imdb}",
            "{ShowImdb}"
        };

        [NotNull]
        public string NameFor([NotNull] ProcessedEpisode pe) => NameFor(pe,string.Empty,0);

        [NotNull]
        public string NameFor([NotNull] ProcessedEpisode pe, [CanBeNull] string extension,int folderNameLength)
        {
            const int MAX_LENGTH = 260;
            int maxFilenameLength = MAX_LENGTH - 1 - folderNameLength - (extension?.Length ?? 5); //Assume a max 5 character extension

            if (maxFilenameLength <= 12)//assume we need space for a 8.3 length filename at least
            {
                throw new System.IO.PathTooLongException(
                    $"Cannot create files as path is too long - please review settings for {pe.Show.ShowName}");
            }

            string r = NameForNoExt(pe, StyleString);

            if (string.IsNullOrEmpty(extension))
            {
                return r.Substring(0,Math.Min(maxFilenameLength, r.Length));
            }

            bool needsSpacer = !extension.StartsWith(".", StringComparison.Ordinal);

            if (needsSpacer)
            {
                return r.Substring(0, Math.Min(r.Length, maxFilenameLength)) + "." + extension;
            }

            return r.Substring(0, Math.Min(r.Length, maxFilenameLength)) + extension;
        }

        [NotNull]
        public string GetTargetEpisodeName([NotNull] ShowItem show, [NotNull] Episode ep)
            => GetTargetEpisodeName(show, ep, false);

        [NotNull]
        private string GetTargetEpisodeName([NotNull] ShowItem show, [NotNull] Episode ep, bool urlEncode)
        {
            //note this is for an Episode and not a ProcessedEpisode
            string name = StyleString;

            string epname = ep.Name;

            name = name.ReplaceInsensitive("{ShowName}", show.ShowName);
            name = name.ReplaceInsensitive("{ShowNameLower}", show.ShowName.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
            name = name.ReplaceInsensitive("{ShowNameInitial}", show.ShowName.Initial().ToLower());
            switch (show.Order)
            {
                case ProcessedSeason.SeasonType.dvd:
                    name = name.ReplaceInsensitive("{Season}", ep.DvdSeasonNumber.ToString());
                    name = name.ReplaceInsensitive("{Season:2}", ep.DvdSeasonNumber.ToString("00"));
                    name = name.ReplaceInsensitive("{SeasonNumber}", show.GetSeasonIndex(ep.DvdSeasonNumber).ToString());
                    name = name.ReplaceInsensitive("{SeasonNumber:2}", show.GetSeasonIndex(ep.DvdSeasonNumber).ToString("00"));
                    name = name.ReplaceInsensitive("{Episode}", ep.DvdEpNum.ToString("00"));
                    name = name.ReplaceInsensitive("{Episode2}", ep.DvdEpNum.ToString("00"));
                    name = Regex.Replace(name, "{AllEpisodes}", ep.DvdEpNum.ToString("00")); break;

                case ProcessedSeason.SeasonType.aired:
                    name = name.ReplaceInsensitive("{Season}", ep.AiredSeasonNumber.ToString());
                    name = name.ReplaceInsensitive("{Season:2}", ep.AiredSeasonNumber.ToString("00"));
                    name = name.ReplaceInsensitive("{SeasonNumber}", show.GetSeasonIndex(ep.AiredSeasonNumber).ToString());
                    name = name.ReplaceInsensitive("{SeasonNumber:2}", show.GetSeasonIndex(ep.AiredSeasonNumber).ToString("00"));
                    name = name.ReplaceInsensitive("{Episode}", ep.AiredEpNum.ToString("00"));
                    name = name.ReplaceInsensitive("{Episode2}", ep.AiredEpNum.ToString("00"));
                    name = Regex.Replace(name, "{AllEpisodes}", ep.AiredEpNum.ToString("00"));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            name = name.ReplaceInsensitive("{EpisodeName}", epname);
            name = name.ReplaceInsensitive("{Number}", "");
            name = name.ReplaceInsensitive("{Number:2}", "");
            name = name.ReplaceInsensitive("{Number:3}", "");
            name = name.ReplaceInsensitive("{Imdb}", ep.ImdbCode);

            SeriesInfo si = show.TheSeries();
            name = name.ReplaceInsensitive("{ShowImdb}", si?.Imdb??string.Empty);
            name = name.ReplaceInsensitive("{Year}", si?.MinYear.ToString() ?? string.Empty);

            ProcessedSeason selectedProcessedSeason = show.GetSeason(ep.GetSeasonNumber(show.Order) );
            name = name.ReplaceInsensitive("{SeasonYear}", selectedProcessedSeason != null ? selectedProcessedSeason.MinYear().ToString() : string.Empty);

            name = ReplaceDates(urlEncode, name, ep.GetAirDateDt(show.GetTimeZone()));

            name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts

            name = name.Replace("\\[", "[");
            name = name.Replace("\\]", "]");

            return name.Trim();
        }

        [NotNull]
        private static string ReplaceDates(bool urlEncode, string name, DateTime? airdt)
        {
            try
            {
                string ymd;

                if (airdt != null)
                {
                    DateTime dt = (DateTime)airdt;
                    name = name.ReplaceInsensitive("{ShortDate}", dt.ToString("d"));
                    name = name.ReplaceInsensitive("{LongDate}", dt.ToString("D"));
                    ymd = dt.ToString("yyyy/MM/dd");
                }
                else
                {
                    name = name.ReplaceInsensitive("{ShortDate}", "---");
                    name = name.ReplaceInsensitive("{LongDate}", "------");
                    ymd = "----/--/--";
                }
                if (urlEncode)
                {
                    ymd = Uri.EscapeDataString(ymd);
                }

                name = name.ReplaceInsensitive("{YMDDate}", ymd);

                return name;
            }
            catch(ArgumentOutOfRangeException)
            {
                if (name.Contains("{ShortDate}") || name.Contains("{LongDate}") || name.Contains("{YMDDate}"))
                {
                    //We don't care that the date can't be parsed
                    return name;
                }
                throw;
            }
        }

        [NotNull]
        public static string NameForNoExt([NotNull] ProcessedEpisode pe, [NotNull]  string styleString) => NameForNoExt(pe, styleString, false);

        [NotNull]
        public static string NameForNoExt([NotNull] ProcessedEpisode pe, [NotNull]  string styleString, bool urlEncode)
        {
            try
            {
                string name = styleString;

                string showname = pe.Show.ShowName;
                string epname = pe.Name;
                if (urlEncode)
                {
                    showname = Uri.EscapeDataString(showname);
                    epname = Uri.EscapeDataString(epname);
                }

                name = name.ReplaceInsensitive("{ShowName}", showname);
                name = name.ReplaceInsensitive("{ShowNameLower}", pe.Show.ShowName.ToLower().Replace(' ','-').RemoveCharactersFrom("()[]{}&$:"));
                name = name.ReplaceInsensitive("{ShowNameInitial}", showname.Initial().ToLower());
                name = name.ReplaceInsensitive("{Season}", pe.AppropriateSeasonNumber.ToString());
                name = name.ReplaceInsensitive("{Season:2}", pe.AppropriateSeasonNumber.ToString("00"));
                name = name.ReplaceInsensitive("{SeasonNumber}", pe.AppropriateSeasonIndex.ToString());
                name = name.ReplaceInsensitive("{SeasonNumber:2}", pe.AppropriateSeasonIndex.ToString("00"));

                string episodeFormat = pe.AppropriateProcessedSeason.Episodes.Count >= 100 ? "000" : "00";
                name = name.ReplaceInsensitive("{Episode}", pe.AppropriateEpNum.ToString(episodeFormat));
                name = name.ReplaceInsensitive("{Episode2}", pe.EpNum2.ToString(episodeFormat));

                name = name.ReplaceInsensitive("{EpisodeName}", epname);
                name = name.ReplaceInsensitive("{Number}", pe.OverallNumber.ToString());
                name = name.ReplaceInsensitive("{Number:2}", pe.OverallNumber.ToString("00"));
                name = name.ReplaceInsensitive("{Number:3}", pe.OverallNumber.ToString("000"));
                name = name.ReplaceInsensitive("{Year}", pe.TheSeries.MinYear.ToString());
                name = name.ReplaceInsensitive("{SeasonYear}", pe.AppropriateProcessedSeason.MinYear().ToString());
                name = name.ReplaceInsensitive("{Imdb}", pe.ImdbCode);
                name = name.ReplaceInsensitive("{ShowImdb}", pe.Show?.TheSeries()?.Imdb ?? string.Empty);

                name = ReplaceDates(urlEncode, name, pe.GetAirDateDt(false));
                name = Regex.Replace(name, "{AllEpisodes}", AllEpsText(pe), RegexOptions.IgnoreCase);

                if (pe.EpNum2 == pe.AppropriateEpNum)
                {
                    name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts
                }
                else
                {
                    name = Regex.Replace(name, "([^\\\\])\\[(.*?[^\\\\])\\]", "$1$2"); // remove just the brackets
                }

                name = name.Replace("\\[", "[");
                name = name.Replace("\\]", "]");

                return name.Trim();
            }
            catch (ArgumentNullException)
            {
                Logger.Error($"Asked to update {styleString} with information from {pe.Show.ShowName}, {pe.SeasonNumberAsText}, {pe.EpNumsAsString()}");
            }

            return string.Empty;
        }

        [NotNull]
        private static string AllEpsText([NotNull] ProcessedEpisode pe)
        {
            string allEps = string.Empty;
            for (int i = pe.AppropriateEpNum; i <= pe.EpNum2; i++)
            {
                allEps += "E" + i.ToString("00");
            }

            return allEps;
        }
    }
}
