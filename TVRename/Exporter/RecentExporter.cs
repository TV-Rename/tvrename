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
using System.IO.Packaging;
using System.Linq;
using System.Text;
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
                            List<FileInfo> files =  TVDoc.FindEpOnDisk(dfc, episode, false);

                            if (!files.Any()) continue;

                            string name = TVSettings.Instance.NamingStyle.NameFor(episode);
                            string image = "file://familyshare/tv/comedy/Barry/Season%2001/Folder.jpg";
                            int length = files.First().GetFilmLength();

                            file.WriteLine(GenerateRecord(episode, files.First(), name,image,length));
                        }
                        file.WriteLine(GenerateFooter());
                    }

                    Logger.Info("Output File to: {0}", Location());
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Failed to Output File to: {0}", Location());
                }
            }
            else
            {
                Logger.Trace("Skipped (Disabled) Output File to: {0}", Location());
            }
        }

        protected abstract string GenerateHeader();
        protected abstract string GenerateRecord(ProcessedEpisode ep, FileInfo file, string name, string image, int length);
        protected abstract string GenerateFooter();
    }
}
