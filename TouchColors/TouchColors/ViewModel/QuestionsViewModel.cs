using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using TouchColors.Helper;
using TouchColors.Model;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml;

namespace TouchColors.ViewModel
{
    public class QuestionsViewModel : ViewModelBase
    {
        private readonly List<NamedColor> _colorList;
        private SpeechRecognizer _speechRecognizer;
        private Random _rnd = new Random();

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

        public QuestionsViewModel()
        {
            _colorList = XElement.Load("Data/SimpleColors.xml").Elements()
                .Select(e => new NamedColor(e.Attribute("name").Value, ColorConverter.FromRgb(e.Attribute("value").Value)))
                .ToList();

            InitializeSpeech();
        }


        private async void InitializeSpeech()
        {
            var speechLanguage = SpeechRecognizer.SystemSpeechLanguage;

            _speechRecognizer = new SpeechRecognizer(speechLanguage);

            var responses = _colorList.Select(c => c.Name);
            var listConstraint = new SpeechRecognitionListConstraint(responses, "colors");
            _speechRecognizer.UIOptions.ExampleText = @"Color";
            _speechRecognizer.Constraints.Add(listConstraint);
            await _speechRecognizer.CompileConstraintsAsync();
            NextColor_Click(null, null);
        }

        public async void NextColor_Click(object sender, RoutedEventArgs e)
        {
            CurrentColor = _colorList[_rnd.Next(_colorList.Count - 1)];
            Result = "What color is this?";

            var speechRecognitionResult = await _speechRecognizer.RecognizeAsync();
            if (speechRecognitionResult.Text == CurrentColor.Name)
            {
                Result = $"{CurrentColor.Name}, GOOD!";
            }
            else
            {
                Result = $"No, this is {CurrentColor.Name}";
            }

        }
    }
}
