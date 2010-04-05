using System;

namespace TVRename
{
    class IPCMethods : MarshalByRefObject
    {
        private static UI TheUI;

        public void SetUI(UI ui)
        {
            TheUI = ui;
        }

        public void BringToForeground()
        {
            if (TheUI != null)
                TheUI.BeginInvoke(TheUI.IPCBringToForeground);
        }
    }
}
