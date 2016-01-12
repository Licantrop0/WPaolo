using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace AudioGraphEffects
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public sealed class VolumeDetectionEffect : IBasicAudioEffect
    {
        private AudioEncodingProperties _currentEncodingProperties;
        private readonly List<AudioEncodingProperties> _supportedEncodingProperties;
        private IPropertySet _effectProperties;

        public VolumeDetectionEffect()
        {
            // Let's only work in 32-bit float
            _supportedEncodingProperties = new List<AudioEncodingProperties>()
            {
                AudioEncodingProperties.CreatePcm(48000, 2, 32),
                AudioEncodingProperties.CreatePcm(44100, 2, 32)
            };
            foreach (var prop in _supportedEncodingProperties) { prop.Subtype = MediaEncodingSubtypes.Float; }
        }

        private double VolumeInDecibels
        {
            get { return (double)_effectProperties["Volume"]; }
            set { _effectProperties["Volume"] = value; }
        }

        public IReadOnlyList<AudioEncodingProperties> SupportedEncodingProperties => _supportedEncodingProperties;

        /// <summary>
        /// We are not modifying any audio data, so pass-through audio data
        /// automatically
        /// </summary>
        public bool UseInputFrameForOutput => true;

        public void Close(MediaEffectClosedReason reason)
        {
            // No resources to clean up, so don't worry about Close
        }

        public void DiscardQueuedFrames()
        {
            // We don't cache frames, so we have nothing to discard
        }

        public unsafe void ProcessFrame(ProcessAudioFrameContext context)
        {
            using (var inputBuffer = context.InputFrame.LockBuffer(AudioBufferAccessMode.Read))
            using (var inputReference = inputBuffer.CreateReference())
            {
                byte* inputInBytes;
                uint inputCapacity;
                
                ((IMemoryBufferByteAccess)inputReference).GetBuffer(out inputInBytes, out inputCapacity);

                var inputInFloats = (float*)inputInBytes;
                var inputLength = inputBuffer.Length / sizeof(float);
                float sum = 0;

                // Only process one channel for now (will average out unless the audio is severely unbalanced between left/right)
                for (var i = 0; i < inputLength; i += 2)
                {
                    sum += (inputInFloats[i] * inputInFloats[i]);
                }
                var rms = Math.Sqrt(sum / (inputLength / 2));
                this.VolumeInDecibels = 20 * Math.Log10(rms);
            }
        }

        public void SetEncodingProperties(AudioEncodingProperties encodingProperties)
        {
            _currentEncodingProperties = encodingProperties;
        }

        public void SetProperties(IPropertySet configuration)
        {
            _effectProperties = configuration;
        }
    }
}