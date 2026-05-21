using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Data.Objects;
using static log4net.Appender.RollingFileAppender;
using System.Web.UI.WebControls;
using Telerik.WinControls;

namespace KnsMigrator
{
    public partial class MigrForm 
    {
        


        private void InitKonfig()
        {
            var dict = new Dictionary<string, string>();


            List<SAPSad> slst = thecontext.SAPSad.Where(a => a.typSad == "SF" || a.kod == "").OrderBy(a => a.miasto).ToList();
            foreach (SAPSad row in slst)
            {
                dict.Add(row.kod, row.miastSad + " (" + row.kod + ")");
            }

            this.KonfigSource.DataSource = thecontext.SAPSad.Where(a=>a.typSad != "SF").ToList().OrderBy(a => a.miasto);
            this.rddlJedGosp.DataSource = this.KonfigSource;
            this.rddlJedGosp.DisplayMember = "miastSad";
            this.rddlJedGosp.ValueMember = "kod";

            this.KonfigSource1.DataSource = dict;//thecontext.SAPSad.Where(a => a.typSad == "SF" || a.kod=="").ToList().OrderBy(a => a.miasto);
            this.rddStanFin.DataSource = this.KonfigSource1;
            this.rddStanFin.DisplayMember = "Value";//"miastSad";
            this.rddStanFin.ValueMember = "Key";//"kod";
            
            
            this.rddlJedGosp.SelectedValue = konfig.JednostkaGospodarcza;
            this.rddStanFin.SelectedValue = konfig.StanowiskoFin;

            this.chkskipRaty.Checked = konfig.skipraty as bool? ?? default(bool);
            this.cbSkipSadEmpty.Checked = konfig.defSad as bool? ?? default(bool);
            // połączenie do bazy KNS
            this.rddlbDostawca.SelectedIndex = konfig.typKns as int? ?? default(int);
            this.tbServer.Text = konfig.srvName;
            this.tbAlias.Text = konfig.srvAlias;
            this.tbBazaDanych.Text = konfig.DbName;
            this.tbUserId.Text = konfig.logId;
            
            if (konfig.DataMiesStart.HasValue)
                dtpMiesPak.Value = konfig.DataMiesStart.Value; 

            if (konfig.StartImportDate != null)
                this.dateStartImport.Value = Convert.ToDateTime(konfig.StartImportDate);
            else
                this.dateStartImport.Value = this.dateStartImport.MinDate;

            if (!String.IsNullOrEmpty(konfig.pwd))
            {
                this.tbPwd.Text = Utils.Decrypt(konfig.pwd,"Application error");    
            
            }
            
            if (konfig.WinLogon == true)
            {
                this.rRBWindows.IsChecked = true;
                this.tbUserId.Enabled = false;
                this.tbPwd.Enabled = false;

            }
            else
            {
                this.rRBSQL.IsChecked = true;
                
            
            }

            ////
            /*
              switch (konfig.typImportSAP)
              { 
                  case 2:
                      this.rbbRUP.IsChecked = true;
                      break;
                  case 1:
                      this.rbbCR2014.IsChecked = true;
                      break;
                  default:
                      this.rbbKNS.IsChecked = true;
                      break;

              }
            */

            if (!String.IsNullOrEmpty(konfig.WSpwd))
            {
                this.tbPwdWS.Text = Utils.Decrypt(konfig.WSpwd, "Application error");

            }
            else
            { 
                
            
            }
            this.tbLoginWS.Text = konfig.WSLogon;
            this.tbDniHasla.Text = konfig.SAPPwdExpPeriod.ToString();

            if (konfig.typDatyPlatn == 1)
                { rbEoY.Checked = true; dtpTerminWymag.Enabled = false; }
            else
                { rbExactDay.Checked = true; dtpTerminWymag.Enabled = true; dtpTerminWymag.Value = (konfig.dplatnosci.HasValue ? konfig.dplatnosci.Value: new DateTime(2000,1,1)); }
            
        }
        
        private void radSplitContainerKonfiguracja_Initialized(object sender, EventArgs e)
        {
            InitKonfig();
   
                      
        }

