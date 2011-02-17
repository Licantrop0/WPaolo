using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;

namespace SgarbiMix
{
    public partial class LoadingPage : PhoneApplicationPage
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            var sr = SoundsResources.ResourceManager
                .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>();

            LoadingProgress.Maximum = sr.Count();

            var bw = new BackgroundWorker() { WorkerReportsProgress = true };
            bw.DoWork += (sender, e) =>
            {
                foreach (var res in sr)
                {
                    App.Sounds.Add(
                        new KeyValuePair<string, SoundEffect>(
                            //Convenzione: "_" = spazio, "1" = punto esclamativo
                            res.Key.ToString().Replace("_", " ").Replace("1", "!"),
                            SoundEffect.FromStream((UnmanagedMemoryStream)res.Value)));
                    bw.ReportProgress(0);
                }
            };

            bw.ProgressChanged += (sender, e) =>
            {
                LoadingProgress.Value += 1;
                if (LoadingProgress.Value == LoadingProgress.Maximum)
                    NavigationService.Navigate(new Uri("/PlaySoundPage.xaml", UriKind.Relative));
            };

            bw.RunWorkerAsync();
        }

    }
}