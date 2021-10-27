using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    internal class MissingMovieCsv : ActionListExporter
    {
        public MissingMovieCsv(ItemList theActionList) : base(theActionList)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportMissingMoviesCSV;
        [NotNull]
        protected override string Name() => "Missing Movie CSV Exporter";

        protected override string Location() => TVSettings.Instance.ExportMissingMoviesCSVTo;

        public override bool ApplicableFor(TVSettings.ScanType st) => st == TVSettings.ScanType.Full;

        protected override void Do()
        {
            using (System.IO.StreamWriter file = new(Location()))
            {
                file.WriteLine("Movie Name,Year,Folder,Nice Name");

                foreach (MovieItemMissing im in TheActionList.MissingMovies.ToList())
                {
                    MovieConfiguration pe = im.MovieConfig;
                    file.WriteLine(
                        $"\"{pe.ShowName}\",{pe.CachedMovie?.Year},\"{im.TargetFolder}\",\"{im.Filename}\"");
                }
            }
        }
    }
}
