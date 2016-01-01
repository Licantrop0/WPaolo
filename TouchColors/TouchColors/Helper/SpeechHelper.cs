using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace TouchColors.Helper
{
    public class SpeechHelper : ISpeechHelper
    {
        private readonly SpeechSynthesizer _synth;
        private readonly MediaPlayer _mediaPlayer;
        private SpeechRecognizer _speechRecognizer;
        private IAsyncOperation<SpeechRecognitionResult> _recognitionOperation;
        private TaskCompletionSource<TimeSpan> _mediaCompletedTCS;

        public SpeechHelper()
        {
            _mediaPlayer = BackgroundMediaPlayer.Current;
            _mediaPlayer.MediaOpened += Mp_MediaOpened;
            _synth = new SpeechSynthesizer();
            _synth.Voice = SpeechSynthesizer.DefaultVoice; //SpeechSynthesizer.AllVoices.First(v => v.Language == "en-US");
        }

        public async Task<bool> InitializeSpeech(IEnumerable<string> responses)
        {
            var permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
            {
                _speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
                var listConstraint = new SpeechRecognitionListConstraint(responses, "colors");
                _speechRecognizer.Constraints.Add(listConstraint);
                await _speechRecognizer.CompileConstraintsAsync();
            }

            return permissionGained;
        }

        public async Task Speak(string text)
        {
            _mediaCompletedTCS = new TaskCompletionSource<TimeSpan>();
            using (var speechStream = await _synth.SynthesizeTextToStreamAsync(text))
            {
                _mediaPlayer.SetStreamSource(speechStream);
                var duration = await _mediaCompletedTCS.Task;
                _mediaPlayer.Play();
                await Task.Delay(duration); //Delay to avoid speech recognizer to start too soon.
            }
        }

        private void Mp_MediaOpened(MediaPlayer sender, object args)
        {
            //Empirically dividing by 2 because of SpeechSynthesizer fucker
            var ms = sender.NaturalDuration.TotalMilliseconds / 2;
            var duration = TimeSpan.FromMilliseconds(ms);
            _mediaCompletedTCS.SetResult(duration);
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