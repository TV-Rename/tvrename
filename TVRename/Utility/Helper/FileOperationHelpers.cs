using Alphaleonis.Win32.Filesystem;
using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TVRename;

public static class FileOperationExtensions
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static bool OpenFolder(this string? folder)
    {
        if (folder is null || !Directory.Exists(folder))
        {
            return false;
        }

        return SysOpen("explorer.exe", folder.EnsureEndsWithSeparator().InDoubleQuotes());
    }

    public static void OpenFolderSelectFile(this string filename)
    {
        string args = $"/e, /select, {filename.InDoubleQuotes()}";

        ProcessStartInfo info = new() { FileName = "explorer", Arguments = args };
        try
        {
            Process.Start(info);
        }
        catch (Win32Exception ex)
        {
            Logger.Error(ex);
        }
    }

    public static bool OpenUrlInBrowser(this string url) => OpenUrlInternal(url);

    private static bool OpenUrlInternal(this string url)
    {
        try
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception e)
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    Logger.Warn($"Could not open URL: {url} - {e.Message}");
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Warn( $"Could not open URL: {url} - {e.Message}");
            return false;
        }

        return true;
    }

    public static void OpenFile(this string filename) => OpenUrlInternal(filename);

    public static void OpenFile(this FileInfo file) => OpenUrlInternal(file.FullName);

    private static bool SysOpen(this string? what, string? arguments)
    {
        if (string.IsNullOrWhiteSpace(what))
        {
            return false;
        }

        try
        {
            if (arguments.HasValue())
            {
                Process.Start(what, arguments);
            }
            else
            {
                Process.Start(what);
            }

            return true;
        }
        catch (Win32Exception e)
        {
            Logger.Warn(e, $"Could not open {what}");
            return false;
        }
        catch (System.IO.FileNotFoundException e)
        {
            Logger.Warn(e, $"Could not open {what}");
            return false;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Could not open {what}");
            return false;
        }
    }
}
