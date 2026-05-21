using ConsInterfeces.Rup2ConsGetStatusContentSystemData;
using MessageSignature;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
using ConsInterfeces;
using ConsInterfeces.Rup2ConsImportContentSystemData;
using ConsInterfeces.Rup2ConsGetStatusContentSystemData;

using System.Xml.Serialization;

namespace ConsImport
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

    public class ConsWebServiceHelper
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

        private static ConsInterfeces.Rup2ConsImportContentSystemData.Admin _adminImportData;
        private static ConsInterfeces.Rup2ConsGetStatusContentSystemData.Admin _adminGetStatusData;

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
            get
            {
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
            set
            {
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


        private static ConsInterfeces.Rup2ConsImportContentSystemData.Admin _admin
        {
            get
            {

                if (_adminImportData != null && !(String.IsNullOrWhiteSpace(_jednostkaGospodarcza)))
                {
                    ;

                }
                else
                {
                    _adminImportData = new ConsInterfeces.Rup2ConsImportContentSystemData.Admin();
                    _adminImportData.Uzytkownik = _mEPUser;
                    _adminImportData.IDSystemMerytorycznego = (String.IsNullOrWhiteSpace(_applicationID) ? "RupInt" : _applicationID);
                    _adminImportData.JednostkaGospodarcza = _jednostkaGospodarcza;

                }
                ;
                _adminImportData.IDKomunikatu = (String.IsNullOrWhiteSpace(_applicationID) ? "RupInt" : _applicationID) + "_" + Guid.NewGuid();
                _adminImportData.UtworzonyData = DateTime.Today.ToString("yyyyMMdd");
                _adminImportData.UtworzonyGodz = DateTime.Now.ToString("HHmm");
                _adminImportData.Haslo = SignatureHelper.SignMessageId(_adminImportData.IDKomunikatu);

                return _adminImportData;

            }
        }


        private static ConsInterfeces.Rup2ConsGetStatusContentSystemData.Admin _adminGetStatus
        {
            get
            {

                if (_adminGetStatusData != null && !(String.IsNullOrWhiteSpace(_jednostkaGospodarcza)))
                {
                    ;

                }
                else
                {
                    _adminGetStatusData = new ConsInterfeces.Rup2ConsGetStatusContentSystemData.Admin();
                    _adminGetStatusData.Uzytkownik = _mEPUser;
                    _adminGetStatusData.IDSystemMerytorycznego = (String.IsNullOrWhiteSpace(_applicationID) ? "RupInt" : _applicationID);
                    _adminGetStatusData.JednostkaGospodarcza = _jednostkaGospodarcza;

                }
                ;
                _adminGetStatusData.IDKomunikatu = (String.IsNullOrWhiteSpace(_applicationID) ? "RupInt" : _applicationID) + "_" + Guid.NewGuid();
                _adminGetStatusData.UtworzonyData = DateTime.Today.ToString("yyyyMMdd");
                _adminGetStatusData.UtworzonyGodz = DateTime.Now.ToString("HHmm");
                _adminGetStatusData.Haslo = SignatureHelper.SignMessageId(_adminGetStatusData.IDKomunikatu);

                return _adminGetStatusData;

            }
        }

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



        public static ImportContentSystemDataResponse ImportData(string MethodName, ConsInterfeces.Rup2ConsImportContentSystemData.ImportContentSystemDataRequest Args, out string request)
        {
            string testPO = "0";

            //Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            //try
            //{
            //    testPO = config.AppSettings.Settings["TestPO"].Value.ToString();
            //}
            //catch { }

            _errorMsg = string.Empty;
            KeyValuePair<string, string>? method = _serviceMapping.Where(a => a.Key == MethodName).FirstOrDefault();
            if (method == null)
            {
                addErrMsg("Brak takiej metody " + MethodName);
                request = string.Empty;
                return null;
            }

            CustomBinding cbind = new CustomBinding("Ex2PSCDBinding");
            EndpointAddress endpoint = new EndpointAddress(new Uri(method.Value.Value));
            ConsInterfeces.Rup2ConsImportContentSystemData.ImportContentSystemDataOutClient theClient = new ConsInterfeces.Rup2ConsImportContentSystemData.ImportContentSystemDataOutClient(cbind,endpoint);

            theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
            theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
            theClient.ClientCredentials.UserName.Password = _basicAuthPassword;
            var requestInterceptor = new InspectorBehavior();
            theClient.Endpoint.Behaviors.Add(requestInterceptor);
            Args.Admin = _admin;
            try
            {
                var serializer = new XmlSerializer(typeof(ImportContentSystemDataRequest));
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, Args);
                    request = writer.ToString();
                }
                string dump = ImportContentSystemDataRequestDumper.DumpImportContentSystemDataRequest(Args);
                log.Debug(dump);
                return theClient.ImportContentSystemDataOut(Args);
            }
            catch (Exception ex)
            {
                addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                throw ex;

            }
        }

        public static GetStatusContentSystemDataResponse GetDataStatus(string MethodName, ConsInterfeces.Rup2ConsGetStatusContentSystemData.GetStatusContentSystemDataRequest Args, out string request)
        {
            _errorMsg = string.Empty;
            KeyValuePair<string, string>? method = _serviceMapping.Where(a => a.Key == MethodName).FirstOrDefault();
            if (method == null)
            {
                addErrMsg("Brak takiej metody " + MethodName);
                request = string.Empty;
                return null;
            }

            CustomBinding cbind = new CustomBinding("Ex2PSCDBinding");
            EndpointAddress endpoint = new EndpointAddress(new Uri(method.Value.Value));
            ConsInterfeces.Rup2ConsGetStatusContentSystemData.GetStatusContentSystemDataOutClient theClient = new ConsInterfeces.Rup2ConsGetStatusContentSystemData.GetStatusContentSystemDataOutClient(cbind, endpoint);

            theClient.ClientCredentials.ClientCertificate.Certificate = _authCert;
            theClient.ClientCredentials.UserName.UserName = _basicAuthLogin;
            theClient.ClientCredentials.UserName.Password = _basicAuthPassword;
            var requestInterceptor = new InspectorBehavior();
            theClient.Endpoint.Behaviors.Add(requestInterceptor);
            Args.Admin = _adminGetStatus;
            try
            {
                var serializer = new XmlSerializer(typeof(ConsInterfeces.Rup2ConsGetStatusContentSystemData.GetStatusContentSystemDataRequest));
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, Args);
                    request = writer.ToString();
                }
                return theClient.GetStatusContentSystemDataOut(Args);
            }
            catch (Exception ex)
            {
                request = string.Empty;
                addErrMsg("Błąd wywołania metody " + MethodName + " " + ex.Message);
                throw ex;
            }
        }
    }


}
    
