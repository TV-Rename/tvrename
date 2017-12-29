using System;
using System.Collections.Generic;
using System.IO;

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
                    using (StreamWriter file = new StreamWriter(Location()))
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
