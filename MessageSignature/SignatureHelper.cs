using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessageSignature
{
    public static class SignatureHelper
    {
        
        private static X509Certificate2 _signCert = null;
        
        private static string errorMessage = string.Empty;

        private static string _password = string.Empty;

        public static void SetCert( byte[] cert)
        {

            if (cert is null)
                errorMessage += "Certyfikat jest pusty ";
            else

            try
            {
                    _signCert = new X509Certificate2(cert);
            }
            catch (Exception ex)
            {
                    errorMessage += "Błąd odczytu certyfikatu " + ex.Message;
                    _signCert = null;
            }

        }

        public static string Password
        {

            set {
                _password = value;
                }
            

        }

        private static bool getCertFromFile(string certFileName, out X509Certificate2 certificate)
        {
            certificate = (X509Certificate2)null;
            try
            {
                certificate = new X509Certificate2(certFileName);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage += "Błąd odczytu certyfikatu " +ex.Message;
                certificate = (X509Certificate2)null;
                return false;
            }
            
        }
       


        public static string SignMessageId(string MessageID, string pwd = null)
        {

            if (_signCert == null)
            {
                errorMessage += " \n\rBłąd certyfikatu. Brak certyfikatu lub niepoprawny format";
                return null;
            }
            if (string.IsNullOrWhiteSpace(MessageID))
            {
                errorMessage += " \n\rKomunikat wejściowy jest pusty";
                return null;
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                if (string.IsNullOrWhiteSpace(_password))
                {
                    errorMessage += " \n\rHasło jest puste";
                    return null;
                }
            }
            string dataToEncrypt = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.IsNullOrWhiteSpace(pwd) ? _password : pwd))+ "|" + MessageID  ;

            CmsRecipient recipient = new CmsRecipient( SubjectIdentifierType.IssuerAndSerialNumber, _signCert);
            EnvelopedCms envelopedCms = new EnvelopedCms(new ContentInfo(Encoding.UTF8.GetBytes(dataToEncrypt)), new AlgorithmIdentifier(new Oid("2.16.840.1.101.3.4.1.2")));
            envelopedCms.Encrypt(recipient);
            return Convert.ToBase64String(envelopedCms.Encode());

        }

        public static string GetErrorMessage()
        {
            return errorMessage;
        
        }
    }
}
