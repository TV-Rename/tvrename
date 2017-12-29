using System;
using System.Collections.Generic;

namespace TVRename
{
    class ShowsTxt : ShowsExporter
    {
        public override bool Active() =>TVSettings.Instance.ExportShowsTxt;
        public override string Location() =>TVSettings.Instance.ExportShowsTxtTo;

        public override void Run(List<ShowItem> shows)
        {
            if (TVSettings.Instance.ExportShowsTxt )
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
                    {
                        foreach (ShowItem si in shows)
                        {
                            file.WriteLine(si.ShowName);
                        }

                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
