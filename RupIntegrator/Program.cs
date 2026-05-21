using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KnsMigrator
{
    static class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( string[] args)
        {
            log.Info("App started");
            if (args.GetLength(0) > 0)
            {

                switch (args[0].ToUpper())
                {
                    case "/P":
                        RunMode.operation = Operations.Przypisy;
                        break;

                    case "/O":
                        RunMode.operation = Operations.Odpisy;
                        break;

                    case "/U":
                        RunMode.operation = Operations.UGO;
                        break;

                    case "/C":
                        RunMode.operation = Operations.Potwierdzenia;
                        break;

                    case "/E":
                        RunMode.operation = Operations.Export;
                        break;
                    default:
                        Utils.LogWriter("Błędny parametr wywołania");
                        Application.Exit();
                        return;

                }

                RunMode.silentMode = true;
                RunMode.tyOpExport = 0;

                if (args.GetLength(0) > 1)
                {
                    if (RunMode.operation != Operations.Export)
                    {

                        int grks = 0;
                        if (int.TryParse(args[1], out grks))
                            RunMode.grKsiag = grks;
                    }
                    else
                    {
                        switch (args[1].ToUpper())
                        {
                            case "P":
                                RunMode.tyOpExport = 2;
                                break;
                            case "O":
                                RunMode.tyOpExport = 3;
                                break;
                            case "U":
                                RunMode.tyOpExport = 6;
                                break;
                            default:
                                RunMode.tyOpExport = 0;
                                break;
                        }

                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                UserInfo.Id = 0; // inicjaloizacja użytkownika.
                using (KnsMigratorEntities _context = new KnsMigratorEntities())
                {
                    string dbversion= string.Empty;

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
                            MigrForm miFo = new MigrForm();
                            miFo.currUser = lgn.usr;
                            Application.Run(miFo);
                        }
                        else
                            Application.Exit();
                    }
                    else
                    {
                        Application.Run(new MigrForm());

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd " + ex.Message + " " +  ((ex.InnerException == null) ? "": ex.InnerException.Message));
            
            
            }
        }
    }
}
