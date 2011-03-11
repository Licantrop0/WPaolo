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

namespace TrovaCAP
{
    enum Step
    {
        scegliProvinciaOComune,
        selezionaProvincia,
        selezionaProvinciaComune,
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

        private CAPDB _capDB;

        Step _state;
        Step _resumeState;

        Dictionary<string, string> _provinceLookUp = new Dictionary<string, string>();

        string _sProvinciaSelezionata = "";
        string _sComuneSelezionato = "";
        string _sFrazioneSelezionata = "";
        string _sIndirizzoSelezionato = "";
        string _sCiviciSelezionato = "";
        string _sCapSelezionato = "";

        List<CAPRecord> _capRecords;

    #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ReadAndParseDataBase();
            ReadAndParseProvinceLookUp();

            _state = Step.scegliProvinciaOComune;

            Reset(); 
        }

        private void Reset()
        {
            messageBox.Text = "Seleziona una provincia o direttamente un comune";
            acbProvince.ItemsSource = null;
            acbComuni.ItemsSource = null;
            acbFrazioni.ItemsSource = null;
            acbIndirizzi.ItemsSource = null;
            acbCivici.ItemsSource = null;

            acbProvince.IsEnabled = true;
            acbProvince.Text = "";
            acbComuni.IsEnabled = true;
            acbComuni.Text = "";
            acbFrazioni.IsEnabled = false;
            acbFrazioni.Text = "";
            acbIndirizzi.IsEnabled = false;
            acbIndirizzi.Text = "";
            acbCivici.IsEnabled = false;
            acbCivici.Text = "";
            tbCap.Text = "";
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
                if(value.Contains(search))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        private void acbProvince_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.scegliProvinciaOComune)
            {
                acbProvince.ItemsSource = _provinceLookUp.Keys;
                _state = Step.selezionaProvincia;
            }
        }

        private void acbProvince_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.selezionaProvincia)
            {
                if (_provinceLookUp.ContainsKey(acbProvince.Text))
                {
                    _sProvinciaSelezionata = _provinceLookUp[acbProvince.Text];
                    acbComuni.ItemsSource = _capDB.Province[_sProvinciaSelezionata].Keys;
                    _state = Step.selezionaProvinciaComune;
                    acbProvince.IsEnabled = false;
                    messageBox.Text = "Ora seleziona un comune";
                    // non riesco a portare il controllo sul comune
                    //acbComuni.Focus();
                    //acbComuni.CaptureMouse();
                }
                else
                {
                    acbProvince.Text = "";
                    messageBox.Text = "Provincia errata, riprova o seleziona direttamente un comune";
                    // <produci un suono di errore>
                    _state = Step.scegliProvinciaOComune;
                    this.Focus();
                }
           } 

            this.Focus();
        }

        private void acbComuni_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.scegliProvinciaOComune)
            {
                acbComuni.ItemsSource = _capDB.Comuni.Keys;
                acbProvince.IsEnabled = false;
                _state = Step.selezionaComune;
            }
        }

        private void acbComuni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.selezionaProvinciaComune || _state == Step.selezionaComune)
            {
                Dictionary<string, Comune> comuniCandidati;
                
                if (_state == Step.selezionaProvinciaComune)
                    comuniCandidati = _capDB.Province[_sProvinciaSelezionata];
                else
                    comuniCandidati = _capDB.Comuni;

                if (comuniCandidati.ContainsKey(acbComuni.Text))
                {
                    _sComuneSelezionato = acbComuni.Text;
                    _capRecords = comuniCandidati[_sComuneSelezionato].CapRecords;

                    if (_state == Step.selezionaComune)
                    {
                        string sSigla = _sComuneSelezionato.Substring(_sComuneSelezionato.Length - 3, 2);

                        _sProvinciaSelezionata = (from p in _provinceLookUp
                                                  where p.Value == sSigla
                                                  select p.Key).First();

                        acbProvince.Text = _sProvinciaSelezionata;
                    }

                    // se i CAPRecords ritornano un solo CAP ritornalo
                    int countCAP = (from cr in _capRecords
                                    select cr.Cap.CAPString).Distinct().Count();

                    if (countCAP == 1)
                    {
                        ShowResult();
                    }
                    else
                    {
                        acbComuni.IsEnabled = false;

                        // populate indirizzi
                        List<string> sIndirizzi = new List<string>();
                        sIndirizzi.AddRange((from capRecord in _capRecords
                                             where capRecord.Indirizzo != ""                
                                             select capRecord.Indirizzo).Distinct());

                        if (sIndirizzi.Count == 0)
                        {
                            _state = Step.selezionaFrazione;
                            messageBox.Text = "Seleziona una frazione";
                        }
                        else
                        {
                            acbIndirizzi.ItemsSource = sIndirizzi;
                            acbIndirizzi.IsEnabled = true;
                        }

                        // populate frazioni
                        List<string> sFrazioni = new List<string>();
                        sFrazioni.AddRange((from capRecord in _capRecords
                                            where capRecord.Frazione != ""
                                            select capRecord.Frazione).Distinct());

                        if (sFrazioni.Count == 0)
                        {
                            _state = Step.selezionaVia;
                            messageBox.Text = "Seleziona un indirizzo";
                        }
                        else  
                        {
                            sFrazioni.Add("NESSUNA FRAZIONE");    
                            acbFrazioni.ItemsSource = sFrazioni;
                            acbFrazioni.IsEnabled = true;
                        }

                        if (_state != Step.selezionaFrazione && _state != Step.selezionaVia)
                        {
                            _state = Step.scegliFrazioneOVia;
                            messageBox.Text = "Seleziona la frazione o l'indirizzo";
                        }
                    }
                }
                else
                {
                    acbComuni.Text = "";
                    // <produci un suono fastidioso e manda un messaggio di errore>
                    // da rivedere questa parte di codice
                }
            }

            this.Focus();
        }

        private void acbFrazioni_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.scegliFrazioneOVia)
                _state = Step.scegliFrazione;
        }

        private void acbFrazioni_LostFocus(object sender, RoutedEventArgs e)
        {
            if ( _state == Step.scegliFrazione || _state == Step.selezionaViaFrazione || _state == Step.selezionaFrazione)
            {
                if ((acbFrazioni.ItemsSource as List<string>).Contains(acbFrazioni.Text))
                {
                    _sFrazioneSelezionata = acbFrazioni.Text;
                    if (_sFrazioneSelezionata == "NESSUNA FRAZIONE")
                        _sFrazioneSelezionata = "";

                    if (_state == Step.selezionaFrazione)
                        _sIndirizzoSelezionato = "";

                    if (_state == Step.selezionaViaFrazione || _state == Step.selezionaFrazione)
                    {
                        _capRecords = (from cr in _capRecords
                                       where (cr.Indirizzo == _sIndirizzoSelezionato && cr.Frazione == _sFrazioneSelezionata)
                                       select cr).ToList<CAPRecord>();
                    }
                    else if (_state == Step.scegliFrazione)
                    {
                        _capRecords = (from cr in _capRecords
                                       where cr.Frazione == _sFrazioneSelezionata
                                       select cr).ToList<CAPRecord>();
                    }

                    int countCAP = (from cr in _capRecords
                                    select cr.Cap).Distinct().Count();

                    if (countCAP == 1)
                    {
                        ShowResult();
                    }
                    else if (_sFrazioneSelezionata == "")
                    {
                        acbFrazioni.IsEnabled = false;
                        if(_state == Step.scegliFrazione)
                            _state = Step.selezionaFrazioneVia; // potrebbe essere anche seleziona civici, no, dopo la frazione non è possibile..., si' è possibile se uno mette nessuna frazione
                        else
                        {
                            acbCivici.IsEnabled = true;
                            _state = Step.selezionaCivici;      // ok, arriva _capRecords Filtrato
                        }
                    }
                    else
                    {
                        // eccezione!!! non dovrei mai venire qua!
                        messageBox.Text = "FANCULO!";
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

        private void acbIndirizzi_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.scegliFrazioneOVia)
                _state = Step.scegliVia;
        }

        private void acbIndirizzi_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.selezionaVia || _state == Step.scegliVia || _state == Step.selezionaFrazioneVia)
            {
                string sFrazione = "";

                if ((acbIndirizzi.ItemsSource as List<string>).Contains(acbIndirizzi.Text))
                {
                    _sIndirizzoSelezionato = acbIndirizzi.Text;

                    if (_state == Step.selezionaFrazioneVia)
                        sFrazione = _sFrazioneSelezionata;

                    if (_state == Step.selezionaVia || _state == Step.selezionaFrazioneVia)
                    {
                        _capRecords = (from cr in _capRecords
                                       where (cr.Indirizzo == _sIndirizzoSelezionato && cr.Frazione == sFrazione)
                                       select cr).ToList<CAPRecord>();

                        int countCAP = (from cr in _capRecords
                                        select cr.Cap).Distinct().Count();

                        if (countCAP == 1)
                        {
                            ShowResult();
                        }
                        else
                        {
                            acbIndirizzi.IsEnabled = false;
                            acbCivici.IsEnabled = true;
                            _state = Step.selezionaCivici;      // ok, arrivano i _capRecords filtrati
                        }
                    }
                    else        // 
                    {
                        // qua dovrei cmq controllare:
                        // - se esistono frazioni associate all'indirizzo
                        // - se esistono altri civici associati all'indirizzo (insomma è un GRAN CASINO!)
                        _capRecords = (from cr in _capRecords
                                       where cr.Indirizzo == _sIndirizzoSelezionato
                                       select cr).ToList<CAPRecord>();

                        // vedo se esistono frazioni associate all'indirizzo
                        int countFrazioni = (from cr in _capRecords
                                             where cr.Frazione != ""
                                             select cr.Frazione).Count();

                        if (countFrazioni > 0)
                        {
                            List<string> sFrazioni = (from cr in _capRecords
                                                      where cr.Frazione != ""
                                                      select cr.Frazione).ToList<string>();

                            sFrazioni.Add("NESSUNA FRAZIONE");
                            acbFrazioni.ItemsSource = sFrazioni;
                            _state = Step.selezionaViaFrazione;
                            //acbFrazioni.IsEnabled = true;  // workaround...che non funziona...
                            acbIndirizzi.IsEnabled = false;   // porca troia se lo disabilito non mi fa vedere il testo! bug assurdo!
                            //acbIndirizzi.Text = _sIndirizzoSelezionato; // workaround...che non funziona...
                        }
                        else
                        {
                            // non ho frazioni associate all'indirizzo, posso avere però diversi civici
                            acbFrazioni.IsEnabled = false;
                            int countCAP =  (from cr in _capRecords
                                             select cr.Cap).Distinct().Count();

                            if(countCAP == 1)
                            {
                                ShowResult();
                            }
                            else
                            {
                                acbIndirizzi.IsEnabled = false;
                                acbCivici.IsEnabled = true;   
                                _state = Step.selezionaCivici;
                            }
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
 
        private void acbCivici_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.selezionaCivici)
            {
                acbCivici.ItemsSource = (from cr in _capRecords
                                         select cr.Civico).ToList<string>();
            }
        }

        private void acbCivici_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.selezionaCivici)
            {
                if ((acbCivici.ItemsSource as List<string>).Contains(acbCivici.Text))
                {
                    _sCiviciSelezionato = acbCivici.Text;

                    _capRecords = (from cr in _capRecords
                                   where cr.Civico == _sCiviciSelezionato
                                   select cr).ToList<CAPRecord>();

                    acbCivici.IsEnabled = false;

                    ShowResult();
                }
                else
                {
                    acbCivici.Text = "";
                    //<produci un suono fastidioso>
                }
            }

            this.Focus();
        }

        private void ShowResult()
        {
            _resumeState = _state;  // per resumare lo stato sull'evento pulsanti di edit
            ControlDisable();
            _sCapSelezionato = _capRecords[0].Cap.CAPString;
            tbCap.Text = "CAP";
            tbCapResult.Text = _sCapSelezionato;
            messageBox.Text = "Ricerca completa, premi Reset per iniziarne un'altra";
            _state = Step.finished;
            //<produci suono di soddisfazione>
        }

        private void ControlDisable()
        {
            acbProvince.IsEnabled = acbComuni.IsEnabled = acbFrazioni.IsEnabled = acbIndirizzi.IsEnabled = acbCivici.IsEnabled = false;
        }

        private void ReadAndParseProvinceLookUp()
        {
            var resource = System.Windows.Application.GetResourceStream(new Uri("provinceLookUp.txt", UriKind.Relative));
            TextReader tr = new StreamReader(resource.Stream);

            string tmp;
            string provinceName;
            string provinceSigla;

            while ((tmp = tr.ReadLine()) != null)
            {
                string[] words = tmp.Split(' ');
                provinceSigla = words[words.Length - 1];
                provinceName = "";
                for (int i = 0; i < words.Length - 1; i++)
                    provinceName += words[i];

                _provinceLookUp.Add(provinceName, provinceSigla);
            }
        }
       
        private void ReadAndParseDataBase()
        {
            Dictionary<string, Dictionary<string, Comune>> dProvince = new Dictionary<string, Dictionary<string, Comune>>();
            Dictionary<string, Comune> dComuni = new Dictionary<string, Comune>();
            Dictionary<string, CAP> dCAPS = new Dictionary<string, CAP>();
            
            var resource = System.Windows.Application.GetResourceStream(new Uri("DB2.txt", UriKind.Relative));
            TextReader tr = new StreamReader(resource.Stream);

            string tmp;

            // read a line of text
            while ((tmp = tr.ReadLine()) != null)
            {
                // read a line on ASCII file, separate strings on comma char
                string[] words = tmp.Split(',');

                string sProvincia = Normalize(words[0]);

                string sComune1 = Normalize(words[1]);
                string sComune2 = Normalize(words[2]);

                string sFrazione1 = Normalize(words[3]);
                string sFrazione2 = Normalize(words[4]);
                string sFrazione = sFrazione2 == "" ? sFrazione1 : sFrazione1 + " - " + sFrazione2;

                string sToponimo1 = Normalize(words[5]);
                string sToponimo2 = Normalize(words[6]);
                string sToponimo = sToponimo2 == "" ? sToponimo1 : sToponimo1 + " - " + sToponimo2;

                string sDaugt = Normalize(words[7]);
                //sToponimo = sToponimo == "" ? sToponimo : sToponimo + " (" + sDaugt + ")";    // notazione innaturale
                sToponimo = sToponimo == "" ? sToponimo : sDaugt + " " + sToponimo;                   // notazione naturale

                string sCivico = Normalize(words[8]);

                string sCap = Normalize(words[9]);

                // insert CAP in CAP Dictionary if necessary
                if (!dCAPS.ContainsKey(sCap))
                {
                    dCAPS.Add(sCap, new CAP(sCap));
                }

                CAPRecord newRecord = new CAPRecord(sFrazione, sToponimo, sCivico);
                newRecord.Cap = dCAPS[sCap];

                // insert in province Dictionary if not already inserted
                if (!dProvince.ContainsKey(sProvincia))
                {
                    dProvince.Add(sProvincia, new Dictionary<string, Comune>());
                }

                // parse Comune1, Comune2 if necessary
                if (sComune2 == "")
                {
                    sComune1 = sComune1 + " (" + sProvincia + ")";
                }
                else
                {
                    string sTmp = sComune1;
                    sComune1 = sTmp + " - " + sComune2 + " (" + sProvincia + ")";
                    sComune2 = sComune2 + " - " + sTmp + " (" + sProvincia + ")";
                }

                // insert into comuni Dictionary if not already inserted, if comune name is bilungual insert two separate records
                if (!dComuni.ContainsKey(sComune1))
                {
                    if (sComune2 != "")
                    {
                        Comune c2 = new Comune();
                        c2.ComuneID = sComune2;
                        c2.CapRecords = new List<CAPRecord>();
                        dComuni.Add(c2.ComuneID, c2);

                        dProvince[sProvincia].Add(c2.ComuneID, c2);
                    }

                    Comune c1 = new Comune();
                    c1.ComuneID = sComune1;
                    c1.CapRecords = new List<CAPRecord>();
                    dComuni.Add(sComune1, c1);

                    dProvince[sProvincia].Add(c1.ComuneID, c1);
                }

                // insert CAP record into both indexes
                if (sComune2 != "")
                {
                    dComuni[sComune2].CapRecords.Add(newRecord);
                }

                dComuni[sComune1].CapRecords.Add(newRecord);
            }

            // incapsule CAP structures into the class CAPDB
            _capDB = new CAPDB(dProvince, dComuni, dCAPS);
        }

        private string Normalize(string s)
        {
            string sReturn;
            if (s == " '")
            {
                sReturn = "";
            }
            else
            {
                sReturn = s.Remove(0, 2);
                sReturn = sReturn.Remove(sReturn.Length - 1, 1);
            }

            return sReturn;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _state = Step.scegliProvinciaOComune;
            Reset();
        }
}