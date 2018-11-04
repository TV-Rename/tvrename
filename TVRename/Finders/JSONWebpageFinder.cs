// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Net;
using Newtonsoft.Json.Linq;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class JSONFinder: DownloadFinder
    {
        public JSONFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchJSON;

        public override FinderDisplayType DisplayType() => FinderDisplayType.search;

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            int c = ActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct);

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();

            foreach (ItemMissing action in ActionList.MissingItems())
            {
                if (ActionCancel)
                    return;

                prog.Invoke(startpct + ((totPct - startpct) * (++n) / (c)));

                ProcessedEpisode pe = action.Episode;
                string simpleShowName = Helpers.SimplifyName(action.Episode.Show.ShowName);
                string simpleSeriesName = Helpers.SimplifyName(action.Episode.TheSeries.Name);

                string imdbId= action.Episode.TheSeries.GetImdbNumber();

                if (string.IsNullOrWhiteSpace(imdbId)) continue;

                WebClient client = new WebClient();
                client.Headers.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
                string response = client.DownloadString($"{TVSettings.Instance.SearchJSONURL}{imdbId}");

                JObject jsonResponse = JObject.Parse(response);
                if (jsonResponse.ContainsKey(TVSettings.Instance.SearchJSONRootNode))
                {
                    foreach (JToken item in jsonResponse[TVSettings.Instance.SearchJSONRootNode])
                    {
                        if (item != null && item is JObject episodeResponse)
                        {
                            if (episodeResponse.ContainsKey(TVSettings.Instance.SearchJSONFilenameToken) &&
                                episodeResponse.ContainsKey(TVSettings.Instance.SearchJSONURLToken))
                            {
                                string itemName = (string) item[TVSettings.Instance.SearchJSONFilenameToken];
                                string itemUrl = (string) item[TVSettings.Instance.SearchJSONURLToken];

                                if (!FileHelper.SimplifyAndCheckFilename(itemName, simpleShowName, true, false) &&
                                    !FileHelper.SimplifyAndCheckFilename(itemName, simpleSeriesName, true, false))
                                    continue;

                                if (!TVDoc.FindSeasEp(itemName, out int seas, out int ep, out int _,
                                    action.Episode.Show))
                                    continue;

                                if (seas != pe.AppropriateSeasonNumber) continue;
                                if (ep != pe.AppropriateEpNum) continue;

                                LOGGER.Info(
                                    $"Adding {itemUrl} as it appears to be match for {pe.Show.ShowName} S{pe.AppropriateSeasonNumber}E{pe.AppropriateEpNum}");

                                newItems.Add(new ActionTDownload(itemName, itemUrl, action.TheFileNoExt, pe));
                                toRemove.Add(action);
                            }
                            else
                            {
                                LOGGER.Info(
                                    $"{TVSettings.Instance.SearchJSONFilenameToken} or {TVSettings.Instance.SearchJSONURLToken} not found in {TVSettings.Instance.SearchJSONURL}{imdbId} for {action.Episode.TheSeries.Name}");
                            }
                        }
                    }
                }
                else
                {
                    LOGGER.Info($"{TVSettings.Instance.SearchJSONRootNode} not found in {TVSettings.Instance.SearchJSONURL}{imdbId} for {action.Episode.TheSeries.Name}");
                }
            }

            foreach (ActionTDownload x in FindDuplicates(newItems))
                newItems.Remove(x);

            foreach (Item i in toRemove)
                ActionList.Remove(i);

            foreach (Item action in newItems)
                ActionList.Add(action);

            prog.Invoke(totPct);
        }
    }
}
