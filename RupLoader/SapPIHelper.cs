using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Windows.Forms;
using System.ServiceModel.Channels;

namespace RupLoader
{
    class SapPIHelper
    {
        private SapPIService.SI_KNS_KNS2ZSRK_osClient theClient;
        public string exceptionMessage { get; set; }
        public bool errorStatus { get; set; }
        private string _login;
        private string _pwd;
        private string _jego;
        private string _erpLogin;
        private Konfiguracja konf = new Konfiguracja();

        /* Endpoints
         172.16.34.32 SAPDEV01
         172.16.34.134 SAPTST06
         172.16.34.135 SAPMIP01
         testy : http://172.16.34.134:56300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_TST&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce
         *    http://sapmitl01.zsrk.ms.gov.pl:56300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_TST&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce
         DEV:   http://172.16.34.32:56100/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_DEV&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce 
             wsdl:   http://172.16.34.32:56100/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=ZMS_KNS_DEV&amp;receiverParty=&amp;receiverService=&amp;interface=SI_KNS_KNS2ZSRK_os&amp;interfaceNamespace=urn%3Akns%3Azsrk%3AServce
         PRODULKCJA: http://172.16.34.135:56600/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_PRD&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce
         10.11.92.92 
         
         
         */

        public SapPIHelper()
        {


        }

        public SapPIHelper(Konfiguracja knf)
        {
            konf = knf;
            _login = knf.WSLogon;
            _pwd = ((!String.IsNullOrEmpty(knf.WSpwd)) ? Utils.Decrypt(knf.WSpwd, "Application error") : "");
            _jego = knf.JednostkaGospodarcza;
            _erpLogin = knf.ERPLogon;
            if (knf.EndpointWS.ToUpper().Contains("ZMS_KNS_TST") || knf.EndpointWS.ToUpper().Contains("ZMS_KNS_DEV"))
                    _pwd = ((!String.IsNullOrEmpty(knf.WSpwd)) ? Utils.Decrypt(knf.WSpwd, "Application error") : "");
            else
                {
                    _login = "EPI02";
                    _pwd = Utils.GetWSPwd();
                }
        }
        private bool initWebMethod()
        {
            errorStatus = false;
            exceptionMessage = "";
            if (!this.SetupClientConnection()) return false;
            return true;
        }

        private SapPIService.ParametryAdministracyjneTyp paramAdm()
        {
            SapPIService.ParametryAdministracyjneTyp padm = new SapPIService.ParametryAdministracyjneTyp();

            padm.DataWyslania = DateTime.Today.ToString("yyyyMMdd");
            padm.GodzinaWyslania = DateTime.Now.ToShortTimeString().Replace(":", "").Replace(" ", "") + "00";
            padm.JednostkaGospodarcza = _jego;
            padm.Uzytkownik = _erpLogin;

            return padm;
        }
        private void setupExceptionMessage(Exception ex)
        {

            exceptionMessage += ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");

        }
        private bool SetupClientConnection()
        {
            exceptionMessage = "";
            try
            {
                if (theClient != null) return true;
                CustomBinding cbind = new CustomBinding("CustomHttpTransportBinding");
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                //basicHttpBinding.SendTimeout = new TimeSpan(12000000000);//
                //basicHttpBinding.MaxReceivedMessageSize = 2147483647;
                //basicHttpBinding.MaxBufferSize = 2147483647;
                //basicHttpBinding.ReaderQuotas.MaxStringContentLength = 1048576;

                SapPIService.SI_KNS_KNS2ZSRK_osClient theClient1 = new SapPIService.SI_KNS_KNS2ZSRK_osClient("HTTP_Port");
                EndpointAddress basicAuthEndpoint = new EndpointAddress(new Uri(this.konf.EndpointWS), theClient1.Endpoint.Address.Identity, theClient1.Endpoint.Address.Headers);

                theClient = new SapPIService.SI_KNS_KNS2ZSRK_osClient(cbind, basicAuthEndpoint);
                theClient.ClientCredentials.UserName.UserName = _login;
                theClient.ClientCredentials.UserName.Password = _pwd;
                return true;

            }
            catch (Exception ex)
            {
                errorStatus = true;
                setupExceptionMessage(ex);
                return false;
            }
        }


