using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RupLoader
{
  public class rStruct
    {
       public  int lp { get; set; }
       public int status{get; set;}
       public string msg { get; set; }
       public string  ImieNazwisko {get;set;}
       public int IdKuratora {get;set;}
       public string NumerKuratora { get; set; }
       public string SadOrzek {get;set;}
       public int IdSadOrzek {get;set;}
       public string SapSad { get; set; }
       public string SapSadOpis { get; set; }
       public string Sygnatura	{get;set;}
       public string SWydzial { get; set; }
       public string SRepertorium { get; set; }
       public string SNumer { get; set; }
       public string SRok { get; set; }
       public string SRodzajPrzedm { get; set; }
       public string SRodzaj { get; set; }
       public string NrRachunku	 {get;set;}
       public DateTime? DataWydZarz	{get;set;}
       public DateTime? DataWplZarz	{get;set;}
       public string    PowoDodRozl	{get;set;}
       public string    TypRozl	{get;set;}
       public string    IdListyPlac	{get;set;}
       public string    Skladnik	{get;set;}
       public decimal   Kwota	{get;set;}
       public decimal   ZwKosztDojSkladnik{get;set;}	
       public decimal   ZwKosztDojKWSkladnik	{get;set;}
       public int        LWywiadow	{get;set;}
       public int        LNadzorow	{get;set;}
       public string     WywiadDaneOsob	{get;set;}
       public string     Uwagi	{get;set;}
       public string     StatusDokumentu	{get;set;}
       public decimal WydatekIncydantalny { get; set; }
       public string  SygnDbName { get; set; }
       public string SygnSrvName { get; set; }
       public int IdCofDB { get; set; }
       public string RodzWypl { get; set; }
       public decimal PotracZaliczki { get; set; }
       public decimal ZwrotKosztKwt2 { get; set; }
       public string ZwrotKosztSkladnik2 { get; set; }
       public decimal ProcDofin { get; set; }
    }

  public class outStruct
  {
       public string ImieNazwisko { get; set; }
       public string NumerKuratora { get; set; }
       public string WazneOd { get; set; }
       public string WazneDo { get; set; }
       public string Sygnatura { get; set; }
       public string NrRachunku { get; set; }
       public string DataWydZarz { get; set; }
       public string DataWplZarz { get; set; }
       public string DataPlatnosci { get; set;}
       public string PowoDodRozl { get; set; }
      public string TypRozl { get; set; }
      public string IdListyPlac { get; set; }
      public string Skladnik { get; set; }
      public decimal Kwota { get; set; }
      public string ZwKosztDojSkladnik { get; set; }
      public string ZwKosztDojKWSkladnik { get; set; }
      public string LWywiadow { get; set; }
      public string LNadzorow { get; set; }
      public string WywiadDaneOsob { get; set; }
      public string Uwagi { get; set; }
      public string StatusDokumentu { get; set; }
      public string WydatekIncydantalny { get; set; }

      public string RodzWypl { get; set; }
      public decimal PotracZaliczki { get; set; }
      public decimal ZwrotKosztKwt2 { get; set; }
      public string ZwrotKosztSkladnik2 { get; set; }
      public decimal ProcDofin { get; set; }
      public string OkresWymag { get; set; }

    }

   public  class RyczaltyService
    {

      public List<rStruct>  lst;
      private int index;

       public RyczaltyService ()
       {
         lst = new List<rStruct>();
         index = 0;
       }

       public List<rStruct> GetAallList()
       {
           return lst;
       
       }


       




       public string GetRyczaltyByDB (int idDB, DateTime od, DateTime dodnia, string filter, ref Label reslabel)
       {

           
           string retcode = "";
           SqlDataReader rdr = null;
           SqlConnection con = null;
           SqlCommand storedProcCommand;
           DataRow dr_save = null ;
         
           
       try
       {
       
           using (RupIntegratorEntities dbContext    = new RupIntegratorEntities())
           {

           
               RL_Konfig cnf = dbContext.RL_Konfig.Where(a=>a.id == idDB).FirstOrDefault();
               if ( cnf == null ) return "Brak zdefiniowanego dostępu do bazy danych ";

                  string ConnectionString = ConfigurationManager.ConnectionStrings["RupLoader.Properties.Settings.RupDB"].ConnectionString;
                   con = new SqlConnection(ConnectionString);
                   //con.Open();
                     if (String.IsNullOrWhiteSpace(cnf.sp_name)) return "Nie zdefiniowano procedury składowanej do obsługi ryczłtów";

                       storedProcCommand = new SqlCommand(cnf.sp_name, con);


                   storedProcCommand.CommandType = CommandType.StoredProcedure;
                   storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(cnf.srvAlias) ? cnf.srvName : cnf.srvAlias));
                   storedProcCommand.Parameters.Add("@dbname", cnf.DbName);
                   storedProcCommand.Parameters.Add("@dataod", od);
                   storedProcCommand.Parameters.Add("@datado",  dodnia );
                   storedProcCommand.Parameters.Add("@what",filter);
                   storedProcCommand.Parameters.Add("@IdZespolu", cnf.WSLogon);
                

                   storedProcCommand.CommandTimeout = 600;
                   storedProcCommand.Connection = con;
                   SqlDataAdapter da = new SqlDataAdapter();
                   Cursor.Current = Cursors.WaitCursor;
                   da.SelectCommand = storedProcCommand;
                   da.SelectCommand.CommandType = CommandType.StoredProcedure;
                   reslabel.Text = "Odczyt ryczałtów - łączenie z bazą... ";
                   reslabel.Refresh();
                   DataTable dt = new DataTable();
                   da.Fill(dt);
                   int i = 0; 
                   
                   int j = dt.Rows.Count;
                   if (dt.Rows.Count > 0)
                   {
                       foreach (DataRow dr in dt.Rows)
                       {
                           dr_save = dr;

                           rStruct r = new rStruct();
                           r.lp = ++index;
                           reslabel.Text = "Odczyt ryczałtu  " + (++i).ToString() + " z " + j.ToString() + " " + dr["Sygnatura"] as string;
                           reslabel.Refresh();
  
                            r.ImieNazwisko = dr["ImieNazwisko"] as string;
                            r.IdKuratora = dr["IdKuratora"] is DBNull ? 0 : Convert.ToInt32(dr["IdKuratora"]);
                           r.SadOrzek = dr["SadOrzek"] as string;
                            r.IdSadOrzek = dr["IdSadOrzek"] is DBNull ? 0 : Convert.ToInt32(dr["IdSadOrzek"]) ;
                           r.Sygnatura = dr["Sygnatura"] as string;
                            r.NrRachunku = dr["NrRachunku"] as string;
                           r.DataWydZarz = Convert.ToDateTime(dr["DataWydZarz"]);
                            r.DataWplZarz = Convert.ToDateTime(dr["DataWplZarz"]);
                           r.PowoDodRozl = dr["PowoDodRozl"] as string;
                            r.TypRozl = dr["TypRozl"] as string;
                           r.IdListyPlac = dr["IdListyPlac"] as string;
                            r.Skladnik = dr["Skladnik"] as string;
                           r.Kwota = Convert.ToDecimal(dr["Kwota"]);
                            r.ZwKosztDojSkladnik = Convert.ToDecimal(dr["ZwKosztDojSkladnik"]);
                            r.ZwKosztDojKWSkladnik = Convert.ToDecimal(dr["ZwKosztDojKWSkladnik"]);
                           r.LWywiadow = Convert.ToInt32(dr["LWywiadow"]);
                            r.LNadzorow = Convert.ToInt32(dr["LNadzorow"]);
                           r.WywiadDaneOsob = dr["WywiadDaneOsob"] as string;
                            r.Uwagi = dr["Uwagi"] as string;
                            r.StatusDokumentu = dr["StatusDokumentu"] as string;
                            r.SygnDbName = cnf.DbName;
                            r.SygnSrvName = cnf.srvName;
                            r.IdCofDB = cnf.id;
                            r.WydatekIncydantalny = dr["WydatekIncydantalny"] is DBNull ? 0: Convert.ToDecimal(dr["WydatekIncydantalny"]);
                            try
                            {
                                r.RodzWypl = dr["RodzWypl"] as string;
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            try
                            {
                                r.PotracZaliczki = dr["PotracZaliczki"] is DBNull ? 0 : Convert.ToDecimal(dr["PotracZaliczki"]);
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            try
                            {
                                r.ZwrotKosztKwt2 = dr["ZwrotKosztKwt2"] is DBNull ? 0 : Convert.ToDecimal(dr["ZwrotKosztKwt2"]);
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            try
                            {
                                r.ZwrotKosztSkladnik2 = dr["ZwrotKosztSkladnik2"] as string;
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            try
                            {
                                r.ProcDofin = dr["ProcDofin"] is DBNull ? 0 : Convert.ToDecimal(dr["ProcDofin"]);
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                            KuratSad ks = dbContext.KuratSad.Where(a => a.dbname == r.SygnDbName && a.srvname == r.SygnSrvName && a.Sad_Id == r.IdSadOrzek).FirstOrDefault();
                           if (ks != null)
                               r.SapSad = ks.SAPSad_Id;
                             KuratMap kur = dbContext.KuratMap.Where(a => a.DbId == r.IdKuratora && a.typPartner == RupDatabase.typPartner && a.servername == r.SygnSrvName && a.dbname == r.SygnDbName).FirstOrDefault();
                            if (kur != null)
                               r.NumerKuratora = kur.SAPId;
                            lst.Add(r);

                       }
                   }
                   Cursor.Current = Cursors.Default;
           }
       }
               
               catch (Exception ex)
       {
                   Cursor.Current = Cursors.Default;
                   // Print error message
                   return ex.Message + " "  + ((ex.InnerException == null) ? "" : ex.InnerException.Message) + (dr_save != null ? dr_save["Sygnatura"] as string:""); 
               }
     
           return retcode;
       }
    }
}
