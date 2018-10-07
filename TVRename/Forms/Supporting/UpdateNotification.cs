using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TVRename.Forms
{
    public partial class UpdateNotification : Form
    {
        private readonly UpdateVersion newVersion;

        public UpdateNotification(UpdateVersion update)
        {
            newVersion = update;
            InitializeComponent();
            tbReleaseNotes.Text = newVersion.ReleaseNotesText;
            lblStatus.Text = $@"There is new version {update} available since {update.ReleaseDate.ToLocalTime()}.";

            //If this call is slow then we can put it in a new thread and update the control as it comes back from GH
            UpdateWithMarkdown();
        }

        private void UpdateWithMarkdown()
        {
            const string GITHUB_CONVERSION_URL = "https://api.github.com/markdown";

            if (WebRequest.Create(new Uri(GITHUB_CONVERSION_URL)) is HttpWebRequest req)
            {
                req.Method = "POST";
                req.ContentType = "application/json";
                req.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

                JObject request = new JObject
                {
                    {"text", newVersion.ReleaseNotesText},
                    {"mode", "gfm"},
                    {"context", "TV-Rename/tvrename"}
                };

                using (StreamWriter writer = new StreamWriter(req.GetRequestStream()))
                {
                    writer.Write(request.ToString());
                }

                string result = null;
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    if (resp != null)
                    {
                        StreamReader reader =
                            new StreamReader(resp.GetResponseStream() ?? throw new InvalidOperationException());

                        result = reader.ReadToEnd();
                    }
                }

                string HTML_HEAD =
                    "<html><head><style type=\"text/css\">* {font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Helvetica, Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\"; font-size:90%}</style></head><body>";

                string HTML_FOOTER = "</body></html>";

                webReleaseNotes.DocumentText = HTML_HEAD + result + HTML_FOOTER;
            }

            webReleaseNotes.Visible = true;
            tbReleaseNotes.Visible = false;

        }

        private void bnReleaseNotes_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen(newVersion.ReleaseNotesUrl);
        }

        private void btnDownloadNow_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen(newVersion.DownloadUrl);
        }

        private void NavigateTo(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.AbsoluteUri;

            if (url.Contains(@"ieframe.dll"))
                url = e.Url.Fragment.Substring(1);

            if ((url.Substring(0, 7).CompareTo("http://") == 0) || (url.Substring(0, 8).CompareTo("https://") == 0))
            {
                e.Cancel = true;
                Helpers.SysOpen(e.Url.AbsoluteUri);
            }
        }
    }
}
