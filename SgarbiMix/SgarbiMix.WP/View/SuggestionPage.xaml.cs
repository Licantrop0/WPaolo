using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SgarbiMix.WP.View
{
    public partial class SuggestionPage : PhoneApplicationPage
    {
        public SuggestionPage()
        {
            InitializeComponent();
            SuggerimentoTextBox.Text += "\n";
        }

        private void Suggersci_Click(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(YoutubeLinkTextBox.Text, @"^(https?\:\/\/)?(www\.|m\.)?(youtube\.com|youtu\.be).*/watch\?v\=\w+$"))
            {
                MessageBox.Show("Mi spiace, ma ho bisogno del link di YouTube contenente l'insulto per aggiungerlo più rapidamente alla lista.", "Link di YouTube", MessageBoxButton.OK);
                return;
            }

            new EmailComposeTask()
            {
                Subject = "[SgarbiMix] Suggerimento insulto",
                To = "wpmobile@hotmail.it",
                Body = SuggerimentoTextBox.Text + "\n" + YoutubeLinkTextBox.Text
            }.Show();
        }

        private void YoutubeLinkTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Suggersci_Click(sender, null);
            }
        }
    }
}