using Alphaleonis.Win32.Filesystem;
using System.Linq;

namespace TVRename;

internal class MovieCheckEmptyManualFolders(MovieConfiguration movie, TVDoc doc) : MovieCheck(movie, doc)
{
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

        return Movie.ManualLocations.Any(FileHelper.DirectoryIsMissingEmpty);
    }


    public override string Explain() => $"{Movie.Name} has manual folders set, these folders are missing or empty: {Movie.ManualLocations.Where(FileHelper.DirectoryIsMissingEmpty).ToCsv()}";

    protected override void FixInternal()
    {
        foreach (string directory in Movie.ManualLocations.Where(FileHelper.DirectoryIsMissingEmpty).ToList())
        {
            Movie.ManualLocations.Remove(directory);
            FileHelper.RemoveEmptyDirectory(directory);
        }

        if (Movie.ManualLocations.Count == 0)
        {
            Movie.UseManualLocations = false;
        }
    }



    protected override string MovieCheckName => "Movie has missing or empty manual folder";
}
