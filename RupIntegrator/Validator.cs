using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Globalization;
using System.Windows.Forms;

namespace KnsMigrator
{
    class Validator
    {

        public KnsMigratorEntities thecontext { get; set; }
        public Guid myguid { get; set; }
        public string fileName { get; set; }

        public bool clearWalidTable()
        {

            Cursor.Current = Cursors.WaitCursor;
            this.thecontext.ExecuteStoreCommand("delete  from WalidSaldo ");
            this.thecontext.Refresh(System.Data.Objects.RefreshMode.StoreWins, thecontext.WalidSaldo);
            
            return true;
        
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
        private WalidSaldo  findSaldo(WalidSaldo ws, out int retcode)
        {
          List<WalidSaldo>  lst;
            WalidSaldo wsx ;
          lst = this.thecontext.WalidSaldo.Where(a => a.KartaDl == ws.KartaDl && a.Naleznosc == ws.Naleznosc && a.Klucz == this.myguid && a.Status == null).ToList();
          if (lst == null || lst.Count == 0) { retcode = -1; return null; }// brak dokumentu
          if (lst.Count == 1)
          {
              wsx = lst.FirstOrDefault();
              if (wsx.Kwota == ws.SAPKwota)
              {

                  retcode = 1;   // wszystko OK
                  return wsx;
              }
              else
              {
                  retcode = -2;  // rzobieżność kwot  
                  return wsx;
              }
          }
          else // wiecej niż jeden dokumen tego typu
          { 
          foreach ( WalidSaldo w in lst)
          {
              if (w.Kwota == ws.SAPKwota)
              {
                  retcode = 1;   // wszystko OK
                  return w;
              }
              retcode = -2;  // rzobieżność kwot  
              return null;
          }
          // brak takiej kwoty
          
          }
        // opuśić rekordy z rozliczeniami
          retcode = 0;
          return null;
        }

        private int findSaldoExtend(WalidSaldo ws)
        {
            List<WalidSaldo> lst;
            WalidSaldo wsx;
            lst = this.thecontext.WalidSaldo.Where(a => a.KartaDl == ws.KartaDl && a.Naleznosc == ws.Naleznosc && a.Klucz == this.myguid && a.Status == null).ToList();
            if (lst == null || lst.Count == 0) { return -1; }// brak dokumentu
            if (lst.Count == 1)
            {
                wsx = lst.FirstOrDefault();
                if (wsx.SAPKwota == null) wsx.SAPKwota = 0;
                    wsx.SAPKwota += ws.SAPKwota;
                    wsx.OpGlowna = ws.OpGlowna;
                    wsx.OpCzesc = ws.OpCzesc;
                    return 1;
               
            }
            else // wiecej niż jeden dokumen tego typu
            {
                foreach (WalidSaldo w in lst)
                {
                    if (w.SAPKwota == null) w.SAPKwota = 0;
                    if (w.Kwota  - ((w.SAPKwota == null) ? 0: w.SAPKwota) == ws.SAPKwota)
                    {
                        w.SAPKwota = (w.SAPKwota == null) ? ws.SAPKwota : w.SAPKwota + ws.SAPKwota;
                        w.OpGlowna = ws.OpGlowna;
                        w.OpCzesc = ws.OpCzesc;
                       return 1;
                    }
                 }
                
                // brak takiej kwoty -trzba rozbic pomiędzy istniejące
                foreach (WalidSaldo w in lst)
                {
                    if (w.SAPKwota == null) w.SAPKwota = 0;
                    if (w.Kwota -  w.SAPKwota <= 0) continue;
 
                    if (w.Kwota -  w.SAPKwota <= ws.SAPKwota)
                    {
                        ws.SAPKwota -= (w.Kwota - w.SAPKwota);
                        w.SAPKwota = w.Kwota;    
                        w.OpGlowna = ws.OpGlowna;
                        w.OpCzesc = ws.OpCzesc;
                        
                    }
                }
                if (ws.SAPKwota > 0)
                {
                    WalidSaldo wsl = lst.FirstOrDefault();
                    wsl.SAPKwota += ws.SAPKwota;
                    return 100;  // dodano na siłę

                }
                else
                    return 50; 
            }
      }


        private int findPrzypis(WalidSaldo ws)
        {
            List<WalidSaldo> lst;
            lst = this.thecontext.WalidSaldo.Where(a => a.KsiegaOpis == ws.KsiegaOpis && a.Klucz == this.myguid && a.Status == null).ToList();
            if (lst == null || lst.Count == 0) { return -1; }// brak dokumentu
            foreach (WalidSaldo w in lst)
            {
                w.Status = "OK";
            
            }
            this.thecontext.SaveChanges();
            return 1;
        }


        public bool ImportZPSCDDOKSPrzypisy(TextBox tbPos, TextBox tbMessage)
        {
            // import potwierdzeń
            int i = 0;
            string guid;
            FileInfo file = new FileInfo(fileName);
            int rCount, rNo;
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
            string kartadl;
            DateTime dksiegowania;

            rCount = 0;
            rNo = 0;
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

                            while (reader.Read())
                            {
                                kartadl = "";
                                tbPos.Text = (++rNo).ToString();
                                tbPos.Refresh();

                                kartadl = reader["Oznaczenie konta umowy"].ToString();
                                if (String.IsNullOrWhiteSpace(kartadl)) continue;  // pomijamy puste karty dłużnika
                                dksiegowania = Convert.ToDateTime(reader["Data księgowania"]);
                                dksiegowania = dksiegowania.Date;
                                Decimal kwota = reader["Kwota transakcji"] as decimal? ?? default(decimal);
                                if (kwota == 0)
                                {
                                    String kwt = reader["Kwota transakcji"].ToString();
                                    if (Decimal.TryParse(kwt.Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite, CultureInfo.InvariantCulture, out kwota)) ;
                                }
                                WalidSaldo ws = new WalidSaldo();
                                string opGlowna = reader["Operacja główna"].ToString();
                                string opczesc = reader["Operacja częściowa"].ToString();
                                string sygnatura = reader["Oznaczenie przedmiotu umowy"].ToString();
                                string pozfin = reader["Pozycja finansowa"].ToString();
                                string n1 = reader["Nazwisko / Nazwa 1"].ToString();
                                string n2 = reader["Imię / Nazwa 2"].ToString();
                                string docNo = reader["Numer dokumentu"].ToString();
                                string partner = reader["Partner biznesowy"].ToString();
                                string kontoUm = reader["Konto umowy"].ToString();
                                string PUm = reader["Przedmiot umowy"].ToString();
                                string dokNum = reader["Numer dokumentu"].ToString();

                                if (dokNum.Length < 12)
                                    dokNum = new String('0', 12 -dokNum.Length ) + dokNum;
                                ws.KartaDl = kartadl;
                                ws.OpGlowna = opGlowna;
                                ws.OpCzesc = opczesc;
                                ws.SAPKwota = kwota;
                                ws.Sygnatura = sygnatura;
                                ws.KsiegaOpis = dokNum;
                                ws.Klucz = myguid;
                                if (pozfin.Contains("D05") || (pozfin.Contains("D2970.01") && opczesc == "0011"))
                                    ws.Naleznosc = "grzywna";
                                else
                                    ws.Naleznosc = "koszty";

                                ws.Ksiega = 1000;  // zapisy z SAP'a n n n n n n n n n n n n n n n n n n n n n                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               nnnnnnnnnnnnnnnnnnnnnnnnnnnn.[;.bxv   bnvcfd4e3
                                ws.Dluznik = n1 + " " + n2;
                                findPrzypis(ws);
                                
                              

                            }


                            reader.Close();
                        }
                        con.Close();
                        tbMessage.Text = "Zapis wyniku walidacji ...";
                        tbMessage.Refresh();
                        this.thecontext.SaveChanges();
                        Cursor.Current = Cursors.Default;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Błąd podczas wczytywania zbioru z ZPSCDDOKS - sprawdź format " + ex.Message);

                return false;
            }
        }


