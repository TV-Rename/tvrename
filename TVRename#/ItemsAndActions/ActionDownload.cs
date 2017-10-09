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


namespace TVRename
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    

    public class ActionDownload : Item, Action, ScanListItem
    {
        private readonly string Path;
        private readonly FileInfo Destination;
        private readonly ShowItem SI;
        private readonly bool ShrinkLargeMede8erImage;

        public ActionDownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string path) : this(si, pe, dest, path, false) { }

        public ActionDownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string path, bool mede8erShrink)
        {
            this.Episode = pe;
            this.SI = si;
            this.Destination = dest;
            this.Path = path;
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
            get { return this.Destination.Name; }
        }

        public double PercentDone
        {
            get { return this.Done ? 100 : 0; }
        }

        public string produces
        {
            get { return this.Destination.FullName; }
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
            byte[] theData = TheTVDB.Instance.GetTVDBDownload(this.Path);
            if ((theData == null) || (theData.Length == 0))
            {
                this.ErrorText = "Unable to download " + this.Path;
                this.Error = true;
                this.Done = true;
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
                FileStream fs = new FileStream(this.Destination.FullName, FileMode.Create);
                fs.Write(theData, 0, theData.Length);
                fs.Close();
            }
            catch (Exception e)
            {
                this.ErrorText = e.Message;
                this.Error = true;
                this.Done = true;
                return false;
            }
                

            this.Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ActionDownload) && ((o as ActionDownload).Destination == this.Destination);
        }

        public int Compare(Item o)
        {
            ActionDownload dl = o as ActionDownload;
            return dl == null ? 0 : this.Destination.FullName.CompareTo(dl.Destination.FullName);
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
                if (this.Destination == null)
                    return null;
                return new IgnoreItem(this.Destination.FullName);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem {
                                                        Text = (this.Episode != null) ? this.Episode.SI.ShowName : ((this.SI != null) ? this.SI.ShowName : "")
                                                    };

                lvi.SubItems.Add(this.Episode != null ? this.Episode.SeasonNumber.ToString() : "");
                lvi.SubItems.Add(this.Episode != null ? this.Episode.NumsAsString() : "");

                if (this.Episode != null)
                {
                    DateTime? dt = this.Episode.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(this.Destination.DirectoryName);
                lvi.SubItems.Add(this.Path);

                if (string.IsNullOrEmpty(this.Path))
                    lvi.BackColor = Helpers.WarningColor();

                lvi.SubItems.Add(this.Destination.Name);

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
                if (this.Destination == null)
                    return null;
                return this.Destination.DirectoryName;
            }
        }

        #endregion
    }
}