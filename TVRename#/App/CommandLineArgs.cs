using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TVRename
{
    /// <summary>
    /// Parse and store command line arguments.
    /// </summary>
    public class CommandLineArgs
    {
        /// <summary>
        /// Actions to perform when a missing folder is found.
        /// </summary>
        public enum MissingFolderBehavior
        {
            Ask,
            Ignore,
            Create
        }

        public bool Help { get; }
        public bool Hide { get; }
        public MissingFolderBehavior MissingFolder { get; set; } // TODO: Make read only
        public bool RenameCheck { get; }
        public bool Quit { get; }
        public bool ForceRecover { get; }
        public bool Scan { get; }
        public bool QuickScan { get; }
        public bool RecentScan { get; }
        public bool DoAll { get; }
        public bool Unattended { get; }
        public string UserFilePath { get; }

        /// <summary>
        /// Initializes a new instance populated with values parsed from the command line arguments.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public CommandLineArgs(ReadOnlyCollection<string> args)
        {
            this.Help = args.Contains("/?", StringComparer.OrdinalIgnoreCase);
            this.Hide = args.Contains("/hide", StringComparer.OrdinalIgnoreCase);
            this.RenameCheck = !args.Contains("/norenamecheck", StringComparer.OrdinalIgnoreCase);
            this.Quit = args.Contains("/quit", StringComparer.OrdinalIgnoreCase);
            this.ForceRecover = args.Contains("/recover", StringComparer.OrdinalIgnoreCase);
            this.DoAll = args.Contains("/doall", StringComparer.OrdinalIgnoreCase);
            this.Scan = args.Contains("/scan", StringComparer.OrdinalIgnoreCase);
            this.QuickScan = args.Contains("/quickscan", StringComparer.OrdinalIgnoreCase);
            this.RecentScan = args.Contains("/recentscan", StringComparer.OrdinalIgnoreCase);
            this.Unattended = args.Contains("/unattended", StringComparer.OrdinalIgnoreCase);

            this.UserFilePath = args.Where(a => a.StartsWith("/userfilepath:", StringComparison.OrdinalIgnoreCase)).Select(a => a.Substring(a.IndexOf(":", StringComparison.Ordinal) + 1)).FirstOrDefault();

            if (args.Contains("/createmissing", StringComparer.OrdinalIgnoreCase))
            {
                this.MissingFolder = MissingFolderBehavior.Create;
            }
            else if (args.Contains("/ignoremissing", StringComparer.OrdinalIgnoreCase))
            {
                this.MissingFolder = MissingFolderBehavior.Ignore;
            }
            else
            {
                this.MissingFolder = MissingFolderBehavior.Ask;
            }
        }

        public static string Helptext()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("/scan will Tell TV Rename to run a scan");
            output.AppendLine("/quickscan will scan shows most likely to need an update: http://www.tvrename.com/userguide#scan");
            output.AppendLine("/recentscan will scan recent shows: http://www.tvrename.com/userguide#scan");
            output.AppendLine("/doall Tell TV Rename execute all the actions it can.");
            output.AppendLine("/quit Tell a running TV Rename session to exit.");
            output.AppendLine("");
            output.AppendLine("/hide will hide the UI");
            output.AppendLine("/unattended same as /hide");
            output.AppendLine("");
            output.AppendLine("/recover will load a dialog box that enables the user to recover a prior TVDB.xml or TVRenameSettings.xml file");
            output.AppendLine("/userfilepath:BLAH  Sets a custom folder path for the settings files.");
            output.AppendLine("/createmissing will Create folders if they are missing.");
            output.AppendLine("/ignoremissing will Ignore missing folders.");
            output.AppendLine("/norenamecheck requests an existing TV Rename session to scan without renaming.");
            output.AppendLine("");
            output.AppendLine("Further information is available at http://www.tvrename.com/cmd-line");

            return output.ToString();


        }
    }
}

