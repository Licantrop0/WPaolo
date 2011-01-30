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
using System.Windows.Resources;
using System.Windows.Threading;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using WPCommon;

/* 
    Copyright (c) 2011 WPME
*/

namespace SgarbiMix
{
    public partial class PlaySoundPage : PhoneApplicationPage
    {
        SoundEffect FascistaSound;
        SoundEffect CapraSound;

        public PlaySoundPage()
        {
            InitializeComponent();
            StreamResourceInfo SoundFileInfo;

            //fascista
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/fascista.wav", UriKind.Relative));
            FascistaSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //capra
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/capra.wav", UriKind.Relative));
            CapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);

        }

        private void Fascista_Click(object sender, RoutedEventArgs e)
        {
            FascistaSound.Play();
        }

        private void Ignorante_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Capra_Click(object sender, RoutedEventArgs e)
        {
            CapraSound.Play();
        }

        private void Taci_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Crimine_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}