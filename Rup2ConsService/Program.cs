using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rup2ConsService
{
    internal static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            log.Info("Aplikacja uruchomiona");
#if DEBUG
            //AsposeLIcences.AsposePdfLicenseManager.EnsureLicense();
            //AsposeLIcences.AsposeWordsLicenseManager.EnsureLicense();
            var username = ConfigurationManager.AppSettings["UserName"];
            var worker = new RupQueue(username); // Obiekt z zewnętrznej klasy

          //  while (true)
            {
                try
                {
                    
                    worker.Pop(); // Wywołanie metody
                    Thread.Sleep(10); // Przykładowy czas oczekiwania między kolejnymi wywołaniami
                }
                catch (Exception ex)
                {
                    log.Error("Błąd przetwarzania komunikatu z kolejki ", ex);
                     
                }
            }

#else

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Rup2ConsService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }


 

}
