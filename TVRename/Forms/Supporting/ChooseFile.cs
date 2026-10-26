//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Humanizer;

namespace TVRename;

public partial class ChooseFile : Form
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public enum ChooseFileDialogResult
    {
        left, right, ignore
    }

    public ChooseFileDialogResult Answer;
    private readonly FileInfo leftFile;
    private readonly FileInfo rightFile;

    public ChooseFile(FileInfo left, FileInfo right)
    {
        InitializeComponent();

        leftFile = left;
        rightFile = right;
    }
    public async Task Setup()
    { 
        (int? leftFrameWidth, bool leftFrameUnknown, TimeSpan? leftFilmLength) = await UpdateFieldsAsync(leftFile, txtNameLeft, txtDimensionsLeft, txtLengthLeft, txtSizeLeft, txtPathLeft);
        (int? rightFrameWidth, bool rightFrameUnknown, TimeSpan? rightFilmLength) = await UpdateFieldsAsync(rightFile, lblNameRight, lblDimensionsRight, lblLengthRight, lblSizeRight, txtPathRight);

        SetBoldFileSize(leftFile, rightFile);

        SetBoldFilmLength(leftFilmLength, rightFilmLength);

        if (rightFrameUnknown || leftFrameUnknown)
        {
            return;
        }

        SetBoldFrameWidth(leftFrameWidth, rightFrameWidth);
    }

    private static async Task<(int?, bool, TimeSpan?)> UpdateFieldsAsync(FileInfo file, Label nameLabel, Label dimensionsLabel, Label lengthLabel, Label sizeLabel, Label pathLabel)
    {
        nameLabel.Text = file.Name.ToUiVersion();
        int? leftFrameWidth = await file.GetFrameWidthAsync();
        bool leftFrameUnknown = !leftFrameWidth.HasValue;
        dimensionsLabel.Text = "Dimensions: " + (leftFrameUnknown ? "Unknown" : leftFrameWidth + "x" + await file.GetFrameHeightAsync());
        TimeSpan? leftFilmLength = await file.GetFilmLengthAsync();

        try
        {
            lengthLabel.Text =
                "Length: " + (!leftFilmLength.HasValue ? "Unknown" : leftFilmLength.Value.Humanize());
        }
        catch (ArgumentException) //Issue in Humanizer causes this in Polish
        {
            lengthLabel.Text =
                "Length: " + (!leftFilmLength.HasValue ? "Unknown" : leftFilmLength.Value.TotalSeconds + " s");
        }

        sizeLabel.Text = GetFileSize(file).ToUiVersion();
        pathLabel.Text = file.DirectoryName.ToUiVersion();

        return (leftFrameWidth, leftFrameUnknown, leftFilmLength);
    }

    private static string GetFileSize(FileInfo file) => file.Length.Bytes().Humanize("#.#");

    private void SetBoldFileSize(FileInfo left, FileInfo right)
    {
        try
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
        catch (System.IO.IOException)
        {
            Logger.Warn($"Can't find File in ChooseFile called {left.Name} or {right.Name}");
        }
    }

    private void SetBoldFilmLength(TimeSpan? leftFilmLength, TimeSpan? rightFilmLength)
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

    private void SetBoldFrameWidth(int? leftFrameWidth, int? rightFrameWidth)
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
        leftFile.FullName.OpenFolderSelectFile();
    }

    private void lnkOpenRightFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        rightFile.FullName.OpenFolderSelectFile();
    }

    private void btnLeft_Click(object sender, EventArgs e)
    {
        Answer = ChooseFileDialogResult.left;
        Close();
    }

    private void Ignore_Click(object sender, EventArgs e)
    {
        Answer = ChooseFileDialogResult.ignore;
        Close();
    }

    private void btnKeepRight_Click(object sender, EventArgs e)
    {
        Answer = ChooseFileDialogResult.right;
        Close();
    }
}
