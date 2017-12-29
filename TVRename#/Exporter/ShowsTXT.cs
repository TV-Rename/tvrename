using System;
using System.Collections.Generic;

namespace TVRename
{
    class ShowsTXT : ShowsExporter
    {
        public override bool Active() =>TVSettings.Instance.ExportShowsTXT;
        public override string Location() =>TVSettings.Instance.ExportShowsTXTTo;

        public override void Run(List<ShowItem> shows)
        {
            if (TVSettings.Instance.ExportShowsTXT )
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
                    logger.Error(e);
                }
            }
        }
    }
}
