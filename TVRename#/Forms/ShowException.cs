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
        private readonly Exception _mException;

        public ShowException(Exception e)
        {
            InitializeComponent();

            _mException = e;
        }

        private void ShowException_Load(object sender, EventArgs e)
        {
            string t = _mException.Message + "\r\n\r\n" + _mException.StackTrace;
            txtText.Text = t;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
