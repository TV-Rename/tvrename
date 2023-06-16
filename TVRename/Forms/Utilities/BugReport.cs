//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TVRename;

/// <summary>
/// Summary for BugReport
///
/// WARNING: If you change the name of this class, you will need to change the
///          'Resource File Name' property for the managed resource compiler tool
///          associated with all .resx files this class depends on.  Otherwise,
///          the designers will not be able to interact properly with localized
///          resources associated with this form.
/// </summary>
public partial class BugReport : Form
{
    private readonly TVDoc mDoc;

    public BugReport(TVDoc doc)
    {
        mDoc = doc;
        InitializeComponent();
    }

    private void bnCreate_Click(object sender, System.EventArgs e)
    {
        txtEmailText.Text = "Working... This may take a while.";
        txtEmailText.Update();

        StringBuilder txt = new();

        if (cbSettings.Checked)
        {
            txt.AppendLine("==== Settings Files ====");
            txt.AppendLine();
            txt.AppendLine("---- TVRenameSettings.xml");
            txt.AppendLine();
            OutputSettingsFile(txt);
            txt.AppendLine("");
        }

        if (cbFOScan.Checked || cbFolderScan.Checked)
        {
            txt.AppendLine("==== Filename processors ====");
            OutputFilenameProcessors(txt);
            txt.AppendLine();
        }

        if (cbFOScan.Checked)
        {
            txt.AppendLine("==== Finding & Organising Directory Scan ====");
            txt.AppendLine();
            ExtractDownloadFolders(txt);
            txt.AppendLine();
        }

        if (cbFolderScan.Checked)
        {
            txt.AppendLine("==== Media Folders Directory Scan ====");
            ExtractShowDetails(txt);
            txt.AppendLine();
        }

        txtEmailText.Text = txt.ToString().ToUiVersion();
    }

    private void ExtractShowDetails(StringBuilder txt)
    {
        foreach (ShowConfiguration si in mDoc.TvLibrary.GetSortedShowItems())
        {
            foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.ActiveSeasons)
            {
                int snum = kvp.Key;
                if (snum == 0 && !si.CountSpecials)
                {
                    continue; // skip specials
                }

                if (snum == 0 && TVSettings.Instance.IgnoreAllSpecials)
                {
                    continue;
                }

                if (!si.AllExistngFolderLocations().TryGetValue(snum, out SafeList<string>? folders))
                {
                    continue; // skip non seen seasons
                }

                foreach (string folder in folders)
                {
                    txt.AppendLine(si + " : " + si.ShowName + " : S" + snum);
                    txt.AppendLine("Folder: " + folder);

                    DirCache files = new();
                    if (Directory.Exists(folder))
                    {
                        files.AddFolder(null, 0, 0, folder, true);
                    }

                    foreach (DirCacheEntry fi in files)
                    {
                        bool r = FinderHelper.FindSeasEp(fi.TheFile, out int seas, out int ep, out int maxEp, si);
                        bool useful = fi.TheFile.IsMovieFile();
                        txt.AppendLine(fi.TheFile.FullName + " (" + (r ? "OK" : "No") + " " + seas + "," + ep + "," +
                                       maxEp + " " + (useful ? fi.TheFile.Extension : "-") + ")");
                    }

                    txt.AppendLine();
                }
            }

            txt.AppendLine();
        }
    }

    private static void ExtractDownloadFolders(StringBuilder txt)
    {
        DirCache dirC = new();
        foreach (string efi in TVSettings.Instance.DownloadFolders)
        {
            dirC.AddFolder(null, 0, 0, efi, true);
        }

        foreach (DirCacheEntry fi in dirC)
        {
            bool r = FinderHelper.FindSeasEp(fi.TheFile, out int seas, out int ep, out int maxEp, null);
            bool useful = fi.TheFile.IsMovieFile();
            txt.AppendLine(fi.TheFile.FullName + " (" + (r ? "OK" : "No") + " " + seas + "," + ep + "," + maxEp + " " +
                           (useful ? fi.TheFile.Extension : "-") + ")");
        }
    }

    private static void OutputFilenameProcessors(StringBuilder txt)
    {
        foreach (TVSettings.FilenameProcessorRE s in TVSettings.Instance.FNPRegexs)
        {
            txt.AppendLine((s.Enabled ? "Enabled " : "Disabled ") + s.RegExpression.InDoubleQuotes() +
                           (s.UseFullPath ? " (FullPath)" : string.Empty));
        }
    }

    private static void OutputSettingsFile(StringBuilder txt)
    {
        try
        {
            using System.IO.StreamReader sr = new(PathManager.TVDocSettingsFile.FullName);
            txt.AppendLine(sr.ReadToEnd());
        }
        catch
        {
            txt.AppendLine("Error reading TVRenameSettings.xml");
        }
    }

    private void bnCopy_Click(object sender, System.EventArgs e)
    {
        Clipboard.SetDataObject(txtEmailText.Text);
    }

    private void linkForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        "https://groups.google.com/forum/#!forum/tvrename".OpenUrlInBrowser();
    }
}
