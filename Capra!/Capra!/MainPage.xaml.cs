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
    Copyright (c) 2010 WPME
*/

namespace Capra
{
    public partial class MainPage : PhoneApplicationPage
    {
        Random Rnd = new Random();
        List<BitmapImage> CapreImages = new List<BitmapImage>();
        SoundEffect CapraSound;
        SoundEffect CapraIgnoranteSound;
        SoundEffect IgnoranteComeCapraSound;
        int curImg;

        public MainPage()
        {
            // init base components
            InitializeComponent();

            // first load sounds
            InitializeSound();

            // metto a zero quelle della sessione corrente
            // quelle totali voglio prenderle dallo storage
            Settings.CountCapre = 0;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Carica le immagini delle capre
            for (int i = 0; i <= 17; i++)
                CapreImages.Add(new BitmapImage(new Uri("Images\\capra" + i + ".jpg", UriKind.Relative)));
        }


        //===============================================================================
        //                        SOUNDS
        //===============================================================================

        private void InitializeSound()
        {
            StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri("Sounds/capra.wav", UriKind.Relative));
            CapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            SoundFileInfo = App.GetResourceStream(new Uri("Sounds/capra_ignorante.wav", UriKind.Relative));
            CapraIgnoranteSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            SoundFileInfo = App.GetResourceStream(new Uri("Sounds/ignorante_come_capra.wav", UriKind.Relative));
            IgnoranteComeCapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);
        }

        //===============================================================================
        //                        IMAGES
        //===============================================================================

        private void SetNewCapraImage()
        {
            // bug fix: a volte ri-genera a caso l'img precedente
            // controllo l'index per evitarlo
            int nextImg = Rnd.Next(CapreImages.Count);
            while (nextImg == curImg)
            {
                nextImg = Rnd.Next(CapreImages.Count);
            }

            CapraImage.ImageSource = CapreImages[nextImg];
            curImg = nextImg;
        }


        //===============================================================================
        //                        MAIN ACTIONS
        //===============================================================================

        private void Capra_Click(object sender, RoutedEventArgs e)
        {
            if (TrialManagement.IsTrialMode == false)
            {
                // normale funzionamento

                Settings.CountCapre++;
                Settings.TotCapre++;
                CapraSound.Play();
                SetNewCapraImage();
            }
            else
            {         
                // trial mode: 1 capra al giorno
                // non incrementa neanche il contatore del totale delle capre

                if (TrialManagement.AlreadyOpenedToday)
                { 
                    NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                }
                else
                {
                    if(Settings.CountCapre == 0)
                    {
                        Settings.CountCapre++;
                        CapraSound.Play();
                        SetNewCapraImage();
                    }
                }
                
            }
        }

        private void FunFact_Click(object sender, RoutedEventArgs e)
        {

            if (TrialManagement.IsTrialMode)
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/FunFactPage.xaml", UriKind.Relative));
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

    }
}