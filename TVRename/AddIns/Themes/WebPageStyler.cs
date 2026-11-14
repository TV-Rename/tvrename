//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using CefSharp;
using CefSharp.WinForms;

namespace TVRename.AddIns.Themes;

/// <summary>
/// The Information, Images and Summary panes are web pages TV Rename builds itself (data: URLs) and styles with
/// Bootstrap, whose tables, season rows, buttons and muted text are made for a light page. This adds a small style
/// sheet to those pages (never to real web sites, such as trailers) as soon as each one starts to load, so they are
/// readable on the dark background. Pictures are not touched.
/// </summary>
internal sealed class WebPageStyler : IRenderProcessMessageHandler
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly Lazy<string> Script = new(BuildScript);

    private readonly IRenderProcessMessageHandler? previous;

    private WebPageStyler(IRenderProcessMessageHandler? previous) => this.previous = previous;

    public static void Attach(ChromiumWebBrowser web)
    {
        if (web.RenderProcessMessageHandler is WebPageStyler)
        {
            return;
        }

        // Keep any handler TV Rename sets itself working.
        web.RenderProcessMessageHandler = new WebPageStyler(web.RenderProcessMessageHandler);
        // Belt and braces: also after each page has loaded (adding the style twice is prevented by the script).
        web.FrameLoadEnd += (_, e) =>
        {
            if (e.Frame.IsMain)
            {
                Inject(e.Frame);
            }
        };
    }

    public void OnContextCreated(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
    {
        previous?.OnContextCreated(chromiumWebBrowser, browser, frame);
        if (frame.IsMain)
        {
            Inject(frame);
        }
    }

    public void OnContextReleased(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        => previous?.OnContextReleased(chromiumWebBrowser, browser, frame);

    public void OnFocusedNodeChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IDomNode node)
        => previous?.OnFocusedNodeChanged(chromiumWebBrowser, browser, frame, node);

    public void OnUncaughtException(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, JavascriptException exception)
        => previous?.OnUncaughtException(chromiumWebBrowser, browser, frame, exception);

    private static void Inject(IFrame frame)
    {
        try
        {
            if (frame.IsValid)
            {
                frame.ExecuteJavaScriptAsync(Script.Value, "about:blank", 1);
            }
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Theme add-in: could not style a web page");
        }
    }

    private static string Css(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    private static string BuildScript()
    {
        Theme t = ThemeManager.Active ?? Theme.BuiltInDark;
        string text = Css(t.WebText);
        string muted = Css(t.WebMutedText);
        string link = Css(t.WebLink);
        string border = Css(t.WebBorder);
        string season = Css(t.WebSeasonRow);
        bool dark = !t.IsLightBase;
        // Hover and stripe shading: a little lighter on dark pages, a little darker on light ones.
        string shade = dark ? "255,255,255" : "0,0,0";

        StringBuilder css = new();
        // Scroll bars and form controls follow the page's colour scheme, not Windows' app colour setting.
        css.Append(dark ? ":root{color-scheme:dark}" : ":root{color-scheme:light}");
        css.Append($"body{{color:{text}}}");
        css.Append($"h1,h2,h3,h4,h5,h6,.lead,p,dl,dt,dd,li{{color:inherit}}");
        css.Append($".text-muted{{color:{muted}!important}}");
        css.Append($"a{{color:{link}}}a:hover{{color:{link};filter:brightness(1.2)}}");
        // Bootstrap tables are dark grey text on a light page.
        css.Append($".table{{color:{text};border-color:{border};--bs-table-striped-color:{text};--bs-table-active-color:{text};" +
                   $"--bs-table-hover-color:{text};--bs-table-hover-bg:rgba({shade},.06);--bs-table-striped-bg:rgba({shade},.04);" +
                   $"--bs-table-active-bg:rgba({shade},.08)}}");
        css.Append($".table>:not(caption)>*>*{{color:inherit;border-bottom-color:{border}}}");
        css.Append($".table>:not(:last-child)>:last-child>*{{border-bottom-color:{border}}}");
        // Season heading rows are a light grey block.
        css.Append($".table-secondary{{--bs-table-bg:{season}!important;--bs-table-hover-bg:{season}!important;" +
                   $"--bs-table-striped-bg:{season}!important;--bs-table-active-bg:{season}!important;" +
                   $"--bs-table-hover-color:{text}!important;color:{text}!important;border-color:{border}!important}}");
        css.Append($".card{{color:{text};border-color:{border}}}");
        css.Append($".btn-outline-secondary{{color:{text};border-color:{muted}}}");
        css.Append($".btn-outline-secondary:hover,.btn-outline-secondary:focus{{color:{text};background-color:rgba({shade},.12);border-color:{muted}}}");
        css.Append($".accordion-item{{border-color:{border}}}.accordion-button,.accordion-body{{color:{text}}}");
        css.Append($".accordion-button:not(.collapsed){{color:{link};box-shadow:inset 0 -1px 0 {border}}}");
        if (dark)
        {
            css.Append(".accordion-button::after{filter:invert(1) grayscale(1) brightness(1.5)}");
        }
        css.Append($"hr{{color:{border};opacity:1}}");
        css.Append($".list-group-item{{background-color:transparent;color:{text};border-color:{border}}}");
        css.Append($".bg-light{{background-color:{season}!important}}.text-dark{{color:{text}!important}}");

        // The simple pages ("Not available for this TV show") colour everything black.
        string oldPage = $"*{{color:{text}!important}}a:link,a:visited,a:active{{color:{link}!important}}" +
                         $"a.search:link,a.search:visited,a.search:active{{color:{Css(ThemeStyler.Readable(Color.FromArgb(128, 0, 0), t.Background))}!important}}";

        // If the theme changes the window background, the pages follow it.
        string background = string.Empty;
        if (t.Background.ToArgb() != SystemColors.ButtonFace.ToArgb())
        {
            string bg = Css(t.Background);
            css.Append($"body,.card,.accordion-item,.accordion-header,.accordion-button{{background-color:{bg}!important}}");
            background = $"*{{background-color:{bg}!important}}img{{background-color:transparent!important}}";
        }

        string cssJson = System.Text.Json.JsonSerializer.Serialize(css.ToString());
        string oldJson = System.Text.Json.JsonSerializer.Serialize(oldPage + background);

        return "(function(){" +
               "if(location.protocol!=='data:')return;" +
               $"var css={cssJson},old={oldJson};" +
               "function add(){" +
               "if(document.getElementById('tvr-theme'))return;" +
               "var host=document.head||document.documentElement;if(!host)return;" +
               "var styles=document.getElementsByTagName('style'),text=css;" +
               "for(var i=0;i<styles.length;i++){if(/\\*\\s*\\{\\s*color:\\s*black\\s*\\}/.test(styles[i].textContent)){text=css+old;break;}}" +
               "var s=document.createElement('style');s.id='tvr-theme';s.textContent=text;host.appendChild(s);}" +
               "if(document.readyState==='loading'){document.addEventListener('DOMContentLoaded',add);}else{add();}" +
               "})();";
    }
}
