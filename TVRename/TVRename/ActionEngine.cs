//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TVRename;

/// <summary>
/// Handles the multithreaded nature of actioning many actions at the same time. It will provide a UI to update the user on the status of the execution if required.
/// </summary>
public class ActionEngine
{
    private bool actionPause;
    private SafeList<(Thread, CancellationTokenSource)>? actionWorkers;
    private bool actionStarting;

    private readonly TVRenameStats mStats; //reference to the main TVRenameStats, so we can update the counts

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly NLog.Logger ThreadsLogger = NLog.LogManager.GetLogger("threads");

    /// <summary>
    /// Asks for execution to pause
    /// </summary>
    public void Pause()
    {
        Logger.Info("Actions requested to be paused");
        actionPause = true;
    }

    /// <summary>
    /// Asks for execution to resume
    /// </summary>
    public void Resume()
    {
        Logger.Info("Actions requested to be resumed");
        actionPause = false;
    }

    public ActionEngine(TVRenameStats stats)
    {
        mStats = stats;
    }

    /// <summary>
    /// Processes an Action by running it.
    /// </summary>
    /// <param name="infoIn">A ProcessActionInfo to be processed. It will contain the Action to be processed</param>
    private void ProcessSingleAction(object? infoIn)
    {
        if (infoIn is not ProcessActionInfo info)
        {
            return;
        }

        try
        {
            info.Queue.Sem.WaitOne(); // don't start until we're allowed to
            actionStarting = false; // let our creator know we're started ok

            Action action = info.TheAction;

            if (info.Token.IsCancellationRequested)
            {
                action.ErrorText = "Process Cancelled";
                return;
            }

            Logger.Trace($"Triggering Action: {action.Name} - {action.Produces} - {action}");
            action.Outcome = action.Go(mStats, info.Token);
            if (action.Outcome.Error)
            {
                action.ErrorText = action.Outcome.LastError?.Message ?? string.Empty;
            }

            if (!action.Outcome.Done)
            {
                Logger.Error("Action did not report whether it was completed");
                info.TheAction.Outcome = new ActionOutcome("Action did not report whether it was completed");
            }
        }
        catch (ThreadAbortException te)
        {
            //Thread has been killed off
            info.TheAction.Outcome = new ActionOutcome(te);
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Unhandled Exception in Process Single Action");
            info.TheAction.Outcome = new ActionOutcome(e);
        }
        finally
        {
            info.Queue.Sem.Release();
        }
    }

    private void WaitForAllActionThreadsAndTidyUp()
    {
        actionWorkers?.ForEach(t =>
        {
            if (t.Item1.IsAlive)
            {
                t.Item1.Join();
            }
        });

        actionWorkers = null;
    }

    /// <summary>
    /// Processes a set of actions, running them in a multi-threaded way based on the application's settings.
    /// </summary>
    /// <param name="theList">An ItemList to be processed.</param>
    /// <param name="token"></param>
    public void DoActions(ItemList? theList, CancellationToken token)
    {
        if (theList is null)
        {
            Logger.Info("Asked to do actions, but none provided....");
            return;
        }

        Logger.Info("**********************");
        Logger.Info($"Doing Selected Actions.... ({theList.Count} items detected, {theList.Actions.Count} actions to be completed )");

        // Run tasks in parallel (as much as is sensible)

        actionPause = false;

        ActionProcessorThreadArgs args = new(theList: theList, token: token);

        Thread actionProcessorThread = new(ActionProcessor)
        {
            Name = "ActionProcessorThread"
        };

        actionProcessorThread.Start(args);
        actionProcessorThread.Join();
    }

    private class ActionProcessorThreadArgs
    {
        public readonly ItemList TheList;
        public readonly CancellationToken Token;

        public ActionProcessorThreadArgs(ItemList theList, CancellationToken token)
        {
            TheList = theList;
            Token = token;
        }
    }

    private void ActionProcessor(object? argsIn)
    {
        try
        {
            if (argsIn is not ActionProcessorThreadArgs args)
            {
                string message =
                    $"Action Processor called with object that is not a ActionProcessorThreadArgs, instead called with a {argsIn?.GetType().FullName}";

                Logger.Fatal(message);
                throw new ArgumentException(message);
            }

            List<ActionQueue> queues = ActionProcessorMakeQueues(args.TheList);

            foreach (ActionQueue queue in queues)
            {
                Logger.Info($"Setting up {queue}");
            }

            try
            {
                actionWorkers = new SafeList<(Thread, CancellationTokenSource)>();

                ExecuteQueues(queues, args.Token);

                WaitForAllActionThreadsAndTidyUp();
            }
            catch (ThreadAbortException)
            {
                AbortAllThreads();
                WaitForAllActionThreadsAndTidyUp();
            }
            WaitForAllActionThreadsAndTidyUp();
            args.TheList.RemoveAll(x => x is Action { Outcome: { Done: true, Error: false } });

            foreach (Action slia in args.TheList.Actions)
            {
                Logger.Warn(slia.Outcome.LastError, $"Failed to complete the following action: {slia.Name}, doing {slia}. Error was {slia.Outcome.LastError?.Message}");
            }

            Logger.Info("Completed Selected Actions");
            Logger.Info("**************************");
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Unhandled Exception in ActionProcessor");
            AbortAllThreads();
        }
    }

