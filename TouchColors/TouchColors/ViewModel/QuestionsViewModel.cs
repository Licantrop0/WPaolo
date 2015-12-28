using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using TouchColors.Helper;
using TouchColors.Model;
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

        private string _result;
        public string Result
        {
            get { return _result; }
            set { Set(ref _result, value); }
        }

        public QuestionsViewModel(ISpeechHelper speechHelper)
        {
            _speechHelper = speechHelper;
            _colorList = XElement.Load("Data/SimpleColors.xml").Elements()
                .Select(e => new NamedColor(e.Attribute("name").Value, ColorConverter.FromRgb(e.Attribute("value").Value)))
                .ToList();

            StartQuestioning();
        }

        private async void StartQuestioning()
        {
            await _speechHelper.InitializeSpeech(_colorList.Select(c => c.Name));
            NextColor_Click(null, null);
        }

        public async void NextColor_Click(object sender, RoutedEventArgs e)
        {
            CurrentColor = _colorList[_rnd.Next(_colorList.Count - 1)];
            Result = "What color is this?";
            _speechHelper.Speak(Result);

            if (await _speechHelper.Recognize() == CurrentColor.Name)
            {
                Result = $"{CurrentColor.Name}, GOOD!";
            }
            else
            {
                Result = $"No, this is {CurrentColor.Name}";
            }
            _speechHelper.Speak(Result);
        }
    }
}
