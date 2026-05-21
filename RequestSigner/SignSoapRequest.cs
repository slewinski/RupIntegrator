using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString SignSoapRequest(SqlString MessageID, SqlString Password, SqlBinary Certificate)
    { 
            if (Certificate == null)
            {
                 return "********B³¹d certyfikatu. Brak certyfikatu lub niepoprawny format********";
            }
            if (MessageID.IsNull || String.IsNullOrWhiteSpace(MessageID.ToString()))
            {
            return "********Komunikat wejœciowy jest pusty********";
            
            }
            if (string.IsNullOrWhiteSpace(Password.ToString()))
            {
            return "********Has³o jest puste********";
            
            }
            string dataToEncrypt = Convert.ToBase64String(Encoding.UTF8.GetBytes(Password.ToString())) + "|" + MessageID.ToString();

            CmsRecipient recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, new X509Certificate2((byte[])Certificate));
            EnvelopedCms envelopedCms = new EnvelopedCms(new ContentInfo(Encoding.UTF8.GetBytes(dataToEncrypt)), new AlgorithmIdentifier(new Oid("2.16.840.1.101.3.4.1.2")));
            envelopedCms.Encrypt(recipient);
            return new SqlString (Convert.ToBase64String(envelopedCms.Encode()));

    }
}
