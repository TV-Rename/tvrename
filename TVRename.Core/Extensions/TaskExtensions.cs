using System.Threading;
using System.Threading.Tasks;

namespace TVRename.Core.Extensions
{
    /// <summary>
    /// Task and TPL type extention methods.
    /// </summary>
    public static class TaskExtensions
    {
        public static Task AsTask(this CancellationToken ct)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            ct.Register(() => tcs.TrySetCanceled(), false);

            return tcs.Task;
        }
    }
}
