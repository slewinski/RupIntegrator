using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.IO;
using Telerik.WinControls.UI.Localization;
using System.Data.SqlClient;
using System.Configuration;
using Telerik.WinControls;
using Telerik.WinControls.Export;
using Telerik.WinControls.Data;
using Ex2PscdInterface.Ex2PscdPaymentListQueryInService;

namespace RupLoader
{
    public partial class Recognizer : Form
    {

        PaymentService psrv;

        private int menuCallContext = 0; // 1 -  okno tytułem, 
        private Font m_underFont = new Font("Helvetica", 8, FontStyle.Underline);




        public Recognizer()
        {
            InitializeComponent();

            RadGridLocalizationProvider.CurrentProvider = new PolishRadGridLocalizationProvider();
            psrv = new PaymentService();
            setupResGrid();

            // setup 
            //this.TopMost = true;
        }



        private void setupResGrid()
        {
            Dictionary<string, string> typOsoby = new Dictionary<string, string>();
            typOsoby.Add(" ", "fizyczna");
            typOsoby.Add("X", "prawna");

            GridViewComboBoxColumn rsColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["RodzSprawy"];
            rsColumn.DataSource = RupDatabase.theContext.SAPRodzajSprawy.ToList();
            rsColumn.ValueMember = "kod";
            rsColumn.DisplayMember = "opis";
            rsColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn taColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["ltomow"];
            taColumn.DataSource = RupDatabase.theContext.SAPTomyAkt.ToList();
            taColumn.ValueMember = "Kod";
            taColumn.DisplayMember = "Opis";
            taColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn krajColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["dlkraj"];
            krajColumn.DataSource = RupDatabase.theContext.SAPKodKraju.OrderBy(a => a.kraj).ToList();
            krajColumn.ValueMember = "kod";
            krajColumn.DisplayMember = "kraj";
            krajColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn fpColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["dlFizPraw"];
            fpColumn.DataSource = typOsoby.ToList();
            fpColumn.ValueMember = "Key";
            fpColumn.DisplayMember = "Value";
            fpColumn.DataSourceNullValue = "";
            fpColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
            GridViewComboBoxColumn rpumoColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["RodzPUmo"];
            rpumoColumn.DataSource = RupDatabase.theContext.SAPOpisPrzedmiotu.ToList();
            rpumoColumn.ValueMember = "Symbol";
            rpumoColumn.DisplayMember = "Opis";
            rpumoColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
        }



        private void rgvWyciag_SelectionChanged(object sender, EventArgs e)
        {
            if ((sender as RadGridView).CurrentRow != null && (sender as RadGridView).CurrentRow.Index >= 0)
            {

                this.tbTytul.Text = (sender as RadGridView).CurrentRow.Cells["F5"].Value.ToString();

                this.tbZlec.Text = (sender as RadGridView).CurrentRow.Cells["F7"].Value.ToString();
                this.tbIBAN.Text = (sender as RadGridView).CurrentRow.Cells["F9"].Value.ToString();

                if ((sender as RadGridView).CurrentRow.Cells["F11"].Value != null)
                    this.tbkwt.Text = String.Format("{0:C}", (sender as RadGridView).CurrentRow.Cells["F11"].Value);
                if ((sender as RadGridView).CurrentRow.Cells["pozostalo"].Value != null)
                    this.tbpozo.Text = String.Format("{0:C}", (sender as RadGridView).CurrentRow.Cells["F11"].Value);
                string key = (sender as RadGridView).CurrentRow.Cells["F1"].Value.ToString();
                psrv.reloadResultGrid(key, rgvDokumenty);

            }
        }

        private void Recognizer_Load(object sender, EventArgs e)
        {
            string appDir;
            tbTytul.Text = "";
            appDir = Path.GetDirectoryName(Application.ExecutablePath);

            rgvPrint.Visible = false;
            if (RunMode.fileName.Substring(0, 1) == "/")
                psrv.AttachCmdDataSource(this.rgvWyciag);
            else
                psrv.AttachDataSource(this.rgvWyciag);

            if (File.Exists(appDir + "\\" + "wyciag.lyt"))
                this.rgvWyciag.LoadLayout(appDir + "\\" + "wyciag.lyt");
            else
                psrv.SetupGrid(this.rgvWyciag);

            psrv.InitPozostaloGrid(this.rgvWyciag);
            if (File.Exists(appDir + "\\" + "znajdz.lyt"))
                this.rgvSearch.LoadLayout(appDir + "\\" + "znajdz.lyt");
            // SetupExtra cols
            //
            string resname = "wynik.lyt";
            if (!String.IsNullOrWhiteSpace(RunMode.data))
                resname = RunMode.fileName.Replace("/", "") + "_wynik.lyt";
            if (File.Exists(appDir + "\\" + resname))
                this.rgvDokumenty.LoadLayout(appDir + "\\" + resname);

            rgvPrint.TableElement.RowHeight = 100;
            this.rgvDokumenty.AllowDeleteRow = true;
            this.rgvDokumenty.AllowEditRow = true;
            this.rgvDokumenty.AllowAddNewRow = false;
            this.btRecognize_Click(sender, e);
        }

