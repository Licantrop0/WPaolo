using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;

namespace TouchColors.Helper
{
    public class SpeechHelper : ISpeechHelper
    {
        private readonly SpeechSynthesizer _synth;
        private readonly MediaPlayer _mediaPlayer;
        private SpeechRecognizer _speechRecognizer;
        private IAsyncOperation<SpeechRecognitionResult> _recognitionOperation;
        private TaskCompletionSource<object> _mediaCompletedTCS;
        public event EventHandler<SpeechRecognizerState> SpeechRecognizerStateChanged;

        public SpeechHelper()
        {
            _mediaPlayer = BackgroundMediaPlayer.Current;
            _mediaPlayer.MediaOpened += Mp_MediaOpened;
            _synth = new SpeechSynthesizer { Voice = SpeechSynthesizer.DefaultVoice };
            //SpeechSynthesizer.AllVoices.First(v => v.Language == "en-US");
        }

        public async Task<bool> InitializeRecognition(IEnumerable<string> responses)
        {
            var permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (!permissionGained) return false;

            _speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
            _speechRecognizer.StateChanged += speechRecognizer_StateChanged;
            var listConstraint = new SpeechRecognitionListConstraint(responses, "colors");
            _speechRecognizer.Constraints.Add(listConstraint);
            await _speechRecognizer.CompileConstraintsAsync();
            return true;
        }

        private void speechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            SpeechRecognizerStateChanged?.Invoke(sender, args.State);
        }

        public async Task Speak(string text)
        {
            using (var speechStream = await _synth.SynthesizeTextToStreamAsync(text))
            {
                _mediaCompletedTCS = new TaskCompletionSource<object>();
                _mediaPlayer.SetStreamSource(speechStream);
                var duration = await _mediaCompletedTCS.Task;
            }
        }

        private async void Mp_MediaOpened(MediaPlayer sender, object args)
        {
            //Empirically dividing by 2 because of SpeechSynthesizer fucker
            var ms = (int)(sender.NaturalDuration.TotalMilliseconds / 2);
            sender.Play();

            //Delay to avoid speech recognizer to start too soon.
            await Task.Delay(ms);
            _mediaCompletedTCS.SetResult(null); //ERROR IF PRESSED MANY TIMES
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