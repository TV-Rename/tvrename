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
            mColWidths = new System.Collections.Generic.List<int>();
            mWindowID = WindowID.kUnknown;
            mMaximised = false;
            mSize = new Size(-1, -1);
            mLocation = new Point(-1, -1);
            mLocked = true;
        }

        public bool Locked()
        {
            return mLocked;
        }

        public void Unlock()
        {
            mLocked = false;
        }

        public void Lock()
        {
            mLocked = true;
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine("WindowID=" + (int) mWindowID);
            sw.WriteLine("Maximised=" + (mMaximised ? "1" : "0"));
            if (!mMaximised)
            {
                sw.WriteLine("Size=" + mSize.Width + " " + mSize.Height);
                sw.WriteLine("Location=" + mLocation.X + " " + mLocation.Y);
            }
            sw.Write("ColWidths=");
            for (int i = 0; i < mColWidths.Count; i++)
                sw.Write(mColWidths[i] + " ");
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
                            mColWidths.Add(n);
                        }
                        break;
                    case "Maximised":
                        mMaximised = l1 == "1";
                        break;
                    case "Size":
                        {
                            p = l1.IndexOf(' ');
                            int x = int.Parse(l1.Substring(0, p));
                            int y = int.Parse(l1.Substring(p + 1));
                            mSize = new Size(x, y);
                        }
                        break;
                    case "Location":
                        {
                            p = l1.IndexOf(' ');
                            int x = int.Parse(l1.Substring(0, p));
                            int y = int.Parse(l1.Substring(p + 1));
                            mLocation = new Point(x, y);
                        }
                        break;
                    case "WindowID":
                        {
                            int n = int.Parse(l1);
                            mWindowID = (WindowID) n;
                        }
                        break;
                }
            }
            return false;
        }

        public void SetFrom(Form f, ListView lv)
        {
            if (mLocked)
                return;

            mLocked = true;

            if (f != null)
            {
                mSize = f.Size;
                mLocation = f.Location;
                mMaximised = f.WindowState == FormWindowState.Maximized;
            }

            mColWidths.Clear();
            if (lv != null)
            {
                for (int i = 0; i < lv.Columns.Count; i++)
                    mColWidths.Add(lv.Columns[i].Width);
            }

            mLocked = false;
        }

        public void Fixup(Form f, ListView lv)
        {
            if (f != null)
            {
                if (mSize != new Size(-1, -1))
                    f.Size = mSize;
                if (mLocation != new Point(-1, -1))
                    f.Location = mLocation;
                f.WindowState = mMaximised ? FormWindowState.Maximized : FormWindowState.Normal;
            }
            if (lv != null)
            {
                for (int i = 0; i < Math.Min(mColWidths.Count, lv.Columns.Count); i++)
                    lv.Columns[i].Width = mColWidths[i];
            }
        }
    }

    public class Layout
    {
        private System.Collections.Generic.List<LayoutInfo> mLayouts;

        public Layout()
        {
            mLayouts = new System.Collections.Generic.List<LayoutInfo>();
            Load();
        }

        public LayoutInfo Get(WindowID id)
        {
            for (int i = 0; i < mLayouts.Count; i++)
            {
                if (mLayouts[i].mWindowID == id)
                    return mLayouts[i];
            }
            LayoutInfo li = new LayoutInfo();
            li.mWindowID = id;
            mLayouts.Add(li);
            return li;
        }

        public void Save()
        {
            StreamWriter sw = new StreamWriter(PathManager.LayoutFile.FullName);
            sw.WriteLine("Version=2");

            for (int i = 0; i < mLayouts.Count; i++)
                mLayouts[i].Save(sw);

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
                    for (int i = 0; i < mLayouts.Count; i++)
                    {
                        if (mLayouts[i].mWindowID == li.mWindowID)
                        {
                            mLayouts.RemoveAt(i);
                            break;
                        }
                    }

                    mLayouts.Add(li);
                }
                else
                    break;
            }
            sr.Close();
        }
    }
}
