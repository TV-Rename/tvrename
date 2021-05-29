using System;
using System.Windows.Forms;

namespace TVRename.Forms.Utilities
{
    public partial class CannotConnectForm : Form
    {
        private readonly TVDoc.ProviderType provider;

        public CannotConnectForm(string header, string message, TVDoc.ProviderType provider)
        {
            this.provider = provider;
            InitializeComponent();
            label1.Text = message;
            Text = header;
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void bnTVDB_Click(object sender, EventArgs e)
        {
            Helpers.OpenUrl(GetCoreUrl(provider));
        }

        private static string GetCoreUrl(TVDoc.ProviderType provider)
        {
            return provider switch
            {
                TVDoc.ProviderType.TVmaze => "https://www.tvmaze.com/",
                TVDoc.ProviderType.TheTVDB => "https://www.thetvdb.com/",
                TVDoc.ProviderType.TMDB => "https://themoviedb.org/",
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }

        private void bnAPICheck_Click(object sender, EventArgs e)
        {
            Helpers.OpenUrl(GetIsDownUrl(provider));
        }

        private static string GetIsDownUrl(TVDoc.ProviderType provider)
        {
            return provider switch
            {
                TVDoc.ProviderType.TVmaze => "https://www.isitdownrightnow.com/api.tvmaze.com.html",
                TVDoc.ProviderType.TheTVDB => "https://www.isitdownrightnow.com/api.thetvdb.com.html",
                TVDoc.ProviderType.TMDB => "https://www.isitdownrightnow.com/api.themoviedb.org.html",
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }

        private void bnOffline_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void bnContinue_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            Close();
        }
    }
}
