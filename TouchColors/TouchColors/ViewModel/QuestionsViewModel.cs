using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Template10.Mvvm;
using TouchColors.Helper;
using TouchColors.Model;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Xaml;

namespace TouchColors.ViewModel
{
    public class QuestionsViewModel : ViewModelBase
    {
        private readonly List<NamedColor> _colorList;
        private readonly ISpeechHelper _speechHelper;
        private readonly string[] _validAnswers = new[] { "{0}, very good!", "Yes, this is {0}", "{0}, good job!", "{0}, you got it!" };
        private string _lastAnswer;
        private NamedColor _currentColor;
        public NamedColor CurrentColor
        {
            get { return _currentColor; }
            set { Set(ref _currentColor, value); }
        }

        private string _buttonText;
        public string ButtonText
        {
            get { return _buttonText; }
            set { Set(ref _buttonText, value); }
        }

        public QuestionsViewModel(ISpeechHelper speechHelper)
        {
            if (DesignMode.DesignModeEnabled)
            {
                CurrentColor = new NamedColor("Blue", Colors.Blue);
                ButtonText = "Blue, GOOD!";
                return;
            }
            _speechHelper = speechHelper;
            _colorList = XElement.Load("Data/RYBColors.xml").Elements()
                .Select(e => new NamedColor(e.Attribute("name").Value, ColorConverter.FromRgb(e.Attribute("value").Value)))
                .ToList();

            StartQuestioning();
        }

        private async void StartQuestioning()
        {
            var initializated = await _speechHelper.InitializeRecognition(_colorList.Select(c => c.Name));

            if (initializated)
                NextColor_Click(null, null);
        }

        public async void NextColor_Click(object sender, RoutedEventArgs e)
        {

            CurrentColor = _colorList.GetNextRandomItem(CurrentColor);

            ButtonText = "What color is this?";
            await _speechHelper.Speak(ButtonText);

            var result = await _speechHelper.Recognize();

            _lastAnswer = _validAnswers.GetNextRandomItem(_lastAnswer);
            
            ButtonText = result == CurrentColor.Name ?
                string.Format(_lastAnswer, CurrentColor.Name) :
                $"No, this is {CurrentColor.Name}";

            await _speechHelper.Speak(ButtonText);
        }
    }
}