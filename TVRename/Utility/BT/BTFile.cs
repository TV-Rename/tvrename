using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTFile
{
    public readonly List<BTItem> Items;

    public BTFile()
    {
        Items = [];
    }

    public List<string>? AllFilesInTorrent()
    {
        List<string> r = [];

        BTItem? bti = GetItem("info");
        if (bti is null || bti.Type != BTChunk.kDictionary)
        {
            return null;
        }

        BTDictionary infoDict = (BTDictionary)bti;

        bti = infoDict.GetItem("files");

        if (bti is null) // single file torrent
        {
            bti = infoDict.GetItem("name");
            if (bti is null || bti.Type != BTChunk.kString)
            {
                return null;
            }

            r.Add(((BTString)bti).AsString());
        }
        else
        {
            // multiple file torrent
            BTList fileList = (BTList)bti;

            foreach (BTItem? thePath in fileList.Items.OfType<BTDictionary>()
                         .Select(file => file.GetItem("path")))
            {
                if (thePath?.Type != BTChunk.kList)
                {
                    return null;
                }

                BTList pathList = (BTList)thePath;
                // want the last of the items in the list, which is the filename itself
                int n = pathList.Items.Count - 1;
                if (n < 0)
                {
                    return null;
                }

                BTString fileName = (BTString)pathList.Items[n];
                r.Add(fileName.AsString());
            }
        }

        return r;
    }

    private BTItem? GetItem(string key) => GetItem(key, false);

    public BTDictionary GetDict()
    {
        Debug.Assert(Items.Count == 1);
        Debug.Assert(Items[0].Type == BTChunk.kDictionary);

        // our first (and only) Item will be a dictionary of stuff
        return (BTDictionary)Items[0];
    }

    private BTItem? GetItem(string key, bool ignoreCase)
    {
        if (Items.Count == 0)
        {
            return null;
        }

        BTDictionary btd = GetDict();
        return btd.GetItem(key, ignoreCase);
    }
}
