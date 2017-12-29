// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TVRename
{
    /// <summary>
    /// Summary for BuyMeADrink
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class BuyMeADrink : Form
    {
        public BuyMeADrink()
        {
            InitializeComponent();
            label1.Text = "If this program has saved you time, and you use it regularly, then please consider buying me a drink to say thanks!\r\r" + "Type in (or choose) an amount, then hit the button to go to Paypal.";
            comboBox1.Items.Add("$" + ((double) 1).ToString(".00"));
            comboBox1.Items.Add("$" + ((double) 2).ToString(".00"));
            comboBox1.Items.Add("$" + ((double) 5).ToString(".00"));
            comboBox1.Items.Add("$" + ((double) 10).ToString(".00"));
            comboBox1.Items.Add("$" + ((double) 20).ToString(".00"));

            comboBox2.Items.Add("AUD");
            comboBox2.Items.Add("USD");

            comboBox1.SelectedIndex = 2;
            comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            double amount = 5.00; // default amount

            try
            {
                string s = Regex.Replace(comboBox1.Text, "\\$", "");
                amount = double.Parse(s);
            }
            catch
            {
            }

            string currency = comboBox2.Text;

            CultureInfo usCi = new CultureInfo("en-US", false);

            string paypalUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=paypal%40tvrename%2ecom&item_name=TVRename%20thank-you%20drink&no_shipping=0&no_note=1&amount=" + amount.ToString("N", usCi) + "&tax=0&currency_code=" + currency + "&lc=AU&bn=PP%2dDonationsBF&charset=UTF%2d8";

            Helpers.SysOpen(paypalUrl);
        }

    }
}
