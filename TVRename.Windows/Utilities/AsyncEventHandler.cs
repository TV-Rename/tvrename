using System;
using System.Threading.Tasks;

namespace TVRename.Windows.Utilities
{
    public class AsyncEventHandler
    {
        private readonly TaskCompletionSource<EventArgs> tcs = new TaskCompletionSource<EventArgs>();

        public EventHandler Handler => (s, a) => this.tcs.SetResult(a);

        public Task<EventArgs> Event => this.tcs.Task;
    }

    public class AsyncEventHandler<TEventArgs>
    {
        private readonly TaskCompletionSource<TEventArgs> tcs = new TaskCompletionSource<TEventArgs>();

        public EventHandler<TEventArgs> Handler => (s, a) => this.tcs.SetResult(a);

        public Task<TEventArgs> Event => this.tcs.Task;
    }
}
