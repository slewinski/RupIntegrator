using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using System.ServiceModel.Channels;
using System.ServiceModel;
using RupBig.ServiceReferenceBigMain;
using System.Globalization;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net;


namespace RupBig
{
    class SubmitRqHelper
    {
      //  public  ServiceReferenceBigMain.IG2BIG_packageSubmitRequest_outRequest;
           
        private string  exceptionMessage = "";

        private ServiceReferenceBigMain.G2BIG_packageSubmitRequest_outClient  theClient;

        private string sadWlasny;
        private string sadIdSad;
        private string sysName;
        private List<KnsKsiegi> ksiegiKNS;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SubmitRqHelper()
        {

            ; 
        }


        private ServiceReferenceBigMain.Credentials[] setupCredentials(List<BIG_Big> bigLst = null)
        {
            List<ServiceReferenceBigMain.Credentials> cred = new List<Credentials>();
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                List<BIG_Big> allBigs =context.BIG_Big.Where(a=>a.Obsluga == true ).ToList();
                if (bigLst != null)
                { 
                    allBigs = allBigs.Where(p => bigLst.Any(p2 => p2.IdBig == p.IdBig)).ToList();
                
                
                }

                foreach (BIG_Big bb in allBigs)
                {
                    Credentials c = new Credentials();
                    BIG_User buser = context.BIG_User.Where (a=>a.IdBIG == bb.IdBig && a.IdUser == UserInfo.Id ).FirstOrDefault();
                    if (buser ==  null ) 
                    {
                        MessageBox.Show("Użykownik nie jest uprawniony do komunikacji z BIG. Sprawdź konfigurację użytkowników ");
                        return null;
                    }   
                    c.big_id = (CredentialsBig_id)Enum.Parse(typeof(CredentialsBig_id), bb.BIGID, false);
                    c.password = buser.BigUserSha256; // Utils.Decrypt(buser.BigUserPassword, "Application error");
                    c.subscriberId = bb.SubscriberId;
                    c.userId = buser.BigUserName;
                    cred.Add(c);
                }
            }


            return cred.ToArray(); 
        
        }

       
       

       

        private string getSadIdWlasny()
        {


            if (!String.IsNullOrWhiteSpace(sadIdSad)) return sadIdSad;

            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {

                Konfiguracja konfig = context.Konfiguracja.FirstOrDefault();
                string jg = String.IsNullOrWhiteSpace(konfig.StanowiskoFin) ? konfig.JednostkaGospodarcza : konfig.StanowiskoFin;
                SAPSad ss = context.SAPSad.Where(a => a.kod == jg).FirstOrDefault();
                sadIdSad = jg;
            }
            return sadIdSad;

        }

        private string getSysName()
        {

            if (!String.IsNullOrWhiteSpace(sysName)) return sysName;

            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {

                BIG_Konfig konfig = context.BIG_Konfig.FirstOrDefault();
                if (konfig == null)
                {
                    MessageBox.Show("Brak konfiguracji systemu źródłowego");
                    return "";
                }
                
                sysName = konfig.SysPrefix;
            }
            return sysName;

        
        }

        private string getSadWlasny()
        {
            

            if (! String.IsNullOrWhiteSpace(sadWlasny) ) return sadWlasny;

            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {

                Konfiguracja konfig = context.Konfiguracja.FirstOrDefault();
                string jg = String.IsNullOrWhiteSpace(konfig.StanowiskoFin) ? konfig.JednostkaGospodarcza : konfig.StanowiskoFin;
                SAPSad ss = context.SAPSad.Where(a => a.kod == jg).FirstOrDefault();
                sadIdSad = jg;
                if (ss != null)
                {
                    sadWlasny = ss.sad;
                    switch (ss.typSad.Trim())
                    { 
                        case "SF":
                            sadWlasny = ss.sad.Replace("SF", "Sąd Rejonowy");
                            break;
                        case "SR":
                            sadWlasny = ss.sad.Replace("SR", "Sąd Rejonowy");
                            break;
                        case "SO":
                            sadWlasny = ss.sad.Replace("SO", "Sąd Okręgowy");
                            break;
                        case "SA":
                            sadWlasny = ss.sad.Replace("SO", "Sąd Apelacyjny");
                            break;
                    
                    }
                    return sadWlasny;
                }
            }
            return "";
        
        }

