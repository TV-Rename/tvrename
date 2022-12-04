using Alphaleonis.Win32.Filesystem;
using System.Linq;

namespace TVRename;

internal class MovieCheckEmptyManualFolders : MovieCheck
{
    public MovieCheckEmptyManualFolders(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
    {
    }

    public override bool Check()
    {
        if (!Movie.UseManualLocations)
        {
            return false;
        }

        if (Movie.ManualLocations.Count == 0)
        {
            return false;
        }

        return Movie.ManualLocations.Any(DirectoryIsMissingEmpty);
    }

    private static bool DirectoryIsMissingEmpty(string path) => !DirectoryHasContents(path);

    private static bool DirectoryHasContents(string path)
    {
        return path.HasValue()
               && Directory.Exists(path)
               && Directory.EnumerateFileSystemEntries(path).Any();
    }

    public override string Explain() => $"{Movie.Name} has manual folders set, these folders are missing or empty: {Movie.ManualLocations.Where(DirectoryIsMissingEmpty).ToCsv()}";

    protected override void FixInternal()
    {
        foreach (string directory in Movie.ManualLocations.Where(DirectoryIsMissingEmpty).ToList())
        {
            Movie.ManualLocations.Remove(directory);
            RemoveEmptyDirectory(directory);
        }

        if (!Movie.ManualLocations.Any())
        {
            Movie.UseManualLocations = false;
        }
    }

    private static void RemoveEmptyDirectory(string directory)
    {
        if (!directory.HasValue() || !DirectoryIsMissingEmpty(directory))
        {
            return;
        }

        try
        {
            Directory.Delete(directory); //TODO Should use a safer version in FileHelper
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            //Suppressed - we want it to be removed anyway
        }
    }

    protected override string MovieCheckName => "Movie has missing or empty manual folder";
}
