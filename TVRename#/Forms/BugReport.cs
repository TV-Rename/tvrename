// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using System.IO;

namespace TVRename
{
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
        private readonly TVDoc _mDoc;

        public BugReport(TVDoc doc)
        {
            _mDoc = doc;
            InitializeComponent();
        }

        private void bnCreate_Click(object sender, System.EventArgs e)
        {
            txtEmailText.Text = "Working... This may take a while.";
            txtEmailText.Update();

            StringBuilder txt = new StringBuilder();
            
            if (cbSettings.Checked)
            {
                txt.Append("==== Settings Files ====" + "\r\n");
                txt.Append("\r\n");
                txt.Append("---- TVRenameSettings.xml" + "\r\n");
                txt.Append("\r\n");
                try
                {
                    StreamReader sr = new StreamReader(PathManager.TVDocSettingsFile.FullName);
                    txt.Append(sr.ReadToEnd());
                    sr.Close();
                    txt.Append("\r\n");
                }
                catch
                {
                    txt.Append("Error reading TVRenameSettings.xml\r\n");
                }
                txt.Append("\r\n");
            }

            if (cbFOScan.Checked || cbFolderScan.Checked)
            {
                txt.Append("==== Filename processors ====\r\n");
                foreach (FilenameProcessorRe s in TVSettings.Instance.FnpRegexs)
                    txt.Append((s.Enabled ? "Enabled" : "Disabled") + " \"" + s.Re + "\" " + (s.UseFullPath ? "(FullPath)" : "") + "\r\n");
                txt.Append("\r\n");
            }

            if (cbFOScan.Checked)
            {
                txt.Append("==== Finding & Organising Directory Scan ====" + "\r\n");
                txt.Append("\r\n");

                DirCache dirC = new DirCache();
                foreach (string efi in _mDoc.SearchFolders)
                    dirC.AddFolder(null, 0, 0, efi, true);

                foreach (DirCacheEntry fi in dirC)
                {
                    int seas;
                    int ep;
                    bool r = TVDoc.FindSeasEp(fi.TheFile, out seas, out ep, null);
                    bool useful = fi.HasUsefulExtensionNotOthersToo;
                    txt.Append(fi.TheFile.FullName + " (" + (r ? "OK" : "No") + " " + seas + "," + ep + " " + (useful ? fi.TheFile.Extension : "-") + ")" + "\r\n");
                }
                txt.Append("\r\n");
            }

            if (cbFolderScan.Checked)
            {
                txt.Append("==== Media Folders Directory Scan ====" + "\r\n");

                foreach (ShowItem si in _mDoc.GetShowItems(true))
                {
                    foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                    {
                        int snum = kvp.Key;
                        if (((snum == 0) && (si.CountSpecials)) || !si.AllFolderLocations().ContainsKey(snum))
                            continue; // skip specials

                        foreach (string folder in si.AllFolderLocations()[snum])
                        {
                            txt.Append(si.TVDBCode + " : " + si.ShowName + " : S" + snum + "\r\n");
                            txt.Append("Folder: " + folder);
                            txt.Append("\r\n");
                            DirCache files = new DirCache();
                            if (Directory.Exists(folder))
                                files.AddFolder(null, 0, 0, folder, true);
                            foreach (DirCacheEntry fi in files)
                            {
                                int seas;
                                int ep;
                                bool r = TVDoc.FindSeasEp(fi.TheFile, out seas, out ep, si);
                                bool useful = fi.HasUsefulExtensionNotOthersToo;
                                txt.Append(fi.TheFile.FullName + " (" + (r ? "OK" : "No") + " " + seas + "," + ep + " " + (useful ? fi.TheFile.Extension : "-") + ")" + "\r\n");
                            }
                            txt.Append("\r\n");
                        }
                    }
                    txt.Append("\r\n");
                }
                _mDoc.UnlockShowItems();

                txt.Append("\r\n");
            }

            txtEmailText.Text = txt.ToString();
        }

        private void bnCopy_Click(object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(txtEmailText.Text);
        }

        private void linkForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helpers.SysOpen("https://groups.google.com/forum/#!forum/tvrename");   
        }
    }
}
