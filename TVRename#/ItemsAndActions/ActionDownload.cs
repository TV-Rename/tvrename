// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    using System;
    using System.Windows.Forms;
    using FileInfo = FileInfo;
    using System.IO;

    public class ActionDownload : Item, Action, ScanListItem
    {
        private readonly string Path;
        private readonly FileInfo Destination;
        private readonly ShowItem SI;
        private readonly bool ShrinkLargeMede8erImage;

        public ActionDownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string path) : this(si, pe, dest, path, false) { }

        public ActionDownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string path, bool mede8erShrink)
        {
            Episode = pe;
            SI = si;
            Destination = dest;
            Path = path;
            ShrinkLargeMede8erImage = mede8erShrink;
        }

        #region Action Members

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }

        public string Name
        {
            get { return "Download"; }
        }

        public string ProgressText
        {
            get { return Destination.Name; }
        }

        public double PercentDone
        {
            get { return Done ? 100 : 0; }
        }

        public string produces
        {
            get { return Destination.FullName; }
        }

        // 0 to 100
        public long SizeOfWork
        {
            get { return 1000000; }
        }

        // http://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
        static Image MaxSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            float nPercentW = ((float)Width / (float)sourceWidth);
            float nPercentH = ((float)Height / (float)sourceHeight);

            //float nPercent = Math.Min(nPercentH, nPercentW);
            int destWidth, destHeight;

            if (nPercentH < nPercentW)
            {
                destHeight = Height;
                destWidth = (int)(sourceWidth * nPercentH);
            }
            else
            {
                destHeight = (int)(sourceHeight * nPercentW);
                destWidth = Width;
            }

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(0, 0, destWidth, destHeight),
                new Rectangle(0, 0, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public bool Go(ref bool pause, TVRenameStats stats)
        {
            byte[] theData = TheTVDB.Instance.GetTVDBDownload(Path);
            if ((theData == null) || (theData.Length == 0))
            {
                ErrorText = "Unable to download " + Path;
                Error = true;
                Done = true;
                return false;
            }

            if (ShrinkLargeMede8erImage)
            {
                // shrink images down to a maximum size of 156x232
                Image im = new Bitmap(new MemoryStream(theData));
                if ((im.Width > 156) || (im.Height > 232))
                {
                    im = MaxSize(im, 156, 232);

                    using (MemoryStream m = new MemoryStream())
                    {
                        im.Save(m, ImageFormat.Jpeg);
                        theData = m.ToArray();
                    }
                }
            }

            try
            {
                FileStream fs = new FileStream(Destination.FullName, FileMode.Create);
                fs.Write(theData, 0, theData.Length);
                fs.Close();
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                Error = true;
                Done = true;
                return false;
            }
                

            Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ActionDownload) && ((o as ActionDownload).Destination == Destination);
        }

        public int Compare(Item o)
        {
            ActionDownload dl = o as ActionDownload;
            return dl == null ? 0 : Destination.FullName.CompareTo(dl.Destination.FullName);
        }

        #endregion

        #region ScanListItem Members

        public int IconNumber
        {
            get { return 5; }
        }

        public ProcessedEpisode Episode { get; set; }

        public IgnoreItem Ignore
        {
            get
            {
                if (Destination == null)
                    return null;
                return new IgnoreItem(Destination.FullName);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem {
                                                        Text = (Episode != null) ? Episode.SI.ShowName : ((SI != null) ? SI.ShowName : "")
                                                    };

                lvi.SubItems.Add(Episode != null ? Episode.SeasonNumber.ToString() : "");
                lvi.SubItems.Add(Episode != null ? Episode.NumsAsString() : "");

                if (Episode != null)
                {
                    DateTime? dt = Episode.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(Destination.DirectoryName);
                lvi.SubItems.Add(Path);

                if (string.IsNullOrEmpty(Path))
                    lvi.BackColor = Helpers.WarningColor();

                lvi.SubItems.Add(Destination.Name);

                lvi.Tag = this;

                return lvi;
            }
        }

        public string ScanListViewGroup
        {
            get { return "lvgActionDownload"; }
        }

        public string TargetFolder
        {
            get
            {
                if (Destination == null)
                    return null;
                return Destination.DirectoryName;
            }
        }

        #endregion
    }
}
