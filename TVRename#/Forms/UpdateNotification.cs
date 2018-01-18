using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TVRename.Forms
{
    public partial class UpdateNotification : Form
    {
        private readonly UpdateVersion newVersion;


        public UpdateNotification(UpdateVersion update)
        {
            this.newVersion = update;
            InitializeComponent();
            this.tbReleaseNotes.Text = this.newVersion.ReleaseNotesText;
            this.lblStatus.Text = $@"There is new version {update.VersionNumber}-{update.Prerelease} available since "+ update.ReleaseDate.ToLocalTime() + ".";

            //If this call is slow then we can put it in a new thread and update the control as it comes back from GH
            UpdateWithMarkdown();
        }

        private void UpdateWithMarkdown()
        {
            const string GITHUB_CONVERSION_URL = "https://api.github.com/markdown";

            //string responsebody;

            HttpWebRequest req = WebRequest.Create(new Uri(GITHUB_CONVERSION_URL)) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/json";
            req.UserAgent= 
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

            JObject request = new JObject();
            request.Add("text", this.newVersion.ReleaseNotesText);
            request.Add("mode", "gfm");
            request.Add("context", "TV-Rename/tvrename");
            
            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            writer.Write(request.ToString());
            writer.Close();

            string result = null;
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                StreamReader reader =
                    new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }

            string HTML_HEAD = "<html><head><style type=\"text/css\">* {font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Helvetica, Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\"; font-size:90%}</style><base target=\"_blank\"></head><body>";
            string HTML_FOOTER = "</body></html>";

            this.webReleaseNotes.DocumentText= HTML_HEAD+result+HTML_FOOTER;
            


            this.webReleaseNotes.Visible = true;
            this.tbReleaseNotes.Visible = false;

        }

        private void bnReleaseNotes_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen(this.newVersion.ReleaseNotesUrl);
        }

        private void btnDownloadNow_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen(this.newVersion.DownloadUrl);
        }

    }
}