    private void AbortAllThreads()
    {
        if (actionWorkers is not null)
        {
            foreach ((Thread, CancellationTokenSource) t in actionWorkers)
            {
                t.Item2.Cancel();
            }
        }
    }

    private void ExecuteQueues(List<ActionQueue> queues, CancellationToken token)
    {
        while (true)
        {
            while (actionPause)
            {
                Thread.Sleep(100);
                if (token.IsCancellationRequested)
                {
                    AbortAllThreads();
                    break; //We have been requested to finish
                }
            }

            if (ReviewQueues(queues))
            {
                break; // all done!
            }
            if (token.IsCancellationRequested)
            {
                AbortAllThreads();
                break; //We have been requested to finish
            }

            while (actionStarting) // wait for thread to get the semaphore
            {
                Thread.Sleep(10); // allow the other thread a chance to run and grab
            }

            TidyDeadWorkers();
        }
    }

    private bool ReviewQueues(IEnumerable<ActionQueue>? queues)
    {
        // look through the list of semaphores to see if there is one waiting for some work to do
        if (queues is null)
        {
            return true;
        }

        bool allDone = true;
        foreach (ActionQueue currentQueue in queues.Where(currentQueue => currentQueue.HasCapacity))
        {
            // something to do in this queue, and semaphore is available
            allDone = false;

            if (currentQueue.Sem.WaitOne(20, false))
            {
                Action act = currentQueue.NextAction();

                if (!act.Outcome.Done)
                {
                    CancellationTokenSource cts = new();
                    StartThread(new ProcessActionInfo(currentQueue, act, cts.Token), cts);
                }
            }
        }

        return allDone;
    }

    private void StartThread(ProcessActionInfo pai, CancellationTokenSource cancellationTokenSource)
    {
        try
        {
            Thread t = new(ProcessSingleAction)
            {
                Name = "ProcessSingleAction(" + pai.TheAction.Name + ":" + pai.TheAction.ProgressText + ")"
            };

            if (actionWorkers is null)
            {
                Logger.Error(
                    $"Asked to start for {pai.TheAction.Name}, but actionWorkers has been removed, please restart TV Rename and contact help if this recurrs.");

                return;
            }

            actionWorkers.Add((t, cancellationTokenSource));
            actionStarting = true; // set to false in thread after it has the semaphore
            t.Start(pai);
        }
        finally
        {
            int nfr = pai.Queue.Sem.Release(); // release our hold on the semaphore, so that worker can grab it
            ThreadsLogger.Trace("ActionProcessor[" + pai.Queue + "] pool has " + nfr + " free");
        }
    }

    private void TidyDeadWorkers()
    {
        if (actionWorkers is null)
        {
            return;
        }

        foreach ((Thread, CancellationTokenSource) aw in actionWorkers.ToList())
        {
            if (aw.Item1.IsAlive == false)
            {
                actionWorkers.Remove(aw);
            }
        }

        // tidy up any finished workers
        for (int i = actionWorkers.Count - 1; i >= 0; i--)
        {
            if (!actionWorkers[i].Item1.IsAlive)
            {
                actionWorkers.RemoveAt(i); // remove dead worker
            }
        }
    }

    private static List<ActionQueue> ActionProcessorMakeQueues(ItemList theList)
    {
        // Take a single list
        // Return an array of "ActionQueue" items.
        // Each individual queue is processed sequentially, but all the queues run in parallel
        // The lists:
        //     - #0 all the cross filesystem moves, and all copies
        //     - #1 all quick "local" moves
        //     - #2 NFO Generator list
        //     - #3 Downloads (rss torrent, thumbnail, folder.jpg) across Settings.ParallelDownloads lists
        // We can discard any non-action items, as there is nothing to do for them
        return EnumerableExtensions.GetAllItems<Action.QueueName>().Select(q => CreateQueue(q, theList)).ToList();
    }

    private static ActionQueue CreateQueue(Action.QueueName queue, ItemList theList)
        => new(GetName(queue), GetParallelLimit(queue), theList.GetActionsForQueue(queue)); 

    private static string GetName(Action.QueueName queue)
    {
        return queue switch
        {
            Action.QueueName.download => "Download", // downloading torrents, banners, thumbnails
            Action.QueueName.writeMetadata => "Write Metadata", // writing KODI NFO files, etc.
            Action.QueueName.slowFileOperation => "Move/Copy", // cross-filesystem moves (slow ones)
            Action.QueueName.quickFileOperation => "Rename/Delete", // local rename/moves
            _ => throw new ArgumentOutOfRangeException(nameof(queue), queue, null)
        };
    }
    private static int GetParallelLimit(Action.QueueName queue)
    {
        return queue switch
        {
            Action.QueueName.download => TVSettings.Instance.ParallelDownloads,
            Action.QueueName.writeMetadata => 4,
            Action.QueueName.slowFileOperation => 1,
            Action.QueueName.quickFileOperation => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(queue), queue, null)
        };
    }

    #region Nested type: ProcessActionInfo

    private class ProcessActionInfo
    {
        public readonly ActionQueue Queue;
        public readonly Action TheAction;
        public readonly CancellationToken Token;

        public ProcessActionInfo(ActionQueue q, Action a, CancellationToken token)
        {
            Queue = q;
            TheAction = a;
            Token = token;
        }
    }

    #endregion Nested type: ProcessActionInfo
}
