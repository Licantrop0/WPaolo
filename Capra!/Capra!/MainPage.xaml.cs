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


/* 
    Copyright (c) 2010 Serena Ivaldi - serena.ivaldi@gmail.com
*/

namespace Capra
{
    public partial class MainPage : PhoneApplicationPage
    {
        Random Rnd = new Random();
        List<BitmapImage> CapreImages = new List<BitmapImage>();
        SoundEffect CapraSound;
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
            for (int i = 0; i <= 9; i++)
                CapreImages.Add(new BitmapImage(new Uri("Images\\capra" + i + ".png", UriKind.Relative)));
        }


        //===============================================================================
        //                        SOUNDS
        //===============================================================================

        private void InitializeSound()
        {
            StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri("capra.wav", UriKind.Relative));
            CapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);
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
            Settings.CountCapre++;
            Settings.TotCapre++;
            CapraSound.Play();
            SetNewCapraImage();
        }

        private void FunFact_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FunFactPage.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

    }
}