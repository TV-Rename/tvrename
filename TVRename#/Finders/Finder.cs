namespace TVRename
{
    internal abstract class Finder
    {
        protected ItemList TheActionList;
        protected bool ActionCancel = false;
        protected readonly TVDoc Doc;

        protected Finder(TVDoc doc)
        {
            this.Doc = doc;
        }

        // ReSharper disable once InconsistentNaming
        public enum FinderDisplayType { Local, Downloading, RSS};

        public abstract void Check(SetProgressDelegate prog, int startpct, int totPct);
        
        public abstract bool Active();

        public abstract FinderDisplayType DisplayType();

        public void SetActionList(ItemList actionList) { this.TheActionList = actionList; }

        public void Interrupt() {
            this.ActionCancel = true; }
        public void Reset() {
            this.ActionCancel = false; }

    }
}
