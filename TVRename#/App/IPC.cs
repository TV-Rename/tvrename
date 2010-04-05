using System;

namespace TVRename
{
    class IPCMethods : MarshalByRefObject
    {
        private static UI TheUI;
        private static TVDoc TheDoc;

        public IPCMethods()
        {
        }

        public IPCMethods(UI ui, TVDoc doc)
        {
            TheDoc = doc;
            TheUI = ui;
        }

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
