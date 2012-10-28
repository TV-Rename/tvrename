// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

// Saves the widths of columns in a listview, and window position, for loading up with the
// same layout next run.

namespace TVRename
{
    public enum WindowID
    {
        kUnknown = 0,
        kMDI = 1,
        kWatchFolders = 2,
        kUpcoming = 3,
        kRename = 4,
        kMonitorFolders = 5,
        kTorrentMatch = 6,
        kEPFinder = 7
    }

    public class LayoutInfo
    {
        public System.Collections.Generic.List<int> mColWidths;
        public Point mLocation;
        public bool mLocked;
        public bool mMaximised;
        public Size mSize;
        public WindowID mWindowID;

        public LayoutInfo()
        {
            this.mColWidths = new System.Collections.Generic.List<int>();
            this.mWindowID = WindowID.kUnknown;
            this.mMaximised = false;
            this.mSize = new Size(-1, -1);
            this.mLocation = new Point(-1, -1);
            this.mLocked = true;
        }

        public bool Locked()
        {
            return this.mLocked;
        }

        public void Unlock()
        {
            this.mLocked = false;
        }

        public void Lock()
        {
            this.mLocked = true;
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine("WindowID=" + (int) this.mWindowID);
            sw.WriteLine("Maximised=" + (this.mMaximised ? "1" : "0"));
            if (!this.mMaximised)
            {
                sw.WriteLine("Size=" + this.mSize.Width + " " + this.mSize.Height);
                sw.WriteLine("Location=" + this.mLocation.X + " " + this.mLocation.Y);
            }
            sw.Write("ColWidths=");
            for (int i = 0; i < this.mColWidths.Count; i++)
                sw.Write(this.mColWidths[i] + " ");
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
                            this.mColWidths.Add(n);
                        }
                        break;
                    case "Maximised":
                        this.mMaximised = l1 == "1";
                        break;
                    case "Size":
                        {
                            p = l1.IndexOf(' ');
                            int x = int.Parse(l1.Substring(0, p));
                            int y = int.Parse(l1.Substring(p + 1));
                            this.mSize = new System.Drawing.Size(x, y);
                        }
                        break;
                    case "Location":
                        {
                            p = l1.IndexOf(' ');
                            int x = int.Parse(l1.Substring(0, p));
                            int y = int.Parse(l1.Substring(p + 1));
                            this.mLocation = new Point(x, y);
                        }
                        break;
                    case "WindowID":
                        {
                            int n = int.Parse(l1);
                            this.mWindowID = (WindowID) n;
                        }
                        break;
                }
            }
            return false;
        }

        public void SetFrom(Form f, ListView lv)
        {
            if (this.mLocked)
                return;

            this.mLocked = true;

            if (f != null)
            {
                this.mSize = f.Size;
                this.mLocation = f.Location;
                this.mMaximised = f.WindowState == FormWindowState.Maximized;
            }

            this.mColWidths.Clear();
            if (lv != null)
            {
                for (int i = 0; i < lv.Columns.Count; i++)
                    this.mColWidths.Add(lv.Columns[i].Width);
            }

            this.mLocked = false;
        }

        public void Fixup(Form f, ListView lv)
        {
            if (f != null)
            {
                if (this.mSize != new Size(-1, -1))
                    f.Size = this.mSize;
                if (this.mLocation != new Point(-1, -1))
                    f.Location = this.mLocation;
                f.WindowState = this.mMaximised ? FormWindowState.Maximized : FormWindowState.Normal;
            }
            if (lv != null)
            {
                for (int i = 0; i < Math.Min(this.mColWidths.Count, lv.Columns.Count); i++)
                    lv.Columns[i].Width = this.mColWidths[i];
            }
        }
    }

    public class Layout
    {
        private System.Collections.Generic.List<LayoutInfo> mLayouts;

        public Layout()
        {
            this.mLayouts = new System.Collections.Generic.List<LayoutInfo>();
            this.Load();
        }

        public LayoutInfo Get(WindowID id)
        {
            for (int i = 0; i < this.mLayouts.Count; i++)
            {
                if (this.mLayouts[i].mWindowID == id)
                    return this.mLayouts[i];
            }
            LayoutInfo li = new LayoutInfo();
            li.mWindowID = id;
            this.mLayouts.Add(li);
            return li;
        }

        public void Save()
        {
            StreamWriter sw = new StreamWriter(PathManager.LayoutFile.FullName);
            sw.WriteLine("Version=2");

            for (int i = 0; i < this.mLayouts.Count; i++)
                this.mLayouts[i].Save(sw);

            sw.Close();
        }

        public void Load()
        {
            System.IO.StreamReader sr;
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
                    for (int i = 0; i < this.mLayouts.Count; i++)
                    {
                        if (this.mLayouts[i].mWindowID == li.mWindowID)
                        {
                            this.mLayouts.RemoveAt(i);
                            break;
                        }
                    }

                    this.mLayouts.Add(li);
                }
                else
                    break;
            }
            sr.Close();
        }
    }
}