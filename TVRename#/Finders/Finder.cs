namespace TVRename
{
    internal abstract class Finder
    {
        protected bool ActionCancel = false;
        protected readonly TVDoc Doc;

        public ItemList ActionList { get; internal set; }

        protected Finder(TVDoc doc)
        {
            this.Doc = doc;
        }

        // ReSharper disable once InconsistentNaming
        public enum FinderDisplayType { Local, Downloading, RSS};

        public abstract void Check(SetProgressDelegate prog, int startpct, int totPct);
        
        public abstract bool Active();

        public abstract FinderDisplayType DisplayType();

        public void Interrupt() {
            this.ActionCancel = true; }
        public void Reset() {
            this.ActionCancel = false; }

    }
}
