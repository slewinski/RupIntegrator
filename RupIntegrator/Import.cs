
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.UI;
using System.Threading;
using System.Data.OleDb;
using System.IO;
using System.Globalization;
using Ex2PscdInterface.Ex2PscdDocumentUpdateOutService;
using SapPOHelper;
using Ex2PscdInterface.Ex2PscdDocumentListQueryOutService;
using Ex2PscdInterface.Ex2PscdDocumentDebtStateUpdateOutService;

namespace KnsMigrator
{
    
    
  


    class Imports
    {
        
        public KnsMigratorEntities  Context { get; set; }
        public Konfiguracja Konfig { get; set; }
        private string[] typFakt = {"GS",  // grzywna  saldo
                                     "KS"};   // koszty saldo 
        public DateTime theday { get; set; }
        public DateTime data_od { get; set; }
        public string uwagi { get;  set;}
        public bool breakIndicator {get; set;}
        public string progressMsg{get; set;}
        public string fileName { get; set; }
        public bool newOnly { get; set; }
        public bool errorStatus { get; set; }
        public List<int> KsiegiKnsLst { get; set; }
        public string sprList { get; set; }
        public Transfer updateTransfer { get; set; }
        private Transfer CurrentTransfer = null;
        
        public int typImport { get; set;}

        public int ImportedDocs { get; set; }
        private ProgresForm pForm;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public MiesPackHelper miesPackHlp = new MiesPackHelper();

        private string  cleanNIP(string NIP)
        {
            string s="";
            int i,j;
            if (NIP == null) return "";
            for (i = 0 ;i<NIP.Length;i++)
            {
                if (int.TryParse(NIP[i].ToString(), out j) == true)
                {
                    s += NIP[i];

                }
             }
            if (s.Length != 10 && s.Length > 0)
            {
                return "??????????";

            }
            else
                return s;
        }
        
        /*
        private void progressWindow()
        {
            pForm = new ProgresForm();
            (pForm.Controls["rbStop"] as RadButton).Click += new EventHandler(rbStop_Click);
            pForm.Show();
            while (!breakIndicator)
            {
                
                (pForm.Controls["lbInfo"] as Label).Text = progressMsg;
                (pForm.Controls["lbInfo"] as Label).Refresh();
                                            
            
            }

                  
        }
        */
        /*
        private void rbStop_Click(object sender, EventArgs e)
        {
            DialogResult dialresult = MessageBox.Show("Czy chcesz przerwać przetwarzanie ?", "Uwaga", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialresult == DialogResult.Yes)
            {
                this.breakIndicator = true;
                pForm.Close();
                return;
            }
        }
        */
        private class kwtData
        {

           public decimal? kwota { get; set; }
           public DateTime data { get; set; }
        
        }

        private void UpdateDocRaty(ref Dokument doc, ref List<kwtData> kwtLst)
        {
            if (kwtLst.Count > 0)
            {
                int i = 1;


                for (int j = 0; j < kwtLst.Count ; j++)
                {
                    kwtData kdta = kwtLst.ElementAt(kwtLst.Count - j -1 );
                    if (kdta.kwota > 0)
                    {
                        switch (j + 1)
                        {
                            case 1: doc.RataData1 = kdta.data;
                                doc.RataKwota1 = kdta.kwota;
                                break;
                            case 2: doc.RataData2 = kdta.data;
                                doc.RataKwota2 = kdta.kwota;
                                break;
                            case 3: doc.RataData3 = kdta.data;
                                doc.RataKwota3 = kdta.kwota;
                                break;
                            case 4: doc.RataData4 = kdta.data;
                                doc.RataKwota4 = kdta.kwota;
                                break;
                            case 5: doc.RataData5 = kdta.data;
                                doc.RataKwota5 = kdta.kwota;
                                break;
                            case 6: doc.RataData6 = kdta.data;
                                doc.RataKwota6 = kdta.kwota;
                                break;
                            case 7: doc.RataData7 = kdta.data;
                                doc.RataKwota7 = kdta.kwota;
                                break;
                            case 8: doc.RataData8 = kdta.data;
                                doc.RataKwota8 = kdta.kwota;
                                break;
                            case 9: doc.RataData9 = kdta.data;
                                doc.RataKwota9 = kdta.kwota;
                                break;
                            case 10: doc.RataData10 = kdta.data;
                                doc.RataKwota10 = kdta.kwota;
                                break;
                            case 11: doc.RataData11 = kdta.data;
                                doc.RataKwota11 = kdta.kwota;
                                break;
                            case 12: doc.RataData12 = kdta.data;
                                doc.RataKwota12 = kdta.kwota;
                                break;
                            case 13: doc.RataData13 = kdta.data;
                                doc.RataKwota13 = kdta.kwota;
                                break;
                            case 14: doc.RataData14 = kdta.data;
                                doc.RataKwota14 = kdta.kwota;
                                break;
                            case 15: doc.RataData15 = kdta.data;
                                doc.RataKwota15 = kdta.kwota;
                                break;
                            case 16: doc.RataData16 = kdta.data;
                                doc.RataKwota16 = kdta.kwota;
                                break;
                            case 17: doc.RataData17 = kdta.data;
                                doc.RataKwota17 = kdta.kwota;
                                break;
                            case 18: doc.RataData18 = kdta.data;
                                doc.RataKwota18 = kdta.kwota;
                                break;
                            case 19: doc.RataData19 = kdta.data;
                                doc.RataKwota19 = kdta.kwota;
                                break;
                            case 20: doc.RataData20 = kdta.data;
                                doc.RataKwota20 = kdta.kwota;
                                break;
                            case 21: doc.RataData21 = kdta.data;
                                doc.RataKwota21 = kdta.kwota;
                                break;
                            case 22: doc.RataData22 = kdta.data;
                                doc.RataKwota22 = kdta.kwota;
                                break;
                            case 23: doc.RataData23 = kdta.data;
                                doc.RataKwota23 = kdta.kwota;
                                break;
                            case 24: doc.RataData24 = kdta.data;
                                doc.RataKwota24 = kdta.kwota;
                                break;
                            case 25: doc.RataData25 = kdta.data;
                                doc.RataKwota25 = kdta.kwota;
                                break;
                            case 26: doc.RataData26 = kdta.data;
                                doc.RataKwota26 = kdta.kwota;
                                break;
                            case 27: doc.RataData27 = kdta.data;
                                doc.RataKwota27 = kdta.kwota;
                                break;
                            case 28: doc.RataData28 = kdta.data;
                                doc.RataKwota28 = kdta.kwota;
                                break;
                            case 29: doc.RataData29 = kdta.data;
                                doc.RataKwota29 = kdta.kwota;
                                break;
                            case 30: doc.RataData30 = kdta.data;
                                doc.RataKwota30 = kdta.kwota;
                                break;
                            case 31: doc.RataData31 = kdta.data;
                                doc.RataKwota31 = kdta.kwota;
                                break;
                            case 32: doc.RataData32 = kdta.data;
                                doc.RataKwota32 = kdta.kwota;
                                break;
                            case 33: doc.RataData33 = kdta.data;
                                doc.RataKwota33 = kdta.kwota;
                                break;
                            case 34: doc.RataData34 = kdta.data;
                                doc.RataKwota34 = kdta.kwota;
                                break;
                            case 35: doc.RataData35 = kdta.data;
                                doc.RataKwota35 = kdta.kwota;
                                break;
                            case 36: doc.RataData36 = kdta.data;
                                doc.RataKwota36 = kdta.kwota;
                                break;

                            default:
                                break;
                        }
                        i++;
                    }

                    
                } // foreach
                kwtLst.Clear();
            }
        }

        private string kodFormat(string kraj, string KodPocztowy)
        {
            string result = null; 
            kraj = kraj.ToUpper();
            if (kraj == "PL") return KodPocztowy;
            if (string.IsNullOrWhiteSpace(kraj)) return KodPocztowy;
            List<KodMaskKonfig> kkonf = Context.KodMaskKonfig.Where(a => a.Kraj == kraj).ToList();
            if (kkonf == null ) return KodPocztowy;
            KodPocztowy = KodPocztowy.Replace("-", "");
            foreach (KodMaskKonfig kk in kkonf)
            {
                result = formatKod(kraj, KodPocztowy, kk.Maska);
                if (!String.IsNullOrWhiteSpace(result)) return result;
            }
            return result;
        }

        private string formatKod(string kraj , string KodPocztowy, string maska)
        {// kraj - kod kraju 
            kraj = kraj.ToUpper();
            string maskChar;
            string kodOut = string.Empty;
        

           
           
            int j = 0; 
            for (int i = 0; i < maska.Length; i++)
            {
                maskChar = maska.Substring(i, 1);
                switch (maskChar)
                {
                    case "C":
                        if (Char.IsLetter(KodPocztowy, j))
                        {
                            kodOut += KodPocztowy.ToUpper().Substring( j,1);
                            j++;
                        }
                        else
                            return null;
                        break;    
                    case "X":
                        if (Char.IsLetter(KodPocztowy, j) || Char.IsDigit(KodPocztowy, j))
                        {
                            kodOut += KodPocztowy.ToUpper().Substring(j, 1);
                            j++;

                        }
                        else
                            return null;
                        break;

                    case "D":
                        if (Char.IsDigit(KodPocztowy, j))
                        {
                            kodOut += KodPocztowy.ToUpper().Substring(j, 1);
                            j++;

                        }
                        else
                            return null;
                        break;
                    default:

                        kodOut += maska.Substring(i, 1);
                        if (!Char.IsDigit(KodPocztowy,j) && KodPocztowy.Substring(j,1) == maskChar)
                            j++;
                        break;


                }


            }
            return kodOut;

        }

        public void CreateSchema()
        {
            FileInfo file = new FileInfo(fileName);
            try
            {
                using (System.IO.StreamWriter schema = new System.IO.StreamWriter(file.DirectoryName + "\\schema.ini"))
                {
                    schema.WriteLine("[" + file.Name + "]");
                    schema.WriteLine("Format=Delimited(;)");
                    schema.WriteLine("ColNameHeader=True");
                    schema.WriteLine("MaxScanRows=0");
                    schema.WriteLine("CharacterSet=1250");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu zbioru schema " + ex.Message);
            }
        
        
        }
        public bool ImportConfirmationZPSCDDOKS(int mode, int TransferId, bool force = false)
        {
            // import potwierdzeń
            int i = 0;
            string guid;
            FileInfo file = new FileInfo(fileName);
            //MessageBox.Show("Funkcja w przygotowaniu");
            //return false; 
            /*
             Numer dokumentu
            Oznaczenie konta umowy
            Nazwisko / Nazwa 1
            Imię / Nazwa 2
            Data księgowania
            Data dokumentu
            Kwota transakcji
            Operacja główna
            Operacja częściowa
            Oznaczenie przedmiotu umowy
            Typ konta umowy
            Pozycja finansowa
            Rodzaj dokumentu
            Partner biznesowy
            Konto umowy
            Przedmiot umowy
            Dokument rozliczenia
            Konto Księgi Głównej
             */
            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName + "\";Extended Properties='text;HDR=Yes';"))
                //using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName ))
                {
                    using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", file.Name), con))
                    {
                        con.Open();

                        // Using a DataReader to process the data
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            List<Dokument>  dList = Context.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.Transfer_Id == TransferId).ToList();

                            Cursor.Current = Cursors.WaitCursor;
                            i = 0;
                            while (reader.Read())
                            {

                                 string kartadl = reader["Oznaczenie konta umowy"].ToString();
                                 DateTime dksiegowania = Convert.ToDateTime(reader["Data księgowania"]);
                                 dksiegowania = dksiegowania.Date;
                                 Decimal kwota = reader["Kwota transakcji"] as decimal? ?? default(decimal);
                                if (kwota == 0)
                                {
                                    String kwt = reader["Kwota transakcji"].ToString();
                                    if (Decimal.TryParse(kwt.Replace(',','.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite, CultureInfo.InvariantCulture, out kwota)) ;
                                 }
                                 string  opGlowna = reader["Operacja główna"].ToString(); 
                                 string  opczesc = reader["Operacja częściowa"].ToString();
                                 string sygnatura = reader["Oznaczenie przedmiotu umowy"].ToString();

                                 List<Dokument> dokL = dList.Where(a => a.Sprawa != null && a.Sprawa.Karta == kartadl).ToList();
                                 if (dokL == null) continue;   // nie znaleziono 
                                 foreach (Dokument d in dokL)
                                 {
                                     if (Math.Abs(Convert.ToDecimal(d.kwota)) == Math.Abs(kwota) && dksiegowania == Convert.ToDateTime(d.DataKsiegowania).Date && opGlowna == d.OperacjaGlowna && opczesc == d.OperacjaCzesciowa)
                                     {

                                         if (String.IsNullOrEmpty(d.SAPDocId) || force)
                                         {
                                             d.SAPDocId = reader["Numer dokumentu"].ToString();
                                             if (d.SAPDocId.Length < 12) d.SAPDocId = new String('0', 12 - d.SAPDocId.Length) + d.SAPDocId;
                                         }

                                         if (String.IsNullOrEmpty(d.Sprawa.SAPKontoUmowy) || force)
                                         {
                                             d.Sprawa.SAPKontoUmowy = reader["Konto umowy"].ToString();
                                             if (d.Sprawa.SAPKontoUmowy.Length < 12) d.Sprawa.SAPKontoUmowy = new String('0', 12 - d.Sprawa.SAPKontoUmowy.Length) + d.Sprawa.SAPKontoUmowy;
                                         }
                                         if (String.IsNullOrEmpty(d.Sprawa.SAPPrzedmiotUmowy) || force)
                                         {
                                             d.Sprawa.SAPPrzedmiotUmowy = reader["Przedmiot umowy"].ToString();
                                             if (d.Sprawa.SAPPrzedmiotUmowy.Length < 20) d.Sprawa.SAPPrzedmiotUmowy = new String('0', 20 - d.Sprawa.SAPPrzedmiotUmowy.Length) + d.Sprawa.SAPPrzedmiotUmowy; 
                                         }
                                         if (String.IsNullOrEmpty(d.Dluznik.SAPKontoPartnera) || force)
                                         {
                                             d.Dluznik.SAPKontoPartnera = reader["Partner biznesowy"].ToString();
                                             if (d.Dluznik.SAPKontoPartnera.Length < 10) d.Dluznik.SAPKontoPartnera = new String('0', 10 - d.Dluznik.SAPKontoPartnera.Length) + d.Dluznik.SAPKontoPartnera;
                                         }
                                         d.SAPImportInfo = "Zaimportowano z ZPSCDDOKS";
                                         d.SAPImportStatus = 1;
                                         i++;
                                     }

                                 }
                            }
                            Cursor.Current = Cursors.Default;
                            reader.Close();

                        }
                        con.Close();
                        Context.SaveChanges();
                        if (mode == 0 )
                            MessageBox.Show("Zaimportowano  " + i.ToString() + " potwierdzeń");
                        else
                            Utils.LogWriter("Zaimportowano  " + i.ToString() + " potwierdzeń");
                          
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (mode == 0)
                    MessageBox.Show("Błąd wczytywania zbioru potwierdzeń " + ex.Message);
                else
                    Utils.LogWriter("Błąd wczytywania zbioru potwierdzeń " + ex.Message);
                return false;
            }
        }

        public bool ImportConfirmationAll()
        {
            // import potwierdzeń
            int i = 0;
            string guid;
            FileInfo file = new FileInfo(fileName);
            //MessageBox.Show("Funkcja w przygotowaniu");
            //return false; 
            /*
             Numer dokumentu
            Oznaczenie konta umowy
            Nazwisko / Nazwa 1
            Imię / Nazwa 2
            Data księgowania
            Data dokumentu
            Kwota transakcji
            Operacja główna
            Operacja częściowa
            Oznaczenie przedmiotu umowy
            Typ konta umowy
            Pozycja finansowa
            Rodzaj dokumentu
            Partner biznesowy
            Konto umowy
            Przedmiot umowy
            Dokument rozliczenia
            Konto Księgi Głównej
             */
            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName + "\";Extended Properties='text;HDR=Yes';"))
                //using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName ))
                {
                    using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", file.Name), con))
                    {
                        con.Open();

                        // Using a DataReader to process the data
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            List<Dokument> dList = Context.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.typFakt == "KS" ||  a.typFakt == "KP" ).ToList();

                            Cursor.Current = Cursors.WaitCursor;
                            i = 0;
                            while (reader.Read())
                            {

                                string kartadl = reader["Oznaczenie konta umowy"].ToString();
                                DateTime dksiegowania = Convert.ToDateTime(reader["Data księgowania"]);
                                dksiegowania = dksiegowania.Date;
                                Decimal kwota = reader["Kwota transakcji"] as decimal? ?? default(decimal);
                                if (kwota == 0)
                                {
                                    String kwt = reader["Kwota transakcji"].ToString();
                                    if (Decimal.TryParse(kwt.Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite, CultureInfo.InvariantCulture, out kwota)) ;
                                }
                                string opGlowna = reader["Operacja główna"].ToString();
                                string opczesc = reader["Operacja częściowa"].ToString();
                                string sygnatura = reader["Oznaczenie przedmiotu umowy"].ToString();
                                string kontoUmowy = reader["Konto umowy"].ToString();

                                List<Dokument> dokL = dList.Where(a => a.Sprawa != null && a.Sprawa.SAPKontoUmowy == kontoUmowy).ToList();
                                if (dokL == null) continue;   // nie znaleziono 
                                if (dokL.Count != 1) continue;
                                foreach (Dokument d in dokL)
                                {

                                            d.SAPDocId = reader["Numer dokumentu"].ToString();
                                            if (d.SAPDocId.Length < 12) d.SAPDocId = new String('0', 12 - d.SAPDocId.Length) + d.SAPDocId;
                                   

                                }
                            }
                            Cursor.Current = Cursors.Default;
                            reader.Close();

                        }
                        con.Close();
                        Context.SaveChanges();
                      

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                 Utils.LogWriter("Błąd wczytywania zbioru potwierdzeń " + ex.Message);
                return false;
            }
        }


        private IEnumerable<string[]> LoadCsvData(string path, params char[] separator)
        {
            return from line in File.ReadLines(path)
                   let parts = (from p in line.Split(separator, StringSplitOptions.RemoveEmptyEntries) select p)
                   select parts.ToArray();
        }

        public List<string>  ImportConfirmationKoszty(string filename)
        {
            // import potwierdzeń
            int i = 0;
            string guid;
            FileInfo file = new FileInfo(fileName);
            List<string> blackLst = new List<string>();
            //MessageBox.Show("Funkcja w przygotowaniu");
            //return false; 
            /*
             Numer dokumentu
            Oznaczenie konta umowy
            Nazwisko / Nazwa 1
            Imię / Nazwa 2
            Data księgowania
            Data dokumentu
            Kwota transakcji
            Operacja główna
            Operacja częściowa
            Oznaczenie przedmiotu umowy
            Typ konta umowy
            Pozycja finansowa
            Rodzaj dokumentu
            Partner biznesowy
            Konto umowy
            Przedmiot umowy
            Dokument rozliczenia
            Konto Księgi Głównej
             */
            try
            {

                 using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName + "\";Extended Properties='text;HDR=Yes';"))
                //using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName ))
                {
                    using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", file.Name), con))
                    {
                        con.Open();

                        // Using a DataReader to process the data
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            i = 0 ;
                            Cursor.Current = Cursors.WaitCursor;
                            while (reader.Read())
                            {
                                if (reader["Numer dokumentu"] == DBNull.Value) break;
                                i++;
                                string nrDok = reader["Numer dokumentu"].ToString().Trim();
                                string targetDoc = reader["Nowy numer dokumentu"].ToString().Trim();
                                if (nrDok.Length == 11)
                                    nrDok = "0" + nrDok;
                                if (targetDoc.Length == 11)
                                    targetDoc = "0" + targetDoc;
                                List<Dokument> dlst = Context.Dokument.Where(a => a.SAPDocId == nrDok && (a.typFakt=="KS" || a.typFakt=="KP" )).ToList();
                                if (dlst == null || dlst.Count == 0 )
                                    blackLst.Add(nrDok);
                                else
                                {
                                    foreach (Dokument d in dlst)
                                    {
                                        d.Info = nrDok;
                                        d.SAPDocId = targetDoc;
                                    
                                    }
                                
                                }

                            }
                            Cursor.Current = Cursors.Default;
                            reader.Close();

                        }
                        con.Close();
                        Context.SaveChanges();

                        MessageBox.Show("Ptrzetworzono  " + i.ToString() + " pozycji " + (blackLst.Count> 0 ? " Nie odnaleziono  " + blackLst.Count.ToString() + " dokumentów":"" ) );
                        
                        return blackLst;
                    }
                }
            }
            catch (Exception ex)
            {
             
                MessageBox.Show("Błąd wczytywania raportu  " + ex.Message);
               
                return null;
            }
        }


        public List<string> ImportConfirmationFPP(string filename)
        {
            // import potwierdzeń
            int i = 0;
            string guid;
            FileInfo file = new FileInfo(fileName);
            List<string> blackLst = new List<string>();
            List<string> blackSprLst = new List<string>();
            //MessageBox.Show("Funkcja w przygotowaniu");
            //return false; 
            /*
             Numer dokumentu
            Oznaczenie konta umowy
            Nazwisko / Nazwa 1
            Imię / Nazwa 2
            Data księgowania
            Data dokumentu
            Kwota transakcji
            Operacja główna
            Operacja częściowa
            Oznaczenie przedmiotu umowy
            Typ konta umowy
            Pozycja finansowa
            Rodzaj dokumentu
            Partner biznesowy
            Konto umowy
            Przedmiot umowy
            Dokument rozliczenia
            Konto Księgi Głównej
             */
            try
            {

                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName + "\";Extended Properties='text;HDR=Yes';"))
                //using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName ))
                {
                    using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", file.Name), con))
                    {
                        con.Open();

                        // Using a DataReader to process the data
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            i = 0;
                            Cursor.Current = Cursors.WaitCursor;
                            string nrDok;
                            string nrKU;
                            while (reader.Read())
                            {
                                if (reader["Numer dokumentu"] == DBNull.Value) break;
                                    i++;
                                    nrDok = reader["Numer dokumentu"].ToString().Trim();
                                    string targetDoc = reader["Nowy numer dokumentu"].ToString().Trim();
                                    if (nrDok.Length == 11)
                                        nrDok = "0" + nrDok;
                                    if (targetDoc.Length == 11)
                                        targetDoc = "0" + targetDoc;
                                    List<Dokument> dlst = Context.Dokument.Where(a => a.SAPDocId == nrDok).ToList();
                                    if (dlst == null || dlst.Count == 0)
                                        blackLst.Add(nrDok);
                                    else
                                    {
                                        foreach (Dokument d in dlst)
                                        {
                                            d.SAPIdDocOld = nrDok;
                                            d.SAPDocId = targetDoc;

                                        }

                                    }
                               
                                {



                                }

                                if (reader["Konto umowy"] == DBNull.Value) break;
                                i++;
                                nrKU = reader["Konto umowy"].ToString().Trim();
                                string targetKU = reader["KU nowy"].ToString().Trim();
                                
                                List<Sprawa> slst = Context.Sprawa.Where(a => a.SAPKontoUmowy == nrKU).ToList();
                                if (slst == null || slst.Count == 0)
                                    blackSprLst.Add(nrKU);
                                else
                                {
                                    foreach (Sprawa s in slst)
                                    {
                                        s.SapKontoUmowyOld = nrKU;
                                        s.SAPKontoUmowy = targetKU;

                                    }

                                }


                            }
                            Cursor.Current = Cursors.Default;
                            reader.Close();

                        }
                        con.Close();
                        Context.SaveChanges();

                        MessageBox.Show("Ptrzetworzono  " + i.ToString() + " pozycji wierszy "  + (blackLst.Count > 0 ? " Nie odnaleziono  " + blackLst.Count.ToString() + " dokumentów" : "") + (blackSprLst.Count > 0 ? " Nie odnaleziono  " + blackSprLst.Count.ToString() + " kont umów" : ""));

                        blackLst.AddRange(blackSprLst);
                        return blackLst;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd wczytywania raportu  " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message:""));
                return null;
            }
        }






        public bool ImportConfirmation(int mode)
        {
            // import potwierdzeń
            int i = 0;
            string guid;
            FileInfo file = new FileInfo(fileName);
            try {
            using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName + "\";Extended Properties='text;HDR=Yes';"))
            //using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName ))
            {
                using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", file.Name), con))
                {
                    con.Open();

                    // Using a DataReader to process the data
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {

                        Cursor.Current = Cursors.WaitCursor;
                        while (reader.Read())
                        {
                            

                                   guid = reader["Dokument Id"].ToString();
                            string partner = reader["Numer partnera handlowego"].ToString();
                            string kontoumowy = reader["Numer konta umowy"].ToString();
                            string przedmiotumowy = reader["Sygnatura sądowa"].ToString();
                            string dokumentId = reader["Numer dokumentu rozrachunków"].ToString();
                            string dokRef = reader["Numer Dokumentu Referencyjnego"].ToString();
                            string dokRat = reader["Numer dokumentu plan rat"].ToString();
                            string message = reader["DIAGNOSTYKA"].ToString();
                            string typOperacji = reader["Kod operacji"].ToString();
                            string[] importStatus=null;

                            if (message != null )
                            {
                              importStatus  = message.Split(new char[] {',',',',',',',',',',','});

                            }
                           
                            Guid gu  = new Guid ();
                            if (!Guid.TryParse(guid, out gu))
                            {
                                MessageBox.Show("Błąd odczytu guid " + guid + "  PU = " + przedmiotumowy);
                                continue;
                            }
                            else
                            {
                                Dokument mydoc = this.Context.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.DocGuid == gu).FirstOrDefault();
                                if (mydoc != null)
                                {
                                    if (!String.IsNullOrEmpty(dokRat.Trim()))
                                        mydoc.SAPRatyId = dokRat.Trim();
                                    else
                                        mydoc.SAPRatyId = null;

                                    if (typOperacji != "GR" && typOperacji != "KR") // raty - tulko update dokumentu rat
                                    {
                                        if (!String.IsNullOrEmpty(dokumentId.Trim()))
                                            mydoc.SAPDocId = dokumentId.Trim();
                                        else
                                            mydoc.SAPDocId = null;

                                        if (!String.IsNullOrEmpty(kontoumowy.Trim()))
                                            mydoc.Sprawa.SAPKontoUmowy = kontoumowy.Trim();
                                        else
                                            mydoc.Sprawa.SAPKontoUmowy = null;

                                        if (!String.IsNullOrEmpty(przedmiotumowy.Trim()))
                                            mydoc.Sprawa.SAPPrzedmiotUmowy = przedmiotumowy.Trim();
                                        else
                                            mydoc.Sprawa.SAPPrzedmiotUmowy = null;


                                        if (!String.IsNullOrEmpty(partner.Trim()))
                                            mydoc.Dluznik.SAPKontoPartnera = partner.Trim();
                                        else
                                            mydoc.Dluznik.SAPKontoPartnera = null;
                                    }
                                    mydoc.SAPImportDate = DateTime.Now;
                                    if (!String.IsNullOrEmpty(message))
                                    {
                                        if ((importStatus[0].Length > 1) ||
                                            (importStatus[1].Length > 1) ||
                                            (importStatus[2].Length > 1) ||
                                            (importStatus[3].Length > 1))
                                        {
                                            if (mydoc.SAPImportStatus > 0)
                                                mydoc.SAPImportStatus = -1;
                                            else
                                                mydoc.SAPImportStatus -= 1;
                                        }
                                        else
                                        {
                                            if (mydoc.SAPImportStatus < 0)
                                                mydoc.SAPImportStatus = 1;
                                            else
                                                mydoc.SAPImportStatus += 1;
                                        }
                                    }
                                    mydoc.SAPImportInfo = message;


                                }
                                i++;
                            }
                        }
                        Cursor.Current = Cursors.Default;
                        if (mode == 0)
                            MessageBox.Show("Zaimpotrowano  " + i.ToString() + " potwierdzeń");
                        else
                            Utils.LogWriter("Zaimpotrowano  " + i.ToString() + " potwierdzeń");
                        reader.Close();

                    }
                    con.Close();
                    Context.SaveChanges();
                    return true;
                }
            }
            }
            catch (Exception ex)
            {
                if (mode == 0)
                    MessageBox.Show("Błąd wczytywania zbioru potwierdzeń " + ex.Message);
                else
                    Utils.LogWriter("Błąd wyczytywania zbioru potwierdzeń " + ex.Message);
                return false;
            }
        }

        public void ImportWplaty()
        {
            // import potwierdzeń
            string blad;
            FileInfo file = new FileInfo(fileName);
            string[] headers;
            Transfer trans = null;
            decimal kwt;
            string[] dateFormats = {"yyyyMMdd"};
            try
            {

                
                {
                

                    using (StreamReader r = new StreamReader(fileName))
                    {
                        // 3
                        // Use while != null pattern for loop
                        string line;
                        if ((line = r.ReadLine()) != null)
                        {


                            headers = line.Split(new char[] { ';', ';', ';', ';', ';', ';' });

                            DateTime d_od, d_do;
                            blad = Utils.ParseFilterValue(headers[3], out d_od, out d_do);
                            if (blad.Length > 0)
                            {

                                return;
                            }
                            trans = new Transfer();
                            trans.rodzaj = 4; // wpłaty
                            trans.DataOd = d_od;
                            trans.DataDo = d_do;
                            trans.DataTransferu = DateTime.Now;
                            trans.Uwagi = "Wpłaty " + headers[0] + ";" + headers[1] + ";" + headers[2] + ";" + headers[3];
                            this.Context.Transfer.AddObject(trans);



                        }
                    }

                    string connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName + "\";Extended Properties=\"Text;HDR=No\";");
                    string cmdString = string.Format("SELECT * FROM [{0}]", file.Name);



                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmdString, connString);
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);
                    blad = "";
                    int rowNumber = 1;
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        if (rowNumber == 0)
                        {

                            string dokPrzypis = row[0].ToString();
                            string dokRozliczWyciag = row[1].ToString();
                            string rodzajDokRozlicz = row[2].ToString();
                            string dataRozlicz = row[3].ToString();

                            DateTime d_od, d_do;
                            blad = Utils.ParseFilterValue(dataRozlicz, out d_od, out d_do);
                            if (blad.Length > 0)
                            {

                                return;
                            }
                            trans = new Transfer();
                            trans.rodzaj = 4; // odpisy
                            trans.DataOd = d_od;
                            trans.DataDo = d_do;
                            trans.Uwagi = "Wpłaty " + dokPrzypis + ";" + dokRozliczWyciag + ";" + rodzajDokRozlicz + ";" + dataRozlicz;
                            trans.DataTransferu = DateTime.Now;
                            this.Context.Transfer.AddObject(trans);

                        }
                        else
                        {
                            string dokPrzypis = row[0].ToString();
                            string dokRozliczWyciag = row[1].ToString();
                            string rodzajDokRozlicz = row[2].ToString();
                            string dataRozlicz = row[3].ToString();
                            string dataKsiegowania = row[4].ToString();
                            string dokOdpisu = row[5].ToString();
                            string kwota = row[6].ToString();


                            Wplata wpl = new Wplata();
                            wpl.SAPDocPRef = dokPrzypis;
                            wpl.SAPDokRozliczany = dokOdpisu;
                            wpl.SAPDokRozliczeniowy = dokRozliczWyciag;
                            wpl.SAPRodzajDok = rodzajDokRozlicz;
                            if (Decimal.TryParse(kwota, NumberStyles.AllowDecimalPoint|NumberStyles.AllowLeadingSign|NumberStyles.AllowLeadingWhite, CultureInfo.InvariantCulture, out kwt))
                                wpl.Kwota = kwt;
                            else
                            {

                                MessageBox.Show("Błąd wyczytywania zbioru wpłat - błąd konwersji kwoty" + kwota);
                                return;
                            }
                            DateTime tmpDt;

                            if (DateTime.TryParseExact(dataRozlicz, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDt))
                            {
                                wpl.DataRozlicz = tmpDt;
                            }
                            else
                                wpl.DataRozlicz = null;

                            if (DateTime.TryParseExact(dataKsiegowania, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDt))
                            {
                                wpl.DataWplaty = tmpDt;
                            }
                            else
                                wpl.DataWplaty = null;
                            //wpl.DataWplaty = new DateTime(Convert.ToInt32(dataKsiegowania.Substring(0, 4)), Convert.ToInt32(dataKsiegowania.Substring(4, 2)), Convert.ToInt32(dataKsiegowania.Substring(6, 2)));
                            Dokument mydoc = this.Context.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.SAPDocId == dokPrzypis).FirstOrDefault();
                            if (mydoc != null)
                            {
                                // rozliczenie 
                                wpl.tytulem = "Karta dł :" + mydoc.Sprawa.Karta + "sygnatura sprawy " + mydoc.Sprawa.Sygnatura + " Dłużnik " + mydoc.Dluznik.Imie + " " + mydoc.Dluznik.Nazwisko;


                            }
                            
                            trans.Wplata.Add(wpl);
                        }
                   
                        rowNumber++;

                    }
                    trans.LFaktow = rowNumber;
                    this.Context.SaveChanges();

                }
            }
              catch (Exception ex)
            {
                

  
                  MessageBox.Show("Błąd wyczytywania zbioru wpłat " + ex.Message);
                

            }
      
    }
                        // Using a DataReader to process the data
                       
