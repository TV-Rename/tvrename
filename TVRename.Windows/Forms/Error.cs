using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TVRename.Windows.Forms
{
    public partial class Error : Form
    {
        public Error(Exception exception)
        {
            InitializeComponent();

            this.textBoxTrace.Text = exception.Message + Environment.NewLine + Environment.NewLine + exception.StackTrace;
        }

        private void buttonReport_Click(object sender, EventArgs e)
        {
            string title = "Crash Report";
            string body = $"```{Environment.NewLine}{this.textBoxTrace.Text}{Environment.NewLine}```"; // TODO: Too long for URL?

            Process.Start($"https://github.com/TV-Rename/tvrename/issues/new?title={Uri.EscapeDataString(title)}&body={Uri.EscapeDataString(body)}");
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();

            Environment.Exit(1);
        }
    }
}
