using Cons2RupModel;
using ConsImport;
using ConsInterfeces.Rup2ConsImportContentSystemData;
using RupDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using System.IO;
using SapPOHelper;
using MessageSignature;


namespace Rup2ConsService
{
    public class RupQueue
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _userName;
        public RupQueue(string userName)
        {

            this._userName = userName;

        }

        public void Pop(int id = 0)
        {
            int confsNumber = 0;
            List<ConsExternalDBConnectionConfig> dbConfigLst = null;
            try
            {
                log.Debug("Połączenie z bazą danych");

                using (RupDBEntities db = new RupDBEntities())
                {
                    log.Debug("Szukanie zadań");
                    var job = db.ConsJobItem.Where(a => a.status == (int)ConsJobStatus.New).OrderBy(a => a.Id).FirstOrDefault();
                    if (job != null)
                    {
#if DEBUG            
                    log.Debug("Znaleziono zadanie, odczyt połączeń");
                    dbConfigLst = db.ConsExternalDBConnectionConfig.Where(a => a.isActive == true).ToList();

#else
                    dbConfigLst = db.ConsExternalDBConnectionConfig.Where(a => a.id == job.consExternalDBConnectionConfig_Id).Where(a => a.isActive == true).ToList();
      
#endif

                        confsNumber = dbConfigLst != null ? dbConfigLst.Count : 0;
                        log.Debug("Pętla po aktywnych połączeniach");
                        foreach (ConsExternalDBConnectionConfig dbConfig in dbConfigLst)
                        {

                            ConsImportFromDB import = new ConsImportFromDB();
                            List<ConsImportData> lst = import.GetDataFromDB(dbConfig, new DateTime(2026, 1, 1), DateTime.Today.AddDays(1));
                            log.Debug("Znaleziono połączeń " + (lst != null ? lst.Count : 0).ToString());
                            foreach (ConsImportData item in lst)
                            {
                                log.Debug("Procedowanie zaimportowanych danych");

                                ConsKartaTransfer karta = new ConsKartaTransfer();
                                karta.dImportu = DateTime.Now;
                                karta.idKomunikatu = Guid.NewGuid().ToString();
                                karta.consJobItemId = job.Id;
                                karta.idSprawyWydzial = item.IdSprawy;
                                karta.idStronyWydzial = item.IdStrony;
                                karta.payload = Utils.SerializeToXmlString(item.importContentSystemDataRequest);
                                karta.hash = Utils.ComputeHash(karta.payload);
                                karta.status = (int)ConsJobStatus.New;
                                db.ConsKartaTransfer.Add(karta);
                                db.SaveChanges();

                                ImportContentSystemDataRequest request = new ImportContentSystemDataRequest();
                                // docelowo powinno być ładowanie z xmla ConsKartaTransfer.payload
                                request.DaneKartyDluznika = item.importContentSystemDataRequest.DaneKartyDluznika;
                                request.ListaDanePartneraBiznesowego = item.importContentSystemDataRequest.ListaDanePartneraBiznesowego;
                                request.DaneDziennika = item.importContentSystemDataRequest.DaneDziennika;
                                request.DaneSygnaturyAkt = item.importContentSystemDataRequest.DaneSygnaturyAkt;
                                request.ListaDaneZdarzen = item.importContentSystemDataRequest.ListaDaneZdarzen;
                                request.GUID = Guid.NewGuid().ToString(); //item.importContentSystemDataRequest.GUID;

                                request.Admin = new Admin();
                                //karta.payload = item.
                                setSAPConnectionParamsCONS();
                                string requestStr;
                                var result = ConsImport.ConsWebServiceHelper.ImportData("ImportContentSystemData", request, out requestStr);
                                karta.status = (int)ConsJobStatus.OnGoing;
                                db.SaveChanges();

                            }
#if DEBUG
                            confsNumber--;
                            if ( confsNumber == 0 )
                            {
                                System.Environment.Exit(0);
                            }
                            
#endif

                        }
                    }
                    else //utworzenie zadań
                    {
                        log.Debug("Tworzenie zadań");
                        List<ConsExternalDBConnectionConfig> lst = db.ConsExternalDBConnectionConfig.Where(a => a.isActive == true).ToList();
                        if (lst != null)
                        {
                            foreach (var el in lst)
                            {
                                ConsJobItem jobItem = new ConsJobItem();
                                jobItem.status = (int)ConsJobStatus.New;
                                jobItem.insertDate = DateTime.Now;
                                jobItem.consExternalDBConnectionConfig_Id = el.id;
                                db.ConsJobItem.Add(jobItem);
                            }
                            db.SaveChanges();

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Error in Pop method", ex);
#if DEBUG
               System.Environment.Exit(0);
#endif

            }
        }

        public void Push(int id = 0)
        {

        }




        public void setSAPConnectionParams()
        {
            using (RupDBEntities context = new RupDBEntities())
            {
                User usr = context.User.Where(a => a.Username == this._userName).FirstOrDefault();
                setSAPConnectionParams(usr);
            }
        }


        private void setSAPConnectionParams(User u)
        {
            using (RupDBEntities context = new RupDBEntities())
            {
                Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

                List<ServiceEndpoint> l = context.ServiceEndpoint.ToList();
                if (l != null)
                    foreach (ServiceEndpoint s in l)
                    {
                        lst.Add(new KeyValuePair<string, string>(s.ServiceName, s.Endpoint));
                    }

                ConsWebServiceHelper.ServiceMapping = lst;
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
        private void setSAPConnectionParamsCONS()
        {
            using (RupDBEntities context = new RupDBEntities())
            {
                User usr = context.User.Where(a => a.Username == this._userName).FirstOrDefault();
                setSAPConnectionParamsCons(usr);
            }
        }

        private void setSAPConnectionParamsCons(User u, bool bezAutent = false)
        {
            using (RupDBEntities context = new RupDBEntities())
            {
                Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

                List<ServiceEndpoint> l = context.ServiceEndpoint.ToList();
                if (l != null)
                    foreach (ServiceEndpoint s in l)
                    {
                        lst.Add(new KeyValuePair<string, string>(s.ServiceName, s.Endpoint));
                    }

                ConsWebServiceHelper.ServiceMapping = lst;
                ConsWebServiceHelper.AuthCert = new X509Certificate2(knf.Pfx, Utils.Decrypt(knf.PfxPassword, "Application error"));

                ConsWebServiceHelper.BasicAuthLogin = knf.WSLogon;
                ConsWebServiceHelper.BasicAuthPassword = knf.WSpwd;
                if (!bezAutent)
                {
                    ConsWebServiceHelper.MEPUser = u.MEPUser;
                    ConsWebServiceHelper.MEPPassword = Utils.Decrypt(u.MEPPassword, "Application error");
                    SignatureHelper.Password = Utils.Decrypt(u.MEPPassword, "Application error");
                    SignatureHelper.SetCert(knf.Cer);
                }
                ConsWebServiceHelper.ApplicationID = knf.AppName;
                ConsWebServiceHelper.JednostkaGospodarcza = knf.JednostkaGospodarcza;



            }


        ;



        }

    }
}
