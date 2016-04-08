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
        public event EventHandler<SpeechRecognizerState> SpeechRecognizerStateChanged;

        private readonly SpeechSynthesizer _synth;
        private readonly MediaPlayer _mediaPlayer;

        private SpeechRecognizer _speechRecognizer;
        private IAsyncOperation<SpeechRecognitionResult> _recognitionOperation;
        private TaskCompletionSource<object> _mediaCompletedTCS;
        private Task<object> _speechOperation;

        public SpeechHelper()
        {
            _mediaPlayer = BackgroundMediaPlayer.Current;
            _mediaPlayer.MediaOpened += Mp_MediaOpened;
            _synth = new SpeechSynthesizer { Voice = SpeechSynthesizer.DefaultVoice };
            //SpeechSynthesizer.AllVoices.First(v => v.Language == "en-US");
        }

        public async Task<bool> InitializeRecognition(IEnumerable<string> responses)
        {
            if(!(await AudioCapturePermissions.RequestMicrophonePermission()))
                return false;

            _speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
            _speechRecognizer.StateChanged += speechRecognizer_StateChanged;
            var listConstraint = new SpeechRecognitionListConstraint(responses);
            
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
            if (_speechOperation != null)
            {
                _mediaCompletedTCS.SetCanceled();
                _speechOperation = null;
                return;
            }

            using (var speechStream = await _synth.SynthesizeTextToStreamAsync(text))
            {
                _mediaCompletedTCS = new TaskCompletionSource<object>();
                try
                {
                    _mediaPlayer.SetStreamSource(speechStream);
                    _speechOperation = _mediaCompletedTCS.Task;
                    await _speechOperation;
                }
                catch (TaskCanceledException) { }
                finally { _speechOperation = null; }
            }
        }

        private async void Mp_MediaOpened(MediaPlayer sender, object args)
        {
            //Empirically dividing by 1.4 because of SpeechSynthesizer fucker
            var ms = (int)(sender.NaturalDuration.TotalMilliseconds / 1.4);

            sender.Play();

            //Delay to avoid speech recognizer to start too soon.
            await Task.Delay(ms);

            if (!_mediaCompletedTCS.Task.IsCanceled)
            {
                _mediaCompletedTCS.SetResult(null);
            }
        }


        public async Task<string> Recognize()
        {
            if (CancelPreviousSpeechRecognitionOperation())
                return null;

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
            finally
            {
                _recognitionOperation = null;
            }
        }

        private bool CancelPreviousSpeechRecognitionOperation()
        {
            if (_speechRecognizer.State == SpeechRecognizerState.Idle || _recognitionOperation == null)
                return false;

            _recognitionOperation.Cancel();
            _recognitionOperation = null;
            return true;
        }
    }
}