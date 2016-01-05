using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace TouchColors.Helper
{
    public interface ISpeechHelper
    {
        event EventHandler<SpeechRecognizerState> SpeechRecognizerStateChanged;
        Task<bool> InitializeRecognition(IEnumerable<string> responses);
        Task<string> Recognize();
        Task Speak(string text);
    }
}