        private void updateKonfig()
        {
    
                konfig = thecontext.Konfiguracja.FirstOrDefault();
                DateTime dtstart = new DateTime(2000, 1, 1);
                konfig.JednostkaGospodarcza = this.rddlJedGosp.SelectedValue.ToString();
                konfig.StanowiskoFin = this.rddStanFin.SelectedValue.ToString();

                konfig.skipraty = this.chkskipRaty.Checked;
                konfig.typKns = this.rddlbDostawca.SelectedIndex;
                konfig.srvName = this.tbServer.Text;
                konfig.srvAlias = this.tbAlias.Text;
                konfig.DbName = this.tbBazaDanych.Text;
                konfig.logId = this.tbUserId.Text;
                konfig.defSad = this.cbSkipSadEmpty.Checked;

                if (this.rRBWindows.IsChecked == true)
                {

                    konfig.WinLogon = true;
                }
                else
                {
                    if (!String.IsNullOrEmpty(tbPwd.Text))
                        konfig.pwd = Utils.Encrypt(this.tbPwd.Text, "Application error");
                    konfig.WinLogon = false;
                }
                if (this.dateStartImport.Value > dtstart)
                    konfig.StartImportDate = this.dateStartImport.Value;
                else
                    konfig.StartImportDate = null;


                
                konfig.WSLogon = this.tbLoginWS.Text;
            if (!String.IsNullOrWhiteSpace(this.tbPwdWS.Text) && !String.IsNullOrWhiteSpace(this.tbPwdWS.Text))
                konfig.WSpwd = Utils.Encrypt(this.tbPwdWS.Text, "Application error");
            else
                konfig.WSpwd = string.Empty;


            if (rbEoY.Checked == true)
                {
                    konfig.typDatyPlatn = 1;

                }
                else
                {
                    konfig.typDatyPlatn = 0;
                    konfig.dplatnosci = dtpTerminWymag.Value;
                }
                konfig.DataMiesStart = dtpMiesPak.Value;
            try
            {
                konfig.SAPPwdExpPeriod = Convert.ToInt32(tbDniHasla.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wprowadź poprawną liczbę dni (0...)  dla zmiany hasła w ZSRK");
                return;
            }
            /*
              if (this.rbbRUP.IsChecked == true)
              {

                  konfig.typImportSAP=2;
              }
              else if (this.rbbCR2014.IsChecked == true)
                      {
                          konfig.typImportSAP = 1;

                      }
                  else
                      konfig.typImportSAP = 0;
              */
            // save endopoints

            foreach (GridViewRowInfo row in this.rgvMethods.Rows)
            {
                ServiceEndpoint se = (ServiceEndpoint)row.DataBoundItem;
                ServiceEndpoint sen = thecontext.ServiceEndpoint.Where(a => a.ServiceName == se.ServiceName).FirstOrDefault();
                if (sen != null)
                {
                    sen.Endpoint = se.Endpoint;
                }
            
            }

            thecontext.SaveChanges();
        }       
        

       
        private void rbTestConnection_Click(object sender, EventArgs e)
        {
            updateKonfig();
            checkConnection();
        }

        private void rbSaveKonfig_Click(object sender, EventArgs e)
        {
            updateKonfig();
            thecontext.SaveChanges();
        }


        private void rbUpdateDB_Click(object sender, EventArgs e)
        {
            rebuildDbScript();


        }



        private void checkConnection()
        {
            string ConnectionString;
            string CommandText="";
            SqlDataReader rdr = null;
            try
            {
                
                switch (this.rddlbDostawca.SelectedIndex)
                {
                    case 0: // currenda;
                         CommandText = "sp_TestCR";                                   
                         break;
                    case 1: // zeto
                         CommandText = "sp_Test";                                   
                         break;
                    case 2: // Orcom
                         CommandText = "sp_TestOR";
                         break;
                    default:
                         break;

                }
                        ConnectionString = "Server=" + this.tbServer.Text.Trim() + ";database=" + this.tbBazaDanych.Text.Trim();
                        if (this.rRBWindows.IsChecked == true)
                        {
                            ConnectionString += ";Trusted_Connection=True;";
                        }
                        else
                        {

                            ConnectionString += ";User Id=" + this.tbUserId.Text + ";Password=" + this.tbPwd.Text + ";";


                        }
                        SqlConnection con = new SqlConnection(ConnectionString);
                        con.Open();
                        con.Close();
                        ConnectionString = Utils.BuildMyConnectionString(thecontext);
                
                        con = new SqlConnection(ConnectionString);
                        con.Open();
                        SqlCommand storedProcCommand = new SqlCommand(CommandText, con);
                         storedProcCommand.CommandType = CommandType.StoredProcedure;
                         storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(this.tbAlias.Text.Trim()) ? this.tbServer.Text.Trim() : this.tbAlias.Text.Trim());
                         storedProcCommand.Parameters.Add("@dbname", this.tbBazaDanych.Text.Trim());
                         storedProcCommand.Connection = con;
                         storedProcCommand.CommandTimeout = 180;
                         rdr = storedProcCommand.ExecuteReader();
                         rdr.Close(); 
                        con.Close();
                        MessageBox.Show("Połączenie z bazą danych przebiegło pomyślnie");
                        return;
                   
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd połaczenia z bazą danych " + ex.Message);
            
            }
            
        
        
        }
        public static string getDBVersion()
        {
            try
            {
                using (KnsMigratorEntities dbContext = new KnsMigratorEntities())
                {
                    string dbversion = dbContext.ExecuteStoreQuery<string>("Select dbversion from Konfiguracja").FirstOrDefault();
                    return dbversion;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd połączenia z bazą danych Integratora " + ex.Message);
                return null;
            }
        }
        public static bool rebuildDbScript()
        {
            string cmd = string.Empty;
            try
            {
                using (KnsMigratorEntities dbContext = new KnsMigratorEntities())
                {
                    foreach (string sqlcmd in sqlCommands())
                    {
                        cmd = sqlcmd;
                        dbContext.ExecuteStoreCommand(cmd);

                    }
                    Konfiguracja knf = dbContext.Konfiguracja.FirstOrDefault();
                    knf.dbversion = RunMode.dbversion;
                    dbContext.SaveChanges();
                }

                MessageBox.Show("Struktura bazy danych została pomyślnie zaktualizowana");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + (ex.InnerException != null ? " "+ex.InnerException.Message:"")+   "\n" + "Błąd podczas przebudowy struktury bazy danych dla polecenia: " + cmd + "\n\r" + ex.Message);
                return false;

            }

        }



