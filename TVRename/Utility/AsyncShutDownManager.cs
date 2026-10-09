//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

/// <summary>
/// Manages async operations and ensures clean shutdown by cancelling and tracking all running tasks.
/// </summary>
public class AsyncShutdownManager : IAsyncDisposable
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly List<CancellationTokenSource> _cancellationTokenSources = [];
    private readonly List<Task> _trackingTasks = [];
    private readonly Lock _lock = new();
    private bool _disposed;

    /// <summary>
    /// Registers a CancellationTokenSource to be cancelled during shutdown.
    /// </summary>
    public void TrackCancellationToken(CancellationTokenSource? cts)
    {
        if (cts == null || _disposed)
            return;

        lock (_lock)
        {
            if (!_cancellationTokenSources.Contains(cts))
            {
                _cancellationTokenSources.Add(cts);
            }
        }
    }

    /// <summary>
    /// Registers a Task to be awaited during shutdown.
    /// </summary>
    public void TrackTask(Task? task)
    {
        if (task == null || _disposed)
            return;

        lock (_lock)
        {
            if (!_trackingTasks.Contains(task))
            {
                _trackingTasks.Add(task);
            }
        }
    }

    /// <summary>
    /// Cancels all tracked tokens and waits for all tasks to complete.
    /// </summary>
    public async Task CancelAllAndWaitAsync(TimeSpan timeout)
    {
        if (_disposed)
            return;

        Logger.Info($"Starting shutdown - waiting up to {timeout.TotalSeconds} seconds for {_trackingTasks.Count} tasks and {_cancellationTokenSources.Count} cancellation tokens");

        // Step 1: Cancel all tracked tokens
        lock (_lock)
        {
            foreach (var cts in _cancellationTokenSources)
            {
                if (!cts.IsCancellationRequested)
                {
                    try
                    {
                        cts.Cancel();
                        Logger.Debug("Cancelled CancellationTokenSource");
                    }
                    catch (ObjectDisposedException)
                    {
                        Logger.Debug("CancellationTokenSource already disposed");
                    }
                }
            }
        }

        // Step 2: Wait for all tasks to complete with timeout
        Task[] tasksToWait;
        lock (_lock)
        {
            tasksToWait = _trackingTasks.ToArray();
        }

        if (tasksToWait.Length > 0)
        {
            try
            {
                await Task.WhenAll(tasksToWait).WaitAsync(timeout);
                Logger.Info("All tasks completed successfully");
            }
            catch (OperationCanceledException ex)
            {
                Logger.Warn(ex, "Tasks were cancelled during shutdown wait");
            }
            catch (TimeoutException ex)
            {
                Logger.Warn(ex, $"Timeout waiting for tasks to complete after {timeout.TotalSeconds}s");
            }
            catch (AggregateException ex)
            {
                Logger.Warn(ex, "One or more tasks threw exceptions during shutdown");
            }
        }
    }

    /// <summary>
    /// Disposes all tracked CancellationTokenSources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        lock (_lock)
        {
            foreach (var cts in _cancellationTokenSources)
            {
                try
                {
                    cts.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex, "Error disposing CancellationTokenSource");
                }
            }
            _cancellationTokenSources.Clear();
            _trackingTasks.Clear();
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Returns the number of tracked tasks still running.
    /// </summary>
    public int GetRunningTaskCount()
    {
        lock (_lock)
        {
            return _trackingTasks.Count(t => !t.IsCompleted);
        }
    }
}
