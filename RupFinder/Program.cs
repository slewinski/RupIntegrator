using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RupFinder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            RupFinder finderWin;
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            finderWin = new RupFinder();
            if (args.GetLength(0) == 1) // wczytywanie
            {

                // parametry
                finderWin.inArg = args[0];


            }
            else if (args.GetLength(0) == 2)
            {

                finderWin.mode = args[0];
                finderWin.inArg = args[1];
            
            }
            Application.Run(finderWin);
        }
    }
}
