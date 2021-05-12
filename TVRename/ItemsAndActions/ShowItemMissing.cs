using System;
using JetBrains.Annotations;

namespace TVRename
{
    public class ShowItemMissing : ItemMissing
    {
        public ShowItemMissing([NotNull] ProcessedEpisode pe, [NotNull] string whereItShouldBeFolder)
        {
            Episode = pe;
            Filename = TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(pe, null, whereItShouldBeFolder.Length));
            TheFileNoExt = whereItShouldBeFolder + System.IO.Path.DirectorySeparatorChar + Filename;
            Folder = whereItShouldBeFolder;
        }
        #region Item Members

        public ProcessedEpisode MissingEpisode =>Episode ?? throw new InvalidOperationException();

        public override bool SameAs(Item o)
        {
            return o is ShowItemMissing missing && string.CompareOrdinal(missing.TheFileNoExt, TheFileNoExt) == 0;
        }
       
        public override string Name => "Missing Episode";

        public override int CompareTo(Item o)
        {
            if (!(o is ShowItemMissing miss))
            {
                return -1;
            }

            if (!MissingEpisode.Show.ShowName.Equals(miss.MissingEpisode.Show.ShowName))
            {
                return string.Compare(MissingEpisode.Show.ShowName, miss.MissingEpisode.Show.ShowName, StringComparison.Ordinal);
            }

            if (!MissingEpisode.AppropriateSeasonNumber.Equals(miss.MissingEpisode.AppropriateSeasonNumber))
            {
                return MissingEpisode.AppropriateSeasonNumber.CompareTo(miss.MissingEpisode.AppropriateSeasonNumber);
            }

            return MissingEpisode.AppropriateEpNum.CompareTo(miss.MissingEpisode.AppropriateEpNum);
        }

        #endregion

        public override bool DoRename => Episode?.Show.DoRename ?? true;

        public override MediaConfiguration Show => MissingEpisode.Show;
        public override string ToString() => $"{Show.ShowName} {MissingEpisode}";
    }
}