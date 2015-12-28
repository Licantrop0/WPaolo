using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using TouchColors.Helper;
using TouchColors.Model;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace TouchColors.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private SpeechSynthesizer _synth;
        private MediaElement _mediaElement;

        public List<NamedColor> ColorList { get; set; }

        private NamedColor _selectedColor;
        public NamedColor SelectedColor
        {
            get { return _selectedColor; }
            set { Set(ref _selectedColor, value); }
        }

        public MainViewModel()
        {
            ColorList = XElement.Load("Data/AllColors.xml").Elements()
                .Select(e => new NamedColor(e.Attribute("name").Value, ColorConverter.FromRgb(e.Attribute("value").Value)))
                .OrderByDescending(c => c.Luminosity)
                .ToList();

            _mediaElement = new MediaElement();
            _synth = new SpeechSynthesizer();
            _synth.Voice = SpeechSynthesizer.AllVoices.First(v => v.Language == "en-US");
        }

        public async void Item_Click(object sender, ItemClickEventArgs e)
        {
            SelectedColor = (NamedColor)e.ClickedItem;
            var stream = await _synth.SynthesizeTextToStreamAsync(SelectedColor.Name);
            _mediaElement.SetSource(stream, stream.ContentType);
            _mediaElement.Play();
        }
    }
}