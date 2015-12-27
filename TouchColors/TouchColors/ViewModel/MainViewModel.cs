using System.Collections.Generic;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Windows.Phone.Speech.Synthesis;
using System.Linq;
using System.Xml.Linq;
using TouchColors.Helper;
using System.Globalization;

namespace TouchColors.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        SpeechSynthesizer synth;
        public Dictionary<string, SolidColorBrush> ColorList { get; set; }
        private KeyValuePair<string, SolidColorBrush> _selectedColor;
        public KeyValuePair<string, SolidColorBrush> SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                RaisePropertyChanged("ColorString");
                synth.SpeakTextAsync(ColorString);
            }
        }

        public string ColorString
        {
            get { return SelectedColor.Key; }
        }

        public MainViewModel()
        {
            ColorList = XElement.Load("Data/colors.xml")
                .Elements()
                .Select(e => new
                {
                    key = e.Attribute("name").Value,
                    value = int.Parse(e.Attribute("value").Value, NumberStyles.HexNumber)
                })
               // .OrderBy(c => c.value)
                .ToDictionary(e => e.key, e => new SolidColorBrush(ColorConverter.FromRgb(e.value)) );

            synth = new SpeechSynthesizer();
            synth.SetVoice(InstalledVoices.All.First(v => v.Language == "en-US"));
        }
    }
}