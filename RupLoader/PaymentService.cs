using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using System.Data;
using Telerik.WinControls.UI;
using System.Globalization;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Data.Common;
using System.Text.RegularExpressions;
using Ex2PscdInterface.Ex2PscdPaymentListQueryInService;

namespace RupLoader
{

    class PaymentService
    {

        public List<Dokument> dokLst { get; set; }
        private int curRow;
        private DataTable inData;
        private DataTable CopyinData;
        private List<SAPRodzajSprawy> rodzSprLst;
        private List<RL_Konfig> knfList = RupDatabase.theContext.RL_Konfig.Where(a => a.rodzajDB < 2).ToList();
        private DataTable dtTbl = new DataTable();
        public class DokExtend : Dokument
        {
            public string d;
            public string c;
            public string b;
            public string o;


        }

        public PaymentService()
        {
            curRow = 0;
            dokLst = new List<Dokument>();

        }


        public void ParseTytul(RadGridView rgvWyciag)
        {
            Cursor.Current = Cursors.WaitCursor;

            foreach (GridViewRowInfo row in rgvWyciag.Rows)
            {
                string dokString = string.Empty;
                if (row.Cells["F12"] != null && row.Cells["F12"].Value != null && !String.IsNullOrWhiteSpace(row.Cells["F12"].Value.ToString()))
                { // jeśli jest dokument 5-tkowy to go dodaj. 

                    dokString = "D;" + row.Cells["F12"].Value.ToString().Trim();
                }
                RecognizeService rs = new RecognizeService();
                int rank = 0;
                if (rs.ParseTytul(row.Cells["F5"].Value.ToString(), out rank))
                {
                    string k;
                    string outStr = rs.recognCode;
                    foreach (string key in rs.keys)
                    {
                        k = key;
                        if (rs.recognCode == "K") k = k.Replace(" ", "");
                        outStr += ";" + k;
                    }

                    row.Cells["result"].Value = String.IsNullOrWhiteSpace(dokString) ? outStr : dokString + "|" + outStr;
                    try
                    {
                        row.Cells["Ranking"].Value = rank;
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }
                else
                    row.Cells["result"].Value = dokString;




            }
            Cursor.Current = Cursors.Default;
        }



        private int validateDoc(Dokument dok)
        {
            Dluznik dl;
            bool found = false;
            string typSad = Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 4000 ? "SR" : (Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 3000 ? "SO" : "SA");
            string typSadOryg = typSad;

            if (!String.IsNullOrWhiteSpace(RupDatabase.theConfig.StanowiskoFin) && Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) < 4000 && typSad != "SA")
            {
                typSad = "SF";
                typSadOryg = "SR";
            }

            dl = dok.Dluznik;
            // niezapisywać KNS 
            if (String.IsNullOrWhiteSpace(dl.SAPKontoPartnera) &&
                 (String.IsNullOrEmpty(dl.Miejscowosc) || String.IsNullOrEmpty(dl.Ulica) || String.IsNullOrEmpty(dl.NrDomu) || String.IsNullOrEmpty(dl.KodPocztowy))
                 && ((dl.FizPraw.Trim() == "" && String.IsNullOrWhiteSpace(dl.Pesel)) || ((dl.FizPraw.Trim().ToUpper() == "X" && String.IsNullOrWhiteSpace(dl.Nip))))) return -1;
            // porównanie reperrtorium z typem sprawy 
            if (String.IsNullOrWhiteSpace(dok.Sprawa.SAPRepertorium) || String.IsNullOrWhiteSpace(dok.Sprawa.SAPRodzajSprawy) || String.IsNullOrWhiteSpace(dok.Sprawa.SAPRodzajPrzedmiotuUmowy)) return -2;
            List<SAPRodzajSprawy> sprLst = (from c in RupDatabase.theContext.SAPRodzajSprawy where c.typSad == typSadOryg && c.repertorium == dok.Sprawa.SAPRepertorium select c).ToList();
            SAPRepertorium rp = (from c in RupDatabase.theContext.SAPRepertorium where c.kod == dok.Sprawa.SAPRepertorium select c).FirstOrDefault();
            if (sprLst == null || sprLst.Count == 0 || rp == null) return -3; // brak repertorium lub rodzaju spraw
            foreach (SAPRodzajSprawy sprspr in sprLst)
            {
                if (sprspr.kod == dok.Sprawa.SAPRodzajSprawy && rp.SymbolRodzajPrzedmiotu == dok.Sprawa.SAPRodzajPrzedmiotuUmowy)
                    found = true;

            }
            if (!found) return -4;
            return 0;

        }

