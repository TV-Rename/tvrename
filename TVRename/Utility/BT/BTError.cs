namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTError(string message) : BTItem(BTChunk.kError)
{
    public readonly string Message = message;
}
