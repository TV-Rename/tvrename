using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal abstract class FindMissingEpisodes : ScanActivity
{
    private readonly List<Finder> finders;

    protected FindMissingEpisodes(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
        finders =
        //These should be in order
        [
            new LibraryFolderFileFinder(doc,settings),
            new SearchFolderFileFinder(doc,settings),
            new uTorrentFinder(doc,settings),
            new qBitTorrentFinder(doc,settings),
            new SABnzbdFinder(doc,settings),
            new RSSFinder(doc,settings), //RSS Finder Should Be last as it is the finder if all others fail
            new JSONWebpageFinder(doc,settings), //Except for JSON which is dead last
            new JackettFinder(doc,settings)
        ];
    }

    protected abstract Finder.FinderDisplayType CurrentType();

    protected override void DoCheck(SetProgressDelegate progress)
    {
        // have a look around for any missing episodes
        List<Finder> appropriateFinders = finders.Where(f => f.DisplayType() == CurrentType() && f.Active()).ToList();
        int currentMatchingFinderId = 0;
        int totalMatchingFinders = appropriateFinders.Count;

        foreach (Finder f in appropriateFinders)
        {
            if (Settings.Token.IsCancellationRequested)
            {
                return;
            }

            if (!MDoc.TheActionList.Missing.Any())
            {
                continue;
            }

            f.ActionList = MDoc.TheActionList;

            currentMatchingFinderId++;
            int startPos = 100 * (currentMatchingFinderId - 1) / totalMatchingFinders;
            int endPos = 100 * currentMatchingFinderId / totalMatchingFinders;
            f.Check(progress, startPos, endPos);
        }
    }

    public override bool Active() => TVSettings.Instance.MissingCheck && finders.Any(f => f.DisplayType() == CurrentType() && f.Active());
}
