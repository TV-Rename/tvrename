// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Net;
using System.Windows.Forms;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace TVRename.Forms
{
    public partial class UpdateNotification : Form
    {
        private readonly Release newVersion;
        private const string GITHUB_CONVERSION_URL = "https://api.github.com/markdown";

        public UpdateNotification([NotNull] Release update)
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
            if (WebRequest.Create(new Uri(GITHUB_CONVERSION_URL)) is HttpWebRequest req)
            {
                req.Method = "POST";
                req.ContentType = "application/json";
                req.UserAgent = TVSettings.Instance.USER_AGENT;

                JObject request = new JObject
                {
                    {"text", newVersion.ReleaseNotesText},
                    {"mode", "gfm"},
                    {"context", "TV-Rename/tvrename"}
                };

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(req.GetRequestStream()))
                {
                    writer.Write(request.ToString());
                }

                string result = null;
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    if (resp != null)
                    {
                        System.IO.StreamReader reader = new System.IO.StreamReader(resp.GetResponseStream() ?? throw new InvalidOperationException());

                        result = reader.ReadToEnd();
                    }
                }

                const string HTML_HEAD = "<html><head><style type=\"text/css\">* {font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Helvetica, Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\"; font-size:90%}</style></head><body>";

                const string HTML_FOOTER = "</body></html>";

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

        private void btnDownloadNowAndQuit_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen(newVersion.DownloadUrl);
            DialogResult = DialogResult.Abort;
        }

        private void NavigateTo(object sender, [NotNull] WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.AbsoluteUri;

            if (url.Contains(@"ieframe.dll"))
            {
                url = e.Url.Fragment.Substring(1);
            }

            if (url.IsWebLink())
            {
                e.Cancel = true;
                Helpers.SysOpen(e.Url.AbsoluteUri);
            }
        }
    }
}
