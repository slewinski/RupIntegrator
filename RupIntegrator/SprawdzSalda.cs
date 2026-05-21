using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Telerik.WinControls.UI;


namespace KnsMigrator
{
    public partial class SprawdzSalda : Form
    {
        public KnsMigratorEntities thecontext {get;set;}
        private BindingSource KsiegiDataSource = new BindingSource();
        public List<int> KsiegiKnsLst { get; set; }
        public  Guid myId {get;set;}
        public int mode { get; set; } 

        public SprawdzSalda()
        {
            InitializeComponent();
           

        }

          private void btSrc_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    this.tbInput.Text = openFileDialog.FileName;
                    this.btRun.Enabled = true;
                }
            }

        }
      
          private void btRun_Click(object sender, EventArgs e)
          {
              // wczytanie zbioru
              // Wczytanie sald dla ksąg na dany dzień 
              // porównanie sald
              // odczyta
              //MessageBox.Show("Funkcja w przygotowaniu");

              Cursor.Current = Cursors.WaitCursor;
              Imports imp; 
              List<int> lstkns = new List<int>();

              tbMess.Text = "Import Sald z systemu merytorycznego...";
              tbMess.Refresh();
           // odczyt listy ksiąg
             if (KsiegiKnsLst == null)
                 KsiegiKnsLst = new List<int>();
             else
                 KsiegiKnsLst.Clear();
            
                 foreach (GridViewRowInfo row in this.rgvKsiegi.Rows)
                 {
                     if ((Convert.ToBoolean(row.Cells["taknie"].Value)))
                     {
                         lstkns.Add((Convert.ToInt32(row.Cells["Id_Ksiegi"].Value)));
                         KsiegiKnsLst.Add((Convert.ToInt32(row.Cells["Id_Ksiegi"].Value)));
                     }
                 }


               
                imp = new Imports();
                imp.Context = thecontext;
                imp.Konfig = this.thecontext.Konfiguracja.FirstOrDefault();
                imp.theday = this.rdtDzien.Value;
                imp.KsiegiKnsLst = this.KsiegiKnsLst;
                //imp.ImportSaldo();
                myId = Guid.NewGuid();
                if (mode == 1) // walidajca przypisów
                {
                    tbMess.Text = "Import przypisów przekazanych do SAP...";
                    tbMess.Refresh();
                    imp.ImportPrzypisRupIntegr(myId, tbPos);
                }
                else
                    imp.ImportSaldoShort(myId, tbPos);
                Validator valid = new Validator();
                valid.thecontext = this.thecontext;
                valid.myguid = myId;
                valid.fileName = this.tbInput.Text;
                tbMess.Text = "Import danych ze zbioru .csv - ZPSCDDOKS..."; 
                valid.CreateSchema();
                if (mode == 1)
                {
                    valid.ImportZPSCDDOKSPrzypisy(tbPos, tbMess);
                    this.Close();
                    return;
                }
                else
                    valid.ImportZPSCDDOKS(tbPos, tbMess);
                // zakończeni 
                tbMess.Text = "Weryfikacja zapisów w SAP...";
                tbMess.Refresh();
                List<WalidSaldo> bezDokum = thecontext.WalidSaldo.Where(a => a.Klucz == myId ).ToList();
                foreach (WalidSaldo ws in bezDokum)
                {
                    if (!String.IsNullOrWhiteSpace(ws.Status)) continue;
                    if (ws.OpGlowna == null)
                     ws.Status = "Brak dokumentu dla kdł. w SAP lub dokument rozliczony w całości";
                    else
                    {
                        if (ws.Kwota == ws.SAPKwota)
                            ws.Status = "OK";
                        else
                            ws.Status = "Różnica w wysokości sald";
                    }
                }
                thecontext.SaveChanges();
                this.Close();
          }


          private void setWinMode()
          {
              if (mode == 1)
              {
                  // jeśłi walidacja dokumentów przpisu
                  label1.Visible = false;
                  rdtDzien.Visible = false;
                  this.Text = "Walidacja przypisów";

              }
          
          
          
          }
         
          private void SprawdzSalda_Load(object sender, EventArgs e)
          {

              
              string tmp;
              int i;
              List<int> knsLst = new List<int>();
              setWinMode();
              this.rdtDzien.Value = DateTime.Today;
              this.KsiegiDataSource.DataSource = this.thecontext.KnsKsiegi.ToList();
              this.rgvKsiegi.DataSource = this.KsiegiDataSource; //.Mains;

              try
              {
                  btRun.Enabled = false;
                  // ładowanie czheckbox - configa
                  foreach (string key in ConfigurationManager.AppSettings)
                  {
                      if (key.StartsWith("KnsNal"))
                      {
                          tmp = ConfigurationManager.AppSettings[key];
                          if (Int32.TryParse(tmp, out i))
                          {
                              knsLst.Add(i);

                          }


                      }
                  }
                  foreach (GridViewRowInfo row in this.rgvKsiegi.Rows)
                  {
                      if (knsLst.Contains(Convert.ToInt32(row.Cells["Id_Ksiegi"].Value)))
                      {
                          row.Cells["taknie"].Value = true;
                      }
                  }


              }
              catch (Exception ex)
              {
                  MessageBox.Show("Błąd ładowania z  pliku konfiguracyjnego " + ex.Message);

              }
          }
        }
}
