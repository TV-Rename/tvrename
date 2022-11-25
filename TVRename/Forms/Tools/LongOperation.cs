using System.Threading;

namespace TVRename.Forms.Tools;

public abstract class LongOperation
{
    public abstract void Start(CancellationToken sourceToken, SetProgressDelegate? progress);
}
