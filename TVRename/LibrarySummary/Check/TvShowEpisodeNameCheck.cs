using System;
using JetBrains.Annotations;

namespace TVRename
{
    internal class TvShowEpisodeNameCheck : TvShowCheck
    {
        public TvShowEpisodeNameCheck([NotNull] ShowConfiguration show) : base(show)
        {
        }

        public override bool Check() => Show.UseCustomNamingFormat;

        public override string Explain() => $"TV Show does not use the standard episode naming format {TVSettings.Instance.NamingStyle.StyleString}, it uses {Show.CustomNamingFormat}";

        protected override void FixInternal()
        {
            //todo - move files
            throw new NotImplementedException();
        }
        public override string CheckName => "[TV] Use Custom Folder Name Format";
    }
}
