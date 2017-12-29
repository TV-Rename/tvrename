// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

// Saves the widths of columns in a listview, and window position, for loading up with the
// same layout next run.

namespace TVRename
{
    public enum WindowId
    {
        KUnknown = 0,
        KMdi = 1,
        KWatchFolders = 2,
        KUpcoming = 3,
        KRename = 4,
        KMonitorFolders = 5,
        KTorrentMatch = 6,
        KEpFinder = 7
    }

    public class LayoutInfo
    {
        public System.Collections.Generic.List<int> MColWidths;
        public Point MLocation;
        public bool MLocked;
        public bool MMaximised;
        public Size MSize;
        public WindowId MWindowId;

        public LayoutInfo()
        {
            MColWidths = new System.Collections.Generic.List<int>();
            MWindowId = WindowId.KUnknown;
            MMaximised = false;
            MSize = new Size(-1, -1);
            MLocation = new Point(-1, -1);
            MLocked = true;
        }

        public bool Locked()
        {
            return MLocked;
        }

        public void Unlock()
        {
            MLocked = false;
        }

        public void Lock()
        {
            MLocked = true;
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine("WindowID=" + (int) MWindowId);
            sw.WriteLine("Maximised=" + (MMaximised ? "1" : "0"));
            if (!MMaximised)
            {
                sw.WriteLine("Size=" + MSize.Width + " " + MSize.Height);
                sw.WriteLine("Location=" + MLocation.X + " " + MLocation.Y);
            }
            sw.Write("ColWidths=");
            for (int i = 0; i < MColWidths.Count; i++)
                sw.Write(MColWidths[i] + " ");
            sw.WriteLine();
            sw.WriteLine("--");
        }

        public bool Load(StreamReader sr)
        {
            string l1;
            while ((l1 = sr.ReadLine()) != null)
            {
                if (l1 == "--")
                    return true;
                int p = l1.IndexOf('=');
                if (p == -1) 
                    continue;

                string what = l1.Substring(0, p);
                l1 = l1.Substring(p + 1);
               
                switch (what)
                {
                    case "ColWidths":
                        while ((p = l1.IndexOf(' ')) != -1)
                        {
                            int n = int.Parse(l1.Substring(0, p));
                            l1 = l1.Substring(p + 1);
                            MColWidths.Add(n);
                        }
                        break;
                    case "Maximised":
                        MMaximised = l1 == "1";
                        break;
                    case "Size":
                        {
                            p = l1.IndexOf(' ');
                            int x = int.Parse(l1.Substring(0, p));
                            int y = int.Parse(l1.Substring(p + 1));
                            MSize = new Size(x, y);
                        }
                        break;
                    case "Location":
                        {
                            p = l1.IndexOf(' ');
                            int x = int.Parse(l1.Substring(0, p));
                            int y = int.Parse(l1.Substring(p + 1));
                            MLocation = new Point(x, y);
                        }
                        break;
                    case "WindowID":
                        {
                            int n = int.Parse(l1);
                            MWindowId = (WindowId) n;
                        }
                        break;
                }
            }
            return false;
        }

        public void SetFrom(Form f, ListView lv)
        {
            if (MLocked)
                return;

            MLocked = true;

            if (f != null)
            {
                MSize = f.Size;
                MLocation = f.Location;
                MMaximised = f.WindowState == FormWindowState.Maximized;
            }

            MColWidths.Clear();
            if (lv != null)
            {
                for (int i = 0; i < lv.Columns.Count; i++)
                    MColWidths.Add(lv.Columns[i].Width);
            }

            MLocked = false;
        }

        public void Fixup(Form f, ListView lv)
        {
            if (f != null)
            {
                if (MSize != new Size(-1, -1))
                    f.Size = MSize;
                if (MLocation != new Point(-1, -1))
                    f.Location = MLocation;
                f.WindowState = MMaximised ? FormWindowState.Maximized : FormWindowState.Normal;
            }
            if (lv != null)
            {
                for (int i = 0; i < Math.Min(MColWidths.Count, lv.Columns.Count); i++)
                    lv.Columns[i].Width = MColWidths[i];
            }
        }
    }

    public class Layout
    {
        private readonly System.Collections.Generic.List<LayoutInfo> _mLayouts;

        public Layout()
        {
            _mLayouts = new System.Collections.Generic.List<LayoutInfo>();
            Load();
        }

        public LayoutInfo Get(WindowId id)
        {
            for (int i = 0; i < _mLayouts.Count; i++)
            {
                if (_mLayouts[i].MWindowId == id)
                    return _mLayouts[i];
            }
            LayoutInfo li = new LayoutInfo();
            li.MWindowId = id;
            _mLayouts.Add(li);
            return li;
        }

        public void Save()
        {
            StreamWriter sw = new StreamWriter(PathManager.LayoutFile.FullName);
            sw.WriteLine("Version=2");

            for (int i = 0; i < _mLayouts.Count; i++)
                _mLayouts[i].Save(sw);

            sw.Close();
        }

        public void Load()
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader(PathManager.LayoutFile.FullName);
            }
            catch
            {
                return;
            }
            string l1 = sr.ReadLine();
            if (l1 == null)
                return;

            int p = l1.IndexOf('=');
            if (p == -1)
                return;

            string what = l1.Substring(0, p);
            l1 = l1.Substring(p + 1);
            if ((what != "Version") && (l1 != "2"))
                return;

            for (;;)
            {
                LayoutInfo li = new LayoutInfo();
                if (li.Load(sr))
                {
                    for (int i = 0; i < _mLayouts.Count; i++)
                    {
                        if (_mLayouts[i].MWindowId == li.MWindowId)
                        {
                            _mLayouts.RemoveAt(i);
                            break;
                        }
                    }

                    _mLayouts.Add(li);
                }
                else
                    break;
            }
            sr.Close();
        }
    }
}
