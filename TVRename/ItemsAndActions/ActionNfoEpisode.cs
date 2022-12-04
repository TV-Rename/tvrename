using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Alphaleonis.Win32.Filesystem;
using TVRename.Forms;

namespace TVRename;

internal class ActionNfoEpisode : ActionNfo
{
    public ActionNfoEpisode(FileInfo nfo, ProcessedEpisode pe) : base(nfo, pe.Show)
    {
        Episode = pe;
    }

    public override string Name => "Write KODI Metadata (Episode)";

    protected override long? UpdateTime() => Episode?.SrvLastUpdated;

    protected override string RootName() => "episodedetails";

    private ActionOutcome ReplaceMultipartFile()
    {
        ShowConfiguration? si = Episode?.Show ?? SelectedShow;

        //We will replace the file as too difficult to update multiparts
        //We can't use XDocument as it's not fully valid XML
        List<XElement> episodeXmLs = new();

        if (Episode != null)
        {
            foreach (Episode ep in Episode.SourceEpisodes)
            {
                XElement epNode = new("episodedetails");
                UpdateEpisodeFields(ep, si, epNode, true);
                episodeXmLs.Add(epNode);
            }
        }

        try
        {
            using System.IO.StreamWriter writer = File.CreateText(Where.FullName);
            foreach (XElement ep in episodeXmLs)
            {
                writer?.WriteLine(ep);
            }
        }
        catch (System.IO.IOException e)
        {
            return new ActionOutcome(e);
        }

        return ActionOutcome.Success();
    }

    private static void UpdateEpisodeFields(Episode episode, ShowConfiguration? show, XElement root, bool isMultiPart)
    {
        root.UpdateElement("title", episode.Name, true);
        root.UpdateElement("id", episode.EpisodeId, true);
        root.UpdateElement("plot", episode.Overview, true);
        root.ReplaceElements("studio", episode.TheCachedSeries.Networks);

        UpdateId(root, "tvdb", "true", episode.EpisodeId);
        UpdateId(root, "imdb", "false", episode.ImdbCode);

        string? showRating = episode.EpisodeRating;
        if (showRating != null)
        {
            UpdateRatings(root, showRating, episode.SiteRatingCount ?? 0);
        }

        if (show is not null)
        {
            root.UpdateElement("originaltitle", show.ShowName, true);
            root.UpdateElement("showtitle", show.ShowName, true);
            root.UpdateElement("sorttitle", UI.GenerateShowUiName(show));
            root.UpdateElement("season", episode.GetSeasonNumber(show.Order), true);
            root.UpdateElement("episode", episode.GetEpisodeNumber(show.Order), true);
            root.UpdateElement("mpaa", show.CachedShow?.ContentRating, true);

            //actor(s) and guest actor(s)
            CachedSeriesInfo? s = show.CachedShow;
            if (s != null)
            {
                ReplaceActors(root, episode.AllActors(s));
            }
        }

        if (episode.FirstAired.HasValue && episode.FirstAired.Value > DateTime.MinValue)
        {
            root.UpdateElement("aired", episode.FirstAired.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), true);
        }

        //Director(s)
        string? epDirector = episode.EpisodeDirector;
        if (!string.IsNullOrEmpty(epDirector))
        {
            IEnumerable<string> dirs = epDirector.FromPsv().ToArray();
            if (dirs.Any())
            {
                root.ReplaceElements("director", dirs);
            }
        }

        //Writers(s)
        string? epWriter = episode.Writer;
        if (!string.IsNullOrEmpty(epWriter))
        {
            string[] writers = epWriter.FromPsv().ToArray();
            if (writers.Any())
            {
                root.ReplaceElements("credits", writers);
            }
        }

        if (isMultiPart && show != null)
        {
            XElement resumeElement = root.GetOrCreateElement("resume");

            //we have to put 0 as we don't know where the multipart episode starts/ends
            resumeElement.UpdateElement("position", 0);
            resumeElement.UpdateElement("total", 0);

            //For now we only put art in for multipart episodes. Kodi finds the art appropriately
            //without our help for the others

            string filename = TVSettings.Instance.FilenameFriendly(show, episode);

            string thumbFilename = filename + ".jpg";
            UpdateAmongstElements(root, "thumb", thumbFilename);
            //Should be able to do this using the local filename, but only seems to work if you provide a URL
            //XMLHelper.WriteElementToXML(writer, "thumb", LocalCache.Instance.GetTVDBDownloadURL(episode.GetFilename()))
        }
    }

    protected override ActionOutcome UpdateFile()
    {
        //We will replace the file as too difficult to update multiparts
        //We can't use XDocument as it's not fully valid XML
        if (Episode is { Type: ProcessedEpisode.ProcessedEpisodeType.merged })
        {
            return ReplaceMultipartFile();
        }

        XDocument doc = XDocument.Load(Where.FullName);
        XElement? root = doc.Root;

        if (root is null)
        {
            return new ActionOutcome($"Could not load {Where.FullName}");
        }

        ShowConfiguration si = Episode!.Show;
        UpdateEpisodeFields(Episode, si, root, false);

        doc.Save(Where.FullName);
        return ActionOutcome.Success();
    }
}
