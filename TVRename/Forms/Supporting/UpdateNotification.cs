//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

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
        this.UpdateColorControls();
        tbReleaseNotes.Text = newVersion.ReleaseNotesText.ToUiVersion();
        lblStatus.Text = $@"There is new version {update} available since {update.ReleaseDate.ToLocalTime()}.";
        webReleaseNotes.RequestHandler = new BrowserRequestHandler();
    }

    public async Task UpdateMarkup()
    {
        //If this call is slow then we can put it in a new thread and update the control as it comes back from GH
        await UpdateWithMarkdown().ConfigureAwait(false);
    }

    private async Task UpdateWithMarkdown()
    {
        using HttpClient client = new();
        try
        {
            JsonObject request = new()
            {
                {"text", newVersion.ReleaseNotesText},
                {"mode", "gfm"},
                {"context", "TV-Rename/tvrename"}
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd(TVSettings.USER_AGENT);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(GITHUB_CONVERSION_URL, request).ConfigureAwait(false);
            using System.IO.StreamReader reader = new(response.Content.ReadAsStream());
            string result = reader.ReadToEnd();

            string HTML_HEAD_LIGHT =
                "<html><head><meta charset=\"utf-8\"><style type=\"text/css\">* {body {background-color: #ffffff;} color: #333333; font-family: Helvetica, Arial, sans-serif; font-size:90%}</style></head><body>";

            string HTML_HEAD_DARK =
                "<html><head><meta charset=\"utf-8\"><style type=\"text/css\">* {body {background-color: #323232;} color: #f4f4f6; font-family: Helvetica, Arial, sans-serif; font-size:90%}</style></head><body>";

            string HTML_FOOTER = "</body></html>";

            webReleaseNotes.SafeInvoke(() =>
            {
                TvWebExtensions.SetHtmlBodyWithEmoji(webReleaseNotes, (ThemeManager.IsDark ? HTML_HEAD_DARK : HTML_HEAD_LIGHT) + result + HTML_FOOTER);
                webReleaseNotes.Visible = true;
            }, true);

            tbReleaseNotes.SafeInvoke(() =>{ tbReleaseNotes.Visible = false;},true);
        }
        catch (WebException wex)
        {
            Logger.LogWebException("GitHub Conversion of markdown failed", wex);
        }
    }

    private void bnReleaseNotes_Click(object sender, EventArgs e)
    {
        newVersion.ReleaseNotesUrl.OpenUrlInBrowser();
    }

    private void btnDownloadNow_Click(object sender, EventArgs e)
    {
        newVersion.DownloadUrl.OpenUrlInBrowser();
    }

    private void btnDownloadNowAndQuit_Click(object sender, EventArgs e)
    {
        newVersion.DownloadUrl.OpenUrlInBrowser();
        DialogResult = DialogResult.Abort;
    }

    private void NavigateTo(object sender, WebBrowserNavigatingEventArgs e)
    {
        string? url = e.Url?.AbsoluteUri;

        if (url is null || e.Url is null) return;

        // ReSharper disable once StringLiteralTypo
        if (url.Contains(@"ieframe.dll"))
        {
            url = e.Url.Fragment.RemoveFirstCharacter();
        }

        if (url.IsWebLink())
        {
            e.Cancel = true;
            e.Url.AbsoluteUri.OpenUrlInBrowser();
        }
    }
}
