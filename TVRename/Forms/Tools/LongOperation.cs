using System.Threading;

namespace TVRename.Forms.Tools;

public abstract class LongOperation
{
    public abstract void Start(SetProgressDelegate? progress, CancellationToken sourceToken);
}
