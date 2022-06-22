namespace TVRename
{
    internal class TvShowEpisodeNameCheck : TvShowCheck
    {
        public TvShowEpisodeNameCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        public override bool Check() => Show.UseCustomNamingFormat;

        public override string Explain() => $"TV Show does not use the standard episode naming format {TVSettings.Instance.NamingStyle.StyleString}, it uses {Show.CustomNamingFormat}";

        protected override void FixInternal()
        {
            Show.UseCustomNamingFormat = false;
        }

        public override string CheckName => "[TV] Use Custom Folder Name Format";
    }
}
