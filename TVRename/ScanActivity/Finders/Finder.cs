namespace TVRename
{
    public abstract class Finder : ScanActivity
    {
        public ItemList ActionList { protected get; set; }

        protected Finder(TVDoc doc) : base(doc)
        {
            ActionList = MDoc.TheActionList;
        }

        // ReSharper disable once InconsistentNaming
        public enum FinderDisplayType { local, downloading, search }

        public abstract FinderDisplayType DisplayType();
    }
}