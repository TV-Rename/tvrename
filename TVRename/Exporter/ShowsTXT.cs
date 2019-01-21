// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class ShowsTXT : ShowsExporter
    {
        public ShowsTXT(List<ShowItem> shows) : base(shows)
        {
        }

        public override bool Active() =>TVSettings.Instance.ExportShowsTXT;
        protected override string Location() =>TVSettings.Instance.ExportShowsTXTTo;

        protected override void Do()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
            {
                foreach (ShowItem si in Shows)
                {
                    file.WriteLine(si.ShowName);
                }
            }
        }
    }
}
