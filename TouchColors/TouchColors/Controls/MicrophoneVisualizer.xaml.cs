using System;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Effects;
using Windows.Media.Render;
using Windows.UI.Xaml;
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
            if (DesignMode.DesignModeEnabled)
            {
                var rnd = new Random();
                for (var i = 0; i < 50; i++)
                    Bars.Items.Add(rnd.Next(60));
                return;
            }
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

            var db = double.Parse(_volumeDetectionConfiguration["Volume"].ToString());
            var height = Math.Min(Math.Max((db + 40), 0) * 10, 100); //restrict from 0 to 100 px

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Bars.Items.Add(height);

                if (Bars.Items.Count > 50)
                    Bars.Items.RemoveAt(0);
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

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var isEnabled = (bool) e.NewValue;
            if (isEnabled)
                _audioGraph.Start();
            else
            {
                _audioGraph.Stop();
                Bars.Items.Clear();
            }
        }
    }
}