//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using TVRename.SAB;

namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class SABnzbdFinder : DownloadingFinder
{
    public SABnzbdFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
    }

    public override bool Active() => TVSettings.Instance.CheckSABnzbd;

    protected override string CheckName() => "Looked in the listed SABnz queue to see if the episode is already being downloaded";

    protected override void DoCheck(SetProgressDelegate progress)
    {
        if (string.IsNullOrEmpty(TVSettings.Instance.SABAPIKey) || string.IsNullOrEmpty(TVSettings.Instance.SABHostPort))
        {
            LOGGER.Warn("Searching SABnzbd Feeds is cancelled as the key and host/port are not provided in Preferences.");
            return;
        }

        // get list of files being downloaded by SABnzbd
        XElement? x = GetSabDownload(TVSettings.Instance.SABHostPort, TVSettings.Instance.SABAPIKey);

        if (x is null)
        {
            return;
        }

        ItemList newList = [];
        ItemList toRemove = [];
        int c = ActionList.Missing.Count + 1;
        int n = 0;

        foreach (ShowItemMissing action in ActionList.MissingEpisodes)
        {
            if (Settings.Token.IsCancellationRequested)
            {
                return;
            }

            UpdateStatus(n++, c, action.Filename);

            string? simpleShowName = action.Episode?.Show.ShowName.CompareName();

            if (string.IsNullOrWhiteSpace(simpleShowName))
            {
                continue;
            }

            foreach (QueueSlotsSlot te in x
                         .Descendants("slots")
                         .Select(slot => CreateQueueSlotsSlot(slot, simpleShowName, action))
                         .OfType<QueueSlotsSlot>()
                     )
            {
                toRemove.Add(action);
                newList.Add(new ItemDownloading(te, action.Episode!, action.TheFileNoExt, DownloadApp.SABnzbd, action));
                break;
            }
        }

        ActionList.Replace(toRemove, newList);
    }

    private static XElement? GetSabDownload(string hostPort, string key)
    {
        // Something like:
        // http://localhost:8080/sabnzbd/api?mode=queue&apikey=xxx&start=0&limit=8888&output=xml
        string theUrl = $"http://{hostPort}/sabnzbd/api?mode=queue&start=0&limit=8888&output=xml&apikey={key}";

        string? responseText;
        try
        {
            using HttpClient client = new();
            using HttpResponseMessage response = client.GetAsync(theUrl).Result;
            using HttpContent content = response.Content;

            responseText = content.ReadAsStringAsync().Result;
        }
        catch (Exception e)
        {
            LOGGER.Warn(e, $"Could not connect to {theUrl}, please recheck the settings");
            return null;
        }

        if (string.IsNullOrWhiteSpace(responseText))
        {
            LOGGER.Warn($"Did not get any response from {theUrl}, please recheck the settings");
            return null;
        }

        XElement x;
        try
        {
            x = XElement.Parse(responseText);
        }
        catch (Exception e)
        {
            LOGGER.Error(e, $"Error processing data from SABnzbd: ({theUrl}): ({responseText})");
            return null;
        }

        return x;
    }

    private static QueueSlotsSlot? CreateQueueSlotsSlot(XElement slot, string simpleShowName, ShowItemMissing action)
    {
        string? filename = slot.Attribute("filename")?.Value;
        if (string.IsNullOrWhiteSpace(filename))
        {
            return null;
        }

        FileInfo file = new(filename);

        if (!FileHelper.SimplifyAndCheckFilename(file.FullName, simpleShowName, true, false))
        {
            return null;
        }

        if (!FinderHelper.FindSeasEp(file, out int seasF, out int epF, out int _,
                action.MissingEpisode.Show) ||
            seasF != action.MissingEpisode.AppropriateSeasonNumber ||
            epF != action.MissingEpisode.AppropriateEpNum)
        {
            return null;
        }

        return new QueueSlotsSlot
        {
            Filename = filename,
            Mb = slot.Attribute("mb")?.Value,
            SizeLeft = slot.Attribute("sizeleft")?.Value,
            Status = slot.Attribute("status")?.Value,
            TimeLeft = slot.Attribute("timeleft")?.Value
        };
    }
}
