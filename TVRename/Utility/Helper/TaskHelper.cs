using System;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename.Utility.Helper;

internal static class TaskHelper
{
    // <summary>
    // Blocks while condition is true or timeout occurs.
    // </summary>
    // <param name="condition">The condition that will perpetuate the block.</param>
    // <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
    // <param name="timeout">Timeout in milliseconds.</param>
    // <exception cref="TimeoutException"></exception>
    // <returns></returns>
    /// <exception cref="TimeoutException">Condition.</exception>
    public static async Task WaitWhileAsync(Func<bool> condition, int frequency = 25, int timeout = -1)
    {
        Task waitTask = Task.Run(async () =>
        {
            while (condition())
            {
                await Task.Delay(frequency).ConfigureAwait(false);
            }
        });

        if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)).ConfigureAwait(false))
        {
            throw new TimeoutException();
        }
    }

    /// <summary>
    /// Blocks until condition is true or timeout occurs.
    /// </summary>
    /// <param name="condition">The break condition.</param>
    /// <param name="frequency">The frequency at which the condition will be checked.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    /// <returns></returns>
    /// <exception cref="TimeoutException">Condition.</exception>
    public static async Task WaitUntilAsync(Func<bool> condition, int frequency = 25, int timeout = -1)
    {
        Task waitTask = Task.Run(async () =>
        {
            while (!condition())
            {
                await Task.Delay(frequency).ConfigureAwait(false);
            }
        });

        if (waitTask != await Task.WhenAny(waitTask,
                Task.Delay(timeout)).ConfigureAwait(false))
        {
            throw new TimeoutException();
        }
    }

    /// <exception cref="ThreadStateException">The thread has already been started.</exception>
    /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread.</exception>
    public static void Run(System.Action action, string name) => Run(action, name, true);

    /// <exception cref="ThreadStateException">The thread has already been started.</exception>
    /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread.</exception>
    public static void Run(System.Action action, string name, bool isBackground)
    {
        Thread t = new(() => action()) { Name = name, IsBackground = isBackground };
        t.Start();
    }
}
