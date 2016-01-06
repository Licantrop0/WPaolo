using System;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Effects;
using Windows.Media.Render;
using Windows.UI.Xaml.Controls;

namespace TouchColors.Controls
{
    public sealed partial class MicrophoneVisualizer : UserControl
    {
        private int _scoreInterval = 0;
        private IPropertySet _volumeDetectionConfiguration;

        public MicrophoneVisualizer()
        {
            this.InitializeComponent();
            CreateAudioGraph();
        }

        private async void CreateAudioGraph()
        {
            var settings = new AudioGraphSettings(AudioRenderCategory.Speech);
            var result = await AudioGraph.CreateAsync(settings);
            var audioGraph = result.Graph;

            var deviceInputNodeResult = await audioGraph.CreateDeviceInputNodeAsync(MediaCategory.Speech);
            var deviceInput = deviceInputNodeResult.DeviceInputNode;

            _volumeDetectionConfiguration = new PropertySet { { "Volume", 0d } };
            deviceInput.EffectDefinitions.Add(
                new AudioEffectDefinition(typeof(VolumeDetectionEffect).FullName,
                _volumeDetectionConfiguration));
            audioGraph.QuantumProcessed += Graph_QuantumProcessed;
        }

        private async void Graph_QuantumProcessed(AudioGraph sender, object args)
        {
            // SCORE: Find the current volume level every 20 intervals
            if (_scoreInterval <= 20)
            {
                _scoreInterval++;
                return;
            }

            var temp = double.Parse(_volumeDetectionConfiguration["Volume"].ToString());
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (temp >= -140 && temp < -60)
                    this.Score.Text = ""; // no bars
                else if (temp >= -60 && temp < -30)
                    this.Score.Text = ""; // one bar
                else if (temp >= -30 && temp < -15)
                    this.Score.Text = ""; // two bar
                else if (temp >= -15 && temp < -7)
                    this.Score.Text = ""; // three bars
                else if (temp >= -7)
                    this.Score.Text = ""; // four/full bars
            });

            // Reset score interval
            _scoreInterval = 0;
        }
    }
}
