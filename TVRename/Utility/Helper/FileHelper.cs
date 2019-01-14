using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Alphaleonis.Win32.Filesystem;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace TVRename
{
    public static class FileHelper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static int GetFrameWidth(this FileInfo movieFile)
        {
            using (ShellObject shell = ShellObject.FromParsingName(movieFile.FullName))
            {
                IShellProperty prop = shell.Properties.System.Video.FrameWidth;
                string returnValue = prop.FormatForDisplay(PropertyDescriptionFormatOptions.None);
                return int.TryParse(returnValue, out int value) ? value : -1;
            }
        }

        public enum VideoComparison
        {
            firstFileBetter,
            secondFileBetter,
            similar,
            cantTell,
            same
        }

        public static VideoComparison BetterQualityFile(FileInfo encumbantFile, FileInfo newFile)
        {
            if (!newFile.IsMovieFile()) return VideoComparison.firstFileBetter;
            if (!encumbantFile.IsMovieFile()) return VideoComparison.secondFileBetter;

            int encumbantLength = encumbantFile.GetFilmLength();
            int newFileLength = newFile.GetFilmLength();
            int encumbantFrameWidth = encumbantFile.GetFrameWidth();
            int newFileFrameWidth = newFile.GetFrameWidth();

            bool newFileContainsTerm =
                TVSettings.Instance.PriorityReplaceTermsArray.Any(term => newFile.Name.Contains(term, StringComparison.OrdinalIgnoreCase));

            float percentMargin = TVSettings.Instance.replaceMargin;
            float marginMultiplier = (percentMargin + 100) / 100;

            bool encumbantFileIsMuchLonger = encumbantLength > newFileLength * marginMultiplier;
            bool newFileIsMuchLonger = encumbantLength * marginMultiplier < newFileLength;

            bool newFileIsBetterQuality = encumbantFrameWidth * marginMultiplier < newFileFrameWidth;
            bool encumbantFileIsBetterQuality = encumbantFrameWidth > newFileFrameWidth * marginMultiplier;

            if (encumbantLength == newFileLength && encumbantFrameWidth == newFileFrameWidth &&
                newFile.Length == encumbantFile.Length) return VideoComparison.same;
            
            if (encumbantLength == -1) return VideoComparison.cantTell;
            if (newFileLength == -1) return VideoComparison.cantTell;
            if (encumbantFrameWidth == -1) return VideoComparison.cantTell;
            if (newFileFrameWidth == -1) return VideoComparison.cantTell;
            
            if (encumbantFileIsBetterQuality && !newFileIsMuchLonger) return VideoComparison.firstFileBetter;  //exting file is better quality
            if (encumbantFileIsMuchLonger && !newFileIsBetterQuality) return VideoComparison.firstFileBetter;  //exting file is longer

            if (newFileIsBetterQuality && !encumbantFileIsMuchLonger) return VideoComparison.secondFileBetter;
            if (newFileIsMuchLonger && !encumbantFileIsBetterQuality) return VideoComparison.secondFileBetter;

            if (newFileContainsTerm) return VideoComparison.secondFileBetter;

            return VideoComparison.similar;
        }

        public static int GetFrameHeight(this FileInfo movieFile)
        {
            using (ShellObject shell = ShellObject.FromParsingName(movieFile.FullName))
            {
                IShellProperty prop = shell.Properties.System.Video.FrameHeight;
                string returnValue = prop.FormatForDisplay(PropertyDescriptionFormatOptions.None);
                return (int.TryParse(returnValue, out int value)) ? value : -1;
            }
        }

        public static bool IsMovieFile(this FileInfo file)
        {
            return TVSettings.Instance.FileHasUsefulExtension(file, false, out string _);
        }
        
        public static bool IsLanguageSpecificSubtitle(this FileInfo file, out string extension)
        {
            foreach (string subExtension in TVSettings.Instance.subtitleExtensionsArray)
            {
                string regex = @"(?<ext>\.\w{2,3}\"+subExtension+")$";

                Match m = Regex.Match(file.Name, regex, RegexOptions.IgnoreCase);

                if (!m.Success) continue;

                extension = m.Groups["ext"].ToString();
                return true;
            }

            extension = string.Empty;
            return false;
        }

        public static string UrlPathFullName(this FileInfo baseFile)
        {
            //TODO
            return baseFile.FullName;
        }

        public static FileInfo WithExtension(this FileInfo baseFile, string extension)
        {
            return new FileInfo(baseFile.RemoveExtension(true)+extension);
        }

        public static int GetFilmLength(this FileInfo movieFile)
        {
            string duration;
            using (ShellObject shell = ShellObject.FromParsingName(movieFile.FullName))
            {
                // alternatively: shell.Properties.GetProperty("System.Media.Duration");
                IShellProperty prop = shell.Properties.System.Media.Duration;
                // Duration will be formatted as 00:44:08
                duration = prop.FormatForDisplay(PropertyDescriptionFormatOptions.None);
            }

            if (string.IsNullOrWhiteSpace(duration)) return -1;
            try
            {
                return (3600 * int.Parse(duration.Split(':')[0]))
                       + (60 * int.Parse(duration.Split(':')[1]))
                       + int.Parse(duration.Split(':')[2]);
            }
            catch (FormatException)
            {
                //Need this section as we get random text back sometimes
//              Unable to parse string Unbekannt as part of GetFilmLength System
//              Unable to parse string Text hinzufügen as part of GetFilmLength System
//              Unable to parse string Add text as part of GetFilmLength System
//              Unable to parse string Unknown as part of GetFilmLength System
//              Unable to parse string Okänt as part of GetFilmLength System
//              Unable to parse string Lägg till text as part of GetFilmLength System

                Logger.Warn($"Unable to parse string '{duration}' as part of GetFilmLength for {movieFile.FullName}");
                return -1;
            }
        }

        public static bool FileExistsCaseSensitive(string filename)
        {
            string name = Path.GetDirectoryName(filename);

            return name != null
                   && Array.Exists(Directory.GetFiles(name), s => s == Path.GetFullPath(filename));
        }

        public static bool SameDirectoryLocation(this string directoryPath1, string directoryPath2)
        {
            // http://stackoverflow.com/questions/1794025/how-to-check-whether-2-directoryinfo-objects-are-pointing-to-the-same-directory
            return string.Compare(directoryPath1.NormalizePath().TrimEnd('\\'), directoryPath2.NormalizePath().TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        private static string NormalizePath(this string path)
        {
            //https://stackoverflow.com/questions/2281531/how-can-i-compare-directory-paths-in-c
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
        }

        public static string RemoveExtension(this FileInfo file, bool useFullPath = false)
        {
            string root = useFullPath ? file.FullName : file.Name;
            return root.Substring(0, root.Length - file.Extension.Length);
        }

        public static string GetFilmDetails(this FileInfo movieFile)
        {
            using (ShellPropertyCollection properties = new ShellPropertyCollection(movieFile.FullName))
            {
                StringBuilder sb = new StringBuilder();
                foreach (IShellProperty prop in properties)
                {
                    string value = (prop.ValueAsObject == null)
                        ? ""
                        : prop.FormatForDisplay(PropertyDescriptionFormatOptions.None);
                    sb.AppendLine($"{prop.CanonicalName} = {value}" );
                }

                return sb.ToString();
            }
        }

        public static bool IsSubfolderOf(this string thisOne, string ofThat)
        {
            // need terminating slash, otherwise "c:\abc def" will match "c:\abc"
            if (!thisOne.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                thisOne += System.IO.Path.DirectorySeparatorChar.ToString();
            }

            if (!ofThat.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                ofThat += System.IO.Path.DirectorySeparatorChar.ToString();
            }

            int l = ofThat.Length;
            return ((thisOne.Length >= l) && (thisOne.Substring(0, l).ToLower() == ofThat.ToLower()));
        }

        public static string TrimSlash(this string s) // trim trailing slash
        {
            return s.TrimEnd(System.IO.Path.DirectorySeparatorChar);
        }

        // ReSharper disable once InconsistentNaming
        public static string GBMB(this long value, int decimalPlaces = 2)
        {
            const long ONE_KB = 1024;
            const long ONE_MB = ONE_KB * 1024;
            const long ONE_GB = ONE_MB * 1024;
            const long ONE_TB = ONE_GB * 1024;

            double asTb = Math.Round((double)value / ONE_TB, decimalPlaces);
            double asGb = Math.Round((double)value / ONE_GB, decimalPlaces);
            double asMb = Math.Round((double)value / ONE_MB, decimalPlaces);
            double asKb = Math.Round((double)value / ONE_KB, decimalPlaces);
            double asB  = Math.Round((double)value, decimalPlaces);
            string chosenValue = asTb >= 1 ? $"{asTb:G3} TB"
                : asGb >= 1 ? $"{asGb:G3} GB"
                : asMb >= 1 ? $"{asMb:G3} MB"
                : asKb >= 1 ? $"{asKb:G3} KB"
                : $"{asB:G3} B";
            return chosenValue;
        }

        /// <summary>
        /// Gets the properties for this file system.
        /// </summary>
        /// <param name="volumeIdentifier">The path whose volume properties are to be queried.</param>
        /// <returns>A <see cref="FileSystemProperties"/> containing the properties for the specified file system.</returns>
        public static FileSystemProperties GetProperties(string volumeIdentifier)
        {
            if (NativeMethods.GetDiskFreeSpaceEx(volumeIdentifier, out ulong available, out ulong total, out ulong free))
            {
                return new FileSystemProperties((long)total, (long)free, (long)available);
            }
            return new FileSystemProperties(null, null, null);
        }

        public static void Rotate(string filenameBase)
        {
            if (File.Exists(filenameBase))
            {
                for (int i = 8; i >= 0; i--)
                {
                    string fn = filenameBase + "." + i;
                    if (File.Exists(fn))
                    {
                        string fn2 = filenameBase + "." + (i + 1);
                        if (File.Exists(fn2))
                            File.Delete(fn2);
                        File.Move(fn, fn2);
                    }
                }
                File.Copy(filenameBase, filenameBase + ".0");
            }
        }

        public static bool Same(FileInfo a, FileInfo b)
        {
            return string.Compare(a.FullName, b.FullName, StringComparison.OrdinalIgnoreCase) == 0; // true->ignore case
        }

        public static bool Same(DirectoryInfo a, DirectoryInfo b)
        {
            string n1 = a.FullName;
            string n2 = b.FullName;
            if (!n1.EndsWith(Path.DirectorySeparatorChar.ToString()))
                n1 = n1 + Path.DirectorySeparatorChar;
            if (!n2.EndsWith(Path.DirectorySeparatorChar.ToString()))
                n2 = n2 + Path.DirectorySeparatorChar;

            return string.Compare(n1, n2, StringComparison.OrdinalIgnoreCase) == 0; // true->ignore case
        }

        public static FileInfo FileInFolder(string dir, string fn)
        {
            return new FileInfo(string.Concat(dir, dir.EndsWith(Path.DirectorySeparatorChar.ToString()) ? "" : Path.DirectorySeparatorChar.ToString(), fn));
        }

        public static FileInfo FileInFolder(DirectoryInfo di, string fn)
        {
            return FileInFolder(di.FullName, fn);
        }

        // see if showname is somewhere in filename
        public static bool SimplifyAndCheckFilename(string filename, string showname, bool simplifyfilename, bool simplifyshowname)
        {
            return Regex.Match(simplifyfilename ? Helpers.SimplifyName(filename) : filename, "\\b" + (simplifyshowname ? Helpers.SimplifyName(showname) : showname) + "\\b", RegexOptions.IgnoreCase).Success;
        }

        public static bool SimplifyAndCheckFilename(string filename, string showname)
        {
            return SimplifyAndCheckFilename(filename, showname,true,true);
        }

        public static string MakeValidPath(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            string directoryName = input;
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                directoryName = directoryName.Replace(c.ToString(), "");
            }
            return directoryName;
        }

        public static bool IgnoreFile(this FileInfo fi)
        {
            if (!fi.IsMovieFile())
                return true; // move on

            if (TVSettings.Instance.IgnoreSamples &&
                Helpers.Contains(fi.FullName, "sample", StringComparison.OrdinalIgnoreCase) &&
                ((fi.Length / (1024 * 1024)) < TVSettings.Instance.SampleFileMaxSizeMB))
                return true;

            if (fi.Name.StartsWith("-.") && (fi.Length / 1024 < 10)) return true;

            return false;
        }

        internal static bool FileExistsCaseSensitive(FileInfo[] files, FileInfo newFile)
        {
            return files.Any(testFile => string.Equals(testFile.Name, newFile.Name, StringComparison.CurrentCulture));
        }
    }
}
