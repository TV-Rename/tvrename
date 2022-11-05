//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace TVRename;

public static class StringExtensions
{
    public static string ItemItems(this int n) => n == 1 ? "Item" : "Items";

    public static bool IsWebLink(this string s) => s.IsHttpLink() || s.IsFtpLink();

    public static bool IsPlaceholderName(this string? s) =>
        s is null
        || s.Equals("TBA", StringComparison.OrdinalIgnoreCase)
        || s.Equals("TBC", StringComparison.OrdinalIgnoreCase)
        || s.Equals("TBD", StringComparison.OrdinalIgnoreCase);

    public static bool IsHttpLink(this string s) =>
        s.StartsWith("http://", StringComparison.Ordinal)
        || s.StartsWith("https://", StringComparison.Ordinal);

    public static bool IsFtpLink(this string s) =>
        s.StartsWith("ftp://", StringComparison.Ordinal);

    public static bool IsFileLink(this string s) =>
        s.StartsWith("file://", StringComparison.Ordinal);

    public static string RemoveCharactersFrom(this string source, IEnumerable<char> badChars)
    {
        string returnValue = source;
        foreach (char x in badChars)
        {
            returnValue = returnValue.Replace(x.ToString(), string.Empty);
        }

        return returnValue;
    }

    public static bool IsNumeric(this string text) => int.TryParse(text, out int _);

    public static bool HasValue([NotNullWhen(true)] this string? s) => !string.IsNullOrWhiteSpace(s);

    public static string CompareName(this string n)
    {
        n = n.ToLower();
        n = RemoveDiacritics(n);
        n = n.Replace(":", "");
        n = n.Replace(".", " ");
        n = n.Replace("'", "");
        n = n.Replace("‘", "");
        n = n.Replace("\"", "");
        n = n.Replace("&", "and");
        n = n.Replace("!", "");
        n = n.Replace("*", "");
        n = Regex.Replace(n, "[_\\W]+", " ");
        n = Regex.Replace(n, "[^\\w ]", " ");
        return n.Trim();
    }

    public static string InDoubleQuotes(this string? source)
    {
        StringBuilder sb = new();
        sb.Append('"').Append(source).Append('"');
        return sb.ToString();
    }
    public static string RemoveDiacritics(this string stIn)
    {
        // From http://blogs.msdn.com/b/michkap/archive/2007/05/14/2629747.aspx
        string stFormD = stIn.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new();

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

    public static string RemovePattern(this string baseText, string pattern)
    {
        if (!baseText.HasValue())
        {
            return string.Empty;
        }

        Match mat = Regex.Match(baseText.Trim(), pattern);

        if (mat.Success)
        {
            //Try removing any year
            return Regex.Replace(baseText.Trim(), pattern, string.Empty).Trim();
        }

        return baseText.Trim();
    }

    //Remove any (nnnn) in the hint - probably a year
    public static string RemoveBracketedYear(this string baseText)
        => baseText
            .RemovePattern(@"\(\d{4}\)")
            .RemovePattern(@"\[\d{4}\]")
            .Trim();

    public static string RemoveYearFromEnd(this string baseText)
        => baseText
            .RemovePattern(@"\s\d{4}$")
            .Trim();

    public static string RemoveBracketedYearFromEnd(this string baseText)
        => baseText
            .RemovePattern(@"\s\[\d{4}\]$")
            .RemovePattern(@"\s\(\d{4}\)$")
            .Trim();

    public static string ReplaceInsensitive(this string source, string search, string? replacement)
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

    public static string ReplaceInsensitiveLazy(this string? source, string search, Lazy<string?> replacement, StringComparison comparison)
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
            sb.Append(source.AsSpan(previousIndex, index - previousIndex));
            sb.Append(replacementValue);

            previousIndex = index + search.Length;
            index = source.IndexOf(search, previousIndex, comparison);
        }
        sb.Append(source.AsSpan(previousIndex));

        return sb.ToString();
    }

    public static bool ContainsAnyCharactersFrom(this string source, IEnumerable<char> possibleChars)
    {
        return possibleChars.Any(testChar => source.Contains(testChar.ToString()));
    }

    public static bool ContainsAnyCharactersFrom(this string source, string possibleChars)
    {
        return ContainsAnyCharactersFrom(source, possibleChars.ToCharArray());
    }

    public static string UppercaseFirst(this string? str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        return char.ToUpper(str[0]) + str.RemoveFirstCharacter().ToLower();
    }

    public static bool IsNullOrWhitespace(this string? text) => string.IsNullOrWhiteSpace(text);

    public static string RemoveLastCharacter(this string instr)
    {
        return instr[..^1];
    }

    public static string First(this string? s, int charsToDisplay)
    {
        if (s.HasValue())
        {
            return s.Length <= charsToDisplay ? s : new string(s.Take(charsToDisplay).ToArray());
        }

        return string.Empty;
    }

    public static string Initial(this string str) => str.HasValue() ? str[..1] : string.Empty;

    public static string RemoveLast(this string instr, int number)
    {
        return instr[..^number];
    }

    public static string RemoveFirstCharacter(this string instr)
    {
        return instr.RemoveFirst(1);
    }

    public static string RemoveFirst(this string instr, int number)
    {
        return instr[number..];
    }
    public static string Take(this string instr, int number)
    {
        return instr[..number];
    }

    public static string GetCommonStartString(IEnumerable<string> testValues)
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

    public static string TrimEnd(this string root, string ending)
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

    public static string TrimStartString(this string root, string startString)
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

    public static string RemoveAfter(this string root, string ending)
    {
        if (root.IndexOf(ending, StringComparison.OrdinalIgnoreCase) != -1)
        {
            return root[..root.IndexOf(ending, StringComparison.OrdinalIgnoreCase)];
        }

        return root;
    }

    public static string TrimEnd(this string root, IEnumerable<string> endings)
    {
        string trimmedString = root;
        foreach (string ending in endings)
        {
            trimmedString = trimmedString.TrimEnd(ending);
        }
        return trimmedString;
    }

    public static string GetCommonStartString(string first, string second)
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

    public static bool ContainsOneOf(this string source, IEnumerable<string> terms) => terms.Any(source.Contains);

    public static string ToCsv(this IEnumerable<string?> values) => string.Join(", ", values);

    public static string ToCsv(this IEnumerable<int> values) => string.Join(",", values);

    public static string ToPsv(this IEnumerable<string> values) => string.Join("|", values);

    public static IEnumerable<string> FromPsv(this string? aggregate) => aggregate.FromSepValues('|');
    public static IEnumerable<string> FromCsv(this string? aggregate) => aggregate.FromSepValues(',');
    private static IEnumerable<string> FromSepValues(this string? aggregate, char delimiter)
    {
        return string.IsNullOrEmpty(aggregate)
            ? Array.Empty<string>()
            : aggregate.Split(delimiter).ValidStrings();
    }

    public static IEnumerable<string> ValidStrings(this IEnumerable<string?>? possibleStrings)
    {
        return possibleStrings?
            .Where(s => s.HasValue())
            .OfType<string>()
            .Select(s => s.Trim()) ?? Array.Empty<string>();
    }

    public static int? ToInt(this string? value)
    {
        if (value.HasValue())
        {
            if (int.TryParse(value, out int x))
            {
                return x;
            }
        }

        return null;
    }

    public static string Concat(this IEnumerable<string> values) => string.Join(string.Empty, values);
}
