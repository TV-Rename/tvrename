using System;
using System.Collections.Generic;
using System.Text;

namespace TVRename
{
    using System.Windows.Forms;

    // Thanks to http://stackoverflow.com/questions/442817/c-flickering-listview-on-update
    public class ListViewFlickerFree : System.Windows.Forms.ListView
    {
        public ListViewFlickerFree()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}
