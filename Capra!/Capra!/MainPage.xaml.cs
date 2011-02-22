using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Threading;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using WPCommon;


/* 
    Copyright (c) 2010-2011 WPME
*/

namespace Capra
{
    public partial class MainPage : PhoneApplicationPage
    {
        Random Rnd = new Random();
        List<BitmapImage> CapreImages = new List<BitmapImage>();
        SoundEffect CapraSound;
        SoundEffect IgnoranteComeCapraSound;
        int curImg;

        public MainPage()
        {
            InitializeComponent();
            InitializeSound();

            var sd = new ShakeDetector();
            sd.ShakeDetected += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() => { CapraIgnorante_Shake(); });
            };
            sd.Start();
        }

        private void InitializeSound()
        {
            StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri("Sounds/capra_b.wav", UriKind.Relative));
            CapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            SoundFileInfo = App.GetResourceStream(new Uri("Sounds/ignorante_come_capra.wav", UriKind.Relative));
            IgnoranteComeCapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Carica le immagini delle capre
            for (int i = 0; i <= 17; i++)
                CapreImages.Add(new BitmapImage(new Uri("Images\\capra" + i + ".jpg", UriKind.Relative)));
        }

        private void SetNewCapraImage()
        {
            // a volte ri-genera a caso l'img precedente
            // controllo l'index per evitarlo
            int nextImg;
            do { nextImg = Rnd.Next(CapreImages.Count); }
            while (curImg == nextImg);

            CapraImage.ImageSource = CapreImages[nextImg];
            curImg = nextImg;
        }

        #region Click Events

        private void Capra_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
            {
                CapraSound.Play();
                SetNewCapraImage();
                Settings.CountCapre++;
                Settings.TotCapre++;
            }
        }

        private void CapraIgnorante_Shake()
        {
            if (CheckTrial())
            {
                IgnoranteComeCapraSound.Play();
                //SetNewCapraImage();
                Settings.CountCapre++;
            }
        }

        private void FunFact_Click(object sender, RoutedEventArgs e)
        {
            if (TrialManagement.IsTrialMode)
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
            else
                NavigationService.Navigate(new Uri("/FunFactPage.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        } 

        #endregion

        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && (App.AlreadyOpenedToday || Settings.CountCapre >= 3))
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            return true;
        }

    }
}