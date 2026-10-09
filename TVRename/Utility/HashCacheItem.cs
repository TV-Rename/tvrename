//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    public class HashCacheItem
    {
        public long FileSize;
        public long PieceSize;
        public byte[] TheHash;
        public long WhereInFile;

        public HashCacheItem(long wif, long ps, long fs, byte[] h)
        {
            WhereInFile = wif;
            PieceSize = ps;
            FileSize = fs;
            TheHash = h;
        }
    }
}
