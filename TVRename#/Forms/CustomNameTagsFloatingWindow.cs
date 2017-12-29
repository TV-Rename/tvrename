// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Summary for CustomNameTagsFloatingWindow
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class CustomNameTagsFloatingWindow : Form
    {
        public CustomNameTagsFloatingWindow(ProcessedEpisode pe)
        {
            InitializeComponent();

            foreach (string s in CustomName.Tags)
            {
                string txt = s;
                if (pe != null)
                    txt += " - " + CustomName.NameForNoExt(pe, s);

                label1.Text += txt + "\r\n";
            }
        }
    }
}
