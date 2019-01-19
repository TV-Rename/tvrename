// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.IO;

namespace TVRename
{
    internal abstract class UpcomingExporter : Exporter
    {
        protected readonly TVDoc Doc;

        protected UpcomingExporter(TVDoc doc)
        {
            Doc = doc;
        }

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
                    List<ProcessedEpisode> lpe = Doc.Library.NextNShows(TVSettings.Instance.ExportRSSMaxShows,
                        TVSettings.Instance.ExportRSSDaysPast, TVSettings.Instance.ExportRSSMaxDays);

                    if (lpe != null)
                        if (Generate(ms, lpe))
                        {
                            return System.Text.Encoding.ASCII.GetString(ms.ToArray());
                        }
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Failed to produce records to put into Export file at: {0}", Location());
            }

            return "";
        }

        public override void Run()
        {
            if (Active())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Location()))
                    {
                        LOGGER.Warn("Please open settings and ensure filenames are provided for each exporter you have enabled");
                        return;
                    }

                    //Create the directory if needed
                    Directory.CreateDirectory(Path.GetDirectoryName(Location()) ?? "");
                    string contents = Produce();

                    //Write Contents to file
                    using (StreamWriter file = new StreamWriter(Location()))
                    {
                        file.Write(contents);
                    }

                    LOGGER.Info("Output File to: {0}", Location());
                    LOGGER.Trace("contents of File are: {0}", contents);
                }
                catch (NotSupportedException e)
                {
                    LOGGER.Warn(e, "Output File must be a local file: {0}", Location());
                }
                catch (Exception e)
                {
                    LOGGER.Error(e, "Failed to Output File to: {0}", Location());
                }
            }
            else
            {
                LOGGER.Trace("Skipped (Disabled) Output File to: {0}", Location());
            }
        }

        protected abstract bool Generate(Stream str, List<ProcessedEpisode> elist);
    }
}