        // klasa do wprowadzania poleceń SAP
        public string Ping()
        {
            try
            {
                if (!initWebMethod()) return null;
                return theClient.Ping("RupIntegrator");

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;

            }


        }

        public SapPIService.WyszukajSygnatureResponse ZnajdzSygnature(SapPIService.SygnaturaWyszukanieTyp myquery)
        {
            SapPIService.WyszukajSygnatureZapytanie getsygn = new SapPIService.WyszukajSygnatureZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                getsygn.ParametryAdministracyjne = this.paramAdm();
                getsygn.SygnaturaWyszukanie = myquery;

                return theClient.WyszukajSygnature(getsygn);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }
        }

        public SapPIService.DodajSygnatureOdpowiedzTyp ZalozSygnature(SapPIService.SygnaturaZapytanieTyp myquery)
        {
            SapPIService.DodajSygnatureZapytanieTyp addsygn = new SapPIService.DodajSygnatureZapytanieTyp();
            string JGWindyk = null;
            string stanfinWindyk = null;
            SapPIService.DodajSygnatureOdpowiedzTyp answer;

            try
            {
                if (!initWebMethod()) return null;
                addsygn.ParametryAdministracyjne = this.paramAdm();
                addsygn.SygnaturaZapytanie = myquery;
                if (!String.IsNullOrWhiteSpace(myquery.DaneDoWindykacjiJednostkaGospodarcza))
                {
                    JGWindyk = myquery.DaneDoWindykacjiJednostkaGospodarcza;
                    stanfinWindyk = myquery.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe;
                    addsygn.SygnaturaZapytanie.DaneDoWindykacjiJednostkaGospodarcza = null;
                    addsygn.SygnaturaZapytanie.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe = null;
                }
                // Obsługa sądów windykacyjnych 

                answer = theClient.DodajSygnature(addsygn);
                if (String.IsNullOrWhiteSpace(JGWindyk)) return answer;
                if (answer != null && answer.SygnaturaOdpowiedz != null && !String.IsNullOrEmpty(answer.SygnaturaOdpowiedz.IDPrzedmiotuUmowy))
                {

                    addsygn.SygnaturaZapytanie.DaneDoWindykacjiJednostkaGospodarcza = JGWindyk;
                    addsygn.SygnaturaZapytanie.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe = stanfinWindyk;
                    return theClient.DodajSygnature(addsygn);

                }
                else return answer;


            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        private SapPIService.PartnerWyszukanieTyp setupGetPartnerbyNIP(String fizpraw, string NIP, string PESEL)
        {
            SapPIService.PartnerWyszukanieTyp partn = new SapPIService.PartnerWyszukanieTyp();

            partn.TypPartnera = fizpraw;
            //#PA podwójne zapytania partn.NumerPartnera = NIP;
            partn.NIP = NIP;
            partn.PESEL = PESEL;

            return partn;

        }


        public SapPIService.PartnerOdpowiedzKomunikatTyp DodajPartnera(SapPIService.PartnerZapytanieTyp myquery)
        {
            SapPIService.PartnerZapytaniePelneTyp addpartner = new SapPIService.PartnerZapytaniePelneTyp();

            try
            {
                if (!initWebMethod()) return null;
                addpartner.ParametryAdministracyjne = this.paramAdm();
                if ((myquery.TypPartnera == "2" && !String.IsNullOrWhiteSpace(myquery.NIP)) || (myquery.TypPartnera == "1" && !String.IsNullOrWhiteSpace(myquery.PESEL)))
                {
                    SapPIService.PartnerWyszukanieTyp partner = this.setupGetPartnerbyNIP(myquery.TypPartnera,myquery.NIP,myquery.PESEL);
                    SapPIService.WyszukajPartneraOdpowiedz anspartner = this.WyszukajPartnera(partner);
                    if (anspartner != null && anspartner.Partnerzy != null && anspartner.Partnerzy.GetUpperBound(0) >= 0 ) //#PA jeśli znalazł partnera to nie dodajemy# && anspartner.Partnerzy[0].NIP == myquery.NIP)
                    {
                        SapPIService.PartnerOdpowiedzKomunikatTyp addanswer = new SapPIService.PartnerOdpowiedzKomunikatTyp();
                     addanswer.PartnerOdpowiedz = new SapPIService.PartnerOdpowiedzTyp();
                     addanswer.PartnerOdpowiedz.NumerPartnera = anspartner.Partnerzy[0].NumerPartnera;
                     return addanswer;   
                    }
                
                }
                addpartner.Partner = myquery;
                return theClient.DodajPartnera(addpartner);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }
        public SapPIService.DodajKontoUmowyOdpowiedz DodajKontoUmowy(SapPIService.DodajKontoUmowyZapytanieTyp mykdl)
        {
            SapPIService.DodajKontoUmowyZapytanie addkdl = new SapPIService.DodajKontoUmowyZapytanie();

            try
            {
                if (!initWebMethod()) return null;

                addkdl.ParametryAdministracyjne = this.paramAdm();
                addkdl.KontoUmowy = mykdl;
                return theClient.DodajKontoUmowy(addkdl);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }
        public SapPIService.AktualizujKontoUmowyOdpowiedz AktualizujKontoUmowy(SapPIService.AktualizujKontoUmowyZapytanieTyp mykdl)
        {
            SapPIService.AktualizujKontoUmowyZapytanie addkdl = new SapPIService.AktualizujKontoUmowyZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                addkdl.ParametryAdministracyjne = this.paramAdm();
                addkdl.AktualizujKontoUmowy = mykdl;
                return theClient.AktualizujKontoUmowy(addkdl);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        public SapPIService.WyszukajKontoUmowyOdpowiedz WyszukajKontoUmowy(SapPIService.WyszukajKontoUmowyZapytanieTyp mykdl)
        {
            SapPIService.WyszukajKontoUmowyZapytanie getkdl = new SapPIService.WyszukajKontoUmowyZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                getkdl.ParametryAdministracyjne = this.paramAdm();
                getkdl.KontoUmowy = mykdl;
                return theClient.WyszukajKontoUmowy(getkdl);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        public SapPIService.WyszukajPartneraOdpowiedz WyszukajPartnera(SapPIService.PartnerWyszukanieTyp  partnerId)
        {
            SapPIService.WyszukajPartneraZapytanie getPartner = new SapPIService.WyszukajPartneraZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                getPartner.ParametryAdministracyjne = this.paramAdm();
                getPartner.Partner  = partnerId;
                return theClient.WyszukajPartnera(getPartner);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        public SapPIService.PobierzRozrachunkiDlaDokumentuOdpowiedz PobierzRozrachunki  (String  dokIn)
        {
            SapPIService.PobierzRozrachunkiDlaDokumentuZapytanie  gerRozrach = new SapPIService.PobierzRozrachunkiDlaDokumentuZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                gerRozrach.ParametryAdministracyjne = this.paramAdm();
                gerRozrach.StanRozrachunkow = new string[1];
                gerRozrach.StanRozrachunkow[0]  =  dokIn;
                return theClient.PobierzRozrachunkiDlaDokumentu(gerRozrach);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        public SapPIService.PobierzRozrachunkiDlaDokumentuOdpowiedz PobierzRozrachunki(String[] dokIn)
        {
            SapPIService.PobierzRozrachunkiDlaDokumentuZapytanie gerRozrach = new SapPIService.PobierzRozrachunkiDlaDokumentuZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                gerRozrach.ParametryAdministracyjne = this.paramAdm();
                gerRozrach.StanRozrachunkow = dokIn;
                return theClient.PobierzRozrachunkiDlaDokumentu(gerRozrach);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }


        public SapPIService.WeryfikacjaPlanuRatOdpowiedz SprawdzPlanRat(String dokIn)
        {
            SapPIService.WeryfikacjaPlanuRatZapytanie sprPlan = new SapPIService.WeryfikacjaPlanuRatZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                sprPlan.ParametryAdministracyjne = this.paramAdm();
                sprPlan.WeryfikacjaPlanyRat  = new SapPIService.WeryfikacjaPlanuRatTyp ();
                sprPlan.WeryfikacjaPlanyRat.NumerDokumentuRozrachunkow = dokIn;
                return theClient.WeryfikacjaPlanuRat(sprPlan);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        public SapPIService.DezaktywacjaPlanuRatOdpowiedz DzeaktywujPlanRat(String planRat)
        {
            SapPIService.DezaktywacjaPlanuRatZapytnie dezPlan = new SapPIService.DezaktywacjaPlanuRatZapytnie();

            try
            {
                if (!initWebMethod()) return null;
                dezPlan.ParametryAdministracyjne = this.paramAdm();
                dezPlan.NumerPlanuRat = planRat;
                return theClient.DezaktywacjaPlanuRat(dezPlan);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }


        public SapPIService.ZmianaTerminuWymagalnosciOdpowiedz ZmienTerminWymagalnosci(String docId, string newDate)
        {
            SapPIService.ZmianaTerminuWymagalnosciZapytanie dokWymag = new SapPIService.ZmianaTerminuWymagalnosciZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                dokWymag.ParametryAdministracyjne = this.paramAdm();
                dokWymag.NumerDokumentu = docId;
                dokWymag.DataWymagalnosci = newDate;
                return theClient.ZmianaTerminuWymagalnosci (dokWymag);

            }

            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        public SapPIService.DodajNaleznoscOdpowiedz DodajPrzypis(SapPIService.NaleznoscTyp mydok)
        {
            SapPIService.DodajNaleznoscZapytanie adddok = new SapPIService.DodajNaleznoscZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                adddok.ParametryAdministracyjne = this.paramAdm();
                adddok.Naleznosci = new SapPIService.NaleznoscTyp[1];
                adddok.Naleznosci[0] = mydok;
                return theClient.DodajNaleznosc(adddok);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }


        public SapPIService.OdpisanieNaleznosciOdpowiedz OdpiszNaleznosc(SapPIService.OdpisanieNaleznosciTyp mydok, string NumDokOdpis)
        {
            SapPIService.OdpisanieNaleznosciZapytanie adddok = new SapPIService.OdpisanieNaleznosciZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                adddok.ParametryAdministracyjne = this.paramAdm();
                adddok.OdpisanieNaleznosci = mydok;
                adddok.NumerDokumentuDoOdpisania = NumDokOdpis;
                return theClient.OdpisanieNaleznosci(adddok);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        public string verifySygnatura(SapPIService.SygnaturaZapytanieTyp sygn)
        {
            try
            {
                string sapSad = string.Empty;
                if (!String.IsNullOrWhiteSpace(sygn.SadFunkcjonalnyStanowiskoFinansowe))
                    sapSad = sygn.SadFunkcjonalnyStanowiskoFinansowe;
                else
                    sapSad = sygn.JednostkaGospodarcza;
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    SAPSygnMapTmp sygnatura = context.SAPSygnMapTmp.Where(a => a.JednostkaGospodarcza == sapSad && a.NumerWydzialuISekcji == sygn.NumerWydzialuISekcji && a.Repertorium == sygn.Repertorium && a.Rok == sygn.Rok && a.KolejnyNumerSprawy == sygn.KolejnyNumerSprawy && a.RodzajSprawy == sygn.RodzajSprawy).FirstOrDefault();

                    if (sygnatura != null && !String.IsNullOrWhiteSpace(sygnatura.PrzedmiotUmowy))
                    {

                        return sygnatura.PrzedmiotUmowy;

                    }
                    else
                        return "";

                }
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public void addSygnatura(SapPIService.SygnaturaZapytanieTyp sygn, string sygnIn, string PrzedmiotUmowy)
        {
            try

            {
                string sapSad = string.Empty;
                if (!String.IsNullOrWhiteSpace(sygn.SadFunkcjonalnyStanowiskoFinansowe))
                    sapSad = sygn.SadFunkcjonalnyStanowiskoFinansowe;
                else
                    sapSad = sygn.JednostkaGospodarcza;

                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    SAPSygnMapTmp sygnatura = new SAPSygnMapTmp();
                    sygnatura.JednostkaGospodarcza = sapSad;
                    sygnatura.KolejnyNumerSprawy = sygn.KolejnyNumerSprawy;
                    sygnatura.NumerWydzialuISekcji = sygn.NumerWydzialuISekcji;
                    sygnatura.PrzedmiotUmowy = PrzedmiotUmowy;
                    sygnatura.Repertorium = sygn.Repertorium;
                    sygnatura.RodzajPrzedmiotuUmowy = sygn.RodzajPrzedmiotuUmowy;
                    sygnatura.RodzajSprawy = sygn.RodzajSprawy;
                    sygnatura.Rok = sygn.Rok;
                    sygnatura.Sygnatura = sygnIn;

                    context.SAPSygnMapTmp.AddObject(sygnatura);
                    context.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public SapPIService.SygnaturaZapytanieTyp setupSygnStruct(rStruct dok, Konfiguracja konf)
        {
            try
            {
                SapPIService.SygnaturaZapytanieTyp sygnqry = new SapPIService.SygnaturaZapytanieTyp();
                sygnqry.JednostkaGospodarcza = dok.SapSad;
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


                sygnqry.NumerWydzialuISekcji = dok.SWydzial;
                sygnqry.Repertorium = dok.SRepertorium.ToUpper();
                sygnqry.KolejnyNumerSprawy = dok.SNumer;
                sygnqry.Rok = dok.SRok;
                sygnqry.RodzajSprawy = dok.SRodzaj;
                sygnqry.RodzajPrzedmiotuUmowy = dok.SRodzajPrzedm;  // rodzaj przedmioru umowy dok.Sprawa.SAPRodzajPrzedmiotuUmowy;
                sygnqry.IloscTomow = "001";
                sygnqry.PodrodzajSprawy = "";

                return sygnqry;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public string getTechSygn(int idConfig)
        {
            int i;
            using (RupIntegratorEntities dbContext = new RupIntegratorEntities())
            {
                RL_Konfig rl = dbContext.RL_Konfig.Where(a => a.id == idConfig).FirstOrDefault();
                if (rl == null) return "";
                if (String.IsNullOrEmpty(rl.EndpointWS) || rl.EndpointWS.Trim().Length < 6) return "";
                if (Int32.TryParse(rl.EndpointWS.Trim().Substring(0, 1), out i) == true && rl.EndpointWS.Trim().Substring(4, 1) == " ")
                // jeśli zaczyna się oznaczeniem sądu funkcjonalnego
                {
                    return rl.EndpointWS.Trim().ToUpper();

                }
                else
                {
                    Konfiguracja knf = dbContext.Konfiguracja.FirstOrDefault();
                    return (String.IsNullOrWhiteSpace(knf.StanowiskoFin) ? knf.JednostkaGospodarcza.Trim() : knf.JednostkaGospodarcza) + " " + rl.EndpointWS.Trim().ToUpper();

                }


            }


        }

    }


}
