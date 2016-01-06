using System;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Render;
using Windows.UI.Xaml.Controls;

namespace TouchColors.Controls
{
    public sealed partial class MicrophoneVisualizer : UserControl
    {
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

            //https://channel9.msdn.com/events/Build/2015/3-634 balasiv@microsoft.com
            //deviceInput.EffectDefinitions.Add(new AudioEffectDefinition(typeof(VolumeDetectionEffect)
        }
    }
}
