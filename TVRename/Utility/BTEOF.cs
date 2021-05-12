namespace TVRename
{
    public class BTEOF : BTItem
    {
        public BTEOF()
            : base(BTChunk.kBTEOF)
        {
        }

        public override void Write(System.IO.Stream sw)
        {
        }
    }
}