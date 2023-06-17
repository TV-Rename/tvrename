using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alphaleonis.Win32.Filesystem;

namespace TVRename;

internal class RemoveShowsWithNoFolders : PostScanActivity
{
    public RemoveShowsWithNoFolders(TVDoc doc) : base(doc)
    {
    }

    public override string ActivityName() => "Clean up shows with no folders that exist";

    protected override bool Active() => true;
    protected override void DoCheck(PostScanProgressDelegate progress, CancellationToken token)
    {
        List<ShowConfiguration> libraryShows = MDoc.TvLibrary.GetSortedShowItems();
        List<MovieConfiguration> movieConfigurations = MDoc.FilmLibrary.GetSortedMovies();

        int totalRecords = libraryShows.Count + movieConfigurations.Count;
        int n = 0;
        string lastUpdate = string.Empty;

        foreach (ShowConfiguration si in libraryShows.Where(HasAiredEpisode))
        {
            progress(n++, totalRecords, si.ShowName, lastUpdate);

            if (token.IsCancellationRequested)
            {
                return;
            }

            bool removeThisShow = si.AllProposedFolderLocations().Any()
                                  && si.AllProposedFolderLocations()
                                       .SelectMany(folderLocation => folderLocation.Value)
                                       .All(NotExist);

            if (removeThisShow && si.AutoAddFolderBase.HasValue() && Directory.Exists(si.AutoAddFolderBase))
            {
                removeThisShow = false;
            }

            if (removeThisShow)
            {
                lastUpdate = $"{si.Name} has no folders that exist, removing";
                MDoc.TheActionList.Add(new RemoveShow(si,MDoc));
            }
        }
    
        foreach (MovieConfiguration si in movieConfigurations.Where(IsReleased))
        {
            progress(n++, totalRecords, si.ShowName, lastUpdate);

            if (token.IsCancellationRequested)
            {
                return;
            }

            bool removeThisShow = si.Locations.All(NotExist);

            if (removeThisShow)
            {
                lastUpdate = $"{si.Name} has no folders that exist, removing";
                MDoc.TheActionList.Add(new RemoveMovie(si, MDoc));
            }
        }
    }

    private bool IsReleased(MovieConfiguration mc) => mc.CachedMovie?.IsReleased() ?? false;

    private bool HasAiredEpisode(ShowConfiguration sc) => sc.GetFirstAvailableEpisode()?.HasAired() ?? false;

    private bool NotExist(string folderName) => !Directory.Exists(folderName);
}
