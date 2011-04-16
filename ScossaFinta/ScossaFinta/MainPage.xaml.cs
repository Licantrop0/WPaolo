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
using Microsoft.Devices;
using Microsoft.Xna.Framework;

namespace ScossaFinta
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            if (Settings.statistics.NumeroDiScosse >= 10)
            {
                Obiettivo1Border.Opacity = 100;
                ThunderButton.Opacity = 1;
            }
            if (Settings.statistics.NumeroDiScosse >= 20)
            {
                Obiettivo2Border.Opacity = 100;
                ScossaButton.Visibility = Visibility.Collapsed;
                ScossaRepeatButton.Visibility = Visibility.Visible;
                ThunderButton.Opacity = 0;
                ThunderRepeatButton.Visibility = Visibility.Visible;
                HoldTextBox1.Opacity = 1;
                HoldTextBox2.Opacity = 1;
            }
            if (Settings.statistics.NumeroDiScosse >= 30)
            {
                Obiettivo3Border.Opacity = 100;
                ZeusButton.Visibility = System.Windows.Visibility.Visible;
                FrameworkDispatcher.Update();
                Settings.ZeusParla.Play();
            }
        }

        private void scossa()
        {
            //incremento il contatore delle scosse
            Settings.statistics.NumeroDiScosse++;

            //controllo raggiungimento obiettivi
            if (Settings.statistics.NumeroDiScosse == 10)
            {
                FrameworkDispatcher.Update();
                Settings.SuonoPremio.Play();
                Obiettivo1Border.Opacity = 1;
                ThunderButton.Opacity = 1;
                return;
            }
            else if (Settings.statistics.NumeroDiScosse == 20)
            {
                FrameworkDispatcher.Update();
                Settings.SuonoPremio.Play();
                Obiettivo2Border.Opacity = 1;
                ScossaButton.Visibility = Visibility.Collapsed;
                ScossaRepeatButton.Visibility = Visibility.Visible;
                ThunderButton.Opacity = 0;
                ThunderRepeatButton.Visibility = Visibility.Visible;
                HoldTextBox1.Opacity = 1;
                HoldTextBox2.Opacity = 1;
                return;
            }
            else if (Settings.statistics.NumeroDiScosse == 30)
            {
                FrameworkDispatcher.Update();
                Settings.SuonoPremio.Play();
                Obiettivo3Border.Opacity = 1;
                ZeusButton.Visibility = Visibility.Visible;
                FrameworkDispatcher.Update();
                Settings.ZeusParla.Play();
                return;
            }

            //faccio vibrare il device per 1 secondo (prima faccio uno stop nel caso stia già vibrando)
            VibrateController vibrateController = VibrateController.Default;
            vibrateController.Stop();
            vibrateController.Start(new TimeSpan(0, 0, 1));
        }

        private void ScossaButton_Click(object sender, RoutedEventArgs e)
        {
            scossa();
            FrameworkDispatcher.Update();
            Settings.SuonoScossa.Play();
            if (Settings.statistics.NumeroDiScosse >= 20)
                System.Threading.Thread.Sleep(300);
        }

        private void ThunderButton_Click(object sender, RoutedEventArgs e)
        {
            scossa();
            FrameworkDispatcher.Update();
            Settings.SuonoThunder.Play();
            if(Settings.statistics.NumeroDiScosse >= 20)
                System.Threading.Thread.Sleep(300);
        }

        private void ZeusButton_Click(object sender, RoutedEventArgs e)
        {
            scossa();
            FrameworkDispatcher.Update();
            Settings.SuonoThunderRisata.Play();
        }
    }
}