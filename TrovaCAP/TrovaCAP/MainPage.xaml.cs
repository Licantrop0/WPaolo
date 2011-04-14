using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Resources;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using System.ComponentModel;

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

        //Comune[] _comuni = null;
        //string[] _comuniNames = null; 

        Step _state = Step.selezionaComune;
        string _sComuneSelezionato = "";
        string _sFrazioneSelezionata = "";

        IEnumerable<string> _sIndirizziComune = null;

        Control _autofocus = null;

        CAPRecord[] _capRecordsComuni = null;
        CAPRecord[] _capRecordsComuniFrazioni = null;

        #endregion

        #region Sounds LazyLoad

        SoundEffect _capFoundSound;
        public void PlayCapFoundSound()
        {
            if (_capFoundSound == null)
                _capFoundSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("sounds/trovatoCAP.wav", UriKind.Relative)).Stream);

            _capFoundSound.Play();
        }

        SoundEffect _errorSound;
        public void PlayErrorSound()
        {
            if (_errorSound == null)
                _errorSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("sounds/error.wav", UriKind.Relative)).Stream);

            _errorSound.Play();
        }

        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _state = Step.selezionaComune;

            AcbComuni._bSupportDicotomicSearch = true;
            AcbIndirizzi._bCashingMode = true;
            AcbIndirizzi.TextFilter = AcbFilterStartsWithExtended;
            
            tbCapResult.Text = "";
        }

        //Ho spostato l'elaborazione del db dal costruttore al Page_Loaded così la pagina viene già visualizzata
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*var bw = new BackgroundWorker();

            //Evento che gira nel thread separato
            bw.DoWork += (sender1, e1) =>
            {
                WPCommon.ExtensionMethods.StartTrace("Deserializing...");
                //ReadAndParseDataBase();
                Deserialize();
                WPCommon.ExtensionMethods.EndTrace();
            };*/

            //LoadComuni();

            //Il caricamento dei soli Comuni va separato e messo nel costruttore
            //AcbComuni.ItemsSource = _comuniNames;
            AcbComuni.ItemsSource = DataLayer.ComuniNames;

            //bw.RunWorkerAsync();
        }

        #region utility fuctions

        private void ShowResult(string sResultCAP)
        {
            //servono proprio tutti tutti gli stati?
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
            PlayCapFoundSound();
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
            // modula MinimumPrefixLength
            // non male, si potrebbe essere più accurati misurando il numero di items nella _view, il comportamento è però soddisfacente già così            
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
            //_capRecordsComuni = _comuni.Where(c => c.ComuneID == _sComuneSelezionato).Single().CapRecords;
            _capRecordsComuni = DataLayer.Comuni.Where(c => c.ComuneID == _sComuneSelezionato).Single().CapRecords;

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
                    AcbIndirizzi.ItemsSource = _sIndirizziComune/*.OrderBy(s => s)*/;           // assegnazione itemsSource indirizzi
                    AcbIndirizzi.IsEnabled = true;
                    AcbFrazioni.ItemsSource = sFrazioni;                    // assegnazione itemsSource frazioni
                    AcbFrazioni.IsEnabled = true;
                    _autofocus = this;
                }
            }
        }

        private void AcbComuni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AcbComuni.Text != "" && AcbComuni.Text != AcbComuni.SelectedItem as string /*.ToString()*/)
            {
                PlayErrorSound();
                AcbComuni.Text = "";
            }

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

                AcbIndirizzi.ItemsSource = from cr in _capRecordsComuniFrazioni
                                           select cr.Indirizzo;
                _autofocus = AcbIndirizzi;
            }
        }

        private void AcbFrazioni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AcbFrazioni.Text != "" && AcbFrazioni.Text != AcbFrazioni.SelectedItem as string /*.ToString()*/)
            {
                PlayErrorSound();
                AcbFrazioni.Text = "";
            }

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
                PlayErrorSound();
                AcbIndirizzi.Text = "";
            }

            Autofocus();
        }

        #endregion

        

        private bool AcbFilterStartsWithExtended(string search, object data)
        {
            string word = data as string;
            string[] words = (word).Split(' ');

            return (words.Where(s => s.StartsWith(search, StringComparison.CurrentCultureIgnoreCase)).Count() > 0) ||
                 (word.ToUpper().Contains(search.ToUpper()) && search.Contains(' '));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //go to about page
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

    }
}