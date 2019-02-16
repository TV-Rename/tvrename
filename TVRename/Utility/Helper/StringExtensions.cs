using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace TVRename
{
    public static class StringExtensions
    {
        public static string ItemItems(this int n)
        {
            return n == 1 ? "Item" : "Items";
        }

        public static string RemoveCharactersFrom(this string source, IEnumerable<char> badChars)
        {
            string returnValue = source;
            foreach (char x in badChars)
            {
                returnValue = returnValue.Replace(x.ToString(), string.Empty);
            }

            return returnValue;
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string ReplaceInsensitive([NotNull] this string source, [NotNull] string search, [NotNull] string replacement)
        {
            return Regex.Replace(
                source,
                Regex.Escape(search),
                string.IsNullOrEmpty(replacement ) ? string.Empty : replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase);
        }

        public static bool ContainsAnyCharctersFrom(this string source, char[] possibleChars)
        {
            return possibleChars.Any(testChar => source.Contains(testChar.ToString()));
        }
        public static bool ContainsAnyCharctersFrom(this string source, string possibleChars)
        {
            return ContainsAnyCharctersFrom(source,possibleChars.ToCharArray());
        }
    }
}
