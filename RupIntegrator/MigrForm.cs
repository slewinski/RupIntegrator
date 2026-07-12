using ConsImport;
using ConsInterfeces.Rup2ConsGetStatusContentSystemData;
using Ex2PscdInterface.Ex2PscdContractAccountCreateOutService;
using Ex2PscdInterface.Ex2PscdContractAccountQueryOutService;
using Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService;
using Ex2PscdInterface.Ex2PscdContractObjectCreateOutService;
using Ex2PscdInterface.Ex2PscdContractObjectQueryOutService;
using Ex2PscdInterface.Ex2PscdDocumentCreateOutService;
using Ex2PscdInterface.Ex2PscdDocumentListQueryOutService;
using Ex2PscdInterface.Ex2PscdDocumentReductionDebtOutService;
using Ex2PscdInterface.Ex2PscdInstalmentPlanDeactivateOutService;
using Ex2PscdInterface.Ex2PscdInstalmentPlanVerifyOutService;
using Ex2PscdInterface.Ex2PscdPartnerCreateOutService;
using Ex2PscdInterface.Ex2PscdPartnerQueryOutService;
using MessageSignature;
using RupBig;
using RupIntegrator;
using SapPOHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Export;
using Telerik.WinControls.UI.Localization;

namespace KnsMigrator
{
    public partial class MigrForm : Telerik.WinControls.UI.RadForm
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private KnsMigratorEntities thecontext;
        private BindingSource MaindataSource = new BindingSource();
        private BindingSource EkstrakcjadataSource = new BindingSource();
        private BindingSource KnsSadDataSource = new BindingSource();
        private BindingSource SapSadyDataSource = new BindingSource();
        private BindingSource RepertoriumDataSource = new BindingSource();
        private BindingSource BankiDataSource = new BindingSource();
        private BindingSource KonfigSource = new BindingSource();
        private BindingSource KonfigSource1 = new BindingSource();
        private BindingSource UserSource = new BindingSource();
        private BindingSource KnsKomornikDataSource = new BindingSource();
        private BindingSource WalidSaldoDataSource = new BindingSource();
        private BindingSource ConsIntegrDataSource = new BindingSource();
        private Konfiguracja konfig;
        private RadGridView rgvCurrent;
        public User currUser { get; set; }

        private string exportFileName = "";

        private RadGridView rgvCurrentSlowniki;
        private bool validateImports = false;

        private string ostatniTypKontaUmowy;



