using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RupLoader
{
    public    enum Operations { Wyciag, Odpisy, Potwierdzenia };
    public static class UserInfo
    {
        public static int Id { get; set; }
        public static string Username { get; set; }
        public static int role { get; set; }
        public static string MEPUser { get; set; }
        public static string MEPPassword { get; set; }
    }


    public static class ExportDetails
    {
        public static int IdTransfer { get; set; }
        
    }
    public static class RupDatabase
    {
        public static RupIntegratorEntities theContext { get; set; }
        public static Konfiguracja theConfig { get; set;}
        public static string jg { get; set; }
        public static int typPartner { get; set; }
       
    }
    
    public static class RunMode
    {
        public static bool silentMode { get; set; }
        public static Operations operation { get; set; }
        public static string fileName { get; set; }
        public static string data { get; set; }
        public static Recognizer wndHandler { get; set; }
        public static string CmdFileName {get; set;}
        public static string WinMode { get; set; }
        public static string dbVersion { get; set; }
    }


}
