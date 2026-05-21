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
            RunMode.dbVersion = "3.6";
            string dbversion = getDBVersion();
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
                        MessageBox.Show("Wersja bazy danych: \"" + dbversion + "\" jest niezgodna z wersją bazy danych systemu \"" + RunMode.dbVersion + "\".\r\nNależy zalogować się na profilu administratora (uruchamiając bezośrednio  RupLoader.exe) i wykonać operację przebudowy bazy danych.");
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
                                            MessageBox.Show("Wersja bazy danych: \"" + dbversion + "\" jest niezgodna z wersją bazy danych systemu \"" + RunMode.dbVersion + "\".\r\nNależy zalogować się na profilu administratora i wykonać operację przebudowy bazy danych.");
                                            Application.Exit();
                                            return;
                                        }
                                        if (MessageBox.Show("Wersja bazy danych: \"" + dbversion + "\" jest niezgodna z wersją bazy danych systemu \"" + RunMode.dbVersion + "\".\r\nSystem wymaga wykonania przebudowy bazy danych. Upewnij się, czy użykownik posiada uprawnienia do przebudowy bazy.\n\r Czy wykonać operację przebudowy bazy danych ? ", "Niezgodność wersji bazy danych", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                        {
                                            Application.Exit();
                                            return;
                                        }
                                        rebuildDbScript();
                                        dbversion = getDBVersion();
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
        private static string getDBVersion()
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
                // ver 3.7
                  " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'ImportContentSystemData') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (38, N'ImportContentSystemData', N'https://sapwitl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ISMSender&receiverParty=&receiverService=&interface=ImportContentSystemDataOut&interfaceNamespace=urn:ms.gov.pl:ISM:ImportContentSystemData') "+
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (39, N'GetStatusContentSystemData', N'https://sapwitl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ISMSender&receiverParty=&receiverService=&interface=GetStatusContentSystemDataOut&interfaceNamespace=urn:ms.gov.pl:ISM:GetStatusContentSystemData') " +
                " END " 


            };

            return commnadsList;
        }
    }
}
