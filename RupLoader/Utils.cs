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

    

   

    class Utils
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

    }



}
        
        
    

