using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename
{
    class ShowsTXT : ShowsExporter
    {
        public override bool Active()
        {
            return TVSettings.Instance.ExportShowsTXT;
        }
        public override string Location()
        {
            return TVSettings.Instance.ExportShowsTXTTo;
        }

        public override void Run(List<ShowItem> shows)
        {
            if (TVSettings.Instance.ExportShowsTXT )
            {

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
                {
                    foreach (ShowItem si in shows)
                    {
                        file.WriteLine(si.ShowName);
                    }

                }

            }
        }
    }
}
