using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using Ex2PscdInterface.Ex2PscdPartnerQueryOutService;
using Newtonsoft.Json;
using MessageSignature;
using System.ServiceModel.Channels;
using Ex2PscdInterface.Ex2PscdContractAccountCreateOutService;
using Ex2PscdInterface.Ex2PscdContractObjectCreateOutService;
using Ex2PscdInterface.Ex2PscdPartnerCreateOutService;
using Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService;
using Ex2PscdInterface.Ex2PscdRelationCreateOutService;
using Ex2PscdInterface.Ex2PscdContractAccountQueryOutService;
using Ex2PscdInterface.Ex2PscdDocumentCreateOutService;
using System.Globalization;
using System.ServiceModel.Description;
using System.IO;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;
using log4net;
using Ex2PscdInterface.Ex2PscdDocumentListQueryOutService;
using Ex2PscdInterface.Ex2PscdInstalmentPlanVerifyOutService;
using Ex2PscdInterface.Ex2PscdInstalmentPlanDeactivateOutService;
using Ex2PscdInterface.Ex2PscdInstalmentPlanCreateOutService;
using Ex2PscdInterface.Ex2PscdDocumentReductionDebtOutService;
using Ex2PscdInterface.Ex2PscdContractObjectQueryOutService;
using Ex2PscdInterface.Ex2PscdDocumentUpdateOutService;
using Ex2PscdInterface.Ex2PscdPaymentClarificationsQueryOutService;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Configuration;
using Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService;
using Ex2PscdInterface.Ex2PscdDocumentDebtStateUpdateOutService;
using Ex2PscdInterface.Ex2PscdPaymentListQueryInService;
using Ex2PscdInterface.Ex2PscdGetCaseRegistryTypesOutService;
using Ex2PscdInterface.Ex2PscdGetCourtsOutService;
using Ex2PscdInterface.Ex2PscdManageAccountOutService;
using Ex2PscdInterface.Ex2PscdGetDepartmentsOutService;


namespace SapPOHelper
{

    public class InspectorBehavior : IEndpointBehavior
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string LastRequestXML
        {
            get
            {


                return myMessageInspector.LastRequestXML;
            }
        }

        public string LastResponseXML
        {
            get
            {
                return myMessageInspector.LastResponseXML;
            }
        }