        public bool oprValidate(Operation opr, out string message)
        {
            string step = "";
            EconomicInformation einf;
            message = "";
            try
            {
                if (opr == null)
                {
                    message = "inny błąd podczas tworzenia operacji ";
                    return false;
                
                }
                step = "operationId";
                if (string.IsNullOrWhiteSpace(opr.OperationId))
                {
                    message = "brak identyfikatora operacji";
                    return false;

                }


                step = "typ operacji";
                if (opr.addInformation != null)
                    einf = opr.addInformation;
                else
                    if (opr.updateInformation != null)
                        einf = opr.updateInformation;
                    else return true;
                step = "dł. imię i nazwisko";
                if (String.IsNullOrWhiteSpace(einf.debtor.surename) || String.IsNullOrWhiteSpace(einf.debtor.forename))
                {
                    message = "uzupełnij imię lub nazwisko dłużnika";
                    return false;
                }
                step = "debtorId";
                if (string.IsNullOrWhiteSpace(einf.debtor.debtorId))
                {
                    message = "brak identyfikatora operacji";
                    return false;

                }
                step = "Obywatelstwo";
                if (einf.debtor.debtorIdentity.foreignCitizenIdentity == null && einf.debtor.debtorIdentity.polishCitizenIdentity == null)
                {
                    message = "brak zdefiniowanego obywatelstwa";
                    return false;

                }
                step = "Pesel";
                if (einf.debtor.debtorIdentity.polishCitizenIdentity != null)
                {
                    string pesel = einf.debtor.debtorIdentity.polishCitizenIdentity.pesel ?? "";
                    if (!Utils.ValidatePesel(ref pesel))
                    {
                        message = "błąd numeru PESEL";
                        return false;
                    }

                    step = "Dokument";
                    if (einf.debtor.debtorIdentity.polishCitizenIdentity.document != null && String.IsNullOrWhiteSpace(einf.debtor.debtorIdentity.polishCitizenIdentity.document.documentNumber))
                    {

                        message = "błąd numeru dokumentu tożsamości";
                        return false;

                    }


                }
                if (einf.debtor.debtorIdentity.foreignCitizenIdentity != null)
                {
                    step = "Dokument";
                    if (einf.debtor.debtorIdentity.foreignCitizenIdentity.document != null && String.IsNullOrWhiteSpace(einf.debtor.debtorIdentity.foreignCitizenIdentity.document.documentNumber))
                    {

                        message = "błąd numeru dokumentu tożsamości";
                        return false;

                    }

                }
                step = "Adres";
                if (einf.debtor.residenceAddress == null)
                {
                    message = "brak adresu";
                    return false;



                }
                step = "Kraj";
                if (String.IsNullOrWhiteSpace(einf.debtor.residenceAddress.country.ToString()) || einf.debtor.residenceAddress.country.ToString().Trim().Length != 3)
                {
                    message = "błędny kraj zamieszkania";
                    return false;

                }
                string kr = einf.debtor.residenceAddress.country.ToString();
                if (Utils.kraje.Where(a => a.skrot == kr).FirstOrDefault() == null)
                {
                    message = "błędny kod kraju zamieszkania";
                    return false;

                }

                step = "Miasto";
                if (String.IsNullOrWhiteSpace(einf.debtor.residenceAddress.city) || einf.debtor.residenceAddress.city.Length > 40)
                {

                    message = "błędna miejscowość ( pusta lub dłuższa niż  40 znaków)";
                    return false;

                }

                step = "Kod pocztowy";
                if (String.IsNullOrWhiteSpace(einf.debtor.residenceAddress.postcode) || einf.debtor.residenceAddress.postcode.Length > 10)
                {

                    message = "brak kodu pocztowego lub kod pocztowy zbyt długi > 10 znaków";
                    return false;

                }
                string kp = einf.debtor.residenceAddress.postcode.Trim();
                if (einf.debtor.residenceAddress.country == countryEnum.POL)
                {
                    if (Char.IsDigit(kp[0]) && Char.IsDigit(kp[1]) && Char.IsDigit(kp[3]) && Char.IsDigit(kp[4]) && Char.IsDigit(kp[5]) && (kp[2] == '-'))
                        ;
                    else
                    {
                        message = "błędny format kodu pocztowego";
                        return false;

                    }


                }
                step = "Ulica";

                if (!String.IsNullOrWhiteSpace(einf.debtor.residenceAddress.street) && einf.debtor.residenceAddress.street.Length > 50)
                {

                    message = "ulica zbyt długa > 50 znaków lub pusta";
                    return false;

                }
                step = "Nr domu";
                if (String.IsNullOrWhiteSpace(einf.debtor.residenceAddress.houseNumber) || einf.debtor.residenceAddress.houseNumber.Length > 10)
                {

                    message = "błędny nr domu ( pusty lub dłuższa niż  10 znaków)";
                    return false;

                }

                step = "Nr lokalu";

                if (!String.IsNullOrWhiteSpace(einf.debtor.residenceAddress.localNumber) && einf.debtor.residenceAddress.localNumber.Length > 20)
                {

                    message = "zbyt długi nr mieszkania > 20 znaków";
                    return false;

                }
                step = "Należność";
                if (einf.liability == null || einf.liability.legalTitle == null || einf.liability.legalTitle.enforceableTitle == null)
                {

                    message = "Brak należności lub tytułu wykonawczego";
                    return false;


                }
                step = "Należność";
                if (einf.liability == null || String.IsNullOrWhiteSpace(einf.liability.liabilityId) || einf.liability.legalTitle == null || einf.liability.legalTitle.enforceableTitle == null)
                {

                    message = "Brak należności, jej oznaczenia lub tytułu wykonawczego";
                    return false;


                }
                step = "Należność";
                if (einf.liability.liabilityType == null)
                {
                    message = "Błędny tytuł należności";
                    return false;
                }

                step = "Sąd";
                if (String.IsNullOrWhiteSpace(einf.liability.legalTitle.enforceableTitle.adjudicatingBody) || einf.liability.legalTitle.enforceableTitle.adjudicatingBody.Length > 250)
                {
                    message = "Błędny sąd orzekający";
                    return false;
                }

                step = "Sygnatura";
                if (String.IsNullOrWhiteSpace(einf.liability.legalTitle.enforceableTitle.enforceableTitleId) || einf.liability.legalTitle.enforceableTitle.enforceableTitleId.Length > 50)
                {
                    message = "Błędna sygnatura tytułu";
                    return false;
                }
                step = "Data tyt.";
                if (einf.liability.legalTitle.enforceableTitle.issueDate == null || einf.liability.legalTitle.enforceableTitle.issueDate < new DateTime(2005, 1, 1) || einf.liability.legalTitle.enforceableTitle.issueDate > DateTime.Today.AddDays(-30))
                {
                    message = "Błędna data tytułu wykonawczego ( upraw. orzeczenia)";
                    return false;
                }
                step = "Kwota zaległości";
                if (!(einf.liability.arrearsAmount > 0))
                {
                    message = "Błędna kwota zaległości";
                    return false;
                }
                step = "Data powstania zaległości";
                if (einf.liability.arrearsRiseDate == null || einf.liability.arrearsRiseDate < new DateTime(2005, 1, 1) || einf.liability.arrearsRiseDate > DateTime.Today.AddDays(-30))
                {
                    message = "Błędna data powstania zaległości";
                    return false;
                }
                step = "Data wysłania wezwania";
                if (einf.liability.paymentRequestDispatchDateSpecified && (einf.liability.paymentRequestDispatchDate < new DateTime(2005, 1, 1) || einf.liability.arrearsRiseDate > DateTime.Today.AddDays(-30)))
                {
                    message = "Błędna data wysłania wezwania";
                    return false;
                }
                step = "porównanie dat";
                if (einf.liability.arrearsRiseDate > einf.liability.legalTitle.enforceableTitle.issueDate)
                {

                    message = "Data należności póxniejsza od daty tytułu";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                message = "Błąd podczas walidacji operacji: " + step + " " + ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : "");
                return false;

            }
        }


