namespace TVRename
{
    using System;

    internal class IPCMethods : MarshalByRefObject
    {
        private static UI TheUI;
        private static TVDoc TheDoc;

        // These "do stuff" methods must not be made static, otherwise it messes up the IPC magic
        // of RegisterWellKnownClientType, etc.
        public CommandLineArgs.MissingFolderBehaviour MissingBehaviour
        {
            get
            {
                if (TheDoc != null)
                    return TheDoc.Args.MissingFolder;

                return CommandLineArgs.MissingFolderBehaviour.Ask;
            }
            set
            {
                if (TheDoc != null)
                    TheDoc.Args.MissingFolder = value;
            }
        }

        public bool RenameBehaviour
        {
            get
            {
                if (TheDoc != null)
                    return TVSettings.Instance.RenameCheck;

                return false;
            }
            set
            {
                if (TheDoc != null)
                    TVSettings.Instance.RenameCheck = value;
            }
        }

        public static void Setup(UI ui, TVDoc doc)
        {
            TheDoc = doc;
            TheUI = ui;
        }

        public void BringToForeground()
        {
            if (TheUI != null)
                TheUI.BeginInvoke(TheUI.IPCBringToForeground);
        }

        public void Scan()
        {
            if (TheUI != null)
                TheUI.Invoke(TheUI.IPCScan);
        }

        public void DoAll()
        {
            if (TheUI != null)
                TheUI.Invoke(TheUI.IPCDoAll);
        }

        public void Quit()
        {
            if (TheUI != null)
                TheUI.BeginInvoke(TheUI.IPCQuit);
        }
    }
}