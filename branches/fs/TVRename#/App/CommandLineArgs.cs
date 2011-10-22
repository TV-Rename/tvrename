using System;
using System.Collections.Generic;
using System.Text;

namespace TVRename
{
    public class CommandLineArgs
    {
        public enum MissingFolderBehaviour
        {
            Ask,
            Ignore,
            Create
        };

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
    }
}
