#define RelationCreate
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Ex2PscdInterface.Ex2PscdContractObjectCreateOutService;
using SapPOHelper;
using Ex2PscdInterface.Ex2PscdContractObjectQueryOutService;
using Ex2PscdInterface.Ex2PscdContractAccountQueryOutService;
using Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService;
using Ex2PscdInterface.Ex2PscdPartnerCreateOutService;
using Ex2PscdInterface.Ex2PscdContractAccountCreateOutService;
using Ex2PscdInterface.Ex2PscdDocumentCreateOutService;
using System.Security.Cryptography.X509Certificates;
using MessageSignature;
using Ex2PscdInterface.Ex2PscdRelationCreateOutService;
using log4net;
using System.Collections;
using Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService;
using Ex2PscdInterface.Ex2PscdPaymentListQueryInService;

namespace RupLoader
{
    class ExportPI
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ContractObjectQueryRequest setupGetSygnStruct(Dokument dok, Konfiguracja konf)
        {
            ContractObjectQueryRequest sygnqry = new ContractObjectQueryRequest();
            sygnqry.Sygnatura = new SygnaturaDefinicja();
            sygnqry.Sygnatura.JednostkaGospodarcza = dok.Sprawa.SAPSadId;
            sygnqry.Sygnatura.NumerWydzialuISekcji = dok.Sprawa.SAPWydział;
            sygnqry.Sygnatura.Repertorium = dok.Sprawa.SAPRepertorium.ToUpper();
            sygnqry.Sygnatura.KolejnyNumerSprawy = dok.Sprawa.Numer.ToString();
            sygnqry.Sygnatura.Rok = dok.Sprawa.Rok.ToString();
            if (sygnqry.Sygnatura != null)
            {
                int jego;
                if (int.TryParse(sygnqry.Sygnatura.JednostkaGospodarcza, out jego))
                    if (jego > 5000)   // stanowisko finansowe; 
                    {
                        sygnqry.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe = sygnqry.Sygnatura.JednostkaGospodarcza;
                        string jedngosp = sygnqry.Sygnatura.JednostkaGospodarcza;
                        SAPSad ss = RupDatabase.theContext.SAPSad.Where(d => d.kod == jedngosp).FirstOrDefault();
                        sygnqry.Sygnatura.JednostkaGospodarcza = ss.JEGO;
                    }
            }
            return sygnqry;
        }




        private SygnaturaTworzenie setupSygnStruct(Dokument dok, Konfiguracja konf)
        {
            SygnaturaTworzenie sygnqry = new SygnaturaTworzenie();
            sygnqry.JednostkaGospodarcza = dok.Sprawa.SAPSadId;
            if (sygnqry.JednostkaGospodarcza != null)
            {
                int jego;
                if (int.TryParse(sygnqry.JednostkaGospodarcza, out jego))
                    if (jego > 5000)   // stanowisko finansowe; 
                    {
                        sygnqry.SadFunkcjonalnyStanowiskoFinansowe = sygnqry.JednostkaGospodarcza;
                        string jedngosp = sygnqry.JednostkaGospodarcza;
                        SAPSad ss = RupDatabase.theContext.SAPSad.Where(d => d.kod == jedngosp).FirstOrDefault();
                        sygnqry.JednostkaGospodarcza = ss.JEGO;
                    }
            }


            sygnqry.NumerWydzialuISekcji = dok.Sprawa.SAPWydział;
            sygnqry.Repertorium = dok.Sprawa.SAPRepertorium.ToUpper();
            sygnqry.KolejnyNumerSprawy = dok.Sprawa.Numer.ToString();
            sygnqry.Rok = dok.Sprawa.Rok.ToString();
            sygnqry.RodzajSprawy = dok.Sprawa.SAPRodzajSprawy;
            sygnqry.RodzajPrzedmiotuUmowy = dok.Sprawa.SAPRodzajPrzedmiotuUmowy;
            sygnqry.IloscTomow = dok.Sprawa.SAPTomyAkt;
            sygnqry.DaneDoWindykacjiJednostkaGospodarcza = konf.JednostkaGospodarcza;
            if (!String.IsNullOrWhiteSpace(konf.StanowiskoFin))
                sygnqry.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe = konf.StanowiskoFin;
            sygnqry.PodrodzajSprawy = "";



            return sygnqry;
        }


        private Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner setupBussinessPartner(Dokument dok, Konfiguracja konf)
        {
            Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner dluqry = new Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner();
            if (dok.Dluznik.FizPraw == "")
            {
                dluqry.TypPartnera = "1";
                dluqry.Imie = dok.Dluznik.Imie;
                dluqry.Nazwisko = dok.Dluznik.Nazwisko;
                dluqry.NazwaOrganizacji1 = "";
                dluqry.NazwaOrganizacji2 = "";
            }
            else
            {
                dluqry.TypPartnera = "2";
                dluqry.NazwaOrganizacji1 = dok.Dluznik.Imie;
                dluqry.NazwaOrganizacji2 = dok.Dluznik.Nazwisko;
                dluqry.Imie = dok.Dluznik.Imie;
                dluqry.Nazwisko = dok.Dluznik.Nazwisko;
                if (String.IsNullOrEmpty(dluqry.Nazwisko))
                {
                    int spc = dluqry.Imie.LastIndexOf(' ');
                    if (spc > 0)
                    {
                        string tmp = dluqry.Imie.Substring(spc + 1);
                        if (tmp.Trim().Length > 0)
                        {
                            dluqry.Imie = dluqry.Imie.Substring(0, spc);
                            dluqry.Nazwisko = tmp.Trim();

                        }

                    }
                    if (String.IsNullOrEmpty(dluqry.Nazwisko)) dluqry.Nazwisko = ".";
                    dluqry.NazwaOrganizacji2 = dluqry.Nazwisko;

                }

            }
            dluqry.AdresPartner = new Ex2PscdInterface.Ex2PscdPartnerCreateOutService.AdresPartner();
            dluqry.AdresPartner.KodPocztowy = dok.Dluznik.KodPocztowy == null ? null : dok.Dluznik.KodPocztowy.Trim();
            dluqry.AdresPartner.Kraj = dok.Dluznik.KluczKraju == null ? null : dok.Dluznik.KluczKraju;
            dluqry.AdresPartner.Miasto = dok.Dluznik.Miejscowosc == null ? null : dok.Dluznik.Miejscowosc.Trim();
            dluqry.NIP = dok.Dluznik.Nip == null ? null : dok.Dluznik.Nip.Trim();
            dluqry.AdresPartner.NumerDomu = dok.Dluznik.NrDomu == null ? null : dok.Dluznik.NrDomu.Trim();
            dluqry.AdresPartner.NumerDomu2 = dok.Dluznik.NrMieszkania == null ? null : dok.Dluznik.NrMieszkania.Trim();
            dluqry.PESEL = dok.Dluznik.Pesel == null ? null : dok.Dluznik.Pesel.Trim();
            if (dok.Dluznik.RBN == null)
                dluqry.RBN = null;
            else
            {
                dluqry.RBN = new RBN();
                dluqry.RBN.KW_RBN = dok.Dluznik.RBN.Trim();
                dluqry.RBN.Data_RBN = (dok.DataDokumentu ?? new DateTime(1900, 1, 1)).ToString("yyyyMMdd");
            }
            dluqry.AdresPartner.Ulica = dok.Dluznik.Ulica == null ? null : dok.Dluznik.Ulica.Trim();

            if (dluqry.AdresPartner.KodPocztowy != null) dluqry.AdresPartner.KodPocztowy = dluqry.AdresPartner.KodPocztowy.Trim().Truncate(10);
            if (dluqry.AdresPartner.Kraj != null) dluqry.AdresPartner.Kraj = dluqry.AdresPartner.Kraj.Trim().Truncate(2);
            if (dluqry.AdresPartner.Miasto != null) dluqry.AdresPartner.Miasto = dluqry.AdresPartner.Miasto.Trim().Truncate(40);
            if (dluqry.NIP != null) dluqry.NIP = dluqry.NIP.Trim().Truncate(10);
            if (dluqry.AdresPartner.NumerDomu != null) dluqry.AdresPartner.NumerDomu = dluqry.AdresPartner.NumerDomu.Trim().Truncate(10);
            if (dluqry.AdresPartner.NumerDomu2 != null) dluqry.AdresPartner.NumerDomu2 = dluqry.AdresPartner.NumerDomu2.Trim().Truncate(10); else dluqry.AdresPartner.NumerDomu2 = "";
            if (dluqry.PESEL != null) dluqry.PESEL = dluqry.PESEL.Trim().Truncate(11);
            if (dluqry.RBN != null) dluqry.RBN.KW_RBN = dluqry.RBN.KW_RBN.Trim().Truncate(2);
            if (dluqry.AdresPartner.Ulica != null) dluqry.AdresPartner.Ulica = dluqry.AdresPartner.Ulica.Trim().Truncate(60);
            return dluqry;


        }

