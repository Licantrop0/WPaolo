﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace TouchColors.Helper
{
    public class SpeechHelper : ISpeechHelper
    {
        private readonly SpeechSynthesizer _synth;
        private readonly MediaElement _mediaElement;
        private readonly SpeechRecognizer _speechRecognizer;
        private IAsyncOperation<SpeechRecognitionResult> _recognitionOperation;

        public SpeechHelper()
        {
            _mediaElement = new MediaElement();
            _synth = new SpeechSynthesizer();
            _synth.Voice = SpeechSynthesizer.DefaultVoice; //SpeechSynthesizer.AllVoices.First(v => v.Language == "en-US");
            _speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        }

        public async Task<bool> InitializeSpeech(IEnumerable<string> responses)
        {
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();

            if (permissionGained)
            {
                var listConstraint = new SpeechRecognitionListConstraint(responses, "colors");
                _speechRecognizer.Constraints.Add(listConstraint);
                await _speechRecognizer.CompileConstraintsAsync();
            }

            return permissionGained;
        }

        public async Task Speak(string text)
        {
            using (var stream = await _synth.SynthesizeTextToStreamAsync(text))
            {
                using (var br = new BinaryReader(stream.AsStreamForRead()))
                {
                    _mediaElement.SetSource(stream, stream.ContentType);
                    _mediaElement.Play();

                    var wave = new WaveFormat(br);
                    var ms = (int)((stream.Size * 8 * 1024 * 1000) / (double)wave.AverageBytesPerSecond);
                    await Task.Delay(ms); //Delay to avoid speech recognizer to start too soon.
                }
            }
        }



        public async Task<string> Recognize()
        {
            if (_speechRecognizer.State != SpeechRecognizerState.Idle && _recognitionOperation != null)
            {
                _recognitionOperation.Cancel();
                _recognitionOperation = null;
                return null;
            }

            try
            {
                _recognitionOperation = _speechRecognizer.RecognizeAsync();
                var result = await _recognitionOperation;
                return result.Text;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }
    }
}