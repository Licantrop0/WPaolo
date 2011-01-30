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
        SoundEffect CulattoniSound;
        SoundEffect CoglioniSound;
        SoundEffect CrimineSound;
        SoundEffect RidicoloSound;
        SoundEffect NonHaFattoSound;
        SoundEffect Ridicolo6Sound;

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
            //culattoni raccomandati
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/culattoniraccomandati.wav", UriKind.Relative));
            CulattoniSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //mi state sui coglioni di principio
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/mistatesuicoglionidiprincipio.wav", UriKind.Relative));
            CoglioniSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //crimine
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/e1crimine.wav", UriKind.Relative));
            CrimineSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //ridicolo
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/ridicolo.wav", UriKind.Relative));
            RidicoloSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //ridicolo 6 ma 6 sempre stato
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/ridicolo6ma6semprestato.wav", UriKind.Relative));
            Ridicolo6Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //non ha mai fatto 1 cazzo in vita sua
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/nonhafattomaiuncazzoinvitasua.wav", UriKind.Relative));
            NonHaFattoSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            

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
            CrimineSound.Play();
        }

        private void Coglioni_Click(object sender, RoutedEventArgs e)
        {
            CoglioniSound.Play();
        }

        private void Culattoni_Click(object sender, RoutedEventArgs e)
        {
            CulattoniSound.Play();
        }

        private void Ridicolo_Click(object sender, RoutedEventArgs e)
        {
            RidicoloSound.Play();
        }

        private void Ridicolo6_Click(object sender, RoutedEventArgs e)
        {
            Ridicolo6Sound.Play();
        }

        private void NonHaMaiFatto_Click(object sender, RoutedEventArgs e)
        {
            NonHaFattoSound.Play();
        }
    }
}