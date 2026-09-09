//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename;

/// <exception cref="ArgumentException"><paramref name="parallelLimit" /> is greater than <paramref name="parallelLimit" />.
/// -or-
/// .NET Framework only: <paramref name="name" /> is longer than MAX_PATH (260 characters).</exception>
/// <exception cref="IOException"><paramref name="name" /> is invalid. This can be for various reasons, including some restrictions that may be placed by the operating system, such as an unknown prefix or invalid characters. Note that the name and common prefixes "Global" and "Local" are case-sensitive.
/// -or-
/// There was some other error. The HResult property may provide more information.</exception>
/// <exception cref="DirectoryNotFoundException">Windows only: <paramref name="name" /> specified an unknown namespace. See Object Names for more information.</exception>
/// <exception cref="UnauthorizedAccessException">The named semaphore exists and has access control security, and the user does not have <see cref="System.Security.AccessControl.SemaphoreRights.FullControl" />.</exception>
/// <exception cref="WaitHandleCannotBeOpenedException">A synchronization object with the provided <paramref name="name" /> cannot be created. A synchronization object of a different type might have the same name.</exception>
public class ActionQueue(string name, int parallelLimit, IEnumerable<Action> actions, TVRenameStats mStats, CancellationTokenSource cts)
{
    private readonly List<Action> actions = [.. actions.OrderBy(a => a.Order)]; // The contents of this queue
    private readonly int parallelThreadLimit = parallelLimit; // Number of tasks in the queue than can be run at once
    public readonly string queueName = name; // Name of this queue
    SemaphoreSlim semaphore = new(parallelLimit, parallelLimit);

    public override string ToString() => $"'{queueName}' worker, with {parallelThreadLimit} threads.";
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly NLog.Logger ThreadsLogger = NLog.LogManager.GetLogger("threads");

    private readonly ManualResetEventSlim _pauseEvent = new(true);
    /// <summary>
    /// Asks for execution to pause
    /// </summary>
    public void Pause()
    {
        Logger.Info("Actions requested to be paused");
        _pauseEvent.Reset(); // Wake up the thread
    }   

    /// <summary>
    /// Asks for execution to resume
    /// </summary>
    public void Resume()
    {
        Logger.Info("Actions requested to be resumed");
        _pauseEvent.Set(); // Pause the thread again if set to false
    }

    public List<Task>? currentTasks; //Task that relates to all the actions in this queue, so that we can wait for it to finish if we need to

    internal void Start()
    {
        semaphore = new SemaphoreSlim(parallelThreadLimit, parallelThreadLimit);
        currentTasks = actions.Select(action => ProcessSingleActionAsync(action, semaphore, cts)).ToList();
    }

    public async Task WaitForCompletionAsync()
    {
        if (currentTasks is not null)
        {
            await Task.WhenAll(currentTasks).ConfigureAwait(false);
        }
        semaphore.Dispose();
    }

    private async Task ProcessSingleActionAsync(Action action, SemaphoreSlim semaphore, CancellationTokenSource cts)
    {
        // don't start until we're allowed to
        await semaphore.WaitAsync(cts.Token).ConfigureAwait(false); // blocks until there is an available slot

        // Pause the thread until _pauseEvent is signaled, or throw if cancelled
        _pauseEvent.Wait(cts.Token);

        // Double check cancellation and condition
        if (cts.Token.IsCancellationRequested)
        {
            action.ErrorText = "Process Cancelled";
            return;
        }

        try
        {
            Logger.Trace($"Triggering Action: {action.Name} - {action.Produces} - {action}");
            action.Outcome = await action.GoAsync(mStats, cts.Token);
            if (action.Outcome.Error)
            {
                action.ErrorText = action.Outcome.LastError?.Message ?? string.Empty;
            }

            if (!action.Outcome.Done)
            {
                Logger.Error("Action did not report whether it was completed");
                action.Outcome = new ActionOutcome("Action did not report whether it was completed");
            }
        }
        catch (OperationCanceledException)
        {
            // Task was cancelled
            action.Outcome = new ActionOutcome("Process Cancelled");
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Unhandled Exception in Process Single Action");
            action.Outcome = new ActionOutcome(e);
        }
        finally
        {
            int nfr = semaphore.Release(); // release our hold on the semaphore, so that worker can grab it
            ThreadsLogger.Trace("ActionProcessor[" + queueName + "] pool has " + nfr + " free");
        }
    }
}
