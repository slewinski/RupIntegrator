// Ostateczna wersja po refaktoryzacji.
// ConsImportFromDB.cs nie jest już wymagany – logika została przeniesiona tutaj.
using System.IO.Compression;
using Cons2RupModel;
using ConsImport;
using ConsInterfeces.Rup2ConsImportContentSystemData;
using RupDatabase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ConsImporterService
{
    /// <summary>
    /// Kompletny proces przygotowania importu CONS:
    /// 1. pobranie aktywnych konfiguracji,
    /// 2. utworzenie ConsJobItem,
    /// 3. wykonanie procedury składowanej,
    /// 4. zbudowanie komunikatów SAP,
    /// 5. zapis zadań ConsKartaTransfer ze statusem Prepared.
    /// </summary>
    /// 


    public class ImportConsRequests
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int JobStatusNew = 0;
        private const int JobStatusInProgress = 1;
        private const int JobStatusDone = 2;
        private const int JobStatusError = 3;

        private static readonly DateTime DefaultStartDate = new DateTime(2010, 1, 1);

        
        private readonly string _connectionString;

        public ImportConsRequests()
        {
         

            _connectionString = ConfigurationManager
                .ConnectionStrings["RupIntegrator.Properties.Settings.RupDB"]
                .ConnectionString;
        }

        /// <summary>
        /// Uruchamia cały proces: tworzy zadania dla aktywnych połączeń,
        /// pobiera dane i zapisuje komunikaty gotowe do transferu do SAP.
        /// </summary>
        public ImportProcessResult Run(int overlapMinutes)
        {
            IList<ConsExternalDBConnectionConfig> connections = GetActiveConnections();
            IList<ConsJobItem> jobs = CreateJobItems(connections, overlapMinutes);

            int preparedTransfers = 0;
            int failedJobs = 0;

            foreach (ConsJobItem job in jobs)
            {
                JobProcessResult result = ProcessSingleJobItem(job.Id);
                preparedTransfers += result.PreparedTransfers;

                if (!result.Success)
                    failedJobs++;
            }

            return new ImportProcessResult
            {
                CreatedJobs = jobs.Count,
                CompletedJobs = jobs.Count - failedJobs,
                FailedJobs = failedJobs,
                PreparedTransfers = preparedTransfers
            };
        }

        public IList<ConsExternalDBConnectionConfig> GetActiveConnections()
        {
            using (var context = new RupDBEntities())
            {
                return context.ConsExternalDBConnectionConfig
                    .AsNoTracking()
                    .Where(x => x.isActive == true)
                    .OrderBy(x => x.ConnectionName)
                    .ThenBy(x => x.id)
                    .ToList();
            }
        }

        public IList<ConsJobItem> CreateJobItems(
            IEnumerable<ConsExternalDBConnectionConfig> activeConnections,
            int overlapMinutes)
        {
            if (activeConnections == null)
                throw new ArgumentNullException("activeConnections");

            if (overlapMinutes < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "overlapMinutes",
                    "Liczba minut cofnięcia nie może być ujemna.");
            }

            DateTime rangeEnd = DateTime.Now;
            List<int> connectionIds = activeConnections.Select(x => x.id).Distinct().ToList();

            using (var context = new RupDBEntities())
           {
                try
                {
                    var createdJobs = new List<ConsJobItem>();

                    foreach (int connectionId in connectionIds)
                    {
                        DateTime? lastFinishDate = context.ConsJobItem
                            .Where(x =>
                                x.consExternalDBConnectionConfig_Id == connectionId &&
                                x.finishDate.HasValue &&
                                x.status != JobStatusError)
                            .OrderByDescending(x => x.finishDate)
                            .Select(x => x.finishDate)
                            .FirstOrDefault();

                        DateTime rangeStart = lastFinishDate.HasValue
                            ? lastFinishDate.Value.AddMinutes(-overlapMinutes)
                            : DefaultStartDate;

                        if (rangeStart > rangeEnd)
                            rangeStart = rangeEnd;

                        var job = new ConsJobItem
                        {
                            insertDate = DateTime.Now,
                            startDate = rangeStart,
                            finishDate = rangeEnd,
                            consExternalDBConnectionConfig_Id = connectionId,
                            status = JobStatusNew,
                            info = "Utworzono zadanie "
                        };

                        context.ConsJobItem.Add(job);
                        createdJobs.Add(job);
                    }

                    context.SaveChanges();
                    return createdJobs;
                }
                catch
                {
                    
                    throw;
                }
            }
        }

        /// <summary>
        /// Przetwarza wszystkie wcześniej utworzone, nieprzetworzone zadania.
        /// </summary>
        public int ProcessPendingJobItems()
        {
            List<int> jobIds;

            using (var context = new RupDBEntities())
            {
                jobIds = context.ConsJobItem
                    .AsNoTracking()
                    .Where(x =>
                        (x.status == null || x.status == JobStatusNew) &&
                        x.startDate.HasValue &&
                        x.finishDate.HasValue)
                    .OrderBy(x => x.insertDate)
                    .ThenBy(x => x.Id)
                    .Select(x => x.Id)
                    .ToList();
            }

            int preparedTransfers = 0;

            foreach (int jobId in jobIds)
                preparedTransfers += ProcessSingleJobItem(jobId).PreparedTransfers;

            return preparedTransfers;
        }

        private JobProcessResult ProcessSingleJobItem(int jobId)
        {
            ConsJobItem job;
            ConsExternalDBConnectionConfig connection;

            using (var context = new RupDBEntities())
            {
                job = context.ConsJobItem
                    .Include(x => x.ConsExternalDBConnectionConfig)
                    .SingleOrDefault(x => x.Id == jobId);

                if (job == null)
                {
                    log.Warn("Nie znaleziono ConsJobItem o Id=" + jobId + ".");
                    return JobProcessResult.Failed();
                }

                if (job.status.HasValue && job.status.Value != JobStatusNew)
                    return JobProcessResult.Failed();

                if (!job.startDate.HasValue || !job.finishDate.HasValue)
                {
                    SetJobError(job, "Brak daty początkowej lub końcowej zakresu.");
                    context.SaveChanges();
                    return JobProcessResult.Failed();
                }

                connection = job.ConsExternalDBConnectionConfig;

                if (connection == null)
                {
                    SetJobError(job, "Brak konfiguracji połączenia.");
                    context.SaveChanges();
                    return JobProcessResult.Failed();
                }

                if (connection.isActive != true)
                {
                    SetJobError(job, "Konfiguracja połączenia nie jest aktywna.");
                    context.SaveChanges();
                    return JobProcessResult.Failed();
                }

                job.status = JobStatusInProgress;
                job.queryDate = DateTime.Now;
                job.info = "Rozpoczęto pobieranie danych ";
                context.SaveChanges();
            }

            try
            {
                DateTime startDate = job.startDate.Value;
                DateTime finishDate = job.finishDate.Value;

                log.Info(
                    "Rozpoczęcie ConsJobItem Id=" + jobId +
                    ", połączenie=" + connection.ConnectionName +
                    ", procedura=" + connection.sp_name +
                    ", zakres=" + startDate.ToString("yyyy-MM-dd HH:mm:ss") +
                    " - " + finishDate.ToString("yyyy-MM-dd HH:mm:ss"));

                DataSet dataSet = ExecuteStoredProcedure(connection, startDate, finishDate);

                if (dataSet == null)
                {
                    MarkJobError(jobId, "Procedura nie zwróciła danych lub wystąpił błąd wykonania.");
                    return JobProcessResult.Failed();
                }

                List<ConsImportData> importItems = BuildImportItems(dataSet);
                int savedCount = SavePreparedTransfers(jobId, importItems);

                MarkJobCompleted(jobId, savedCount);

                log.Info(
                    "Zakończono ConsJobItem Id=" + jobId +
                    ". Zapisano zadań transferu do SAP: " + savedCount + ".");

                return JobProcessResult.Succeeded(savedCount);
            }
            catch (Exception ex)
            {
                log.Error("Błąd przetwarzania ConsJobItem Id=" + jobId + ".", ex);
                MarkJobError(jobId, GetFullExceptionMessage(ex));
                return JobProcessResult.Failed();
            }
        }

        private int SavePreparedTransfers(int jobId, IEnumerable<ConsImportData> importItems)
        {
            int duplicateCount = 0;

            if (importItems == null)
                return 0;

            int savedCount = 0;

            using (var context = new RupDBEntities())
            {
                try
                {
                    foreach (ConsImportData importItem in importItems)
                    {
                        if (importItem == null ||
                            importItem.importContentSystemDataRequest == null)
                        {
                            continue;
                        }

                        int idSprawy = importItem.IdSprawy;
                        int idStrony = importItem.IdStrony;

                        ImportContentSystemDataRequest request =
                            importItem.importContentSystemDataRequest;

                        string originalGuid = request.GUID;

                        request.GUID = null;
                        string payloadHash = CalculateSha256(SerializeRequest(request));

                        request.GUID = String.IsNullOrWhiteSpace(originalGuid)
                            ? Guid.NewGuid().ToString()
                            : originalGuid;

                        string payload = SerializeRequest(request);

                        ConsKartaTransfer existingTransfer =
                            context.ConsKartaTransfer
                                .Where(x =>
                                    x.hash == payloadHash &&
                                    (
                                        x.status == (int)ConsImportStatus.Prepared ||
                                        x.status == (int)ConsImportStatus.Pending ||
                                        x.status == (int)ConsImportStatus.Done ||
                                        x.status == (int)ConsImportStatus.Duplicate
                                    ))
                                .OrderBy(x => x.Id)
                                .FirstOrDefault();

                        bool isDuplicate = existingTransfer != null;

                        var transfer = new ConsKartaTransfer
                        {
                            guidImport= new Guid( request.GUID),
                            idKomunikatu = Guid.NewGuid().ToString(),

                            status = isDuplicate
                                ? (int)ConsImportStatus.Duplicate
                                : (int)ConsImportStatus.Prepared,

                            idStronyWydzial = idStrony,
                            idSprawyWydzial = idSprawy,

                            dImportu = DateTime.Now,

                            trescOdpowiedzi = isDuplicate
                                ? "Duplikat komunikatu. Wcześniejszy transfer Id=" +
                                  existingTransfer.Id + "."
                                : null,

                            payload = payload,
                            hash = payloadHash,
                            consJobItemId = jobId

                        };

                        context.ConsKartaTransfer.Add(transfer);

                        if (isDuplicate)
                        {
                            duplicateCount++;

                            log.Info(
                                "Wykryto duplikat ConsKartaTransfer. " +
                                "IdSprawy=" + idSprawy +
                                ", IdStrony=" + idStrony +
                                ", poprzedni transfer Id=" + existingTransfer.Id +
                                ", hash=" + payloadHash + ".");
                        }
                        else
                        {
                            savedCount++;
                        }
                    }
                    context.SaveChanges();
                    return savedCount;
                }
                catch
                {
                    throw;
                }
            }
        }

        private void MarkJobCompleted(int jobId, int createdCount)
        {
            using (var context = new RupDBEntities())
            {
                ConsJobItem job = context.ConsJobItem.SingleOrDefault(x => x.Id == jobId);
                if (job == null)
                    return;

                job.status = JobStatusDone;
                job.info = "Pobieranie zakończone. Zapisano zadań transferu do SAP: " +
                    createdCount;
                context.SaveChanges();
            }
        }

        private void MarkJobError(int jobId, string errorMessage)
        {
            using (var context = new RupDBEntities())
            {
                ConsJobItem job = context.ConsJobItem.SingleOrDefault(x => x.Id == jobId);
                if (job == null)
                    return;

                SetJobError(job, errorMessage);
                context.SaveChanges();
            }
        }

        private static void SetJobError(ConsJobItem job, string errorMessage)
        {
            job.status = JobStatusError;
            job.info = Truncate(errorMessage, 2000);
        }

        private static string SerializeRequest(ImportContentSystemDataRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var serializer = new XmlSerializer(typeof(ImportContentSystemDataRequest));
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = false,
                OmitXmlDeclaration = false
            };

            using (var stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    serializer.Serialize(writer, request);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static string CalculateSha256(string value)
        {
            byte[] input = Encoding.UTF8.GetBytes(value ?? String.Empty);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(input);
                var result = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                    result.Append(b.ToString("x2"));

                return result.ToString();
            }
        }

        private static string GetFullExceptionMessage(Exception exception)
        {
            var result = new StringBuilder();

            while (exception != null)
            {
                if (result.Length > 0)
                    result.Append(" | ");

                result.Append(exception.Message);
                exception = exception.InnerException;
            }

            return result.ToString();
        }

        private static string Truncate(string value, int maximumLength)
        {
            if (String.IsNullOrEmpty(value) || value.Length <= maximumLength)
                return value;

            return value.Substring(0, maximumLength);
        }

        private List<ConsImportData> BuildImportItems(DataSet ds)
        {
            var result = new List<ConsImportData>();

            if (ds == null || ds.Tables == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return result;

            DataTable mainTable = ds.Tables[0];
            DataTable addressTable = ds.Tables.Count > 1 ? ds.Tables[1] : null;
            DataTable orzeczeniaTable = ds.Tables.Count > 2 ? ds.Tables[2] : null;
            DataTable zdarzeniaTable = ds.Tables.Count > 3 ? ds.Tables[3] : null;
            // testowy zapis orzeczenia do pliku
            /*
            if (orzeczeniaTable != null && orzeczeniaTable.Rows.Count > 0)
            {
                DataRow o = orzeczeniaTable.Rows[0];

                byte[] content = null;
                byte[] raw = null;
                if (o["msword"] != DBNull.Value)
                {
                    if (o["msword"] is byte[])
                    {
                       raw = (byte[])o["msword"];
                       content = Utils.DecompressMsWord(raw);

                    }
                    else
                    {
                        // jeśli msword przychodzi jako base64 string
                        string base64 = Convert.ToString(o["msword"]);
                        content = Convert.FromBase64String(base64);
                    }
                }

                if (content != null && content.Length > 0)
                {
                    string fileName = "orzeczenie_test.doc";
                    string path = System.IO.Path.Combine(@"C:\temp", fileName);

                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                    System.IO.File.WriteAllBytes(path, content);
                }
            }
            */


            Func<DataRow, string, bool> HasColumn = (row, name) =>
                row != null &&
                row.Table != null &&
                row.Table.Columns.Contains(name) &&
                row[name] != DBNull.Value;

            Func<string, string> Clean = value =>
            {
                if (String.IsNullOrWhiteSpace(value))
                    return String.Empty;

                return value
                    .Replace("ł", "l").Replace("Ł", "L")
                    .Replace("ą", "a").Replace("Ą", "A")
                    .Replace("ć", "c").Replace("Ć", "C")
                    .Replace("ę", "e").Replace("Ę", "E")
                    .Replace("ń", "n").Replace("Ń", "N")
                    .Replace("ó", "o").Replace("Ó", "O")
                    .Replace("ś", "s").Replace("Ś", "S")
                    .Replace("ż", "z").Replace("Ż", "Z")
                    .Replace("ź", "z").Replace("Ź", "Z")
                    .Trim();
            };

            Func<DataRow, string, string> S = (row, name) =>
                HasColumn(row, name) ?  Convert.ToString(row[name]).Trim() : String.Empty;

            Func<DataRow, string[], string> SAny = (row, names) =>
            {
                foreach (string name in names)
                {
                    string value = S(row, name);
                    if (!String.IsNullOrWhiteSpace(value))
                        return value;
                }

                return String.Empty;
            };

            Func<DataRow, string, int> I = (row, name) =>
            {
                if (!HasColumn(row, name))
                    return 0;

                int value;
                return Int32.TryParse(Convert.ToString(row[name]), out value) ? value : 0;
            };

            Func<DataRow, string, decimal> D = (row, name) =>
            {
                if (!HasColumn(row, name))
                    return 0m;

                string value = Convert.ToString(row[name]).Trim();

                if (String.IsNullOrWhiteSpace(value))
                    return 0m;

                decimal resultd;

                if (Decimal.TryParse(
                    value,
                    System.Globalization.NumberStyles.Any,
                    new System.Globalization.CultureInfo("pl-PL"),
                    out resultd))
                {
                    return resultd;
                }

                if (Decimal.TryParse(
                    value.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out resultd))
                {
                    return resultd;
                }

                return Convert.ToDecimal(row[name]);
            };

            Func<DataRow, string, string> DateS = (row, name) =>
            {
                if (!HasColumn(row, name))
                    return String.Empty;

                if (row[name] is DateTime)
                    return ((DateTime)row[name]).ToString("yyyyMMdd");

                string value = Convert.ToString(row[name]).Trim();

                if (String.IsNullOrWhiteSpace(value))
                    return String.Empty;

                DateTime parsed;

                if (DateTime.TryParse(value, out parsed))
                    return parsed.ToString("yyyyMMdd");

                value = value.Replace("-", "").Replace(".", "").Replace("/", "");

                return value;
            };

            Func<DataRow, string[], string> DateAny = (row, names) =>
            {
                foreach (string name in names)
                {
                    string value = DateS(row, name);
                    if (!String.IsNullOrWhiteSpace(value))
                        return value;
                }

                return String.Empty;
            };

            Func<object, string> ToBase64 = value =>
            {
                if (value == null || value == DBNull.Value)
                    return String.Empty;

                if (value is byte[])
                    return Convert.ToBase64String((byte[])value);

                string text = Convert.ToString(value);

                if (String.IsNullOrWhiteSpace(text))
                    return String.Empty;

                return text.Trim();
            };
            Func<string, string> SafeAttachmentName = name =>
            {
                //name = Clean(name);

                if (String.IsNullOrWhiteSpace(name))
                    return "orzeczenie.doc";

                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    name = name.Replace(c.ToString(), String.Empty);

                if (!name.EndsWith(".doc", StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                {
                    name += ".doc";
                }

                return name;
            };

            Func<DataRow, string, string> AttachmentContent = (r, columnName) =>
            {
                if (!HasColumn(r, columnName))
                    return String.Empty;

                object value = r[columnName];

                if (value == null || value == DBNull.Value)
                    return String.Empty;

                if (value is byte[])
                {
                    byte[] raw = (byte[])value;

                    try
                    {
                        return ToBase64(Utils.DecompressMsWord(raw));
                    }
                    catch
                    {
                        return Convert.ToBase64String(raw);
                    }
                }

                return Convert.ToString(value);
            };

            Func<DataRow, int, PozycjaPoleKonfigurowalne[]> BuildPolaKonfigurowalne = (r, max) =>
            {
                return Enumerable.Range(1, max)
                    .Select(n =>
                    {
                        string nazwa = S(r, "PozycjaPoleKonfigurowalneNazwa" + n);

                        if (String.IsNullOrWhiteSpace(nazwa))
                            return null;

                        bool isDateField =
                            nazwa == "DATA_ORZ" ||
                            nazwa == "DATA_UPR" ||
                            nazwa == "DATA_SKIEROWANIA" ||
                            nazwa == "DATA_PRZED_KS" ||
                            nazwa == "DATA_PRZED_GR" ||
                            nazwa == "DATA_KOLEJNA" ||
                            nazwa == "DATA_REJESTRACJI_KO" ||
                            nazwa == "DATA_ZALATWIENIA" ||
                            nazwa == "DATA_WYKO"||
                            nazwa == "DATA_WYKONANIA";

                        string wartosc = isDateField
                            ? DateS(r, "PozycjaPoleKonfigurowalneWartosc" + n)
                            : S(r, "PozycjaPoleKonfigurowalneWartosc" + n);

                        if (String.IsNullOrWhiteSpace(wartosc))
                            return null;

                        return new PozycjaPoleKonfigurowalne
                        {
                            Nazwa = nazwa,
                            Wartosc = wartosc
                        };
                    })
                    .Where(x => x != null)
                    .ToArray();
            };

            Func<string, string> CountryCode = value =>
            {
                //value = Clean(value);

                if (String.IsNullOrWhiteSpace(value))
                    return String.Empty;

                if (value.Equals("Polska", StringComparison.OrdinalIgnoreCase))
                    return "PL";

                if (value.Equals("Poland", StringComparison.OrdinalIgnoreCase))
                    return "PL";

                if (value.Length == 2)
                    return value.ToUpper();

                return value;
            };

            Func<string, string> RegionCode = value =>
            {
               value = Clean(value).ToLower();

                if (String.IsNullOrWhiteSpace(value))
                    return String.Empty;

                switch (value)
                {
                    case "dolnoslaskie": return "DSL";
                    case "kujawsko-pomorskie": return "K-P";
                    case "lubelskie": return "LBL";
                    case "lubuskie": return "LBS";
                    case "lodzkie": return "LDZ";
                    case "malopolskie": return "MAL";
                    case "mazowieckie": return "MAZ";
                    case "opolskie": return "OPO";
                    case "podkarpackie": return "PDK";
                    case "podlaskie": return "PDL";
                    case "pomorskie": return "POM";
                    case "slaskie": return "SLS";
                    case "swietokrzyskie": return "SWK";
                    case "warminsko-mazurskie": return "W-M";
                    case "wielkopolskie": return "WLK";
                    case "zachodniopomorskie": return "Z-P";
                    default: return value.ToUpper();
                }
            };

            using (RupDBEntities context = new RupDBEntities())
            {
                var groups = mainTable.AsEnumerable()
                    .GroupBy(r => new
                    {
                        IdSprawy = HasColumn(r, "id_sprawy") ? I(r, "id_sprawy") : I(r, "idSprawy"),
                        IdStrony = HasColumn(r, "id_strony") ? I(r, "id_strony") : I(r, "idStrony")
                    });

                foreach (var group in groups)
                {
                    DataRow row = group.First();

                    int idSprawy = group.Key.IdSprawy;
                    int idStrony = group.Key.IdStrony;

                    List<DataRow> orzeczenieRows = new List<DataRow>();

                    if (orzeczeniaTable != null)
                    {
                        orzeczenieRows = orzeczeniaTable
                                        .AsEnumerable()
                                        .Where(o =>
                                            (!HasColumn(o, "id_sprawy") ||
                                             I(o, "id_sprawy") == idSprawy) &&
                                            (!HasColumn(o, "id_strony") ||
                                             I(o, "id_strony") == idStrony))
                                        .OrderBy(o =>
                                            HasColumn(o, "d_orzecz") &&
                                            o["d_orzecz"] != DBNull.Value
                                                ? Convert.ToDateTime(o["d_orzecz"])
                                                : DateTime.MinValue)
                                        .ToList();
                    }

                    string zalacznikNazwa = null;  //S(row, "ZalacznikNazwa");
                    string zalacznikZawartosc = null;  //S(row, "ZalacznikZawartosc");

                    if (orzeczenieRows.Count > 0)
                    {
                        zalacznikZawartosc = BuildZipBase64(
                            orzeczenieRows,
                            SafeAttachmentName);

                        if (!String.IsNullOrWhiteSpace(zalacznikZawartosc))
                        {
                            zalacznikNazwa =
                                "orzeczenia_" +
                                idSprawy +
                                "_" +
                                idStrony +
                                ".gzip";
                        }
                    }


                    bool skip = context.ConsKartaTransfer.Any(a =>
                        a.idStronyWydzial == idStrony &&
                        a.idSprawyWydzial == idSprawy &&
                        (
                            (ConsImportStatus)a.status == ConsImportStatus.Done ||
                            (ConsImportStatus)a.status == ConsImportStatus.Pending
                        ));

                    if (skip)
                        continue;

                    var partner = new PozycjaDanePartneraBiznesowego
                    {
                        TypPartnera = SAny(row, new[] { "TypPartnera" }),
                        NumerPartneraNadrzednego = String.Empty,
                        NumerPartneraBiznesowego = String.Empty,
                        TypPartneraHandlowego = SAny(row, new[] { "TypPartneraHandlowego" }),

                        PartnerHandlowyImie = S(row, "PartnerHandlowyImie"),
                        PartnerHandlowyDrugieImie = S(row, "PartnerHandlowyDrugieImie"),
                        PartnerHandlowyNazwisko = S(row, "PartnerHandlowyNazwisko"),
                        PartnerHandlowyNazwiskoRodowe = S(row, "PartnerHandlowyNazwiskoRodowe"),
                        PartnerHandlowyNazwa1 = S(row, "PartnerHandlowyNazwa1"),
                        PartnerHandlowyNazwa2 = S(row, "PartnerHandlowyNazwa2"),
                        PartnerHandlowyNazwa3 = S(row, "PartnerHandlowyNazwa3"),
                        PartnerHandlowyNazwa4 = S(row, "PartnerHandlowyNazwa4"),

                        PartnerHandlowyPesel = S(row, "PartnerHandlowyPesel"),
                        PartnerHandlowyRegon = S(row, "PartnerHandlowyRegon"),
                        PartnerHandlowyNip = S(row, "PartnerHandlowyNip"),

                        PartnerHandlowyPanstwoUrodzenia =  S(row, "PartnerHandlowyPanstwoUrodzenia"),
                        PartnerHandlowyObywatelstwo = String.IsNullOrWhiteSpace(S(row, "PartnerHandlowyObywatelstwo"))?"PL" : S(row, "PartnerHandlowyObywatelstwo"),
                        PartnerHandlowyInneObywatelstwa = S(row, "PartnerHandlowyInneObywatelstwa"),
                        PartnerHandlowyStatusZatrudnienia = S(row, "PartnerHandlowyStatusZatrudnienia"),
                        PartnerHandlowyZawod = S(row, "PartnerHandlowyZawod"),
                        PartnerHandlowyWyksztalcenie = S(row, "PartnerHandlowyWyksztalcenie"),
                        PartnerHandlowyEmail = S(row, "PartnerHandlowyEmail"),
                        PartnerHandlowyWykonywanieFunkcji = S(row, "PartnerHandlowyWykonywanieFunkcji"),
                        PartnerHandlowyDataUrodzenia = DateS(row, "PartnerHandlowyDataUrodzenia"),
                        PartnerHandlowyImieOjca = S(row, "PartnerHandlowyImieOjca"),
                        PartnerHandlowyImieMatki = S(row, "PartnerHandlowyImieMatki"),
                        PartnerHandlowyNazwiskoRodoweMatki = S(row, "PartnerHandlowyNazwiskoRodoweMatki"),
                        InformacjaInna = S(row, "InformacjaInna"),
                        PartnerHandlowyPobytZakladKarny = S(row, "PartnerHandlowyPobytZakladKarny"),
                        PartnerHandlowyObronca = S(row, "PartnerHandlowyObronca"),

                        Krs = S(row, "Krs"),
                        TypPartneraZCONS = !String.IsNullOrWhiteSpace(S(row, "TypPartneraZCONS")) ? S(row, "TypPartneraZCONS") : "DL_GLOWNY",
                        NumerRBN = !String.IsNullOrWhiteSpace(S(row, "NumerRBN")) ? S(row, "NumerRBN") : "09",
                        NumerPartneraSystemuZewnetrznego = S(row, "NumerPartneraSystemuZewnetrznego"),
                        NumerNadrzednegoPartneraSystemuZewnetrznego = S(row, "NumerNadrzednegoPartneraSystemuZewnetrznego"),

                        PartnerSprawy = false,
                        PartnerSprawySpecified = true,
                        PartnerKarty = false,
                        PartnerKartySpecified = true,

                        Skorowidz = !String.IsNullOrWhiteSpace(S(row, "Skorowidz")) ? S(row, "Skorowidz") : "Pozostali"
                    };

                    string plec = S(row, "PartnerHandlowyPlec");

                    if (!String.IsNullOrWhiteSpace(plec))
                    {
                        partner.PartnerHandlowyPlec =
                            (PozycjaDanePartneraBiznesowegoPartnerHandlowyPlec)
                            Enum.Parse(typeof(PozycjaDanePartneraBiznesowegoPartnerHandlowyPlec), plec, true);

                        partner.PartnerHandlowyPlecSpecified = true;
                    }

                    if (!String.IsNullOrWhiteSpace(S(row, "PartnerHandlowyZakladPracyNazwa")) ||
                        !String.IsNullOrWhiteSpace(S(row, "PartnerHandlowyZakladPracyNip")))
                    {
                        partner.PartnerHandlowyZakladPracy = new ZakladPracy
                        {
                            Nazwa = S(row, "PartnerHandlowyZakladPracyNazwa"),
                            Nip = S(row, "PartnerHandlowyZakladPracyNip")
                        };
                    }

                    if (addressTable != null)
                    {
                        partner.PartnerHandlowyAdresy = addressTable.AsEnumerable()
                            .Where(a =>
                                (HasColumn(a, "id_strony") && I(a, "id_strony") == idStrony) ||
                                (HasColumn(a, "idStrony") && I(a, "idStrony") == idStrony))
                            .Select(a => new Adres
                            {
                                Rodzaj = S(a, "PartnerHandlowyAdresyRodzaj"),
                                KluczKraju = !String.IsNullOrWhiteSpace(CountryCode(S(a, "PartnerHandlowyAdresyKluczKraju")))
                                    ? CountryCode(S(a, "PartnerHandlowyAdresyKluczKraju"))
                                    : "PL",
                                Miasto = S(a, "PartnerHandlowyAdresyMiasto"),
                                KodPocztowy = S(a, "PartnerHandlowyAdresyKodPocztowy"),
                                Ulica = S(a, "PartnerHandlowyAdresyUlica"),
                                NumerDomu = S(a, "PartnerHandlowyAdresyNumerDomu"),
                                Region = !String.IsNullOrWhiteSpace(RegionCode(SAny(a, new[]
                                {
                                      "PartnerHandlowyAdresyRegion",
                                      "PartnerHandlowyAdresyNumerRegion"
                                })))
                                    ? RegionCode(SAny(a, new[]
                                    {
                                          "PartnerHandlowyAdresyRegion",
                                          "PartnerHandlowyAdresyNumerRegion"
                                    }))
                                    : null//"DSL"
                            })
                            .ToArray();
                    }

                    string dokumentTyp = S(row, "PartnerHandlowyDokumentTozsamosciTyp");
                    string dokumentNumer = S(row, "PartnerHandlowyDokumentTozsamosciNumer");

                    if (!String.IsNullOrWhiteSpace(dokumentTyp) || !String.IsNullOrWhiteSpace(dokumentNumer))
                    {
                        string dokumentDataWydania = DateAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciDataWydania"
                  });

                        string dokumentDataWaznosciOd = DateAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciDataWaznosciOd",
                      "PartnerHandlowyDokumentTozsamosciDataWydaniaDataWaznosciOd"
                  });

                        string dokumentDataWaznosciDo = DateAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciDataWaznosciDo",
                      "PartnerHandlowyDokumentTozsamosciDataWydaniaDataWaznosciDo"
                  });

                        string dokumentKraj = CountryCode(SAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciKraj",
                      "PartnerHandlowyDokumentTozsamosciDataWydaniaDataWaznosciKraj"
                  }));

                        string dokumentRegion = RegionCode(SAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciRegion",
                      "PartnerHandlowyDokumentTozsamosciDataWydaniaDataWaznosciRegion"
                  }));

                        if (String.IsNullOrWhiteSpace(dokumentDataWydania))
                            dokumentDataWydania = "20200101";

                        if (String.IsNullOrWhiteSpace(dokumentDataWaznosciOd))
                            dokumentDataWaznosciOd = dokumentDataWydania;

                        if (String.IsNullOrWhiteSpace(dokumentDataWaznosciDo))
                            dokumentDataWaznosciDo = "20300101";

                        if (String.IsNullOrWhiteSpace(dokumentKraj))
                            dokumentKraj = "PL";


                        partner.PartnerHandlowyDokumentTozsamosci = new[]
                        {
                      new DokumentTozsamosci
                      {
                          Typ = dokumentTyp,
                          Numer = dokumentNumer,
                          Wydal = S(row, "PartnerHandlowyDokumentTozsamosciWydal"),
                          DataWydania = dokumentDataWydania,
                          DataWaznosciOd = dokumentDataWaznosciOd,
                          DataWaznosciDo = dokumentDataWaznosciDo,
                          Kraj = dokumentKraj,
                          Region = dokumentRegion
                      }
                  };
                    }
                    var polaKonfigurowalne = BuildPolaKonfigurowalne(row, 10);

                    var dodatkoweZdarzenia = new List<PozycjaDaneZdarzenia>();

                    if (zdarzeniaTable != null)
                    {
                        string identyfikatorWyroku = S(row, "IdentyfikatorWyrokuZSystemuZewnetrznego");

                        dodatkoweZdarzenia = zdarzeniaTable.AsEnumerable()
                            .Where(z => HasColumn(z, "id_sprawy") && I(z, "id_sprawy") == idSprawy)
                            .Select(z =>
                            {
                                string zalacznikNazwaZdarzenia = S(z, "ZalacznikNazwa");
                                string zalacznikZawartoscZdarzenia = AttachmentContent(z, "ZalacznikZawartosc");

                                return new PozycjaDaneZdarzenia
                                {
                                    DataZdarzenia = DateS(z, "DataZdarzenia"),
                                    DataKsiegowania = DateS(z, "DataZdarzenia"),
                                    IdentyfikatorWyrokuZSystemuZewnetrznego = !String.IsNullOrWhiteSpace(S(z, "IdentyfikatorWyrokuZSystemuZewnetrznego")) ? S(z, "IdentyfikatorWyrokuZSystemuZewnetrznego") : identyfikatorWyroku,
                                    TypZdarzenia = S(z, "TypZdarzenia"),
                                    ZalacznikNazwa = String.IsNullOrWhiteSpace(zalacznikNazwaZdarzenia) ? null : SafeAttachmentName(zalacznikNazwaZdarzenia),
                                    ZalacznikZawartosc = String.IsNullOrWhiteSpace(zalacznikZawartoscZdarzenia) ? null : zalacznikZawartoscZdarzenia,
                                    ListaDaneFinansowe = new PozycjaDaneFinansowe[0],
                                    ListaPlanRatalny = new PozycjaPlanRatalny[0],
                                    ListaParametryRat = new PozycjaParametryRat[0],
                                    ListaPolaKonfigurowalne = BuildPolaKonfigurowalne(z, 14)
                                };
                            })
                            .ToList();
                    }

                    var request = new ImportContentSystemDataRequest
                    {
                        GUID = Guid.NewGuid().ToString(),

                        DaneDziennika = new DaneDziennika
                        {
                            JednostkaGospodarcza = S(row, "DaneSygnaturyAktJednostkaGospodarcza"),
                            StanowiskoFinansowe = S(row, "DaneSygnaturyAktStanowiskoFinansowe"),
                            NumerWydzialuISekcji = S(row, "NumerWydzialuISekcji"),
                            Repertorium = S(row, "Repertorium")
                        },

                        ListaDanePartneraBiznesowego = new[] { partner },

                        DaneKartyDluznika = new DaneKartyDluznika
                        {
                            RodzajKarty = S(row, "DaneKartyDluznikaRodzajKarty"),
                            OznaczenieKontaUmowy = S(row, "OznaczenieKontaUmowy"),
                            NumerKontaUmowy = S(row, "NumerKontaUmowy"),
                            JednostkaGospodarcza = !String.IsNullOrWhiteSpace(S(row, "DaneKartyDluznikaJednostkaGospodarcza"))
                                ? S(row, "DaneKartyDluznikaJednostkaGospodarcza")
                                : S(row, "DaneSygnaturyAktJednostkaGospodarcza"),
                            StanowiskoFinansowe = S(row, "DaneKartyDluznikaStanowiskoFinansowe"),
                            DataKartyZdarzenia = DateS(row, "DataKartyZdarzenia")
                        },

                        DaneSygnaturyAkt = new DaneSygnaturyAkt
                        {
                            PrzedmiotyUmowy = S(row, "PrzedmiotyUmowy"),
                            RodzajPrzedmiotuUmowy = S(row, "RodzajPrzedmiotuUmowy"),
                            JednostkaGospodarcza = S(row, "DaneSygnaturyAktJednostkaGospodarcza"),
                            StanowiskoFinansowe = S(row, "DaneSygnaturyAktStanowiskoFinansowe"),
                            NumerWydzialuISekcji = S(row, "PrzedmiotyUmowyNumerWydzialuISekcji"),
                            Repertorium = S(row, "PrzedmiotyUmowyRepertorium"),
                            KolejnyNumerSprawy = S(row, "PrzedmiotyUmowyKolejnyNumerSprawy"),
                            Rok = S(row, "PrzedmiotyUmowyRok"),
                            RodzajSprawy = S(row, "PrzedmiotyUmowyRodzajSprawy"),
                            PodrodzajSprawy = S(row, "PrzedmiotyUmowyPodrodzajSprawy"),
                            JednostkaGospodarczaSygnaturaArchiwalna = S(row, "JednostkaGospodarczaSygnaturaArchiwalna"),
                            StanowiskoFinansoweSygnaturaArchiwalna = S(row, "StanowiskoFinansoweSygnaturaArchiwalna"),
                            NumerWydzialuISekcjiSygnaturaArchiwalna = S(row, "NumerWydzialuISekcjiSygnaturaArchiwalna"),
                            RepertoriumSygnaturaArchiwalna = S(row, "RepertoriumSygnaturaArchiwalna"),
                            KolejnyNumerSprawySygnaturaArchiwalna = S(row, "KolejnyNumerSprawySygnaturaArchiwalna"),
                            RokSygnaturaArchiwalna = S(row, "RokSygnaturaArchiwalna"),
                            JednostkaGospodarczaWindykacja = S(row, "JednostkaGospodarczaWindykacja"),
                            StanowiskoFinansoweWindykacja = S(row, "StanowiskoFinansoweWindykacja"),
                            KodOkreguKW = S(row, "KodOkreguKW"),
                            KontrolkaSygnaturyKW = S(row, "KontrolkaSygnaturyKW")
                        },

                        ListaDaneZdarzen = new[]
        {
            new PozycjaDaneZdarzenia
            {
                DataZdarzenia = DateS(row, "DataKartyZdarzenia"),
                DataKsiegowania = DateS(row, "DataKartyZdarzenia"),
                IdentyfikatorWyrokuZSystemuZewnetrznego = S(row, "IdentyfikatorWyrokuZSystemuZewnetrznego"),
                TypZdarzenia = S(row, "TypZdarzenia"),
                ZalacznikNazwa = zalacznikNazwa,
                ZalacznikZawartosc = zalacznikZawartosc,
        
                ListaDaneFinansowe = group
                .SelectMany(r =>
                    {
                        var lista = new List<PozycjaDaneFinansowe>();
        
                        Action<string> AddFinanse = suffix =>
                        {
                            decimal kwota = D(r, "PozycjaDaneFinansoweKwota" + suffix);
        
                            if (kwota <= 0)
                                return;
        
                            string data = DateS(r, "PozycjaDaneFinansoweData" + suffix);
                            string typ = S(r, "PozycjaDaneFinansoweTyp" + suffix);
                            string nazwa = S(r, "PozycjaDaneFinansoweNazwa" + suffix);
                            string ilosc = S(r, "PozycjaDaneFinansoweIlosc" + suffix);
                            string numer = S(r, "PozycjaDaneFinansoweNumerDokumentu" + suffix);
                            string pozycja = S(r, "PozycjaDaneFinansowePozycjaDokumentu" + suffix);
                            string operacjaGlowna = S(r, "OperacjaGlowna" + suffix);
                            string operacjaCzesciowa = S(r, "OperacjaCzesciowa" + suffix);
                            decimal kwotaSkladnika = D(r, "PozycjaDaneFinansoweKwotaSkladnika" + suffix);
        
                            if (String.IsNullOrWhiteSpace(data) ||
                                String.IsNullOrWhiteSpace(typ) ||
                                String.IsNullOrWhiteSpace(nazwa) ||
                                String.IsNullOrWhiteSpace(operacjaGlowna) ||
                                String.IsNullOrWhiteSpace(operacjaCzesciowa))
                            {
                                return;
                            }
        
                            lista.Add(new PozycjaDaneFinansowe
                            {
                                Data = data,
                                Typ = typ,
                                Nazwa = nazwa,
                                Ilosc = ilosc,
                                OperacjaGlowna = operacjaGlowna,
                                OperacjaCzesciowa = operacjaCzesciowa,
                                Kwota = kwota,
                                KwotaSkladnika = kwotaSkladnika,
                                PozycjaDokumentu = pozycja.Length > 0 ? pozycja : null,
                                NumerDokumentu = numer.Length > 0 ? numer : null
                            });
                        };
        
                        AddFinanse("Koszty");
                        AddFinanse("FPPSP");
                        AddFinanse("FPPNAW");
                        AddFinanse("PK");
                        AddFinanse("KPNawSP");
                        AddFinanse("Grzywna");
        
                        return lista;
                    })
                    .ToArray(),
        
                            ListaPlanRatalny = new PozycjaPlanRatalny[0],
                            ListaParametryRat = new PozycjaParametryRat[0],
                            ListaPolaKonfigurowalne = polaKonfigurowalne
                        }
                                }
                                .Concat(dodatkoweZdarzenia)
                                .ToArray()
                            };
                            if (String.IsNullOrWhiteSpace(request.DaneSygnaturyAkt.RepertoriumSygnaturaArchiwalna))
                                request.DaneSygnaturyAkt.RepertoriumSygnaturaArchiwalna = null;
        
                            if (String.IsNullOrWhiteSpace(request.DaneSygnaturyAkt.KolejnyNumerSprawySygnaturaArchiwalna))
                                request.DaneSygnaturyAkt.KolejnyNumerSprawySygnaturaArchiwalna = null;
        
                            if (String.IsNullOrWhiteSpace(request.DaneSygnaturyAkt.RokSygnaturaArchiwalna))
                                request.DaneSygnaturyAkt.RokSygnaturaArchiwalna = null;
        
                            // stabilizacja
                            /*
                            foreach (var p in request.ListaDanePartneraBiznesowego ?? new PozycjaDanePartneraBiznesowego[0])
                            {
                                p.PartnerHandlowyPanstwoUrodzenia = "PL";
                                p.PartnerHandlowyObywatelstwo = "PL";
        
                                p.PartnerSprawy = false;
                                p.PartnerSprawySpecified = true;
        
                                p.PartnerKarty = false;
                                p.PartnerKartySpecified = true;
        
                                if (String.IsNullOrWhiteSpace(p.Skorowidz))
                                    p.Skorowidz = "Pozostali";
        
                                if (p.PartnerHandlowyAdresy != null)
                                {
                                    p.PartnerHandlowyAdresy = p.PartnerHandlowyAdresy
                                        .Where(a => a != null && a.Rodzaj == "Koresp.")
                                        .ToArray();
        
                                    if (p.PartnerHandlowyAdresy.Length == 0)
                                    {
                                        p.PartnerHandlowyAdresy = new[]
                                        {
                          new Adres
                          {
                              Rodzaj = "Koresp.",
                              KluczKraju = "PL",
                              Miasto = "Piast",
                              KodPocztowy = "98-332",
                              Ulica = "Poloninska",
                              NumerDomu = "25",
                              Region = "PKR"
                          }
                      };
                                    }
        
                                    foreach (var a in p.PartnerHandlowyAdresy)
                                    {
                                        a.Rodzaj = "Koresp.";
                                        a.KluczKraju = "PL";
        
                                        if (String.IsNullOrWhiteSpace(a.Region))
                                            a.Region = "PKR";
                                    }
                                }
        
                                if (p.PartnerHandlowyDokumentTozsamosci != null)
                                {
                                    foreach (var d in p.PartnerHandlowyDokumentTozsamosci)
                                    {
                                        if (String.IsNullOrWhiteSpace(d.Typ))
                                            d.Typ = "Dowod osobisty";
        
                                        if (String.IsNullOrWhiteSpace(d.Numer))
                                            d.Numer = "TEMP123";
        
                                        if (String.IsNullOrWhiteSpace(d.Wydal))
                                            d.Wydal = "Urzad";
        
                                        d.DataWydania = "20200101";
                                        d.DataWaznosciOd = "20200101";
                                        d.DataWaznosciDo = "20300101";
                                        d.Kraj = "PL";
                                        d.Region = "DSL";
                                    }
                                }
                            }
                             
                            foreach (var z in request.ListaDaneZdarzen ?? new PozycjaDaneZdarzenia[0])
                            {
                                if (String.IsNullOrWhiteSpace(z.DataZdarzenia))
                                    z.DataZdarzenia = "20260101";
        
                                if (String.IsNullOrWhiteSpace(z.DataKsiegowania))
                                    z.DataKsiegowania = z.DataZdarzenia;
        
                                if (z.ListaDaneFinansowe != null)
                                {
                                    foreach (var f in z.ListaDaneFinansowe)
                                    {
                                        if (String.IsNullOrWhiteSpace(f.Data))
                                            f.Data = z.DataZdarzenia;
        
                                        if (String.IsNullOrWhiteSpace(f.Typ))
                                            f.Typ = "WYROK";
        
                                        if (String.IsNullOrWhiteSpace(f.Nazwa))
                                            f.Nazwa = "Wezwanie";
        
                                        if (String.IsNullOrWhiteSpace(f.Ilosc))
                                            f.Ilosc = "1";
        
                                        // Na potrzeby testu walidacji szyny — wartości jak w requestach, które przechodzą.
                                        if (String.IsNullOrWhiteSpace(f.OperacjaGlowna))
                                            f.OperacjaGlowna = "N010";
        
                                        if (String.IsNullOrWhiteSpace(f.OperacjaCzesciowa))
                                            f.OperacjaCzesciowa = "0020";
                                    }
                                }
        
                                if (z.ListaPolaKonfigurowalne != null)
                                {
                                    z.ListaPolaKonfigurowalne = z.ListaPolaKonfigurowalne
                                        .Where(p =>
                                            p != null &&
                                            !String.IsNullOrWhiteSpace(p.Nazwa) &&
                                            !String.IsNullOrWhiteSpace(p.Wartosc))
                                        .ToArray();
                                }
        
                                if (z.ListaPlanRatalny == null)
                                    z.ListaPlanRatalny = new PozycjaPlanRatalny[0];
        
                                if (z.ListaParametryRat == null)
                                    z.ListaParametryRat = new PozycjaParametryRat[0];
                            }
                            */
        
        
        
                            //request.DaneSygnaturyAkt.RepertoriumSygnaturaArchiwalna = null;
                            //request.DaneSygnaturyAkt.KolejnyNumerSprawySygnaturaArchiwalna = null;
                            //request.DaneSygnaturyAkt.RokSygnaturaArchiwalna = null;
                            //request.DaneSygnaturyAkt.JednostkaGospodarczaWindykacja = null;
                            //request.DaneSygnaturyAkt.StanowiskoFinansoweWindykacja = null;
                            //request.DaneSygnaturyAkt.KodOkreguKW = null;
                            //request.DaneSygnaturyAkt.KontrolkaSygnaturyKW = null;
        
                            /*
                            foreach (var p in request.ListaDanePartneraBiznesowego)
                            {
                                p.PartnerHandlowyDrugieImie = null;
                                p.PartnerHandlowyNazwa1 = null;
                                p.PartnerHandlowyNazwa2 = null;
                                p.PartnerHandlowyNazwa3 = null;
                                p.PartnerHandlowyNazwa4 = null;
                                p.PartnerHandlowyRegon = null;
                                p.PartnerHandlowyNip = null;
                                p.PartnerHandlowyInneObywatelstwa = null;
                                p.PartnerHandlowyStatusZatrudnienia = null;
                                p.PartnerHandlowyWyksztalcenie = null;
                                p.PartnerHandlowyWykonywanieFunkcji = null;
                                p.PartnerHandlowyPobytZakladKarny = null;
                                p.PartnerHandlowyObronca = null;
                                p.Krs = null;
                                p.NumerNadrzednegoPartneraSystemuZewnetrznego = null;
                            }
        
                            foreach (var p in request.ListaDanePartneraBiznesowego)
                            {
                                if (p.PartnerHandlowyDokumentTozsamosci != null)
                                {
                                    foreach (var d in p.PartnerHandlowyDokumentTozsamosci)
                                    {
                                        d.Typ = "Dowod osobisty";
                                        d.Wydal = "Urzad Miasta Tychy";
                                    }
                                }
                            }
                            */
                            // koniec stabilizacji
        
        
        
                            result.Add(new ConsImportData
                            {
                                IdSprawy = idSprawy,
                                IdStrony = idStrony,
                                status = ConsImportStatus.Prepared,
                                importContentSystemDataRequest = request
                            });
                        }
                    }

            return result;
        }

        private static string MakeUniqueFileName( string fileName, HashSet<string> usedNames)
                {
                    if (usedNames.Add(fileName))
                        return fileName;
        
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    string extension = Path.GetExtension(fileName);
        
                    int number = 2;
        
                    while (true)
                    {
                        string candidate =
                            name + "_" + number + extension;
        
                        if (usedNames.Add(candidate))
                            return candidate;
        
                        number++;
                    }
                }

        private static byte[] GetDocumentContent(object value)
                {
                    if (value == null || value == DBNull.Value)
                        return null;
        
                    if (value is byte[])
                    {
                        byte[] raw = (byte[])value;
        
                        try
                        {
                            byte[] decompressed =
                                Utils.DecompressMsWord(raw);
        
                            if (decompressed != null &&
                                decompressed.Length > 0)
                            {
                                return decompressed;
                            }
                        }
                        catch
                        {
                            // Dane mogą już być nieskompresowanym dokumentem.
                        }
        
                        return raw;
                    }
        
                    string text = Convert.ToString(value);
        
                    if (String.IsNullOrWhiteSpace(text))
                        return null;
        
                    try
                    {
                        return Convert.FromBase64String(
                            text.Trim());
                    }
                    catch (FormatException ex)
                    {
                        throw new InvalidOperationException(
                            "Zawartość dokumentu nie jest poprawnym Base64.",
                            ex);
                    }
                }

        private static string GetDocumentExtension( byte[] documentContent)
                {
                    if (documentContent == null ||
                        documentContent.Length < 4)
                    {
                        return ".doc";
                    }
        
                    // DOCX jest archiwum ZIP i zazwyczaj zaczyna się od PK 03 04.
                    if (documentContent[0] == 0x50 &&
                        documentContent[1] == 0x4B &&
                        documentContent[2] == 0x03 &&
                        documentContent[3] == 0x04)
                    {
                        return ".docx";
                    }
        
                    // Stary binarny format Microsoft Word.
                    if (documentContent.Length >= 8 &&
                        documentContent[0] == 0xD0 &&
                        documentContent[1] == 0xCF &&
                        documentContent[2] == 0x11 &&
                        documentContent[3] == 0xE0)
                    {
                        return ".doc";
                    }
        
                    return ".doc";
                }

        private static string BuildZipBase64(
   IEnumerable<DataRow> documentRows,
   Func<string, string> safeAttachmentName)
        {
            if (documentRows == null)
                return null;

            using (var zipStream = new MemoryStream())
            {
                int documentNumber = 1;
                int documentCount = 0;

                var usedNames = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

                using (var archive = new ZipArchive(
                    zipStream,
                    ZipArchiveMode.Create,
                    true))
                {
                    foreach (DataRow documentRow in documentRows)
                    {
                        if (documentRow == null ||
                            documentRow.Table == null ||
                            !documentRow.Table.Columns.Contains("msword") ||
                            documentRow["msword"] == null ||
                            documentRow["msword"] == DBNull.Value)
                        {
                            continue;
                        }

                        byte[] documentContent =
                            GetDocumentContent(documentRow["msword"]);

                        if (documentContent == null ||
                            documentContent.Length == 0)
                        {
                            continue;
                        }

                        string sourceFileName = null;

                        if (documentRow.Table.Columns.Contains("nazwa") &&
                            documentRow["nazwa"] != DBNull.Value)
                        {
                            sourceFileName =
                                Convert.ToString(documentRow["nazwa"]);
                        }

                        if (String.IsNullOrWhiteSpace(sourceFileName))
                        {
                            sourceFileName =
                                "orzeczenie_" +
                                documentNumber.ToString("000") +
                                GetDocumentExtension(documentContent);
                        }
                        else
                        {
                            sourceFileName =
                                safeAttachmentName(sourceFileName);
                        }

                        string uniqueFileName =
                            MakeUniqueFileName(
                                sourceFileName,
                                usedNames);

                        ZipArchiveEntry entry =
                            archive.CreateEntry(
                                uniqueFileName,
                                CompressionLevel.Optimal);

                        using (Stream entryStream = entry.Open())
                        {
                            entryStream.Write(
                                documentContent,
                                0,
                                documentContent.Length);
                        }

                        documentNumber++;
                        documentCount++;
                    }
                }

                if (documentCount == 0)
                    return null;

                return Convert.ToBase64String(
                    zipStream.ToArray());
            }
        }

        private DataSet ExecuteStoredProcedure(ConsExternalDBConnectionConfig knf, DateTime odDnia, DateTime doDnia)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand storedProcCommand = new SqlCommand(knf.sp_name, con))
                    {
                        storedProcCommand.CommandType = CommandType.StoredProcedure;
                        storedProcCommand.Parameters.AddWithValue("@sourcesrv", (String.IsNullOrEmpty(knf.srvAlias) ? knf.srvName : knf.srvAlias));
                        storedProcCommand.Parameters.AddWithValue("@dbname", knf.DbName);
                        storedProcCommand.Parameters.AddWithValue("@nazwaDok", knf.sp_param);
                        storedProcCommand.Parameters.AddWithValue("@dataOd", odDnia);
                        storedProcCommand.Parameters.AddWithValue("@dataDo", doDnia);
                        storedProcCommand.Parameters.AddWithValue("@tryb", knf.SAPKnsId);

                        storedProcCommand.CommandTimeout = 600;
                        storedProcCommand.Connection = con;
                        SqlDataAdapter da = new SqlDataAdapter();

                        da.SelectCommand = storedProcCommand;
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;

                    }

                }



            }
            catch (Exception ex)
            {
                log.Error("Błąd odczytu danych przy użyciu " + knf.sp_name, ex);
                return null;

            }


        }

    }

    public sealed class ImportProcessResult
    {
        public int CreatedJobs { get; set; }
        public int CompletedJobs { get; set; }
        public int FailedJobs { get; set; }
        public int PreparedTransfers { get; set; }
    }

    internal sealed class JobProcessResult
    {
        public bool Success { get; private set; }
        public int PreparedTransfers { get; private set; }

        public static JobProcessResult Succeeded(int preparedTransfers)
        {
            return new JobProcessResult
            {
                Success = true,
                PreparedTransfers = preparedTransfers
            };
        }

        public static JobProcessResult Failed()
        {
            return new JobProcessResult
            {
                Success = false,
                PreparedTransfers = 0
            };
        }
    }
}
