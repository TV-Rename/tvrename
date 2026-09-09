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
using System.Threading.Tasks;

namespace TVRename;

/// <summary>
/// Handles the multithreaded nature of actioning many actions at the same time. It will provide a UI to update the user on the status of the execution if required.
/// </summary>
public class ActionEngine(TVRenameStats stats)
{
    private readonly TVRenameStats mStats = stats; //reference to the main TVRenameStats, so we can update the counts
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private List<ActionQueue> actionWorkers = [];

    /// <summary>
    /// Asks for execution to pause
    /// </summary>
    public void Pause()
    {
        Logger.Info("Actions requested to be paused");
        actionWorkers.ForEach(x => x.Pause());
    }

    /// <summary>
    /// Asks for execution to resume
    /// </summary>
    public void Resume()
    {
        Logger.Info("Actions requested to be resumed");
        actionWorkers.ForEach(x => x.Resume());
    }

    /// <summary>
    /// Processes a set of actions, running them in a multi-threaded way based on the application's settings.
    /// </summary>
    /// <param name="theList">An ItemList to be processed.</param>
    /// <param name="token"></param>
    public async Task DoActionsAsync(ItemList theList, CancellationTokenSource token)
    {
        Logger.Info("**********************");
        Logger.Info($"Doing Selected Actions.... ({theList.Count} items detected, {theList.Actions.Count} actions to be completed )");

        // Run tasks in parallel (as much as is sensible)
        try
        {
            actionWorkers = ActionProcessorMakeQueues(theList, token);

            foreach (ActionQueue queue in actionWorkers)
            {
                Logger.Info($"Starting {queue}");
                queue.Start();
            }

            await Task.WhenAll(actionWorkers.Select(q => q.WaitForCompletionAsync()));

            theList.RemoveAll(x => x is Action { Outcome: { Done: true, Error: false } });

            foreach (Action slia in theList.Actions)
            {
                Logger.Warn(slia.Outcome.LastError, $"Failed to complete the following action: {slia.Name}, doing {slia}. Error was {slia.Outcome.LastError?.Message}");
            }

        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Unhandled Exception in ActionProcessor");
        }

        Logger.Info("Completed Selected Actions");
        Logger.Info("**************************");

    }
   
    private List<ActionQueue> ActionProcessorMakeQueues(ItemList theList, CancellationTokenSource cts)
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
        return EnumerableExtensions.GetAllItems<Action.QueueName>()
            .Select(q => new ActionQueue(GetName(q), GetParallelLimit(q), theList.GetActionsForQueue(q), mStats, cts))
            .ToList();
    }

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
}
