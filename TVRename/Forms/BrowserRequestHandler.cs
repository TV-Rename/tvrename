using CefSharp;
using CefSharp.Handler;
using System;
using TVRename.Forms;

namespace TVRename;


public class BrowserRequestHandler : RequestHandler
{
    protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
    {
        // Return your custom resource handler instance here
        return new BrowserResourceRequestHandler();
    }

    protected override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
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

}

public class BrowserResourceRequestHandler : ResourceRequestHandler
{
    protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
    {
        request.SetReferrer("http://tvreanme.com", ReferrerPolicy.NeverClearReferrer);
        return CefReturnValue.Continue; // Let the request continue normally
    }
}