        private KontoUmowyTworzenie setupKdl(Dokument dok, Konfiguracja knf, string typkdl)
        {
            KontoUmowyTworzenie kdlqry = new KontoUmowyTworzenie();
            kdlqry.JednostkaGospodarcza = knf.JednostkaGospodarcza;
            kdlqry.NumerPartnera = dok.Dluznik.SAPKontoPartnera;
            kdlqry.OznaczenieKontaUmowy = dok.Sprawa.Karta;
            kdlqry.RelacjaPartneraHandlowego = dok.Sprawa.SAPRelacjaKontaUmowy;
            if (String.IsNullOrEmpty(kdlqry.RelacjaPartneraHandlowego)) kdlqry.RelacjaPartneraHandlowego = "99";
            kdlqry.StandardowaJednostkaGospodarcza = knf.JednostkaGospodarcza;
            if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                kdlqry.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
            kdlqry.TypKontaUmowy = typkdl;
            kdlqry.IDPrzedmiotuUmowy = dok.Sprawa.SAPPrzedmiotUmowy;

            return kdlqry;

        }


        private DocumentCreateRequest setupPrzypis(Dokument inDok, Konfiguracja knf, string kluczUzg)
        {

            DocumentCreateRequest dok = new DocumentCreateRequest();
            Ex2PscdInterface.Ex2PscdDocumentCreateOutService.NaglowekDokument naglowek = new Ex2PscdInterface.Ex2PscdDocumentCreateOutService.NaglowekDokument();
            PozycjaDokumentuPH pozDph = new PozycjaDokumentuPH();
            if (inDok.DataDokumentu == null || inDok.DataDokumentu < new DateTime(2000, 1, 1) || inDok.DataKsiegowania == null || inDok.DataKsiegowania < new DateTime(2000, 1, 1) || inDok.kwota == 0 || String.IsNullOrWhiteSpace(inDok.OperacjaCzesciowa) || String.IsNullOrWhiteSpace(inDok.OperacjaGlowna))
            {
                DokDetail dd = new DokDetail();
                dd.opCz = inDok.OperacjaCzesciowa;
                dd.opGl = inDok.OperacjaGlowna;
                dd.dDokumentu = Convert.ToDateTime(inDok.DataDokumentu);
                dd.dKsiegowania = Convert.ToDateTime(inDok.DataKsiegowania);
                dd.kwota = Convert.ToDecimal(inDok.kwota);
                if (dd.ShowDialog() == DialogResult.OK)
                {
                    inDok.OperacjaCzesciowa = dd.opCz;
                    inDok.OperacjaGlowna = dd.opGl;
                    inDok.DataDokumentu = dd.dDokumentu;
                    inDok.DataKsiegowania = dd.dKsiegowania;
                    inDok.kwota = dd.kwota;
                }
                else return null;
            }


            dok.NaglowekDokument = naglowek;
            dok.PozycjaDokumentPH = pozDph;


            pozDph.OperacjaCz = inDok.OperacjaCzesciowa;
            naglowek.DataDokument = Convert.ToDateTime(inDok.DataDokumentu).ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
            naglowek.DataKsiegowanie = Convert.ToDateTime(inDok.DataKsiegowania).ToString("yyyyMMdd"); //; Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
            pozDph.DataPlatnosci = Convert.ToDateTime(inDok.DataPlatnosci).ToString("yyyyMMdd");
            pozDph.OperacjaGl = inDok.OperacjaGlowna;
            pozDph.IDSygnatura = inDok.Sprawa.SAPPrzedmiotUmowy;
            pozDph.JednostkaGospodarcza = knf.JednostkaGospodarcza;
            // dok.KluczUzgodnienia = kluczUzg;
            naglowek.Waluta = "PLN";
            pozDph.Kwota = Convert.ToDecimal(inDok.kwota).ToString(CultureInfo.GetCultureInfo("en-US"));
            // dok.NumerDokumentuRozrachunkow = "";
            pozDph.IDKontoUmowy = inDok.Sprawa.SAPKontoUmowy;
            pozDph.IDPartner = inDok.Dluznik.SAPKontoPartnera;

            naglowek.RodzajDokumentu = "NS";
            if (!String.IsNullOrEmpty(inDok.SAPRodzajDokumentu))
                naglowek.RodzajDokumentu = inDok.SAPRodzajDokumentu;

            pozDph.Tekst = inDok.Opis;

            return dok;

        }


        /*

                private SapPIService.NaleznoscTyp setupPrzypis(Dokument inDok, Konfiguracja knf, string kluczUzg)
                {
                    SapPIService.NaleznoscTyp dok = new SapPIService.NaleznoscTyp();

                    if (inDok.DataDokumentu == null || inDok.DataDokumentu < new DateTime(2000, 1, 1) || inDok.DataKsiegowania == null || inDok.DataKsiegowania < new DateTime(2000, 1, 1) || inDok.kwota == 0 || String.IsNullOrWhiteSpace(inDok.OperacjaCzesciowa) || String.IsNullOrWhiteSpace(inDok.OperacjaGlowna))
                    {
                        DokDetail dd = new DokDetail();
                        dd.opCz = inDok.OperacjaCzesciowa;
                        dd.opGl = inDok.OperacjaGlowna;
                        dd.dDokumentu = Convert.ToDateTime(inDok.DataDokumentu);
                        dd.dKsiegowania = Convert.ToDateTime(inDok.DataKsiegowania);
                        dd.kwota = Convert.ToDecimal(inDok.kwota) ;
                        if (dd.ShowDialog() == DialogResult.OK)
                        {
                            inDok.OperacjaCzesciowa = dd.opCz;
                            inDok.OperacjaGlowna = dd.opGl;
                            inDok.DataDokumentu = dd.dDokumentu ;
                            inDok.DataKsiegowania = dd.dKsiegowania ;
                            inDok.kwota = dd.kwota;
                        }
                        else return null;    
                    }


                    dok.CzesciowaOperacja = inDok.OperacjaCzesciowa;
                    dok.DataDokumentu = Convert.ToDateTime(inDok.DataDokumentu).ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
                    dok.DataKsiegowania = Convert.ToDateTime(inDok.DataKsiegowania).ToString("yyyyMMdd");
                    dok.DataPlatnosciNetto =  Convert.ToDateTime(inDok.DataPlatnosci).ToString("yyyyMMdd");
                    dok.GlownaOperacja = inDok.OperacjaGlowna;
                    dok.IDPrzedmiotuUmowy = inDok.Sprawa.SAPPrzedmiotUmowy;
                    dok.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                    dok.KluczUzgodnienia = kluczUzg;
                    dok.KodWaluty = "PLN";
                    dok.KwotaNaleznosci = Convert.ToDecimal(inDok.kwota).ToString(CultureInfo.GetCultureInfo("en-US"));
                    dok.NumerDokumentuRozrachunkow = "";
                    dok.NumerKontaUmowy = inDok.Sprawa.SAPKontoUmowy;
                    dok.NumerPartnera =  inDok.Dluznik.SAPKontoPartnera;
                    dok.PrzyczynaBlokPlatnosci = "X";
                    dok.RodzajDokumentu = "NS";
                    if (!String.IsNullOrEmpty(inDok.SAPRodzajDokumentu))
                        dok.RodzajDokumentu  = inDok.SAPRodzajDokumentu;
                     dok.TekstWyjasniajacy = inDok.Opis; 

                    return dok;

                }
                */
        /*
        private SapPIService.PartnerWyszukanieTyp setupGetPartner(Dokument dok, Konfiguracja knf, string kluczUzg)
        {
            SapPIService.PartnerWyszukanieTyp partn = new SapPIService.PartnerWyszukanieTyp();

            partn.IDPrzedmiotuUmowy = dok.Sprawa.SAPPrzedmiotUmowy;
            partn.TypPartnera = (dok.Dluznik.FizPraw == "X") ? "2" : "1";
            partn.NumerPartnera = dok.Dluznik.SAPKontoPartnera;


            return partn;

        }
        */

