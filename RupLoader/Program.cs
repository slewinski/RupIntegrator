using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Configuration;
using SapPOHelper;

namespace RupLoader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RunMode.dbVersion = "3.7";
            string dbversion = Utils.getDBVersion();
            try
            {


                RupDatabase.theContext = new RupIntegratorEntities();
                RupDatabase.typPartner = 0;
                
                #if DEMO
                    DateTime dt;
                    if (RupDatabase.theConfig != null && !String.IsNullOrEmpty(RupDatabase.theConfig.OdpisFile) && DateTime.TryParse(RupDatabase.theConfig.OdpisFile, out dt))
                    {
                        if ( DateTime.Today > dt.AddDays(30))
                        {
                            MessageBox.Show("Przekroczono limit użycia dla wersji demontracyjnej, jeśli nadal chcesz korzystać z aplikacji zamów wersję pełną", "Wersja demonstracyjna");
                            Application.Exit();
                            return;

                        }


                        else
                        {
                            MessageBox.Show("Aplikacja w wersji demonstracyjnej. Możesz jej używać jeszcze przez " + (30 - (DateTime.Today - dt).TotalDays).ToString() + " dni", "Wersja demonstracyjna");

                        }
                    }
                    else
                    {
                        RupDatabase.theConfig.OdpisFile = DateTime.Today.ToString();
                        RupDatabase.theContext.SaveChanges();
                        MessageBox.Show("Aplikacja w wersji demonstracyjnej. Możesz jej używać jeszcze przez 30 dni");
                    }

                #endif
                //RupDatabase.jg = (RupDatabase.theConfig.StanowiskoFin == null) ? RupDatabase.theConfig.JednostkaGospodarcza : (RupDatabase.theConfig.StanowiskoFin.Trim().Length == 4) ? RupDatabase.theConfig.StanowiskoFin : RupDatabase.theConfig.JednostkaGospodarcza;
                // odczyt parametrów
                RunMode.CmdFileName = "CommandFile.txt"; 

                if (args.GetLength(0) > 0)
                {
                    // logowanie z aoo.config
                    string userName = string.Empty;
                    string userPwd = string.Empty;

                    Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                    try
                    {
                        userName = config.AppSettings.Settings["UserName"].Value.ToString();
                    }
                    catch { }
                    try
                    {
                        userPwd = config.AppSettings.Settings["UserPwd"].Value.ToString();
                    }
                    catch { }
                    
                    User chkUser = RupDatabase.theContext.User.Where(a => a.Username == userName && a.Pssword == userPwd && (a.deleted == false) && a.suspend == false).FirstOrDefault();
                    if (chkUser == null)
                    {
                        MessageBox.Show("Błędna nazwa użytkownika lub hasło w zbiorze konfiguracyjnym");
                        return;

                    }
                    if (chkUser.role == 1)
                    {
                        MessageBox.Show("Ten tryb wywołania nie jest właściwy dla administratora");
                        return;
                    }

                    if (dbversion != RunMode.dbVersion)
                    {
                        MessageBox.Show("Wersja bazy danych: \"" + dbversion + "\" jest niezgodna z wersją aplikacji \"" + RunMode.dbVersion + "\".\r\nNależy zalogować się na profilu administratora (uruchamiając bezośrednio  RupLoader.exe) i wykonać operację przebudowy bazy danych.");
                        Application.Exit();
                        return;
                    }
                    RupDatabase.theConfig = (from c in RupDatabase.theContext.Konfiguracja select c).FirstOrDefault();
                    RupDatabase.jg = (RupDatabase.theConfig.StanowiskoFin == null) ? RupDatabase.theConfig.JednostkaGospodarcza : (RupDatabase.theConfig.StanowiskoFin.Trim().Length == 4) ? RupDatabase.theConfig.StanowiskoFin : RupDatabase.theConfig.JednostkaGospodarcza;

                    UserInfo.Id = chkUser.Id;
                    UserInfo.role = chkUser.role;
                    UserInfo.MEPUser = chkUser.MEPUser;
                    UserInfo.Username = chkUser.Username;
                    UserInfo.MEPPassword = chkUser.MEPPassword;
                    // sprawdzenie przeterminowania hasła mep
                    if (RupDatabase.theConfig.SAPPwdExpPeriod > 0)
                    {

                        verifySAPPwd();


                    }

                    if (args[0].ToUpper().Trim() == "/KUR")
                    {
                        RupDatabase.typPartner = 0;
                        Application.Run(new RyczaltyKuratorskie());
                        return;
                    
                    }
                    if (args[0].ToUpper().Trim() == "/BIEG")
                    {
                        RupDatabase.typPartner = 1;
                        Application.Run(new RyczaltyKuratorskie());
                        return;

                    }
                    if (args[0].ToUpper().Trim() == "/ŁAW")
                    {
                        RupDatabase.typPartner = 2;
                        Application.Run(new RyczaltyKuratorskie());
                        return;

                    }
                    if (args[0].ToUpper().Trim() == "/ZDOB")
                    {
                       
                        Application.Run(new ZDOBAnalizer());
                        return;

                    }

                    RunMode.operation= Operations.Wyciag; // analiza zbioru wejściwego
                    RunMode.fileName = args[0];
                    int Id = System.Diagnostics.Process.GetCurrentProcess().Id;
                    if ( args.GetLength(0) > 1 )
                    {


                        RunMode.data = args[1];
                        RunMode.WinMode =  (args[args.GetLength(0) -1 ]).ToUpper(); 

                    }

                    /*
                    if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 0)
                    {
                        // Save arguments to file 
                        if (!File.Exists(RunMode.CmdFileName))
                        {
                            File.WriteAllText(RunMode.CmdFileName, RunMode.data);
                        
                        }
                        Application.Exit();
                        return;
                    }
                     * */
                    // FileListener work = new FileListener();
                    if (RunMode.fileName.ToUpper() == "/KNS")
                    {
                        RupFinder.RupFinder rf = new RupFinder.RupFinder();
                        rf.connStr = ConfigurationManager.ConnectionStrings["RupLoader.Properties.Settings.RupDB"].ConnectionString;
                        if (args.GetLength(0) == 2)
                            rf.inArg = args[1];

                        if (args.GetLength(0) == 3)
                        {
                            rf.mode = args[1];
                            rf.inArg = args[2];
                        }

                        Application.Run(rf);
                        return;
                    }
                    else
                        RunMode.wndHandler = new Recognizer();
                    Application.Run(RunMode.wndHandler);
                }
                else

                {
                    try
                    {
                        UserInfo.Id = 0; // inicjaloizacja użytkownika.
                        using (RupIntegratorEntities _context = new RupIntegratorEntities())
                        {
                          

                            if (_context.User.Count() > 0 && !RunMode.silentMode)
                            {
                                Logon lgn = new Logon();
                                lgn.Context = _context;
                                lgn.ShowDialog();
                                if (lgn.DialogResult == DialogResult.OK)
                                {
                                    UserInfo.Id = lgn.usr.Id;
                                    UserInfo.Username = lgn.usr.Username;
                                    UserInfo.role = lgn.usr.role;
                                    UserInfo.MEPUser = lgn.usr.MEPUser;
                                    UserInfo.MEPPassword = lgn.usr.MEPPassword;
                                    // sprawdzenie wersji
                                   
                                    while (dbversion != RunMode.dbVersion)
                                    {

                                        if (lgn.usr.role != 1)
                                        {
                                            MessageBox.Show("Wersja bazy danych: \"" + dbversion + "\" jest niezgodna z wersją aplikacji \"" + RunMode.dbVersion + "\".\r\nNależy zalogować się na profilu administratora i wykonać operację przebudowy bazy danych.");
                                            Application.Exit();
                                            return;
                                        }
                                        if (MessageBox.Show("Wersja bazy danych: \"" + dbversion + "\" jest niezgodna z wersją aplikacji \"" + RunMode.dbVersion + "\".\r\nSystem wymaga wykonania przebudowy bazy danych. Upewnij się, czy użykownik posiada uprawnienia do przebudowy bazy.\n\r Czy wykonać operację przebudowy bazy danych ? ", "Niezgodność wersji bazy danych", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                        {
                                            MessageBox.Show("Użyj funkcji przebudowy bazy danych dostępnej w menu administratora - Konfiguracja->Przebudowa bazy danych.");
                                            break;
                                        }
                                        Utils.rebuildDbScript();
                                        dbversion = Utils.getDBVersion();
                                    }
                                    RupDatabase.theConfig = (from c in RupDatabase.theContext.Konfiguracja select c).FirstOrDefault();
                                    RupDatabase.jg = (RupDatabase.theConfig.StanowiskoFin == null) ? RupDatabase.theConfig.JednostkaGospodarcza : (RupDatabase.theConfig.StanowiskoFin.Trim().Length == 4) ? RupDatabase.theConfig.StanowiskoFin : RupDatabase.theConfig.JednostkaGospodarcza;


                                    RupLoaderMain miFo = new RupLoaderMain();
                                    miFo.currUser = lgn.usr;
                                    Application.Run(miFo);
                                }
                                else
                                    Application.Exit();

                                // sprawdzenie wersji  
                               

                            }
                            else
                            {
                                Application.Run(new RupLoaderMain());

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd " + ex.Message + " " + ((ex.InnerException == null) ? "" : ex.InnerException.Message));


                    }
                }
            }
           
                catch (Exception ex)
            {
                MessageBox.Show("Błąd " + ex.Message + " " +  ((ex.InnerException == null) ? "": ex.InnerException.Message));
            
            
            }
        }
      

     
        private static void verifySAPPwd()
        {
            string EncryptPhase = "Application error";
           
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                     ExportPI exportPI = new ExportPI();
                        exportPI.setSAPConnectionParams();
                if (ChngSAPPwd.VerifySAPPwdExpire(RupDatabase.theConfig.SAPPwdExpPeriod.Value))
                {
                    ChangeSAPPwd changeSAPPwd = new ChangeSAPPwd();
                    if (changeSAPPwd.ShowDialog() == DialogResult.OK)
                    {

                        User usr = context.User.Where(a => a.Id == UserProfile.UserID).FirstOrDefault();
                        usr.MEPPassword = Utils.Encrypt(changeSAPPwd.NewPassword, EncryptPhase);
                        context.SaveChanges();
                        MessageBox.Show("Hasło do ZSRK/MEP zostało zmienione. Używaj go również podczas logowania do systemu ZSRK", " Potwierdzenie zmiany hasła");
                        UserInfo.MEPPassword = usr.MEPPassword;
                        exportPI.setSAPConnectionParams();


                    }

                }

            }

        }


    }
}
