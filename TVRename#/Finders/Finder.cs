namespace TVRename
{
    abstract class Finder
    {
        protected ItemList TheActionList;
        protected bool ActionCancel;
        protected TVDoc MDoc;

        public Finder(TVDoc doc)
        {
            MDoc = doc;
        }

        public enum FinderDisplayType { Local, Downloading, Rss}

        public abstract void Check(SetProgressDelegate prog, int startpct, int totPct);
        
        public abstract bool Active();

        public abstract FinderDisplayType DisplayType();

        public void SetActionList(ItemList actionList) { TheActionList = actionList; }

        public void Interrupt() { ActionCancel = true; }
        public void Reset() { ActionCancel = false; }

    }
}
