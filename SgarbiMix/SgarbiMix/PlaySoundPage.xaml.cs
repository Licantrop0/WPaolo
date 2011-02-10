using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using WPCommon;

/* 
    Copyright (c) 2011 WPME
*/

namespace SgarbiMix
{
    public partial class PlaySoundPage : PhoneApplicationPage
    {

        #region Private Sound Fields

        SoundEffect IgnoranteCapraSound;
        SoundEffect AteoFasulloSound;
        SoundEffect CapraSound;
        SoundEffect CapraIgnoranteSound;
        SoundEffect CretinateSound;
        SoundEffect CriminaleSound;
        SoundEffect CulattoniSound;
        SoundEffect CrimineSound;
        SoundEffect InsopportabileSound;
        SoundEffect PirlaSound;
        SoundEffect FascistaSound;
        SoundEffect LicenziareSound;
        SoundEffect LeggiStudiaSound;
        SoundEffect LeggaDanteManzoniSound;
        SoundEffect MangereiVivoSound;
        SoundEffect GuardaCheRobaSound;
        SoundEffect CoglioniSound;
        SoundEffect NonHaFattoSound;
        SoundEffect RidicoloSound;
        SoundEffect Ridicolo6Sound;
        SoundEffect VaiStudiareSound;
        SoundEffect VergognaSound;

        SoundEffectInstance Base1Loop;
        SoundEffectInstance Base2Loop;
        SoundEffectInstance Base3Loop;

        #endregion

        public PlaySoundPage()
        {
            InitializeComponent();
            InitializeSounds();
            InitializeBases();
            InitializeButtonColors();
        }

        private void InitializeSounds()
        {
            //fascista
            FascistaSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/fascista.wav", UriKind.Relative)).Stream);
            //capra
            CapraSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/capra1.wav", UriKind.Relative)).Stream);
            //capra ignorante
            CapraIgnoranteSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/capraignorante.wav", UriKind.Relative)).Stream);
            //culattoni raccomandati
            CulattoniSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/culattoniraccomandati.wav", UriKind.Relative)).Stream);
            //mi state sui coglioni di principio
            CoglioniSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/mistatesuicoglionidiprincipio.wav", UriKind.Relative)).Stream);
            //crimine
            CrimineSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/e1crimine1.wav", UriKind.Relative)).Stream);
            //ridicolo
            RidicoloSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/ridicolo1.wav", UriKind.Relative)).Stream);
            //ridicolo 6 ma 6 sempre stato
            Ridicolo6Sound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/ridicolo6ma6semprestato1.wav", UriKind.Relative)).Stream);
            //non ha mai fatto 1 cazzo in vita sua
            NonHaFattoSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/nonhafattomaiuncazzoinvitasua1.wav", UriKind.Relative)).Stream);
            //vergogna vergogna
            VergognaSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/vergognavergogna.wav", UriKind.Relative)).Stream);
            //6 ignorante come 1 capra
            IgnoranteCapraSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/61ignorantecome1capra.wav", UriKind.Relative)).Stream);
            //ateo fasullo
            AteoFasulloSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/ateofasullo.wav", UriKind.Relative)).Stream);
            //criminale
            CriminaleSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/criminale.wav", UriKind.Relative)).Stream);
            //è insopportabile
            InsopportabileSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/einsopportabile.wav", UriKind.Relative)).Stream);
            //e tu sei un pirla
            PirlaSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/etu61pirla.wav", UriKind.Relative)).Stream);
            //io sono convinto che vi debbano licenziare
            LicenziareSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/iosonoconvintochevidebbanolicenziare.wav", UriKind.Relative)).Stream);
            //leggi studia
            LeggiStudiaSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/leggistudia.wav", UriKind.Relative)).Stream);
            //lo mangerei vivo
            MangereiVivoSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/lomangereivivo.wav", UriKind.Relative)).Stream);
            //ma guarda che roba
            GuardaCheRobaSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/maguardacheroba1.wav", UriKind.Relative)).Stream);
            //vai a studiare
            VaiStudiareSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/vaiastudiare.wav", UriKind.Relative)).Stream);
            //legga dante, legga manzoni, impari quella capra
            LeggaDanteManzoniSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/leggadanteleggamanzoniimpariquellacapra.wav", UriKind.Relative)).Stream);
            //non dire cretinate
            CretinateSound = SoundEffect.FromStream(App.GetResourceStream(new Uri("sounds/nondirecretinate.wav", UriKind.Relative)).Stream);
        }

