//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TVRename
{
    internal abstract class UpcomingExporter : Exporter
    {
        private readonly TVDoc doc;

        protected UpcomingExporter(TVDoc doc)
        {
            this.doc = doc;
        }

        [NotNull]
        private string Produce()
        {
            try
            {
                // dirty try/catch to "fix" the problem that a share can disappear during a sleep/resume, and
                // when windows restarts, the share isn't "back" before this timer times out and fires
                // windows explorer tends to lose explorer windows on shares when slept/resumed, too, so its not
                // just me :P

                using (MemoryStream ms = new MemoryStream())
                {
                    List<ProcessedEpisode> lpe = doc.TvLibrary.NextNShows(TVSettings.Instance.ExportRSSMaxShows,
                        TVSettings.Instance.ExportRSSDaysPast, TVSettings.Instance.ExportRSSMaxDays);

                    if (Generate(ms, lpe))
                    {
                        return Encoding.ASCII.GetString(ms.ToArray());
                    }
                }

                LOGGER.Error("Failed to generate records to put into Export file at: {0}", Location());
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Failed to produce records to put into Export file at: {0}", Location());
            }

            return string.Empty;
        }

        protected override void Do()
        {
            string contents = Produce();

            //Write Contents to file
            using (StreamWriter file = new StreamWriter(Location()))
            {
                file.Write(contents);
            }

            LOGGER.Trace("contents of File are: {0}", contents);
        }

        protected abstract bool Generate(Stream str, IEnumerable<ProcessedEpisode> elist);
    }
}
