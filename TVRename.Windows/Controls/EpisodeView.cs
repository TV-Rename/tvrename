using System.Windows.Forms;
using TVRename.Core.Models.Cache;
using TVRename.Core.TVDB;

namespace TVRename.Windows.Controls
{
    public partial class EpisodeView : UserControl
    {
        private Episode episode;

        public Episode Item
        {
            get => this.episode;
            set
            {
                this.episode = value;

                this.pictureBoxThumbnail.ImageLocation = Client.ImageUrl + "_cache/" + this.Item.Thumbnail;
                this.labelName.Text = $"E{this.Item.Number:D2} - {this.Item.Name}";
                this.labelDate.Text = $"{this.Item.FirstAired:dd/MM/yyyy} (Aired)"; // TODO
                this.labelOverview.Text = this.Item.Overview;
                this.labelRating.Text = $"{this.Item.Rating.Score}/10 ({this.Item.Rating.Score} votes)"; // TODO
            }
        }

        public EpisodeView() : this(new Episode()) { }

        public EpisodeView(Episode episode)
        {
            InitializeComponent();

            this.Item = episode;
        }
    }
}
