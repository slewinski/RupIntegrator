using Ex2PscdInterface.Ex2PscdContractObjectCreateOutService;
using MessageSignature;
using SapPOHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RupLoader
{
    public class JobManager
    {
        private List<rStruct> lst = new List<rStruct>();
      
        private DataTable dtTbl = new DataTable();
        private void setSAPConnectionParams()
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                User usr = context.User.Where(a => a.Id == UserInfo.Id).FirstOrDefault();
                setSAPConnectionParams(usr);
            }
        }


        private void setSAPConnectionParams(User u)
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

                List<ServiceEndpoint> l = context.ServiceEndpoint.ToList();
                if (l != null)
                    foreach (ServiceEndpoint s in l)
                    {
                        lst.Add(new KeyValuePair<string, string>(s.ServiceName, s.Endpoint));
                    }

                ZSRKRequestHelper.ServiceMapping = lst;
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


        public JobManager()
        {
            setSAPConnectionParams();

        }

        private string getWydzIds(string s, int knfId)
        {
            string outstring = "";
            List<string> lst = new List<string>();
            lst = s.Split(',').ToList();
            lst = lst.Select(x => x).Distinct().ToList();
            foreach (string ss in lst)
            {
                if (!String.IsNullOrWhiteSpace(ss))
                {
                    long i;
                    i = Convert.ToInt32(ss);
                    if ((i / 10000000) as long? == (long)knfId)
                    {
                        outstring += (outstring.Length > 0 ? "," : "") + (i % 10000000).ToString();

                    }


                }


            }
            return outstring;
        }

        private Dictionary<string, string> getArguments(string arglst)
        {
            Dictionary<string, string> lst = new Dictionary<string, string>();
            List<string> ls = arglst?.Split(',').ToList();
            if (ls != null)
            {
                foreach (string s in ls)
                {
                    int charLocation = s.IndexOf("=");
                    if (charLocation > 0)
                    {
                        string name = s.Substring(0, charLocation).Trim().ToLower();
                        string value = s.Substring(charLocation + 1).Trim().ToLower();
                        lst.Add(name, value);
                    }
                }

            }
            return lst;
        }

        private string getArg(Dictionary<string, string> argLst, string argname)
        {
            string s = argLst.Where(a => a.Key == argname).Select(a => a.Value).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(s))
                return "";
            return s;

        }

        private bool compareDlu(Dluznik dl, Dluznik dlx)
        {
            if (!String.IsNullOrWhiteSpace(dl.Nip) && !String.IsNullOrWhiteSpace(dlx.Nip) && dl.Nip == dlx.Nip) return true;
            if (dl.Imie.DoTrim() == dlx.Imie.DoTrim() && dl.Nazwisko.DoTrim() == dlx.Nazwisko.DoTrim() && dl.Miejscowosc.DoTrim() == dlx.Miejscowosc.DoTrim() && dl.KodPocztowy.DoTrim() == dlx.KodPocztowy.DoTrim() && dl.Ulica.DoTrim() == dlx.Ulica.DoTrim() && dl.NrDomu.DoTrim() == dlx.NrDomu.DoTrim() && dl.NrMieszkania.DoTrim() == dlx.NrMieszkania.DoTrim())
                return true;
            return false;

        }


        private bool proceedSygnaturaRyczalty()
        {
       
            rStruct therow = null;
            bool czyblad = false;
            string mojeJG;


            using (RupIntegratorEntities dbContext = new RupIntegratorEntities())
            {
                Utils.LogWriter( "Weryfikacja sądów orzekających ");
                Konfiguracja konf = dbContext.Konfiguracja.FirstOrDefault();
                mojeJG = (String.IsNullOrWhiteSpace(konf.StanowiskoFin) ? konf.JednostkaGospodarcza.Trim() : konf.StanowiskoFin);

                foreach (rStruct r in lst)
                {
                    // sprawdzenie czy jest numer sapowy
                    Utils.LogWriter("Weryfikacja sądów orzekających dla " + r.Sygnatura);
                    therow = r;
                    if (r.IdSadOrzek <= 0) continue;
                    KuratSad ks = dbContext.KuratSad.Where(a => a.dbname == r.SygnDbName && a.srvname == r.SygnSrvName && a.Sad_Id == r.IdSadOrzek).FirstOrDefault();
                    if (ks != null && !String.IsNullOrWhiteSpace(ks.SAPSad_Id))
                        r.SapSad = ks.SAPSad_Id;
                    else
                    {//pobierz sąd
                        r.status = -1; // sąd do pobrania
                        Utils.LogWriter("Nie znaleziono sądu orzzekającecgo. Konieczne uruchominie z poziomu interfejsu użytkownika");
                    }


                }

            
                int step = 0;



                try
                {
                    czyblad = false;   // weyfikacja  sygnatur

                    //startProgressWindow();
                   Utils.LogWriter( "Weryfikacja sygnatur... ");
                    
                    foreach (rStruct r in lst)
                    {
                        int rok;
                        step = 0;
                        int nr;
                        string repOryg;
                        string wydzial, repert;
                        string ans;
                        string outsad = "";
                        therow = r;
                        if (therow.status == -1) // niezidentyfikowane sądy opuszczamy
                            continue;
                        Utils.LogWriter("Weryfikacja sygnatury " + r.Sygnatura);
                        
                        if (!String.IsNullOrWhiteSpace(r.SRepertorium) && !String.IsNullOrWhiteSpace(r.SWydzial) && !String.IsNullOrWhiteSpace(r.SNumer) && !String.IsNullOrWhiteSpace(r.SRok)) continue;

                        if (/*cbObce.Checked && */ !String.IsNullOrWhiteSpace(r.SapSad) && !String.IsNullOrWhiteSpace(r.Sygnatura) && r.SapSad != mojeJG)   // jeśli nielusta i obca 
                        {
                            r.Sygnatura = Utils.getTechSygn(r.IdCofDB);
                            if (!String.IsNullOrWhiteSpace(r.Sygnatura) && r.Sygnatura.Length > 5 && r.Sygnatura.Substring(4, 1) == " ")
                                r.Sygnatura = r.Sygnatura.Substring(4).Trim();
                            r.SapSad = mojeJG;
                        }
                        /*
                         else
                             if (cbEmpty.Checked && (String.IsNullOrWhiteSpace(r.Sygnatura) || r.status == -1))
                                 outr.Sygnatura = getTechSygn(r.IdCofDB);
                             else
                                 outr.Sygnatura = r.SapSad + " " + r.SWydzial + r.SRepertorium + " " + r.SNumer + "/" + (!String.IsNullOrWhiteSpace(r.SRok) && r.SRok.Trim().Length >= 4 ? r.SRok.Substring(2, 2) : "");

                         */

                        r.msg = "";
                        r.status = 0;
                        step = 1;
                        ans = Utils.ParseSygn(String.IsNullOrWhiteSpace(r.Sygnatura) ? "" : r.Sygnatura.ToUpper(), out wydzial, out repert, out nr, out rok, out repOryg, out outsad, r.SapSad);
                        step = 111;
                        if (!String.IsNullOrWhiteSpace(ans))
                        {
                            r.msg += ans;
                            r.status = -1;
                            czyblad = true;
                        }
                        else
                        {
                            r.SWydzial = wydzial;
                            r.SRepertorium = repert;
                            r.SNumer = nr.ToString();
                            r.SRok = rok.ToString();
                            if (rok <= 0)
                            {
                                r.msg += " Rok sprawy nie może być zerowy ";
                                r.status = -1;
                                czyblad = true;
                            }
                            if (!String.IsNullOrWhiteSpace(outsad)) r.SapSad = outsad;
                            step = 11;
                            SAPRepertorium rep = dbContext.SAPRepertorium.Where(a => a.kod == repert).FirstOrDefault();
                            if (rep == null)
                            {
                                r.msg += "W słowniku brak takiego repertorium - rodzaju przedmiotu";
                                r.status = -1;
                                czyblad = true;
                            }
                            else
                                r.SRodzajPrzedm = rep.SymbolRodzajPrzedmiotu;
                            step = 2;
                            string typsad = "SR";
                            if (String.IsNullOrWhiteSpace(r.SapSad))
                            {
                                r.msg += "Sąd wymaga wyboru ze słownika w systemie kuratorskim";
                                r.status = -1;
                                czyblad = true;
                            }
                            else
                                switch (r.SapSad.Substring(0, 1).ToUpper())
                                {
                                    case "5":
                                    case "4":
                                        typsad = "SR";
                                        break;
                                    case "3":
                                        typsad = "SO";
                                        break;
                                    case "2":
                                        typsad = "SA";
                                        break;
                                    default:
                                        break;
                                }
                            step = 3;
                            SAPRodzajSprawy sps = dbContext.SAPRodzajSprawy.Where(a => a.repertorium == repert && a.typSad == typsad).FirstOrDefault();
                            step = 5;
                            if (sps == null)
                            {
                                r.msg += "W słowniku brak takiego rodzaju sprawy";
                                r.status = -1;
                                czyblad = true;
                            }
                            else
                                r.SRodzaj = sps.kod;
                        }
                        step = 99;
                        if (r.status != -1) r.status = 1;
                    }

                    if (czyblad == true)
                    {
                        Utils.LogWriter("Walidacja sygnatur zakończyła się błędem. Poprawne pozycje zostaną zweryfikowane w ZSRK"); //return;
                    }

                 
                }

                catch (Exception ex)
                {
                    Utils.LogWriter("Błąd walidacji sygnatur " + step.ToString());
                    Utils.LogWriter(ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "") + (therow != null ? " " + therow.Sygnatura : ""));
                    return false;

                }

            



                Konfiguracja knf = RupDatabase.theContext.Konfiguracja.FirstOrDefault();
                try
                {
                    Utils.LogWriter( "Weryfikacja sygnatury w systemie ZSRK...");
                    
                    //weryfikacja sygnatur wde SAP'ie i ew założenie;
                    foreach (rStruct r in lst)
                    {
                        if (r.status == -1)
                            continue;
                        Double nop = 0;
                        if (!String.IsNullOrWhiteSpace(r.msg) && r.msg.Length > 15 && r.msg.Length < 23 && Double.TryParse(r.msg, out nop)) continue;
                        Utils.LogWriter("Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura);
                        therow = r;



                        SygnaturaTworzenie sygnqry = Utils.setupSygnStruct(r, knf);
                        if (sygnqry == null) { r.status = -1; continue; }
                        string Przedmiotumowy = Utils.verifySygnatura(sygnqry);
                        if (!String.IsNullOrWhiteSpace(Przedmiotumowy))
                        {
                            Utils.LogWriter("Odczyt sygnatury lokalnie " + r.Sygnatura + " OK nr przedmiotu" + Przedmiotumowy);
                            r.msg += Przedmiotumowy;
                            r.status = 1;
                            continue;
                        }
                        ContractObjectCreateResponse anssygn = ZSRKRequestHelper.ZalozSygnature(sygnqry);
                        if (anssygn != null)
                        {
                            if (anssygn.Sygnatura != null)
                            {
                                if (String.IsNullOrWhiteSpace(anssygn.Sygnatura.IDPrzedmiotuUmowy))
                                {
                                    r.msg += "Błąd podczas zakładania/wyszukiwania sygnatury ";
                                    if (anssygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0 )
                                    {
                                        r.msg = anssygn.Komunikaty[0].Komunikat1 + " " +r.msg; 
                                    }
                                    r.status = -1;
                                    czyblad = true;

                                }
                                else
                                {
                                    Utils.LogWriter("Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura + " OK nr przedmiotu" + anssygn.Sygnatura.IDPrzedmiotuUmowy);
                                    r.msg += anssygn.Sygnatura.IDPrzedmiotuUmowy;
                                    r.status = 1;
                                    Utils.addSygnatura(sygnqry, r.Sygnatura, anssygn.Sygnatura.IDPrzedmiotuUmowy);
                                }



                            }

                            else
                            {
                                if (anssygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0 )
                                {
                                    r.msg += anssygn.Komunikaty[0].Komunikat1 + " Błąd podczas zakładania/wyszukiwania sygnatury ";
                                    Utils.LogWriter("Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura + " Błąd " + anssygn.Komunikaty[0].Komunikat1);
                                    r.status = -1;
                                    czyblad = true;
                                }

                            }

                        }
                        else
                        {
                                continue;
                         
                        }


                    }
                    if (czyblad == true)
                    {
                       Utils.LogWriter("Sprawdzenie sygnatur w ZSRK zakończyła się błędem");
                        return false;
                    }

                } // try
                catch (Exception ex)
                {
                    Utils.LogWriter(ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "") + (therow != null ? " " + therow.Sygnatura : ""));
                    return false;

                }

            }
      

            return true;

        }


        private bool proceedSearch(SchedulerItem task)
        {
            string thekey, wydzial, repertorium, mode;
            int numer = 0 , rok = 0 , skipkns = 0 ;
            RL_Konfig knf;
            string idList;
            int tmp;
        
            SqlDataReader rdr = null;
            SqlConnection con = null;
            SqlCommand storedProcCommand;
            List<string> l = new List<string>();
            Dictionary<string, string> argLst;
            string arg;
            try
            {
                // Open connection to the database

                 knf = task.RL_Konfig;
                if (knf == null)
                {

                    Utils.LogWriter("Brak zdefiniowanego dostępu do bazy danych ");
                    return false;
                }
                if (String.IsNullOrWhiteSpace(knf.sp_name))
                {
                    Utils.LogWriter("Nie zdefiniowano procedury składowanej do obsługi ryczłtów");
                    return false;
                }

                argLst = getArguments(task.Arguments);
                thekey = (getArg(argLst, "@key") ?? "").Trim();
                wydzial = (getArg(argLst, "@wydzial") ?? "").Trim();
                repertorium = (getArg(argLst, "@repertorium") ?? "").Trim();
                if (int.TryParse((getArg(argLst, "@numer") ?? "").Trim(), out tmp))
                {
                    numer = tmp;

                }
                if (int.TryParse((getArg(argLst, "@rok") ?? "").Trim(), out tmp))
                {
                    rok = tmp;
                }
                if (int.TryParse((getArg(argLst, "@skipkns") ?? "").Trim(), out tmp))
                {
                    skipkns = tmp;
                }
                idList = (getArg(argLst, "@idList") ?? "").Trim();
                mode = (getArg(argLst, "@mode") ?? "").Trim();
               
               l = idList.Split(',').Distinct().ToList();

                string ConnectionString = ConfigurationManager.ConnectionStrings["RupLoader.Properties.Settings.RupDB"].ConnectionString;
                con = new SqlConnection(ConnectionString);
                //con.Open();
                if (knf == null)
                    knf = (from c in RupDatabase.theContext.RL_Konfig select c).FirstOrDefault();
                if (String.IsNullOrWhiteSpace(knf.sp_name))
                    switch (knf.typDB)
                    {
                        case 0: // currenda
                            storedProcCommand = new SqlCommand("sp_RozpoznajPrzelewCR", con);
                            break;
                        case 1: // Zeto
                            storedProcCommand = new SqlCommand("sp_RozpoznajPrzelew", con);
                            break;
                        case 2: // Zeto
                            storedProcCommand = new SqlCommand("sp_RozpoznajPrzelewOR", con);
                            break;
                        case 3: // Zeto
                            storedProcCommand = new SqlCommand("sp_RozpoznajPrzelewAL", con);
                            break;
                        default:
                            return false;
                    }
                else
                    storedProcCommand = new SqlCommand(knf.sp_name, con);


                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(knf.srvAlias) ? knf.srvName : knf.srvAlias));
                storedProcCommand.Parameters.Add("@dbname", knf.DbName);
                storedProcCommand.Parameters.Add("@key", thekey);
                storedProcCommand.Parameters.Add("@wydzial", wydzial);
                storedProcCommand.Parameters.Add("@repertorium", repertorium);
                storedProcCommand.Parameters.Add("@numer", numer);
                storedProcCommand.Parameters.Add("@rok", rok);
                storedProcCommand.Parameters.Add("@skipkns", skipkns);
                storedProcCommand.Parameters.Add("@idList", getWydzIds(idList, knf.id));
                storedProcCommand.Parameters.Add("@mode", String.IsNullOrWhiteSpace(RunMode.data) ? "" : RunMode.fileName.Replace("/", ""));

                storedProcCommand.CommandTimeout = 600;
                storedProcCommand.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter();

                da.SelectCommand = storedProcCommand;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dtTbl.Rows.Count == 0) dtTbl = dt.Clone();
                    foreach (DataRow dr in dt.Rows)
                    {
                        dtTbl.Rows.Add(dr.ItemArray);
                        l.Add(((dr["IdSprawy"] as int?) + knf.id * 10000000).ToString());

                    }
                }


            }
            catch (Exception ex)
            {
                // Print error message
                
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con.State == ConnectionState.Open)
                    con.Close();
            }


            //return string.Join(",", l.Distinct().ToArray());
            return true;
        }





        private bool proceedRyczalty(SchedulerItem task)
        {


            string retcode = "";
            SqlDataReader rdr = null;
            SqlConnection con = null;
            SqlCommand storedProcCommand;
            DataRow dr_save = null;

            DateTime dodnia = DateTime.Today; 
            DateTime od = dodnia.AddDays(-28);
            String filter = "DWN";
            int index = 0;
            Dictionary<string, string> argLst;
            string arg;

            if (DateTime.Today.Day > 20)
                dodnia = DateTime.Today;
            else
            {
                dodnia = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                od = new DateTime(dodnia.Year, dodnia.Month, 1); 
            }
            try
            {

                using (RupIntegratorEntities dbContext = new RupIntegratorEntities())
                {


                    RL_Konfig cnf = task.RL_Konfig;
                    if (cnf == null)
                    {

                      Utils.LogWriter( "Brak zdefiniowanego dostępu do bazy danych ");
                        return false;
                    }
                    string ConnectionString = ConfigurationManager.ConnectionStrings["RupLoader.Properties.Settings.RupDB"].ConnectionString;
                    con = new SqlConnection(ConnectionString);
                    //con.Open();
                    if (String.IsNullOrWhiteSpace(cnf.sp_name))
                    {
                        Utils.LogWriter("Nie zdefiniowano procedury składowanej do obsługi ryczłtów");
                        return false;
                    }
                    storedProcCommand = new SqlCommand(cnf.sp_name, con);
                    argLst = getArguments(task.Arguments);

                    storedProcCommand.CommandType = CommandType.StoredProcedure;
                    storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(cnf.srvAlias) ? cnf.srvName : cnf.srvAlias));
                    storedProcCommand.Parameters.Add("@dbname", cnf.DbName);
                    if (!String.IsNullOrWhiteSpace(arg = getArg(argLst, "@dataod")))
                        {
                            DateTime _od;
                        if (DateTime.TryParse(arg, out _od))
                        {
                            od = _od;
                        }
                        }
                    storedProcCommand.Parameters.Add("@dataod", od);

                    if (!String.IsNullOrWhiteSpace(arg = getArg(argLst, "@datado")))
                    {
                        DateTime _do;
                        if (DateTime.TryParse(arg, out _do))
                        {
                            dodnia = _do;
                        }
                    }
                    storedProcCommand.Parameters.Add("@datado", dodnia);

                    if (!String.IsNullOrWhiteSpace(arg = getArg(argLst, "@what")))
                    {
                        filter = arg;
                    }
                    storedProcCommand.Parameters.Add("@what", filter);
                    storedProcCommand.Parameters.Add("@IdZespolu", cnf.WSLogon);


                    storedProcCommand.CommandTimeout = 600;
                    storedProcCommand.Connection = con;
                    SqlDataAdapter da = new SqlDataAdapter();
                    Cursor.Current = Cursors.WaitCursor;
                    da.SelectCommand = storedProcCommand;
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    Utils.LogWriter( "Odczyt ryczałtów - łączenie z bazą... ");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    int i = 0;

                    int j = dt.Rows.Count;
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr_save = dr;

                            rStruct r = new rStruct();
                            r.lp = ++index;
                            Utils.LogWriter( "Odczyt ryczałtu  " + (++i).ToString() + " z " + j.ToString() + " " + dr["Sygnatura"]);
                            r.ImieNazwisko = dr["ImieNazwisko"] as string;
                            r.IdKuratora = dr["IdKuratora"] is DBNull ? 0 : Convert.ToInt32(dr["IdKuratora"]);
                            r.SadOrzek = dr["SadOrzek"] as string;
                            r.IdSadOrzek = dr["IdSadOrzek"] is DBNull ? 0 : Convert.ToInt32(dr["IdSadOrzek"]);
                            r.Sygnatura = dr["Sygnatura"] as string;
                            r.NrRachunku = dr["NrRachunku"] as string;
                            r.DataWydZarz = Convert.ToDateTime(dr["DataWydZarz"]);
                            r.DataWplZarz = Convert.ToDateTime(dr["DataWplZarz"]);
                            r.PowoDodRozl = dr["PowoDodRozl"] as string;
                            r.TypRozl = dr["TypRozl"] as string;
                            r.IdListyPlac = dr["IdListyPlac"] as string;
                            r.Skladnik = dr["Skladnik"] as string;
                            r.Kwota = Convert.ToDecimal(dr["Kwota"]);
                            r.ZwKosztDojSkladnik = Convert.ToDecimal(dr["ZwKosztDojSkladnik"]);
                            r.ZwKosztDojKWSkladnik = Convert.ToDecimal(dr["ZwKosztDojKWSkladnik"]);
                            r.LWywiadow = Convert.ToInt32(dr["LWywiadow"]);
                            r.LNadzorow = Convert.ToInt32(dr["LNadzorow"]);
                            r.WywiadDaneOsob = dr["WywiadDaneOsob"] as string;
                            r.Uwagi = dr["Uwagi"] as string;
                            r.StatusDokumentu = dr["StatusDokumentu"] as string;
                            r.SygnDbName = cnf.DbName;
                            r.SygnSrvName = cnf.srvName;
                            r.IdCofDB = cnf.id;
                            r.WydatekIncydantalny = dr["WydatekIncydantalny"] is DBNull ? 0 : Convert.ToDecimal(dr["WydatekIncydantalny"]);
                            try
                            {
                                r.RodzWypl = dr["RodzWypl"] as string;
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            try
                            {
                                r.PotracZaliczki = dr["PotracZaliczki"] is DBNull ? 0 : Convert.ToDecimal(dr["PotracZaliczki"]);
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            try
                            {
                                r.ZwrotKosztKwt2 = dr["ZwrotKosztKwt2"] is DBNull ? 0 : Convert.ToDecimal(dr["ZwrotKosztKwt2"]);
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            try
                            {
                                r.ZwrotKosztSkladnik2 = dr["ZwrotKosztSkladnik2"] as string;
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            try
                            {
                                r.ProcDofin = dr["ProcDofin"] is DBNull ? 0 : Convert.ToDecimal(dr["ProcDofin"]);
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            KuratSad ks = dbContext.KuratSad.Where(a => a.dbname == r.SygnDbName && a.srvname == r.SygnSrvName && a.Sad_Id == r.IdSadOrzek).FirstOrDefault();
                            if (ks != null)
                                r.SapSad = ks.SAPSad_Id;
                            KuratMap kur = dbContext.KuratMap.Where(a => a.DbId == r.IdKuratora && a.typPartner == RupDatabase.typPartner && a.servername == r.SygnSrvName && a.dbname == r.SygnDbName).FirstOrDefault();
                            if (kur != null)
                                r.NumerKuratora = kur.SAPId;
                            lst.Add(r);

                        }
                    }
              
                }
            }

            catch (Exception ex)
            {
                
                // Print error message
               Utils.LogWriter( ex.Message + " " + ((ex.InnerException == null) ? "" : ex.InnerException.Message) + (dr_save != null ? dr_save["Sygnatura"] as string : ""));
                return false;
            }







            return true;
       
    }

        public bool proceedSearchSygnatury()
        {
            // sygnatury i wiązanie 




            string typSad = Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 4000 ? "SR" : (Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 3000 ? "SO" : "SA");
            string typSadOryg = typSad;
            if (!String.IsNullOrWhiteSpace(RupDatabase.theConfig.StanowiskoFin) && Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) < 4000 && typSad != "SA")
            {
                typSad = "SF";
                typSadOryg = "SR";
            }
            Sprawa spr;
            Dluznik dl;

            try
            {
                foreach (DataRow theRow in dtTbl.Rows)
                {

                    dl = new Dluznik();
                    if (!String.IsNullOrEmpty(theRow["typPartnera"].ToString().Trim()))
                        dl.FizPraw = theRow["typPartnera"].ToString();
                    else
                        dl.FizPraw = "";
                    dl.Imie = theRow["Nazwa2"]?.ToString();
                    dl.Nazwisko = theRow["Nazwa1"]?.ToString();
                    dl.KnsDluz_Id = Convert.ToInt32(theRow["IdStrony"].ToString());
                    dl.IdSrcDane = Convert.ToInt64(theRow["idDanychStrony"].ToString());
                    if (dl.FizPraw == "X") // jesli osoba prawna - podziel nazwę 
                    {
                        dl.Imie = theRow["Nazwa1"]?.ToString();
                        dl.Nazwisko = theRow["Nazwa2"]?.ToString();

                        if (!String.IsNullOrEmpty(dl.Nazwisko))
                        {
                            int spc = dl.Imie.LastIndexOf(' ');
                            if (spc > 0 && dl.Nazwisko[0] != ' ')
                            {
                                string tmp = dl.Imie.Substring(spc + 1);
                                if (tmp.Trim().Length > 0)
                                {
                                    dl.Imie = dl.Imie.Substring(0, spc);
                                    dl.Nazwisko = tmp.Trim() + dl.Nazwisko;

                                }

                            }
                            dl.Nazwisko = dl.Nazwisko.Trim();
                        }


                    }

                    if (dl.Nazwisko.Length > 40)
                        dl.Nazwisko = dl.Nazwisko.Substring(0, 40);
                    if (dl.Imie.Length > 40)
                        dl.Imie = dl.Imie.Substring(0, 40);
                    // dodanie IBAN jeśli się

                    dl.Ulica = theRow["ulica"]?.ToString();
                    dl.NrDomu = theRow["nr_domu"]?.ToString();
                    dl.NrMieszkania = theRow["nr_mieszkania"]?.ToString();
                    dl.NrMieszkania = (!String.IsNullOrWhiteSpace(dl.NrMieszkania) ? dl.NrMieszkania = dl.NrMieszkania.Trim().Truncate(10) : "");
                    dl.NrDomu = (!String.IsNullOrWhiteSpace(dl.NrDomu) ? dl.NrDomu = dl.NrDomu.Trim().Truncate(10) : "");

                    dl.Pesel = theRow["pesel"]?.ToString().Trim();
                    if (string.IsNullOrEmpty(dl.Pesel))
                        dl.Pesel = null;
                    else
                    {
                        foreach (char c in dl.Pesel.Trim())
                        {
                            if (!Char.IsDigit(c))
                            {
                                dl.Pesel = null;
                                break;
                            }
                        }
                    }
                    dl.Nip = Utils.cleanNIP(theRow.ToString().Trim());
                    if (String.IsNullOrEmpty(dl.NrDomu))
                    {
                        if (dl.Ulica != null)
                        {
                            string s = dl.Ulica.Trim();
                            int ii = s.LastIndexOf(' ');
                            int jj;
                            if (ii > 3 && Int32.TryParse(s[ii + 1].ToString(), out jj))
                            {
                                dl.Ulica = s.Substring(0, ii).Trim();
                                dl.NrDomu = s.Substring(ii).Trim();
                                jj = 0;
                                jj = dl.NrDomu.IndexOf('/');
                                if (jj > 0 && jj < dl.NrDomu.Length - 1)
                                {
                                    dl.NrMieszkania = dl.NrDomu.Substring(jj + 1).Trim();
                                    dl.NrDomu = dl.NrDomu.Substring(0, jj).Trim();
                                }
                            }

                        }

                    }

                    if (String.IsNullOrEmpty(dl.NrMieszkania) && dl.NrDomu.ToLower().Contains('m'))
                    {
                        // wyj

                        dl.NrMieszkania = dl.NrDomu.Substring(dl.NrDomu.ToLower().IndexOf('m') + 1).Trim();
                        dl.NrDomu = dl.NrDomu.Substring(0, dl.NrDomu.ToLower().IndexOf('m')).Trim();

                    }



                    dl.KodPocztowy = theRow["kod"]?.ToString().Trim();
                    if (dl.KodPocztowy.Length == 5 && !dl.KodPocztowy.Contains("-"))
                        dl.KodPocztowy = dl.KodPocztowy.Substring(0, 2) + "-" + dl.KodPocztowy.Substring(2, 3);
                    dl.Miejscowosc = theRow["miejscowosc"]?.ToString();
                    {
                        string kk = theRow["kraj"]?.ToString().Trim().ToUpper();
                        if (kk != "PL")
                        {
                            SAPKodKraju kdkr;

                            kdkr = (from m in RupDatabase.theContext.SAPKodKraju
                                    where m.kraj.ToUpper() == kk
                                    select m).FirstOrDefault();
                            if (kdkr != null)
                            {
                                dl.KluczKraju = kdkr.kod;

                            }
                            else
                            {
                                dl.KluczKraju = "PL";

                            }
                        }
                        else
                            dl.KluczKraju = kk;

                    }

                    dl.Iban = theRow["IBAN"]?.ToString();
                    dl.RBN = theRow["RBN"]?.ToString();

                    if (string.IsNullOrEmpty(dl.RBN) || string.IsNullOrWhiteSpace(dl.RBN))
                    {
                        if (dl.FizPraw == "X")
                            dl.RBN = "08";
                        else
                            dl.RBN = "09";

                    }
                    dl.SAPKontoPartnera = theRow["NumerPartnera"]?.ToString();







                    spr = new Sprawa();

                    spr.KnsSprawa_id = Convert.ToInt32(theRow["IdSprawy"]);
                    spr.KnsKsiega = Convert.ToInt32(theRow["Ksiega"] == null ? "0" : theRow["Ksiega"].ToString());
                    spr.KNSSadOrzek_id = null;
                    spr.Karta = theRow["OznKontaUmowy"]?.ToString().Trim();  // karta dłużnika
                    spr.SAPKontoUmowy = theRow["KontoUmowy"]?.ToString();
                    spr.SAPPrzedmiotUmowy = theRow["PrzedmiotUmowy"]?.ToString();

                    if (theRow["TypKontaUmowy"] != null && !String.IsNullOrEmpty(theRow["TypKontaUmowy"].ToString()))
                    {
                        spr.SAPTypKontaUmowy = theRow["TypKontaUmowy"].ToString();
                    }
                    else
                    {

                        spr.SAPTypKontaUmowy = "DO";
                    }



                    spr.SAPWydział = theRow["kodWydzial"]?.ToString().Trim();
                    spr.SAPRepertorium = theRow["repertorium"]?.ToString().Trim().ToUpper();
                    spr.Rok = Convert.ToInt32(theRow["rok"]?.ToString());
                    spr.Numer = Convert.ToInt32(theRow["nr"]?.ToString());
                    spr.SAPSadId = !String.IsNullOrEmpty(RupDatabase.theConfig.StanowiskoFin.DoTrim()) ? RupDatabase.theConfig.StanowiskoFin : RupDatabase.theConfig.JednostkaGospodarcza;
                    spr.Sygnatura = theRow["sygnatura"]?.ToString();
                    SAPRepertorium repertorzek = (from e in RupDatabase.theContext.SAPRepertorium
                                                  where e.kod.ToUpper() == spr.SAPRepertorium.ToUpper()
                                                  select e).FirstOrDefault();
                    if (repertorzek != null)
                    {
                        spr.SAPRodzajPrzedmiotuUmowy = repertorzek.SymbolRodzajPrzedmiotu;

                    }
                    if (spr.SAPRepertorium.Length > 0)
                    {
                        SAPRodzajSprawy rodzajSpr = (from f in RupDatabase.theContext.SAPRodzajSprawy where f.repertorium == spr.SAPRepertorium && f.typSad == typSadOryg orderby f.id select f).FirstOrDefault();
                        if (rodzajSpr != null)
                        {
                            spr.SAPRodzajSprawy = rodzajSpr.kod;

                        }
                    }
                    spr.SAPTomyAkt = "001";

                    // sprawdzamy czy mamy już taką sprawę
                    {
                        List<Sprawa> sprxL;
                        sprxL = RupDatabase.theContext.Sprawa.Include("Dluznik").Where(a => a.SAPSadId == spr.SAPSadId && a.SAPWydział == spr.SAPWydział && a.Rok == spr.Rok && a.Numer == spr.Numer && a.SAPRepertorium == spr.SAPRepertorium &&
                                                                        a.SAPPrzedmiotUmowy != null && a.SAPTypKontaUmowy == spr.SAPTypKontaUmowy).OrderByDescending(a => a.Id).ToList();
                        Sprawa sprx = (from x in sprxL
                                       where x.Dluznik.Any(t => t.KnsDluz_Id == dl.KnsDluz_Id && dl.SAPKontoPartnera != null)
                                       select x).FirstOrDefault();
                        if (sprx != null)
                        {
                            spr.SAPKontoUmowy = sprx.SAPKontoUmowy;
                            spr.SAPPrzedmiotUmowy = sprx.SAPPrzedmiotUmowy;
                            if (spr.SAPTypKontaUmowy == "KN")    // jeśli kns w tej sprawie.
                                dl.SAPKontoPartnera = sprx.Dluznik.FirstOrDefault().SAPKontoPartnera;
                            else
                                if ((!String.IsNullOrWhiteSpace(dl.Pesel) && (dl.Pesel == sprx.Dluznik.FirstOrDefault().Pesel)) || (!String.IsNullOrWhiteSpace(dl.Nip) && (dl.Nip == sprx.Dluznik.FirstOrDefault().Nip)) || compareDlu(dl, sprx.Dluznik.FirstOrDefault()))
                                dl.SAPKontoPartnera = sprx.Dluznik.FirstOrDefault().SAPKontoPartnera;
                        }

                    }
                    if (theRow["RelacjaKonta"] != null && !String.IsNullOrEmpty(theRow["RelacjaKonta"]?.ToString()))
                        spr.SAPRelacjaKontaUmowy = theRow["RelacjaKonta"].ToString().Trim();
                    else
                        switch (theRow["rola"].ToString().ToUpper())
                        {
                            case "POWÓD":
                            case "WNIOSKODAWCA":
                                spr.SAPRelacjaKontaUmowy = "01";
                                break;
                            case "OSKARŻONY":
                            case "UCZESTNIK":
                                spr.SAPRelacjaKontaUmowy = "02";
                                break;
                            case "POZWANY":
                                spr.SAPRelacjaKontaUmowy = "03";
                                break;
                            case "ŚWIADEK":
                                spr.SAPRelacjaKontaUmowy = "04";
                                break;

                            default:
                                spr.SAPRelacjaKontaUmowy = "99";
                                break;

                        }

                    // załozenie sygnatury
                    if (string.IsNullOrWhiteSpace(spr.SAPPrzedmiotUmowy))
                    {
                        ExportPI exp = new ExportPI();


                        string nrPartnera = (theRow["NumerPartnera"] != null ? theRow["NumerPartnera"].ToString() : "");
                        string nrKontaUmowy = (theRow["KontoUmowy"] != null ? theRow["KontoUmowy"].ToString() : "");
                        RunMode.silentMode = true;
                        Dokument dok = new Dokument();
                        spr.Dokument.Add(dok);
                        string s = exp.DoExport(dok, 3, true, null, string.IsNullOrWhiteSpace(nrPartnera) ? "" : nrPartnera, string.IsNullOrWhiteSpace(nrKontaUmowy) ? "" : nrKontaUmowy);
                        if (!String.IsNullOrWhiteSpace(s))
                        {
                            dok.SAPImportStatus = 1;

                        }
                        else
                        {
                            dok.SAPImportStatus = -1;

                        }
                        if (!String.IsNullOrWhiteSpace(spr.SAPPrzedmiotUmowy))
                        {
                            // sygnatura została założona
                            // dodanie dłużnika. 
                            if (dok.SAPImportStatus == 1 && !String.IsNullOrWhiteSpace(dl.SAPKontoPartnera))
                            {
                                // dodanie dłużnika do bazy.

                                Dluznik dluz = (from c in RupDatabase.theContext.Dluznik where c.IdSrcDane == dl.IdSrcDane && c.SAPKontoPartnera != null orderby c.Id descending select c).FirstOrDefault();
                                if (dluz != null)
                                {

                                    dl = dluz;
                                }

                                spr.Dluznik.Add(dl);
                            }
                            RupDatabase.theContext.Sprawa.AddObject(spr);
                            RupDatabase.theContext.SaveChanges();
                            Utils.LogWriter(" Dodano sygnaturę " + spr.Sygnatura + " " + spr.SAPPrzedmiotUmowy);
                        }


                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogWriter("proces został przerwany z powodu błędu " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return false;
            }
        } 
        
        public void ExecJob(int jobtype)
        {
            RunMode.silentMode = true;
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                List<SchedulerItem> tasks = context.SchedulerItem.Include("RL_Konfig").Where(j => j.SchedulerJob.JobType == jobtype).ToList();
                if (tasks != null)
                {
                    foreach (SchedulerItem task in tasks)
                    {
                        switch (task.RL_Konfig.rodzajDB)
                        {
                            case 2: // ryczalty kuratorskie
                                if (proceedRyczalty(task))
                                    proceedSygnaturaRyczalty();
                                break;
                            case 0:
                                if (proceedSearch(task))
                                    proceedSearchSygnatury();
                                
                                // rozpoznanwanie
                                break;


                        }  

                    }

                }


            }


        }

    }
}
