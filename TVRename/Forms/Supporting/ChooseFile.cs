// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Humanizer;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    public partial class ChooseFile : Form
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public enum ChooseFileDialogResult
        {
            left,right,ignore
        }

        public ChooseFileDialogResult Answer;
        private readonly FileInfo leftFile;
        private readonly FileInfo rightFile;

        public ChooseFile(FileInfo left, FileInfo right)
        {
            InitializeComponent();

            leftFile = left;
            txtNameLeft.Text = left.Name;
            int leftFrameWidth = left.GetFrameWidth();
            bool leftFrameUnknown = leftFrameWidth == -1;
            txtDimensionsLeft.Text = "Dimensions: " + (leftFrameUnknown ? "Unknown" : leftFrameWidth + "x" + left.GetFrameHeight());
            int leftFilmLength = left.GetFilmLength();
            txtLengthLeft.Text = "Length: " + ((leftFilmLength == -1) ? "Unknown" : leftFilmLength.Seconds().Humanize(2));
            txtSizeLeft.Text = GetFileSize(left);
            txtPathLeft.Text = left.DirectoryName;

            rightFile = right;
            lblNameRight.Text = right.Name;
            int rightFrameWidth = right.GetFrameWidth();
            bool rightFrameUnknown = rightFrameWidth == -1;
            lblDimensionsRight.Text = "Dimensions: " + (rightFrameUnknown ? "Unknown" : rightFrameWidth + "x" + right.GetFrameHeight());
            int rightFilmLength = right.GetFilmLength();
            lblLengthRight.Text = "Length: " + ((rightFilmLength == -1) ? "Unknown" : rightFilmLength.Seconds().Humanize(2));
            lblSizeRight.Text = GetFileSize(right);
            txtPathRight.Text = right.DirectoryName;

            SetBoldFileSize(left, right);

            SetBoldFilmLength(leftFilmLength, rightFilmLength);

            if (rightFrameUnknown || leftFrameUnknown) return;

            SetBoldFrameWidth(leftFrameWidth, rightFrameWidth);
        }

        private static string GetFileSize(FileInfo file)
        {
            try
            {
                return file.Length.Bytes().Humanize("#.#");
            }
            catch (FileNotFoundException fnfex)
            {
                Logger.Fatal(fnfex,$"Can't find File in ChooseFile called {file.Name}");
                return "Unknown";
            }
        }

        private void SetBoldFileSize(FileInfo left, FileInfo right)
        {
            if (left.Length > right.Length)
            {
                txtSizeLeft.Font = new Font(txtSizeLeft.Font.Name, txtSizeLeft.Font.Size, FontStyle.Bold);
                lblSizeRight.Font = new Font(lblSizeRight.Font.Name, lblSizeRight.Font.Size, FontStyle.Regular);
            }
            else if (left.Length < right.Length)
            {
                txtSizeLeft.Font = new Font(txtSizeLeft.Font.Name, txtSizeLeft.Font.Size, FontStyle.Regular);
                lblSizeRight.Font = new Font(lblSizeRight.Font.Name, lblSizeRight.Font.Size, FontStyle.Bold);
            }
            else
            {
                txtSizeLeft.Font = new Font(txtSizeLeft.Font.Name, txtSizeLeft.Font.Size, FontStyle.Regular);
                lblSizeRight.Font = new Font(lblSizeRight.Font.Name, lblSizeRight.Font.Size, FontStyle.Regular);
            }
        }

        private void SetBoldFilmLength(int leftFilmLength, int rightFilmLength)
        {
            if (leftFilmLength > rightFilmLength)
            {
                txtLengthLeft.Font = new Font(txtLengthLeft.Font.Name, txtLengthLeft.Font.Size, FontStyle.Bold);
                lblLengthRight.Font = new Font(lblLengthRight.Font.Name, lblLengthRight.Font.Size, FontStyle.Regular);
            }
            else if (leftFilmLength < rightFilmLength)
            {
                txtLengthLeft.Font = new Font(txtLengthLeft.Font.Name, txtLengthLeft.Font.Size, FontStyle.Regular);
                lblLengthRight.Font = new Font(lblLengthRight.Font.Name, lblLengthRight.Font.Size, FontStyle.Bold);
            }
            else
            {
                txtLengthLeft.Font = new Font(txtLengthLeft.Font.Name, txtLengthLeft.Font.Size, FontStyle.Regular);
                lblLengthRight.Font = new Font(lblLengthRight.Font.Name, lblLengthRight.Font.Size, FontStyle.Regular);
            }
        }

        private void SetBoldFrameWidth(int leftFrameWidth, int rightFrameWidth)
        {
            if (leftFrameWidth > rightFrameWidth)
            {
                txtDimensionsLeft.Font = new Font(txtDimensionsLeft.Font.Name, txtDimensionsLeft.Font.Size, FontStyle.Bold);
                lblDimensionsRight.Font = new Font(lblDimensionsRight.Font.Name, lblDimensionsRight.Font.Size, FontStyle.Regular);
            }
            else if (leftFrameWidth < rightFrameWidth)
            {
                txtDimensionsLeft.Font = new Font(txtDimensionsLeft.Font.Name, txtDimensionsLeft.Font.Size, FontStyle.Regular);
                lblDimensionsRight.Font = new Font(lblDimensionsRight.Font.Name, lblDimensionsRight.Font.Size, FontStyle.Bold);
            }
            else
            {
                txtDimensionsLeft.Font = new Font(txtDimensionsLeft.Font.Name, txtDimensionsLeft.Font.Size, FontStyle.Regular);
                lblDimensionsRight.Font = new Font(lblDimensionsRight.Font.Name, lblDimensionsRight.Font.Size, FontStyle.Regular);
            }
        }

        private void lnkOpenLeftFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helpers.SysOpen(leftFile.DirectoryName);
        }

        private void lnkOpenRightFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helpers.SysOpen(rightFile.DirectoryName);
        }

        private void btnLeft_Click(object sender, System.EventArgs e)
        {
            Answer = ChooseFileDialogResult.left;
            Close();
        }

        private void Ignore_Click(object sender, System.EventArgs e)
        {
            Answer = ChooseFileDialogResult.ignore;
            Close();
        }

        private void btnKeepRight_Click(object sender, System.EventArgs e)
        {
            Answer = ChooseFileDialogResult.right;
            Close();
        }
    }
}
