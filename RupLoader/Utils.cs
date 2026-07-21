using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.ServiceModel;
using RupLoader;
using Ex2PscdInterface.Ex2PscdContractObjectCreateOutService;

namespace RupLoader
{

    public static class GlobalStrings
    {
        public const string SYGN_IN_SAD = "Oznaczenie sądu w sygnaturze";
        public const string APP_ERROR = "Application error";
    }
    public static class UserProfile
    {
        public static string Username { get; set; }
        public static int UserID { get; set; }

    }

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string DoTrim(this string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return  value.Trim();
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

    public class typPartner
    {
        public int  nr { get; set; }
        public string nazwa { get; set; }
    }

    

   

    public class Utils
    {

        public static IList<typPartner> naleznosci = new List<typPartner>()
        {
          new typPartner{ nr = 0 ,   nazwa = "<Wszyscy>"},
          new typPartner{ nr = 1 ,   nazwa = "Pełnomocnik/Obrońca"},
          new typPartner{  nr =  2 , nazwa = "Komornik"},
          new typPartner{  nr =  3 , nazwa = "Podmiot"},
          new typPartner{  nr =  4 , nazwa = "Os fizyczna"},
          new typPartner{  nr =  5 , nazwa = "Funkcjonariusz Policji/SM"},
        };
        
        private static string[] FilterOperators = { "EQ", "NE", "BT" };
    

        private  static string[]  errtab ={"Sygnatura nie może być pusta",
                                    "Sygnatura nie może być krótsza niż 6 znaków",
                                     "Błąd sygnatury, brak znaku / " ,
                                     "Błędny rok sprawy w  sygnaturze",
                                      "Błędny numer sprawy"   ,
                                      "Niepoprawne oznaczenie wydziału/sekcji",
                                       "Błędne oznaczenie repertorium",
                                       "Karta dłużnika nie może być pusta",
                                        "Błędne oznaczenie (długość) karty dłużnika",
                                        "Błędny numer karty dłużnika",
                                        "Błędny rok karty dłużnika"};

        public static bool isWSConn(Konfiguracja knskonfig)
        {
            
            if (knskonfig.DbName.ToUpper().Contains("HTTP") && knskonfig.typKns == 2 )  // dla Orcom - centralny
                return true;
            
            return false;
        }

      

        public static string BuildKnsConnectionString(Konfiguracja konfig)
        {
            
            string ConnectionString;

            ConnectionString = "";

            switch (konfig.typKns)
            {
                case 0: // currenda;            
                case 1: // zeto swidnica
                case 2: // Orcom
                case 3: 
                    ConnectionString = "Server=" + konfig.srvName + ";database=" + konfig.DbName;
                    if (konfig.WinLogon == true)
                    {
                        ConnectionString += ";Trusted_Connection=True;";
                    }
                    else
                    {

                        ConnectionString += ";User Id=" + konfig.logId + ";Password=" + Utils.Decrypt(konfig.pwd,"Application error")  + ";";


                    }
                    break;

                default:
                    break;
            }
            return ConnectionString;

        
        
        }

        public static string BuildMyConnectionString(RupIntegratorEntities myContext )
        {



            RL_Konfig knf = myContext.RL_Konfig.FirstOrDefault();

            EntityConnection ec = (EntityConnection)myContext.Connection;
            SqlConnection sc = (SqlConnection)ec.StoreConnection; //get the SQLConnection that your entity object would use
            string adoConnStr = sc.ConnectionString;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(adoConnStr);

            // Supply the additional values.
            builder.Password = Utils.Decrypt(myContext.Konfiguracja.FirstOrDefault().pwd, "Application error");
            if (knf.typDB == 2) // orcom
            {
                builder.UserID = knf.logId;
                builder.IntegratedSecurity = Convert.ToBoolean(knf.WinLogon);
                builder.DataSource = knf.srvName;
                builder.InitialCatalog = knf.DbName;
                     
            }
            return builder.ConnectionString; //sc.ConnectionString;



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

        public static string GetWSPwd()
        {
            string pwd = "nsttC/uv3mhPxqABh4vj8A==";
            return Decrypt(pwd, "Application error");
        
        
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

         private static char normalizeChar( char c)
        {
             
        switch ( c)
        {
            case 'ą':
                return 'a';
            case 'ć':
                return 'c';
            case 'ę':
                return 'e';
            case 'ł':
                return 'l';
            case 'ń':
                return 'n';
            case 'ó':
                return 'o';
            case 'ś':
                return 's';
            case 'ż':
            case 'ź':
                return 'z';
            case 'Ą':
                return 'A';
            case 'Ć':
                return 'C';
            case 'Ę':
                return 'E';
            case 'Ł':
                return 'L';
            case 'Ń':
                return 'N';
            case 'Ó':
                return 'O';
            case 'Ś':
                return 'S';
            case 'Ż':
            case 'Ź':
                return 'Z';
        }
        return c;
        }

         public static  string normalizeString(string s)
         {
             if (s == null) return null;
             string outstring = "";

             foreach (char c in s)
             {
                 outstring += normalizeChar(c);
             
             
             }


             return outstring;
         
         }



        public static  void setEncodingFile(string exportFileName)
        {
            string fileName = null;
            //exportFileName =  normalizeString(exportFileName);
            try
            {
                fileName = Path.GetDirectoryName(exportFileName) + "\\" + normalizeString(Path.GetFileNameWithoutExtension(exportFileName)) + "_1" + Path.GetExtension(exportFileName);
                StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8);
                using (StreamReader sr = new StreamReader(exportFileName))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {

                        line = Regex.Replace(line, "\";\"", ";");
                        line = Regex.Replace(line, "\"", "");
                        
                        if (line.Length > 1)
                        {
                            if (line[0] == '"')
                                line = line.Substring(1);
                            if (line[line.Length - 1] == '"')
                                line = line.Substring(0, line.Length - 1);
                        }
                        sw.WriteLine(line);

                        //sw.WriteLine(Regex.Replace(line, "\"", ""));
                        sw.Flush();
                    }
                }
                sw.Close();
                File.Delete(exportFileName);
                File.Move(fileName, exportFileName);
            }

            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas normalizacji zbioru eksportu " + ex.Message); 
            
            
            }
        }

        public static string ParseKartaDl(int KnsType, string kdl, out string ksiega, out int numer, out int rok)
        {
            string errcode = "";
            int pos,  n;
            string s;

            ksiega = "";
            numer = 0;
            rok = 0;
            if (kdl == null) { return errtab[7]; }
            if ((kdl.Trim().Length < 8)) { return errtab[8]; }

            switch (KnsType)
            {
                case 0:
                case 3:     // system WW |Currenda lub Albit
                    {

                        pos = kdl.IndexOf('/');
                        if (pos > 0)
                        {
                            s = kdl.Substring(0, pos);
                            if (!int.TryParse(s, out n)) return errtab[9];
                            numer = n;
                            kdl = kdl.Substring(pos + 1);
                        }
                        else return errtab[9];
                        pos = kdl.IndexOf('/');
                        if (pos > 0)
                        {
                            s = kdl.Substring(0, pos);
                            if (!int.TryParse(s, out n)) return errtab[10];
                            rok = n;
                            ksiega = kdl.Substring(pos + 1);
                        }
                        else return errtab[10];

                    }
                    break;

                case 1:  // ZETO
                    
                    {

                        pos = kdl.IndexOf('/');
                        if (pos > 0)
                        {
                            s = kdl.Substring(0,pos);
                            pos = s.LastIndexOf(" ");
                            if (pos > 0 )
                            {
                                s = s.Substring(pos).Trim();
                                if (!int.TryParse(s, out n)) return errtab[9];
                                numer = n;
                             kdl = kdl.Substring(pos + 1);
                            }
                        }
                        else return errtab[9];
                        pos = kdl.IndexOf('/');
                        if (pos > 0)
                        {
                            s = kdl.Substring(pos + 1, 2);
                            if (!int.TryParse(s, out n)) return errtab[10];
                            if (n > 70)
                                n += 1900;
                            else
                                n += 2000;
                            rok = n;
                            ksiega = ""; // nieistotne
                        }
                        else return errtab[10];

                    }
                    break;
                default:
                    break;
            }


            return errcode;
        }

        public static string ParseFilterValue(string filterValue,out DateTime d_od,out DateTime d_do)
        { 
            char[] septab  = new char[]{'(','-',',',')'};
            string[] values = new string[4]; 
            string errMsg;
          

            d_od = DateTime.MinValue;
            d_do = DateTime.MinValue;
            try{

                values[0] = filterValue.Substring(1, 2);
                values[1] = filterValue.Substring(4, 8);
                values[2] = filterValue.Substring(13, 8);

            switch (values[0])
            {
                case "BT" :
                                d_od  = new DateTime(Convert.ToInt32(values[1].Substring(0,4)),Convert.ToInt32(values[1].Substring(4,2)),Convert.ToInt32(values[1].Substring(6,2)));
                                d_do  = new DateTime(Convert.ToInt32(values[2].Substring(0,4)),Convert.ToInt32(values[2].Substring(4,2)),Convert.ToInt32(values[2].Substring(6,2)));
                    break;
                case "EQ":
                        d_od  = new DateTime(Convert.ToInt32(values[1].Substring(0,4)),Convert.ToInt32(values[1].Substring(4,2)),Convert.ToInt32(values[1].Substring(6,2)));
                        d_do = d_od;
                    break;
                default:
                        errMsg = "Niedozwolona wartosć filtra " + values[1] +  "   Filtr " + filterValue;
                        MessageBox.Show (errMsg);
                        return errMsg;
                    
            }
            return "";
            }
        
        catch(Exception ex)
          {

              errMsg =  "Niedozwolona wartosć filtra " + filterValue + " Wyjątek " + ex.Message;
              MessageBox.Show(errMsg);
              return errMsg;
               
        }
        }
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static  string isnSpot(string instring)
        {
            string outstring="";
            bool isdone = false;

            if (String.IsNullOrWhiteSpace(instring)) return instring;

            if (instring.Where(x => Char.IsDigit(x)).Any())
            {
                foreach (char c in instring)
                {
                    if (Char.IsDigit(c) && !isdone)
                    {
                        outstring += '.';
                        isdone = true; 
                    }
                    outstring += c;
                }

                return outstring;
            }
            else
                return instring;
 
        }


        public static string ParseSygn(string sygnatura, out string wydzialSekcja, out string repertorium, out int nr, out int year, out string oryginRepertorium, out string sadout, string sapSadId = "")
        {


            string sygn; 
            int pos, i, digit;
            int rok;
            string yr;
            string numer = "";
            int pos1;
            string[] romanDigits = { "I", "V", "X", "." };
            string blad = "";
            string destSad = "";


            wydzialSekcja = "";
            nr = 0;
            year = 0;
            repertorium = "";
            pos1 = 0;
            oryginRepertorium = "";
            wydzialSekcja = "";
            sadout = sapSadId;
            
            sygn = sygnatura;

            int j = 0;
            try
            {
               
                if (!String.IsNullOrWhiteSpace(sapSadId) && Int32.TryParse(sapSadId, out j))
                {
                    
                    if (j >= 6000)
                    {
                        ReplaceSygn(sapSadId, wydzialSekcja, sygn, nr, year, out destSad, out wydzialSekcja, out sygn, out nr, out year);
                        repertorium = sygn;
                        sadout = destSad;
                        wydzialSekcja = isnSpot(wydzialSekcja);
                        return "";
                    }
                }
                
                if (String.IsNullOrWhiteSpace(sygnatura)) { return errtab[0]; } // sygnatura 

                if ((sygn.Trim().Length < 4)) { return errtab[1]; }
                /*
                        pos = sygn.ToUpper().LastIndexOf("SR");
                        if (pos > 0)
                            ;
                        else
                        {
                            pos = sygn.ToUpper().LastIndexOf("S.R");
                            if (pos > 0)
                                ;
                            else
                                pos = sygn.ToUpper().LastIndexOf(" Sąd");
                        }
                        if (pos > 6)
                        {
                            sygn = sygn.Substring(0, pos).Trim();
                            blad = GlobalStrings.SYGN_IN_SAD;
                        }
                 * */
                pos = sygn.IndexOf('/');
                if (pos > 0)
                    ;
                else
                    pos = sygn.Length;
                if (pos > 0)
                {
                    int yr_digit = 0;
                    rok = 0;
                    for (i = pos + 1; i < sygn.Length; i++)
                    {
                        string s = sygn.Substring(i, 1);
                        if (s == " " && rok == 0) continue;
                        if (!int.TryParse(s, out yr_digit)) break; // for
                        rok = rok * 10 + yr_digit;
                    }

                    if (rok >= 0 && rok < 30)
                        rok += 2000;
                    else
                        if (rok > 88 && rok < 100)
                            rok += 1900;
                        else
                        {
                            if (rok > 1000)
                                ;
                            else
                                rok = 0;
                        }
                }
                else return errtab[2];
                bool isnew = true;
                for (i = pos - 1; i >= 0; i--)
                {
                    yr = sygn.Substring(i, 1);
                    if (isnew && String.IsNullOrWhiteSpace(yr)) continue;
                    if (int.TryParse(yr, out digit))
                    {
                        isnew = false;
                        numer = yr + numer;
                    }
                    else
                    {
                        pos1 = i;
                        break;
                    }

                }

                if (rok >= 0 && numer.Length > 0)
                {
                    year = rok;
                    nr = Convert.ToInt32(numer);

                }
                else return errtab[4];
                // 
                sygn = sygn.Substring(0, pos1).Trim();
                pos1 = 0;
                for (i = 0; i < sygn.Length; i++)
                {
                    yr = sygn.Substring(i, 1).ToUpper();
                    if (yr == " ") continue;
                    if (romanDigits.Contains(yr) || int.TryParse(yr, out digit))
                        wydzialSekcja += yr.ToUpper();
                    else
                    {
                        pos1 = i;
                        break;
                    }

                }

                sygn = sygn.Substring(pos1).Trim().Replace(" ", String.Empty).ToUpper();
                i = 0;



                oryginRepertorium = sygn;

               
         
                ReplaceSygn(sapSadId, wydzialSekcja, sygn, nr, year, out destSad, out wydzialSekcja, out sygn, out nr, out year);

                repertorium = sygn;
                sadout = destSad;
                if (wydzialSekcja.Length == 0) return errtab[5];
                wydzialSekcja = isnSpot(wydzialSekcja); 
                if (String.IsNullOrEmpty(repertorium))
                {
                    repertorium = "";
                    return errtab[6];
                }
                return blad;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd podczas parsowania sygnatury " + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "") + sygnatura);
                return blad;
            }
        }




        public static string cleanNIP(string NIP)
        {
            string s = "";
            int i, j;
            if (NIP == null) return "";
            for (i = 0; i < NIP.Length; i++)
            {
                if (int.TryParse(NIP[i].ToString(), out j) == true)
                {
                    s += NIP[i];

                }
            }
            if (s.Length != 10 && s.Length > 0)
            {
                return "";

            }
            else
                return s;
        }
        public static Int32 HashFromString(string key)
        {
            UInt32 hash, i;

            char[] szArr = key.ToCharArray();

            for (hash = i = 0; i < szArr.Length; ++i)
            {
                hash += szArr[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            
            return (int)hash;
        }
        public static string getSygnatura(string inString)
        {
            // [IXV]+[\s]*[A-Z]+[\s]*[0-9]+\s*/[\s]*[0-9]+
        //  http://www.freeformatter.com/regex-tester.html
            
            inString = inString.ToUpper().Replace("\\","/");
            Regex r = new Regex(@"[IXV]+[\s]*[A-Z]+[\s]*[0-9]+\s*/[\s]*[0-9]+");
            Match m = r.Match(inString);
            if (m.Success)
                return m.Value;
            else return null;

           
        
        }

        public static void LogWriter(string logMesgParam)
        {
            //Ustawienia ustawienia = new Ustawienia();
            //switch(ustawienia.logowanie)
            //{
             //   case 1:
                    using (StreamWriter w = File.AppendText("RupIntegratorLog.txt"))
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

        private static string translateS(string instr, string replacement)
        {
            string outstr = "";
            if (String.IsNullOrWhiteSpace(instr))
                return replacement;
            for (int i = 0; i < instr.Length; i++)
            {
                char c = instr[i];
                if (c == '*')
                {
                    outstr += replacement;

                }
                else
                {
                    outstr += c;
                }

            }
            return outstr;
        }

        private static int translateI(string instr, int repl)
        {
            bool islen = false;
            int totallength = 0;
            bool is_before = true;
            string before = "";
            string replacement = repl.ToString();
            string outstr = "";
            if (String.IsNullOrWhiteSpace(instr))
                return repl;
            for (int i = 0; i < instr.Length; i++)
            {
                char c = instr[i];
                if (c == '{')
                {

                    islen = true;
                    continue;
                }
                if (c == '}')
                {
                    islen = false;
                    continue;
                }
                if (islen)
                {
                    totallength = totallength * 10 + Convert.ToInt32(c.ToString());
                    continue;
                }
                if (c == '*')
                {

                    is_before = false;
                    outstr += replacement;

                }
                else
                {
                    if (is_before)
                        before += c;
                    else
                        outstr += c;
                }

            }

            if (totallength > 0 && totallength >= before.Length + outstr.Length)
            {
                outstr = before + new String('0', totallength - before.Length - outstr.Length) + outstr;

            }
            else
                if (totallength > 0)
                {
                    outstr = outstr.Right(totallength);

                }
                else
                    outstr = before + outstr;
            int outint;

            if (Int32.TryParse(outstr, out outint))
                return outint;
            else
                return 0;

        }
        public static int ReplaceSygn(string srcSad, string srcWydz, string srcRep, int srcNr, int srcRok, out string destSad, out string destWydz, out string destRep, out int destNr, out int destRok)
        {
            List<SygnMap> snmap = null;
            destWydz = srcWydz;
            destRep = srcRep;
            destNr = srcNr;
            destRok = srcRok;
            destSad = srcSad;
            using (RupIntegratorEntities  knsMigr = new RupIntegratorEntities())
            {
                snmap = knsMigr.SygnMap.OrderBy(a => a.priorytet).ToList();
                if (!snmap.Any())
                    return -1; // brak zamienników.
                foreach (SygnMap sm in snmap)
                {
                    if (String.IsNullOrWhiteSpace(sm.SrcSad) && String.IsNullOrWhiteSpace(sm.SrcWydz) && String.IsNullOrWhiteSpace(sm.SrcRep)) continue;
                    if ((String.IsNullOrWhiteSpace(sm.SrcSad) || (!String.IsNullOrWhiteSpace(sm.SrcSad) && sm.SrcSad == srcSad)) &&
                        (String.IsNullOrWhiteSpace(sm.SrcWydz) || sm.SrcWydz == "*" || (!String.IsNullOrWhiteSpace(sm.SrcWydz) && sm.SrcWydz == srcWydz)) &&
                        (String.IsNullOrWhiteSpace(sm.SrcRep) || sm.SrcRep == "*" || (!String.IsNullOrWhiteSpace(sm.SrcRep) && srcRep.ToUpper() == sm.SrcRep.ToUpper())))
                    {

                        destSad = translateS(sm.DestSad, srcSad);
                        destWydz = translateS(sm.DestWydz, srcWydz);
                        destRep = translateS(sm.DestRep, srcRep);
                        destNr = translateI(sm.DestNr, srcNr);
                        destRok = translateI(sm.DestRok, srcRok);

                        return sm.Id; // wykonano podmianę 
                    }


                }



            }

            return 0;
        }
        public static void LogNamedWriter(string logMesgParam, string fname)
        {
            //Ustawienia ustawienia = new Ustawienia();
            //switch(ustawienia.logowanie)
            //{
            //   case 1:
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

        public static string getTechSygn(int idConfig)
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

        public static void addSygnatura(SygnaturaTworzenie sygn, string sygnIn, string PrzedmiotUmowy)
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

        public static string verifySygnatura(SygnaturaTworzenie sygn)
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



        public static SygnaturaTworzenie setupSygnStruct(rStruct dok, Konfiguracja konf)
        {
            try
            {
                SygnaturaTworzenie sygnqry = new SygnaturaTworzenie();
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
        public static bool rebuildDbScript()
        {
            string cmd = string.Empty;
            try
            {
                using (RupIntegratorEntities dbContext = new RupIntegratorEntities())
                {
                    foreach (string sqlcmd in sqlCommands())
                    {
                        cmd = sqlcmd;
                        dbContext.ExecuteStoreCommand(cmd);

                    }
                    Konfiguracja knf = dbContext.Konfiguracja.FirstOrDefault();
                    knf.dbversion = RunMode.dbVersion;
                    dbContext.SaveChanges();
                }

                MessageBox.Show("Struktura bazy danych została pomyślnie zaktualizowana");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "") + "\n" + "Błąd podczas przebudowy struktury bazy danych dla polecenia: " + cmd + "\n\r" + ex.Message);
                return false;

            }

        }

        private static string[] sqlCommands()
        {
            string[] commnadsList = {
                 // wer 3.3
                " IF isnull((select count(1) from [user] where role = 1 ),0) = 0  BEGIN " +
                " INSERT [dbo].[User] ([Username], [Pssword], [role], [LastPwdChngDate], [suspend], [ChangePwd], [FirstName], [LastName], [deleted], [CreationDate], [DeleteDate], [PwdPeriodChange]) VALUES ( N'admin', N'j/6oZDQ3GQ4=', 1, NULL, 0, 0, N'Admin', N'Admin', 0, CAST(N'2017-03-07T10:35:03.193' AS DateTime), NULL, 0 )  END " ,
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Dokument' AND COLUMN_NAME = 'StanowiskoFianasoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Dokument ADD  [StanowiskoFianasoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'StanowiskoFianasoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Konfiguracja ADD  [StanowiskoFianasoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Sprawa' AND COLUMN_NAME = 'StanowiskoFianasoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Sprawa ADD  [StanowiskoFianasoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Ekstrakcja' AND COLUMN_NAME = 'StanowiskoFianasoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Ekstrakcja ADD  [StanowiskoFianasoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Transfer' AND COLUMN_NAME = 'StanowiskoFinansoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Transfer ADD  [StanowiskoFinansoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Ekstrakcja' AND COLUMN_NAME = 'SAPKluczUzgodnienia' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Ekstrakcja ADD  [SAPKluczUzgodnienia][varchar] (12) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Ekstrakcja' AND COLUMN_NAME = 'JeGoWindyk' AND TABLE_SCHEMA='DBO')  "+
                        "  BEGIN ALTER TABLE dbo.Ekstrakcja ADD  [JeGoWindyk][varchar] (4) NULL END ",

                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'Pfx' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Konfiguracja ADD Pfx varbinary(MAX) NULL, Cer varbinary(MAX) NULL END ",
                  "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'PfxPassword' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Konfiguracja ADD PfxPassword nvarchar(50) NULL END ",
                     "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'User' AND COLUMN_NAME = 'MEPPassword' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE [dbo].[User] ADD MEPPassword nvarchar(50) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'AppName' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Konfiguracja ADD AppName nvarchar(50) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'ServiceEndpoint' AND COLUMN_NAME = 'Id' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN CREATE TABLE [dbo].[ServiceEndpoint]( 	[Id] [int] IDENTITY(1,1) NOT NULL, 	[ServiceId] [int] NULL,	[ServiceName] [nvarchar](100) NULL,	[Endpoint] [nvarchar](300) NULL, CONSTRAINT [PK_ServiceEndpoint] PRIMARY KEY CLUSTERED (	[Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] END  ",
                " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'ContractAccountCreateOut') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (1, N'ContractAccountCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractAccountCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractAccountCreate') "+
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (2, N'ContractAccountQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractAccountQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractAccountQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (3, N'ContractAccountRelationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractAccountRelationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractAccountRelationCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (4, N'ContractAccountUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractAccountUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractAccountUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (5, N'ContractObjectCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractObjectCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractObjectCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (6, N'ContractObjectQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractObjectQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractObjectQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (7, N'DebtorDepositListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DebtorDepositListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DebtorDepositListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (8, N'DepartmentDictionaryQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DepartmentDictionaryQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DepartmentDictionaryQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (9, N'DepositListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DepositListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DepositListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 10, N'DocumentBailiffListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentBailiffListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentBailiffListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 11, N'DocumentCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 12, N'DocumentDebtStateUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentDebtStateUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentDebtStateUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 13, N'DocumentListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 14, N'DocumentReductionDebtOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentReductionDebtOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentReductionDebt') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 15, N'DocumentReferenceUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentReferenceUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentReferenceUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 16, N'DocumentReverseCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentReverseCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentReverseCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 17, N'DocumentUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 18, N'InstalmentPlanCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=InstalmentPlanCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:InstalmentPlanCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 19, N'InstalmentPlanDeactivateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=InstalmentPlanDeactivateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:InstalmentPlanDeactivate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 20, N'InstalmentPlanVerifyOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=InstalmentPlanVerifyOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:InstalmentPlanVerify') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 21, N'PartnerCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PartnerCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PartnerCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 22, N'PartnerQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PartnerQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PartnerQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 23, N'PartnerUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PartnerUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PartnerUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 24, N'PaymentCancellationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentCancellationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentCancellationCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 25, N'PaymentClarificationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentClarificationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentClarificationCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 26, N'PaymentClarificationZDOBCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentClarificationZDOBCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentClarificationZDOBCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 27, N'PaymentClarificationsQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentClarificationsQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentClarificationsQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 28, N'PaymentListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 29, N'PaymentReservationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentReservationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentReservationCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 30, N'PostingDataPrepareOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PostingDataPrepareOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PostingDataPrepare') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 31, N'PostingStatusQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PostingStatusQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PostingStatusQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 32, N'RelationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=RelationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:RelationCreate') " +
                "END ",    // wer 3.4
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Dokument' AND COLUMN_NAME = 'referencja' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Dokument ADD referencja varchar(1024) NULL, 	tekst varchar(1024) NULL,  	IDZadanieKsiegowania varchar(30) NULL, 	DataStanu datetime NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'SAPSlownikRozlicz' AND COLUMN_NAME = 'SAPSlownikRozlicz_Id' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN CREATE TABLE [dbo].[SAPSlownikRozlicz]( 	[SAPSlownikRozlicz_Id] [int] IDENTITY(1,1) NOT NULL, 	[kasabank] [int] NULL,	[nazwa] [varchar](50) NULL,	[rodzaj] [int] NULL, CONSTRAINT [PK_SAPSlownikRozlicz] PRIMARY KEY CLUSTERED (	[SAPSlownikRozlicz_Id] ASC)) END  " ,
                 " IF NOT EXISTS (SELECT NULL FROM [dbo].[SAPSlownikRozlicz] WHERE [rodzaj] = 2 ) BEGIN " +
" SET IDENTITY_INSERT [dbo].[SAPSlownikRozlicz] ON  " +
" INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (1, 1, N'Dochody', 1)   " +
" INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (2, 1, N'Wydatki', 2)	   " +
" INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (3, 1, N'Sumy na zlecenia', 3)  " +
" INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (4, 1, N'FPP', 4) " +
" INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (5, 2, N'Dochody', 2) " +
" SET IDENTITY_INSERT [dbo].[SAPSlownikRozlicz] OFF END ",
                  " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'PaymentListQueryIn') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 33, N'PaymentListQueryIn', N'https://sapwitl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentListQuery') " +
                "END ",
                 // wer 3.5
                " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'GetCaseRegistryTypesOut') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (34, N'GetCaseRegistryTypesOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=DictionariesSOAPSender&receiverParty=&receiverService=&interface=GetCaseRegistryTypesOut&interfaceNamespace=urn:ms.gov.pl:Dictionaries:FI') "+
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (35, N'GetCourtsOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=DictionariesSOAPSender&receiverParty=&receiverService=&interface=GetCourtsOut&interfaceNamespace=urn:ms.gov.pl:Dictionaries:FI') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (36, N'GetDepartmentsOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=DictionariesSOAPSender&receiverParty=&receiverService=&interface=GetDepartmentsOut&interfaceNamespace=urn:ms.gov.pl:Dictionaries:FI') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (37, N'ManageAccountOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ManageAccountOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ManageAccount') " +
                " END " ,
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'SAPRepertorium' AND COLUMN_NAME = 'typSad' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN " +
                " CREATE TABLE dbo.Tmp_SAPRepertorium ( 	id int NOT NULL IDENTITY (1, 1),	kod varchar(50) NOT NULL,	SymbolRodzajPrzedmiotu varchar(4) NULL,	typSad varchar(2) NULL 	)  ON [PRIMARY] " +
                " ALTER TABLE dbo.Tmp_SAPRepertorium SET (LOCK_ESCALATION = TABLE) " +
                " SET IDENTITY_INSERT dbo.Tmp_SAPRepertorium OFF " +
                " IF EXISTS(SELECT * FROM dbo.SAPRepertorium) " +
                " EXEC('INSERT INTO dbo.Tmp_SAPRepertorium (kod, SymbolRodzajPrzedmiotu) " +
                " SELECT kod, SymbolRodzajPrzedmiotu FROM dbo.SAPRepertorium WITH (HOLDLOCK TABLOCKX)') " +
                " DROP TABLE dbo.SAPRepertorium " +
                " EXECUTE sp_rename N'dbo.Tmp_SAPRepertorium', N'SAPRepertorium', 'OBJECT'  " +
                " ALTER TABLE dbo.SAPRepertorium ADD CONSTRAINT PK_SAPRepertorium_1 PRIMARY KEY CLUSTERED ( id 	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] " +
                " END " ,
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'SAPSad' AND COLUMN_NAME = 'WazneOd' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN " +
                 "  ALTER TABLE dbo.SAPSad ADD " +
                 "  WazneOd datetime NULL, " +
                 "  WazneDo datetime NULL  " +
                 " END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'SAPPwdExpPeriod' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN " +
                "  ALTER TABLE dbo.Konfiguracja ADD   SAPPwdExpPeriod int NULL " +
                "  END ",
                " Update Konfiguracja set SAPPwdExpPeriod = isnull(SAPPwdExpPeriod, 7) ",
                  // ver 3.6
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'KnsKsiegi' AND COLUMN_NAME = 'ksGrzFPPMap' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN " +
                "   ALTER TABLE dbo.KnsKsiegi ADD ksGrzFPPMap int NULL " +
                "  END ",
                "  IF  (select count(*) from SAPKodyOpr where operacjaGlowna = 'N010' and kod = '0140') = 0  "+
                "  BEGIN " +
                "   insert into SAPKodyOpr(kod, nazwa, grzywnakoszty, samoistna, operacjaGlowna, oznaczenieOpGlownej, id) values ( '0140','Nawiązka SP','g', '', 'N010',  'Przypis Nawiązka SP','N0100140') " +
                "  END ",
                "  IF  (select count(*) from SAPKodyOpr where operacjaGlowna = 'N020' and kod = '0140') = 0  "+
                "  BEGIN " +
                "   insert into SAPKodyOpr(kod, nazwa, grzywnakoszty, samoistna, operacjaGlowna, oznaczenieOpGlownej, id) values ( '0140','Nawiązka SP','g', '', 'N020',  'Odpis Nawiązka SP','N0200140') " +
                "  END ",
                  // ver 3.7
                " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'ImportContentSystemData') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (38, N'ImportContentSystemData', N'https://sapwitl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ISMSender&receiverParty=&receiverService=&interface=ImportContentSystemDataOut&interfaceNamespace=urn:ms.gov.pl:ISM:ImportContentSystemData') "+
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (39, N'GetStatusContentSystemData', N'https://sapwitl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ISMSender&receiverParty=&receiverService=&interface=GetStatusContentSystemDataOut&interfaceNamespace=urn:ms.gov.pl:ISM:GetStatusContentSystemData') " +
                " END "


            };

            return commnadsList;
        }
        public static string getDBVersion()
        {
            try
            {
                using (RupIntegratorEntities dbContext = new RupIntegratorEntities())
                {
                    string dbversion = dbContext.ExecuteStoreQuery<string>("Select dbversion from Konfiguracja").FirstOrDefault();
                    return dbversion;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd połączenia z bazą danych Integratora " + ex.Message);
                return null;
            }
        }

    }



}
        
        
    