        /*
        private SapPIService.OdpisanieNaleznosciTyp setupOdpis(GridViewRowInfo row, Konfiguracja knf, string kluczUzg , ref string NumerDokDoDdpis)
        {
            SapPIService.OdpisanieNaleznosciTyp dok = new SapPIService.OdpisanieNaleznosciTyp();
          

            dok.CzesciowaOperacja = row.Cells["OperacjaCzesciowa"].Value as string;
            dok.DataDokumentu = row.Cells["DataDokumentu"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
            dok.DataKsiegowania = row.Cells["DataKsiegowania"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //; Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
            dok.DataPlatnosciNetto = row.Cells["DataPlatnosci"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataPlatnosci"].Value).ToString("yyyyMMdd");
            dok.GlownaOperacja = row.Cells["OperacjaGlowna"].Value as string;
            dok.JednostkaGospodarcza = knf.JednostkaGospodarcza;
            dok.KluczUzgodnienia = kluczUzg;
            dok.PrzyczynaBlokPlatnosci = "A";
            dok.TekstWyjasniajacy = row.Cells["Opis"].Value as string;
            dok.KwotaNaleznosci = "-" +  Convert.ToDecimal(row.Cells["kwota"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
            
            NumerDokDoDdpis = (row.Cells["SAPDocIdRef"].Value) as string;
         
            

            return dok;

        }

        */

        private KontoUmowyDefinicja setupGetKonto(Dokument indok, Konfiguracja knf, string nrPartnera, string nrKontaUmowy, string nrSygnatury)
        {
            KontoUmowyDefinicja getkdl = new KontoUmowyDefinicja();

            string typkdl = "DO";

            if (indok != null && indok.Sprawa != null && !String.IsNullOrEmpty(indok.Sprawa.SAPTypKontaUmowy))
            {
                typkdl = indok.Sprawa.SAPTypKontaUmowy;
            }

            getkdl.NumerPartnera = nrPartnera;
            getkdl.IDPrzedmiotuUmowy = nrSygnatury;
            getkdl.NumerKontaUmowy = nrKontaUmowy;

            //if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
            //    getkdl.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
            getkdl.TypKontaUmowy = typkdl;
            return getkdl;
        }

        private KontoUmowyDefinicja setupGetKonto(Dokument indok, Konfiguracja knf)
        {
            KontoUmowyDefinicja getkdl = new KontoUmowyDefinicja();


            string typkdl = "DO";
            if (!String.IsNullOrEmpty(indok.Sprawa.SAPTypKontaUmowy))
            {
                typkdl = indok.Sprawa.SAPTypKontaUmowy;
            }



            getkdl.NumerPartnera = indok.Dluznik.SAPKontoPartnera;
            getkdl.IDPrzedmiotuUmowy = indok.Sprawa.SAPPrzedmiotUmowy;
            getkdl.NumerKontaUmowy = indok.Sprawa.SAPKontoUmowy;

            //if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
            //    getkdl.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
            getkdl.TypKontaUmowy = typkdl;
            return getkdl;



        }


        private int getKontoDepozyt(KontoUmowyDefinicja getkdl, out string kontoUmowy, out string errMsg)
        {
            kontoUmowy = null;
            errMsg = null;

            if (getkdl.TypKontaUmowy != "BE")
            {
                errMsg = "Błędne oznaczenie konta";
                return -1;  // błedne oznaczenie konta
            }
            try
            {
                getkdl.TypKontaUmowy = "B*";
                ContractAccountQueryResponse ansget = ZSRKRequestHelper.WyszukajKontoUmowy(getkdl);

                if (ansget == null) { errMsg = " Błąd odpowiedzi usługi sieciowej "; return -2; } // błędna odpowiedź z serwisu
                if (ansget.Komunikaty != null && ansget.Komunikaty.Any() && ansget.Komunikaty.GetUpperBound(0) >= 0)
                {
                    /*
                    if (ansget.Komunikaty[0].RodzajKomunikatu == "E")
                    {
                        errMsg = ansget.Komunikaty[0].Komunikat;
                        return -3;
                    
                    }
                    */
                }
                else
                {

                    errMsg = "Błędny format odpowiedzi ";
                    return -4;

                }
                if (ansget.KontaUmowy != null && ansget.KontaUmowy.GetUpperBound(0) >= 0)
                {

                    List<KontoUmowyDefinicja> kumowy = ansget.KontaUmowy.Where(a => a.StandardowaJednostkaGospodarcza == RupDatabase.theConfig.JednostkaGospodarcza).OrderByDescending(a => a.NumerKontaUmowy).ToList();
                    if (kumowy != null)
                    {
                        // 
                        int maxB = -1;
                        int curr = 0;
                        foreach (KontoUmowyDefinicja ko in kumowy)
                        {
                            string num = ko.TypKontaUmowy.Substring(1, 1).ToUpper();
                            if (num == "E" && maxB < 0)
                                maxB = 0;
                            else if (Int32.TryParse(num, out curr))
                            {
                                maxB = curr;
                            }
                        }
                        if (maxB >= 0)
                        {
                            kontoUmowy = "B" + (++maxB).ToString();
                            return 1;
                        }
                        else
                        {
                            kontoUmowy = "BE";
                            return 1;

                        }
                    }
                    else
                    {
                        errMsg = "Brak pozycji spełniajacych kryterium";
                        kontoUmowy = "BE";
                        return 0;
                    }

                }
                else
                {
                    errMsg = "Brak pozycji spełniajacych kryterium";
                    kontoUmowy = "BE";
                    return 0;


                }
                errMsg = "Nieznany błąd";
                return -100;

            }
            catch (Exception ex)
            {
                errMsg = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                return -4;
            }

        }