        public MigrForm()
        {


            try
            {
                // alter proced


                thecontext = new KnsMigratorEntities();
                // login


                Cursor.Current = Cursors.WaitCursor;
                konfig = thecontext.Konfiguracja.FirstOrDefault();  // odczyt konfiguracji



                InitializeComponent();


                if (RunMode.silentMode)
                {

                    this.Visible = false;
                    this.WindowState = FormWindowState.Minimized;
                }
                else
                    RadGridLocalizationProvider.CurrentProvider = new PolishRadGridLocalizationProvider();

                string sapExport = Properties.Settings.Default.SAPExport;

                if (sapExport != null && sapExport.ToUpper().Trim() == "NO")
                {
                    rmiDanePodst.Enabled = false;
                    rmiDpodst.Enabled = false;
                    rmi_OdpisyWS.Enabled = false;

                }
                Cursor.Current = Cursors.Default;


            }

            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                Utils.showMessage("Sprawdź połączenie z bazą danych, błąd: " + ex.Message + "\nStack trace : " + ex.StackTrace + "\nInner exception " + ex.InnerException.Message);
                Application.Exit();
            }
            //this.rgvValid.ViewCellFormatting += new CellFormattingEventHandler(rgvValid_ViewCellFormatting);

        }


        private bool checkboxFlag = false;


        private void ReloadVAlidGrid()
        {
            //this.thecontext.Refresh(System.Data.Objects.RefreshMode.StoreWins, Main);
        }

        private void SetUserView(int role)
        {
            if (role == 0)  // użytkownik
            {
                this.rpvAdministracja.Pages["radPageKonfig"].Enabled = false;
                this.rpvMenu.Pages["rpKonfig"].Enabled = false;

            }
            else
            {
                this.rpvAdministracja.Pages["radPageEkstrakcja"].Enabled = false;
                this.rpvAdministracja.Pages["radPageImports"].Enabled = false;
                this.rpvAdministracja.Pages["radPageMapowanie"].Enabled = false;
                this.rpvAdministracja.Pages["radPageSlowniki"].Enabled = false;
                this.rpvAdministracja.Pages["radPageBanki"].Enabled = false;
                this.rpvAdministracja.Pages["radPageBIG"].Enabled = false;

                this.rpvMenu.Pages["rpSlowniki"].Enabled = false;
                this.rpvMenu.Pages["rpMapowania"].Enabled = false;
                this.rpvMenu.Pages["rpImporty"].Enabled = false;
                this.rpvMenu.Pages["rpEkstrakcja"].Enabled = false;
                this.rpvMenu.Pages["rpBanki"].Enabled = false;
                this.rpvMenu.Pages["rpBIG"].Enabled = false;
                this.rpvMenu.Pages["rpKontoMEP"].Enabled = false;
            }


        }


        private void MigrForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'knsMigratorDataSet.Ekstrakcja' table. You can move, or remove it, as needed.

            if (currUser != null)
                SetUserView(currUser.role);
            this.EkstrakcjadataSource.DataSource = thecontext.Ekstrakcja.ToList();
            this.rgvEkstrakcja.DataSource = this.EkstrakcjadataSource; //.Mains;
            this.ConsIntegrDataSource.DataSource = thecontext.ConsExternalDBConnectionConfig.ToList();
            this.rgvConsSystems.DataSource = this.ConsIntegrDataSource;
            this.BankiDataSource.DataSource = thecontext.BankiKonfig.ToList();
            this.rgvBanki.DataSource = this.BankiDataSource; //.Mains;
            this.rpvAdministracja.SelectedPage = this.radPageSlowniki;
            this.dtWplDo.Value = DateTime.Today;
            this.dtWplOd.Value = DateTime.Today.AddDays(-30);
            // kolumny statusów

            //SetColors();
            //
            //this.rgvValid.Columns["Ksiega"].FilterDescriptor.Operator = FilterOperator.IsEqualTo;
            if (RunMode.silentMode)
            {
                switch (RunMode.operation)
                {
                    case Operations.Przypisy:
                        this.ImportSilent();
                        this.Visible = false;
                        break;
                    case Operations.Potwierdzenia:
                        this.ImportConfirmationsSilent();
                        break;
                    case Operations.Odpisy:
                        this.ImportOdpisSilent();
                        this.Visible = false;
                        break;
                    case Operations.UGO:
                        this.ImportUGOSilent();
                        break;
                    case Operations.Export:
                        this.ExportSilent(RunMode.tyOpExport, 0);
                        break;
                    default:
                        break;

                }


            }
            else
            {

                //if (konfig.EndpointWS != null && (konfig.EndpointWS.ToUpper().Contains("ZMS_KNS_TST") || konfig.EndpointWS.ToUpper().Contains("ZMS_KNS_DEV")))
                {
                    logonlab.Visible = true;
                    pwdLabel.Visible = true;
                    tbLoginWS.Visible = true;
                    tbPwdWS.Visible = true;
                }
                /*
                else
                {
                    logonlab.Visible = false;
                    pwdLabel.Visible = false;
                    tbLoginWS.Visible = false;
                    tbPwdWS.Visible = false;
                }
                */
            }
            // weryfikacja hasła SAP
            this.verifySAPPwd();

        }





        private void rbSaveValid_Click(object sender, EventArgs e)
        {


            thecontext.SaveChanges();

        }












        private void rpvMenu_SelectedPageChanged(object sender, EventArgs e)
        {
            if (this.rpvMenu.SelectedPage == this.rpSlowniki)
            {
                this.rpvAdministracja.SelectedPage = this.radPageSlowniki;
            }
            if (this.rpvMenu.SelectedPage == this.rpMapowania)
            {
                this.rpvAdministracja.SelectedPage = this.radPageMapowanie;
            }

            if (this.rpvMenu.SelectedPage == this.rpBanki)
            {
                this.rpvAdministracja.SelectedPage = this.radPageBanki;
            }

            if (this.rpvMenu.SelectedPage == this.rpBIG)
            {
                this.rpvAdministracja.SelectedPage = this.radPageBIG;
            }


            if (this.rpvMenu.SelectedPage == this.rpEkstrakcja)
            {
                Cursor.Current = Cursors.WaitCursor;
                this.rpvAdministracja.SelectedPage = this.radPageEkstrakcja;
                EkstrakcjadataSource.DataSource = null;
                EkstrakcjadataSource.DataSource = thecontext.Ekstrakcja.ToList();
                rgvEkstrakcja.DataSource = EkstrakcjadataSource;
                Cursor.Current = Cursors.Default;
            }

            if (this.rpvMenu.SelectedPage == this.rpKonfig)
            {
                this.rpvAdministracja.SelectedPage = this.radPageKonfig;

            }
            if (this.rpvMenu.SelectedPage == this.rpImporty)
            {
                this.rpvAdministracja.SelectedPage = this.radPageImports;

            }

        }




        // private  void exporter_CSVTableCreated( object sender, CSVTableCreatedEventArgs e)

        private bool ExtractToCSV(RadGridView rgvEkstrakt, bool silentMode, string filename)
        {
            if (!silentMode)
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV (*.csv)|*.csv";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!saveFileDialog.FileName.Equals(String.Empty))
                    {
                        FileInfo file = new FileInfo(saveFileDialog.FileName);

                        if (file.Extension.Equals(".csv"))
                        {
                            exportFileName = saveFileDialog.FileName;
                            exportFileName = Path.GetDirectoryName(exportFileName) + "\\" + Utils.normalizeString(Path.GetFileNameWithoutExtension(exportFileName)) + Path.GetExtension(exportFileName);

                            //ExportToExcelML export = new ExportToExcelML(this.rgvEkstrakcja);


                            //Process.Start(fileName);
                        }

                        else
                        {
                            Utils.showMessage("Błędny fromat zbioru");
                            return false;
                        }
                    }
                    else
                    {
                        Utils.showMessage("Podaj nazwę zbioru");
                        return false;
                    }
                }
                else return false;
            }
            else
                exportFileName = filename;

            try
            {
                ExportToCSV exporter = new ExportToCSV(rgvEkstrakt);
                // exporter.CSVTableCreated += new Telerik.WinControls.UI.Export.CSV.CSVTableCreatedEventHandler(exporter_CSVTableCreated);
                exporter.ColumnDelimiter = ";";
                exporter.SummariesExportOption = SummariesOption.DoNotExport;

                //export.ExcelCellFormatting += export_ExcelCellFormatting;
                exporter.RunExport(exportFileName);
                Utils.setEncodingFile(exportFileName);
                return true;
            }

            catch (Exception ex)
            {
                if (!silentMode)
                    Utils.showMessage(ex.Message, "");
                else
                    Utils.LogWriter(ex.Message);

                return false;
            }

        }

        private void rbExcel_Click(object sender, EventArgs e)
        {
            ExtractToCSV(this.rgvEkstrakcja, false, null);
        }


        private void rgvValid_RowFormatting(object sender, RowFormattingEventArgs e)
        {
            if (!checkboxFlag) return;
            if (e.RowElement.RowInfo.Cells[2].Value == null) return;
            if ((bool)(e.RowElement.RowInfo.Cells[2].Value) == true)
            {
                e.RowElement.DrawFill = true;
                e.RowElement.GradientStyle = GradientStyles.Solid;
                e.RowElement.BackColor = Color.LightCoral;
            }
            else
            {
                e.RowElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                e.RowElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                e.RowElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
            }

        }

        private void rbLoad_Click(object sender, EventArgs e)
        {


        }

        private void rlMenuMapowania_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {

            if (rgvCurrent != null)
            {
                rgvCurrent.Visible = false;

            }
            rbImport.Visible = true;
            rbImportSadZsrk.Visible = false;
            mapMode = (sender as RadListElement).ActiveItem.Tag.ToString();
            switch ((sender as RadListElement).ActiveItem.Tag.ToString())
            {
                case "Ksiegi":
                    rgvCurrent = rgvKsiegi;
                    rgvSygnMap.Visible = false;
                    rgvKsiegi.Visible = true;
                    rgvKodyMask.Visible = false;
                    rgvSadyFunkcjonalne.Visible = false;
                    rgvKsiegi.Dock = DockStyle.Fill;
                    break;
                case "SF":
                    rgvCurrent = rgvSadyFunkcjonalne;
                    rgvSadyFunkcjonalne.Visible = true;
                    rgvKomornicy.Visible = false;
                    rgvKsiegi.Visible = false;
                    rgvSygnMap.Visible = false;
                    rgvKodyMask.Visible = false;
                    rgvSadyFunkcjonalne.Dock = DockStyle.Fill;
                    rbImport.Visible = false;
                    break;
                case "SadWydzialy":
                    rgvCurrent = rgvKnsSady;
                    rgvKnsSady.Visible = true;
                    rgvSadyFunkcjonalne.Visible = false;
                    rgvKomornicy.Visible = false;
                    rgvKsiegi.Visible = false;
                    rgvSygnMap.Visible = false;
                    rgvKodyMask.Visible = false;
                    rgvKnsSady.Dock = DockStyle.Fill;
                    rbImportSadZsrk.Visible = true;
                    break;
                case "Komornicy":
                    rgvCurrent = rgvKomornicy;
                    rgvSadyFunkcjonalne.Visible = false;
                    rgvSygnMap.Visible = false;
                    rgvKomornicy.Visible = true;
                    rgvKodyMask.Visible = false;
                    rgvKomornicy.Dock = DockStyle.Fill;
                    break;
                case "Sygnatury":
                    rgvCurrent = rgvSygnMap;
                    rgvSygnMap.Visible = true;
                    rgvSadyFunkcjonalne.Visible = false;
                    rgvKnsSady.Visible = true;
                    rgvKomornicy.Visible = false;
                    rgvKsiegi.Visible = false;
                    rgvKodyMask.Visible = false;
                    rgvSygnMap.Dock = DockStyle.Fill;
                    break;
                case "Kody":
                    rgvCurrent = rgvKodyMask;
                    rgvKodyMask.Visible = true;
                    rgvSygnMap.Visible = false;
                    rgvSadyFunkcjonalne.Visible = false;
                    rgvKnsSady.Visible = false;
                    rgvKomornicy.Visible = false;
                    rgvKsiegi.Visible = false;
                    rgvKodyMask.Dock = DockStyle.Fill;
                    break;
                default:
                    break;
            }


        }

        private void rgvKnsSady_Initialized(object sender, EventArgs e)
        {
            // dodanie kolumn
            this.KnsSadDataSource.DataSource = thecontext.KnsSad.OrderBy(a => a.Id).ToList();
            this.rgvKnsSady.DataSource = this.KnsSadDataSource; //.Mains;
            var dict = new Dictionary<string, string>();
            List<SAPSad>  slst =  thecontext.SAPSad.OrderBy(a => a.miastSad).ToList();
            foreach (SAPSad row in slst)
            {
                dict.Add(row.kod, row.miastSad + " (" + row.kod +")");
            }

            GridViewComboBoxColumn SadIDColumn = new GridViewComboBoxColumn();
            SadIDColumn.Name = "SAPSad_Id";
            SadIDColumn.HeaderText = "Oznaczenie JG/FS SAP";
            SadIDColumn.IsVisible = true;
            SadIDColumn.DataSource = dict;//thecontext.SAPSad.OrderBy(a => a.miastSad).ToList();
            SadIDColumn.ValueMember = "Key";//"kod";
            SadIDColumn.DisplayMember = "Value";//"miastSad";
            SadIDColumn.Width = 350;
            SadIDColumn.FieldName = "SAPSad_Id";
            SadIDColumn.AllowFiltering = true;
            this.rgvKnsSady.Columns.Add(SadIDColumn);
            /*
            GridViewComboBoxColumn SadJGColumn = new GridViewComboBoxColumn();
            SadIDColumn.Name = "JEGO";
            SadIDColumn.HeaderText = "Jedn Gosp. dla SF";
            SadIDColumn.IsVisible = true;
            SadIDColumn.DataSource = thecontext.SAPSad.Where(a=>a.typSad=="SO").OrderBy(a => a.miastSad).ToList();
            SadIDColumn.ValueMember = "kod";
            SadIDColumn.DisplayMember = "miastSad";
            SadIDColumn.Width = 350;
            SadIDColumn.FieldName = "JEGO";
            SadIDColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
            this.rgvKnsSady.Columns.Add(SadJGColumn);
            */
            /*
                        GridViewComboBoxColumn WydzIDColumn = new GridViewComboBoxColumn();
                        WydzIDColumn.Name = "SAPWydz_Id";
                        WydzIDColumn.HeaderText = "Oznaczenie Wydziału/SAP";
                        WydzIDColumn.IsVisible = true;
                        WydzIDColumn.DataSource = thecontext.SAPWydzial.OrderBy(a=>a.nazwa).ToList();
                        WydzIDColumn.ValueMember = "SadWydzial";
                        WydzIDColumn.DisplayMember = "Nazwa";
                        WydzIDColumn.Width = 150;
                        WydzIDColumn.FieldName = "SAPWydz_Id";
                        WydzIDColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
                        this.rgvKnsSady.Columns.Add(WydzIDColumn);
            */

            this.rgvKnsSady.ShowFilteringRow = true;
        }

        

        private void rbSaveMapowania_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                thecontext.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sprawdź kompletność danych. \n\r Komunikat błędu: " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), "Błąd podczas zapisu danych");

            }
            Cursor.Current = Cursors.Default;
        }

        private void rgvRepertorium_Initialized(object sender, EventArgs e)
        {
            this.RepertoriumDataSource.DataSource = thecontext.SAPRepertorium.ToList();
            this.rgvRepertorium.DataSource = this.RepertoriumDataSource; //.Mains;

            GridViewComboBoxColumn TypSprColumn = new GridViewComboBoxColumn();
            TypSprColumn.Name = "PrzedmiotSprawy";
            TypSprColumn.HeaderText = "Przedmiot Sprawy";
            TypSprColumn.IsVisible = true;
            TypSprColumn.DataSource = thecontext.SAPOpisPrzedmiotu.ToList();
            TypSprColumn.ValueMember = "Symbol";
            TypSprColumn.DisplayMember = "Opis";
            TypSprColumn.Width = 150;
            TypSprColumn.FieldName = "SymbolRodzajPrzedmiotu";
            TypSprColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
            this.rgvRepertorium.Columns.Add(TypSprColumn);
        }



        private void rlSlowniki_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (rgvCurrentSlowniki != null)
            {
                rgvCurrentSlowniki.Visible = false;

            }

            switch ((sender as RadListElement).ActiveItem.Tag.ToString())
            {
                case "Repertoria":
                    rgvCurrentSlowniki = rgvRepertorium;
                    rgvRepertorium.Visible = true;
                    rgvRepertorium.Dock = DockStyle.Fill;
                    rbImportRep.Visible = true;
                    break;
                default:
                    break;
            }
        }

        private void radPageViewWorkspace_SelectedPageChanged(object sender, EventArgs e)
        {

            if (this.rpvAdministracja.SelectedPage == this.radPageSlowniki)
            {
                this.rpvMenu.SelectedPage = this.rpSlowniki;
            }


            if (this.rpvAdministracja.SelectedPage == this.radPageMapowanie)
            {
                this.rpvMenu.SelectedPage = this.rpMapowania;
            }

            if (this.rpvAdministracja.SelectedPage == this.radPageBanki)
            {
                this.rpvMenu.SelectedPage = this.rpBanki;
            }

            if (this.rpvAdministracja.SelectedPage == this.radPageBIG)
            {
                this.rpvMenu.SelectedPage = this.rpBIG;
            }

            if (this.rpvAdministracja.SelectedPage == this.radPageEkstrakcja)
            {
                this.ReloadWplaty();
            }

            if (this.rpvAdministracja.SelectedPage == this.radPageKonfig)
                this.rpvMenu.SelectedPage = this.rpKonfig;

            if (this.rpvAdministracja.SelectedPage == this.radPageImports)
                this.rpvMenu.SelectedPage = this.rpImporty;
        }

        private void rgvKsiegi_Initialized(object sender, EventArgs e)
        {
            this.InitKsiegiDictionary();
        }



        private void rgvTransfer_Initialized(object sender, EventArgs e)
        {
            this.InitTransfer();
        }

        private void rgvDokumenty_Initialized(object sender, EventArgs e)
        {
            this.InitDokumenty();
        }

        private void rbRefresh_Click(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;
            EkstrakcjadataSource.DataSource = null;
            EkstrakcjadataSource.DataSource = thecontext.Ekstrakcja.ToList();
            rgvEkstrakcja.DataSource = EkstrakcjadataSource;
            Cursor.Current = Cursors.Default;

        }

        private void rbClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            var query = thecontext.Ekstrakcja.ToList();
            foreach (var q in query)
            {
                thecontext.Ekstrakcja.DeleteObject(q);
            }
            thecontext.SaveChanges();
            EkstrakcjadataSource.DataSource = null;
            EkstrakcjadataSource.DataSource = thecontext.Ekstrakcja.ToList();
            rgvEkstrakcja.DataSource = EkstrakcjadataSource;
            Cursor.Current = Cursors.Default;
        }

        private void rgvWplaty_Initialized(object sender, EventArgs e)
        {
            this.InitWplaty();
        }




        private void rgvDokumenty_CellFormatting(object sender, CellFormattingEventArgs e)
        {

            if (e.CellElement is GridRowHeaderCellElement && e.RowIndex >= 0)
            {
                e.CellElement.Text = (e.CellElement.RowIndex + 1).ToString();
                e.CellElement.Image = null;
            }
        }




        private void rbAddAccount_Click(object sender, EventArgs e)
        {
            UserAccount acc = new UserAccount();
            //tdl.dOd = DateTime.Today;
            //tdl.dDo = DateTime.Today;
            acc.Context = thecontext;
            acc.Id = 0;
            acc.ShowDialog();
            if (acc.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.UserSource.DataSource = thecontext.User.ToList();
                this.rgvUsers.DataSource = this.UserSource;
            }
        }

        private void rgvUsers_Initialized(object sender, EventArgs e)
        {

            this.UserSource.DataSource = thecontext.User.ToList();
            this.rgvUsers.DataSource = this.UserSource;
        }

        private void rbManage_Click(object sender, EventArgs e)
        {
            UserAccount acc = new UserAccount();
            //tdl.dOd = DateTime.Today;
            //tdl.dDo = DateTime.Today;
            if (rgvUsers.SelectedRows.Count == 0) return;
            GridViewRowInfo therow = rgvUsers.SelectedRows[0];

            acc.Context = thecontext;
            acc.Id = Convert.ToInt32(therow.Cells["Id"].Value);
            acc.ShowDialog();
            if (acc.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.UserSource.DataSource = thecontext.User.ToList();
                this.rgvUsers.DataSource = this.UserSource;
            }

        }

        private void rbDeleteAcc_Click(object sender, EventArgs e)
        {
            if (rgvUsers.SelectedRows.Count == 0) return;
            GridViewRowInfo therow = rgvUsers.SelectedRows[0];
            int Id = Convert.ToInt32(therow.Cells["Id"].Value);
            bool isdel = Convert.ToBoolean(therow.Cells["deleted"].Value);
            if (isdel)
            {
                MessageBox.Show("To konto zostało już  usunięte");
                return;
            }
            if (MessageBox.Show("Czy na pewno chcesz usunąć wybrane konto ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                User usr = this.thecontext.User.Where(a => a.Id == Id).FirstOrDefault();
                usr.deleted = true;
                usr.DeleteDate = DateTime.Now;
                thecontext.SaveChanges();
                this.UserSource.DataSource = thecontext.User.ToList();
                this.rgvUsers.DataSource = this.UserSource;



            }

        }

        private void rbSaveBanki_Click(object sender, EventArgs e)
        {
            this.thecontext.SaveChanges();


        }

        private void rgvBanki_CommandCellClick(object sender, EventArgs e)
        {
            GridViewRowInfo currRow = (e as GridViewCellEventArgs).Row;
            try
            {
                if (currRow != null)
                {
                    string exe = currRow.Cells["ExePath"].Value.ToString();
                    string folder = currRow.Cells["Folder"].Value.ToString();

                    if (File.Exists(exe))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exe;
                        startInfo.Arguments = folder;
                        Process.Start(startInfo);
                    }
                    else
                    {
                        Utils.showMessage(exe + " nie istnieje ");

                    }

                }
            }
            catch (Exception ex)
            {
                Utils.showMessage("Błąd " + ex.Message);


            }
        }

        private void rbAdd_Click(object sender, EventArgs e)
        {
            AddBank adb = new AddBank();
            adb.Context = thecontext;
            adb.ShowDialog();
            if (adb.DialogResult == DialogResult.OK)
            {
                this.thecontext.SaveChanges();

                this.BankiDataSource.DataSource = thecontext.BankiKonfig.ToList();
                this.rgvBanki.DataSource = this.BankiDataSource;

            }
        }

        private void rbDel_Click(object sender, EventArgs e)
        {
            int Id;
            try
            {
                if (rgvBanki.SelectedRows.Count > 0)
                {

                    Id = Convert.ToInt32(rgvBanki.SelectedRows[0].Cells["Id"]);
                    if (Id > 0)
                        if (MessageBox.Show("Potwierdź", "Czy na pewno chcesz usunać wskazany bank ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            BankiKonfig bk = this.thecontext.BankiKonfig.Where(a => a.Id == Id).FirstOrDefault();
                            if (bk != null)
                            {
                                this.thecontext.BankiKonfig.DeleteObject(bk);
                                this.thecontext.SaveChanges();
                                this.rgvBanki.DataSource = null;
                                this.rgvBanki.DataSource = this.BankiDataSource;
                            }
                        }
                }
            }
            catch (Exception ex)
            {


            }
        }

        private void linkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutRupIntegrator ab = new AboutRupIntegrator();
            ab.ShowDialog();
        }

        private void rdbBreak_Click(object sender, EventArgs e)
        {

        }

        private void btKonwWyc_Click(object sender, EventArgs e)
        {
            KonwertWyc konwWycForm = new KonwertWyc();
            konwWycForm.theContext = this.thecontext;
            konwWycForm.ShowDialog();

        }

        private void rgvKomornicy_Initialized(object sender, EventArgs e)
        {
            // dodanie kolumn
            this.KnsKomornikDataSource.DataSource = thecontext.KnsKomornik.OrderBy(a => a.Miasto).ToList();
            this.rgvKomornicy.DataSource = this.KnsKomornikDataSource; //.Mains;


        }

        private void rgvTransfer_DataBindingComplete(object sender, GridViewBindingCompleteEventArgs e)
        {
            if (this.rgvTransfer.Rows.Count > 0)
            {
                //this.rgvTransfer.TableElement.VScrollBar.Value = 0;   
                //GridViewChildRowCollection childRows = this.rgvTransfer.MasterTemplate.ChildRows;
                //GridViewRowInfo firsRow = childRows[0];
                //this.rgvTransfer.TableElement.ScrollToRow(firsRow);
                this.rgvTransfer.TableElement.VScrollBar.Value = 0;
                //this.rgvTransfer.TableElement.ScrollToRow(this.rgvTransfer.Rows.First());
                this.rgvTransfer.Rows[0].IsCurrent = true;
            }
        }

        private ContractObjectQueryRequest setupGetSygnStruct(GridViewRowInfo row, Konfiguracja konf)
        {
            ContractObjectQueryRequest sygnqry = new ContractObjectQueryRequest();
            sygnqry.Sygnatura = new SygnaturaDefinicja();
            sygnqry.Sygnatura.JednostkaGospodarcza = row.Cells["sadorzek"].Value as string;
            sygnqry.Sygnatura.NumerWydzialuISekcji = row.Cells["wydzialsekcja"].Value as string;
            sygnqry.Sygnatura.Repertorium = (row.Cells["repertorium"].Value as string).ToUpper();
            sygnqry.Sygnatura.KolejnyNumerSprawy = row.Cells["Numer"].Value.ToString();
            sygnqry.Sygnatura.Rok = row.Cells["rok"].Value.ToString();
            if (sygnqry.Sygnatura != null)
            {
                int jego;
                if (int.TryParse(sygnqry.Sygnatura.JednostkaGospodarcza, out jego))
                    if (jego > 5000)   // stanowisko finansowe; 
                    {
                        sygnqry.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe = sygnqry.Sygnatura.JednostkaGospodarcza;
                        string jedngosp = sygnqry.Sygnatura.JednostkaGospodarcza;
                        SAPSad ss = thecontext.SAPSad.Where(d => d.kod == jedngosp).FirstOrDefault();
                        sygnqry.Sygnatura.JednostkaGospodarcza = ss.JEGO;
                    }
            }
            return sygnqry;
        }




        private SygnaturaTworzenie setupSygnStruct(GridViewRowInfo row, Konfiguracja konf)
        {
            SygnaturaTworzenie sygnqry = new SygnaturaTworzenie();
            sygnqry.JednostkaGospodarcza = row.Cells["sadorzek"].Value as string;
            if (sygnqry.JednostkaGospodarcza != null)
            {
                int jego;
                if (int.TryParse(sygnqry.JednostkaGospodarcza, out jego))
                    if (jego > 5000)   // stanowisko finansowe; 
                    {
                        sygnqry.SadFunkcjonalnyStanowiskoFinansowe = sygnqry.JednostkaGospodarcza;
                        string jedngosp = sygnqry.JednostkaGospodarcza;
                        SAPSad ss = thecontext.SAPSad.Where(d => d.kod == jedngosp).FirstOrDefault();
                        sygnqry.JednostkaGospodarcza = ss.JEGO;
                    }
            }


            sygnqry.NumerWydzialuISekcji = row.Cells["wydzialsekcja"].Value as string;
            sygnqry.Repertorium = (row.Cells["repertorium"].Value as string).ToUpper();
            sygnqry.KolejnyNumerSprawy = row.Cells["Numer"].Value.ToString();
            sygnqry.Rok = row.Cells["rok"].Value.ToString();
            sygnqry.RodzajSprawy = row.Cells["RodzSprawy"].Value as string;
            sygnqry.RodzajPrzedmiotuUmowy = row.Cells["RodzPUmo"].Value as string;
            sygnqry.IloscTomow = row.Cells["ltomow"].Value as string;
            sygnqry.DaneDoWindykacjiJednostkaGospodarcza = konf.JednostkaGospodarcza;
            if (!String.IsNullOrWhiteSpace(konf.StanowiskoFin))
                sygnqry.DaneDoWindykacjiSadFunkcjonalnyStanowiskoFinansowe = konf.StanowiskoFin;
            sygnqry.PodrodzajSprawy = "";

            return sygnqry;
        }


        private Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner setupBussinessPartner(GridViewRowInfo row, Konfiguracja konf)
        {
            Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner dluqry = new Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner();
            if ((row.Cells["dlFizPraw"].Value).ToString().Trim() == "")
            {
                dluqry.TypPartnera = "1";
                dluqry.Imie = (row.Cells["dlimie"].Value as string).Trim();
                dluqry.Nazwisko = (row.Cells["dlnazwisko"].Value as string).Trim();
                dluqry.NazwaOrganizacji1 = "";
                dluqry.NazwaOrganizacji2 = "";

            }
            else
            {
                dluqry.TypPartnera = "2";
                dluqry.NazwaOrganizacji1 = (row.Cells["dlimie"].Value as string).Trim();
                dluqry.NazwaOrganizacji2 = (row.Cells["dlnazwisko"].Value as string).Trim();
                dluqry.Imie = (row.Cells["dlimie"].Value as string).Trim();
                dluqry.Nazwisko = (row.Cells["dlnazwisko"].Value as string).Trim();
                if (String.IsNullOrEmpty(dluqry.Nazwisko))
                {
                    int spc = dluqry.Imie.LastIndexOf(' ');
                    if (spc > 0)
                    {
                        string tmp = dluqry.Imie.Substring(spc + 1);
                        if (tmp.Trim().Length > 0)
                        {
                            dluqry.Imie = dluqry.Imie.Substring(0, spc);
                            dluqry.Nazwisko = tmp.Trim();

                        }

                    }
                    if (String.IsNullOrEmpty(dluqry.Nazwisko)) dluqry.Nazwisko = ".";
                    dluqry.NazwaOrganizacji2 = dluqry.Nazwisko;

                }

            }
            dluqry.AdresPartner = new Ex2PscdInterface.Ex2PscdPartnerCreateOutService.AdresPartner();
            dluqry.AdresPartner.KodPocztowy = row.Cells["dlkodpoczt"].Value as string; ;
            dluqry.AdresPartner.Kraj = row.Cells["dlkraj"].Value as string;
            dluqry.AdresPartner.Miasto = (row.Cells["dlmiejscowosc"].Value as string).Trim();
            dluqry.NIP = row.Cells["dlNip"].Value as string;
            dluqry.AdresPartner.NumerDomu = (row.Cells["dlnrdomu"].Value as string).Trim();
            dluqry.AdresPartner.NumerDomu2 = (row.Cells["dlnrmieszkania"].Value == null) ? "" : (row.Cells["dlnrmieszkania"].Value as string).Trim();
            dluqry.PESEL = row.Cells["dlpesel"].Value as string;

            if (row.Cells["dlRBN"] != null && row.Cells["dlRBN"].Value != null)
            {
                dluqry.RBN = new Ex2PscdInterface.Ex2PscdPartnerCreateOutService.RBN();
                dluqry.RBN.KW_RBN = row.Cells["dlRBN"].Value as string;
                dluqry.RBN.Data_RBN = (row.Cells["DataDokumentu"] == null || row.Cells["DataDokumentu"].Value == null) ? DateTime.Today.ToString("yyyyMMdd") : Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
            }
            else
                dluqry.RBN = null;

            dluqry.AdresPartner.Ulica = (row.Cells["dlUlica"].Value as string).Trim();

            if (dluqry.AdresPartner.KodPocztowy != null) dluqry.AdresPartner.KodPocztowy = dluqry.AdresPartner.KodPocztowy.Trim().Truncate(10);
            if (dluqry.AdresPartner.Kraj != null) dluqry.AdresPartner.Kraj = dluqry.AdresPartner.Kraj.Trim().Truncate(2);
            if (dluqry.AdresPartner.Miasto != null) dluqry.AdresPartner.Miasto = dluqry.AdresPartner.Miasto.Trim().Truncate(40);
            if (dluqry.NIP != null) dluqry.NIP = dluqry.NIP.Trim().Truncate(10);
            if (dluqry.AdresPartner.NumerDomu != null) dluqry.AdresPartner.NumerDomu = dluqry.AdresPartner.NumerDomu.Trim().Truncate(10);
            if (dluqry.AdresPartner.NumerDomu2 != null) dluqry.AdresPartner.NumerDomu2 = dluqry.AdresPartner.NumerDomu2.Trim().Truncate(10); else dluqry.AdresPartner.NumerDomu2 = "";
            if (dluqry.PESEL != null) dluqry.PESEL = dluqry.PESEL.Trim().Truncate(11);
            if (dluqry.RBN != null) dluqry.RBN.KW_RBN = dluqry.RBN.KW_RBN.Trim().Truncate(2);
            if (dluqry.AdresPartner.Ulica != null) dluqry.AdresPartner.Ulica = dluqry.AdresPartner.Ulica.Trim().Truncate(60);
            return dluqry;


        }


        private Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Partner setupBussinessPartner4Query(GridViewRowInfo row, Konfiguracja konf)
        {
            Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Partner dluqry = new Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Partner();
            if ((row.Cells["dlFizPraw"].Value).ToString().Trim() == "")
            {
                dluqry.TypPartnera = "1";
                dluqry.Imie = (row.Cells["dlimie"].Value as string).Trim();
                dluqry.Nazwisko = (row.Cells["dlnazwisko"].Value as string).Trim();
                dluqry.NazwaOrganizacji1 = "";
                dluqry.NazwaOrganizacji2 = "";
            }
            else
            {
                dluqry.TypPartnera = "2";
                dluqry.NazwaOrganizacji1 = (row.Cells["dlimie"].Value as string).Trim();
                dluqry.NazwaOrganizacji2 = (row.Cells["dlnazwisko"].Value as string).Trim();
                dluqry.Imie = (row.Cells["dlimie"].Value as string).Trim();
                dluqry.Nazwisko = (row.Cells["dlnazwisko"].Value as string).Trim();
                if (String.IsNullOrEmpty(dluqry.Nazwisko))
                {
                    int spc = dluqry.Imie.LastIndexOf(' ');
                    if (spc > 0)
                    {
                        string tmp = dluqry.Imie.Substring(spc + 1);
                        if (tmp.Trim().Length > 0)
                        {
                            dluqry.Imie = dluqry.Imie.Substring(0, spc);
                            dluqry.Nazwisko = tmp.Trim();

                        }

                    }
                    if (String.IsNullOrEmpty(dluqry.Nazwisko)) dluqry.Nazwisko = ".";
                    dluqry.NazwaOrganizacji2 = dluqry.Nazwisko;

                }

            }
            dluqry.AdresPartner = new Ex2PscdInterface.Ex2PscdPartnerQueryOutService.AdresPartner();
            dluqry.AdresPartner.KodPocztowy = row.Cells["dlkodpoczt"].Value as string; ;
            dluqry.AdresPartner.Kraj = row.Cells["dlkraj"].Value as string;
            dluqry.AdresPartner.Miasto = (row.Cells["dlmiejscowosc"].Value as string).Trim();
            dluqry.NIP = row.Cells["dlNip"].Value as string;
            dluqry.AdresPartner.NumerDomu = (row.Cells["dlnrdomu"].Value as string).Trim();
            dluqry.AdresPartner.NumerDomu2 = (row.Cells["dlnrmieszkania"].Value == null) ? "" : (row.Cells["dlnrmieszkania"].Value as string).Trim();
            dluqry.PESEL = row.Cells["dlpesel"].Value as string;
            if (row.Cells["dlRBN"] == null || row.Cells["dlRBN"].Value == null || string.IsNullOrWhiteSpace(row.Cells["dlRBN"].Value as string))
            {
                dluqry.RBN = null;
            }
            else
            {
                dluqry.RBN = new Ex2PscdInterface.Ex2PscdPartnerQueryOutService.RBN();
                dluqry.RBN.KW_RBN = row.Cells["dlRBN"].Value as string;
                dluqry.RBN.Data_RBN = (row.Cells["DataDokumentu"] == null || row.Cells["DataDokumentu"].Value == null) ? DateTime.Today.ToString("yyyyMMdd") : Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");

            }

            dluqry.AdresPartner.Ulica = (row.Cells["dlUlica"].Value as string).Trim();

            if (dluqry.AdresPartner.KodPocztowy != null) dluqry.AdresPartner.KodPocztowy = dluqry.AdresPartner.KodPocztowy.Trim().Truncate(10);
            if (dluqry.AdresPartner.Kraj != null) dluqry.AdresPartner.Kraj = dluqry.AdresPartner.Kraj.Trim().Truncate(2);
            if (dluqry.AdresPartner.Miasto != null) dluqry.AdresPartner.Miasto = dluqry.AdresPartner.Miasto.Trim().Truncate(40);
            if (dluqry.NIP != null) dluqry.NIP = dluqry.NIP.Trim().Truncate(10);
            if (dluqry.AdresPartner.NumerDomu != null) dluqry.AdresPartner.NumerDomu = dluqry.AdresPartner.NumerDomu.Trim().Truncate(10);
            if (dluqry.AdresPartner.NumerDomu2 != null) dluqry.AdresPartner.NumerDomu2 = dluqry.AdresPartner.NumerDomu2.Trim().Truncate(10); else dluqry.AdresPartner.NumerDomu2 = "";
            if (dluqry.PESEL != null) dluqry.PESEL = dluqry.PESEL.Trim().Truncate(11);
            if (dluqry.RBN != null) dluqry.RBN.KW_RBN = dluqry.RBN.KW_RBN.Trim().Truncate(2);
            if (dluqry.AdresPartner.Ulica != null) dluqry.AdresPartner.Ulica = dluqry.AdresPartner.Ulica.Trim().Truncate(60);
            return dluqry;

        }


        private string AddNewDl(GridViewRowInfo row, Konfiguracja knf)
        {

            PartnerQuery partner = this.setupGetPartner(row);
            PartnerQueryResponse anspartner = ZSRKRequestHelper.WyszukajPartnera(partner);
            if (anspartner == null)
            {
                Utils.showMessage("Błąd usługi sieciowaej [Wyszukaj partnera]");
                return null;
            }
            if (anspartner.Partnerzy == null || !(anspartner.Partnerzy.GetUpperBound(0) >= 0))
            {
                return null;
            }

            if ((partner.TypPartnera == "1" && String.IsNullOrWhiteSpace(anspartner.Partnerzy[0].PESEL)) || (partner.TypPartnera == "2" && String.IsNullOrWhiteSpace(anspartner.Partnerzy[0].NIP)))
                return null;




            Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner dluqry_new = setupBussinessPartner(row, knf);
            /*
            if (dluqry_new.TypPartnera == "1")
                // osoba fizyczna
                dluqry_new.PESEL = "";
            else
                dluqry_new.NIP = "";
            */
            if (dluqry_new.AdresPartner.Ulica.Contains(" "))
                dluqry_new.AdresPartner.Ulica = Utils.ReplaceFirst(dluqry_new.AdresPartner.Ulica, " ", "  ");
            else if (dluqry_new.AdresPartner.Ulica.Contains("."))
                dluqry_new.AdresPartner.Ulica.Replace(".", " .");
            else dluqry_new.AdresPartner.Ulica = dluqry_new.AdresPartner.Ulica + ".";


            PartnerCreateResponse anspart_new = ZSRKRequestHelper.DodajPartnera(dluqry_new);
            if (anspart_new != null)
            {

                if (!String.IsNullOrWhiteSpace(anspart_new.IDPartner))
                {
                    row.Cells["SAPKontoPartnera"].Value = anspart_new.IDPartner;
                    row.Cells["Diagnostyka"].Value = (anspart_new.Komunikaty != null && anspart_new.Komunikaty.GetUpperBound(0) >= 0 ? anspart_new.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                }
                else
                {
                    row.Cells["Diagnostyka"].Value = (anspart_new.Komunikaty != null && anspart_new.Komunikaty.GetUpperBound(0) >= 0 ? anspart_new.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                    row.Cells["SAPImportStatus"].Value = -1;
                    row.Cells["Blad"].Value = row.Cells["Blad"].Value as string + "; Błąd podczas zakładania dłużnika";

                }


                decimal? id = Convert.ToDecimal(row.Cells["id"].Value);
                Dokument dok = thecontext.Dokument.Where(a => a.id == id).FirstOrDefault();
                if (dok != null)
                {
                    dok.SAPImportStatus = row.Cells["SAPImportStatus"].Value as int?;
                    dok.SAPImportInfo = row.Cells["Diagnostyka"].Value.ToString() + dok.SAPImportInfo;
                    dok.SAPImportInfo = dok.SAPImportInfo.Truncate(255);
                    if (anspart_new.IDPartner != null)
                    {

                        Dluznik dlu = thecontext.Dluznik.Where(t => t.Id == dok.Dluznik_Id).FirstOrDefault();
                        if (dlu != null)
                            dlu.SAPKontoPartnera = anspart_new.IDPartner;
                        else
                        {
                            Utils.showMessage("Nie znalezniono partnera dla dokumentu podczas zakładania obiektu " + row.Cells["SAPKontoPartnera"].Value.ToString());
                            return null;
                        }
                    }

                    thecontext.SaveChanges();



                }
            }
            return anspart_new.IDPartner;

        }


        private KontoUmowyTworzenie setupKdl(GridViewRowInfo row, Konfiguracja knf, string typkdl)
        {
            KontoUmowyTworzenie kdlqry = new KontoUmowyTworzenie();
            kdlqry.JednostkaGospodarcza = knf.JednostkaGospodarcza;
            kdlqry.NumerPartnera = row.Cells["SAPKontoPartnera"].Value.ToString();
            if (kdlqry.NumerPartnera != null && kdlqry.NumerPartnera.StartsWith("*"))
            {
                kdlqry.NumerPartnera = kdlqry.NumerPartnera.Substring(1);
            }
            kdlqry.OznaczenieKontaUmowy = row.Cells["Karta"].Value as string;
            kdlqry.RelacjaPartneraHandlowego = row.Cells["sprRelacjaKUm"].Value as string;
            if (String.IsNullOrEmpty(kdlqry.RelacjaPartneraHandlowego)) kdlqry.RelacjaPartneraHandlowego = "99";
            kdlqry.StandardowaJednostkaGospodarcza = knf.JednostkaGospodarcza;
            if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                kdlqry.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
            kdlqry.TypKontaUmowy = typkdl;
            kdlqry.IDPrzedmiotuUmowy = row.Cells["SAPPrzedmiotUmowy"].Value as string;

            return kdlqry;

        }

        private DocumentCreateRequest setupPrzypis(GridViewRowInfo row, Konfiguracja knf, string kluczUzg)
        {
            DocumentCreateRequest dok = new DocumentCreateRequest();
            Ex2PscdInterface.Ex2PscdDocumentCreateOutService.NaglowekDokument naglowek = new Ex2PscdInterface.Ex2PscdDocumentCreateOutService.NaglowekDokument();
            PozycjaDokumentuPH pozDph = new PozycjaDokumentuPH();
            dok.NaglowekDokument = naglowek;
            dok.PozycjaDokumentPH = pozDph;

            pozDph.OperacjaCz = row.Cells["OperacjaCzesciowa"].Value as string; ;
            naglowek.DataDokument = row.Cells["DataDokumentu"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
            naglowek.DataKsiegowanie = row.Cells["DataKsiegowania"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //; Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
            pozDph.DataPlatnosci = row.Cells["DataPlatnosci"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataPlatnosci"].Value).ToString("yyyyMMdd");
            pozDph.OperacjaGl = row.Cells["OperacjaGlowna"].Value as string;
            pozDph.IDSygnatura = (row.Cells["SAPPrzedmiotUmowy"].Value) as string;
            pozDph.JednostkaGospodarcza = knf.JednostkaGospodarcza;
            //naglowek.KluczUzgodnienia = kluczUzg;
            naglowek.Waluta = "PLN";
            pozDph.Kwota = Convert.ToDecimal(row.Cells["kwota"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
            //dok.NumerDokumentuRozrachunkow = "";
            pozDph.IDKontoUmowy = (row.Cells["SAPKontoUmowy"].Value) as string;
            pozDph.IDPartner = (row.Cells["SAPKontoPartnera"].Value) as string;
            // .PrzyczynaBlokPlatnosci = "A";
            naglowek.RodzajDokumentu = "NS";
            if (!String.IsNullOrEmpty(row.Cells["RodzajDokumentu"].Value as string))
                naglowek.RodzajDokumentu = row.Cells["RodzajDokumentu"].Value as string;

            if (row.Cells["OperacjaGlowna"] != null)
            {
                string opGlowna = row.Cells["OperacjaGlowna"].Value.ToString().ToUpper();
                if (opGlowna.Contains("FP"))
                    naglowek.RodzajDokumentu = "FP";
                else if (opGlowna.Contains("N033") || opGlowna.Contains("N034"))
                    naglowek.RodzajDokumentu = "NS";
            }

            pozDph.Tekst = row.Cells["Opis"].Value as string;


            return dok;

        }



        private DocumentCreateRequest setupPrzypis(string operCz, DateTime dDok, DateTime dKsie, DateTime dPlatn, string opGl, string przedmiotuUmowy, string opis, decimal kwota, string kontoUmowy, string Partner, string rodzajDok, Konfiguracja knf, string kluczUzg)
        {

            DocumentCreateRequest dok = new DocumentCreateRequest();
            Ex2PscdInterface.Ex2PscdDocumentCreateOutService.NaglowekDokument naglowek = new Ex2PscdInterface.Ex2PscdDocumentCreateOutService.NaglowekDokument();
            PozycjaDokumentuPH pozDph = new PozycjaDokumentuPH();
            dok.NaglowekDokument = naglowek;
            dok.PozycjaDokumentPH = pozDph;


            pozDph.OperacjaCz = operCz;
            naglowek.DataDokument = Convert.ToDateTime(dDok).ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
            naglowek.DataKsiegowanie = Convert.ToDateTime(dKsie).ToString("yyyyMMdd"); //; Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
            pozDph.DataPlatnosci = Convert.ToDateTime(dPlatn).ToString("yyyyMMdd");
            pozDph.OperacjaGl = opGl;
            pozDph.IDSygnatura = przedmiotuUmowy;
            pozDph.JednostkaGospodarcza = knf.JednostkaGospodarcza;
            // dok.KluczUzgodnienia = kluczUzg;
            naglowek.Waluta = "PLN";
            pozDph.Kwota = Convert.ToDecimal(kwota).ToString(CultureInfo.GetCultureInfo("en-US"));
            // dok.NumerDokumentuRozrachunkow = "";
            pozDph.IDKontoUmowy = kontoUmowy;
            pozDph.IDPartner = Partner;
            //dok.PrzyczynaBlokPlatnosci = "A";
            if (!String.IsNullOrEmpty(rodzajDok))
                naglowek.RodzajDokumentu = rodzajDok;
            if (!String.IsNullOrWhiteSpace(opGl) && opGl.ToUpper().Contains("FP"))
                naglowek.RodzajDokumentu = "FP";
            pozDph.Tekst = opis;


            return dok;

        }



        private PartnerQuery setupGetPartner(GridViewRowInfo row)
        {
            PartnerQuery partn = new PartnerQuery();

            //   partn.Partner.IDPrzedmiotuUmowy = (row.Cells["SAPPrzedmiotUmowy"].Value) as string;
            partn.TypPartnera = (((row.Cells["dlFizPraw"].Value) as string) == "X") ? "2" : "1";
            partn.IDPartnera = (row.Cells["SAPKontoPartnera"].Value) as string;


            return partn;

        }

        private KontoUmowyDefinicja setupGetKonto(GridViewRowInfo row, Konfiguracja knf, int nr)
        {
            KontoUmowyDefinicja getkdl = new KontoUmowyDefinicja();
            string typkdl = "KN";


            if (row.Cells["sprTypKontaUm"] != null && !String.IsNullOrEmpty(row.Cells["sprTypKontaUm"].Value as string))
            {
                typkdl = row.Cells["sprTypKontaUm"].Value as string;
            }

            string opGl; //  = row.Cells["OperacjaGlowna"];
            //if (row.Cells["OperacjaGlowna"] != null && row.Cells["OperacjaGlowna"].Value.ToString().ToUpper().Contains("FP")) {
            try
            {
                typkdl = row.Cells["sprTypKontaUm"].Value.ToString();
            }
            catch (Exception ex)
            {
                typkdl = null;
            }
            //}

            /* @@@@@@@@@@@@@@@ sprawdzic  tu jest null @@@@@@@@@@ */
            getkdl.NumerPartnera = row.Cells["SAPKontoPartnera"].Value.ToString();
            if (getkdl.NumerPartnera.StartsWith("*"))
            {
                getkdl.NumerPartnera = getkdl.NumerPartnera.Substring(1);
            }
            getkdl.IDPrzedmiotuUmowy = row.Cells["SAPPrzedmiotUmowy"].Value.ToString();
            getkdl.NumerKontaUmowy = (row.Cells["SAPKontoUmowy"]!= null && row.Cells["SAPKontoUmowy"].Value != null) ? row.Cells["SAPKontoUmowy"].Value.ToString() : null;
            if (typkdl == "KN" && nr > 0 && nr < 9)
            {
                typkdl = "K" + nr.ToString();
                row.Cells["sprTypKontaUm"].Value = typkdl;
            }
            //if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
            //    getkdl.SadFunkcjonalnyStanowiskoFinansowe = knf.StanowiskoFin;
            getkdl.TypKontaUmowy = typkdl;
            return getkdl;
        }
        /*
        private KontoUmowyDefinicja filterKontaUmowy(KontoUmowyDefinicja[] konta, string opGlowna, string karta_dl, Konfiguracja knf, out string nastepne)
        {
            bool czykns = true;
            string opgl;
            int nr = 0;
            int nrk = 0;
            KontoUmowyDefinicja kumowy = null;
            if (string.IsNullOrWhiteSpace(opGlowna))
                opgl = "N0";
            else
                opgl = opGlowna;


            List<KontoUmowyDefinicja> kontox;
            if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
            {
                kontox = konta.Where(a => a.OznaczenieKontaUmowy == karta_dl && a.StandardowaJednostkaGospodarcza == knf.JednostkaGospodarcza ).OrderByDescending(a => a.TypKontaUmowy).ToList();

            }
            else
                kontox = konta.Where(a => a.OznaczenieKontaUmowy == karta_dl && a.StandardowaJednostkaGospodarcza == knf.JednostkaGospodarcza).OrderByDescending(a => a.TypKontaUmowy).ToList();


            if (kontox != null && kontox.Count > 0)
            {
                foreach (var kn in kontox)
                {
                    if (opgl.Contains("FP"))
                    {
                        if (kn.TypKontaUmowy.Substring(0, 1) == "F")
                        {
                            if (int.TryParse(kn.TypKontaUmowy.Substring(1), out nr))
                            {
                                if (nr > nrk)
                                {
                                    kumowy = kn;
                                    nrk = nr;
                                }
                            }

                        }

                    }
                    else if (kn.TypKontaUmowy.Substring(0, 1) == "K" || kn.TypKontaUmowy.Substring(0, 1) == "L")
                    {
                        if (kn.TypKontaUmowy == "KN")
                        {
                            kumowy = kn;
                        }
                        else if (int.TryParse(kn.TypKontaUmowy.Substring(1), out nr))
                        {
                            if (kn.TypKontaUmowy.Substring(0, 1) == "L")
                                nr += 100;
                            if (nr > nrk)
                            {
                                kumowy = kn;
                                nrk = nr;
                            }
                        }


                    }

                }
                if (kumowy == null)
                {
                    if (opgl.Contains("FP"))
                    {


                    }
                    else
                    {


                    }
                    row.Cells["sprTypKontaUm"].Value = kontox.TypKontaUmowy;
                    thecontext.SaveChanges();

                    goto skipnewkdl;
                }


                if (opGlowna.Contains("FP"))
                {
                    if (konta == null || konta.Length == 0)
                    {
                        nastepne = "F1";
                        return null;
                    }

                    foreach (var kn in konta)
                    {
                        if (kn.TypKontaUmowy.Substring(0, 1) == "F")
                        {
                            if (int.TryParse(kn.TypKontaUmowy, out nr))
                            {
                                if (nr > nrk)
                                    nrk = nr;

                            }
                        }
                    }

                }
                else // kns
                {
                    if (konta == null || konta.Length == 0)
                    {
                        nastepne = "KN";
                        return null;
                    }
                    foreach (var kn in konta)
                    {
                        if (kn.TypKontaUmowy.Substring(0, 1) == "K")
                        {
                            if (int.TryParse(kn.TypKontaUmowy, out nr))
                            {
                                if (nr > nrk)
                                    nrk = nr;

                            }
                        }
                    }



                }



            }

*/

        private OdpisanieNaleznosciElement setupOdpis(GridViewRowInfo row, Konfiguracja knf, string kluczUzg, ref string NumerDokDoDdpis)
        {
            OdpisanieNaleznosciElement dok = new OdpisanieNaleznosciElement();

            dok.CzesciowaOperacja = row.Cells["OperacjaCzesciowa"].Value as string;
            dok.DataDokumentu = row.Cells["DataDokumentu"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
            dok.DataKsiegowania = row.Cells["DataKsiegowania"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //; Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
            dok.DataPlatnosciNetto = row.Cells["DataPlatnosci"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataPlatnosci"].Value).ToString("yyyyMMdd");
            dok.GlownaOperacja = row.Cells["OperacjaGlowna"].Value as string;
            dok.JednostkaGospodarcza = knf.JednostkaGospodarcza;
            dok.KluczUzgodnienia = kluczUzg;
            dok.PrzyczynaBlokPlatnosci = "p";
            dok.TekstWyjasniajacy = row.Cells["Opis"].Value as string;
            dok.KwotaNaleznosci = "-" + Convert.ToDecimal(row.Cells["kwota"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));

            NumerDokDoDdpis = (row.Cells["SAPDocIdRef"].Value) as string;



            return dok;

        }


        private int canOdpis(DocumentListQueryRequest docQuery, decimal kwt, out string message)
        {// sprawdzenie czy można odpisać dany dokment do kwoty 

            DocumentListQueryResponse ans;
            InstalmentPlanVerifyResponse ansPlan;
            InstalmentPlanDeactivateResponse dezPlan;
            try
            {
                ans = ZSRKRequestHelper.PobierzRozrachunki(docQuery);
            }
            catch (Exception ex)
            {
                message = ex.Message + " Błąd wywołania usługi sieciowej - [Pobierz rozrachunki]";
                return -1;
            }

            if (ans != null)
            {
                decimal kwtall = 0;
                decimal kwtfresh = 0;

                if (ans.DokumentPSCD != null && ans.DokumentPSCD.FirstOrDefault() != null)
                {
                    foreach (DokumentPSCD roz in ans.DokumentPSCD)
                    {
                        foreach (PozycjaDokumentPH pozd in roz.PozycjaDokumentPH)
                            if (String.IsNullOrEmpty(pozd.PowodRozliczenia))
                            {
                                kwtall += Convert.ToDecimal(pozd.Kwota.Replace(".", ","));
                                //if (String.IsNullOrEmpty(roz.PozycjaDokumentPH.FirstOrDefault().roz.PozycjaCzesciowaWDokumencie))
                                //        kwtfresh += roz.KwotaWWalucieKrajowej;
                            }

                    }


                    if (kwtall < kwt)  // nie można rozliczyć 
                    {
                        message = "Do rozliczenia została kwota ";
                        if (kwtall > 0)
                            message += kwtall.ToString();
                        else
                            message += "0,00 ";
                        message += " nie można zaksięgować odpisu ";
                        return -1;
                    }
                    else
                    {
                        if (kwtall >= kwt && kwtfresh < kwt)
                        {
                            message = "Do rozliczenia została kwota ";
                            message += kwtall.ToString();
                            message += " ale  SAP2KNS może rozliczyć tylko kwotę " + kwtfresh.ToString();
                            message += " opracuj konto w środowisku SAP";
                            return 1000;
                        }

                        // sprawdzenie czy jest plan rat
                        try
                        {
                            log.Debug("Czy raty ?");
                            ansPlan = ZSRKRequestHelper.SprawdzPlanRat(docQuery.IdDanePSCD.IDDokument, docQuery.IdDanePSCD.IDKontoUmowy);
                            if (ansPlan != null && !string.IsNullOrEmpty(ansPlan.NumerPlanuRat))
                            {
                                // jest plan rat  do dezaktywacji
                                dezPlan = ZSRKRequestHelper.DzeaktywujPlanRat(ansPlan.NumerPlanuRat);

                            }

                        }
                        catch (Exception ex1)
                        {


                            message = ex1.Message + " Błąd wywołania usługi sieciowej - [Weryfikuj/dezaktywujplan rat]";
                            return -1;

                        }
                    }
                }
                message = "";
                return 1;

            }
            else
            {
                message = " Błąd wywołania etody  sieciowej - [Pobierz rozrachunki]";
                return -1;
            }


        }




        private void rmiSaldaKNS_Click(object sender, EventArgs e)
        {
            // Wybór ksiąg
            SprawdzSalda sprSld = new SprawdzSalda();
            sprSld.thecontext = this.thecontext;
            sprSld.ShowDialog();

        }

        // Eksport danych za pomoca usług sieciowych.



        // 



        private void rmiCsv_Click(object sender, EventArgs e)
        {
            int Id;
            bool range;
            // Po0bierz aktualny transfer
            if (this.rgvTransfer.CurrentRow != null)
            {
                Id = Convert.ToInt32(this.rgvTransfer.CurrentRow.Cells["Id"].Value);
                if (Id > 0)
                {

                    switch (MessageBox.Show("Czy  chcesz  eksportować wszystkie wiersze <TAK> , tylko wybrane <NIE>  ?", "Określ zakres eksportu", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case System.Windows.Forms.DialogResult.Yes:
                            range = true;
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            range = false;
                            break;

                        default: return;
                    }
                    //rgvTransfer.DataSource = null;
                    ExportDetails.IdTransfer = Id;
                    //trns = thecontext.Transfer.Where(a => a.Id == TransferId).FirstOrDefault();
                    // usunięcie istniejących 

                    Cursor.Current = Cursors.WaitCursor;
                    thecontext.ExecuteStoreCommand("delete  from Ekstrakcja  where UserId = @p0", new SqlParameter { ParameterName = "p0", Value = -Id });

                    if (!DoEkstrakcja(0, false, range)) { Cursor.Current = Cursors.Default; return; }

                    EkstrakcjadataSource.DataSource = null;
                    EkstrakcjadataSource.DataSource = thecontext.Ekstrakcja.Where(a => a.UserId == -Id).ToList();
                    rgvEkstrakcja.DataSource = EkstrakcjadataSource;
                    Cursor.Current = Cursors.Default;
                    ExtractToCSV(this.rgvEkstrakcja, false, null);
                    thecontext.ExecuteStoreCommand("delete  from Ekstrakcja  where UserId = @p0", new SqlParameter { ParameterName = "p0", Value = -Id });

                }
            }






        }

        private void rmiPing_Click(object sender, EventArgs e)
        {
            rbTestWS_Click(sender, e);
        }

        private string wyznaczTypKonta(KontoUmowyDefinicja[] konta, string rodzajKonta = "KN")
        { int nr = 0, nrk = 0;
            string kumowy;
            bool jest_konto = false;


            foreach (var kn in konta)
            {
                if (rodzajKonta.Contains("FP"))
                {
                    jest_konto = true;
                    if (kn.TypKontaUmowy.Substring(0, 1) == "F")
                    {
                        if (int.TryParse(kn.TypKontaUmowy.Substring(1), out nr))
                        {
                            if (nr > nrk)
                            {

                                nrk = nr;
                            }
                        }

                    }
                    kumowy = "F" + nrk.ToString();
                }
                else if (kn.TypKontaUmowy.Substring(0, 1) == "K" || kn.TypKontaUmowy.Substring(0, 1) == "L")
                {
                    jest_konto = true;
                    if (kn.TypKontaUmowy == "KN")
                    {
                        ;
                    }
                    else if (int.TryParse(kn.TypKontaUmowy.Substring(1), out nr))
                    {
                        if (kn.TypKontaUmowy.Substring(0, 1) == "L")
                            nr += 100;
                        if (nr > nrk)
                        {

                            nrk = nr;
                        }
                    }


                }


            }
            if (rodzajKonta.Contains("FP") && jest_konto == false)
            {
                kumowy = "F1";

            }
            else if (jest_konto == false)
            {
                kumowy = "KN";

            }
            else if (nrk == 9)
            {

                kumowy = "L1";
            }
            else if (nrk >= 100)
            {
                kumowy = "L" + ((nrk % 100) + 1).ToString();

            }
            else
            {
                if (rodzajKonta.Contains("FP"))
                    kumowy = "F" + (nrk + 1).ToString();
                else
                    kumowy = "K" + (nrk + 1).ToString();
            }
            return kumowy;
        }
        /*
        private bool setupFakePartner(GridViewRowInfo theRow)
        {
            if (theRow.Cells["SapKontoPartnera"] != null && theRow.Cells["SapKontoPartnera"].Value != null && theRow.Cells["SapKontoPartnera"].Value.ToString().Length> 0 && theRow.Cells["SapKontoPartnera"].Value.ToString().StartsWith("*"))
            { 
            PartnerQuery arg = new PartnerQuery();
            arg.TypPartnera = "1";
            arg.IDPartnera = theRow.Cells["SapKontoPartnera"].Value.ToString().Substring(1);
            PartnerQueryRequest queryPartner = new PartnerQueryRequest();
            queryPartner.Partner = arg;
                try
                {
                    PartnerQueryResponse resp = (PartnerQueryResponse)(ZSRKRequestHelper.CallSAPMethod("PartnerQueryOut", queryPartner));
                    if (resp.Partnerzy.Length > 0)
                    {





                    }
                }
                catch { return false; }

        }
        */
        private void ExportData(int mode)
        {
            int Id;
            int rowno = 0;
            int allrows = 0;

            int impStatus;
            string diagnostyka;
            string PU;
            string KU;
            string PH;
            string kluczUzg = "";
            
            DateTime dKsiegowania;
            decimal id;
            ContractAccountCreateResponse ans;
            // Po0bierz aktualny transfer
            if (this.rgvTransfer.CurrentRow != null)
            {



                Id = Convert.ToInt32(this.rgvTransfer.CurrentRow.Cells["Id"].Value);
                if (Id > 0)
                {
                    if ((this.rgvTransfer.CurrentRow.Cells["Rodzaj"].Value as int?) != 2 && (this.rgvTransfer.CurrentRow.Cells["Rodzaj"].Value as int?) != 6 && (this.rgvTransfer.CurrentRow.Cells["Rodzaj"].Value as int?) != 7 && (this.rgvTransfer.CurrentRow.Cells["Rodzaj"].Value as int?) != 1)
                    {
                        Utils.showMessage("Rodzaj operacji nie pasuje do typy importu ");
                        return;

                    }
                    /*
                    if ((this.rgvTransfer.CurrentRow.Cells["Rodzaj"].Value as int?) == 1 && mode == 0)
                    {
                        Utils.showMessage("Nie można eksportować dokumentów dla sald - użyj opcji  Tylko dane podstawowe ");
                        return;
                    }
                    */
                    if (RunMode.silentMode)
                        rgvDokumenty.SelectAll();
                    else
                        switch (MessageBox.Show("Czy  chcesz  eksportować do ZSRK wszystkie wiersze <TAK> , tylko dla wybrane <NIE>  ?", "Określ zakres eksportu", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                        {
                            case System.Windows.Forms.DialogResult.Yes:
                                rgvDokumenty.SelectAll();
                                break;
                            case System.Windows.Forms.DialogResult.No:

                                break;

                            default: return;
                        }


                    // walidacja wioerszy
                    // dodanie kolumn
                    int loopcounter = 0;
                    int isvalid = 0;
                    string message;


                    foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
                    {

                        this.rlProgress.Text = "Walidacja " + (++loopcounter).ToString();
                        rlProgress.Refresh();
                        if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;
                        message = ValidateRow(row); 

                        Guid? docguid = row.Cells["DocGuid"].Value as System.Guid?;
                        Ekstrakcja eks = thecontext.Ekstrakcja.Where(a => a.DocGuid == docguid).FirstOrDefault();
                        if (eks != null) message += ";  wiersz o  takim  Id jest na zakładce  Ekstrakcja";
                        if (message.Length > 0)
                        {
                            isvalid++;
                            row.Cells["Blad"].Value = "Uzupełnij :" + message.Truncate(240);

                        }
                        else
                            row.Cells["Blad"].Value = null;


                    }
                    thecontext.SaveChanges();
                    if (isvalid > 0)
                    {

                        Utils.showMessage("Wykryto błędy w " + isvalid.ToString() + "  wierszach. Szczegóły zawarte w kolumnie Info ", " Błąd walidacji ");
                        return;
                    }





                    //rgvTransfer.DataSource = null;
                    ExportDetails.IdTransfer = Id;
                    //trns = thecontext.Transfer.Where(a => a.Id == TransferId).FirstOrDefault();
                    // usunięcie istniejących 

                    Cursor.Current = Cursors.WaitCursor;
                    setSAPConnectionParams();
                    Konfiguracja knf = thecontext.Konfiguracja.FirstOrDefault();
                    try
                    {

                        allrows = this.rgvDokumenty.SelectedRows.Count;
                        rowno = 0;
                        if ((this.rgvTransfer.CurrentRow.Cells["Rodzaj"].Value as int?) == 7) // jeśli zwrot 3/4 
                        {
                            GetDate dtKsie = new GetDate();
                            dtKsie.theDay = DateTime.Today;
                            if (dtKsie.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
                            if (!dtKsie.leaveUnchanged)
                                dKsiegowania = dtKsie.theDay;
                            else
                                dKsiegowania = DateTime.MinValue;

                        }
                        else
                            dKsiegowania = DateTime.MinValue;

                        foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
                        {
                            impStatus = 0;
                            diagnostyka = "";
                            id = Convert.ToDecimal(row.Cells["id"].Value);

                            rlProgress.Text = "Poz: (" + (++rowno).ToString() + "/" + allrows.ToString() + ")" + "                               ";
                            rlProgress.Refresh();
                            row.Cells["Blad"].Value = "";
                            if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;



                            if (row.Cells["SAPPrzedmiotUmowy"].Value != null)
                            {
                                if (!String.IsNullOrEmpty(row.Cells["SAPPrzedmiotUmowy"].Value.ToString().Trim())) goto skipsygn;

                            }
                            SygnaturaTworzenie sygnqry = setupSygnStruct(row, knf);

                            ContractObjectCreateResponse anssygn = ZSRKRequestHelper.ZalozSygnature(sygnqry);

                            if (anssygn != null)
                            {

                                if (anssygn.Sygnatura != null)
                                {
                                    if (anssygn.Sygnatura.IDPrzedmiotuUmowy != null)
                                    {
                                        row.Cells["SAPPrzedmiotUmowy"].Value = anssygn.Sygnatura.IDPrzedmiotuUmowy;
                                        row.Cells["Diagnostyka"].Value = (anssygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0 ? anssygn.Komunikaty[0].Komunikat1 : "");
                                    }
                                    else
                                    {   /******** tu zmiana ***********/

                                        ContractObjectQueryRequest sygnGetquery = setupGetSygnStruct(row, knf);
                                        ContractObjectQueryResponse getsygn = ZSRKRequestHelper.ZnajdzSygnature(sygnGetquery);
                                        if (getsygn == null)
                                        {
                                            
                                            row.Cells["Diagnostyka"].Value = (row.Cells["Diagnostyka"] != null && row.Cells["Diagnostyka"].Value != null ? row.Cells["Diagnostyka"].Value : "") + "Błąd zakładania sygnatury podczas odczytu czy istnieje";
                                            row.Cells["Blad"].Value = " Błąd podczas zakładania sygnatury "  + (row.Cells["Blad"] != null && row.Cells["Blad"].Value != null ? row.Cells["Blad"].Value : "");
                                            impStatus = -1;
                                            diagnostyka = " Bląd podczas zakładania sygnatury";
                                            row.Cells["SAPImportStatus"].Value = -1;
                                            continue;
                                        }
                                        if (getsygn.Sygnatura != null && getsygn.Sygnatura.Length == 1 && getsygn.Sygnatura[0].IDPrzedmiotuUmowy != null && getsygn.Sygnatura[0].OznaczeniePrzedmiotuUmowy.StartsWith(String.IsNullOrWhiteSpace(sygnGetquery.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe) ? sygnGetquery.Sygnatura.JednostkaGospodarcza : sygnGetquery.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe))
                                        {
                                            // dodaj sąd funkcjonalny jeśli istniej !!!! do windykacyjnych 
                                            if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin) && !(sygnGetquery.Sygnatura.SadFunkcjonalnyStanowiskoFinansowe == knf.StanowiskoFin))
                                            {
                                                if (row.Cells["SAPImportPonowne"].Value == null || row.Cells["SAPImportPonowne"].Value.ToString().ToUpper().Trim() != "P")
                                                {
                                                    row.Cells["SAPPrzedmiotUmowy"].Value = getsygn.Sygnatura[0].IDPrzedmiotuUmowy;
                                                    row.Cells["Blad"].Value = " Sygnatura może nie zawierać prawidłowego sądu windykacyjnego. Upewnij się, wprowadż P do kolumy ponownie i ponów operację";
                                                    impStatus = -1;
                                                    diagnostyka = row.Cells["Blad"].Value.ToString();
                                                    row.Cells["SAPImportStatus"].Value = -1;
                                                    continue;
                                                }

                                            }
                                            // jeśłi jestem sądem funkkcjonalnym a sygnatura |}
                                            row.Cells["SAPPrzedmiotUmowy"].Value = getsygn.Sygnatura[0].IDPrzedmiotuUmowy;
                                            row.Cells["Diagnostyka"].Value = (getsygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0 ? anssygn.Komunikaty[0].Komunikat1 : "") + (row.Cells["Diagnostyka"] != null ? row.Cells["Diagnostyka"].Value : "");
                                        }
                                        else
                                        {
                                            row.Cells["Diagnostyka"].Value = (anssygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0 ? anssygn.Komunikaty[0].Komunikat1 : "") + (row.Cells["Diagnostyka"] != null && row.Cells["Diagnostyka"].Value != null ? row.Cells["Diagnostyka"].Value : "");
                                            row.Cells["Blad"].Value = (row.Cells["Blad"]!= null && row.Cells["Blad"].Value != null ? row.Cells["Blad"].Value as string :"") + "; Błąd podczas zakładania sygnatury ";
                                            impStatus = -1;
                                            diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                            row.Cells["SAPImportStatus"].Value = -1;
                                            continue;
                                        }
                                    }
                                }

                                Dokument dok = thecontext.Dokument.Where(a => a.id == id).FirstOrDefault();
                                if (dok != null)
                                {
                                    dok.SAPImportStatus = impStatus;
                                    dok.SAPImportInfo = diagnostyka;
                                    dok.SAPImportInfo = dok.SAPImportInfo.Truncate(255);
                                    if (anssygn.Sygnatura.IDPrzedmiotuUmowy != null)
                                    {

                                        Sprawa spr = thecontext.Sprawa.Where(s => s.Id == dok.Sprawa_Id).FirstOrDefault();
                                        if (spr != null)
                                        {
                                            spr.SentDate = DateTime.Now;
                                            spr.SentBy = UserInfo.Username;
                                            spr.SAPPrzedmiotUmowy = row.Cells["SAPPrzedmiotUmowy"].Value.ToString();
                                        }
                                    }
                                    thecontext.SaveChanges();
                                }
                            }
                            else
                            {
                                Utils.showMessage("Błąd wywołania usługi sieciowej - [Dodaj sygnaturę] dla " + row.Cells["Sygnatura"].Value.ToString() + " kdł: " + row.Cells["Karta"].Value.ToString());
                                break;
                            }


                        skipsygn:
                            // Dodaj partnera
                            if (row.Cells["SAPKontoPartnera"].Value != null)
                            {
                                if (!String.IsNullOrEmpty(row.Cells["SAPKontoPartnera"].Value.ToString().Trim())) goto skippartner;

                            }

                            Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner dluqry = setupBussinessPartner(row, knf);
                            PartnerCreateResponse anspart = ZSRKRequestHelper.DodajPartnera(dluqry);

                            if (anspart != null)
                            {
                                if (anspart.IDPartner != null)
                                {
                                    row.Cells["SAPKontoPartnera"].Value = anspart.IDPartner;
                                    row.Cells["Diagnostyka"].Value = (anspart.Komunikaty != null && anspart.Komunikaty.GetUpperBound(0) >= 0 ? anspart.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                                    diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                }
                                else
                                {
                                    row.Cells["Diagnostyka"].Value = (anspart.Komunikaty != null && anspart.Komunikaty.GetUpperBound(0) >= 0 ? anspart.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                                    row.Cells["Blad"].Value = row.Cells["Blad"].Value as string + "; Błąd podczas zakładania dłużnika";
                                    impStatus = -1;
                                    diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                    row.Cells["SAPImportStatus"].Value = -1;

                                }

                                Dokument dok = thecontext.Dokument.Where(a => a.id == id).FirstOrDefault();
                                if (dok != null)
                                {
                                    dok.SAPImportStatus = impStatus;
                                    dok.SAPImportInfo = diagnostyka + dok.SAPImportInfo;
                                    dok.SAPImportInfo = dok.SAPImportInfo.Truncate(255);
                                    if (anspart.IDPartner != null)
                                    {

                                        Dluznik dlu = thecontext.Dluznik.Where(t => t.Id == dok.Dluznik_Id).FirstOrDefault();
                                        if (dlu != null)
                                        {
                                            dlu.SentBy = UserInfo.Username;
                                            dlu.SentDate = DateTime.Now;
                                            dlu.SAPKontoPartnera = anspart.IDPartner;
                                        }
                                    }

                                    thecontext.SaveChanges();
                                }
                                if (anspart.IDPartner == null) continue;
                            }




                            else
                            {
                                Utils.showMessage("Błąd wywołania usługi sieciowej - [Dodaj Partnera] dla " + row.Cells["Sygnatura"].Value.ToString() + " kdł: " + row.Cells["Karta"].Value.ToString());
                                break;
                            }

                        skippartner:
                            // Dodawanie karty umowy
                            // Sprawdzenie czy takie konto  już istnieje
                            if (row.Cells["SAPKontoPartnera"] == null || row.Cells["SAPKontoPartnera"].Value == null || String.IsNullOrEmpty(row.Cells["SAPKontoPartnera"].Value.ToString().Trim())) continue;
                            if (row.Cells["SAPPrzedmiotUmowy"] == null || row.Cells["SAPPrzedmiotUmowy"].Value == null || String.IsNullOrEmpty(row.Cells["SAPPrzedmiotUmowy"].Value.ToString().Trim())) continue;
                            KontoUmowyDefinicja getkdl = setupGetKonto(row, knf, 0);

                            //getkdl.RelacjaPartneraHandlowego = "99";
                            getkdl.TypKontaUmowy = null;
                            ContractAccountQueryResponse ansget = ZSRKRequestHelper.WyszukajKontoUmowy(getkdl);

                            if (ansget != null)
                            {
                                if (ansget.KontaUmowy != null)
                                    if (ansget.KontaUmowy.GetUpperBound(0) >= 0)
                                    {
                                        int ile = ansget.KontaUmowy.Count();
                                        // jesli znajde z mojeje jednostki z takim samymo oznaczeniem to nie zakładam.
                                        //##PA UWAGA TUTAJ KU
                                        log.Debug("Istnieje konto umowy dla " + getkdl.NumerPartnera + " / " + getkdl.IDPrzedmiotuUmowy);
                                        KontoUmowyDefinicja kontox;
                                        if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                                        {
                                            kontox = ansget.KontaUmowy.Where(a => a.OznaczenieKontaUmowy == row.Cells["Karta"].Value.ToString() && a.StandardowaJednostkaGospodarcza == knf.JednostkaGospodarcza /*&& a.SadFunkcjonalnyStanowiskoFinansowe == knf.StanowiskoFin*/).OrderByDescending(a => a.TypKontaUmowy).FirstOrDefault();

                                        }
                                        else
                                            kontox = ansget.KontaUmowy.Where(a => a.OznaczenieKontaUmowy == row.Cells["Karta"].Value.ToString() && a.StandardowaJednostkaGospodarcza == knf.JednostkaGospodarcza).OrderByDescending(a => a.TypKontaUmowy).FirstOrDefault();

                                        if (kontox != null)
                                        {
                                            bool jestkonto = false;
                                            if (row.Cells["OperacjaGlowna"] != null && row.Cells["OperacjaGlowna"].Value != null)
                                            {
                                                if (row.Cells["OperacjaGlowna"].Value.ToString().ToUpper().Contains("FP"))
                                                {
                                                    if (kontox.TypKontaUmowy.Substring(0, 1) == "F")
                                                        jestkonto = true;
                                                }
                                                else
                                                {
                                                    if (kontox.TypKontaUmowy.Substring(0, 1) == "K" || kontox.TypKontaUmowy.Substring(0, 1) == "L")
                                                        jestkonto = true;

                                                }
                                            }
                                            else
                                                jestkonto = true;

                                            if (jestkonto)
                                            {
                                                log.Debug("Ustalono konto umowy na podstawie nr karty " + row.Cells["Karta"].Value);
                                                row.Cells["KontoUmowy"].Value = kontox.NumerKontaUmowy;
                                                row.Cells["sprTypKontaUm"].Value = kontox.TypKontaUmowy;
                                                thecontext.SaveChanges();

                                                goto skipnewkdl;
                                            }
                                            else
                                            {
                                                log.Debug("Brak konta umowy możliwa niezgodność operacji głównej z typem konta");

                                            }
                                        }

                                        {  // dodajemy tego samego partnera
                                            log.Debug("Wyznaczanie typu konta umowy dla " + getkdl.NumerPartnera + " / " + getkdl.IDPrzedmiotuUmowy);
                                            //var ksiegiCfg = this.thecontext.KnsKsiegi.Where(x=>x.nazwa )
                                            if (row.Cells["OperacjaGlowna"].Value != null && row.Cells["OperacjaGlowna"].Value.ToString().ToUpper().Contains("FP"))
                                            {
                                                //    var kontaDluznikow = ansget.KontaUmowy.Where(x => x.TypKontaUmowy.StartsWith("F")).ToList();
                                                //    row.Cells["sprTypKontaUm"].Value = string.Concat("F" + (kontaDluznikow.Count()+1).ToString());
                                                //}
                                                //else
                                                //{
                                                log.Debug("Wyznaczanie typu konta umowy dla " + getkdl.NumerPartnera + " / " + getkdl.IDPrzedmiotuUmowy);
                                                var kontaDluznikow = ansget.KontaUmowy.Where(x => x.TypKontaUmowy.StartsWith("F")).OrderBy(x => x.TypKontaUmowy).ToList();
                                                //wyznaczenie maksymalnego nr
                                                int nr = 0;
                                                if (kontaDluznikow != null)
                                                {
                                                    foreach (var kx in kontaDluznikow)
                                                    {
                                                        if (kx.TypKontaUmowy.Length > 1)
                                                        {
                                                            int nrk = 0;
                                                            if (int.TryParse(kx.TypKontaUmowy.Substring(1), out nrk))
                                                            {
                                                                if (nrk > nr)
                                                                    nr = nrk;

                                                            }

                                                        }

                                                    }
                                                }
                                                nr += 1;
                                                row.Cells["sprTypKontaUm"].Value = "F" + nr.ToString();
                                            }
                                            else
                                            {
                                                log.Debug("wyznaczanie nr karty dla kns ");
                                                var kontaDluznikow = ansget.KontaUmowy.Where(x => x.TypKontaUmowy.StartsWith("K") && !x.TypKontaUmowy.StartsWith("KO")).OrderByDescending(a => a.TypKontaUmowy).ToList();
                                                if (kontaDluznikow == null || kontaDluznikow.Count() == 0)
                                                {
                                                    row.Cells["sprTypKontaUm"].Value = "KN";
                                                }
                                                else ///*********************** tu zmiana  L1-L9 *********************/
                                                {
                                                    log.Debug("Pobieranie kont umów dla " + getkdl.NumerPartnera + " / " + getkdl.IDPrzedmiotuUmowy);

                                                    var kontax = kontaDluznikow.Where(a => a.TypKontaUmowy != "KN").OrderByDescending(a => a.TypKontaUmowy);
                                                    string newType = string.Empty;
                                                    if (kontax == null || kontax.Count() == 0)
                                                        newType = "K1";
                                                    else
                                                    {

                                                        newType = wyznaczTypKonta(ansget.KontaUmowy);
                                                        //string tkm = (kontax.OrderByDescending(a => a.TypKontaUmowy).FirstOrDefault()).TypKontaUmowy;
                                                        //log.Debug("Ostatnie konto = " + tkm);
                                                        //if (tkm == "K9")
                                                        //    tkm = "L1";
                                                        //else
                                                        //    tkm = tkm.Substring(0,1)+ (Convert.ToInt32(kontax.FirstOrDefault().TypKontaUmowy.Substring(1)) + 1).ToString();
                                                        //newType = tkm;
                                                        ////log.Debug("nowe konto umowy - typ K: " + kontax.FirstOrDefault().TypKontaUmowy);
                                                        ////newType = "K" + (Convert.ToInt32(kontax.FirstOrDefault().TypKontaUmowy.Substring(1)) + 1).ToString();
                                                        log.Debug("nowe konto umowy - typ " + newType);
                                                    }
                                                    //  this.ostatniTypKontaUmowy = string.Concat("K" + (kontaDluznikow.Count()).ToString());

                                                    row.Cells["sprTypKontaUm"].Value = newType;
                                                }
                                            }

                                            //}

                                            Ex2PscdInterface.Ex2PscdPartnerCreateOutService.Partner dluqry_new = setupBussinessPartner(row, knf);
                                            //##PA Duplikaty KN
                                            //if (dluqry_new.TypPartnera == "1")
                                            //    // osoba fizyczna
                                            //    dluqry_new.PESEL = "";
                                            //else
                                            //    dluqry_new.NIP = "";

                                            if (dluqry_new.AdresPartner.Ulica.Contains(" "))
                                                dluqry_new.AdresPartner.Ulica = Utils.ReplaceFirst(dluqry_new.AdresPartner.Ulica, " ", "  ");
                                            else if (dluqry_new.AdresPartner.Ulica.Contains("."))
                                                dluqry_new.AdresPartner.Ulica.Replace(".", " .");
                                            else dluqry_new.AdresPartner.Ulica = dluqry_new.AdresPartner.Ulica + ".";


                                            PartnerCreateResponse anspart_new = ZSRKRequestHelper.DodajPartnera(dluqry_new);
                                            if (anspart_new != null)
                                            {

                                                if (anspart_new.IDPartner != null)
                                                {
                                                    row.Cells["SAPKontoPartnera"].Value = anspart_new.IDPartner;
                                                    row.Cells["Diagnostyka"].Value = (anspart_new.Komunikaty != null && anspart_new.Komunikaty.GetUpperBound(0) >= 0 ? anspart_new.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                                                    diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                                }
                                                else
                                                {
                                                    row.Cells["Diagnostyka"].Value = (anspart_new.Komunikaty != null && anspart_new.Komunikaty.GetUpperBound(0) >= 0 ? anspart_new.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                                                    row.Cells["Blad"].Value = row.Cells["Blad"].Value as string + "; Błąd podczas zakładania dłużnika";
                                                    diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                                    impStatus = -1;
                                                    row.Cells["SAPImportStatus"].Value = -1;

                                                }



                                                Dokument dok = thecontext.Dokument.Where(a => a.id == id).FirstOrDefault();
                                                if (dok != null)
                                                {
                                                    dok.SAPImportStatus = impStatus;
                                                    dok.SAPImportInfo = diagnostyka + dok.SAPImportInfo;
                                                    dok.SAPImportInfo = dok.SAPImportInfo.Truncate(255);
                                                    if (anspart_new.IDPartner != null)
                                                    {

                                                        Dluznik dlu = thecontext.Dluznik.Where(t => t.Id == dok.Dluznik_Id).FirstOrDefault();
                                                        if (dlu != null)
                                                        {
                                                            dlu.SentDate = DateTime.Now;
                                                            dlu.SentBy = UserInfo.Username;
                                                            dlu.SAPKontoPartnera = anspart_new.IDPartner;
                                                        }
                                                        else
                                                        {
                                                            Utils.showMessage("Nie znalezniono partnera dla dokumentu podczas zakładania obiektu " + row.Cells["SAPKontoPartnera"].Value.ToString());
                                                            return;
                                                        }
                                                    }

                                                    thecontext.SaveChanges();
                                                }
                                                if (anspart_new.IDPartner == null) continue;
                                            }




                                            else
                                            {
                                                Utils.showMessage("Błąd wywołania usługi sieciowej - [Dodaj Partnera] dla " + row.Cells["Sygnatura"].Value.ToString() + " kdł: " + row.Cells["Karta"].Value.ToString());
                                                break;
                                            }

                                        }
                                        //if (ile > 0)
                                        //    typkdl = "K" + ile.ToString();

                                    }
                                    else { // brak konta umowy dla  partnera
                                        
                                        
                                    }
                            }
                            else
                            {
                                Utils.showMessage("Błąd wywołania usługi sieciowej - [Dodaj Partnera] dla " + row.Cells["Sygnatura"].Value.ToString() + " kdł: " + row.Cells["Karta"].Value.ToString());
                                break;
                            }


                            if (row.Cells["KontoUmowy"].Value != null)
                            {
                                if (!String.IsNullOrEmpty(row.Cells["KontoUmowy"].Value.ToString().Trim())) goto skipkdl;

                            }

                            if (row.Cells["SAPKontoPartnera"].Value == null)
                            {

                                Utils.showMessage("Dla karty  " + row.Cells["Karta"].Value.ToString() + " nie wyeksportowano dłużnika. Wyeksportuj go i ponów eksport karty.", "Dłużnik nie został wyeksportowany do ZSRK");
                                row.Cells["Blad"].Value = "Brak numeru partnera ( Wyeksportuj go)";
                                row.Cells["SAPImportStatus"].Value = 0;
                                continue;
                            }
                            if (row.Cells["SAPPrzedmiotUmowy"].Value == null)
                            {

                                Utils.showMessage("Dla karty  " + row.Cells["Karta"].Value.ToString() + " nie wyeksportowano sygnatury. Wyeksportuj ją i ponów eksport karty.", "Dłużnik nie został wyeksportowany do ZSRK");
                                row.Cells["Blad"].Value = "Brak numeru Sygnatury ( Wyeksportuj ją)";
                                row.Cells["SAPImportStatus"].Value = 0;
                                continue;
                            }

                        addkdl:
                            string typkdl = "KN";

                            if (row.Cells["sprTypKontaUm"] != null && !String.IsNullOrEmpty(row.Cells["sprTypKontaUm"].Value as string))
                            {
                                typkdl = row.Cells["sprTypKontaUm"].Value as string;
                            }
                            else
                            {
                                if (row.Cells["OperacjaGlowna"] != null && row.Cells["OperacjaGlowna"].Value.ToString().ToUpper().Contains("FP"))
                                {
                                    typkdl = "F1";
                                }

                                row.Cells["sprTypKontaUm"].Value = typkdl;
                            }

                            KontoUmowyTworzenie kdlqry = setupKdl(row, knf, typkdl);

                            ans = ZSRKRequestHelper.DodajKontoUmowy(kdlqry);
                            if (ans != null)
                            {
                                if (ans.KontoUmowyIdentyfikacja != null)
                                {
                                    if (ans.KontoUmowyIdentyfikacja.NumerKontaUmowy != null)
                                    {
                                        row.Cells["KontoUmowy"].Value = ans.KontoUmowyIdentyfikacja.NumerKontaUmowy;
                                        row.Cells["Diagnostyka"].Value = (ans.Komunikaty != null && ans.Komunikaty.GetUpperBound(0) >= 0 ? ans.Komunikaty[0].Komunikat1 : "");
                                        diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                        ;
                                    }
                                    else
                                    {
                                        row.Cells["Diagnostyka"].Value = (ans.Komunikaty != null && ans.Komunikaty.GetUpperBound(0) >= 0 ? ans.Komunikaty[0].Komunikat1 : "");
                                        diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                        impStatus = -1;
                                        row.Cells["SAPImportStatus"].Value = -1;

                                    }

                                }
                                Dokument dok = thecontext.Dokument.Where(a => a.id == id).FirstOrDefault();
                                if (dok != null)
                                {
                                    dok.SAPImportStatus = impStatus;
                                    dok.SAPImportInfo = diagnostyka + dok.SAPImportInfo;
                                    dok.SAPImportInfo = dok.SAPImportInfo.Truncate(255);
                                    if (ans.KontoUmowyIdentyfikacja.NumerKontaUmowy != null)
                                    {

                                        Sprawa spr = thecontext.Sprawa.Where(s => s.Id == dok.Sprawa_Id).FirstOrDefault();
                                        if (spr != null)
                                        {
                                            spr.SentDate = DateTime.Now;
                                            spr.SentBy = UserInfo.Username;
                                            spr.SAPKontoUmowy = ans.KontoUmowyIdentyfikacja.NumerKontaUmowy;
                                            spr.SAPTypKontaUmowy = typkdl;
                                        }
                                        else
                                        {
                                            Utils.showMessage("Nie znalezniono sprawy dla dokumentu podczas zakładania konta umowy " + ans.KontoUmowyIdentyfikacja.NumerKontaUmowy);
                                            return;
                                        }
                                    }

                                    thecontext.SaveChanges();
                                }
                                else
                                {
                                    Utils.showMessage("Nie znalezniono sprawy dla dokumentu podczas zakładania konta umowy " + ans.KontoUmowyIdentyfikacja.NumerKontaUmowy);
                                    return;
                                }
                            }




                            else
                            {
                                Utils.showMessage("Błąd wywołania usługi sieciowej - [Dodaj konto umowy] dla " + row.Cells["Sygnatura"].Value.ToString() + " kdł: " + row.Cells["Karta"].Value.ToString());
                                break;
                            }

                        skipkdl:
                            // sprawdź czy jest relacja 
                            getkdl = setupGetKonto(row, knf, 0);
                            if (row.Cells["SAPKontoPartnera"] != null && row.Cells["SAPKontoPartnera"].Value != null && row.Cells["SAPKontoPartnera"].Value.ToString().Trim().StartsWith("*"))
                            {

                                ;
                            }
                            else
                            {
                                ContractAccountQueryResponse ansget1 = ZSRKRequestHelper.WyszukajKontoUmowy(getkdl);
                                if (ansget1 != null)
                                {
                                    if (ansget1.KontaUmowy != null)
                                        if (ansget1.KontaUmowy.GetUpperBound(0) >= 0)
                                            if (ansget1.KontaUmowy[0].NumerPartnera == row.Cells["SAPKontoPartnera"].Value.ToString() && ansget1.KontaUmowy[0].IDPrzedmiotuUmowy == row.Cells["SAPPrzedmiotUmowy"].Value.ToString()) { row.Cells["SAPImportStatus"].Value = 1; continue; }
                                }
                            }
                                
                            Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy kdl = new Ex2PscdInterface.Ex2PscdContractAccountRelationCreateOutService.KontoUmowy();
                            kdl.TypKontoUmowy = getkdl.TypKontaUmowy;
                            kdl.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                            kdl.IDPartnera = row.Cells["SAPKontoPartnera"].Value.ToString();
                            kdl.RelacjaKonta = row.Cells["sprRelacjaKUm"].Value as string;
                            kdl.Opis = row.Cells["Karta"].Value as string;

                            if (String.IsNullOrEmpty(kdl.RelacjaKonta)) kdl.RelacjaKonta = "99";
                            if (!String.IsNullOrWhiteSpace(knf.StanowiskoFin))
                                kdl.StanowiskoFinansowe = knf.StanowiskoFin;




                            ContractAccountRelationCreateResponse ans1 = ZSRKRequestHelper.AktualizujKontoUmowy(kdl, row.Cells["SAPPrzedmiotUmowy"].Value.ToString());
                            if (ans1.Komunikaty != null)
                            {
                                if (ans1.Komunikaty[0].RodzajKomunikatu == "E")
                                {
                                    string newpartner;
                                    newpartner = AddNewDl(row, knf);
                                    if (!String.IsNullOrWhiteSpace(newpartner)) goto addkdl;


                                    row.Cells["Diagnostyka"].Value = (ans1.Komunikaty != null && ans1.Komunikaty.GetUpperBound(0) >= 0 ? ans1.Komunikaty[0].Komunikat1 : "");
                                    row.Cells["Blad"].Value = ";Założono konto umowy ale nie powiązano go z sygnaturą " + row.Cells["Blad"].Value;
                                    diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                    impStatus = -1;
                                    row.Cells["SAPImportStatus"].Value = -1;
                                    continue;
                                }
                                else // odnaleziono konto umowy
                                {
                                    ;

                                }
                            }
                            else
                            {
                                Utils.showMessage("Błąd wywołania usługi sieciowej - [Aktualizuj konto umowy] dla " + row.Cells["Sygnatura"].Value.ToString() + " kdł: " + row.Cells["Karta"].Value.ToString());
                                break;
                            }
                        skipnewkdl:
                            // Ksiegowanie dokumentu
                            if (row.Cells["SAPDocId"].Value != null && !String.IsNullOrEmpty(row.Cells["SAPDocId"].Value.ToString().Trim())) continue;
                            if (row.Cells["KontoUmowy"] == null || row.Cells["SAPKontoPartnera"] == null || row.Cells["SAPPrzedmiotUmowy"] == null || String.IsNullOrEmpty(row.Cells["KontoUmowy"].Value.ToString()) || String.IsNullOrEmpty(row.Cells["SAPKontoPartnera"].Value.ToString()) || String.IsNullOrEmpty(row.Cells["SAPPrzedmiotUmowy"].Value.ToString()))
                            {
                                row.Cells["Blad"].Value = "brak obiektów podstawowywch do zaksięgowania dokumentu;" + row.Cells["Blad"].Value;
                                continue;
                            }
                            // sprawdzenie czy są powiazane konto, partner i przedmiot 
                            getkdl = setupGetKonto(row, knf, 0);

                            ContractAccountQueryResponse ansget2 = ZSRKRequestHelper.WyszukajKontoUmowy(getkdl);
                            if (ansget2 != null)
                            {
                                if (ansget2.KontaUmowy != null && ansget2.KontaUmowy.GetUpperBound(0) >= 0 && ansget2.KontaUmowy[0].NumerPartnera == row.Cells["SAPKontoPartnera"].Value.ToString() && ansget2.KontaUmowy[0].IDPrzedmiotuUmowy == row.Cells["SAPPrzedmiotUmowy"].Value.ToString())
                                    ;
                                else
                                {
                                    row.Cells["KontoUmowy"].Value = null;
                                    //row.Cells["SAPKontoPartnera"].Value = null;
                                    //row.Cells["SAPPrzedmiotUmowy"].Value = null;
                                    //row.Cells["dlNip"].Value = null;
                                    //row.Cells["dlpesel"].Value = null;
                                    // tu powinno być zakładanie konta K1, KN....
                                    row.Cells["Blad"].Value = "Nie można zestawić relacji Konta umowy" + row.Cells["Blad"].Value as string;
                                    impStatus = -1;
                                    row.Cells["SAPImportStatus"].Value = -1;
                                    continue;

                                }

                            }
                            else
                            {
                                Utils.showMessage("Błąd usługi sieciowej [Wyszukaj konto umowy]");
                                return;
                            }
                            if (mode == 0)
                            {
                                string docID = null;
                                string typop = (row.Cells["typFakt"].Value).ToString().Trim().ToUpper();
                                if (typop == "KP" || typop == "GP" || typop == "GS" || typop == "KS")
                                {
                                    if (dKsiegowania != DateTime.MinValue)
                                        row.Cells["DataKsiegowania"].Value = dKsiegowania;

                                    DocumentCreateRequest adddok = this.setupPrzypis(row, knf, kluczUzg);
                                    DocumentCreateResponse ansdok = ZSRKRequestHelper.DodajPrzypis(adddok);
                                    if (ansdok != null)
                                    {
                                        if (!String.IsNullOrWhiteSpace(ansdok.IDDokument))
                                        {
                                            if (ansdok.Komunikaty[0].RodzajKomunikatu == "E")
                                            {
                                                row.Cells["Diagnostyka"].Value = (ansdok.Komunikaty != null && ansdok.Komunikaty.GetUpperBound(0) >= 0 ? ansdok.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                                                row.Cells["Blad"].Value = "Błąd eksportu dokumentu;" + row.Cells["Blad"].Value;
                                                diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                                impStatus = -1;
                                                row.Cells["SAPImportStatus"].Value = -1;


                                            }
                                            else
                                            {
                                                row.Cells["SAPDocId"].Value = ansdok.IDDokument;
                                                docID = ansdok.IDDokument;
                                                row.Cells["Diagnostyka"].Value = (ansdok.Komunikaty != null && ansdok.Komunikaty.GetUpperBound(0) >= 0 ? ansdok.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                                                diagnostyka = row.Cells["Diagnostyka"].Value.ToString();

                                            }
                                        }
                                        else
                                        {
                                            row.Cells["Diagnostyka"].Value = (ansdok.Komunikaty != null && ansdok.Komunikaty.GetUpperBound(0) >= 0 ? ansdok.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                                            row.Cells["Blad"].Value = " Błąd eksportu dokumentu " + row.Cells["Blad"].Value;
                                            diagnostyka = row.Cells["Diagnostyka"].Value.ToString();
                                            impStatus = -1;
                                            row.Cells["SAPImportStatus"].Value = -1;
                                        }


                                        Dokument dok = thecontext.Dokument.Where(a => a.id == id).FirstOrDefault();

                                        if (dok != null)
                                        {
                                            dok.SentBy = UserInfo.Username;
                                            dok.SentDate = DateTime.Now;

                                            dok.SAPImportStatus = impStatus;
                                            dok.SAPImportInfo += diagnostyka;
                                            dok.SAPImportInfo = dok.SAPImportInfo.Truncate(255);
                                            if (!String.IsNullOrWhiteSpace(docID)) dok.SAPDocId = docID.Trim();

                                            thecontext.SaveChanges();
                                        }
                                    }
                                    if (row.Cells["SAPDocId"].Value != null && row.Cells["KontoUmowy"].Value != null && row.Cells["SAPKontoPartnera"].Value != null && row.Cells["SAPPrzedmiotUmowy"].Value != null && row.Cells["SAPDocId"].Value.ToString().Trim().Length > 0 && row.Cells["KontoUmowy"].Value.ToString().Trim().Length > 0 && row.Cells["SAPKontoPartnera"].Value.ToString().Trim().Length > 0 && row.Cells["SAPPrzedmiotUmowy"].Value.ToString().Trim().Length > 0)
                                    {


                                        Dokument dok1 = thecontext.Dokument.Where(a => a.id == id).FirstOrDefault();
                                        dok1.SAPImportStatus = 1;
                                        row.Cells["SAPImportStatus"].Value = 1;
                                        thecontext.SaveChanges();

                                    }

                                }
                            }
                        }



                        Cursor.Current = Cursors.Default;

                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        Utils.showMessage("Błąd: " + ex.Message + "\nStack trace : " + ex.StackTrace + (ex.InnerException != null ? " Szczegóły " + ex.InnerException.Message : ""));
                        return;
                    }
                }
            }



        }


        private void rmiDpodst_Click(object sender, EventArgs e)
        {
            ExportData(0);
            if (this.rgvTransfer.CurrentRow != null)
            {
                Transfer t = this.rgvTransfer.CurrentRow.DataBoundItem as Transfer;

                if (t != null)
                {

                    t.Bledne = this.thecontext.Dokument.Where(a => a.Transfer_Id == t.Id && a.SAPImportStatus < 0).Count();
                    t.Zaimportowane = this.thecontext.Dokument.Where(a => a.Transfer_Id == t.Id && a.SAPImportStatus > 0).Count();
                    t.LFaktow = this.thecontext.Dokument.Where(a => a.Transfer_Id == t.Id).Count();

                }
                //eksport wszystkich danych        
            }
        }

        private void rRBWindows_ToggleStateChanged(object sender, StateChangedEventArgs args)
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

        private void rgvEkstrakcja_Initialized(object sender, EventArgs e)
        {
            InitEkstrakcja();
        }
        private void setSAPConnectionParams(bool bezAutent = false)
        {
            using (KnsMigratorEntities context = new KnsMigratorEntities())
            {
                User usr = context.User.Where(a => a.Id == UserInfo.Id).FirstOrDefault();
                setSAPConnectionParams(usr, bezAutent);
            }
        }


        private void setSAPConnectionParams(User u,bool bezAutent = false)
        {
            using (KnsMigratorEntities context = new KnsMigratorEntities())
            {
                Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

                List<ServiceEndpoint> l = context.ServiceEndpoint.ToList();
                if (l != null)
                    foreach (ServiceEndpoint s in l)
                    {
                        lst.Add(new KeyValuePair<string, string>(s.ServiceName, s.Endpoint));
                    }

                ZSRKRequestHelper.ServiceMapping = lst;
                ZSRKRequestHelper.AuthCert = new X509Certificate2(knf.Pfx, Utils.Decrypt(knf.PfxPassword, "Application error"));
                
                ZSRKRequestHelper.BasicAuthLogin = knf.WSLogon;
                ZSRKRequestHelper.BasicAuthPassword = knf.WSpwd;
                if (!bezAutent)
                {
                    ZSRKRequestHelper.MEPUser = u.MEPUser;
                    ZSRKRequestHelper.MEPPassword = Utils.Decrypt(u.MEPPassword, "Application error");
                    SignatureHelper.Password = Utils.Decrypt(u.MEPPassword, "Application error");
                    SignatureHelper.SetCert(knf.Cer);
                }
                ZSRKRequestHelper.ApplicationID = knf.AppName;
                ZSRKRequestHelper.JednostkaGospodarcza = knf.JednostkaGospodarcza;

               

            }


            ;



        }
        private void setSAPConnectionParamsCons(User u, bool bezAutent = false)
        {
            using (KnsMigratorEntities context = new KnsMigratorEntities())
            {
                Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

                List<ServiceEndpoint> l = context.ServiceEndpoint.ToList();
                if (l != null)
                    foreach (ServiceEndpoint s in l)
                    {
                        lst.Add(new KeyValuePair<string, string>(s.ServiceName, s.Endpoint));
                    }

                ConsWebServiceHelper.ServiceMapping = lst;
                ConsWebServiceHelper.AuthCert = new X509Certificate2(knf.Pfx, Utils.Decrypt(knf.PfxPassword, "Application error"));

                ConsWebServiceHelper.BasicAuthLogin = knf.WSLogon;
                ConsWebServiceHelper.BasicAuthPassword = knf.WSpwd;
                if (!bezAutent)
                {
                    ConsWebServiceHelper.MEPUser = u.MEPUser;
                    ConsWebServiceHelper.MEPPassword = Utils.Decrypt(u.MEPPassword, "Application error");
                    SignatureHelper.Password = Utils.Decrypt(u.MEPPassword, "Application error");
                    SignatureHelper.SetCert(knf.Cer);
                }
                ConsWebServiceHelper.ApplicationID = knf.AppName;
                ConsWebServiceHelper.JednostkaGospodarcza = knf.JednostkaGospodarcza;



            }


           ;



        }
        



        private void rbTestWS_Click(object sender, EventArgs e)
        {
            User usr = null;
            SelectUser su = new SelectUser();
            string requestStr = string.Empty;
            if (su.ShowDialog() == DialogResult.OK)
            {
                usr = su.SelectedUser;
            }
            else return;

            updateKonfig();
            setSAPConnectionParamsCons(usr);
            GetStatusContentSystemDataRequest arg = new GetStatusContentSystemDataRequest();
            arg.GUID = Guid.NewGuid().ToString();
           
            try
            {
                var result = ConsImport.ConsWebServiceHelper.GetDataStatus("GetStatusContentSystemData", arg, out requestStr);
                if (result.ListaKomunikat != null)
                {
                    string s = string.Empty;
                    foreach (var k in result.ListaKomunikat)
                    {
                        s += "\n\r" + (k.TypKomunikatu + " " + k.NumerKomunikatu + " " + k.TrescKomunikatu).Trim();


                    }
                    if (!String.IsNullOrWhiteSpace(s))
                        MessageBox.Show(s);
                    else
                        MessageBox.Show("Połączenie z systemem API CONS  przebiegło pomyślnie");
                    

                }
                else
                    MessageBox.Show("Nieznany błąd podczas połączenia z API CONS");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ZSRKRequestHelper.GetErrorMessage() + ex.Message, "Błąd podczas próby połączenia z API CONS");

            }
        }

        private void rbTestWSEx2PSCD_Click(object sender, EventArgs e)
        {
            User usr = null;
            SelectUser su = new SelectUser();
            if (su.ShowDialog() == DialogResult.OK)
            {
                usr = su.SelectedUser;
            }
            else return;

            updateKonfig();
            setSAPConnectionParams(usr);
            PartnerQuery arg = new PartnerQuery();
            arg.TypPartnera = "1";
            arg.PESEL = "94050395939";
            PartnerQueryRequest queryPartner = new PartnerQueryRequest();
            queryPartner.Partner = arg;
            try
            {
                PartnerQueryResponse resp = (PartnerQueryResponse)(ZSRKRequestHelper.CallSAPMethod("PartnerQueryOut", queryPartner));
                if (resp.Komunikaty != null && resp.Komunikaty.ToList().Count > 0)
                {
                    string s = string.Empty;
                    foreach (Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Komunikat k in resp.Komunikaty)
                    {
                        s += "\n\r" + (k.IDKomunikatu + " " + k.NumerKomunikatu + " " + k.Komunikat1 + " " + k.RodzajKomunikatu).Trim();


                    }
                    if (!String.IsNullOrWhiteSpace(s))
                        MessageBox.Show(s);
                    else
                        MessageBox.Show("Połączenie z systemem ZSRK przebiegło pomyślnie");


                }
                else
                    MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd podczas połączenia z Ex2PSCD");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ZSRKRequestHelper.GetErrorMessage() + ex.Message, "Błąd podczas próby połączenia z Ex2PSCD");

            }
        }


        private void ExportOdpis()
        {
            int Id;
            int rowno = 0;
            int allrows = 0;
            string kluczUzg = "";
            string dokPrzyp = "";
            SapPIService.DodajKontoUmowyOdpowiedz ans;
            // Po0bierz aktualny transfer
            if (this.rgvTransfer.CurrentRow != null)
            {
                Id = Convert.ToInt32(this.rgvTransfer.CurrentRow.Cells["Id"].Value);

                if (Id > 0)
                {
                    if ((this.rgvTransfer.CurrentRow.Cells["Rodzaj"].Value as int?) != 3)
                    {
                        Utils.showMessage(" rodzaj operacji nie pasuje do typy importu ");
                        return;

                    }

                    switch (MessageBox.Show("Czy  chcesz  eksportować dane podstawowe  dla wszystkich wierszy <TAK> , tylko dla wybranych <NIE>  ?", "Określ zakres eksportu", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case System.Windows.Forms.DialogResult.Yes:
                            rgvDokumenty.SelectAll();
                            break;
                        case System.Windows.Forms.DialogResult.No:

                            break;

                        default: return;
                    }
                    //rgvTransfer.DataSource = null;
                    ExportDetails.IdTransfer = Id;
                    //trns = thecontext.Transfer.Where(a => a.Id == TransferId).FirstOrDefault();
                    // usunięcie istniejących 

                    Cursor.Current = Cursors.WaitCursor;
                    Konfiguracja knf = thecontext.Konfiguracja.FirstOrDefault();
                    setSAPConnectionParams();
                    try
                    {
                        allrows = this.rgvDokumenty.SelectedRows.Count;
                        rowno = 0;

                        foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
                        {
                            rlProgress.Text = "Poz: (" + (++rowno).ToString() + "/" + allrows.ToString() + ")" + "                               ";
                            rlProgress.Refresh();

                            log.Debug("Eksport odpisu");


                            if (row.Cells["SAPDocId"].Value != null && !String.IsNullOrEmpty(row.Cells["SAPDocId"].Value.ToString().Trim())) continue;
                            // .ToString().Trim())) { row.Cells["Blad"].Value += ";Brak dokumentu przypisu lub salda"; continue; }
                            log.Debug("Krok 1");
                            if (row.Cells["SAPDocIdRef"].Value == null || String.IsNullOrEmpty(row.Cells["SAPDocIdRef"].Value.ToString().Trim()))
                            { row.Cells["Blad"].Value = "Brak dokumentu przypisu lub salda;" + row.Cells["Blad"].Value; continue; }
                            log.Debug("Krok 2");
                            string typop = (row.Cells["typFakt"].Value).ToString().Trim().ToUpper();
                            if (typop == "KO" || typop == "GO")
                            {
                                string err;
                                decimal kwt;

                                log.Debug("Krok 3");
                                kwt = Convert.ToDecimal(row.Cells["kwota"].Value);

                                int retcode = 0;
                                // Weryfikacja czy można odpisaćnależność
                                DocumentListQueryRequest doc2query = new DocumentListQueryRequest();
                                doc2query.IdDanePSCD = new IdDanePSCDZapytanie();


                                if (row.Cells["OperacjaGlowna"].Value != null && ((row.Cells["OperacjaGlowna"].Value as string).StartsWith("F") || (row.Cells["OperacjaGlowna"].Value as string).StartsWith("N")))
                                {
                                    doc2query.IdDanePSCD.TypKontoUmowy = "F1";
                                }
                                else
                                {
                                    doc2query.IdDanePSCD.TypKontoUmowy = "KN";
                                }
                                doc2query.IdDanePSCD.IDDokument = row.Cells["SAPDocIdRef"].Value == null ? null : row.Cells["SAPDocIdRef"].Value.ToString();
                                doc2query.IdDanePSCD.IDKontoUmowy = row.Cells["SAPKontoUmowy"].Value == null ? null :  row.Cells["SAPKontoUmowy"].Value.ToString();
                                doc2query.IdDanePSCD.IDPartner = row.Cells["SAPKontoPartnera"].Value == null ? null:  row.Cells["SAPKontoPartnera"].Value.ToString();
                                doc2query.IdDanePSCD.IDSygnatura = row.Cells["SAPPrzedmiotUmowy"].Value == null ? null:  row.Cells["SAPPrzedmiotUmowy"].Value.ToString();
                                doc2query.IdDanePSCD.JednostkaGospodarcza = konfig.JednostkaGospodarcza;


                                doc2query.PozDoWyj = new PozDoWyj();
                                doc2query.PozDoWyj.IdPozycjaWyj = "";
                                doc2query.PozDoWyj.PartiaPlatnosciID = "";
                                doc2query.PozDoWyj.PartiaPlatnosciNrPozycja = "";


                                log.Debug("Sprawdzenie możliwości odpisu");
                                if ((retcode = canOdpis(doc2query, kwt, out err)) > 0)
                                {
                                    log.Debug("Krok 1");
                                    OdpisanieNaleznosciElement odpdok = this.setupOdpis(row, knf, kluczUzg, ref dokPrzyp);
                                    if (String.IsNullOrWhiteSpace(odpdok.CzesciowaOperacja) || String.IsNullOrWhiteSpace(odpdok.GlownaOperacja))
                                    {
                                        row.Cells["Diagnostyka"].Value = "Nie uzupełniona operacja główna lub częściowa" + row.Cells["Diagnostyka"].Value;
                                        row.Cells["SAPImportStatus"].Value = -1;
                                        row.Cells["Blad"].Value = "Brak operacji głównej lub częściowej " + row.Cells["Blad"].Value;
                                        continue;
                                    }
                                    DocumentReductionDebtResponse ansdok = ZSRKRequestHelper.OdpiszNaleznosc(dokPrzyp, odpdok);
                                    log.Debug("Krok 2");
                                    {
                                        if (ansdok != null)
                                        {
                                            log.Debug("Krok 3");
                                            if (ansdok != null && ansdok.OdpisanieNaleznosciOdpowiedz != null && ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu != null && ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu.Trim().Length > 0)
                                            {
                                                log.Debug("Krok 4");
                                                row.Cells["SAPDocId"].Value = ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu;
                                                {
                                                    string s = string.Empty;
                                                    foreach (var k in ansdok.Komunikaty)
                                                    {
                                                        s += k.IDKomunikatu + " " + k.Komunikat1 + " " + k.RodzajKomunikatu;
                                                    }
                                                    row.Cells["Diagnostyka"].Value += s;

                                                }
                                                log.Debug("Krok 5");
                                                if (!String.IsNullOrWhiteSpace(ansdok.OdpisanieNaleznosciOdpowiedz.NumerDokumentuRozliczeniaOdpisu))
                                                {
                                                    log.Debug("Krok 4,5");
                                                    row.Cells["SAPCLDocId"].Value = ansdok.OdpisanieNaleznosciOdpowiedz.NumerDokumentuRozliczeniaOdpisu;
                                                    row.Cells["SAPImportStatus"].Value = 1;
                                                }
                                                else
                                                    row.Cells["SAPImportStatus"].Value = -1;
                                            }
                                            else
                                            {
                                                log.Debug("Krok 6");
                                                row.Cells["Diagnostyka"].Value = (ansdok.Komunikaty != null && ansdok.Komunikaty.GetUpperBound(0) >= 0 ? ansdok.Komunikaty[0].Komunikat1 : "") + row.Cells["Diagnostyka"].Value;
                                                row.Cells["SAPImportStatus"].Value = -1;
                                                row.Cells["Blad"].Value = ";Błąd eksportu dokumentu " + row.Cells["Blad"].Value;

                                            }

                                        }
                                        else
                                        {
                                            if (retcode == 1000)
                                            {
                                                log.Debug("Krok 7");
                                                row.Cells["Blad"].Value = err + row.Cells["Blad"].Value;
                                                row.Cells["SAPImportStatus"].Value = -1000;
                                            }
                                            else
                                            {
                                                log.Debug("Krok 8");
                                                row.Cells["Blad"].Value = ";Nieznany błąd podczas eksportu odpisu" + row.Cells["Blad"].Value;
                                                row.Cells["SAPImportStatus"].Value = -1;
                                            }
                                        }
                                        decimal? id = Convert.ToDecimal(row.Cells["id"].Value);
                                        Dokument dok = thecontext.Dokument.Where(a => a.id == id).FirstOrDefault();
                                        if (dok != null)
                                        {
                                            dok.SentBy = UserInfo.Username;
                                            dok.SentDate = DateTime.Now;
                                            log.Debug("Krok 9");
                                            dok.SAPImportStatus = row.Cells["SAPImportStatus"].Value as int?;
                                            dok.SAPImportInfo = (row.Cells["Diagnostyka"].Value != null ? row.Cells["Diagnostyka"].Value.ToString() : "") + dok.SAPImportInfo;
                                            dok.SAPImportInfo = dok.SAPImportInfo.Truncate(255);
                                            if (row.Cells["SAPDocId"].Value != null) dok.SAPDocId = row.Cells["SAPDocId"].Value.ToString().Trim();
                                            thecontext.SaveChanges();
                                            log.Debug("Krok 10");
                                        }
                                    }
                                }
                                else
                                {
                                    row.Cells["Blad"].Value = err + row.Cells["Blad"].Value;
                                    row.Cells["SAPImportStatus"].Value = -1;


                                }

                            }



                        }



                        Cursor.Current = Cursors.Default;

                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        Utils.showMessage("Błąd: " + ex.Message + "\nStack trace : " + ex.StackTrace + (ex.InnerException != null ? " Szczegóły " + ex.InnerException.Message : ""));
                        return;
                    }
                }

            }

        }

        private void rmi_OdpisyWS_Click(object sender, EventArgs e)
        {
            this.ExportOdpis();
        }


        private void rbClearFilters_Click(object sender, EventArgs e)
        {
            this.rgvDokumenty.MasterTemplate.FilterDescriptors.Clear();
        }

        private void rmiPrzypisOplat_Click(object sender, EventArgs e)
        {

        }

        private void rmiDanePodst_Click(object sender, EventArgs e)
        {

            ExportData(1);// eksport tylko danych podstawowywch 
        }
     

        private void rmiRunOdpis_Click(object sender, EventArgs e)
        {
            decimal kwota;
            decimal do_odpisu = 0;
            string DocId;
            string KontoUmowy;
            int status = 0;
            string msg = "";
            tbCurrent.Visible = true;
            tbAll.Visible = true;
            tbKarta.Visible = true;
            Konfiguracja knf = thecontext.Konfiguracja.FirstOrDefault();

            tbAll.Text = rgvMasowe.SelectedRows.Count.ToString();
            tbAll.Refresh();
            int i = 0;
            foreach (GridViewRowInfo theRow in rgvMasowe.SelectedRows)
            {

                tbCurrent.Text = (++i).ToString();
                tbKarta.Text = theRow.Cells["Karta"].Value.ToString();
                tbKarta.Refresh();
                tbCurrent.Refresh();
                kwota = Convert.ToDecimal(theRow.Cells["kwota"].Value);
                DocId = theRow.Cells["SAPDocId"].Value.ToString();
                KontoUmowy = theRow.Cells["SAPKontoUmowy"].Value.ToString();

                do_odpisu = getSaldoSAP(DocId, KontoUmowy, knf.JednostkaGospodarcza, kwota, out msg, out status);
                theRow.Cells["status"].Value = status;
                theRow.Cells["informacja"].Value = msg;
                theRow.Cells["SaldoSAP"].Value = do_odpisu;

            }
            rmiDoOdpis.Enabled = true;
            rmIExpOdpCsv.Enabled = true;
        }


        private void setOdpisyDS(RadGridView rgv, bool koszty2016 = false)
        {
            SqlDataReader rdr = null;
            SqlConnection con = null;
            SqlCommand msSQLCommand;

            try
            {
                // Open connection to the database
                Cursor.Current = Cursors.WaitCursor;
                string ConnectionString = Properties.Settings.Default.KnsMigratorConnectionString; //Utils.BuildMyConnectionString(thecontext);
                con = new SqlConnection(ConnectionString);
                con.Open();
                //con.Open();
                if (koszty2016 == true)
                {

                    msSQLCommand = new SqlCommand(" select * from v_DokDoOdpisu where DataKsiegowania < '2017-01-01' and (typFakt = 'KP' or typFakt = 'KS')   order by KnsKsiega, KdRok, KdNumer ");
                }
                else
                    msSQLCommand = new SqlCommand(" select * from v_DokDoOdpisu order by KnsKsiega, KdRok, KdNumer ");
                msSQLCommand.CommandType = CommandType.Text;
                msSQLCommand.CommandTimeout = 600;
                msSQLCommand.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter();

                da.SelectCommand = msSQLCommand;
                da.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                da.Fill(dt);
                rgv.DataSource = dt;

                Cursor.Current = Cursors.Default;

            }
            catch (Exception ex)
            {
                // Print error message
                Cursor.Current = Cursors.Default;
                Utils.showMessage(ex.Message);
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


        private decimal getSaldoSAP(String docNo, string kontoUmowy, string jednostkaGosp, decimal kwt, out string message, out int status)
        {


            // Po0bierz aktualny transfer
            Cursor.Current = Cursors.WaitCursor;

            DocumentListQueryRequest docQuery = new DocumentListQueryRequest();
            docQuery.IdDanePSCD = new IdDanePSCDZapytanie();
            docQuery.IdDanePSCD.IDDokument = docNo;
            docQuery.IdDanePSCD.JednostkaGospodarcza = jednostkaGosp;
            docQuery.PozDoWyj = new PozDoWyj();
            docQuery.PozDoWyj.IdPozycjaWyj = "";
            docQuery.PozDoWyj.PartiaPlatnosciID = "";
            docQuery.PozDoWyj.PartiaPlatnosciNrPozycja = "";

            DocumentListQueryResponse ans;
            DokumentPSCD[] rozlicz;
            InstalmentPlanVerifyResponse ansPlan;

            try
            {
                ans = ZSRKRequestHelper.PobierzRozrachunki(docQuery);
                rozlicz = ans.DokumentPSCD;
            }
            catch (Exception ex)
            {
                message = ex.Message + " Błąd wywołania usługi sieciowej - [Pobierz rozrachunki]";
                status = -1;
                return 0.0M;
            }
            if (ans != null)
            {
                /*@@@@@@@@@@@@@@@
                if (ans.Komunikaty != null && ans.Komunikaty[0] != null && ans.Komunikaty[0][0].RodzajKomunikatu == "E")
                {
                    status = -2;
                    message = ans.Komunikaty[0][0].Komunikat1;
                    return 0.00M;

                }
                */
                if (ans.DokumentPSCD[0].PozycjaDokumentPH.FirstOrDefault() != null && ans.DokumentPSCD[0].PozycjaDokumentPH[0].DokumentRozliczeniowy != null)
                {
                    //  rozlicz = ans.DokumentPSCD[0].PozycjaDokumentPH[0].DokumentRozliczeniowy
                    if (!String.IsNullOrEmpty(rozlicz[0].PozycjaDokumentPH[0].PowodRozliczenia) || Convert.ToDecimal(rozlicz[0].PozycjaDokumentPH[0].Kwota) <= 0 || Convert.ToDecimal(rozlicz[0].PozycjaDokumentPH[0].Kwota) > kwt)  // nie można rozliczyć 
                    {
                        message = "Do rozliczenia została kwota ";
                        if (String.IsNullOrEmpty(rozlicz[0].PozycjaDokumentPH[0].PowodRozliczenia))
                            message += rozlicz[0].PozycjaDokumentPH[0].Kwota;
                        else
                            message += "0,00 ";
                        status = 0;
                        return 0.00M;
                    }
                    else
                    {
                        // sprawdzenie czy jest plan rat
                        try
                        {
                            ansPlan = ZSRKRequestHelper.SprawdzPlanRat(docNo, kontoUmowy);
                            if (ansPlan != null && !string.IsNullOrEmpty(ansPlan.NumerPlanuRat))
                            {
                                message = "Znaleziono plan ratalny dla dokumentu: " + ansPlan.NumerPlanuRat;// jest plan rat  do dezaktywacji
                                // dezPlan = sapPI.DzeaktywujPlanRat(ansPlan.NumerPlanuRat);
                                status = 1;
                            }
                            else
                            {
                                status = 100;
                                message = "";
                            }
                            return Convert.ToDecimal(rozlicz[0].PozycjaDokumentPH[0].Kwota);
                        }
                        catch (Exception ex1)
                        {


                            message = ex1.Message + " Błąd wywołania usługi sieciowej - [Weryfikuj/dezaktywujplan rat]";
                            status = -100;

                        }
                    }
                }
                status = 100;
                message = "";
                return kwt;

            }
            else
            {
                message = " Błąd wywołania metody  sieciowej - [Pobierz rozrachunki]";
                status = -100;
                return 0.0M;
            }


        }



        private void rmiValidRun_Click(object sender, EventArgs e)
        {
            SprawdzSalda ssld = new SprawdzSalda();
            ssld.thecontext = thecontext;
            ssld.mode = 0;
            ssld.ShowDialog();

            WalidSaldoDataSource.DataSource = this.thecontext.WalidSaldo.Where(a => a.Klucz == ssld.myId).ToList();
            // dodanie podsumowań



            GridViewSummaryItem summaryItemKwota = new GridViewSummaryItem("Kwota", "{0}", GridAggregateFunction.Sum);
            GridViewSummaryItem summaryItemSAPKwota = new GridViewSummaryItem("SAPKwota", "{0}", GridAggregateFunction.Sum);
            GridViewSummaryItem summaryItemStatus = new GridViewSummaryItem("Status", "{0}", GridAggregateFunction.Count);



            GridViewSummaryRowItem summaryRowItem = new GridViewSummaryRowItem(new GridViewSummaryItem[] { summaryItemStatus, summaryItemKwota, summaryItemSAPKwota });

            this.rgvValidSaldo.SummaryRowsTop.Add(summaryRowItem);

            this.rgvEkstrakcja.Visible = false;
            this.rgvValidSaldo.Visible = true;
            this.rgvSAPWplaty.Visible = false;
            this.rgvMasowe.Visible = false;
            this.rgvValidSaldo.Dock = DockStyle.Fill;
            this.rgvValidSaldo.DataSource = WalidSaldoDataSource;

        }

        private void rmIExpOdpCsv_Click(object sender, EventArgs e)
        {
            // eksport do csv.
            int rowno = 0;
            string err;
            decimal kwt;
            string opGL;
            string kluczUzg;
            string opis;
            string dokRefNo;
            int index;
            GridViewRowInfo selRow;
            Dokument dokref;
            DateTime dKsie, dDok;


            Konfiguracja knf = thecontext.Konfiguracja.FirstOrDefault();
            if (this.rgvMasowe.SelectedRows.Count <= 0) return;
            GetOdpisDetails wnd = new GetOdpisDetails();
            if (wnd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            Cursor.Current = Cursors.WaitCursor;

            opGL = wnd.opGl;
            kluczUzg = wnd.kluczUzg;
            dKsie = wnd.dKsiegowania;
            dDok = wnd.dDok;
            opis = wnd.Opis;
            Random rnd = new Random();
            index = rnd.Next(1000000, 100000000);
            // usunięcie 
            tbAll.Text = this.rgvMasowe.SelectedRows.Count.ToString();
            tbAll.Refresh();
            selRow = null;
            try
            {
                thecontext.ExecuteStoreCommand("delete  from Ekstrakcja  where UserId = @p0", new SqlParameter { ParameterName = "p0", Value = -index });
                foreach (GridViewRowInfo row in this.rgvMasowe.SelectedRows)
                {
                    tbCurrent.Text = (++rowno).ToString();
                    selRow = row;
                    tbCurrent.Refresh();
                    if ((row.Cells["status"].Value as int?) > 0)
                    {
                        if (row.Cells["status"].Value as int? == 1) // jeśli są raty
                        {
                            // dezaktywacja rat
                            InstalmentPlanDeactivateResponse dezPlan;
                            string planRat = row.Cells["informacja"].Value.ToString();
                            planRat = planRat.Substring(planRat.IndexOf("dokumentu:") + "dokumentu:".Length + 1).Trim();
                            dezPlan = ZSRKRequestHelper.DzeaktywujPlanRat(planRat);

                        }
                        dokRefNo = row.Cells["SAPDocId"].Value.ToString();
                        dokref = thecontext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.SAPDocId == dokRefNo).FirstOrDefault();
                        Ekstrakcja ekstr = new Ekstrakcja();


                        ekstr.KodOperacji = (row.Cells["typfakt"].Value).ToString().Trim();
                        if (ekstr.KodOperacji == "KP" || ekstr.KodOperacji == "KS")
                            ekstr.KodOperacji = "KO";
                        else
                            ekstr.KodOperacji = "GO";
                        ekstr.SAPImportPonowne = "";
                        ekstr.Osoba_fizyczna_Osoba_prawna = dokref.Dluznik.FizPraw;
                        ekstr.Imię_Nazwa1 = dokref.Dluznik.Imie;
                        ekstr.Nazwisko__Nazwa2 = dokref.Dluznik.Nazwisko;
                        ekstr.Ulica = dokref.Dluznik.Ulica;
                        ekstr.Nrdomu = dokref.Dluznik.NrDomu;
                        ekstr.Nrmieszkania = dokref.Dluznik.NrMieszkania;
                        ekstr.Kodpocztowy = dokref.Dluznik.KodPocztowy;
                        ekstr.Miejscowość = dokref.Dluznik.Miejscowosc;
                        ekstr.Kluczkraju = dokref.Dluznik.KluczKraju;
                        ekstr.NIP = dokref.Dluznik.Nip;
                        ekstr.Pesel = dokref.Dluznik.Pesel;
                        ekstr.KwalifikatordoRBN = dokref.Dluznik.RBN;
                        ekstr.Typkontaumowy = dokref.Sprawa.SAPTypKontaUmowy;
                        if (String.IsNullOrWhiteSpace(ekstr.Typkontaumowy))
                            ekstr.Typkontaumowy = "KN";

                        ekstr.Oznaczeniekontaumowy = dokref.Sprawa.Karta;
                        ekstr.Relacjakonta = dokref.Sprawa.SAPRelacjaKontaUmowy;
                        if (String.IsNullOrWhiteSpace(ekstr.Relacjakonta))
                            ekstr.Relacjakonta = "99";
                        ekstr.GrupaJG = ""; //row.Cells["RelacjaKonta"].Value as string;
                        ekstr.StandardowaJG = knf.JednostkaGospodarcza; //""; //
                        ekstr.StanowiskoFinansoweKU = knf.StanowiskoFin;
                        ekstr.Rodzajprzedmiotuumowy = dokref.Sprawa.SAPRodzajPrzedmiotuUmowy;
                        ekstr.JednostkaGospodarcza = dokref.Sprawa.SAPSadId;
                        if (ekstr.JednostkaGospodarcza != null)
                        {
                            int jego;
                            if (int.TryParse(ekstr.JednostkaGospodarcza, out jego))
                                if (jego > 5000)   // stanowisko finansowe; 
                                {
                                    ekstr.StanowiskoFinansowePU = ekstr.JednostkaGospodarcza;
                                    string jedngosp = ekstr.JednostkaGospodarcza;
                                    SAPSad ss = thecontext.SAPSad.Where(d => d.kod == jedngosp).FirstOrDefault();
                                    ekstr.JednostkaGospodarcza = ss.JEGO;
                                }
                        }
                        ekstr.Nrwydziałuisekcji = dokref.Sprawa.SAPWydział;
                        ekstr.Repertorium = dokref.Sprawa.SAPRepertorium;
                        ekstr.NrSprawy = dokref.Sprawa.Numer.ToString();
                        ekstr.Rok = dokref.Sprawa.Rok.ToString();
                        ekstr.Rodzajsprawy = dokref.Sprawa.SAPRodzajSprawy;
                        ekstr.Ilośćtomów = dokref.Sprawa.SAPTomyAkt;
                        ekstr.SygnaturaPoprzednia = dokref.Sprawa.Sygnatura;
                        ekstr.Datadokumentu = dDok.ToString("yyyyMMdd");
                        ekstr.Dataksięgowania = dKsie.ToString("yyyyMMdd");
                        ekstr.Rodzajdokumentu = dokref.SAPRodzajDokumentu;
                        if (String.IsNullOrWhiteSpace(ekstr.Rodzajdokumentu))
                            ekstr.Rodzajdokumentu = "NS";
                        ekstr.Waluta = dokref.SAPWaluta;
                        if (String.IsNullOrEmpty(ekstr.Waluta))
                            ekstr.Waluta = "PLN";
                        ekstr.Kluczuzgodnienia = "";//row.Cells["KluczUzgodnienia"].Value as string;
                        ekstr.JednostkaGospodarcza32 = knf.JednostkaGospodarcza; //row.Cells["JednostakaGospodarcaWłasna"].Value as string;
                        ekstr.Operacjagłówna = opGL;
                        ekstr.Operacjaczęściowa = row.Cells["OperacjaCzesciowa"].Value as string;
                        ekstr.KwotawPLN = Convert.ToDecimal(row.Cells["SaldoSAP"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                        ekstr.Datapłatności = dDok.ToString("yyyyMMdd");
                        ekstr.Stannależności = dokref.Stan;
                        ekstr.Opis = opis;
                        ekstr.DocGuid = Guid.NewGuid();
                        ekstr.NumerPartnera = dokref.Dluznik.SAPKontoPartnera;
                        ekstr.NumerKontaUmowy = dokref.Sprawa.SAPKontoUmowy;
                        ekstr.NumerPrzedmiotuUmowy = dokref.Sprawa.SAPPrzedmiotUmowy;
                        ekstr.NumerDokumentuReferencyjnego = dokref.SAPDocId;
                        ekstr.NumerDokumentuPlanRat = "";
                        ekstr.RataData1 = "";
                        ekstr.RataData2 = "";
                        ekstr.RataData3 = "";
                        ekstr.RataData4 = "";
                        ekstr.RataData5 = "";
                        ekstr.RataData6 = "";
                        ekstr.RataData7 = "";
                        ekstr.RataData8 = "";
                        ekstr.RataData9 = "";
                        ekstr.RataData10 = "";
                        ekstr.RataData11 = "";
                        ekstr.RataData12 = "";
                        ekstr.RataData13 = "";
                        ekstr.RataData14 = "";
                        ekstr.RataData15 = "";
                        ekstr.RataData16 = "";
                        ekstr.RataData17 = "";
                        ekstr.RataData18 = "";
                        ekstr.RataData19 = "";
                        ekstr.RataData20 = "";
                        ekstr.RataData21 = "";
                        ekstr.RataData22 = "";
                        ekstr.RataData23 = "";
                        ekstr.RataData24 = "";
                        ekstr.RataData25 = "";
                        ekstr.RataData26 = "";
                        ekstr.RataData27 = "";
                        ekstr.RataData28 = "";
                        ekstr.RataData29 = "";
                        ekstr.RataData30 = "";
                        ekstr.RataData31 = "";
                        ekstr.RataData32 = "";
                        ekstr.RataData33 = "";
                        ekstr.RataData34 = "";
                        ekstr.RataData35 = "";
                        ekstr.RataData36 = "";
                        ekstr.UserId = -index;
                        ekstr.IsDeleted = false;
                        thecontext.Ekstrakcja.AddObject(ekstr);

                    }
                    thecontext.SaveChanges();

                }

                EkstrakcjadataSource.DataSource = null;
                EkstrakcjadataSource.DataSource = thecontext.Ekstrakcja.Where(a => a.UserId == -index).ToList();
                rgvEkstrakcja.DataSource = EkstrakcjadataSource;
                Cursor.Current = Cursors.Default;
                ExtractToCSV(this.rgvEkstrakcja, false, null);
                thecontext.ExecuteStoreCommand("delete  from Ekstrakcja  where UserId = @p0", new SqlParameter { ParameterName = "p0", Value = -index });
            } // try

            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                if (selRow != null)
                    Utils.showMessage("Błąd podczas ekstrakcji dłużnik " + selRow.Cells["Karta"].ToString() + " " + selRow.Cells["SAPDocId"].ToString() + ex.Message);
                else
                    Utils.showMessage("Błąd podczas ekstrakcji " + ex.Message);

            }




        }

        private void rmi_DokPrzypis_Click(object sender, EventArgs e)
        {
            // weryfikacja dokument
            SprawdzSalda ssld = new SprawdzSalda();
            ssld.thecontext = thecontext;
            ssld.mode = 1;
            ssld.ShowDialog();

            WalidSaldoDataSource.DataSource = this.thecontext.WalidSaldo.Where(a => a.Klucz == ssld.myId).ToList();
            // dodanie podsumowań




            this.rgvEkstrakcja.Visible = false;
            this.rgvSAPWplaty.Visible = false;
            this.rgvValidSaldo.Visible = true;
            this.rgvMasowe.Visible = false;
            this.rgvValidSaldo.Dock = DockStyle.Fill;
            this.rgvValidSaldo.DataSource = WalidSaldoDataSource;

        }

        private void rgvSygnMap_Initialized(object sender, EventArgs e)
        {
            this.InitSygnMapping();
        }



        private void rmiPokazWplaty_Click(object sender, EventArgs e)
        {
            // pobierz nowe wplaty
            SAPWplaty wpl = thecontext.SAPWplaty.OrderByDescending(a => a.DataKsiegowania).FirstOrDefault();
            if (wpl != null)
                this.GetWplatyOdDo(wpl.DataKsiegowania.Value, DateTime.Today);
            else
                this.GetWplatyOdDo(DateTime.Today, DateTime.Today);

        }

        private void rmiPobierzWplaty_Click(object sender, EventArgs e)
        {
            this.GetWplatyOdDo(this.dtWplOd.Value, this.dtWplDo.Value);

        }

        private void btRefrWpl_Click(object sender, EventArgs e)
        {
            this.rgvEkstrakcja.Visible = false;
            this.rgvSAPWplaty.Visible = true;
            this.rgvValidSaldo.Visible = false;
            this.rgvMasowe.Visible = false;
            this.rgvSAPWplaty.Dock = DockStyle.Fill;

            ReloadWplaty();
        }

        private void rbExactDay_CheckedChanged(object sender, EventArgs e)
        {
            if (rbExactDay.Checked) dtpTerminWymag.Enabled = true; else dtpTerminWymag.Enabled = false;
        }

        private void ktowykonalMenuItem_Click(object sender, EventArgs e)
        {

            if (rgvDokumenty.SelectedRows.Count > 0 && rgvDokumenty.CurrentRow != null)
            {
                Dokument dok = (Dokument)rgvDokumenty.CurrentRow.DataBoundItem;
                if (dok != null)
                {
                    WhoWnd whwnd = new WhoWnd();
                    whwnd.docId = dok.id;
                    whwnd.ShowDialog();

                }
            }
        }

        private void rgvDokumenty_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {

            RadMenuItem ktowykonalMenuItem = new RadMenuItem();
            ktowykonalMenuItem.Text = "Kto Wykonał";
            RadMenuSeparatorItem separator = new RadMenuSeparatorItem();
            ktowykonalMenuItem.Click += new EventHandler(ktowykonalMenuItem_Click);
            e.ContextMenu.Items.Add(separator);
            e.ContextMenu.Items.Add(ktowykonalMenuItem);
            RadMenuSeparatorItem separator1 = new RadMenuSeparatorItem();
            RadMenuItem refreshMenuItem = new RadMenuItem();
            refreshMenuItem.Text = "Odśwież zaznaczone";
            refreshMenuItem.Click += new EventHandler(refreshMenuItem_Click);
            e.ContextMenu.Items.Add(separator1);
            e.ContextMenu.Items.Add(refreshMenuItem);

            RadMenuItem refreshAllMenuItem = new RadMenuItem();
            refreshAllMenuItem.Text = "Odśwież wszystki";
            refreshAllMenuItem.Click += new EventHandler(refreshAllMenuItem_Click);
            e.ContextMenu.Items.Add(refreshAllMenuItem);

        }

        private string serializePos(IEnumerable<GridViewRowInfo> lstdok, out string blacklst, out List<int> ksiegiLst)
        {
            string blackList = "";
            string posLst = string.Empty;
            ksiegiLst = new List<int>();
            foreach (GridViewRowInfo row in lstdok)
            {
                Dokument d = row.DataBoundItem as Dokument;
                if (d.SAPImportStatus == 1)
                {
                    if (!String.IsNullOrEmpty(blackList))
                    {
                        blackList += "\n";

                    }
                    blackList += d.Sprawa.Karta + " " + d.Dluznik.Nazwisko + "\t- pozycja została zapisana w ZSRK";
                    continue;
                }
                if (!String.IsNullOrEmpty(posLst))
                    posLst += ",";
                posLst += (d.Sprawa.KnsSprawa_id > 10000000 ? (d.Sprawa.KnsSprawa_id - 10000000).ToString() : d.Sprawa.KnsSprawa_id.ToString());
                if (d.Sprawa.KnsKsiega != null && !ksiegiLst.Contains(d.Sprawa.KnsKsiega.Value))
                    ksiegiLst.Add(d.Sprawa.KnsKsiega.Value);

            }

            blacklst = blackList;
            return posLst;
        }



        void refreshAllMenuItem_Click(object sender, EventArgs e)
        {
            IEnumerable<GridViewRowInfo> lst = this.rgvDokumenty.Rows;
            this.doUpdate(lst);
        }

        void refreshMenuItem_Click(object sender, EventArgs e)
        {
            IEnumerable<GridViewRowInfo> lst = this.rgvDokumenty.SelectedRows;
            this.doUpdate(lst);

        }

        private void doUpdate(IEnumerable<GridViewRowInfo> lstrows)
        {


            this.thecontext.SaveChanges();
            this.thecontext = null;
            this.thecontext = new KnsMigratorEntities();
            if (lstrows == null || !lstrows.Any())
            {
                Utils.showMessage("Brak pozycji do weryfikacji");
                return;

            }
            Dokument dk = lstrows.First().DataBoundItem as Dokument;


            imp = new Imports();
            imp.Context = thecontext;
            imp.Konfig = konfig;
            Transfer T = thecontext.Transfer.Where(a => a.Id == dk.Transfer_Id).FirstOrDefault();
            if (T == null || T.rodzaj != 2)
            {
                Utils.showMessage(" Opcja dostępna tylko dla przypisów ");
                return;
            }
            imp.data_od = T.DataOd.Value;
            imp.theday = T.DataDo.Value;
            imp.updateTransfer = T;
            string blackLst = "";
            List<int> ksKns;
            imp.sprList = serializePos(lstrows, out blackLst, out ksKns);
            if (!String.IsNullOrWhiteSpace(blackLst))
                Utils.showMessage(blackLst, "Pozycje zostaną pominięte  - zostały zaimportowane do ZSRK");
            imp.KsiegiKnsLst = ksKns;
            imp.typImport = 2;

            //imp.ImportSaldo();
            Cursor.Current = Cursors.WaitCursor;
            imp.ImportPrzypis();
            this.DokumentyBindingDataSource.DataSource = thecontext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.Transfer_Id == TransferId).OrderBy(b => b.Sprawa.KdNumer).OrderBy(b => b.Sprawa.KdRok).OrderBy(a => a.Sprawa.KnsKsiega).ToList();
            this.rgvDokumenty.DataSource = this.DokumentyBindingDataSource;
            Cursor.Current = Cursors.Default;

            //Thread thImportRaty = new Thread(imp.ImportRaty);
            //thImportRaty.Start();

            /*
             foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
            {

                this.rlProgress.Text = "Walidacja " + (++loopcounter).ToString();
                rlProgress.Refresh();
                if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;
                message =  ValidateRow (row);
              */




        }


        private void rmiWszystkieNal_Click(object sender, EventArgs e)
        {
            rgvEkstrakcja.Visible = false;
            rgvSAPWplaty.Visible = false;
            rgvSAPWplaty.Visible = false;
            rgvMasowe.Visible = true;
            rgvMasowe.Dock = DockStyle.Fill;
            setOdpisyDS(rgvMasowe);


            rmiRunOdpis.Enabled = true;
        }

        private void rmiKoszty2017_Click(object sender, EventArgs e)
        {
            rgvEkstrakcja.Visible = false;
            rgvSAPWplaty.Visible = false;
            rgvSAPWplaty.Visible = false;
            rgvMasowe.Visible = true;
            rgvMasowe.Dock = DockStyle.Fill;
            setOdpisyDS(rgvMasowe, true);


            rmiRunOdpis.Enabled = true;
        }

        private void rmiOdpisOnly_Click(object sender, EventArgs e)
        {
            int rowno = 0;
            string err;
            decimal kwt;
            string opGL;
            string kluczUzg;
            string opis;
            DateTime dKsie, dDok;

            Cursor.Current = Cursors.WaitCursor;
            Konfiguracja knf = thecontext.Konfiguracja.FirstOrDefault();
            if (this.rgvMasowe.SelectedRows.Count <= 0) return;
            GetOdpisDetails wnd = new GetOdpisDetails();
            if (wnd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            opGL = wnd.opGl;
            kluczUzg = wnd.kluczUzg;
            dKsie = wnd.dKsiegowania;
            dDok = wnd.dDok;
            opis = wnd.Opis;


            // SapPIHelper sapPI = new SapPIHelper(knf);
            tbAll.Text = this.rgvMasowe.SelectedRows.Count.ToString();
            tbAll.Refresh();

            foreach (GridViewRowInfo row in this.rgvMasowe.SelectedRows)
            {
                tbCurrent.Text = (++rowno).ToString();
                tbCurrent.Refresh();
                if ((row.Cells["status"].Value as int?) > 0)
                {
                    if (row.Cells["status"].Value as int? == 1) // jeśli są raty
                    {
                        // dezaktywacja rat
                        InstalmentPlanDeactivateResponse dezPlan;
                        string planRat = row.Cells["informacja"].Value.ToString();
                        planRat = planRat.Substring(planRat.IndexOf("dokumentu:") + "dokumentu:".Length + 1).Trim();
                        dezPlan = ZSRKRequestHelper.DzeaktywujPlanRat(planRat);

                    }

                    OdpisanieNaleznosciElement dok = new OdpisanieNaleznosciElement();

                    try
                    {
                        dok.CzesciowaOperacja = row.Cells["OperacjaCzesciowa"].Value as string;
                        dok.DataDokumentu = dDok.ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
                        dok.DataKsiegowania = dKsie.ToString("yyyyMMdd"); //; Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
                        dok.DataPlatnosciNetto = dDok.ToString("yyyyMMdd");
                        dok.GlownaOperacja = opGL;
                        dok.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                        dok.KluczUzgodnienia = kluczUzg;
                        dok.PrzyczynaBlokPlatnosci = "A";
                        dok.TekstWyjasniajacy = opis;
                        dok.KwotaNaleznosci = "-" + Convert.ToDecimal(row.Cells["SaldoSAP"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                        DocumentReductionDebtResponse ansdok = ZSRKRequestHelper.OdpiszNaleznosc(row.Cells["SAPDocId"].Value.ToString(), dok);
                        if (ansdok != null)
                        {
                            if (ansdok != null && ansdok.OdpisanieNaleznosciOdpowiedz != null && ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu != null && ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu.Trim().Length > 0)
                            {
                                row.Cells["informacja"].Value = "Numer dokumentu " + ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu;
                                row.Cells["Status"].Value = 1000;
                            }
                            else
                            {
                                row.Cells["informacja"].Value = "Błąd podczas próby księgowania brak numeru dokumentu " + ansdok.Komunikaty[0].Komunikat1 + " " + ansdok.Komunikaty[0].NumerKomunikatu + " " + ansdok.Komunikaty[0].RodzajKomunikatu;
                                row.Cells["Status"].Value = -500;

                            }

                        }
                        else
                        {
                            row.Cells["informacja"].Value = "Błąd podczas próby księgowania,  brak odpowiedzi lub uznanie nierozliczone";
                            row.Cells["Status"].Value = -600;
                        }
                    }
                    catch (Exception ex)
                    {

                        row.Cells["status"].Value = -1000;
                        row.Cells["informacja"].Value = "Błąd działania usługi sieciowej " + ex.Message;
                    }


                }





            }



        }

        private string delPos(int idDok, decimal kwota, string opGl, string opCz, DateTime dKsieP, DateTime dDokP, string opis)
        {
            using (KnsMigratorEntities context = new KnsMigratorEntities())
            {

                Dokument dok = context.Dokument.Where(a => a.id == idDok).FirstOrDefault();
                if (dok == null) return "Brak żródowego dokumentu pzypisu";
                DateTime dtr = new DateTime(2017, 1, 1);
                Transfer tr = context.Transfer.Where(a => a.DataDo == dtr && a.rodzaj == 1).FirstOrDefault();
                if (tr == null)
                {
                    tr = new Transfer();
                    tr.rodzaj = 1;
                    tr.DataTransferu = DateTime.Today;
                    tr.DataDo = dtr;
                    tr.LFaktow = 0;
                    tr.status = 0;
                    tr.Uwagi = "Migracja Kosztów sądowych";
                    context.Transfer.AddObject(tr);
                }
                Dokument doku = new Dokument();
                doku.InsDate = DateTime.Now;
                doku.InsertedBy = UserInfo.MEPUser;
                //doku.KnsKsiegaDzNal = 

                /////
                try
                {
                    doku.kwota = kwota;
                    doku.typFakt = "KS";
                    doku.SAPDocId = "";
                    doku.Sprawa_Id = dok.Sprawa_Id;
                    doku.DataDokumentu = dDokP;
                    doku.DataKsiegowania = dKsieP;
                    doku.DataPlatnosci = dok.DataPlatnosci;
                    doku.Dluznik_Id = dok.Dluznik_Id;
                    doku.DocGuid = new Guid();
                    doku.DocGuid = Guid.NewGuid();
                    doku.grzSamoistna = "";
                    doku.OperacjaCzesciowa = (String.IsNullOrWhiteSpace(opCz) ? dok.OperacjaCzesciowa : opCz);
                    doku.OperacjaGlowna = opGl;
                    //  doku.Opis = opis;
                    doku.SAPImportDate = DateTime.Now;
                    doku.SAPImportStatus = 0;
                    doku.SAPRodzajDokumentu = "NS";
                    doku.SAPWaluta = "PLN";
                    doku.SentBy = UserInfo.MEPUser;
                    doku.typFakt = "KS";
                    doku.Stan = dok.Stan;



                    dok.typFakt = "XS";
                    List<Dokument> lst = context.Dokument.Where(a => a.Sprawa_Id == dok.Sprawa_Id && a.SAPDocIdRef == dok.SAPDocId && a.typFakt == "KO" && a.DataKsiegowania < dKsieP).ToList();
                    if (lst != null && lst.Any())
                    {
                        foreach (Dokument d in lst)
                        {
                            d.typFakt = "XO";

                        }

                    }

                    tr.Dokument.Add(doku);
                    context.SaveChanges();
                    return "";

                }
                catch (Exception ex)
                {
                    return "Błąd podczas zapisu dok. Przypisu " + ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "");

                }


            }

        }

        private void rmiOdpiszPrzypisz_Click(object sender, EventArgs e)
        {
            int rowno = 0;
            string err;
            decimal kwt;
            string opGL;
            string opCze;
            string kluczUzg;
            string opis;
            DateTime dKsie, dDok;
            bool czyDataDokP;
            string opGLP;
            string kluczUzgP;
            string opisP;
            string opCzP;
            DateTime dKsieP, dDokP;

            Cursor.Current = Cursors.WaitCursor;
            Konfiguracja knf = thecontext.Konfiguracja.FirstOrDefault();
            if (this.rgvMasowe.SelectedRows.Count <= 0) return;

            GetOdpisDetails wnd = new GetOdpisDetails();
            wnd.czyP = true;
            wnd.dDok = new DateTime(2016, 12, 31);
            wnd.dKsiegowania = new DateTime(2016, 12, 31);

            wnd.dKsiegowaniaP = new DateTime(2017, 1, 1);
            wnd.opGlP = "N011";

            if (wnd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            opGL = wnd.opGl;
            kluczUzg = wnd.kluczUzg;
            dKsie = wnd.dKsiegowania;
            dDok = wnd.dDok;
            opis = wnd.Opis;
            opCze = wnd.opCze;

            opGLP = wnd.opGlP;
            kluczUzgP = wnd.kluczUzgP;
            dKsieP = wnd.dKsiegowaniaP;
            dDokP = wnd.dDokP;
            opisP = wnd.OpisP;
            czyDataDokP = wnd.czyDataP;
            opCzP = wnd.opCzeP;

            tbAll.Text = this.rgvMasowe.SelectedRows.Count.ToString();
            tbAll.Refresh();

            foreach (GridViewRowInfo row in this.rgvMasowe.SelectedRows)
            {
                string DocPId;
                tbCurrent.Text = (++rowno).ToString();
                tbCurrent.Refresh();
                if ((row.Cells["status"].Value as int?) > 0)
                {
                    if (row.Cells["status"].Value as int? == 1) // jeśli są raty
                    {
                        // dezaktywacja rat
                        InstalmentPlanDeactivateResponse dezPlan;
                        string planRat = row.Cells["informacja"].Value.ToString();
                        planRat = planRat.Substring(planRat.IndexOf("dokumentu:") + "dokumentu:".Length + 1).Trim();
                        dezPlan = ZSRKRequestHelper.DzeaktywujPlanRat(planRat);

                    }

                    OdpisanieNaleznosciElement dok = new OdpisanieNaleznosciElement();

                    try
                    {
                        dok.CzesciowaOperacja = (String.IsNullOrWhiteSpace(opCze) ? row.Cells["OperacjaCzesciowa"].Value as string : opCze);
                        dok.DataDokumentu = dDok.ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
                        dok.DataKsiegowania = dKsie.ToString("yyyyMMdd"); //; Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
                        dok.DataPlatnosciNetto = dDok.ToString("yyyyMMdd");
                        dok.GlownaOperacja = opGL;
                        dok.JednostkaGospodarcza = knf.JednostkaGospodarcza;
                        dok.KluczUzgodnienia = kluczUzg;
                        dok.PrzyczynaBlokPlatnosci = "p";
                        dok.TekstWyjasniajacy = opis;
                        dok.KwotaNaleznosci = "-" + Convert.ToDecimal(row.Cells["SaldoSAP"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                        DocumentReductionDebtResponse ansdok = ZSRKRequestHelper.OdpiszNaleznosc(row.Cells["SAPDocId"].Value.ToString(), dok);
                        if (ansdok != null)
                        {
                            if (ansdok != null && ansdok.OdpisanieNaleznosciOdpowiedz != null && ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu != null && ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu.Trim().Length > 0)
                            {
                                row.Cells["informacja"].Value = "Numer dokumentu " + ansdok.OdpisanieNaleznosciOdpowiedz.NumerZaksiegowanegoDokumentu;
                                row.Cells["Status"].Value = 1000;

                                // jest OK - można zaksięgować przypis 

                                string retstr = delPos(Convert.ToInt32(row.Cells["Id"].Value), Convert.ToDecimal(row.Cells["SaldoSAP"].Value), opGLP, opCzP, dKsieP, czyDataDokP ? Convert.ToDateTime(row.Cells["DataDokumentu"].Value) : dDokP, opisP);
                                if (!String.IsNullOrWhiteSpace(retstr))
                                {

                                    row.Cells["informacja"].Value = retstr;
                                    row.Cells["Status"].Value = -800;
                                }
                                /////
                                /*
                                SapPIService.DodajNaleznoscOdpowiedz ansdokP = sapPI.DodajPrzypis(dokP);
                                if (ansdokP != null)
                                {
                                    if (ansdokP.Naleznosci != null)
                                    {
                                        if (ansdokP.Naleznosci.GetUpperBound(0) >= 0)
                                        {
                                            if (ansdokP.Komunikaty[0].RodzajKomunikatu == "E")
                                            {
                                                row.Cells["informacja"].Value = "Błąd podczas próby księgowania przypisu " + (ansdokP.Komunikaty != null && ansdokP.Komunikaty.GetUpperBound(0) >= 0 ? ansdokP.Komunikaty[0].Komunikat + " " + ansdokP.Komunikaty[0].NumerKomunikatu + " " + ansdokP.Komunikaty[0].RodzajKomunikatu: "" ) ;
                                                row.Cells["Status"].Value = -700;

                                            }
                                            else
                                            {
                                                DocPId = ansdokP.Naleznosci[0].NumerDokumentuRozrachunkow;
                                                if (!String.IsNullOrWhiteSpace(DocPId))
                                                {

                                                    ;
                                                
                                                }
                                            }
                                        }
                                        else
                                        {
                                            row.Cells["informacja"].Value += (ansdokP.Komunikaty != null && ansdokP.Komunikaty.GetUpperBound(0) >= 0 ? ansdokP.Komunikaty[0].Komunikat : "");
                                            row.Cells["Status"].Value = -800;
                                        }

                                    }
                                }
                                 */
                                /////
                            }
                            else
                            {
                                row.Cells["informacja"].Value = "Błąd podczas próby księgowania odpisu brak numeru dokumentu " + ansdok.Komunikaty[0].Komunikat1 + " " + ansdok.Komunikaty[0].NumerKomunikatu + " " + ansdok.Komunikaty[0].RodzajKomunikatu;
                                row.Cells["Status"].Value = -500;

                            }

                        }
                        else
                        {
                            row.Cells["informacja"].Value = "Błąd podczas próby księgowania,  brak odpowiedzi lub uznanie nierozliczone";
                            row.Cells["Status"].Value = -600;
                        }
                    }
                    catch (Exception ex)
                    {

                        row.Cells["status"].Value = -1000;
                        row.Cells["informacja"].Value = "Błąd działania usługi sieciowej " + ex.Message;
                    }


                }





            }



        }

        private void rb_BIG_Click(object sender, EventArgs e)
        {

            WinBIGMain wBig = new WinBIGMain();
            wBig.Show();
            RupBig.UserInfo.Id = UserInfo.Id;
            RupBig.UserInfo.Username = UserInfo.Username;
            RupBig.UserInfo.role = UserInfo.role;
            RupBig.UserInfo.logMode = true; //RunMode. logMode;

        }

        private void rgvKodyMask_Initialized(object sender, EventArgs e)
        {

            GridViewComboBoxColumn KodKraj = (GridViewComboBoxColumn)rgvKodyMask.Columns["Kraj"];
            KodKraj.DataSource = thecontext.SAPKodKraju.OrderBy(a => a.kraj).ToList();
            KodKraj.ValueMember = "kod";
            KodKraj.DisplayMember = "kraj";
            rgvKodyMask.DataSource = KodyPocztKonfig;
            KodyPocztKonfig.DataSource = thecontext.KodMaskKonfig.OrderBy(a => a.Kraj).ToList();


        }

        private void rgvKodyMask_UserAddedRow(object sender, GridViewRowEventArgs e)
        {
            //
            KodMaskKonfig k = (e.Row.DataBoundItem as KodMaskKonfig);
            if (k.Maska == null) k.Maska = "";
            thecontext.KodMaskKonfig.AddObject(k);
            thecontext.SaveChanges();

        }

        private void btPfx_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "pfx (*.pfx)|*.pfx";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    byte[] pfx = File.ReadAllBytes(openFileDialog.FileName);
                    if (pfx != null)
                    {
                        using (KnsMigratorEntities context = new KnsMigratorEntities())
                        {
                            Konfiguracja konf = context.Konfiguracja.FirstOrDefault();
                            konf.Pfx = pfx;
                            context.SaveChanges();

                        }

                    }
                }
            }
        }

        private void rbtCer_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "cer (*.cer)|*.cer";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    byte[] pfx = File.ReadAllBytes(openFileDialog.FileName);
                    if (pfx != null)
                    {
                        using (KnsMigratorEntities context = new KnsMigratorEntities())
                        {
                            Konfiguracja konf = context.Konfiguracja.FirstOrDefault();
                            konf.Cer = pfx;
                            context.SaveChanges();

                        }

                    }
                }
            }
        }

        private void rbtTestPfx_Click(object sender, EventArgs e)
        {
            try
            {
                using (KnsMigratorEntities context = new KnsMigratorEntities())
                {
                    Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                    var certificate = new X509Certificate2(knf.Pfx, Utils.Decrypt(knf.PfxPassword, GlobalStrings.APP_ERROR));


                }
            }
            catch (CryptographicException ex)
            {
                if ((ex.HResult & 0xFFFF) == 0x56)
                {
                    MessageBox.Show("Błędne hasło do certyfikatu *.pfx lub certyfikat nie został zaimportowany");
                    return;
                };


            }

            MessageBox.Show("Instalacja certyfikatu *.pfx poprawna");

        }

        private void rbtPfxPassword_Click(object sender, EventArgs e)
        {

            SetPfxPwd chdlg = new SetPfxPwd();
            chdlg.ShowDialog();


        }

        private void rlKontoMEP_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            ChngMEPPwd mepUser = new ChngMEPPwd();
            mepUser.ShowDialog();
        }

        private void rlKontoMEP_DoubleClick(object sender, EventArgs e)
        {
            ChngMEPPwd mepUser = new ChngMEPPwd();
            mepUser.ShowDialog();
        }

        private void rPVKonfig_SelectedPageChanged(object sender, EventArgs e)
        {
            if (this.rgvMethods.Rows.Count == 0)
                if ((sender as RadPageView).SelectedPage.Name == "rpvWS")
                {

                    using (KnsMigratorEntities context = new KnsMigratorEntities())
                    {
                        List<ServiceEndpoint> lst = context.ServiceEndpoint.ToList();
                        this.rgvMethods.DataSource = lst;

                    }

                }

        }



        private void rbSaveReplace_Click(object sender, EventArgs e)
        {
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

        private void rbReplace_Click(object sender, EventArgs e)
        {
            if (tbFrom.Text.Length == 0) return;
            List<ServiceEndpoint> lst = (List<ServiceEndpoint>)this.rgvMethods.DataSource;

            foreach (GridViewRowInfo row in this.rgvMethods.Rows)
            {
                ServiceEndpoint se = (ServiceEndpoint)row.DataBoundItem;
                se.Endpoint = se.Endpoint.Replace(tbFrom.Text, tbTo.Text);

            }

            this.rgvMethods.MasterTemplate.Refresh();


        }

        private void rgvDokumenty_Click(object sender, EventArgs e)
        {
            try
            {

                if ((sender as Telerik.WinControls.UI.RadGridView).CurrentCell.Value != null && (sender as Telerik.WinControls.UI.RadGridView).CurrentCell.Value.ToString() == "status")
                {
                    string diag = string.Empty;
                    string info = string.Empty;

                    diag = (sender as Telerik.WinControls.UI.RadGridView).CurrentRow.Cells["DIAGNOSTYKA"] != null && (sender as Telerik.WinControls.UI.RadGridView).CurrentRow.Cells["DIAGNOSTYKA"].Value != null ? (sender as Telerik.WinControls.UI.RadGridView).CurrentRow.Cells["DIAGNOSTYKA"].Value.ToString() : "";
                    info = (sender as Telerik.WinControls.UI.RadGridView).CurrentRow.Cells["Info"] != null && (sender as Telerik.WinControls.UI.RadGridView).CurrentRow.Cells["Info"].Value != null ? (sender as Telerik.WinControls.UI.RadGridView).CurrentRow.Cells["Info"].Value.ToString() : "";
                    ErrorInfo whwnd = new ErrorInfo();
                    whwnd.info = (diag + "\n\r" + info).Trim();
                    whwnd.ShowDialog();


                }
            }
            catch
            {
            }

        }

        private void rmiStanyNal_Click(object sender, EventArgs e)
        {
            TransferDialog tdl = new TransferDialog();
            tdl.dOd = DateTime.Today;
            tdl.dDo = DateTime.Today;
            tdl.Context = this.thecontext;
            tdl.TypTransfer = "Stany należności";

            tdl.ShowDialog();


            if (tdl.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                imp = new Imports();
                this.thecontext.SaveChanges();
                this.thecontext = null;
                this.thecontext = new KnsMigratorEntities();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.data_od = tdl.dOd;
                imp.theday = tdl.dDo;
                imp.uwagi = tdl.Uwagi;
                imp.newOnly = tdl.newOnly;
                imp.KsiegiKnsLst = tdl.KsiegiKnsLst;
                imp.typImport = 10;
                Konfiguracja knf = thecontext.Konfiguracja.FirstOrDefault();
                setSAPConnectionParams();
                //imp.ImportSaldo();
                Thread thImport = new Thread(imp.ImportStanyNal);
                thImport.Start();

                while (!imp.breakIndicator)
                {

                    Thread.Sleep(300);
                    this.rlProgress.Text = imp.progressMsg;
                    this.rlProgress.Refresh();

                }


            }
        }

        private void rbImportRep_Click(object sender, EventArgs e)
        {
            ImportRepertorium();
            this.rgvRepertorium.DataSource = thecontext.SAPRepertorium.ToList();

        }

        
        private void rbImportSadZsrk_Click(object sender, EventArgs e)
        {
            ImportSady();
            GridViewComboBoxColumn col = this.rgvKnsSady.Columns["SAPSad_Id"] as GridViewComboBoxColumn;
            col.DataSource = thecontext.SAPSad.OrderBy(a => a.miastSad).ToList();
            this.rgvKnsSady.DataSource = thecontext.KnsSad.OrderBy(a => a.Id).ToList();

        }

        private void rbSaveSlownik_Click(object sender, EventArgs e)
        {
            // zapis słowników.

        }

        private void rgvSadyFunkcjonalne_Initialized(object sender, EventArgs e)
        {
            this.SapSadyDataSource.DataSource = thecontext.SAPSad.Where(a => a.typSad == "SF").OrderBy(a => a.kod).ToList();
            this.rgvSadyFunkcjonalne.DataSource = this.SapSadyDataSource; //.Mains;

            GridViewComboBoxColumn SadJGColumn = new GridViewComboBoxColumn();
            SadJGColumn.Name = "JEGO";
            SadJGColumn.HeaderText = "Jedn. Gosp. dla Stanowiska Finansowego";
            SadJGColumn.IsVisible = true;
            SadJGColumn.DataSource = thecontext.SAPSad.Where(a => a.typSad == "SO").OrderBy(a => a.miastSad).ToList();
            SadJGColumn.ValueMember = "kod";
            SadJGColumn.DisplayMember = "miastSad";
            SadJGColumn.Width = 350;
            SadJGColumn.FieldName = "JEGO";
            SadJGColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
            this.rgvSadyFunkcjonalne.Columns.Add(SadJGColumn);

        }

        private void rbAddConsConnection_Click(object sender, EventArgs e)
        {
            ConfigDB conscfg = new ConfigDB();
            conscfg.ShowDialog();
            if (conscfg.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.ConsIntegrDataSource.DataSource = thecontext.ConsExternalDBConnectionConfig.ToList();
                this.rgvConsSystems.DataSource = this.ConsIntegrDataSource;
            }
        }

        private void rbEditConsConnection_Click(object sender, EventArgs e)
        {
            ConfigDB conscfg = new ConfigDB();
            GridViewRowInfo therow = rgvConsSystems.SelectedRows[0];
            if (therow == null) return;
            conscfg.Id = Convert.ToInt32(therow.Cells["Id"].Value);
            conscfg.ShowDialog();
            

            if (conscfg.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                using (KnsMigratorEntities context = new KnsMigratorEntities())
                {
                    this.ConsIntegrDataSource.DataSource = context.ConsExternalDBConnectionConfig.ToList();
                    this.rgvConsSystems.DataSource = this.ConsIntegrDataSource;
                }
                
            }
        }

        private void rbDelConsConnection_Click(object sender, EventArgs e)
        {
            if (rgvConsSystems.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Czy na pewno chcesz usunąć zaznaczone połączenie?", "Potwierdzenie", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (GridViewRowInfo row in rgvConsSystems.SelectedRows)
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    ConsExternalDBConnectionConfig cfg = thecontext.ConsExternalDBConnectionConfig.Where(a => a.id == id).FirstOrDefault();
                    if (cfg != null)
                    {
                        if (thecontext.ConsJobItem.Where(a=>a.consExternalDBConnectionConfig_Id == cfg.id).FirstOrDefault() == null)
                            thecontext.ConsExternalDBConnectionConfig.DeleteObject(cfg);
                        else
                        {
                            MessageBox.Show("Nie można usunąć konfiguracji połączenia bo w systemie znajdują się zadania importu z nią związane. Deaktywuj połączenie");
                            return;
                        }
                    }
                }
                thecontext.SaveChanges();
                this.ConsIntegrDataSource.DataSource = thecontext.ConsExternalDBConnectionConfig.ToList();
                this.rgvConsSystems.DataSource = this.ConsIntegrDataSource;
            }
        }

        private void rgvConsSystems_Click(object sender, EventArgs e)
        {

        }
    }



}


