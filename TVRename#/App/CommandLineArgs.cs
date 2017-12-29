using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TVRename
{
    public class CommandLineArgs
    {
        public enum MissingFolderBehaviour
        {
            Ask,
            Ignore,
            Create
        }

        // holds boolean settings set on the command line
        public bool Hide;
        public MissingFolderBehaviour MissingFolder;
        public bool RenameCheck;
        public bool Quit;
        public bool ForceRecover;
        public bool Scan;
        public bool DoAll;
        public bool Unattended;
        public string UserFilePath;

        public CommandLineArgs(String[] args)
        {
            // force all arguments to lower case
            for (int i = 0; i < args.Length; i++)
                if (args[i][0] == '/')
                    args[i] = args[i].ToLower();

            Hide = Array.IndexOf(args, "/hide") != -1;

            if (Array.IndexOf(args, "/createmissing") != -1)
                MissingFolder = MissingFolderBehaviour.Create;
            else if (Array.IndexOf(args, "/ignoremissing") != -1)
                MissingFolder = MissingFolderBehaviour.Ignore;
            else
                MissingFolder = MissingFolderBehaviour.Ask;

            RenameCheck = !(Array.IndexOf(args, "/norenamecheck") != -1);
            Quit = Array.IndexOf(args, "/quit") != -1;
            ForceRecover = Array.IndexOf(args, "/recover") != -1;

            DoAll = Array.IndexOf(args, "/doall") != -1;
            Scan = Array.IndexOf(args, "/scan") != -1;

            Unattended = Array.IndexOf(args, "/unattended") != -1;

            foreach (string arg in args)
            {
                if (arg.StartsWith("/userfilepath:"))
                {
                    UserFilePath = arg.Substring(arg.IndexOf(":") + 1);
                }
            }
        }

        public CommandLineArgs(ReadOnlyCollection<string> commandLineArgs)
        {
            Hide = commandLineArgs.Contains("/hide", StringComparer.OrdinalIgnoreCase);

            if (commandLineArgs.Contains("/createmissing", StringComparer.OrdinalIgnoreCase) )
                MissingFolder = MissingFolderBehaviour.Create;
            else if (commandLineArgs.Contains("/ignoremissing", StringComparer.OrdinalIgnoreCase))
                MissingFolder = MissingFolderBehaviour.Ignore;
            else
                MissingFolder = MissingFolderBehaviour.Ask;

            RenameCheck = !commandLineArgs.Contains("/norenamecheck", StringComparer.OrdinalIgnoreCase); 
            Quit = commandLineArgs.Contains("/quit", StringComparer.OrdinalIgnoreCase);
            ForceRecover = commandLineArgs.Contains("/recover", StringComparer.OrdinalIgnoreCase);

            DoAll = commandLineArgs.Contains("/doall", StringComparer.OrdinalIgnoreCase); 
            Scan = commandLineArgs.Contains("/scan", StringComparer.OrdinalIgnoreCase); 

            Unattended = commandLineArgs.Contains("/unattended", StringComparer.OrdinalIgnoreCase); 

            foreach (string arg in commandLineArgs)
            {
                if (arg.StartsWith("/userfilepath:"))
                {
                    UserFilePath = arg.Substring(arg.IndexOf(":") + 1);
                }
            }
        }
    }
}
