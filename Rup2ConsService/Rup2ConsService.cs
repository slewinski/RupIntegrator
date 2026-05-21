

using log4net.Config;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using AsposeLIcences;

namespace Rup2ConsService
{
    public partial class Rup2ConsService : ServiceBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource _cts;
        private int _threadCount;
        private int _idleDelay = 1000; // Czas oczekiwania w milisekundach, gdy nie ma zadań do przetworzenia
        private int _delay = 200; // Czas oczekiwania w milisekundach między kolejnymi sprawdzeniami kolejki
        private string username = string.Empty;
        public Rup2ConsService()
        {
            InitializeComponent();
            this.ServiceName = "Rup2ConsService";
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Service  CaseFileBuilder started.");
            // ustawienie aby dwa pierwsze procki zostały dla systemu operacyjnego.
            var process = Process.GetCurrentProcess();
            //int cpuCount = Environment.ProcessorCount;
            //
            //long affinityMask = 0;
            //
            //// Buduj maskę z wyłączeniem CPU 0 i 1
            //for (int i = 2; i < cpuCount; i++)
            //{
            //    affinityMask |= 1L << i;
            //}

            // Ustaw affinity
            //process.ProcessorAffinity = (IntPtr)affinityMask;
            
            var userName = ConfigurationManager.AppSettings["UserName"];

            _cts = new CancellationTokenSource();
            try
            {
                _threadCount = int.TryParse(ConfigurationManager.AppSettings["ThreadCount"], out int count) ? count : 5;
            }
            catch
            {
                _threadCount = 1;
            }
            try
            {
                _idleDelay = int.TryParse(ConfigurationManager.AppSettings["IdleDelay"], out int count) ? count : 1000;
            }
            catch
            {
                _idleDelay = 1000;
            }
            try
            {
                _delay = int.TryParse(ConfigurationManager.AppSettings["Delay"], out int count) ? count : 200;
            }
            catch
            {
                _delay = 200;
            }

            try
            {
                userName = ConfigurationManager.AppSettings["UserName"];
            }
            catch
            {
                log.Error("Brak nazwy uzytkownika Rup Integrator. Uzupełnij zbiór.config");
            }

            EventLog.WriteEntry("Service started with: " + _threadCount + " threads, " + _delay + " delay (ms), " + _idleDelay + " idle delay ( ms)");

            //AsposePdfLicenseManager.EnsureLicense();
            //AsposeWordsLicenseManager.EnsureLicense();

            for (int i = 0; i < _threadCount; i++)
            {
                Thread.Sleep(500); // Krótkie opóźnienie między uruchomieniami wątków
                Task.Run(() => RunWorker(_cts.Token));
            }
        }

        private void RunWorker(CancellationToken token)
        {
            //  var worker = new CaseFileUpdater.CaseFileQueueService(); // Obiekt z zewnętrznej klasy
            username = ConfigurationManager.AppSettings["MockKartaFilePath"];
            var worker = new   RupQueue (username); // Obiekt z zewnętrznej klasy
            while (!token.IsCancellationRequested)
            {
                try
                {

                    worker.Pop(); // Wywołanie metody wstrzymanie procesu jest w procedurze pop.

                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Error: " + ex.Message + " " + ex.ToString(), EventLogEntryType.Error);
                }
            }
        }

        protected override void OnStop()
        {
            _cts.Cancel();
            EventLog.WriteEntry("Service stopped.");
        }
    }

}

