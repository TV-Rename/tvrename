using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TVRename.Core.Utility
{
    public static class Helpers
    {
        public static string SimplifyName(string n)
        {
            n = n.ToLower();
            n = n.Replace("the", string.Empty);
            n = n.Replace("'", string.Empty);
            n = n.Replace("&", string.Empty);
            n = n.Replace("and", string.Empty);
            n = n.Replace("!", string.Empty);
            n = Regex.Replace(n, "[_\\W]+", " ");
            return n.Trim();
        }
        
        public static IEnumerable<string> GetFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return GetFiles(path, new[] { searchPattern }, searchOption);
        }

        public static IEnumerable<string> GetFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return searchPatterns.AsParallel().SelectMany(searchPattern =>
            {
                try
                {
                    return Directory.EnumerateFiles(path, searchPattern, searchOption);
                }
                catch (DirectoryNotFoundException)
                {
                    return Enumerable.Empty<string>();
                }
                catch (UnauthorizedAccessException)
                {
                    return Enumerable.Empty<string>();
                }
            });
        }

        public static IEnumerable<string> GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return GetDirectories(path, new[] { searchPattern }, searchOption);
        }

        public static IEnumerable<string> GetDirectories(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return searchPatterns.AsParallel().SelectMany(searchPattern =>
            {
                try
                {
                    return Directory.EnumerateDirectories(path, searchPattern, searchOption);
                }
                catch (UnauthorizedAccessException)
                {
                    return Enumerable.Empty<string>();
                }
            });
        }

        public static async Task<IEnumerable<string>> GetFilesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return await GetFilesAsync(path, new[] { searchPattern }, searchOption);
        }

        public static async Task<IEnumerable<string>> GetFilesAsync(string directory, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return await Task.Run(() =>
            {
                return GetFiles(directory, searchPatterns, searchOption).OrderBy(x => x);
            });
        }

        public static async Task<IEnumerable<string>> GetDirectoriesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return await GetDirectoriesAsync(path, new[] { searchPattern }, searchOption);
        }

        public static async Task<IEnumerable<string>> GetDirectoriesAsync(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return await Task.Run(() =>
            {
                return GetDirectories(path, searchPatterns, searchOption).OrderBy(x => x);
            });
        }
    }
}
