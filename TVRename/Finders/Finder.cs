namespace TVRename
{
    internal abstract class Finder
    {
        protected bool ActionCancel = false;
        protected readonly TVDoc Doc;
        protected readonly static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ItemList ActionList { get; internal set; }

        protected Finder(TVDoc doc)
        {
            Doc = doc;
        }

        // ReSharper disable once InconsistentNaming
        public enum FinderDisplayType { Local, Downloading, RSS};

        public abstract void Check(SetProgressDelegate prog, int startpct, int totPct);
        
        public abstract bool Active();

        public abstract FinderDisplayType DisplayType();

        public void Interrupt() {
            ActionCancel = true; }
        public void Reset() {
            ActionCancel = false; }

    }
}
