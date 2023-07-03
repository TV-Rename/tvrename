//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using System.Threading;
using TVRename.Utility.Helper;

namespace TVRename;

internal abstract class Exporter
{
    protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

    private void Run()
    {
        if (!Active())
        {
            LOGGER.Trace($"Skipped (Disabled) Output File to: {Location()}");
            return;
        }
        if (Location().IsNullOrWhitespace())
        {
            LOGGER.Warn("Please open settings and ensure filenames are provided for each exporter you have enabled");
            return;
        }
        if (Location().IsWebLink())
        {
            LOGGER.Warn($"TV Rename cannot export file to a web location: {Location()}, please update in the setttings");
            return;
        }

        try
        {
            string dir = Path.GetDirectoryName(Location()) ?? string.Empty;

            if (dir.IsNullOrWhitespace())
            {
                LOGGER.Warn($"Please open settings and ensure filenames are provided for each exporter you have enabled: {Location()} is invalid");
                return;
            }

            //Create the directory if needed
            Directory.CreateDirectory(dir);
            Do();
            LOGGER.Info($"Output File to: {Location()}");
        }
        catch (NotSupportedException e)
        {
            LOGGER.Warn($"Output File must be a local file: {Location()} {e.ErrorText()}");
        }
        catch (System.IO.DirectoryNotFoundException e)
        {
            LOGGER.Warn($"Could not find File/Directory at: {Location()} {e.ErrorText()}");
        }
        catch (UnauthorizedAccessException e)
        {
            LOGGER.Warn($"Could not access File/Directory at: {Location()} {e.ErrorText()}");
        }
        catch (System.IO.IOException e)
        {
            LOGGER.Warn($"Could not access File/Directory at: {Location()} {e.ErrorText()}");
        }
        catch (Exception e)
        {
            LOGGER.Error(e, $"Failed to Output File to: {Location()}");
        }
    }

    public abstract bool Active();
    protected abstract string Location();
    protected abstract string Name();

    /// <exception cref="ArgumentException">Locaiton is not valid.</exception>
    /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="System.IO.IOException"></exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    protected abstract void Do();

    public void RunAsThread()
    {
        if (Active())
        {
            try
            {
                TaskHelper.Run(Run, $"{Name()} Thread");
            }
            catch (ThreadStateException ex)
            {
                LOGGER.Error($"Failed to run {Name()} to {Location()}",ex);
            }
            catch (OutOfMemoryException ex)
            {
                LOGGER.Error($"Failed to run {Name()} to {Location()}", ex);
            }
        }
    }
}
