using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace ConsImporterService
{
    public partial class ConsImportService : ServiceBase
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        

        private Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _currentExecutionTask;

        /*
         * 0 - import nie jest wykonywany,
         * 1 - import jest aktualnie wykonywany.
         */
        private int _isRunning;

        private int _overlapMinutes;
        private int? _intervalMinutes;

        public ConsImportService()
        {
            InitializeComponent();
        }
        public void DebugRun()
        {
            StartImporter();
        }
        public void DebugRunOnce()
        {
            log.Info("Uruchomienie jednorazowe w trybie DEBUG.");

            LoadConfiguration();

            ExecuteImport();

            log.Info("Zakończenie jednorazowego wykonania w trybie DEBUG.");
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                log.Info("Uruchamianie usługi ConsImporterService.");

                LoadConfiguration();

                _cancellationTokenSource =
                    new CancellationTokenSource();

                /*
                 * W usłudze import uruchamiamy asynchronicznie,
                 * żeby OnStart szybko się zakończył.
                 */
                StartImportExecution();

                if (_intervalMinutes.HasValue)
                {
                    TimeSpan interval =
                        TimeSpan.FromMinutes(_intervalMinutes.Value);

                    _timer = new Timer(
                        TimerCallback,
                        null,
                        interval,
                        interval);

                    log.Info(
                        "Import będzie wykonywany cyklicznie co " +
                        _intervalMinutes.Value +
                        " minut.");
                }
                else
                {
                    log.Info(
                        "Brak interwału. Import zostanie wykonany tylko raz.");
                }
            }
            catch (Exception ex)
            {
                log.Error(
                    "Błąd podczas uruchamiania usługi.",
                    ex);

                throw;
            }
        }


        protected override void OnStop()
        {
            log.Info("Zatrzymywanie usługi ConsImporterService.");

            try
            {
                if (_timer != null)
                {
                    _timer.Change(
                        Timeout.Infinite,
                        Timeout.Infinite);

                    _timer.Dispose();
                    _timer = null;
                }

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }

                /*
                 * Importer.Run() jest metodą synchroniczną i nie obsługuje
                 * CancellationToken. Dlatego anulowanie zapobiega uruchamianiu
                 * kolejnych zadań, ale nie przerwie wykonywanej procedury SQL.
                 *
                 * Czekamy krótko na zakończenie bieżącego wykonania.
                 */
                Task currentTask = _currentExecutionTask;

                if (currentTask != null &&
                    !currentTask.IsCompleted)
                {
                    bool completed = currentTask.Wait(
                        TimeSpan.FromSeconds(30));

                    if (!completed)
                    {
                        log.Warn(
                            "Bieżący import nie zakończył się w czasie " +
                            "oczekiwania podczas zatrzymywania usługi.");
                    }
                }
            }
            catch (AggregateException ex)
            {
                log.Error(
                    "Błąd zadania importu podczas zatrzymywania usługi.",
                    ex.Flatten());
            }
            catch (Exception ex)
            {
                log.Error(
                    "Błąd podczas zatrzymywania usługi ConsImporterService.",
                    ex);
            }
            finally
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }

                log.Info(
                    "Usługa ConsImporterService została zatrzymana.");
            }
        }

        private void StartImporter()
        {
            try
            {
                log.Info("Uruchamianie usługi ConsImporterService.");

                _cancellationTokenSource =
                    new CancellationTokenSource();

                _overlapMinutes = GetIntSetting(
                    "ConsImportOverlapMinutes",
                    defaultValue: 10,
                    minimumValue: 0);

                _intervalMinutes =
                    GetOptionalPositiveIntSetting(
                        "ConsImportIntervalMinutes");

                /*
                 * Pierwsze uruchomienie następuje zawsze od razu,
                 * niezależnie od tego, czy ustawiono interwał.
                 */
                StartImportExecution();

                if (_intervalMinutes.HasValue)
                {
                    TimeSpan interval =
                        TimeSpan.FromMinutes(
                            _intervalMinutes.Value);

                    /*
                     * Pierwsze wywołanie timera następuje dopiero
                     * po upływie pełnego interwału, ponieważ import
                     * został już uruchomiony bezpośrednio powyżej.
                     */
                    _timer = new Timer(
                        TimerCallback,
                        null,
                        interval,
                        interval);

                    log.Info(
                        "Import będzie wykonywany cyklicznie co " +
                        _intervalMinutes.Value +
                        " minut.");
                }
                else
                {
                    log.Info(
                        "Nie ustawiono ConsImportIntervalMinutes. " +
                        "Import zostanie wykonany tylko raz po starcie usługi.");
                }

                log.Info(
                    "Usługa ConsImporterService została uruchomiona.");
            }
            catch (Exception ex)
            {
                log.Error(
                    "Błąd podczas uruchamiania usługi ConsImporterService.",
                    ex);

                throw;
            }
        }

        private void TimerCallback(object state)
        {
            try
            {
                if (_cancellationTokenSource == null ||
                    _cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                StartImportExecution();
            }
            catch (Exception ex)
            {
                /*
                 * Wyjątek nie może wydostać się z callbacka timera,
                 * ponieważ mógłby zakończyć proces usługi.
                 */
                log.Error(
                    "Błąd callbacka timera importu.",
                    ex);
            }
        }

        private void StartImportExecution()
        {
            if (_cancellationTokenSource == null ||
                _cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            /*
             * Jeżeli poprzedni import nadal trwa, kolejne wykonanie
             * zostaje pominięte.
             */
            if (Interlocked.CompareExchange(
                    ref _isRunning,
                    1,
                    0) != 0)
            {
                log.Warn(
                    "Pominięto cykliczne uruchomienie importu, " +
                    "ponieważ poprzedni import nadal jest wykonywany.");

                return;
            }

            CancellationToken token =
                _cancellationTokenSource.Token;

            _currentExecutionTask = Task.Run(
                () =>
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            return;

                        ExecuteImport();
                    }
                    catch (Exception ex)
                    {
                        /*
                         * Obsługujemy wyjątek wewnątrz Task.Run,
                         * aby zadanie nie pozostało jako Faulted
                         * bez odczytania wyjątku.
                         */
                        log.Error(
                            "Nieobsłużony błąd wykonania importu.",
                            ex);
                    }
                    finally
                    {
                        Interlocked.Exchange(
                            ref _isRunning,
                            0);
                    }
                },
                token);
        }

        private void ExecuteImport()
        {
            DateTime executionStart = DateTime.Now;

            log.Info(
                "Rozpoczęto proces importu CONS. " +
                "OverlapMinutes=" + _overlapMinutes + ".");

            try
            {
                var importer =
                    new ImportConsRequests();

                ImportProcessResult result =
                    importer.Run(_overlapMinutes);

                TimeSpan duration =
                    DateTime.Now - executionStart;

                log.Info(
                    "Zakończono proces importu CONS. " +
                    "Utworzono zadań: " +
                    result.CreatedJobs +
                    ", zakończono zadań: " +
                    result.CompletedJobs +
                    ", błędnych zadań: " +
                    result.FailedJobs +
                    ", przygotowano transferów do SAP: " +
                    result.PreparedTransfers +
                    ", czas wykonania: " +
                    duration.ToString(@"hh\:mm\:ss") +
                    ".");
            }
            catch (Exception ex)
            {
                log.Error(
                    "Błąd procesu importu CONS.",
                    ex);
            }
        }

        private static int GetIntSetting(
            string key,
            int defaultValue,
            int minimumValue)
        {
            string value =
                ConfigurationManager.AppSettings[key];

            if (String.IsNullOrWhiteSpace(value))
                return defaultValue;

            int parsedValue;

            if (!Int32.TryParse(
                    value,
                    out parsedValue))
            {
                log.Warn(
                    "Nieprawidłowa wartość ustawienia " +
                    key +
                    "='" + value +
                    "'. Przyjęto wartość domyślną: " +
                    defaultValue +
                    ".");

                return defaultValue;
            }

            if (parsedValue < minimumValue)
            {
                log.Warn(
                    "Wartość ustawienia " +
                    key +
                    " nie może być mniejsza niż " +
                    minimumValue +
                    ". Przyjęto wartość domyślną: " +
                    defaultValue +
                    ".");

                return defaultValue;
            }

            return parsedValue;
        }

        private static int? GetOptionalPositiveIntSetting(
            string key)
        {
            string value =
                ConfigurationManager.AppSettings[key];

            /*
             * Brak wpisu oznacza pracę jednorazową.
             */
            if (String.IsNullOrWhiteSpace(value))
                return null;

            int parsedValue;

            if (!Int32.TryParse(
                    value,
                    out parsedValue))
            {
                log.Warn(
                    "Nieprawidłowa wartość ustawienia " +
                    key +
                    "='" + value +
                    "'. Import zostanie wykonany tylko raz.");

                return null;
            }

            if (parsedValue <= 0)
            {
                log.Warn(
                    "Wartość ustawienia " +
                    key +
                    " musi być większa od zera. " +
                    "Import zostanie wykonany tylko raz.");

                return null;
            }

            return parsedValue;
        }
        private void LoadConfiguration()
        {
            _overlapMinutes = GetIntSetting(
                "ConsImportOverlapMinutes",
                defaultValue: 10,
                minimumValue: 0);

            _intervalMinutes = GetOptionalPositiveIntSetting(
                "ConsImportIntervalMinutes");
        }
       
    }
}