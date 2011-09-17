using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Capra.Sounds;
using WPCommon;

namespace Capra
{
    public partial class MainPage : PhoneApplicationPage
    {
        Random Rnd = new Random();
        List<BitmapImage> CapreImages;
        int curImg;

        public MainPage()
        {
            InitializeComponent();
            InitializeShaker();
            InitializeImages();
        }

        private void InitializeShaker()
        {
            var sd = new ShakeDetector();
            sd.ShakeDetected += (sender, e) =>
            {
                SoundManager.PlayIgnoranteComeCapra();
            };
            sd.Start();
        }

        private void InitializeImages()
        {
            CapreImages = new List<BitmapImage>();
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
            SoundManager.PlayCapra();
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