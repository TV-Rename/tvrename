using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TVRename.Core.Actions;
using TVRename.Core.Models;
using TVRename.Core.Utility;

namespace TVRename.Core.Metadata
{
    public abstract class Identifier
    {
        [JsonIgnore]
        public abstract FileType FileType { get; }

        public abstract string Location { get; set; }

        public abstract string FileName { get; set; }

        public abstract Target Target { get; set; }

        [CanBeNull]
        protected abstract IAction ProcessShow(ProcessedShow show, FileInfo file, bool force = false);

        [CanBeNull]
        protected abstract IAction ProcessSeason(ProcessedShow show, ProcessedSeason season, FileInfo file, bool force = false);

        [CanBeNull]
        protected abstract IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false);

        [CanBeNull]
        public IAction ProcessShow(ProcessedShow show, bool force = false)
        {
            if (this.Target != Target.Show) return null;

            return ProcessShow(show, FileTemplate(new Dictionary<string, object>
            {
                {"show", show}
            }), force);
        }

        [CanBeNull]
        public IAction ProcessSeason(ProcessedShow show, ProcessedSeason season, bool force = false)
        {
            if (this.Target != Target.Season) return null;

            return ProcessSeason(show, season, FileTemplate(new Dictionary<string, object>
            {
                {"show", show},
                {"season", season}
            }), force);
        }

        [CanBeNull]
        public IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, bool force = false)
        {
            if (this.Target != Target.Episode) return null;

            return ProcessEpisode(show, season, episode, FileTemplate(new Dictionary<string, object>
            {
                {"show", show},
                {"season", season},
                {"episode", episode}
            }), force);
        }

        private FileInfo FileTemplate(Dictionary<string, object> vars)
        {
            string path = this.Location.Template(vars);
            string filename = this.FileName.Template(vars);

            return new FileInfo(Path.Combine(path, filename));
        }
    }
}
