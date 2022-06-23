//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Net;
using System.Windows.Forms;

namespace TVRename.Forms;

public partial class UpdateNotification : Form
{
    private readonly ServerRelease newVersion;
    private const string GITHUB_CONVERSION_URL = "https://api.github.com/markdown";
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public UpdateNotification(ServerRelease update)
    {
        newVersion = update;
        InitializeComponent();
        tbReleaseNotes.Text = newVersion.ReleaseNotesText.ToUiVersion();
        lblStatus.Text = $@"There is new version {update} available since {update.ReleaseDate.ToLocalTime()}.";

        //If this call is slow then we can put it in a new thread and update the control as it comes back from GH
        UpdateWithMarkdown();
    }

    private void UpdateWithMarkdown()
    {
        try
        {
            if (WebRequest.Create(new Uri(GITHUB_CONVERSION_URL)) is HttpWebRequest req)
            {
                req.Method = "POST";
                req.ContentType = "application/json";
                req.UserAgent = TVSettings.USER_AGENT;

                JObject request = new()
                {
                    {"text", newVersion.ReleaseNotesText},
                    {"mode", "gfm"},
                    {"context", "TV-Rename/tvrename"}
                };

                using (System.IO.StreamWriter writer = new(req.GetRequestStream()))
                {
                    writer.Write(request.ToString());
                }

                string? result = null;
                using (HttpWebResponse? resp = req.GetResponse() as HttpWebResponse)
                {
                    if (resp != null)
                    {
                        System.IO.StreamReader reader =
                            new(resp.GetResponseStream() ??
                                throw new InvalidOperationException());

                        result = reader.ReadToEnd();
                    }
                }

                const string HTML_HEAD =
                    "<html><head><style type=\"text/css\">* {font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Helvetica, Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\"; font-size:90%}</style></head><body>";

                const string HTML_FOOTER = "</body></html>";

                webReleaseNotes.DocumentText = HTML_HEAD + result + HTML_FOOTER;
            }
            webReleaseNotes.Visible = true;
            tbReleaseNotes.Visible = false;
        }
        catch (WebException wex)
        {
            Logger.LogWebException("GitHub Conversion of markdown failed", wex);
        }
    }

    private void bnReleaseNotes_Click(object sender, EventArgs e)
    {
        Helpers.OpenUrl(newVersion.ReleaseNotesUrl);
    }

    private void btnDownloadNow_Click(object sender, EventArgs e)
    {
        Helpers.OpenUrl(newVersion.DownloadUrl);
    }

    private void btnDownloadNowAndQuit_Click(object sender, EventArgs e)
    {
        Helpers.OpenUrl(newVersion.DownloadUrl);
        DialogResult = DialogResult.Abort;
    }

    private void NavigateTo(object sender, WebBrowserNavigatingEventArgs e)
    {
        string url = e.Url.AbsoluteUri;

        // ReSharper disable once StringLiteralTypo
        if (url.Contains(@"ieframe.dll"))
        {
            url = e.Url.Fragment.Substring(1);
        }

        if (url.IsWebLink())
        {
            e.Cancel = true;
            Helpers.OpenUrl(e.Url.AbsoluteUri);
        }
    }
}
