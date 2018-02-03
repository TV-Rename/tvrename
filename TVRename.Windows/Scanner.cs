using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;
using TVRename.Core.Models.Cache;
using TVRename.Core.Utility;
using TVRename.Windows.Configuration;
using TVRename.Windows.Models;
using static TVRename.Windows.Utilities.Helpers;
using Helpers = TVRename.Core.Utility.Helpers;
using Show = TVRename.Core.Models.Show;

namespace TVRename.Windows
{
    public static class Scanner
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task<List<TreeNode>> BuildTree()
        {
            List<TreeNode> results = new List<TreeNode>();

            foreach (Show show in Settings.Instance.Shows.OrderBy(s => s.Metadata.Name))
            {
                TreeNode node = new TreeNode
                {
                    Text = show.Metadata.Name,
                    Name = show.TVDBId.ToString()
                };

                foreach (KeyValuePair<int, Season> season in show.Metadata.Seasons.OrderBy(s => s.Value.Number))
                {
                    node.Nodes.Add(new TreeNode
                    {
                        Text = season.Value.ToString(),
                        Name = season.Key.ToString()
                    });
                }

                results.Add(node);
            }

            return results;
        }

        public static async Task<List<ListViewItem>> BuildCalendar(ListViewGroupCollection groups)
        {
            List<ListViewItem> results = new List<ListViewItem>();

            foreach (Show show in Settings.Instance.Shows)
            {
                //if (!show.NextAirs.HasValue) continue;

                foreach (Season season in show.Metadata.Seasons.Values)
                {
                    if (show.IgnoredSeasons.Contains(season.Number)) continue;

                    foreach (Episode episode in season.Episodes.Values)
                    {
                        if (!episode.FirstAired.HasValue || episode.FirstAired?.CompareTo(DateTime.MaxValue) == 0) continue;

                        TimeSpan delta = episode.FirstAired.Value.Subtract(DateTime.UtcNow);

                        if (delta < -TimeSpan.FromDays(Settings.Instance.RecentDays)) continue;

                        ListViewItem lvi = new ListViewItem
                        {
                            Text = show.Metadata.Name,
                            SubItems =
                            {
                                season.Number.ToString(),
                                episode.Number.ToString(),
                                episode.FirstAired.Value.ToShortDateString(),
                                episode.FirstAired.Value.ToString("t"),
                                episode.FirstAired.Value.ToString("ddd"),
                                episode.FirstAired.Value < DateTime.UtcNow ? "Aired" : $"{delta.Days}d {delta.Hours}h",
                                episode.Name
                            },
                            Tag = episode
                        };

                        if (delta.TotalHours < 0)
                        {
                            lvi.Group = groups["recent"];
                        }
                        else if (delta < TimeSpan.FromDays(Settings.Instance.RecentDays))
                        {
                            lvi.Group = groups["soon"];
                        }
                        //else if (episode.NextToAir)
                        //{
                        //    lvi.Group = groups["future"];
                        //}
                        else
                        {
                            lvi.Group = groups["later"];
                        }

                        if (episode.FirstAired?.CompareTo(DateTime.Now) < 0) // has aired
                        {
                            ProcessedEpisode processedEpisode = await ProcessEpisode(show, season, episode);

                            if (processedEpisode.Exists)
                            {
                                lvi.ImageIndex = 1;
                            }
                            else if (show.CheckMissing)
                            {
                                lvi.ImageIndex = 0;
                            }
                        }

                        results.Add(lvi);
                    }
                }
            }

            return results;
        }

        public static async Task<ProcessedShow> ProcessShow(Show show)
        {
            return new ProcessedShow(show);
        }

        public static async Task<ProcessedSeason> ProcessSeason(Show show, Season season)
        {
            return await ProcessSeason(await ProcessShow(show), season);
        }

        public static async Task<ProcessedSeason> ProcessSeason(ProcessedShow processedShow, Season season)
        {
            ProcessedSeason processedSeason = new ProcessedSeason(season);

            // Resolve season template
            processedSeason.Directory = EscapeTemplatePath((processedSeason.Number == 0 ? Settings.Instance.SpecialsTemplate : Settings.Instance.SeasonTemplate).Template(season));

            // Set season location
            processedSeason.Location = Path.Combine(processedShow.Location, processedSeason.Directory);

            return processedSeason;
        }

        public static async Task<ProcessedEpisode> ProcessEpisode(Show show, Season season, Episode episode)
        {
            ProcessedShow processedShow = await ProcessShow(show);
            ProcessedSeason processedSeason = await ProcessSeason(processedShow, season);

            return await ProcessEpisode(processedShow, processedSeason, episode);
        }

        public static async Task<ProcessedEpisode> ProcessEpisode(ProcessedShow processedShow, Season season, Episode episode)
        {
            ProcessedSeason processedSeason = await ProcessSeason(processedShow, season);

            return await ProcessEpisode(processedShow, processedSeason, episode);
        }

