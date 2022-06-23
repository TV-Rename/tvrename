using CefSharp;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Web;

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
            string openlocation = HttpUtility.UrlDecode(url.Substring(UI.EXPLORE_PROXY.Length));
            if (Helpers.OpenFolder(openlocation))
            {
                return true;
            }
            Helpers.OpenFolderSelectFile(openlocation);
            return true;
        }

        if (url.StartsWith(UI.WATCH_PROXY, StringComparison.InvariantCultureIgnoreCase))
        {
            string fileName = HttpUtility.UrlDecode(url.Substring(UI.WATCH_PROXY.Length)).Replace('/', '\\');
            Helpers.OpenFile(fileName);
            return true;
        }

        if (url.IsHttpLink() || url.IsFileLink())
        {
            Helpers.OpenUrl(url);
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

    public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
    {
        throw new Exception("Plugin crashed!");
    }

    public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
        IRequestCallback callback)
    {
        return CefReturnValue.Continue;
    }

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