        private void InitializeBases()
        {
            // mixer 1
            Base1Loop = SoundEffect.FromStream(App.GetResourceStream(
                new Uri("sounds/base1.wav", UriKind.Relative)).Stream).CreateInstance();
            Base1Loop.IsLooped = true;

            // mixer 2
            Base2Loop = SoundEffect.FromStream(App.GetResourceStream(
                new Uri("sounds/base2.wav", UriKind.Relative)).Stream).CreateInstance();
            Base2Loop.IsLooped = true;

            // mixer 3
            Base3Loop = SoundEffect.FromStream(App.GetResourceStream(
                new Uri("sounds/base3.wav", UriKind.Relative)).Stream).CreateInstance();
            Base3Loop.IsLooped = true;

        }

        private void InitializeButtonColors()
        {
            int Toggle = 1;
            //prende tutti i bottoni all'interno della griglia
            foreach (var b in ButtonsStackPanel.Children.Where(c => c is Button).Cast<Button>())
            {
                switch (Toggle)
                {
                    case 1: b.Background = new SolidColorBrush(Colors.Red); break;
                    case 2: b.Background = new SolidColorBrush(Colors.White); break;
                    case 3: b.Background = new SolidColorBrush(Colors.Green); break;
                }
                Toggle++;
                if (Toggle > 3) Toggle = 1;
            }
        }

        #region Play Buttons

        private void Fascista_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                FascistaSound.Play();
        }

        private void Ignorante_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                IgnoranteCapraSound.Play();
        }

        private void Capra_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                CapraSound.Play();
        }

        private void Crimine_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                CrimineSound.Play();
        }

        private void Coglioni_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                CoglioniSound.Play();
        }

        private void Culattoni_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                CulattoniSound.Play();
        }

        private void Ridicolo_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                RidicoloSound.Play();
        }

        private void Ridicolo6_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                Ridicolo6Sound.Play();
        }

        private void NonHaMaiFatto_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                NonHaFattoSound.Play();
        }

        private void CapraIgnorante_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                CapraIgnoranteSound.Play();
        }

        private void Pirla_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                PirlaSound.Play();
        }

        private void LeggiStudia_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                LeggiStudiaSound.Play();
        }

        private void Criminale_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                CriminaleSound.Play();
        }

        private void Guarda_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                GuardaCheRobaSound.Play();
        }

        private void Vergogna_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                VergognaSound.Play();
        }

        private void Mangerei_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                MangereiVivoSound.Play();
        }

        private void Insopportabile_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                InsopportabileSound.Play();
        }

        private void Ateo_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                AteoFasulloSound.Play();
        }

        private void Studiare_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                VaiStudiareSound.Play();
        }

        private void Licenziare_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                LicenziareSound.Play();
        }

        private void Cretinate_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                CretinateSound.Play();
        }

        private void CapraIgnorante2_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                CapraIgnoranteSound.Play();
        }

        private void DanteManzoni_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
                LeggaDanteManzoniSound.Play();
        }

        #endregion

        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && TrialManagement.AlreadyOpenedToday)
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            return true;
        }

        private void Base1ApplicationBar_Click(object sender, EventArgs e)
        {
            switch (Base1Loop.State)
            {
                case SoundState.Paused:
                    Base1Loop.Resume();
                    break;
                case SoundState.Playing:
                    Base1Loop.Pause();
                    break;
                default:
                    Base1Loop.Play();
                    break;
            }
        }

        private void Base2ApplicationBar_Click(object sender, EventArgs e)
        {
            switch (Base2Loop.State)
            {
                case SoundState.Paused:
                    Base2Loop.Resume();
                    break;
                case SoundState.Playing:
                    Base2Loop.Pause();
                    break;
                default:
                    Base2Loop.Play();
                    break;
            }
        }

        private void Base3ApplicationBar_Click(object sender, EventArgs e)
        {
            switch (Base3Loop.State)
            {
                case SoundState.Paused:
                    Base3Loop.Resume();
                    break;
                case SoundState.Playing:
                    Base3Loop.Pause();
                    break;
                default:
                    Base3Loop.Play();
                    break;
            }
        }

        private void DisclaimerApplicationBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void AboutApplicationBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

    }
}