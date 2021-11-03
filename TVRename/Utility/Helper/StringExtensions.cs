//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TVRename
{
    public static class StringExtensions
    {
        [NotNull]
        public static string ItemItems(this int n) => n == 1 ? "Item" : "Items";

        public static bool IsWebLink([NotNull] this string s)
        {
            return s.IsHttpLink() || s.IsFtpLink();
        }

        public static bool IsHttpLink([NotNull] this string s)
        {
            return s.StartsWith("http://", StringComparison.Ordinal)
                || s.StartsWith("https://", StringComparison.Ordinal);
        }

        public static bool IsFtpLink([NotNull] this string s)
        {
            return s.StartsWith("ftp://", StringComparison.Ordinal);
        }

        public static bool IsFileLink([NotNull] this string s)
        {
            return s.StartsWith("file://", StringComparison.Ordinal);
        }

        public static string RemoveCharactersFrom(this string source, [NotNull] IEnumerable<char> badChars)
        {
            string returnValue = source;
            foreach (char x in badChars)
            {
                returnValue = returnValue.Replace(x.ToString(), string.Empty);
            }

            return returnValue;
        }

        public static bool Contains([NotNull] this string source, [NotNull] string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool IsNumeric(this string text) => int.TryParse(text, out int _);

        public static bool HasValue(this string? s) => !string.IsNullOrWhiteSpace(s);

        [NotNull]
        public static string ReplaceInsensitive([NotNull] this string source, [NotNull] string search, string? replacement)
        {
            if (!source.HasValue())
            {
                return string.Empty;
            }

            return Regex.Replace(
                source,
                Regex.Escape(search),
                string.IsNullOrEmpty(replacement) ? string.Empty : replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase);
        }

        [NotNull]
        public static string ReplaceInsensitiveLazy(this string? source, [NotNull] string search, Lazy<string?> replacement, StringComparison comparison)
        {
            if (source == null)
            {
                return string.Empty;
            }

            if (source == "")
            {
                return string.Empty;
            }

            if (search == string.Empty)
            {
                return source;
            }

            int index = source.IndexOf(search, comparison);

            if (index == -1)
            {
                //Does not contain the search string
                return source;
            }

            //We are going to need to do the replacement, so take the time to do the action
            string replacementValue = replacement.Value ?? string.Empty;

            StringBuilder sb = new();
            int previousIndex = 0;

            while (index != -1)
            {
                sb.Append(source.Substring(previousIndex, index - previousIndex));
                sb.Append(replacementValue);

                previousIndex = index + search.Length;
                index = source.IndexOf(search, previousIndex, comparison);
            }
            sb.Append(source.Substring(previousIndex));

            return sb.ToString();
        }

        public static bool ContainsAnyCharactersFrom(this string source, [NotNull] IEnumerable<char> possibleChars)
        {
            return possibleChars.Any(testChar => source.Contains(testChar.ToString()));
        }

        public static bool ContainsAnyCharactersFrom(this string source, [NotNull] string possibleChars)
        {
            return ContainsAnyCharactersFrom(source, possibleChars.ToCharArray());
        }

        [NotNull]
        public static string UppercaseFirst([CanBeNull] this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public static bool IsNullOrWhitespace(this string? text) => string.IsNullOrWhiteSpace(text);

        [NotNull]
        public static string RemoveLastCharacter([NotNull] this string instr)
        {
            return instr.Substring(0, instr.Length - 1);
        }

        [NotNull]
        public static string First(this string s, int charsToDisplay)
        {
            if (s.HasValue())
            {
                return s.Length <= charsToDisplay ? s : new string(s.Take(charsToDisplay).ToArray());
            }

            return string.Empty;
        }

        [NotNull]
        public static string Initial([NotNull] this string str) => str.HasValue() ? str.Substring(0, 1) : string.Empty;

        [NotNull]
        public static string RemoveLast([NotNull] this string instr, int number)
        {
            return instr.Substring(0, instr.Length - number);
        }

        [NotNull]
        public static string RemoveFirstCharacter([NotNull] this string instr)
        {
            return instr.Substring(1);
        }

        [NotNull]
        public static string RemoveFirst([NotNull] this string instr, int number)
        {
            return instr.Substring(number);
        }

        public static string GetCommonStartString([NotNull] IEnumerable<string> testValues)
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
            if (!root.HasValue())
            {
                return root;
            }
            if (!root.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
            {
                return root;
            }

            return root.RemoveLast(ending.Length);
        }

        [NotNull]
        public static string TrimStartString([NotNull] this string root, [NotNull] string startString)
        {
            if (!root.HasValue())
            {
                return root;
            }
            if (!root.StartsWith(startString, StringComparison.OrdinalIgnoreCase))
            {
                return root;
            }

            return root.RemoveFirst(startString.Length);
        }

        [NotNull]
        public static string RemoveAfter([NotNull] this string root, [NotNull] string ending)
        {
            if (root.IndexOf(ending, StringComparison.OrdinalIgnoreCase) != -1)
            {
                return root.Substring(0, root.IndexOf(ending, StringComparison.OrdinalIgnoreCase));
            }

            return root;
        }

        public static string TrimEnd(this string root, [NotNull] IEnumerable<string> endings)
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
            StringBuilder builder = new();

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

        public static bool ContainsOneOf([NotNull] this string source, [NotNull] IEnumerable<string> terms)
        {
            return terms.Any(source.Contains);
        }

        [NotNull]
        public static string ToCsv([NotNull] this IEnumerable<string> values)
        {
            return string.Join(", ", values);
        }

        [NotNull]
        public static string ToCsv([NotNull] this ICollection<int> values)
        {
            return string.Join(",", values);
        }

        [NotNull]
        public static string ToPsv([NotNull] this IEnumerable<string> values)
        {
            return string.Join("|", values);
        }

        [NotNull]
        public static IEnumerable<string> FromPsv(this string aggregate) => aggregate.FromSepValues('|');
        [NotNull]
        public static IEnumerable<string> FromCsv(this string aggregate) => aggregate.FromSepValues(',');
        [NotNull]
        private static IEnumerable<string> FromSepValues([CanBeNull] this string aggregate, char delimiter)
        {
            return string.IsNullOrEmpty(aggregate)
                ? new string[] { }
                : aggregate.Split(delimiter)
                    .Where(s => s.HasValue())
                    .Select(s => s.Trim());
        }
       
    public static int? ToInt(this string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [NotNull]
        public static string Concat([NotNull] this IEnumerable<string> values)
        {
            return string.Join(string.Empty, values);
        }
    }
}
