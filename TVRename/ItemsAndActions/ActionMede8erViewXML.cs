// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;
    using System.Xml;

    // ReSharper disable once InconsistentNaming
    public class ActionMede8erViewXML : ActionWriteMetadata
    {
        private readonly int snum;

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si) : base(nfo, si)
        {
            snum = -1;
        }

        public ActionMede8erViewXML(FileInfo nfo, ShowItem si, int snum) : base(nfo, si)
        {
            this.snum = snum;
        }

        #region Action Members

        public override string Name => "Write Mede8er View Data";

        public override ActionOutcome Go(TVRenameStats stats)
        {
            XmlWriterSettings settings = new XmlWriterSettings
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

        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return o is ActionMede8erViewXML xml && xml.Where == Where;
        }

        public override int CompareTo(object? o)
        {
            if (o is null || !(o is ActionMede8erViewXML nfo))
            {
                return -1;
            }

            return string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        public override string SeriesName => SelectedShow.ShowName;
        public override string SeasonNumber => snum > 0 ? snum.ToString() : string.Empty;
        public override string EpisodeString => string.Empty;
        public override string AirDateString => string.Empty;

        #endregion
    }
}
