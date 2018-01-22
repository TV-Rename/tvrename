using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using TVRename.Core.Models;
using TVRename.Core.TVDB;

namespace TVRename.Windows.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("Item")]
    public partial class ShowView : UserControl
    {
        private Show show;

        [Category("Action")]
        public event EventHandler RefreshClicked;

        public bool RefreshEnabled
        {
            get => this.buttonRefresh.Enabled;
            set => this.buttonRefresh.Enabled = value;
        }

        public Show Item
        {
            get => this.show;
            set
            {
                this.show = value;

                this.pictureBoxBanner.ImageLocation = Client.ImageUrl + this.Item.Metadata.Banner;
                this.labelName.Text = this.Item.Metadata.Name;
                this.labelOverview.Text = this.Item.Metadata.Overview;
                this.labelActors.Text = string.Join(", ", this.Item.Metadata.Actors);
                this.labelAirs.Text = $"{this.Item.Metadata.AirDay} at {this.Item.Metadata.AirTime:hh\\:mm} on {this.Item.Metadata.Network}";
                this.labelFirstAired.Text = $"{this.Item.Metadata.FirstAired:dd MMMM yyyy}";
                this.labelRuntime.Text = $"{this.Item.Metadata.Runtime} minutes";
                this.labelStatus.Text = this.Item.Metadata.Status.ToString();
                this.labelGenres.Text = string.Join(", ", this.Item.Metadata.Genres);
                this.labelRating.Text = $"{this.Item.Metadata.Rating.Score}/10 ({this.Item.Metadata.Rating.Votes} votes)";
            }
        }

        public ShowView() : this(new Show()) { }

        public ShowView(Show show)
        {
            InitializeComponent();

            this.tableLayoutPanel.HorizontalScroll.Enabled = false;
            this.tableLayoutPanel.HorizontalScroll.Visible = false;

            Padding p = this.tableLayoutPanel.Padding;
            this.tableLayoutPanel.Padding = new Padding(p.Left, p.Top, SystemInformation.VerticalScrollBarWidth, p.Bottom);

            this.Item = show;
        }

        private void ShowView_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

        private void buttonImdb_Click(object sender, EventArgs e)
        {
            Process.Start($"http://www.imdb.com/title/{this.Item.Metadata.ImdbId}");
        }
        
        private void buttonTvCom_Click(object sender, EventArgs e)
        {
            Process.Start($"http://www.tv.com/show/{this.Item.Metadata.Id}/summary.html");
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            this.RefreshClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
