using System;
using Directory = Alphaleonis.Win32.Filesystem.Directory;

namespace TVRename;

internal class SubdirectoryMovieCheck : MovieCheck
{
    public SubdirectoryMovieCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
    {
    }

    public override bool Check() => Movie.UseCustomFolderNameFormat;

    public override string Explain() => $"This movie does not use the standard folder naming format '{TVSettings.Instance.MovieFolderFormat}', it uses '{Movie.CustomFolderNameFormat}'";

    /// <exception cref="FixCheckException">Condition.</exception>
    protected override void FixInternal()
    {
        string newLocation = Movie.AutomaticFolderRoot.EnsureEndsWithSeparator() + CustomMovieName.DirectoryNameFor(Movie, TVSettings.Instance.MovieFolderFormat);
        string currentLocation = Movie.AutoFolderNameForMovie();
        string message = $"Could not move files for {Movie.ShowName}. Would have liked to move files from [{currentLocation}] to '{newLocation}'";

        Movie.UseCustomFolderNameFormat = false;

        if (currentLocation == newLocation)
        {
            //Nothing to do
            return;
        }

        LOGGER.Info($"Moving files from '{currentLocation}' to '{newLocation}'");

        if (Directory.Exists(newLocation))
        {
            throw new FixCheckException($"{message}, but that directory already exists.");
        }

        try
        {
            Directory.Move(currentLocation, newLocation);
        }
        catch (UnauthorizedAccessException uae)
        {
            throw new FixCheckException(message + ": " + uae.Message,uae);
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            //the source was not present anyway, so do nothing
        }
        catch (System.IO.PathTooLongException ptle)
        {
            throw new FixCheckException(message + ": " + ptle.Message,ptle);
        }
        catch (System.IO.IOException ex)
        {
            throw new FixCheckException(message + ": " + ex.Message, ex);
        }
    }

    protected override string MovieCheckName => "Use custom folder name format";
}
