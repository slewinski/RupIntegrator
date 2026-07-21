using log4net;
using log4net.Config;
using System;
using System.Globalization;
using System.IO;
using System.ServiceProcess;

namespace Rup2ConsService
{
    internal static class Program
    {
        private static readonly ILog log =
            LogManager.GetLogger(typeof(Program));

        private static int Main(string[] args)
        {
            var repository =
                LogManager.GetRepository(
                    System.Reflection.Assembly
                        .GetExecutingAssembly());

            XmlConfigurator.Configure(
                repository,
                new FileInfo("log4net.config"));

            try
            {
                int? transferId =
                    GetTransferId(args);

                /*
                 * Podanie --transfer-id zawsze uruchamia aplikację
                 * w trybie jednorazowym, niezależnie od DEBUG/RELEASE.
                 */
                if (transferId.HasValue)
                {
                    return RunSingleTransfer(
                        transferId.Value);
                }

#if DEBUG
                new Rup2ConsService().DebugRun();
                return 0;
#else
                ServiceBase.Run(
                    new ServiceBase[]
                    {
                        new Rup2ConsService()
                    });

                return 0;
#endif
            }
            catch (Exception ex)
            {
                log.Fatal(
                    "Błąd uruchomienia aplikacji.",
                    ex);

#if DEBUG
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine(
                    "Naciśnij ENTER, aby zakończyć.");

                Console.ReadLine();
#endif

                return 1;
            }
        }

        private static int RunSingleTransfer(int transferId)
        {
            log.Info(
                "Tryb jednorazowy. Transfer Id=" +
                transferId + ".");

            var service =
                new Rup2ConsService();

            bool processed = service.RunSingleTransfer(transferId);

            if (!processed)
            {
                log.Warn(
                    "Transfer Id=" + transferId +
                    " nie został przetworzony.");

                return 2;
            }

            return 0;
        }

        private static int? GetTransferId(
            string[] args)
        {
            if (args == null ||
                args.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i];

                if (String.IsNullOrWhiteSpace(argument))
                    continue;

                /*
                 * Obsługiwane warianty:
                 *
                 * --transfer-id=123
                 * /transfer-id:123
                 * --transfer-id 123
                 */

                const string longPrefix =
                    "--transfer-id=";

                const string windowsPrefix =
                    "/transfer-id:";

                if (argument.StartsWith(
                    longPrefix,
                    StringComparison.OrdinalIgnoreCase))
                {
                    string value =
                        argument.Substring(
                            longPrefix.Length);

                    return ParseTransferId(value);
                }

                if (argument.StartsWith(
                    windowsPrefix,
                    StringComparison.OrdinalIgnoreCase))
                {
                    string value =
                        argument.Substring(
                            windowsPrefix.Length);

                    return ParseTransferId(value);
                }

                if (argument.Equals(
                    "--transfer-id",
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 >= args.Length)
                    {
                        throw new ArgumentException(
                            "Brak wartości po parametrze --transfer-id.");
                    }

                    return ParseTransferId(
                        args[i + 1]);
                }
            }

            return null;
        }

        private static int ParseTransferId(
            string value)
        {
            int transferId;

            if (!Int32.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out transferId) ||
                transferId <= 0)
            {
                throw new ArgumentException(
                    "Nieprawidłowa wartość transfer-id: '" +
                    value + "'.");
            }

            return transferId;
        }
    }
}