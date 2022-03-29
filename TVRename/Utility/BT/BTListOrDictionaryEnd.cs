using JetBrains.Annotations;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class BTListOrDictionaryEnd : BTItem
    {
        public BTListOrDictionaryEnd()
            : base(BTChunk.kListOrDictionaryEnd)
        {
        }

        public override void Write([NotNull] System.IO.Stream sw)
        {
            sw.WriteByte((byte)'e');
        }
    }
}
