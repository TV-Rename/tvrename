namespace TVRename
{
    public abstract class Finder
    {
        protected bool ActionCancel;
        protected readonly TVDoc Doc;
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public ItemList ActionList { protected get; set; }

        protected Finder(TVDoc doc) => Doc = doc;

        // ReSharper disable once InconsistentNaming
        public enum FinderDisplayType { local, downloading, search}

        public abstract void Check(SetProgressDelegate prog, int startpct, int totPct);
        
        public abstract bool Active();

        public abstract FinderDisplayType DisplayType();

        public void Interrupt() => ActionCancel = true;
        public void Reset() => ActionCancel = false;
    }
}
