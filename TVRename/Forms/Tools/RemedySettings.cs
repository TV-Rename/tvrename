using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TVRename.Forms.Tools;

namespace TVRename.Forms;

internal class RemedySettings(IEnumerable<SettingsCheck> selectedItems, SettingsReview parent) : LongOperation
{
    private readonly IEnumerable<SettingsCheck> selectedItems = selectedItems;
    private readonly SettingsReview parent = parent;

    public override async Task StartAsync(IProgress<TaskProgress> progress, CancellationToken sourceToken)
    {
        ThreadSafeCounter currentRecord = new();
        int totalRecords = selectedItems.Count();
        progress.Report(new TaskProgress(0, "Fixing Issues", string.Empty));
        
        foreach (SettingsCheck selected in selectedItems)
        {
            if (sourceToken.IsCancellationRequested)
            {
                selected.Cancel();
            }
            else
            {
                selected.Fix();
                if (!selected.IsError)
                {
                    parent.Remove(selected);
                }
            }
            int position = 100 * currentRecord.Increment() / (totalRecords + 1);
            
            progress.Report(new TaskProgress(position, selected.CheckName, selected.MediaName));
        }

        progress.Report(new TaskProgress(100, "Completed",string.Empty));
    }
}
