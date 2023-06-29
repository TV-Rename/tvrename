namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTError : BTItem
{
    public readonly string Message;

    public BTError(string message) : base(BTChunk.kError)
    {
        Message = message;
    }
}