        public void setSAPConnectionParams()
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                User usr = context.User.Where(a => a.Id == UserInfo.Id).FirstOrDefault();
                setSAPConnectionParams(usr);
            }
        }


        private void setSAPConnectionParams(User u)
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

                List<ServiceEndpoint> l = context.ServiceEndpoint.ToList();
                if (l != null)
                    foreach (ServiceEndpoint s in l)
                    {
                        lst.Add(new KeyValuePair<string, string>(s.ServiceName, s.Endpoint));
                    }

                ZSRKRequestHelper.ServiceMapping = lst;
                ZSRKRequestHelper.AuthCert = new X509Certificate2(knf.Pfx, Utils.Decrypt(knf.PfxPassword, "Application error"));
                ZSRKRequestHelper.BasicAuthLogin = knf.WSLogon;
                ZSRKRequestHelper.BasicAuthPassword = knf.WSpwd;
                ZSRKRequestHelper.MEPUser = u.MEPUser;
                ZSRKRequestHelper.MEPPassword = Utils.Decrypt(u.MEPPassword, "Application error");
                ZSRKRequestHelper.ApplicationID = knf.AppName;
                ZSRKRequestHelper.JednostkaGospodarcza = knf.JednostkaGospodarcza;

                SignatureHelper.Password = Utils.Decrypt(u.MEPPassword, "Application error");
                SignatureHelper.SetCert(knf.Cer);

            }
        }


        public string DoExport(Dokument inDok, int wariant, bool skipSave = false, TextBox tbPrompt = null, string nrPartnera = null, string nrKontaUmowy = null)
        {
            int Id;
            string kluczUzg = "";
            DateTime dKsiegowania;
            string retvalue = "";
            ContractAccountCreateResponse ans;
            // Po0bierz aktualny transfer
            inDok.SAPImportInfo = "";
            Cursor.Current = Cursors.WaitCursor;
            if (!skipSave)
            {
                Utils.LogWriter("Entering DoExport");
                RupDatabase.theContext.SaveChanges();
                Utils.LogWriter("DoExport- 0");
            }
            Konfiguracja knf = RupDatabase.theContext.Konfiguracja.FirstOrDefault();
            try
            {

                dKsiegowania = DateTime.Today;
                setSAPConnectionParams();
            nextTry:
                retvalue = "";
                if (!String.IsNullOrEmpty(inDok.Sprawa.SAPPrzedmiotUmowy)) { retvalue += inDok.Sprawa.SAPPrzedmiotUmowy; goto skipsygn; }

                SygnaturaTworzenie sygnqry = setupSygnStruct(inDok, knf);
                ContractObjectCreateResponse anssygn = ZSRKRequestHelper.ZalozSygnature(sygnqry);
                if (tbPrompt != null) tbPrompt.Text = inDok.Sprawa.Sygnatura;
                if (anssygn != null)
                {
                    if (anssygn.Sygnatura != null)
                    {
                        if (!String.IsNullOrWhiteSpace(anssygn.Sygnatura.IDPrzedmiotuUmowy))
                        {
                            retvalue = anssygn.Sygnatura.IDPrzedmiotuUmowy;
                            inDok.Sprawa.SAPPrzedmiotUmowy = anssygn.Sygnatura.IDPrzedmiotuUmowy;
                            Utils.LogWriter("Przedmiot umowy 1 " + anssygn.Sygnatura.IDPrzedmiotuUmowy.Length.ToString() + " " + anssygn.Sygnatura.IDPrzedmiotuUmowy);

                            if (!skipSave) RupDatabase.theContext.SaveChanges();

                        }
                        else
                        {
                            inDok.SAPImportInfo = ((anssygn.Komunikaty != null && anssygn.Komunikaty.Any()) ? " " + anssygn.Komunikaty.FirstOrDefault().IDKomunikatu + " " + anssygn.Komunikaty.FirstOrDefault().Komunikat1 : "") + "; " + inDok.SAPImportInfo;
                            if (!skipSave) RupDatabase.theContext.SaveChanges();
                            ContractObjectQueryRequest sygnGetquery = setupGetSygnStruct(inDok, knf);
                            ContractObjectQueryResponse getsygn = ZSRKRequestHelper.ZnajdzSygnature(sygnGetquery);
                            if (getsygn == null)
                            {
                                //inDok.SAPImportInfo += (getsygn.Komunikaty != null && getsygn.Komunikaty.Any()) ? "; " + getsygn.Komunikaty.FirstOrDefault().Komunikat1 : "";
                                if (RunMode.silentMode)
                                    Utils.LogWriter("Błąd podczas zakładania sygnatury");
                                else
                                    MessageBox.Show("Błąd podczas zakładania sygnatury");
                                return "";
                            }


                            if (getsygn.Sygnatura != null && getsygn.Sygnatura.Length == 1 && getsygn.Sygnatura[0].IDPrzedmiotuUmowy != null && getsygn.Sygnatura[0].OznaczeniePrzedmiotuUmowy.StartsWith(String.IsNullOrWhiteSpace(sygnGetquery.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe) ? sygnGetquery.Sygnatura.JednostkaGospodarcza : sygnGetquery.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe))
                            {
                                retvalue = getsygn.Sygnatura[0].IDPrzedmiotuUmowy;
                                inDok.Sprawa.SAPPrzedmiotUmowy = getsygn.Sygnatura[0].IDPrzedmiotuUmowy;
                                Utils.LogWriter("Przedmiot umowy 2 " + getsygn.Sygnatura[0].IDPrzedmiotuUmowy.Length.ToString() + getsygn.Sygnatura[0].IDPrzedmiotuUmowy);
                                if (!skipSave) RupDatabase.theContext.SaveChanges();
                            }
                            else
                            {

                                inDok.SAPImportInfo = ((getsygn.Komunikaty != null && getsygn.Komunikaty.Any()) ? "; " + getsygn.Komunikaty.FirstOrDefault().Komunikat1 : "") + " " + inDok.SAPImportInfo;
                                if (!skipSave) RupDatabase.theContext.SaveChanges();

                                if (RunMode.silentMode)
                                    Utils.LogWriter("Błąd podczas zakładania sygnatury");
                                else
                                    MessageBox.Show("Błąd podczas zakładania sygnatury");
                                return "";
                            }
                        }

                    }


                }
                else
                {
                    inDok.SAPImportInfo = ZSRKRequestHelper.GetErrorMessage() + " " + inDok.SAPImportInfo;
                    if (!skipSave) RupDatabase.theContext.SaveChanges();

                    if (RunMode.silentMode)
                        Utils.LogWriter(ZSRKRequestHelper.GetErrorMessage() + " Błąd wywołania usługi sieciowej - [Dodaj sygnaturę]  ");
                    else
                        MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd wywołania usługi sieciowej - [Dodaj sygnaturę]  ");
                    return "";
                }


            skipsygn:

                if (wariant == 3)
                {
                    if (string.IsNullOrWhiteSpace(nrPartnera) || string.IsNullOrWhiteSpace(nrKontaUmowy))
                    {
                        return retvalue;
                    }
                    else
                    {

                        KontoUmowyDefinicja getKontoUmowy = setupGetKonto(inDok, knf, nrPartnera, nrKontaUmowy, retvalue);
                        ContractAccountQueryResponse ansget2 = ZSRKRequestHelper.WyszukajKontoUmowy(getKontoUmowy);
                        if (ansget2 != null)
                        {
                            if (ansget2.KontaUmowy != null && ansget2.KontaUmowy.GetUpperBound(0) >= 0 && ansget2.KontaUmowy[0].NumerPartnera == nrPartnera && ansget2.KontaUmowy[0].IDPrzedmiotuUmowy == retvalue)
                                return retvalue;
                            else
                            {
                                Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy updtKdl = new Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy();
                                updtKdl.TypKontoUmowy = getKontoUmowy.TypKontaUmowy;
                                updtKdl.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                                updtKdl.IDPartnera = string.IsNullOrWhiteSpace(nrPartnera) ? inDok.Dluznik.SAPKontoPartnera : nrPartnera;
                                updtKdl.RelacjaKonta = getKontoUmowy.RelacjaPartneraHandlowego;
                                if (String.IsNullOrEmpty(updtKdl.RelacjaKonta)) updtKdl.RelacjaKonta = "99";
                                if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                                    updtKdl.StanowiskoFinansowe = knf.StanowiskoFin;
                                updtKdl.TypKontoUmowy = getKontoUmowy.TypKontaUmowy;
#if RelationCreate
                                RelationCreateResponse ans1 = ZSRKRequestHelper.UtworzRelacje(nrKontaUmowy, getKontoUmowy.IDPrzedmiotuUmowy);
#else
                                ContractAccountRelationCreateResponse ans1 = ZSRKRequestHelper.AktualizujKontoUmowy(updtKdl, retvalue);
#endif
                                if (ans1.Komunikaty != null && ans1.Komunikaty.Any())
                                {
                                    if (ans1.Komunikaty[0].RodzajKomunikatu == "E")
                                    {
                                        inDok.SAPImportInfo =( (ans1.Komunikaty != null) ? "; " + ans1.Komunikaty.FirstOrDefault().Komunikat1 : "") + " " + inDok.SAPImportInfo;
                                        if (!skipSave) RupDatabase.theContext.SaveChanges();

                                        if (RunMode.silentMode)
                                            Utils.LogWriter("Nie powiązano sygnatury z partnerem i kontem umowy ");
                                        else
                                            MessageBox.Show("Nie powiązano sygnatury z partnerem i kontem umowy ");
                                        return retvalue;
                                    }
                                    return retvalue;
                                }
                                else
                                {
                                    inDok.SAPImportInfo = ZSRKRequestHelper.GetErrorMessage() + " " + inDok.SAPImportInfo;

                                    if (!skipSave) RupDatabase.theContext.SaveChanges();
                                    if (RunMode.silentMode)
                                        Utils.LogWriter(ZSRKRequestHelper.GetErrorMessage() + " Błąd wywołania usługi sieciowej - [Aktualizuj konto umowy] ");
                                    else
                                        MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd wywołania usługi sieciowej - [Aktualizuj konto umowy] ");
                                    return retvalue;
                                }

                            }

                        }
                        else
                        {
                            if (RunMode.silentMode)
                                Utils.LogWriter("Błąd usługi sieciowej [Wyszukaj konto umowy] sygnatura została złożona");
                            else
                                MessageBox.Show("Błąd usługi sieciowej [Wyszukaj konto umowy] sygnatura została złożona");
                            return retvalue;
                        }

                    }
                    return retvalue;
                }          // Dodaj partnera
                if (inDok.Dluznik.SAPKontoPartnera != null)
                {
                    if (!String.IsNullOrEmpty(inDok.Dluznik.SAPKontoPartnera)) goto skippartner;

                }

                Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner dluqry = setupBussinessPartner(inDok, knf);
                if (tbPrompt != null) tbPrompt.Text = inDok.Dluznik.Imie + " " + (String.IsNullOrWhiteSpace(inDok.Dluznik.Nazwisko) ? "" : " " + inDok.Dluznik.Nazwisko);
                PartnerCreateResponse anspart = ZSRKRequestHelper.DodajPartnera(dluqry);

                if (anspart != null)
                {
                    if (anspart.IDPartner != null)
                    {
                        retvalue += ";" + anspart.IDPartner;
                        inDok.Dluznik.SAPKontoPartnera = anspart.IDPartner;
                        if (!skipSave) RupDatabase.theContext.SaveChanges();

                    }
                    else
                    {
                        inDok.SAPImportInfo = ((anspart.Komunikaty != null && anspart.Komunikaty.Any() && anspart.Komunikaty.GetUpperBound(0) >= 0 ? anspart.Komunikaty[0].Komunikat1 : "")) + " " + inDok.SAPImportInfo;
                        if (!skipSave) RupDatabase.theContext.SaveChanges();

                        MessageBox.Show(" Błąd podczas zakładania partnera");
                        return "";

                    }

                }
                else
                {
                    inDok.SAPImportInfo = ZSRKRequestHelper.GetErrorMessage() + " " + inDok.SAPImportInfo;
                    if (!skipSave) RupDatabase.theContext.SaveChanges();

                    MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd wywołania usługi sieciowej - [Dodaj Partnera] ");
                    return "";
                }


            skippartner:



                if (wariant == 2) return retvalue;
                // Dodawanie konta umowy
                // Sprawdzenie czy takie konto  już istnieje
                if (String.IsNullOrEmpty(inDok.Dluznik.SAPKontoPartnera)) return retvalue;
                if (String.IsNullOrEmpty(inDok.Sprawa.SAPPrzedmiotUmowy)) return retvalue;



                KontoUmowyDefinicja getkdl = new KontoUmowyDefinicja();

                //getkdl.RelacjaPartneraHandlowego = "99";
                getkdl.TypKontaUmowy = null;


                string typkdl = "KN";

                typkdl = inDok.Sprawa.SAPTypKontaUmowy;
                string opGl = inDok.OperacjaGlowna; //  = row.Cells["OperacjaGlowna"];

                getkdl.NumerPartnera = inDok.Dluznik.SAPKontoPartnera;
                if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                    getkdl.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
                else if (!String.IsNullOrWhiteSpace(knf.JednostkaGospodarcza))
                    getkdl.SadFunkcjonalnyStanowiskoFinansowe = knf.JednostkaGospodarcza;
                getkdl.TypKontaUmowy = typkdl;
                // obsługa BE 


                if (typkdl == "DO" || typkdl == "WY" || typkdl == "KO" || (RupDatabase.theConfig.czyautoks == 1 && typkdl == "SZ"))
                {
                    if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                        getkdl.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
                }
                else
                    getkdl.IDPrzedmiotuUmowy = inDok.Sprawa.SAPPrzedmiotUmowy;

                // if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                //getkdl.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
                //getkdl.RelacjaPartneraHandlowego = "99";
                string typKontaBE = "";
                string erM = "";
                if (typkdl == "BE")
                {
                    int rcode = getKontoDepozyt(getkdl, out typKontaBE, out erM);
                    if (rcode < 0)
                    {
                        inDok.SAPImportInfo = erM + " " + inDok.SAPImportInfo;
                        if (!skipSave) RupDatabase.theContext.SaveChanges();

                        MessageBox.Show(erM);
                        return retvalue;
                    }
                    typkdl = typKontaBE;
                    inDok.Sprawa.SAPTypKontaUmowy = typkdl;
                    goto addKontoUmowy;

                }

                ContractAccountQueryResponse ansget = ZSRKRequestHelper.WyszukajKontoUmowy(getkdl);

                if (ansget != null)
                {

                    if (ansget.KontaUmowy != null && ansget.KontaUmowy.GetUpperBound(0) >= 0)
                    {
                        int ile = ansget.KontaUmowy.Count();
                        // jesli znajde z mojeje jednostki z takim samymo oznaczeniem to nie zakładam.
                        //#PA obsługa jednego konta umoway dla  DO oraz WY w kontekście jednej jednostki
                        KontoUmowyDefinicja kontox = ansget.KontaUmowy.Where(a => a.StandardowaJednostkaGospodarcza == knf.JednostkaGospodarcza && a.IDPrzedmiotuUmowy == inDok.Sprawa.SAPPrzedmiotUmowy).OrderByDescending(a => a.IDPrzedmiotuUmowy).FirstOrDefault();
                        if (kontox == null)
                        {
                            kontox = ansget.KontaUmowy.Where(a => a.StandardowaJednostkaGospodarcza == knf.JednostkaGospodarcza).OrderByDescending(a => a.IDPrzedmiotuUmowy).FirstOrDefault();
                            if (kontox != null)
                            {

                                Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy updtKdl = new Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy();
                                updtKdl.TypKontoUmowy = kontox.TypKontaUmowy;
                                updtKdl.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                                updtKdl.IDPartnera = inDok.Dluznik.SAPKontoPartnera;
                                updtKdl.RelacjaKonta = kontox.RelacjaPartneraHandlowego;
                                if (String.IsNullOrEmpty(updtKdl.RelacjaKonta)) updtKdl.RelacjaKonta = "99";
                                if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                                    updtKdl.StanowiskoFinansowe = knf.StanowiskoFin;
#if RelationCreate
                                RelationCreateResponse ans1 = ZSRKRequestHelper.UtworzRelacje(kontox.NumerKontaUmowy, inDok.Sprawa.SAPPrzedmiotUmowy);
#else
                                ContractAccountRelationCreateResponse ans1 = ZSRKRequestHelper.AktualizujKontoUmowy(updtKdl, inDok.Sprawa.SAPPrzedmiotUmowy);
#endif
                                if (ans1.Komunikaty != null && ans1.Komunikaty.Any())
                                {
                                    if (ans1.Komunikaty[0].RodzajKomunikatu == "E")
                                    {
                                        inDok.SAPImportInfo = ((ans1.Komunikaty != null) ? "; " + ans1.Komunikaty.FirstOrDefault().Komunikat1 : "") + " " + inDok.SAPImportInfo;
                                        if (!skipSave) RupDatabase.theContext.SaveChanges();

                                        MessageBox.Show("Założono konto umowy ale nie powiązano go z sygnaturą ");
                                        return retvalue;
                                    }
                                }
                                else
                                {
                                    inDok.SAPImportInfo = ZSRKRequestHelper.GetErrorMessage() + " " + inDok.SAPImportInfo;
                                    if (!skipSave) RupDatabase.theContext.SaveChanges();

                                    MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd wywołania usługi sieciowej - [Aktualizuj konto umowy] ");
                                    return retvalue;
                                }

                            }
                        }
                        if (kontox != null)
                        {
                            inDok.Sprawa.SAPKontoUmowy = kontox.NumerKontaUmowy;
                            goto skipnewkdl;
                        }
                        else
                        {  // dodajemy tego samego partnera


                            Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner dluqry_new = setupBussinessPartner(inDok, knf);

                            if (dluqry_new.TypPartnera == "1")
                                // osoba fizyczna
                                dluqry_new.PESEL = "";
                            else
                                dluqry_new.NIP = "";
                            if (dluqry_new.AdresPartner.Ulica.Contains(" "))
                                dluqry_new.AdresPartner.Ulica = Utils.ReplaceFirst(dluqry_new.AdresPartner.Ulica, " ", "  ");
                            else if (dluqry_new.AdresPartner.Ulica.Contains("."))
                                dluqry_new.AdresPartner.Ulica.Replace(".", " .");
                            else dluqry_new.AdresPartner.Ulica = dluqry_new.AdresPartner.Ulica + ".";

                            PartnerCreateResponse anspart_new = ZSRKRequestHelper.DodajPartnera(dluqry_new);
                            if (anspart_new != null)
                            {
                                if (anspart_new.IDPartner != null)
                                {
                                    if (anspart_new.IDPartner != null)
                                    {
                                        inDok.Dluznik.SAPKontoPartnera = anspart_new.IDPartner;

                                    }
                                    else
                                    {
                                        inDok.SAPImportInfo = ((anspart_new.Komunikaty != null && anspart_new.Komunikaty.Any()) ? "; " + anspart_new.Komunikaty.FirstOrDefault().Komunikat1 : "") + " " + inDok.SAPImportInfo;
                                        if (!skipSave) RupDatabase.theContext.SaveChanges();

                                        MessageBox.Show("Błąd podczas zakładania Partnera ");
                                        return retvalue;

                                    }

                                }

                            }




                            else
                            {
                                inDok.SAPImportInfo = ZSRKRequestHelper.GetErrorMessage() + " " + inDok.SAPImportInfo;
                                if (!skipSave) RupDatabase.theContext.SaveChanges();

                                MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd wywołania usługi sieciowej - [Dodaj Partnera]  ");
                                return retvalue;
                            }

                        }
                        //if (ile > 0)
                        //    typkdl = "K" + ile.ToString();

                    }

                }
                else
                {
                    inDok.SAPImportInfo = ZSRKRequestHelper.GetErrorMessage() + " " + inDok.SAPImportInfo;
                    if (!skipSave) RupDatabase.theContext.SaveChanges();

                    MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd wywołania usługi sieciowej - [Wyszukaj konto umowy] ");
                    return retvalue;
                }


                if (!String.IsNullOrEmpty(inDok.Sprawa.SAPKontoUmowy)) goto skipkdl;



                if (String.IsNullOrEmpty(inDok.Dluznik.SAPKontoPartnera))
                {

                    MessageBox.Show(" Brak Partnera - nie można założyć konta umowy");
                    return retvalue;
                }
                if (String.IsNullOrEmpty(inDok.Sprawa.SAPPrzedmiotUmowy))
                {

                    MessageBox.Show("Brak numeru przedmiotu umowy (sygnatury)");
                    return retvalue;
                }
            addKontoUmowy:
                KontoUmowyTworzenie kdlqry = setupKdl(inDok, knf, typkdl);
                ans = ZSRKRequestHelper.DodajKontoUmowy(kdlqry);
                if (ans != null)
                {
                    if (ans.KontoUmowyIdentyfikacja != null)
                    {
                        if (ans.KontoUmowyIdentyfikacja.NumerKontaUmowy != null)
                        {

                            retvalue += ";" + ans.KontoUmowyIdentyfikacja.NumerKontaUmowy;
                            inDok.Sprawa.SAPKontoUmowy = ans.KontoUmowyIdentyfikacja.NumerKontaUmowy;
                        }
                        else
                        {
                            inDok.SAPImportInfo = ((ans.Komunikaty != null && ans.Komunikaty.Any()) ? "; " + ans.Komunikaty.FirstOrDefault().Komunikat1 : "") + " " + inDok.SAPImportInfo;
                            if (!skipSave) RupDatabase.theContext.SaveChanges();

                            MessageBox.Show("Błąd podczas zakładania konta umowy");
                            return retvalue;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Błąd podczas zakładania konta umowy");
                        return retvalue;


                    }


                }

                else
                {
                    MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd wywołania usługi sieciowej - [Dodaj konto umowy] ");
                    return retvalue;
                }
            skipkdl:
                // sprawdź czy jest relacja 


                getkdl = setupGetKonto(inDok, knf);

                if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                    getkdl.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
                ContractAccountQueryResponse ansget1 = ZSRKRequestHelper.WyszukajKontoUmowy(getkdl);
                if (ansget1 != null)
                if (ansget1 != null)
                {
                    if (ansget1.KontaUmowy != null)
                        if (ansget1.KontaUmowy.GetUpperBound(0) >= 0)
                            if (ansget1.KontaUmowy[0].NumerPartnera == inDok.Dluznik.SAPKontoPartnera && ansget1.KontaUmowy[0].IDPrzedmiotuUmowy == inDok.Sprawa.SAPPrzedmiotUmowy) { goto skipnewkdl; }
                }
                Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy updtKdl1 = new Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy();
                updtKdl1.TypKontoUmowy = getkdl.TypKontaUmowy;
                updtKdl1.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                updtKdl1.IDPartnera = string.IsNullOrWhiteSpace(nrPartnera) ? inDok.Dluznik.SAPKontoPartnera : nrPartnera;
                updtKdl1.RelacjaKonta = getkdl.RelacjaPartneraHandlowego;
                if (String.IsNullOrEmpty(updtKdl1.RelacjaKonta)) updtKdl1.RelacjaKonta = "99";
                if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                    updtKdl1.StanowiskoFinansowe = knf.StanowiskoFin;
#if RelationCreate
                RelationCreateResponse ans2 = ZSRKRequestHelper.UtworzRelacje(inDok.Sprawa.SAPKontoUmowy, inDok.Sprawa.SAPPrzedmiotUmowy);
#else
                ContractAccountRelationCreateResponse ans2 = ZSRKRequestHelper.AktualizujKontoUmowy(updtKdl1, inDok.Sprawa.SAPPrzedmiotUmowy);
#endif
                if (ans2.Komunikaty != null && ans2.Komunikaty.Any())
                {
                    if (ans2.Komunikaty[0].RodzajKomunikatu == "E")
                    {
                        inDok.SAPImportInfo = ((ans2.Komunikaty != null) ? "; " + ans2.Komunikaty.FirstOrDefault().Komunikat1 : "") + " " + inDok.SAPImportInfo;
                        if (!skipSave) RupDatabase.theContext.SaveChanges();

                        MessageBox.Show("Założono konto umowy ale nie powiązano go z sygnaturą ");
                        return retvalue;
                    }
                }
                else
                {
                    MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd wywołania usługi sieciowej - [Aktualizuj konto umowy] ");
                    return retvalue;
                }
            skipnewkdl:
                // Ksiegowanie dokumentu
                //return retvalue;
                if (wariant > 0)  // jeśli z ksiegowaniem
                {


                    if (inDok.SAPDocId != null && !String.IsNullOrEmpty(inDok.SAPDocId.Trim())) return retvalue;
                    if (inDok.Sprawa.SAPKontoUmowy == null || inDok.Dluznik.SAPKontoPartnera == null || inDok.Sprawa.SAPPrzedmiotUmowy == null || String.IsNullOrEmpty(inDok.Sprawa.SAPPrzedmiotUmowy) || String.IsNullOrEmpty(inDok.Dluznik.SAPKontoPartnera) || String.IsNullOrEmpty(inDok.Sprawa.SAPPrzedmiotUmowy))
                    {
                        MessageBox.Show("Brak obiektów podstawowywch do zaksięgowania dokumentu");
                        return retvalue;
                    }

                    getkdl = setupGetKonto(inDok, knf);

                    //if ( !String.IsNullOrWhiteSpace(knf.StanowiskoFin)) 
                    //        getkdl.SadFunkcjonalnyStanowiskoFinansowe  = knf.StanowiskoFin;

                    getkdl = setupGetKonto(inDok, knf);
                    ContractAccountQueryResponse ansget2 = ZSRKRequestHelper.WyszukajKontoUmowy(getkdl);
                    if (ansget2 != null)
                    {
                        if (ansget2.KontaUmowy != null && ansget2.KontaUmowy.GetUpperBound(0) >= 0 && ansget2.KontaUmowy[0].NumerPartnera == inDok.Dluznik.SAPKontoPartnera && ansget2.KontaUmowy[0].IDPrzedmiotuUmowy == inDok.Sprawa.SAPPrzedmiotUmowy)
                            ;
                        else
                        {
                            inDok.Sprawa.SAPKontoUmowy = null;
                            inDok.Dluznik.SAPKontoPartnera = null;
                            inDok.Sprawa.SAPPrzedmiotUmowy = null;
                            inDok.Dluznik.Nip = null;
                            inDok.Dluznik.Pesel = null;
                            goto nextTry;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Błąd usługi sieciowej [Wyszukaj konto umowy]");
                        return retvalue;
                    }

                    string typop = inDok.typFakt.Trim().ToUpper();

                    if (dKsiegowania != DateTime.MinValue)
                        inDok.DataKsiegowania = dKsiegowania;

                    DocumentCreateRequest adddok = this.setupPrzypis(inDok, knf, kluczUzg);
                    DocumentCreateResponse ansdok = ZSRKRequestHelper.DodajPrzypis(adddok);
                    if (ansdok != null)
                    {
                        if (!String.IsNullOrWhiteSpace(ansdok.IDDokument))
                        {
                            if (ansdok.Komunikaty[0].RodzajKomunikatu == "E")
                            {
                                inDok.SAPImportInfo = ((ansdok.Komunikaty != null && ansdok.Komunikaty.Any() && ansdok.Komunikaty.GetUpperBound(0) >= 0 ? ansdok.Komunikaty[0].Komunikat1 : "")) + " " + inDok.SAPImportInfo;
                                if (!skipSave) RupDatabase.theContext.SaveChanges();

                                MessageBox.Show("Błąd eksportu dokumentu do ZSRK");
                                return retvalue;

                            }
                            else
                            {

                                inDok.SAPDocId = ansdok.IDDokument;
                                retvalue += ";" + ansdok.IDDokument;
                                inDok.SAPKluczUzgodnienia = kluczUzg;
                                if (!skipSave) RupDatabase.theContext.SaveChanges();

                            }
                        }
                        else
                        {
                            MessageBox.Show(";Błąd eksportu dokumentu " + (ansdok.Komunikaty != null && ansdok.Komunikaty.Any() && ansdok.Komunikaty.GetUpperBound(0) >= 0 ? ansdok.Komunikaty[0].Komunikat1 : ""));
                            return retvalue;
                        }



                    }
                    if (inDok.SAPDocId != null && inDok.Sprawa.SAPKontoUmowy != null && inDok.Dluznik.SAPKontoPartnera != null && inDok.Sprawa.SAPPrzedmiotUmowy != null && inDok.SAPDocId.Trim().Length > 0 && inDok.Sprawa.SAPKontoUmowy.Trim().Length > 0 && inDok.Dluznik.SAPKontoPartnera.Trim().Length > 0 && inDok.Sprawa.SAPPrzedmiotUmowy.Trim().Length > 0)
                    {
                        inDok.SAPImportStatus = 1;
                        if (!skipSave) RupDatabase.theContext.SaveChanges();

                    }


                }

                Cursor.Current = Cursors.Default;

            }

            catch (Exception ex)
            {

                MessageBox.Show("Błąd: " + ex.Message + "\nStack trace : " + ex.StackTrace + (ex.InnerException != null ? " Szczegóły " + ex.InnerException.Message : ""));
                log.Error("Błąd metody DoExport", ex);
                Utils.LogWriter("Błąd DoExport: " + ex.Message + "\n" + (ex.InnerException != null ? " Szczegóły " + (ex.InnerException != null ? " " + (ex.InnerException != null ? " " + ex.InnerException.Message : "") : "") : "") + "\nStack trace : " + ex.StackTrace);
                foreach (DictionaryEntry de in ex.Data)
                    Utils.LogWriter("    Key: " + de.Key.ToString() + "     Value: " + de.Value);
                Cursor.Current = Cursors.Default;
                return "";
            }
            return retvalue;
        }

        private string komunikty2String(Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.Komunikat[] komunikaty)
        {
            string result = string.Empty;
            if (komunikaty == null)
                return result;
            foreach (Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.Komunikat k in komunikaty)
            {
                if (result.Length > 0)
                    result += "\n\r";

                result += "[" + k.IDKomunikatu + "] " + k.Komunikat1 + " (" + k.NumerKomunikatu + ")";

            }
            return result;

        }

        public string ZapiszWPoczekalni(Dokument inDok, int wariant, bool skipSave = false, TextBox tbPrompt = null, string nrPartnera = null, string nrKontaUmowy = null)
        {
            int Id;
            string kluczUzg = "";
            DateTime dKsiegowania;
            string retvalue = "";
            Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.KontoUmowy knu = new Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.KontoUmowy();
            Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.Partner partner = new Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.Partner();
            Sygnatura sygn = new Sygnatura();



            dKsiegowania = DateTime.Today;
            
            PostingDataPrepareRequest request = new PostingDataPrepareRequest();
            ContractAccountCreateResponse ans;
            // Po0bierz aktualny transfer
            inDok.SAPImportInfo = "";
            Cursor.Current = Cursors.WaitCursor;
            if (!skipSave)
            {
                Utils.LogWriter("Entering DoExport");
                RupDatabase.theContext.SaveChanges();
                Utils.LogWriter("DoExport- 0");
            }
            Konfiguracja knf = RupDatabase.theContext.Konfiguracja.FirstOrDefault();
            try
            {
                string IdPrzedmiotuUmowy = string.Empty;

                request.ZadanieKsiegowanie = new ZadanieKsiegowanie();
                Platnosc plat = new Platnosc();
                if (inDok.Sprawa != null && !String.IsNullOrWhiteSpace(inDok.Sprawa.SAPPrzedmiotUmowy))
                    IdPrzedmiotuUmowy = inDok.Sprawa.SAPPrzedmiotUmowy;
                if (string.IsNullOrWhiteSpace(IdPrzedmiotuUmowy))
                {
                    ContractObjectQueryRequest sygnGetquery = setupGetSygnStruct(inDok, knf);
                    ContractObjectQueryResponse getsygn = ZSRKRequestHelper.ZnajdzSygnature(sygnGetquery);
                    if (getsygn == null || getsygn.Sygnatura == null || getsygn.Sygnatura.Length == 0 || getsygn.Sygnatura[0].IDPrzedmiotuUmowy == null)
                    {
                        SygnaturaTworzenie sygnqry = setupSygnStruct(inDok, knf);

                        sygn.JednostkaGospodarcza = sygnqry.JednostkaGospodarcza;
                        sygn.NrSprawy = sygnqry.KolejnyNumerSprawy;
                        sygn.NumerWydzialu = sygnqry.NumerWydzialuISekcji;
                        sygn.PodrodzajSprawy = sygnqry.PodrodzajSprawy;
                        sygn.Repertorium = sygnqry.Repertorium;
                        sygn.RodzajPrzedmiotu = sygnqry.RodzajPrzedmiotuUmowy;
                        sygn.RodzajSprawy = sygnqry.RodzajSprawy;
                        sygn.Rok = sygnqry.Rok;
                        sygn.StanowiskoFinansowe = sygnqry.SadFunkcjonalnyStanowiskoFinansowe;

                    }
                    else
                    if (getsygn.Sygnatura != null && getsygn.Sygnatura.Length == 1 && getsygn.Sygnatura[0].IDPrzedmiotuUmowy != null && getsygn.Sygnatura[0].OznaczeniePrzedmiotuUmowy.StartsWith(String.IsNullOrWhiteSpace(sygnGetquery.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe) ? sygnGetquery.Sygnatura.JednostkaGospodarcza : sygnGetquery.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe))
                    {
                        IdPrzedmiotuUmowy = getsygn.Sygnatura[0].IDPrzedmiotuUmowy;
                    }
                }
                // partner
                // 
                if (string.IsNullOrWhiteSpace(nrPartnera))
                {

                    if (inDok.Dluznik != null && !String.IsNullOrWhiteSpace(inDok.Dluznik.SAPKontoPartnera))
                    {
                        nrPartnera = inDok.Dluznik.SAPKontoPartnera;

                    }
                }
                if (string.IsNullOrWhiteSpace(nrPartnera))
                {
                    Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner part1 = setupBussinessPartner(inDok, knf);
                    if (tbPrompt != null) tbPrompt.Text = inDok.Dluznik.Imie + " " + (String.IsNullOrWhiteSpace(inDok.Dluznik.Nazwisko) ? "" : " " + inDok.Dluznik.Nazwisko);

                    partner.AdresPartner = new Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.AdresPartner();
                    partner.AdresPartner.KodPocztowy = part1.AdresPartner.KodPocztowy;
                    partner.AdresPartner.Kraj = part1.AdresPartner.Kraj;
                    partner.AdresPartner.Miasto = part1.AdresPartner.Miasto;
                    partner.AdresPartner.NumerDomu = part1.AdresPartner.NumerDomu;
                    partner.AdresPartner.NumerDomu2 = part1.AdresPartner.NumerDomu2;
                    partner.AdresPartner.Ulica = part1.AdresPartner.Ulica;
                    partner.Imie = part1.Imie;
                    partner.NazwaOrganizacji1 = part1.NazwaOrganizacji1;
                    partner.NazwaOrganizacji2 = part1.NazwaOrganizacji2;
                    partner.NazwaOrganizacji3 = part1.NazwaOrganizacji3;
                    partner.NazwaOrganizacji4 = part1.NazwaOrganizacji4;
                    partner.Nazwisko = part1.Nazwisko;
                    partner.NIP = part1.NIP;
                    partner.PESEL = part1.PESEL;
                    if (part1.RachunekBankowy != null)
                    {
                        partner.RachunekBankowy = new Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.RachunekBankowy();
                        partner.RachunekBankowy.KodKontrolny = part1.RachunekBankowy.KodKontrolny;
                        partner.RachunekBankowy.KontoBankowe = part1.RachunekBankowy.KontoBankowe;

                    }
                    partner.TypPartnera = part1.TypPartnera;

                }
                if (String.IsNullOrWhiteSpace(nrKontaUmowy))
                {
                    string typkdl = "DO";
                    typkdl = inDok.Sprawa.SAPTypKontaUmowy;
                    string opGl = inDok.OperacjaGlowna; //  = row.Cells["OperacjaGlowna"];

                    knu.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                    knu.TypKontoUmowy = typkdl;
                    

                    if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                        knu.StanowiskoFinansowe = knf.StanowiskoFin;

                    if (!string.IsNullOrWhiteSpace(inDok.Sprawa.SAPRelacjaKontaUmowy))
                    {
                        knu.RelacjaKonta = inDok.Sprawa.SAPRelacjaKontaUmowy;
                    }
                    else
                    {
                        knu.RelacjaKonta = "99";
                    }
                    /*
                    string typKontaBE = "";
                    string erM = "";
                    if (typkdl == "BE")
                    {
                        int rcode = getKontoDepozyt(getkdl, out typKontaBE, out erM);
                        if (rcode < 0)
                        {
                            inDok.SAPImportInfo += erM;

                            MessageBox.Show(erM);
                            return retvalue;
                        }
                        typkdl = typKontaBE;
                        inDok.Sprawa.SAPTypKontaUmowy = typkdl;
                        goto addKontoUmowy;

                    }
                    */
                }


                DocumentCreateRequest adddok = this.setupPrzypis(inDok, knf, kluczUzg);
                ZadanieDokNaglowek nagl = new ZadanieDokNaglowek();
                nagl.DataDokument = adddok.NaglowekDokument.DataDokument;
                nagl.DataKsiegowanie = adddok.NaglowekDokument.DataKsiegowanie;
                nagl.RodzajDokument = adddok.NaglowekDokument.RodzajDokumentu;
                nagl.Referencja = inDok.referencja;
                nagl.Waluta = adddok.NaglowekDokument.Waluta;

                ZadaniePozycjaPH ph = new ZadaniePozycjaPH();
                if (string.IsNullOrWhiteSpace(nrKontaUmowy))
                {
                    ph.DaneKontoUmowy = knu;

                }
                else
                    ph.IDKontoUmowy = nrKontaUmowy;

                if (string.IsNullOrWhiteSpace(nrPartnera))
                {
                    ph.DanePartnera = partner;

                }
                else
                    ph.IDPartner = nrPartnera;

                if (string.IsNullOrWhiteSpace(IdPrzedmiotuUmowy))
                {
                    ph.DaneSygnatura = sygn;


                }
                else
                    ph.IDSygnatura = IdPrzedmiotuUmowy;

                ph.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                ph.Kwota = Convert.ToDecimal(inDok.kwota).ToString(CultureInfo.GetCultureInfo("en-US"));
                ph.OperacjaCz = adddok.PozycjaDokumentPH.OperacjaCz;
                ph.OperacjaGl = adddok.PozycjaDokumentPH.OperacjaGl;
                ph.Tekst =  inDok.tekst;

                ph.DataPlatnosci = adddok.PozycjaDokumentPH.DataPlatnosci;
                ph.FormaPlatnosci = "P";

                request.ZadanieKsiegowanie = new ZadanieKsiegowanie();
                request.ZadanieKsiegowanie.ZadaniePozycjaPH = ph;
                request.ZadanieKsiegowanie.ZadanieDokNaglowek = nagl;
                request.ZadanieKsiegowanie.DanePlatnosci = plat;
                
                try
                {
                    setSAPConnectionParams();
                    PostingDataPrepareResponse resp = ZSRKRequestHelper.WyslijDoPoczekalni(request);

                    if (tbPrompt != null) tbPrompt.Text = inDok.Sprawa.Sygnatura;
                    if (resp != null)
                    {
                        if (!String.IsNullOrWhiteSpace(resp.IDZadanieKsiegowania))
                        {

                            retvalue = resp.IDZadanieKsiegowania;
                            inDok.IDZadanieKsiegowania = resp.IDZadanieKsiegowania;
                            inDok.SAPImportInfo.Truncate(250);
                            if (!skipSave) RupDatabase.theContext.SaveChanges();

                        }
                        else
                        {
                            inDok.SAPImportInfo = (ZSRKRequestHelper.GetErrorMessage() + " " + komunikty2String(resp.Komunikaty)) + " " + inDok.SAPImportInfo;
                            inDok.SAPImportInfo.Truncate(250);
                            if (!skipSave) RupDatabase.theContext.SaveChanges();

                            if (RunMode.silentMode)
                                Utils.LogWriter("Błąd podczas rejestracji w poczekalni " + komunikty2String(resp.Komunikaty));
                            else
                                MessageBox.Show("Błąd podczas rejestracji w poczekalni" + komunikty2String(resp.Komunikaty));
                            return  null ;
                        }
                    }
                }
                catch (Exception ex)
                {
                    inDok.SAPImportInfo = ZSRKRequestHelper.GetErrorMessage() + " " + inDok.SAPImportInfo + " " + ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : "");
                    inDok.SAPImportInfo.Truncate(250);
                    if (!skipSave) RupDatabase.theContext.SaveChanges();

                    if (RunMode.silentMode)
                        Utils.LogWriter("Błąd podczas rejestracji w poczekalni " + ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));
                    else
                        MessageBox.Show("Błąd podczas rejestracji w poczekalni" + ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));
                    return null;

                }

                return retvalue;
            }
            catch (Exception exe)
            {
                inDok.SAPImportInfo = ZSRKRequestHelper.GetErrorMessage() + " " + inDok.SAPImportInfo +  " " + (exe.InnerException != null ? exe.InnerException.Message : "");
                inDok.SAPImportInfo.Truncate(250);
                if (!skipSave) RupDatabase.theContext.SaveChanges();

                if (RunMode.silentMode)
                    Utils.LogWriter("Błąd podczas rejestracji w poczekalni " + exe.Message + " " + (exe.InnerException != null ? exe.InnerException.Message : ""));
                else
                    MessageBox.Show("Błąd podczas rejestracji w poczekalni" + exe.Message + " " + (exe.InnerException != null ? exe.InnerException.Message : ""));
                return null;



            }

        }

        public PaymentListQueryResponse PobierzPozycjePlatnosci(string RodzajRachunkuBankowego, string StatusRozliczenia, string TypPozycji, string DataOd, string DataDo)
        {
            Cursor.Current = Cursors.WaitCursor;
          
            Utils.LogWriter("Entering DoExport");
            RupDatabase.theContext.SaveChanges();
          
            Konfiguracja knf = RupDatabase.theContext.Konfiguracja.FirstOrDefault();
            try
            {
                setSAPConnectionParams();
                var ans1 = ZSRKRequestHelper.PobierzWplatyNierozpoznane(RodzajRachunkuBankowego, StatusRozliczenia, TypPozycji, DataOd, DataDo, knf.JednostkaGospodarcza);

                if (ans1.Komunikaty != null && ans1.Komunikaty.Any())
                {
                    if (ans1.Komunikaty[0].RodzajKomunikatu == "E")
                    {
                        MessageBox.Show((ans1.Komunikaty != null) ? "; " + ans1.Komunikaty.FirstOrDefault().Komunikat1 : "", "Błąd importu nierozpoznanych pozycji wyciągu");
                        return null;

                    }
                }
                return ans1;
            }
            catch (Exception exe)
            {
                if (RunMode.silentMode)
                    Utils.LogWriter("Błąd podczas pobienia pozycji płatności " + exe.Message + " " + (exe.InnerException != null ? exe.InnerException.Message : ""));
                else
                    MessageBox.Show("Błąd podczas pobienia pozycji płatności " + exe.Message + " " + (exe.InnerException != null ? exe.InnerException.Message : ""));

                return null;

            }

        }
    }
}
    



