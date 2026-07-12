using System;
using System.ServiceProcess;

namespace ConsImporterService
{
    static class Program
    {
        static void Main()
        {
#if DEBUG

            Console.WriteLine("DEBUG MODE");
            Console.WriteLine();

            var service = new ConsImportService();

            service.DebugRun();

            Console.WriteLine();
            Console.WriteLine("Koniec. Naciśnij ENTER...");
            Console.ReadLine();

#else

            ServiceBase.Run(new ConsImportService());

#endif
        }
    }
}