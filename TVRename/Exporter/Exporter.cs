// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.IO;

namespace TVRename
{
    internal abstract class Exporter
    {
        public abstract bool Active();

        public void Run()
        {
            if (!Active())
            {
                LOGGER.Trace("Skipped (Disabled) Output File to: {0}", Location());
                return;
            }

            if (string.IsNullOrWhiteSpace(Location()))
            {
                LOGGER.Warn("Please open settings and ensure filenames are provided for each exporter you have enabled");
                return;
            }
            if (Location().StartsWith("http://") || Location().StartsWith("https://"))
            {
                LOGGER.Warn($"TV Rename cannot export file to {Location()}, please update in the setttings");
                return;
            }

            //Create the directory if needed
            Directory.CreateDirectory(Path.GetDirectoryName(Location()) ?? "");

            try
            {
                Do();
                LOGGER.Info("Output File to: {0}", Location());
            }
            catch (NotSupportedException e)
            {
                LOGGER.Warn(e, "Output File must be a local file: {0}", Location());
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Failed to Output File to: {0}", Location());
            }

        }

        protected abstract string Location();
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        internal abstract void Do();
    }
}
