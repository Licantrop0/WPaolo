using System.Collections.Generic;
using System.Threading.Tasks;
using TouchColors.Helper;

namespace TouchColors.DesignMode
{
    public class DesignSpeechHelper : ISpeechHelper
    {
        public Task<bool> InitializeSpeech(IEnumerable<string> responses)
        {
            return Task.FromResult(true);
        }

        public Task<string> Recognize()
        {
            return Task.FromResult(string.Empty);
        }

        public Task Speak(string text)
        {
            return Task.Delay(0);
        }
    }
}