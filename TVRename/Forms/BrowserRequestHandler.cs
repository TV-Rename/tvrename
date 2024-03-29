using CefSharp;
using System;
using System.Security.Cryptography.X509Certificates;
using TVRename.Forms;

namespace TVRename;

public class BrowserRequestHandler : IRequestHandler
{
    public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
    {
        if (request.Url is null)
        {
            return false;
        }

        string url = request.Url;

        if (string.Compare(url, "about:blank", StringComparison.Ordinal) == 0)
        {
            return false; // don't intercept about:blank
        }

        if (url == UI.QuickStartGuide())
        {
            return false; // let the quick-start guide be shown
        }

        if (url.Contains("://www.youtube.com/embed/"))
        {
            return false; // let embedded youtube URL be show
        }

        if (url.StartsWith(UI.EXPLORE_PROXY, StringComparison.InvariantCultureIgnoreCase))
        {
            string openlocation = System.Web.HttpUtility.UrlDecode(url.RemoveFirst(UI.EXPLORE_PROXY.Length));
            if (openlocation.OpenFolder())
            {
                return true;
            }
            openlocation.OpenFolderSelectFile();
            return true;
        }

        if (url.StartsWith(UI.WATCH_PROXY, StringComparison.InvariantCultureIgnoreCase))
        {
            string fileName = System.Web.HttpUtility.UrlDecode(url.RemoveFirst(UI.WATCH_PROXY.Length)).Replace('/', '\\');
            fileName.OpenFile();
            return true;
        }

        if (url.IsHttpLink() || url.IsFileLink())
        {
            url.OpenUrlInBrowser();
            return true;
        }
        return false;
    }

    public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
    {
    }

    public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
        WindowOpenDisposition targetDisposition, bool userGesture)
    {
        return false;
    }

    public IResourceRequestHandler? GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
        IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) => null;

    public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host,
        int port, string realm, string scheme, IAuthCallback callback)
    {
        callback.Dispose();
        return false;
    }

    public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl,
        ISslInfo sslInfo, IRequestCallback callback)
    {
        callback.Dispose();
        return false;
    }

    public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port,
        X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) =>
        false;

    public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
    {
    }

    public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize,
        IRequestCallback callback)
    {
        callback.Dispose();
        return false;
    }

    public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
    {
    }
}
