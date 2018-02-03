using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using TVRename.Core.Metadata;
using TVRename.Core.Models;
using TVRename.Core.Utility;
using Newtonsoft.Json;
using TVRename.Core.Metadata.Identifiers;
using TVRename.Windows.Models;

namespace TVRename.Windows.Configuration
{
    /// <summary>
    /// Stores and represents application settings.
    /// See <see cref="JsonSettings{T}"/>.
    /// </summary>
    /// <seealso cref="JsonSettings{Settings}" />
    /// <inheritdoc />
    public class Settings : JsonSettings<Settings>
    {
        [JsonIgnore]
        internal bool Dirty { get; set; }

        public string SeasonTemplate { get; set; } = "Season {{number | pad}}";

        public string SpecialsTemplate { get; set; } = "Specials";

        public string EpisodeTemplate { get; set; } = "{{show.name}} - S{{season.number | pad}}E{{episode.number | pad}} - {{episode.name}}";

        public Language Language { get; set; } = new Language
        {
            Id = 7,
            Abbreviation = "en"
        };

        public bool AutoCreateFolders { get; set; } = false;

        public int DownloadThreads { get; set; } = 8;

        public int RecentDays { get; set; } = 7;

        public string DefaultLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        public List<string> SearchDirectories { get; set; } = new List<string>();

        public Dictionary<char, string> FilenameReplacements { get; set; } = new Dictionary<char, string>();

        public List<string> VideoFileExtensions { get; set; } = new List<string>();

        public List<FilenameProcessor> FilenameProcessors { get; set; } = new List<FilenameProcessor>();

        public List<Identifier> Identifiers { get; set; } = new List<Identifier>();

        public List<Show> Shows { get; set; } = new List<Show>();

