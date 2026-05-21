using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Data;
using Telerik.WinControls.UI;

namespace RupLoader
{
    class Imports
    {

        public Dokument  ImportData(GridViewRowInfo theRow, int dokType, DateTime dksiegowania)
        {
                    Dokument doc;
                    string typSad = Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 4000 ? "SR" : (Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 3000 ? "SO" : "SA");
                    string typSadOryg = typSad;
                    if (!String.IsNullOrWhiteSpace(RupDatabase.theConfig.StanowiskoFin) && Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) < 4000 && typSad != "SA")
                    {
                        typSad = "SF";
                        typSadOryg = "SR";
                    }

                    Dluznik dl;
                    string errmsg;
                    Transfer   trans = new Transfer();
                    trans.DataTransferu = DateTime.Today;
                    trans.rodzaj = dokType;
            
                    trans.DataOd = DateTime.Today;
                    trans.DataDo = DateTime.Today;   // doccelowo podać datę 

                    if (trans.DataOd > dksiegowania) trans.DataOd = dksiegowania;
                    if (trans.DataDo < dksiegowania) trans.DataDo = dksiegowania;
                    
                    trans.Uwagi = "Dane podst.";
                    
                    int curKsiega = Convert.ToInt32(theRow.Cells["Ksiega"] == null  ? 0 : theRow.Cells["Ksiega"].Value);
                       
                        //(pForm.Controls["lbInfo"] as Label).Refresh();
                        errmsg = "";
                        doc = null;
                      
                    try  {

                        dl = new Dluznik();
                        if (!String.IsNullOrEmpty(theRow.Cells["Osoba fizyczna/Osoba prawna"].Value.ToString().Trim()))
                            dl.FizPraw = theRow.Cells["Osoba fizyczna/Osoba prawna"].Value.ToString();
                        else
                            dl.FizPraw = "";
                        dl.Imie = theRow.Cells["Imię/Nazwa 1"].Value.ToString();
                        dl.Nazwisko = theRow.Cells["Nazwisko / Nazwa 2"].Value.ToString();
                        dl.KnsDluz_Id = Convert.ToInt32(theRow.Cells["IdStrony"].Value) ;
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


                        dl.Ulica = theRow.Cells["Ulica"].Value.ToString();
                        dl.NrDomu = theRow.Cells["Nr domu"].Value.ToString();
                        dl.NrMieszkania = theRow.Cells["Nr mieszkania"].Value.ToString();
                        dl.Pesel = theRow.Cells["Pesel"].Value.ToString().Trim();
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
                        dl.Nip = Utils.cleanNIP(theRow.Cells["NIP"].Value.ToString().Trim());

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



                        dl.KodPocztowy = theRow.Cells["Kod pocztowy"].Value.ToString();
                        dl.Miejscowosc = theRow.Cells["Miejscowość"].Value.ToString();
                        {
                            string kk = theRow.Cells["Klucz kraju"].Value == null ? "PL" : theRow.Cells["Klucz kraju"].Value.ToString().Trim().ToUpper();
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
                                    dl.KluczKraju = "??";
                                    errmsg = "Nieokreślony kod kraju osoby";
                                }
                            }
                            else
                                dl.KluczKraju = kk;

                        }

                        dl.Iban = theRow.Cells["IBAN"].Value == null ? "" : theRow.Cells["IBAN"].Value.ToString();
                        dl.RBN = theRow.Cells["Kwalifikator do RBN"].Value == null ? null: theRow.Cells["Kwalifikator do RBN"].Value.ToString();
                        if (string.IsNullOrEmpty(dl.RBN) || string.IsNullOrWhiteSpace(dl.RBN))
                        {
                            if (dl.FizPraw == "X")
                                dl.RBN = "08";
                            else
                                dl.RBN = "09";

                        }
                        


                        Sprawa spr = new Sprawa();

                        spr.KnsSprawa_id = Convert.ToInt32(theRow.Cells["IdSprawy"].Value);
                        spr.KnsKsiega = Convert.ToInt32(theRow.Cells["Ksiega"] == null ? 0 : theRow.Cells["Ksiega"].Value);
                        spr.KnsSad = theRow.Cells["SadKns"].Value == null ? "":  theRow.Cells["SadKns"].Value.ToString().Trim();
                        spr.KNSSadOrzek_id = null;
                        spr.Karta = theRow.Cells["Oznaczenie konta umowy"].Value.ToString() == null ? "" : theRow.Cells["Oznaczenie konta umowy"].Value.ToString().Trim();  // karta dłużnika
                        if (theRow.Cells["Typ konta umowy"] != null && !String.IsNullOrEmpty(theRow.Cells["Typ konta umowy"].Value.ToString()))
                        {
                            spr.SAPTypKontaUmowy = theRow.Cells["Typ konta umowy"].Value.ToString();
                        }
                        else
                        {

                            spr.SAPTypKontaUmowy = "DO";
                        }
                        // sprawdzamy czy mamy już taką sprawę
                        {
                            List<Sprawa> sprxL;
                            sprxL = RupDatabase.theContext.Sprawa.Include("Dluznik").Where(a => a.KnsSprawa_id == spr.KnsSprawa_id && a.SAPPrzedmiotUmowy != null  &&  a.SAPTypKontaUmowy == spr.SAPTypKontaUmowy   ).OrderByDescending(a => a.Id).ToList();
                            Sprawa sprx =  (from x in sprxL 
                                           where x.Dluznik.Any(t=>t.KnsDluz_Id == dl.KnsDluz_Id && dl.SAPKontoPartnera != null )
                                           select x).FirstOrDefault();
                            if (sprx != null)
                            {
                                spr.SAPKontoUmowy = sprx.SAPKontoUmowy;
                                spr.SAPPrzedmiotUmowy = sprx.SAPPrzedmiotUmowy;
                                dl.SAPKontoPartnera = sprx.Dluznik.FirstOrDefault().SAPKontoPartnera;

                                

                            }
                        }

