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
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    /// <summary>
    /// Handles the multithreaded nature of actioning many actions at the same time. It will provide a UI to update the user on the status of the execution if required.
    /// </summary>
    public class ActionEngine
    {
        private bool actionPause;
        private List<Thread> actionWorkers;
        private bool actionStarting;

        private readonly TVRenameStats mStats; //reference to the main TVRenameStats, so we can update the counts

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly NLog.Logger Threadslogger = NLog.LogManager.GetLogger("threads");

        /// <summary>
        /// Asks for execution to pause
        /// </summary>
        public void Pause()
        {
            actionPause = true;
        }

        /// <summary>
        /// Asks for execution to resume
        /// </summary>
        public void Unpause()
        {
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
                if (action == null)
                {
                    return;
                }

                Logger.Trace("Triggering Action: {0} - {1} - {2}", action.Name, action.Produces, action.ToString());
                action.Outcome = action.Go(mStats);
                if (action.Outcome.Error)
                {
                    action.ErrorText = action.Outcome.LastError.Message;
                }

                if (!action.Outcome.Done)
                {
                    Logger.Error("Hlep");
                }
            }
            catch (ThreadAbortException)
            {
                //Thread has been killed off
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in Process Single Action");
                info.TheAction.Outcome = ActionOutcome.CompleteFail();
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
        public void DoActions([CanBeNull] ItemList theList, bool showUi)
        {
            if (theList is null)
            {
                Logger.Info("Asked to do actions, but none provided....");
                return;
            }

            Logger.Info("**********************");
            Logger.Info($"Doing Selected Actions.... ({theList.Count} items detected, {theList.Actions().Count()} actions to be completed )");

            // Run tasks in parallel (as much as is sensible)

            ActionQueue[] queues = ActionProcessorMakeQueues(theList);
            actionPause = false;

            // If not /hide, show CopyMoveProgress dialog

            CopyMoveProgress cmp = null;
            if (showUi)
            {
                cmp = new CopyMoveProgress(this, queues);
            }

            Thread actionProcessorThread = new Thread(ActionProcessor)
            {
                Name = "ActionProcessorThread"
            };

            actionProcessorThread.Start(queues);

            if (cmp != null && cmp.ShowDialog() == DialogResult.Cancel)
            {
                actionProcessorThread.Abort();
            }

            actionProcessorThread.Join();

            theList.RemoveAll(x => x is Action action && action.Outcome.Done && !action.Outcome.Error);

            foreach (Action slia in theList.Actions())
            {
                Logger.Warn(slia.Outcome.LastError,$"Failed to complete the following action: {slia.Name}, doing {slia}. Error was {slia.Outcome.LastError?.Message}");
            }

            Logger.Info("Completed Selected Actions");
            Logger.Info("**************************");
        }

        private void ActionProcessor(object queuesIn)
        {
            try
            {
                if (!(queuesIn is ActionQueue[] queues))
                {
                    string message =
                        $"Action Processor called with object that is not a ActionQueue[], instead called with a {queuesIn.GetType().FullName}";

                    Logger.Fatal(message);
                    throw new ArgumentException(message);
                }

                foreach (ActionQueue queue in queues)
                {
                    Logger.Info($"Setting up {queue}");
                }

                try
                {
                    actionWorkers = new List<Thread>();

                    ExecuteQueues(queues);

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

            WaitForAllActionThreadsAndTidyUp();
        }

        private void ExecuteQueues(ActionQueue[] queues)
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

                while (actionStarting) // wait for thread to get the semaphore
                {
                    Thread.Sleep(10); // allow the other thread a chance to run and grab
                }

                TidyDeadWorkers();
            }
        }

        private bool ReviewQueues([CanBeNull] IEnumerable<ActionQueue> queues)
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

                    if (act is null)
                    {
                        return false;
                    }

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
                Thread t = new Thread(ProcessSingleAction)
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
                Threadslogger.Trace("ActionProcessor[" + pai.Sem + "] pool has " + nfr + " free");
            }
        }

        private void TidyDeadWorkers()
        {
            if (actionWorkers is null)
            {
                return;
            }

            // tidy up any finished workers
            for (int i = actionWorkers.Count - 1; i >= 0; i--)
            {
                if (actionWorkers[i] is null)
                {
                    continue;
                }

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
            queues[1] = new ActionQueue("Move/Delete", 1); // local rename/moves
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
                    return 3;

                case ActionCopyMoveRename rename:
                    return rename.QuickOperation() ? 1 : 0;

                case ActionDeleteFile _:
                case ActionDeleteDirectory _:
                    return 1;

                case ActionDateTouch _:
                    // add them after the slow move/reanems (ie last)
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
        #endregion
    }
}
