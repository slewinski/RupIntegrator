using log4net.Config;
using System;
using System.IO;
using System.ServiceProcess;

namespace ConsImporterService
{
    internal static class Program
    {
        private static void Main()
        {
            ConfigureLog4Net();


#if DEBUG
            try
            {
                Console.WriteLine(
                    "Uruchomienie ConsImporterService w trybie DEBUG.");

                var service =
                    new ConsImportService();

                service.DebugRunOnce();

                Console.WriteLine();
                Console.WriteLine(
                    "Import zakończony. Naciśnij ENTER...");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Wystąpił błąd:");
                Console.WriteLine(ex);
            }

            Console.ReadLine();
#else
            ServiceBase.Run(
                new ServiceBase[]
                {
                    new ConsImportService()
                });
#endif
        }
        private static void ConfigureLog4Net()
        {
            string configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "log4net.config");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException(
                    "Nie znaleziono pliku konfiguracji log4net.",
                    configPath);
            }

            XmlConfigurator.ConfigureAndWatch(
                new FileInfo(configPath));
        }
    }
}