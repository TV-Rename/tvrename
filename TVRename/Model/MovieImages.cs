using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public class MovieImages : SafeList<MovieImage>
    {
        public void MergeImages([NotNull] MovieImages images)
        {
            if (!this.Any())
            {
                Clear();
                AddRange(images);
                return;
            }
            foreach (MovieImage i in images)
            {
                if (this.All(si => si.Id != i.Id))
                {
                    Add(i);
                }
            }
        }
    }
}
