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

namespace TouchColors.ViewModel
{
    public class QuestionsViewModel : ViewModelBase
    {
        private readonly CoreDispatcher _dispatcher;
        private readonly List<NamedColor> _colorList;
        private readonly ISpeechHelper _speechHelper;
        private readonly string[] _validAnswers = { "{0}, very good!", "Yes, this is {0}", "{0}, good job!", "{0}, you got it!" };
        private string _validAnswer;
        private SemaphoreSlim _clickSemaphore;

        private NamedColor _currentColor;
        public NamedColor CurrentColor
        {
            get { return _currentColor; }
            private set { Set(ref _currentColor, value); }
        }

        private Visibility _isRecognizingVisibility;

        public Visibility IsRecognizingVisibility
        {
            get { return _isRecognizingVisibility; }
            set { Set(ref _isRecognizingVisibility, value); }
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

            _speechHelper = speechHelper;
            _speechHelper.SpeechRecognizerStateChanged += speechHelper_SpeechRecognizerStateChanged;

            _colorList = XElement.Load("Data/RYBColors.xml").Elements()
                .Select(e => new NamedColor(e.Attribute("name").Value, ColorConverter.FromRgb(e.Attribute("value").Value)))
                .ToList();

            StartQuestioning();
        }

        private async void speechHelper_SpeechRecognizerStateChanged(object sender, SpeechRecognizerState state)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsRecognizingVisibility = state == SpeechRecognizerState.Capturing ?
                Visibility.Visible :
                Visibility.Collapsed;
            });
        }

        private async void StartQuestioning()
        {
            var initializated = await _speechHelper.InitializeRecognition(_colorList.Select(c => c.Name));
            if (initializated)
                NextColor_Click(null, null);
        }

        public async void NextColor_Click(object sender, RoutedEventArgs e)
        {
            if (_clickSemaphore.CurrentCount == 0)
                return;
            
            //TODO: reduce semaphore scope
            await _clickSemaphore.WaitAsync();
            try
            {
                CurrentColor = _colorList.GetNextRandomItem(CurrentColor);

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