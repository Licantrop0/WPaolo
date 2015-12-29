using System.Collections.Generic;
using System.Threading.Tasks;

namespace TouchColors.Helper
{
    public interface ISpeechHelper
    {
        Task<bool> InitializeSpeech(IEnumerable<string> responses);
        Task<string> Recognize();
        Task Speak(string text);
    }
}