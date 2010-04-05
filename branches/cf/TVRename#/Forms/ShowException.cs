// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Summary for ShowException
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class ShowException : Form
    {
        private Exception mException;

        public ShowException(Exception e)
        {
            this.InitializeComponent();

            this.mException = e;
        }

        private void ShowException_Load(object sender, System.EventArgs e)
        {
            string t;
            t = this.mException.Message + "\r\n\r\n" + this.mException.StackTrace;
            this.txtText.Text = t;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}