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
using System.Linq;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal abstract class RecentExporter : Exporter
    {
        private readonly TVDoc doc;

        protected RecentExporter(TVDoc doc)
        {
            this.doc = doc;
        }

        public override void Run()
        {
            if (Active())
            {
                if (string.IsNullOrWhiteSpace(Location()))
                {
                    LOGGER.Warn("Please open settings and update Export Recent Shows Filename");
                    return;
                }

                try
                {
                    //Create the directory if needed
                    Directory.CreateDirectory(Path.GetDirectoryName(Location()) ?? "");

                    List<ProcessedEpisode> lpe = doc.Library.RecentEpisodes(TVSettings.Instance.WTWRecentDays);
                    DirFilesCache dfc = new DirFilesCache();

                    //Write Contents to file
                    using (StreamWriter file = new StreamWriter(Location()))
                    {
                        file.WriteLine(GenerateHeader());
                        foreach (ProcessedEpisode episode in lpe)
                        {
                            List<FileInfo> files =  dfc.FindEpOnDisk(episode, false);

                            if (!files.Any())
                            {
                                continue;
                            }

                            string name = TVSettings.Instance.NamingStyle.NameFor(episode);
                            int length = files.First().GetFilmLength();

                            file.WriteLine(GenerateRecord(episode, files.First(), name,length));
                        }
                        file.WriteLine(GenerateFooter());
                    }

                    LOGGER.Info("Output File to: {0}", Location());
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

        protected abstract string GenerateHeader();
        protected abstract string GenerateRecord(ProcessedEpisode ep, FileInfo file, string name, int length);
        protected abstract string GenerateFooter();
    }
}
