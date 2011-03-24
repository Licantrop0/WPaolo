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
        string _sFrazioneSelezionata = "";      // TODO resettarla nei resume state

        IEnumerable<string> _sIndirizziComune = null;

        Control _autofocus = null;

        CAPRecord[] _capRecordsComuni = null;
        CAPRecord[] _capRecordsComuniFrazioni = null;

        Stopwatch sw = new Stopwatch();

        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            sw.Start();
            ReadAndParseDataBase();
            //Deserialize();
            sw.Stop();
            tbBenchmark.Text = "db: " + sw.ElapsedMilliseconds + " is: ";

            sw.Reset();
            sw.Start();
            acbComuni.ItemsSource = _comuni.Select(c => c.ComuneID);
            sw.Stop();

            tbBenchmark.Text += sw.ElapsedMilliseconds.ToString();

            acbFrazioni.IsEnabled = false;
            acbIndirizzi.TextFilter = AcbFilterStartsWithExtended;
            acbIndirizzi.IsEnabled = false;
            tbCapResult.Text = "";
            _state = Step.selezionaComune;
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
            acbComuni.SelectionChanged -= acbComuni_SelectionChanged;
            acbComuni.Text = "";
            acbComuni.SelectionChanged += acbComuni_SelectionChanged;
        }

        private void ResetFrazioniTextBox()
        {
            acbFrazioni.SelectionChanged -= acbFrazioni_SelectionChanged;
            acbFrazioni.Text = "";
            acbFrazioni.SelectionChanged += acbFrazioni_SelectionChanged;
        }

        private void ResetIndirizziTextBox()
        {
            acbIndirizzi.SelectionChanged -= acbIndirizzi_SelectionChanged;
            acbIndirizzi.Text = "";
            acbIndirizzi.SelectionChanged += acbIndirizzi_SelectionChanged;
        }

        private void acbComuni_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_state != Step.selezionaComune)
            {
                // reset
                ResetComuniTextBox();
                ResetFrazioniTextBox();
                acbFrazioni.IsEnabled = false;
                ResetIndirizziTextBox();
                acbIndirizzi.IsEnabled = false;
                tbCapResult.Text = "";
                _capRecordsComuni = null;
                 _capRecordsComuniFrazioni = null;
                _state = Step.selezionaComune;
            }
        }

        private void acbComuni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbBenchmark.Text = _state.ToString();

            _autofocus = null;

            if (!_comuni.Any(c => c.ComuneID == acbComuni.Text))        // questo andrebbe messo nel lost focus
            {
                acbComuni.Text = "";
                // <produci un suono fastidioso>
            }
            else
            {
                _sComuneSelezionato = acbComuni.Text;
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
                        acbIndirizzi.ItemsSource = _sIndirizziComune;           // assegnazione itemsSource indirizzi!
                        acbIndirizzi.IsEnabled = true;
                        _autofocus = acbIndirizzi;
                    }
                    else if (_state == Step.selezionaFrazione)
                    {
                        acbFrazioni.ItemsSource = sFrazioni;                    // assegnazione itemsSource frazioni!
                        acbFrazioni.IsEnabled = true;
                        _autofocus = acbFrazioni;
                    }
                    else
                    {
                        acbIndirizzi.ItemsSource = _sIndirizziComune;           // assegnazione itemsSource indirizzi
                        acbIndirizzi.IsEnabled = true;
                        acbFrazioni.ItemsSource = sFrazioni;                    // assegnazione itemsSource frazioni
                        acbFrazioni.IsEnabled = true;
                        _autofocus = this;
                    }
                }
            }

            tbBenchmark.Text = _state.ToString();
        }

        private void acbComuni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!_comuni.Any(c => c.ComuneID == acbComuni.Text))        // questo andrebbe messo nel lost focus
            {
                acbComuni.Text = "";
                // <produci un suono fastidioso>
            }

            Autofocus();
        }

        private void acbFrazioni_GotFocus(object sender, RoutedEventArgs e)
        {
            // sec me va filtrato sullo stato, senno' rimane aperta sta stronza
            if (_state == Step.scegliFrazioneOVia || _state == Step.selezionaFrazione)
            {
               acbFrazioni.IsDropDownOpen = true;
            }
        }

        private void acbFrazioni_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                acbIndirizzi.ItemsSource = _sIndirizziComune;   // importante
                _state = Step.scegliFrazioneOVia;
            }
            else if (_state == Step.scegliViaFinished)
            {
                ResetIndirizziTextBox();
                _state = Step.scegliFrazioneOVia;
            }
        }

        private void acbFrazioni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbBenchmark.Text = _state.ToString();

            /*// filtering is necessary (no more with handlers unlinking)
            if (_state != Step.selezionaFrazione && _state != Step.scegliFrazioneOVia)
            {
                return;
            }*/

            _autofocus = null;

            if (!(acbFrazioni.ItemsSource as IEnumerable<string>).Contains(acbFrazioni.Text))
            {
                acbFrazioni.Text = "";
                // <produci un suono fastidioso>
            }
            else
            {
                if (_state == Step.scegliFrazioneOVia)
                    _state = Step.scegliFrazione;
                
                _sFrazioneSelezionata = acbFrazioni.Text;
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

                    acbIndirizzi.ItemsSource = from cr in _capRecordsComuniFrazioni     // assegnazione itemsSource indirizzi, qui c'è l'errore, ma lo scovo di là...
                                               select cr.Indirizzo;
                    _autofocus = acbIndirizzi;
                }

            }

            tbBenchmark.Text = _state.ToString();
        }

        private void acbFrazioni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(acbFrazioni.ItemsSource as IEnumerable<string>).Contains(acbFrazioni.Text))
            {
                acbFrazioni.Text = "";
                // <produci un suono fastidioso>
            }

            Autofocus();
        }

        private void acbIndirizzi_GotFocus(object sender, RoutedEventArgs e)
        {
            // work around in order to prevent acbFrazioni drop item to open without any reason, it remain a little flickering anyway...
            acbFrazioni.IsDropDownOpen = false;
        }

        private void acbIndirizzi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void acbIndirizzi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbBenchmark.Text = _state.ToString();

            /*// filtering is necessary (no more with handlers unlinking)
            if (_state != Step.selezionaVia && _state != Step.scegliFrazioneOVia && _state != Step.selezionaFrazioneVia)
            {
                return;
            }*/

            _autofocus = null;

            if (!(acbIndirizzi.ItemsSource as IEnumerable<string>).Contains(acbIndirizzi.Text))
            {
                acbIndirizzi.Text = "";
                // <produci un suono fastidioso>  
            }
            else
            {
                if (_state == Step.scegliFrazioneOVia)
                    _state = Step.scegliVia;

                string sFrazioneSelezionata = "";
                string sIndirizzoSelezionato = "";

                if (_state == Step.scegliVia || _state == Step.selezionaVia)
                {
                    // extract indirizzo and frazione from indirizzo string (maybe it can be written better)
                    if (acbIndirizzi.Text.Contains("(fr. "))
                    {
                        string[] words = acbIndirizzi.Text.Split(' ');

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
                        sIndirizzoSelezionato = acbIndirizzi.Text;
                        sFrazioneSelezionata = "";
                    }
                }
                else
                {
                    sIndirizzoSelezionato = acbIndirizzi.Text;
                    sFrazioneSelezionata = _sFrazioneSelezionata;
                }

                CAPRecord[] capRecordsSource = _state == Step.selezionaFrazioneVia ? _capRecordsComuniFrazioni : _capRecordsComuni;

                CAPRecord[] capRecordsResults = (from cr in capRecordsSource
                                                 where cr.Indirizzo == sIndirizzoSelezionato
                                                 where cr.Frazione == sFrazioneSelezionata
                                                 select cr).ToArray();

                ShowResult(capRecordsResults[0].Cap);
            }

            tbBenchmark.Text = _state.ToString();
        }

        private void acbIndirizzi_LostFocus(object sender, RoutedEventArgs e)
        {
            Autofocus();
        }

        private void ShowResult(string sResultCAP)
        {
            switch(_state)
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
            //<produci suono di soddisfazione>
        }


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

        
        private bool AcbFilterStartsWithExtended(string search, object data)
        {
            string[] words = (data as string).Split(' ');
            string   word  = data as string;

            return (words.Where(s => s.StartsWith(search, StringComparison.CurrentCultureIgnoreCase)).Count() > 0) ||
                (word.ToUpper().Contains(search.ToUpper()) && search.Contains(' '));
        }


        // serialize???
            //CapDB capDB = new CapDB(_comuni);

            // serialization
            /*MemoryStream ms = new MemoryStream();
            CustomBinarySerializer ser = new CustomBinarySerializer(capDB.GetType());
            ser.WriteObject(ms, _comuni);

            ms.Position = 0;

            // deserialization
            CustomBinarySerializer ser2 = new CustomBinarySerializer(typeof(CapDB));
            Comune[] comuniAfterDeserialization = ser2.ReadObject(ms) as Comune[];

            _comuni = new Comune[0];
            _comuni = comuniAfterDeserialization;

            ms.Close();*/

            // serialization
            /*StreamResourceInfo sri = Application.GetResourceStream(new Uri("DB.ser", UriKind.Relative));
            CustomBinarySerializer ser = new CustomBinarySerializer(capDB.GetType());
            ser.WriteObject(sri.Stream, _comuni);
            sri.Stream.Close();*/

    }
}