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

// This builds the filenames to rename to, for any given episode (or multi-episode episode)

namespace TVRename
{
    public class CustomEpisodeName
    {
        public string StyleString;

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
            if ((n >= 0) && (n < 9))
                return PRESETS[n];

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

        public string NameFor(ProcessedEpisode pe) => NameFor(pe,string.Empty);

        public string NameFor(ProcessedEpisode pe, string extension)
        {
            string r = NameForNoExt(pe, StyleString);

            if (string.IsNullOrEmpty(extension))
            {
                return r;
            }

            if (!extension.StartsWith("."))
                r += ".";
            r += extension;
            return r;
        }

        public string GetTargetEpisodeName(ShowItem show, Episode ep, TimeZoneInfo tz, bool dvdOrder)
            => GetTargetEpisodeName(show, ep,  tz, dvdOrder, false);

        private string GetTargetEpisodeName(ShowItem show, Episode ep, TimeZoneInfo tz, bool dvdOrder, bool urlEncode)
        {
            //note this is for an Episode and not a ProcessedEpisode
            string name = StyleString;

            string epname = ep.Name;

            name = name.ReplaceInsensitive("{ShowName}", show.ShowName);
            if (dvdOrder)
            {
                name = name.ReplaceInsensitive("{Season}", ep.DvdSeasonNumber.ToString());
                name = name.ReplaceInsensitive("{Season:2}", ep.DvdSeasonNumber.ToString("00"));
                name = name.ReplaceInsensitive("{SeasonNumber}", ep.DvdSeasonIndex.ToString());
                name = name.ReplaceInsensitive("{SeasonNumber:2}", ep.DvdSeasonIndex.ToString("00"));
                name = name.ReplaceInsensitive("{Episode}", ep.DvdEpNum.ToString("00"));
                name = name.ReplaceInsensitive("{Episode2}", ep.DvdEpNum.ToString("00"));
                name = Regex.Replace(name, "{AllEpisodes}", ep.DvdEpNum.ToString("00"));
            }
            else
            {
                name = name.ReplaceInsensitive("{Season}", ep.AiredSeasonNumber.ToString());
                name = name.ReplaceInsensitive("{Season:2}", ep.AiredSeasonNumber.ToString("00"));
                name = name.ReplaceInsensitive("{SeasonNumber}", ep.AiredSeasonIndex.ToString());
                name = name.ReplaceInsensitive("{SeasonNumber:2}", ep.AiredSeasonIndex.ToString("00"));
                name = name.ReplaceInsensitive("{Episode}", ep.AiredEpNum.ToString("00"));
                name = name.ReplaceInsensitive("{Episode2}", ep.AiredEpNum.ToString("00"));
                name = Regex.Replace(name, "{AllEpisodes}", ep.AiredEpNum.ToString("00"));
            }
            name = name.ReplaceInsensitive("{EpisodeName}", epname);
            name = name.ReplaceInsensitive("{Number}", "");
            name = name.ReplaceInsensitive("{Number:2}", "");
            name = name.ReplaceInsensitive("{Number:3}", "");
            name = name.ReplaceInsensitive("{Year}", show.TheSeries().MinYear.ToString());
            name = name.ReplaceInsensitive("{SeasonYear}", show.GetSeason(dvdOrder ? ep.DvdSeasonNumber : ep.AiredSeasonNumber).MinYear().ToString());
            name = name.ReplaceInsensitive("{Imdb}", ep.ImdbCode);
            name = name.ReplaceInsensitive("{ShowImdb}", show.TheSeries().Imdb);

            name = ReplaceDates(urlEncode, name, ep.GetAirDateDt(tz));

            name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts

            name = name.Replace("\\[", "[");
            name = name.Replace("\\]", "]");

            return name.Trim();
        }

        private static string ReplaceDates(bool urlEncode, string name, DateTime? airdt)
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
                ymd = Uri.EscapeDataString(ymd);
            name = name.ReplaceInsensitive("{YMDDate}", ymd);

            return name;
        }

        public static string NameForNoExt(ProcessedEpisode pe, string styleString) => NameForNoExt(pe, styleString, false);

        public static string NameForNoExt(ProcessedEpisode pe, string styleString, bool urlEncode)
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
            name = name.ReplaceInsensitive("{Season}", pe.AppropriateSeasonNumber.ToString());
            name = name.ReplaceInsensitive("{Season:2}", pe.AppropriateSeasonNumber.ToString("00"));
            name = name.ReplaceInsensitive("{SeasonNumber}", pe.AppropriateSeasonIndex.ToString());
            name = name.ReplaceInsensitive("{SeasonNumber:2}", pe.AppropriateSeasonIndex.ToString("00"));
            if (pe.AppropriateSeason.Episodes.Count >= 100)
            {
                name = name.ReplaceInsensitive("{Episode}", pe.AppropriateEpNum.ToString("000"));
                name = name.ReplaceInsensitive("{Episode2}", pe.EpNum2.ToString("000"));
            }
            else
            {
                name = name.ReplaceInsensitive("{Episode}", pe.AppropriateEpNum.ToString("00"));
                name = name.ReplaceInsensitive("{Episode2}", pe.EpNum2.ToString("00"));
            }
            name = name.ReplaceInsensitive("{EpisodeName}", epname);
            name = name.ReplaceInsensitive("{Number}", pe.OverallNumber.ToString());
            name = name.ReplaceInsensitive("{Number:2}", pe.OverallNumber.ToString("00"));
            name = name.ReplaceInsensitive("{Number:3}", pe.OverallNumber.ToString("000"));
            name = name.ReplaceInsensitive("{Year}", pe.TheSeries.MinYear.ToString());
            name = name.ReplaceInsensitive("{SeasonYear}", pe.AppropriateSeason.MinYear().ToString());
            name = name.ReplaceInsensitive("{Imdb}", pe.ImdbCode);
            name = name.ReplaceInsensitive("{ShowImdb}", pe.Show.TheSeries().Imdb);

            name = ReplaceDates(urlEncode, name, pe.GetAirDateDt(false));

            string allEps = "";
            for (int i = pe.AppropriateEpNum; i <= pe.EpNum2; i++)
                allEps += "E" + i.ToString("00");
            name = Regex.Replace(name, "{AllEpisodes}", allEps, RegexOptions.IgnoreCase);

            if (pe.EpNum2 == pe.AppropriateEpNum)
                name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts
            else
                name = Regex.Replace(name, "([^\\\\])\\[(.*?[^\\\\])\\]", "$1$2"); // remove just the brackets

            name = name.Replace("\\[", "[");
            name = name.Replace("\\]", "]");

            return name.Trim();
        }
    }
}
