//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

using System.Windows.Forms;
using System.IO;

namespace TVRename
{
	/// <summary>
	/// Summary for CustomNameDesigner
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public partial class CustomNameDesigner : Form
	{
		private ProcessedEpisodeList Eps;
		private CustomName CN;
		private TVDoc mDoc;

		public CustomNameDesigner(ProcessedEpisodeList pel, CustomName cn, TVDoc doc)
		{
			Eps = pel;
			CN = cn;
			mDoc = doc;

			InitializeComponent();

			if (Eps == null)
				lvTest.Enabled = false;
			txtTemplate.Text = CN.StyleString;

			FillExamples();
			FillCombos();
		}

		private void FillCombos()
		{
			cbTags.Items.Clear();
			cbPresets.Items.Clear();
			ProcessedEpisode pe = null;
			if (lvTest.SelectedItems.Count == 0)
				pe = ((Eps != null) && (Eps.Count>0)) ? Eps[0] : null;
			else
				pe = (ProcessedEpisode)(lvTest.SelectedItems[0].Tag);

			foreach (string s in CustomName.Tags())
			{
				string txt = s;
				if (pe != null)
					txt += " - " + CustomName.NameForNoExt(pe, s);
				cbTags.Items.Add(txt);
			}

			foreach (string s in CustomName.Presets())
			{
				if (pe != null)
					cbPresets.Items.Add(CustomName.NameForNoExt(pe, s));
				else
					cbPresets.Items.Add(s);
			}

		}

		private void FillExamples()
		{
			if (Eps == null)
				return;

			lvTest.Items.Clear();
			foreach (ProcessedEpisode pe in Eps)
			{
				ListViewItem lvi = new ListViewItem();
				string fn = mDoc.FilenameFriendly(CN.NameForExt(pe,null));
				lvi.Text = fn;

				bool ok = false;
				bool ok1 = false;
				bool ok2 = false;
				if (fn.Length < 255)
				{
					int seas;
					int ep;
					ok = mDoc.FindSeasEp(new FileInfo(fn+".avi"), out seas, out ep, pe.SI.ShowName());
					ok1 = ok && (seas == pe.SeasonNumber);
					ok2 = ok && (ep == pe.EpNum);
					string pre1 = ok1 ? "" : "* ";
					string pre2 = ok2 ? "" : "* ";

					lvi.SubItems.Add(pre1 + ((seas != -1) ? seas.ToString() : ""));
					lvi.SubItems.Add(pre2 + ((ep != -1) ? ep.ToString() : ""));
					lvi.Tag = pe;
				}
				if (!ok || !ok1 || !ok2)
					lvi.BackColor = Helpers.WarningColor();
				lvTest.Items.Add(lvi);
			}
		}

	    private void cbPresets_SelectedIndexChanged(object sender, System.EventArgs e)
        {
			 int n = cbPresets.SelectedIndex;
			 if (n == -1)
				 return;

			 txtTemplate.Text = CustomName.Presets()[n];
			 cbPresets.SelectedIndex = -1;
        }

	    private void txtTemplate_TextChanged(object sender, System.EventArgs e)
        {
			 CN.StyleString = txtTemplate.Text;
			 FillExamples();
        }

	    private void cbTags_SelectedIndexChanged(object sender, System.EventArgs e)
        {
			 int n = cbTags.SelectedIndex;
			 if (n == -1)
				 return;

			 int p = txtTemplate.SelectionStart;
			 string s = txtTemplate.Text;
			 txtTemplate.Text = s.Substring(0, p) + CustomName.Tags()[cbTags.SelectedIndex] + s.Substring(p);

			 cbTags.SelectedIndex = -1;
        }
	
        private void lvTest_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            FillCombos();
        }
	}
}