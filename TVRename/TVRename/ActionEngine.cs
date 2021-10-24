//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TVRename
{
    /// <summary>
    /// Handles the multithreaded nature of actioning many actions at the same time. It will provide a UI to update the user on the status of the execution if required.
    /// </summary>
    public class ActionEngine
    {
        private bool actionPause;
        private SafeList<Thread>? actionWorkers;
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
        private void ProcessSingleAction(object infoIn)
        {
            if (!(infoIn is ProcessActionInfo info))
            {
                return;
            }

            try
            {
                info.Sem.WaitOne(); // don't start until we're allowed to
                actionStarting = false; // let our creator know we're started ok

                Action action = info.TheAction;

                Logger.Trace("Triggering Action: {0} - {1} - {2}", action.Name, action.Produces, action.ToString());
                action.Outcome = action.Go(mStats);
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
                info.Sem.Release();
            }
        }

        private void WaitForAllActionThreadsAndTidyUp()
        {
            if (actionWorkers != null)
            {
                foreach (Thread t in actionWorkers)
                {
                    if (t.IsAlive)
                    {
                        t.Join();
                    }
                }
            }

            actionWorkers = null;
        }

        /// <summary>
        /// Processes a set of actions, running them in a multi-threaded way based on the application's settings.
        /// </summary>
        /// <param name="theList">An ItemList to be processed.</param>
        /// <param name="showUi">Whether or not we should display a UI to inform the user about progress.</param>
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

            ActionProcessorThreadArgs args = new() {TheList = theList, Token = token};

            Thread actionProcessorThread = new(ActionProcessor)
            {
                Name = "ActionProcessorThread"
            };

            actionProcessorThread.Start(args);
            actionProcessorThread.Join();
        }

        private class ActionProcessorThreadArgs
        {
            public ItemList TheList;
            public CancellationToken Token;
        }

        private void ActionProcessor(object argsIn)
        {
            try
            {
                if (!(argsIn is ActionProcessorThreadArgs args))
                {
                    string message =
                        $"Action Processor called with object that is not a ActionProcessorThreadArgs, instead called with a {argsIn.GetType().FullName}";

                    Logger.Fatal(message);
                    throw new ArgumentException(message);
                }

                ActionQueue[] queues = ActionProcessorMakeQueues(args.TheList);

                foreach (ActionQueue queue in queues)
                {
                    Logger.Info($"Setting up {queue}");
                }

                try
                {
                    actionWorkers = new SafeList<Thread>();

                    ExecuteQueues(queues,args.Token);

                    WaitForAllActionThreadsAndTidyUp();
                }
                catch (ThreadAbortException)
                {
                    if (!(actionWorkers is null))
                    {
                        foreach (Thread t in actionWorkers)
                        {
                            t?.Abort();
                        }
                    }

                    WaitForAllActionThreadsAndTidyUp();
                }
                WaitForAllActionThreadsAndTidyUp();
                args.TheList.RemoveAll(x => x is Action action && action.Outcome.Done && !action.Outcome.Error);

                foreach (Action slia in args.TheList.Actions)
                {
                    Logger.Warn(slia.Outcome.LastError, $"Failed to complete the following action: {slia.Name}, doing {slia}. Error was {slia.Outcome.LastError?.Message}");
                }

                Logger.Info("Completed Selected Actions");
                Logger.Info("**************************");
            }
            catch (ThreadAbortException)
            {
                //Ignore this Exception as we can be aborting threads
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in ActionProcessor");
                if (!(actionWorkers is null))
                {
                    foreach (Thread t in actionWorkers)
                    {
                        t?.Abort();
                    }
                }
            }
        }

        private void ExecuteQueues(ActionQueue[] queues, CancellationToken token)
        {
            while (true)
            {
                while (actionPause)
                {
                    Thread.Sleep(100);
                }

                if (ReviewQueues(queues))
                {
                    break; // all done!
                }
                if (token.IsCancellationRequested)
                {
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
                        StartThread(new ProcessActionInfo(currentQueue.Sem, act));
                    }
                }
            }

            return allDone;
        }

        private void StartThread([NotNull] ProcessActionInfo pai)
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

                actionWorkers.Add(t);
                actionStarting = true; // set to false in thread after it has the semaphore
                t.Start(pai);
            }
            finally
            {
                int nfr = pai.Sem.Release(); // release our hold on the semaphore, so that worker can grab it
                ThreadsLogger.Trace("ActionProcessor[" + pai.Sem + "] pool has " + nfr + " free");
            }
        }

        private void TidyDeadWorkers()
        {
            if (actionWorkers is null)
            {
                return;
            }

            foreach (Thread aw in actionWorkers.ToList())
            {
                if (aw is null)
                {
                    continue;
                }

                if (!aw.IsAlive)
                {
                    actionWorkers.Remove(aw);
                }
            }

            // tidy up any finished workers
            for (int i = actionWorkers.Count - 1; i >= 0; i--)
            {
                if (!actionWorkers[i].IsAlive)
                {
                    actionWorkers.RemoveAt(i); // remove dead worker
                }
            }
        }

        [NotNull]
        private static ActionQueue[] ActionProcessorMakeQueues([NotNull] ItemList theList)
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

            ActionQueue[] queues = new ActionQueue[4];
            queues[0] = new ActionQueue("Move/Copy", 1); // cross-filesystem moves (slow ones)
            queues[1] = new ActionQueue("Rename/Delete", 1); // local rename/moves
            queues[2] = new ActionQueue("Write Metadata", 4); // writing KODI NFO files, etc.
            queues[3] = new ActionQueue("Download",
                TVSettings.Instance.ParallelDownloads); // downloading torrents, banners, thumbnails

            foreach (Item sli in theList)
            {
                if (!(sli is Action action))
                {
                    continue; // skip non-actions
                }

                queues[GetQueueId(action)].Actions.Add(action);
            }
            return queues;
        }

        private static int GetQueueId([NotNull] Action action)
        {
            switch (action)
            {
                // base interface that all metadata actions are derived from
                case ActionWriteMetadata _:
                    return 2;

                case ActionDownloadImage _:
                case ActionTDownload _:
                case ActionTRemove _:
                    return 3;

                case ActionCopyMoveRename rename:
                    return rename.QuickOperation() ? 1 : 0;

                case ActionDeleteFile _:
                case ActionDeleteDirectory _:
                case ActionMoveRenameDirectory _:
                    return 1;

                case ActionDateTouch _:
                    // add them after the slow move/renames (ie last)
                    return 0;

                default:
                    Logger.Fatal("No action type found for {0}, Please follow up with a developer.", action.GetType());
                    // put it in this queue by default
                    return 3;
            }
        }

        #region Nested type: ProcessActionInfo

        private class ProcessActionInfo
        {
            public readonly Semaphore Sem;
            public readonly Action TheAction;

            public ProcessActionInfo(Semaphore s, Action a)
            {
                Sem = s;
                TheAction = a;
            }
        }

        #endregion Nested type: ProcessActionInfo
    }
}
