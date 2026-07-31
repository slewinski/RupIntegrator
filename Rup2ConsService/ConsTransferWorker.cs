using Cons2RupModel;
using ConsImport;
using ConsInterfeces.Rup2ConsImportContentSystemData;
using RupDatabase;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Rup2ConsService
{
    /// <summary>
    /// Pobiera pojedyncze rekordy ConsKartaTransfer i wysyła je do SAP.
    /// Każda instancja może być używana przez jeden worker.
    /// </summary>
    public sealed class ConsTransferWorker
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly int _idleDelayMilliseconds;

        public ConsTransferWorker(int idleDelayMilliseconds)
        {
            if (idleDelayMilliseconds < 1)
                throw new ArgumentOutOfRangeException("idleDelayMilliseconds");

            _idleDelayMilliseconds = idleDelayMilliseconds;
        }
        /// <summary>
        /// Przetwarza jednorazowo wskazany rekord ConsKartaTransfer.
        /// Rekord musi mieć status Prepared.
        /// </summary>
        /// <returns>
        /// true – rekord został przejęty i podjęto jego przetwarzanie,
        /// false – rekord nie istnieje albo nie ma statusu Prepared.
        /// </returns>
        public bool ProcessSingleTransfer(int transferId)
        {
            if (transferId <= 0)
                throw new ArgumentOutOfRangeException("transferId");

            bool claimed = TryClaimSpecificTransfer(transferId);

            if (!claimed)
            {
                log.Warn(
                    "Nie przejęto wskazanego transferu Id=" + transferId +
                    ". Rekord nie istnieje albo nie ma statusu Prepared.");

                return false;
            }

            ProcessTransfer(transferId);
            return true;
        }
        public void Run(CancellationToken token)
        {
            while (IsDebugMode || !token.IsCancellationRequested)
            {
                try
                {
                    int? transferId = TryClaimNextTransfer();

                    if (!transferId.HasValue)
                    {
                        Wait(token);
                        continue;
                    }

                    ProcessTransfer(transferId.Value);
                }
                catch (Exception ex)
                {
                    log.Error("Nieobsłużony błąd workera transferu CONS do SAP.", ex);
                    Wait(token);
                }
            }
        }


        /// <summary>
        /// Atomowo przejmuje jeden rekord Prepared. READPAST powoduje, że inne
        /// workery omijają rekord już zablokowany przez bieżący worker.
        /// </summary>
        private int? TryClaimNextTransfer()
        {
            const string sql = @"
                ;WITH next_item AS
                (
                    SELECT TOP (1) Id, status, dImportu
                    FROM dbo.ConsKartaTransfer WITH (ROWLOCK, READPAST, UPDLOCK)
                    WHERE status = @PreparedStatus
                    ORDER BY Id
                )
                UPDATE next_item
                SET status = @PendingStatus,
                    dImportu = GETDATE()
                OUTPUT INSERTED.Id;";

            using (var context = new RupDBEntities())
            {
                DbConnection connection = context.Database.Connection;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (DbTransaction transaction =
                    connection.BeginTransaction(IsolationLevel.ReadCommitted))
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;

                    AddParameter(command, "@PreparedStatus", (int)ConsImportStatus.Prepared);
                    AddParameter(command, "@PendingStatus", (int)ConsImportStatus.Pending);

                    object result = command.ExecuteScalar();
                    transaction.Commit();

                    if (result == null || result == DBNull.Value)
                        return null;

                    return Convert.ToInt32(result);
                }
            }
        }
        /// <summary>
        /// Atomowo zmienia status wskazanego rekordu z Prepared na Pending.
        /// </summary>
        private bool TryClaimSpecificTransfer(int transferId)
        {
            const string sql = @"
            UPDATE dbo.ConsKartaTransfer WITH (ROWLOCK, UPDLOCK)
            SET status = @PendingStatus,
            dImportu = GETDATE()
            WHERE Id = @TransferId
            AND status = @PreparedStatus;
            SELECT @@ROWCOUNT;";

            using (var context = new RupDBEntities())
            {
                DbConnection connection = context.Database.Connection;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (DbTransaction transaction =
                    connection.BeginTransaction(IsolationLevel.ReadCommitted))
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;

                    AddParameter(
                        command,
                        "@TransferId",
                        transferId);

                    AddParameter(
                        command,
                        "@PreparedStatus",
                        (int)ConsImportStatus.Prepared);

                    AddParameter(
                        command,
                        "@PendingStatus",
                        (int)ConsImportStatus.Pending);

                    object result = command.ExecuteScalar();

                    transaction.Commit();

                    return result != null &&
                           result != DBNull.Value &&
                           Convert.ToInt32(result) == 1;
                }
            }
        }
        private void ProcessTransfer(int transferId)
        {
            ConsKartaTransfer transfer;

            using (var context = new RupDBEntities())
            {
                transfer = context.ConsKartaTransfer
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Id == transferId);


                if (transfer == null)
                {
                    log.Warn("Nie znaleziono ConsKartaTransfer Id=" + transferId + ".");
                    return;
                }

                try
                {
                    if (String.IsNullOrWhiteSpace(transfer.payload))
                        throw new InvalidOperationException("Payload transferu jest pusty.");

                    ImportContentSystemDataRequest request =
                        DeserializeRequest(transfer.payload);

                    if ( transfer.guidImport!= null)
                        request.GUID = transfer.guidImport.ToString();



                    if (request == null)
                        throw new InvalidOperationException("Nie udało się odtworzyć komunikatu z payload.");

                    
                    string requestXml;
                    string idKomunikatu = String.IsNullOrWhiteSpace(transfer.idKomunikatu) ? Guid.NewGuid().ToString() : transfer.idKomunikatu;

                    ImportContentSystemDataResponse response =
                        ConsWebServiceHelper.ImportData(
                            "ImportContentSystemData",
                            request, idKomunikatu,
                            out requestXml);

                    if (response == null)
                    {   log.Error("Transfer CONS do SAP zakończył się błędem. Id=" + transferId +
                            ". SAP zwrócił pustą odpowiedź.");  
                        throw new InvalidOperationException("SAP zwrócił pustą odpowiedź.");
                    }
                    string errorMessage;
                    string responseXml = SerializeResponse(response);


                    if (HasErrors(response, out errorMessage))
                    {
                        MarkAsError(
                            transferId,
                            Truncate(errorMessage + Environment.NewLine + responseXml, 4000));

                        log.Error(
                            "Transfer CONS do SAP zakończył się błędem. Id=" + transferId +
                            ". " + errorMessage);

                        return;
                    }
                    MarkAsDone(transferId, responseXml, idKomunikatu);

                }
                catch (Exception ex)
                {
                    MarkAsError(transferId, GetFullExceptionMessage(ex));

                    log.Error(
                        "Błąd transferu CONS do SAP. Id=" + transferId +
                        ", idKomunikatu=" + transfer.idKomunikatu + ".",
                        ex);
                }
            }
        }



        private static void MarkAsDone(int transferId, string responseXml, string idKomunikatu)
        {
            using (var context = new RupDBEntities())
            {
                ConsKartaTransfer transfer = context.ConsKartaTransfer
                    .SingleOrDefault(x => x.Id == transferId);

                if (transfer == null)
                    return;

                transfer.status = (int)ConsImportStatus.Done;
                transfer.dImportu = DateTime.Now;
                transfer.trescOdpowiedzi = Truncate(responseXml, 4000);
                transfer.idKomunikatu = idKomunikatu;
                context.SaveChanges();
            }
        }

        private static void MarkAsError(int transferId, string errorMessage)
        {
            using (var context = new RupDBEntities())
            {
                ConsKartaTransfer transfer = context.ConsKartaTransfer
                    .SingleOrDefault(x => x.Id == transferId);

                if (transfer == null)
                    return;

                transfer.status = (int)ConsImportStatus.Error;
                transfer.dImportu = DateTime.Now;
                transfer.trescOdpowiedzi = Truncate(errorMessage, 8000);
                context.SaveChanges();
            }
        }

        public static int RecoverStalePendingTransfers(int staleAfterMinutes)
        {
            if (staleAfterMinutes <= 0)
                return 0;

            DateTime limit = DateTime.Now.AddMinutes(-staleAfterMinutes);

            using (var context = new RupDBEntities())
            {
                var stale = context.ConsKartaTransfer
                    .Where(x =>
                        x.status == (int)ConsImportStatus.Pending &&
                        x.dImportu.HasValue &&
                        x.dImportu.Value < limit)
                    .ToList();

                foreach (ConsKartaTransfer transfer in stale)
                {
                    transfer.status = (int)ConsImportStatus.Prepared;
                    transfer.trescOdpowiedzi =
                        "Przywrócono do kolejki po przerwanym przetwarzaniu.";
                }

                if (stale.Count > 0)
                    context.SaveChanges();

                return stale.Count;
            }
        }

        private static ImportContentSystemDataRequest DeserializeRequest(string xml)
        {
            var serializer = new XmlSerializer(typeof(ImportContentSystemDataRequest));

            using (var reader = new StringReader(xml))
            {
                return (ImportContentSystemDataRequest)serializer.Deserialize(reader);
            }
        }

        private static string SerializeResponse(ImportContentSystemDataResponse response)
        {
            var serializer = new XmlSerializer(typeof(ImportContentSystemDataResponse));
            var builder = new StringBuilder();

            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, response);
            }

            return builder.ToString();
        }

        private static void AddParameter(DbCommand command, string name, int value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = DbType.Int32;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        private static string GetFullExceptionMessage(Exception exception)
        {
            var builder = new StringBuilder();
            Exception current = exception;

            while (current != null)
            {
                if (builder.Length > 0)
                    builder.Append(" | ");

                builder.Append(current.Message);
                current = current.InnerException;
            }

            return builder.ToString();
        }

        private static string Truncate(string value, int maximumLength)
        {
            if (String.IsNullOrEmpty(value) || value.Length <= maximumLength)
                return value;

            return value.Substring(0, maximumLength);
        }
        private void Wait(CancellationToken token)
        {
            if (IsDebugMode)
                Thread.Sleep(_idleDelayMilliseconds);
            else
                token.WaitHandle.WaitOne(_idleDelayMilliseconds);
        }

        private static bool IsSuccessfulResponse(
        ImportContentSystemDataResponse response,
        out string errorMessage)
        {
            errorMessage = null;


            return false;
        }

        private static bool IsDebugMode
        {
            get
            {
#if DEBUG
                return true;
#else
        return false;
#endif
            }
        }
        private static bool HasErrors( ImportContentSystemDataResponse response, out string errorMessage)
        {
            errorMessage = null;

            if (response == null || response.ListaKomunikat == null)
            {
                errorMessage = "Brak odpowiedzi z SAP.";
                return true;
            }

            var errors = response.ListaKomunikat
                .Where(k => String.Equals(k.TypKomunikatu, "E",
                                          StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (errors.Count == 0)
                return false;

            errorMessage = String.Join(
                Environment.NewLine,
                errors.Select(k =>
                    String.Format(
                        "[{0}] {1}",
                        k.NumerKomunikatu,
                        k.TrescKomunikatu)));

            return true;
        }
    }
}
