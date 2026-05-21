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
using ExcelDataReader;
using System.Globalization;

namespace RupLoader
{
    public partial class ZDOBAnalizer : Form
    {

        PaymentService psrv;

        private int menuCallContext = 0; // 1 -  okno tytułem, 
        private Font m_underFont = new Font("Helvetica", 8, FontStyle.Underline);




        public ZDOBAnalizer()
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

        private void ZDOBAnalizer_Load(object sender, EventArgs e)
        {


            rgvPrint.TableElement.RowHeight = 100;
            this.rgvDokumenty.AllowDeleteRow = true;
            this.rgvDokumenty.AllowEditRow = true;
            this.rgvDokumenty.AllowAddNewRow = false;
            //  this.btRecognize_Click( sender,  e);
        }

        private void tbSaveLayout_Click(object sender, EventArgs e)
        {
            string appDir = Path.GetDirectoryName(Application.ExecutablePath);

            if (MessageBox.Show("Czy chcesz zapisać bieżący układ ?", "Zapis układu", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                /*
                string resname = "wynik.lyt";
                if (!String.IsNullOrWhiteSpace(RunMode.data))
                   resname =  RunMode.fileName.Replace("/", "") + "_wynik.lyt";
                */
                this.rgvWyciag.SaveLayout(appDir + "\\" + "wyciagMasowo.lyt");
                //this.rgvDokumenty.SaveLayout(appDir + "\\" + resname); // <- zmiana
                this.rgvSearch.SaveLayout(appDir + "\\" + "znajdzMasowo.lyt");
            }
        }

        private void btRecognize_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            this.psrv.ParseTytul(rgvWyciag);
            Cursor.Current = Cursors.Default; ;
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
                        if (rgvWyciag.CurrentRow != null && rgvWyciag.CurrentRow.Index >= 0)
                        {
                            Cursor = Cursors.WaitCursor;
                            string key = rgvWyciag.CurrentRow.Cells["F1"].Value.ToString();
                            if (psrv.addResultRow(key, exWin.theRow, this.tbIBAN.Text, rgvWyciag.CurrentRow.Cells["pozostalo"].Value != null ? Convert.ToDecimal(rgvWyciag.CurrentRow.Cells["pozostalo"].Value) : 0, tbZlec.Text))
                            {
                                psrv.reloadResultGrid(key, rgvDokumenty);
                                this.tbpozo.Text = String.Format("{0:C}", 0);
                                rgvWyciag.CurrentRow.Cells["pozostalo"].Value = 0;

                            }
                            Cursor = Cursors.Default;
                        }
                    }

                }
                // otwarcie okna 
                ;


            }
        }

        private void rgvSearch_CellFormatting(object sender, CellFormattingEventArgs e)
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

        private void ContextMenuItem_SygnaturaIPartner_Click(object sender, EventArgs e)
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

            customMenuItem.Text = "Załóż dane podstawowe";
            customMenuItem2.Text = "Załóż dane podstawowe i zaksięguj";
            customMenuItem3.Text = "Załóż sygnaturę i partnera";
            customMenuItem4.Text = "Załóż sygnaturę";
            customMenuItemClip.Text = "Kopiuj zaznaczone do schowka";
            RadMenuSeparatorItem separator = new RadMenuSeparatorItem();
            customMenuItem.Click += new EventHandler(ContextMenuItem_DanePodst_Click);
            customMenuItem2.Click += new EventHandler(ContextMenuItem_DanePodstKsieguj_Click);
            customMenuItem3.Click += new EventHandler(ContextMenuItem_SygnaturaIPartner_Click);
            customMenuItem4.Click += new EventHandler(ContextMenuItem_Sygnatura_Click);
            customMenuItemClip.Click += new EventHandler(ContextMenuItem_CopyAll);
            e.ContextMenu.Items.Add(separator);
            e.ContextMenu.Items.Add(customMenuItem);
            e.ContextMenu.Items.Add(customMenuItem2);
            e.ContextMenu.Items.Add(customMenuItem3);
            e.ContextMenu.Items.Add(customMenuItem4);
            RadMenuSeparatorItem separator1 = new RadMenuSeparatorItem();
            e.ContextMenu.Items.Add(separator1);
            e.ContextMenu.Items.Add(customMenuItemClip);

        }

        private bool manyRowsMarked(RadGridView theGrid)
        {

            try
            {
                foreach (GridViewRowInfo therow in theGrid.ChildRows)
                {
                    if (therow.Cells["TAKNIE"] != null && therow.Cells["TAKNIE"].Value != null)

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


                            if (psrv.addResultRow(key, cRow, this.tbIBAN.Text, 0, tbZlec.Text))
                            {

                                ;
                            }


                        }

                }
                psrv.reloadResultGrid(key, rgvDokumenty);
                Cursor.Current = Cursors.Default;
                return;
            }


            if (RunMode.fileName.Substring(0, 1) != "/")  // jeśli nie /ZDB
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
        private void transfile(Stream binXlSX)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(binXlSX);
            DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });
            
            DataTable worksheet = result.Tables[0];
            //Zapisać tabelę w bazie <TODO>
            this.BulkCopyTable("ZDOB_EXPORT", worksheet, new string[]{ "pozycja", "kwota_platnosci", "zleceniodawca", "tekst", "partia", "numer_dokumentu", "kraj_banku", "kod_banku", "konto_bankowe", "kod_kontrolny_banku" });
            if (worksheet.Rows.Count > 0)
            {

                this.psrv.AttachCmdDataSource(rgvWyciag, worksheet, true);
                this.psrv.SetupGrid(rgvWyciag, true);
                this.setupInitialGrid();

            }
        }
        private void BulkCopyTable (string destinationTableName, DataTable tableToCopy, string[] columnNames)
        {
            DataTable tempTable = tableToCopy.Copy();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RupLoader.Properties.Settings.RupDB"].ToString()))
            {
                SqlBulkCopy copyMachine = new SqlBulkCopy(connection);
                copyMachine.DestinationTableName = destinationTableName;
                foreach (DataColumn c in tempTable.Columns)
                {
                    c.ColumnName = this.StripText(c.ColumnName);
                    
                    if (columnNames.Contains(c.ColumnName))
                    {
                        copyMachine.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                    }
                }
                SqlCommand cmd = new SqlCommand(string.Format("TRUNCATE TABLE {0}", destinationTableName), connection);
                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    copyMachine.WriteToServer(tempTable);
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// Metoda podmienia polskie znaki w łańcuchu na odpowiedniki oraz spację na _
        /// </summary>
        /// <param name="text">Tekst do podmiany</param>
        /// <returns>Tekst bez polskich znaków i spacji</returns>
        private string StripText(string text)
        {
            string input = text.Trim().ToLower();
            string tmp = input.Replace("ą", "a");
            tmp = tmp.Replace("ć", "c");
            tmp = tmp.Replace("ę", "e");
            tmp = tmp.Replace("ł", "l");
            tmp = tmp.Replace("ń", "n");
            tmp = tmp.Replace("ó", "o");
            tmp = tmp.Replace("ś", "s");
            tmp = tmp.Replace("ź", "z");
            tmp = tmp.Replace("ż", "z");
            string output = tmp.Replace(" ", "_");
            return output;
        }
        private void btGetFilename_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Zbiory XLSX (*.xlsx)|*.xlsx|Wszystkie zbiory (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {

                this.tbFilename.Text = dlg.FileName;

                try
                {
                    using (Stream stream = dlg.OpenFile())
                    {
                        transfile(stream);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd importu");
                    return;

                }
            }
        }


        private void setupInitialGrid()
        {
            string appDir;
            tbTytul.Text = "";
            appDir = Path.GetDirectoryName(Application.ExecutablePath);
            rgvPrint.Visible = false;

            //psrv.AttachDataSource(this.rgvWyciag);

            if (File.Exists(appDir + "\\" + "wyciagMasowo.lyt"))
                this.rgvWyciag.LoadLayout(appDir + "\\" + "wyciagMasowo.lyt");
            //else
            //    psrv.SetupGrid(this.rgvWyciag);

            // psrv.InitPozostaloGrid(this.rgvWyciag);
            if (File.Exists(appDir + "\\" + "znajdzMasowo.lyt"))
                this.rgvSearch.LoadLayout(appDir + "\\" + "znajdzMasowo.lyt");
            // SetupExtra cols
            //
            /*
            string resname = "wynik.lyt";
            if (!String.IsNullOrWhiteSpace(RunMode.data))
                resname = RunMode.fileName.Replace("/", "") + "_wynik.lyt";
            if (File.Exists(appDir + "\\" + resname))
                this.rgvDokumenty.LoadLayout(appDir + "\\" + resname);
            */
        }

        private void rbSzukaj_Click(object sender, EventArgs e)
        {
            string thekey;
            string IdList;
            int ranking;
            List<string> lst = new List<string>();
            if (rgvWyciag.SelectedRows.Count <= 0) return;
            psrv.ClearDataSource();
            foreach (GridViewRowInfo therow in rgvWyciag.Rows)
            {
                thekey = therow.Cells["result"].Value == null ? "" : therow.Cells["result"].Value.ToString();
                ranking = int.Parse(therow.Cells["Ranking"] == null || therow.Cells["Ranking"].Value == null || String.IsNullOrWhiteSpace(therow.Cells["Ranking"].Value.ToString()) ? "0" : therow.Cells["Ranking"].Value.ToString());
                if (String.IsNullOrWhiteSpace(thekey)) continue;
                lst = thekey.Split('|').ToList();
                IdList = "";
                foreach (string item in lst)
                {
                    if (!String.IsNullOrWhiteSpace(item))
                        IdList += (IdList.Length > 0 ? "," : "") + psrv.DoSearchEx(item, rgvSearch, IdList, true, item.StartsWith("D") ? 1000 : (uint)ranking);
                }
                psrv.AttachTableAsDatatSource(rgvSearch);
            }

        }

        private void rgvSearch_CommandCellClick(object sender, GridViewCellEventArgs e)
        {

        }

        private Dokument setupDok(GridViewRowInfo theRow)
        {
            Dokument dok = new Dokument();

            Sprawa spr = new Sprawa();
            string typSad = Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 4000 ? "SR" : (Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) >= 3000 ? "SO" : "SA");
            string typSadOryg = typSad;
            if (!String.IsNullOrWhiteSpace(RupDatabase.theConfig.StanowiskoFin) && Convert.ToInt32(RupDatabase.theConfig.JednostkaGospodarcza) < 4000 && typSad !="SA")
            {
                typSad = "SF";
                typSadOryg = "SR";
            }
            spr.KnsSprawa_id = Convert.ToInt32(theRow.Cells["IdSprawy"].Value);
            spr.KnsKsiega = Convert.ToInt32(theRow.Cells["Ksiega"] == null ? 0 : theRow.Cells["Ksiega"].Value);
            spr.KNSSadOrzek_id = null;
            spr.Karta = theRow.Cells["OznKontaUmowy"].Value.ToString().Trim();  // karta dłużnika
            spr.SAPKontoUmowy = theRow.Cells["KontoUmowy"].Value.ToString();
            spr.SAPPrzedmiotUmowy = theRow.Cells["PrzedmiotUmowy"].Value.ToString();

            if (theRow.Cells["TypKontaUmowy"] != null && !String.IsNullOrEmpty(theRow.Cells["TypKontaUmowy"].Value.ToString()))
            {
                spr.SAPTypKontaUmowy = theRow.Cells["TypKontaUmowy"].Value.ToString();
            }
            else
            {

                spr.SAPTypKontaUmowy = "DO";
            }



            spr.SAPWydział = theRow.Cells["kodWydzial"].Value.ToString().Trim();
            spr.SAPRepertorium = theRow.Cells["repertorium"].Value.ToString().Trim().ToUpper();
            spr.Rok = Convert.ToInt32(theRow.Cells["rok"].Value);
            spr.Numer = Convert.ToInt32(theRow.Cells["nr"].Value);
            spr.SAPSadId = !String.IsNullOrEmpty(RupDatabase.theConfig.StanowiskoFin.DoTrim()) ? RupDatabase.theConfig.StanowiskoFin : RupDatabase.theConfig.JednostkaGospodarcza;

            // sprawdzamy czy mamy już taką sprawę
            {
                List<Sprawa> sprxL;
                sprxL = RupDatabase.theContext.Sprawa.Include("Dluznik").Where(a => a.SAPSadId == spr.SAPSadId && a.SAPWydział == spr.SAPWydział && a.Rok == spr.Rok && a.Numer == spr.Numer && a.SAPRepertorium == spr.SAPRepertorium &&
                                                                a.SAPPrzedmiotUmowy != null && a.SAPTypKontaUmowy == spr.SAPTypKontaUmowy).OrderByDescending(a => a.Id).ToList();
                if (sprxL != null && sprxL.Count > 0)
                    return null;
            }
            if (theRow.Cells["RelacjaKonta"] != null && !String.IsNullOrEmpty(theRow.Cells["RelacjaKonta"].Value.ToString()))
                spr.SAPRelacjaKontaUmowy = theRow.Cells["RelacjaKonta"].Value.ToString().Trim();
            else
                switch (theRow.Cells["rola"].Value.ToString().ToUpper())
                {
                    case "POWÓD":
                    case "WNIOSKODAWCA":
                        spr.SAPRelacjaKontaUmowy = "01";
                        break;
                    case "OSKARŻONY":
                    case "UCZESTNIK":
                        spr.SAPRelacjaKontaUmowy = "02";
                        break;
                    case "POZWANY":
                        spr.SAPRelacjaKontaUmowy = "03";
                        break;
                    case "ŚWIADEK":
                        spr.SAPRelacjaKontaUmowy = "04";
                        break;

                    default:
                        spr.SAPRelacjaKontaUmowy = "99";
                        break;

                }

         

            if (spr.SAPRepertorium.Length > 0)
            {
                SAPRodzajSprawy rodzajSpr = (from f in RupDatabase.theContext.SAPRodzajSprawy where f.repertorium == spr.SAPRepertorium && f.typSad == typSadOryg orderby f.id select f).FirstOrDefault();
                if (rodzajSpr != null)
                {
                    spr.SAPRodzajSprawy = rodzajSpr.kod;

                }
            }

            spr.SAPTomyAkt = "001";
            dok.SAPImportStatus = 0;
            dok.DocGuid = Guid.NewGuid();
            dok.KnsPozDzNal = 0;
            dok.kwota = (theRow.Cells["kwota"].Value == DBNull.Value ? 0 : Convert.ToDecimal(theRow.Cells["kwota"].Value.ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL")));
            try
            {
                dok.Opis = theRow.Cells["Opis"].Value.ToString();
            }
            catch (Exception ex1)
            {

                ;

            }
            try
            {
                dok.uwagi = theRow.Cells["uwagi"].Value.ToString();
            }
            catch (Exception ex1)
            {

                ;

            }
          

     
            if (dok != null)
            {

                if (theRow.Cells["ZrodloDanych"].Value.ToString() == "KNS")
                    dok.SAPDocIdRef = theRow.Cells["NrDokumentu"].Value.ToString();
                else
                    dok.SAPDocId = theRow.Cells["NrDokumentu"].Value.ToString();


                spr.Dokument.Add(dok);
                
            }
            // return null;
           



            return dok;

        }

        

        private void radMenuItem1_Click(object sender, EventArgs e)
        {
            Dokument dok = null;
            bool done = false;
            string s;
            string nrPartnera;
            string nrKontaUmowy;

            if (rgvSearch.RowCount > 0)
            {
                foreach (GridViewRowInfo cRow in rgvSearch.Rows)
                {

                    done = false;
                    nrPartnera = null;
                    nrKontaUmowy = null;
                    if (cRow.Cells["TAKNIE"] != null && cRow.Cells["TAKNIE"].Value != null)
                    {
                        if ((bool)cRow.Cells["TAKNIE"].Value == true)
                        {
                            //int myKey = (cRow.Cells["id"].Value != null ? Convert.ToInt32(cRow.Cells["id"].Value) : 0);
                            dok = this.setupDok(cRow);
                            if (dok != null)
                            {

                                ExportPI exp = new ExportPI();


                              //  nrPartnera = (cRow.Cells["NumerPartnera"].Value != null ? cRow.Cells["NumerPartnera"].Value.ToString() : "");
                              //  nrKontaUmowy = (cRow.Cells["KontoUmowy"].Value != null ? cRow.Cells["KontoUmowy"].Value.ToString() : "");
                                s = exp.DoExport(dok, 3, true, null, string.IsNullOrWhiteSpace(nrPartnera) ? "" : nrPartnera, string.IsNullOrWhiteSpace(nrKontaUmowy) ? "" : nrKontaUmowy);
                                if (!String.IsNullOrWhiteSpace(s))
                                {
                                    dok.SAPImportStatus = 1;
                                    done = true;
                                    RupDatabase.theContext.Sprawa.AddObject(dok.Sprawa);
                                    RupDatabase.theContext.SaveChanges();
                                }
                                else
                                {
                                    dok.SAPImportStatus = -1;

                                }


                                if (done)
                                {
                                    ;
                                }

                            }

                        }

                    }


                }
            }
        }
    }
}
