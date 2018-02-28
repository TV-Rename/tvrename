using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace TVRename.Windows.Controls
{
    /// <summary>
    /// Represents a Windows control that displays a frame around a group of controls with a checkbox caption to enable/disable them.
    /// </summary>
    /// <seealso cref="GroupBox" />
    /// <inheritdoc />
    [DefaultProperty("Text")]
    [DefaultEvent("CheckedChanged")]
    public partial class CheckedGroupBox : GroupBox
    {
        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        /// <inheritdoc />
        [Editor("System.ComponentModel.Design.MultilineStringEditor", typeof(UITypeEditor))]
        public override string Text
        {
            get => this.checkBox.Text;
            set => this.checkBox.Text = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CheckedGroupBox"/> is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool Checked
        {
            get => this.checkBox.Checked;
            set => this.checkBox.Checked = value;
        }
        
        /// <summary>
        /// Occurs when the value of the <see cref='Checked'/> property changes.
        /// </summary>
        public event EventHandler CheckedChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedGroupBox" /> class.
        /// </summary>
        /// <inheritdoc />
        public CheckedGroupBox()
        {
            InitializeComponent();

            ControlAdded += (s, a) => EnableControls(this.checkBox.Checked);

            this.checkBox.CheckedChanged += (s, a) =>
            {
                EnableControls(this.checkBox.Checked);
                CheckedChanged?.Invoke(s, a);
            };
        }

        /// <summary>
        /// Enables or disable the child controls.
        /// </summary>
        /// <param name="enable">if set to <c>true</c> enable child controls, else disable.</param>
        private void EnableControls(bool enable)
        {
            foreach (Control control in this.Controls)
            {
                if (control.Name == "checkBox") continue;

                control.Enabled = enable;
            }
        }
    }
}
