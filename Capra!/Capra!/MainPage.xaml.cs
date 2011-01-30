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
        List<FunFact> FunFacts;
        List<BitmapImage> CapreImages = new List<BitmapImage>();
        SoundEffect CapraSound;
        SoundEffectInstance CapraLoop;
        bool factIsShown;
        int prizeIsShown;

        public MainPage()
        {
            // init base components
            InitializeComponent();
            factIsShown = false;
            prizeIsShown = 0;

            // first load sounds
            InitializeSound();

            // then init xna timer
            // Timer to simulate the XNA game loop (SoundEffect classes are from the XNA Framework)
            DispatcherTimer XnaDispatchTimer = new DispatcherTimer();
            XnaDispatchTimer.Interval = TimeSpan.FromMilliseconds(50);
            // Call FrameworkDispatcher.Update to update the XNA Framework internals.
            XnaDispatchTimer.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            // Start the DispatchTimer running.
            XnaDispatchTimer.Start();
            Settings.TotCapre = 0;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Carica i FunFacts
            XDocument XFunFacts = XDocument.Load("funFacts.xml");
            FunFacts = XFunFacts.Descendants("FunFact").Select(ff =>
                new FunFact(ff.Attribute("Type").Value, ff.Attribute("Text").Value)).ToList();

            //Carica le immagini delle capre
            for (int i = 0; i <= 9; i++)
                CapreImages.Add(new BitmapImage(new Uri("Images\\capra" + i + ".png", UriKind.Relative)));

            SetNewCapraImage();
        }

        private void HideAll()
        {
            //textFunFact.Visibility = Visibility.Collapsed;
            //titleFunFact.Visibility = Visibility.Collapsed;
            SblocButton.Visibility = Visibility.Collapsed;
            //textObiettivo.Visibility = Visibility.Collapsed;        
        }

        private void resetToCapra()
        {
            HideAll();
            SetNewCapraImage();
            factIsShown = false;
        }




        //===============================================================================
        //                        TOUCH & PLAY 
        //===============================================================================

        private void CapraButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            // if it's the first touch after the fun fact is shown, just change capra
            if (factIsShown == true)
            {
                // just remove facts text and change image
                resetToCapra();
            }
            else  // normal touch
            {
                PlayOnce();
            }
        }

        private void CapraButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            // show fun fact only after playing with touch is over
            if (Settings.CountCapre >= 3)
            {
                // show a random fun fact
                this.Focus();
                Settings.CountCapre = 0;
                //showFunFacts();
            }
        }

        void PlayOnce()
        {
            Settings.CountCapre++;
            Settings.TotCapre++;
            CapraSound.Play();
            SetNewCapraImage();
        }


        //===============================================================================
        //                        SOUNDS
        //===============================================================================

        private void InitializeSound()
        {
            StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri("capra.wav", UriKind.Relative));
            CapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);

            CapraLoop = CapraSound.CreateInstance();
            CapraLoop.IsLooped = true;
        }

        //===============================================================================
        //                        FUN FACTS
        //===============================================================================

        //private void showFunFacts()
        //{
        //    // sometimes instead of fun facts I'll show the achievements

        //    if ((Settings.TotCapre >= 10) && (Settings.TotCapre < 100) && (prizeIsShown == 0))
        //    {
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\o10.png", UriKind.Relative));
        //        textObiettivo.Text = "Obiettivo sbloccato!"; textObiettivo.Visibility = Visibility.Visible;
        //        textFunFact.Text = "Hai detto almeno 10 volte Capra!"; textFunFact.Visibility = Visibility.Visible;
        //        titleFunFact.Visibility = Visibility.Collapsed;
        //        prizeIsShown = 1;
        //    }
        //    else if ((Settings.TotCapre >= 100) && (Settings.TotCapre < 500) && (prizeIsShown == 1))
        //    {
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\o100.png", UriKind.Relative));
        //        textObiettivo.Text = "Obiettivo sbloccato!"; textObiettivo.Visibility = Visibility.Visible;
        //        textFunFact.Text = "Hai detto almeno 100 volte Capra!"; textFunFact.Visibility = Visibility.Visible;
        //        titleFunFact.Visibility = Visibility.Collapsed;
        //        prizeIsShown = 2;
        //    }
        //    else if ((Settings.TotCapre >= 500) && (Settings.TotCapre < 1000) && (prizeIsShown == 2))
        //    {
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\o500.png", UriKind.Relative));
        //        textObiettivo.Text = "Obiettivo sbloccato!"; textObiettivo.Visibility = Visibility.Visible;
        //        textFunFact.Text = "Hai detto almeno 500 volte Capra!"; textFunFact.Visibility = Visibility.Visible;
        //        titleFunFact.Visibility = Visibility.Collapsed;
        //        prizeIsShown = 3;
        //    }
        //    else if ((Settings.TotCapre >= 1000) && (prizeIsShown == 3))
        //    {
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\o1000.png", UriKind.Relative));
        //        textObiettivo.Text = "Obiettivi sbloccati!"; textObiettivo.Visibility = Visibility.Visible;
        //        textFunFact.Text = "Hai detto almeno 1000 volte Capra! Congratulazioni, hai sbloccato tutti gli obiettivi! Ora puoi fregiarti del prezioso titolo di Mastro Capraio!"; textFunFact.Visibility = Visibility.Visible;
        //        titleFunFact.Visibility = Visibility.Collapsed;
        //        prizeIsShown = 4;
        //    }
        //    else
        //    {
        //        this.Focus();
        //        int randomFact = Rnd.Next(FunFacts.Count);
        //        titleFunFact.Text = FunFacts[randomFact].Type; titleFunFact.Visibility = Visibility.Visible;
        //        textFunFact.Text = FunFacts[randomFact].Text; textFunFact.Visibility = Visibility.Visible;
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\funFact.png", UriKind.Relative));
        //    }
        //    SblocButton.Visibility = Visibility.Visible;
        //    factIsShown = true;

        //}


        //===============================================================================
        //                        IMAGES
        //===============================================================================

        private void SetNewCapraImage()
        {
            CapraImage.Source = CapreImages[Rnd.Next(CapreImages.Count)];
        }


        //===============================================================================
        //                        ACHIEVEMENTS
        //===============================================================================

        //private void SblocButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        //{
        //    resetToCapra();
        //}

        //private void SblocButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        //{
        //    CapraImage.Source = new BitmapImage(new Uri("Images\\o1000.png", UriKind.Relative));

        //    if (prizeIsShown == 4)
        //    {
        //        textFunFact.Text = "Congratulazioni, hai sbloccato tutti gli obiettivi e puoi fregiarti del titolo di Mastro Capraio. Ora sei anche il massimo esperto mondiale di Capre, e sei autorizzato a dare della capra a chiunque!";
        //        textObiettivo.Text = "Mastro Capraio";
        //    }
        //    else
        //    {
        //        textFunFact.Text = "Devi sbloccare tutti gli obiettivi per poterti fregiare del titolo di Mastro Capraio. Continua a chiamare Capra! C'è pieno di capre intorno a te!";
        //        textObiettivo.Text = "";
        //    }

        //    textObiettivo.Visibility = Visibility.Visible;
        //    textFunFact.Visibility = Visibility.Visible;
        //    titleFunFact.Visibility = Visibility.Collapsed;

        //}


        //===============================================================================
        //                        ABOUT
        //===============================================================================

        private void PageTitle_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void Image_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FunFactPage.xaml", UriKind.Relative));
        }


    }
}