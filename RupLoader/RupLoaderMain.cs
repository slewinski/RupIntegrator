using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Localization;
using System.IO;
using System.Linq;
using SapPOHelper;

namespace RupLoader
{
    public partial class RupLoaderMain : Form
    {
        private string EncryptPhase = "Application error";
        public User currUser { get; set; }
        public RupLoaderMain()
        {
            InitializeComponent();
            RadGridLocalizationProvider.CurrentProvider = new PolishRadGridLocalizationProvider();
            RadTimePickerLocalizationProvider.CurrentProvider = new MyTimePickerLocalizationProvider();


        }


        public void FindSAPIds(string InString, string wydzial, string rep, int numer, int rok, RL_Konfig knf = null)
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
                        storedProcCommand = new SqlCommand("sp_GetDataCR", con);
                        break;
                    case 1: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetData", con);
                        break;
                    case 2: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetDataOR", con);
                        break;
                    case 3: // Zeto
                        storedProcCommand = new SqlCommand("sp_GetDataAL", con);
                        break;
                    default:
                        return;
                }

                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@instring", InString);
                storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(knf.srvAlias) ? knf.srvName : knf.srvAlias) + (RupDatabase.theConfig.typKns == 2 ? "@@" + RupDatabase.jg : ""));
                storedProcCommand.Parameters.Add("@dbname", knf.DbName);
                storedProcCommand.Parameters.Add("@dataOd", new DateTime(2014, 1, 1));
                storedProcCommand.Parameters.Add("@dataDo", DateTime.Today);
                storedProcCommand.Parameters.Add("@mode", 1);
                storedProcCommand.Parameters.Add("@wydzial", wydzial);
                storedProcCommand.Parameters.Add("@repertorium", rep);
                storedProcCommand.Parameters.Add("@numer", numer);
                storedProcCommand.Parameters.Add("@rok", rok);
                storedProcCommand.CommandTimeout = 600;
                storedProcCommand.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter();

                da.SelectCommand = storedProcCommand;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgVResult.DataSource = dt;
                if (File.Exists("searchGrid.lyt"))
                {
                    this.dgVResult.LoadLayout("searchGrid.lyt");
                }
                if (dgVResult.RowCount > 0)
                {
                    foreach (GridViewColumn dc in dgVResult.Columns)
                    {
                        if (dc.Name.ToLower() == "grzywna" || dc.Name.ToLower() == "koszty")
                        {
                            dc.ReadOnly = false;
                        }
                        else
                        {
                            dc.ReadOnly = true;
                        }
                    }
                }

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


        private void btFind_Click(object sender, EventArgs e)
        {
            // Parsowanie sygnatury 
            string inSygn;
            string oryginRep = "";
            string repertorium = "";
            string wydzialSekcja = "";
            int numer;
            int rok;



            if (tbFind.Text.Trim().Length == 0 && tbTextAll.Text.Trim().Length > 0)
            {
                string sygnatura = Utils.getSygnatura(tbTextAll.Text);
                if (sygnatura.Length > 0)
                {
                    tbFind.Text = sygnatura;
                    tbFind.Refresh();
                }


            }
            if (tbFind.Text.Trim().Length < 3)
                MessageBox.Show("Wzorzec wyszukiwania musi mieć co najmniej 3 znaki ");
            else
            {
                inSygn = this.tbFind.Text.Trim().Replace("\\", "/");
                string outsad = "";
                string retval = Utils.ParseSygn(inSygn, out wydzialSekcja, out repertorium, out numer, out rok, out oryginRep, out outsad);
                FindSAPIds(tbFind.Text, wydzialSekcja, oryginRep, numer, rok);
            }
        }

        private void dgVResult_DoubleClick(object sender, EventArgs e)
        {

            int row;
            if (dgVResult.SelectedRows.Count == 1)
            {

                row = (sender as RadGridView).CurrentRow.Index;
                int? idSprawy = (sender as RadGridView).CurrentRow.Cells["IdSprawy"].Value as int?;
                SprDetails winDetail = new SprDetails();
                winDetail.IdSprawy = Convert.ToInt32(idSprawy);
                winDetail.ShowDialog();

            }
        }

        private void tSMenuItemAddData_Click(object sender, EventArgs e)
        {
            if (dgVResult.SelectedRows.Count > 0)
            {
                Dokument dok;
                String s;
                Imports imp = new Imports();
                dok = imp.ImportData(dgVResult.SelectedRows[0], 1000, DateTime.Today);
                if (dok != null)
                {
                    this.tbTextAll.Text = "";
                    ExportPI exp = new ExportPI();
                    s = exp.DoExport(dok, 0);
                    if (s != null) s = s.Replace(';', '\t');
                    this.tbTextAll.Text = s;
                    Clipboard.SetDataObject(s, true);

                }
            }
        }


        private void closeMenu_Click(object sender, EventArgs e)
        {
            ;
        }

        private void cMItemAddDataBook_Click(object sender, EventArgs e)
        {
            if (dgVResult.SelectedRows.Count > 0)
            {
                Dokument dok;
                String s;
                Imports imp = new Imports();
                dok = imp.ImportData(dgVResult.SelectedRows[0], 1000, DateTime.Today);
                if (dok != null)
                {
                    this.tbTextAll.Text = "";
                    ExportPI exp = new ExportPI();
                    s = exp.DoExport(dok, 1);
                    if (s != null) s = s.Replace(';', '\t');
                    this.tbTextAll.Text = s;
                    Clipboard.SetDataObject(s, true);
                }
            }
        }

        private void btLayout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Czy chcesz zapisać układ tabeli ?", "Zapis układu tabeli ", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                this.dgVResult.SaveLayout("searchGrid.lyt");
        }

        private void RupLoaderMain_Load(object sender, EventArgs e)
        {
            if (File.Exists("searchGrid.lyt"))
            {
                this.dgVResult.LoadLayout("searchGrid.lyt");
            }
            if (UserInfo.role == 1)
            {
                rmKonfig.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                rmPredykcja.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                rmRyczalty.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                rmKontoMEP.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            }
            else
            {
                rmKonfig.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                rmPredykcja.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                rmRyczalty.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                rmUserMgr.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                rmIDBRebuild.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            }

            this.verifySAPPwd();
        }

        private void dgVResult_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {

            Point pt = (sender as RadGridView).PointToClient(Control.MousePosition);
            GridCellElement cell = (sender as RadGridView).ElementTree.GetElementAtPoint(pt) as GridCellElement;
            if (cell != null && cell.IsCurrentRow == true)

            {
                RadDropDownMenu contextMenu = new RadDropDownMenu();
                RadMenuItem menuItem1 = new RadMenuItem("Załóż dane podstawowe");
                menuItem1.Click += new EventHandler(tSMenuItemAddData_Click);
                RadMenuItem menuItem2 = new RadMenuItem("Załóż dane podstawowe i zaksięguj");
                menuItem2.Click += new EventHandler(cMItemAddDataBook_Click);
                RadMenuItem menuItem3 = new RadMenuItem("Anuluj");
                menuItem3.Click += new EventHandler(closeMenu_Click);
                contextMenu.Items.Add(menuItem1);
                contextMenu.Items.Add(menuItem2);
                contextMenu.Items.Add(menuItem3);
                e.ContextMenu = contextMenu; // To show your context menu  
            }       //dgVResult.ContextMenu.MenuItems["tSMenuItemAddData"].Click += new EventHandler(tSMenuItemAddData_Click);
        }

        private void rmKonfig_Click(object sender, EventArgs e)
        {
            KonfiguracjaForm knf = new KonfiguracjaForm();
            knf.ShowDialog();
        }

        private void rmiAbout_Click(object sender, EventArgs e)
        {
            AboutRupLoader about = new AboutRupLoader();
            about.ShowDialog();
        }

        private void rmRyczalty_Click(object sender, EventArgs e)
        {
            // ryczałty kuratorskie
            RyczaltyKuratorskie ryczaltyWin = new RyczaltyKuratorskie();
            ryczaltyWin.Show();


        }

        private void rmKonfigJobs_Click(object sender, EventArgs e)
        {
            SchedulerForm schForm = new SchedulerForm();
            schForm.ShowDialog();

        }

        private void rmPredykcja_Click(object sender, EventArgs e)
        {
            ZDOBAnalizer zAnal = new ZDOBAnalizer();
            zAnal.ShowDialog();
        }

        private void rmKontoMEP_Click(object sender, EventArgs e)
        {
            ChngMEPPwd mepUser = new ChngMEPPwd();
            mepUser.ShowDialog();
        }

        private void rmUserMgr_Click(object sender, EventArgs e)
        {
            UsrManager usrmgr = new UsrManager();
            usrmgr.ShowDialog();
        }

        private void verifySAPPwd()
        {

            if (UserInfo.role != 1) // jeśli nie admin
            {
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                    if (knf.SAPPwdExpPeriod > 0)
                    {
                        ExportPI exportPI = new ExportPI();
                        exportPI.setSAPConnectionParams();
                        if (ChngSAPPwd.VerifySAPPwdExpire(knf.SAPPwdExpPeriod.Value))
                        {
                            ChangeSAPPwd changeSAPPwd = new ChangeSAPPwd();
                            if (changeSAPPwd.ShowDialog() == DialogResult.OK)
                            {

                                User usr = context.User.Where(a => a.Id == UserProfile.UserID).FirstOrDefault();
                                usr.MEPPassword = Utils.Encrypt(changeSAPPwd.NewPassword, EncryptPhase);
                                context.SaveChanges();
                                MessageBox.Show("Hasło do ZSRK/MEP zostało zmienione. Używaj go również podczas logowania do systemu ZSRK", " Potwierdzenie zmiany hasła");
                                UserInfo.MEPPassword = usr.MEPPassword;
                                exportPI.setSAPConnectionParams();


                            }

                        }
                    }

                }


            }

        }

        private void rmIDBRebuild_Click(object sender, EventArgs e)
        {
            Utils.rebuildDbScript();
            
        }

        // Add the missing method to fix CS0103
       
    }
}