/*
Nr. dokumentu przypisu
Nr. dokumentu rozliczeniowego/wyciąg bankowy
Rodzaj dokumentu rozliczeniowego
Data rozliczenia
Nr. dokumentu odpisu
Kwota
*/
       /*                     int rowNumber = 1; 
                            while (dataSet.Tables[0])
                            {
                                
                                //string dataKsiegowania = reader[6].ToString();

                                if (rowNumber == 0)
                                {
                                    
                                    string dokPrzypis = reader[0].ToString();
                                    string dokRozliczWyciag = reader[1].ToString();
                                    string rodzajDokRozlicz = reader[2].ToString();
                                    string dataRozlicz = reader[3].ToString();
                                   
                                    DateTime d_od, d_do;
                                    blad = Utils.ParseFilterValue(dataRozlicz, out d_od, out d_do);
                                    if (blad.Length > 0) 
                                    {
                                        reader.Close();
                                        return;
                                    }
                                    Transfer trans = new Transfer();
                                    trans.rodzaj = 4; // odpisy
                                    trans.DataOd = d_od;
                                    trans.DataDo = d_do;
                                    trans.Uwagi = "Wpłaty " + dokPrzypis + ";" + dokRozliczWyciag + ";" + rodzajDokRozlicz + ";" + dataRozlicz;
                                    this.Context.Transfer.AddObject(trans);
                                     
                                }
                                else
                                {
                                    string dokPrzypis = reader[0].ToString();
                                    string dokRozliczWyciag = reader[1].ToString();
                                    string rodzajDokRozlicz = reader[2].ToString();
                                    string dataRozlicz = reader[3].ToString();
                                    string dokOdpisu = reader[4].ToString();
                                    string kwota = reader[5].ToString();

                                    Dokument mydoc = this.Context.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.SAPDocId == dokPrzypis).FirstOrDefault();
                                    Wplata wpl = new Wplata();
                                    wpl.SAPDokRozliczany = dokPrzypis;
                                    wpl.SAPDokRozliczeniowy = dokRozliczWyciag;
                                    wpl.SAPRodzajDok = rodzajDokRozlicz;
                                    wpl.DataRozlicz = new DateTime(Convert.ToInt32(dataRozlicz.Substring(0, 4)), Convert.ToInt32(dataRozlicz.Substring(4, 2)), Convert.ToInt32(dataRozlicz.Substring(6, 2)));
                                    //wpl.DataWplaty = new DateTime(Convert.ToInt32(dataKsiegowania.Substring(0, 4)), Convert.ToInt32(dataKsiegowania.Substring(4, 2)), Convert.ToInt32(dataKsiegowania.Substring(6, 2)));
                                    
                                    if (mydoc != null)
                                    {
                                        // rozliczenie 
                                        wpl.tytulem = "Karta dł :" + mydoc.Sprawa.Karta + "sygnatura sprawy " + mydoc.Sprawa.Sygnatura + " Dłużnik " + mydoc.Dluznik.Imie + " " + mydoc.Dluznik.Nazwisko;     


                                    }
                                    rowNumber++;
                                }
                            }
                           

                        }
                        con.Close();
                        Context.SaveChanges();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wyczytywania zbioru potwierdzeń " + ex.Message);

            }
        }
*/
        public void ImportRatyHarmonogram()
        { 
        // import Rat 
            SqlConnection con = null;
            SqlDataReader rdr_rh = null;
            SqlDataReader rdr_r = null;
            bool rStatus = false;
            List<kwtData> kwtLst = new List<kwtData>();  
             
             decimal pozostaloscRaty = 0;
             string currentSpr = "";   
             int SprawaId;
            try
            {

                if (CurrentTransfer == null) return;
                // Open connection to the database
                //string ConnectionString = Utils.BuildKnsConnectionString(Konfig);

                string ConnectionString = Utils.BuildMyConnectionString(Context);
                con = new SqlConnection(ConnectionString);
                con.Open();
                string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                SqlCommand storedProcCommand = new SqlCommand("sp_Raty_harmonogram", con);
                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" +  jg : ""));
                //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                storedProcCommand.Parameters.Add("@dzien", theday);
                storedProcCommand.Connection = con;
                storedProcCommand.CommandTimeout = 600;
                progressMsg = "Odczyt harmonogramów rat...";
                rdr_rh = storedProcCommand.ExecuteReader();
                /*
                SqlCommand storedProcRCommand = new SqlCommand("sp_Raty", con);
                storedProcRCommand.CommandType = CommandType.StoredProcedure;
                storedProcRCommand.Parameters.Add("@dzien", theday);
                storedProcRCommand.Connection = con;
                storedProcRCommand.CommandTimeout = 180;
                progressMsg = "Odczyt rat...";
                rdr_r = storedProcRCommand.ExecuteReader();
                */

                if (rdr_rh.HasRows)
                {
                    SprawaId = 0;
                    foreach (Dokument doc in CurrentTransfer.Dokument.Where(c => c.Stan == "B").OrderBy(a => a.Sprawa_Id).ThenBy(b => b.typFakt))
                    {
                        decimal? kwt = doc.kwota;
                        currentSpr = doc.Sprawa.Karta + " Id = " + doc.Sprawa.KnsSprawa_id.ToString() + " Typ operacji:" + doc.typFakt;
                        rStatus = true;
                        if (doc.Sprawa.KnsSprawa_id == 55497)
                        {
                            ;
                        }
                        if (doc.typFakt == "GS" && doc.Stan == "B")     // saldo grzywny
                        {
                            if (pozostaloscRaty > 0 && SprawaId == doc.Sprawa.KnsSprawa_id)
                            {
                                kwtData kwx = new kwtData();
                                kwx.data = Convert.ToDateTime(rdr_rh["Data_Raty"]);
                                kwx.kwota = pozostaloscRaty;
                                kwtLst.Add(kwx);
                                kwt -= pozostaloscRaty;
                                rStatus = rdr_rh.Read();
                                SprawaId = Convert.ToInt32(rdr_rh["Sprawa_Id"]);
                                pozostaloscRaty = 0;
                            }
                            else
                                pozostaloscRaty = 0;


                            while (rStatus && SprawaId < doc.Sprawa.KnsSprawa_id)
                            {
                                rStatus = rdr_rh.Read();
                                if (!rStatus) break;
                                SprawaId = Convert.ToInt32(rdr_rh["Sprawa_Id"]);
                            }
                            if (rStatus && SprawaId == doc.Sprawa.KnsSprawa_id)
                            {// zdejmujemy od odtsniej raty - najpierw grzywna


                                while (SprawaId == doc.Sprawa.KnsSprawa_id)
                                {
                                    decimal? rata = Convert.ToDecimal(rdr_rh["Kwota_Raty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")); 
                                    kwt = kwt - rata;

                                    kwtData kdt = new kwtData();
                                    kdt.data = Convert.ToDateTime(rdr_rh["Data_Raty"]);
                                    if (kwt < 0)
                                        kdt.kwota = rata + kwt;
                                    else
                                        kdt.kwota = rata;
                                    kwtLst.Add( kdt);
                                    if (kwt <= 0)
                                    {
                                        pozostaloscRaty = Math.Abs(Convert.ToDecimal(kwt));
                                        break;
                                    }
                                    if (!rdr_rh.Read()) break;
                                    SprawaId = Convert.ToInt32(rdr_rh["Sprawa_Id"]);
                                }

                                #region updtlst
                                if (kwtLst.Count > 0)
                                {
                                    int i = 1;
                                    
                                    for (int j = kwtLst.Count - 1 ; j >= 0; j--)
                                    {
                                        kwtData kdta = kwtLst.ElementAt(j);
                                        if (kdta.kwota > 0)
                                        {
                                            switch (i)
                                            {
                                                case 1: doc.RataData1 = kdta.data;
                                                    doc.RataKwota1 = kdta.kwota;
                                                    break;
                                                case 2: doc.RataData2 = kdta.data;
                                                    doc.RataKwota2 = kdta.kwota;
                                                    break;
                                                case 3: doc.RataData3 = kdta.data;
                                                    doc.RataKwota3 = kdta.kwota;
                                                    break;
                                                case 4: doc.RataData4 = kdta.data;
                                                    doc.RataKwota4 = kdta.kwota;
                                                    break;
                                                case 5: doc.RataData5 = kdta.data;
                                                    doc.RataKwota5 = kdta.kwota;
                                                    break;
                                                case 6: doc.RataData6 = kdta.data;
                                                    doc.RataKwota6 = kdta.kwota;
                                                    break;
                                                case 7: doc.RataData7 = kdta.data;
                                                    doc.RataKwota7 = kdta.kwota;
                                                    break;
                                                case 8: doc.RataData8 = kdta.data;
                                                    doc.RataKwota8 = kdta.kwota;
                                                    break;
                                                case 9: doc.RataData9 = kdta.data;
                                                    doc.RataKwota9 = kdta.kwota;
                                                    break;
                                                case 10: doc.RataData10 = kdta.data;
                                                    doc.RataKwota10 = kdta.kwota;
                                                    break;
                                                case 11: doc.RataData11 = kdta.data;
                                                    doc.RataKwota11 = kdta.kwota;
                                                    break;
                                                case 12: doc.RataData12 = kdta.data;
                                                    doc.RataKwota12 = kdta.kwota;
                                                    break;
                                                case 13: doc.RataData13 = kdta.data;
                                                    doc.RataKwota13 = kdta.kwota;
                                                    break;
                                                case 14: doc.RataData14 = kdta.data;
                                                    doc.RataKwota14 = kdta.kwota;
                                                    break;
                                                case 15: doc.RataData15 = kdta.data;
                                                    doc.RataKwota15 = kdta.kwota;
                                                    break;
                                                case 16: doc.RataData16 = kdta.data;
                                                    doc.RataKwota16 = kdta.kwota;
                                                    break;
                                                case 17: doc.RataData17 = kdta.data;
                                                    doc.RataKwota17 = kdta.kwota;
                                                    break;
                                                case 18: doc.RataData18 = kdta.data;
                                                    doc.RataKwota18 = kdta.kwota;
                                                    break;
                                                case 19: doc.RataData19 = kdta.data;
                                                    doc.RataKwota19 = kdta.kwota;
                                                    break;
                                                case 20: doc.RataData20 = kdta.data;
                                                    doc.RataKwota20 = kdta.kwota;
                                                    break;
                                                case 21: doc.RataData21 = kdta.data;
                                                    doc.RataKwota21 = kdta.kwota;
                                                    break;
                                                case 22: doc.RataData22 = kdta.data;
                                                    doc.RataKwota22 = kdta.kwota;
                                                    break;
                                                case 23: doc.RataData23 = kdta.data;
                                                    doc.RataKwota23 = kdta.kwota;
                                                    break;
                                                case 24: doc.RataData24 = kdta.data;
                                                    doc.RataKwota24 = kdta.kwota;
                                                    break;
                                                case 25: doc.RataData25 = kdta.data;
                                                    doc.RataKwota25 = kdta.kwota;
                                                    break;
                                                case 26: doc.RataData26 = kdta.data;
                                                    doc.RataKwota26 = kdta.kwota;
                                                    break;
                                                case 27: doc.RataData27 = kdta.data;
                                                    doc.RataKwota27 = kdta.kwota;
                                                    break;
                                                case 28: doc.RataData28 = kdta.data;
                                                    doc.RataKwota28 = kdta.kwota;
                                                    break;
                                                case 29: doc.RataData29 = kdta.data;
                                                    doc.RataKwota29 = kdta.kwota;
                                                    break;
                                                case 30: doc.RataData30 = kdta.data;
                                                    doc.RataKwota30 = kdta.kwota;
                                                    break;
                                                case 31: doc.RataData31 = kdta.data;
                                                    doc.RataKwota31 = kdta.kwota;
                                                    break;
                                                case 32: doc.RataData32 = kdta.data;
                                                    doc.RataKwota32 = kdta.kwota;
                                                    break;
                                                case 33: doc.RataData33 = kdta.data;
                                                    doc.RataKwota33 = kdta.kwota;
                                                    break;
                                                case 34: doc.RataData34 = kdta.data;
                                                    doc.RataKwota34 = kdta.kwota;
                                                    break;
                                                case 35: doc.RataData35 = kdta.data;
                                                    doc.RataKwota35 = kdta.kwota;
                                                    break;
                                                case 36: doc.RataData36 = kdta.data;
                                                    doc.RataKwota36 = kdta.kwota;
                                                    break;

                                                default:
                                                    break;
                                            }
                                            i++;
                                        }
                                           
                                    } // foreach
                                    kwtLst.Clear();
                                  
                                }// if
                                #endregion updtlst
                            } //  if (rStatus && Convert.ToInt32(rdr_rh["Sprawa_Id"]) == doc.Sprawa_Id)



                        } // if (doc.typFakt == "GS" && doc.Stan == "B") 
                        else
                            if (doc.typFakt == "KS" && doc.Stan == "B")  // koszty 
                            {
                                rStatus = true;

                                if (pozostaloscRaty > 0 && SprawaId == doc.Sprawa.KnsSprawa_id)
                                {
                                    kwtData kwx = new kwtData();
                                    kwx.data = Convert.ToDateTime(rdr_rh["Data_Raty"]);
                                    kwx.kwota = pozostaloscRaty;
                                    kwtLst.Add( kwx);
                                    kwt -= pozostaloscRaty;
                                    rStatus = rdr_rh.Read();
                                    SprawaId = Convert.ToInt32(rdr_rh["Sprawa_Id"]);
                                    pozostaloscRaty = 0;
                                }
                                else
                                    pozostaloscRaty = 0;

                                while (rStatus && SprawaId <  doc.Sprawa.KnsSprawa_id)
                                {
                                    rStatus = rdr_rh.Read();
                                    if (!rStatus) break;
                                    SprawaId = Convert.ToInt32(rdr_rh["Sprawa_Id"]);
                                }



                                if (rStatus && SprawaId  == doc.Sprawa.KnsSprawa_id)
                                {// zdejmujemy od odstatniej raty - koszty

                                    if (kwtLst == null)  
                                      kwtLst = new List<kwtData>();
                                    
                                    while (SprawaId ==  doc.Sprawa.KnsSprawa_id)
                                    {
                                        decimal? rata = Convert.ToDecimal(rdr_rh["Kwota_Raty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")); 
                                        kwt = kwt - rata;

                                        kwtData kdt = new kwtData();
                                        kdt.data = Convert.ToDateTime(rdr_rh["Data_Raty"]);
                                        if (kwt < 0)
                                            kdt.kwota = rata + kwt;
                                        else
                                            kdt.kwota = rata;
                                        kwtLst.Add( kdt);
                                        if (kwt <= 0)
                                        {
                                            pozostaloscRaty = Math.Abs(Convert.ToDecimal(kwt));
                                            break;
                                        }
                                        if (!rdr_rh.Read()) break;
                                        SprawaId = Convert.ToInt32(rdr_rh["Sprawa_Id"]);
                                    }

                                    #region updtlst1
                                    if (kwtLst.Count > 0)
                                    {
                                        int i = 1;
                                        for (int j = kwtLst.Count - 1; j >= 0; j--)
                                        {
                                            kwtData kdta = kwtLst.ElementAt(j);
                                            if (kdta.kwota > 0)
                                            {
                                                switch (i)
                                                {
                                                    case 1: doc.RataData1 = kdta.data;
                                                        doc.RataKwota1 = kdta.kwota;
                                                        break;
                                                    case 2: doc.RataData2 = kdta.data;
                                                        doc.RataKwota2 = kdta.kwota;
                                                        break;
                                                    case 3: doc.RataData3 = kdta.data;
                                                        doc.RataKwota3 = kdta.kwota;
                                                        break;
                                                    case 4: doc.RataData4 = kdta.data;
                                                        doc.RataKwota4 = kdta.kwota;
                                                        break;
                                                    case 5: doc.RataData5 = kdta.data;
                                                        doc.RataKwota5 = kdta.kwota;
                                                        break;
                                                    case 6: doc.RataData6 = kdta.data;
                                                        doc.RataKwota6 = kdta.kwota;
                                                        break;
                                                    case 7: doc.RataData7 = kdta.data;
                                                        doc.RataKwota7 = kdta.kwota;
                                                        break;
                                                    case 8: doc.RataData8 = kdta.data;
                                                        doc.RataKwota8 = kdta.kwota;
                                                        break;
                                                    case 9: doc.RataData9 = kdta.data;
                                                        doc.RataKwota9 = kdta.kwota;
                                                        break;
                                                    case 10: doc.RataData10 = kdta.data;
                                                        doc.RataKwota10 = kdta.kwota;
                                                        break;
                                                    case 11: doc.RataData11 = kdta.data;
                                                        doc.RataKwota11 = kdta.kwota;
                                                        break;
                                                    case 12: doc.RataData12 = kdta.data;
                                                        doc.RataKwota12 = kdta.kwota;
                                                        break;
                                                    case 13: doc.RataData13 = kdta.data;
                                                        doc.RataKwota13 = kdta.kwota;
                                                        break;
                                                    case 14: doc.RataData14 = kdta.data;
                                                        doc.RataKwota14 = kdta.kwota;
                                                        break;
                                                    case 15: doc.RataData15 = kdta.data;
                                                        doc.RataKwota15 = kdta.kwota;
                                                        break;
                                                    case 16: doc.RataData16 = kdta.data;
                                                        doc.RataKwota16 = kdta.kwota;
                                                        break;
                                                    case 17: doc.RataData17 = kdta.data;
                                                        doc.RataKwota17 = kdta.kwota;
                                                        break;
                                                    case 18: doc.RataData18 = kdta.data;
                                                        doc.RataKwota18 = kdta.kwota;
                                                        break;
                                                    case 19: doc.RataData19 = kdta.data;
                                                        doc.RataKwota19 = kdta.kwota;
                                                        break;
                                                    case 20: doc.RataData20 = kdta.data;
                                                        doc.RataKwota20 = kdta.kwota;
                                                        break;
                                                    case 21: doc.RataData21 = kdta.data;
                                                        doc.RataKwota21 = kdta.kwota;
                                                        break;
                                                    case 22: doc.RataData22 = kdta.data;
                                                        doc.RataKwota22 = kdta.kwota;
                                                        break;
                                                    case 23: doc.RataData23 = kdta.data;
                                                        doc.RataKwota23 = kdta.kwota;
                                                        break;
                                                    case 24: doc.RataData24 = kdta.data;
                                                        doc.RataKwota24 = kdta.kwota;
                                                        break;
                                                    case 25: doc.RataData25 = kdta.data;
                                                        doc.RataKwota25 = kdta.kwota;
                                                        break;
                                                    case 26: doc.RataData26 = kdta.data;
                                                        doc.RataKwota26 = kdta.kwota;
                                                        break;
                                                    case 27: doc.RataData27 = kdta.data;
                                                        doc.RataKwota27 = kdta.kwota;
                                                        break;
                                                    case 28: doc.RataData28 = kdta.data;
                                                        doc.RataKwota28 = kdta.kwota;
                                                        break;
                                                    case 29: doc.RataData29 = kdta.data;
                                                        doc.RataKwota29 = kdta.kwota;
                                                        break;
                                                    case 30: doc.RataData30 = kdta.data;
                                                        doc.RataKwota30 = kdta.kwota;
                                                        break;
                                                    case 31: doc.RataData31 = kdta.data;
                                                        doc.RataKwota31 = kdta.kwota;
                                                        break;
                                                    case 32: doc.RataData32 = kdta.data;
                                                        doc.RataKwota32 = kdta.kwota;
                                                        break;
                                                    case 33: doc.RataData33 = kdta.data;
                                                        doc.RataKwota33 = kdta.kwota;
                                                        break;
                                                    case 34: doc.RataData34 = kdta.data;
                                                        doc.RataKwota34 = kdta.kwota;
                                                        break;
                                                    case 35: doc.RataData35 = kdta.data;
                                                        doc.RataKwota35 = kdta.kwota;
                                                        break;
                                                    case 36: doc.RataData36 = kdta.data;
                                                        doc.RataKwota36 = kdta.kwota;
                                                        break;

                                                    default:
                                                        break;
                                                }
                                                i++;
                                            }
                                            
                                        } // foreach
                                        kwtLst.Clear();
                                       
                                    }// if
                                    #endregion updtlst1
                                } //  if (rStatus && Convert.ToInt32(rdr_rh["Sprawa_Id"]) == doc.Sprawa_Id)



                                


                            }

                    }
                    Context.SaveChanges(); 
                } //if (rdr_rh.HasRows)
               
            }
            
            catch (Exception ex)
            {
                string msg = "Błąd ";
                // Print error message

                Context.SaveChanges(); 
                MessageBox.Show(msg + currentSpr + ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr_rh != null)
                    rdr_rh.Close();

                if (con.State == ConnectionState.Open)
                    con.Close();
                Context.SaveChanges();
                breakIndicator = true;
            }
        }

        private void setCurrendaRata()
        {
         
        
        
        
        }
        public void ImportRaty()
        {
            // import Rat 
            SqlConnection con = null;
            SqlDataReader rdr_rh = null;
            bool rStatus = false;
            List<kwtData> kwtLst = new List<kwtData>();
            DateTime lastrata = DateTime.Today;
            decimal? pozostaloscRaty = 0;
            string currentSpr = "";
            int SprawaId;
            List<Dokument> dokLst;
            SqlCommand storedProcCommand = null;
            try
            {

                if (CurrentTransfer == null) return;
                // Open connection to the database
                //string ConnectionString = Utils.BuildKnsConnectionString(Konfig);

                string ConnectionString = Utils.BuildMyConnectionString(Context);
                con = new SqlConnection(ConnectionString);
                con.Open();
                switch (Konfig.typKns)
                {
                    case 0:
                        storedProcCommand = new SqlCommand("sp_RatyCR", con);
                        break; 
                    case 1:
                        storedProcCommand = new SqlCommand("sp_Raty", con);
                        break;
                    case 2:
                        storedProcCommand = new SqlCommand("sp_RatyOR", con);
                        break;
                    case 3:
                        storedProcCommand = new SqlCommand("sp_RatyAL", con);
                        break;
                    default:
                        break;
                }
               
                storedProcCommand.CommandType = CommandType.StoredProcedure;
                string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                storedProcCommand.Parameters.Add("@dzien", theday);
                storedProcCommand.Connection = con;
                storedProcCommand.CommandTimeout = 600;
                progressMsg = "Odczyt rat...";
                rdr_rh = storedProcCommand.ExecuteReader();
              
                if (rdr_rh.HasRows)
                {
                    SprawaId = 0;
                    dokLst = CurrentTransfer.Dokument.Where(c => c.Stan == "B").OrderBy(a => a.Sprawa_Id).ThenBy(b => b.typFakt).ToList();

                    foreach (Dokument doc in dokLst)
                    {
                        if (doc.RataKwota36 > 0 || doc.RataKwota35 > 0 || doc.RataKwota34 > 0 || doc.RataKwota33 > 0 || doc.RataKwota32 > 0 || doc.RataKwota31 > 0 || doc.RataKwota30 > 0 || doc.RataKwota29 > 0 || doc.RataKwota28 > 0 || doc.RataKwota27 > 0 || doc.RataKwota26 > 0 || doc.RataKwota25 > 0 || doc.RataKwota24 > 0 || doc.RataKwota23 > 0 || doc.RataKwota22 > 0 || doc.RataKwota21 > 0 || doc.RataKwota20 > 0 || doc.RataKwota19 > 0 ||
                            doc.RataKwota18 > 0 || doc.RataKwota17 > 0 || doc.RataKwota16 > 0 || doc.RataKwota15 > 0 || doc.RataKwota14 > 0 || doc.RataKwota13 > 0 || doc.RataKwota12 > 0 || doc.RataKwota11 > 0 || doc.RataKwota10 > 0 || doc.RataKwota9 > 0 || doc.RataKwota8 > 0 || doc.RataKwota7 > 0 || doc.RataKwota6 > 0 || doc.RataKwota5 > 0 || doc.RataKwota4 > 0 || doc.RataKwota3 > 0 || doc.RataKwota2 > 0 || doc.RataKwota1 > 0
                            ) continue;  // raty z harmonogramu

                        decimal? kwt = 0;
                        
                        currentSpr = doc.Sprawa.Karta + " Id = " + doc.Sprawa.KnsSprawa_id.ToString() + " Typ operacji:" + doc.typFakt;
                        rStatus = true;
                       

                        if (pozostaloscRaty > 0 && SprawaId == doc.Sprawa.KnsSprawa_id)
                        {
                            kwtData kwx = new kwtData();
                            kwx.data = lastrata;
                            kwx.kwota = pozostaloscRaty;
                            kwtLst.Add(kwx);
                            kwt -= pozostaloscRaty;
                            rStatus = rdr_rh.Read();
                            SprawaId = Convert.ToInt32(rdr_rh["Sprawa_Id"]);
                            pozostaloscRaty = 0;
                        }

                        while (rStatus && SprawaId < doc.Sprawa.KnsSprawa_id)
                        {
                            rStatus = rdr_rh.Read();
                            if (!rStatus) break;
                            SprawaId = Convert.ToInt32(rdr_rh["Sprawa_Id"]);
                        }
                        if (rStatus && SprawaId == doc.Sprawa.KnsSprawa_id)
                        {
                            if (doc.typFakt == "GS" && Convert.ToDecimal(rdr_rh["grzywna_pr"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                                kwt = doc.kwota;
                            else if (doc.typFakt == "KS" && Convert.ToDecimal(rdr_rh["koszty_pr"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0) 
                                kwt = doc.kwota;
                            {
                                decimal? ostatnia_rata;
                                decimal? kwota;
                                switch (Konfig.typKns) // currenda
                                {
                                    case 0:
                                        if (Convert.ToDecimal(rdr_rh["nst_rata"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) == 0)
                                        {
                                            continue;
                                        
                                        }
                                        ostatnia_rata = Convert.ToDecimal(rdr_rh["nst_rata"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                                        int ilerat =  Convert.ToInt32(Math.Round(
                                                        (Convert.ToDecimal(rdr_rh["koszty_pr"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"))  + 
                                                        Convert.ToDecimal(rdr_rh["grzywna_pr"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"))   - 
                                                       Convert.ToDecimal(rdr_rh["pierwsz_rata"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) ) /
                                                        Convert.ToDecimal(rdr_rh["nst_rata"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"))  )) +1 ;

                                            DateTime rata1 = Convert.ToDateTime(rdr_rh["dt_pierwszej_raty"]);
                                            int dzn = Convert.ToInt32(rdr_rh["na_jaki_dzien"]);
                                            if ( dzn < 1 || dzn > 31 ) 
                                                dzn = 30;

                                            DateTime ddd = new  DateTime(rata1.Year, rata1.Month, 1);
                                            ddd.AddMonths(ilerat);
                                            if (DateTime.DaysInMonth(ddd.Year, ddd.Month) < dzn)
                                                dzn = DateTime.DaysInMonth(ddd.Year, ddd.Month);
                                            lastrata = new DateTime(ddd.Year, ddd.Month, dzn);
                                        break;
                                    case 1:
                                        ostatnia_rata = Convert.ToDecimal(rdr_rh["ostatnia_rata"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")); 
                                        lastrata =  Convert.ToDateTime(rdr_rh["dt_ostatniej_raty"]);
                                        break;
                                    default:
                                        ostatnia_rata = Convert.ToDecimal(rdr_rh["ostatnia_rata"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")); 
                                        lastrata = Convert.ToDateTime(rdr_rh["dt_ostatniej_raty"]);
                                    break;
                                }
                                if (kwt > 0)
                                {
                                    if (kwt > ostatnia_rata)
                                    {
                                        kwota = ostatnia_rata;
                                        kwt -= ostatnia_rata;
                                    }
                                    else
                                    {
                                        kwota = kwt;
                                        pozostaloscRaty = ostatnia_rata - kwt;
                                        kwt = 0;
                                    }
                                    if (kwota > 0)
                                    {
                                        kwtData kdt = new kwtData();
                                        kdt.data = lastrata;
                                        kdt.kwota = kwota;
                                        kwtLst.Add(kdt);
                                    }
                                    while (kwt > 0)
                                    {
                                        DateTime tmp;
                                        int na_jaki_dzien;
                                        
                                        na_jaki_dzien = Convert.ToInt32(rdr_rh["na_jaki_dzien"]);
                                        if (na_jaki_dzien < 1) na_jaki_dzien = 1;
                                        tmp = new DateTime(lastrata.Year, lastrata.Month, 1);
                                        tmp = tmp.AddDays(-1);
                                        if (na_jaki_dzien > DateTime.DaysInMonth(tmp.Year, tmp.Month))
                                            lastrata = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                                        else
                                            lastrata = new DateTime(tmp.Year, tmp.Month, na_jaki_dzien);
                                        if (kwt > ostatnia_rata)
                                        {
                                            kwota = ostatnia_rata;
                                            kwt -= ostatnia_rata;
                                        }
                                        else
                                        {
                                            kwota = kwt;
                                            pozostaloscRaty = ostatnia_rata - kwt;
                                            kwt = 0;
                                        }
                                      
                                        if (kwota > 0)
                                        {
                                            kwtData kdt = new kwtData();
                                            kdt.data = lastrata;
                                            kdt.kwota = kwota;
                                        
                                            kwtLst.Add(kdt);
                                        }



                                    }// while


                                    // posostale raty grzywny  

                                }



                                #region updtlst
                                if (kwtLst.Count > 0)
                                {
                                    int i = 1;

                                    for (int j = kwtLst.Count - 1; j >= 0; j--)
                                    {
                                        kwtData kdta = kwtLst.ElementAt(j);
                                        if (kdta.kwota > 0)
                                        {
                                            switch (i)
                                            {
                                                case 1: doc.RataData1 = kdta.data;
                                                    doc.RataKwota1 = kdta.kwota;
                                                    break;
                                                case 2: doc.RataData2 = kdta.data;
                                                    doc.RataKwota2 = kdta.kwota;
                                                    break;
                                                case 3: doc.RataData3 = kdta.data;
                                                    doc.RataKwota3 = kdta.kwota;
                                                    break;
                                                case 4: doc.RataData4 = kdta.data;
                                                    doc.RataKwota4 = kdta.kwota;
                                                    break;
                                                case 5: doc.RataData5 = kdta.data;
                                                    doc.RataKwota5 = kdta.kwota;
                                                    break;
                                                case 6: doc.RataData6 = kdta.data;
                                                    doc.RataKwota6 = kdta.kwota;
                                                    break;
                                                case 7: doc.RataData7 = kdta.data;
                                                    doc.RataKwota7 = kdta.kwota;
                                                    break;
                                                case 8: doc.RataData8 = kdta.data;
                                                    doc.RataKwota8 = kdta.kwota;
                                                    break;
                                                case 9: doc.RataData9 = kdta.data;
                                                    doc.RataKwota9 = kdta.kwota;
                                                    break;
                                                case 10: doc.RataData10 = kdta.data;
                                                    doc.RataKwota10 = kdta.kwota;
                                                    break;
                                                case 11: doc.RataData11 = kdta.data;
                                                    doc.RataKwota11 = kdta.kwota;
                                                    break;
                                                case 12: doc.RataData12 = kdta.data;
                                                    doc.RataKwota12 = kdta.kwota;
                                                    break;
                                                case 13: doc.RataData13 = kdta.data;
                                                    doc.RataKwota13 = kdta.kwota;
                                                    break;
                                                case 14: doc.RataData14 = kdta.data;
                                                    doc.RataKwota14 = kdta.kwota;
                                                    break;
                                                case 15: doc.RataData15 = kdta.data;
                                                    doc.RataKwota15 = kdta.kwota;
                                                    break;
                                                case 16: doc.RataData16 = kdta.data;
                                                    doc.RataKwota16 = kdta.kwota;
                                                    break;
                                                case 17: doc.RataData17 = kdta.data;
                                                    doc.RataKwota17 = kdta.kwota;
                                                    break;
                                                case 18: doc.RataData18 = kdta.data;
                                                    doc.RataKwota18 = kdta.kwota;
                                                    break;
                                                case 19: doc.RataData19 = kdta.data;
                                                    doc.RataKwota19 = kdta.kwota;
                                                    break;
                                                case 20: doc.RataData20 = kdta.data;
                                                    doc.RataKwota20 = kdta.kwota;
                                                    break;
                                                case 21: doc.RataData21 = kdta.data;
                                                    doc.RataKwota21 = kdta.kwota;
                                                    break;
                                                case 22: doc.RataData22 = kdta.data;
                                                    doc.RataKwota22 = kdta.kwota;
                                                    break;
                                                case 23: doc.RataData23 = kdta.data;
                                                    doc.RataKwota23 = kdta.kwota;
                                                    break;
                                                case 24: doc.RataData24 = kdta.data;
                                                    doc.RataKwota24 = kdta.kwota;
                                                    break;
                                                case 25: doc.RataData25 = kdta.data;
                                                    doc.RataKwota25 = kdta.kwota;
                                                    break;
                                                case 26: doc.RataData26 = kdta.data;
                                                    doc.RataKwota26 = kdta.kwota;
                                                    break;
                                                case 27: doc.RataData27 = kdta.data;
                                                    doc.RataKwota27 = kdta.kwota;
                                                    break;
                                                case 28: doc.RataData28 = kdta.data;
                                                    doc.RataKwota28 = kdta.kwota;
                                                    break;
                                                case 29: doc.RataData29 = kdta.data;
                                                    doc.RataKwota29 = kdta.kwota;
                                                    break;
                                                case 30: doc.RataData30 = kdta.data;
                                                    doc.RataKwota30 = kdta.kwota;
                                                    break;
                                                case 31: doc.RataData31 = kdta.data;
                                                    doc.RataKwota31 = kdta.kwota;
                                                    break;
                                                case 32: doc.RataData32 = kdta.data;
                                                    doc.RataKwota32 = kdta.kwota;
                                                    break;
                                                case 33: doc.RataData33 = kdta.data;
                                                    doc.RataKwota33 = kdta.kwota;
                                                    break;
                                                case 34: doc.RataData34 = kdta.data;
                                                    doc.RataKwota34 = kdta.kwota;
                                                    break;
                                                case 35: doc.RataData35 = kdta.data;
                                                    doc.RataKwota35 = kdta.kwota;
                                                    break;
                                                case 36: doc.RataData36 = kdta.data;
                                                    doc.RataKwota36 = kdta.kwota;
                                                    break;

                                                default:
                                                    break;
                                            }
                                            i++;
                                        }
                                    } // foreach
                                    kwtLst.Clear();

                                }// if
                                #endregion updtlst
                            } //  if (rStatus && Convert.ToInt32(rdr_rh["Sprawa_Id"]) == doc.Sprawa_Id)

                        } //  if (rStatus && Convert.ToInt32(rdr_rh["Sprawa_Id"]) == doc.Sprawa_Id)

                    }
                    
                    Context.SaveChanges();
                } //if (rdr_rh.HasRows)
            }

            catch (Exception ex)
            {
                string msg = "Błąd ";
                // Print error message

                Context.SaveChanges();
                MessageBox.Show(msg + currentSpr + ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr_rh != null)
                    rdr_rh.Close();

                if (con.State == ConnectionState.Open)
                    con.Close();
                Context.SaveChanges();
                breakIndicator = true;
            }
        }

        public void ImportSaldo()
        {

            SqlDataReader rdr = null;
            SqlConnection con = null;
            const int counter = 200;
            int i;
            int loopcount = 0;
            Sprawa spr;
            Dluznik dl;
            KnsKsiegi knsks;
            string wydzialSekcja;
            string repertorium;
            string ksiega;
            string outSad;
            int numer, rok;
            string oryginRep;
            string errmsg;
            List<string> rList = new List<string>();
            List<string> orygrList = new List<string>();
            int IdSaduOrzek;
            SAPSad mySad; 
            string typSad;
            SAPRodzajSprawy rodzajSpr;
            Dokument doc;
            Dokument dock;
            KnsSad  SadOrzekKns;
            SAPRepertorium repertorzek;
            SqlCommand storedProcCommand = null;
            bool bylySad;
            string field = "";
            Transfer trans;
          //  Thread th = new Thread(progressWindow);
           // th.Start();
            
            DataTable dt = null; 
            DataRow currdtr = null;
            

            try
            {
             
                i = counter;
                ////////
                // p[obranie własnego sądu
                if (String.IsNullOrWhiteSpace(Konfig.StanowiskoFin))
                    mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                else
                    mySad = (from c in Context.SAPSad where c.kod == Konfig.StanowiskoFin select c).FirstOrDefault();
                
                
                //mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                if (mySad == null)
                {
                    MessageBox.Show("Ustaw własny sąd w konfiguracji");
                    return;
                }



                // przygotowanie listy repertoriów
                foreach (SAPRepertorium srep in Context.SAPRepertorium.ToList())
                {
                    string s;
                    string s1;
                    s = srep.kod.Trim().ToUpper();
                    rList.Add(s);
                    s1 = srep.kod.Trim();
                    orygrList.Add(s1);

                }



                
                {
                    //string ConnectionString = Utils.BuildMyConnectionString(Context);
                    string ConnectionString = (Konfig.typKns ==2 ) ?  Utils.BuildMyConnectionString(Context) :  Properties.Settings.Default.KnsMigratorConnectionString;
                    con = new SqlConnection(ConnectionString);
                    con.Open();
                    switch (Konfig.typKns)
                    {
                        case 0: // currenda
                            storedProcCommand = new SqlCommand("sp_DziennikNaleznosciCR", con);
                            break;
                        case 1: // Zeto
                            storedProcCommand = new SqlCommand("sp_DziennikNaleznosci", con);
                            break;
                        case 2: // Orcom
                            storedProcCommand = new SqlCommand("sp_DziennikNaleznosciOR", con);
                            break;
                        case 3: // Albit
                            storedProcCommand = new SqlCommand("sp_DziennikNaleznosciAL", con);
                            break;

                        default:
                            break;
                    }

                    storedProcCommand.CommandType = CommandType.StoredProcedure;
                    string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                    storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                    //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                    storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                    storedProcCommand.Parameters.Add("@dzien", theday);
                    storedProcCommand.Connection = con;
                    storedProcCommand.CommandTimeout = 600;
                    progressMsg = "Odczyt danych...";
                    rdr = storedProcCommand.ExecuteReader();
                    

                    if (rdr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(rdr);
                        
                    }
                    else
                    {
                        MessageBox.Show("Brak danych do importu");
                        return;
                    }
                    //////
                }
                loopcount = 0;
                // setup 
                if (dt != null)
                {
                    trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = 1; // salda
                    trans.DataDo = theday;   // doccelowo podać datę 
                    trans.Uwagi = uwagi;
                    Context.Transfer.AddObject(trans);
                    this.CurrentTransfer = trans;
                    
                    foreach (DataRow dtr in dt.Rows)
                    {
                        currdtr = dtr;
                        
                        field = "";
                        if (breakIndicator == true) break;
                        // sprawdzenie czy jest na liście 
                        int curKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        if (!KsiegiKnsLst.Contains(curKsiega)) continue;

                        // jeśli księga nie jest na liście.

                        progressMsg = "Dokument " + (++loopcount).ToString();
                        //(pForm.Controls["lbInfo"] as Label).Refresh();
                        errmsg = "";
                        doc = null;
                        dock = null;
                        bylySad = false;
                      

                        field = "Osoba fizyczna/Osoba prawna";
                        dl = new Dluznik();
                        if (!String.IsNullOrEmpty(dtr["Osoba fizyczna/Osoba prawna"].ToString().Trim()))
                            dl.FizPraw = dtr["Osoba fizyczna/Osoba prawna"].ToString();
                        else
                            dl.FizPraw = " ";

                        field = "Imię/Nazwa 1";
                      
                        dl.Imie = dtr["Imię/Nazwa 1"] != null ? dtr["Imię/Nazwa 1"].ToString().Replace(";"," "): null;
                     
                        field = "Nazwisko / Nazwa 2";
                        dl.Nazwisko = dtr["Nazwisko / Nazwa 2"] != null ? dtr["Nazwisko / Nazwa 2"].ToString().Replace(";", " ") : "";

                        field = "fiz/praw";
                        if (dl.FizPraw == "X") // jesli osoba prawna - podziel nazwę 
                        {
                            if (!String.IsNullOrEmpty(dl.Nazwisko))
                            {
                                int spc = dl.Imie.LastIndexOf(' ');
                                if (spc > 0 && dl.Nazwisko[0] != ' ')
                                {
                                    string tmp = dl.Imie.Substring(spc + 1);
                                    if (tmp.Trim().Length > 0)
                                    {
                                        dl.Imie = dl.Imie.Substring(0, spc);
                                        dl.Nazwisko = tmp.Trim() + dl.Nazwisko;

                                    }

                                }
                                dl.Nazwisko = dl.Nazwisko.Trim();



                            }

                        }
                        if (dl.Nazwisko.Length > 40)
                            dl.Nazwisko = dl.Nazwisko.Substring(0, 40);
                        if (dl.Imie.Length > 40)
                            dl.Imie = dl.Imie.Substring(0, 40);
                        field = "Ulica";

                        dl.Ulica = dtr["Ulica"] != null ? dtr["Ulica"].ToString().Replace(";", " "):null;
                        field = "Nr domu";
                        dl.NrDomu = dtr["Nr domu"] != null ? dtr["Nr domu"].ToString().Replace(";", " ") :null;
                        field = "Nr mieszkania";
                        dl.NrMieszkania = dtr["Nr mieszkania"] != null ? dtr["Nr mieszkania"].ToString().Replace(";", " ") : null;
                        field = "Pesel";
                        dl.Pesel = dtr["Pesel"].ToString().Trim();
                        if (string.IsNullOrEmpty(dl.Pesel))
                            dl.Pesel = null;
                        else
                        {
                            foreach (char c in dl.Pesel.Trim())
                            {
                                if (!Char.IsDigit(c))
                                {
                                    dl.Pesel = null;
                                    break;
                                }
                            }
                        }
                        field = "NIP";
                        dl.Nip = cleanNIP(dtr["NIP"].ToString().Trim());
                        field = "Parsowanie nrdomu";
                        if (String.IsNullOrEmpty(dl.NrDomu))
                        {
                            if (dl.Ulica != null)
                            {
                                string s = dl.Ulica.Trim();
                                int ii = s.LastIndexOf(' ');
                                int jj;
                                if (ii > 3 && Int32.TryParse(s[ii + 1].ToString(), out jj))
                                {
                                    dl.Ulica = s.Substring(0, ii).Trim();
                                    dl.NrDomu = s.Substring(ii).Trim();
                                    jj = 0;
                                    jj = dl.NrDomu.IndexOf('/');
                                    if (jj > 0 && jj < dl.NrDomu.Length - 1)
                                    {
                                        dl.NrMieszkania = dl.NrDomu.Substring(jj + 1).Trim();
                                        dl.NrDomu = dl.NrDomu.Substring(0, jj).Trim();
                                    }
                                }

                            }

                        }
                        field = "Parsowanie nrmieszkania";
                        if (String.IsNullOrEmpty(dl.NrMieszkania) && dl.NrDomu.ToLower().Contains('m'))
                        {
                            // wyj

                            dl.NrMieszkania = dl.NrDomu.Substring(dl.NrDomu.ToLower().IndexOf('m') + 1).Trim();
                            dl.NrDomu = dl.NrDomu.Substring(0, dl.NrDomu.ToLower().IndexOf('m')).Trim();

                        }
                        dl.NrMieszkania = String.IsNullOrEmpty(dl.NrMieszkania) ? dl.NrMieszkania : dl.NrMieszkania.Replace(" ", String.Empty);
                        dl.NrDomu = String.IsNullOrEmpty(dl.NrDomu) ? dl.NrDomu : dl.NrDomu.Replace(" ", String.Empty);
                        dl.NrMieszkania = dl.NrMieszkania.Length > 10 ? dl.NrMieszkania.Substring(0, 10) : dl.NrMieszkania;
                        dl.NrDomu = dl.NrDomu.Length > 10 ? dl.NrDomu.Substring(0, 10) : dl.NrDomu;

                        field = "Kod pocztowy";
                        dl.KodPocztowy = dtr["Kod pocztowy"].ToString();
                        field = "Miejscowosc";
                        dl.Miejscowosc = dtr["Miejscowość"] != null ? dtr["Miejscowość"].ToString().Replace(";", " ") : null ;
                        {
                            field = "Klucz kraju";
                            string kk = dtr["Klucz kraju"].ToString().Trim().ToUpper();
                            if (kk != "PL")
                            {
                                SAPKodKraju kdkr;

                                kdkr = (from m in Context.SAPKodKraju
                                        where m.kraj.ToUpper() == kk
                                        select m).FirstOrDefault();
                                if (kdkr != null)
                                {
                                    dl.KluczKraju = kdkr.kod;
                                    if (!String.IsNullOrWhiteSpace(dl.KodPocztowy))
                                    {
                                        string kod = this.kodFormat(kdkr.kod, dl.KodPocztowy);
                                        if (!String.IsNullOrWhiteSpace(kod))
                                        {
                                            dl.KodPocztowy = kod;
                                        }
                                    }
                                }
                                else
                                {
                                    dl.KluczKraju = "??";
                                    errmsg = "Nieokreślony kod kraju dłużnika";
                                }
                            }
                            else
                                dl.KluczKraju = kk;

                        }
                        field = "IBAN";
                        dl.Iban = dtr["IBAN"].ToString();
                        field = "Kwalifikator do RBN";
                        dl.RBN = dtr["Kwalifikator do RBN"].ToString();
                        if (string.IsNullOrEmpty(dl.RBN) || string.IsNullOrWhiteSpace(dl.RBN))
                        {
                            if (dl.FizPraw == "X") 
                                dl.RBN = "08";
                            else
                                dl.RBN = "09";

                        }



                        spr = new Sprawa();
                        field = "Sprawa_id";
                        spr.KnsSprawa_id = Convert.ToInt32(dtr["Sprawa_id"]);
                        field = "Ksiega";
                        spr.KnsKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        field = "SadKns";
                        spr.KnsSad = dtr["SadKns"].ToString().Trim();
                        field = "IdSaduOrzek";
                        spr.KNSSadOrzek_id = Convert.ToInt32(dtr["IdSaduOrzek"] == DBNull.Value ? 0 : dtr["IdSaduOrzek"]);
                        field = "Oznaczenie konta umowy";
                        spr.Karta = dtr["Oznaczenie konta umowy"].ToString().Replace(";"," ").Trim();  // karta dłużnika
                        // parsowanie katry dłużnika 
                        {
                            string retval = Utils.ParseKartaDl(Convert.ToInt32(Konfig.typKns), spr.Karta, out ksiega, out numer, out rok);
                            if (retval.Length == 0)
                            {
                                spr.KdRok = rok;
                                spr.KdNumer = numer;


                            }
                            else errmsg += " ; " + retval;
                        }

                        if (spr.Karta == "356/2009/K")
                        {
                            ;

                        }
                        typSad = "";
                        // mn.Relacja_konta = dtr["Relacja konta"].ToString();  stał wartość  99
                        //mn.Typ_konta_umowy = dtr["Typ konta umowy"].ToString();  KN, KN1 jeśli w ramach jednej sygnatury wystepuje kilka kart dłuBnika dla tego samego dłuBnika – dla kol;enych kart wartosci K1, K2…, K9
                        knsks = Context.KnsKsiegi.Where(a => a.Id_Ksiegi == spr.KnsKsiega).SingleOrDefault<KnsKsiegi>();
                        // ksiega 
                        field = "SAP Sad Id";
                        if (knsks != null)
                            spr.SAPRodzajPrzedmiotuUmowy = knsks.rodzajPrzedmiotu; // rodzaj przedmiotu umowy na podstawie ksiegi
                        if (spr.KNSSadOrzek_id > 0)
                        {
                            IdSaduOrzek = spr.KNSSadOrzek_id as int? ?? default(int);
                            SadOrzekKns = (from d in Context.KnsSad
                                           where d.Sad_Id == IdSaduOrzek
                                           select d).FirstOrDefault();
                            if (SadOrzekKns == null)
                            {
                                if (! Convert.ToBoolean(Konfig.defSad))
                                {
                                    spr.SAPSadId = mySad.kod;
                                    typSad = mySad.typSad;
                                    bylySad = true;

                                }//
                            }
                            else
                            {
                                spr.SAPSadId = SadOrzekKns.SAPSad_Id;
                                SAPSad ssd = (from m in Context.SAPSad
                                              where m.kod == spr.SAPSadId
                                              select m).FirstOrDefault();
                                if (ssd != null)
                                    typSad = ssd.typSad;
                                else
                                {

                                    string tmp1 = dtr["Sygnatura"] != null ? dtr["Sygnatura"].ToString().Replace(";"," ").Trim():"";
                                    if (tmp1.Length > 6)
                                    {
                                        tmp1 = tmp1.Substring(6);
                                        if (tmp1.ToUpper().Contains("SR") || tmp1.ToUpper().Contains("S.R") || tmp1.ToUpper().Contains("S. R"))
                                            typSad = "SR";


                                    }

                                }
                            }
                        }

                        else // zaklądamy, że  z własnego sądu
                        {
                            if (!Convert.ToBoolean(Konfig.defSad))
                            {
                                spr.SAPSadId = mySad.kod;
                            }
                                typSad = mySad.typSad;
                            
                            string tmp1 = dtr["Sygnatura"] != null ? dtr["Sygnatura"].ToString().Replace(";", " ").Trim() : null;
                            if (tmp1.Length > 6)
                            {
                                tmp1 = tmp1.Substring(6);
                                if (tmp1.ToUpper().Contains("SR") || tmp1.ToUpper().Contains("S.R") || tmp1.ToUpper().Contains("S. R"))
                                    typSad = "SR";


                            }

                        }// szukamy w tabeli SAP rodzaj sprawy 

                        //spr.SAPSadId = typSad;  // sad orzekający
                        field = "Sygnatura";
                        spr.Sygnatura = dtr["Sygnatura"] != null  ? dtr["Sygnatura"].ToString().Replace(";", " ").Trim():null;
                        {
                            oryginRep = "";
                            repertorium = "";
                            wydzialSekcja = "";
                            string retval = Utils.ParseSygn(spr.Sygnatura, spr.SAPSadId, rList, orygrList, out wydzialSekcja, out repertorium, out numer, out rok, out oryginRep, out outSad );
                            spr.SAPSadId  =outSad;
                            if (retval.Length == 0)
                            {

                                ;

                            }
                            else
                                if (!String.IsNullOrEmpty(spr.SAPSadId) && retval == GlobalStrings.SYGN_IN_SAD)
                                    retval = "";
                                else
                                    errmsg += " ; " + retval;


                            repertorium = repertorium.Trim();
                            spr.Rok = rok;

                            if (repertorium != "")
                                spr.SAPRepertorium = repertorium;

                            if (bylySad)   // jeśli sąd zniesiony 
                            {
                                // szukamy sygnatury 
                                if (!String.IsNullOrEmpty(oryginRep))
                                {
                                    // szukamy sygnatiury w moim sądzie 
                                    SAPRepertorium srp = Context.SAPRepertorium.Where(a => a.kod == oryginRep).FirstOrDefault();
                                    string rpu;
                                    if (srp != null)  // szukamy wydziału w moim sadzie ??? o najwyższym  numerze 
                                    {
                                        //SAPWydzial wydzrep = Context.SAPWydzial.Where(c => c.rodzajSprawy == srp.SymbolRodzajPrzedmiotu && c.kodSad == mySad.kod).OrderByDescending(c => c.numerWydz).FirstOrDefault();
                                        spr.SAPRepertorium = "";
                                        //spr.SAPWydział = wydzrep.numer;
                                        spr.Numer = numer;
                                        //spr.SAPSadId = mySad.kod;
                                        typSad = mySad.typSad;
                                        spr.SAPRodzajPrzedmiotuUmowy = srp.SymbolRodzajPrzedmiotu;

                                    }

                                }


                            }
                            else
                            {
                                spr.SAPWydział = wydzialSekcja.Trim();

                                spr.Numer = numer;
                                // repertorium 2 typsporawy
                                repertorzek = (from e in Context.SAPRepertorium
                                               where e.kod == repertorium
                                               select e).FirstOrDefault();
                                if (repertorzek != null)
                                {
                                    spr.SAPRodzajPrzedmiotuUmowy = repertorzek.SymbolRodzajPrzedmiotu;
                                    if (String.IsNullOrEmpty(repertorzek.SymbolRodzajPrzedmiotu))
                                    {
                                        KnsKsiegi knsk;

                                        knsk = Context.KnsKsiegi.Where(a => a.Id_Ksiegi == spr.KnsKsiega).FirstOrDefault();
                                        if (knsk != null)
                                            spr.SAPRodzajPrzedmiotuUmowy = knsk.rodzajPrzedmiotu;

                                    }


                                }
                            }


                            field = "SAPRodzajSprawy";
                            rodzajSpr = null;
                            if (typSad == "SF") typSad = "SR";
                            if (repertorium.Length > 0)
                            {
                                rodzajSpr = (from f in Context.SAPRodzajSprawy where f.repertorium == repertorium && f.typSad == typSad orderby f.id select f).FirstOrDefault();
                                if (rodzajSpr != null)
                                {
                                    spr.SAPRodzajSprawy = rodzajSpr.kod;

                                }
                            }

                        }
                        field = "SAPWydział";
                        if (spr.SAPWydział != null)
                            if (spr.SAPWydział.Trim().Length == 0)
                            {
                                spr.SAPWydział = null;
                            }
                        field = "SAPRepertorium";
                        if (spr.SAPRepertorium != null)
                            if (spr.SAPRepertorium.Trim().Length == 0)
                            {
                                spr.SAPRepertorium = null;
                            }

                        // rodzaj sprawy 



                        // parsowanie sygnatury
                        // Jednostka Gospodarcza - kolejno =  z Id Sadu  w sprawie, jełśi pusta - to własna  , 
                        field = "SAPTomyAkt";
                        spr.SAPTomyAkt = "001";
                        // grzywna i koszty oddzielnie


                        if (Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".",","),CultureInfo.GetCultureInfo("pl-PL")) > 0 ) // ((dtr["grzywna"] as decimal? ?? default(decimal)) > 0)
                        {
                            doc = new Dokument();
                            doc.SAPImportStatus = 0;
                            doc.DocGuid = Guid.NewGuid();
                            field = "Data dokumentu grzywna";
                            doc.DataDokumentu = dtr["Data dokumentu grzywna"] as DateTime? ?? null;
                            field = "Data księgowania";
                            if (dtr["Data księgowania"] != null)
                                doc.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]);
                            field = "OperacjaGlowna";
                            if (knsks.czyFPP == 1)
                                doc.OperacjaGlowna = "FPP0";
                            else if (knsks.czyFPP == 2)
                                doc.OperacjaGlowna = "N033";
                            else
                                doc.OperacjaGlowna = "N010";
                            /*
                            mn.Rodzaj_dokumentu = dtr["Rodzaj dokumentu"].ToString();
                            mn.Waluta = dtr["Waluta"].ToString();
                            mn.Klucz_uzgodnienia = dtr["Klucz uzgodnienia"].ToString();
                            mn.Jednostaka_gospodarca_własna = mySad
                            */
                            field = "Czysamoistna";
                            if (!String.IsNullOrEmpty(dtr["Czysamoistna"].ToString()))
                            {
                                doc.grzSamoistna = (dtr["Czysamoistna"]).ToString();
                                spr.grzSamoistna = (dtr["Czysamoistna"]).ToString();
                            }
                            else
                            {
                                doc.grzSamoistna = "";
                                spr.grzSamoistna = "";
                            }
                            field = "OperacjaCzesciowa";
                            if (knsks.czyFPP == 1)
                            {
                             
                                doc.OperacjaCzesciowa = (Konfig.typKns == 2) ? "0010" : "0011";
                             
                            } else if (knsks.czyFPP == 2)
                            {
                                doc.OperacjaCzesciowa = "0001";
                            }
                            else
                                switch (spr.SAPRodzajPrzedmiotuUmowy)
                                {
                                    case "SPPR":
                                    case "SROD":
                                    case "SUBE":
                                    case "SRES":
                                    case "SGOS":
                                    case "SCYW":
                                        if (dl.FizPraw == "X")   // osoba prawna
                                            doc.OperacjaCzesciowa = "0090";  // lub "0100"  ??
                                        else
                                            doc.OperacjaCzesciowa = "0010";
                                        break;


                                    case "SKAR":
                                        if (dl.FizPraw == "X")   // osoba prawna
                                            doc.OperacjaCzesciowa = "0090";
                                        else // osoba fizyczna  sprawdzić  czy wykroczenia i czy samoistna
                                        {
                                            if (doc.grzSamoistna == "s")
                                            {
                                                if (repertorium.ToUpper() == "W")
                                                    // wykroczenie
                                                    doc.OperacjaCzesciowa = "0070";
                                                else
                                                    doc.OperacjaCzesciowa = "0040";

                                            }
                                            else
                                            {
                                                if (repertorium.ToUpper() == "W")
                                                    // wykroczenie
                                                    doc.OperacjaCzesciowa = "0050";
                                                else
                                                    doc.OperacjaCzesciowa = "0020";
                                            }
                                        }
                                        break;
                                    default:
                                        errmsg += " ; " + "Brak oznaczenia operacji cześciowej (grzywna) ";
                                        break;
                                }


                            field = "Data wymagalności";
                            doc.DataPlatnosci = dtr["Data wymagalności"] as DateTime? ?? null;   // sprawdzić przy kposztach
                            field = "kwota";
                            doc.kwota = Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));// as decimal? ?? default(decimal);
                            field = "Stan";
                            if (!String.IsNullOrEmpty(dtr["Kara zastępcza"].ToString()))
                                doc.Stan = "F";    // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                            else if (!String.IsNullOrEmpty(dtr["Egzekucja grzywny"].ToString()))
                                doc.Stan = "C";
                            else if (!String.IsNullOrEmpty(dtr["Grzywny odroczone"].ToString()))
                                doc.Stan = "D";
                            else if (!String.IsNullOrEmpty(dtr["Raty grzywna"].ToString()))
                                doc.Stan = "B";
                            else doc.Stan = "A";
                            doc.typFakt = "GS";
                            doc.Info = errmsg.Truncate(255);
                            if (String.IsNullOrEmpty(doc.Info)) doc.Info = null;
                        }

                        if (Convert.ToDecimal(dtr["koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)  //((dtr["koszty"] as decimal? ?? default(decimal)) > 0)
                        {
                            dock = new Dokument();
                            dock.SAPImportStatus = 0;
                            dock.DocGuid = Guid.NewGuid();
                            field = "Data dokumentu koszty";
                            dock.DataDokumentu = dtr["Data dokumentu koszty"] as DateTime? ?? null;
                            field = "Data ksiegowania koszty";
                            if (dtr["Data księgowania"] != null)
                                dock.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]);
                            field = "OperacjaGlowna";
                            if (knsks.czyFPP == 1)
                                dock.OperacjaGlowna = "FPP0";
                            if (knsks.czyFPP == 2)
                                dock.OperacjaGlowna = "N033";
                            else
                            {
                                if (dock.DataKsiegowania.Value.Year >= 2017)
                                    dock.OperacjaGlowna = "N011";
                                else
                                    dock.OperacjaGlowna = "N010";
                            }
                            /*
                            mn.Rodzaj_dokumentu = dtr["Rodzaj dokumentu"].ToString();
                            mn.Waluta = dtr["Waluta"].ToString();
                            mn.Klucz_uzgodnienia = dtr["Klucz uzgodnienia"].ToString();
                            mn.Jednostaka_gospodarca_własna = mySad
                            */

                            field = "grzSamoistnaKoszty";
                            dock.grzSamoistna = "";
                            field = "OperacjaCzesciowa";
                            if (knsks.czyFPP == 1)
                                dock.OperacjaCzesciowa = (Konfig.typKns == 2) ? "0011": "0010";
                            if (knsks.czyFPP == 2)
                                dock.OperacjaCzesciowa = "0001";
                            else
                                switch (spr.SAPRodzajPrzedmiotuUmowy)
                                {
                                    case "SROD":
                                        dock.OperacjaCzesciowa = "0120";
                                        break;
                                    case "SPPR":
                                    case "SUBE":
                                    case "SGOS":
                                    case "SCYW":
                                    case "SRES":
                                        if (dl.FizPraw == "X")   // osoba prawna
                                            dock.OperacjaCzesciowa = "0110";  // brak pozycji w słowniku.
                                        else
                                            dock.OperacjaCzesciowa = "0110";
                                        break;


                                    case "SKAR":
                                        dock.OperacjaCzesciowa = "0130";
                                        break;
                                    default:
                                        errmsg += " ; " + "Brak oznaczenia operacji cześciowej (koszty) ";
                                        break;
                                }
                            field = "Data wymagalności";
                            dock.DataPlatnosci = dtr["Data wymagalności"] as DateTime? ?? null;   // sprawdzić przy kposztach
                            dock.kwota = Convert.ToDecimal(dtr["koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")); //dtr["koszty"] as decimal? ?? default(decimal);
                            field = "Stan Koszty";
                            if (!String.IsNullOrEmpty(dtr["Egzekucja koszty"].ToString()))
                                dock.Stan = "C";
                            else if (!String.IsNullOrEmpty(dtr["Koszty odroczone"].ToString()))
                                dock.Stan = "D";
                            else if (!String.IsNullOrEmpty(dtr["Raty koszty"].ToString()))
                                dock.Stan = "B";
                            else dock.Stan = "A";
                            dock.typFakt = "KS";
                            dock.Info = errmsg.Truncate(255);
                            if (String.IsNullOrEmpty(dock.Info)) dock.Info = null;
                        }
                        field = "Dodawanie dłużnika";
                        spr.Dluznik.Add(dl);

                        if (doc != null)
                        {
                            field = "Dodawanie dokumentu - grzywny";
                            doc.InsertedBy = UserInfo.Username;
                            doc.InsDate = DateTime.Now;
                            spr.Dokument.Add(doc);
                            dl.Dokument.Add(doc);
                            trans.Dokument.Add(doc);
                        }
                        if (dock != null)
                        {
                            field = "Dodawanie dokumentu - koszty";
                            dock.InsertedBy = UserInfo.Username;
                            dock.InsDate = DateTime.Now;
                            spr.Dokument.Add(dock);
                            dl.Dokument.Add(dock);
                            trans.Dokument.Add(dock);
                        }

                        trans.LFaktow = loopcount;
                        field = "Zapis wiersza(y) dla " + spr.Karta;
                        Context.SaveChanges();

                        /*
                        if (--i == 0)
                        {
                            Context.SaveChanges();
                            i = counter;
                            loopcount++;
                        }
                        */


                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Błąd ";
                string innerExcept = "";
                // Print error message
                if (currdtr != null)
                    if (currdtr["Oznaczenie konta umowy"] != null) msg += currdtr["Oznaczenie konta umowy"].ToString().Trim() + " ";
                if (ex.InnerException != null)
                    if (ex.InnerException.Message != null)
                        innerExcept = ex.InnerException.Message;
                MessageBox.Show(msg + " " + ex.Message + "  ost. kolumna: " + field + " " + innerExcept);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con != null)
                if (con.State == ConnectionState.Open)
                    con.Close();
                Context.SaveChanges();
                breakIndicator = true;
            }
        }

        public void ImportPrzypisRupIntegr(Guid myId, TextBox tbmess)
        {
            List<Dokument> dlist = new List<Dokument>();
            int rNo = 0;
            int rCount = 0;

            dlist = (from c in Context.Dokument where c.SAPDocId != null && c.SAPDocId.Length > 1 && (c.typFakt == "GS" || c.typFakt == "KS" || c.typFakt == "KP" || c.typFakt == "GP" ) select c ).ToList();
            if (dlist == null) return;
            foreach (Dokument d in dlist)
            {
                tbmess.Text = (++rNo).ToString() + "/" + rCount.ToString();
                tbmess.Refresh();
                if (!KsiegiKnsLst.Contains(Convert.ToInt32(d.Sprawa.KnsKsiega))) continue;

                // jeśli księga nie jest na liście.
                WalidSaldo walsalk = new WalidSaldo();
                walsalk.Klucz =  myId;
                walsalk.KsiegaOpis = d.SAPDocId;
                walsalk.KartaDl = d.Sprawa.Karta;
                walsalk.Dluznik = d.Dluznik.Imie + " " + d.Dluznik.Nazwisko;
                walsalk.Kwota  = d.kwota;
                walsalk.OpCzesc = d.OperacjaCzesciowa;
                walsalk.OpGlowna = d.OperacjaGlowna;
                walsalk.Sygnatura = d.Sprawa.Sygnatura;
                Context.AddToWalidSaldo(walsalk);
                       
            }
            Context.SaveChanges();
        }
        
        public void ImportSaldoShort(Guid myId, TextBox tbmess)
        {

            SqlDataReader rdr = null;
            SqlConnection con = null;
            const int counter = 200;
            int i;
            int loopcount = 0;
            Sprawa spr;
            Dluznik dl;
            KnsKsiegi knsks;
            string wydzialSekcja;
            string repertorium;
            string ksiega;
            int numer, rok;
            string oryginRep;
            string errmsg;
            List<string> rList = new List<string>();
            List<string> orygrList = new List<string>();
            int IdSaduOrzek;
            SAPSad mySad;
            string typSad;
            SAPRodzajSprawy rodzajSpr;
            Dokument doc;
            Dokument dock;
            KnsSad SadOrzekKns;
            SAPRepertorium repertorzek;
            SqlCommand storedProcCommand = null;
            bool bylySad;
            string field = "";
            Transfer trans;
            //  Thread th = new Thread(progressWindow);
            // th.Start();
            DataTable dt = null;
            DataRow currentdtr = null;
            int rCount = 0 ;
            try
            {
                // Open connection to the database
                //string ConnectionString = Utils.BuildKnsConnectionString(Konfig);
                //string ConnectionString = Utils.BuildMyConnectionString(Context);
                string ConnectionString = (Konfig.typKns == 2) ? Utils.BuildMyConnectionString(Context) : Properties.Settings.Default.KnsMigratorConnectionString;
                //string ConnectionString = Properties.Settings.Default.KnsMigratorConnectionString;
                con = new SqlConnection(ConnectionString);
                con.Open();
                switch (Konfig.typKns)
                {
                    case 0: // currenda
                        storedProcCommand = new SqlCommand("sp_DziennikNaleznosciCR", con);
                        break;
                    case 1: // Zeto
                        storedProcCommand = new SqlCommand("sp_DziennikNaleznosci", con);
                        break;
                    case 2: // Orcom
                        storedProcCommand = new SqlCommand("sp_DziennikNaleznosciOR", con);
                        break;
                    case 3: // Albit
                        storedProcCommand = new SqlCommand("sp_DziennikNaleznosciAL", con);
                        break;

                    default:
                        break;
                }

                storedProcCommand.CommandType = CommandType.StoredProcedure;
                string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                storedProcCommand.Parameters.Add("@dzien", theday);
                storedProcCommand.Connection = con;
                storedProcCommand.CommandTimeout = 600;
                progressMsg = "Odczyt danych...";

                rdr = storedProcCommand.ExecuteReader();

                i = counter;
                ////////
                // p[obranie własnego sądu
                mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                if (mySad == null)
                {
                    MessageBox.Show("Ustaw własny sąd w konfiguracji");
                    return;
                }



                // przygotowanie listy repertoriów
                foreach (SAPRepertorium srep in Context.SAPRepertorium.ToList())
                {
                    string s;
                    string s1;
                    s = srep.kod.Trim().ToUpper();
                    rList.Add(s);
                    s1 = srep.kod.Trim();
                    orygrList.Add(s1);


                }


                if (rdr.HasRows)
                {
                    trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = 1; // salda
                    trans.DataDo = theday;   // doccelowo podać datę 
                    trans.Uwagi = uwagi;
                    dt = new DataTable();
                    
                    dt.Load(rdr);
                    rCount = dt.Rows.Count;
                    
                }
                else
                {
                    MessageBox.Show("Brak danych do importu");
                    return;
                }
                //////

                loopcount = 0;
                // setup 



                if (dt != null)
                {
                    int rNo = 0;
                    foreach (DataRow dtr in dt.Rows)
                    {
                        tbmess.Text = (++rNo).ToString() + "/" + rCount.ToString();
                        tbmess.Refresh();
                        currentdtr = dtr;
                        field = "";
                        if (breakIndicator == true) break;
                        // sprawdzenie czy jest na liście 
                        int curKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        if (!KsiegiKnsLst.Contains(curKsiega)) continue;

                        // jeśli księga nie jest na liście.
                        WalidSaldo walsal = new WalidSaldo();
                        WalidSaldo walsalk = new WalidSaldo();
                        walsalk.Klucz = walsal.Klucz = myId;

                        progressMsg = "Dokument " + (++loopcount).ToString();
                        //(pForm.Controls["lbInfo"] as Label).Refresh();
                        errmsg = "";
                        doc = null;
                        dock = null;
                        bylySad = false;

                        dl = new Dluznik();

                        field = "Imię/Nazwa 1";
                        dl.Imie = dtr["Imię/Nazwa 1"].ToString();
                        field = "Nazwisko / Nazwa 2";
                        dl.Nazwisko = dtr["Nazwisko / Nazwa 2"].ToString();
                        field = "fiz/praw";
                        if (dl.FizPraw == "X") // jesli osoba prawna - podziel nazwę 
                        {
                            if (!String.IsNullOrEmpty(dl.Nazwisko))
                            {
                                int spc = dl.Imie.LastIndexOf(' ');
                                if (spc > 0 && dl.Nazwisko[0] != ' ')
                                {
                                    string tmp = dl.Imie.Substring(spc + 1);
                                    if (tmp.Trim().Length > 0)
                                    {
                                        dl.Imie = dl.Imie.Substring(0, spc);
                                        dl.Nazwisko = tmp.Trim() + dl.Nazwisko;

                                    }

                                }
                                dl.Nazwisko = dl.Nazwisko.Trim();



                            }

                        }

                        if (dl.Nazwisko.Length > 40)
                            dl.Nazwisko = dl.Nazwisko.Substring(0, 40);
                        if (dl.Imie.Length > 40)
                            dl.Imie = dl.Imie.Substring(0, 40);
                        walsalk.Dluznik = walsal.Dluznik = (dl.Imie + " " + dl.Nazwisko).Trim();





                        walsalk.SprawaId =  walsal.SprawaId = Convert.ToInt32(dtr["Sprawa_id"]);
                        field = "Ksiega";
                        walsalk.Ksiega =  walsal.Ksiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        walsalk.KartaDl =  walsal.KartaDl = dtr["Oznaczenie konta umowy"].ToString().Trim();  // karta dłużnika


                        knsks = Context.KnsKsiegi.Where(a => a.Id_Ksiegi == walsal.Ksiega).SingleOrDefault<KnsKsiegi>();
                        walsalk.KsiegaOpis = walsal.KsiegaOpis = knsks.nazwa;
                        walsalk.Sygnatura =  walsal.Sygnatura = dtr["Sygnatura"].ToString().Trim();


                        if (Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0) // ((dtr["grzywna"] as decimal? ?? default(decimal)) > 0)
                         {
                            walsal.Naleznosc = "grzywna";
                            walsal.Kwota = Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                            if (dtr["Data księgowania"] != null)
                                walsal.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]);
                            Context.AddToWalidSaldo(walsal);
                        }

                        if (Convert.ToDecimal(dtr["koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                        {
                            walsalk.Naleznosc = "koszty";
                            walsalk.Kwota = Convert.ToDecimal(dtr["koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                            if (dtr["Data księgowania"] != null)
                                walsalk.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]);
                            Context.AddToWalidSaldo(walsalk);
                        }

                    }

                    Context.SaveChanges();





                }
            }

            catch (Exception ex)
            {
                string msg = "Błąd ";
                string innerExcept = "";
                // Print error message
                if (currentdtr != null)
                    if (currentdtr["Oznaczenie konta umowy"] != null) msg += currentdtr["Oznaczenie konta umowy"].ToString().Trim() + " ";
                if (ex.InnerException != null)
                    if (ex.InnerException.Message != null)
                        innerExcept = ex.InnerException.Message;
                MessageBox.Show(msg + " " + ex.Message + "  ost. kolumna: " + field + " " + innerExcept);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();

                if (con.State == ConnectionState.Open)
                    con.Close();
                Context.SaveChanges();
                breakIndicator = true;
            }
        }


        private int docExists(Dokument doc, string karta ,  out string message)
         { 
           // sprawdzenie czy dany dokument już istnieje
            List<Dokument> docLst  = new List<Dokument>(); 

            //docLst =  Context.Dokument.Include("Transfer").Where (a=>a.KnsKsiegaDzNal == doc.KnsKsiegaDzNal && a.typFakt == doc.typFakt && a.DataKsiegowania == doc.DataKsiegowania  && a.KnsPozDzNal == doc.KnsPozDzNal && a.KnsRokDzNal == doc.KnsRokDzNal && a.OperacjaGlowna == doc.OperacjaGlowna  && a.kwota == doc.kwota ).ToList();
           //  docLst = Context.Dokument.Include("Transfer").Where(a => a.KnsKsiegaDzNal == doc.KnsKsiegaDzNal && a.typFakt == doc.typFakt && a.DataKsiegowania == doc.DataKsiegowania && a.KnsPozDzNal == doc.KnsPozDzNal && a.KnsRokDzNal == doc.KnsRokDzNal && a.OperacjaGlowna == doc.OperacjaGlowna ).ToList();
            
            docLst =    (from m in Context.Sprawa
                                  join n in Context.Dokument on m.Id equals n.Sprawa_Id
                         where n.typFakt == doc.typFakt && n.DataKsiegowania >= this.data_od && n.DataKsiegowania <= this.theday && n.KnsPozDzNal == doc.KnsPozDzNal && n.KnsRokDzNal == doc.KnsRokDzNal && n.OperacjaGlowna == doc.OperacjaGlowna && n.kwota == doc.kwota && m.Karta == karta
                                  select n).ToList();

            // docLst = Context.Dokument.Include("Transfer").Where(a => a.typFakt == doc.typFakt && a.DataKsiegowania >= this.data_od && a.DataKsiegowania<= this.theday && a.KnsPozDzNal == doc.KnsPozDzNal && a.KnsRokDzNal == doc.KnsRokDzNal && a.OperacjaGlowna == doc.OperacjaGlowna && a.kwota == doc.kwota && a.Sprawa.Karta == doc.Sprawa.Karta).ToList();
            
            if (docLst == null || !docLst.Any())
            {
                message = "";
                return 0;
            
            }

            message = "Taki dokument już został dodany,  import: " + docLst.FirstOrDefault().Transfer.DataTransferu.ToString() + docLst.FirstOrDefault().Transfer.Uwagi;
              return 1;
        
           
        
        }

        private Sprawa updateSprawa(Sprawa inDl,  Sprawa outDl)
        {
        outDl.KNSSadOrzek_id = inDl.KNSSadOrzek_id;
        outDl.KnsWydzial = inDl.KnsWydzial;
        outDl.Numer = inDl.Numer;
        outDl.Rok = inDl.Rok;
        outDl.SAPRodzajPrzedmiotuUmowy = inDl.SAPRodzajPrzedmiotuUmowy;
        outDl.SAPRodzajSprawy = inDl.SAPRodzajSprawy;
        outDl.SAPSadId = inDl.SAPSadId;
        outDl.SAPTomyAkt = inDl.SAPTomyAkt ;
        outDl.SAPTypKontaUmowy = inDl.SAPTypKontaUmowy;
        outDl.SAPWydział = inDl.SAPWydział;
        outDl.Sygnatura = inDl.Sygnatura;
        outDl.grzSamoistna = inDl.grzSamoistna;
        outDl.DataWyroku = inDl.DataWyroku;
        outDl.DataWymagalnosci = inDl.DataWymagalnosci;
        outDl.DataPrawomocn = inDl.DataPrawomocn;
        outDl.SAPRepertorium = inDl.SAPRepertorium;
        outDl.SAPRelacjaKontaUmowy = String.IsNullOrWhiteSpace(inDl.SAPRelacjaKontaUmowy) ? outDl.SAPRelacjaKontaUmowy : inDl.SAPRelacjaKontaUmowy;
        return outDl;
        }

        private Dluznik updateDluznik(Dluznik inDl, Dluznik outDl)
        {
            //outDl.FizPraw = inDl.FizPraw;
            outDl.Iban = inDl.Iban;
            outDl.Imie = inDl.Imie;
            outDl.KluczKraju = inDl.KluczKraju;
            outDl.KodPocztowy = inDl.KodPocztowy;
            outDl.Miejscowosc = inDl.Miejscowosc;
            outDl.Nazwisko = inDl.Nazwisko;
            outDl.Nip = inDl.Nip;
            outDl.NrDomu = inDl.NrDomu;
            outDl.NrMieszkania = inDl.NrMieszkania;
            outDl.Pesel = inDl.Pesel;
            outDl.RBN = inDl.RBN;
            outDl.Ulica = inDl.Ulica;
           
            return outDl;
        }


        private void refreshPosition ( Sprawa sp, Dluznik dl )
        {
            if (this.updateTransfer == null)
                return ;
            foreach (Dokument d in this.updateTransfer.Dokument.ToList())
            { 
                if (d.Sprawa == null || !d.Sprawa.KnsSprawa_id.HasValue) continue;
                if (!String.IsNullOrWhiteSpace(d.Sprawa.SAPPrzedmiotUmowy)) continue; // sprawa jest już w sap'ie
                if (d.Sprawa.Karta == sp.Karta && d.Sprawa.KnsSprawa_id.Value == sp.KnsSprawa_id.Value)
                {
                   d.Sprawa =  updateSprawa(sp, d.Sprawa);
                   if (!String.IsNullOrWhiteSpace(d.Dluznik.SAPKontoPartnera)) continue;
;                        d.Dluznik = updateDluznik(dl, d.Dluznik);
                }
            
            }
            
        
        }
        private void updateTrasDates(Transfer trans, DateTime d)
        {
            trans.DataDo = d > (trans.DataDo ?? DateTime.MinValue) ? d : trans.DataDo;
            trans.DataOd = d < (trans.DataOd ?? DateTime.MaxValue) ? d : trans.DataOd;
            if (trans.DataOd < DateTime.MaxValue)
            {
                trans.Miesiac = trans.DataOd.Value.Month;
                trans.Rok = trans.DataOd.Value.Year;
            }
        }

        public void ImportPrzypis()
        {

            SqlDataReader rdr = null;
            SqlConnection con = null;
            const int counter = 200;
            int i;
            int loopcount = 0;
            Sprawa spr;
            Dluznik dl;
            KnsKsiegi knsks;
            string wydzialSekcja;
            string repertorium;
            string ksiega;
            string outSad;
            int numer, rok;
            string errmsg;
            List<string> rList = new List<string>();
            List<string> orygrList = new List<string>();
            int IdSaduOrzek;
            SAPSad mySad;
            string typSad;
            SAPRodzajSprawy rodzajSpr;
            Dokument doc;
            Dokument dock;
            KnsSad SadOrzekKns;
            SAPRepertorium repertorzek;
            string oryginRep;
            SqlCommand storedProcCommand = null;
            string doc2Hash = String.Empty;
            string dock2Hash = String.Empty;
            Transfer trans;
            DateTime dFirst;
            DateTime dLast;
            DataTable dt = new DataTable();
            DataRow currentdtr = null;
            bool refreshMode = false;
            
            //  Thread th = new Thread(progressWindow);
            // th.Start();

            try
            {
                // Open connection to the database
                errorStatus = false;
                i = counter;
                ImportedDocs = 0;
                miesPackHlp.Context = this.Context;
                if (String.IsNullOrWhiteSpace(Konfig.StanowiskoFin ))
                    mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                else
                    mySad = (from c in Context.SAPSad where c.kod == Konfig.StanowiskoFin select c).FirstOrDefault();

                if (mySad == null)
                {
                    MessageBox.Show("Ustaw własny sąd w konfiguracji");
                    return;
                }

          

                // przygotowanie listy repertoriów
                foreach (SAPRepertorium srep in Context.SAPRepertorium.ToList())
                {
                    string s;
                    string s1;
                    s = srep.kod.Trim().ToUpper();
                    rList.Add(s);
                    s1 = srep.kod.Trim();
                    orygrList.Add(s1);

                }
            
               {
                    //string ConnectionString = Utils.BuildMyConnectionString(Context);
                    //string ConnectionString = (Konfig.typKns == 2) ? Utils. : Properties.Settings.Default.KnsMigratorConnectionString;
                  
                    string ConnectionString = (Konfig.typKns == 2) ? Utils.BuildMyConnectionString(Context) : Properties.Settings.Default.KnsMigratorConnectionString;
                    con = new SqlConnection(ConnectionString);
                    con.Open();
                
                    if (this.typImport == 6) // UGO
                    {
                        switch (Konfig.typKns)
                        {
                            case 0: // currenda
                                storedProcCommand = new SqlCommand("sp_UgoCR", con);
                                break;
                            case 1: // Zeto
                                storedProcCommand = new SqlCommand("sp_Ugo", con);
                                break;
                            case 2: // Zeto
                                storedProcCommand = new SqlCommand("sp_UgoOR", con);
                                break;
                            case 3: // Zeto
                                storedProcCommand = new SqlCommand("sp_UgoAL", con);
                                break;
                            default: 
                                break;
                        }
                    }
                    else
                    switch (Konfig.typKns)
                    {
                        case 0: // currenda
                            storedProcCommand = new SqlCommand("sp_PrzypisyCR", con);
                            break;
                        case 1: // Zeto
                            storedProcCommand = new SqlCommand("sp_Przypisy", con);
                            break;
                        case 2: // Zeto
                            storedProcCommand = new SqlCommand("sp_PrzypisyOR", con);
                            break;
                        case 3: // Zeto
                            storedProcCommand = new SqlCommand("sp_PrzypisyAL", con);
                            break;
                        default:
                            break;
                    }
                    storedProcCommand.CommandType = CommandType.StoredProcedure;
                    string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                    storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                    //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                    storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                    storedProcCommand.Parameters.Add("@dataDo", theday);
                    storedProcCommand.Parameters.Add("@dataOd", data_od);
                    if ( this.typImport == 2 ) // tylko dla przypisów
                        storedProcCommand.Parameters.Add("@sprList", this.sprList == null ? "":this.sprList);

                    storedProcCommand.Connection = con;
                    storedProcCommand.CommandTimeout = 600;
                    progressMsg = "Odczyt danych...";
                    if (!String.IsNullOrWhiteSpace(sprList)) refreshMode = true;
                    rdr = storedProcCommand.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(rdr);

                    }
                }


                


                ////////
                // p[obranie własnego sądu


                if (dt == null)
                {
                    if (RunMode.silentMode)
                        Utils.LogWriter("Brak danych do importu");
                    else
                        MessageBox.Show(refreshMode  ? "Brak danych" : "Brak danych do importu");
                    errorStatus = true;
                    return;


                }
                else
                {
                    trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = this.typImport; // przypisy
                    trans.DataOd = new DateTime(2099, 12, 31);
                    trans.DataDo = new DateTime(2000, 1, 1);   // doccelowo podać datę 
                    trans.Uwagi = uwagi;
                    dFirst = DateTime.MaxValue;
                    dLast = DateTime.MinValue;
                    trans.Bledne = 0;
                    trans.Kwota = 0;
                    trans.LFaktow = 0;
                    trans.Zaimportowane = 0;
                    
                    
                    if (!refreshMode)
                        this.CurrentTransfer = trans;


                    loopcount = 0;
                    // setup 


                    
                    foreach (DataRow dtr in dt.Rows)
                    {
                        currentdtr = dtr;
                        if (breakIndicator == true) break;
                        int curKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        //if (!RunMode.silentMode) // jeśli w trybie cichym to ze wszystkich ksiąg 
                        //{
                        if (KsiegiKnsLst.Any())
                        {
                            if (!KsiegiKnsLst.Contains(curKsiega)) continue;
                        }
                            //}
                        progressMsg = "Dokument " + (++loopcount).ToString();
                       
                        doc2Hash = curKsiega.ToString();
                        dock2Hash = curKsiega.ToString();
                        //(pForm.Controls["lbInfo"] as Label).Refresh();
                        errmsg = "";
                        doc = null;
                        dock = null;

                       // Utils.LogWriter("Imp dłużnik");
                        dl = new Dluznik();
                        if (!String.IsNullOrEmpty(dtr["Osoba fizyczna/Osoba prawna"].ToString().Trim()))
                            dl.FizPraw = dtr["Osoba fizyczna/Osoba prawna"].ToString();
                        else
                            dl.FizPraw = "";
                        dl.Imie = dtr["Imię/Nazwa 1"].ToString();
                        dl.Nazwisko = dtr["Nazwisko / Nazwa 2"].ToString();
                        // Utils.LogWriter("Imp dłużnik OK");
                        if (dl.FizPraw == "X") // jesli osoba prawna - podziel nazwę 
                        {
                            if (!String.IsNullOrEmpty(dl.Nazwisko))
                            {
                                int spc = dl.Imie.LastIndexOf(' ');
                                if (spc > 0 && dl.Nazwisko[0] != ' ')
                                {
                                    string tmp = dl.Imie.Substring(spc + 1);
                                    if (tmp.Trim().Length > 0)
                                    {
                                        dl.Imie = dl.Imie.Substring(0, spc);
                                        dl.Nazwisko = tmp.Trim() + dl.Nazwisko;

                                    }

                                }
                                dl.Nazwisko = dl.Nazwisko.Trim();



                            }

                        }
                        // Utils.LogWriter("Imp dłużnik 1");
                        if (dl.Nazwisko.Length > 40)
                            dl.Nazwisko = dl.Nazwisko.Substring(0, 40);
                        if (dl.Imie.Length > 40)
                            dl.Imie = dl.Imie.Substring(0, 40);
                        // Utils.LogWriter("Imp dłużnik 2");
                        dl.Ulica = dtr["Ulica"].ToString();
                        dl.NrDomu = dtr["Nr domu"].ToString();
                        dl.NrMieszkania = dtr["Nr mieszkania"].ToString();
                        dl.Pesel = dtr["Pesel"].ToString().Trim();
                        dl.InsertedBy = UserInfo.Username;
                        dl.InsDate = DateTime.Now;
                        // Utils.LogWriter("Imp dłużnik 3");
                        if (string.IsNullOrEmpty(dl.Pesel))
                            dl.Pesel = null;
                        else
                        {
                            foreach (char c in dl.Pesel.Trim())
                            {
                                if (!Char.IsDigit(c))
                                {
                                    dl.Pesel = null;
                                    break;
                                }
                            }
                        }

                        // Utils.LogWriter("Imp dłużnik Pesel");
                        dl.Nip = cleanNIP(dtr["NIP"].ToString().Trim());
                        if (String.IsNullOrEmpty(dl.NrDomu))
                        {
                            if (dl.Ulica != null)
                            {
                                string s = dl.Ulica.Trim();
                                int ii = s.LastIndexOf(' ');
                                int jj;
                                if (ii > 3 && Int32.TryParse(s[ii + 1].ToString(), out jj))
                                {
                                    dl.Ulica = s.Substring(0, ii).Trim();
                                    dl.NrDomu = s.Substring(ii).Trim();
                                    jj = 0;
                                    jj = dl.NrDomu.IndexOf('/');
                                    if (jj > 0 && jj < dl.NrDomu.Length - 1)
                                    {
                                        dl.NrMieszkania = dl.NrDomu.Substring(jj + 1).Trim();
                                        dl.NrDomu = dl.NrDomu.Substring(0, jj).Trim();
                                    }
                                }

                            }

                        }
                        // Utils.LogWriter("Imp dłużnik split nr dom");
                        if (String.IsNullOrEmpty(dl.NrMieszkania) && dl.NrDomu.ToLower().Contains('m'))
                        {
                            // wyj

                            dl.NrMieszkania = dl.NrDomu.Substring(dl.NrDomu.ToLower().IndexOf('m') + 1).Trim();
                            dl.NrDomu = dl.NrDomu.Substring(0, dl.NrDomu.ToLower().IndexOf('m')).Trim();

                        }

                        
                        dl.KodPocztowy = dtr["Kod pocztowy"].ToString();
                        dl.Miejscowosc = dtr["Miejscowość"].ToString();
                        {
                            string kk = dtr["Klucz kraju"].ToString().Trim().ToUpper();
                            if (kk != "PL")
                            {
                                SAPKodKraju kdkr;

                                kdkr = (from m in Context.SAPKodKraju
                                        where m.kraj.ToUpper() == kk
                                        select m).FirstOrDefault();
                                if (kdkr != null)
                                {
                                    dl.KluczKraju = kdkr.kod;
                                    // sformatowanie kodu pocztowego
                                    if (!String.IsNullOrWhiteSpace(dl.KodPocztowy))
                                    {
                                        string kod = this.kodFormat(kdkr.kod, dl.KodPocztowy);
                                        if (!String.IsNullOrWhiteSpace(kod))
                                        {
                                            dl.KodPocztowy = kod;
                                        }
                                     }
                                }
                                else
                                {
                                    dl.KluczKraju = "??";
                                    errmsg = "Nieokreślony kod kraju dłużnika";
                                }
                            }
                            else
                                dl.KluczKraju = kk;

                        }
                        // Utils.LogWriter("Imp dłużnik 4");
                        dl.Iban = dtr["IBAN"].ToString();
                        dl.RBN = dtr["Kwalifikator do RBN"].ToString();
                        if (string.IsNullOrEmpty(dl.RBN) || string.IsNullOrWhiteSpace(dl.RBN))
                        {
                            if (dl.FizPraw == "X")
                                dl.RBN = "08";
                            else
                                dl.RBN = "09";

                        }
                        // Utils.LogWriter("Imp dłużnik OK " + dl.Nazwisko);


                        spr = new Sprawa();

                        spr.KnsSprawa_id = Convert.ToInt32(dtr["Sprawa_id"]);
                        spr.KnsKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        spr.KnsSad = dtr["SadKns"].ToString().Trim();
                        // Utils.LogWriter("Imp sprawy 1" );
                        spr.KNSSadOrzek_id = Convert.ToInt32(dtr["IdSaduOrzek"] == DBNull.Value ? 0 : dtr["IdSaduOrzek"]);
                        spr.Karta = dtr["Oznaczenie konta umowy"].ToString().Trim();  // karta dłużnika
                        // Utils.LogWriter("Imp sprawy 2");
                        spr.InsDate = DateTime.Now;
                        spr.InsertedBy = UserInfo.Username;
                        // sprawdzamy czy mamy już taką sprawę
                        if (!refreshMode )         
                        {
                            Sprawa sprx =  (from s in Context.Sprawa
                                                    join d in Context.Dokument
                                                    on s.Id equals d.Sprawa_Id
                                                    where s.KnsSprawa_id == spr.KnsSprawa_id && s.SAPPrzedmiotUmowy.Length > 5 && (d.typFakt == "GP" || d.typFakt == "KP" || d.typFakt == "GS" || d.typFakt == "KS")
                                                    orderby s.Id descending
                                                    select s).FirstOrDefault();
                            if (sprx != null)
                            sprx = this.Context.Sprawa.Include("Dluznik").Where(a => a.Id == sprx.Id).FirstOrDefault();
                            if (sprx != null)
                            {
                                // Utils.LogWriter("Imp sprawy -znaleziona " + sprx.Sygnatura);
                                spr.SAPKontoUmowy = sprx.SAPKontoUmowy;
                                // Utils.LogWriter("Imp sprawy a" + sprx.SAPKontoUmowy);
                                spr.SAPPrzedmiotUmowy = sprx.SAPPrzedmiotUmowy;
                                // Utils.LogWriter("Imp sprawy b ");
                                if (sprx.Dluznik != null && sprx.Dluznik.Any())
                                {
                                    dl.SAPKontoPartnera = sprx.Dluznik.FirstOrDefault().SAPKontoPartnera;

                                }

                            }
                        }
                        // Utils.LogWriter("Imp sprawy 3" + spr.Karta);
                        // parsowanie katry dłużnika 
                        {
                            string retval = Utils.ParseKartaDl(Convert.ToInt32(Konfig.typKns), spr.Karta, out ksiega, out numer, out rok);
                            if (retval.Length == 0)
                            {
                                spr.KdRok = rok;
                                spr.KdNumer = numer;
                                doc2Hash += rok.ToString();
                                dock2Hash += rok.ToString();
                                doc2Hash += numer.ToString();
                                dock2Hash += numer.ToString();

                            }
                            else errmsg += " ; " + retval;
                        }

                        // Utils.LogWriter("Imp sprawy 2 ");
                        // mn.Relacja_konta = dtr["Relacja konta"].ToString();  stał wartość  99
                        //mn.Typ_konta_umowy = dtr["Typ konta umowy"].ToString();  KN, KN1 jeśli w ramach jednej sygnatury wystepuje kilka kart dłuBnika dla tego samego dłuBnika – dla kol;enych kart wartosci K1, K2…, K9
                        knsks = Context.KnsKsiegi.Where(a => a.Id_Ksiegi == spr.KnsKsiega).SingleOrDefault<KnsKsiegi>();
                        // ksiega 
                        if (knsks != null)
                            spr.SAPRodzajPrzedmiotuUmowy = knsks.rodzajPrzedmiotu; // rodzaj przedmiotu umowy na podstawie ksiegi
                        typSad = null;
                        if (spr.KNSSadOrzek_id > 0)
                        {
                            IdSaduOrzek = spr.KNSSadOrzek_id as int? ?? default(int);
                            SadOrzekKns = (from d in Context.KnsSad
                                           where d.Sad_Id == IdSaduOrzek
                                           select d).FirstOrDefault();
                            if (SadOrzekKns == null)
                            {
                                //spr.SAPSadId = mySad.kod;
                                errmsg += " ; " + "Brak sąd orzekającego w słowniku - zaimportuj sądy i zamapuj je a następnie ponów import ";
                                //typSad = mySad.typSad;
                            }
                            else
                            {
                                spr.SAPSadId = SadOrzekKns.SAPSad_Id;
                                SAPSad ss = (from d in Context.SAPSad
                                             where d.kod == spr.SAPSadId
                                             select d).FirstOrDefault();
                                if (ss != null)
                                    typSad = ss.typSad;
                                else
                                    typSad = null;

                            }
                        }

                        else // zaklądamy, że  z włsnego sądu
                        {
                            spr.SAPSadId = mySad.kod;
                            typSad = mySad.typSad;

                        }// szukamy w tabeli SAP rodzaj sprawy 

                        //spr.SAPSadId = typSad;  // sad orzekający
                        
                        spr.Sygnatura = dtr["Sygnatura"].ToString().Trim();
                        // Utils.LogWriter("Imp sprawy -sąd orzek " + spr.Sygnatura);
                        {
                            string retval = Utils.ParseSygn(spr.Sygnatura, spr.SAPSadId, rList, orygrList, out wydzialSekcja, out repertorium, out numer, out rok, out oryginRep, out outSad);
                            spr.SAPSadId = outSad;
                            if (retval.Length == 0)
                            {

                                ;

                            }
                            else
                                if (!String.IsNullOrEmpty(spr.SAPSadId) && retval == GlobalStrings.SYGN_IN_SAD)
                                    retval = "";
                                else
                                    errmsg += " ; " + retval;



                            repertorium = repertorium.Trim();
                            spr.Rok = rok;
                            if (repertorium != "")
                                spr.SAPRepertorium = repertorium;
                            spr.SAPWydział = wydzialSekcja.Trim();
                            spr.Numer = numer;
                            // repertorium 2 typsporawy
                            repertorzek = (from e in Context.SAPRepertorium
                                           where e.kod == repertorium
                                           select e).FirstOrDefault();
                            if (repertorzek != null)
                            {
                                spr.SAPRodzajPrzedmiotuUmowy = repertorzek.SymbolRodzajPrzedmiotu;
                                if (String.IsNullOrEmpty(repertorzek.SymbolRodzajPrzedmiotu))
                                {
                                    KnsKsiegi knsk;

                                    knsk = Context.KnsKsiegi.Where(a => a.Id_Ksiegi == spr.KnsKsiega).FirstOrDefault();
                                    if (knsk != null)
                                        spr.SAPRodzajPrzedmiotuUmowy = knsk.rodzajPrzedmiotu;

                                }
                            }

                         }
                        // Utils.LogWriter("Imp sprawy rodzaj sądu  " );
                        rodzajSpr = null;
                        if (typSad == "SF") typSad = "SR";
                        if (repertorium.Length > 0)
                        {
                        
                            rodzajSpr = (from f in Context.SAPRodzajSprawy where f.repertorium == repertorium && f.typSad == typSad orderby f.id select f).FirstOrDefault();
                            if (rodzajSpr != null)
                            {
                                spr.SAPRodzajSprawy = rodzajSpr.kod;

                            }
                        }

                        // rodzaj sprawy 

                        // Utils.LogWriter("Imp Należnosci");

                        // parsowanie sygnatury
                        // Jednostka Gospodarcza - kolejno =  z Id Sadu  w sprawie, jełśi pusta - to własna  , 

                        spr.SAPTomyAkt = "001";
                        // grzywna i koszty oddzielnie

                        if (Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                        {
                            // Utils.LogWriter("Imp grzywny");
                            doc = new Dokument();
                            doc.SAPImportStatus = 0;
                            doc.DocGuid = Guid.NewGuid();
                            doc.KnsPozDzNal = Convert.ToInt32(dtr["pozycja"] == null ? "0" : dtr["pozycja"]);
                            doc.DataDokumentu = dtr["Data dokumentu grzywna"] as DateTime? ?? null;
                            doc2Hash += doc.KnsPozDzNal.ToString();
                            doc2Hash += Convert.ToDateTime(doc.DataDokumentu).ToString("yyyyMMdd");
                            doc.KnsKsiegaDzNal = curKsiega;

                            if (dtr["Data księgowania"] != null)
                            {
                                doc.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]);
                                doc.KnsRokDzNal = doc.DataKsiegowania.Value.Year;
                            }
                            if (knsks.czyFPP == 1)
                                doc.OperacjaGlowna = "FPP0";
                            else if (knsks.czyFPP == 2)
                                doc.OperacjaGlowna = "N033";
                            else
                                doc.OperacjaGlowna = "N010";

                            /*
                            mn.Rodzaj_dokumentu = dtr["Rodzaj dokumentu"].ToString();
                            mn.Waluta = dtr["Waluta"].ToString();
                            mn.Klucz_uzgodnienia = dtr["Klucz uzgodnienia"].ToString();
                            mn.Jednostaka_gospodarca_własna = mySad
                            */

                            if (!String.IsNullOrEmpty(dtr["Czysamoistna"].ToString()))
                            {
                                doc.grzSamoistna = (dtr["Czysamoistna"]).ToString();
                                spr.grzSamoistna = (dtr["Czysamoistna"]).ToString();
                            }
                            else
                            {
                                doc.grzSamoistna = "";
                                spr.grzSamoistna = "";
                            }

                          
                            if (typImport == 6)
                            {
                                if (doc.OperacjaGlowna == "FPP0")
                                {
                                    //doc.OperacjaCzesciowa =  (Konfig.typKns == 2) ? "0010" : "0011";
                                    doc.OperacjaCzesciowa = "0012";
                                }
                                else
                                {
                                    if (repertorium.ToUpper() == "W")
                                        // wykroczenie
                                        doc.OperacjaCzesciowa = "0060";
                                    else
                                        doc.OperacjaCzesciowa = "0030";
                                }
                            }
                            else
                            {

                                if (knsks.czyFPP == 1)
                                    doc.OperacjaCzesciowa = (Konfig.typKns == 2) ? "0010" : "0011";
                                else if (knsks.czyFPP == 2)
                                    doc.OperacjaCzesciowa = "0001";
                                else
                                    switch (spr.SAPRodzajPrzedmiotuUmowy)
                                    {
                                        case "SPPR":
                                        case "SROD":
                                        case "SUBE":
                                        case "SRES":
                                        case "SGOS":
                                        case "SCYW":
                                            if (dl.FizPraw == "X")   // osoba prawna
                                                doc.OperacjaCzesciowa = "0090";  // lub "0100"  ??
                                            else
                                                doc.OperacjaCzesciowa = "0010";

                                            break;


                                        case "SKAR":
                                            if (dl.FizPraw == "X")   // osoba prawna
                                                doc.OperacjaCzesciowa = "0090";
                                            else // osoba fizyczna  sprawdzić  czy wykroczenia i czy samoistna
                                            {
                                                if (doc.grzSamoistna == "s")
                                                {
                                                    if (repertorium.ToUpper() == "W")
                                                        // wykroczenie
                                                        doc.OperacjaCzesciowa = "0070";
                                                    else
                                                        doc.OperacjaCzesciowa = "0040";

                                                }
                                                else
                                                {
                                                    if (repertorium.ToUpper() == "W")
                                                        // wykroczenie
                                                        doc.OperacjaCzesciowa = "0050";
                                                    else
                                                        doc.OperacjaCzesciowa = "0020";
                                                }
                                            }
                                            break;
                                        default:
                                            errmsg += " ; " + "Brak oznaczenia operacji cześciowej (grzywna) ";
                                            break;
                                    }

                            }
                            // Utils.LogWriter("Imp dat wymag.");
                            doc.DataPlatnosci = dtr["Data wymagalności"] as DateTime? ?? null;   // sprawdzić przy kposztach
                            doc.kwota = Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                            doc2Hash += doc.kwota.ToString();
                            
                            if (!String.IsNullOrEmpty(dtr["Kara zastępcza"].ToString()))
                                doc.Stan = "F";    // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                            else if (!String.IsNullOrEmpty(dtr["Egzekucja grzywny"].ToString()))
                                doc.Stan = "C";
                            else if (!String.IsNullOrEmpty(dtr["Grzywny odroczone"].ToString()))
                                doc.Stan = "D";
                            else if (!String.IsNullOrEmpty(dtr["Raty grzywna"].ToString()))
                                doc.Stan = "B";
                            else doc.Stan = "A";
                            doc.typFakt = "GP";
                            // Utils.LogWriter("Imp daty księgowania");
                            if (doc.DataPlatnosci < doc.DataKsiegowania)
                                doc.DataPlatnosci = doc.DataKsiegowania;
                            if (doc.Stan == "C" || doc.Stan == "F" && doc.DataPlatnosci > DateTime.Today)
                                doc.DataPlatnosci = doc.DataKsiegowania;

                            if (doc.DataPlatnosci == new DateTime(2099, 12, 31))
                            {
                                if (Konfig.typDatyPlatn == 1)
                                    doc.DataPlatnosci = new DateTime(doc.DataKsiegowania.Value.Year, 12, 31);
                                else
                                    doc.DataPlatnosci = Konfig.dplatnosci.Value;


                            }
                            // Utils.LogWriter("Koniec impportu grzywny");
                            doc.Info = (String.IsNullOrEmpty(errmsg) ? null : errmsg);
                            if (!String.IsNullOrEmpty(errmsg)) errorStatus = true;
                        }
                        if (Convert.ToDecimal(dtr["koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                        {
                            dock = new Dokument();
                            dock.DocGuid = Guid.NewGuid();
                            dock.KnsPozDzNal = Convert.ToInt32(dtr["pozycja"] == null ? "0" : dtr["pozycja"]);
                            dock2Hash += dock.KnsPozDzNal.ToString();
                            dock.SAPImportStatus = 0;
                            dock.DataDokumentu = dtr["Data dokumentu koszty"] as DateTime? ?? null;
                            dock2Hash += Convert.ToDateTime(dock.DataDokumentu).ToString("yyyyMMdd");
                            dock.KnsKsiegaDzNal = curKsiega;
                            if (dtr["Data księgowania"] != null)
                            {
                                dock.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]);
                                dock.KnsRokDzNal = dock.DataKsiegowania.Value.Year;
                            }
                            if (knsks.czyFPP == 1)
                                dock.OperacjaGlowna = "FPP0";
                            else if (knsks.czyFPP == 2)
                                dock.OperacjaGlowna = "N033";
                            else
                            {
                                if (dock.DataKsiegowania.Value.Year >= 2017)
                                    dock.OperacjaGlowna = "N011";
                                else
                                    dock.OperacjaGlowna = "N010";
                            }
                            /*
                            mn.Rodzaj_dokumentu = dtr["Rodzaj dokumentu"].ToString();
                            mn.Waluta = dtr["Waluta"].ToString();
                            mn.Klucz_uzgodnienia = dtr["Klucz uzgodnienia"].ToString();
                            mn.Jednostaka_gospodarca_własna = mySad
                            */

                            dock.grzSamoistna = "";
                            if (knsks.czyFPP == 1)
                                dock.OperacjaCzesciowa = (Konfig.typKns == 2) ? "0011" : "0010";
                            else if (knsks.czyFPP == 2)
                                dock.OperacjaCzesciowa = "0001";
                            else
                                switch (spr.SAPRodzajPrzedmiotuUmowy)
                                {
                                    case "SROD":
                                        dock.OperacjaCzesciowa = "0120";
                                        break;
                                    case "SPPR":
                                    case "SUBE":
                                    case "SGOS":
                                    case "SRES":
                                    case "SCYW":
                                        if (dl.FizPraw == "X")   // osoba prawna
                                            dock.OperacjaCzesciowa = "0110";  // brak pozycji w słowniku.
                                        else
                                            dock.OperacjaCzesciowa = "0110";
                                        break;


                                    case "SKAR":
                                        dock.OperacjaCzesciowa = "0130";
                                        break;
                                    default:
                                        errmsg += " ; " + "Brak oznaczenia operacji cześciowej (koszty) ";
                                        break;
                                }
                            dock.DataPlatnosci = dtr["Data wymagalności"] as DateTime? ?? null;   // sprawdzić przy kposztach
                            dock.kwota = Convert.ToDecimal(dtr["koszty"].ToString().Replace(".",","),CultureInfo.GetCultureInfo("pl-PL"));
                            dock2Hash += dock.kwota.ToString();

                            if (!String.IsNullOrEmpty(dtr["Egzekucja koszty"].ToString()))
                                dock.Stan = "C";
                            else if (!String.IsNullOrEmpty(dtr["Koszty odroczone"].ToString()))
                                dock.Stan = "D";
                            else if (!String.IsNullOrEmpty(dtr["Raty koszty"].ToString()))
                                dock.Stan = "B";
                            else dock.Stan = "A";
                            if (dock.DataPlatnosci < dock.DataKsiegowania)
                                dock.DataPlatnosci = dock.DataKsiegowania;
                            if (dock.Stan == "C" || dock.Stan == "F" && dock.DataPlatnosci > DateTime.Today)
                                dock.DataPlatnosci = dock.DataKsiegowania;

                            if (dock.DataPlatnosci == new DateTime(2099, 12, 31))
                            {
                                if (Konfig.typDatyPlatn == 1)
                                    dock.DataPlatnosci = new DateTime(dock.DataKsiegowania.Value.Year, 12, 31);
                                else
                                    dock.DataPlatnosci = Konfig.dplatnosci.Value;


                            }

                            dock.typFakt = "KP";
                            dock.Info = (String.IsNullOrEmpty(errmsg) ? null : errmsg);
                            if (!String.IsNullOrEmpty(errmsg)) errorStatus = true;
                        }
                        if (refreshMode)
                        {
                            this.refreshPosition(spr, dl);
                            this.Context.SaveChanges();
                            
                            continue;
                        }
                        {
                            if (!refreshMode)
                            {
                                Transfer tt = miesPackHlp.setTransfer(curKsiega, (doc != null ? doc.DataKsiegowania.Value : dock.DataKsiegowania.Value), this.typImport); // dla przypiosu
                                if (tt != null)
                                {
                                    trans = tt;
                                    this.CurrentTransfer = trans;
                                }
                             

                            }


                        }// obsługa transferów miesiecznych 

                        spr.Dluznik.Add(dl);
                        if (doc != null)
                        {
                            string outmsg;
                            int ans;
                            doc.InsertedBy = UserInfo.Username;
                            doc.InsDate = DateTime.Now;
                            doc.SrcDocumentHash = Utils.HashFromString(doc2Hash);
                            // Sprawdzenie czy dokument instnieje
                            // Utils.LogWriter("Sprawdzenie czy dokument istnieje");
                            if ((ans = docExists(doc, (spr ==null ? "": spr.Karta)  ,out outmsg)) <= 0)
                            {
                                if (ans < 0)
                                    doc.Info = doc.Info + (doc.Info == null ? "" : ";" + doc.Info);

                                this.updateTrasDates(trans, Convert.ToDateTime(doc.DataKsiegowania));
                                spr.Dokument.Add(doc);
                                dl.Dokument.Add(doc);
                               
                                trans.Kwota += doc.kwota;
                                trans.Dokument.Add(doc);
                                // Utils.LogWriter("Dodano dokument");
                            }
                            else ;
                               // MessageBox.Show(" Przypis " + spr.Karta + "  już istnieje w bazie  Komunikat " + outmsg + " informacja  techniczna: hash dokumentu = " + doc.SrcDocumentHash.ToString());

                        }
                        if (dock != null)
                        {
                            string outmsg;
                            int ans;
                            dock.InsertedBy = UserInfo.Username;
                            dock.InsDate = DateTime.Now;
                            dock.SrcDocumentHash = Utils.HashFromString(dock2Hash);

                            if ((ans = docExists(dock, (spr == null ? "" : spr.Karta) , out outmsg)) <= 0)
                            {
                                if (ans < 0)
                                    dock.Info = dock.Info + (dock.Info == null ? "" : ";" + dock.Info);
                                this.updateTrasDates(trans, Convert.ToDateTime(dock.DataKsiegowania));
                                spr.Dokument.Add(dock);
                                dl.Dokument.Add(dock);
                                trans.Dokument.Add(dock);
                               
                                trans.Kwota += dock.kwota;
                            }
                            else ;
                                //MessageBox.Show(" Przypis " + spr.Karta + "już istnieje w bazie  Komunikat " + outmsg + " informacja  techniczna: hash dokumentu = " + dock.SrcDocumentHash.ToString());


                        }
                      
                        trans.LFaktow = trans.Dokument.Count;
                        this.ImportedDocs = trans.Dokument.Count;
                            if (trans.LFaktow > 0 && trans.EntityState == EntityState.Detached)
                                Context.Transfer.AddObject(trans);
                            Context.SaveChanges();
                        /*
                        if (--i == 0)
                        {
                            Context.SaveChanges();
                            i = counter;
                            loopcount++;
                        }
                        */


                    }
                    if (trans.Dokument.Count == 0)
                        errorStatus = true;
                }
            }
            catch (Exception ex)
            {
                //string s = CustomExtensions.ToTraceString(Context);
                errorStatus = true;
                string msg = "Błąd ";
                // Print error message
                if (currentdtr != null)
                    if (currentdtr["Oznaczenie konta umowy"] != null) msg += currentdtr["Oznaczenie konta umowy"].ToString().Trim() + " ";

                if (RunMode.silentMode)
                    Utils.LogWriter(msg + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
                else
                    MessageBox.Show(msg + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if ( con != null)
                if (con.State == ConnectionState.Open)
                    con.Close();
                //Context.SaveChanges();
                
                breakIndicator = true;
            }
        }

        public void ImportTerminWymag()
        {

            SqlDataReader rdr = null;
            SqlConnection con = null;
            const int counter = 200;
            int i;
            int loopcount = 0;
            Sprawa spr;
            Dluznik dl;
            KnsKsiegi knsks;
            string wydzialSekcja;
            string repertorium;
            string ksiega;
            int numer, rok;
            string errmsg;
            List<string> rList = new List<string>();
            List<string> orygrList = new List<string>();
            int IdSaduOrzek;
            SAPSad mySad;
            string typSad;
            SAPRodzajSprawy rodzajSpr;
            Dokument doc;
            Dokument dock;
            KnsSad SadOrzekKns;
            SAPRepertorium repertorzek;
            string oryginRep;
            SqlCommand storedProcCommand = null;
            string doc2Hash = String.Empty;
            string dock2Hash = String.Empty;
            Transfer trans;
            DateTime dFirst;
            DateTime dLast;
            DataTable dt = new DataTable();
            DataRow currentdtr = null;
            bool change_done; 
            //  Thread th = new Thread(progressWindow);
            // th.Start();

            try
            {
                // Open connection to the database
                errorStatus = false;
                i = counter;
                ImportedDocs = 0;
                mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                if (mySad == null)
                {
                    MessageBox.Show("Ustaw własny sąd w konfiguracji");
                    return;
                }



                // przygotowanie listy repertoriów
                foreach (SAPRepertorium srep in Context.SAPRepertorium.ToList())
                {
                    string s;
                    string s1;
                    s = srep.kod.Trim().ToUpper();
                    rList.Add(s);
                    s1 = srep.kod.Trim();
                    orygrList.Add(s1);

                }

                
                    string ConnectionString = Utils.BuildMyConnectionString(Context);
                    con = new SqlConnection(ConnectionString);
                    con.Open();
                                      switch (Konfig.typKns)
                        {
                            case 0: // currenda
                                storedProcCommand = new SqlCommand("sp_PrzypisyCR", con);
                                break;
                            case 1: // Zeto
                                storedProcCommand = new SqlCommand("sp_Przypisy", con);
                                break;
                            case 2: // Zeto
                                storedProcCommand = new SqlCommand("sp_PrzypisyOR", con);
                                break;
                            case 3: // Zeto
                                storedProcCommand = new SqlCommand("sp_PrzypisyAL", con);
                                break;
                            default:
                                break;
                        }

                    storedProcCommand.CommandType = CommandType.StoredProcedure;
                    string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                    storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                    //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                    storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                    storedProcCommand.Parameters.Add("@dataDo", theday);
                    storedProcCommand.Parameters.Add("@dataOd", data_od);
                    storedProcCommand.Parameters.Add("@sprList", "");
                    storedProcCommand.Connection = con;
                    storedProcCommand.CommandTimeout = 600;
                    progressMsg = "Odczyt danych...";

                    rdr = storedProcCommand.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(rdr);

                    }
                





                ////////
                // p[obranie własnego sądu


                if (dt == null)
                {
                    if (RunMode.silentMode)
                        Utils.LogWriter("Brak danych do importu");
                    else
                        MessageBox.Show("Brak danych do importu");
                    errorStatus = true;
                    return;


                }
                else
                {
                    trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = this.typImport; // Daty 
                    trans.DataOd = data_od;
                    trans.DataDo = theday;   // doccelowo podać datę 
                    trans.Uwagi = uwagi;
                    dFirst = DateTime.MaxValue;
                    dLast = DateTime.MinValue;
                    this.CurrentTransfer = trans;


                    loopcount = 0;
                    // setup 


                    

                    foreach (DataRow dtr in dt.Rows)
                    {
                        currentdtr = dtr;
                        if (breakIndicator == true) break;
                        int curKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        if (!RunMode.silentMode) // jeśli w trybie cichym to ze wszystkich ksiąg 
                        {
                            if (!KsiegiKnsLst.Contains(curKsiega)) continue;
                        }
                        progressMsg = "Dokument " + (++loopcount).ToString();

                        spr = new Sprawa();

                        spr.KnsSprawa_id = Convert.ToInt32(dtr["Sprawa_id"]);
                        spr.KnsKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        spr.Karta = dtr["Oznaczenie konta umowy"].ToString().Trim();  // karta dłużnika
                        spr.Sygnatura = dtr["Sygnatura"].ToString().Trim();
                        // sprawdzamy czy mamy już taką sprawę
                        {
                            List<Sprawa> lstspr = this.Context.Sprawa.Where(a => a.KnsSprawa_id == spr.KnsSprawa_id && a.SAPKontoUmowy != null && a.SAPPrzedmiotUmowy != null && a.SAPTypKontaUmowy == "KN").OrderByDescending(a => a.Id).ToList();

                            foreach (Sprawa sprx in lstspr)
                            {

                                if (sprx != null)
                                {
                                    spr.SAPKontoUmowy = sprx.SAPKontoUmowy;
                                    spr.SAPPrzedmiotUmowy = sprx.SAPPrzedmiotUmowy;
                                    
                                    // znajdź dokumenty
                                    List<Dokument> doklst = this.Context.Dokument.Where(b => b.Sprawa_Id == sprx.Id && b.SAPDocId != null).ToList();
                                    doc = new Dokument();
                                    dock = new Dokument();
                                    if (Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                                    {
                                        
                                        doc.DataPlatnosci = dtr["Data wymagalności"] as DateTime? ?? null;   // sprawdzić przy kposztach
                                        doc.kwota = Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                                        if (dtr["Data księgowania"] != null)
                                        {
                                            doc.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]).Date;

                                        }
                                        if (!String.IsNullOrEmpty(dtr["Kara zastępcza"].ToString()))
                                            doc.Stan = "F";    // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                                        else if (!String.IsNullOrEmpty(dtr["Egzekucja grzywny"].ToString()))
                                            doc.Stan = "C";
                                        else if (!String.IsNullOrEmpty(dtr["Grzywny odroczone"].ToString()))
                                            doc.Stan = "D";
                                        else if (!String.IsNullOrEmpty(dtr["Raty grzywna"].ToString()))
                                            doc.Stan = "B";
                                        else doc.Stan = "A";
                                        if (doc.Stan == "B") doc.DataPlatnosci = new DateTime(2099, 12, 31);
                                        if (doc.Stan == "F" || doc.Stan == "C")
                                            if (doc.DataPlatnosci > DateTime.Today)
                                                doc.DataPlatnosci = doc.DataKsiegowania;
                                            // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                                        if (doc.DataPlatnosci == new DateTime(2099, 12, 31))
                                        {
                                            if (Konfig.typDatyPlatn == 1)
                                                doc.DataPlatnosci = new DateTime(doc.DataKsiegowania.Value.Year, 12, 31);
                                            else
                                                doc.DataPlatnosci = Konfig.dplatnosci.Value;


                                        }

                                    }


                                    if (Convert.ToDecimal(dtr["koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                                    {
                                        
                                        dock.kwota = Convert.ToDecimal(dtr["koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                                        dock.DataPlatnosci = dtr["Data wymagalności"] as DateTime? ?? null;   // sprawdzić przy kposztach
                                        if (dtr["Data księgowania"] != null)
                                        {
                                            dock.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]).Date;

                                        }
                                        if (!String.IsNullOrEmpty(dtr["Kara zastępcza"].ToString()))
                                            dock.Stan = "F";    // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                                        else if (!String.IsNullOrEmpty(dtr["Egzekucja grzywny"].ToString()))
                                            dock.Stan = "C";
                                        else if (!String.IsNullOrEmpty(dtr["Grzywny odroczone"].ToString()))
                                            dock.Stan = "D";
                                        else if (!String.IsNullOrEmpty(dtr["Raty grzywna"].ToString()))
                                            dock.Stan = "B";
                                        else dock.Stan = "A";
                                        if (dock.Stan == "B") dock.DataPlatnosci = new DateTime(2099, 12, 31);
                                        if (dock.Stan == "F" || dock.Stan == "C")
                                            if (dock.DataPlatnosci > DateTime.Today)
                                                dock.DataPlatnosci = dock.DataKsiegowania;
                                        // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                                        if (dock.DataPlatnosci == new DateTime(2099, 12, 31))
                                        {
                                            if (Konfig.typDatyPlatn == 1)
                                                dock.DataPlatnosci = new DateTime(dock.DataKsiegowania.Value.Year, 12, 31);
                                            else
                                                dock.DataPlatnosci = Konfig.dplatnosci.Value;
                                        
                                        
                                        }

                                    }
                                    change_done = false;
                                    if (doklst != null && doklst.Count > 0)
                                        foreach (Dokument d in doklst)
                                        {
                                            change_done = false;
                                       //    MessageBox.Show("Dok Id SAP " + d.SAPDocId + " id dokum = " + d.id.ToString());
                                      //      MessageBox.Show("Data 1  " + d.DataKsiegowania.ToString());
                                            d.DataKsiegowania = Convert.ToDateTime(d.DataKsiegowania).Date;
                                        //    MessageBox.Show("Data  2 " + d.DataKsiegowania.ToString());
                                            if (d.typFakt == "GP")
                                            {
                                                
                                                if (doc != null && doc.kwota == d.kwota)
                                                {
                                                    if (d.DataPlatnosci != doc.DataPlatnosci)
                                                    {
                                                        
                                                        d.DataPlatnosci = doc.DataPlatnosci;
                                                        change_done = true;
                                                    }
                                                }

                                            }
                                            else if (d.typFakt == "KP")
                                            {
                                                
                                                if (dock != null && dock.kwota == d.kwota)
                                                {
                                                    if (d.DataPlatnosci != dock.DataPlatnosci)
                                                    {
                                                        d.DataPlatnosci = dock.DataPlatnosci;
                                                        change_done = true;
                                                    }
                                                }

                                            }
                                            else if (d.typFakt == "KS")
                                            {
                                                
                                                if (dock != null && dock.kwota > 0 && d.kwota > 0)
                                                {
                                                    if (d.DataPlatnosci != dock.DataPlatnosci)
                                                    {
                                                        d.DataPlatnosci = dock.DataPlatnosci;
                                                        change_done = true;
                                                    }
                                                }

                                            }
                                            else if (d.typFakt == "GS")
                                            {
                                                
                                                if (doc != null && doc.kwota > 0 && d.kwota > 0)
                                                {
                                                    if (d.DataPlatnosci != doc.DataPlatnosci)
                                                    {
                                                        d.DataPlatnosci = doc.DataPlatnosci;
                                                        change_done = true;
                                                    }
                                                }

                                            }
                                            if (change_done == true)
                                            {

                                                // wywołanie usługi sieciowe
                                                if (d == null || (d != null && (d.DataPlatnosci.Value.Year > 2099 || d.DataPlatnosci.Value.Year < 2010)))
                                                {
                                                    MessageBox.Show(" Błędna data płatności dla karty " + currentdtr["Oznaczenie konta umowy"].ToString().Trim() + " data " + (d.DataPlatnosci == null ? "null" : d.DataPlatnosci.Value.ToString()));
                                                    continue;

                                                }
                                                
                                                DocumentUpdateResponse ansZm = ZSRKRequestHelper.ZmienTerminWymagalnosci(d.SAPDocId, Convert.ToDateTime(d.DataPlatnosci).ToString("yyyyMMdd"));
                                                if (ansZm != null)
                                                {
                                                    if (ansZm.Komunikaty != null && ansZm.Komunikaty.GetUpperBound(0) >= 0 && ansZm.Komunikaty[0].RodzajKomunikatu != "E")
                                                       ; 
                                                    else
                                                    {
                                                        MessageBox.Show("Błąd podczas próby zmiany Daty płatności dla dokumentu" + d.SAPDocId + " Kdł " + spr.Karta + " ;" + spr.Sygnatura + " " + ansZm.Komunikaty[0].Komunikat1);
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Błąd komunikacji z usługą sieciową. Sprawdź paramatery połączenia ");
                                                    return;
                                                }


                                                this.Context.SaveChanges();
                                            }
                                        }
                                }
                            }

                        }



                    }
                    
                    
                }
                MessageBox.Show("Synchronizacja terminów płatnosci zakończona");
            }
            catch (Exception ex)
            {
                //string s = CustomExtensions.ToTraceString(Context);
                errorStatus = true;
                string msg = "Błąd ";
                // Print error message
                if (currentdtr != null)
                    if (currentdtr["Oznaczenie konta umowy"] != null) msg += currentdtr["Oznaczenie konta umowy"].ToString().Trim() + " ";

                if (RunMode.silentMode)
                    Utils.LogWriter(msg + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
                else
                    MessageBox.Show(msg + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con != null)
                    if (con.State == ConnectionState.Open)
                        con.Close();
                //Context.SaveChanges();

                breakIndicator = true;
            }
        }

        public void ImportStanyNal()
        {

            SqlDataReader rdr = null;
            SqlConnection con = null;
            const int counter = 200;
            int i;
            int loopcount = 0;
            Sprawa spr;
            Dluznik dl;
            KnsKsiegi knsks;
            string wydzialSekcja;
            string repertorium;
            string ksiega;
            int numer, rok;
            string errmsg;
            List<string> rList = new List<string>();
            List<string> orygrList = new List<string>();
            int IdSaduOrzek;
            SAPSad mySad;
            string typSad;
            SAPRodzajSprawy rodzajSpr;
            Dokument doc;
            Dokument dock;
            KnsSad SadOrzekKns;
            SAPRepertorium repertorzek;
            string oryginRep;
            SqlCommand storedProcCommand = null;
            string doc2Hash = String.Empty;
            string dock2Hash = String.Empty;
            Transfer trans;
            DateTime dFirst;
            DateTime dLast;
            DataTable dt = new DataTable();
            DataRow currentdtr = null;
            bool change_done;
            //  Thread th = new Thread(progressWindow);
            // th.Start();

            try
            {
                // Open connection to the database
                errorStatus = false;
                i = counter;
                ImportedDocs = 0;
                mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                if (mySad == null)
                {
                    MessageBox.Show("Ustaw własny sąd w konfiguracji");
                    return;
                }

           

                // przygotowanie listy repertoriów
                foreach (SAPRepertorium srep in Context.SAPRepertorium.ToList())
                {
                    string s;
                    string s1;
                    s = srep.kod.Trim().ToUpper();
                    rList.Add(s);
                    s1 = srep.kod.Trim();
                    orygrList.Add(s1);

                }


                string ConnectionString = Utils.BuildMyConnectionString(Context);
                con = new SqlConnection(ConnectionString);
                con.Open();



                //string ConnectionString = Utils.BuildMyConnectionString(Context);
                switch (Konfig.typKns)
                {
                    case 0: // currenda
                        storedProcCommand = new SqlCommand("sp_DziennikNaleznosciCR", con);
                        break;
                    case 1: // Zeto
                        storedProcCommand = new SqlCommand("sp_DziennikNaleznosci", con);
                        break;
                    case 2: // Orcom
                        storedProcCommand = new SqlCommand("sp_DziennikNaleznosciOR", con);
                        break;
                    case 3: // Albit
                        storedProcCommand = new SqlCommand("sp_DziennikNaleznosciAL", con);
                        break;

                    default:
                        break;
                }

                storedProcCommand.CommandType = CommandType.StoredProcedure;
                string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                storedProcCommand.Parameters.Add("@dzien", theday);
                storedProcCommand.Connection = con;
                storedProcCommand.CommandTimeout = 600;
                progressMsg = "Odczyt danych...";
                rdr = storedProcCommand.ExecuteReader();

                if (rdr.HasRows)
                {
                    dt = new DataTable();
                    dt.Load(rdr);

                }






                ////////
                // p[obranie własnego sądu


                if (dt == null)
                {
                    if (RunMode.silentMode)
                        Utils.LogWriter("Brak danych do importu");
                    else
                        MessageBox.Show("Brak danych do importu");
                    errorStatus = true;
                    return;


                }
                else
                {
                    trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = this.typImport; // Daty 
                    trans.DataOd = data_od;
                    trans.DataDo = theday;   // doccelowo podać datę 
                    trans.Uwagi = uwagi;
                    dFirst = DateTime.MaxValue;
                    dLast = DateTime.MinValue;
                    this.CurrentTransfer = trans;


                    loopcount = 0;
                    // setup 




                    foreach (DataRow dtr in dt.Rows)
                    {
                        currentdtr = dtr;
                        if (breakIndicator == true) break;
                        int curKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        if (!RunMode.silentMode) // jeśli w trybie cichym to ze wszystkich ksiąg 
                        {
                            if (!KsiegiKnsLst.Contains(curKsiega)) continue;
                        }
                        progressMsg = "Dokument " + (++loopcount).ToString();

                        spr = new Sprawa();

                        spr.KnsSprawa_id = Convert.ToInt32(dtr["Sprawa_id"]);
                        spr.KnsKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        spr.Karta = dtr["Oznaczenie konta umowy"].ToString().Trim();  // karta dłużnika
                        spr.Sygnatura = dtr["Sygnatura"].ToString().Trim();
                        // sprawdzamy czy mamy już taką sprawę
                        {
                            List<Sprawa> lstspr = this.Context.Sprawa.Where(a => a.KnsSprawa_id == spr.KnsSprawa_id && a.SAPKontoUmowy != null && a.SAPPrzedmiotUmowy != null && a.SAPTypKontaUmowy == "KN").OrderByDescending(a => a.Id).ToList();

                            foreach (Sprawa sprx in lstspr)
                            {

                                if (sprx != null)
                                {
                                    spr.SAPKontoUmowy = sprx.SAPKontoUmowy;
                                    spr.SAPPrzedmiotUmowy = sprx.SAPPrzedmiotUmowy;

                                    // znajdź dokumenty
                                    List<Dokument> doklst = this.Context.Dokument.Where(b => b.Sprawa_Id == sprx.Id && b.SAPDocId != null).ToList();
                                    doc = new Dokument();
                                    dock = new Dokument();
                                    if (Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                                    {

                                      
                                        if (!String.IsNullOrEmpty(dtr["Kara zastępcza"].ToString()))
                                            doc.Stan = "F";    // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                                        else if (!String.IsNullOrEmpty(dtr["Egzekucja grzywny"].ToString()))
                                            doc.Stan = "C";
                                        else if (!String.IsNullOrEmpty(dtr["Grzywny odroczone"].ToString()))
                                            doc.Stan = "D";
                                        else if (!String.IsNullOrEmpty(dtr["Raty grzywna"].ToString()))
                                            doc.Stan = "B";
                                        else doc.Stan = "A";
                                        if (doc.Stan == "B") doc.DataPlatnosci = new DateTime(2099, 12, 31);
                                     
                                   

                                    }


                                    if (Convert.ToDecimal(dtr["koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                                    {

                                       
                                        if (!String.IsNullOrEmpty(dtr["Kara zastępcza"].ToString()))
                                            dock.Stan = "F";    // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                                        else if (!String.IsNullOrEmpty(dtr["Egzekucja grzywny"].ToString()))
                                            dock.Stan = "C";
                                        else if (!String.IsNullOrEmpty(dtr["Grzywny odroczone"].ToString()))
                                            dock.Stan = "D";
                                        else if (!String.IsNullOrEmpty(dtr["Raty grzywna"].ToString()))
                                            dock.Stan = "B";
                                        else dock.Stan = "A";
                                        if (dock.Stan == "B") dock.DataPlatnosci = new DateTime(2099, 12, 31);
                                        
                                        // kara zastępcza , odpisanie grzywny nie jest tożsame z karą zastępczą.
                                        

                                    }
                                    change_done = false;
                                    if (doklst != null && doklst.Count > 0)
                                        foreach (Dokument d in doklst)
                                        {
                                            change_done = false;
                                            //    MessageBox.Show("Dok Id SAP " + d.SAPDocId + " id dokum = " + d.id.ToString());
                                            //      MessageBox.Show("Data 1  " + d.DataKsiegowania.ToString());
                                            d.DataKsiegowania = Convert.ToDateTime(d.DataKsiegowania).Date;
                                            //    MessageBox.Show("Data  2 " + d.DataKsiegowania.ToString());
                                            if (d.typFakt == "GP")
                                            {

                                               
                                                    if (d.Stan != doc.Stan)
                                                    {

                                                        d.Stan = doc.Stan;
                                                        change_done = true;
                                                    }
                                               
                                            }
                                            else if (d.typFakt == "KP")
                                            {

                                              
                                                    if (d.Stan != dock.Stan)
                                                    {
                                                        d.Stan = dock.Stan;
                                                        change_done = true;
                                                    }
                                               

                                            }
                                            else if (d.typFakt == "KS")
                                            {

                                              
                                                    if (d.Stan != dock.Stan)
                                                    {
                                                        d.Stan = dock.Stan;
                                                        change_done = true;
                                                    }
                                              

                                            }
                                            else if (d.typFakt == "GS")
                                            {

                                                    if (d.Stan != doc.Stan)
                                                    {
                                                        d.Stan = doc.Stan;
                                                        change_done = true;
                                                    }
                                               

                                            }
                                            if (change_done == true)
                                            {

                                                DocumentDebtStateUpdateResponse ansZm = ZSRKRequestHelper.ZmienStanNaleznosci(d.SAPDocId, theday.ToString("yyyyMMdd"), d.Stan);
                                                if (ansZm != null)
                                                {
                                                    if (ansZm.Komunikaty != null && ansZm.Komunikaty.GetUpperBound(0) >= 0 && ansZm.Komunikaty[0].RodzajKomunikatu != "E")
                                                        ;
                                                    else
                                                    {
                                                        MessageBox.Show("Błąd podczas próby zmiany Stanu należności, dokument:  " + d.SAPDocId + " Kdł " + spr.Karta + " ;" + spr.Sygnatura + " " + ansZm.Komunikaty[0].Komunikat1);
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Błąd komunikacji z usługą sieciową. Sprawdź paramatery połączenia ");
                                                    return;
                                                }


                                                this.Context.SaveChanges();
                                            }
                                        }
                                }
                            }

                        }



                    }


                }
                MessageBox.Show("Synchronizacja stanów należności zakończona");
            }
            catch (Exception ex)
            {
                //string s = CustomExtensions.ToTraceString(Context);
                errorStatus = true;
                string msg = "Błąd ";
                // Print error message
                if (currentdtr != null)
                    if (currentdtr["Oznaczenie konta umowy"] != null) msg += currentdtr["Oznaczenie konta umowy"].ToString().Trim() + " ";

                if (RunMode.silentMode)
                    Utils.LogWriter(msg + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
                else
                    MessageBox.Show(msg + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con != null)
                    if (con.State == ConnectionState.Open)
                        con.Close();
                //Context.SaveChanges();

                breakIndicator = true;
            }
        }
        public void ImportZwrot_3_4()
        {

            SqlDataReader rdr = null;
            SqlConnection con = null;
            const int counter = 200;
            int i;
            int loopcount = 0;
            Sprawa spr;
            Dluznik dl;
            KnsKsiegi knsks;
            string wydzialSekcja;
            string repertorium;
            string ksiega;
            string outSad;
            int numer, rok;
            string errmsg;
            List<string> rList = new List<string>();
            List<string> orygrList = new List<string>();
            int IdSaduOrzek;
            SAPSad mySad;
            string typSad;
            SAPRodzajSprawy rodzajSpr;
            Dokument doc;
            KnsSad SadOrzekKns;
            SAPRepertorium repertorzek;
            string oryginRep;
            SqlCommand storedProcCommand = null;
            string doc2Hash = String.Empty;
            string dock2Hash = String.Empty;
            Transfer trans;
            DateTime dFirst;
            DateTime dLast;
            DataTable dt = new DataTable();
            DataRow currentdtr = null;
            bool czyPH = false;
            bool czyKU = false;
            bool czyPU = false;

 

            //  Thread th = new Thread(progressWindow);
            // th.Start();

            try
            {
                // Open connection to the database
                errorStatus = false;
                i = counter;
                ImportedDocs = 0;
                if (String.IsNullOrWhiteSpace(Konfig.StanowiskoFin))
                    mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                else
                    mySad = (from c in Context.SAPSad where c.kod == Konfig.StanowiskoFin select c).FirstOrDefault();
                //mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                if (mySad == null)
                {
                    MessageBox.Show("Ustaw własny sąd w konfiguracji");
                    return;
                }



                // przygotowanie listy repertoriów
                foreach (SAPRepertorium srep in Context.SAPRepertorium.ToList())
                {
                    string s;
                    string s1;
                    s = srep.kod.Trim().ToUpper();
                    rList.Add(s);
                    s1 = srep.kod.Trim();
                    orygrList.Add(s1);

                }

                {
                    string ConnectionString = Utils.BuildMyConnectionString(Context);
                    con = new SqlConnection(ConnectionString);
                    con.Open();
                        switch (Konfig.typKns)
                        {
                            case 0: // currenda
                                storedProcCommand = new SqlCommand("sp_Zwrot34CR", con);
                                break;
                            case 1: // Zeto
                                storedProcCommand = new SqlCommand("sp_Zwrot34", con);
                                break;
                                   
                            default:
                                break;
                        }
                    
                    storedProcCommand.CommandType = CommandType.StoredProcedure;
                    string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                    storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                    //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                    storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                    storedProcCommand.Parameters.Add("@dataDo", theday);
                    storedProcCommand.Parameters.Add("@dataOd", data_od);
                    storedProcCommand.Connection = con;
                    storedProcCommand.CommandTimeout = 600;
                    progressMsg = "Odczyt danych...";

                    rdr = storedProcCommand.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(rdr);

                    }
                }






                ////////
                // p[obranie własnego sądu


                if (dt == null)
                {
                    if (RunMode.silentMode)
                        Utils.LogWriter("Brak danych do importu");
                    else
                        MessageBox.Show("Brak danych do importu");
                    errorStatus = true;
                    return;


                }
                else
                {
                    trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = this.typImport; // przypisy
                    trans.DataOd = data_od;
                    trans.DataDo = theday;   // doccelowo podać datę 
                    trans.Uwagi = uwagi;
                    dFirst = DateTime.MaxValue;
                    dLast = DateTime.MinValue;
                    this.CurrentTransfer = trans;


                    loopcount = 0;
                    // setup 


                     if (dt.Columns.Contains("Numer partnera handlowego"))   
                            czyPH  = true;
                     if (dt.Columns.Contains("Numer konta umowy"))   
                            czyKU  = true;
                     if (dt.Columns.Contains("Sygnatura sądowa"))   
                            czyPU  = true;
                        

                    foreach (DataRow dtr in dt.Rows)
                    {
                        currentdtr = dtr;
                        if (breakIndicator == true) break;
                        int curKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        if (!RunMode.silentMode) // jeśli w trybie cichym to ze wszystkich ksiąg 
                        {
                            //if (!KsiegiKnsLst.Contains(curKsiega)) continue;
                        }
                        progressMsg = "Dokument " + (++loopcount).ToString();
                        doc2Hash = curKsiega.ToString();
               

                        //(pForm.Controls["lbInfo"] as Label).Refresh();
                        errmsg = "";
                        doc = null;
                      
                        //if (dtr.ItemArray.
                         //   "SapKontoPartnera"
                         //       "SapKontoUmowy"
                         //     SAPPrzedmiotUmowy

                       

                        dl = new Dluznik();
                        if (!String.IsNullOrEmpty(dtr["Osoba fizyczna/Osoba prawna"].ToString().Trim()))
                            dl.FizPraw = dtr["Osoba fizyczna/Osoba prawna"].ToString();
                        else
                            dl.FizPraw = "";
                        
                        if (czyPH)
                            if (dtr["Numer partnera handlowego"] != null)
                                if (!String.IsNullOrWhiteSpace(dtr["Numer partnera handlowego"].ToString()))
                                    dl.SAPKontoPartnera = dtr["Numer partnera handlowego"].ToString().Trim();


                        dl.Imie = dtr["Imię/Nazwa 1"].ToString();
                        dl.Nazwisko = dtr["Nazwisko / Nazwa 2"].ToString();

                        if (dl.FizPraw == "X") // jesli osoba prawna - podziel nazwę 
                        {
                            if (!String.IsNullOrEmpty(dl.Nazwisko))
                            {
                                int spc = dl.Imie.LastIndexOf(' ');
                                if (spc > 0 && dl.Nazwisko[0] != ' ')
                                {
                                    string tmp = dl.Imie.Substring(spc + 1);
                                    if (tmp.Trim().Length > 0)
                                    {
                                        dl.Imie = dl.Imie.Substring(0, spc);
                                        dl.Nazwisko = tmp.Trim() + dl.Nazwisko;

                                    }

                                }
                                dl.Nazwisko = dl.Nazwisko.Trim();



                            }

                        }
                        if (dl.Nazwisko.Length > 40)
                            dl.Nazwisko = dl.Nazwisko.Substring(0, 40);
                        if (dl.Imie.Length > 40)
                            dl.Imie = dl.Imie.Substring(0, 40);


                        dl.Ulica = dtr["Ulica"].ToString();
                        dl.NrDomu = dtr["Nr domu"].ToString();
                        dl.NrMieszkania = dtr["Nr mieszkania"].ToString();
                        dl.Pesel = dtr["Pesel"].ToString().Trim();
                        if (string.IsNullOrEmpty(dl.Pesel))
                            dl.Pesel = null;
                        else
                        {
                            foreach (char c in dl.Pesel.Trim())
                            {
                                if (!Char.IsDigit(c))
                                {
                                    dl.Pesel = null;
                                    break;
                                }
                            }
                        }
                        dl.Nip = cleanNIP(dtr["NIP"].ToString().Trim());

                        if (String.IsNullOrEmpty(dl.NrDomu))
                        {
                            if (dl.Ulica != null)
                            {
                                string s = dl.Ulica.Trim();
                                int ii = s.LastIndexOf(' ');
                                int jj;
                                if (ii > 3 && Int32.TryParse(s[ii + 1].ToString(), out jj))
                                {
                                    dl.Ulica = s.Substring(0, ii).Trim();
                                    dl.NrDomu = s.Substring(ii).Trim();
                                    jj = 0;
                                    jj = dl.NrDomu.IndexOf('/');
                                    if (jj > 0 && jj < dl.NrDomu.Length - 1)
                                    {
                                        dl.NrMieszkania = dl.NrDomu.Substring(jj + 1).Trim();
                                        dl.NrDomu = dl.NrDomu.Substring(0, jj).Trim();
                                    }
                                }

                            }

                        }

                        if (String.IsNullOrEmpty(dl.NrMieszkania) && dl.NrDomu.ToLower().Contains('m'))
                        {
                            // wyj

                            dl.NrMieszkania = dl.NrDomu.Substring(dl.NrDomu.ToLower().IndexOf('m') + 1).Trim();
                            dl.NrDomu = dl.NrDomu.Substring(0, dl.NrDomu.ToLower().IndexOf('m')).Trim();

                        }



                        dl.KodPocztowy = dtr["Kod pocztowy"].ToString();
                        dl.Miejscowosc = dtr["Miejscowość"].ToString();
                        {
                            string kk = dtr["Klucz kraju"].ToString().Trim().ToUpper();
                            if (kk != "PL")
                            {
                                SAPKodKraju kdkr;

                                kdkr = (from m in Context.SAPKodKraju
                                        where m.kraj.ToUpper() == kk
                                        select m).FirstOrDefault();
                                if (kdkr != null)
                                {
                                    dl.KluczKraju = kdkr.kod;
                                    if (!String.IsNullOrWhiteSpace(dl.KodPocztowy))
                                    {
                                        string kod = this.kodFormat(kdkr.kod, dl.KodPocztowy);
                                        if (!String.IsNullOrWhiteSpace(kod))
                                        {
                                            dl.KodPocztowy = kod;
                                        }
                                    }
                                }
                                else
                                {
                                    dl.KluczKraju = "??";
                                    errmsg = "Nieokreślony kod kraju dłużnika";
                                }
                            }
                            else
                                dl.KluczKraju = kk;

                        }

                        dl.Iban = dtr["IBAN"].ToString();
                        dl.RBN = dtr["Kwalifikator do RBN"].ToString();
                        if (string.IsNullOrEmpty(dl.RBN) || string.IsNullOrWhiteSpace(dl.RBN))
                        {
                            if (dl.FizPraw == "X")
                                dl.RBN = "08";
                            else
                                dl.RBN = "09";

                        }
                        doc2Hash += dl.Nazwisko + dl.Imie;


                        spr = new Sprawa();

                        if (czyKU)
                            if (dtr["Numer konta umowy"] != null)
                                if (!String.IsNullOrWhiteSpace(dtr["Numer konta umowy"].ToString()))
                                    spr.SAPKontoUmowy = dtr["Numer konta umowy"].ToString().Trim();
                         if (czyPU)
                             if (dtr["Sygnatura sądowa"] != null)
                                 if (!String.IsNullOrWhiteSpace(dtr["Sygnatura sądowa"].ToString()))
                                     spr.SAPPrzedmiotUmowy = dtr["Sygnatura sądowa"].ToString().Trim();
                        

                        spr.KnsSprawa_id = Convert.ToInt32(dtr["Sprawa_id"]);
                        spr.KnsKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        spr.KnsSad = dtr["SadKns"].ToString().Trim();
                        spr.KNSSadOrzek_id = Convert.ToInt32(dtr["IdSaduOrzek"] == DBNull.Value ? 0 : dtr["IdSaduOrzek"]);
                        spr.Karta = dtr["Oznaczenie konta umowy"].ToString().Trim();  // karta dłużnika
                        if (dtr["Typ konta umowy"] != null && !String.IsNullOrEmpty(dtr["Typ konta umowy"].ToString()))
                        {
                            spr.SAPTypKontaUmowy = dtr["Typ konta umowy"].ToString();
                        }
                        else
                        {

                            spr.SAPTypKontaUmowy = "DO";
                        }
                        // sprawdzamy czy mamy już taką sprawę
                        {
                            Sprawa sprx;
                            sprx = this.Context.Sprawa.Include("Dluznik").Where(a => a.KnsSprawa_id == spr.KnsSprawa_id && a.SAPPrzedmiotUmowy != null).OrderByDescending(a => a.Id).FirstOrDefault();
                            if (sprx != null)
                            {
                                spr.SAPKontoUmowy = sprx.SAPKontoUmowy;
                                spr.SAPPrzedmiotUmowy = sprx.SAPPrzedmiotUmowy;
                                if (sprx.Dluznik != null)
                                {
                                    dl.SAPKontoPartnera = sprx.Dluznik.FirstOrDefault().SAPKontoPartnera;

                                }

                            }
                        }

                        if (dtr["Relacja konta"] != null && !String.IsNullOrEmpty(dtr["Relacja konta"].ToString()))
                            spr.SAPRelacjaKontaUmowy = dtr["Relacja konta"].ToString().Trim();
                        else
                            spr.SAPRelacjaKontaUmowy = "99";

                        // mn.Relacja_konta = dtr["Relacja konta"].ToString();  stał wartość  99
                        //mn.Typ_konta_umowy = dtr["Typ konta umowy"].ToString();  KN, KN1 jeśli w ramach jednej sygnatury wystepuje kilka kart dłuBnika dla tego samego dłuBnika – dla kol;enych kart wartosci K1, K2…, K9
                        knsks = Context.KnsKsiegi.Where(a => a.Id_Ksiegi == spr.KnsKsiega).SingleOrDefault<KnsKsiegi>();
                        // ksiega 
                        if (knsks != null)
                            spr.SAPRodzajPrzedmiotuUmowy = knsks.rodzajPrzedmiotu; // rodzaj przedmiotu umowy na podstawie ksiegi
                        typSad = null;
                       
                            spr.SAPSadId = mySad.kod;
                            typSad = mySad.typSad;

                       
                        //spr.SAPSadId = typSad;  // sad orzekający

                        spr.Sygnatura = dtr["Sygnatura"].ToString().Trim();
                        {
                            string retval = Utils.ParseSygn(spr.Sygnatura, spr.SAPSadId, rList, orygrList, out wydzialSekcja, out repertorium, out numer, out rok, out oryginRep, out outSad);
                            spr.SAPSadId = outSad; 
                            if (retval.Length == 0)
                            {

                                ;

                            }
                            else
                                if (!String.IsNullOrEmpty(spr.SAPSadId) && retval == GlobalStrings.SYGN_IN_SAD)
                                    retval = "";
                                else
                                    errmsg += " ; " + retval;



                            repertorium = repertorium.Trim();
                            spr.Rok = rok;
                            if (repertorium != "")
                                spr.SAPRepertorium = repertorium;
                            spr.SAPWydział = wydzialSekcja.Trim();
                            spr.Numer = numer;
                            // repertorium 2 typsporawy
                            repertorzek = (from e in Context.SAPRepertorium
                                           where e.kod == repertorium
                                           select e).FirstOrDefault();
                            if (repertorzek != null)
                            {
                                spr.SAPRodzajPrzedmiotuUmowy = repertorzek.SymbolRodzajPrzedmiotu;
                               
                            }


                        }
                        rodzajSpr = null;
                        if (typSad == "SF") typSad = "SR";
                        if (repertorium.Length > 0)
                        {
                            rodzajSpr = (from f in Context.SAPRodzajSprawy where f.repertorium == repertorium && f.typSad == typSad orderby f.id select f).FirstOrDefault();
                            if (rodzajSpr != null)
                            {
                                spr.SAPRodzajSprawy = rodzajSpr.kod;

                            }
                        }


                        doc2Hash += spr.Sygnatura;
                        spr.SAPTomyAkt = "001";
                        // grzywna i koszty oddzielnie


                        if (Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) < 0)
                        {
                            doc = new Dokument();
                            doc.SAPImportStatus = 0;
                            doc.DocGuid = Guid.NewGuid();
                            doc.KnsPozDzNal = Convert.ToInt32(dtr["pozycja"] == null ? "0" : dtr["pozycja"]);
                            doc.DataDokumentu = dtr["Data dokumentu grzywna"] as DateTime? ?? null;
                            doc2Hash += doc.KnsPozDzNal.ToString();
                            doc2Hash += Convert.ToDateTime(doc.DataDokumentu).ToString("yyyyMMdd");
                            doc.KnsKsiegaDzNal = curKsiega;

                            if (dtr["Data księgowania"] != null)
                            {
                                doc.DataKsiegowania = Convert.ToDateTime(dtr["Data księgowania"]);
                                doc.KnsRokDzNal = doc.DataKsiegowania.Value.Year;
                            }
                            if (dtr["Operacja główna"] != null && !String.IsNullOrEmpty(dtr["Operacja główna"].ToString()))
                                doc.OperacjaGlowna = dtr["Operacja główna"].ToString();
                            else
                                doc.OperacjaGlowna = "P020";
                            /*
                            mn.Rodzaj_dokumentu = dtr["Rodzaj dokumentu"].ToString();
                            mn.Waluta = dtr["Waluta"].ToString();
                            mn.Klucz_uzgodnienia = dtr["Klucz uzgodnienia"].ToString();
                            mn.Jednostaka_gospodarca_własna = mySad
                            */

                            if (!String.IsNullOrEmpty(dtr["Czysamoistna"].ToString()))
                            {
                                doc.grzSamoistna = (dtr["Czysamoistna"]).ToString();
                                spr.grzSamoistna = (dtr["Czysamoistna"]).ToString();
                            }
                            else
                            {
                                doc.grzSamoistna = "";
                                spr.grzSamoistna = "";
                            }
                            if (dtr["Częściowo grzywna"] != null && !String.IsNullOrEmpty(dtr["Częściowo grzywna"].ToString()))
                                doc.OperacjaCzesciowa = dtr["Częściowo grzywna"].ToString();
                            else
                            {
                                doc.OperacjaCzesciowa = "0040";
                               
                            }
                            if ( dtr["Rodzaj dokumentu"] != null && ! String.IsNullOrEmpty(dtr["Rodzaj dokumentu"].ToString()))
                                 doc.SAPRodzajDokumentu = dtr["Rodzaj dokumentu"].ToString();
                            else
                                doc.SAPRodzajDokumentu = "DN";
                            doc2Hash += doc.OperacjaGlowna;
                            doc2Hash += doc.OperacjaCzesciowa;
                            doc2Hash += doc.SAPRodzajDokumentu;
                            doc.DataPlatnosci = dtr["Data wymagalności"] as DateTime? ?? null;   // sprawdzić przy kposztach
                            doc.kwota = Convert.ToDecimal(dtr["grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                            doc2Hash += doc.kwota.ToString();
                            doc.typFakt = "GP";
                            doc.Info = (String.IsNullOrEmpty(errmsg) ? null : errmsg);
                            if (!String.IsNullOrEmpty(errmsg)) errorStatus = true;
                        }

                       
                        spr.Dluznik.Add(dl);
                        if (doc != null)
                        {
                            string outmsg;
                            int ans;
                            doc.SrcDocumentHash = Utils.HashFromString(doc2Hash);
                            // Sprawdzenie czy dokument instnieje
                            if ((ans = docExists(doc, (spr == null ? "" : spr.Karta), out outmsg)) <= 0)
                            {
                                if (ans < 0)
                                    doc.Info = doc.Info + (doc.Info == null ? "" : ";" + doc.Info);

                                if (doc.DataDokumentu > dLast) dLast = Convert.ToDateTime(doc.DataDokumentu);
                                if (doc.DataDokumentu < dFirst) dFirst = Convert.ToDateTime(doc.DataDokumentu);
                                spr.Dokument.Add(doc);
                                dl.Dokument.Add(doc);
                                trans.Dokument.Add(doc);
                            }
                            else ;
                            // MessageBox.Show(" Przypis " + spr.Karta + "  już istnieje w bazie  Komunikat " + outmsg + " informacja  techniczna: hash dokumentu = " + doc.SrcDocumentHash.ToString());

                        }
                      
                        trans.DataDo = dLast;
                        trans.DataOd = dFirst;
                        trans.LFaktow = trans.Dokument.Count;
                        this.ImportedDocs = trans.Dokument.Count;
                        if (dFirst == DateTime.MaxValue || dLast == DateTime.MinValue)
                        {

                            ;
                        }
                        else
                        {
                            if (trans.EntityState == EntityState.Detached)
                                Context.Transfer.AddObject(trans);
                            Context.SaveChanges();

                        }
                        /*
                        if (--i == 0)
                        {
                            Context.SaveChanges();
                            i = counter;
                            loopcount++;
                        }
                        */


                    }
                    if (trans.Dokument.Count == 0)
                        errorStatus = true;
                }
            }
            catch (Exception ex)
            {
                //string s = CustomExtensions.ToTraceString(Context);
                errorStatus = true;
                string msg = "Błąd ";
                // Print error message
                if (currentdtr != null)
                    if (currentdtr["Oznaczenie konta umowy"] != null) msg += currentdtr["Oznaczenie konta umowy"].ToString().Trim() + " ";

                if (RunMode.silentMode)
                    Utils.LogWriter(msg + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
                else
                    MessageBox.Show(msg + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con != null)
                    if (con.State == ConnectionState.Open)
                        con.Close();
                //Context.SaveChanges();

                breakIndicator = true;
            }
        }

        private Dictionary<string, decimal> findPrzypis(List<Sprawa> lspr, Sprawa spr, int typ, decimal kwt)
        {

            List<Dokument> dx;
            List<Dokument> dokAllTmp;
            List<Dokument> dokAll;
            Dictionary<string, decimal> retValue = new Dictionary<string, decimal>();

            dokAllTmp = new List<Dokument>();
            foreach (Sprawa s in lspr)
            {
                dokAllTmp.AddRange(s.Dokument);

            }


            dokAll = new List<Dokument>();
            foreach (Dokument dd in dokAllTmp)
            {

                if (!String.IsNullOrWhiteSpace(dd.SAPDocId))
                    dokAll.Add(dd);


            }



            // typp : 1 - grzywna ; 0 - koszty

            {
                List<Dokument> tmp = null;

                if (typ == 1) // grzywna
                {

                    tmp = dokAll.Where(a => a.typFakt == "GS" || a.typFakt == "GP").ToList();

                }
                if (typ == 0) // koszty
                {

                    tmp = dokAll.Where(a => a.typFakt == "KS" || a.typFakt == "KP").ToList();

                }
                if (tmp != null && tmp.Count == 1)
                {
                    Dokument df = tmp.FirstOrDefault();
                    retValue.Add(df.SAPDocId, kwt);
                    int id_sprawy = df.Sprawa_Id.Value;
                    spr = lspr.Where(a => a.Id == id_sprawy).FirstOrDefault();
                    return retValue;
                }



            }



            foreach (Dokument d in dokAll)
            {
                if (typ == 1 && (d.typFakt == "GS" || d.typFakt == "GP"))
                {
                    if (!String.IsNullOrWhiteSpace(d.SAPDocId))
                    {
                        // czy jest dokument odposi

                        List<Dokument> lstdok = Context.Dokument.Where(c => c.SAPDocIdRef == d.SAPDocId && c.Sprawa_Id == spr.Id).ToList();
                        if ((lstdok == null || lstdok.Count == 0) && d.kwota == kwt)
                        {

                            retValue.Add(d.SAPDocId, kwt);
                            spr = d.Sprawa;
                            return retValue;

                        }
                    }



                }
                if (typ == 0 && (d.typFakt == "KS" || d.typFakt == "KP"))
                {
                    if (!String.IsNullOrWhiteSpace(d.SAPDocId))
                    {
                        List<Dokument> lstdok = Context.Dokument.Where(c => c.SAPDocIdRef == d.SAPDocId && c.Sprawa_Id == spr.Id).ToList();
                        if ((lstdok == null || lstdok.Count == 0) && d.kwota == kwt)
                        {
                            retValue.Add(d.SAPDocId, kwt);
                            spr = d.Sprawa;
                            return retValue;
                        }
                    }


                }




            }
            // 
            dx = null;
            if (typ == 1)
            {
                dx = dokAll.Where(c => (c.typFakt == "GP" || c.typFakt == "GS") && (c.SAPDocId != null) && (c.OperacjaCzesciowa != "0030" && c.OperacjaCzesciowa != "0060")).OrderByDescending(c => c.id).ToList();




            }
            else if (typ == 0)
            {
                dx = dokAll.Where(c => (c.typFakt == "KP" || c.typFakt == "KS") && (c.SAPDocId != null) && (c.OperacjaCzesciowa != "0030" && c.OperacjaCzesciowa != "0060")).OrderByDescending(c => c.id).ToList();

            }

            if (dx != null)
            {
                List<string> lst = new List<string>();
                bool foundItem;
                decimal sum = 0;
                KeyValuePair<string, decimal> item = new KeyValuePair<string, decimal>();
                foreach (Dokument dy in dx)
                {

                    lst.Add(dy.SAPDocId);

                }
                if (lst.Any())
                {
                    Dictionary<string, decimal> lstPozo = getSaldoSAP(lst,Konfig.JednostkaGospodarcza);


                    if (lstPozo != null)
                    {
                        List<string> l = new List<string>();
                        // odj ęcie pozycji już spłąconych w tymobiegu
                        foreach (KeyValuePair<string, decimal> x in lstPozo)
                        {
                            Dokument ddk = dokAll.Where(a => a.SAPDocIdRef == x.Key && a.SAPDocId == null).FirstOrDefault();
                            if (ddk != null)
                            {
                                l.Add(x.Key);
                            }

                        }
                        foreach (string ss in l)
                        {
                            lstPozo[ss] = 0;

                        }

                        foundItem = false;
                        sum = 0;
                        foreach (KeyValuePair<string, decimal> x in lstPozo)
                        {
                            sum += x.Value;
                            if (x.Value == kwt)
                            {
                                foundItem = true;
                                item = x;
                                break;
                            }


                        }
                        if (foundItem)
                        {

                            retValue.Add(item.Key, item.Value); return retValue;
                        }
                        // nie znalkeziono -  trzeba rozrzucić po wielu 
                        if (sum < kwt)
                            return null; // nadpłata - nie ma na co zaliczyć 
                            foreach (KeyValuePair<string, decimal> x in lstPozo)
                            {
                                decimal val = 0;
                                if (x.Value == 0) continue;
                                if (x.Value < kwt)
                                    val = x.Value;

                                else
                                    val = kwt;
                                //Dokument dz = Context.Dokument.Where(c => c.SAPDocId == item.Key && c.Sprawa_Id == spr.Id).FirstOrDefault();
                                retValue.Add(x.Key, val);
                                kwt -= val;
                                if (kwt <= 0)
                                    break;
                            }

                        
                        return retValue;

                    }


                }
            }

            return null;
        }

        private Sprawa findPrzypisExtend(List<Sprawa> lspr, int typ, out Dokument dref, decimal kwt)
        {
            Sprawa spr;
            Dokument dx;
            bool found = false;

            // typp : 1 - grzywna ; 0 - koszty
            dref = null;
            foreach (Sprawa s in lspr)
            {
                foreach (Dokument d in s.Dokument)
                {
                    if (typ == 1 && (d.typFakt == "GS" || d.typFakt == "GP"))
                    {
                        if (!String.IsNullOrWhiteSpace(d.SAPDocId))
                        {
                            // czy jest dokument odposi


                            List<Dokument> lstdok = Context.Dokument.Where(c => c.SAPDocIdRef == d.SAPDocId && c.Sprawa_Id == s.Id).ToList();
                            if ((lstdok == null || lstdok.Count == 0) && d.kwota == kwt)
                            {

                                dref = d;
                                return s;

                            }
                        }



                    }
                    if (typ == 0 && (d.typFakt == "KS" || d.typFakt == "KP"))
                    {
                        if (!String.IsNullOrWhiteSpace(d.SAPDocId))
                        {
                            List<Dokument> lstdok = Context.Dokument.Where(c => c.SAPDocIdRef == d.SAPDocId && c.Sprawa_Id == s.Id).ToList();
                            if (lstdok == null || lstdok.Count == 0 && d.kwota == kwt)
                            {
                                dref = d;
                                return s;
                            }
                        }


                    }
                }



            }
            // 
            // Kwoty się nie zgadzały trzyba poszukac sumy kwot
 


            if (lspr != null && lspr.Count > 0)
            {

                spr = lspr.FirstOrDefault();
                if (typ == 1)
                {
                    dx = Context.Dokument.Where(c => c.Sprawa_Id == spr.Id && (c.typFakt == "GP" || c.typFakt == "GS") && (c.SAPDocId != null)).OrderByDescending(c => c.id).FirstOrDefault();
                    if (dx != null)
                    {
                        dref = dx;
                        return spr;
                    }



                }
                else if (typ == 0)
                {
                    dx = Context.Dokument.Where(c => c.Sprawa_Id == spr.Id && (c.typFakt == "KP" || c.typFakt == "KS") && (c.SAPDocId != null)).OrderByDescending(c => c.id).FirstOrDefault();
                    if (dx != null)
                    {
                        dref = dx;
                        return spr;
                    }


                }

            }
            dref = null;
            return null;
        }


        public Dictionary<string, decimal> getSaldoSAP(List<String> docNo, string jednostkaGospodacza)
        {


            // Po0bierz aktualny transfer
            Cursor.Current = Cursors.WaitCursor;
            Dictionary<string, decimal> retVal = new Dictionary<string, decimal>();


           DocumentListQueryResponse ans;
           foreach (string doc in docNo)
            { 

            try
            {

                ans = ZSRKRequestHelper.PobierzRozrachunki(doc, jednostkaGospodacza);
            }
            catch (Exception ex)
            {

                return null;
            }
            if (ans == null)
            {

                return null;
            }
            /*@@@@@@@@@@@@@@@@@@@@@
            if (ans.Komunikaty != null && ans.Komunikaty[0] != null &&  ans.Komunikaty[0].Length >0  && ans.Komunikaty[0][0].RodzajKomunikatu == "E")
            {
                return null;

            }
            */
            List<DokumentPSCD> lstNierozlicz = ans.DokumentPSCD.Where(a => a.PozycjaDokumentPH[0].PowodRozliczenia == null).ToList();


            foreach (DokumentPSCD dx in lstNierozlicz)
            {
                KeyValuePair<string, decimal> kpv = retVal.Where(a => a.Key == dx.NaglowekDokument.NumerDokumentu).FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(kpv.Key))
                    retVal[kpv.Key] += Convert.ToDecimal(dx.PozycjaDokumentPH[0].Kwota);
                else
                    retVal.Add(dx.NaglowekDokument.NumerDokumentu, Convert.ToDecimal(dx.PozycjaDokumentPH[0].Kwota));
            }
        }
            return retVal;
            
        }        
        private Dokument duplicateDoc(Dokument docIn)
        {
            Dokument docOut = new Dokument();
            docOut.DocGuid = docIn.DocGuid;
            docOut.DataDokumentu = docIn.DataDokumentu;
            docOut.DataKsiegowania = docIn.DataKsiegowania;
            docOut.grzSamoistna = docIn.grzSamoistna;
            docOut.kwota = docIn.kwota;
            docOut.OperacjaCzesciowa = docIn.OperacjaCzesciowa;
            docOut.DataPlatnosci = docIn.DataPlatnosci;
            docOut.Stan = docIn.Stan;
            docOut.Opis = docIn.Opis;
            docOut.typFakt = docIn.typFakt;
            docOut.Sprawa_Id = docIn.Sprawa_Id;
            docOut.SrcSystemId = docIn.SrcSystemId;
            docOut.SrcDocumentHash = docIn.SrcDocumentHash;
            docOut.Document_Id = docIn.Document_Id;
            docOut.DocGuid_Ref = docIn.DocGuid_Ref;
            docOut.SAPDocId = docIn.SAPDocId;
            docOut.KnsPozDzNal = docIn.KnsPozDzNal;
            docOut.KnsRokDzNal = docIn.KnsRokDzNal;
            docOut.KnsKsiegaDzNal = docIn.KnsKsiegaDzNal;
            docOut.Transfer_Id = docIn.Transfer_Id;
            docOut.Dluznik_Id = docIn.Dluznik_Id;
            docOut.RataKwota1 = docIn.RataKwota1;
            docOut.RataData1 = docIn.RataData1;
            docOut.RataKwota2 = docIn.RataKwota2;
            docOut.RataData2 = docIn.RataData2;
            docOut.RataKwota3 = docIn.RataKwota3;
            docOut.RataData3 = docIn.RataData3;
            docOut.RataKwota4 = docIn.RataKwota4;
            docOut.RataData4 = docIn.RataData4;
            docOut.RataKwota5 = docIn.RataKwota5;
            docOut.RataData5 = docIn.RataData5;
            docOut.RataKwota6 = docIn.RataKwota6;
            docOut.RataData6 = docIn.RataData6;
            docOut.RataKwota7 = docIn.RataKwota7;
            docOut.RataData7 = docIn.RataData7;
            docOut.RataKwota8 = docIn.RataKwota8;
            docOut.RataData8 = docIn.RataData8;
            docOut.RataKwota9 = docIn.RataKwota9;
            docOut.RataData9 = docIn.RataData9;
            docOut.RataKwota10 = docIn.RataKwota10;
            docOut.RataData10 = docIn.RataData10;
            docOut.RataKwota11 = docIn.RataKwota11;
            docOut.RataData11 = docIn.RataData11;
            docOut.RataKwota12 = docIn.RataKwota12;
            docOut.RataData12 = docIn.RataData12;
            docOut.RataKwota13 = docIn.RataKwota13;
            docOut.RataData13 = docIn.RataData13;
            docOut.RataKwota14 = docIn.RataKwota14;
            docOut.RataData14 = docIn.RataData14;
            docOut.RataKwota15 = docIn.RataKwota15;
            docOut.RataData15 = docIn.RataData15;
            docOut.RataKwota16 = docIn.RataKwota16;
            docOut.RataData16 = docIn.RataData16;
            docOut.RataKwota17 = docIn.RataKwota17;
            docOut.RataData17 = docIn.RataData17;
            docOut.RataKwota18 = docIn.RataKwota18;
            docOut.RataData18 = docIn.RataData18;
            docOut.RataKwota19 = docIn.RataKwota19;
            docOut.RataData19 = docIn.RataData19;
            docOut.RataKwota20 = docIn.RataKwota20;
            docOut.RataData20 = docIn.RataData20;
            docOut.RataKwota21 = docIn.RataKwota21;
            docOut.RataData21 = docIn.RataData21;
            docOut.RataKwota22 = docIn.RataKwota22;
            docOut.RataData22 = docIn.RataData22;
            docOut.RataKwota23 = docIn.RataKwota23;
            docOut.RataData23 = docIn.RataData23;
            docOut.RataKwota24 = docIn.RataKwota24;
            docOut.RataData24 = docIn.RataData24;
            docOut.RataKwota25 = docIn.RataKwota25;
            docOut.RataData25 = docIn.RataData25;
            docOut.RataKwota26 = docIn.RataKwota26;
            docOut.RataData26 = docIn.RataData26;
            docOut.RataKwota27 = docIn.RataKwota27;
            docOut.RataData27 = docIn.RataData27;
            docOut.RataKwota28 = docIn.RataKwota28;
            docOut.RataData28 = docIn.RataData28;
            docOut.RataKwota29 = docIn.RataKwota29;
            docOut.RataData29 = docIn.RataData29;
            docOut.RataKwota30 = docIn.RataKwota30;
            docOut.RataData30 = docIn.RataData30;
            docOut.RataKwota31 = docIn.RataKwota31;
            docOut.RataData31 = docIn.RataData31;
            docOut.RataKwota32 = docIn.RataKwota32;
            docOut.RataData32 = docIn.RataData32;
            docOut.RataKwota33 = docIn.RataKwota33;
            docOut.RataData33 = docIn.RataData33;
            docOut.RataKwota34 = docIn.RataKwota34;
            docOut.RataData34 = docIn.RataData34;
            docOut.RataKwota35 = docIn.RataKwota35;
            docOut.RataData35 = docIn.RataData35;
            docOut.RataKwota36 = docIn.RataKwota36;
            docOut.RataData36 = docIn.RataData36;
            docOut.SAPRatyId = docIn.SAPRatyId;
            docOut.SAPDocIdRef = docIn.SAPDocIdRef;
            docOut.Info = docIn.Info;
            docOut.OperacjaGlowna = docIn.OperacjaGlowna;
            docOut.SAPImportInfo = docIn.SAPImportInfo;
            docOut.SAPImportStatus = docIn.SAPImportStatus;
            docOut.SAPImportDate = docIn.SAPImportDate;
            docOut.SAPImportPonowne = docIn.SAPImportPonowne;
            docOut.SAPRodzajDokumentu = docIn.SAPRodzajDokumentu;
            docOut.SAPKontoKG = docIn.SAPKontoKG;
            docOut.SAPWaluta = docIn.SAPWaluta;




            return docOut;
        }

        public void ImportOdpis()
        {

            SqlDataReader rdr = null;
            SqlConnection con = null;
            const int counter = 200;
            int i;
            int loopcount = 0;
            int ratyCount = 0;
            Sprawa spr;
            int sprawa_Id;
            Dluznik dl;
            DateTime dFirst;
            DateTime dLast;
           
            string errmsg;
            List<string> rList = new List<string>();
            List<string> orygrList = new List<string>();
            List<Sprawa> lspraw = null;

            SAPSad mySad;
           
            Dokument doc;
            Dokument dock;
            Dokument dokref; // dokument referencyjny saldo lub przypis grzywny
            Dokument dockref; // dokument referencyjny saldo lub przypis kosztów
            
            string CommandText="";
            Transfer trans;
            string doc2Hash = String.Empty;
            string dock2Hash = String.Empty;
            List<Sprawa> lspr = null;
            KnsKsiegi knsks;
            int stepNo = 0;
            DataTable dt = null;
            DataRow currentdtr = null;
            SqlCommand storedProcCommand = null;
            Dictionary<string, decimal> lstValGrz;
            Dictionary<string, decimal> lstValKs;
            //  Thread th = new Thread(progressWindow);
            // th.Start();


            try
            {
                // Open connection to the database
                ImportedDocs = 0;
                errorStatus = false;
                miesPackHlp.Context = this.Context;
                if (String.IsNullOrWhiteSpace(Konfig.StanowiskoFin))
                    mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                else
                    mySad = (from c in Context.SAPSad where c.kod == Konfig.StanowiskoFin select c).FirstOrDefault();
                //mySad = (from c in Context.SAPSad where c.kod == Konfig.JednostkaGospodarcza select c).FirstOrDefault();
                if (mySad == null)
                {
                    MessageBox.Show("Ustaw własny sąd w konfiguracji");
                    return;
                }

                List<KnsKsiegi> knsLst = this.Context.KnsKsiegi.ToList();
                KnsKsiegi ksiega;
             
                // przygotowanie listy repertoriów
                foreach (SAPRepertorium srep in Context.SAPRepertorium.ToList())
                {
                    string s;
                    string s1;
                    s = srep.kod.Trim().ToUpper();
                    rList.Add(s);
                    s1 = srep.kod.Trim();
                    orygrList.Add(s1);

                }


             
                {
                    //string ConnectionString = Utils.BuildMyConnectionString(Context);
                    //string ConnectionString = Properties.Settings.Default.KnsMigratorConnectionString;
                    string ConnectionString = (Konfig.typKns == 2) ? Utils.BuildMyConnectionString(Context) : Properties.Settings.Default.KnsMigratorConnectionString;
                   con = new SqlConnection(ConnectionString);
                    con.Open();
             
/*
                    switch (Konfig.typKns)
                    {
                        case 0: // currenda
                            CommandText = "sp_OdpisyCR";
                            break;
                        case 1: // Zeto
                            CommandText = "sp_Odpisy";
                            break;
                        case 2: // Zeto
                            CommandText = "sp_OdpisyOR";
                            break;
                        case 3: // Zeto
                            CommandText = "sp_OdpisyAL";
                            break;
                        default:
                            break;
                    }
                    */
                    switch (Konfig.typKns)
                    {
                        case 0: // currenda
                            storedProcCommand = new SqlCommand("sp_OdpisyCR", con);
                            break;
                        case 1: // Zeto
                            storedProcCommand = new SqlCommand("sp_Odpisy", con);
                            break;
                        case 2: // Zeto
                            storedProcCommand = new SqlCommand("sp_OdpisyOR", con);
                            break;
                        case 3: // Zeto
                            storedProcCommand = new SqlCommand("sp_OdpisyAL", con);
                            break;
                        default:
                            break;
                    }

             
                    storedProcCommand.CommandType = CommandType.StoredProcedure;
                    string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                    storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                    //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                    storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                    storedProcCommand.Parameters.Add("@dataDo", theday);
                    storedProcCommand.Parameters.Add("@dataOd", data_od);
                    
                    storedProcCommand.Connection = con;
                    storedProcCommand.CommandTimeout = 600;
                    progressMsg = "Odczyt danych...";
                    rdr = storedProcCommand.ExecuteReader();
               
                    if (rdr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(rdr);
                    }

                }
                if (dt == null)
                {
                    if (RunMode.silentMode)
                    {
                        Utils.LogWriter("Brak danych do importu");
                        errorStatus = true;
                    }
                    else
                        MessageBox.Show("Brak danych do importu");
                    return;

                }
                else
                {
                    trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = 3; // odpisy
                    trans.DataOd = new DateTime(2099, 12, 31);
                    trans.DataDo = new DateTime(2000, 1, 1);
                    trans.Uwagi = uwagi;
                    dFirst = DateTime.MaxValue;
                    dLast = DateTime.MinValue;
                    trans.Bledne = 0;
                    trans.Kwota = 0;
                    trans.LFaktow = 0;
                    trans.Zaimportowane = 0;
                    this.CurrentTransfer = trans;
               
                    //////
                    loopcount = 0;
                    // setup 


                    stepNo = 1;
               
                    foreach (DataRow dtr in dt.Rows)
                    {
                        lstValGrz = null;
                        lstValKs = null;
                        currentdtr = dtr;
                        stepNo = 1;
                        if (breakIndicator == true) break;
                        stepNo++;
                        int curKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                        //if (!RunMode.silentMode) // jeśli w trybie cichym to ze wszystkich ksiąg 
                        //{
                        if (KsiegiKnsLst.Any())
                            if (!KsiegiKnsLst.Contains(curKsiega)) continue;
                        //}

                        stepNo++;
                        ksiega = knsLst.Where(a => a.Id_Ksiegi == curKsiega).FirstOrDefault();
                        stepNo++;
                        progressMsg = "Dokument " + (++loopcount).ToString();
                        //(pForm.Controls["lbInfo"] as Label).Refresh();
                        errmsg = "";
                        doc = null;
                        dock = null;
                        dokref = null;
                        dockref = null;
                        doc2Hash = curKsiega.ToString();
                        dock2Hash = curKsiega.ToString();

                        stepNo++;
                        sprawa_Id = Convert.ToInt32(dtr["Sprawa_id"]);
                        dl = null;
                        spr = null;
                        stepNo++;
                        lspr = Context.Sprawa.Where(a => a.KnsSprawa_id == sprawa_Id && a.KnsKsiega == curKsiega && a.SAPKontoUmowy.Length > 6 && a.Dluznik.Any() && a.Dokument.Any()).OrderByDescending(a => a.Id).ToList();
                        spr = Context.Sprawa.Where(a => a.KnsSprawa_id == sprawa_Id && a.KnsKsiega == curKsiega && a.SAPKontoUmowy.Length > 6 && a.Dluznik.Any() && a.Dokument.Any()).OrderByDescending(a => a.Id).FirstOrDefault();
                        // ksiega 
                        stepNo++;
                        if (spr == null)
                        {
                            errmsg += "Brak w bazie Integratora sprawy, której dotyczy dokument lub brak referencji Konta umowy w SAP/RUP  , poz dziennika " + dtr["Pozycja"].ToString() + "/" + dtr["Rok"].ToString();
                            errorStatus = true;
                            stepNo++;
                           
                        }
                        else
                        {
                            dl = Context.Dluznik.Where(a => a.Sprawa_Id == spr.Id).FirstOrDefault();
                            doc2Hash += spr.KdRok.ToString();
                            dock2Hash += spr.KdRok.ToString();
                            doc2Hash += spr.KdNumer.ToString();
                            dock2Hash += spr.KdNumer.ToString();
                            stepNo++;
                        }
                        log.Debug("Odpisywanie ....");
                        if   (Convert.ToDecimal(dtr["grzywna_odpis"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0) 
                        {
                            lstValGrz = findPrzypis(lspr,spr, 1, Convert.ToDecimal(dtr["grzywna_odpis"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")));
                            log.Debug("grzywna");
                            stepNo = 100;
                            doc = new Dokument();
                            doc.SAPImportStatus = 0;
                            doc.DocGuid = Guid.NewGuid();
                            stepNo = 101;
                            doc.DataDokumentu = dtr["DataDokumentu"] as DateTime? ?? null;
                            doc.DataKsiegowania = dtr["DataKsiegowania"] as DateTime? ?? null;
                            doc.KnsPozDzNal = Convert.ToInt32(dtr["pozycja"] == null ? "0" : dtr["pozycja"]);
                            stepNo = 102;
                            doc.KnsKsiegaDzNal = curKsiega;
                            doc.KnsRokDzNal = doc.DataKsiegowania.Value.Year;
                            stepNo = 103;
                            doc2Hash += doc.KnsPozDzNal.ToString();
                            doc2Hash += Convert.ToDateTime(doc.DataDokumentu).ToString("yyyyMMdd");
                            stepNo = 104;
                            doc.DataPlatnosci = doc.DataDokumentu;
                            stepNo = 105;
                            //Mark. 
                            if (ksiega.czyFPP == 1)
                                doc.OperacjaGlowna = "FPP0";
                            else if (ksiega.czyFPP == 2)
                                doc.OperacjaGlowna = "N034";
                            else
                                if (dtr["zrodlo"].ToString() == "przedawnienie")
                                {
                                    doc.OperacjaGlowna = "N021";
                                    stepNo = 106;
                                }
                                else if (dtr["ns1"].ToString() == "5c")
                                    doc.OperacjaGlowna = "N030";
                                else
                                    doc.OperacjaGlowna = "N020";
                            stepNo = 107;



                            if (lstValGrz != null && lstValGrz.Count!= 0 )
                            {
                                string dId = lstValGrz.First().Key;
                                dokref = this.Context.Dokument.Where(a => a.SAPDocId == dId).OrderByDescending(a => a.id).FirstOrDefault();
                                doc.OperacjaCzesciowa = dokref.OperacjaCzesciowa;
                                stepNo++;
                                doc.grzSamoistna = dokref.grzSamoistna;
                                // sprawdzenie czy są raty
                                stepNo = 108;
                                Dokument raty;
                                if (!String.IsNullOrEmpty(dokref.SAPRatyId))
                                { doc.SAPRatyId = dokref.SAPRatyId; ratyCount++; }
                                else
                                {
                                    raty = Context.Dokument.Include("Transfer").Where(b => b.SAPDocId == dokref.SAPDocId && b.typFakt == "GR").OrderByDescending(a => a.Transfer.DataTransferu).FirstOrDefault();
                                    if (raty != null)
                                    {
                                        stepNo = 109;
                                        doc.SAPRatyId = raty.SAPRatyId;
                                        ratyCount++;
                                    }
                                }


                            }
                            else
                            {
                                errmsg += " Brak referencyjnego dokumentu przypisu (salda) lub  brak oznaczenia dokumentu w RUP SAP";
                                stepNo = 110;
                                errorStatus = true;
                                if (spr != null)
                                {
                                    doc.grzSamoistna = spr.grzSamoistna;

                                    stepNo = 111;
                                    switch (spr.SAPRodzajPrzedmiotuUmowy)
                                    {
                                        case "SPPR":
                                        case "SROD":
                                        case "SUBE":
                                        case "SGOS":
                                        case "SCYW":
                                            if (dl.FizPraw == "X")   // osoba prawna
                                                doc.OperacjaCzesciowa = "0090";  // lub "0100"  ??
                                            else
                                                doc.OperacjaCzesciowa = "0010";
                                            stepNo = 112;
                                            break;


                                        case "SKAR":
                                            if (dl.FizPraw == "X")   // osoba prawna
                                                doc.OperacjaCzesciowa = "0090";
                                            else // osoba fizyczna  sprawdzić  czy wykroczenia i czy samoistna
                                            {
                                                if (spr.grzSamoistna == "s")
                                                {
                                                    if (spr.SAPRepertorium.ToUpper() == "W")
                                                        // wykroczenie
                                                        doc.OperacjaCzesciowa = "0070";
                                                    else
                                                        doc.OperacjaCzesciowa = "0040";
                                                    stepNo = 113;
                                                }
                                                else
                                                {
                                                    if (spr.SAPRepertorium.ToUpper() == "W")
                                                        // wykroczenie
                                                        doc.OperacjaCzesciowa = "0050";
                                                    else
                                                        doc.OperacjaCzesciowa = "0020";
                                                    stepNo = 114;
                                                }
                                            }
                                            break;
                                        default:
                                            errmsg += " ; " + "Brak oznaczenia operacji cześciowej (grzywna) ";
                                            errorStatus = true;
                                            stepNo = 115;
                                            break;
                                    }

                                }
                            }
                            // dodanie dodatkowych oznaczeń 

                            if (doc.OperacjaCzesciowa == "0050")
                            {
                                if (dtr["ns1"].ToString() == "5a")
                                    doc.OperacjaCzesciowa = "0060";
                                if (dtr["ns1"].ToString() == "5b")
                                    doc.OperacjaCzesciowa = "0031";
                                stepNo = 116;
                            }
                            else
                            {
                                if (dtr["ns1"].ToString() == "5a")
                                    doc.OperacjaCzesciowa = "0030";
                                if (dtr["ns1"].ToString() == "5b")
                                    doc.OperacjaCzesciowa = "0031";
                                stepNo = 117;
                            }

                            if (dtr["zrodlo"].ToString() == "PSU")
                                doc.OperacjaCzesciowa = "0080";
                            stepNo = 118;
                            if (ksiega.czyFPP == 1)
                                doc.OperacjaCzesciowa = (Konfig.typKns == 2) ? "0020" : "0021"; 

                            doc.kwota = Convert.ToDecimal(dtr["grzywna_odpis"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                            doc2Hash += doc.kwota.ToString();
                            doc.typFakt = "GO";
                            doc.Info = errmsg.Truncate(255);
                            stepNo = 119;
                            if (String.IsNullOrEmpty(doc.Info)) doc.Info = null;
                        }
                        stepNo = 120;
                        if (Convert.ToDecimal(dtr["koszty_odpis"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")) > 0)
                        {
                            lstValKs = findPrzypis(lspr,spr, 0,  Convert.ToDecimal(dtr["koszty_odpis"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")));  
                            stepNo = 121;
                            dock = new Dokument();
                            dock.SAPImportStatus = 0;
                            dock.DocGuid = Guid.NewGuid();
                            stepNo = 122;
                            dock.KnsPozDzNal = Convert.ToInt32(dtr["pozycja"] == null ? "0" : dtr["pozycja"]);
                            dock.DataDokumentu = dtr["DataDokumentu"] as DateTime? ?? null;
                            dock.DataKsiegowania = dtr["DataKsiegowania"] as DateTime? ?? null;
                            stepNo = 123;
                            dock.KnsRokDzNal = dock.DataKsiegowania.Value.Year;
                            dock.KnsKsiegaDzNal = curKsiega;
                            dock.DataPlatnosci = dock.DataDokumentu;
                            dock2Hash += dock.KnsPozDzNal.ToString();
                            dock2Hash += Convert.ToDateTime(dock.DataDokumentu).ToString("yyyyMMdd");
                            stepNo = 124;
                           // if (spr != null) dockref = Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "KS" || a.typFakt == "KP") && a.SAPDocId.Length > 6   ).FirstOrDefault();
                            if (ksiega.czyFPP == 1)
                                dock.OperacjaGlowna = "FPP0";
                            else if (ksiega.czyFPP == 2)
                                dock.OperacjaGlowna = "N034";
                            else
                                if (dtr["zrodlo"].ToString() == "przedawnienie")
                                {
                                   if (dock.DataKsiegowania.Value.Year >= 2017)
                                        dock.OperacjaGlowna = "N023";
                                   else
                                       dock.OperacjaGlowna = "N021";

                                }
                                else if (dtr["ns1"].ToString() == "5c")
                                {
                                    if (dock.DataKsiegowania.Value.Year >= 2017)
                                        dock.OperacjaGlowna = "N031";
                                    else
                                        dock.OperacjaGlowna = "N030";
                                }
                                else
                                {
                                    if (dock.DataKsiegowania.Value.Year >= 2017)
                                        dock.OperacjaGlowna = "N022";
                                    else
                                        dock.OperacjaGlowna = "N020";
                                }
                            Dokument raty;
                            stepNo = 125;
                            if (lstValKs != null && lstValKs.Count> 0 )
                            {
                                string dId = lstValKs.First().Key;
                                dockref = this.Context.Dokument.Where(a => a.SAPDocId == dId).OrderByDescending(a => a.id).FirstOrDefault();
                                stepNo = 126;
                                if (ksiega.czyFPP == 1)
                                    dock.OperacjaCzesciowa = (Konfig.typKns == 2) ? "0021" : "0020";
                                else if (ksiega.czyFPP == 2)
                                    dock.OperacjaCzesciowa = "0001";
                                else
                                    dock.OperacjaCzesciowa = dockref.OperacjaCzesciowa;
                                dock.grzSamoistna = dockref.grzSamoistna;
                                if (!String.IsNullOrEmpty(dockref.SAPRatyId))
                                { dock.SAPRatyId = dockref.SAPRatyId; ratyCount++; }
                                else
                                {
                                    stepNo = 127;
                                    raty = Context.Dokument.Include("Transfer").Where(b => b.SAPDocId == dockref.SAPDocId && b.typFakt == "KR").OrderByDescending(a => a.Transfer.DataTransferu).FirstOrDefault();
                                    if (raty != null)
                                    {
                                        stepNo = 128;
                                        dock.SAPRatyId = raty.SAPRatyId;
                                        ratyCount++;
                                    }
                                }
                            }
                            else
                            {
                                errmsg += " Brak referencyjnego dokumentu przypisu (salda) lub  brak oznaczenia dokumentu w RUP SAP";
                                stepNo = 110;
                                errorStatus = true;
                                stepNo = 129;
                                dock.grzSamoistna = "";
                                if (spr != null)
                                {

                                    stepNo = 130;
                                    if (ksiega.czyFPP == 1)
                                        dock.OperacjaCzesciowa = (Konfig.typKns == 2) ? "0021" : "0020";
                                    else if (ksiega.czyFPP == 2)
                                        dokref.OperacjaCzesciowa = "0001";
                                    else
                                        switch (spr.SAPRodzajPrzedmiotuUmowy)
                                        {
                                            case "SROD":
                                                dock.OperacjaCzesciowa = "0120";
                                                break;
                                            case "SPPR":
                                            case "SUBE":
                                            case "SGOS":
                                            case "SCYW":
                                                if (dl.FizPraw == "X")   // osoba prawna
                                                    dock.OperacjaCzesciowa = "0110";  // brak pozycji w słowniku.
                                                else
                                                    dock.OperacjaCzesciowa = "0110";
                                                stepNo = 131;
                                                break;


                                            case "SKAR":
                                                dock.OperacjaCzesciowa = "0130";
                                                break;
                                            default:
                                                errmsg += " ; " + "Brak oznaczenia operacji cześciowej (koszty) ";
                                                errorStatus = true;
                                                stepNo = 132;
                                                break;
                                        }
                                }
                            }
                            stepNo = 133;
                            dock.kwota = Convert.ToDecimal(dtr["koszty_odpis"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                            dock2Hash += dock.kwota.ToString();
                            stepNo = 134;
                            dock.typFakt = "KO";
                            dock.Info = errmsg.Truncate(255);
                        }

                        Transfer tt = miesPackHlp.setTransfer(curKsiega, (doc != null ? doc.DataKsiegowania.Value : dock.DataKsiegowania.Value), this.typImport); // dla odpisu
                        if (tt != null)
                        {
                            trans = tt;
                            this.CurrentTransfer = trans;
                        }
                        if (doc != null)
                        {
                            int ans;
                            string outmsg;
                            doc.InsertedBy = UserInfo.Username;
                            doc.InsDate = DateTime.Now;
                            doc.SrcDocumentHash = Utils.HashFromString(doc2Hash);
                        
                            stepNo = 135;
                            if (dtr["opis"] == DBNull.Value)
                                doc.Opis = "";
                            else
                                doc.Opis = dtr["opis"].ToString();

                            if (doc.Opis != null)
                                doc.Opis = doc.Opis.Truncate(50);

                            if ((ans = docExists(doc, spr == null ? "" : spr.Karta, out outmsg)) <= 0)
                            {

                            
                                stepNo = 136;
                                if (ans < 0)
                                    doc.Info = doc.Info + (doc.Info == null ? "" : ";" + doc.Info);
                                doc.SAPImportInfo = doc.SAPImportInfo.Truncate(255);
                              
                                doc.Info = doc.Info.Truncate(255);
                                if (lstValGrz != null && lstValGrz.Count > 0)
                                { // dodawanie dokumentów 
                                    foreach (KeyValuePair<string, decimal> y in lstValGrz)
                                    {
                                        Dokument do1 = duplicateDoc(doc);
                                        do1.DocGuid = Guid.NewGuid();
                                        do1.SAPDocIdRef = y.Key;
                                        do1.kwota = y.Value;
                                        Sprawa sp =
                                            (from m in Context.Sprawa
                                             join n in Context.Dokument on m.Id equals n.Sprawa_Id
                                             where n.SAPDocId == y.Key
                                             select m).FirstOrDefault();

                                        stepNo = 137;
                                        // sprawa 
                                        if (sp != null)
                                        {
                                            spr.Dokument.Add(do1);
                                            sp.Dluznik.FirstOrDefault().Dokument.Add(do1);
                                        }
                                        stepNo = 138;
                                        trans.Dokument.Add(do1);
                                    }
                                }
                                else
                                {

                                    if (spr != null) spr.Dokument.Add(doc);
                                    if (dl != null) dl.Dokument.Add(doc);
                                    stepNo = 144;
                                    trans.Dokument.Add(doc);

                                }

                                this.updateTrasDates(trans, Convert.ToDateTime(doc.DataKsiegowania));
                                stepNo = 139;
                            }
                            else
                            {
                                // doc.Info = errmsg;
                                // if (spr != null) spr.Dokument.Add(doc);
                                // if (dl != null) dl.Dokument.Add(doc);
                                // stepNo = 1444;

                                //  trans.Dokument.Add(doc);
                                //  this.updateTrasDates(trans, Convert.ToDateTime(doc.DataKsiegowania));
                                ;
                            }
                                
                                //MessageBox.Show(" Odpis,  poz" + doc.KnsPozDzNal.ToString() + "/" + doc.KnsRokDzNal.ToString() + "  już istnieje w bazie  Komunikat " + outmsg + " informacja  techniczna: hash dokumentu = " + doc.SrcDocumentHash.ToString());
                        }
                        if (dock != null)
                        {
                            int ans;
                            string outmsg;
                            stepNo = 140;
                            dock.InsertedBy = UserInfo.Username;
                            dock.InsDate = DateTime.Now;
                            dock.SrcDocumentHash = Utils.HashFromString(dock2Hash);
                            stepNo = 141;
                            if (dtr["opis"] == DBNull.Value)
                                dock.Opis = "";
                            else
                                dock.Opis = dtr["opis"].ToString();    
 
                            stepNo = 142;
                            if (dock.Opis != null)
                                dock.Opis = dock.Opis.Substring(0, dock.Opis.Length > 50 ? 50 : dock.Opis.Length);
                            if ((ans = docExists(dock,spr == null ? "" : spr.Karta, out outmsg)) <= 0)
                            {
                                stepNo = 143;
                                if (ans < 0)
                                    dock.Info = dock.Info + (dock.Info == null ? "" : ";" + dock.Info);
                                dock.SAPImportInfo = dock.SAPImportInfo.Truncate(255);
                                dock.Info = dock.Info.Truncate(255);
                                if (lstValKs != null && lstValKs.Count > 0)
                                { // dodawanie dokumentów 
                                    foreach (KeyValuePair<string, decimal> y in lstValKs)
                                    {
                                        Dokument do1 = duplicateDoc(dock);
                                        do1.DocGuid = Guid.NewGuid();
                                        do1.SAPDocIdRef = y.Key;
                                        do1.kwota = y.Value;
                                        stepNo = 137;
                                        Sprawa sp =
                                           (from m in Context.Sprawa
                                            join n in Context.Dokument on m.Id equals n.Sprawa_Id
                                            where n.SAPDocId == y.Key
                                            select m).FirstOrDefault();

                                        stepNo = 137;
                                        // sprawa 
                                        if (sp != null)
                                        {
                                            spr.Dokument.Add(do1);
                                            sp.Dluznik.FirstOrDefault().Dokument.Add(do1);
                                        }
                                       
                                        trans.Dokument.Add(do1);
                                    }
                                }
                                else
                                {
                                    if (dockref != null) dock.SAPDocIdRef = dockref.SAPDocId;
                                    if (spr != null) spr.Dokument.Add(dock);
                                    if (dl != null) dl.Dokument.Add(dock);
                                    stepNo = 144;
                                    trans.Dokument.Add(dock);
                                }
                                this.updateTrasDates(trans, Convert.ToDateTime(dock.DataKsiegowania));
                                stepNo = 145;
                            }
                            else 
                            {
                                // dock.Info = errmsg;
                                //  if (spr != null) spr.Dokument.Add(dock);
                                //   if (dl != null) dl.Dokument.Add(dock);
                                //  stepNo = 1444;
                                //  trans.Dokument.Add(dock);
                                //  this.updateTrasDates(trans, Convert.ToDateTime(dock.DataKsiegowania));
                                ;
                            }                       
}
                       
                   
                        trans.LFaktow = trans.Dokument.Count;
                        ImportedDocs = trans.Dokument.Count;
                    
                        stepNo = 146;

                        /*
                        if (--i == 0)
                        {
                            Context.SaveChanges();
                            i = counter;
                            loopcount++;
                        }
                        */


                    }
                  
                        stepNo = 148;
                        if ( trans.LFaktow > 0  && trans.EntityState == EntityState.Detached)
                            Context.Transfer.AddObject(trans);
                        Context.SaveChanges();
                        stepNo = 149;
                
                    if (ratyCount > 0)
                    {
                        stepNo = 150;
                        errorStatus = true;
                        if (RunMode.silentMode)
                        {
                            Utils.LogWriter(ratyCount.ToString() + " pozycji na liście odpisuje należności rozłożone na raty. Dezaktywuj plany ratalne przed transferem danych do SAP.  Numery dokumentów rat w kolumnie <<Numer dokumentu plan rat>>");
                            errorStatus = true;
                        }
                        else
                            MessageBox.Show(ratyCount.ToString() + " pozycji na liście odpisuje należności rozłożone na raty. Dezaktywuj plany ratalne przed transferem danych do SAP.  Numery dokumentów rat w kolumnie <<Numer dokumentu plan rat>>", "Uwaga");

                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Błąd ";
                // Print error message
                if (rdr != null)
                    msg = " Błąd przy pozycji dziennika " + currentdtr["Pozycja"].ToString() + "/" + currentdtr["Rok"];
                if (RunMode.silentMode)
                {
                    Utils.LogWriter(msg + ex.Message + "  " + (ex.InnerException != null ? ex.InnerException.Message : "" ) + " Krok = " + stepNo.ToString());
                    errorStatus = true;
                }
                else
                    MessageBox.Show(msg + ex.Message + "  " + (ex.InnerException != null ? ex.InnerException.Message : "") + " Krok = " + stepNo.ToString());
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con != null)
                if (con.State == ConnectionState.Open)
                    con.Close();
               
                breakIndicator = true;
            }
        }

        public void ImportRatRozlicz()
        {

            SqlDataReader rdr = null;
            SqlConnection con = null;
            int i;
            int loopcount = 0;
            Sprawa spr = null;
            int sprawa_Id;
            int lastSprawa_id;
            Dluznik dl;
            decimal grzywna = 0;
            decimal koszty  = 0;
            DateTime dataRaty;
            DateTime dataPostan;
            decimal grz_start = 0;
            decimal ks_start = 0; 
            decimal rata  = 0 ;
            string errmsg;
            List<kwtData> gLst = new List<kwtData>();
            List<kwtData> kLst = new List<kwtData>();

            dataRaty = DateTime.Today;
            Dokument doc;
            DataTable dt = null;
            DataRow currentdtr = null;

            string CommandText = "";
            Transfer trans;
            //  Thread th = new Thread(progressWindow);
            // th.Start();


            try
            {
                // Open connection to the database
                string ConnectionString = Utils.BuildMyConnectionString(this.Context);
                con = new SqlConnection(ConnectionString);
                con.Open();
                                
               


                switch (Konfig.typKns)
                {
                    case 0: // currenda
                        CommandText = "sp_Raty2";
                        break;
                    case 1: // Zeto
                        CommandText = "sp_Raty_HarmonogramRozlicz";
                        break;
                    default:
                        break;
                }


                SqlCommand storedProcCommand = new SqlCommand(CommandText, con);
                storedProcCommand.CommandType = CommandType.StoredProcedure;
                string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                storedProcCommand.Parameters.Add("@dataOd", data_od);
                storedProcCommand.Parameters.Add("@dataDo", theday);
                storedProcCommand.Connection = con;
                storedProcCommand.CommandTimeout = 600;
                progressMsg = "Odczyt danych...";

                rdr = storedProcCommand.ExecuteReader();

              

                if (rdr.HasRows)
                {
                    trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = 5; // Raty
                    trans.DataDo = theday;   // doccelowo podać datę 
                    trans.DataOd = data_od;
                    trans.Uwagi = uwagi;
                    Context.Transfer.AddObject(trans);
                    this.CurrentTransfer = trans;
                    dt.Load(rdr);
                }
                else
                {
                    MessageBox.Show("Brak danych do importu");
                    return;
                }
                //////

                loopcount = 0;
                // setup 


                lastSprawa_id = 0;
                    sprawa_Id = 0;
                    dataPostan = DateTime.Today;
                    foreach (DataRow dtr in dt.Rows)
                {
                    currentdtr = dtr;
                    if (breakIndicator == true) break;
                    int curKsiega = Convert.ToInt32(dtr["Ksiega"] == DBNull.Value ? 0 : dtr["Ksiega"]);
                    if (!KsiegiKnsLst.Contains(curKsiega)) continue; 
                    progressMsg = "Dokument " + (++loopcount).ToString();
                    //(pForm.Controls["lbInfo"] as Label).Refresh();
                    errmsg = "";
                    doc = null;
                   


                    sprawa_Id = Convert.ToInt32(dtr["Sprawa_id"]);

                    if (lastSprawa_id != sprawa_Id )
                    {
                        if (lastSprawa_id > 0)
                        {
                            spr = this.Context.Sprawa.Include("Dluznik").Where(a => a.KnsSprawa_id == lastSprawa_id &&  a.SAPKontoUmowy.Length > 5 ).FirstOrDefault();
                            if (spr == null)
                            {
                                errmsg += " Brak sprawy zarejestrowanej w SAP ";
                                spr = this.Context.Sprawa.Include("Dluznik").Where(a => a.KnsSprawa_id == lastSprawa_id ).FirstOrDefault();
                            }
                            if (spr != null)
                            {
                                if (kLst.Count > 0)
                                {
                                    Dokument docRef = this.Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "KS" || a.typFakt == "KP") && a.SAPDocId.Length > 5 ).OrderByDescending(a => a.DataDokumentu).FirstOrDefault();
                                    if (docRef == null)
                                    {
                                        docRef = this.Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "GS" || a.typFakt == "GP")).OrderByDescending(a => a.DataDokumentu).FirstOrDefault();

                                    }
                                    if (docRef != null)
                                    {
                                        doc = new Dokument();
                                        doc.SAPImportStatus = 0;
                                        doc.DocGuid = Guid.NewGuid();
                                        doc.DataKsiegowania = dataPostan;
                                        doc.typFakt = "KR";
                                        doc.DataDokumentu = dataPostan;
                                        doc.DataPlatnosci = doc.DataDokumentu;
                                        doc.Stan = "B";
                                        spr.Dokument.Add(doc);
                                        doc.kwota = ks_start;
                                        Dluznik dluzn = spr.Dluznik.FirstOrDefault();
                                        dluzn.Dokument.Add(doc);
                                        doc.Sprawa_Id = spr.Id;
                                        doc.SAPDocId = docRef.SAPDocId;
                                        UpdateDocRaty(ref doc, ref kLst);
                                        CurrentTransfer.Dokument.Add(doc);
                                        //this.Context.Dokument.AddObject(doc);

                                                                         }
                                    else MessageBox.Show("Brak kompelemntarnego dokumentu przypisu lub salda dla kosztów  " + spr.Karta); 
                                }
                                if (gLst.Count > 0)
                                {
                                    Dokument docgRef = this.Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "GS" || a.typFakt == "GP") && a.SAPDocId.Length > 5).OrderByDescending(a => a.DataDokumentu).FirstOrDefault();
                                    if (docgRef == null )
                                    {
                                       docgRef = this.Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "GS" || a.typFakt == "GP") ).OrderByDescending(a => a.DataDokumentu).FirstOrDefault(); 

                                    }
                                    if (docgRef != null)
                                    {
                                        doc = new Dokument();
                                        doc.SAPImportStatus = 0;
                                        doc.DocGuid = Guid.NewGuid();
                                        doc.DataKsiegowania = dataPostan;
                                        doc.DataDokumentu = dataPostan;
                                        doc.kwota = grz_start;
                                        doc.Stan = "B";
                                        doc.DataPlatnosci = doc.DataDokumentu;
                                        doc.typFakt = "GR";
                                        spr.Dokument.Add(doc);
                                        Dluznik dluzn = spr.Dluznik.FirstOrDefault();
                                        dluzn.Dokument.Add(doc);
                                        doc.Sprawa_Id = spr.Id;
                                        doc.SAPDocId = docgRef.SAPDocId;
                                        UpdateDocRaty(ref doc, ref gLst);
                                        CurrentTransfer.Dokument.Add(doc);
                                        //this.Context.Dokument.AddObject(doc);
                                        // wrzucić raty grzywny
                                        // ********
                                    }
                                    else MessageBox.Show("Brak kompelemntarnego dokumentu przypisu lub salda dla grzywny " + spr.Karta); 
                                }


                            }
                            else  // spr == null
                           {
                               MessageBox.Show("Błąd - brak sprawy dla rozłożenia grzywny " + grz_start.ToString() + " i/lub kosztów " + ks_start.ToString());   
                            
                            }
                        }

                        if (sprawa_Id == 60482)
                        {

                            ;
                        }
                            
                            // reset 
                        grzywna = dtr["grzywna"] as decimal? ?? default(decimal);
                        koszty = dtr["koszty"] as decimal? ?? default(decimal);
                        grz_start = grzywna;
                        ks_start = koszty;
                        lastSprawa_id = sprawa_Id;
                        gLst.Clear();
                        kLst.Clear();

                    }
                    dataPostan = dtr["data_wyst_post"] as DateTime? ?? default(DateTime);
                    rata = dtr["Kwota_Raty"] as decimal? ?? default(decimal);
                    dataRaty = dtr["Data_Raty"] as DateTime? ?? default(DateTime);

                    if ( koszty > 0 )
                    {
                        kwtData kwtd = new kwtData();
                        if (koszty > rata ) 
                        { 
                            
                            kwtd.data =  dataRaty;
                            kwtd.kwota = rata;
                            koszty -= rata;
                            kLst.Add(kwtd);
                        }
                        else 
                        {
                            kwtd.data =  dataRaty;
                            kwtd.kwota = koszty;
                            rata -= koszty;
                            koszty = 0 ;
                            kLst.Add(kwtd);
                            if (rata > 0 )
                            {
                               kwtData kwtdg = new kwtData();
                               kwtdg.kwota = rata;
                               kwtdg.data = dataRaty;
                               gLst.Add(kwtdg);  
                               grzywna -= rata; 
                            }
                        }
                    }
                   else 
                        if (grzywna > 0 )// grzywna
                    {
                         kwtData kwtdg = new kwtData();
                          if (grzywna > rata ) 
                        { 
                            
                            kwtdg.data =  dataRaty;
                            kwtdg.kwota = rata;
                            grzywna -= rata;
                            gLst.Add(kwtdg);
                        }
                        else 
                        {
                            kwtdg.data =  dataRaty;
                            kwtdg.kwota = grzywna;
                            rata -= grzywna;
                            grzywna = 0 ;
                            gLst.Add(kwtdg);
                         }
                    
                       }

                    } //while

                //  zapis ostatnich rat
                    if (lastSprawa_id > 0)
                        {
                            spr = this.Context.Sprawa.Include("Dluznik").Where(a => a.KnsSprawa_id == lastSprawa_id &&  a.SAPKontoUmowy.Length > 5 ).FirstOrDefault();
                            if (spr == null)
                            {
                                MessageBox.Show("Brak sprawy zarejestrowanej w SAP");
                                spr = this.Context.Sprawa.Include("Dluznik").Where(a => a.KnsSprawa_id == lastSprawa_id ).FirstOrDefault();
                            }
                            if (spr != null)
                            {
                                if (kLst.Count > 0)
                                {
                                    Dokument docRef = this.Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "KS" || a.typFakt == "KP") && a.SAPDocId.Length > 5).OrderByDescending(a => a.DataDokumentu).FirstOrDefault();
                                    if (docRef == null)
                                    {
                                        docRef = this.Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "GS" || a.typFakt == "GP")).OrderByDescending(a => a.DataDokumentu).FirstOrDefault();

                                    }
                                    if (docRef != null)
                                    {
                                        doc = new Dokument();
                                        doc.SAPImportStatus = 0;
                                        doc.DocGuid = Guid.NewGuid();
                                        doc.DataKsiegowania = dataPostan;
                                        doc.typFakt = "KR";
                                        doc.DataDokumentu = dataPostan;
                                        doc.DataPlatnosci = doc.DataDokumentu;
                                        doc.Stan = "B";
                                        spr.Dokument.Add(doc);
                                        doc.kwota = ks_start;
                                        Dluznik dluzn = spr.Dluznik.FirstOrDefault();
                                        dluzn.Dokument.Add(doc);
                                        doc.Sprawa_Id = spr.Id;
                                        doc.SAPDocId = docRef.SAPDocId;
                                        UpdateDocRaty(ref doc, ref kLst);
                                        CurrentTransfer.Dokument.Add(doc);
                                        //this.Context.Dokument.AddObject(doc);

                                    }
                                }
                                if (gLst.Count > 0)
                                {
                                    Dokument docgRef = this.Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "GS" || a.typFakt == "GP") && a.SAPDocId.Length > 5).OrderByDescending(a => a.DataDokumentu).FirstOrDefault();
                                    if (docgRef == null)
                                    {
                                        docgRef = this.Context.Dokument.Where(a => a.Sprawa_Id == spr.Id && (a.typFakt == "GS" || a.typFakt == "GP")).OrderByDescending(a => a.DataDokumentu).FirstOrDefault();

                                    } 
                                    if (docgRef != null)
                                    {
                                        doc = new Dokument();
                                        doc.SAPImportStatus = 0;
                                        doc.DocGuid = Guid.NewGuid();
                                        doc.typFakt = "GR";
                                        doc.DataKsiegowania = dataPostan;
                                        doc.DataDokumentu = dataPostan;
                                        doc.DataPlatnosci = doc.DataDokumentu;
                                        doc.Stan = "B";
                                        doc.kwota = grz_start;
                                        spr.Dokument.Add(doc);
                                        Dluznik dluzn = spr.Dluznik.FirstOrDefault();
                                        dluzn.Dokument.Add(doc);
                                        doc.Sprawa_Id = spr.Id;
                                        doc.SAPDocId = docgRef.SAPDocId;
                                        UpdateDocRaty(ref doc, ref gLst);
                                        CurrentTransfer.Dokument.Add(doc);
                                        //this.Context.Dokument.AddObject(doc);
                                        // wrzucić raty grzywny
                                        // ********
                                    }
                                }
                            }
                }
               
                    trans.LFaktow = loopcount;

                    Context.SaveChanges();



                }
            
            catch (Exception ex)
            {
                string msg = "Błąd ";
                // Print error message
                if (currentdtr != null)
                    msg = " Błąd przy pozycji dziennika " + currentdtr["Pozycja"].ToString() + "/" + currentdtr["Rok"];

                MessageBox.Show(msg + ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();

                if (con.State == ConnectionState.Open)
                    con.Close();
                Context.SaveChanges();
                breakIndicator = true;
            }
        }

        public void ImportKsiega(DateTime theNextDay)
        {
            SqlDataReader rdr = null;
            SqlConnection con = null;
            string CommandText = "";
            SqlCommand storedProcCommand; 
             DataTable dt = null ;
             try
             {
                
                 
                 {
                     string ConnectionString = Utils.BuildMyConnectionString(Context);
                     con = new SqlConnection(ConnectionString);
                     con.Open();
                     switch (Konfig.typKns)
                     {
                         case 0: // currenda
                             
                             storedProcCommand = new SqlCommand("sp_KsiegiCR", con);
                             //CommandText = "select ks.Id, rtrim(ks.skrot) + '  ' + kn.nazwa as Nazwa, sk.nazwa2 as Wydzial     from ksiegi_sady ks  inner join ksiegi_nazwy kn on kn.id = ks.id_nazwy inner join skor sk on sk.id = ks.id_sadu  where sys  = 0";
                             break;
                         case 1: // Zeto
                             
                             storedProcCommand = new SqlCommand("sp_Ksiegi", con);

                             break;
                         case 2:
                             //Orcom
                             storedProcCommand = new SqlCommand("sp_KsiegiOR", con);
                             break;

                         case 3:
                             //Albit
                         
                             storedProcCommand = new SqlCommand("sp_KsiegiAL", con);
                             break;
                         default:
                             storedProcCommand = null;
                             break;
                     }
                     storedProcCommand.CommandType = CommandType.StoredProcedure;
                     string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                     storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                     storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                     // storedProcCommand.Parameters.Add("@thenextday", theNextDay);
                     storedProcCommand.CommandTimeout = 600;
                     storedProcCommand.Connection = con;
                     rdr = storedProcCommand.ExecuteReader();
                     if (rdr.HasRows)
                     {
                         dt = new DataTable();
                         dt.Load(rdr);
                     }
                 }



                 if (dt != null)
                 {
                     foreach (DataRow dtr in dt.Rows)
                     {

                         KnsKsiegi ks = new KnsKsiegi();
                         ks.Id_Ksiegi = Convert.ToInt32(dtr["Id"]);
                         ks.wydzial = dtr["Wydzial"].ToString();
                         ks.wydzial = dtr["Wydzial"].ToString();
                         ks.nazwa = dtr["Nazwa"].ToString();

                         KnsKsiegi kks = Context.KnsKsiegi.Where(a => a.Id_Ksiegi == ks.Id_Ksiegi).FirstOrDefault();
                         if (kks == null)
                             Context.AddToKnsKsiegi(ks);
                         else
                         {
                             kks.wydzial = ks.wydzial;
                             kks.nazwa = ks.nazwa;
                         }

                         // (ks);
                     }
                     Context.SaveChanges();
                 }
             }

             catch (Exception ex)
             {
                 // Print error message
                 MessageBox.Show(ex.Message);
             }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con != null)
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            };




        }
        
        public void ImportSadWydz(DateTime theNextDay)
        {
            SqlDataReader rdr = null;
            SqlConnection con = null;
            string CommandText = "";
            SqlCommand storedProcCommand;
            DataTable dt = null;
            DataRow currentdtr = null;
            try
            {
                 
                 {
                string ConnectionString = Utils.BuildMyConnectionString(Context);
                con = new SqlConnection(ConnectionString);
                con.Open();

                switch (Konfig.typKns)
                {
                    case 0: // currenda
                        storedProcCommand = new SqlCommand("sp_SadWydzCR", con);

                        /*CommandText = "select s.id as Id, max(rtrim(isnull(s.nazwa,'')+ ' ' + isnull(s.nazwa2,'') +  isnull(', ' + s.miejsce,''))) as nazwa , count(*) as ile" +
                                     " from skor s inner join kns_sprawa spr on spr.id_sad = s.id " +
                                     " where ( select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r < @thenextday ) > 0 " +
                                     " or (select isnull(sum (kns_dz_nal.przypis_kosztow), 0) - isnull(sum (kns_dz_nal.uiszczenia_kostow), 0) - isnull(sum(kns_dz_nal.odpisanie_kosztow), 0) from kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r  < @thenextday )> 0 " +
                                      " group by s.id  order by ile desc ";
                       
                         */ 
                        break;
                    case 1: // Zeto
                    storedProcCommand = new SqlCommand("sp_SadWydz", con);    
                    
                    /*
                        CommandText = " select  slas.kod  as Id, max(rtrim(isnull(slas.nazwa,'')))  +  ' ' + max(rtrim(isnull(slas.nazwa1,''))) + ' ' + max(rtrim(isnull(slas.miejscowosc,''))) as nazwa, " +
                                      " count(*) as ile " +
                                       " from ( select nal.id_dluznik, sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis )   as grzywna, " +
                                       " sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis )	as koszty " +
                                       " from naleznosci_dziennik nal where  isnull(nal.data_operacji,nal.data_wprow_zapisu) < '2013-12-31'  and isnull ( nal.data_usun_zapisu,'2099-12-31') >  @thenextday " +
                                       " group by id_dluznik  " +
                                       "  having sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis ) > 0 or sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis ) > 0 " +
                                        " ) nals " +
                                      " LEFT JOIN dbo.Dluznik dlu  ON  dlu.id_dluznik=  nals.id_dluznik " +
                                      " LEFT JOIN dbo.DLUZNIK_SPRAWA_SADOWA dlss ON dlss.id_dluznik  = dlu.id_dluznik " +
                                      " LEFT Join dbo.SL_ADR_SADOW slas ON dlss.id_sad_obcy = slas.kod " +
                                      " where slas.kod is not null " +
                                      " group by slas.kod " +
                                      " order by ile desc "; */
                    break;
                    case 2: // Orcom
                    storedProcCommand = new SqlCommand("sp_SadWydzOR", con);       
                    break;
                    case 3: // Albit 
                    storedProcCommand = new SqlCommand("sp_SadWydzAL", con);
                    break;
                    default:
                        storedProcCommand = null;
                        break;
                }




                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@dataDo", theday);
                string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias ) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                storedProcCommand.CommandTimeout = 600;
                storedProcCommand.Connection = con;
                rdr = storedProcCommand.ExecuteReader();
                if (rdr.HasRows)
                {
                    dt = new DataTable();
                    dt.Load(rdr);
                }
                 } 
                 if (dt != null)
                 {
                    foreach (DataRow dtr in dt.Rows)
                    {

                        KnsSad ks = new KnsSad();
                        ks.Sad_Id = Convert.ToInt32(dtr["Id"]);
                        ks.Nazwa = dtr["Nazwa"].ToString();
                        ks.Nazwa = ks.Nazwa.Substring(0, ks.Nazwa.Length > 100 ? 100 : ks.Nazwa.Length);

                        KnsSad ksad = Context.KnsSad.Where(a => a.Sad_Id == ks.Sad_Id).FirstOrDefault();
                        if (ksad == null)
                            Context.AddToKnsSad(ks);
                        else
                        {
                            ksad.Nazwa = ks.Nazwa;
                        }

                        // (ks);
                    }
                    Context.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                // Print error message
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con != null)
                if (con.State == ConnectionState.Open)
                    con.Close();
                
            };



            
        }
        public void ImportKomornik(DateTime theNextDay)
        {
            SqlDataReader rdr = null;
            SqlConnection con = null;
            SqlCommand storedProcCommand;
            DataTable dt = null;
            try
            {
                // Open connection to the database
                string ConnectionString = Utils.BuildMyConnectionString(Context);
                con = new SqlConnection(ConnectionString);
                con.Open();

                switch (Konfig.typKns)
                {
                    case 0: // currenda
                        storedProcCommand = new SqlCommand("sp_KomornicyCR", con);

                        /*CommandText = "select s.id as Id, max(rtrim(isnull(s.nazwa,'')+ ' ' + isnull(s.nazwa2,'') +  isnull(', ' + s.miejsce,''))) as nazwa , count(*) as ile" +
                                     " from skor s inner join kns_sprawa spr on spr.id_sad = s.id " +
                                     " where ( select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r < @thenextday ) > 0 " +
                                     " or (select isnull(sum (kns_dz_nal.przypis_kosztow), 0) - isnull(sum (kns_dz_nal.uiszczenia_kostow), 0) - isnull(sum(kns_dz_nal.odpisanie_kosztow), 0) from kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r  < @thenextday )> 0 " +
                                      " group by s.id  order by ile desc ";
                       
                         */
                        break;
                    case 1: // Zeto
                        storedProcCommand = new SqlCommand("sp_Komornicy", con);

                        /*
                            CommandText = " select  slas.kod  as Id, max(rtrim(isnull(slas.nazwa,'')))  +  ' ' + max(rtrim(isnull(slas.nazwa1,''))) + ' ' + max(rtrim(isnull(slas.miejscowosc,''))) as nazwa, " +
                                          " count(*) as ile " +
                                           " from ( select nal.id_dluznik, sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis )   as grzywna, " +
                                           " sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis )	as koszty " +
                                           " from naleznosci_dziennik nal where  isnull(nal.data_operacji,nal.data_wprow_zapisu) < '2013-12-31'  and isnull ( nal.data_usun_zapisu,'2099-12-31') >  @thenextday " +
                                           " group by id_dluznik  " +
                                           "  having sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis ) > 0 or sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis ) > 0 " +
                                            " ) nals " +
                                          " LEFT JOIN dbo.Dluznik dlu  ON  dlu.id_dluznik=  nals.id_dluznik " +
                                          " LEFT JOIN dbo.DLUZNIK_SPRAWA_SADOWA dlss ON dlss.id_dluznik  = dlu.id_dluznik " +
                                          " LEFT Join dbo.SL_ADR_SADOW slas ON dlss.id_sad_obcy = slas.kod " +
                                          " where slas.kod is not null " +
                                          " group by slas.kod " +
                                          " order by ile desc "; */

                        break;
                    case 2: // Orcom
                        storedProcCommand = new SqlCommand("sp_Komornicy", con);
                        break;
                    case 3: // Albit
                        storedProcCommand = new SqlCommand("sp_KomornicyAL", con);
                        break;
                    default:
                        storedProcCommand = null;
                        break;
                }




                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@dataDo", theday);
                //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                storedProcCommand.CommandTimeout = 600;
                storedProcCommand.Connection = con;
                rdr = storedProcCommand.ExecuteReader();
                if (rdr.HasRows)
                {
                    dt.Load(rdr);
                    foreach (DataRow dtr in dt.Rows)
                    {

                        KnsKomornik ks = new KnsKomornik();
                        ks.Komornik_id = Convert.ToInt32(dtr["Id"]);
                        ks.Nazwa = dtr["nazwa"].ToString();
                        ks.Miasto = dtr["miasto"].ToString();
                        ks.Ulica = dtr["ulica"].ToString();

                        KnsKomornik ksad = Context.KnsKomornik.Where(a => a.Komornik_id == ks.Komornik_id).FirstOrDefault();
                        if (ksad == null)
                            Context.AddToKnsKomornik(ks);
                        else
                        {
                            ksad.Nazwa = ks.Nazwa;
                            ksad.Miasto = ks.Miasto;
                            ksad.Ulica = ks.Ulica;
                        }

                        // (ks);
                    }
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Print error message
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con.State == ConnectionState.Open)
                    con.Close();

            };




        }
    }
}