        private MyMessageInspector myMessageInspector = new MyMessageInspector();
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }


        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(myMessageInspector);
        }
    }





    public class MyMessageInspector : IClientMessageInspector
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string LastRequestXML { get; private set; }
        public string LastResponseXML { get; private set; }

        public string PackageFullId { get; set; }

        private void ChangeMessage(ref System.ServiceModel.Channels.Message message)
        {
            MemoryStream ms = new MemoryStream();
            Encoding encoding = Encoding.UTF8;
            XmlWriterSettings writerSettings = new XmlWriterSettings { Encoding = encoding };
           
        }


        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            LastResponseXML = reply.ToString();
            log.Debug(LastResponseXML);

        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            
            LastRequestXML = request.ToString();
            log.Debug(LastRequestXML);
            

            return request;
            
        }
    }


    public class ZSRKRequestHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string _errorMsg;

        private static X509Certificate2 _authCert;

        private static List<KeyValuePair<string, string>> _serviceMapping;

        private static string _basicAuthLogin;

        private static string _basicAuthPassword;

        private static string _mEPUser;

        private static string _mEPPassword;

        private static string _applicationID;

        private static string _jednostkaGospodarcza;

        private static string _stanowiskoFinansowe;

        private static Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Admin _adminData;

        private static string decrypt(string strEncrypted, string strKey)
        {
            if (string.IsNullOrWhiteSpace(strEncrypted)) return strEncrypted;
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString
                (objDESCrypto.CreateDecryptor().TransformFinalBlock
                (byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;
                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Błąd odczytu" + ex.Message;
            }
        }




        public static List<KeyValuePair<string, string>> ServiceMapping
        {
            get { return _serviceMapping; }

            set { _serviceMapping = value; }
        }

        public static X509Certificate2 AuthCert
        {
            get { return _authCert; }
            set { _authCert = value; }

        }

        public static String BasicAuthLogin
        {
            get {
                if (!string.IsNullOrWhiteSpace(_basicAuthLogin))
                    return _basicAuthLogin;
                else
                    return "EPI02";


            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _basicAuthLogin = value;
                else
                    _basicAuthLogin = "EPI02";
            }

        }

        public static String BasicAuthPassword
        {
            get { return _basicAuthPassword; }
            set {
                if (String.IsNullOrWhiteSpace(value) || String.IsNullOrWhiteSpace(BasicAuthLogin))
                    _basicAuthPassword = decrypt("nsttC/uv3mhPxqABh4vj8A==", "Application error");
                else
                    _basicAuthPassword = decrypt(value, "Application error");
            }

        }


        public static String MEPUser
        {
            get { return _mEPUser; }
            set { _mEPUser = value; }

        }

        public static String MEPPassword
        {
            get { return _mEPPassword; }
            set { _mEPPassword = value; }

        }

        public static String ApplicationID
        {
            get { return _applicationID; }
            set { _applicationID = value; }

        }

        public static String JednostkaGospodarcza
        {
            get { return _jednostkaGospodarcza; }
            set { _jednostkaGospodarcza = value; }

        }
        public static String StanowiskoFinansowe
        {
            get { return _stanowiskoFinansowe; }
            set { _stanowiskoFinansowe = value; }

        }


        private static Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Admin _admin
        {
            get
            {
            
                if (_adminData != null && !(String.IsNullOrWhiteSpace(_jednostkaGospodarcza)))
                {
                    ;

                }
                else
                {
                    _adminData = new Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Admin();
                    _adminData.Uzytkownik = _mEPUser;
                    _adminData.IDSystemMerytorycznego = (String.IsNullOrWhiteSpace(_applicationID) ? "RupInt" : _applicationID);
                    _adminData.JednostkaGospodarcza = _jednostkaGospodarcza;

                };
                _adminData.IDKomunikatu = (String.IsNullOrWhiteSpace(_applicationID) ? "RupInt" : _applicationID) + "_" + Guid.NewGuid();
                _adminData.UtworzonyData = DateTime.Today.ToString("yyyyMMdd");
                _adminData.UtworzonyGodz = DateTime.Now.ToString("HHmm");
                _adminData.Haslo = SignatureHelper.SignMessageId(_adminData.IDKomunikatu);

                return _adminData;

            }
        }

        public static ContractAccountRelationCreateOutClient ContractAccountRelationCreateOutClient { get; private set; }

        static T ConvertObject<T>(object M) where T : class
        {
            // Serialize the original object to json
            // Desarialize the json object to the new type 
            var obj = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(M));
            return obj;
        }
        // Test ObjectToCast is type Namespace1.Class, obj is Namespace2 
        //   Namespace2.Class obj = ConvertObject<Namespace2.Class>(ObjectToCast);

        private static void addErrMsg(Exception ex)
        {
            if (String.IsNullOrWhiteSpace(_errorMsg))
            {

                _errorMsg += "\n\r";

            }
            _errorMsg += ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");

        }

        private static void addErrMsg(string msg, Exception ex)
        {
            if (String.IsNullOrWhiteSpace(_errorMsg))
            {

                _errorMsg += "\n\r";

            }
            _errorMsg += msg + " " + ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");

        }

        private static void addErrMsg(string msg)
        {
            if (String.IsNullOrWhiteSpace(_errorMsg))
            {
                _errorMsg = msg;

            }
            else
            {
                _errorMsg += "\n\r" + msg;

            }

        }



        public static object CallSAPMethod(string MethodName, object Args)
        {
            string testPO = "0";

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            try
            {
                testPO = config.AppSettings.Settings["TestPO"].Value.ToString();
            }
            catch { }

            if (testPO.Trim() == "1")
                MessageBox.Show("Przed wywołaniem metody: " + MethodName, "Wywołanie metody ZSRK");
            _errorMsg = string.Empty;
            KeyValuePair<string, string>? method = _serviceMapping.Where(a => a.Key == MethodName).FirstOrDefault();
            if (method == null)
            {
                addErrMsg("Brak takiej metody " + MethodName);
                return null;
            }

            CustomBinding cbind = new CustomBinding("Ex2PSCDBinding");
            EndpointAddress endpoint = new EndpointAddress(new Uri(method.Value.Value));

            switch (MethodName)
            {

                case "ContractAccountCreateOut":
                    {
                        ContractAccountCreateOutClient theClient = new ContractAccountCreateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);


                        Ex2PscdInterface.Ex2PscdContractAccountCreateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdContractAccountCreateOutService.Admin>(_admin);
                        ((ContractAccountCreateRequest)Args).Admin = obj;


                        try
                        {
                            return theClient.ContractAccountCreateOut((ContractAccountCreateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }
                    }

                case "ContractAccountQueryOut":
                    {
                        ContractAccountQueryOutClient theClient = new ContractAccountQueryOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdContractAccountQueryOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdContractAccountQueryOutService.Admin>(_admin);
                        ((ContractAccountQueryRequest)Args).Admin = obj;


                        try
                        {
                            return theClient.ContractAccountQueryOut((ContractAccountQueryRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }
                    }


                case "ContractAccountRelationCreateOut":
                    {
                        ContractAccountRelationCreateOutClient theClient = new ContractAccountRelationCreateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);


                        Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.Admin>(_admin);
                        ((ContractAccountRelationCreateRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.ContractAccountRelationCreateOut((ContractAccountRelationCreateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                case "ContractAccountUpdateOut":
                    break;
                case "ContractObjectCreateOut":
                    {
                        ContractObjectCreateOutClient theClient = new ContractObjectCreateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);


                        Ex2PscdInterface.Ex2PscdContractObjectCreateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdContractObjectCreateOutService.Admin>(_admin);
                        ((ContractObjectCreateRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.ContractObjectCreateOut((ContractObjectCreateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }
                    }

                case "ContractObjectQueryOut":
                    {
                        ContractObjectQueryOutClient theClient = new ContractObjectQueryOutClient();
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);


                        Ex2PscdInterface.Ex2PscdContractObjectQueryOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdContractObjectQueryOutService.Admin>(_admin);
                        ((ContractObjectQueryRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.ContractObjectQueryOut((ContractObjectQueryRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }
                    }


                case "DebtorDepositListQueryOut":
                    break;
                case "DepartmentDictionaryQueryOut":
                    break;
                case "DepositListQueryOut":
                    break;
                case "DocumentBailiffListQueryOut":
                    break;
                case "DocumentCreateOut":
                    {
                        DocumentCreateOutClient theClient = new DocumentCreateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        DocumentCreateRequest dc = (DocumentCreateRequest)Args;
                        if (dc.PozycjaDokumentPH.RachunekBankowyOdbiorcaPlatnosci == null)
                        {
                            dc.PozycjaDokumentPH.RachunekBankowyOdbiorcaPlatnosci = new Ex2PscdInterface.Ex2PscdDocumentCreateOutService.RachunekBankowy();
                            dc.PozycjaDokumentPH.RachunekBankowyOdbiorcaPlatnosci.Kraj = "";
                            dc.PozycjaDokumentPH.RachunekBankowyOdbiorcaPlatnosci.NumerBanku = "";
                            dc.PozycjaDokumentPH.RachunekBankowyOdbiorcaPlatnosci.KontoBankowe = "";
                            dc.PozycjaDokumentPH.RachunekBankowyOdbiorcaPlatnosci.KodKontrolny = "";
                        }

                        Ex2PscdInterface.Ex2PscdDocumentCreateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdDocumentCreateOutService.Admin>(_admin);
                        ((DocumentCreateRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.DocumentCreateOut((DocumentCreateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                case "DocumentDebtStateUpdateOut":
                    {
                        DocumentDebtStateUpdateOutClient theClient = new DocumentDebtStateUpdateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdDocumentDebtStateUpdateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdDocumentDebtStateUpdateOutService.Admin>(_admin);
                        ((DocumentDebtStateUpdateRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.DocumentDebtStateUpdateOut((DocumentDebtStateUpdateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }

                    break;
                case "DocumentListQueryOut":
                    {
                        DocumentListQueryOutClient theClient = new DocumentListQueryOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdDocumentListQueryOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdDocumentListQueryOutService.Admin>(_admin);
                        ((DocumentListQueryRequest)Args).Admin = obj;

                        try
                        {
                            DocumentListQueryRequest thedoc = (DocumentListQueryRequest)Args;
                            return theClient.DocumentListQueryOut(thedoc);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }

                case "DocumentReductionDebtOut":
                    {
                        DocumentReductionDebtOutClient theClient = new DocumentReductionDebtOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdDocumentReductionDebtOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdDocumentReductionDebtOutService.Admin>(_admin);
                        ((DocumentReductionDebtRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.DocumentReductionDebtOut((DocumentReductionDebtRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }


                case "DocumentReferenceUpdateOut":
                    break;
                case "DocumentReverseCreateOut":
                    break;
                case "DocumentUpdateOut":
                    {
                        DocumentUpdateOutClient theClient = new DocumentUpdateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdDocumentUpdateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdDocumentUpdateOutService.Admin>(_admin);
                        ((DocumentUpdateRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.DocumentUpdateOut((DocumentUpdateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                case "GetCaseRegistryTypesOut":
                    {
                        GetCaseRegistryTypesOutClient theClient = new GetCaseRegistryTypesOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                       // Ex2PscdInterface.Ex2PscdGetCaseRegistryTypesOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdDocumentUpdateOutService.Admin>(_admin);
                       // ((DocumentUpdateRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.GetCaseRegistryTypesOut((GetCaseRegistryTypesRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                case "GetCourtsOut":
                    {
                        GetCourtsOutClient theClient = new GetCourtsOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        // Ex2PscdInterface.Ex2PscdGetCaseRegistryTypesOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdDocumentUpdateOutService.Admin>(_admin);
                        // ((DocumentUpdateRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.GetCourtsOut((GetCourtsRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                case "GetDepartmentsOut":
                    {
                        GetDepartmentsOutClient theClient = new GetDepartmentsOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        // Ex2PscdInterface.Ex2PscdGetCaseRegistryTypesOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdDocumentUpdateOutService.Admin>(_admin);
                        // ((DocumentUpdateRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.GetDepartmentsOut((GetDepartmentsRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                case "InstalmentPlanCreateOut":
                    {
                        InstalmentPlanCreateOutClient theClient = new InstalmentPlanCreateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);


                        Ex2PscdInterface.Ex2PscdInstalmentPlanCreateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdInstalmentPlanCreateOutService.Admin>(_admin);
                        ((InstalmentPlanCreateRequest)Args).Admin = obj;
                        try
                        {
                            return theClient.InstalmentPlanCreateOut((InstalmentPlanCreateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                    
                case "InstalmentPlanDeactivateOut":
                    {
                        InstalmentPlanDeactivateOutClient theClient = new InstalmentPlanDeactivateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdInstalmentPlanDeactivateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdInstalmentPlanDeactivateOutService.Admin>(_admin);
                        ((InstalmentPlanDeactivateRequest)Args).Admin = obj;
                        try
                        {
                            return theClient.InstalmentPlanDeactivateOut((InstalmentPlanDeactivateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }

                case "ManageAccountOut": 
                    { 
                        ManageAccountOutClient theClient = new ManageAccountOutClient(cbind, endpoint);
                        
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdManageAccountOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdManageAccountOutService.Admin>(_admin);
                        ((ManageAccountRequest)Args).Admin = obj;
                        try
                        {

                            ((ManageAccountRequest)Args).NoweHaslo =  SignatureHelper.SignMessageId(obj.IDKomunikatu, ((ManageAccountRequest)Args).NoweHaslo);
                            return theClient.ManageAccountOut((ManageAccountRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                 
                case "InstalmentPlanVerifyOut":
                    {
                        InstalmentPlanVerifyOutClient theClient = new InstalmentPlanVerifyOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdInstalmentPlanVerifyOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdInstalmentPlanVerifyOutService.Admin>(_admin);
                        ((InstalmentPlanVerifyRequest)Args).Admin = obj;
                        try
                        {
                            return theClient.InstalmentPlanVerifyOut((InstalmentPlanVerifyRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }

                case "PartnerCreateOut":
                    {
                        PartnerCreateOutClient theClient = new PartnerCreateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Admin>(_admin);
                        ((PartnerCreateRequest)Args).Admin = obj;
                        ((PartnerCreateRequest)Args).RodzajOp = "1"; // create;
                        try
                        {
                            return theClient.PartnerCreateOut((PartnerCreateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }

                    }
                case "PartnerQueryOut":
                    {
                        PartnerQueryOutClient theClient = new PartnerQueryOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Admin>(_admin);
                        ((PartnerQueryRequest)Args).Admin = obj;

                        try
                        {
                            return theClient.PartnerQueryOut((PartnerQueryRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }
                    }

                case "PartnerUpdateOut":
                    break;
                case "PaymentCancellationCreateOut":
                    break;
                case "PaymentClarificationCreateOut":
                    break;
                case "PaymentClarificationZDOBCreateOut":
                    break;
                case "PaymentClarificationsQueryOut":
                    {
                        PaymentClarificationsQueryOutClient theClient = new PaymentClarificationsQueryOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;


                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdPaymentClarificationsQueryOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdPaymentClarificationsQueryOutService.Admin>(_admin);
                        PaymentClarificationsQueryRequest argument = (PaymentClarificationsQueryRequest)Args;
                        if (argument.DataKsStornaOd == null)
                            argument.DataKsStornaOd = "";
                        if (argument.DataKsStornaDo == null)
                            argument.DataKsStornaDo = "";

                        argument.Admin = obj;


                        try
                        {
                            return theClient.PaymentClarificationsQueryOut(argument);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }
                    }
/*
                case "PaymentListQueryOut":
                    {
                        PaymentListQueryOutClient theClient = new PaymentListQueryOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdPaymentListQueryOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdPaymentListQueryOutService.Admin>(_admin);
                        ((PaymentListQueryRequest)Args).Admin = obj;
                        try
                        {
                            return theClient.PaymentListQueryOut((PaymentListQueryRequest)Args);
                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }


                    }
*/
                case "PaymentListQueryIn":
                    {
                        PaymentListQueryOutClient  theClient = new PaymentListQueryOutClient (cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdPaymentListQueryInService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdPaymentListQueryInService.Admin>(_admin);
                        ((PaymentListQueryRequest)Args).Admin = obj;
                        try
                        {
                            return theClient.PaymentListQueryOut((PaymentListQueryRequest)Args);
                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }


                    }

                case "PaymentReservationCreateOut":
                    break;
                case "PostingDataPrepareOut":
                    {
                        PostingDataPrepareOutClient theClient = new PostingDataPrepareOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdPostingDataPrepareOutService.Admin>(_admin);
                        ((PostingDataPrepareRequest)Args).Admin = obj;
                        try
                        {
                            return theClient.PostingDataPrepareOut((PostingDataPrepareRequest)Args);
                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }


                    }

                case "PostingStatusQueryOut":
                    break;



                case "RelationCreateOut":
                    {
                        RelationCreateOutClient theClient = new RelationCreateOutClient(cbind, endpoint);
                        theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
                        theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
                        theClient.ClientCredentials.UserName.Password = _basicAuthPassword;

                        var requestInterceptor = new InspectorBehavior();
                        theClient.Endpoint.Behaviors.Add(requestInterceptor);

                        Ex2PscdInterface.Ex2PscdRelationCreateOutService.Admin obj = ConvertObject<Ex2PscdInterface.Ex2PscdRelationCreateOutService.Admin>(_admin);
                        ((RelationCreateRequest)Args).Admin = obj;


                        try
                        {
                            return theClient.RelationCreateOut((RelationCreateRequest)Args);

                        }
                        catch (Exception ex)
                        {
                            addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                            throw ex;

                        }
                    }


                default:
                    {
                        addErrMsg("Brak metody " + MethodName);
                        return null;
                    }


            }
            return null;

        }

        public static string GetErrorMessage()
        {
            return _errorMsg;

        }
        #region publiczne metody merytoryczne
        public static PostingDataPrepareResponse WyslijDoPoczekalni(PostingDataPrepareRequest requst)
        {
            PostingDataPrepareResponse answer = null;
            answer = (PostingDataPrepareResponse)CallSAPMethod("PostingDataPrepareOut", requst);



            return answer;
        }

        public static PaymentListQueryResponse PobierzWplatyNierozpoznane(string RodzajRachunkuBankowego, string StatusRozliczenia, string TypPozycji, string DataOd, string DataDo, string JG) 
        {
            PaymentListQueryRequest rq = new PaymentListQueryRequest();
            rq. KryteriumWyboruPozWB = new KryteriumWyboruPozWB();
            rq.KryteriumWyboruPozWB.DataKsiegowaniaOd = DataOd;
            rq.KryteriumWyboruPozWB.DataKsiegowaniaDo = DataDo;
            rq.RodzajRachunkuBankowego = RodzajRachunkuBankowego;
            rq.StatusRozliczenia = StatusRozliczenia;
            rq.KryteriumWyboruPozWB.JednostkaGospodarcza = JG;
            rq.TypPozycji = TypPozycji;

            PaymentListQueryResponse answer = null;
            answer = (PaymentListQueryResponse)CallSAPMethod("PaymentListQueryIn", rq);


            return answer;

        }
        public static ContractObjectCreateResponse  ZalozSygnature(SygnaturaTworzenie myquery)
        {
            ContractObjectCreateRequest addsygn = new ContractObjectCreateRequest();
            string JGWindyk = null;
            string stanfinWindyk = null;
            ContractObjectCreateResponse answer;

            try
            {
                addsygn.Sygnatura  = myquery;
                if (!String.IsNullOrWhiteSpace(myquery.DaneDoWindykacjiJednostkaGospodarcza))
                {
                    if (myquery.DaneDoWindykacjiJednostkaGospodarcza.Substring(0, 1) != "3" && !String.IsNullOrWhiteSpace(myquery.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe))
                    {
                        addErrMsg("Niepoprawne stanowisko finansowe sądu windykacyjnego: " + myquery.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe);
                        return null;
                    }
                    JGWindyk = myquery.DaneDoWindykacjiJednostkaGospodarcza;
                    stanfinWindyk = myquery.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe;

                    addsygn.Sygnatura.DaneDoWindykacjiJednostkaGospodarcza = null;
                    addsygn.Sygnatura.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe = null;
                }
                // Obsługa sądów windykacyjnych 
                if (addsygn.Sygnatura.JednostkaGospodarcza.Substring(0, 1) != "3" && !String.IsNullOrWhiteSpace(addsygn.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe))
                {
                    addErrMsg("Niepoprawne stanowisko finansowe sygnatury " + addsygn.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe);
                    return null;
                }
                answer =  (ContractObjectCreateResponse)CallSAPMethod("ContractObjectCreateOut",addsygn);
                if (String.IsNullOrWhiteSpace(JGWindyk)) return answer;
                if (answer != null && answer.Sygnatura != null && !String.IsNullOrEmpty(answer.Sygnatura.IDPrzedmiotuUmowy))
                {

                    addsygn.Sygnatura.DaneDoWindykacjiJednostkaGospodarcza = JGWindyk;
                    addsygn.Sygnatura.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe = stanfinWindyk;
                    if ((addsygn.Sygnatura.DaneDoWindykacjiJednostkaGospodarcza.Substring(0, 1) != "3" && !String.IsNullOrWhiteSpace(addsygn.Sygnatura.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe)))
                    {
                        addErrMsg("Niepoprawne stanowisko finansowe sądu windykacyjnego: " + addsygn.Sygnatura.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe);
                        return null;
                    }

                    return (ContractObjectCreateResponse)CallSAPMethod("ContractObjectCreateOut", addsygn);

                }
                else
                {

                    return answer;

                }
                    
                    


            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas zkładania sygnatury " + ex);

                return null;
            }

        }

        public static ContractObjectQueryResponse ZnajdzSygnature(ContractObjectQueryRequest myquery)
        {
          
            try
            {
                return (ContractObjectQueryResponse)CallSAPMethod("ContractObjectQueryOut", myquery);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas wyszukiwania sygnatury " + ex);
                return null;
            }

        }

        public static PartnerCreateResponse  DodajPartnera(Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner myquery)
        {
            PartnerCreateRequest addpartner = new PartnerCreateRequest(); 

            try
            {
                addpartner.Partner = myquery;
                return (PartnerCreateResponse)CallSAPMethod("PartnerCreateOut",addpartner);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas zakładaina partnera ", ex);
                return null;
            }

        }
        
        public static ContractAccountCreateResponse DodajKontoUmowy(KontoUmowyTworzenie mykdl)
        {
            ContractAccountCreateRequest addkdl = new ContractAccountCreateRequest();

            try
            {
                addkdl.KontoUmowy = mykdl;
                addkdl.AutomatyczneTworzenie = "1";
                if ((addkdl.KontoUmowy.JednostkaGospodarcza.Substring(0, 1) != "3" || addkdl.KontoUmowy.StandardowaJednostkaGospodarcza.Substring(0, 1) != "3") && !String.IsNullOrWhiteSpace(addkdl.KontoUmowy.SadFunkcjonalnyStanowiskoFinansowe))
                {
                    addErrMsg("Błąd stanowiska finansowego konta umowy: " + addkdl.KontoUmowy.SadFunkcjonalnyStanowiskoFinansowe);
                    return null;
                }
#if DEBUG
                if ( String.IsNullOrWhiteSpace(addkdl.KontoUmowy.SadFunkcjonalnyStanowiskoFinansowe) )
                {
                    addkdl.KontoUmowy.SadFunkcjonalnyStanowiskoFinansowe = addkdl.KontoUmowy.JednostkaGospodarcza;

                }
#endif
                    return (ContractAccountCreateResponse)CallSAPMethod("ContractAccountCreateOut", addkdl);
            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas zakłądania konta umowy", ex);
                return null;
            }

        }

        public static RelationCreateResponse AktualizujKontoUmowy(string IdPrzedmiotuUmowy, string NumerKontaUmowy)
        {
            RelationCreateRequest addkdl = new RelationCreateRequest();

            try
            {
                addkdl.Relacja = new Relacja();
                addkdl.Relacja.IDPrzedmiotuUmowy = IdPrzedmiotuUmowy;
                addkdl.Relacja.NumerKontaUmowy = NumerKontaUmowy;
                return (RelationCreateResponse)CallSAPMethod("RelationCreateOut",addkdl);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd ustanawiania relacji ", ex);
                return null;
            }

        }


        public static ContractAccountRelationCreateResponse AktualizujKontoUmowy(Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy konto, string IdPrzedmiotUmowy)
        {
            ContractAccountRelationCreateRequest addkdl = new ContractAccountRelationCreateRequest();
            addkdl.DaneKontoUmowy = konto;
            addkdl.IDSygnatura = IdPrzedmiotUmowy;

            try
            {
                return (ContractAccountRelationCreateResponse)CallSAPMethod("ContractAccountRelationCreateOut", addkdl);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd ustanawiania relacji ", ex);
                return null;
            }

        }


        public static RelationCreateResponse UtworzRelacje( string NumerKontaUmowy, string IdPrzedmiotUmowy)
        {
            RelationCreateRequest addkdl = new RelationCreateRequest();

            addkdl.Relacja = new Relacja();
            addkdl.Relacja.NumerKontaUmowy = NumerKontaUmowy;
            addkdl.Relacja.IDPrzedmiotuUmowy = IdPrzedmiotUmowy;
            
            try
            {
                return (RelationCreateResponse)CallSAPMethod("RelationCreateOut", addkdl);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd ustanawiania relacji metodą RelationCreate ", ex);
                return null;
            }

        }


        public static ContractAccountQueryResponse WyszukajKontoUmowy(KontoUmowyDefinicja mykdl)
        {
            ContractAccountQueryRequest getkdl = new ContractAccountQueryRequest();

            try
            {
               
                getkdl.KontoUmowy = mykdl;
                return (ContractAccountQueryResponse)CallSAPMethod("ContractAccountQueryOut", getkdl);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd wyszukiwania konta umowy ", ex);
                return null;
            }

        }

        public static PartnerQueryResponse WyszukajPartnera(PartnerQuery partnerId)
        {
            PartnerQueryRequest getPartner = new PartnerQueryRequest();

            try
            {
                getPartner.Partner = partnerId;
                return (PartnerQueryResponse)CallSAPMethod("PartnerQueryOut",getPartner);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd wyszukiwania partnera ", ex );
                return null;
            }

        }

        public static DocumentCreateResponse  DodajPrzypis(DocumentCreateRequest doc)
        {
            

            try
            {
                return (DocumentCreateResponse)CallSAPMethod("DocumentCreateOut", doc);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas tworzenia dokumentu", ex);
                return null;
            }

        }


        public static   InstalmentPlanVerifyResponse SprawdzPlanRat(String dokIn, String kontoUmowy)
        {
            InstalmentPlanVerifyRequest sprPlan = new InstalmentPlanVerifyRequest();

            try
            {

                sprPlan.WeryfikacjaPlanuRat = new WeryfikacjaPlanuRat();
                sprPlan.WeryfikacjaPlanuRat.NumerDokumentuRozrachunkow = dokIn;
                sprPlan.WeryfikacjaPlanuRat.NumerKontaUmowy = kontoUmowy;
                return (InstalmentPlanVerifyResponse)CallSAPMethod("InstalmentPlanVerifyOut", sprPlan);
            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas weryfikacji planu rat", ex);
                return null;
            }

        }


        public static InstalmentPlanDeactivateResponse DzeaktywujPlanRat(String planRat)
        {
            InstalmentPlanDeactivateRequest sprPlan = new InstalmentPlanDeactivateRequest();

            try
            {

                sprPlan.NumerPlanuRat = planRat; 
                return (InstalmentPlanDeactivateResponse)CallSAPMethod("InstalmentPlanDeactivateOut", sprPlan);
            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas deaktywacji planu rat", ex);
                return null;
            }

        }


        public static DocumentListQueryResponse PobierzRozrachunki(DocumentListQueryRequest doc)
        {


            try
            {
                return (DocumentListQueryResponse)CallSAPMethod("DocumentListQueryOut", doc);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas pobierania rozrachunków dla dokumentu", ex);
                return null;
            }

        }

        public static DocumentListQueryResponse PobierzRozrachunki(string doc, string jednGosp )
        {
            DocumentListQueryRequest dr;
            dr = new DocumentListQueryRequest();
            dr.PozDoWyj = new PozDoWyj();
            dr.PozDoWyj.IdPozycjaWyj = "";
            dr.PozDoWyj.PartiaPlatnosciID = "";
            dr.PozDoWyj.PartiaPlatnosciNrPozycja = "";
            dr.IdDanePSCD = new IdDanePSCDZapytanie();
            dr.IdDanePSCD.JednostkaGospodarcza = jednGosp;    
            dr.IdDanePSCD.IDDokument = doc;


            try
            {
                return (DocumentListQueryResponse)CallSAPMethod("DocumentListQueryOut", dr);

            }
            catch (Exception ex)
            {

                addErrMsg("Błąd podczas pobierania rozrachunków dla dokumentu", ex);
                return null;
            }

        }


        public static DocumentReductionDebtResponse OdpiszNaleznosc(string nrDok, OdpisanieNaleznosciElement dokOdpis)
        {
            DocumentReductionDebtRequest adddok = new DocumentReductionDebtRequest();
            adddok.OdpisanieNaleznosci = new OdpisanieNaleznosci();
            adddok.OdpisanieNaleznosci.NumerDokumentuDoOdpisania = nrDok;
            adddok.OdpisanieNaleznosci.OdpisanieNaleznosci1 = dokOdpis;
            try
            {
                return (DocumentReductionDebtResponse)CallSAPMethod("DocumentReductionDebtOut", adddok);
            }
            catch (Exception ex)
            {


                addErrMsg("Błąd podczas zapisu dokumentu odpisu ", ex);
                return null;
            }

        }

    
        public static DocumentUpdateResponse ZmienTerminWymagalnosci(String docId, string newDate)
    {
            DocumentUpdateRequest dokWymag = new DocumentUpdateRequest();
            dokWymag.ZmianaTerminu = new ZmianaTerminu();
            dokWymag.ZmianaTerminu.DataWymagalnosci = newDate;
            dokWymag.ZmianaTerminu.NumerDokumentu = docId;

        try
        {

            return (DocumentUpdateResponse)CallSAPMethod("DocumentUpdateOut", dokWymag);
         
        }

        catch (Exception ex)
        {


            addErrMsg("Błąd podczas aktualizacji terminu wymagalności " , ex);
            return null;
        }

    }

        public static DocumentDebtStateUpdateResponse ZmienStanNaleznosci(String docId, string newDate, string stanNal)
        {
            DocumentDebtStateUpdateRequest dokStan = new DocumentDebtStateUpdateRequest();

            dokStan.ZmianaStatusuNaleznosci = new ZmianaStatusuNaleznosci();
            dokStan.ZmianaStatusuNaleznosci.NumerDokumentuRozrachunkow = docId;
            dokStan.ZmianaStatusuNaleznosci.PoczatekStanuNaleznosci = newDate;
            dokStan.ZmianaStatusuNaleznosci.StanNaleznosci = stanNal;
            dokStan.ZmianaStatusuNaleznosci.Uzytkownik = _mEPUser;

            try
            {

                return (DocumentDebtStateUpdateResponse)CallSAPMethod("DocumentDebtStateUpdateOut", dokStan);

            }

            catch (Exception ex)
            {


                addErrMsg("Błąd podczas aktualizacji stanu należności ", ex);
                return null;
            }

        }

        public static PaymentClarificationsQueryResponse PokazWplatyZaksiegowane(DateTime dOd, DateTime dDo)
        {
            PaymentClarificationsQueryRequest doc = new PaymentClarificationsQueryRequest();
            doc.DataKsiegowaniaOd = dOd.ToString("yyyyMMdd");
            doc.DataKsiegowaniaDo = dDo.ToString("yyyyMMdd");


            try
            {

                return (PaymentClarificationsQueryResponse)CallSAPMethod("PaymentClarificationsQueryOut", doc);

            }

            catch (Exception ex)
            {


                addErrMsg("Błąd podczas zapisu dokumentu odpisu ", ex);
                return null;
            }



        }

        #endregion
        #region słowniki
        public static CaseRegistryTypeData[] ImportujRepertoria()
        {
            GetCaseRegistryTypesRequest rep = new GetCaseRegistryTypesRequest();
           
            rep.RodzajSprawySpecified = false;

            try
            {

                return (CaseRegistryTypeData[])CallSAPMethod("GetCaseRegistryTypesOut", rep);

            }

            catch (Exception ex)
            {


                addErrMsg("Błąd podczas odczytu listy repertoriów", ex);
                return null;
            }



        }

        public static CourtData[] ImportujSady()
        {
            GetCourtsRequest rep = new GetCourtsRequest();
         
            try
            {

                return (CourtData [])CallSAPMethod("GetCourtsOut", rep);

            }

            catch (Exception ex)
            {


                addErrMsg("Błąd podczas odczytu listy sądów", ex);
                return null;
            }



        }

        public static DepartmentData[] ImportujWydzialy()
        {
            
            GetDepartmentsRequest dep = new GetDepartmentsRequest();


            try
            {

                return (DepartmentData[])CallSAPMethod("GetDepartmentsOut", dep);

            }

            catch (Exception ex)
            {


                addErrMsg("Błąd podczas odczytu listy wydziałów", ex);
                return null;
            }



        }
        #endregion


    }
}
