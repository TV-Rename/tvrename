using System;
using System.Text;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.Web.WebView2.Core;

namespace TVRename.Forms.Utilities
{
    public partial class BrowserTest : Form
    {
        [NotNull] private readonly string content;
        private readonly CoreWebView2Environment webView2Environment;

        public BrowserTest([NotNull] string content)
        {
            this.content = content;
            InitializeComponent();

            webView2Environment = CoreWebView2Environment.CreateAsync(
                     null,
                            PathManager.CefCachePath
                     ).Result;
        }

        private async void BrowserTest_Load(object sender, EventArgs e)
        {
            webChrome.Load("data:text/html;base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(content)));

            await webEdge.EnsureCoreWebView2Async(webView2Environment);
            webEdge.NavigateToString(content);
        }

        private void BrowserTest_Resize(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = (this.Width-4 )/2;
        }
    }
}
