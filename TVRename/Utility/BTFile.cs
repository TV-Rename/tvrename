using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename
{
    public class BTFile
    {
        public List<BTItem> Items;

        public BTFile()
        {
            Items = new List<BTItem>();
        }

        public List<string> AllFilesInTorrent()
        {
            List<string> r = new List<string>();

            BTItem bti = GetItem("info");
            if ((bti is null) || (bti.Type != BTChunk.kDictionary))
                return null;

            BTDictionary infoDict = (BTDictionary)(bti);

            bti = infoDict.GetItem("files");

            if (bti is null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti is null) || (bti.Type != BTChunk.kString))
                    return null;

                r.Add(((BTString)bti).AsString());
            }
            else
            {
                // multiple file torrent
                BTList fileList = (BTList)(bti);

                foreach (BTItem it in fileList.Items)
                {
                    BTDictionary file = (BTDictionary)(it);
                    BTItem thePath = file.GetItem("path");
                    if (thePath.Type != BTChunk.kList)
                        return null;

                    BTList pathList = (BTList)(thePath);
                    // want the last of the items in the list, which is the filename itself
                    int n = pathList.Items.Count - 1;
                    if (n < 0)
                        return null;

                    BTString fileName = (BTString)(pathList.Items[n]);
                    r.Add(fileName.AsString());
                }
            }

            return r;
        }

        public void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BT File");
            tn.Add(n);
            foreach (BTItem t in Items)
                t.Tree(n.Nodes);
        }

        public BTItem GetItem(string key)
        {
            return GetItem(key, false);
        }

        public BTDictionary GetDict()
        {
            System.Diagnostics.Debug.Assert(Items.Count == 1);
            System.Diagnostics.Debug.Assert(Items[0].Type == BTChunk.kDictionary);

            // our first (and only) Item will be a dictionary of stuff
            return (BTDictionary)(Items[0]);
        }

        public BTItem GetItem(string key, bool ignoreCase)
        {
            if (Items.Count == 0)
                return null;

            BTDictionary btd = GetDict();
            return btd.GetItem(key, ignoreCase);
        }

        public void Write(System.IO.Stream sw)
        {
            foreach (BTItem i in Items)
                i.Write(sw);
        }
    }
}