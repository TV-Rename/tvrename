//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;

namespace TVRename
{
    using Alphaleonis.Win32.Filesystem;
    using System;
    using System.Xml;

    // ReSharper disable once InconsistentNaming
    public class ActionMede8erViewXML : ActionWriteMetadata
    {
        private readonly int snum;

        public ActionMede8erViewXML(FileInfo nfo, ShowConfiguration si) : base(nfo, si)
        {
            snum = -1;
        }

        public ActionMede8erViewXML(FileInfo nfo, ShowConfiguration si, int snum) : base(nfo, si)
        {
            this.snum = snum;
        }

        #region Action Members

        [NotNull]
        public override string Name => "Write Mede8er View Data";

        [NotNull]
        public override ActionOutcome Go(TVRenameStats stats)
        {
            XmlWriterSettings settings = new()
            {
                Indent = true,
                NewLineOnAttributes = true
            };

            try
            {
                using (XmlWriter writer = XmlWriter.Create(Where.FullName, settings))
                {
                    writer.WriteStartElement("FolderTag");
                    writer.WriteElement("ViewMode", "Movie");
                    writer.WriteElement("ViewType", "Video");
                    writer.WriteEndElement();
                }
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }
            return ActionOutcome.Success();
        }

        #endregion Action Members

        #region Item Members

        public override bool SameAs(Item o)
        {
            return o is ActionMede8erViewXML xml && xml.Where == Where;
        }

        public override int CompareTo(Item o)
        {
            if (o is not ActionMede8erViewXML nfo)
            {
                return -1;
            }

            return string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);
        }

        #endregion Item Members

        #region Item Members

        public override string SeriesName => SelectedShow?.ShowName ?? Movie!.ShowName;
        public override string SeasonNumber => snum > 0 ? snum.ToString() : string.Empty;
        public override int? SeasonNumberAsInt => snum;
        public override string EpisodeString => string.Empty;
        public override string AirDateString => string.Empty;

        #endregion Item Members
    }
}
