using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    internal abstract class FindMissingEpisodes : ScanActivity
    {
        private readonly List<Finder> finders;

        protected FindMissingEpisodes(TVDoc doc) : base(doc)
        {
            finders = new List<Finder> //These should be in order
            {
                new LibraryFolderFileFinder(doc),
                new SearchFolderFileFinder(doc),
                new uTorrentFinder(doc),
                new qBitTorrentFinder(doc),
                new SABnzbdFinder(doc),
                new RSSFinder(doc), //RSS Finder Should Be last as it is the finder if all others fail
                new JSONWebpageFinder(doc), //Except for JSON which is dead last
                new JackettFinder(doc)
            };
        }

        protected abstract Finder.FinderDisplayType CurrentType();

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            // have a look around for any missing episodes
            List < Finder > appropriateFinders =  finders.Where(f => f.DisplayType() == CurrentType() && f.Active()).ToList();
            int currentMatchingFinderId = 0;
            int totalMatchingFinders = appropriateFinders.Count;

            foreach (Finder f in appropriateFinders)
            {
                if (settings.Token.IsCancellationRequested)
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
                f.Check(prog,startPos, endPos, showList, settings);

                MDoc.RemoveIgnored();
            }
        }

        public override bool Active() => TVSettings.Instance.MissingCheck && finders.Any(f => f.DisplayType() == CurrentType() && f.Active());
    }
}
