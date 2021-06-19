using System.Windows.Forms;

namespace TVRename.Forms.Tools
{
    public partial class RecommendationViewPreferences : Form
    {
        public RecommendationViewPreferences(int trendingWeight, int topWeight, int relatedWeight, int similarWeight)
        {
            InitializeComponent();

            trackBar1.Value = trendingWeight;
            trackBar2.Value = topWeight;
            trackBar3.Value = relatedWeight;
            trackBar4.Value = similarWeight;
        }

        public int TrendingWeight => trackBar1.Value;
        public int TopWeight => trackBar2.Value;
        public int RelatedWeight => trackBar3.Value;
        public int SimilarWeight => trackBar4.Value;

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
