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
using System.ComponentModel;
using Microsoft.Xna.Framework.Audio;
using System.Threading;
using System.Diagnostics;
using Microsoft.Phone.Tasks;

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

        Step _state = Step.selezionaComune;
        string _sComuneSelezionato = "";
        string _sFrazioneSelezionata = "";

        List<string> _acbIndirizziOriginalItemsSource = null;
        List<string> _acbIndirizziCashedItemsSource = null;
        string _acbIndirizziCashedSearchKey = "";

        List<string> _sIndirizziComune = null;

        Control _autofocus = null;

        bool _bIndirizziAlreadySelected = false;
        bool _bComuniAlreadySelected = false;
        bool _bFrazioniAlreadySelected = false;

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

        public MainPage()
        {
            InitializeComponent();
            AcbComuni.ItemsSource = DataLayer.ComuniNames;
            AcbIndirizzi.TextFilter = AcbFilterStartsWithExtended;
            AcbFrazioni.TextFilter = AcbFilterStartsWithExtended;
            AcbComuni.IsEnabled = true;
            AcbFrazioni.IsManualDropDownOpen = true;
            if (WPCommon.TrialManagement.IsTrialMode)
                TrialStackPanel.Visibility = Visibility.Visible;
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

            if (WPCommon.TrialManagement.IsTrialMode)
                sResultCAP = sResultCAP.Substring(0, 4) + "?";

            TbCapResult.Text = sResultCAP;
            _autofocus = this;
            PlayCapFoundSound();
        }

        private void CloseDropDown()
        {
            Dispatcher.BeginInvoke(() =>
            {
                AcbComuni.IsDropDownOpen = false;
                AcbIndirizzi.IsDropDownOpen = false;

                if (_state != Step.scegliFrazioneOVia && _state != Step.selezionaFrazione)
                {
                    AcbFrazioni.IsDropDownOpen = false;
                }
            });
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
            AcbIndirizzi.Text = string.Empty;
            AcbIndirizzi.SelectionChanged += AcbIndirizzi_SelectionChanged;
        }

        #endregion

        #region autocomplete boxes event handlers

        private void AcbComuni_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TbLoading.Text = string.Empty;

            if (_state != Step.selezionaComune)
            {
                // reset
                ResetComuniTextBox();
                ResetFrazioniTextBox();
                AcbFrazioni.IsEnabled = false;
                ResetIndirizziTextBox();
                AcbIndirizzi.IsEnabled = false;
                TbCapResult.Text = "";
                _capRecordsComuni = null;
                _capRecordsComuniFrazioni = null;
                _acbIndirizziCashedSearchKey = "";
                _bComuniAlreadySelected = false;
                _bFrazioniAlreadySelected = false;
                _bIndirizziAlreadySelected = false;

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
            CloseDropDown();

            _bComuniAlreadySelected = true;

            _sComuneSelezionato = AcbComuni.Text;
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

                _sIndirizziComune = (from capRecord in _capRecordsComuni
                                     where capRecord.Indirizzo != ""
                                     let ind = string.IsNullOrEmpty(capRecord.Frazione) ? capRecord.Indirizzo : capRecord.Indirizzo + " (fr. " + capRecord.Frazione + ")"
                                     select ind).ToList();

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
                    _acbIndirizziOriginalItemsSource = _sIndirizziComune;
                    AcbIndirizzi.IsEnabled = true;
                    _autofocus = AcbIndirizzi;
                }
                else if (_state == Step.selezionaFrazione)
                {
                    AcbFrazioni.ItemsSource = sFrazioni;
                    AcbFrazioni.IsEnabled = true;
                    _autofocus = AcbFrazioni;
                }
                else
                {
                    _acbIndirizziOriginalItemsSource = _sIndirizziComune;
                    AcbIndirizzi.IsEnabled = true;
                    AcbFrazioni.ItemsSource = sFrazioni;
                    AcbFrazioni.IsEnabled = true;
                    _autofocus = this;
                }
            }
        }

        //private void AcbComuni_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (AcbComuni.Text != "" && AcbComuni.Text != AcbComuni.SelectedItem as string /*.ToString()*/)
        //    {
        //        PlayErrorSound();
        //        AcbComuni.Text = "";
        //    }

        //    Autofocus();
        //}

        private void AcbComuni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AcbComuni.Text != "" && AcbComuni.View.Count == 0)
            {
                PlayErrorSound();
                AcbComuni.Text = "";
            }
            else if (AcbComuni.Text != "" && AcbComuni.View.Count > 0 && !_bComuniAlreadySelected)
            {
                AcbComuni.Text = AcbComuni.View.FirstOrDefault();
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
            TbLoading.Text = string.Empty;

            // reset control and intermediate filtered cap records
            if (_state != Step.selezionaFrazione && _state != Step.scegliFrazioneOVia)
            {
                ResetFrazioniTextBox();
                TbCapResult.Text = "";
                _bFrazioniAlreadySelected = false;
                _bIndirizziAlreadySelected = false;
            }

            // state resume
            if (_state == Step.scegliFrazioneFinished)
                _state = Step.scegliFrazioneOVia;
            else if (_state == Step.selezionaFrazioneFinished)
                _state = Step.selezionaFrazione;
            else if (_state == Step.selezionaFrazioneVia)
            {
                _acbIndirizziOriginalItemsSource = _sIndirizziComune;
                _state = Step.scegliFrazioneOVia;
            }
            else if (_state == Step.SelezionaFrazioneViaFinished)
            {
                ResetIndirizziTextBox();
                _acbIndirizziOriginalItemsSource = _sIndirizziComune;
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
            CloseDropDown();
            //Dispatcher.BeginInvoke(() => { AcbFrazioni.IsDropDownOpen = false; });
            AcbFrazioni.IsDropDownOpen = false;

            _bFrazioniAlreadySelected = true;

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

                _acbIndirizziOriginalItemsSource = (from cr in _capRecordsComuniFrazioni
                                                    select cr.Indirizzo).ToList();

                _autofocus = AcbIndirizzi;
            }
        }

        private void AcbFrazioni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AcbFrazioni.Text != "" && AcbFrazioni.View.Count == 0)
            {
                PlayErrorSound();
                AcbFrazioni.Text = "";
            }
            else if (AcbFrazioni.Text != "" && AcbFrazioni.View.Count > 0 && !_bFrazioniAlreadySelected)
            {
                AcbFrazioni.Text = AcbFrazioni.View.FirstOrDefault();
            }

            Autofocus();
        }

        private void AcbIndirizzi_GotFocus(object sender, RoutedEventArgs e)
        {
            // work around in order to prevent AcbFrazioni drop item to open without any reason, it remain a little flickering anyway...
            //AcbFrazioni.IsDropDownOpen = false;
            //AcbComuni.IsDropDownOpen = false;
        }

        private void AcbIndirizzi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TbLoading.Text = string.Empty;

            // reset control
            if (_state != Step.selezionaVia && _state != Step.scegliFrazioneOVia && _state != Step.selezionaFrazioneVia)
            {
                ResetIndirizziTextBox();
                TbCapResult.Text = "";
                _bIndirizziAlreadySelected = false;
                _bFrazioniAlreadySelected = false;
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
            if (AcbIndirizzi.Text.Length <= 2)
                AcbIndirizzi.MinimumPopulateDelay = 2000;
            else if (AcbIndirizzi.Text.Length == 3)
                AcbIndirizzi.MinimumPopulateDelay = 1500;
            else if (AcbIndirizzi.Text.Length == 4)
                AcbIndirizzi.MinimumPopulateDelay = 1000;
            else if (AcbIndirizzi.Text.Length >= 5)
                AcbIndirizzi.MinimumPopulateDelay = 750;
        }

        private void AcbIndirizzi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _autofocus = null;
            CloseDropDown();

            _acbIndirizziCashedSearchKey = "";

            if (_state == Step.scegliFrazioneOVia)
                _state = Step.scegliVia;

            string sFrazioneSelezionata = "";
            string sIndirizzoSelezionato = "";

            if (_state == Step.scegliVia || _state == Step.selezionaVia)
            {
                _bIndirizziAlreadySelected = true;

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
            if (AcbIndirizzi.Text != "" && AcbIndirizzi.View.Count == 0)
            {
                PlayErrorSound();
                AcbIndirizzi.Text = "";
            }
            else if (AcbIndirizzi.Text != "" && AcbIndirizzi.View.Count > 0 && !_bIndirizziAlreadySelected)
            {
                AcbIndirizzi.Text = AcbIndirizzi.View.FirstOrDefault();
            }

            Autofocus();
        }

        #endregion

        static bool AcbFilterStartsWithExtended(string search, string word)
        {
            string[] words = word.Split(' ');
            string[] searchWords = search.Split(' ');

            /*return searchWords.All(s =>           // LINQ: bello ma nel 90% dei casi inutile
                words.Any(w =>
                    w.StartsWith(s, StringComparison.OrdinalIgnoreCase)));*/

            int wordsCount = words.Count();
            int wordsStartIndex = 0;

            foreach (string sw in searchWords)
            {
                bool match = false;
                int wordsIndex = wordsStartIndex;

                while (wordsIndex < wordsCount)
                {
                    if (words[wordsIndex].StartsWith(sw, StringComparison.OrdinalIgnoreCase))
                    {
                        wordsStartIndex++;
                        match = true;
                        break;
                    }
                    else
                    {
                        wordsStartIndex++;
                        wordsIndex++;
                    }
                }

                if (!match)
                    return false;
            }

            return true;
        }


        private void tbCapResult_GotFocus(object sender, RoutedEventArgs e)
        {
            TbCapResult.SelectAll();
        }

        private void AcbIndirizzi_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;

            // workaround used to filter double access when item is selected
            if (_state == Step.scegliViaFinished || _state == Step.SelezionaFrazioneViaFinished || _state == Step.selezionaViaFinished)
                return;

            AcbIndirizzi.Populating -= AcbIndirizzi_Populating;

            TbLoading.Text = AppResources.Loading;
            TbLoading.Opacity = 1;
            var bw = new BackgroundWorker();

            List<string> itemSource = new List<string>();

            bw.DoWork += (sender1, e1) =>
            {
                string text = (string)e1.Argument;

                if (!(_acbIndirizziCashedSearchKey.Length > 2 && text.Length > _acbIndirizziCashedSearchKey.Length &&
                   AcbFilterStartsWithExtended(_acbIndirizziCashedSearchKey, text)))
                {
                    _acbIndirizziCashedItemsSource = _acbIndirizziOriginalItemsSource;
                }

                _acbIndirizziCashedSearchKey = text;

                // fisso l'itemsource (da ottimizzare, sarebbe meglio eliminare gli item che non ci sono più, non ricrearlo da zero)
                foreach (var s in _acbIndirizziCashedItemsSource)
                {
                    if (AcbFilterStartsWithExtended(text, s))
                        itemSource.Add(s);
                }

                _acbIndirizziCashedItemsSource = itemSource;
            };

            bw.RunWorkerCompleted += (sender1, e1) =>
            {
                AcbIndirizzi.ItemsSource = itemSource;
                AcbIndirizzi.Populating += AcbIndirizzi_Populating;
                if (AcbIndirizzi.ItemsSource.Count() <= 325)
                {
                    TbLoading.Opacity = 0;
                    AcbIndirizzi.PopulateComplete();
                }
                else
                {
                    AcbIndirizzi.ClearView();
                    TbLoading.Text = AppResources.TooManyResults;
                }

            };
            bw.RunWorkerAsync(AcbIndirizzi.Text);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var detailTask = new MarketplaceDetailTask();
            detailTask.Show();
        }
    }
}