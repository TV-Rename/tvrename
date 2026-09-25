
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename;

internal class RemoveShowsWithNoFolders(TVDoc doc) : PostScanActivity(doc)
{
    public override string ActivityName() => "Clean up shows with no folders that exist";

    protected override bool Active() => true;
    protected override async Task DoCheckAsync(CancellationToken token)
    {
        List<ShowConfiguration> libraryShows = MDoc.TvLibrary.GetSortedShows();
        List<MovieConfiguration> movieConfigurations = MDoc.FilmLibrary.GetSortedMovies();

        int totalRecords = libraryShows.Count + movieConfigurations.Count;
        ThreadSafeCounter n = new();
        string lastUpdate = string.Empty;

        foreach (ShowConfiguration si in libraryShows.Where(HasAiredEpisode))
        {
            UpdateStatus(n.Increment(), totalRecords, si.ShowName, lastUpdate);

            if (token.IsCancellationRequested)
            {
                return;
            }

            var x = await si.AllProposedFolderLocationsAsync();

            bool removeThisShow = x.IsAny()
                                  && x
                                       .SelectMany(folderLocation => folderLocation.Value)
                                       .All(NotExist);

            if (removeThisShow && si.AutoAddFolderBase.HasValue() && Directory.Exists(si.AutoAddFolderBase))
            {
                removeThisShow = false;
            }

            if (removeThisShow)
            {
                lastUpdate = $"{si.Name} has no folders that exist, removing";
                MDoc.TheActionList.Add(new ActionChangeLibraryRemoveShow(si, MDoc));
            }
        }

        foreach (MovieConfiguration si in movieConfigurations.Where(IsReleased))
        {
            UpdateStatus(n.Increment(), totalRecords, si.ShowName, lastUpdate);

            if (token.IsCancellationRequested)
            {
                return;
            }

            bool removeThisShow = (await si.LocationsAsync()).All(NotExist);

            if (removeThisShow)
            {
                lastUpdate = $"{si.Name} has no folders that exist, removing";
                MDoc.TheActionList.Add(new ActionChangeLibraryRemoveMovie(si, MDoc));
            }
        }
    }

    private static bool IsReleased(MovieConfiguration mc) => mc.CachedMovie?.IsReleased() ?? false;

    private static bool HasAiredEpisode(ShowConfiguration sc) => sc.GetFirstAvailableEpisode()?.HasAired() ?? false;

    private static bool NotExist(string folderName) => !Directory.Exists(folderName);
}
