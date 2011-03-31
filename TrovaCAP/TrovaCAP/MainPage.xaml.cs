using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.Diagnostics;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace TrovaCAP
{
    enum Step
    {
        selezionaComune,
        scegliFrazioneOVia,
        scegliFrazione,
        scegliVia,
        selezionaFrazione,
        selezionaVia,
        selezionaFrazioneVia,
        selezionaComuneFinished,
        scegliFrazioneFinished,
        scegliViaFinished,
        selezionaFrazioneFinished,
        selezionaViaFinished,
        SelezionaFrazioneViaFinished,
    }

    public partial class MainPage : PhoneApplicationPage
    {
        #region private members

        private Comune[] _comuni = null;

        Step _state = Step.selezionaComune;
        string _sComuneSelezionato = "";
        string _sFrazioneSelezionata = "";

        IEnumerable<string> _sIndirizziComune = null;

        Control _autofocus = null;

        CAPRecord[] _capRecordsComuni = null;
        CAPRecord[] _capRecordsComuniFrazioni = null;

        SoundEffect FoundCAPSound;
        SoundEffect ErrorSound;

        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri("sounds/trovatoCAP.wav", UriKind.Relative));
            FoundCAPSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            SoundFileInfo = App.GetResourceStream(new Uri("sounds/error.wav", UriKind.Relative));
            ErrorSound = SoundEffect.FromStream(SoundFileInfo.Stream);
            //ReadAndParseDataBase();
            Deserialize();

            AcbComuni.ItemsSource = _comuni.Select(c => c.ComuneID);
            AcbComuni._bSupportDicotomicSearch = true;

            AcbFrazioni.IsEnabled = false;
            AcbIndirizzi.TextFilter = AcbFilterStartsWithExtended;
            AcbIndirizzi.IsEnabled = false;
            tbCapResult.Text = "";
            _state = Step.selezionaComune;

            // Timer to simulate the XNA game loop (SoundEffect classes are from the XNA Framework)
            DispatcherTimer XnaDispatchTimer = new DispatcherTimer();
            XnaDispatchTimer.Interval = TimeSpan.FromMilliseconds(50);

            // Call FrameworkDispatcher.Update to update the XNA Framework internals.
            XnaDispatchTimer.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };

            // Start the DispatchTimer running.
            XnaDispatchTimer.Start();
        }

        #region utility fuctions

        private void ShowResult(string sResultCAP)
        {
            switch (_state)
            {
                case Step.selezionaComune:
                    _state = Step.selezionaComuneFinished;
                    break;
                case Step.scegliFrazione:
                    _state = Step.scegliFrazioneFinished;
                    break;
                case Step.scegliVia:
                    _state = Step.scegliViaFinished;
                    break;
                case Step.selezionaFrazione:
                    _state = Step.selezionaFrazioneFinished;
                    break;
                case Step.selezionaVia:
                    _state = Step.selezionaViaFinished;
                    break;
                case Step.selezionaFrazioneVia:
                    _state = Step.SelezionaFrazioneViaFinished;
                    break;
                default:
                    throw new Exception("FANCULO!");

            }

            tbCapResult.Text = sResultCAP;
            _autofocus = this;
            FoundCAPSound.Play();
        }

        private void Autofocus()
        {
            if (_autofocus != null)
            {
                _autofocus.Focus();
                _autofocus = null;
            }
        }

        private void ResetComuniTextBox()
        {
            AcbComuni.SelectionChanged -= AcbComuni_SelectionChanged;
            AcbComuni.Text = "";
            AcbComuni.SelectionChanged += AcbComuni_SelectionChanged;
        }

        private void ResetFrazioniTextBox()
        {
            AcbFrazioni.SelectionChanged -= AcbFrazioni_SelectionChanged;
            AcbFrazioni.Text = "";
            AcbFrazioni.SelectionChanged += AcbFrazioni_SelectionChanged;
        }

        private void ResetIndirizziTextBox()
        {
            AcbIndirizzi.SelectionChanged -= AcbIndirizzi_SelectionChanged;
            AcbIndirizzi.Text = "";
            AcbIndirizzi.SelectionChanged += AcbIndirizzi_SelectionChanged;
        }

        #endregion

        #region autocomplete boxes event handlers

        private void AcbComuni_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_state != Step.selezionaComune)
            {
                // reset
                ResetComuniTextBox();
                ResetFrazioniTextBox();
                AcbFrazioni.IsEnabled = false;
                ResetIndirizziTextBox();
                AcbIndirizzi.IsEnabled = false;
                tbCapResult.Text = "";
                _capRecordsComuni = null;
                _capRecordsComuniFrazioni = null;
                _state = Step.selezionaComune;
            }
        }

        private void AcbComuni_TextChanged(object sender, RoutedEventArgs e)
        {
            // modula MinimumPrefixLength   // non male, si potrebbe essere più accurati misurando il numero di items nella _view, il comportamento è però soddisfacente già così
            if (AcbComuni.Text.Length <= 2)
                AcbComuni.MinimumPopulateDelay = 2000;
            else if (AcbComuni.Text.Length == 3)
                AcbComuni.MinimumPopulateDelay = 1000;
            else if (AcbComuni.Text.Length == 4)
                AcbComuni.MinimumPopulateDelay = 500;
            else if (AcbComuni.Text.Length == 5)
                AcbComuni.MinimumPopulateDelay = 250;
            else
                AcbComuni.MinimumPopulateDelay = 0;
        }

        private void AcbComuni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _autofocus = null;

            _sComuneSelezionato = AcbComuni.Text;
            _capRecordsComuni = _comuni.Where(c => c.ComuneID == _sComuneSelezionato).Single().CapRecords;

            // if CAPRecords returns only one CAP job is finished
            int countCAP = (from cr in _capRecordsComuni
                            select cr.Cap).Distinct().Count();

            if (countCAP == 1)
            {
                ShowResult(_capRecordsComuni[0].Cap);
            }
            else
            {
                var sFrazioni = (from capRecord in _capRecordsComuni
                                 where capRecord.Frazione != ""
                                 select capRecord.Frazione).Distinct().ToList();

                sFrazioni.Sort();

                if (sFrazioni.Count() == 0)
                {
                    _state = Step.selezionaVia;
                }
                else
                {
                    sFrazioni.Insert(0, _sComuneSelezionato + " (nessuna frazione)");
                }

                _sIndirizziComune = from capRecord in _capRecordsComuni
                                    where capRecord.Indirizzo != ""
                                    let ind = string.IsNullOrEmpty(capRecord.Frazione) ? capRecord.Indirizzo : capRecord.Indirizzo + " (fr. " + capRecord.Frazione + ")"
                                    select ind;

                if (_sIndirizziComune.Count() == 0)
                {
                    _state = Step.selezionaFrazione;
                }

                if (_state != Step.selezionaVia && _state != Step.selezionaFrazione)
                {
                    _state = Step.scegliFrazioneOVia;
                }

                if (_state == Step.selezionaVia)
                {
                    AcbIndirizzi.ItemsSource = _sIndirizziComune;           // assegnazione itemsSource indirizzi!
                    AcbIndirizzi.IsEnabled = true;
                    _autofocus = AcbIndirizzi;
                }
                else if (_state == Step.selezionaFrazione)
                {
                    AcbFrazioni.ItemsSource = sFrazioni;                    // assegnazione itemsSource frazioni!
                    AcbFrazioni.IsEnabled = true;
                    _autofocus = AcbFrazioni;
                }
                else
                {
                    AcbIndirizzi.ItemsSource = _sIndirizziComune;           // assegnazione itemsSource indirizzi
                    AcbIndirizzi.IsEnabled = true;
                    AcbFrazioni.ItemsSource = sFrazioni;                    // assegnazione itemsSource frazioni
                    AcbFrazioni.IsEnabled = true;
                    _autofocus = this;
                }
            }
        }

        private void AcbComuni_LostFocus(object sender, RoutedEventArgs e)
        {
            if ( AcbComuni.Text != "" && AcbComuni.Text != AcbComuni.SelectedItem as string /*.ToString()*/)
            {
                ErrorSound.Play();
                AcbComuni.Text = "";
            }

            /*if (!_comuni.Any(c => c.ComuneID == AcbComuni.Text))
            {
                if (AcbComuni.Text != "")
                    ErrorSound.Play();

                AcbComuni.Text = "";          
            }*/

            Autofocus();
        }

        private void AcbFrazioni_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.scegliFrazioneOVia || _state == Step.selezionaFrazione)
            {
                AcbFrazioni.IsDropDownOpen = true;
            }
        }

        private void AcbFrazioni_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // reset control and intermediate filtered cap records
            if (_state != Step.selezionaFrazione && _state != Step.scegliFrazioneOVia)
            {
                ResetFrazioniTextBox();
                tbCapResult.Text = "";
            }

            // state resume
            if (_state == Step.scegliFrazioneFinished)
                _state = Step.scegliFrazioneOVia;
            else if (_state == Step.selezionaFrazioneFinished)
                _state = Step.selezionaFrazione;
            else if (_state == Step.selezionaFrazioneVia)
                _state = Step.scegliFrazioneOVia;
            else if (_state == Step.SelezionaFrazioneViaFinished)
            {
                ResetIndirizziTextBox();
                AcbIndirizzi.ItemsSource = _sIndirizziComune;   // importante
                _state = Step.scegliFrazioneOVia;
            }
            else if (_state == Step.scegliViaFinished)
            {
                ResetIndirizziTextBox();
                _state = Step.scegliFrazioneOVia;
            }
        }

        private void AcbFrazioni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _autofocus = null;

            if (_state == Step.scegliFrazioneOVia)
                _state = Step.scegliFrazione;

            _sFrazioneSelezionata = AcbFrazioni.Text;
            if (_sFrazioneSelezionata == _sComuneSelezionato + " (nessuna frazione)")
                _sFrazioneSelezionata = "";

            _capRecordsComuniFrazioni = (from cr in _capRecordsComuni
                                         where cr.Frazione == _sFrazioneSelezionata
                                         select cr).ToArray();

            int countCAP = (from cr in _capRecordsComuniFrazioni
                            select cr.Cap).Distinct().Count();

            if (countCAP == 1)
            {
                ShowResult(_capRecordsComuniFrazioni[0].Cap);
            }
            else
            {
                _state = Step.selezionaFrazioneVia;

                AcbIndirizzi.ItemsSource = from cr in _capRecordsComuniFrazioni     // assegnazione itemsSource indirizzi, qui c'è l'errore, ma lo scovo di là...
                                           select cr.Indirizzo;
                _autofocus = AcbIndirizzi;
            }
        }

        private void AcbFrazioni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AcbFrazioni.Text != "" && AcbFrazioni.Text != AcbFrazioni.SelectedItem as string /*.ToString()*/)
            {
                ErrorSound.Play();
                AcbFrazioni.Text = ""; 
            }

            /*if (!(AcbFrazioni.ItemsSource as IEnumerable<string>).Contains(AcbFrazioni.Text))
            {
                if (AcbFrazioni.Text != "")
                    ErrorSound.Play();

                AcbFrazioni.Text = ""; 
            }*/

            Autofocus();
        }

        private void AcbIndirizzi_GotFocus(object sender, RoutedEventArgs e)
        {
            // work around in order to prevent AcbFrazioni drop item to open without any reason, it remain a little flickering anyway...
            AcbFrazioni.IsDropDownOpen = false;
        }

        private void AcbIndirizzi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // reset control
            if (_state != Step.selezionaVia && _state != Step.scegliFrazioneOVia && _state != Step.selezionaFrazioneVia)
            {
                ResetIndirizziTextBox();
                tbCapResult.Text = "";
            }

            // state resume
            if (_state == Step.scegliViaFinished)
                _state = Step.scegliFrazioneOVia;
            else if (_state == Step.selezionaViaFinished)
                _state = Step.selezionaVia;
            else if (_state == Step.SelezionaFrazioneViaFinished)
            {
                _state = Step.selezionaFrazioneVia;
            }
            else if (_state == Step.scegliFrazioneFinished)
            {
                ResetFrazioniTextBox();
                _state = Step.scegliFrazioneOVia;
            }
        }

        private void AcbIndirizzi_TextChanged(object sender, RoutedEventArgs e)
        {
            // modula MinimumPrefixLength  // no è inutile, devo scalarmo sul numero di record, lo posso faro quando carico la item soure
                                           // milano si comporta troppo diversamente da palermo
            if (AcbIndirizzi.Text.Length <= 2)
                AcbIndirizzi.MinimumPopulateDelay = 2000;
            else if (AcbIndirizzi.Text.Length == 3)
                AcbIndirizzi.MinimumPopulateDelay = 1000;
            else if (AcbIndirizzi.Text.Length >= 4)
                AcbIndirizzi.MinimumPopulateDelay = 500;
            /*else if (AcbIndirizzi.Text.Length == 5)
                AcbIndirizzi.MinimumPopulateDelay = 250;
            else
                AcbIndirizzi.MinimumPopulateDelay = 0;*/
        }

        private void AcbIndirizzi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _autofocus = null;

            if (_state == Step.scegliFrazioneOVia)
                _state = Step.scegliVia;

            string sFrazioneSelezionata = "";
            string sIndirizzoSelezionato = "";

            if (_state == Step.scegliVia || _state == Step.selezionaVia)
            {
                // extract indirizzo and frazione from indirizzo string (maybe it can be written better)
                if (AcbIndirizzi.Text.Contains("(fr. "))
                {
                    string[] words = AcbIndirizzi.Text.Split(' ');

                    int i = 0;
                    while (words[i] != "(fr.")
                    {
                        sIndirizzoSelezionato += words[i] + " ";
                        i++;
                    }
                    sIndirizzoSelezionato = sIndirizzoSelezionato.Remove(sIndirizzoSelezionato.Length - 1, 1);

                    i++;
                    while (!words[i].Contains(")"))
                    {
                        sFrazioneSelezionata += words[i] + " ";
                        i++;
                    }
                    sFrazioneSelezionata += words[i];
                    sFrazioneSelezionata = sFrazioneSelezionata.Remove(sFrazioneSelezionata.Length - 1, 1);
                }
                else
                {
                    sIndirizzoSelezionato = AcbIndirizzi.Text;
                    sFrazioneSelezionata = "";
                }
            }
            else
            {
                sIndirizzoSelezionato = AcbIndirizzi.Text;
                sFrazioneSelezionata = _sFrazioneSelezionata;
            }

            CAPRecord[] capRecordsSource = _state == Step.selezionaFrazioneVia ? _capRecordsComuniFrazioni : _capRecordsComuni;

            CAPRecord[] capRecordsResults = (from cr in capRecordsSource
                                             where cr.Indirizzo == sIndirizzoSelezionato
                                             where cr.Frazione == sFrazioneSelezionata
                                             select cr).ToArray();

            ShowResult(capRecordsResults[0].Cap);
        }

        private void AcbIndirizzi_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AcbIndirizzi.Text != "" && AcbIndirizzi.Text != AcbIndirizzi.SelectedItem as string)
            {
                ErrorSound.Play();
                AcbIndirizzi.Text = "";
            }

            /*if (!(AcbIndirizzi.ItemsSource as IEnumerable<string>).Contains(AcbIndirizzi.Text))
            {
                if (AcbIndirizzi.Text != "")
                    ErrorSound.Play();

                AcbIndirizzi.Text = "";
            }*/

            Autofocus();
        }

        #endregion

        #region read from files functions

        private void ReadAndParseDataBase()
        {
            var resource = System.Windows.Application.GetResourceStream(new Uri("DBout.txt", UriKind.Relative));
            using (var tr = new StreamReader(resource.Stream))
            {
                int count = int.Parse(tr.ReadLine());
                _comuni = new Comune[count];

                for (int i = 0; i < count; i++)
                {
                    string[] words = tr.ReadLine().Split('|');

                    int nRecordCount = int.Parse(words[1]);
                    _comuni[i] = new Comune(words[0], new CAPRecord[nRecordCount]);

                    for (int j = 0; j < nRecordCount; j++)
                    {
                        string[] parole = tr.ReadLine().Split('|');
                        _comuni[i].CapRecords[j] = new CAPRecord(parole[0], parole[1], parole[2]);
                    }
                }
            }
        }


        private void Deserialize()
        {
            // deserialization
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("DB.ser", UriKind.Relative));
            using (var fin = new StreamReader(sri.Stream))
            {
                CustomBinarySerializer ser2 = new CustomBinarySerializer(typeof(CapDB));
                Comune[] comuniAfterDeserialization = ser2.ReadObject(sri.Stream) as Comune[];
                _comuni = comuniAfterDeserialization;
            }
        }

        #endregion

        private bool AcbFilterStartsWithExtended(string search, object data)
        {
            string[] words = (data as string).Split(' ');
            string   word  = data as string;

            return (words.Where(s => s.StartsWith(search, StringComparison.CurrentCultureIgnoreCase)).Count() > 0) ||
                (word.ToUpper().Contains(search.ToUpper()) && search.Contains(' '));
        }
    }
}