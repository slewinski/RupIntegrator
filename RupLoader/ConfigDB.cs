using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RupLoader
{
    public partial class ConfigDB : Form
    {
        public int Id {get;set;}
        private RL_Konfig konfig;
        private string EncryptPhase = "Application error"; 
        
        
        private bool LoadConfig ()
        {
            if (Id > 0)
            {
                konfig = RupDatabase.theContext.RL_Konfig.Where(a => a.id == Id).FirstOrDefault();
                if (konfig == null)
                    return false;
                this.tbServer.Text = konfig.srvName;
                this.tbAlias.Text = konfig.srvAlias;
                this.tbBazaDanych.Text = konfig.DbName;
                this.tbUserId.Text = konfig.logId;
                this.rddlbDostawca.SelectedIndex = konfig.typDB as int? ?? default(int);
                this.rddbtypDb.SelectedIndex = konfig.rodzajDB as int? ?? default(int);
                this.tbWydzialy.Text = konfig.ERPLogon;
                this.tb_sp.Text = konfig.sp_name;
                this.tbIdZespolu.Text = konfig.WSLogon;
                this.tbSygnTech.Text = konfig.EndpointWS;
 

                if (!String.IsNullOrEmpty(konfig.pwd))
                {
                    this.tbPwd.Text = Utils.Decrypt(konfig.pwd, "Application error");

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
            }
          
              return true ;
              
        }


        public ConfigDB()
        {
            InitializeComponent();
        }
        private void checkConnection()
        {
            string ConnectionString;
            string CommandText = "";
            string sp;
            sp = tb_sp.Text;
            SqlDataReader rdr = null;

            try
            {
                if ( String.IsNullOrWhiteSpace(sp))
                switch (this.rddlbDostawca.SelectedIndex)
                {
                    case 0: // currenda;
                        CommandText = "sp_RozpoznajPrzelewCR";
                        break;
                    case 1: // zeto
                        CommandText = "sp_RozpoznajPrzelew";
                        break;
                    default:
                        break;

                }
                else
                    CommandText = sp;
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
                ConnectionString = Utils.BuildMyConnectionString(RupDatabase.theContext);

                con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand storedProcCommand = new SqlCommand(CommandText, con);
                storedProcCommand.CommandType = CommandType.StoredProcedure;

                 switch ( this.rddbtypDb.SelectedIndex)
                 {
                     case 0:
                     case 1:
                storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(this.tbAlias.Text.Trim()) ? this.tbServer.Text.Trim() : this.tbAlias.Text.Trim());
                storedProcCommand.Parameters.Add("@dbname", this.tbBazaDanych.Text.Trim());
                storedProcCommand.Parameters.Add("@key", "S;XXX SS 99999/80");
                storedProcCommand.Parameters.Add("@wydzial", "XXX");
                storedProcCommand.Parameters.Add("@repertorium", "SS");
                storedProcCommand.Parameters.Add("@numer", 99999);
                storedProcCommand.Parameters.Add("@rok", 1980);
                storedProcCommand.Parameters.Add("@skipkns", 1);
                storedProcCommand.Parameters.Add("@idList", "");
                storedProcCommand.Parameters.Add("@mode", "WY");
                        break;
                     case 2:
                     case 3:
                         storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(this.tbAlias.Text.Trim()) ? this.tbServer.Text.Trim() : this.tbAlias.Text.Trim());
                         storedProcCommand.Parameters.Add("@dbname", this.tbBazaDanych.Text.Trim());
                         storedProcCommand.Parameters.Add("@dataod", DateTime.Today);
                         storedProcCommand.Parameters.Add("@datado", DateTime.Today);
                         storedProcCommand.Parameters.Add("@what", "RNW");
                         storedProcCommand.Parameters.Add("@IdZespolu", "II");

                         break;
                     default:
                         MessageBox.Show("Wybrano błędny wariant");
                         return;
                 }

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
        private void rbOK_Click(object sender, EventArgs e)
        {
           
            try
            {
                if (konfig == null) konfig = new RL_Konfig();
                konfig.typDB = this.rddlbDostawca.SelectedIndex;
                konfig.srvName = this.tbServer.Text;
                konfig.srvAlias = this.tbAlias.Text;
                konfig.DbName = this.tbBazaDanych.Text;
                konfig.logId = this.tbUserId.Text;
                konfig.ERPLogon = this.tbWydzialy.Text;
                konfig.rodzajDB = this.rddbtypDb.SelectedIndex;
                konfig.sp_name = this.tb_sp.Text;
                konfig.EndpointWS = this.tbSygnTech.Text ;

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

                konfig.WSLogon = this.tbIdZespolu.Text;

                if (this.Id > 0)
                {

                    ;

                }
                else
                { // nowe połączenie 

                    RupDatabase.theContext.RL_Konfig.AddObject(konfig);


                }


                
                
                RupDatabase.theContext.SaveChanges();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd " + ex.Message);
                this.DialogResult = DialogResult.None;

            }
        }

        private void UserAccount_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void rbTestConnection_Click(object sender, EventArgs e)
        {
            checkConnection();
        }

        private void rRBWindows_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {

           if (this.rRBWindows.IsChecked == true)
            {

                this.tbUserId.Enabled = false;
                this.tbPwd.Enabled = false;

            }
            else
            {
                this.tbUserId.Enabled = true;
                this.tbPwd.Enabled = true;


            }
        }
       
        }


    }