        public static async Task<ProcessedEpisode> ProcessEpisode(ProcessedShow processedShow, ProcessedSeason processedSeason, Episode episode)
        {
            ProcessedEpisode processedEpisode = new ProcessedEpisode(episode);

            // Resolve episode filename template
            string episodeFile = Settings.Instance.EpisodeTemplate.Template(new Dictionary<string, object>
            {
                { "show", processedShow },
                { "season", processedSeason },
                { "episode", processedEpisode }
            });

            // Set variables for templates
            processedEpisode.Location = processedSeason.Location;

            // Remove illigal path characters with user replacements
            processedEpisode.Filename = Path.GetFileNameWithoutExtension(EscapeTemplatePath(episodeFile));

            string path = Path.Combine(processedSeason.Location, processedEpisode.Filename);

            // Search for the file with any video extention
            foreach (string ext in Settings.Instance.VideoFileExtensions)
            {
                if (!File.Exists($"{path}.{ext}")) continue;

                // Add extension
                processedEpisode.Extension = ext;

                break;
            }

            // Check file exists
            processedEpisode.Exists = File.Exists(processedEpisode.FullPath);

            return processedEpisode;
        }

        public static async Task<List<ListViewItem>> Scan(ListView listViewScan, CancellationToken ct)
        {
            List<ListViewItem> results = new List<ListViewItem>();

            // Loop shows
            foreach (Show show in Settings.Instance.Shows)
            {
                ProcessedShow processedShow = await ProcessShow(show);

                // Process show identifiers
                results.AddRange(Settings.Instance.Identifiers.Select(i => i.ProcessShow(processedShow))
                    .Where(s => s != null)
                    .Select(action => new ListViewItem(new[] { processedShow.Name, string.Empty, string.Empty, processedShow.LastUpdated.ToShortDateString(), Path.GetDirectoryName(action.Produces), Path.GetFileName(action.Produces), string.Empty })
                    {
                        Tag = action,
                        Checked = true,
                        Group = listViewScan.Groups[action.Type.ToLowerInvariant()]
                    }));

                // Loop show seasons, skipping user ignored
                foreach (Season season in show.Metadata.Seasons.Values.Where(s => !show.IgnoredSeasons.Contains(s.Number)))
                {
                    ProcessedSeason processedSeason = await ProcessSeason(processedShow, season);

                    // Process season identifiers
                    results.AddRange(Settings.Instance.Identifiers.Select(i => i.ProcessSeason(processedShow, processedSeason))
                        .Where(s => s != null)
                        .Select(action => new ListViewItem(new[] { processedShow.Name, processedSeason.Number == 0 ? "Specials" : processedSeason.Number.ToString(), string.Empty, processedShow.LastUpdated.ToShortDateString(), Path.GetDirectoryName(action.Produces), Path.GetFileName(action.Produces), string.Empty })
                        {
                            Tag = action,
                            Checked = true,
                            Group = listViewScan.Groups[action.Type.ToLowerInvariant()]
                        }));

                    // Loop season episodes
                    foreach (Episode episode in season.Episodes.Values)
                    {
                        ProcessedEpisode processedEpisode = await ProcessEpisode(processedShow, processedSeason, episode);

                        if (processedEpisode.Exists)
                        {
                            // Process episode identifiers
                            results.AddRange(Settings.Instance.Identifiers.Select(i => i.ProcessEpisode(processedShow, processedSeason, processedEpisode))
                                .Where(s => s != null)
                                .Select(action => new ListViewItem(new[] { processedShow.Name, processedSeason.Number == 0 ? "Specials" : processedSeason.Number.ToString(), processedEpisode.Number.ToString(), processedEpisode.LastUpdated.ToShortDateString(), Path.GetDirectoryName(action.Produces), Path.GetFileName(action.Produces), string.Empty })
                                {
                                    Tag = action,
                                    Checked = true,
                                    Group = listViewScan.Groups[action.Type.ToLowerInvariant()]
                                }));
                        }
                        else
                        {
                            // Add missing episode item
                            results.Add(new ListViewItem(new[]
                            {
                                processedShow.Name,
                                processedSeason.Number == 0 ? Settings.Instance.SpecialsTemplate.Template(season) : processedSeason.Number.ToString(),
                                processedEpisode.Number.ToString(),
                                processedEpisode.FirstAired?.ToShortDateString(),
                                Path.GetDirectoryName(processedEpisode.FullPath),
                                Path.GetFileName(processedEpisode.FullPath),
                                string.Empty
                            })
                            {
                                Tag = new MissingAction(processedShow, processedSeason, processedEpisode), // TODO
                                Checked = false,
                                Group = listViewScan.Groups["missing"]
                            });
                        }
                    }

                    // Get all video files in the season directory
                    foreach (string file in await Helpers.GetFilesAsync(processedSeason.Location, Settings.Instance.VideoFileExtensions.Select(e => $"*.{e}").ToArray()))
                    {
                        // Try to match the file against the show
                        if (!FindEpisode(processedSeason.Location, Path.GetFileName(file), out int matchedSeasonNumber, out int matchedEpisodeNumber, processedShow)) continue;

                        Logger.Debug($"{file}: {matchedSeasonNumber} {matchedEpisodeNumber}");

                        Season matchedSeason = show.Metadata.Seasons[matchedSeasonNumber];
                        Episode matchedEpisode = matchedSeason.Episodes.Values.First(e => e.Number == matchedEpisodeNumber);

                        // Resolve episode filename template
                        string episodeFile = Settings.Instance.EpisodeTemplate.Template(new Dictionary<string, object>
                        {
                            { "show", processedShow },
                            { "season", matchedSeason },
                            { "episode", matchedEpisode }
                        });

                        episodeFile = EscapeTemplatePath(episodeFile);

                        string episodePath = Path.Combine(processedSeason.Location, episodeFile);

                        foreach (string ext in Settings.Instance.VideoFileExtensions)
                        {
                            if (!File.Exists($"{episodePath}.{ext}")) continue;

                            episodeFile += $".{ext}";
                            episodePath = Path.Combine(processedSeason.Location, episodeFile);

                            break;
                        }

                        FileInfo correctFile = new FileInfo(episodePath);

                        if (file != correctFile.FullName)
                        {
                            // TODO: MOVE
                        }
                    }
                }
            }

            // Look for missing episodes
            foreach (ListViewItem lvi in results.Where(l => l.Tag is MissingAction))
            {
                MissingAction action = (MissingAction)lvi.Tag;

                // Loop all files in the search directories
                foreach (string file in Settings.Instance.SearchDirectories.Where(Directory.Exists).SelectMany(Directory.GetFiles)) // TODO: Async
                {
                    string directory = Path.GetDirectoryName(file);
                    string fileName = Path.GetFileName(file);
                    string fileNameSimplified = Helpers.SimplifyName(fileName);

                    bool matched = action.Show.AllNames.Any(n => Regex.Match(fileNameSimplified, $@"\b{Helpers.SimplifyName(n)}\b", RegexOptions.IgnoreCase).Success);

                    if (!matched) continue;

                    if (!FindEpisode(directory, fileName, out int seasF, out int epF, action.Show) || seasF != action.Season.Number || epF != action.Episode.Number) continue;

                    FileAction fileAction = new FileAction(new FileInfo(file), new FileInfo(action.Produces + Path.GetExtension(file)), FileAction.FileOperation.Copy);

                    lvi.SubItems[4].Text = file;
                    lvi.SubItems[5].Text = action.Produces + Path.GetExtension(file);
                    lvi.Tag = fileAction;
                    lvi.Checked = true;
                    lvi.Group = listViewScan.Groups[fileAction.Type.ToLowerInvariant()];

                    break;
                }
            }

            return results;
        }

