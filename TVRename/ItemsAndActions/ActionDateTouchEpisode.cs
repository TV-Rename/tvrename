using Alphaleonis.Win32.Filesystem;
using System;

namespace TVRename
{
    internal class ActionDateTouchEpisode : ActionDateTouchFile
    {
        public ActionDateTouchEpisode(FileInfo f, ProcessedEpisode pe, DateTime date) : base(f, date)
        {
            Episode = pe;
        }

        public override bool SameAs(Item o)
        {
            return o is ActionDateTouchEpisode touch && touch.WhereFile == WhereFile;
        }

        public override int CompareTo(Item? o)
        {
            if (o is not ActionDateTouchEpisode nfo)
            {
                return -1;
            }

            if (Episode is null)
            {
                return 1;
            }

            if (nfo.Episode is null)
            {
                return -1;
            }

            return string.Compare(WhereFile.FullName + Episode.Name, nfo.WhereFile.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
    }
}
