using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Telerik.WinControls.UI;
using System.IO;

namespace RupBig
{
    public static class UserInfo
    {
        public static int Id { get; set; }
        public static string Username { get; set; }
        public static int role { get; set; }
        public static string MEPUser { get; set; }
        public static bool logMode {get;set;}

    }

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string Right(this string sValue, int iMaxLength)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(sValue))
            {
                //Set valid empty string as string could be null
                sValue = string.Empty;
            }
            else if (sValue.Length > iMaxLength)
            {
                //Make the string no longer than the max length
                sValue = sValue.Substring(sValue.Length - iMaxLength, iMaxLength);
            }

            //Return the string
            return sValue;
        }
    }


    public class countries
    {
        public string skrot { get; set; }
        public string nazwa { get; set; }
    }


    public class rodzNal
    {
        public string id { get; set; }
        public string nazwa { get; set; }
    }


    class Utils
    {



        public static void LogWriter(string logMesgParam)
        {
            //Ustawienia ustawienia = new Ustawienia();
            //switch(ustawienia.logowanie)
            //{
             //   case 1:
            if ( UserInfo.logMode )
            {
            using (StreamWriter w = File.AppendText("RupIntegratorLog.txt"))
                    {
                        Log(logMesgParam, w);
                       
                        // Close the writer and underlying file.
                        w.Close();
                    }
            }
                //     break;
            
                //    case 2:
             //       System.Console.WriteLine(logMesgParam);
              //      break;
                    
            //}
        
        }

        public static void LogNamedWriter(string logMesgParam, string fname)
        {
            //Ustawienia ustawienia = new Ustawienia();
            //switch(ustawienia.logowanie)
            //{
            //   case 1:
            if (UserInfo.logMode)
            {
            using (StreamWriter w = File.AppendText(fname))
            {
                Log(logMesgParam, w);

                // Close the writer and underlying file.
                w.Close();
            }
            //     break;
            //    case 2:
            //       System.Console.WriteLine(logMesgParam);
            //      break;

            //}
            }
        }
        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
            // Update the underlying file.
            w.Flush();
            w.Close();
        }

    
    

    
   
        
        
    



        public static string getPackageId(string xml)
        { 
            string id = "";
            int i =  xml.IndexOf("</packageId");
            if ( i >= 0 )
            {
                
                int j = xml.LastIndexOf('>',i);
                if (j >= 0)
                    return xml.Substring(j + 1,i-j - 1);
            
            } 
            return "";
        }
            

        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static bool ValidatePesel(ref string szPesel)
        {
            byte[] tab = new byte[10] { 9, 7, 3, 1, 9, 7, 3, 1, 9, 7 };
            byte[] tablicz = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
            bool bResult = false;
            int suma = 0;
            int sumcontrol = 0;

            szPesel = szPesel.Trim();

            if (szPesel.Length == 11)
            {
                foreach (char l in szPesel)
                {
                    byte b = Convert.ToByte(l);
                    if (Array.IndexOf(tablicz, Convert.ToByte(l)) == -1) return false;
                }

                sumcontrol = Convert.ToInt32(szPesel[10].ToString());

                for (int i = 0; i < 10; i++)
                {
                    suma += tab[i] * Convert.ToInt32(szPesel[i].ToString());
                }

                bResult = ((suma % 10) == sumcontrol);

                if (bResult)
                {
                    int rok = 0;
                    int mies = 0;
                    int dzien = Convert.ToInt32(szPesel[4].ToString()) * 10 + Convert.ToInt32(szPesel[5].ToString());

                    if (szPesel[2] == '0' || szPesel[2] == '1')
                    {
                        rok = 1900;
                        mies = Convert.ToInt32(szPesel[2].ToString()) * 10 + Convert.ToInt32(szPesel[3].ToString());
                    }
                    else if (szPesel[2] == '2' || szPesel[2] == '3')
                    {
                        rok = 2000;
                        mies = (Convert.ToInt32(szPesel[2].ToString()) * 10 + Convert.ToInt32(szPesel[3].ToString()) - 20);
                    }
                    else if (szPesel[2] == '4' || szPesel[2] == '5')
                    {
                        rok = 2100;
                        mies = (Convert.ToInt32(szPesel[2].ToString()) * 10 + Convert.ToInt32(szPesel[3].ToString()) - 40);
                    }
                    else if (szPesel[2] == '6' || szPesel[2] == '7')
                    {
                        rok = 2200;
                        mies = (Convert.ToInt32(szPesel[2].ToString()) * 10 + Convert.ToInt32(szPesel[3].ToString()) - 60);
                    }
                    else if (szPesel[2] == '8' || szPesel[2] == '9')
                    {
                        rok = 1800;
                        mies = (Convert.ToInt32(szPesel[2].ToString()) * 10 + Convert.ToInt32(szPesel[3].ToString()) - 80);
                    }
                    rok += Convert.ToInt32(szPesel[0].ToString()) * 10 + Convert.ToInt32(szPesel[1].ToString());
                    String szDate = rok.ToString() + "-" + (mies < 10 ? "0" + mies.ToString() : mies.ToString()) + "-" + (dzien < 10 ? "0" + dzien.ToString() : dzien.ToString());
                    DateTime dt;
                    bResult = DateTime.TryParse(szDate, out dt);
                }
            }
            else return false;

            return bResult;
        }

        public static IList<rodzNal> naleznosci = new List<rodzNal>()
        {
          new rodzNal{ id= "fine", nazwa = "Grzywna"},
          new rodzNal{ id = "compensation", nazwa = "Nawiązka"},
          new rodzNal{ id = "forfeit", nazwa = "Przedmiot przepadku"},
          new rodzNal{ id = "courtCosts", nazwa = "Koszty"},
          new rodzNal{ id = "monetaryPenalty", nazwa = "Kara porządkowa"},
          new rodzNal{ id = "compensatoryDamages", nazwa = "Naprawienie szkody"},
          new rodzNal{ id = "cashBenefits", nazwa = "Świadczenie pieniężne"}
         };

        public static IList<countries> kraje = new List<countries>()
        {
          new countries{ skrot = "AFG", nazwa = "Afganistan"},
new countries{ skrot = "ALB", nazwa = "Albania"},
new countries{ skrot = "DZA", nazwa = "Algieria"},
new countries{ skrot = "AND", nazwa = "Andora"},
new countries{ skrot = "AGO", nazwa = "Angola"},
new countries{ skrot = "AIA", nazwa = "Anguilla"},
new countries{ skrot = "ATA", nazwa = "Antarktyka"},
new countries{ skrot = "ATG", nazwa = "Antigua i Barbuda"},
new countries{ skrot = "SAU", nazwa = "Arabia Saudyjska"},
new countries{ skrot = "ARG", nazwa = "Argentyna"},
new countries{ skrot = "ARM", nazwa = "Armenia"},
new countries{ skrot = "ABW", nazwa = "Aruba"},
new countries{ skrot = "AUS", nazwa = "Australia"},
new countries{ skrot = "AUT", nazwa = "Austria"},
new countries{ skrot = "AZE", nazwa = "Azerbejdżan"},
new countries{ skrot = "BHS", nazwa = "Bahamy"},
new countries{ skrot = "BHR", nazwa = "Bahrajn"},
new countries{ skrot = "BGD", nazwa = "Bangladesz"},
new countries{ skrot = "BRB", nazwa = "Barbados"},
new countries{ skrot = "BEL", nazwa = "Belgia"},
new countries{ skrot = "BLZ", nazwa = "Belize"},
new countries{ skrot = "BEN", nazwa = "Benin"},
new countries{ skrot = "BMU", nazwa = "Bermudy"},
new countries{ skrot = "BTN", nazwa = "Bhutan"},
new countries{ skrot = "BLR", nazwa = "Białoruś"},
new countries{ skrot = "BOL", nazwa = "Boliwia"},
new countries{ skrot = "BES", nazwa = "Bonaire, Sint Eustatius i Saba"},
new countries{ skrot = "BIH", nazwa = "Bośnia i Hercegowina"},
new countries{ skrot = "BWA", nazwa = "Botswana"},
new countries{ skrot = "BRA", nazwa = "Brazylia"},
new countries{ skrot = "BRN", nazwa = "Brunei"},
new countries{ skrot = "IOT", nazwa = "Brytyjskie Terytorium Oceanu Indyjskiego"},
new countries{ skrot = "VGB", nazwa = "Brytyjskie Wyspy Dziewicze"},
new countries{ skrot = "BGR", nazwa = "Bułgaria"},
new countries{ skrot = "BFA", nazwa = "Burkina Faso"},
new countries{ skrot = "BDI", nazwa = "Burundi"},
new countries{ skrot = "CHL", nazwa = "Chile"},
new countries{ skrot = "CHN", nazwa = "Chiny"},
new countries{ skrot = "HRV", nazwa = "Chorwacja"},
new countries{ skrot = "CUW", nazwa = "Curaçao"},
new countries{ skrot = "CYP", nazwa = "Cypr"},
new countries{ skrot = "TCD", nazwa = "Czad"},
new countries{ skrot = "MNE", nazwa = "Czarnogóra"},
new countries{ skrot = "CZE", nazwa = "Czechy"},
new countries{ skrot = "UMI", nazwa = "Dalekie Wyspy Mniejsze Stanów Zjednoczonych"},
new countries{ skrot = "DNK", nazwa = "Dania"},
new countries{ skrot = "COD", nazwa = "Demokratyczna Republika Konga"},
new countries{ skrot = "DMA", nazwa = "Dominika"},
new countries{ skrot = "DOM", nazwa = "Dominikana"},
new countries{ skrot = "DJI", nazwa = "Dżibuti"},
new countries{ skrot = "EGY", nazwa = "Egipt"},
new countries{ skrot = "ECU", nazwa = "Ekwador"},
new countries{ skrot = "ERI", nazwa = "Erytrea"},
new countries{ skrot = "EST", nazwa = "Estonia"},
new countries{ skrot = "ETH", nazwa = "Etiopia"},
new countries{ skrot = "FLK", nazwa = "Falklandy"},
new countries{ skrot = "FJI", nazwa = "Fidżi"},
new countries{ skrot = "PHL", nazwa = "Filipiny"},
new countries{ skrot = "FIN", nazwa = "Finlandia"},
new countries{ skrot = "FRA", nazwa = "Francja"},
new countries{ skrot = "ATF", nazwa = "Francuskie Terytoria Południowe i Antarktyczne"},
new countries{ skrot = "GAB", nazwa = "Gabon"},
new countries{ skrot = "GMB", nazwa = "Gambia"},
new countries{ skrot = "SGS", nazwa = "Georgia Południowa i Sandwich Południowy"},
new countries{ skrot = "GHA", nazwa = "Ghana"},
new countries{ skrot = "GIB", nazwa = "Gibraltar"},
new countries{ skrot = "GRC", nazwa = "Grecja"},
new countries{ skrot = "GRD", nazwa = "Grenada"},
new countries{ skrot = "GRL", nazwa = "Grenlandia"},
new countries{ skrot = "GEO", nazwa = "Gruzja"},
new countries{ skrot = "GUM", nazwa = "Guam"},
new countries{ skrot = "GGY", nazwa = "Guernsey"},
new countries{ skrot = "GUY", nazwa = "Gujana"},
new countries{ skrot = "GUF", nazwa = "Gujana Francuska"},
new countries{ skrot = "GLP", nazwa = "Gwadelupa"},
new countries{ skrot = "GTM", nazwa = "Gwatemala"},
new countries{ skrot = "GIN", nazwa = "Gwinea"},
new countries{ skrot = "GNB", nazwa = "Gwinea Bissau"},
new countries{ skrot = "GNQ", nazwa = "Gwinea Równikowa"},
new countries{ skrot = "HTI", nazwa = "Haiti"},
new countries{ skrot = "ESP", nazwa = "Hiszpania"},
new countries{ skrot = "NLD", nazwa = "Holandia"},
new countries{ skrot = "HND", nazwa = "Honduras"},
new countries{ skrot = "HKG", nazwa = "Hongkong"},
new countries{ skrot = "IND", nazwa = "Indie"},
new countries{ skrot = "IDN", nazwa = "Indonezja"},
new countries{ skrot = "IRQ", nazwa = "Irak"},
new countries{ skrot = "IRN", nazwa = "Iran"},
new countries{ skrot = "IRL", nazwa = "Irlandia"},
new countries{ skrot = "ISL", nazwa = "Islandia"},
new countries{ skrot = "ISR", nazwa = "Izrael"},
new countries{ skrot = "JAM", nazwa = "Jamajka"},
new countries{ skrot = "JPN", nazwa = "Japonia"},
new countries{ skrot = "YEM", nazwa = "Jemen"},
new countries{ skrot = "JEY", nazwa = "Jersey"},
new countries{ skrot = "JOR", nazwa = "Jordania"},
new countries{ skrot = "CYM", nazwa = "Kajmany"},
new countries{ skrot = "KHM", nazwa = "Kambodża"},
new countries{ skrot = "CMR", nazwa = "Kamerun"},
new countries{ skrot = "CAN", nazwa = "Kanada"},
new countries{ skrot = "QAT", nazwa = "Katar"},
new countries{ skrot = "KAZ", nazwa = "Kazachstan"},
new countries{ skrot = "KEN", nazwa = "Kenia"},
new countries{ skrot = "KGZ", nazwa = "Kirgistan"},
new countries{ skrot = "KIR", nazwa = "Kiribati"},
new countries{ skrot = "COL", nazwa = "Kolumbia"},
new countries{ skrot = "COM", nazwa = "Komory"},
new countries{ skrot = "COG", nazwa = "Kongo"},
new countries{ skrot = "KOR", nazwa = "Korea Południowa"},
new countries{ skrot = "PRK", nazwa = "Korea Północna"},
new countries{ skrot = "CRI", nazwa = "Kostaryka"},
new countries{ skrot = "CUB", nazwa = "Kuba"},
new countries{ skrot = "KWT", nazwa = "Kuwejt"},
new countries{ skrot = "LAO", nazwa = "Laos"},
new countries{ skrot = "LSO", nazwa = "Lesotho"},
new countries{ skrot = "LBN", nazwa = "Liban"},
new countries{ skrot = "LBR", nazwa = "Liberia"},
new countries{ skrot = "LBY", nazwa = "Libia"},
new countries{ skrot = "LIE", nazwa = "Liechtenstein"},
new countries{ skrot = "LTU", nazwa = "Litwa"},
new countries{ skrot = "LUX", nazwa = "Luksemburg"},
new countries{ skrot = "LVA", nazwa = "Łotwa"},
new countries{ skrot = "MKD", nazwa = "Macedonia"},
new countries{ skrot = "MDG", nazwa = "Madagaskar"},
new countries{ skrot = "MYT", nazwa = "Majotta"},
new countries{ skrot = "MAC", nazwa = "Makau"},
new countries{ skrot = "MWI", nazwa = "Malawi"},
new countries{ skrot = "MDV", nazwa = "Malediwy"},
new countries{ skrot = "MYS", nazwa = "Malezja"},
new countries{ skrot = "MLI", nazwa = "Mali"},
new countries{ skrot = "MLT", nazwa = "Malta"},
new countries{ skrot = "MNP", nazwa = "Mariany Północne"},
new countries{ skrot = "MAR", nazwa = "Maroko"},
new countries{ skrot = "MTQ", nazwa = "Martynika"},
new countries{ skrot = "MRT", nazwa = "Mauretania"},
new countries{ skrot = "MUS", nazwa = "Mauritius"},
new countries{ skrot = "MEX", nazwa = "Meksyk"},
new countries{ skrot = "FSM", nazwa = "Mikronezja"},
new countries{ skrot = "MMR", nazwa = "Mjanma"},
new countries{ skrot = "MDA", nazwa = "Mołdawia"},
new countries{ skrot = "MCO", nazwa = "Monako"},
new countries{ skrot = "MNG", nazwa = "Mongolia"},
new countries{ skrot = "MSR", nazwa = "Montserrat"},
new countries{ skrot = "MOZ", nazwa = "Mozambik"},
new countries{ skrot = "NAM", nazwa = "Namibia"},
new countries{ skrot = "NRU", nazwa = "Nauru"},
new countries{ skrot = "NPL", nazwa = "Nepal"},
new countries{ skrot = "DEU", nazwa = "Niemcy"},
new countries{ skrot = "NER", nazwa = "Niger"},
new countries{ skrot = "NGA", nazwa = "Nigeria"},
new countries{ skrot = "NIC", nazwa = "Nikaragua"},
new countries{ skrot = "NIU", nazwa = "Niue"},
new countries{ skrot = "NFK", nazwa = "Norfolk"},
new countries{ skrot = "NOR", nazwa = "Norwegia"},
new countries{ skrot = "NCL", nazwa = "Nowa Kaledonia"},
new countries{ skrot = "NZL", nazwa = "Nowa Zelandia"},
new countries{ skrot = "OMN", nazwa = "Oman"},
new countries{ skrot = "PAK", nazwa = "Pakistan"},
new countries{ skrot = "PLW", nazwa = "Palau"},
new countries{ skrot = "PSE", nazwa = "Palestyna"},
new countries{ skrot = "PAN", nazwa = "Panama"},
new countries{ skrot = "PNG", nazwa = "Papua-Nowa Gwinea"},
new countries{ skrot = "PRY", nazwa = "Paragwaj"},
new countries{ skrot = "PER", nazwa = "Peru"},
new countries{ skrot = "PCN", nazwa = "Pitcairn"},
new countries{ skrot = "PYF", nazwa = "Polinezja Francuska"},
new countries{ skrot = "POL", nazwa = "Polska"},
new countries{ skrot = "ZAF", nazwa = "Południowa Afryka"},
new countries{ skrot = "PRI", nazwa = "Portoryko"},
new countries{ skrot = "PRT", nazwa = "Portugalia"},
new countries{ skrot = "CAF", nazwa = "Republika Środkowoafrykańska"},
new countries{ skrot = "CPV", nazwa = "Republika Zielonego Przylądka"},
new countries{ skrot = "REU", nazwa = "Reunion"},
new countries{ skrot = "RUS", nazwa = "Rosja"},
new countries{ skrot = "ROU", nazwa = "Rumunia"},
new countries{ skrot = "RWA", nazwa = "Rwanda"},
new countries{ skrot = "ESH", nazwa = "Sahara Zachodnia"},
new countries{ skrot = "KNA", nazwa = "Saint Kitts i Nevis"},
new countries{ skrot = "LCA", nazwa = "Saint Lucia"},
new countries{ skrot = "VCT", nazwa = "Saint Vincent i Grenadyny"},
new countries{ skrot = "BLM", nazwa = "Saint-Barthélemy"},
new countries{ skrot = "MAF", nazwa = "Saint-Martin"},
new countries{ skrot = "SPM", nazwa = "Saint-Pierre i Miquelon"},
new countries{ skrot = "SLV", nazwa = "Salwador"},
new countries{ skrot = "WSM", nazwa = "Samoa"},
new countries{ skrot = "ASM", nazwa = "Samoa Amerykańskie"},
new countries{ skrot = "SMR", nazwa = "San Marino"},
new countries{ skrot = "SEN", nazwa = "Senegal"},
new countries{ skrot = "SRB", nazwa = "Serbia"},
new countries{ skrot = "SYC", nazwa = "Seszele"},
new countries{ skrot = "SLE", nazwa = "Sierra Leone"},
new countries{ skrot = "SGP", nazwa = "Singapur"},
new countries{ skrot = "SXM", nazwa = "Sint Maarten"},
new countries{ skrot = "SVK", nazwa = "Słowacja"},
new countries{ skrot = "SVN", nazwa = "Słowenia"},
new countries{ skrot = "SOM", nazwa = "Somalia"},
new countries{ skrot = "LKA", nazwa = "Sri Lanka"},
new countries{ skrot = "USA", nazwa = "Stany Zjednoczone"},
new countries{ skrot = "SWZ", nazwa = "Suazi"},
new countries{ skrot = "SDN", nazwa = "Sudan"},
new countries{ skrot = "SSD", nazwa = "Sudan Południowy"},
new countries{ skrot = "SUR", nazwa = "Surinam"},
new countries{ skrot = "SJM", nazwa = "Svalbard i Jan Mayen"},
new countries{ skrot = "SYR", nazwa = "Syria"},
new countries{ skrot = "CHE", nazwa = "Szwajcaria"},
new countries{ skrot = "SWE", nazwa = "Szwecja"},
new countries{ skrot = "TJK", nazwa = "Tadżykistan"},
new countries{ skrot = "THA", nazwa = "Tajlandia"},
new countries{ skrot = "TWN", nazwa = "Tajwan"},
new countries{ skrot = "TZA", nazwa = "Tanzania"},
new countries{ skrot = "TLS", nazwa = "Timor Wschodni"},
new countries{ skrot = "TGO", nazwa = "Togo"},
new countries{ skrot = "TKL", nazwa = "Tokelau"},
new countries{ skrot = "TON", nazwa = "Tonga"},
new countries{ skrot = "TTO", nazwa = "Trynidad i Tobago"},
new countries{ skrot = "TUN", nazwa = "Tunezja"},
new countries{ skrot = "TUR", nazwa = "Turcja"},
new countries{ skrot = "TKM", nazwa = "Turkmenistan"},
new countries{ skrot = "TCA", nazwa = "Turks i Caicos"},
new countries{ skrot = "TUV", nazwa = "Tuvalu"},
new countries{ skrot = "UGA", nazwa = "Uganda"},
new countries{ skrot = "UKR", nazwa = "Ukraina"},
new countries{ skrot = "URY", nazwa = "Urugwaj"},
new countries{ skrot = "UZB", nazwa = "Uzbekistan"},
new countries{ skrot = "VUT", nazwa = "Vanuatu"},
new countries{ skrot = "WLF", nazwa = "Wallis i Futuna"},
new countries{ skrot = "VAT", nazwa = "Watykan"},
new countries{ skrot = "VEN", nazwa = "Wenezuela"},
new countries{ skrot = "HUN", nazwa = "Węgry"},
new countries{ skrot = "GBR", nazwa = "Wielka Brytania"},
new countries{ skrot = "VNM", nazwa = "Wietnam"},
new countries{ skrot = "ITA", nazwa = "Włochy"},
new countries{ skrot = "CIV", nazwa = "Wybrzeże Kości Słoniowej"},
new countries{ skrot = "BVT", nazwa = "Wyspa Bouveta"},
new countries{ skrot = "CXR", nazwa = "Wyspa Bożego Narodzenia"},
new countries{ skrot = "IMN", nazwa = "Wyspa Man"},
new countries{ skrot = "SHN", nazwa = "Wyspa Świętej Heleny, Wyspa Wniebowstąpienia i Tristan da Cunha"},
new countries{ skrot = "ALA", nazwa = "Wyspy Alandzkie"},
new countries{ skrot = "COK", nazwa = "Wyspy Cooka"},
new countries{ skrot = "VIR", nazwa = "Wyspy Dziewicze Stanów Zjednoczonych"},
new countries{ skrot = "HMD", nazwa = "Wyspy Heard i McDonalda"},
new countries{ skrot = "CCK", nazwa = "Wyspy Kokosowe"},
new countries{ skrot = "MHL", nazwa = "Wyspy Marshalla"},
new countries{ skrot = "FRO", nazwa = "Wyspy Owcze"},
new countries{ skrot = "SLB", nazwa = "Wyspy Salomona"},
new countries{ skrot = "STP", nazwa = "Wyspy Świętego Tomasza i Książęca"},
new countries{ skrot = "ZMB", nazwa = "Zambia"},
new countries{ skrot = "ZWE", nazwa = "Zimbabwe"},
new countries{ skrot = "ARE", nazwa = "Zjednoczone Emiraty Arabskie"}
        };

         public static string getLiabilId( string kartadl, string dbId , int what ,int number)
        {
            return kartadl +"/" + (what == 0 ? "G":"K" )  +"/" + dbId + "/" + number.ToString().PadLeft(10, '0');
        
        
        }


         public static string getDebtorId(string kartadl, string dbId,  int number)
         {
             return kartadl + "/" + dbId + "/" + number.ToString().PadLeft(10, '0');


         }

        public static string ifDBNULLString(GridViewCellInfo o)
        {
            if (o.Value is DBNull)
                return "";
            else
                return o.Value.ToString();

        }
       


       public static string  SetupExceptionMessage(Exception ex)
        {

            return  ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");

        }


       

        public static string Encrypt(string strToEncrypt, string strKey)
        {
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
                byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().
                    TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Błąd kodowania " + ex.Message;
            }
        }


        public static string Decrypt(string strEncrypted, string strKey)
        {
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

       
        public static string BuildMyConnectionString(RupIntegratorEntities myContext)
        {



            Konfiguracja knf = myContext.Konfiguracja.FirstOrDefault();

            EntityConnection ec = (EntityConnection)myContext.Connection;
            SqlConnection sc = (SqlConnection)ec.StoreConnection; //get the SQLConnection that your entity object would use
            string adoConnStr = sc.ConnectionString;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(adoConnStr);

            // Supply the additional values.
            builder.Password = Utils.Decrypt(myContext.Konfiguracja.FirstOrDefault().pwd, "Application error");
            if (knf.typKns == 2) // orcom
            {
                builder.UserID = knf.logId;
                builder.IntegratedSecurity = Convert.ToBoolean(knf.WinLogon);
                builder.DataSource = knf.srvName;
                builder.InitialCatalog = knf.DbName;

            }
            return builder.ConnectionString; //sc.ConnectionString;



        }

    }

      

    public class PeselCheckSumCalculator
    {
        private static readonly int[] _Weight = new[] { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

        public static int Calculate(string pesel)
        {
            int checkSum = pesel.Zip(_Weight, (digit, weight) => (digit - '0') * weight)
                .Sum();

            int lastDigit = checkSum % 10;

            return lastDigit == 0 ? 0 : 10 - lastDigit;
        }
    }


    public class PeselGenerator
    {
        private readonly Random _random;

        public PeselGenerator()
        {
            _random = new Random();
        }

        public string Generate()
        {
            var peselStringBuilder = new StringBuilder();
            DateTime birthDate = GenerateDate(1900, 2099);

            AppendPeselDate(birthDate, peselStringBuilder);

            peselStringBuilder.Append(GenerateRandomNumbers(4));

            peselStringBuilder.Append(PeselCheckSumCalculator.Calculate(peselStringBuilder.ToString()));

            return peselStringBuilder.ToString();
        }

        public static string GetPeselMonthShiftedByYear(DateTime date)
        {
            if (date.Year < 1900 || date.Year > 2299)
            {
                throw new NotSupportedException(string.Format("PESEL for year: {0} is not supported", date.Year));
            }

            int monthShift = (int)((date.Year - 1900) / 100) * 20;

            return (date.Month + monthShift).ToString("00");
        }

        private DateTime GenerateDate(int yearFrom, int yearTo)
        {
            int year = _random.Next(yearFrom, yearTo + 1);
            int month = _random.Next(12) + 1;
            int day = _random.Next(DateTime.DaysInMonth(year, month)) + 1;

            return new DateTime(year, month, day);
        }

        private void AppendPeselDate(DateTime date, StringBuilder builder)
        {
            builder.Append((date.Year % 100).ToString("00"));
            builder.Append(GetPeselMonthShiftedByYear(date));
            builder.Append(date.Day.ToString("00"));
        }

        private string GenerateRandomNumbers(int numbersCount)
        {
            int maxValue = (int)Math.Pow(10, numbersCount);
            string format = "D" + numbersCount;

            return _random.Next(maxValue).ToString(format);
        }
    }



}
