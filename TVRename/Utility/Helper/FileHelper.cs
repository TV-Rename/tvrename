//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Alphaleonis.Win32.Filesystem;
using Humanizer;
using MediaInfo;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.COMNative.Shell.PropertySystem;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename;

public static class FileHelper
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    internal static void DeleteOrRecycleFolder(DirectoryInfo? di, TVSettings.TidySettings? tidyup)
    {
        if (di is null)
        {
            return;
        }

        if (tidyup == null || tidyup.DeleteEmptyIsRecycle)
        {
            //TODO make all one use of the folder removal
            Logger.Info($"Recycling {di.FullName}");
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(di.FullName,
                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }
        else
        {
            Logger.Info($"Deleting {di.FullName}");
            di.Delete(true, true);
        }
    }

    public static void DoTidyUp(DirectoryInfo? di, TVSettings.TidySettings? settings)
    {
        if (settings is null || !settings.DeleteEmpty)
        {
            return;
        }

        // See if we should now delete the folder we just moved that file from.
        if (di is null)
        {
            return;
        }

        //if there are sub-directories then we shouldn't remove this one
        DirectoryInfo[] directories = di.GetDirectories();
        foreach (DirectoryInfo subdi in directories)
        {
            bool okToDelete = settings.EmptyIgnoreWordsArray.Any(word =>
                subdi.Name.Contains(word, StringComparison.OrdinalIgnoreCase));

            if (!okToDelete)
            {
                Logger.Info($"Not Removing {di.FullName} as it contains {subdi.Name} which does not have {settings.EmptyIgnoreWordsArray.ToCsv()} in the directory name.");
                return;
            }
        }
        //we know that each subfolder is OK to delete

        //if the directory is the root download folder do not delete
        if (TVSettings.Instance.DownloadFolders.Contains(di.FullName))
        {
            return;
        }

        // Do not delete any monitor folders either
        if (TVSettings.Instance.LibraryFolders.Contains(di.FullName))
        {
            return;
        }
        if (TVSettings.Instance.MovieLibraryFolders.Contains(di.FullName))
        {
            return;
        }

        FileInfo[] files = di.GetFiles();
        if (files.Length == 0)
        {
            // its empty, so just delete it
            DeleteOrRecycleFolder(di, settings);
            return;
        }

        if (settings.EmptyIgnoreExtensions && !settings.EmptyIgnoreWords)
        {
            return; // nope
        }

        foreach (FileInfo fi in files.Where(x=>!CanDelete(x,settings)))
        {
            Logger.Info($"Not Removing {di.FullName} as it contains {fi.Name} which does not have {settings.EmptyIgnoreExtensionsArray.ToCsv()} as extension or {settings.EmptyIgnoreWordsArray.ToCsv()} in the filename.");
            return;
        }

        if (settings.EmptyMaxSizeCheck)
        {
            // how many MB are we deleting?
            long totalBytes = files.Sum(fi => fi.Length);

            double mbytes = 1.0 * totalBytes / (1024 * 1024);
            if (mbytes > settings.EmptyMaxSizeMB)
            {
                Logger.Info(
                    $"Not Removing {di.FullName} as it contains too much Mb of files [{mbytes} vs {settings.EmptyMaxSizeMB}]");
                return; // too much
            }
        }

        DeleteOrRecycleFolder(di, settings);
    }

    private static bool CanDelete(FileInfo fi, TVSettings.TidySettings settings)
    {
        bool extensionOkToRemove = settings.EmptyIgnoreExtensions &&
                                   settings.EmptyIgnoreExtensionsArray.Contains(fi.Extension);

        bool fileOkToRemove = settings.EmptyIgnoreWordsArray
            .Any(word => fi.Name.Contains(word, StringComparison.OrdinalIgnoreCase));

        return extensionOkToRemove || fileOkToRemove;
    }

    public static int GetFrameWidth(this FileInfo movieFile)
    {
        return GetMetaDetails(movieFile, o => o.Properties.System.Video.FrameWidth, Parse, "GetFrameWidth",
            wrapper => wrapper.Width, i => i);
    }

    public enum VideoComparison
    {
        firstFileBetter,
        secondFileBetter,
        similar,
        cantTell,
        same
    }

    public static VideoComparison BetterQualityFile(FileInfo incumbantFile, FileInfo newFile)
    {
        if (!newFile.IsMovieFile())
        {
            return VideoComparison.firstFileBetter;
        }

        if (!incumbantFile.IsMovieFile())
        {
            return VideoComparison.secondFileBetter;
        }

        int encumbantLength = incumbantFile.GetFilmLength();
        int newFileLength = newFile.GetFilmLength();
        int encumbantFrameWidth = incumbantFile.GetFrameWidth();
        int newFileFrameWidth = newFile.GetFrameWidth();

        bool newFileContainsTerm =
            TVSettings.Instance.PriorityReplaceTermsArray.Any(term => newFile.Name.Contains(term, StringComparison.OrdinalIgnoreCase));

        bool incumbantFileContainsTerm =
            TVSettings.Instance.PriorityReplaceTermsArray.Any(term => incumbantFile.Name.Contains(term, StringComparison.OrdinalIgnoreCase));

        float percentMargin = TVSettings.Instance.replaceMargin;
        float marginMultiplier = (percentMargin + 100) / 100;

        bool incumbantFileIsMuchLonger = encumbantLength > newFileLength * marginMultiplier;
        bool newFileIsMuchLonger = encumbantLength * marginMultiplier < newFileLength;

        bool newFileIsBetterQuality = encumbantFrameWidth * marginMultiplier < newFileFrameWidth;
        bool incumbantFileIsBetterQuality = encumbantFrameWidth > newFileFrameWidth * marginMultiplier;

        if (encumbantLength == newFileLength && encumbantFrameWidth == newFileFrameWidth &&
            newFile.Length == incumbantFile.Length)
        {
            return VideoComparison.same;
        }

        if (encumbantLength == -1
            || newFileLength == -1
            || encumbantFrameWidth == -1
            || newFileFrameWidth == -1)
        {
            return VideoComparison.cantTell;
        }

        if (incumbantFileIsBetterQuality && !newFileIsMuchLonger)
        {
            return VideoComparison.firstFileBetter;  //existing file is better quality
        }

        if (incumbantFileIsMuchLonger && !newFileIsBetterQuality)
        {
            return VideoComparison.firstFileBetter;  //existing file is longer
        }

        if (newFileIsBetterQuality && !incumbantFileIsMuchLonger)
        {
            return VideoComparison.secondFileBetter;
        }

        if (newFileIsMuchLonger && !incumbantFileIsBetterQuality)
        {
            return VideoComparison.secondFileBetter;
        }

        if (newFileContainsTerm)
        {
            return VideoComparison.secondFileBetter;
        }

        if (incumbantFileContainsTerm)
        {
            return VideoComparison.firstFileBetter;
        }

        return VideoComparison.similar;
    }

    public static int GetFrameHeight(this FileInfo movieFile)
    {
        return GetMetaDetails(movieFile, o => o.Properties.System.Video.FrameHeight, Parse, "GetFrameHeight",
            wrapper => wrapper.Height, i => i);
    }

    private static int Parse(string returnValue, FileInfo sourceFile)
    {
        if (int.TryParse(returnValue, out int value))
        {
            return value;
        }

        return -1;
    }

    public static bool IsImportant(this DirectoryInfo info)
    {
        if (info.Name.Equals("subs", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (info.Name.Equals("subtitles", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (info.Name.Equals("subfiles", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (info.Name.Equals(".actors", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (info.Name.Equals("actors", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (info.Name.Equals("sample", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (info.Name.Equals(".AppleDouble", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (info.Name.Equals("art", StringComparison.CurrentCultureIgnoreCase))
        {
            return false;
        }

        return true;
    }

    public static bool IsSampleFile(this FileInfo fi) => fi.FullName.Contains("sample", StringComparison.OrdinalIgnoreCase) &&
                                                         fi.Length / (1024 * 1024) < TVSettings.Instance.SampleFileMaxSizeMB;

    public static bool IsDeletedStubFile(this FileInfo fi) =>
        (fi.Name.StartsWith("-.", StringComparison.Ordinal) || fi.Name.StartsWith("._", StringComparison.Ordinal)) && fi.Length / 1024 < 10;

    public static bool IsMovieFile(this FileInfo file) => TVSettings.Instance.FileHasUsefulExtension(file, false);

    public static bool IsThumb(this FileInfo file) =>
        file.FileNameNoExt().EndsWith("-thumb", StringComparison.OrdinalIgnoreCase);

    public static bool IsArchiveFile(this FileInfo file) => file.IsFileOfType(".zip;.rar;.tar;.tar.gz;.gz;.7z");
    public static bool IsImageFile(this FileInfo file) => file.IsFileOfType(".jpg;.jpeg;.gif;.tbn");
    private static bool IsFileOfType(this FileInfo file,string extensions)
    {
        return extensions.Split(';')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Any(s => file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase));
    }

    public static bool IsMovieFile(this string filename) => TVSettings.Instance.FileHasUsefulExtension(filename, false);
    public static bool IsMovieOrUsefulFile(this FileInfo file) => TVSettings.Instance.FileHasUsefulExtension(file, true);

    public static (bool found, string extension) IsLanguageSpecificSubtitle(this FileInfo file)
    {
        const string TOKEN = "{EXTENSION}";

        //Important we have the longer ones first
        string[] regexPatterns =
        {
            @"(?<ext>\.synced\.\w{2}-\w{2}\" + TOKEN + ")$",
            @"(?<ext>\.synced\.\w{2,3}\" + TOKEN + ")$",
            @"(?<ext>\.\w{2,3}\.synced\" + TOKEN + ")$",
            @"(?<ext>\.\w{2}-\w{2}\.synced\" + TOKEN + ")$",

            @"(?<ext>\.forced\.\w{2}-\w{2}\" + TOKEN + ")$",
            @"(?<ext>\.forced\.\w{2,3}\" + TOKEN + ")$",
            @"(?<ext>\.\w{2,3}\.forced\" + TOKEN + ")$",
            @"(?<ext>\.\w{2}-\w{2}\.forced\" + TOKEN + ")$",

            @"(?<ext>\.cc\.\w{2}-\w{2}\" + TOKEN + ")$",
            @"(?<ext>\.cc\.\w{2,3}\" + TOKEN + ")$",
            @"(?<ext>\.\w{2,3}\.cc\" + TOKEN + ")$",
            @"(?<ext>\.\w{2}-\w{2}\.cc\" + TOKEN + ")$",

            @"(?<ext>\.sdh\.\w{2}-\w{2}\" + TOKEN + ")$",
            @"(?<ext>\.sdh\.\w{2,3}\" + TOKEN + ")$",
            @"(?<ext>\.\w{2,3}\.sdh\" + TOKEN + ")$",
            @"(?<ext>\.\w{2}-\w{2}\.sdh\" + TOKEN + ")$",

            @"(?<ext>\.\w{2,3}\" + TOKEN + ")$",
            @"(?<ext>\.\w{2}-\w{2}\" + TOKEN + ")$",
        };

        foreach (string subExtension in TVSettings.Instance.SubtitleExtensionsArray
                     .Where(subExtension => file.Name.EndsWith(subExtension, StringComparison.CurrentCultureIgnoreCase))
                )
        {
            foreach (Match m in regexPatterns
                         .Select(oldpattern =>  oldpattern.Replace(TOKEN, subExtension))
                         .Select(regexPattern => Regex.Match(file.Name, regexPattern, RegexOptions.IgnoreCase))
                         .Where(m=>m.Success)
                    )
            {
                return (true, m.Groups["ext"].ToString());
            }
        }

        return (false, string.Empty);
    }

    public static string UrlPathFullName(this FileInfo baseFile)
    {
        return baseFile.FullName;
    }

    public static FileInfo WithExtension(this FileInfo baseFile, string extension) => new(baseFile.RemoveExtension(true) + extension);

    private static int GetMetaDetails(this FileInfo movieFile, Func<ShellObject, IShellProperty> extractMethod, Func<string, FileInfo, int> parseMethod, string operation, Func<MediaInfoWrapper, int> meExtractMethod, Func<int, int> meParseMethod)
    {
        try
        {
            string duration;
            using (ShellObject shell = ShellObject.FromParsingName(movieFile.FullName))
            {
                duration = extractMethod(shell).FormatForDisplay(PropertyDescriptionFormatOptions.None);
            }

            if (!duration.IsNullOrWhitespace())
            {
                int returnValue = parseMethod(duration, movieFile);

                if (returnValue > 0)
                {
                    return returnValue;
                }
            }
        }
        catch (System.IO.FileNotFoundException)
        {
            Logger.Warn($"Unable to find file as part of {operation} for {movieFile.FullName}");
            return -1;
        }
        catch (ArgumentException)
        {
            Logger.Warn(
                $"Unable to use shell to access file (ArgumentException) as part of {operation} for {movieFile.FullName}");
        }
        catch (ShellException)
        {
            Logger.Warn($"Unable to use shell to access file as part of {operation} for {movieFile.FullName}");
        }
        catch (PlatformNotSupportedException pe)
        {
            Logger.Error(
                $"Unable to use shell to access file as part of {operation} for {movieFile.FullName}. Platform is not supported: {pe.Message}");
        }

        MediaInfoWrapper mw = new(movieFile.FullName);
        int returnVal = meExtractMethod(mw);

        if (returnVal != 0)
        {
            return meParseMethod(returnVal);
        }

        Logger.Warn($"Could not {operation} for {movieFile.FullName} by using MediaInfo.");

        return -1;
    }

    public static int GetFilmLength(this FileInfo movieFile)
    {
        return GetMetaDetails(movieFile, o => o.Properties.System.Media.Duration, ParseDuration,
            "GetFilmLength", wrapper => wrapper.Duration, ParseMwDuration);
    }

    private static int ParseMwDuration(int returnVal)
    {
        float ret = returnVal / 1000f;
        return (int)Math.Round(ret);
    }

    private static int ParseDuration(string? duration, FileInfo sourceFile)
    {
        try
        {
            // Duration should be formatted as "00:44:08"
            if (duration.HasValue())
            {
                string[] timeParts = duration.Split(':');

                return (int)(int.Parse(timeParts[0]).Hours().TotalSeconds
                             + int.Parse(timeParts[1]).Minutes().TotalSeconds
                             + int.Parse(timeParts[2]).Seconds().TotalSeconds);
            }
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

            Logger.Warn($"Unable to parse string '{duration}' as part of GetFilmLength for {sourceFile.FullName}");
        }

        return -1;
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
        try
        {
            return string.Compare(directoryPath1.NormalizePath().TrimEnd('\\'),
                       directoryPath2.NormalizePath().TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) ==
                   0;
        }
        catch (UriFormatException e)
        {
            Logger.Warn(e, $"Could not compare {directoryPath1} and {directoryPath2}, assuming they are not the same location");
            return false;
        }
    }

    private static string NormalizePath(this string path)
    {
        //https://stackoverflow.com/questions/2281531/how-can-i-compare-directory-paths-in-c
        return Path.GetFullPath(new Uri(path).LocalPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .ToUpperInvariant();
    }

    public static string RemoveExtension(this FileInfo file) => RemoveExtension(file, false);

    public static string RemoveExtension(this FileInfo file, bool useFullPath)
    {
        string root = useFullPath ? file.FullName : file.Name;
        return root.RemoveLast(file.Extension.Length);
    }

    public static string GetFilmDetails(this FileInfo movieFile)
    {
        using ShellPropertyCollection properties = new(movieFile.FullName);
        StringBuilder sb = new();
        foreach (IShellProperty prop in properties)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            string value = prop.ValueAsObject is null
                ? string.Empty
                : prop.FormatForDisplay(PropertyDescriptionFormatOptions.None);

            sb.AppendLine($"{prop.CanonicalName} = {value}");
        }

        return sb.ToString();
    }

    public static bool IsSubfolderOf(this string thisOne, string ofThat)
    {
        // need terminating slash, otherwise "c:\abc def" will match "c:\abc"
        if (!thisOne.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
        {
            thisOne += System.IO.Path.DirectorySeparatorChar.ToString();
        }

        if (!ofThat.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
        {
            ofThat += System.IO.Path.DirectorySeparatorChar.ToString();
        }

        int l = ofThat.Length;
        return thisOne.Length >= l && string.Equals(thisOne.Take(l), ofThat, StringComparison.CurrentCultureIgnoreCase);
    }

    public static string TrimSlash(this string s) // trim trailing slash
    {
        return s.TrimEnd(System.IO.Path.DirectorySeparatorChar);
    }

    // ReSharper disable once InconsistentNaming
    public static string GBMB(this long value) => GBMB(value, 2);

    // ReSharper disable once InconsistentNaming
    public static string GBMB(this long value, int decimalPlaces)
    {
        const long ONE_KB = 1024;
        const long ONE_MB = ONE_KB * 1024;
        const long ONE_GB = ONE_MB * 1024;
        const long ONE_TB = ONE_GB * 1024;

        double asTb = Math.Round((double)value / ONE_TB, decimalPlaces);
        double asGb = Math.Round((double)value / ONE_GB, decimalPlaces);
        double asMb = Math.Round((double)value / ONE_MB, decimalPlaces);
        double asKb = Math.Round((double)value / ONE_KB, decimalPlaces);
        double asB = Math.Round((double)value, decimalPlaces);
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

    public static string ToCsv(this IEnumerable<FileInfo> files) => files.Select(f => f.Name).ToCsv();

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
                    {
                        File.Delete(fn2);
                    }

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
        string n1 = a.FullName.EnsureEndsWithSeparator();
        string n2 = b.FullName.EnsureEndsWithSeparator();

        return string.Compare(n1, n2, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static string EnsureEndsWithSeparator(this string source)
    {
        if (!source.HasValue())
        {
            return Path.DirectorySeparatorChar.ToString();
        }
        if (source.Trim().EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
        {
            return source.Trim();
        }
        return source.Trim() + Path.DirectorySeparatorChar;
    }

    public static FileInfo FileInFolder(string dir, string fn) => new(dir.EnsureEndsWithSeparator() + fn);

    public static void RemoveDirectory(string folderName)
    {
        try
        {
            Logger.Info($"Removing {folderName} as part of the library clean up");
            foreach (string file in Directory.GetFiles(folderName,"*",System.IO.SearchOption.AllDirectories))
            {
                Logger.Info($"    Folder contains {file}");
            }
            foreach (string file in Directory.GetDirectories(folderName))
            {
                Logger.Info($"    Folder contains folder {file}");
            }

            //TODO make all one use of the folder removal
            Logger.Info($"Recycling {folderName}");
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(folderName,
                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }
        catch (FileReadOnlyException)
        {
            Logger.Warn($"Could not recycle {folderName} as we got a FileReadOnlyException");
        }
        catch (DirectoryReadOnlyException)
        {
            Logger.Warn($"Could not recycle {folderName} as we got a DirectoryReadOnlyException");
        }
        catch (UnauthorizedAccessException)
        {
            Logger.Warn($"Could not recycle {folderName} as we got a UnauthorizedAccessException");
        }
        catch (System.IO.PathTooLongException)
        {
            Logger.Warn($"Could not recycle {folderName} as we got a PathTooLongException");
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            Logger.Info($"Could not recycle {folderName} as we got a DirectoryNotFoundException");
        }
        catch (DirectoryNotEmptyException)
        {
            Logger.Warn($"Could not recycle {folderName} as we got a DirectoryNotEmptyException");
        }
        catch (OperationCanceledException)
        {
            Logger.Info($"Could not recycle {folderName} as we got a OperationCanceledException");
        }
        catch (System.IO.IOException i)
        {
            Logger.Warn($"Could not find {folderName} as we got a OperationCanceledException: {i.Message}");
        }
    }

    public static FileInfo FileInFolder(DirectoryInfo di, string fn) => FileInFolder(di.FullName, fn);

    // see if showname is somewhere in filename
    public static bool SimplifyAndCheckFilename(string filename, string showName, bool simplifyfilename, bool simplifyshowname)
    {
        Match match = Regex.Match(simplifyfilename ? filename.CompareName() : filename, "\\b" + (simplifyshowname ? showName.CompareName() : showName) + "\\b", RegexOptions.IgnoreCase);
        return match.Success;
    }

    public static int SimplifyAndCheckFilenameLength(string filename, string showName, bool simplifyfilename, bool simplifyshowname)
    {
        Match match = Regex.Match(simplifyfilename ? filename.CompareName() : filename, "\\b" + (simplifyshowname ? showName.CompareName() : showName) + "\\b", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Length;
        }
        return 0;
    }

    public static bool SimplifyAndCheckFilenameAtStart(string filename, string showName) =>
        SimplifyAndCheckFilenameAtStart(filename, showName, true, true);

    private static bool SimplifyAndCheckFilenameAtStart(string filename, string showName, bool simplifyFilename, bool simplifyShowName)
    {
        string showPattern = simplifyShowName ? showName.CompareName() : showName;

        return (simplifyFilename ? filename.CompareName() : filename).StartsWith(showPattern, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool SimplifyAndCheckFilename(string filename, string showName)
    {
        return SimplifyAndCheckFilename(filename, showName, true, true);
    }

    public static string MakeValidPath(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

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
        {
            return true; // move on
        }

        if (TVSettings.Instance.IgnoreSamples && fi.IsSampleFile())
        {
            return true;
        }

        if (fi.IsDeletedStubFile())
        {
            return true;
        }

        return false;
    }

    internal static bool FileExistsCaseSensitive(IEnumerable<FileInfo> files, FileInfo newFile)
    {
        return files.Any(testFile => string.Equals(testFile.Name, newFile.Name, StringComparison.CurrentCulture));
    }

    public static bool IsValidDirectory(this string directoryName)
    {
        if (directoryName.ContainsAnyCharactersFrom(Path.GetInvalidPathChars()))
        {
            return false;
        }

        if (directoryName.ContainsAnyCharactersFrom("*?"))
        {
            return false;
        }

        return true;
    }

    // see https://kodi.wiki/view/Naming_video_files/Movies#Split_Video_Files
    private static readonly List<string> Ending = new() { "part", "cd", "dvd", "pt", "disk", "disc" };

    public static bool IsDoublePartMovie(FileInfo f1, FileInfo f2)
    {
        return Ending.Any(end => HasEnding(f1, f2, end));
    }

    private static bool HasEnding(FileInfo f1, FileInfo f2, string part)
    {
        if (f1.FileNameNoExt().EndsWith(part + "1", true, CultureInfo.CurrentCulture) &&
            f2.FileNameNoExt().EndsWith(part + "2", true, CultureInfo.CurrentCulture))
        {
            return true;
        }
        if (f1.FileNameNoExt().EndsWith(part + "2", true, CultureInfo.CurrentCulture) &&
            f2.FileNameNoExt().EndsWith(part + "1", true, CultureInfo.CurrentCulture))
        {
            return true;
        }
        if (f1.FileNameNoExt().EndsWith(part + ".1", true, CultureInfo.CurrentCulture) &&
            f2.FileNameNoExt().EndsWith(part + ".2", true, CultureInfo.CurrentCulture))
        {
            return true;
        }
        if (f1.FileNameNoExt().EndsWith(part + ".2", true, CultureInfo.CurrentCulture) &&
            f2.FileNameNoExt().EndsWith(part + ".1", true, CultureInfo.CurrentCulture))
        {
            return true;
        }
        if (f1.FileNameNoExt().EndsWith(part + " 1", true, CultureInfo.CurrentCulture) &&
            f2.FileNameNoExt().EndsWith(part + " 2", true, CultureInfo.CurrentCulture))
        {
            return true;
        }
        if (f1.FileNameNoExt().EndsWith(part + " 2", true, CultureInfo.CurrentCulture) &&
            f2.FileNameNoExt().EndsWith(part + " 1", true, CultureInfo.CurrentCulture))
        {
            return true;
        }
        if (f1.FileNameNoExt().EndsWith(part + "01", true, CultureInfo.CurrentCulture) &&
            f2.FileNameNoExt().EndsWith(part + "02", true, CultureInfo.CurrentCulture))
        {
            return true;
        }
        if (f1.FileNameNoExt().EndsWith(part + "02", true, CultureInfo.CurrentCulture) &&
            f2.FileNameNoExt().EndsWith(part + "01", true, CultureInfo.CurrentCulture))
        {
            return true;
        }
        return false;
    }

    public static string FileNameNoExt(this FileInfo f) => f.Name.RemoveAfter(f.Extension);
    public static string FileFullNameNoExt(this FileInfo f) => f.FullName.RemoveAfter(f.Extension);

    private static readonly Regex[] MovieMultiPartRegex =
    {
        new(@"(?<base>.*)[ _.-]+(cd|dvd|pt|part|disc|disk)[ _.-0]*(?<part>[0-9]|[A-D])$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"(?<base>.*)[ ._-]+(?<part>[A-D])$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
    };

    public static bool IsRecycleBin(this DirectoryInfo di2)
    {
        bool endsWith = di2.FullName.Contains("$RECYCLE.BIN", StringComparison.OrdinalIgnoreCase)
                        || di2.FullName.Contains("\\@Recycle\\", StringComparison.OrdinalIgnoreCase)
                        || di2.FullName.EndsWith("\\@Recycle", StringComparison.OrdinalIgnoreCase);

        return endsWith;
    }

    public static string MovieFileNameBase(this FileInfo movieFile)
    {
        string longbase = movieFile.FileNameNoExt();
        foreach (Match x in MovieMultiPartRegex.Select(tets => tets.Match(longbase)).Where(x => x.Success))
        {
            return x.Groups["base"].Value;
        }

        return longbase;
    }

    public static bool IsRootDirectory(string folderName)
    {
        DirectoryInfo d = new(folderName);
        return d.Parent == null;
    }
}
