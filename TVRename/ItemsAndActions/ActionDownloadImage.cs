// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using JetBrains.Annotations;

namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;

    public class ActionDownloadImage : ActionDownload
    {
        private readonly string path;
        private readonly FileInfo destination;
        private readonly ShowItem si;
        private readonly bool shrinkLargeMede8ErImage;

        public ActionDownloadImage(ShowItem si, ProcessedEpisode pe, FileInfo dest, string path) : this(si, pe, dest, path, false) { }

        public ActionDownloadImage(ShowItem si, ProcessedEpisode pe, FileInfo dest, string path, bool shrink)
        {
            Episode = pe;
            this.si = si;
            destination = dest;
            this.path = path;
            shrinkLargeMede8ErImage = shrink;
        }

        #region Action Members

        [NotNull]
        public override string Name => "Download";

        public override string ProgressText => destination.Name;

        public override string Produces => destination.FullName;

        // 0 to 100
        public override long SizeOfWork => 1000000;

        // http://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
        [NotNull]
        private static Image MaxSize([NotNull] Image imgPhoto, int width, int height)
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
            byte[] theData = TheTVDB.Instance.GetTvdbDownload(path);
            if ((theData is null) || (theData.Length == 0))
            {
                ErrorText = "Unable to download " + path;
                Error = true;
                Done = true;
                return false;
            }

            if (shrinkLargeMede8ErImage)
            {
                // shrink images down to a maximum size of 156x232
                Image im = new Bitmap(new System.IO.MemoryStream(theData));
                if (Episode is null)
                {
                    if ((im.Width > 156) || (im.Height > 232))
                    {
                        im = MaxSize(im, 156, 232);

                        using (System.IO.MemoryStream m = new System.IO.MemoryStream())
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

                        using (System.IO.MemoryStream m = new System.IO.MemoryStream())
                        {
                            im.Save(m, ImageFormat.Jpeg);
                            theData = m.ToArray();
                        }
                    }
                }
            }

            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(destination.FullName, System.IO.FileMode.Create);
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
            return (o is ActionDownloadImage image) && (image.destination == destination);
        }

        public override int Compare(Item o)
        {
            return !(o is ActionDownloadImage dl) ? 0 : string.Compare(destination.FullName, dl.destination.FullName, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        public override int IconNumber => 5;

        [CanBeNull]
        public override IgnoreItem Ignore => GenerateIgnore(destination?.FullName);

        protected override string SeriesName =>
            (Episode != null) ? Episode.Show.ShowName : ((si != null) ? si.ShowName : "");

        [CanBeNull]
        protected override string DestinationFolder => TargetFolder;
        protected override string DestinationFile => destination.Name;
        protected override string SourceDetails => path;
        protected override bool InError => string.IsNullOrEmpty(path);
        [NotNull]
        public override string ScanListViewGroup => "lvgActionDownload";
        [CanBeNull]
        public override string TargetFolder => destination is null ? null : destination.DirectoryName;
        #endregion
    }
}
