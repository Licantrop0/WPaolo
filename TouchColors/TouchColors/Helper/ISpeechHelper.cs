﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace TouchColors.Helper
{
    public interface ISpeechHelper
    {
        Task InitializeSpeech(IEnumerable<string> responses);
        Task<string> Recognize();
        void Speak(string text);
    }
}