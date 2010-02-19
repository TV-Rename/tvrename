//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;


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
	public class CustomNameTagsFloatingWindow : System.Windows.Forms.Form
	{
			public CustomNameTagsFloatingWindow(ProcessedEpisode pe)
			{
				InitializeComponent();

				foreach (string s in CustomName.Tags())
				{
					string txt = s;
					if (pe != null)
						txt += " - " + CustomName.NameForNoExt(pe, s);

					label1.Text += txt + "\r\n";
				}
				}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		private System.Windows.Forms.Label label1;

#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
					System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(CustomNameTagsFloatingWindow)));
					this.label1 = (new System.Windows.Forms.Label());
					this.SuspendLayout();
					// 
					// label1
					// 
					this.label1.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
					this.label1.AutoSize = true;
					this.label1.Location = new System.Drawing.Point(12, 9);
					this.label1.Name = "label1";
					this.label1.Size = new System.Drawing.Size(0, 13);
					this.label1.TabIndex = 0;
					// 
					// CustomNameTagsFloatingWindow
					// 
					this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
					this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
					this.AutoSize = true;
					this.ClientSize = new System.Drawing.Size(248, 41);
					this.Controls.Add(this.label1);
					this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
					this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
					this.Name = "CustomNameTagsFloatingWindow";
					this.ShowInTaskbar = false;
					this.Text = "Tags";
					this.ResumeLayout(false);
					this.PerformLayout();

				}
#endregion
	}
}