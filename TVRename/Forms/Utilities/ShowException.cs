// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private readonly Exception mException;

        public ShowException(Exception e)
        {
            InitializeComponent();

            mException = e;
        }

        private void ShowException_Load(object sender, EventArgs e)
        {
            string t = mException.Message + "\r\n\r\n" + mException.StackTrace;
            txtText.Text = t;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
