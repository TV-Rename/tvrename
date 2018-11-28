namespace TVRename
{
    public abstract class Finder :ScanActivity
    {
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public ItemList ActionList { protected get; set; }

        protected Finder(TVDoc doc) : base(doc){}

        // ReSharper disable once InconsistentNaming
        public enum FinderDisplayType { local, downloading, search}

        public abstract FinderDisplayType DisplayType();
    }
}
