using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Audio;
using WPCommon;

namespace Capra
{
    public partial class MainPage : PhoneApplicationPage
    {
        Random Rnd = new Random();
        List<BitmapImage> CapreImages;
        private bool SoundCanExecute = true;
        private ShakeDetector sd = new ShakeDetector();
        private DispatcherTimer tmr = new DispatcherTimer();
        int curImg;

        SoundEffect _capraSound;
        public SoundEffect CapraSound
        {
            get
            {
                if (_capraSound == null)
                    _capraSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds/capra_b.wav", UriKind.Relative)).Stream);
                return _capraSound;
            }
        }

        SoundEffect _ignoranteComeCapraSound;
        public SoundEffect IgnoranteComeCapraSound
        {
            get
            {
                if (_ignoranteComeCapraSound == null)
                    _ignoranteComeCapraSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds/ignorante_come_capra.wav", UriKind.Relative)).Stream);
                return _ignoranteComeCapraSound;
            }
        }


        public MainPage()
        {
            InitializeComponent();

            sd.ShakeDetected += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() => { CapraIgnorante_Shake(); });
            };
            sd.Start();

            tmr.Interval = TimeSpan.FromMilliseconds(700);
            tmr.Tick += (sender, e) =>
            {
                tmr.Stop();
                SoundCanExecute = true;
            };
        }

        private void CapraIgnorante_Shake()
        {
            if (SoundCanExecute)
                IgnoranteComeCapraSound.Play();

            SoundCanExecute = false;
            tmr.Start();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Carica le immagini delle capre

            CapreImages =  new List<BitmapImage>();
            for (int i = 0; i <= 17; i++)
                CapreImages.Add(new BitmapImage(new Uri("Images\\capra" + i + ".jpg", UriKind.Relative)));
        }

        private void SetNewCapraImage()
        {
            // a volte ri-genera a caso l'img precedente
            // controllo l'index per evitarlo
            int nextImg;
            do
            {
                nextImg = Rnd.Next(CapreImages.Count);
            }
            while (curImg == nextImg);

            CapraImage.ImageSource = CapreImages[nextImg];
            curImg = nextImg;
        }

        private void Capra_Click(object sender, RoutedEventArgs e)
        {
            CapraSound.Play();
            SetNewCapraImage();
            Settings.TotCapre++;
        }

        private void Sgarbi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new MarketplaceDetailTask()
                {
                    ContentIdentifier = "5925f9d6-483d-e011-854c-00237de2db9e"
                }.Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }

    }
}