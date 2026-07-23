using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnsMigrator
{
    public    enum Operations { Przypisy, Odpisy, Potwierdzenia, UGO, Export };
    public static class UserInfo
    {
      public static  int Id{get;set;} 
      public static string  Username {get;set;} 
      public static int role {get;set;}
      public static string MEPUser { get; set; }
      public static string MEPPassword { get; set; }  
	  
    }

    
    public static class ExportDetails
    {
        public static int IdTransfer { get; set; }
        
    }


    public static class RunMode
    {
        public static bool silentMode { get; set; }
        public static Operations operation { get; set; }
        public static string fileName { get; set; }
        public static int grKsiag { get; set; }
        public static int tyOpExport { get; set; }
        public static string dbversion = "3.7.1";
    }


}
