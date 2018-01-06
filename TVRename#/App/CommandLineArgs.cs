using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        public bool Hide { get; }
        public MissingFolderBehavior MissingFolder { get; set; } // TODO: Make read only
        public bool RenameCheck { get; }
        public bool Quit { get; }
        public bool ForceRecover { get; }
        public bool Scan { get; }
        public bool DoAll { get; }
        public bool Unattended { get; }
        public string UserFilePath { get; }

        /// <summary>
        /// Initializes a new instance populated with values parsed from the command line arguments.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public CommandLineArgs(ReadOnlyCollection<string> args)
        {
            this.Hide = args.Contains("/hide", StringComparer.OrdinalIgnoreCase);
            this.RenameCheck = !args.Contains("/norenamecheck", StringComparer.OrdinalIgnoreCase);
            this.Quit = args.Contains("/quit", StringComparer.OrdinalIgnoreCase);
            this.ForceRecover = args.Contains("/recover", StringComparer.OrdinalIgnoreCase);
            this.DoAll = args.Contains("/doall", StringComparer.OrdinalIgnoreCase);
            this.Scan = args.Contains("/scan", StringComparer.OrdinalIgnoreCase);
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
    }
}
