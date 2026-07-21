using System;
using System.ServiceProcess;

namespace ConsImporterService
{
    internal static class Program
    {
        private static void Main()
        {
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
    }
}