using ConsImport;
using MessageSignature;
using RupDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Rup2ConsService
{
    public static class SapConsClientConfiguration
    {
        public static void Initialize(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Brak nazwy użytkownika konfiguracji SAP.", "userName");

            using (var context = new RupDBEntities())
            {
                User user = context.User.FirstOrDefault(x => x.Username == userName);
                if (user == null)
                    throw new InvalidOperationException("Nie znaleziono użytkownika: " + userName + ".");

                Konfiguracja configuration = context.Konfiguracja.FirstOrDefault();
                if (configuration == null)
                    throw new InvalidOperationException("Brak rekordu Konfiguracja.");

                List<KeyValuePair<string, string>> serviceMapping =
                    context.ServiceEndpoint
                        .ToList()
                        .Select(x => new KeyValuePair<string, string>(x.ServiceName, x.Endpoint))
                        .ToList();

                if (!serviceMapping.Any(x => x.Key == "ImportContentSystemData"))
                {
                    throw new InvalidOperationException(
                        "Brak endpointu ImportContentSystemData w tabeli ServiceEndpoint.");
                }

                ConsWebServiceHelper.ServiceMapping = serviceMapping;
                ConsWebServiceHelper.AuthCert = new X509Certificate2(
                    configuration.Pfx,
                    Utils.Decrypt(configuration.PfxPassword, "Application error"));

                ConsWebServiceHelper.BasicAuthLogin = configuration.WSLogon;
                ConsWebServiceHelper.BasicAuthPassword = configuration.WSpwd;
                ConsWebServiceHelper.MEPUser = user.MEPUser;
                ConsWebServiceHelper.MEPPassword =
                    Utils.Decrypt(user.MEPPassword, "Application error");
                ConsWebServiceHelper.ApplicationID = configuration.AppName;
                ConsWebServiceHelper.JednostkaGospodarcza =
                    configuration.JednostkaGospodarcza;

                SignatureHelper.Password =
                    Utils.Decrypt(user.MEPPassword, "Application error");
                SignatureHelper.SetCert(configuration.Cer);
            }
        }
    }
}
