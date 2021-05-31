//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using JetBrains.Annotations;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

// Helpful functions and classes

namespace TVRename
{
    public static class Helpers
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets a value indicating whether application is running under Mono.
        /// </summary>
        /// <value>
        ///   <c>true</c> if application is running under Mono; otherwise, <c>false</c>.
        /// </value>
        public static bool OnMono => Type.GetType("Mono.Runtime") != null;

        public static bool In<T>([CanBeNull] this T item, [NotNull] params T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.Contains(item);
        }

        [NotNull]
        public static string PrettyPrint(this TVSettings.ScanType st)
        {
            return st switch
            {
                TVSettings.ScanType.Quick => "Quick",
                TVSettings.ScanType.Full => "Full",
                TVSettings.ScanType.Recent => "Recent",
                TVSettings.ScanType.SingleShow => "Single",
                _ => throw new ArgumentOutOfRangeException(nameof(st), st, null)
            };
        }

        public static string PrettyPrint(this MediaConfiguration.MediaType st)
        {
            return st switch
            {
                MediaConfiguration.MediaType.tv => "TV",
                MediaConfiguration.MediaType.movie => "Movies",
                MediaConfiguration.MediaType.both => "TV and Movies",
                _ => throw new ArgumentOutOfRangeException(nameof(st), st, null)
            };
        }

        public static string PrettyPrint(this ProcessedSeason.SeasonType st)
        {
            return st switch
            {
                ProcessedSeason.SeasonType.dvd => "dvd",
                ProcessedSeason.SeasonType.aired => "official",
                ProcessedSeason.SeasonType.absolute => "absolute",
                _ => throw new ArgumentOutOfRangeException(nameof(st), st, null)
            };
        }

        public static string PrettyPrint(this TVDoc.ProviderType type)
        {
            return type switch
            {
                TVDoc.ProviderType.libraryDefault => "Library Default",
                TVDoc.ProviderType.TVmaze => "TV Maze",
                TVDoc.ProviderType.TheTVDB => "The TVDB",
                TVDoc.ProviderType.TMDB => "TMDB",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static void Swap<T>(
            this IList<T> list,
            int firstIndex,
            int secondIndex
        )
        {
            if (firstIndex == secondIndex)
            {
                return;
            }
            T temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static void SafeInvoke([NotNull] this Control uiElement, System.Action updater, bool forceSynchronous)
        {
            if (uiElement is null)
            {
                throw new ArgumentNullException(nameof(uiElement));
            }

            if (uiElement.InvokeRequired)
            {
                if (forceSynchronous)
                {
                    uiElement.Invoke((System.Action)delegate { SafeInvoke(uiElement, updater, true); });
                }
                else
                {
                    uiElement.BeginInvoke((System.Action)delegate { SafeInvoke(uiElement, updater, false); });
                }
            }
            else
            {
                if (uiElement.IsDisposed)
                {
                    throw new ObjectDisposedException("Control is already disposed.");
                }

                updater();
            }
        }

        /// <summary>
        /// Gets the application display version from the current assemblies <see cref="AssemblyInformationalVersionAttribute"/>.
        /// </summary>
        /// <value>
        /// The application display version.
        /// </value>
        [NotNull]
        public static string DisplayVersion
        {
            get
            {
                string v = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().First().InformationalVersion;
#if DEBUG
                v += DebugText;
#endif
                return v;
            }
        }

        [NotNull]
        public static string DebugText => " ** Debug Build **";

        [NotNull]
        public static string Pad(int i)
        {
            if (i.ToString().Length > 1)
            {
                return i.ToString();
            }

            return "0" + i;
        }

        [NotNull]
        public static string PrettyPrint(this DateTime? dt)
        {
            if (dt != null && dt.Value.CompareTo(DateTime.MaxValue) != 0)
            {
                return dt.Value.ToShortDateString();
            }

            return string.Empty;
        }

        public static long ToUnixTime(this DateTime date) => Convert.ToInt64((date.ToUniversalTime() - Epoch).TotalSeconds);

        public static DateTime FromUnixTime(double unixTime) => Epoch.AddSeconds(unixTime);

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime WindowsStartDateTime = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static bool OpenFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                return SysOpen(folder);
            }
            return false;
        }

        public static void OpenFolderSelectFile(string filename)
        {
            string args = $"/e, /select, \"{filename}\"";

            ProcessStartInfo info = new ProcessStartInfo { FileName = "explorer", Arguments = args };
            Process.Start(info);
        }

        public static bool OpenUrl(string url) => SysOpen(url);

        public static void OpenFile(string filename) => SysOpen(filename);

        private static bool SysOpen(string? what) => SysOpen(what, null);

        private static bool SysOpen(string? what, string? arguments)
        {
            if (string.IsNullOrWhiteSpace(what))
            {
                return false;
            }

            try
            {
                Process.Start(what, arguments);
                return true;
            }
            catch (Win32Exception e)
            {
                Logger.Warn(e, $"Could not open {what}");
                return false;
            }
            catch (FileNotFoundException e)
            {
                Logger.Warn(e, $"Could not open {what}");
                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Could not open {what}");
                return false;
            }
        }

        public static Color WarningColor() => Color.FromArgb(255, 210, 210);

        public static T LongestShowName<T>(this IEnumerable<T> media) where T : MediaConfiguration
        {
            IEnumerable<T> mediaConfigurations = media as T[] ?? media.ToArray();
            int longestName = mediaConfigurations.Select(configuration => configuration.ShowName.Length).Max();
            return mediaConfigurations.First(config => config.ShowName.Length == longestName);
        }

        public static bool Contains([NotNull] string source, [NotNull] string toCheck, StringComparison comp) => source.IndexOf(toCheck, comp) >= 0;

        [NotNull]
        public static string TranslateColorToHtml(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        [NotNull]
        public static string CompareName(this string n)
        {
            n = n.ToLower();
            n = RemoveDiacritics(n);
            n = n.Replace(".", " ");
            n = n.Replace("'", "");
            n = n.Replace("&", "and");
            n = n.Replace("!", "");
            n = n.Replace("*", "");
            n = Regex.Replace(n, "[_\\W]+", " ");
            n = Regex.Replace(n, "[^\\w ]", " ");
            return n.Trim();
        }

        [NotNull]
        public static string RemoveDiacritics([NotNull] this string stIn)
        {
            // From http://blogs.msdn.com/b/michkap/archive/2007/05/14/2629747.aspx
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char t in stFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static DateTime GetMinWindowsTime(DateTime dateTime)
        {
            //Any cachedSeries before 1980 will get 1980 as the timestamp
            return dateTime.CompareTo(WindowsStartDateTime) < 0 ? WindowsStartDateTime : dateTime;
        }

        public static int ToInt(this string? text, int def)
        {
            if (text is null)
            {
                return def;
            }
            try
            {
                return int.Parse(text);
            }
            catch
            {
                return def;
            }
        }

        public static float ToPercent(this string text, float def)
        {
            float value;
            try
            {
                value = float.Parse(text);
            }
            catch
            {
                return def;
            }

            if (value < 1)
            {
                return 1;
            }

            if (value > 100)
            {
                return 100;
            }

            return value;
        }

        public static int ToInt(this string text, int min, int def, int max)
        {
            int value;
            try
            {
                value = int.Parse(text);
            }
            catch
            {
                return def;
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}
