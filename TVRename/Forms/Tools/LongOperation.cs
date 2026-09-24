using System;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename.Forms.Tools;

public abstract class LongOperation
{
    public abstract Task StartAsync(IProgress<TaskProgress> progress, CancellationToken sourceToken);
}
