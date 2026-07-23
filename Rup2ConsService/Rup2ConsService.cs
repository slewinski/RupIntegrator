using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Rup2ConsService
{
    public partial class Rup2ConsService : ServiceBase
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource _cancellationTokenSource;
        private readonly List<Task> _workerTasks = new List<Task>();

        private int _threadCount;
        private int _idleDelayMilliseconds;
        private int _stopTimeoutSeconds;
        private int _stalePendingMinutes;

        public Rup2ConsService()
        {
            InitializeComponent();
            ServiceName = "Rup2ConsService";
        }

        protected override void OnStart(string[] args)
        {
            StartWorkers();
        }

        protected override void OnStop()
        {
            StopWorkers();
        }

        public void DebugRun()
        {
            StartWorkers();

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Jednorazowo przetwarza wskazany rekord ConsKartaTransfer.
        /// Nie uruchamia pętli workerów.
        /// </summary>
        /// 
        public bool RunSingleTransfer(int transferId)
        {
            if (transferId <= 0)
                throw new ArgumentOutOfRangeException("transferId");

            log.Info(
                "Uruchomiono jednorazowe przetwarzanie transferu Id=" +
                transferId + ".");

            string userName =
                ConfigurationManager.AppSettings["UserName"];

            SapConsClientConfiguration.Initialize(userName);

            int idleDelayMilliseconds =
                GetPositiveIntSetting(
                    "IdleDelayMilliseconds",
                    1000);

            var worker =
                new ConsTransferWorker(idleDelayMilliseconds);

            bool processed =
                worker.ProcessSingleTransfer(transferId);

            if (processed)
            {
                log.Info(
                    "Zakończono jednorazowe przetwarzanie transferu Id=" +
                    transferId + ".");
            }
            else
            {
                log.Warn(
                    "Nie wykonano jednorazowego transferu Id=" +
                    transferId + ".");
            }

            return processed;
        }
        private void StartWorkers()
        {
            log.Info("Uruchamianie usługi transferu Zaimportowanych danych do CONS/SAP.");

            _threadCount = GetPositiveIntSetting("ThreadCount", 5);
            _idleDelayMilliseconds = GetPositiveIntSetting("IdleDelayMilliseconds", 1000);
            _stopTimeoutSeconds = GetPositiveIntSetting("StopTimeoutSeconds", 60);
            _stalePendingMinutes = GetPositiveIntSetting("StalePendingMinutes", 30);

            string userName = ConfigurationManager.AppSettings["UserName"];
            SapConsClientConfiguration.Initialize(userName);

            int recovered = ConsTransferWorker.RecoverStalePendingTransfers(
                _stalePendingMinutes);

            if (recovered > 0)
                log.Warn("Przywrócono do kolejki transferów: " + recovered + ".");

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            for (int workerNumber = 1; workerNumber <= _threadCount; workerNumber++)
            {
                int capturedWorkerNumber = workerNumber;

                Task task = Task.Factory.StartNew(
                    () => RunWorker(capturedWorkerNumber, token),
                    token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);

                _workerTasks.Add(task);
            }

            log.Info(
                "Uruchomiono " + _threadCount +
                " workerów transferu CONS do SAP.");
        }

        private void RunWorker(int workerNumber, CancellationToken token)
        {
            log.Info("Worker " + workerNumber + " uruchomiony.");

            try
            {
                var worker = new ConsTransferWorker(_idleDelayMilliseconds);
                worker.Run(token);
            }
            catch (Exception ex)
            {
                log.Error("Worker " + workerNumber + " zakończył się błędem.", ex);
            }
            finally
            {
                log.Info("Worker " + workerNumber + " zatrzymany.");
            }
        }

        private void StopWorkers()
        {
            log.Info("Zatrzymywanie usługi transferu CONS do SAP.");

            if (_cancellationTokenSource == null)
                return;

            _cancellationTokenSource.Cancel();

            try
            {
                bool completed = Task.WaitAll(
                    _workerTasks.ToArray(),
                    TimeSpan.FromSeconds(_stopTimeoutSeconds));

                if (!completed)
                {
                    log.Warn(
                        "Nie wszystkie workery zakończyły pracę w czasie " +
                        _stopTimeoutSeconds + " sekund.");
                }
            }
            catch (AggregateException ex)
            {
                log.Error("Błąd podczas zatrzymywania workerów.", ex.Flatten());
            }
            finally
            {
                _workerTasks.Clear();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            log.Info("Usługa transferu CONS do SAP została zatrzymana.");
        }

        private static int GetPositiveIntSetting(string key, int defaultValue)
        {
            string text = ConfigurationManager.AppSettings[key];
            int value;

            if (!Int32.TryParse(text, out value) || value <= 0)
                return defaultValue;

            return value;
        }
    }
}
