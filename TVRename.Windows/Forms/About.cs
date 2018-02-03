using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TVRename.Windows.Utilities;

namespace TVRename.Windows.Forms
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            this.labelVersion.Text += $" {Helpers.DisplayVersion}";

            // Make the logo black
            Bitmap bmp = new Bitmap(this.pictureBoxLogo.Image);

            for (int y = 0; y <= bmp.Height - 1; y++)
            {
                for (int x = 0; x <= bmp.Width - 1; x++)
                {
                    if (bmp.GetPixel(x, y).Name == "0") continue;

                    bmp.SetPixel(x, y, Color.Black);
                }
            }

            this.pictureBoxLogo.Image = bmp;
        }
        
        private void linkLabelSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/TV-Rename/tvrename");
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
