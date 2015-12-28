using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public SpeechHelper()
        {
            _mediaElement = new MediaElement();
            _synth = new SpeechSynthesizer();
            _synth.Voice = SpeechSynthesizer.DefaultVoice; //SpeechSynthesizer.AllVoices.First(v => v.Language == "en-US");
            _speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        }

        public async Task InitializeSpeech(IEnumerable<string> responses)
        {
            var listConstraint = new SpeechRecognitionListConstraint(responses, "colors");
            _speechRecognizer.UIOptions.ExampleText = @"Color";
            _speechRecognizer.Constraints.Add(listConstraint);
            await _speechRecognizer.CompileConstraintsAsync();
        }

        public async void Speak(string text)
        {
            var stream = await _synth.SynthesizeTextToStreamAsync(text);
            _mediaElement.SetSource(stream, stream.ContentType);
            _mediaElement.Play();
        }

        public async Task<string> Recognize()
        {
            var result = await _speechRecognizer.RecognizeAsync();
            return result.Text;
        }
    }
}