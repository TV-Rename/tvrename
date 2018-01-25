using System.Windows.Forms;
using TVRename.Windows.Utilities;

namespace TVRename.Windows.Forms
{
    /// <summary>
    /// Displays a splash screen show progress and status.
    /// </summary>
    /// <seealso cref="Form" />
    /// <inheritdoc />
    public partial class SplashScreen : Form
    {
        /// <summary>
        /// Sets the status text.
        /// </summary>
        /// <value>
        /// The status text.
        /// </value>
        public string Status
        {
            set => this.labelStatus.Text = value;
        }

        /// <summary>
        /// Sets the progress percentage.
        /// </summary>
        /// <value>
        /// The progress percentage.
        /// </value>
        public int Progress
        {
            set => this.progressBarLoading.Value = value;
        }

        public SplashScreen()
        {
            InitializeComponent();

            this.labelVersion.Text = Helpers.DisplayVersion;
        }
    }
}