        // TODO: Review
        private static bool FindEpisode(string directory, string filename, out int season, out int episode, ProcessedShow show)
        {
            season = -1;
            episode = -1;

            filename = SimplifyFilename(filename, show != null ? show.Name : string.Empty);

            string fullPath = directory + Path.DirectorySeparatorChar + filename; // construct full path with sanitised filename

            int leftMostPos = filename.Length;

            filename = filename.ToLower() + " ";
            fullPath = fullPath.ToLower() + " ";

            foreach (FilenameProcessor processor in Settings.Instance.FilenameProcessors)
            {
                if (!processor.Enabled) continue;

                try
                {
                    Match match = Regex.Match(processor.UseFullPath ? fullPath : filename, processor.Pattern, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        int adj = processor.UseFullPath ? fullPath.Length - filename.Length : 0;

                        int p = Math.Min(match.Groups["s"].Index, match.Groups["e"].Index) - adj;

                        if (p >= leftMostPos) continue;

                        if (!int.TryParse(match.Groups["s"].ToString(), out season)) season = -1;
                        if (!int.TryParse(match.Groups["e"].ToString(), out episode)) episode = -1;

                        leftMostPos = p;
                    }
                }
                catch (FormatException)
                { }
                catch (ArgumentException)
                { }
            }

            return season != -1 || episode != -1;
        }
        
        // TODO: Review
        // Look at showName and try to remove the first occurance of it from filename
        // This is very helpful if the showname has a >= 4 digit number in it, as that
        // would trigger the 1302 -> 13,02 matcher
        // Also, shows like "24" can cause confusion
        private static string SimplifyFilename(string filename, string showName)
        {
            filename = filename.Replace(".", " "); // turn dots into spaces

            if (string.IsNullOrEmpty(showName)) return filename;

            bool nameIsNumber = Regex.Match(showName, "^[0-9]+$").Success;

            if (filename.IndexOf(showName, StringComparison.InvariantCulture) == 0)
            {
                return filename.Remove(0, showName.Length);
            }

            // e.g. "24", or easy exact match of show name at start of filename
            if (nameIsNumber) return filename;

            foreach (Match m in Regex.Matches(showName, "(?:^|[^a-z]|\\b)([0-9]{3,})")) // find >= 3 digit numbers in show name
            {
                if (m.Groups.Count <= 1) continue;

                filename = Regex.Replace(filename, "(^|\\W)" + m.Groups[1].Value + "\\b", ""); // remove any occurances of that number in the filename
            }

            return filename;
        }
    }
}
