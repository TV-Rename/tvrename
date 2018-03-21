// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace TVRename
{
    using System;
    using System.Windows.Forms;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
    using System.IO;

    public class ActionDownloadImage : ActionDownload
    {
        private readonly string Path;
        private readonly FileInfo Destination;
        private readonly ShowItem SI;
        private readonly bool ShrinkLargeMede8erImage;

        public ActionDownloadImage(ShowItem si, ProcessedEpisode pe, FileInfo dest, string path) : this(si, pe, dest, path, false) { }

        public ActionDownloadImage(ShowItem si, ProcessedEpisode pe, FileInfo dest, string path, bool mede8erShrink)
        {
            this.Episode = pe;
            this.SI = si;
            this.Destination = dest;
            this.Path = path;
            this.ShrinkLargeMede8erImage = mede8erShrink;
        }

        #region Action Members

        public override string Name => "Download";

        public override string ProgressText => this.Destination.Name;

        public override string Produces => this.Destination.FullName;

        // 0 to 100
        public override long SizeOfWork => 1000000;

        // http://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
        static Image MaxSize(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            float nPercentW = (width / (float)sourceWidth);
            float nPercentH = (height / (float)sourceHeight);

            //float nPercent = Math.Min(nPercentH, nPercentW);
            int destWidth, destHeight;

            if (nPercentH < nPercentW)
            {
                destHeight = height;
                destWidth = (int)(sourceWidth * nPercentH);
            }
            else
            {
                destHeight = (int)(sourceHeight * nPercentW);
                destWidth = width;
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

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            byte[] theData = TheTVDB.Instance.GetTVDBDownload(this.Path);
            if ((theData == null) || (theData.Length == 0))
            {
                this.ErrorText = "Unable to download " + this.Path;
                this.Error = true;
                this.Done = true;
                return false;
            }

            if (this.ShrinkLargeMede8erImage)
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

        public override bool SameAs(Item o)
        {
            return (o is ActionDownloadImage image) && (image.Destination == this.Destination);
        }

        public override int Compare(Item o)
        {
            return !(o is ActionDownloadImage dl) ? 0 : this.Destination.FullName.CompareTo(dl.Destination.FullName);
        }

        #endregion

        #region Item Members

        public override int IconNumber => 5;

        public override IgnoreItem Ignore
        {
            get
            {
                if (this.Destination == null)
                    return null;
                return new IgnoreItem(this.Destination.FullName);
            }
        }

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem {
                                                        Text = (this.Episode != null) ? this.Episode.SI.ShowName : ((this.SI != null) ? this.SI.ShowName : "")
                                                    };

                lvi.SubItems.Add(this.Episode?.AppropriateSeasonNumber.ToString() ?? "");
                lvi.SubItems.Add(this.Episode?.NumsAsString() ?? "");

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

        public override string ScanListViewGroup => "lvgActionDownload";

        public override string TargetFolder
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
