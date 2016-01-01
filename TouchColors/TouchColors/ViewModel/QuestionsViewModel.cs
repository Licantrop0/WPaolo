using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using TouchColors.DesignMode;
using TouchColors.Helper;
using TouchColors.Model;
using Windows.UI;
using Windows.UI.Xaml;

namespace TouchColors.ViewModel
{
    public class QuestionsViewModel : ViewModelBase
    {
        private readonly List<NamedColor> _colorList;
        private readonly Random _rnd = new Random();
        private readonly ISpeechHelper _speechHelper;

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
            if (IsInDesignMode)
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
            var initializated = await _speechHelper.InitializeSpeech(_colorList.Select(c => c.Name));

            if (initializated)
                NextColor_Click(null, null);
        }

        public async void NextColor_Click(object sender, RoutedEventArgs e)
        {
            NamedColor nextColor;
            do
            {
                nextColor = _colorList[_rnd.Next(_colorList.Count - 1)];
            } while (CurrentColor == nextColor);

            CurrentColor = nextColor;

            ButtonText = "What color is this?";
            await _speechHelper.Speak(ButtonText);

            var result = await _speechHelper.Recognize();

            ButtonText = result == CurrentColor.Name ?
                $"{CurrentColor.Name}, GOOD!" :
                $"No, this is {CurrentColor.Name}";

            await _speechHelper.Speak(ButtonText);
        }
    }
}