        private void tbSaveLayout_Click(object sender, EventArgs e)
        {
            string appDir = Path.GetDirectoryName(Application.ExecutablePath);

            if (MessageBox.Show("Czy chcesz zapisać bieżący układ ?", "Zapis układu", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                string resname = "wynik.lyt";
                if (!String.IsNullOrWhiteSpace(RunMode.data))
                    resname = RunMode.fileName.Replace("/", "") + "_wynik.lyt";

                this.rgvWyciag.SaveLayout(appDir + "\\" + "wyciag.lyt");
                this.rgvDokumenty.SaveLayout(appDir + "\\" + resname); // <- zmiana
                this.rgvSearch.SaveLayout(appDir + "\\" + "znajdz.lyt");
            }
        }

        private void btRecognize_Click(object sender, EventArgs e)
        {

            this.psrv.ParseTytul(rgvWyciag);

        }

        private void Rozpoznaj_ContextMenuItem(object sender, EventArgs e)
        {

            string thekey;
            string IdList;
            List<string> lst = new List<string>();


            if (rgvWyciag.SelectedRows.Count <= 0) return;
            thekey = rgvWyciag.SelectedRows[0].Cells["result"].Value == null ? "" : rgvWyciag.SelectedRows[0].Cells["result"].Value.ToString();
            if (String.IsNullOrWhiteSpace(thekey)) return;
            lst = thekey.Split('|').ToList();
            IdList = "";
            if (lst.Count > 0) ;
            foreach (string item in lst)
            { if (!String.IsNullOrWhiteSpace(item))
                    IdList += (IdList.Length > 0 ? "," : "") + psrv.DoSearchEx(item, rgvSearch, IdList);
            }
        }




        private void rmiWritenSend_Click(object sender, EventArgs e)
        {

            psrv.writeAllContent(this.tbStep, this.tbStep);

        }

        private void rgvDokumenty_UserDeletingRow(object sender, GridViewRowCancelEventArgs e)
        {
            foreach (GridViewRowInfo row in e.Rows)
            {
                int key = (row.Cells["id"].Value != null ? Convert.ToInt32(row.Cells["id"].Value) : 0);
                if (key > 0)
                {
                    psrv.deleteDoc(key);

                }


            }
            decimal d = 0;
            foreach (GridViewRowInfo row in rgvDokumenty.Rows)
            {
                d += Convert.ToDecimal(row.Cells["kwota"].Value != null ? row.Cells["kwota"].Value : 0);
                if (this.rgvWyciag.CurrentRow != null && this.rgvWyciag.CurrentRow.Index >= 0)
                {

                    d = this.rgvWyciag.CurrentRow.Cells["F11"].Value != null ? Convert.ToDecimal(this.rgvWyciag.CurrentRow.Cells["F11"].Value) : 0 - d;
                    this.rgvWyciag.CurrentRow.Cells["pozostalo"].Value = d;
                    tbpozo.Text = String.Format("{0:C}", d);
                }
            }

        }



        private string getSelectedText()
        {
            if (!String.IsNullOrWhiteSpace(tbTytul.SelectedText)) return tbTytul.SelectedText;
            if (!String.IsNullOrWhiteSpace(tbZlec.SelectedText)) return tbZlec.SelectedText;
            if (!String.IsNullOrWhiteSpace(tbIBAN.SelectedText)) return tbIBAN.SelectedText;
            return null;
            /*
            switch (menuCallContext){ 
                case 1:
                    return tbTytul.SelectedText;
                case 2:
                    return tbZlec.SelectedText;
                case 3:
                    return tbIBAN.SelectedText;
                default: return "";
            
            
            }
            */


        }

        private void tsMenupoSygn_Click(object sender, EventArgs e)
        {

            string thekey;

            thekey = getSelectedText();
            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "S;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }

        private void tsMenuPoNazwie_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "N;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }

        private void rozpoznajPoKarcieDłToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "K;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }

        private void rozpoznajPoUlicyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "U;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }

        private void rozpoznajPoIBANToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "I;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }

        private void rozpoznajPoWPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "W;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }

        private void rozpoznajPoMiejscowościToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "M;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }



        private void tbZlec_Click(object sender, EventArgs e)
        {
            menuCallContext = 2;
        }

        private void tbIBAN_Click(object sender, EventArgs e)
        {
            menuCallContext = 3;
        }

        private void tbTytul_Click(object sender, EventArgs e)
        {
            menuCallContext = 1;
        }

        private void rmiWriteOnly_Click(object sender, EventArgs e)
        {

        }

        private void rmiWriteCurrent_Click(object sender, EventArgs e)
        {
            // zapisanie tylko bieżących 
            GridViewRowInfo therow = rgvWyciag.CurrentRow;
            if (therow != null && therow.Index >= 0)
            {

                string key = therow.Cells["F1"].Value != null ? therow.Cells["F1"].Value.ToString() : "";
                psrv.writePartContent(key, tbStep, tbItem);

            }
        }

        private void rgvDokumenty_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            // 
            if (e.Column.Name == "kwota")
            {
                decimal d = 0;
                foreach (GridViewRowInfo row in rgvDokumenty.Rows)
                {
                    d += Convert.ToDecimal(row.Cells["kwota"].Value != null ? row.Cells["kwota"].Value : 0);
                }
                if (this.rgvWyciag.CurrentRow != null && this.rgvWyciag.CurrentRow.Index >= 0)
                {

                    d = (this.rgvWyciag.CurrentRow.Cells["F11"].Value != null ? Convert.ToDecimal(this.rgvWyciag.CurrentRow.Cells["F11"].Value) : 0) - d;
                    this.rgvWyciag.CurrentRow.Cells["pozostalo"].Value = d;
                    tbpozo.Text = String.Format("{0:C}", d);
                }

                rgvWyciag.Refresh();
            }

        }

        private void rgvSearch_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (e.Column.Name == "sygnatura")
            {
                ExtraSearch exWin = new ExtraSearch();
                exWin.searchKey = "S;" + e.Row.Cells["sygnatura"].Value.ToString();

                exWin.ShowDialog();
                if (exWin.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    if (exWin.theRow != null)
                    {
                        string key = string.Empty;
                        if (rgvWyciag.CurrentRow != null && rgvWyciag.CurrentRow.Index >= 0)
                        {

                            key = rgvWyciag.CurrentRow.Cells["F1"].Value.ToString();
                        }
                            Cursor = Cursors.WaitCursor;
                            if (psrv.addResultRow(key, exWin.theRow, this.tbIBAN.Text, rgvWyciag.CurrentRow != null && rgvWyciag.CurrentRow.Index >= 0 &&  rgvWyciag.CurrentRow.Cells["pozostalo"].Value != null ? Convert.ToDecimal(rgvWyciag.CurrentRow.Cells["pozostalo"].Value) : 0, tbZlec.Text))
                            {
                                psrv.reloadResultGrid(key, rgvDokumenty);
                                this.tbpozo.Text = String.Format("{0:C}", 0);
                            if (rgvWyciag.CurrentRow != null && rgvWyciag.CurrentRow.Index >= 0)
                            {
                                rgvWyciag.CurrentRow.Cells["pozostalo"].Value = 0;
                            }

                            }
                            Cursor = Cursors.Default;
                        
                    }

                }
                // otwarcie okna 
                ;


            }
        }

