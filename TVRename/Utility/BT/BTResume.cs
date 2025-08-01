using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTResume : BTCore
{
    // ReSharper disable once InconsistentNaming
    private static class BTPrio
    {
        public const int SKIP = 0x80;
    }

    private BTFile? resumeDat; // resume file, if we're using it
    private readonly string resumeDatPath;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public BTResume(string resumeDatFile)
    {
        resumeDatPath = resumeDatFile;
    }

    private static int PercentBitsOn(BTString? s)
    {
        if (s == null)
        {
            return 0;
        }
        int totalBits = 0;
        int bitsOn = 0;

        foreach (byte t in s.Data)
        {
            totalBits += 8;
            byte c = t;
            for (int j = 0; j < 8; j++)
            {
                if ((c & 0x01) != 0)
                {
                    bitsOn++;
                }

                c >>= 1;
            }
        }
        if (totalBits == 0) { return 100; }

        return 100 * bitsOn / totalBits;
    }

    public List<TorrentEntry> AllFilesBeingDownloaded()
    {
        List<TorrentEntry> r = [];

        if (resumeDat is null)
        {
            return r;
        }

        foreach (BTDictionaryItem dictitem in resumeDat.GetDict().Items)
        {
            if (dictitem.Type != BTChunk.kDictionaryItem)
            {
                continue;
            }

            if (dictitem.Key == ".fileguard" || dictitem.Data.Type != BTChunk.kDictionary)
            {
                continue;
            }

            if (dictitem.Data is BTError err)
            {
                Logger.Error($"Error finding BT items: {err.Message}");
                return r;
            }

            BTDictionary d2 = (BTDictionary)dictitem.Data;

            BTItem? p = d2.GetItem("prio");
            if (p is null || p.Type != BTChunk.kString)
            {
                continue;
            }

            BTString prioString = (BTString)p;
            string directoryName = Path.GetDirectoryName(resumeDatPath) + System.IO.Path.DirectorySeparatorChar;

            string torrentFile = dictitem.Key;
            if (!File.Exists(torrentFile)) // if the torrent file doesn't exist
            {
                torrentFile = directoryName + torrentFile; // ..try pre-pending the resume.dat folder's path to it.
            }

            if (!File.Exists(torrentFile))
            {
                continue; // can't find it.  give up!
            }

            BTFile? tor = BEncodeLoader.Load(torrentFile);

            List<string>? a = tor?.AllFilesInTorrent();
            if (a is null)
            {
                continue;
            }

            p = d2.GetItem("path");
            if (p is null || p.Type != BTChunk.kString)
            {
                continue;
            }

            string defaultFolder = ((BTString)p).AsString();

            BTItem? targets = d2.GetItem("targets");
            bool hasTargets = targets is { Type: BTChunk.kList };
            BTList? targetList = (BTList?)targets;

            ProcessFiles(r, d2, prioString, torrentFile, a, defaultFolder, hasTargets, targetList);
        }

        return r;
    }

    private static void ProcessFiles(List<TorrentEntry> r, BTDictionary d2, BTString prioString, string torrentFile, List<string> a, string defaultFolder, bool hasTargets, BTList? targetList)
    {
        int c = 0;
        foreach (string s in a)
        {
            if (c < prioString.Data.Length && prioString.Data[c] != BTPrio.SKIP)
            {
                Process(r, d2, torrentFile, defaultFolder, hasTargets, targetList, s, c);
            }

            c++;
        }
    }

    private static void Process(List<TorrentEntry> r, BTDictionary d2, string torrentFile, string defaultFolder, bool hasTargets,
        BTList? targetList, string s, int c)
    {
        try
        {
            string saveTo = FileHelper
                .FileInFolder(defaultFolder, TVSettings.Instance.FilenameFriendly(s)).Name;

            if (hasTargets && targetList != null)
            {
                saveTo = GetTargetSaveLocation(targetList, c) ?? saveTo;
            }

            bool completed = ((BTInteger?)d2.GetItem("order"))?.Value == -1;
            int percent = completed ? 100 : PercentBitsOn((BTString?)d2.GetItem("have"));
            TorrentEntry te = new(torrentFile, saveTo, percent, completed, torrentFile);
            r.Add(te);
        }
        catch (PathTooLongException ptle)
        {
            //this is not the file we are looking for
            Logger.Debug(ptle);
        }
    }

    private static string? GetTargetSaveLocation(BTList targetList, int c)
    {
        // see if there is a target for this (the c'th) file
        foreach (BTList l in targetList.Items.OfType<BTList>())
        {
            BTInteger n = (BTInteger)l.Items[0];
            BTString dest = (BTString)l.Items[1];
            if (n.Value == c)
            {
                return dest.AsString();
            }
        }

        return null;
    }

    public bool LoadResumeDat()
    {
        resumeDat = BEncodeLoader.Load(resumeDatPath);
        return resumeDat != null;
    }
}
