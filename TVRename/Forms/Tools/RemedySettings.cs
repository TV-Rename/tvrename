using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TVRename.Forms.Tools;

namespace TVRename.Forms;

internal class RemedySettings : LongOperation
{
    private readonly IEnumerable<SettingsCheck> selectedItems;
    private readonly SettingsReview parent;

    public RemedySettings(IEnumerable<SettingsCheck> selectedItems,SettingsReview parent)
    {
        this.selectedItems = selectedItems;
        this.parent = parent;
    }

    public override void Start(CancellationToken sourceToken, SetProgressDelegate? progress)
    {
        int currentRecord = 0;
        int totalRecords = selectedItems.Count();
        progress?.Invoke(0, "Fixing Issues", string.Empty);

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
            int position = 100* currentRecord++ / (totalRecords + 1);
            progress?.Invoke(position, selected.CheckName, selected.MediaName);
        }

        progress?.Invoke(100, "Completed", string.Empty);
    }
}
