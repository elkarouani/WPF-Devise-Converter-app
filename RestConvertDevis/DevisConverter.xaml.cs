using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace RestConvertDevis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, string> base_currency_dict = new Dictionary<string, string>() {
            {"Bitcoin", "btc-bitcoin"},
            {"Euro", "eur-euro-token"},
            {"Neurochain", "ncc-neurochain"}
        };

        Dictionary<string, string> quote_currency_dict = new Dictionary<string, string>() {
            {"USD", "usd-us-dollars"},
            {"Ethereum", "eth-ethereum"},
            {"XRP", "xrp-xrp"}
        };


        public MainWindow()
        {
            InitializeComponent();
        }

        private string SendConvertRequest(int montant, string base_currency_id, string quote_currency_id)
        {
            string result = "";

            try
            {
                WebClient web_client = new WebClient();
                web_client.QueryString.Add("base_currency_id", base_currency_id);
                web_client.QueryString.Add("quote_currency_id", quote_currency_id);
                web_client.QueryString.Add("amount", montant.ToString());
                string response = web_client.DownloadString("https://api.coinpaprika.com/v1/price-converter");

                Dictionary<string, string> response_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

                result = response_dict["price"];
            }
            catch (WebException exception) { 
                MessageBox.Show($"Veuiller réssayer la conversion plus tard, et assurer que vous avez connecté à internet !\nDescription d'exception : {exception.Message}"); 
                if (exception.Status.ToString() == "429") { MessageBox.Show($"Veuiller réssayer la conversion plus tard, problème trouvé à cause de briser une limite de taux de demandes!\nDescription d'exception : {exception.Message}"); }
                if (exception.Status.ToString()[0] == '5') { MessageBox.Show($"Veuiller réssayer la conversion plus tard, le problème est sur le côté de service Coinpaprika!\nDescription d'exception : {exception.Message}"); }
            }
            
            return result;
        }

        private void ConvertAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int montant = int.Parse(MontantInput.Text);
            
                ComboBoxItem base_currency_item = (ComboBoxItem)BaseCurrencyInput.SelectedItem;
                string base_currency = base_currency_item.Content.ToString();

                ComboBoxItem quote_currency_item = (ComboBoxItem)QuoteCurrencyInput.SelectedItem;
                string quote_currency = quote_currency_item.Content.ToString();

                string price = this.SendConvertRequest(montant, this.base_currency_dict[base_currency], this.quote_currency_dict[quote_currency]);

                ConvertOutput.Content = $"\nLe prix retourné : {price}";
            }
            catch (FormatException exception) { MessageBox.Show($"Veuillez saisir la correcte forme montant à convertir, doit être un nombre et pas vide !\nDescription d'exception : {exception.Message}"); }
        }
    }
}
