using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using TouchColors.Helper;
using TouchColors.Model;
using Windows.Media.SpeechRecognition;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;

namespace TouchColors.ViewModel
{
    public class QuestionsViewModel : ViewModelBase
    {
        private readonly CoreDispatcher _dispatcher;
        private readonly List<NamedColor> _colorList;
        private readonly ISpeechHelper _speechHelper;
        private readonly string[] _validAnswers = { "{0}, very good!", "Yes, this is {0}", "{0}, good job!", "{0}, you got it!" };
        private readonly SemaphoreSlim _clickSemaphore;
        private string _validAnswer;

        public RelayCommand NextColorCommand { get; }
        public RelayCommand StartQuestioningCommand { get; }

        private NamedColor _currentColor;
        public NamedColor CurrentColor
        {
            get { return _currentColor; }
            private set { Set(ref _currentColor, value); }
        }

        private bool _isRecognizing;

        public bool IsRecognizing
        {
            get { return _isRecognizing; }
            set { Set(ref _isRecognizing, value); }
        }

        public QuestionsViewModel(ISpeechHelper speechHelper)
        {
            if (IsInDesignMode)
            {
                CurrentColor = new NamedColor("Blue", Colors.Blue);
                return;
            }

            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            _clickSemaphore = new SemaphoreSlim(1);
            NextColorCommand = new RelayCommand(NextColorAction);
            StartQuestioningCommand = new RelayCommand(StartQuestioningAction);
            _speechHelper = speechHelper;
            _speechHelper.SpeechRecognizerStateChanged += speechHelper_SpeechRecognizerStateChanged;

            _colorList = XElement.Load("Data/RYBColors.xml").Elements()
                .Select(e => new NamedColor(e.Attribute("name").Value, ColorConverter.FromRgb(e.Attribute("value").Value)))
                .ToList();
        }


        private async void speechHelper_SpeechRecognizerStateChanged(object sender, SpeechRecognizerState state)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsRecognizing = state == SpeechRecognizerState.Capturing ||
                                state == SpeechRecognizerState.SoundStarted ||
                                state == SpeechRecognizerState.SoundEnded;
            });
        }

        private async void StartQuestioningAction()
        {
            var initializated = await _speechHelper.InitializeRecognition(_colorList.Select(c => c.Name));
            if (initializated)
                NextColorAction();
        }
        private async void NextColorAction()
        {
            if (_clickSemaphore.CurrentCount == 0)
                return;

            CurrentColor = _colorList.GetNextRandomItem(CurrentColor);

            //TODO: reduce semaphore scope
            await _clickSemaphore.WaitAsync();
            try
            {

                await _speechHelper.Speak("What color is this?");

                var result = await _speechHelper.Recognize();

                _validAnswer = _validAnswers.GetNextRandomItem(_validAnswer);

                var answer = result == CurrentColor.Name
                    ? string.Format(_validAnswer, CurrentColor.Name)
                    : $"No, this is {CurrentColor.Name}";

                await _speechHelper.Speak(answer);
            }
            finally
            {
                _clickSemaphore.Release();
            }
        }
    }
}