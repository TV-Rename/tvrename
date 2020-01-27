// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using JetBrains.Annotations;
using NLog;

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

        [NotNull]
        public static string PrettyPrint(this TVSettings.ScanType st)
        {
            switch (st)
            {
                case TVSettings.ScanType.Quick:
                    return "Quick";
                case TVSettings.ScanType.Full:
                    return "Full";
                case TVSettings.ScanType.Recent:
                    return "Recent";
                case TVSettings.ScanType.SingleShow:
                    return "Single";
                default:
                    throw new ArgumentOutOfRangeException(nameof(st), st, null);
            }
        }

        public static void Swap<T>(
            this IList<T> list,
            int firstIndex,
            int secondIndex
        )
        {
            Contract.Requires(list != null);
            Contract.Requires(firstIndex >= 0 && firstIndex < list.Count);
            Contract.Requires(secondIndex >= 0 && secondIndex < list.Count);
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

        public static bool SysOpen([CanBeNull] string what) => SysOpen(what, null);

        public static bool SysOpen([CanBeNull] string what, [CanBeNull] string arguments)
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

        public static bool Contains([NotNull] string source, [NotNull] string toCheck, StringComparison comp) => source.IndexOf(toCheck, comp) >= 0;
        
        [NotNull]
        public static string TranslateColorToHtml(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        
        [NotNull]
        public static string SimplifyName(string n)
        {
            n = n.ToLower();
            n = n.Replace("'", "");
            n = n.Replace("&", "and");
            n = n.Replace("!", "");
            n = Regex.Replace(n, "[_\\W]+", " ");
            return n.Trim();
        }

        [NotNull]
        public static string CompareName( this string n)
        {
            //TODO consider whether merge with above
            n = RemoveDiacritics(n);
            n = Regex.Replace(n, "[^\\w ]", "");
            return SimplifyName(n);
        }

        [NotNull]
        public static string RemoveDot([NotNull] this string s) => s.Replace(".", " ");

        public static string GetCommonStartString([NotNull] List<string> testValues)
        {
            string root = string.Empty;
            bool first = true;
            foreach (string test in testValues)
            {
                if (first)
                {
                    root = test;
                    first = false;
                }
                else
                {
                    root = GetCommonStartString(root, test);
                }
            }
            return root;
        }

        [NotNull]
        public static string TrimEnd([NotNull] this string root, [NotNull] string ending)
        {
            if (!root.EndsWith(ending,StringComparison.OrdinalIgnoreCase))
            {
                return root;
            }

            return root.Substring(0, root.Length - ending.Length);
        }

        [NotNull]
        public static string RemoveAfter([NotNull] this string root, [NotNull] string ending)
        {
            if (root.IndexOf(ending, StringComparison.OrdinalIgnoreCase) !=-1)
            {
                return   root.Substring(0, root.IndexOf(ending,StringComparison.OrdinalIgnoreCase));
            }

            return root;
        }

        public static string TrimEnd(this string root, [NotNull] string[] endings)
        {
            string trimmedString = root;
            foreach (string ending in endings)
            {
                trimmedString = trimmedString.TrimEnd(ending);
            }
            return trimmedString;
        }

        [NotNull]
        public static string GetCommonStartString([NotNull] string first, [NotNull] string second)
        {
            StringBuilder builder = new StringBuilder();
            
            int minLength = Math.Min(first.Length, second.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (first[i].Equals(second[i]))
                {
                    builder.Append(first[i]);
                }
                else
                {
                    break;
                }
            }
            return builder.ToString();
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

        public static bool ContainsOneOf([NotNull] this string source, [NotNull] IEnumerable<string> terms)
        {
            return terms.Any(source.Contains);
        }

        public static DateTime GetMinWindowsTime(DateTime dateTime)
        {
            //Any series before 1980 will get 1980 as the timestamp
            return dateTime.CompareTo(WindowsStartDateTime) < 0 ? WindowsStartDateTime : dateTime;
        }

        public static int ToInt(this string text, int def)
        {
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
