using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Windows.Forms;
using System.ServiceModel.Channels;

namespace KnsMigrator
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
         testy :  http://sapmitl01.zsrk.ms.gov.pl:56300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_TST&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce
                  http://sapwitl01.zsrk.ms.gov.pl:8100/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_TST&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce
         
           DEV:   http://172.16.34.32:56100/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_DEV&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce
                  http://sapmidl01.zsrk.ms.gov.pl:56100/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_DEV&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce    
           
           wsdl (stary )  : http://172.16.34.32:56100/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=ZMS_KNS_DEV&amp;receiverParty=&amp;receiverService=&amp;interface=SI_KNS_KNS2ZSRK_os&amp;interfaceNamespace=urn%3Akns%3Azsrk%3AServce
         
          PRODULKCJA: http://172.16.34.135:56600/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_PRD&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce
                    http://sapwipl01.zsrk.ms.gov.pl:8100/XISOAPAdapter/MessageServlet?senderParty=&senderService=ZMS_KNS_PRD&receiverParty=&receiverService=&interface=SI_KNS_KNS2ZSRK_os&interfaceNamespace=urn:kns:zsrk:Servce
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
            if (!String.IsNullOrWhiteSpace(UserInfo.MEPUser))
                _erpLogin = UserInfo.MEPUser;
            else
                _erpLogin = knf.ERPLogon;

            if (knf.EndpointWS.ToUpper().Contains("ZMS_KNS_TST") || knf.EndpointWS.ToUpper().Contains("ZMS_KNS_DEV"))
                _pwd = ((!String.IsNullOrEmpty(knf.WSpwd)) ? Utils.Decrypt(knf.WSpwd, "Application error") : "");
            else
            {
                //_login = "EPI02";
                _login = "EPI_TMP";
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
            //MessageBox.Show("Data:" + padm.DataWyslania + " Godzina:" + padm.GodzinaWyslania + " User SAP:" + padm.Uzytkownik);
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
        public SapPIService.PokazZaksiegowaneWplatyOdpowiedz PokazWplatyZaksiegowane(DateTime DataOd, DateTime DataDo)
        {
            SapPIService.PokazZaksiegowaneWplatyZapytanie myquery = new SapPIService.PokazZaksiegowaneWplatyZapytanie();
             try
            {
            if (!initWebMethod()) return null;
            myquery.DataKsiegowaniaOd = DataOd.ToString("yyyyMMdd");
            myquery.DataKsiegowaniaDo = DataDo.ToString("yyyyMMdd");
            myquery.ParametryAdministracyjne = this.paramAdm();
            return theClient.PokazZaksiegowaneWplaty(myquery);
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
                //myquery.DaneDoWindykacjiJednostkaGospodarcza = null;
                addsygn.SygnaturaZapytanie = myquery;

                //return  theClient.DodajSygnature(addsygn);
                if (!String.IsNullOrWhiteSpace(myquery.DaneDoWindykacjiJednostkaGospodarcza))
                {
                    if (myquery.DaneDoWindykacjiJednostkaGospodarcza.Substring(0, 1) != "3" && !String.IsNullOrWhiteSpace(myquery.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe))
                    {
                        Utils.showMessage("Niepoprawne stanowisko finansowe sądu windykacyjnego: " + myquery.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe);
                        return null;
                    }
                    JGWindyk = myquery.DaneDoWindykacjiJednostkaGospodarcza;
                    stanfinWindyk = myquery.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe;

                    addsygn.SygnaturaZapytanie.DaneDoWindykacjiJednostkaGospodarcza = null;
                    addsygn.SygnaturaZapytanie.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe = null;
                }
                // Obsługa sądów windykacyjnych 
                if ( addsygn.SygnaturaZapytanie.JednostkaGospodarcza.Substring(0,1) != "3" && !String.IsNullOrWhiteSpace(addsygn.SygnaturaZapytanie.SadFunkcjonalnyStanowiskoFinansowe ))
                {
                    Utils.showMessage("Niepoprawne stanowisko finansowe sygnatury " + addsygn.SygnaturaZapytanie.SadFunkcjonalnyStanowiskoFinansowe);
                    return null;
                }
                answer = theClient.DodajSygnature(addsygn);
                if (String.IsNullOrWhiteSpace(JGWindyk)) return answer;
                if (answer != null && answer.SygnaturaOdpowiedz != null && !String.IsNullOrEmpty(answer.SygnaturaOdpowiedz.IDPrzedmiotuUmowy))
                {

                    addsygn.SygnaturaZapytanie.DaneDoWindykacjiJednostkaGospodarcza = JGWindyk;
                    addsygn.SygnaturaZapytanie.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe = stanfinWindyk;
                    if ((addsygn.SygnaturaZapytanie.DaneDoWindykacjiJednostkaGospodarcza.Substring(0, 1) != "3" && !String.IsNullOrWhiteSpace(addsygn.SygnaturaZapytanie.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe )) )
                    {
                        Utils.showMessage("Niepoprawne stanowisko finansowe sądu windykacyjnego: " + addsygn.SygnaturaZapytanie.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe);
                        return null;
                    }

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

        public SapPIService.PartnerOdpowiedzKomunikatTyp DodajPartnera(SapPIService.PartnerZapytanieTyp myquery)
        {
            SapPIService.PartnerZapytaniePelneTyp addpartner = new SapPIService.PartnerZapytaniePelneTyp();

            try
            {
                if (!initWebMethod()) return null;
                addpartner.ParametryAdministracyjne = this.paramAdm();
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
                if ((addkdl.KontoUmowy.JednostkaGospodarcza.Substring(0, 1) != "3" || addkdl.KontoUmowy.StandardowaJednostkaGospodarcza.Substring(0, 1) != "3") && !String.IsNullOrWhiteSpace(addkdl.KontoUmowy.SadFunkcjonalnyStanowiskoFinansowe))
                {
                    Utils.showMessage("Błąd stanowiska finansowego konta umowy: " + addkdl.KontoUmowy.SadFunkcjonalnyStanowiskoFinansowe);
                    return null;
                }
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

        public SapPIService.WyszukajPartneraOdpowiedz WyszukajPartnera(SapPIService.PartnerWyszukanieTyp partnerId)
        {
            SapPIService.WyszukajPartneraZapytanie getPartner = new SapPIService.WyszukajPartneraZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                getPartner.ParametryAdministracyjne = this.paramAdm();
                getPartner.Partner = partnerId;
                return theClient.WyszukajPartnera(getPartner);

            }
            catch (Exception ex)
            {

                setupExceptionMessage(ex);
                errorStatus = true;
                return null;
            }

        }

        public SapPIService.PobierzRozrachunkiDlaDokumentuOdpowiedz PobierzRozrachunki(String dokIn)
        {
            SapPIService.PobierzRozrachunkiDlaDokumentuZapytanie gerRozrach = new SapPIService.PobierzRozrachunkiDlaDokumentuZapytanie();

            try
            {
                if (!initWebMethod()) return null;
                gerRozrach.ParametryAdministracyjne = this.paramAdm();
                gerRozrach.StanRozrachunkow = new string[1];
                gerRozrach.StanRozrachunkow[0] = dokIn;
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
                sprPlan.WeryfikacjaPlanyRat = new SapPIService.WeryfikacjaPlanuRatTyp();
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
                return theClient.ZmianaTerminuWymagalnosci(dokWymag);

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
    }


}