        private static string[] sqlCommands()
        {
            string[] commnadsList = {
                 // wer 3.3
                " IF isnull((select count(1) from [user] ),0) = 0  BEGIN " +
                " INSERT [dbo].[User] ([Username], [Pssword], [role], [LastPwdChngDate], [suspend], [ChangePwd], [FirstName], [LastName], [deleted], [CreationDate], [DeleteDate], [PwdPeriodChange]) VALUES ( N'admin', N'j/6oZDQ3GQ4=', 1, NULL, 0, 0, N'Admin', N'Admin', 0, CAST(N'2017-03-07T10:35:03.193' AS DateTime), NULL, 0 )  END " ,
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Dokument' AND COLUMN_NAME = 'StanowiskoFianasoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Dokument ADD  [StanowiskoFianasoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'StanowiskoFianasoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Konfiguracja ADD  [StanowiskoFianasoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Sprawa' AND COLUMN_NAME = 'StanowiskoFianasoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Sprawa ADD  [StanowiskoFianasoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Ekstrakcja' AND COLUMN_NAME = 'StanowiskoFianasoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Ekstrakcja ADD  [StanowiskoFianasoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Transfer' AND COLUMN_NAME = 'StanowiskoFinansoweWindyk' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Transfer ADD  [StanowiskoFinansoweWindyk][varchar] (4) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Ekstrakcja' AND COLUMN_NAME = 'SAPKluczUzgodnienia' AND TABLE_SCHEMA='DBO')  "+
                                "  BEGIN ALTER TABLE dbo.Ekstrakcja ADD  [SAPKluczUzgodnienia][varchar] (12) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Ekstrakcja' AND COLUMN_NAME = 'JeGoWindyk' AND TABLE_SCHEMA='DBO')  "+
                        "  BEGIN ALTER TABLE dbo.Ekstrakcja ADD  [JeGoWindyk][varchar] (4) NULL END ",

                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'Pfx' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Konfiguracja ADD Pfx varbinary(MAX) NULL, Cer varbinary(MAX) NULL END ",
                  "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'PfxPassword' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Konfiguracja ADD PfxPassword nvarchar(50) NULL END ",
                     "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'User' AND COLUMN_NAME = 'MEPPassword' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE [dbo].[User] ADD MEPPassword nvarchar(50) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'AppName' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Konfiguracja ADD AppName nvarchar(50) NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'ServiceEndpoint' AND COLUMN_NAME = 'Id' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN CREATE TABLE [dbo].[ServiceEndpoint]( 	[Id] [int] IDENTITY(1,1) NOT NULL, 	[ServiceId] [int] NULL,	[ServiceName] [nvarchar](100) NULL,	[Endpoint] [nvarchar](300) NULL, CONSTRAINT [PK_ServiceEndpoint] PRIMARY KEY CLUSTERED (	[Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] END  ",
                " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'ContractAccountCreateOut') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (1, N'ContractAccountCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractAccountCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractAccountCreate') "+
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (2, N'ContractAccountQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractAccountQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractAccountQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (3, N'ContractAccountRelationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractAccountRelationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractAccountRelationCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (4, N'ContractAccountUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractAccountUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractAccountUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (5, N'ContractObjectCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractObjectCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractObjectCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (6, N'ContractObjectQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ContractObjectQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ContractObjectQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (7, N'DebtorDepositListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DebtorDepositListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DebtorDepositListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (8, N'DepartmentDictionaryQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DepartmentDictionaryQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DepartmentDictionaryQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (9, N'DepositListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DepositListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DepositListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 10, N'DocumentBailiffListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentBailiffListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentBailiffListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 11, N'DocumentCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 12, N'DocumentDebtStateUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentDebtStateUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentDebtStateUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 13, N'DocumentListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 14, N'DocumentReductionDebtOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentReductionDebtOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentReductionDebt') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 15, N'DocumentReferenceUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentReferenceUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentReferenceUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 16, N'DocumentReverseCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentReverseCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentReverseCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 17, N'DocumentUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=DocumentUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:DocumentUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 18, N'InstalmentPlanCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=InstalmentPlanCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:InstalmentPlanCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 19, N'InstalmentPlanDeactivateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=InstalmentPlanDeactivateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:InstalmentPlanDeactivate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 20, N'InstalmentPlanVerifyOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=InstalmentPlanVerifyOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:InstalmentPlanVerify') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 21, N'PartnerCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PartnerCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PartnerCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 22, N'PartnerQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PartnerQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PartnerQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 23, N'PartnerUpdateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PartnerUpdateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PartnerUpdate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 24, N'PaymentCancellationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentCancellationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentCancellationCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 25, N'PaymentClarificationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentClarificationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentClarificationCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 26, N'PaymentClarificationZDOBCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentClarificationZDOBCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentClarificationZDOBCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 27, N'PaymentClarificationsQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentClarificationsQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentClarificationsQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 28, N'PaymentListQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentListQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 29, N'PaymentReservationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentReservationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentReservationCreate') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 30, N'PostingDataPrepareOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PostingDataPrepareOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PostingDataPrepare') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 31, N'PostingStatusQueryOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PostingStatusQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PostingStatusQuery') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 32, N'RelationCreateOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=RelationCreateOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:RelationCreate') " +
                "END "
                ,    // wer 3.4
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Dokument' AND COLUMN_NAME = 'referencja' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN ALTER TABLE dbo.Dokument ADD referencja varchar(1024) NULL, 	tekst varchar(1024) NULL,  	IDZadanieKsiegowania varchar(30) NULL, 	DataStanu datetime NULL END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'SAPSlownikRozlicz' AND COLUMN_NAME = 'SAPSlownikRozlicz_Id' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN CREATE TABLE [dbo].[SAPSlownikRozlicz]( 	[SAPSlownikRozlicz_Id] [int] IDENTITY(1,1) NOT NULL, 	[kasabank] [int] NULL,	[nazwa] [varchar](50) NULL,	[rodzaj] [int] NULL, CONSTRAINT [PK_SAPSlownikRozlicz] PRIMARY KEY CLUSTERED (	[SAPSlownikRozlicz_Id] ASC)) END  " ,
                 " IF NOT EXISTS (SELECT NULL FROM [dbo].[SAPSlownikRozlicz] WHERE [rodzaj] = 2 ) BEGIN " +
                    " SET IDENTITY_INSERT [dbo].[SAPSlownikRozlicz] ON  " +
                    " INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (1, 1, N'Dochody', 1)   " +
                    " INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (2, 1, N'Wydatki', 2)	   " +
                    " INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (3, 1, N'Sumy na zlecenia', 3)  " +
                    " INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (4, 1, N'FPP', 4) " +
                    " INSERT [dbo].[SAPSlownikRozlicz] ([SAPSlownikRozlicz_Id], [kasabank], [nazwa], [rodzaj]) VALUES (5, 2, N'Dochody', 2) " +
                    " SET IDENTITY_INSERT [dbo].[SAPSlownikRozlicz] OFF END ",
                    " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'PaymentListQueryIn') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES ( 33, N'PaymentListQueryIn', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=PaymentListQueryOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:PaymentListQuery') " +
                "END "
                , // wer 3.5
                " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'GetCaseRegistryTypesOut') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (34, N'GetCaseRegistryTypesOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=DictionariesSOAPSender&receiverParty=&receiverService=&interface=GetCaseRegistryTypesOut&interfaceNamespace=urn:ms.gov.pl:Dictionaries:FI') "+
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (35, N'GetCourtsOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=DictionariesSOAPSender&receiverParty=&receiverService=&interface=GetCourtsOut&interfaceNamespace=urn:ms.gov.pl:Dictionaries:FI') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (36, N'GetDepartmentsOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=DictionariesSOAPSender&receiverParty=&receiverService=&interface=GetDepartmentsOut&interfaceNamespace=urn:ms.gov.pl:Dictionaries:FI') " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (37, N'ManageAccountOut', N'https://sapwipl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=EX2PSCDSender&receiverParty=&receiverService=&interface=ManageAccountOut&interfaceNamespace=urn:ms.gov.pl:EX2PSCD:ManageAccount') " +
                " END " ,
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'SAPRepertorium' AND COLUMN_NAME = 'typSad' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN " +
                " CREATE TABLE dbo.Tmp_SAPRepertorium ( 	id int NOT NULL IDENTITY (1, 1),	kod varchar(50) NOT NULL,	SymbolRodzajPrzedmiotu varchar(4) NULL,	typSad varchar(2) NULL 	)  ON [PRIMARY] " +
                " ALTER TABLE dbo.Tmp_SAPRepertorium SET (LOCK_ESCALATION = TABLE) " +
                " SET IDENTITY_INSERT dbo.Tmp_SAPRepertorium OFF " +
                " IF EXISTS(SELECT * FROM dbo.SAPRepertorium) " +
                " EXEC('INSERT INTO dbo.Tmp_SAPRepertorium (kod, SymbolRodzajPrzedmiotu) " +
                " SELECT kod, SymbolRodzajPrzedmiotu FROM dbo.SAPRepertorium WITH (HOLDLOCK TABLOCKX)') " +
                " DROP TABLE dbo.SAPRepertorium " +
                " EXECUTE sp_rename N'dbo.Tmp_SAPRepertorium', N'SAPRepertorium', 'OBJECT'  " +
                " ALTER TABLE dbo.SAPRepertorium ADD CONSTRAINT PK_SAPRepertorium_1 PRIMARY KEY CLUSTERED ( id 	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] " +
                " END " ,
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'SAPSad' AND COLUMN_NAME = 'WazneOd' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN " +
                 "  ALTER TABLE dbo.SAPSad ADD " +
                 "  WazneOd datetime NULL, " +
                 "  WazneDo datetime NULL  " +
                 " END ",
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Konfiguracja' AND COLUMN_NAME = 'SAPPwdExpPeriod' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN " +
                "  ALTER TABLE dbo.Konfiguracja ADD   SAPPwdExpPeriod int NULL " +
                "  END ",
                " Update Konfiguracja set SAPPwdExpPeriod = isnull(SAPPwdExpPeriod, 7) ",
                // ver 3.6
                "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'KnsKsiegi' AND COLUMN_NAME = 'ksGrzFPPMap' AND TABLE_SCHEMA='DBO')  "+
                "  BEGIN " +
                "   ALTER TABLE dbo.KnsKsiegi ADD ksGrzFPPMap int NULL " +
                "  END "
               ,
                // ver 3.7
                  " IF NOT EXISTS (SELECT NULL FROM [dbo].[ServiceEndpoint] WHERE [ServiceName] = 'ImportContentSystemData') BEGIN " +
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (38, N'ImportContentSystemData', N'https://sapwitl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ISMSender&receiverParty=&receiverService=&interface=ImportContentSystemDataOut&interfaceNamespace=urn:ms.gov.pl:ISM:ImportContentSystemData') "+
                    " INSERT [dbo].[ServiceEndpoint] ( [ServiceId], [ServiceName], [Endpoint]) VALUES (39, N'GetStatusContentSystemData', N'https://sapwitl01.zsrk.ms.gov.pl:44300/XISOAPAdapter/MessageServlet?senderParty=&senderService=ISMSender&receiverParty=&receiverService=&interface=GetStatusContentSystemDataOut&interfaceNamespace=urn:ms.gov.pl:ISM:GetStatusContentSystemData') " +
                " END " ,
                 "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'ConsExternalDBConnectionConfig' AND COLUMN_NAME = 'typDB' AND TABLE_SCHEMA='DBO')  "+
                 "  BEGIN " +
                 " CREATE TABLE [dbo].[ConsExternalDBConnectionConfig]( 	[id] [int] IDENTITY(1,1) NOT NULL,	[typDB] [int] NULL,	[rodzajDB] [int] NULL,	[srvName] [varchar](100) NULL,	[DbName] [varchar](100) NULL,	[pwd] [varchar](100) NULL, " +
                 " [logId] [varchar](100) NULL,    [WinLogon] [bit] NULL,  [srvAlias] [varchar](100) NULL, [sp_name] [varchar](100) NULL,  [SAPKnsId] [varchar](100) NULL, [ConnectionName] [varchar](100) NULL,   [sp_param] [varchar](100) NULL, [isActive] [bit] NULL, " +
                 "  CONSTRAINT [PK_ConsExternalDBConnectionConfig] PRIMARY KEY CLUSTERED (	[id] ASC ) ) ON [PRIMARY] " +
                 " END ",
                 "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'ConsJobItem' AND COLUMN_NAME = 'insertDate' AND TABLE_SCHEMA='DBO')  "+
                 "  BEGIN " +
                 " CREATE TABLE [dbo].[ConsJobItem]( 	[Id] [int] IDENTITY(1,1) NOT NULL,	[insertDate] [datetime] NOT NULL,	[startDate] [datetime] NULL,	[finishDate] [datetime] NULL,	[consExternalDBConnectionConfig_Id] [int] NOT NULL, " +
                 "	[status] [int] NULL, 	[info] [varchar](1000) NULL,	[queryDate] [datetime] NULL, CONSTRAINT [PK_ConsJobItem] PRIMARY KEY CLUSTERED (	[Id] ASC ) ) ON [PRIMARY] " +
                 "  ALTER TABLE [dbo].[ConsJobItem] ADD  CONSTRAINT [DF_ConsJobItem_insertDate]  DEFAULT (getdate()) FOR [insertDate] " +
                 "  ALTER TABLE [dbo].[ConsJobItem]  WITH CHECK ADD  CONSTRAINT [FK_ConsJobItem_ConsExternalDBConnectionConfig] FOREIGN KEY([consExternalDBConnectionConfig_Id]) REFERENCES [dbo].[ConsExternalDBConnectionConfig] ([id]) " +
                 "  ALTER TABLE [dbo].[ConsJobItem] CHECK CONSTRAINT [FK_ConsJobItem_ConsExternalDBConnectionConfig] " +
                 " END " ,
                  "  IF NOT EXISTS (SELECT 1  FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'ConsKartaTransfer' AND COLUMN_NAME = 'idKomunikatu' AND TABLE_SCHEMA='DBO')  "+
                  "  BEGIN " +
                    " CREATE TABLE [dbo].[ConsKartaTransfer]( [Id] [int] IDENTITY(1,1) NOT NULL,	[idKomunikatu] [nvarchar](50) NOT NULL,	[status] [int] NOT NULL,	[idStronyWydzial] [int] NULL,	[idSprawyWydzial] [int] NULL, " +
                    " [dImportu] [datetime] NULL,	[trescOdpowiedzi] [nvarchar](4000) NULL,	[payload] [nvarchar](max) NULL,	[hash] [nvarchar](512) NULL,	[consJobItemId] [int] NULL, CONSTRAINT [PK_ConsKartaTransfer] PRIMARY KEY CLUSTERED " +
                    " ( [Id] ASC ) ON [PRIMARY] ) " +
                    "  ALTER TABLE [dbo].[ConsKartaTransfer] ADD  CONSTRAINT [DF_ConsKartaTransfer_status]  DEFAULT ((0)) FOR [status] "+
                    "  ALTER TABLE [dbo].[ConsKartaTransfer]  WITH CHECK ADD  CONSTRAINT [FK_ConsKartaTransfer_ConsJobItem] FOREIGN KEY([consJobItemId]) " +
                    " REFERENCES [dbo].[ConsJobItem] ([Id])  " +
                    " ALTER TABLE [dbo].[ConsKartaTransfer] CHECK CONSTRAINT [FK_ConsKartaTransfer_ConsJobItem] " +                 
                  " END "
            };

            return commnadsList;
        }
    }
}
