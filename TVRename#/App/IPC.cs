using System;

namespace TVRename
{
    internal class IPCMethods : MarshalByRefObject
    {
        private static Ui _theUi;
        private static TVDoc _theDoc;

        // These "do stuff" methods must not be made static, otherwise it messes up the IPC magic
        // of RegisterWellKnownClientType, etc.
        public CommandLineArgs.MissingFolderBehaviour MissingBehaviour
        {
            get
            {
                if (_theDoc != null)
                    return _theDoc.Args.MissingFolder;

                return CommandLineArgs.MissingFolderBehaviour.Ask;
            }
            set
            {
                if (_theDoc != null)
                    _theDoc.Args.MissingFolder = value;
            }
        }

        public bool RenameBehaviour
        {
            get
            {
                if (_theDoc != null)
                    return TVSettings.Instance.RenameCheck;

                return false;
            }
            set
            {
                if (_theDoc != null)
                    TVSettings.Instance.RenameCheck = value;
            }
        }

        public static void Setup(Ui ui, TVDoc doc)
        {
            _theDoc = doc;
            _theUi = ui;
        }

        public void BringToForeground()
        {
            if (_theUi != null)
                _theUi.BeginInvoke(_theUi.IPCBringToForeground);
        }

        public void Scan()
        {
            if (_theUi != null)
                _theUi.Invoke(_theUi.IPCScan);
        }

        public void DoAll()
        {
            if (_theUi != null)
                _theUi.Invoke(_theUi.IPCDoAll);
        }

        public void Quit()
        {
            if (_theUi != null)
                _theUi.BeginInvoke(_theUi.IPCQuit);
        }
    }
}
