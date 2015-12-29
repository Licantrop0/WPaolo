﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task InitializeSpeech(IEnumerable<string> responses)
        {
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();

            var listConstraint = new SpeechRecognitionListConstraint(responses, "colors");
            _speechRecognizer.Constraints.Add(listConstraint);
            _speechRecognizer.UIOptions.IsReadBackEnabled = false;
            _speechRecognizer.UIOptions.ShowConfirmation = false;
            _speechRecognizer.UIOptions.AudiblePrompt = "Say the color";
            await _speechRecognizer.CompileConstraintsAsync();
        }

        public async Task Speak(string text)
        {
            var stream = await _synth.SynthesizeTextToStreamAsync(text);
            _mediaElement.SetSource(stream, stream.ContentType);
            _mediaElement.Play();
            await Task.Delay(1000);
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