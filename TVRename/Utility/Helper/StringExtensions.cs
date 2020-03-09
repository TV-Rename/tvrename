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
using System.Text.RegularExpressions;
using JetBrains.Annotations;

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
            return  s.StartsWith("http://", StringComparison.Ordinal)
                ||  s.StartsWith("https://", StringComparison.Ordinal);
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

        public static bool HasValue([CanBeNull] this string s) => !string.IsNullOrWhiteSpace(s);

        [NotNull]
        public static string ReplaceInsensitive([NotNull] this string source, [NotNull] string search, [NotNull] string replacement)
        {
            return Regex.Replace(
                source,
                Regex.Escape(search),
                string.IsNullOrEmpty(replacement ) ? string.Empty : replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase);
        }

        public static bool ContainsAnyCharctersFrom(this string source, [NotNull] IEnumerable<char> possibleChars)
        {
            return possibleChars.Any(testChar => source.Contains(testChar.ToString()));
        }
        public static bool ContainsAnyCharctersFrom(this string source, [NotNull] string possibleChars)
        {
            return ContainsAnyCharctersFrom(source,possibleChars.ToCharArray());
        }

        public static bool IsNullOrWhitespace([CanBeNull] this string text) => string.IsNullOrWhiteSpace(text);

        [NotNull]
        public static string RemoveLastCharacter([NotNull] this string instr)
        {
            return instr.Substring(0, instr.Length - 1);
        }
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
            if (!root.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
            {
                return root;
            }

            return root.RemoveLast(ending.Length);
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

        public static bool ContainsOneOf([NotNull] this string source, [NotNull] IEnumerable<string> terms)
        {
            return terms.Any(source.Contains);
        }

        [NotNull]
        public static string ToCsv([NotNull] this IEnumerable<string> values)
        {
            return string.Join(", ", values);
        }
    }
}
