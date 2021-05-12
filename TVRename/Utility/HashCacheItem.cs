namespace TVRename
{
    public class HashCacheItem
    {
        public long fileSize;
        public long pieceSize;
        public byte[] theHash;
        public long whereInFile;

        public HashCacheItem(long wif, long ps, long fs, byte[] h)
        {
            whereInFile = wif;
            pieceSize = ps;
            fileSize = fs;
            theHash = h;
        }
    }
}