namespace TVRename
{
    public class BTListOrDictionaryEnd : BTItem
    {
        public BTListOrDictionaryEnd()
            : base(BTChunk.kListOrDictionaryEnd)
        {
        }

        public override void Write(System.IO.Stream sw)
        {
            sw.WriteByte((byte) 'e');
        }
    }
}