        private bool compareDlu(Dluznik dl, Dluznik dlx)
        {
            if (!String.IsNullOrWhiteSpace(dl.Nip) && !String.IsNullOrWhiteSpace(dlx.Nip) && dl.Nip == dlx.Nip) return true;
            if (dl.Imie.DoTrim() == dlx.Imie.DoTrim() && dl.Nazwisko.DoTrim() == dlx.Nazwisko.DoTrim() && dl.Miejscowosc.DoTrim() == dlx.Miejscowosc.DoTrim() && dl.KodPocztowy.DoTrim() == dlx.KodPocztowy.DoTrim() && dl.Ulica.DoTrim() == dlx.Ulica.DoTrim() && dl.NrDomu.DoTrim() == dlx.NrDomu.DoTrim() && dl.NrMieszkania.DoTrim() == dlx.NrMieszkania.DoTrim())
                return true;
            return false;

        }
        private void CreateSchema()
        {
            FileInfo file = new FileInfo(RunMode.fileName);

            try
            {
                using (System.IO.StreamWriter schema = new System.IO.StreamWriter(file.DirectoryName + "\\schema.ini"))
                {
                    schema.WriteLine("[" + file.Name + "]");
                    schema.WriteLine("Format=Delimited(|)");
                    schema.WriteLine("ColNameHeader=False");
                    schema.WriteLine("MaxScanRows=0");
                    schema.WriteLine("CharacterSet=1250");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu zbioru schema.ini  w folderze " + file.DirectoryName + "  " + ex.Message);
            }


        }

        public bool updateDocs()
        {
            try
            {
                foreach (Dokument dok in dokLst)
                {
                    if (dok.SAPImportStatus == 1)
                    { // zapisz 
                        string key;
                        if (dok.Info.IndexOf('#') > 0)
                            key = dok.Info.Substring(0, dok.Info.IndexOf('#') - 1);
                        else
                            key = dok.Info;
                        Transfer trn = (from c in RupDatabase.theContext.Transfer where c.rodzaj == 1001 && c.Uwagi == key orderby c.Id descending select c).FirstOrDefault();
                        if (trn != null)
                        {
                            trn.LFaktow += 1;
                            if (trn.DataDo < DateTime.Today) trn.DataDo = DateTime.Today;
                            if (trn.DataOd > DateTime.Today) trn.DataOd = DateTime.Today;

                        }
                        else
                        {
                            trn = new Transfer();
                            trn.rodzaj = 1001; // wyciąg bankowy
                            trn.LFaktow = 1;
                            trn.Uwagi = key;
                            trn.DataDo = DateTime.Today;
                            trn.DataOd = DateTime.Today;
                            trn.DataTransferu = DateTime.Now;

                            RupDatabase.theContext.Transfer.AddObject(trn);

                        }
                        trn.Dokument.Add(dok);
                        RupDatabase.theContext.SaveChanges();
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Błąd " + ex.Message + " " + ((ex.InnerException == null) ? "" : ex.InnerException.Message));
                return false;

            }
            return true;
        }
        public void writeAllContent(TextBox tbStep, TextBox tbItem)
        {
            DataColumn dt1;
            bool done;
            done = false;


            foreach (Dokument dok in dokLst)
            {
                string s;
                tbStep.Text = dok.Info;
                tbStep.Refresh();
                if (dok.SAPRodzajDokumentu == "NS")  // mależności sądowych nie rozpoznajemy
                    continue;

                ExportPI exp = new ExportPI();
                s = exp.DoExport(dok, 0, false, tbItem);
                if (s != null)
                {
                    dok.SAPImportStatus = 1;
                    done = true;
                }
                else
                { dok.SAPImportStatus = -1;

                }

            }
            updateDocs();
            if (done)
            {


                this.flushFile();
                MessageBox.Show("Zbiór z rozpoznanymi pozycjami został zapisany. Zamknij aplikację, przejdź do ZDB i pobierz go ");
            }
            else
            {
                MessageBox.Show("Wystąpił co najmniej jeden błąd podczas zapisu danych w ZSRK");
            }

        }


        public void writePartContent(string key, TextBox tbStep, TextBox tbItem)
        {
            DataColumn dt1;
            bool done;
            done = false;
            List<Dokument> dlList;

            dlList = (from c in dokLst where c.Info == key select c).ToList();

            foreach (Dokument dok in dlList)
            {
                string s;
                tbStep.Text = dok.Info;
                ExportPI exp = new ExportPI();
                s = exp.DoExport(dok, 0, true, tbItem);
                if (s != null)
                {
                    dok.SAPImportStatus = 1;
                    done = true;
                }
                else
                { dok.SAPImportStatus = -1; }

            }
            updateDocs();
            if (done)
            {
                MessageBox.Show("Dane zostały pomyślnie zapisane w ZSRK");
            }
        }


        public void reloadResultGrid(string key, RadGridView rgv)
        {
            if (dokLst != null && dokLst.Count > 0)
            {

                List<Dokument> ds = (from c in dokLst where c.Info == key select c).ToList();

                rgv.DataSource = ds;
                foreach (GridViewRowInfo row in rgv.Rows)
                {
                    row.Cells["dd"].Value = "D";
                    row.Cells["cc"].Value = "C";
                    row.Cells["oo"].Value = "O";
                    row.Cells["bb"].Value = "B";
                    row.Cells["Separator"].Value = "||";

                }
            }
        }


        private Dluznik getByIban(string IBAN)
        {
            Dluznik dl = null;
            if (!String.IsNullOrEmpty(IBAN))
            {
                string acc = IBAN.Replace(" ", "").Trim();
                dl = (from c in RupDatabase.theContext.Dluznik where c.Iban == IBAN && c.SAPKontoPartnera != null orderby c.Id descending select c).FirstOrDefault();
                if (dl == null)
                {
                    if (acc.Length == 28)
                    {  // poszukiwanie po nrb
                        acc = acc.Substring(2);
                        dl = (from c in RupDatabase.theContext.Dluznik where c.Iban == IBAN && c.SAPKontoPartnera != null orderby c.Id descending select c).FirstOrDefault();

                    }
                }



            }

            return dl;
        }

        private Dluznik getByIdDanych(long idDanych)
        {
            Dluznik dl = null;
            if (idDanych > 0)
            {
                dl = (from c in RupDatabase.theContext.Dluznik where c.IdSrcDane == idDanych && c.SAPKontoPartnera != null orderby c.Id descending select c).FirstOrDefault();

            }

            return dl;
        }


        private MapPartner getForMap(long idDanych, string NIP, string Pesel, string Nazwa, string Imie)
        {
            MapPartner dl = null;
            //RupDatabase.theContext.di
            if (idDanych > 0)
            {
                dl = (from c in RupDatabase.theContext.MapPartner where c.IdDanych == idDanych && c.SAPPartner != null orderby c.Id descending select c).FirstOrDefault();

            }

            return dl;
        }

        public void AttachTableAsDatatSource(RadGridView rgvSearch)
        {

            rgvSearch.DataSource = this.dtTbl;
        }


        public void ClearDataSource()
        {

            if (dtTbl != null)
                dtTbl.Clear();

        }

        public string DoSearchEx(string thekey, RadGridView rgvSearch, string IdList, bool isMassMode = false, uint ranking = 0)
        {
            string wydzial = "";
            string repertorium = "";
            int numer = 0;
            int rok = 0;
            string orygrep = "";
            string sadout = "";

            if (thekey.Substring(0, 1) == "S")
            {
                Utils.ParseSygn(thekey.Substring(2), out wydzial, out repertorium, out numer, out rok, out orygrep, out sadout);
                if (numer == 0 && rok == 0)
                {
                    MessageBox.Show("Błąd rozpoznawania sygnatury");
                    return null;

                }
                else
                    repertorium = orygrep;

            }

            Cursor.Current = Cursors.WaitCursor;
            try
            {

                if (!isMassMode && rgvSearch.Rows.Count > 0)
                {
                    rgvSearch.DataSource = null;
                    dtTbl.Clear();
                }

                int i = 1;
                foreach (RL_Konfig knf in knfList)
                {

                    if (thekey.Substring(0, 1) == "S" && !String.IsNullOrWhiteSpace(wydzial))
                    {
                        List<String> listStrLineElements = null;

                        if (knf.ERPLogon != null)
                            listStrLineElements = (knf.ERPLogon.Replace(" ", "").ToUpper()).Split(';').ToList();
                        if (listStrLineElements == null || listStrLineElements.Count == 0 || listStrLineElements.Contains(wydzial.ToUpper()))
                        {

                            IdList += (IdList.Length > 0 ? "," : "") + DoSearch(thekey, rgvSearch, wydzial, repertorium, numer, rok, knf, i > 0 ? 0 : 1, IdList, ranking);
                        }
                    }
                    else
                    {

                        IdList += (IdList.Length > 0 ? "," : "") + DoSearch(thekey, rgvSearch, wydzial, repertorium, numer, rok, knf, i > 0 ? 0 : 1, IdList, ranking);
                    }
                    i--;   // 
                }
                if (!isMassMode)
                {
                    rgvSearch.DataSource = dtTbl;
                    if (dtTbl.Rows.Count > 0)
                    {

                        rgvSearch.Columns["sygnatura"].ReadOnly = true;

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd " + ex.Message + " " + ((ex.InnerException == null) ? "" : ex.InnerException.Message));

            }
            Cursor.Current = Cursors.Default;
            return IdList;
        }

        private string getWydzIds(string s, int knfId)
        {
            string outstring = "";
            List<string> lst = new List<string>();
            lst = s.Split(',').ToList();
            lst = lst.Select(x => x).Distinct().ToList();
            foreach (string ss in lst)
            {
                if (!String.IsNullOrWhiteSpace(ss))
                {
                    long i;
                    i = Convert.ToInt32(ss);
                    if ((i / 10000000) as long? == (long)knfId)
                    {
                        outstring += (outstring.Length > 0 ? "," : "") + (i % 10000000).ToString();

                    }


                }


            }
            return outstring;
        }

        private string DoSearch(string thekey, RadGridView rgvSearch, string wydzial, string repertorium, int numer, int rok, RL_Konfig knf, int skipkns, string idList, uint ranking = 0)
        {
            SqlDataReader rdr = null;
            SqlConnection con = null;
            SqlCommand storedProcCommand;
            List<string> l = new List<string>();

            try
            {
                // Open connection to the database

                l = idList.Split(',').Distinct().ToList();

                string ConnectionString = ConfigurationManager.ConnectionStrings["RupLoader.Properties.Settings.RupDB"].ConnectionString;
                con = new SqlConnection(ConnectionString);
                //con.Open();
                if (knf == null)
                    knf = (from c in RupDatabase.theContext.RL_Konfig select c).FirstOrDefault();
                if (String.IsNullOrWhiteSpace(knf.sp_name))
                    switch (knf.typDB)
                    {
                        case 0: // currenda
                            storedProcCommand = new SqlCommand("sp_RozpoznajPrzelewCR", con);
                            break;
                        case 1: // Zeto
                            storedProcCommand = new SqlCommand("sp_RozpoznajPrzelew", con);
                            break;
                        case 2: // Zeto
                            storedProcCommand = new SqlCommand("sp_RozpoznajPrzelewOR", con);
                            break;
                        case 3: // Zeto
                            storedProcCommand = new SqlCommand("sp_RozpoznajPrzelewAL", con);
                            break;
                        default:
                            return "";
                    }
                else
                    storedProcCommand = new SqlCommand(knf.sp_name, con);


                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(knf.srvAlias) ? knf.srvName : knf.srvAlias));
                storedProcCommand.Parameters.Add("@dbname", knf.DbName);
                storedProcCommand.Parameters.Add("@key", thekey);
                storedProcCommand.Parameters.Add("@wydzial", wydzial);
                storedProcCommand.Parameters.Add("@repertorium", repertorium);
                storedProcCommand.Parameters.Add("@numer", numer);
                storedProcCommand.Parameters.Add("@rok", rok);
                storedProcCommand.Parameters.Add("@skipkns", skipkns);
                storedProcCommand.Parameters.Add("@idList", getWydzIds(idList, knf.id));
                storedProcCommand.Parameters.Add("@mode", String.IsNullOrWhiteSpace(RunMode.data) ? "" : RunMode.fileName.Replace("/", ""));

                storedProcCommand.CommandTimeout = 600;
                storedProcCommand.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter();

                da.SelectCommand = storedProcCommand;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                dt.Columns.Add("Ranking", typeof(System.Int32));

                if (dt.Rows.Count > 0)
                {
                    if (dtTbl.Rows.Count == 0) dtTbl = dt.Clone();

                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Ranking"] = ranking;
                        dtTbl.Rows.Add(dr.ItemArray);
                        l.Add(((dr["IdSprawy"] as int?) + knf.id * 10000000).ToString());

                    }
                }


            }
            catch (Exception ex)
            {
                // Print error message
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return string.Join(",", l.Distinct().ToArray());

        }


        public bool addResultRow(string key, GridViewRowInfo theRow, string IBAN, decimal kwota, string zleceniodawca)
        {
            Dluznik dl;
            Dokument dok = new Dokument();
            this.curRow += 1;
            dok.id = this.curRow;
            string typSad = Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 4000 ? "SR" : (Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 3000 ? "SO" : "SA");
            string typSadOryg = typSad;
            if (!String.IsNullOrWhiteSpace(RupDatabase.theConfig.StanowiskoFin) && Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) < 4000 && typSad != "SA")
            {
                typSad = "SF";
                typSadOryg = "SR";
            }
            Sprawa spr;

            try
            {
                dl = new Dluznik();
                if (!String.IsNullOrEmpty(theRow.Cells["typPartnera"].Value.ToString().Trim()))
                    dl.FizPraw = theRow.Cells["typPartnera"].Value.ToString();
                else
                    dl.FizPraw = "";
                dl.Imie = theRow.Cells["Nazwa2"].Value.ToString();
                dl.Nazwisko = theRow.Cells["Nazwa1"].Value.ToString();
                dl.KnsDluz_Id = Convert.ToInt32(theRow.Cells["IdStrony"].Value);
                dl.IdSrcDane = Convert.ToInt64(theRow.Cells["idDanychStrony"].Value);
                // if ( theRow.Cells["idDanychStrony"].Value
                if (dl.FizPraw == "X") // jesli osoba prawna - podziel nazwę 
                {
                    dl.Imie = theRow.Cells["Nazwa1"].Value.ToString();
                    dl.Nazwisko = theRow.Cells["Nazwa2"].Value.ToString();

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
                // dodanie IBAN jeśli się

                dl.Ulica = theRow.Cells["ulica"].Value.ToString();
                dl.NrDomu = theRow.Cells["nr_domu"].Value.ToString();
                dl.NrMieszkania = theRow.Cells["nr_mieszkania"].Value.ToString();
                dl.NrMieszkania = (!String.IsNullOrWhiteSpace(dl.NrMieszkania) ? dl.NrMieszkania = dl.NrMieszkania.Trim().Truncate(10) : "");
                dl.NrDomu = (!String.IsNullOrWhiteSpace(dl.NrDomu) ? dl.NrDomu = dl.NrDomu.Trim().Truncate(10) : "");

                dl.Pesel = theRow.Cells["pesel"].Value.ToString().Trim();
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
                dl.Nip = Utils.cleanNIP(theRow.Cells["nip"].Value.ToString().Trim());
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



                dl.KodPocztowy = theRow.Cells["kod"].Value.ToString().Trim();
                if (dl.KodPocztowy.Length == 5 && !dl.KodPocztowy.Contains("-"))
                    dl.KodPocztowy = dl.KodPocztowy.Substring(0, 2) + "-" + dl.KodPocztowy.Substring(2, 3);
                dl.Miejscowosc = theRow.Cells["miejscowosc"].Value.ToString();
                {
                    string kk = theRow.Cells["kraj"].Value.ToString().Trim().ToUpper();
                    if (kk != "PL")
                    {
                        SAPKodKraju kdkr;

                        kdkr = (from m in RupDatabase.theContext.SAPKodKraju
                                where m.kraj.ToUpper() == kk
                                select m).FirstOrDefault();
                        if (kdkr != null)
                        {
                            dl.KluczKraju = kdkr.kod;

                        }
                        else
                        {
                            dl.KluczKraju = "PL";

                        }
                    }
                    else
                        dl.KluczKraju = kk;

                }

                dl.Iban = theRow.Cells["IBAN"].Value.ToString();
                dl.RBN = theRow.Cells["RBN"].Value.ToString();
                if (String.IsNullOrEmpty(dl.Iban))
                {
                    if ((dl.FizPraw != "X" && zleceniodawca.ToUpper().Contains(dl.Imie.ToUpper()) && zleceniodawca.ToUpper().Contains(dl.Nazwisko.ToUpper())) || dl.FizPraw == "X")
                    {
                        dl.Iban = IBAN;

                    }
                }

                if (string.IsNullOrEmpty(dl.RBN) || string.IsNullOrWhiteSpace(dl.RBN))
                {
                    if (dl.FizPraw == "X")
                        dl.RBN = "08";
                    else
                        dl.RBN = "09";

                }
                dl.SAPKontoPartnera = theRow.Cells["NumerPartnera"].Value.ToString();

                spr = new Sprawa();

                spr.KnsSprawa_id = Convert.ToInt32(theRow.Cells["IdSprawy"].Value);
                spr.KnsKsiega = Convert.ToInt32(theRow.Cells["Ksiega"] == null ? 0 : theRow.Cells["Ksiega"].Value);
                spr.KNSSadOrzek_id = null;
                spr.Karta = theRow.Cells["OznKontaUmowy"].Value.ToString().Trim();  // karta dłużnika
                spr.SAPKontoUmowy = theRow.Cells["KontoUmowy"].Value.ToString();
                spr.SAPPrzedmiotUmowy = theRow.Cells["PrzedmiotUmowy"].Value.ToString();

                if (theRow.Cells["TypKontaUmowy"] != null && !String.IsNullOrEmpty(theRow.Cells["TypKontaUmowy"].Value.ToString()))
                {
                    spr.SAPTypKontaUmowy = theRow.Cells["TypKontaUmowy"].Value.ToString();
                }
                else
                {

                    spr.SAPTypKontaUmowy = "DO";
                }



                spr.SAPWydział = theRow.Cells["kodWydzial"].Value.ToString().Trim();
                spr.SAPRepertorium = theRow.Cells["repertorium"].Value.ToString().Trim().ToUpper();
                spr.Rok = Convert.ToInt32(theRow.Cells["rok"].Value);
                spr.Numer = Convert.ToInt32(theRow.Cells["nr"].Value);
                spr.SAPSadId = !String.IsNullOrEmpty(RupDatabase.theConfig.StanowiskoFin.DoTrim()) ? RupDatabase.theConfig.StanowiskoFin : RupDatabase.theConfig.JednostkaGospodarcza;

                // sprawdzamy czy mamy już taką sprawę
                {
                    List<Sprawa> sprxL;
                    sprxL = RupDatabase.theContext.Sprawa.Include("Dluznik").Where(a => a.SAPSadId == spr.SAPSadId && a.SAPWydział == spr.SAPWydział && a.Rok == spr.Rok && a.Numer == spr.Numer && a.SAPRepertorium == spr.SAPRepertorium &&
                                                                    a.SAPPrzedmiotUmowy != null && a.SAPTypKontaUmowy == spr.SAPTypKontaUmowy).OrderByDescending(a => a.Id).ToList();
                    Sprawa sprx = (from x in sprxL
                                   where x.Dluznik.Any(t => t.KnsDluz_Id == dl.KnsDluz_Id && dl.SAPKontoPartnera != null)
                                   select x).FirstOrDefault();
                    if (sprx != null)
                    {
                        spr.SAPKontoUmowy = sprx.SAPKontoUmowy;
                        spr.SAPPrzedmiotUmowy = sprx.SAPPrzedmiotUmowy;
                        if (spr.SAPTypKontaUmowy == "KN")    // jeśli kns w tej sprawie.
                            dl.SAPKontoPartnera = sprx.Dluznik.FirstOrDefault().SAPKontoPartnera;
                        else
                            if ((!String.IsNullOrWhiteSpace(dl.Pesel) && (dl.Pesel == sprx.Dluznik.FirstOrDefault().Pesel)) || (!String.IsNullOrWhiteSpace(dl.Nip) && (dl.Nip == sprx.Dluznik.FirstOrDefault().Nip)) || compareDlu(dl, sprx.Dluznik.FirstOrDefault()))
                            dl.SAPKontoPartnera = sprx.Dluznik.FirstOrDefault().SAPKontoPartnera;
                    }

                }
                if (theRow.Cells["RelacjaKonta"] != null && !String.IsNullOrEmpty(theRow.Cells["RelacjaKonta"].Value.ToString()))
                    spr.SAPRelacjaKontaUmowy = theRow.Cells["RelacjaKonta"].Value.ToString().Trim();
                else
                    switch (theRow.Cells["rola"].Value.ToString().ToUpper())
                    {
                        case "POWÓD":
                        case "WNIOSKODAWCA":
                            spr.SAPRelacjaKontaUmowy = "01";
                            break;
                        case "OSKARŻONY":
                        case "UCZESTNIK":
                            spr.SAPRelacjaKontaUmowy = "02";
                            break;
                        case "POZWANY":
                            spr.SAPRelacjaKontaUmowy = "03";
                            break;
                        case "ŚWIADEK":
                            spr.SAPRelacjaKontaUmowy = "04";
                            break;

                        default:
                            spr.SAPRelacjaKontaUmowy = "99";
                            break;

                    }

                //    if (String.IsNullOrEmpty(dl.Miejscowosc) || String.IsNullOrEmpty(dl.Ulica) || String.IsNullOrEmpty(dl.NrDomu) || String.IsNullOrEmpty(dl.KodPocztowy))
                //{
                Dluznik dlu;

                //      porównanie ze słownikiem


                dlu = getByIdDanych(dl.IdSrcDane.Value);

                //
                if (dlu == null) getByIban(IBAN);
                if (dlu != null)
                {
                    dl.Iban = IBAN;
                    dl.Miejscowosc = dlu.Miejscowosc;
                    dl.KodPocztowy = dlu.KodPocztowy;
                    dl.NrDomu = dlu.NrDomu;
                    dl.NrMieszkania = dlu.NrMieszkania;
                    dl.Ulica = dlu.Ulica;
                    dl.Nip = dlu.Nip;
                    dl.Pesel = dlu.Pesel;
                    dl.RBN = dlu.RBN;
                    dl.KluczKraju = dlu.KluczKraju;
                    dl.IdSrcDane = dlu.IdSrcDane;
                    if (dl.Nazwisko.Trim().ToUpper() == dlu.Nazwisko.Trim().ToUpper() && (dl.Imie.Trim().ToUpper() == dlu.Imie.Trim().ToUpper()))
                    {
                        dl.SAPKontoPartnera = dlu.SAPKontoPartnera;
                    }
                }

                //      }   

                // mn.Relacja_konta = dtr["Relacja konta"].ToString();  stał wartość  99
                //mn.Typ_konta_umowy = dtr["Typ konta umowy"].ToString();  KN, KN1 jeśli w ramach jednej sygnatury wystepuje kilka kart dłuBnika dla tego samego dłuBnika – dla kol;enych kart wartosci K1, K2…, K9
                spr.Sygnatura = theRow.Cells["sygnatura"].Value.ToString();
                SAPRepertorium repertorzek = (from e in RupDatabase.theContext.SAPRepertorium
                                              where e.kod.ToUpper() == spr.SAPRepertorium.ToUpper()
                                              select e).FirstOrDefault();
                if (repertorzek != null)
                {
                    spr.SAPRodzajPrzedmiotuUmowy = repertorzek.SymbolRodzajPrzedmiotu;

                }



                if (spr.SAPRepertorium.Length > 0)
                {
                    SAPRodzajSprawy rodzajSpr = (from f in RupDatabase.theContext.SAPRodzajSprawy where f.repertorium == spr.SAPRepertorium && f.typSad == typSadOryg orderby f.id select f).FirstOrDefault();
                    if (rodzajSpr != null)
                    {
                        spr.SAPRodzajSprawy = rodzajSpr.kod;

                    }
                }

                spr.SAPTomyAkt = "001";
                dok.SAPImportStatus = 0;
                dok.DocGuid = Guid.NewGuid();
                dok.KnsPozDzNal = 0;
                dok.kwota = (theRow.Cells["kwota"].Value == DBNull.Value ? 0 : Convert.ToDecimal(theRow.Cells["kwota"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")));
                try
                {
                    dok.referencja = theRow.Cells["referencja"].Value.ToString();
                    dok.tekst = theRow.Cells["tekst"].Value.ToString();
                }
                catch
                {; 
                }
                    dok.kwota = kwota;
                try
                {
                    dok.Opis = theRow.Cells["Opis"].Value.ToString();
                }
                catch (Exception ex1)
                {

                    ;

                }
                try
                {
                    dok.uwagi = theRow.Cells["uwagi"].Value.ToString();
                }
                catch (Exception ex1)
                {

                    ;

                }
                /*
                dok.DataDokumentu = theRow.Cells["DataDokumentu"].Value as DateTime? ?? null;
                if (theRow.Cells["DataKsiegowania"].Value != DBNull.Value)
                {
                    dok.DataKsiegowania = Convert.ToDateTime(theRow.Cells["DataKsiegowania"].Value);
                    dok.KnsRokDzNal = dok.DataKsiegowania.Value.Year;
                }
                */
                if (theRow.Cells["OperacjaGlowna"].Value != null && !String.IsNullOrEmpty(theRow.Cells["OperacjaGlowna"].Value.ToString()))
                    dok.OperacjaGlowna = theRow.Cells["OperacjaGlowna"].Value.ToString();
                else
                {
                    if (kwota < 0)
                        dok.OperacjaGlowna = "P020";
                    else
                        dok.OperacjaGlowna = "P010";
                }

                dok.grzSamoistna = "";

                if (theRow.Cells["OperacjaCzesciowa"].Value != DBNull.Value && !String.IsNullOrEmpty(theRow.Cells["OperacjaCzesciowa"].Value.ToString()))
                    dok.OperacjaCzesciowa = theRow.Cells["OperacjaCzesciowa"].Value.ToString();
                else
                {
                    if (theRow.Cells["rodzWydz"].Value.ToString() == "WK" || theRow.Cells["rodzWydz"].Value.ToString() == "EK")
                        dok.OperacjaCzesciowa = "0050";
                    else
                        dok.OperacjaCzesciowa = "0040";

                }
                if (theRow.Cells["RodzajDokumentu"].Value != DBNull.Value && !String.IsNullOrEmpty(theRow.Cells["RodzajDokumentu"].Value.ToString()))
                    dok.SAPRodzajDokumentu = theRow.Cells["RodzajDokumentu"].Value.ToString();
                else
                    dok.SAPRodzajDokumentu = "DN";

                //dok.DataPlatnosci = theRow.Cells["DataWymagalnosci"].Value as DateTime? ?? null;   // sprawdzić przy kposztach

                dok.typFakt = "WB";
                dok.Info = key;


                spr.Dluznik.Add(dl);
                if (dok != null)
                {

                    if (theRow.Cells["ZrodloDanych"].Value.ToString() == "KNS")
                        dok.SAPDocIdRef = theRow.Cells["NrDokumentu"].Value.ToString();
                    else
                        dok.SAPDocId = theRow.Cells["NrDokumentu"].Value.ToString();


                    spr.Dokument.Add(dok);
                    dl.Dokument.Add(dok);
                    dokLst.Add(dok);
                    // trans.Dokument.Add(dok);
                    // RupDatabase.theContext.Transfer.AddObject(trans);
                    // RupDatabase.theContext.SaveChanges();
                    //  return doc;
                    switch (validateDoc(dok))
                    {
                        case -1:
                            MessageBox.Show("Niekompletne dane dłużnika", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case -2:
                            MessageBox.Show("Niekompletne dane PU ( Sygnatury)", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case -3:
                            MessageBox.Show("Błędne repertorium  i/lub rodzaj sprawy", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case -4:
                            MessageBox.Show("Brak repertorium i/lub rodzaju sprawy w słowniku ZSRK", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;

                    }
                    return true;
                }
                // return null;
                return false;
            }


            catch (Exception ex)
            {
                //string s = CustomExtensions.ToTraceString(Context);

                MessageBox.Show("Błąd podczas zapisu dokumentu " + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
                return false;

            }

        }

        public void InitPozostaloGrid(RadGridView rgv)
        {
            try
            {
                foreach (GridViewRowInfo row in rgv.Rows)
                {

                    if (row.Cells["F11"].Value != null)
                        row.Cells["pozostalo"].Value = row.Cells["F11"].Value;

                }

                rgv.Refresh();

            }
            catch (Exception ex)
            {
                ;


            }
        }


        public void SetupGrid(RadGridView rgv, bool isMassMode = false)
        {

            HideEmptyColumns(rgv);
            rgv.Columns["F3"].IsVisible = false;
            nameColumns(rgv);
            //rgv.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;

            //rgv.Columns["F5"].BestFit();
            InsertColumns(rgv);
            //rgv.ShowColumnHeaders = false;
            rgv.EnableFiltering = true;
            rgv.MasterTemplate.EnableFiltering = true;
            rgv.Columns["F11"].FormatString = "{0:C}"; //"{0:#,###0.00;#,###0.00;0}";
            GridViewCommandColumn commandColumn = new GridViewCommandColumn();
            commandColumn.Name = "CommandColumn";
            commandColumn.MaxWidth = 50;
            commandColumn.MinWidth = 50;
            commandColumn.AllowResize = false;
            commandColumn.AllowReorder = false;
            commandColumn.UseDefaultText = true;
            commandColumn.FieldName = null;
            commandColumn.DefaultText = "Znajdź";
            commandColumn.HeaderText = "Znajdź";
            if (isMassMode)
            {
                commandColumn.IsVisible = false;

            }

            rgv.MasterTemplate.Columns.Insert(0, commandColumn);

            GridViewDecimalColumn pozoColumn = rgv.Columns["pozostalo"] as GridViewDecimalColumn;
            pozoColumn.MaxWidth = 70;
            pozoColumn.MinWidth = 70;
            pozoColumn.ReadOnly = true;
            pozoColumn.HeaderText = "Pozostało";
            pozoColumn.FormatString = "{0:C}";//"{0:#,###0.00;(#,###0.00);0}";
            if (isMassMode)
            {
                commandColumn.IsVisible = false;
                rgv.Columns["F12"].HeaderText = "Numer dokumentu";

            }
            // pozoColumn.ReadOnly = true;

            //  rgv.MasterTemplate.Columns.Insert(1, pozoColumn);

            ExpressionFormattingObject obj = new ExpressionFormattingObject("Cond1", "pozostalo = F11", false);
            obj.CellBackColor = Color.LightGray;
            obj.CellForeColor = Color.Black;

            pozoColumn.ConditionalFormattingObjectList.Add(obj);

            obj = new ExpressionFormattingObject("Cond2", "pozostalo = 0", false);
            obj.CellBackColor = Color.Green;
            obj.CellForeColor = Color.Black;
            pozoColumn.ConditionalFormattingObjectList.Add(obj);

            obj = new ExpressionFormattingObject("Cond3", "pozostalo > 0 and pozostalo < F11", false);
            obj.CellBackColor = Color.Yellow;
            obj.CellForeColor = Color.Black;
            pozoColumn.ConditionalFormattingObjectList.Add(obj);
            /*  obj = new ExpressionFormattingObject("Cond2", "SAPImportStatus < 0", false);
              obj.CellBackColor = Color.Red;
              obj.CellForeColor = Color.Black;
              rgv.Columns["SAPImportStatus"].ConditionalFormattingObjectList.Add(obj);
              obj= new ExpressionFormattingObject("Cond2", "SAPImportStatus > 0", false);
              obj.CellBackColor = Color.Green;
              obj.CellForeColor = Color.Black;
              rgv.Columns["SAPImportStatus"].ConditionalFormattingObjectList.Add(obj);
  */

        }
        private void InsertColumns(RadGridView rgv)
        {
            GridViewTextBoxColumn resultCol = new GridViewTextBoxColumn();

            resultCol.Name = "result";
            resultCol.HeaderText = "Klucz wyszukiwania";
            resultCol.IsVisible = true;
            resultCol.Width = 150;
            rgv.Columns.Insert(0, resultCol);

            GridViewTextBoxColumn resultColRanking = new GridViewTextBoxColumn();
            resultColRanking.Name = "Ranking";
            resultColRanking.HeaderText = "Ranking";
            resultColRanking.IsVisible = true;
            resultColRanking.Width = 50;
            rgv.Columns.Add(resultColRanking);



        }
        private void HideEmptyColumns(RadGridView rgv)
        {

            if (rgv.Rows.Count > 0)
            {
                foreach (GridViewCellInfo cell in rgv.Rows[0].Cells)
                {
                    if (cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()))
                    {
                        rgv.Columns[cell.ColumnInfo.Index].IsVisible = false;

                    }


                }

            }


        }
        private void nameColumns(RadGridView rgv)
        {

            rgv.Columns["F1"].HeaderText = "Id";
            rgv.Columns["F5"].HeaderText = "Tutułem";
            rgv.Columns["F7"].HeaderText = "Zleceniodawca";
            rgv.Columns["F9"].HeaderText = "Nr Rachunku";
            rgv.Columns["F11"].HeaderText = "Kwota transakcji";
            GridViewColumn c = rgv.Columns.Where(a => a.Name == "F10").FirstOrDefault();
            if (c != null)
                rgv.Columns["F10"].HeaderText = "Nr dokumentu";


        }
        private void flushFile()
        {
            StringBuilder sb = new StringBuilder();
            string key;



            foreach (DataRow row in CopyinData.Rows)
            {

                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.Append(string.Join("|", fields));

                key = row.ItemArray[0].ToString();
                List<Dokument> lst = (from x in dokLst where x.Info == key select x).ToList();
                int loop_no = 0;
                if (lst != null && lst.Count > 0)
                {
                    foreach (Dokument d in lst)
                    {
                        if (loop_no > 0)
                        {
                            sb.AppendLine("");
                            sb.Append(string.Join("|", fields));
                        }
                        sb.Append("||D " + (String.IsNullOrEmpty(d.SAPDocId) ? "" : d.SAPDocId));
                        sb.Append("||C " + (String.IsNullOrEmpty(d.Sprawa.SAPKontoUmowy) ? "" : d.Sprawa.SAPKontoUmowy));
                        sb.Append("||P " + (String.IsNullOrEmpty(d.Dluznik.SAPKontoPartnera) ? "" : d.Dluznik.SAPKontoPartnera));
                        sb.Append("||U " + (String.IsNullOrEmpty(d.Sprawa.SAPPrzedmiotUmowy) ? "" : d.Sprawa.SAPPrzedmiotUmowy));
                        sb.Append("||M " + (d.kwota != null ? d.kwota.ToString().Replace(",", ".") : ""));
                        sb.Append("||R " + (String.IsNullOrEmpty(d.SAPDocIdRef) ? "" : d.SAPDocIdRef));
                        sb.Append("||A " + (String.IsNullOrEmpty(d.OperacjaGlowna) ? "" : d.OperacjaGlowna));
                        sb.Append("||T " + (String.IsNullOrEmpty(d.OperacjaCzesciowa) ? "" : d.OperacjaCzesciowa));
                        sb.Append("||I " + (String.IsNullOrEmpty(d.Dluznik.Iban) ? "" : d.Dluznik.Iban));
                        sb.Append("||K " + (String.IsNullOrWhiteSpace(d.SAPDocIdRef) ? "0" : (RupDatabase.theConfig.czyautoks == 1 ? "1" : "0")));
                        loop_no++;
                    }
                }

                sb.AppendLine("");
            }

            FileInfo file = new FileInfo(RunMode.fileName);
            string outFileName = file.DirectoryName + "\\OK " + file.Name;

            File.WriteAllText(outFileName, sb.ToString());


        }
        /*
        private void flushFile()
        {
            StringBuilder sb = new StringBuilder();
            string key;

            foreach (DataRow row in CopyinData.Rows)
            {

                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.Append(string.Join("|", fields));

                key = row.ItemArray[0].ToString();
                List<Dokument> lst = (from x in dokLst where x.Info == key select x).ToList();
                if (lst != null && lst.Count > 0)
                {
                    foreach (Dokument d in lst)
                    {
                        sb.Append("||D " + (String.IsNullOrEmpty(d.SAPDocId) ? "" : d.SAPDocId));
                        sb.Append("||C " + (String.IsNullOrEmpty(d.Sprawa.SAPKontoUmowy) ? "" : d.Sprawa.SAPKontoUmowy));
                        sb.Append("||P " + (String.IsNullOrEmpty(d.Dluznik.SAPKontoPartnera) ? "" : d.Dluznik.SAPKontoPartnera));
                        sb.Append("||U " + (String.IsNullOrEmpty(d.Sprawa.SAPPrzedmiotUmowy) ? "" : d.Sprawa.SAPPrzedmiotUmowy));
                        sb.Append("||M " + (d.kwota != null ? d.kwota.ToString().Replace(",", ".") : ""));
                        sb.Append("||R " + (String.IsNullOrEmpty(d.SAPDocIdRef) ? "" : d.SAPDocIdRef));
                        sb.Append("||A " + (String.IsNullOrEmpty(d.OperacjaGlowna) ? "" : d.OperacjaGlowna));
                        sb.Append("||T " + (String.IsNullOrEmpty(d.OperacjaCzesciowa) ? "" : d.OperacjaCzesciowa));
                        sb.Append("||I " + (String.IsNullOrEmpty(d.Dluznik.Iban) ? "" : d.Dluznik.Iban));
                    }
                }

                sb.AppendLine("");
            }

            FileInfo file = new FileInfo(RunMode.fileName);
            string outFileName = file.DirectoryName + "\\OK " + file.Name;

            File.WriteAllText(outFileName, sb.ToString());


        }  */


        public void PrepareToPrint(RadGridView rgv)
        {
            DataTable dtPrint = new DataTable();
            dtPrint.Columns.Add("Id", typeof(string));
            dtPrint.Columns.Add("Tytulem", typeof(string));
            dtPrint.Columns.Add("Zleceniodawca", typeof(string));
            dtPrint.Columns.Add("Kwota", typeof(decimal));
            dtPrint.Columns.Add("Wydzial", typeof(string));
            dtPrint.Columns.Add("Sygnatura", typeof(string));
            dtPrint.Columns.Add("Uczestnik", typeof(string));
            dtPrint.Columns.Add("KwotaNa", typeof(decimal));
            dtPrint.Columns.Add("Uwagi", typeof(string));
            rgv.Columns["Kwota"].FormatString = "{0:#,###0.00;#,###0.00;0}";
            rgv.Columns["KwotaNa"].FormatString = "{0:#,###0.00;#,###0.00;0}";
            foreach (Dokument doc in dokLst)
            {
                if (doc.kwota > 0)
                {
                    DataRow dt = dtPrint.NewRow();
                    dt["Id"] = doc.Info;
                    var results = (from myRow in inData.AsEnumerable()
                                   where myRow.Field<string>("F1") == doc.Info
                                   select myRow).FirstOrDefault();
                    if (results != null)
                    {
                        dt["Tytulem"] = results["F5"];
                        dt["Zleceniodawca"] = results["F7"];
                        dt["Kwota"] = results["F11"];
                    }
                    dt["Wydzial"] = String.IsNullOrWhiteSpace(doc.SAPDocId) ? doc.Sprawa.SAPWydział : "KNS: " + doc.Sprawa.SAPWydział;
                    dt["Sygnatura"] = String.IsNullOrWhiteSpace(doc.SAPDocId) ? doc.Sprawa.Sygnatura : doc.Sprawa.Sygnatura + " " + doc.Sprawa.Karta;
                    dt["Uczestnik"] = doc.Dluznik.Imie + " " + doc.Dluznik.Nazwisko + " " + doc.Dluznik.Ulica + " " + doc.Dluznik.NrDomu + (String.IsNullOrWhiteSpace(doc.Dluznik.NrMieszkania) ? "" : "/" + doc.Dluznik.NrMieszkania) + " " + doc.Dluznik.Miejscowosc;
                    dt["KwotaNa"] = doc.kwota;
                    if (!String.IsNullOrWhiteSpace(doc.SAPDocId))
                        dt["Uwagi"] = (doc.typFakt == "GS" || doc.typFakt == "GP") ? "grzynwa" : "koszty";
                    dtPrint.Rows.Add(dt);
                }
            }
            rgv.DataSource = dtPrint;

        }



        public void deleteDoc(int key)
        {

            Dokument dok = (from c in dokLst where c.id == key select c).FirstOrDefault();
            if (dok != null) dokLst.Remove(dok);

        }

        private List<double> replaceSAPNegative(string filename)

        {
            List<double> values = new List<double>();

            string fileOut = "";
            string line = "";
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(filename);

                while ((line = file.ReadLine()) != null)
                {

                    line = Regex.Replace(line, @"(.{0,500})(\|\|)(\d{1,10}\.\d{2})(-{0,1})", @"$4$3");
                    values.Add(Convert.ToDouble(line.Replace(".", ",")));
                }

                file.Close();

                //File.WriteAllText(filename, fileOut,Encoding.);
                return values;
            }
            catch (Exception ex)
            {

                MessageBox.Show(" Błąd parsowania linii " + line);
                return null;
            }
        }


        public void SetWyciagData(RadGridView rgv, DataTable inData)
        {

            string theData = RunMode.data;
            int row_nr = 0;
            try
            {



                foreach (string row in theData.Split('\n'))
                {
                    row_nr++;
                    DataRow dataRow = inData.NewRow();
                    int i = 0;
                    // sprawdzenie liczby TAB w wierszu 
                    int fcount = row.Count(c => c == '\t');

                    if (fcount > 3)
                    {
                        // pełna linia
                        foreach (string cell in row.Split('\t'))
                        {
                            switch (++i)
                            {
                                case 1:
                                    dataRow["F1"] = cell;
                                    break;
                                case 2:
                                    dataRow["F5"] = cell;
                                    break;
                                case 3:
                                    dataRow["F7"] = cell;
                                    break;
                                case 5:
                                    dataRow["F9"] = cell;
                                    break;
                                case 6:
                                    if (String.IsNullOrWhiteSpace(cell))
                                        dataRow["F11"] = 0;
                                    else
                                    {
                                        decimal d;
                                        if (cell.Trim().Substring(cell.Trim().Length - 1, 1) == "-")
                                            d = -Convert.ToDecimal(cell.Replace(".", ",").Replace("-", ""), CultureInfo.GetCultureInfo("pl-PL"));
                                        else
                                            d = Convert.ToDecimal(cell.Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                                        dataRow["F11"] = d;
                                        dataRow["pozostalo"] = d;
                                    }
                                    break;
                            }


                        }
                    }
                    else
                    {
                        string[] arr = row.Split('\t');
                        // tylko tytułem 
                        if (arr.Length <= 2)
                        {
                            dataRow["F1"] = "900000001#" + row_nr.ToString();
                            dataRow["F5"] = arr[0];

                            // odnalezienie konta 
                            Regex r = new Regex(@"PL\d{26}");
                            Match m = r.Match(arr[0]);
                            if (m.Success)
                                dataRow["F9"] = m.Value;
                            dataRow["F11"] = 0;
                            dataRow["pozostalo"] = 0;
                            if (arr.Length == 2)
                                dataRow["F7"] = arr[1];
                        }

                    }

                    inData.Rows.Add(dataRow);
                }
                rgv.DataSource = inData;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Błąd Importu danych " + ex.Message);
            }


        }


        public void ZdobWyciagData(RadGridView rgv, DataTable inData, DataTable srcTable)
        {

            string theData = RunMode.data;
            int row_nr = 0;
            try
            {




                foreach (string row in theData.Split('\n'))
                {
                    row_nr++;
                    DataRow dataRow = inData.NewRow();
                    int i = 0;
                    // sprawdzenie liczby TAB w wierszu 
                    int fcount = row.Count(c => c == '\t');

                    if (fcount > 3)
                    {
                        // pełna linia
                        foreach (string cell in row.Split('\t'))
                        {
                            switch (++i)
                            {
                                case 1:
                                    dataRow["F1"] = cell;
                                    break;
                                case 2:
                                    dataRow["F5"] = cell;
                                    break;
                                case 3:
                                    dataRow["F7"] = cell;
                                    break;
                                case 5:
                                    dataRow["F9"] = cell;
                                    break;
                                case 6:
                                    if (String.IsNullOrWhiteSpace(cell))
                                        dataRow["F11"] = 0;
                                    else
                                    {
                                        decimal d;
                                        if (cell.Trim().Substring(cell.Trim().Length - 1, 1) == "-")
                                            d = -Convert.ToDecimal(cell.Replace(".", ",").Replace("-", ""), CultureInfo.GetCultureInfo("pl-PL"));
                                        else
                                            d = Convert.ToDecimal(cell.Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                                        dataRow["F11"] = d;
                                        dataRow["pozostalo"] = d;
                                    }
                                    break;
                            }


                        }
                    }
                    else
                    {
                        string[] arr = row.Split('\t');
                        // tylko tytułem 
                        if (arr.Length <= 2)
                        {
                            dataRow["F1"] = "900000001#" + row_nr.ToString();
                            dataRow["F5"] = arr[0];

                            // odnalezienie konta 
                            Regex r = new Regex(@"PL\d{26}");
                            Match m = r.Match(arr[0]);
                            if (m.Success)
                                dataRow["F9"] = m.Value;
                            dataRow["F11"] = 0;
                            dataRow["pozostalo"] = 0;
                            if (arr.Length == 2)
                                dataRow["F7"] = arr[1];
                        }

                    }

                    inData.Rows.Add(dataRow);
                }
                rgv.DataSource = inData;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Błąd Importu danych " + ex.Message);
            }


        }
        public DataTable Wyciag2Datatble(DataTable inData,  PaymentListQueryResponse values)
        {
            if (values == null)
                return inData;
            if (inData == null)
            {
               inData = new DataTable();

                DataColumn f1 = new DataColumn("F1");
                f1.DataType = System.Type.GetType("System.String");
                inData.Columns.Add(f1);

                DataColumn f3 = new DataColumn("F3");
                f3.DataType = System.Type.GetType("System.String");
                inData.Columns.Add(f3);

                DataColumn f5 = new DataColumn("F5");
                f5.DataType = System.Type.GetType("System.String");
                inData.Columns.Add(f5);

                DataColumn f7 = new DataColumn("F7");
                f7.DataType = System.Type.GetType("System.String");
                inData.Columns.Add(f7);

                DataColumn f9 = new DataColumn("F9");
                f9.DataType = System.Type.GetType("System.String");
                inData.Columns.Add(f9);

                DataColumn f11 = new DataColumn("F11");
                f11.DataType = System.Type.GetType("System.Decimal");
                inData.Columns.Add(f11);

                DataColumn pozo = new DataColumn();
                pozo.ColumnName = "pozostalo";
                pozo.DefaultValue = 0;
                pozo.DataType = System.Type.GetType("System.Decimal");
                inData.Columns.Add(pozo);




            }

            foreach (PozycjaWB pwb in values.PozycjaWB)
            {
                DataRow dataRow = inData.NewRow();
                decimal kwt = 0, kwtprzypis = 0;

                dataRow["F1"] = pwb.PartiaPlatnosciID + "|" + pwb.PartiaPlatnosciNrPozycja;
                dataRow["F3"] = pwb.TekstPlatnosci;
                dataRow["F5"] = pwb.TekstPlatnosci;
                dataRow["F7"] = pwb.Zleceniodawca;
                dataRow["F9"] = pwb.RachBankZleceniodawca.Kraj + pwb.RachBankZleceniodawca.NumerBanku + pwb.RachBankZleceniodawca.KodKontrolny + pwb.RachBankZleceniodawca.KontoBankowe;
                try
                {
                    kwt = Convert.ToDecimal(pwb.Kwota.Replace(",","."),CultureInfo.InvariantCulture);
                }
                catch { 
                }
                try
                {
                    kwtprzypis = Convert.ToDecimal(pwb.KwotaPrzypisana.Replace(",", "."), CultureInfo.InvariantCulture);
                }
                catch
                {
                }
                dataRow["F11"] = kwt;
                dataRow["pozostalo"] = kwt - kwtprzypis;
                inData.Rows.Add(dataRow);
            }

            return inData;


        }



        public void AttachCmdDataSource(RadGridView rgv, DataTable srcTable = null, bool isZdobFile = false)
        {




            DataTable inData = new DataTable();

            DataColumn f1 = new DataColumn("F1");
            f1.DataType = System.Type.GetType("System.String");
            inData.Columns.Add(f1);

            DataColumn f3 = new DataColumn("F3");
            f3.DataType = System.Type.GetType("System.String");
            inData.Columns.Add(f3);

            DataColumn f5 = new DataColumn("F5");
            f5.DataType = System.Type.GetType("System.String");
            inData.Columns.Add(f5);

            DataColumn f7 = new DataColumn("F7");
            f7.DataType = System.Type.GetType("System.String");
            inData.Columns.Add(f7);

            DataColumn f9 = new DataColumn("F9");
            f9.DataType = System.Type.GetType("System.String");
            inData.Columns.Add(f9);

            DataColumn f11 = new DataColumn("F11");
            f11.DataType = System.Type.GetType("System.Decimal");
            inData.Columns.Add(f11);

            DataColumn pozo = new DataColumn();
            pozo.ColumnName = "pozostalo";
            pozo.DefaultValue = 0;
            pozo.DataType = System.Type.GetType("System.Decimal");
            inData.Columns.Add(pozo);
            if (isZdobFile)
            {
                DataColumn f12 = new DataColumn("F12");
                f12.ColumnName = "F12";
                f12.DataType = System.Type.GetType("System.String");
                inData.Columns.Add(f12);

                if (srcTable != null)
                {
                    string poz = string.Empty;
                    string colname = string.Empty;
                    int i = 0;
                    int DokumentNrColName = 16;
                    int dColname = 0;

                    List<int> cLst = new List<int>();
                    int n = 0;
                    foreach (DataColumn x in srcTable.Columns)
                    {
                        if (x.ColumnName.StartsWith("Numer dokumentu"))
                            cLst.Add(n);
                        n++;  
                    }
                    
                    try
                    {

                        foreach (DataRow row in srcTable.Rows)
                        {
                          
                            i++;
                            decimal d = 0;
                            DataRow dataRow = inData.NewRow();
                            colname = "Pozycja " + " " + "Partia";
                            if (row["Pozycja"] == null || String.IsNullOrWhiteSpace(row["Pozycja"].ToString()))
                                continue;
                            dataRow["F1"] = row["Pozycja"].ToString() + "|" + row["Partia"].ToString();
                            colname = "Tekst";
                            dataRow["F3"] = row["Tekst"].ToString();
                            dataRow["F5"] = row["Tekst"].ToString();
                            colname = "Zleceniodawca";
                            dataRow["F7"] = row["Zleceniodawca"].ToString();
                            colname = "Kraj banku " + "Kod banku " + "Kod kontrolny banku " + "Konto bankowe";
                            dataRow["F9"] = row["Kraj banku"].ToString() + row["Kod banku"].ToString() + row["Kod kontrolny banku"].ToString() + row["Konto bankowe"].ToString();
                            colname = "Numer dokumentu";
                            if (dColname == 0)
                            {
                                foreach (int x in cLst)
                                    if (!String.IsNullOrWhiteSpace(row[x].ToString()))
                                    {
                                        dColname = x;
                                        break;

                                    }

                            }
                            dataRow["F12"] = row[dColname].ToString();
                            colname = "Kwota transakcji";
                            if (Decimal.TryParse(row["Kwota płatności"].ToString(), out d))
                            {
                                dataRow["F11"] = d;
                                dataRow["pozostalo"] = d;
                            }
                            inData.Rows.Add(dataRow);
                        }
                        rgv.DataSource = inData;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas importu w wierszu " + i.ToString() + " kolumna " + colname + " " + ex.Message, "Błąd podczcas importu ");
                    }
                }
                else
                    SetWyciagData(rgv, inData);
            }
        }




        

        public void AttachDataSource(RadGridView rgv, bool isZdobMode = false)
        {

            FileInfo file = new FileInfo(RunMode.fileName);
            List<double> kwt = null;
            CreateSchema();
            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName + "\";Extended Properties='text;HDR=No';"))
                //using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.DirectoryName ))
                {
                    using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", file.Name), con))
                    {
                        con.Open();


                        // Using a DataReader to process the data
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {


                            Cursor.Current = Cursors.WaitCursor;
                            inData = new DataTable();
                            inData.Load(reader);
                            kwt = replaceSAPNegative(RunMode.fileName);
                            
                            CopyinData = inData.Clone();
                            CopyinData.Columns["F11"].DataType = System.Type.GetType("System.String");
                            foreach (DataRow row in inData.Rows)
                            {
                                
                                CopyinData.ImportRow(row);
                                      
                            }
                            for (int i = 0 ; i < CopyinData.Rows.Count; i++)
                            {
                                string s;
                                inData.Rows[i][10] = kwt.ToArray()[i];
                                s = CopyinData.Rows[i][10].ToString().Replace(",", ".");

                                CopyinData.Rows[i][10] = s;

                            }
                            //row.Cells["pozostalo"].Value = row.Cells["F11"].Value;

                            DataColumn pozo = new DataColumn();
                            pozo.ColumnName = "pozostalo";
                            pozo.DefaultValue = 0;
                            pozo.DataType = System.Type.GetType("System.Decimal");
                            inData.Columns.Add(pozo);


                            rgv.DataSource = inData;


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd Importu wyciągu " + ex.Message);
            }

        }
    }
}

