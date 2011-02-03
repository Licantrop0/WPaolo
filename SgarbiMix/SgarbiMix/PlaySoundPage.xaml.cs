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

        SoundEffect mix1Sound;
        SoundEffectInstance mix1Loop;

        public PlaySoundPage()
        {
            InitializeComponent();
            StreamResourceInfo SoundFileInfo;

            //fascista
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/fascista.wav", UriKind.Relative));
            FascistaSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //capra
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/capra1.wav", UriKind.Relative));
            CapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //capra ignorante
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/capraignorante.wav", UriKind.Relative));
            CapraIgnoranteSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //culattoni raccomandati
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/culattoniraccomandati.wav", UriKind.Relative));
            CulattoniSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //mi state sui coglioni di principio
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/mistatesuicoglionidiprincipio.wav", UriKind.Relative));
            CoglioniSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //crimine
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/e1crimine1.wav", UriKind.Relative));
            CrimineSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //ridicolo
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/ridicolo1.wav", UriKind.Relative));
            RidicoloSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //ridicolo 6 ma 6 sempre stato
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/ridicolo6ma6semprestato1.wav", UriKind.Relative));
            Ridicolo6Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //non ha mai fatto 1 cazzo in vita sua
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/nonhafattomaiuncazzoinvitasua1.wav", UriKind.Relative));
            NonHaFattoSound = SoundEffect.FromStream(SoundFileInfo.Stream);

            //vergogna vergogna
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/vergognavergogna.wav", UriKind.Relative));
            VergognaSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // 6 ignorante come 1 capra
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/61ignorantecome1capra.wav", UriKind.Relative));
            IgnoranteCapraSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // ateo fasullo
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/ateofasullo.wav", UriKind.Relative));
            AteoFasulloSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // criminale
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/criminale.wav", UriKind.Relative));
            CriminaleSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // e' insopportabile
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/einsopportabile.wav", UriKind.Relative));
            InsopportabileSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // e tu sei un pirla
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/etu61pirla.wav", UriKind.Relative));
            PirlaSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // io sono convinto che vi debbano licenziare
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/iosonoconvintochevidebbanolicenziare.wav", UriKind.Relative));
            LicenziareSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // leggi studia
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/leggistudia.wav", UriKind.Relative));
            LeggiStudiaSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // lo mangerei vivo
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/lomangereivivo.wav", UriKind.Relative));
            MangereiVivoSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // ma guarda che roba
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/maguardacheroba1.wav", UriKind.Relative));
            GuardaCheRobaSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // vai a studiare
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/vaiastudiare.wav", UriKind.Relative));
            VaiStudiareSound = SoundEffect.FromStream(SoundFileInfo.Stream);

            //legga dante, legga manzoni, impari quella capra
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/leggadanteleggamanzoniimpariquellacapra.wav", UriKind.Relative));
            LeggaDanteManzoniSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            // non dire cretinate
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/nondirecretinate.wav", UriKind.Relative));
            CretinateSound = SoundEffect.FromStream(SoundFileInfo.Stream);

            // mixer 1
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/base1.wav", UriKind.Relative));
            mix1Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
            mix1Loop = mix1Sound.CreateInstance();

            // Set the volume a little lower than full so it becomes the background.
            mix1Loop.Volume = 0.8f;

            // Turn on looping so it runs continually in the background.
            mix1Loop.IsLooped = true;

            // Timer to simulate the XNA game loop (SoundEffect classes are from the XNA Framework)
            DispatcherTimer XnaDispatchTimer = new DispatcherTimer();
            XnaDispatchTimer.Interval = TimeSpan.FromMilliseconds(50);

            // Call FrameworkDispatcher.Update to update the XNA Framework internals.
            XnaDispatchTimer.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };

            // Start the DispatchTimer running.
            XnaDispatchTimer.Start();

        }


        private void Fascista_Click(object sender, RoutedEventArgs e)
        {
            FascistaSound.Play();
        }

        private void Ignorante_Click(object sender, RoutedEventArgs e)
        {
            IgnoranteCapraSound.Play();
        }

        private void Capra_Click(object sender, RoutedEventArgs e)
        {
            CapraSound.Play();
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

        private void CapraIgnorante_Click(object sender, RoutedEventArgs e)
        {
            CapraIgnoranteSound.Play();
        }

        private void Pirla_Click(object sender, RoutedEventArgs e)
        {
            PirlaSound.Play();
        }

        private void LeggiStudia_Click(object sender, RoutedEventArgs e)
        {
            LeggiStudiaSound.Play();
        }

        private void Criminale_Click(object sender, RoutedEventArgs e)
        {
            CriminaleSound.Play();
        }

        private void Guarda_Click(object sender, RoutedEventArgs e)
        {
            GuardaCheRobaSound.Play();
        }

        private void Vergogna_Click(object sender, RoutedEventArgs e)
        {
            VergognaSound.Play();
        }

        private void Mangerei_Click(object sender, RoutedEventArgs e)
        {
            MangereiVivoSound.Play();
        }

        private void Insopportabile_Click(object sender, RoutedEventArgs e)
        {
            InsopportabileSound.Play();
        }

        private void Ateo_Click(object sender, RoutedEventArgs e)
        {
            AteoFasulloSound.Play();
        }

        private void Studiare_Click(object sender, RoutedEventArgs e)
        {
            VaiStudiareSound.Play();
        }

        private void Licenziare_Click(object sender, RoutedEventArgs e)
        {
            LicenziareSound.Play();
        }

        private void Cretinate_Click(object sender, RoutedEventArgs e)
        {
            CretinateSound.Play();
        }

        private void CapraIgnorante2_Click(object sender, RoutedEventArgs e)
        {
            CapraIgnoranteSound.Play();
        }

        private void DanteManzoni_Click(object sender, RoutedEventArgs e)
        {
            LeggaDanteManzoniSound.Play();
        }

        private void mix1CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (mix1Loop.State == SoundState.Paused)
            {
                mix1Loop.Resume();
            }
            else
            {
                mix1Loop.Play();
            }
        }

        private void mix2CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void mix3CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void mix3CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void mix2CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void mix1CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (mix1Loop.State == SoundState.Playing)
            {
                mix1Loop.Pause();
            }
        }
    }
}