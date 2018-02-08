using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Models;
using TVRename.Core.TVDB;
using TVRename.Windows.Models;

namespace TVRename.Windows.Forms
{
    public partial class Identify : Form
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private CancellationTokenSource cts;

        public FoundShowFolder NewShow { get; private set; }

        public Identify(FoundShowFolder show)
        {
            InitializeComponent();

            this.NewShow = show;
        }

        private void Identify_Load(object sender, EventArgs e)
        {
            this.textBoxSearch.Text = Path.GetFileName(this.NewShow.Location);

            ValidateInput();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            ValidateInput();
        }

        private async void buttonSearch_Click(object sender, EventArgs e)
        {
            this.listViewResults.Items.Clear();

            if (this.buttonSearch.Text == "&Search")
            {
                this.buttonSearch.Text = "&Cancel";
                this.textBoxSearch.Enabled = false;

                this.cts = new CancellationTokenSource();

                try
                {
                    List<SearchResult> results = await TVDB.Instance.Search(this.textBoxSearch.Text.Trim(), this.cts.Token);

                    this.listViewResults.BeginUpdate();

                    foreach (SearchResult result in results)
                    {
                        this.listViewResults.Items.Add(new ListViewItem(new[]
                        {
                            result.Id.ToString(),
                            result.Name,
                            result.Year.ToString()
                        })
                        {
                            Tag = result.Id
                        });
                    }

                    if (this.listViewResults.Items.Count > 0)
                    {
                        ListViewItem match = this.listViewResults.Items.Cast<ListViewItem>().FirstOrDefault(lvi => string.Equals(lvi.SubItems[1].Text, this.textBoxSearch.Text.Trim(), StringComparison.InvariantCultureIgnoreCase));

                        if (match != null)
                        {
                            match.Selected = true;
                        }
                        else
                        {
                            this.listViewResults.Items[0].Selected = true;
                        }
                    }

                    this.listViewResults.EndUpdate();

                    this.buttonSearch.Text = "&Search";
                    this.textBoxSearch.Enabled = true;
                }
                catch (TaskCanceledException exception)
                {
                    Logger.Error(exception);
                }
            }
            else
            {
                this.cts.Cancel();

                this.buttonSearch.Text = "&Search";
                this.textBoxSearch.Enabled = true;
            }

            ValidateInput();
        }

        private void listViewResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewResults.SelectedItems.Count == 0) return;

            ValidateInput();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            this.NewShow.TvdbId = int.Parse(this.listViewResults.SelectedItems[0].Text);
            this.NewShow.Name = this.listViewResults.SelectedItems[0].SubItems[1].Text;

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ValidateInput()
        {
            bool searchFocused = this.textBoxSearch.Focused;
            bool showSelected = this.listViewResults.SelectedItems.Count > 0;

            if (searchFocused || !showSelected)
            {
                this.AcceptButton = this.buttonSearch;
            }
            else
            {
                this.AcceptButton = this.buttonAdd;
            }

            this.buttonAdd.Enabled = showSelected;
        }
    }
}
