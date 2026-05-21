using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using KnsMigrator;

namespace RupIntegrator
{
    public partial class ConfigDB : Form
    {
        public int Id {get;set;}
        private ConsExternalDBConnectionConfig konfig;
        private string EncryptPhase = "Application error";


        private bool LoadConfig()
        {
            if (Id > 0)
            {
                using (var theContext = new KnsMigratorEntities())
                {
                    konfig = theContext.ConsExternalDBConnectionConfig.Where(a => a.id == Id).FirstOrDefault();
                    if (konfig == null)
                        return false;
                    this.tbServer.Text = konfig.srvName;
                    this.tbAlias.Text = konfig.srvAlias;
                    this.tbBazaDanych.Text = konfig.DbName;
                    this.tbUserId.Text = konfig.logId;
                    this.rddlbDostawca.SelectedIndex = konfig.typDB as int? ?? default(int);
                    this.tbNazwaPolaczenia.Text = konfig.ConnectionName;
                    this.tbParam.Text = konfig.sp_param;
                    //this.rddbtypDb.SelectedIndex = konfig.rodzajDB as int? ?? default(int);
                    this.tb_sp.Text = konfig.sp_name;
                    this.tbNazwaPolaczenia.Text = konfig.ConnectionName;
                    this.tb_sp.Text =  konfig.sp_name;
                    this.tbConsKNSId.Text = konfig.SAPKnsId;
                    this.chbIsActive.Checked  = konfig.isActive?? false;




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

                return true;

            }
            return false;
        }

        public ConfigDB()
        {
            InitializeComponent();
        }

        private  bool checkDatabaseConnection(string connectionString, out string message)
        {
            message = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT 1", connection))
                    {
                        var result = command.ExecuteScalar();

                        if (result != null && Convert.ToInt32(result) == 1)
                        {
                            message = "Połączenie z bazą danych działa poprawnie.";
                            return true;
                        }
                        else
                        {
                            message = "Zapytanie testowe nie zwróciło oczekiwanego wyniku.";
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                message = $"Błąd SQL: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                message = $"Błąd ogólny: {ex.Message}";
                return false;
            }
        }



        private void checkConnection()
        {
            string connectionString;
            string message;

            connectionString = "Server=" + this.tbServer.Text.Trim() + ";database=" + this.tbBazaDanych.Text.Trim();
            if (this.rRBWindows.IsChecked == true)
            {
                connectionString += ";Trusted_Connection=True;";
            }
            else
            {

                connectionString += ";User Id=" + this.tbUserId.Text + ";Password=" + this.tbPwd.Text + ";";


            }

            if (checkDatabaseConnection(connectionString, out message))
            {
                MessageBox.Show("Połączenie z  bazą danych poprawne");
            }
            else
            {
                MessageBox.Show(message);
            }
        }


        private void rbOK_Click(object sender, EventArgs e)
        {
           
            try
            {
                using (var theContext = new KnsMigratorEntities())
                {

                    if (konfig == null)
                        konfig = new ConsExternalDBConnectionConfig();
                    else
                        if (this.Id > 0)
                    {

                        konfig = theContext.ConsExternalDBConnectionConfig.Where(a => a.id == Id).FirstOrDefault();

                    }
                    konfig.typDB = this.rddlbDostawca.SelectedIndex;
                    konfig.srvName = this.tbServer.Text;
                    konfig.srvAlias = this.tbAlias.Text;
                    konfig.DbName = this.tbBazaDanych.Text;
                    konfig.logId = this.tbUserId.Text;
                    //konfig.rodzajDB = this.rddbtypDb.SelectedIndex;
                    konfig.ConnectionName = this.tbNazwaPolaczenia.Text;
                    konfig.sp_param = this.tbParam.Text;
                    konfig.sp_name = this.tb_sp.Text;
                    konfig.SAPKnsId = this.tbConsKNSId.Text;
                    konfig.isActive = this.chbIsActive.Checked;

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
                    konfig.isActive  = this.chbIsActive.Checked;
                 



                    if (this.Id > 0)
                    {

                        ;

                    }
                    else
                    { // nowe połączenie 

                        theContext.ConsExternalDBConnectionConfig.AddObject(konfig);


                    }




                    theContext.SaveChanges();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd " + ex.Message);
                this.DialogResult = DialogResult.None;

            }
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

        private void ConfigDB_Load(object sender, EventArgs e)
        {
            this.LoadConfig();
        }
    }


    }

