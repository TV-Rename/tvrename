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
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace TVRename
{
    public static class StringExtensions
    {
        [NotNull]
        public static string ItemItems(this int n)
        {
            return n == 1 ? "Item" : "Items";
        }

        public static bool IsWebLink([NotNull] this string s)
        {
            if(s.StartsWith("http://", StringComparison.Ordinal))
            {
                return true;
            }

            if (s.StartsWith("https://", StringComparison.Ordinal))
            {
                return true;
            }

            if (s.StartsWith("ftp://", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
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

        [NotNull]
        public static string ReplaceInsensitive([NotNull] this string source, [NotNull] string search, [NotNull] string replacement)
        {
            return Regex.Replace(
                source,
                Regex.Escape(search),
                string.IsNullOrEmpty(replacement ) ? string.Empty : replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase);
        }

        public static bool ContainsAnyCharctersFrom(this string source, [NotNull] char[] possibleChars)
        {
            return possibleChars.Any(testChar => source.Contains(testChar.ToString()));
        }
        public static bool ContainsAnyCharctersFrom(this string source, [NotNull] string possibleChars)
        {
            return ContainsAnyCharctersFrom(source,possibleChars.ToCharArray());
        }
    }
}
