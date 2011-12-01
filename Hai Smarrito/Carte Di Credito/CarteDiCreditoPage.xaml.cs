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

namespace HaiSmarrito
{
    public partial class CarteDiCreditoPage : PhoneApplicationPage
    {
        public CarteDiCreditoPage()
        {
            InitializeComponent();
        }

        private void AppBarInfoButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Carte Di Credito/InfoPage.xaml", UriKind.Relative));
        }
    }
}