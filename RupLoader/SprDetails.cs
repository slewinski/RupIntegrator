using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using Telerik.WinControls.UI.Localization;
using Telerik.WinControls.UI;

namespace RupLoader
{
    public partial class SprDetails : Form
    {

        public int IdSprawy { get; set; }
        public string  Sygnatura { get; set; }
        public SprDetails()
        {
            InitializeComponent();
            RadGridLocalizationProvider.CurrentProvider = new  PolishRadGridLocalizationProvider();
        }

       private void LoadDetails(RL_Konfig knf = null)
        {
            SqlDataReader rdr = null;
            SqlConnection con = null;
            SqlCommand storedProcCommand;



            try
            {
                // Open connection to the database
                Cursor.Current = Cursors.WaitCursor;
                string ConnectionString = ConfigurationManager.ConnectionStrings["RupLoader.Properties.Settings.RupDB"].ConnectionString;
                con = new SqlConnection(ConnectionString);
                //con.Open();

                if (knf == null)
                    knf = (from c in RupDatabase.theContext.RL_Konfig select c).FirstOrDefault();

                switch (knf.typDB)
                {
                    case 0: // currenda
                        storedProcCommand = new SqlCommand("sp_GetStronaCR", con);
                        break;
                    case 1: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetStrona", con);
                        break;
                    case 2: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetStronaOR", con);
                        break;
                    case 3: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetStronaAL", con);
                        break;
                    default:
                        return;
                }
              


                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(knf.srvAlias) ? knf.srvName : knf.srvAlias) + (RupDatabase.theConfig.typKns == 2 ? "@@" + RupDatabase.jg : ""));
                storedProcCommand.Parameters.Add("@dbname", knf.DbName);
                storedProcCommand.Parameters.Add("@idSprawa", IdSprawy);
                storedProcCommand.CommandTimeout = 600;
                storedProcCommand.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter();

                da.SelectCommand = storedProcCommand;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                //this.dgvStrony.DataSource = dt;
                this.rgvStrony.DataSource = dt;


                switch (knf.typDB)
                {
                    case 0: // currenda
                        storedProcCommand = new SqlCommand("sp_GetDocumentCR", con);
                        break;
                    case 1: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetDocument", con);
                        break;
                    case 2: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetDocumentOR", con);
                        break;
                    case 3: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetDocumentAL", con);
                        break;
                    default:
                        return;
                }
                
              
                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(knf.srvAlias) ? knf.srvName : knf.srvAlias) + (RupDatabase.theConfig.typKns == 2 ? "@@" + RupDatabase.jg : ""));
                storedProcCommand.Parameters.Add("@dbname", knf.DbName);

                storedProcCommand.Parameters.Add("@idSprawa", IdSprawy);
                storedProcCommand.CommandTimeout = 600;
                storedProcCommand.Connection = con;
                SqlDataAdapter da_doc = new SqlDataAdapter();
                da_doc.SelectCommand = storedProcCommand;
                da_doc.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt_doc = new DataTable();
                da_doc.Fill(dt_doc);
                this.dgvDokumenty.DataSource = dt_doc;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                // Print error message
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con.State == ConnectionState.Open)
                    con.Close();

            };




        }

        private void SprDetails_Load(object sender, EventArgs e)
        {
            if (IdSprawy > 0)
                LoadDetails();
        }

        private void dgvDokumenty_DoubleClick(object sender, EventArgs e)
        {

            int row;
            if (dgvDokumenty.SelectedRows.Count == 1)
            {

                row = (sender as RadGridView).CurrentRow.Index;
                DocViewer theViewer = new DocViewer();
                theViewer.RtfDoc = (sender as RadGridView).CurrentRow.Cells["Tresc"].Value as string;
                theViewer.ShowDialog();

            }
        }

    }
}
