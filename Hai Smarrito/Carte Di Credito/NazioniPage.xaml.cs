using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class NazioniPage : PhoneApplicationPage
    {
        public NazioniPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            switch (NavigationContext.QueryString["type"])
            {
                case "amex":
                    this.Title1Textbox.Text = "American Express";
                    break;
                case "visa":
                    this.Title1Textbox.Text = "Visa";
                    break;
                case "mastercard":
                    this.Title1Textbox.Text = "Mastercard";
                    break;

                default:
                    break;
            }

            base.OnNavigatedTo(e);
        }


        private void AmericanSamoa_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Argentina_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Australia_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}