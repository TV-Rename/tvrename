// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using TVRename.SAB;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    class SABnzbdFinder : DownloadingFinder
    {
        public SABnzbdFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.CheckSABnzbd;
        protected override string Checkname() => "Looked in the listed SABnz queue to see if the episode is already being downloaded";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            if (string.IsNullOrEmpty(TVSettings.Instance.SABAPIKey) || string.IsNullOrEmpty(TVSettings.Instance.SABHostPort))
            {
                LOGGER.Info("Searching SABnzxdb Feeds is cancelled as the key and host/port are notprovided in Preferences.");
                return;
            }

            // get list of files being downloaded by SABnzbd

            // Something like:
            // http://localhost:8080/sabnzbd/api?mode=queue&apikey=xxx&start=0&limit=8888&output=xml
            string theUrl = $"http://{TVSettings.Instance.SABHostPort}/sabnzbd/api?mode=queue&start=0&limit=8888&output=xml&apikey={TVSettings.Instance.SABAPIKey}";

            string response = new WebClient().DownloadString(theUrl);
            //string response = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <queue><noofslots_total>0</noofslots_total> <diskspace2_norm>2.2 T</diskspace2_norm> <paused>False</paused> <finish>0</finish> <speedlimit_abs></speedlimit_abs> <slots></slots> <speed>0  </speed> <size>0 B</size> <rating_enable>False</rating_enable> <eta>unknown</eta> <refresh_rate>1</refresh_rate> <start>0</start> <version>2.3.7</version> <diskspacetotal2>2794.39</diskspacetotal2> <limit>8888</limit> <diskspacetotal1>2794.39</diskspacetotal1> <status>Idle</status> <have_warnings>0</have_warnings> <cache_art>0</cache_art> <sizeleft>0 B</sizeleft> <finishaction>None</finishaction> <paused_all>False</paused_all> <quota>0 </quota> <have_quota>False</have_quota> <mbleft>0.00</mbleft> <diskspace2>2236.57</diskspace2> <diskspace1>2236.57</diskspace1> <scripts></scripts> <categories><category>*</category> </categories> <timeleft>0:00:00</timeleft> <pause_int>0</pause_int> <noofslots>0</noofslots> <mb>0.00</mb> <loadavg></loadavg> <cache_max>536870912</cache_max> <kbpersec>0.00</kbpersec> <speedlimit>100</speedlimit> <cache_size>0 B</cache_size> <left_quota>0 </left_quota> <diskspace1_norm>2.2 T</diskspace1_norm> <queue_details>0</queue_details> </queue>";

            if (string.IsNullOrWhiteSpace(response))
            {
                LOGGER.Warn($"Did not get any response from {theUrl}, please recheck the settings");
                return;
            }

            XElement x;
            try
            {
                x = XElement.Parse(response);
            }
            catch (Exception e)
            {
                LOGGER.Error(e, $"Error processing data from SABnzbd: ({theUrl}): ({response})");
                return;
            }

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = ActionList.MissingItems().Count() + 1;
            int n = 0;

            try
            {
                foreach (ItemMissing action in ActionList.MissingItems())
                {
                    if (settings.Token.IsCancellationRequested)
                        return;

                    UpdateStatus(n++, c, action.Filename);

                    if (action.Episode?.Show is null) continue;

                    string simpleShowName = Helpers.SimplifyName(action.Episode.Show.ShowName);

                    if (string.IsNullOrWhiteSpace(simpleShowName)) continue;

                    foreach (XElement slot in x.Descendants("slots"))
                    {
                        string filename = slot.Attribute("filename")?.Value;
                        if (string.IsNullOrWhiteSpace(filename)) continue;

                        FileInfo file = new FileInfo(filename);

                        if (!FileHelper.SimplifyAndCheckFilename(file.FullName, simpleShowName, true, false)) continue;

                        if (!FinderHelper.FindSeasEp(file, out int seasF, out int epF, out int _,
                                action.Episode.Show) ||
                            (seasF != action.Episode.AppropriateSeasonNumber) ||
                            (epF != action.Episode.AppropriateEpNum)) continue;

                        QueueSlotsSlot te = new QueueSlotsSlot
                        {
                            Filename = filename,
                            Mb = slot.Attribute("mb")?.Value,
                            Sizeleft = slot.Attribute("sizeleft")?.Value,
                            Status = slot.Attribute("status")?.Value,
                            Timeleft = slot.Attribute("timeleft")?.Value
                        };

                        toRemove.Add(action);
                        newList.Add(new ItemDownloading(te, action.Episode, action.TheFileNoExt, DownloadApp.SABnzbd));
                        break;
                    }
                }

                ActionList.Replace(toRemove, newList);
            }
            catch (NullReferenceException nre)
            {
                LOGGER.Error(nre,$"Null Reference in SAB - {response}");
            }
        }
    }
}
