using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Handles the multithreaded nature of actioning many actions at the same time. It will provide a UI to update the user on the status of the execution if required.
    /// </summary>
    public class ActionEngine
    {
        private Thread actionProcessorThread;
        private bool actionPause;
        private List<Thread> actionWorkers;
        private Semaphore[] actionSemaphores;
        private bool actionStarting;

        private readonly TVRenameStats mStats; //reference to the main TVRenameStats, so we can udpate the counts

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
        public void ProcessSingleAction(object infoIn)
        {
            try
            {
                if (!(infoIn is ProcessActionInfo info))
                    return;

                actionSemaphores[info.SemaphoreNumber].WaitOne(); // don't start until we're allowed to
                actionStarting = false; // let our creator know we're started ok

                Action action = info.TheAction;
                if (action != null)
                {
                    Logger.Trace("Triggering Action: {0} - {1} - {2}", action.Name, action.Produces, action.ToString());
                    action.Go(ref actionPause, mStats);
                }

                actionSemaphores[info.SemaphoreNumber].Release(1);
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in Process Single Action");
            }
        }

        private void WaitForAllActionThreadsAndTidyUp()
        {
            if (actionWorkers != null)
            {
                foreach (Thread t in actionWorkers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            actionWorkers = null;
            actionSemaphores = null;
        }

        /// <summary>
        /// Processes a set of actions, running them in a multi-threaded way based on the application's settings.
        /// </summary>
        /// <param name="theList">An ItemList to be processed.</param>
        /// <param name="showUi">Whether or not we should display a UI to inform the user about progress.</param>
        public void DoActions(ItemList theList, bool showUi)
        {
            if (theList == null)
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
                cmp = new CopyMoveProgress(this, queues);

            actionProcessorThread = new Thread(ActionProcessor)
            {
                Name = "ActionProcessorThread"
            };

            actionProcessorThread.Start(queues);

            if ((cmp != null) && (cmp.ShowDialog() == DialogResult.Cancel))
                actionProcessorThread.Abort();
            
            actionProcessorThread.Join();

            theList.RemoveAll(x => (x is Action action) && action.Done && !action.Error);

            foreach (Action slia in theList.Actions())
            {
                Logger.Warn("Failed to complete the following action: {0}, doing {1}. Error was {2}", slia.Name, slia.ToString(), slia.ErrorText);
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

                SetupQueues(queues);

                try
                {
                    while (true)
                    {
                        while (actionPause)
                            Thread.Sleep(100);

                        // look through the list of semaphores to see if there is one waiting for some work to do
                        bool allDone = true;
                        int which = -1;
                        for (int i = 0; i < queues.Length; i++)
                        {
                            // something to do in this queue, and semaphore is available
                            if (queues[i].ActionPosition < queues[i].Actions.Count)
                            {
                                allDone = false;
                                if (actionSemaphores[i].WaitOne(20, false))
                                {
                                    which = i;
                                    break;
                                }
                            }
                        }

                        if ((which == -1) && (allDone))
                            break; // all done!

                        if (which == -1)
                            continue; // no semaphores available yet, try again for one

                        ActionQueue q = queues[which];
                        Action act = q.Actions[q.ActionPosition++];

                        if (act == null)
                            continue;

                        if (!act.Done)
                        {
                            StartThread(which, act);
                        }

                        while (actionStarting) // wait for thread to get the semaphore
                            Thread.Sleep(10); // allow the other thread a chance to run and grab

                        TidyDeadWorkers();
                    }

                    WaitForAllActionThreadsAndTidyUp();
                }
                catch (ThreadAbortException)
                {
                    foreach (Thread t in actionWorkers)
                        t.Abort();

                    WaitForAllActionThreadsAndTidyUp();
                }
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in ActionProcessor");
                foreach (Thread t in actionWorkers)
                    t.Abort();

                WaitForAllActionThreadsAndTidyUp();
            }
        }

        private void SetupQueues(ActionQueue[] queues)
        {
            int n = queues.Length;

            actionWorkers = new List<Thread>();
            actionSemaphores = new Semaphore[n];

            for (int i = 0; i < n; i++)
            {
                actionSemaphores[i] =
                    new Semaphore(queues[i].ParallelLimit,
                        queues[i].ParallelLimit); // allow up to numWorkers working at once

                Logger.Info("Setting up '{0}' worker, with {1} threads in position {2}.", queues[i].Name,
                    queues[i].ParallelLimit, i);
            }
        }

        private void StartThread(int which, Action act)
        {
            Thread t = new Thread(ProcessSingleAction)
            {
                Name = "ProcessSingleAction(" + act.Name + ":" + act.ProgressText + ")"
            };

            actionWorkers.Add(t);
            actionStarting = true; // set to false in thread after it has the semaphore
            t.Start(new ProcessActionInfo(which, act));

            int nfr = actionSemaphores[which]
                .Release(1); // release our hold on the semaphore, so that worker can grab it

            Threadslogger.Trace("ActionProcessor[" + which + "] pool has " + nfr + " free");
        }

        private void TidyDeadWorkers()
        {
            // tidy up any finished workers
            for (int i = actionWorkers.Count - 1; i >= 0; i--)
            {
                if (!actionWorkers[i].IsAlive)
                    actionWorkers.RemoveAt(i); // remove dead worker
            }
        }

        private static ActionQueue[] ActionProcessorMakeQueues(ItemList theList)
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
                    continue; // skip non-actions

                if (action is ActionWriteMetadata) // base interface that all metadata actions are derived from
                    queues[2].Actions.Add(action);
                else if ((action is ActionDownloadImage) || (action is ActionTDownload))
                    queues[3].Actions.Add(action);
                else if (action is ActionCopyMoveRename rename)
                    queues[rename.QuickOperation() ? 1 : 0].Actions.Add(rename);
                else if ((action is ActionDeleteFile) || (action is ActionDeleteDirectory))
                    queues[1].Actions.Add(action);
                else if (action is ActionDateTouch)
                    queues[0].Actions.Add(action); // add them after the slow move/reanems (ie last)
                else
                {
                    Logger.Fatal("No action type found for {0}, Please follow up with a developer.", action.GetType());
                    queues[3].Actions.Add(action); // put it in this queue by default
                }
            }
            return queues;
        }
        #region Nested type: ProcessActionInfo

        private class ProcessActionInfo
        {
            public readonly int SemaphoreNumber;
            public readonly Action TheAction;

            public ProcessActionInfo(int n, Action a)
            {
                SemaphoreNumber = n;
                TheAction = a;
            }
        }
        #endregion
    }
}
