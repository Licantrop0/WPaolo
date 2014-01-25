using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Text.RegularExpressions;
using System.Windows;

namespace SgarbiMix.View
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
                MessageBox.Show("Mi spiace, ma è necessario inserire anche il link di YouTube contenente l'insulto per inviare il suggerimento.", "Link di YouTube", MessageBoxButton.OK);
                return;
            }

            new EmailComposeTask()
            {
                Subject = "[SgarbiMix] Suggerimento insulto",
                To = "wpmobile@hotmail.it",
                Body = SuggerimentoTextBox.Text + "\n" + YoutubeLinkTextBox.Text
            }.Show();
        }
    }
}