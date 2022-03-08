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
using JetBrains.Annotations;

namespace TVRename
{
    public class ActionUnArchive : ActionFileOperation
    {
        private readonly FileInfo archiveFile;
        private readonly MediaConfiguration show; // if for an entire show, rather than specific episode

        public override string SeriesName => show.ShowName;
        public override string SeasonNumber => string.Empty;
        public override int? SeasonNumberAsInt => null;

        public ActionUnArchive(FileInfo fi, ShowConfiguration showConfiguration)
        {
            archiveFile = fi;
            show = showConfiguration;
        }

        public ActionUnArchive(FileInfo fi, MovieConfiguration movieConfiguration)
        {
            archiveFile = fi;
            Movie = movieConfiguration;
            show = movieConfiguration;
        }

        public override string ProgressText => archiveFile.Name;

        public override long SizeOfWork => ArchiveFileSize();

        private long ArchiveFileSize()
        {
            try
            {
                return archiveFile.Length;
            }
            catch
            {
                return 1;
            }
        }

        public override string Produces => archiveFile.FullName;

        public override string TargetFolder => archiveFile.DirectoryName;

        [NotNull]
        public override string ScanListViewGroup => "lvgActionUnpack";

        public override int IconNumber => 11;
        public override IgnoreItem? Ignore => null;

        [NotNull]
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

        [NotNull]
        public override ActionOutcome Go(TVRenameStats stats)
        {
            if (archiveFile is null)
            {
                return ActionOutcome.CompleteFail();
            }

            try
            {
                Directory.CreateDirectory(archiveFile.FileFullNameNoExt());

                using (IArchive archive = GetArchive(archiveFile))
                {
                    foreach (IArchiveEntry entry in archive.Entries)
                    {
                        entry.WriteToDirectory(archiveFile.FileFullNameNoExt(), new ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true,
                        });
                    }
                }

                DeleteOrRecycleFile(archiveFile);
                return ActionOutcome.Success();
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                return new ActionOutcome(e);
            }
            catch (System.IO.IOException e)
            {
                return new ActionOutcome(e);
            }
            catch (InvalidFormatException e)
            {
                return new ActionOutcome(e);
            }
            catch (ArgumentException e)
            {
                return new ActionOutcome(e);
            }
            catch (IncompleteArchiveException e)
            {
                return new ActionOutcome(e);
            }
        }
        private static IArchive GetArchive([NotNull] FileInfo archive)
        {
            if (archive.Name.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
            {
                return RarArchive.Open(archive.FullName);
            }
            if (archive.Name.EndsWith(".7z", StringComparison.OrdinalIgnoreCase))
            {
                return SevenZipArchive.Open(archive.FullName);
            }
            if (archive.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                return ZipArchive.Open(archive.FullName);
            }
            if (archive.Name.EndsWith(".gzip", StringComparison.OrdinalIgnoreCase))
            {
                return GZipArchive.Open(archive.FullName);
            }
            return TarArchive.Open(archive.FullName);
        }

        public override bool SameAs(Item o) => o is ActionUnArchive touch && touch.archiveFile == archiveFile;
    }
}
