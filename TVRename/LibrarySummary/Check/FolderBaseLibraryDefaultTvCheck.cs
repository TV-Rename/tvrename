using System;
using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename;

internal class FolderBaseLibraryDefaultTvCheck : TvShowCheck
{
    public FolderBaseLibraryDefaultTvCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
    {
    }

    public override bool Check() => Show.AutoAddFolderBase.HasValue() && TVSettings.Instance.LibraryFolders.Any(lf => lf.IsSubfolderOf(Show.AutoAddFolderBase));

    public override string Explain() => "This TV show's folder is a Library folder. This indicates that the files are stored at the root of the library.";

    /// <exception cref="FixCheckException"></exception>
    protected override void FixInternal()
    {
        //we want to move the files into a sub-folder.
        //This means define new folder and copy any matching files into that location
        string oldLocation = Show.AutoAddFolderBase;
        string libraryRoot =
            TVSettings.Instance.LibraryFolders.First(lf => lf.IsSubfolderOf(oldLocation));
        string newRoot = libraryRoot.EnsureEndsWithSeparator() + TVSettings.Instance.DefaultTVShowFolder(Show);

        //Then copy any matching files to new location
        try
        {
            List<FileInfo> filesWeMayCopy = new DirectoryInfo(oldLocation).EnumerateFiles(DirectoryEnumerationOptions.Recursive)
                .Where(f => f.IsMovieFile())
                .Where(f => Show.NameMatch(f, false))
                .ToList();

            foreach (FileInfo file in filesWeMayCopy)
            {
                string newLocation = file.DirectoryName.Replace(oldLocation.EnsureEndsWithSeparator(),
                    newRoot.EnsureEndsWithSeparator());
                LOGGER.Warn($"Moving {file.FullName} to {newLocation} as part of {CheckName}");
                Directory.CreateDirectory(newLocation);
                file.MoveTo(System.IO.Path.Combine(newLocation, file.Name));
            }

            Show.AutoAddFolderBase = newRoot;
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new FixCheckException(ex.Message, ex);
        }
        catch (IOException ex)
        {
            throw new FixCheckException(ex.Message, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new FixCheckException(ex.Message, ex);
        }
    }

    public override string CheckName => "[TV] Has an base folder that is a library folder";
}
