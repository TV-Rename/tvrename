using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Handles the multithreaded nature of actioning many actions at the same time. It will provide a UI to update the user on the status of the execution if required.
    /// </summary>
    public class ActionEngine
    {
        
        private Thread ActionProcessorThread;
        private bool ActionPause;
        private List<Thread> ActionWorkers;
        private Semaphore[] ActionSemaphores;
        private bool ActionStarting;

        private TVRenameStats mStats; //reference to the main TVRenameStats, so we can udpate the counts

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static NLog.Logger threadslogger = NLog.LogManager.GetLogger("threads");

        /// <summary>
        /// Asks for execution to pause
        /// </summary>
        public void pause() { ActionPause = true; }
        
        /// <summary>
        /// Asks for execution to resume
        /// </summary>
        public void unpause() { ActionPause = false; }

        public ActionEngine(TVRenameStats stats)
        {
            mStats = stats;

        }
        
        /// <summary>
        /// Processes an Action by running it.
        /// </summary>
        /// <param name="infoIn">A ProcessActionInfo to be processed. It will contain the Action to be processed</param>
        public void ProcessSingleAction(Object infoIn)
        {
            try
            {
                ProcessActionInfo info = infoIn as ProcessActionInfo;
                if (info == null)
                    return;

                this.ActionSemaphores[info.SemaphoreNumber].WaitOne(); // don't start until we're allowed to
                this.ActionStarting = false; // let our creator know we're started ok

                Action action = info.TheAction;
                if (action != null)
                {
                    logger.Trace("Triggering Action: {0} - {1} - {2}", action.Name, action.Produces, action.ToString());
                    action.Go(ref this.ActionPause, mStats);
                }


                this.ActionSemaphores[info.SemaphoreNumber].Release(1);
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Unhandled Exception in Process Single Action");
                return;
            }
        }



        private void WaitForAllActionThreadsAndTidyUp()
        {
            if (this.ActionWorkers != null)
            {
                foreach (Thread t in this.ActionWorkers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            this.ActionWorkers = null;
            this.ActionSemaphores = null;
        }



        /// <summary>
        /// Processes a set of actions, running them in a multi-threaded way based on the application's settings.
        /// </summary>
        /// <param name="theList">An ItemList to be processed.</param>
        /// <param name="showUI">Whether or not we should display a UI to inform the user about progress.</param>
        public void DoActions(ItemList theList, bool showUI)
        {
            logger.Info("**********************");
            logger.Info("Doing Selected Actions....");
            if (theList == null)
                return;

            // Run tasks in parallel (as much as is sensible)

            ActionQueue[] queues = this.ActionProcessorMakeQueues(theList);
            this.ActionPause = false;

            // If not /hide, show CopyMoveProgress dialog

            CopyMoveProgress cmp = null;
            if (showUI)
                cmp = new CopyMoveProgress(this, queues);

            this.ActionProcessorThread = new Thread(this.ActionProcessor)
            {
                Name = "ActionProcessorThread"
            };

            this.ActionProcessorThread.Start(queues);

            if ((cmp != null) && (cmp.ShowDialog() == DialogResult.Cancel))
                this.ActionProcessorThread.Abort();

            this.ActionProcessorThread.Join();

            theList.RemoveAll(x => (x is Action) && ((Action)x).Done && !((Action)x).Error);

            foreach (Item sli in theList)
            {
                if (sli is Action)
                {
                    Action slia = (Action)sli;
                    logger.Warn("Failed to complete the following action: {0}, doing {1}. Error was {2}", slia.Name, slia.ToString(), slia.ErrorText);
                }
            }

            logger.Info("Completed Selected Actions");
            logger.Info("**************************");

        }
        public void ActionProcessor(Object queuesIn)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(queuesIn is ActionQueue[]);
#endif
            try
            {
                ActionQueue[] queues = queuesIn as ActionQueue[];

                if (queues == null)
                    return;

                int N = queues.Length;

                this.ActionWorkers = new List<Thread>();
                this.ActionSemaphores = new Semaphore[N];

                for (int i = 0; i < N; i++)
                {
                    this.ActionSemaphores[i] =
                        new Semaphore(queues[i].ParallelLimit,
                            queues[i].ParallelLimit); // allow up to numWorkers working at once
                    logger.Info("Setting up '{0}' worker, with {1} threads in position {2}.", queues[i].Name,
                        queues[i].ParallelLimit, i);
                }


                try
                {
                    for (; ; )
                    {
                        while (this.ActionPause)
                            Thread.Sleep(100);

                        // look through the list of semaphores to see if there is one waiting for some work to do
                        bool allDone = true;
                        int which = -1;
                        for (int i = 0; i < N; i++)
                        {
                            // something to do in this queue, and semaphore is available
                            if (queues[i].ActionPosition < queues[i].Actions.Count)
                            {
                                allDone = false;
                                if (this.ActionSemaphores[i].WaitOne(20, false))
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

                        ActionQueue Q = queues[which];
                        Action act = Q.Actions[Q.ActionPosition++];

                        if (act == null)
                            continue;

                        if (!act.Done)
                        {
                            Thread t = new Thread(this.ProcessSingleAction)
                            {
                                Name = "ProcessSingleAction(" + act.Name + ":" + act.ProgressText + ")"
                            };
                            this.ActionWorkers.Add(t);
                            this.ActionStarting = true; // set to false in thread after it has the semaphore
                            t.Start(new ProcessActionInfo(which, act));

                            int nfr = this.ActionSemaphores[which]
                                .Release(1); // release our hold on the semaphore, so that worker can grab it
                            threadslogger.Trace("ActionProcessor[" + which + "] pool has " + nfr + " free");
                        }

                        while (this.ActionStarting) // wait for thread to get the semaphore
                            Thread.Sleep(10); // allow the other thread a chance to run and grab

                        // tidy up any finished workers
                        for (int i = this.ActionWorkers.Count - 1; i >= 0; i--)
                        {
                            if (!this.ActionWorkers[i].IsAlive)
                                this.ActionWorkers.RemoveAt(i); // remove dead worker
                        }
                    }

                    this.WaitForAllActionThreadsAndTidyUp();
                }
                catch (ThreadAbortException)
                {
                    foreach (Thread t in this.ActionWorkers)
                        t.Abort();
                    this.WaitForAllActionThreadsAndTidyUp();
                }
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Unhandled Exception in ActionProcessor");
                foreach (Thread t in this.ActionWorkers)
                    t.Abort();
                this.WaitForAllActionThreadsAndTidyUp();
                return;
            }
        }


        public ActionQueue[] ActionProcessorMakeQueues(ItemList theList)
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
            queues[3] = new ActionQueue("Download", TVSettings.Instance.ParallelDownloads); // downloading torrents, banners, thumbnails

            foreach (Item sli in theList)
            {
                Action action = sli as Action;

                if (action == null)
                    continue; // skip non-actions

                if ((action is ActionWriteMetadata) || (action is ActionDateTouch)) // base interface that all metadata actions are derived from
                    queues[2].Actions.Add(action);
                else if ((action is ActionDownloadImage) || (action is ActionRSS))
                    queues[3].Actions.Add(action);
                else if (action is ActionCopyMoveRename)
                    queues[(action as ActionCopyMoveRename).QuickOperation() ? 1 : 0].Actions.Add(action);
                else if ((action is ActionDeleteFile) || (action is ActionDeleteDirectory))
                    queues[1].Actions.Add(action);
                else
                {
#if DEBUG
                    System.Diagnostics.Debug.Fail("Unknown action type for making processing queue");
#endif
                    logger.Error("No action type found for {0}, Please follow up with a developer.", action.GetType());
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
                this.SemaphoreNumber = n;
                this.TheAction = a;
            }
        };

        #endregion

    }
}