                        if (theRow.Cells["Relacja konta"] != null && !String.IsNullOrEmpty(theRow.Cells["Relacja konta"].Value.ToString()))
                            spr.SAPRelacjaKontaUmowy = theRow.Cells["Relacja konta"].Value.ToString().Trim();
                        else
                            spr.SAPRelacjaKontaUmowy = "99";

                        // mn.Relacja_konta = dtr["Relacja konta"].ToString();  stał wartość  99
                        //mn.Typ_konta_umowy = dtr["Typ konta umowy"].ToString();  KN, KN1 jeśli w ramach jednej sygnatury wystepuje kilka kart dłuBnika dla tego samego dłuBnika – dla kol;enych kart wartosci K1, K2…, K9
                        spr.SAPSadId =  RupDatabase.theConfig.JednostkaGospodarcza ;
                        spr.Sygnatura = theRow.Cells["sygnatura"].Value.ToString();
                        spr.SAPWydział  = theRow.Cells["Nr wydziału i sekcji"].Value.ToString();
                        spr.SAPRepertorium = theRow.Cells["repertorium"].Value.ToString();
                        spr.Numer  = Convert.ToInt32(theRow.Cells["Nr sprawy"].Value);
                        spr.Rok  =   Convert.ToInt32(theRow.Cells["Rok"].Value);
                        SAPRepertorium    repertorzek = (from e in RupDatabase.theContext.SAPRepertorium
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
                        // grzywna i koszty oddzielnie


                       
                            doc = new Dokument();
                            doc.SAPImportStatus = 0;
                            doc.DocGuid = Guid.NewGuid();
                            doc.KnsPozDzNal = 0;
                            doc.DataDokumentu = theRow.Cells["Data dokumentu grzywna"].Value as DateTime? ?? null;
                            doc.KnsKsiegaDzNal = curKsiega;

                            if (theRow.Cells["Data księgowania"].Value != DBNull.Value)
                            {
                                doc.DataKsiegowania = Convert.ToDateTime(theRow.Cells["Data księgowania"].Value);
                                doc.KnsRokDzNal = doc.DataKsiegowania.Value.Year;
                            }
                            if (theRow.Cells["Operacja główna"].Value != null && !String.IsNullOrEmpty(theRow.Cells["Operacja główna"].Value.ToString()))
                                doc.OperacjaGlowna = theRow.Cells["Operacja główna"].Value.ToString();
                            else
                                doc.OperacjaGlowna = "P010";
                            /*
                            mn.Rodzaj_dokumentu = dtr["Rodzaj dokumentu"].ToString();
                            mn.Waluta = dtr["Waluta"].ToString();
                            mn.Klucz_uzgodnienia = dtr["Klucz uzgodnienia"].ToString();
                            mn.Jednostaka_gospodarca_własna = mySad
                            */

                            if (!String.IsNullOrEmpty(theRow.Cells["Czysamoistna"].Value.ToString()))
                            {
                                doc.grzSamoistna = (theRow.Cells["Czysamoistna"].Value).ToString();
                                spr.grzSamoistna = (theRow.Cells["Czysamoistna"].Value).ToString();
                            }
                            else
                            {
                                doc.grzSamoistna = "";
                                spr.grzSamoistna = "";
                            }
                            if (theRow.Cells["Częściowo grzywna"].Value != DBNull.Value && !String.IsNullOrEmpty(theRow.Cells["Częściowo grzywna"].Value.ToString()))
                                doc.OperacjaCzesciowa = theRow.Cells["Częściowo grzywna"].Value.ToString();
                            else
                            {
                                doc.OperacjaCzesciowa = "0040";
                               
                            }
                            if (theRow.Cells["Rodzaj dokumentu"].Value != DBNull.Value  && !String.IsNullOrEmpty(theRow.Cells["Rodzaj dokumentu"].Value.ToString()))
                                doc.SAPRodzajDokumentu = theRow.Cells["Rodzaj dokumentu"].Value.ToString();
                            else
                                doc.SAPRodzajDokumentu = "DN";
                           
                            doc.DataPlatnosci = theRow.Cells["Data wymagalności"].Value as DateTime? ?? null;   // sprawdzić przy kposztach
                            doc.kwota =  (theRow.Cells["grzywna"].Value == DBNull.Value ? 0 :  Convert.ToDecimal(theRow.Cells["grzywna"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")));
                            doc.typFakt = "BD";
                            doc.Info = (String.IsNullOrEmpty(errmsg) ? null : errmsg);
                           
                       

                       
                        spr.Dluznik.Add(dl);
                        if (doc != null)
                         {
                                                  
                                spr.Dokument.Add(doc);
                                dl.Dokument.Add(doc);
                                trans.Dokument.Add(doc);
                                RupDatabase.theContext.Transfer.AddObject(trans);
                                RupDatabase.theContext.SaveChanges();
                                return doc;
                           }
                        return null;
                   }
                        
            
            catch (Exception ex)
            {
                //string s = CustomExtensions.ToTraceString(Context);
              
                    MessageBox.Show("Błąd podczas zapisu dokumentu " + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : ""));
                    return null;

            }
            
            
    
        }
    }
}
