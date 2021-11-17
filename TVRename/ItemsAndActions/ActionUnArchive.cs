//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.Linq;

namespace TVRename
{
    public class ActionUnArchive : ActionFileOperation
    {
        private FileInfo archiveFile;
        private readonly MediaConfiguration show; // if for an entire show, rather than specific episode

        public override string SeriesName => show.ShowName;
        public override string SeasonNumber => string.Empty;

        public ActionUnArchive(FileInfo fi, ShowConfiguration showConfiguration)
        {
            this.archiveFile = fi;
            this.show = showConfiguration;
        }

        public ActionUnArchive(FileInfo fi, MovieConfiguration movieConfiguration)
        {
            this.archiveFile = fi;
            this.Movie = movieConfiguration;
            this.show = movieConfiguration;
        }

        public override string ProgressText => archiveFile.Name;

        public override long SizeOfWork => archiveFile.Length;

        public override string Produces => archiveFile.FullName;

        public override string TargetFolder => archiveFile.DirectoryName;

        public override string ScanListViewGroup => "lvgActionUnpack";

        public override int IconNumber => 11; 
        public override IgnoreItem Ignore => null;

        public override string Name => "Unpack";

        public override string DestinationFolder => TargetFolder;

        public override string DestinationFile => archiveFile.Name;

        public override int CompareTo(Item o)
        {
            ActionUnArchive nfo = o as ActionUnArchive;

            if (nfo?.archiveFile is null)
            {
                return -1;
            }

            return string.Compare(archiveFile.FullName, nfo.archiveFile.FullName, StringComparison.Ordinal);
        }

        public override ActionOutcome Go(TVRenameStats stats)
        {
            if (archiveFile is null)
            {
                return ActionOutcome.CompleteFail();
            }
            Directory.CreateDirectory(archiveFile.FileFullNameNoExt());

            using (IArchive archive = GetArchive(archiveFile))
            {
                foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(archiveFile.FileFullNameNoExt(), new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true,
                    });
                }
            }
            DeleteOrRecycleFile(archiveFile);
            return ActionOutcome.Success();
        }
        private IArchive GetArchive(FileInfo archive)
        {
            if (archiveFile.Name.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
            {
                return RarArchive.Open(archiveFile.FullName);
            }
            if (archiveFile.Name.EndsWith(".7z", StringComparison.OrdinalIgnoreCase))
            {
                return SevenZipArchive.Open(archiveFile.FullName);
            }
            if (archiveFile.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                return ZipArchive.Open(archiveFile.FullName);
            }
            if (archiveFile.Name.EndsWith(".gzip", StringComparison.OrdinalIgnoreCase))
            {
                return GZipArchive.Open(archiveFile.FullName);
            }
            return TarArchive.Open(archiveFile.FullName);
        }

        public override bool SameAs(Item o)
        {
            return o is ActionUnArchive touch && touch.archiveFile == archiveFile;
        }
    }
}
