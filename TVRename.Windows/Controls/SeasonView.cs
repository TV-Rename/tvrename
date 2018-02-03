using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TVRename.Core.Models.Cache;

namespace TVRename.Windows.Controls
{
    public partial class SeasonView : UserControl
    {
        private Season season;

        public Season Item
        {
            get => this.season;
            set
            {
                this.season = value;
                
                SuspendLayout();

                foreach (KeyValuePair<int, Episode> episode in this.Item.Episodes.OrderBy(e => e.Value.Number).ThenBy(e => e.Value.FirstAired).Reverse())
                {
                    this.Controls.Add(new EpisodeView(episode.Value)
                    {
                        Dock = DockStyle.Top
                    });
                }

                ResumeLayout();
            }
        }

        public SeasonView() : this(new Season()) { }

        public SeasonView(Season season)
        {
            InitializeComponent();

            this.HorizontalScroll.Enabled = false;
            this.HorizontalScroll.Visible = false;

            this.Item = season;
        }
    }
}
