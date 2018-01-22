using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Microsoft.WindowsAPICodePack.Dialogs;
using TVRename.Core.Models;
using TVRename.Core.TVDB;
using TVRename.Windows.Configuration;

namespace TVRename.Windows.Forms
{
    public partial class Add : Form
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private CancellationTokenSource cts;

        public Show NewShow { get; private set; }

        public Add()
        {
            InitializeComponent();

            this.textBoxLocation.Text = Settings.Instance.DefaultLocation;
            this.textBoxLocation.Tag = null;
        }

        private void Add_Load(object sender, EventArgs e)
        {
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

            if (this.textBoxLocation.Tag == null)
            {
                this.textBoxLocation.TextChanged -= this.textBoxLocation_TextChanged;
                this.textBoxLocation.Text = Path.Combine(Settings.Instance.DefaultLocation, this.listViewResults.SelectedItems[0].SubItems[1].Text);
                this.textBoxLocation.TextChanged += this.textBoxLocation_TextChanged;
            }

            ValidateInput();
        }

        private void textBoxLocation_TextChanged(object sender, EventArgs e)
        {
            this.textBoxLocation.Tag = string.IsNullOrEmpty(this.textBoxLocation.Text) ? (object)null : true;

            ValidateInput();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog
            {
                DefaultDirectory = this.textBoxLocation.Text,
                IsFolderPicker = true,
                Multiselect = false,
                RestoreDirectory = true,
                EnsureValidNames = true,
                EnsurePathExists = true,
                EnsureFileExists = true
            };

            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.textBoxLocation.Text = Path.GetFullPath(ofd.FileName);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            this.NewShow = new Show
            {
                TVDBId = int.Parse(this.listViewResults.SelectedItems[0].Text),
                Location = Path.GetFullPath(this.textBoxLocation.Text)
            };

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void ValidateInput()
        {
            bool searchFocused = this.textBoxSearch.Focused;
            bool showSelected = this.listViewResults.SelectedItems.Count > 0;
            bool directoryExists = Directory.Exists(this.textBoxLocation.Text);

            if (searchFocused || !showSelected)
            {
                this.AcceptButton = this.buttonSearch; 
            }
            else
            {
                this.AcceptButton = directoryExists ? this.buttonAdd : this.buttonBrowse;
            }

            this.buttonAdd.Enabled = showSelected && directoryExists;
        }
    }
}
