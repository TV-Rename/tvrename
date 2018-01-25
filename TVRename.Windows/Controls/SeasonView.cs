using System;
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
                
                this.SuspendLayout();

                foreach (KeyValuePair<int, Episode> episode in this.Item.Episodes.OrderBy(e => e.Key).Reverse())
                {
                    this.Controls.Add(new EpisodeView(episode.Value)
                    {
                        Dock = DockStyle.Top
                    });
                }

                this.ResumeLayout();
            }
        }

        public SeasonView() : this(new Season()) { }

        public SeasonView(Season season)
        {
            InitializeComponent();

            this.Item = season;
        }

        private void SeasonView_Load(object sender, EventArgs e)
        {
            this.HorizontalScroll.Enabled = false;
            this.HorizontalScroll.Visible = false;
        }
    }
}
