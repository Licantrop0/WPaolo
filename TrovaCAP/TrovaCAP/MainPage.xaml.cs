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
        selezionaViaFrazione,
        selezionaCivici,
        finished
    }

    public partial class MainPage : PhoneApplicationPage
    {
        #region private members

        private Comune[] _comuni;

        Step _state;
        Step _resumeState;

        string _sComuneSelezionato = "";
        string _sFrazioneSelezionata = "";
        string _sIndirizzoSelezionato = "";
        string _sCapSelezionato = "";

        CAPRecord[] _capRecords;
        Stopwatch sw = new Stopwatch();

        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            sw.Start();
            ReadAndParseDataBase();
            sw.Stop();
            tbBenchmark.Text = "db: " + sw.ElapsedMilliseconds + " is: ";

            sw.Reset();
            sw.Start();
            acbComuni.ItemsSource = _comuni.Select(c => c.ComuneID);
            sw.Stop();

            tbBenchmark.Text += sw.ElapsedMilliseconds.ToString();

            _state = Step.selezionaComune;

            Reset();
        }

        private void Reset()
        {
            
            acbIndirizzi.ItemsSource = null;

            acbComuni.IsEnabled = true;
            acbComuni.Text = "";

            acbFrazioni.ItemsSource = null;
            acbFrazioni.IsEnabled = false;

            acbFrazioni.Text = "";
            acbIndirizzi.IsEnabled = false;
            acbIndirizzi.Text = "";
            tbCapResult.Text = "";

            this.Focus();
        }

        public AutoCompleteFilterPredicate<object> FilterItem
        {
            get
            {
                return AcbFilterContainExtended;
            }

        }

        private bool AcbFilterContainExtended(string search, object data)
        {
            if (search == "")
            {
                return true;
            }
            else
            {
                string value = data as string;
                if (value.Contains(search))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void acbComuni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_state == Step.selezionaComune)
            {
                bool bAutoFocus = false;

                if (_comuni.Any(c=> c.ComuneID == acbComuni.Text))
                {
                    _sComuneSelezionato = acbComuni.Text;
                    _capRecords = _comuni.Where(c => c.ComuneID == _sComuneSelezionato).Single().CapRecords;

                    // se i CAPRecords ritornano un solo CAP ritornalo
                    int countCAP = (from cr in _capRecords
                                    select cr.Cap).Distinct().Count();

                    if (countCAP == 1)
                    {
                        ShowResult();
                    }
                    else
                    {
                        //acbComuni.IsEnabled = false;  // sempre tutti abilitati!

                        // populate indirizzi
                        var sIndirizzi =  (from capRecord in _capRecords
                                           where capRecord.Indirizzo != ""
                                           select capRecord.Indirizzo).Distinct();

                        if (sIndirizzi.Count() == 0)
                        {
                            _state = Step.selezionaFrazione;
                        }

                        // populate frazioni
                        List<string> sFrazioni = (from capRecord in _capRecords
                                                  where capRecord.Frazione != ""
                                                  select capRecord.Frazione).Distinct().ToList();
                        
                        if (sFrazioni.Count == 0)
                        {
                            _state = Step.selezionaVia;
                        }

                        if (_state != Step.selezionaFrazione && _state != Step.selezionaVia)
                        {
                            _state = Step.scegliFrazioneOVia;
                        }

                        if (_state == Step.selezionaFrazione || _state == Step.scegliFrazioneOVia)
                        {
                            sFrazioni.Insert(0, _sComuneSelezionato + " (nessuna frazione)");
                            acbFrazioni.ItemsSource = sFrazioni;
                            acbFrazioni.IsEnabled = true;

                            acbFrazioni.Focus();
                            acbFrazioni.IsDropDownOpen = true;
                            bAutoFocus = true;
                        }

                        if (_state == Step.selezionaVia || _state == Step.scegliFrazioneOVia)
                        {
                            acbIndirizzi.ItemsSource = sIndirizzi;
                            acbIndirizzi.IsEnabled = true;

                            if (_state == Step.selezionaVia)
                            {
                                acbIndirizzi.Focus();
                                bAutoFocus = true;
                            }
                        }
                    }
                }
                else
                {
                    acbComuni.Text = "";
                    // <produci un suono fastidioso e manda un messaggio di errore>
                }

                if (!bAutoFocus)
                    this.Focus();
            }

            //acbComuni.IsEnabled = true;
        }

        

        private void acbFrazioni_GotFocus(object sender, RoutedEventArgs e)
        {
            /*if (_state == Step.scegliFrazioneOVia)
            {
                _state = Step.scegliFrazione;
                acbFrazioni.IsDropDownOpen = true;
            }
            else if (_state == Step.selezionaFrazione || _state == Step.selezionaViaFrazione)
            {
                acbFrazioni.IsDropDownOpen = true;
            }*/
            acbFrazioni.IsDropDownOpen = true;
        }


        private void acbFrazioni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_state == Step.scegliFrazione || _state == Step.selezionaViaFrazione || _state == Step.selezionaFrazione)
            {
                bool bAutoFocus = false;

                if ((acbFrazioni.ItemsSource as List<string>).Contains(acbFrazioni.Text))
                {
                    _sFrazioneSelezionata = acbFrazioni.Text;
                    if (_sFrazioneSelezionata == _sComuneSelezionato + " (nessuna frazione)")
                        _sFrazioneSelezionata = "";

                    if (_state == Step.selezionaFrazione)
                        _sIndirizzoSelezionato = "";

                    if (_state == Step.selezionaViaFrazione || _state == Step.selezionaFrazione)
                    {
                        _capRecords = (from cr in _capRecords
                                       where (cr.Indirizzo == _sIndirizzoSelezionato && cr.Frazione == _sFrazioneSelezionata)
                                       select cr).ToArray();
                    }
                    else if (_state == Step.scegliFrazione)
                    {
                        _capRecords = (from cr in _capRecords
                                       where cr.Frazione == _sFrazioneSelezionata
                                       select cr).ToArray();
                    }

                    int countCAP = (from cr in _capRecords
                                    select cr.Cap).Distinct().Count();

                    if (countCAP == 1)
                    {
                        ShowResult();
                    }
                    else if (_sFrazioneSelezionata == "")
                    {
                        acbFrazioni.IsEnabled = false;    // i controlli si lasciano sempre tutti attivi
                        if (_state == Step.scegliFrazione)
                        {
                            _state = Step.selezionaFrazioneVia;
                            acbIndirizzi.ItemsSource = from cr in _capRecords
                                                       select cr.Indirizzo;

                            bAutoFocus = true;
                            acbIndirizzi.Focus();
                        }
                        else
                        {
                            // I must never get here!
                            throw new Exception("FANCULO!!!!!!!!");
                        }
                    }
                }
                else
                {
                    acbIndirizzi.Text = "";
                    // <produci un suono fastidioso e manda un messaggio di errore>
                    // non cambio stato
                }

                if (!bAutoFocus)
                    this.Focus();
            }
        }

        private void acbIndirizzi_GotFocus(object sender, RoutedEventArgs e)
        {
            // non va qua la gestione dello stato, la via deve essere stata effettivamente scelta per avere un cambio di stato!
            if (_state == Step.scegliFrazioneOVia)
                _state = Step.scegliVia;

            acbComuni.IsEnabled = true;
        }


        private void acbIndirizzi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_state == Step.selezionaVia || _state == Step.scegliVia || _state == Step.selezionaFrazioneVia)
            {
                string sFrazione = "";

                if ((acbIndirizzi.ItemsSource as IEnumerable<string>).Contains(acbIndirizzi.Text))
                {
                    _sIndirizzoSelezionato = acbIndirizzi.Text;

                    if (_state == Step.selezionaFrazioneVia)
                        sFrazione = _sFrazioneSelezionata;

                    if (_state == Step.selezionaVia || _state == Step.selezionaFrazioneVia)
                    {
                        _capRecords = (from cr in _capRecords
                                       where cr.Indirizzo == _sIndirizzoSelezionato
                                       where cr.Frazione == sFrazione
                                       select cr).ToArray();

                        ShowResult();
                    }
                    else
                    {
                        // qua dovrei cmq controllare:
                        // - se esistono frazioni associate all'indirizzo
                        // - se esistono altri civici associati all'indirizzo (insomma è un GRAN CASINO!)
                        _capRecords = (from cr in _capRecords
                                       where cr.Indirizzo == _sIndirizzoSelezionato
                                       select cr).ToArray();

                        // vedo se esistono frazioni associate all'indirizzo
                        int countFrazioni = (from cr in _capRecords
                                             where cr.Frazione != ""
                                             select cr.Frazione).Count();

                        if (countFrazioni > 0)
                        {
                            List<string> sFrazioni = (from cr in _capRecords
                                                      where cr.Frazione != ""
                                                      select cr.Frazione).ToList<string>();

                            sFrazioni.Insert(0, _sComuneSelezionato + " (nessuna frazione)");
                            acbFrazioni.ItemsSource = sFrazioni;
                            _state = Step.selezionaViaFrazione;
                            acbIndirizzi.IsEnabled = false;   // non disabilito più nessun controllo
                        }
                        else
                        {
                            // non ho frazioni associate all'indirizzo, posso avere però diversi civici
                            ShowResult();
                        }
                    }
                }
                else
                {
                    acbIndirizzi.Text = "";
                    // <produci un suono fastidioso e manda un messaggio di errore>
                    // non cambio stato
                }
            }

            this.Focus();
        }


        private void ShowResult()
        {
            _resumeState = _state;  // per resumare lo stato sull'evento pulsanti di edit
            ControlDisable();
            _sCapSelezionato = _capRecords[0].Cap;
            tbCapResult.Text = _sCapSelezionato;

            _state = Step.finished;
            //<produci suono di soddisfazione>
        }

        private void ControlDisable()
        {
            acbFrazioni.IsEnabled = acbIndirizzi.IsEnabled = false;
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


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _state = Step.selezionaComune;
            Reset();
        }

        private void acbComuni_LostFocus(object sender, RoutedEventArgs e)
        {
            //acbIndirizzi.Focus();

            acbFrazioni.Focus();
        }

        //private void FrazioniListPicker_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (_state == Step.scegliFrazioneOVia)
        //    {
        //        _state = Step.scegliFrazione;
        //        //acbFrazioni.IsDropDownOpen = true;
        //        FrazioniListPicker.ListPickerMode = ListPickerMode.Expanded;
        //    }
        //    else if (_state == Step.selezionaFrazione || _state == Step.selezionaViaFrazione)
        //    {
        //        //FrazioniListPicker.m = true;
        //    }

        //}

        //private void FrazioniListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_state == Step.scegliFrazione || _state == Step.selezionaViaFrazione || _state == Step.selezionaFrazione)
        //    {
        //        if (FrazioniListPicker.SelectedItem.ToString() != " ")
        //        {
        //            _sFrazioneSelezionata = FrazioniListPicker.SelectedItem.ToString();
        //            if (_sFrazioneSelezionata == _sComuneSelezionato + " (nessuna frazione)")
        //                _sFrazioneSelezionata = "";

        //            if (_state == Step.selezionaFrazione)
        //                _sIndirizzoSelezionato = "";

        //            if (_state == Step.selezionaViaFrazione || _state == Step.selezionaFrazione)
        //            {
        //                _capRecords = (from cr in _capRecords
        //                               where (cr.Indirizzo == _sIndirizzoSelezionato && cr.Frazione == _sFrazioneSelezionata)
        //                               select cr).ToArray();
        //            }
        //            else if (_state == Step.scegliFrazione)
        //            {
        //                _capRecords = (from cr in _capRecords
        //                               where cr.Frazione == _sFrazioneSelezionata
        //                               select cr).ToArray();
        //            }

        //            int countCAP = (from cr in _capRecords
        //                            select cr.Cap).Distinct().Count();

        //            if (countCAP == 1)
        //            {
        //                ShowResult();
        //            }
        //            else if (_sFrazioneSelezionata == "")
        //            {
        //                //acbFrazioni.IsEnabled = false;    // i controlli si lasciano sempre tutti attivi
        //                if (_state == Step.scegliFrazione)
        //                {
        //                    _state = Step.selezionaFrazioneVia;
        //                    acbIndirizzi.Focus();
        //                }
        //                else
        //                {
        //                    // I must never get here!
        //                    throw new Exception("FANCULO!!!!!!!!");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            acbIndirizzi.Text = "";
        //            // <produci un suono fastidioso e manda un messaggio di errore>
        //            // non cambio stato
        //        }
        //    }

        //    this.Focus();

        //}
    }
}