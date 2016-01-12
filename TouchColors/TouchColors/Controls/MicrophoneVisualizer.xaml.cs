using System;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Effects;
using Windows.Media.Render;
using Windows.UI.Xaml.Controls;
using AudioGraphEffects;

namespace TouchColors.Controls
{
    public sealed partial class MicrophoneVisualizer : UserControl
    {
        private int _scoreInterval = 0;
        private IPropertySet _volumeDetectionConfiguration;
        private AudioGraph _audioGraph;

        public MicrophoneVisualizer()
        {
            this.InitializeComponent();
            CreateAudioGraph();
        }        

        private async void CreateAudioGraph()
        {
            var settings = new AudioGraphSettings(AudioRenderCategory.Speech);
            var result = await AudioGraph.CreateAsync(settings);
            _audioGraph = result.Graph;

            var deviceInputNodeResult = await _audioGraph.CreateDeviceInputNodeAsync(MediaCategory.Speech);
            var deviceInput = deviceInputNodeResult.DeviceInputNode;

            _volumeDetectionConfiguration = new PropertySet { { "Volume", 0d } };
            deviceInput.EffectDefinitions.Add(new AudioEffectDefinition(
                typeof(VolumeDetectionEffect).FullName,
                _volumeDetectionConfiguration));

            _audioGraph.QuantumProcessed += Graph_QuantumProcessed;
            _audioGraph.Start();
        }

        private async void Graph_QuantumProcessed(AudioGraph sender, object args)
        {
            // SCORE: Find the current volume level every 20 intervals
            if (_scoreInterval <= 4)
            {
                _scoreInterval++;
                return;
            }
            _scoreInterval = 0;

            var temp = double.Parse(_volumeDetectionConfiguration["Volume"].ToString());
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Hal.Opacity = Math.Min(Math.Max(0, (temp + 45.5)/(-6.4 + 45.5)), 1);

                if (temp < -44)
                    this.Score.Text = ""; // no bars
                else if (temp >= -44 && temp < -38)
                    this.Score.Text = ""; // one bar
                else if (temp >= -38 && temp < -32)
                    this.Score.Text = ""; // two bar
                else if (temp >= -32 && temp < -26)
                    this.Score.Text = ""; // three bars
                else if (temp >= -26)
                    this.Score.Text = ""; // four/full bars
            });

        }

        private void UserControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _audioGraph.QuantumProcessed -= Graph_QuantumProcessed;
            _audioGraph.Stop();
        }

        private void stop_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _audioGraph.Stop();            
        }
    }
}