        private Operation oprSetup(GridViewRowInfo row, int what, ref List<extraData> exData, int typOper)
        {
            // 0 - grzywna
            // 1 koszty
            Utils.LogWriter("Przygotowanie pakietu");
            try
            {
                Operation opr = new Operation();
                EconomicInformation inf = new EconomicInformation();

                Debtor dbt = new Debtor();
              //  string debtorId =   Utils.getDebtorId(Utils.ifDBNULLString(row.Cells["KartaDl"]), "001",Convert.ToInt32(row.Cells["IdSkaz"].Value));

                string debtorId = Utils.getLiabilId(Utils.ifDBNULLString(row.Cells["KartaDl"]), "001", what, Convert.ToInt32(row.Cells["IdNal"].Value));  // getDebtorId(row.Cells["KartaDl"].Value.ToString(), "001", Convert.ToInt32(row.Cells["IdSkaz"].Value));


                if (typOper != 2)
                { 
                    // get debtorid 
                     // pobranie operacji  
                    using (RupIntegratorEntities context = new RupIntegratorEntities())
                    {
                        BIG_InfoOperation bio = context.BIG_InfoOperation.Where(a => a.LiabilityId == debtorId && a.OperType == 1).OrderBy(a => a.IdBIG_InfoOperation).FirstOrDefault();
                        // szukamy pierwszego dopisania.
                        if (bio != null && !string.IsNullOrWhiteSpace(bio.DebatorID))
                            debtorId = bio.DebatorID;
                    }
                }

                dbt.debtorId = debtorId;
                
                Utils.LogWriter("Dłużnik " + dbt.debtorId);
                dbt.forename = Utils.ifDBNULLString(row.Cells["Imie"]);
                dbt.surename = Utils.ifDBNULLString(row.Cells["Nazwisko"]);
                Address adr = new Address();
                adr.country = (Utils.ifDBNULLString(row.Cells["Country"]) == "POL" ? countryEnum.POL : (countryEnum)Enum.Parse(typeof(countryEnum), Utils.ifDBNULLString(row.Cells["Country"]), true));
                adr.city = Utils.ifDBNULLString(row.Cells["City"]);
                adr.street = Utils.ifDBNULLString(row.Cells["Street"]);
                adr.houseNumber = Utils.ifDBNULLString(row.Cells["HouseNumber"]);
                adr.localNumber = Utils.ifDBNULLString(row.Cells["LocalNumber"]);
                adr.postcode = Utils.ifDBNULLString(row.Cells["PostCode"]);
                dbt.residenceAddress = adr;
                Utils.LogWriter("Dłużnik " + dbt.debtorId);
                DebtorIdentity id = new DebtorIdentity();
                if (Utils.ifDBNULLString(row.Cells["Citizen"]) == "POL")
                {
                    PolishCitizenIdentity plid = new PolishCitizenIdentity();
                    plid.pesel = Utils.ifDBNULLString(row.Cells["PESEL"]);
                 
                    /*
                     if (Utils.ifDBNULLString(row.Cells["IdentityCard"]).Trim().Length>3 )
                     {
                         plid.document = new Document();
                         plid.document.documentNumber = Utils.ifDBNULLString(row.Cells["IdentityCard"]).Trim().Truncate(40);
                         plid.document.documentType = documentTypeEnum.idCard;
                         plid.document.documentTypeSpecified = true;
                     }
                    
                    */


                    id.polishCitizenIdentity = plid;
                }
                else
                {
                    ForeignCitizenIdentity fid = new ForeignCitizenIdentity();
                    fid.document = new Document();
                    fid.document.documentType = documentTypeEnum.other;
                    fid.document.documentNumber = Utils.ifDBNULLString(row.Cells["IdentityCard"]).Trim();
                    id.foreignCitizenIdentity = fid;
                    fid.document.documentTypeSpecified = true;
                }
                dbt.debtorIdentity = id;
                inf.debtor = dbt;
                Utils.LogWriter("Dłużnik " + dbt.debtorId);
                Liability li = new Liability();
                li.currency = currencyEnum.PLN;
                if (what == 0) // kolumna grzywna
                {
                    
                    li.liabilityId = Utils.getLiabilId(Utils.ifDBNULLString(row.Cells["KartaDl"]), "001", what, Convert.ToInt32(row.Cells["IdNal"].Value));
                    li.liabilityAmountSpecified = true;
                    int ksiega = Convert.ToInt32(row.Cells["IdKsiega"].Value);
                    KnsKsiegi kk = ksiegiKNS.Where(a => a.Id_Ksiegi == ksiega).FirstOrDefault();
                    if (kk != null && kk.czyFPP == 1)
                    {
                        

                        if (kk.ksGrzFPPMap == 2)
                            li.liabilityType = liabilityTypeEnum.cashBenefits;
                        else
                            li.liabilityType = liabilityTypeEnum.compensation;
                    }
                    else
                    {
                        if (kk != null && kk.czyFPP == 2)
                            li.liabilityType = liabilityTypeEnum.forfeit;
                        else
                        {
                            if (kk != null && kk.czyFPP == 3)
                                li.liabilityType = liabilityTypeEnum.compensatoryDamages;
                            else
                                li.liabilityType = liabilityTypeEnum.fine;
                        }
                    }
                    li.liabilityAmount = Convert.ToDecimal(row.Cells["Grzywna"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                    li.arrearsAmount = Convert.ToDecimal(row.Cells["SaldoGrzywna"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                }
                if (what == 1)  // koszty
                {
                    li.liabilityId = Utils.getLiabilId(Utils.ifDBNULLString(row.Cells["KartaDl"]), "001", what, Convert.ToInt32(row.Cells["IdNal"].Value));
                    li.liabilityAmountSpecified = true;
                    int ksie = Convert.ToInt32(row.Cells["IdKsiega"].Value);
                    KnsKsiegi kk = ksiegiKNS.Where(a => a.Id_Ksiegi == ksie).FirstOrDefault();
                    
                    if (kk != null && kk.czyFPP == 1)
                    {
                    
                        if (kk.ksGrzFPPMap == 2)
                            li.liabilityType = liabilityTypeEnum.compensation;
                        else
                            li.liabilityType = liabilityTypeEnum.cashBenefits;
                    }
                    else
                    {
                        if (kk != null && kk.czyFPP == 2)
                            li.liabilityType = liabilityTypeEnum.forfeit;
                        else
                            li.liabilityType = liabilityTypeEnum.courtCosts;
                    }
                    li.liabilityAmount = Convert.ToDecimal(row.Cells["Koszty"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                    li.arrearsAmount = Convert.ToDecimal(row.Cells["SaldoKoszty"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                    li.liabilityAmountSpecified = true;
                }

            
                //li.
                Utils.LogWriter("Należność ");
                LegalTitle lt = new LegalTitle();
                lt.enforceableTitle = new EnforceableTitle();
                lt.enforceableTitle.adjudicatingBody = String.IsNullOrWhiteSpace(Utils.ifDBNULLString(row.Cells["Sad"]).Trim()) ? getSadWlasny() : Utils.ifDBNULLString(row.Cells["Sad"]);
                lt.enforceableTitle.enforceableTitleId = Utils.ifDBNULLString(row.Cells["Sygnatura"]);
                lt.enforceableTitle.issueDate = !(row.Cells["DataTytWyk"].Value is DBNull) ? Convert.ToDateTime(row.Cells["DataTytWyk"].Value).Date : DateTime.MinValue;
                li.legalTitle = lt;
                if (!(row.Cells["DataNal"].Value is DBNull))
                    li.arrearsRiseDate = Convert.ToDateTime(row.Cells["DataNal"].Value).Date;
                li.disputedAmountSpecified = false;
                if (!(row.Cells["PaymentRequestDispatchDate"].Value is DBNull))
                {
                    li.paymentRequestDispatchDate = Convert.ToDateTime(row.Cells["PaymentRequestDispatchDate"].Value).Date;
                    li.paymentRequestDispatchDateSpecified = true;
                }
                else
                    li.paymentRequestDispatchDateSpecified = false;
                li.shareSuspensionFinalDateSpecified = false;
                inf.liability = li;
                if (typOper == 1)
                {
                    opr.addInformation = inf;
                    opr.OperationId = "A" + "/" + li.liabilityId;
                }
                else
                {
                    opr.updateInformation = inf;
                    opr.OperationId = "U" + "/" + li.liabilityId;


                }
                Utils.LogWriter("Dodano nalezność  " + opr.OperationId);
                if (exData == null)
                    exData = new List<extraData>();
                extraData exDataItem = new extraData();
                exDataItem.operId = opr.OperationId;
                exDataItem.dataname = "KartaDl";
                exDataItem.datavalue = row.Cells["KartaDl"].Value.ToString();
                exData.Add(exDataItem);

                exDataItem = new extraData();
                exDataItem.operId = opr.OperationId;
                exDataItem.dataname = "IdKsiega";
                exDataItem.datavalue = row.Cells["IdKsiega"].Value.ToString();
                exData.Add(exDataItem);

                exDataItem = new extraData();
                exDataItem.operId = opr.OperationId;
                exDataItem.dataname = "Citizen";
                exDataItem.datavalue = row.Cells["Citizen"].Value.ToString();
                exData.Add(exDataItem);
                Utils.LogWriter("Koniec dodawania operacji");
                return opr;
            }
            catch (Exception ex)
            { 
                MessageBox.Show("Błąd tworzenia operacji do BIG "+ ex.Message + (ex.InnerException != null ? " " +ex.InnerException.Message:"" ));
                Utils.LogWriter("Błąd tworzenia operacji " + ex.Message);
                return null;
            }
        }

        private string getPackageId()
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                BIG_Package sp = new BIG_Package();
                context.BIG_Package.AddObject(sp);
                context.SaveChanges();
                return sp.IdBIG_Package.ToString("D10");
            
            
            
            }
        
        
        
        }

        private PackageSubmit setupPackageSubmit(List<GridViewRowInfo> rowLst, ref List<extraData> lstExDta, int typOper, out int errCount)
        {
            PackageSubmit  ps = new PackageSubmit();
            List<Operation> opLst = new List<Operation>(); 
            errCount = 0 ;
            Utils.LogWriter("Przygotowanie pakietu danych");
            foreach (GridViewRowInfo row in rowLst)
            {

                Operation opg;
                Operation opk;
                string mesg = "";
                string mesk="";

                decimal kwtGrz = Convert.ToDecimal(row.Cells["Grzywna"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                decimal kwtKoszty = Convert.ToDecimal(row.Cells["Koszty"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                decimal kwtZalGrz = Convert.ToDecimal(row.Cells["SaldoGrzywna"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                decimal kwtZalKoszty = Convert.ToDecimal(row.Cells["SaldoKoszty"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));

                if ( kwtZalGrz > 0 )
                {

                    opg = oprSetup(row, 0, ref lstExDta, typOper);
                    if ( !oprValidate(opg, out mesg) )
                    {
                        errCount ++;
                        row.Cells["info"].Value = mesg;
                    }
                        opLst.Add(opg);
                }

                if ( kwtZalKoszty > 0 )
                {
                    opk = oprSetup(row, 1, ref lstExDta, typOper);
                    if (!oprValidate(opk, out mesk))
                    {
                        errCount++;
                        row.Cells["info"].Value = mesg + ' ' + mesk;
                    } 
                    opLst.Add(opk);
                }

                

               
                
                }
            
        
           ps.operation = opLst.ToArray();
           ps.packageId = getSadIdWlasny() + "/" + getSysName() + "/" + getPackageId();
           Utils.LogWriter("Koniec tworzenia pakietu " + ps.packageId);
           return ps;
        
        }





        private Operation DbOper2Oper(vw_BIG_Operacje opr, RupIntegratorEntities context, int operType , ref List<extraData> lstExDta)
        {
            Operation op = new Operation();
            EconomicInformation einf = new EconomicInformation();
            op.OperationId = opr.OperationId.Substring(1);

            switch (operType)
            {
                case 1: //
                    op.addInformation = einf;
                    op.OperationId = "A" + op.OperationId;

                    break;
                case 2:
                    op.updateInformation = einf;
                    op.OperationId = "U" + op.OperationId;
                    break;
                case 3:
                    op.deleteInformation = new InformationSelector();
                    op.deleteInformation.debtorId = opr.DebatorID;
                    op.deleteInformation.liabilityId = opr.LiabilityId;
                    op.OperationId = "D" + op.OperationId;
                    break;
                case 4:
                    op.blockInformation = new BlockInformation();
                    op.blockInformation.debtorId = opr.DebatorID;
                    op.blockInformation.liabilityId = opr.LiabilityId;
                    op.blockInformation.shareSuspensionFinalDate = opr.ShareSuspensionFinalDate.Value;

                    break;
                case 5:
                    op.unblockInformation = new InformationSelector();
                    op.deleteInformation.debtorId = opr.DebatorID;
                    op.deleteInformation.liabilityId = opr.LiabilityId;
                    break;
                default:
                    this.exceptionMessage = "Błędny typ operacji ";
                    return null;
            }
            if (opr.OperType == 1 || opr.OperType == 2)
            {
                einf.liability = new Liability();
                einf.liability.legalTitle = new LegalTitle();
                einf.liability.legalTitle.enforceableTitle = new EnforceableTitle();
                einf.liability.liabilityId = opr.LiabilityId;
                einf.liability.liabilityType = (liabilityTypeEnum)Enum.Parse(typeof(liabilityTypeEnum), opr.LiabilityType, false);
                einf.liability.legalTitle.enforceableTitle.adjudicatingBody = opr.AdjudicatingBody;
                einf.liability.legalTitle.enforceableTitle.issueDate = opr.IssueDate.Value;
                einf.liability.legalTitle.enforceableTitle.enforceableTitleId = opr.Sygnatura;
                einf.liability.currency = (currencyEnum)Enum.Parse(typeof(currencyEnum), opr.Currency, true);
                einf.liability.arrearsAmount = opr.ArrearsAmount.Value;
                einf.liability.arrearsRiseDate = opr.ArrearsRiseDate.Value;

                if ((opr.DisputedAmount ?? 0) > 0)
                {
                    einf.liability.disputedAmountSpecified = true;
                    einf.liability.disputedAmount = opr.DisputedAmount.Value;
                }
                else
                    einf.liability.disputedAmountSpecified = false;

                if ((opr.LiabilityAmount ?? 0) > 0)
                {
                    einf.liability.liabilityAmount = opr.LiabilityAmount.Value;
                    einf.liability.liabilityAmountSpecified = true;

                }
                else
                    einf.liability.liabilityAmountSpecified = false;

                if (opr.PaymentRequestDispatchDate != null)
                {
                    einf.liability.paymentRequestDispatchDateSpecified = true;
                    einf.liability.paymentRequestDispatchDate = opr.PaymentRequestDispatchDate.Value;
                }
                else
                    einf.liability.paymentRequestDispatchDateSpecified = false;

                if (opr.ShareSuspensionFinalDate != null)
                {
                    einf.liability.shareSuspensionFinalDateSpecified = true;
                    einf.liability.shareSuspensionFinalDate = opr.ShareSuspensionFinalDate.Value;
                }
                else
                    einf.liability.shareSuspensionFinalDateSpecified = false;
                // dłużnik
                einf.debtor = new Debtor();
                einf.debtor.debtorId = opr.DebatorID;
                einf.debtor.forename = opr.Forename;
                einf.debtor.surename = opr.Surename;
                if (!String.IsNullOrWhiteSpace(opr.CA_City))
                {
                    einf.debtor.corespondenceAddress = new Address();
                    einf.debtor.corespondenceAddress.city = opr.CA_City;
                    einf.debtor.corespondenceAddress.country = (countryEnum)Enum.Parse(typeof(countryEnum), opr.CA_Country, true);
                    einf.debtor.corespondenceAddress.houseNumber = opr.CA_HouseNumber;
                    einf.debtor.corespondenceAddress.localNumber = opr.CA_LocalNumber;
                    einf.debtor.corespondenceAddress.postcode = opr.CA_Postcode;
                    einf.debtor.corespondenceAddress.street = opr.CA_Street;
                }

                if (!String.IsNullOrWhiteSpace(opr.RA_City))
                {
                    einf.debtor.residenceAddress = new Address();
                    einf.debtor.residenceAddress.city = opr.RA_City;
                    einf.debtor.residenceAddress.country = (countryEnum)Enum.Parse(typeof(countryEnum), opr.RA_Country, true);
                    einf.debtor.residenceAddress.houseNumber = opr.RA_HouseNumber;
                    einf.debtor.residenceAddress.localNumber = opr.RA_LocalNumber;
                    einf.debtor.residenceAddress.postcode = opr.RA_Postcode;
                    einf.debtor.residenceAddress.street = opr.RA_Street;
                }
                einf.debtor.debtorIdentity = new DebtorIdentity();
                if (String.IsNullOrWhiteSpace(opr.Pesel))
                {

                    einf.debtor.debtorIdentity.foreignCitizenIdentity = new ForeignCitizenIdentity();

                    if (!String.IsNullOrWhiteSpace(opr.DocumentNumber))
                    {
                        einf.debtor.debtorIdentity.foreignCitizenIdentity.document = new Document();
                        einf.debtor.debtorIdentity.foreignCitizenIdentity.document.documentTypeSpecified = true;
                        einf.debtor.debtorIdentity.foreignCitizenIdentity.document.documentNumber = opr.DocumentNumber;
                        einf.debtor.debtorIdentity.foreignCitizenIdentity.document.documentType = (documentTypeEnum)Enum.Parse(typeof(documentTypeEnum), opr.DocumentType, true);
                    }
                }
                else
                {
                    einf.debtor.debtorIdentity.polishCitizenIdentity = new PolishCitizenIdentity();
                    if (!String.IsNullOrWhiteSpace(opr.DocumentNumber))
                    {
                        einf.debtor.debtorIdentity.polishCitizenIdentity.document = new Document();
                        einf.debtor.debtorIdentity.polishCitizenIdentity.document.documentTypeSpecified = true;
                        einf.debtor.debtorIdentity.polishCitizenIdentity.document.documentNumber = opr.DocumentNumber;
                        einf.debtor.debtorIdentity.polishCitizenIdentity.document.documentType = (documentTypeEnum)Enum.Parse(typeof(documentTypeEnum), opr.DocumentType, true);

                    }
                    einf.debtor.debtorIdentity.polishCitizenIdentity.pesel = opr.Pesel;
                }

            }
            if (lstExDta == null)
                lstExDta = new List<extraData>();
            extraData exDataItem = new extraData();
            exDataItem.operId = op.OperationId;
            exDataItem.dataname = "KartaDl";
            exDataItem.datavalue = opr.KartaDl;
            lstExDta.Add(exDataItem);

            exDataItem = new extraData();
            exDataItem.operId = op.OperationId;
            exDataItem.dataname = "IdKsiega";
            exDataItem.datavalue = opr.IdKsiega.ToString();
            lstExDta.Add(exDataItem);


            return op;
        }



        private vw_BIG_Operacje Dlu2BIGOper(vw_BIG_Dluznicy opr)
        {
            vw_BIG_Operacje op = new vw_BIG_Operacje();

            op.IdBIG_InfoOperation = opr.IdBIG_InfoOperation;
            op.OperType = opr.OperType;
            op.IdBIGPackage = opr.IdBIGPackage;
            op.OperationId = opr.OperationId;
            op.DebatorID = opr.DebatorID;
            op.Forename = opr.Forename;
            op.Surename = opr.Surename;
            op.Citizenship = opr.Citizenship;
            op.Pesel = opr.Pesel;
            op.DocumentType = opr.DocumentType;
            op.DocumentNumber = opr.DocumentNumber;
            op.RA_Country = opr.RA_Country;
            op.RA_Postcode = opr.RA_Postcode;
            op.RA_City = opr.RA_City;
            op.RA_Street = opr.RA_Street;
            op.RA_HouseNumber = opr.RA_HouseNumber;
            op.RA_LocalNumber = opr.RA_LocalNumber;
            op.CA_Country = opr.CA_Country;
            op.CA_Postcode = opr.CA_Postcode;
            op.CA_City = opr.CA_City;
            op.CA_Street = opr.CA_Street;
            op.CA_HouseNumber = opr.CA_HouseNumber;
            op.CA_LocalNumber = opr.CA_LocalNumber;
            op.LiabilityId = opr.LiabilityId;
            op.InstitutionDataShareRestricted = opr.InstitutionDataShareRestricted;
            op.ShareSuspensionFinalDate = opr.ShareSuspensionFinalDate;
            op.LiabilityType = opr.LiabilityType;
            op.Sygnatura = opr.Sygnatura;
            op.AdjudicatingBody = opr.AdjudicatingBody;
            op.IssueDate = opr.IssueDate;
            op.OtherTitle = opr.OtherTitle;
            op.Currency = opr.Currency;
            op.LiabilityAmount = opr.LiabilityAmount;
            op.ArrearsAmount = opr.ArrearsAmount;
            op.DisputedAmount = opr.DisputedAmount;
            op.ArrearsRiseDate = opr.ArrearsRiseDate;
            op.PaymentRequestDispatchDate = opr.PaymentRequestDispatchDate;
            op.KartaDl = opr.KartaDl;
            op.DataPrzypisu = opr.DataPrzypisu;
            op.IdKsiega = opr.IdKsiega;
            op.PackageFullId = opr.PackageFullId;
            op.SentDate = opr.SentDate;
            op.SentStatusInfo = opr.SentStatusInfo;
            op.SentStatus = opr.SentStatus;
            op.KnsKsiega = opr.KnsKsiega;
            op.status = opr.status;
            op.ileOK = opr.ileOK;


            return op;
        }




        private PackageSubmit setupPackageSubmitFromDB(List<GridViewRowInfo> rowLst, ref List<extraData> lstExDta, int what)
        {
            PackageSubmit ps = new PackageSubmit();
            List<Operation> opLst = new List<Operation>();
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
            foreach (GridViewRowInfo row in rowLst)
            {
               vw_BIG_Operacje biop = null;
                if (  (row.DataBoundItem).GetType() == typeof (vw_BIG_Operacje))
                  biop = (vw_BIG_Operacje)row.DataBoundItem;
                else
                 {
                 int operid =  ((vw_BIG_Dluznicy)row.DataBoundItem).IdBIG_InfoOperation;
                 biop = Dlu2BIGOper((vw_BIG_Dluznicy)row.DataBoundItem); 
                 }
                    
                    if (biop == null)
                 {
                     MessageBox.Show("Błąd odczytu z bazy danych");
                     return null;
                 }
                 Operation opg = this.DbOper2Oper(biop, context, what,ref lstExDta);
                 
                 opLst.Add(opg);

                }
  
            }


            ps.operation = opLst.ToArray();
            ps.packageId = getSadIdWlasny() + "/" + getSysName() + "/" + getPackageId();
            return ps;

        }

        
        public Package CreateInsUpdtRq(List<GridViewRowInfo> rowLst, ref List<extraData> lstExtraData, int what)
        {
            int errCount = 0;

            if (ksiegiKNS == null || !ksiegiKNS.Any())
            {
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    ksiegiKNS = context.KnsKsiegi.ToList();

                }
            }
            Utils.LogWriter("Przygotowanie pakietu");
            if (rowLst == null || !rowLst.Any())
                return null;
            Package message = new Package();
            message.credentials = this.setupCredentials();
            Utils.LogWriter("Przygotowano dane autoryzacyjne");
            if (message.credentials == null || !message.credentials.Any())
            {
                MessageBox.Show("Brak BIG do wysłania komunikatu ");
                return null;
            }

            message.packageSubmit = setupPackageSubmit(rowLst, ref lstExtraData, what, out errCount);
            if (errCount > 0)
                return null;
            return message;
         }


        public Package CreateUpdtRqFromDB(List<GridViewRowInfo> rowLst, ref List<extraData> lstExtraData, int what, List<BIG_Big> lstBig = null)
        {
            if (rowLst == null || !rowLst.Any())
                return null;
            Package message = new Package();
            message.credentials = this.setupCredentials(lstBig);
            if (message.credentials == null || !message.credentials.Any())
            {
                MessageBox.Show("Brak BIG do wysłania komunikatu ");
                return null;
            }
            message.packageSubmit = setupPackageSubmitFromDB(rowLst, ref lstExtraData, what);

            return message;
        }



        public Package CreateDelRq(List<string> liabilLst, ref  List<extraData> exData)
        {
            if (liabilLst == null || !liabilLst.Any())
                return null;
            Package message = new Package();
            List<Operation> oprLst = new List<Operation>();

            message.credentials = this.setupCredentials();
            if (message.credentials == null || !message.credentials.Any())
            {
                MessageBox.Show("Brak BIG do wysłania komunikatu ");
                return null;
            }

            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                foreach (string liId in liabilLst)
                {
                    BIG_InfoOperation bigOpr = context.BIG_InfoOperation.Where(a => a.LiabilityId == liId && (a.OperType == 1 || a.OperType == 2)).OrderByDescending(a => a.IdBIG_InfoOperation).FirstOrDefault();
                    if (bigOpr == null)
                        continue;

                    Operation opr = new Operation();
                    opr.deleteInformation = new InformationSelector();
                    opr.deleteInformation.debtorId = bigOpr.DebatorID;
                    opr.deleteInformation.liabilityId = liId;
                    opr.OperationId = "D" + bigOpr.OperationId.Substring(1); 
                    oprLst.Add(opr);
                     if (exData  == null )
                    exData = new List<extraData>();
                extraData exDataItem = new extraData();
                exDataItem.operId = opr.OperationId;
                exDataItem.dataname = "KartaDl";
                exDataItem.datavalue = bigOpr.KartaDl;
                exData.Add(exDataItem);

                exDataItem = new extraData();
                exDataItem.operId = opr.OperationId;
                exDataItem.dataname = "IdKsiega";
                exDataItem.datavalue = bigOpr.IdKsiega > 0 ? "0" : bigOpr.IdKsiega.ToString();
                exData.Add(exDataItem);
                }
            }

            message.packageSubmit = new PackageSubmit();
            message.packageSubmit.operation = oprLst.ToArray();
            message.packageSubmit.packageId = getSadIdWlasny() + "/" + getSysName() + "/" + getPackageId();
            
            return message;



        }


        private bool setupCilent()
        {
            BIG_Konfig bk = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;

            try
            {
                if (theClient != null) return true;
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    bk = context.BIG_Konfig.FirstOrDefault();

                }
                if (bk == null)
                {
                    MessageBox.Show("Brak konfiguracji usługi sieciowej BIG");
                    return false;
                }  
              
              
              CustomBinding cbind = new CustomBinding("Ex2BIGBinding");

                BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                basicHttpBinding.SendTimeout = new TimeSpan(0,5,0);//
                basicHttpBinding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                basicHttpBinding.OpenTimeout = new TimeSpan(0, 2, 0);
                basicHttpBinding.MaxReceivedMessageSize = 2147483647;
                basicHttpBinding.MaxBufferSize = 2147483647;
                basicHttpBinding.ReaderQuotas.MaxStringContentLength = 1048576;
              
                cbind.SendTimeout = new TimeSpan(0, 5, 0);//
                cbind.ReceiveTimeout = new TimeSpan(0, 5, 0);
                cbind.OpenTimeout = new TimeSpan(0, 2, 0);
             

                //ServiceReferenceBigMain.G2BIG_packageSubmitRequest_outClient  theClient1 = new ServiceReferenceBigMain.G2BIG_packageSubmitRequest_outClient ("HTTP_Package");
                EndpointAddress basicAuthEndpoint = new EndpointAddress(new Uri(bk.SubmitEndpoint)); //, theClient1.Endpoint.Address.Identity, theClient1.Endpoint.Address.Headers);

                theClient = new ServiceReferenceBigMain.G2BIG_packageSubmitRequest_outClient(cbind, basicAuthEndpoint);
                theClient.ClientCredentials.UserName.UserName = bk.SubmitAuthUser;
                theClient.ClientCredentials.UserName.Password = Utils.Decrypt(bk.SubmitAuthPasword,"Application error");
               
                var requestInterceptor = new InspectorBehavior();
                theClient.Endpoint.Behaviors.Add(requestInterceptor);
                return true;

            }
            catch (Exception ex)
            {
              
               Utils.SetupExceptionMessage(ex);
               return false;
            }
        
        
        }


        private bool verifyPackage(Package thePackage)
        {
            if (thePackage == null) return false;
            if (thePackage.packageSubmit == null)
                return false;
            if (thePackage.packageSubmit == null)
                return false;
            if (thePackage.packageSubmit.operation == null)
                return false;
            if (thePackage.packageSubmit.operation.Count() <= 0)
                return false;
            foreach (Operation o in thePackage.packageSubmit.operation)
            {
                if (o.addInformation != null)
                {

                    if (o.addInformation.debtor == null) return false;

                }

                if (o.updateInformation != null)
                {

                    if (o.updateInformation.debtor == null) return false;

                }
                if (o.deleteInformation != null)
                {

                    if (o.deleteInformation.debtorId == null) return false;

                }

            }
            return true;
        }

        public bool sendPackage(Package thePackage)
       {
       //ServiceReferenceBigMain.G2BIG_packageSubmitRequest_outClient  client = new ServiceReferenceBigMain.G2BIG_packageSubmitRequest_outClient();
         //  client.
           BIG_Package bp = null;
           if (!setupCilent())
           {
             exceptionMessage = "Błąd inicjalizacji połączenia z serwisem " + exceptionMessage;
             return false; 
           }
            Utils.LogWriter("Walidacja pakietu");
            if (!verifyPackage(thePackage))
            {
                MessageBox.Show("Błąd walidacji pakietu");
                return false;

            }
           try
           {

               Utils.LogWriter("Wysyłka pakietu");
               Confirmation conf = this.theClient.IG2BIG_packageSubmitRequest_out(thePackage);
               if (conf.AckStatus == "SUCCESS")
               {
                   return true;

               }
               else
               {

                   string mess = string.Empty;
                   if ( conf.AckStatus != null)
                       mess = String.Join(";", conf.AckStatus);
                  
                   MessageBox.Show("Wysyłka nie powiadła się " + conf.AckMessage + " ; " + mess);

                   return false;
               }
               return true;
           }
           catch (Exception ex)
           {

               MessageBox.Show( exceptionMessage = "Wysyłka nie powiadła się " + ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message:""));
               
               return false;
           
           }
       
       }
    }
} 
