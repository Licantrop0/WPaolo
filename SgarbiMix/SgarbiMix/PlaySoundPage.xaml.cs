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

        SoundEffectInstance mix1Loop;
        SoundEffectInstance mix2Loop;
        SoundEffectInstance mix3Loop;

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
            mix1Loop = SoundEffect.FromStream(App.GetResourceStream(
                new Uri("sounds/base1.wav", UriKind.Relative)).Stream).CreateInstance();
            mix1Loop.IsLooped = true;

            //// mixer 2
            //mix2Loop = SoundEffect.FromStream(App.GetResourceStream(
            //    new Uri("sounds/base1.wav", UriKind.Relative)).Stream).CreateInstance();
            //mix2Loop.IsLooped = true;

            //// mixer 3
            //mix2Loop = SoundEffect.FromStream(App.GetResourceStream(
            //    new Uri("sounds/base1.wav", UriKind.Relative)).Stream).CreateInstance();
            //mix2Loop.IsLooped = true;

        }

        private void InitializeButtonColors()
        {
            //prende tutti i bottoni all'interno della griglia
            foreach (var b in ButtonsGrid.Children.Where(c => c is Button).Cast<Button>())
            {
                //prende le righe pari e colora il bottone di rosso
                if (b.GetRow() % 2 == 0)
                    b.Background = new SolidColorBrush(Colors.Red);
            }
        }

        #region Play Buttons

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

        #endregion


        #region Mix Checkboxes

        private void mix1CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (mix1Loop.State == SoundState.Paused)
                mix1Loop.Resume();
            else
                mix1Loop.Play();
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
                 mix1Loop.Pause();
        }

        #endregion

    }
}