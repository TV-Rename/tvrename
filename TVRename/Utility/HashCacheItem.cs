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