        private void rgvSearch_CellFormatting(object sender, Telerik.WinControls.UI.CellFormattingEventArgs e)
        {
            if (e.Column.Name == "sygnatura")
            {

                e.CellElement.ForeColor = Color.DarkBlue;


            }
            else
            {
                e.CellElement.ResetValue(LightVisualElement.ForeColorProperty, ValueResetFlags.Local);
            }


        }

        private void rozpoznajPoOznPowodaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "Z;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }
        private void rozpoznajPoFragOznPowodaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "Y;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");

        }

        private void rozpoznajPoFragmencieNazwyczasochłonneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "F;" + thekey;
            psrv.DoSearchEx(thekey, rgvSearch, "");
        }



        private void rmiPrint_Click(object sender, EventArgs e)
        {
            psrv.PrepareToPrint(rgvPrint);
            RadPrintDocument document = new RadPrintDocument();
            document.DefaultPageSettings.Landscape = true;
            document.DefaultPageSettings.Margins.Top = 30;
            document.DefaultPageSettings.Margins.Left = 30;
            document.DefaultPageSettings.Margins.Right = 30;
            document.DefaultPageSettings.Margins.Bottom = 30;

            document.AssociatedObject = this.rgvPrint;
            document.Print();

        }

        private void rmiPreview_Click(object sender, EventArgs e)
        {
            psrv.PrepareToPrint(rgvPrint);
            RadPrintDocument document = new RadPrintDocument();
            document.DefaultPageSettings.Landscape = true;
            document.DefaultPageSettings.Margins.Top = 30;
            document.DefaultPageSettings.Margins.Left = 30;
            document.DefaultPageSettings.Margins.Right = 30;
            document.DefaultPageSettings.Margins.Bottom = 30;

            //document.AssociatedObject = this.rgvPrint;

            rgvPrint.PrintPreview(document);

        }

        private void tsmiKryteriumNazwa_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText().Trim();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "N;" + thekey;

            rgvWyciag.SelectedRows[0].Cells["result"].Value = (rgvWyciag.SelectedRows[0].Cells["result"].Value == null || String.IsNullOrWhiteSpace(rgvWyciag.SelectedRows[0].Cells["result"].Value.ToString())) ? thekey : rgvWyciag.SelectedRows[0].Cells["result"].Value.ToString() + "|" + thekey;

        }

        private void tsmiKryteriumFragment_Click(object sender, EventArgs e)
        {
            string thekey;

            thekey = getSelectedText().Trim();

            if (String.IsNullOrEmpty(thekey)) return;
            thekey = "F;" + thekey;
            rgvWyciag.SelectedRows[0].Cells["result"].Value = (rgvWyciag.SelectedRows[0].Cells["result"].Value == null || String.IsNullOrWhiteSpace(rgvWyciag.SelectedRows[0].Cells["result"].Value.ToString())) ? thekey : rgvWyciag.SelectedRows[0].Cells["result"].Value.ToString() + "|" + thekey;
        }

        private void ContextMenuItem_DanePodst_Click(object sender, EventArgs e)
        {

            Dokument dok = null;
            bool done = false;
            string s;
            if (rgvDokumenty.SelectedRows.Count <= 0) return;

            int key = (rgvDokumenty.SelectedRows[0].Cells["id"].Value != null ? Convert.ToInt32(rgvDokumenty.SelectedRows[0].Cells["id"].Value) : 0);
            dok = this.psrv.dokLst.Where(a => a.id == key).FirstOrDefault();
            if (dok != null)
            {

                ExportPI exp = new ExportPI();
                s = exp.DoExport(dok, 0, false, tbItem);
                if (s != null)
                {
                    dok.SAPImportStatus = 1;
                    done = true;
                }
                else
                { dok.SAPImportStatus = -1;

                }


                if (done)
                {
                    this.psrv.updateDocs();
                    this.psrv.dokLst.Remove(dok);
                }
                this.rgvDokumenty.Refresh();
            }
        }
        private void ContextMenuItem_DanePodstKsieguj_Click(object sender, EventArgs e)
        {
            Dokument dok = null;
            bool done = false;
            string s;
            if (rgvDokumenty.SelectedRows.Count <= 0) return;

            int key = (rgvDokumenty.SelectedRows[0].Cells["id"].Value != null ? Convert.ToInt32(rgvDokumenty.SelectedRows[0].Cells["id"].Value) : 0);
            dok = this.psrv.dokLst.Where(a => a.id == key).FirstOrDefault();
            if (dok != null)
            {

                ExportPI exp = new ExportPI();
                s = exp.DoExport(dok, 1, false, tbItem);
                if (s != null)
                {
                    dok.SAPImportStatus = 1;
                    done = true;
                }
                else
                {
                    dok.SAPImportStatus = -1;

                }


                if (done)
                {
                    this.psrv.updateDocs();
                    this.psrv.dokLst.Remove(dok);
                }
                this.rgvDokumenty.Refresh();
            }


        }

        private void ContextMenuItemPoczekalnia(object sender, EventArgs e)
        {
            Dokument dok = null;
            bool done = false;
            string s;
            if (rgvDokumenty.SelectedRows.Count <= 0) return;

            int key = (rgvDokumenty.SelectedRows[0].Cells["id"].Value != null ? Convert.ToInt32(rgvDokumenty.SelectedRows[0].Cells["id"].Value) : 0);
            dok = this.psrv.dokLst.Where(a => a.id == key).FirstOrDefault();
            if (dok != null)
            {

                ExportPI exp = new ExportPI();
                s = exp.ZapiszWPoczekalni(dok, 1, false, tbItem);
                if (s != null)
                {
                    dok.SAPImportStatus = 1;
                    done = true;
                }
                else
                {
                    dok.SAPImportStatus = -1;

                }


                if (done)
                {
                    this.psrv.updateDocs();
                    this.psrv.dokLst.Remove(dok);
                }
                this.rgvDokumenty.Refresh();
            }


        }

        private void ContextMenuItem_Diagnostics(object sender, EventArgs e)
        {
            Dokument dok = null;
            bool done = false;
            string s= string.Empty;
            if (rgvDokumenty.SelectedRows.Count <= 0) return;

            int key = (rgvDokumenty.SelectedRows[0].Cells["id"].Value != null ? Convert.ToInt32(rgvDokumenty.SelectedRows[0].Cells["id"].Value) : 0);
            dok = this.psrv.dokLst.Where(a => a.id == key).FirstOrDefault();
            if (dok != null)
            {
                s = dok.SAPImportInfo;
            }
            else

                try {

                    s = rgvDokumenty.SelectedRows[0].Cells["DIAGNOSTYKA"].Value.ToString();
                }
                catch { }

            ErrorInfoRL wnd = new ErrorInfoRL();
            wnd.info = s;
            wnd.ShowDialog();

        
    }


        private void ContextMenuItem_SygnaturaIPartner_Click(object sender, EventArgs e)
        {
            Dokument dok = null; 
            bool done = false;
            string s;
            if (rgvDokumenty.SelectedRows.Count <= 0 ) return;
            
            int key = (rgvDokumenty.SelectedRows[0].Cells["id"].Value != null ? Convert.ToInt32(rgvDokumenty.SelectedRows[0].Cells["id"].Value) : 0);
            dok = this.psrv.dokLst.Where(a => a.id == key).FirstOrDefault();
            if (dok != null)
            {
            
             ExportPI exp = new ExportPI();
                s = exp.DoExport(dok, 2, false, tbItem);
                if (s != null)
                {
                    dok.SAPImportStatus = 1;
                    done = true;
                }
                else
                { dok.SAPImportStatus = -1; 
                    
                }

            
            if (done)
            {
                this.psrv.updateDocs();
                this.psrv.dokLst.Remove(dok);
            }
            this.rgvDokumenty.Refresh();
            }
        }

        private void ContextMenuItem_Sygnatura_Click(object sender, EventArgs e)
        {
            Dokument dok = null;
            bool done = false;
            string s;
            string nrPartnera;
            string nrKontaUmowy;


            if (manyRowsMarked(rgvDokumenty))
            {

                foreach (GridViewRowInfo cRow in rgvDokumenty.ChildRows)
                {
                    done = false;
                    if (cRow.Cells["TAKNIE"] != null && cRow.Cells["TAKNIE"].Value != null)
                    { 
                        if ((bool)cRow.Cells["TAKNIE"].Value == true)
                        {
                            int myKey = (cRow.Cells["id"].Value != null ? Convert.ToInt32(cRow.Cells["id"].Value) : 0);
                            dok = this.psrv.dokLst.Where(a => a.id == myKey).FirstOrDefault();
                            if (dok != null)
                            {

                                ExportPI exp = new ExportPI();


                                nrPartnera = (cRow.Cells["SAPKontoPartnera"].Value != null ? cRow.Cells["SAPKontoPartnera"].Value.ToString() : "");
                                nrKontaUmowy = (cRow.Cells["SAPKontoUmowy"].Value != null ? cRow.Cells["SAPKontoUmowy"].Value.ToString() : "");
                                s = exp.DoExport(dok, 3, false, tbItem, string.IsNullOrWhiteSpace(nrPartnera) ? "" : nrPartnera, string.IsNullOrWhiteSpace(nrKontaUmowy) ? "" : nrKontaUmowy);
                                if (s != null)
                                {
                                    dok.SAPImportStatus = 1;
                                    done = true;
                                }
                                else
                                {
                                    dok.SAPImportStatus = -1;

                                }


                                if (done)
                                {
                                    this.psrv.updateDocs();
                                    this.psrv.dokLst.Remove(dok);
                                }

                            }

                        }
                }
                }
                this.rgvDokumenty.Refresh();
                return;
            }
                    if (rgvDokumenty.SelectedRows.Count <= 0) return;

            int key = (rgvDokumenty.SelectedRows[0].Cells["id"].Value != null ? Convert.ToInt32(rgvDokumenty.SelectedRows[0].Cells["id"].Value) : 0);
            dok = this.psrv.dokLst.Where(a => a.id == key).FirstOrDefault();
            if (dok != null)
            {

                ExportPI exp = new ExportPI();


                nrPartnera = (rgvDokumenty.SelectedRows[0].Cells["SAPKontoPartnera"].Value != null ? rgvDokumenty.SelectedRows[0].Cells["SAPKontoPartnera"].Value.ToString() : "");
                nrKontaUmowy = (rgvDokumenty.SelectedRows[0].Cells["SAPKontoUmowy"].Value != null ? rgvDokumenty.SelectedRows[0].Cells["SAPKontoUmowy"].Value.ToString() : "");
                s = exp.DoExport(dok, 3, false, tbItem, string.IsNullOrWhiteSpace(nrPartnera) ? "":nrPartnera, string.IsNullOrWhiteSpace(nrKontaUmowy)?"": nrKontaUmowy );
                if (s != null)
                {
                    dok.SAPImportStatus = 1;
                    done = true;
                }
                else
                {
                    dok.SAPImportStatus = -1;

                }


                if (done)
                {
                    this.psrv.updateDocs();
                    this.psrv.dokLst.Remove(dok);
                }
                this.rgvDokumenty.Refresh();
            }
        }


        

        private void rgvDokumenty_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            RadMenuItem customMenuItem = new RadMenuItem();
            RadMenuItem customMenuItem2 = new RadMenuItem();
            RadMenuItem customMenuItem3 = new RadMenuItem();
            RadMenuItem customMenuItem4 = new RadMenuItem();
            RadMenuItem customMenuItemClip = new RadMenuItem();
            RadMenuItem customMeuItemExportToExcel = new RadMenuItem();
            RadMenuItem customMeuItemExportAllNew= new RadMenuItem();
            RadMenuItem customMeuItemDiagnostics = new RadMenuItem();

            customMenuItem.Text = "Załóż dane podstawowe";
            customMenuItem2.Text = "Załóż dane podstawowe i zaksięguj";
            customMenuItem3.Text = "Załóż sygnaturę i partnera";
            customMenuItem4.Text = "Załóż sygnaturę";
            customMenuItemClip.Text = "Kopiuj zaznaczone do schowka";
            customMeuItemExportToExcel.Text = "Eksportuj do SAP";
            customMeuItemExportAllNew.Text = "Zarejestruj w \"poczekalni\" ZSRK";
            customMeuItemDiagnostics.Text = "Status operacji";

            RadMenuSeparatorItem separator = new RadMenuSeparatorItem();
            customMenuItem.Click += new EventHandler(ContextMenuItem_DanePodst_Click);
            customMenuItem2.Click += new EventHandler(ContextMenuItem_DanePodstKsieguj_Click);
            customMenuItem3.Click += new EventHandler(ContextMenuItem_SygnaturaIPartner_Click);
            customMenuItem4.Click += new EventHandler(ContextMenuItem_Sygnatura_Click);
            customMenuItemClip.Click += new EventHandler(ContextMenuItem_CopyAll);
            customMeuItemExportToExcel.Click += new EventHandler(saveToExcel);

            customMeuItemExportAllNew.Click += new EventHandler(ContextMenuItemPoczekalnia);
            customMeuItemDiagnostics.Click += new EventHandler(ContextMenuItem_Diagnostics);
            
            e.ContextMenu.Items.Add(separator);
            e.ContextMenu.Items.Add(customMenuItem);
            e.ContextMenu.Items.Add(customMenuItem2);
            e.ContextMenu.Items.Add(customMenuItem3);
            e.ContextMenu.Items.Add(customMenuItem4);
            RadMenuSeparatorItem separator1 = new RadMenuSeparatorItem();
            e.ContextMenu.Items.Add(separator1);
            e.ContextMenu.Items.Add(customMenuItemClip);
            e.ContextMenu.Items.Add(customMeuItemExportToExcel);
            e.ContextMenu.Items.Add(customMeuItemExportAllNew);
            e.ContextMenu.Items.Add(new RadMenuSeparatorItem());
            e.ContextMenu.Items.Add(customMeuItemDiagnostics);


        }

        private bool manyRowsMarked(RadGridView theGrid)
        {
           
            try
            {
                foreach (GridViewRowInfo therow in theGrid.ChildRows)
                {
                    if ( therow.Cells["TAKNIE"] != null && therow.Cells["TAKNIE"].Value != null)

                    if ((bool)therow.Cells["TAKNIE"].Value == true)
                        return true;
                }
                return false;
            }
            catch (Exception) { }
            return false;
        }
    

    private void rgvSearch_CommandCellClick(object sender, EventArgs e)
         {
            if (manyRowsMarked(rgvSearch)) // jeśłi zaznaczono  wiersze tzn zasilanie jest z innego miejsca
            {
                Cursor.Current = Cursors.WaitCursor;
                string key = Guid.NewGuid().ToString();
                foreach (GridViewRowInfo cRow in rgvSearch.ChildRows)
                {
                    if (cRow.Cells["TAKNIE"] != null && cRow.Cells["TAKNIE"].Value != null)
                        if ((bool)cRow.Cells["TAKNIE"].Value == true)
                    {
                        
                        
                        if (psrv.addResultRow(key, cRow, this.tbIBAN.Text,  0, tbZlec.Text))
                        {

                            ;
                        }


                    }

                }
                psrv.reloadResultGrid(key, rgvDokumenty);
                Cursor.Current = Cursors.Default;
                return;
            }


            if (RunMode.fileName.Substring(0,1) != "/")  // jeśli nie /ZDB
            if (rgvWyciag.CurrentRow != null && rgvWyciag.CurrentRow.Cells["pozostalo"].Value != null && Convert.ToDecimal(rgvWyciag.CurrentRow.Cells["pozostalo"].Value) == 0)
            {
                MessageBox.Show("Cała kwota zostala już rozdysponowana");
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            if (rgvSearch.CurrentRow != null && rgvSearch.CurrentRow.Index >= 0)
            {
                if (rgvWyciag.CurrentRow != null && rgvWyciag.CurrentRow.Index >= 0)
                {
                    string key = rgvWyciag.CurrentRow.Cells["F1"].Value.ToString();
                    GridViewRowInfo therow = rgvSearch.CurrentRow;
                    if (psrv.addResultRow(key, therow, this.tbIBAN.Text, rgvWyciag.CurrentRow.Cells["pozostalo"].Value != null ? Convert.ToDecimal(rgvWyciag.CurrentRow.Cells["pozostalo"].Value) : 0, tbZlec.Text))
                    {
                        psrv.reloadResultGrid(key, rgvDokumenty);

                        this.tbpozo.Text = String.Format("{0:C}", 0);
                        rgvWyciag.CurrentRow.Cells["pozostalo"].Value = 0;

                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void rgvDokumenty_CommandCellClick(object sender, EventArgs e)
        {
            string value = "";


            if (manyRowsMarked(rgvDokumenty)) // jeśłi zaznaczono  wiersze tzn zasilanie jest z innego miejsca
            {
                Cursor.Current = Cursors.WaitCursor;
                string key = Guid.NewGuid().ToString();
                foreach (GridViewRowInfo therow in rgvDokumenty.ChildRows)
                {
                    if (therow.Cells["TAKNIE"] != null && therow.Cells["TAKNIE"].Value != null)
                    {
                        if ((bool)therow.Cells["TAKNIE"].Value == true)
                        {
                            if (value.Length > 0) value += '\n';
                            foreach (GridViewCellInfo cell in therow.Cells)
                            {
                                if (cell.ColumnInfo.Name == "Separator") break;
                                if (cell.ColumnInfo.Name == "Wybierz" || cell.ColumnInfo.Name == "TAKNIE") continue;
                                if (!cell.ColumnInfo.IsVisible) continue;
                                if (!String.IsNullOrEmpty(value)) value += '\t';
                                value += cell.Value;

                            }
                           
                        }
                    }
                }
                Clipboard.SetDataObject(value, true);
                if (RunMode.WinMode == "/MIN")
                {
                    this.WindowState = FormWindowState.Minimized;

                }
                else
                    Application.Exit();
            }
            if (rgvDokumenty.CurrentRow != null)
            {
                GridViewRowInfo therow;
                therow = rgvDokumenty.CurrentRow;
                foreach (GridViewCellInfo cell in therow.Cells)
                {
                    if (cell.ColumnInfo.Name == "Separator") break;
                    if (cell.ColumnInfo.Name == "Wybierz" || cell.ColumnInfo.Name == "TAKNIE") continue;
                    if (!cell.ColumnInfo.IsVisible) continue;
                    if (!String.IsNullOrEmpty(value)) value += '\t';
                    value += cell.Value;

                }
                Clipboard.SetDataObject(value, true);
                if (RunMode.WinMode == "/MIN")
                {
                    this.WindowState = FormWindowState.Minimized;

                }
                else
                    Application.Exit();
            }
        }


        private void ContextMenuItem_CopyAll(object sender, EventArgs e)
        { // kopiowanie wszystkich zaznaczonych do schowka
            string value = String.Empty;

            Cursor.Current = Cursors.WaitCursor;
                string key = Guid.NewGuid().ToString();
                foreach (GridViewRowInfo therow in rgvDokumenty.ChildRows)
                {
                    if (therow.Cells["TAKNIE"] != null && therow.Cells["TAKNIE"].Value != null)
                    {
                        if ((bool)therow.Cells["TAKNIE"].Value == true)
                        {
                            if (value.Length > 0) value += '\n';
                            bool pierwszaKolumna = true;

                            foreach (GridViewCellInfo cell in therow.Cells)
                            {
                                if (cell.ColumnInfo.Name == "Separator") break;
                                if (cell.ColumnInfo.Name == "Wybierz" || cell.ColumnInfo.Name == "TAKNIE") continue;
                                if (!cell.ColumnInfo.IsVisible) continue;
                                //#PA poprawienie przesunięcia w kolejnych wierszach     if (!String.IsNullOrEmpty(value) && !pierwszaKolumna)
                                if (!pierwszaKolumna)
                                {
                                    value += '\t';
                                    
                                }
                                value += cell.Value;
                                pierwszaKolumna = false;
                            }
                        //xx yy zz
                        //   xx yy zz
                        //   xx yy zz
                        }
                    }
                }
            Clipboard.SetDataObject(value, true);
        }
        private void saveToExcel(object sender, EventArgs e)
        {
            RadSaveFileDialog saveFileDialog = new RadSaveFileDialog();
            saveFileDialog.DefaultExt = "xlsx";
            DialogResult dr = saveFileDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = saveFileDialog.FileName;
            } else
            {
                return;
            }

            RadGridView rgv = new RadGridView();

            rgv.Columns.Add("Opis");
            rgv.Columns.Add("SAPKontoPartnera");
            rgv.Columns.Add("SAPKontoUmowy");
            rgv.Columns.Add("SAPPrzedmiotUmowy");

            foreach (GridViewRowInfo therow in rgvDokumenty.ChildRows)
            {
                if (therow.Cells["TAKNIE"] != null && therow.Cells["TAKNIE"].Value != null)
                {
                    if ((bool)therow.Cells["TAKNIE"].Value == true)
                    {
                        rgv.Rows.Add(new object[]{ therow.Cells["Opis"].Value, therow.Cells["SAPKontoPartnera"].Value, therow.Cells["SAPKontoUmowy"].Value, therow.Cells["SAPPrzedmiotUmowy"].Value });
                    }
                }
            }

            string exportFile = saveFileDialog.FileName;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                Telerik.WinControls.Export.GridViewSpreadExport exporter = new Telerik.WinControls.Export.GridViewSpreadExport(rgv);
                Telerik.WinControls.Export.SpreadExportRenderer renderer = new Telerik.WinControls.Export.SpreadExportRenderer();
                exporter.HiddenColumnOption = Telerik.WinControls.UI.Export.HiddenOption.DoNotExport;
                exporter.ExportFormat = SpreadExportFormat.Xlsx;
                exporter.RunExport(ms, renderer);
                

                using (System.IO.FileStream fileStream = new System.IO.FileStream(exportFile, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(fileStream);
                }
            }
        }
        


        private void rgvDokumenty_Click(object sender, EventArgs e)
        {

        }

        private void btCleanAll_Click(object sender, EventArgs e)
        {
           
           
        
            RunMode.data = tbTytul.Text;
            DataTable dt = rgvWyciag.DataSource as DataTable;
            dt.Clear();
            psrv.SetWyciagData(rgvWyciag, dt);
            this.btRecognize_Click(sender, e);
            //rgvDokumenty.DataSource = null;
            
            rgvSearch.DataSource = null;
            if (rgvDokumenty.Rows.Count > 0)
                rgvDokumenty.Rows.Remove(rgvDokumenty.Rows[0]);
          

            
        }

        private void rgvWyciag_CommandCellClick(object sender, GridViewCellEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.Rozpoznaj_ContextMenuItem(sender, e);
            Cursor.Current = Cursors.Default;
        }

        private void partnerzyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SlowPartner sp = new SlowPartner();
            sp.ShowDialog();
        }

        private void rbWyciagSAP_Click(object sender, EventArgs e)
        {
            PozWyciagFilter wnd = new PozWyciagFilter();
            PaymentListQueryResponse resp = null;
            if (wnd.ShowDialog() == DialogResult.OK)
            {
                ExportPI exp = new ExportPI();
#if DEBUG
                //resp = new PaymentListQueryResponse();
                //List<PozycjaWB> lst = new List<PozycjaWB>();
                //PozycjaWB item = new PozycjaWB();
                //item.JednostkaGospodarcza = "4205";
                //item.Kwota = "123,34";
                //item.KwotaPrzypisana = "12.34";
                //item.PartiaPlatnosciID = "50002200";
                //item.TekstPlatnosci = "Wpłata do sprawy Nc 12/18";
                //item.PartiaPlatnosciNrPozycja = "1";
                //item.Zleceniodawca = "Kredyt Inkaso sp z o.o.";
                //item.RachBankZleceniodawca = new RachBankZleceniodawca();
                //item.RachBankZleceniodawca.KodKontrolny = "12";
                //item.RachBankZleceniodawca.KontoBankowe = "2323232 3232323232232";
                //lst.Add(item);
                //resp.PozycjaWB = lst.ToArray();
                resp = exp.PobierzPozycjePlatnosci(wnd.RodzajRachunkuBankowego, wnd.StatusRozliczenia, wnd.TypPozycji, wnd.dOd, wnd.dDo);


#else
            resp = exp.PobierzPozycjePlatnosci(wnd.RodzajRachunkuBankowego, wnd.StatusRozliczenia, wnd.TypPozycji, wnd.dOd, wnd.dDo);
                ;
#endif


                if (resp != null)
                {
                    DataTable table = psrv.Wyciag2Datatble(rgvWyciag.DataSource as DataTable, resp);
                    rgvWyciag.DataSource = table;


                }

            }


        }



        /*
                private void rgvWyciag_CommandCellClick(object sender, EventArgs e)
                {

                }

                */






    }
}