        public bool ImportZPSCDDOKS(TextBox tbPos, TextBox tbMessage)
        {
            // import potwierdzeń
            int i = 0;
            string guid;
            FileInfo file = new FileInfo(fileName);
            int rCount, rNo;
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
            string kartadl;
            DateTime dksiegowania;

            rCount = 0;
            rNo = 0;
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
                             
                            while (reader.Read())
                            {
                                kartadl = "";
                                tbPos.Text = (++rNo).ToString();
                                tbPos.Refresh();
                               
                                string dokRozlicz = reader["Dokument rozliczenia"].ToString();
                                //if (!String.IsNullOrWhiteSpace(dokRozlicz)) continue;  // pomijamy części rozliczone
                                kartadl = reader["Oznaczenie konta umowy"].ToString();
                                if (String.IsNullOrWhiteSpace(kartadl)) continue;  // pomijamy puste karty dłużnika
                                dksiegowania = Convert.ToDateTime(reader["Data księgowania"]);
                                dksiegowania = dksiegowania.Date;
                                Decimal kwota = reader["Kwota transakcji"] as decimal? ?? default(decimal);
                                if (kwota == 0)
                                {
                                    String kwt = reader["Kwota transakcji"].ToString();
                                    if (Decimal.TryParse(kwt.Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite, CultureInfo.InvariantCulture, out kwota)) ;
                                }
                                WalidSaldo ws = new WalidSaldo();
                                string opGlowna = reader["Operacja główna"].ToString();
                                string opczesc = reader["Operacja częściowa"].ToString();
                                string sygnatura = reader["Oznaczenie przedmiotu umowy"].ToString();
                                string pozfin = reader["Pozycja finansowa"].ToString();
                                string n1 = reader["Nazwisko / Nazwa 1"].ToString();
                                string n2 = reader["Imię / Nazwa 2"].ToString();
                                string docNo = reader["Numer dokumentu"].ToString();
                                string partner = reader["Partner biznesowy"].ToString();
                                string kontoUm = reader["Konto umowy"].ToString();
                                string PUm = reader["Przedmiot umowy"].ToString();

                                ws.KartaDl = kartadl;
                                ws.OpGlowna = opGlowna;
                                ws.OpCzesc = opczesc;
                                ws.SAPKwota = kwota;
                                ws.Sygnatura = sygnatura;
                                ws.Klucz = myguid;
                               if (pozfin.Contains("D05") || (pozfin.Contains("D1080.01") && opczesc == "0011") )
                                    ws.Naleznosc = "grzywna";
                                else
                                    ws.Naleznosc = "koszty";

                                ws.Ksiega = 1000;  // zapisy z SAP'a n n n n n n n n n n n n n n n n n n n n n                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               nnnnnnnnnnnnnnnnnnnnnnnnnnnn.[;.bxv   bnvcfd4e3
                                ws.Dluznik = n1 + " " + n2;
                                if (findSaldoExtend(ws) < 0)
                                {
                                    ws.Status = "Brak pozycji lub zerowe saldo w systemie merytorycznym";
                                    this.thecontext.WalidSaldo.AddObject(ws);
                                
                                }
                                /*
                                int retcode;
                                WalidSaldo rec = findSaldo(ws, out retcode);
                                switch (retcode)
                                { 
                                    case -1:
                                        ws.Status = "Brak pozycji w systemie merytorycznym ";
                                        this.thecontext.WalidSaldo.AddObject(ws);
                                        break;
                                    case -2:
                                        if (rec != null)
                                        {
                                            rec.SAPKwota = ws.SAPKwota;
                                            rec.OpGlowna = ws.OpGlowna;
                                            rec.OpCzesc = ws.OpCzesc;
                                            rec.Status = "Róznica w wysokości salda";
                                        }
                                        else
                                        {
                                            ws.Status = "Nie skojarzono z pozycją na karcie dł.";  
                                            this.thecontext.WalidSaldo.AddObject(ws);
                                        }
                                            break;
                                    case 1:
                                            rec.SAPKwota = ws.SAPKwota;
                                            rec.OpGlowna = ws.OpGlowna;
                                            rec.OpCzesc = ws.OpCzesc;
                                            rec.Status = "OK";
                                            break;
                                        
                                            
                                }
                                 
                                this.thecontext.WalidSaldo.AddObject(ws);
                                //ws.KsiegaOpis = 
                                */



                            }
                        
                        
                        reader.Close();
                       }
                        con.Close();
                        tbMessage.Text = "Zapis wyniku walidacji ...";
                        tbMessage.Refresh();
                        this.thecontext.SaveChanges();
                        Cursor.Current = Cursors.Default;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;    
                MessageBox.Show("Błąd podczas wczytywania zbioru z ZPSCDDOKS - sprawdź format " + ex.Message);
                
                return false;
            }
        }



    }
}
