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
            Episode = pe;
            SI = si;
            Destination = dest;
            Path = path;
            ShrinkLargeMede8erImage = mede8erShrink;
        }

        #region Action Members

        public override string Name => "Download";

        public override string ProgressText => Destination.Name;

        public override string Produces => Destination.FullName;

        // 0 to 100
        public override long SizeOfWork => 1000000;

        // http://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
        private static Image MaxSize(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            float nPercentW = (width / (float)sourceWidth);
            float nPercentH = (height / (float)sourceHeight);

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
            byte[] theData = TheTVDB.Instance.GetTvdbDownload(Path);
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
                if (Episode == null)
                {
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
                else {
                    if ((im.Width > 232) || (im.Height > 156))
                    {
                        im = MaxSize(im, 232, 156);

                        using (MemoryStream m = new MemoryStream())
                        {
                            im.Save(m, ImageFormat.Jpeg);
                            theData = m.ToArray();
                        }
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

        public override bool SameAs(Item o)
        {
            return (o is ActionDownloadImage image) && (image.Destination == Destination);
        }

        public override int Compare(Item o)
        {
            return !(o is ActionDownloadImage dl) ? 0 : Destination.FullName.CompareTo(dl.Destination.FullName);
        }

        #endregion

        #region Item Members

        public override int IconNumber => 5;

        public override IgnoreItem Ignore => GenerateIgnore(Destination?.FullName);
        
        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem {
                                                        Text = (Episode != null) ? Episode.Show.ShowName : ((SI != null) ? SI.ShowName : "")
                                                    };

                lvi.SubItems.Add(Episode?.AppropriateSeasonNumber.ToString() ?? "");
                lvi.SubItems.Add(Episode?.NumsAsString() ?? "");
                lvi.SubItems.Add(Episode != null ? Episode.GetAirDateDT(true).PrettyPrint() : "");
                lvi.SubItems.Add(Destination.DirectoryName);
                lvi.SubItems.Add(Path);

                if (string.IsNullOrEmpty(Path))
                    lvi.BackColor = Helpers.WarningColor();

                lvi.SubItems.Add(Destination.Name);

                lvi.Tag = this;

                return lvi;
            }
        }

        public override string ScanListViewGroup => "lvgActionDownload";

        public override string TargetFolder
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