        public override Settings Initialize()
        {
            return new Settings
            {
                SearchDirectories = new List<string>
                {
                    @"D:\Downloads\Video"
                },

                FilenameReplacements = new Dictionary<char, string>
                {
                    {':', "-"},
                    {'"', "'"},
                    {'*', "#"},
                    {'?', ""},
                    {'>', ""},
                    {'<', ""},
                    {'/', "-"},
                    {'\\', "-"},
                    {'|', "-"}
                },

                VideoFileExtensions = new List<string>
                {
                    "avi",
                    "mkv",
                    "mp4"
                },

                FilenameProcessors = new List<FilenameProcessor>
                {
                    new FilenameProcessor
                    {
                        Enabled = true,
                        Pattern = "(^|[^a-z])s?(?<s>[0-9]+)[ex](?<e>[0-9]{2,})(e[0-9]{2,})*[^a-z]",
                        Notes = "3x23 s3x23 3e23 s3e23 s04e01e02e03",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = false,
                        Pattern = "(^|[^a-z])s?(?<s>[0-9]+)(?<e>[0-9]{2,})[^a-z]",
                        Notes = "323 or s323 for season 3, episode 23. 2004 for season 20, episode 4.",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = false,
                        Pattern = "(^|[^a-z])s(?<s>[0-9]+)--e(?<e>[0-9]{2,})[^a-z]",
                        Notes = "S02--E03",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = false,
                        Pattern = "(^|[^a-z])s(?<s>[0-9]+) e(?<e>[0-9]{2,})[^a-z]",
                        Notes = "'S02.E04' and 'S02 E04'",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = false,
                        Pattern = "^(?<s>[0-9]+) (?<e>[0-9]{2,})",
                        Notes = "filenames starting with '1.23' for season 1, episode 23",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = true,
                        Pattern = "(^|[^a-z])(?<s>[0-9])(?<e>[0-9]{2,})[^a-z]",
                        Notes = "Show - 323 - Foo",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = true,
                        Pattern = "(^|[^a-z])se(?<s>[0-9]+)([ex]|ep|xep)?(?<e>[0-9]+)[^a-z]",
                        Notes = "se3e23 se323 se1ep1 se01xep01...",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = true,
                        Pattern = "(^|[^a-z])(?<s>[0-9]+)-(?<e>[0-9]{2,})[^a-z]",
                        Notes = "3-23 EpName",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = true,
                        Pattern = "(^|[^a-z])s(?<s>[0-9]+) +- +e(?<e>[0-9]{2,})[^a-z]",
                        Notes = "ShowName - S01 - E01",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = true,
                        Pattern = "\\b(?<e>[0-9]{2,}) ?- ?.* ?- ?(?<s>[0-9]+)",
                        Notes = "like '13 - Showname - 2 - Episode Title.avi'",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = true,
                        Pattern = "\\b(episode|ep|e) ?(?<e>[0-9]{2,}) ?- ?(series|season) ?(?<s>[0-9]+)",
                        Notes = "episode 3 - season 4",
                        UseFullPath = false
                    },
                    new FilenameProcessor
                    {
                        Enabled = true,
                        Pattern = "season (?<s>[0-9]+)\\\\e?(?<e>[0-9]{1,3}) ?-",
                        Notes = "Show Season 3\\E23 - Epname",
                        UseFullPath = true
                    },
                    new FilenameProcessor
                    {
                        Enabled = false,
                        Pattern = "season (?<s>[0-9]+)\\\\episode (?<e>[0-9]{1,3})",
                        Notes = "Season 3\\Episode 23",
                        UseFullPath = true
                    }
                },

                Identifiers = new List<Identifier>
                {
                    new KodiIdentifier
                    {
                        Target = Target.Show,
                        Location = "{{show.location}}",
                        FileName = "tvshow.nfo"
                    },
                    new TVDBImageIdentifier
                    {
                        ImageType = ImageType.ShowPoster,
                        ImageFormat = ImageFormat.Jpeg,
                        Location = "{{show.location}}",
                        FileName = "poster.jpg"
                    },
                    new TVDBImageIdentifier
                    {
                        ImageType = ImageType.ShowBanner,
                        ImageFormat = ImageFormat.Jpeg,
                        Location = "{{show.location}}",
                        FileName = "banner.jpg"
                    },
                    new TVDBImageIdentifier
                    {
                        ImageType = ImageType.ShowFanart,
                        ImageFormat = ImageFormat.Jpeg,
                        Location = "{{show.location}}",
                        FileName = "fanart.jpg"
                    },
                    new TVDBImageIdentifier
                    {
                        ImageType = ImageType.SeasonPoster,
                        ImageFormat = ImageFormat.Jpeg,
                        Location = "{{show.location}}",
                        FileName = "season{{season.number | pad}}-poster.jpg"
                    },
                    new TVDBImageIdentifier
                    {
                        ImageType = ImageType.SeasonBanner,
                        ImageFormat = ImageFormat.Jpeg,
                        Location = "{{show.location}}",
                        FileName = "season{{season.number | pad}}-banner.jpg"
                    },
                    new KodiIdentifier
                    {
                        Target = Target.Episode,
                        Location = "{{episode.location}}",
                        FileName = "{{episode.filename}}.nfo"
                    },
                    new TVDBImageIdentifier
                    {
                        ImageType = ImageType.EpisodeThumbnail,
                        ImageFormat = ImageFormat.Jpeg,
                        Location = "{{episode.location}}",
                        FileName = "{{episode.filename}}-thumb.jpg"
                    },
                    new Mede8erIdentifier
                    {
                        Target = Target.Show,
                        Location = "{{show.location}}",
                        FileName = "series.xml"
                    },
                    new Mede8erViewIdentifier
                    {
                        Target = Target.Show,
                        Location = "{{show.location}}",
                        FileName = "view.xml"
                    },
                    new Mede8erViewIdentifier
                    {
                        Target = Target.Season,
                        Location = "{{season.location}}",
                        FileName = "view.xml"
                    },
                    new Mede8erIdentifier
                    {
                        Target = Target.Episode,
                        Location = "{{episode.location}}",
                        FileName = "{{episode.filename}}.xml"
                    },
                    new PyTivoIdentifier
                    {
                        Target = Target.Episode,
                        Location = "{{episode.location}}",
                        FileName = "{{episode.filename}}.txt" // TODO: Must inc video file ext
                    }
                }
            };
        }
    }
}
