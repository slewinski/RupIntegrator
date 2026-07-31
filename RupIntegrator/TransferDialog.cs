using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using System.Configuration;
using Telerik.WinControls.UI;



namespace KnsMigrator
{
    public partial class TransferDialog : Telerik.WinControls.UI.RadForm
    {
        public string TypTransfer { get; set; }
        public DateTime dOd { get; set; }
        public DateTime dDo { get; set; }
        public string Uwagi { get; set; }
        public List<int> KsiegiKnsLst { get; set; }
        public KnsMigratorEntities Context { get; set; }
        public bool newOnly { get; set; } 
        
        private BindingSource KsiegiDataSource = new BindingSource();
        public TransferDialog()
        {
            InitializeComponent();
            dOd = Convert.ToDateTime("1990-01-01");
            dDo = DateTime.Now; 
            
        }


        private void TransferDialog_Load(object sender, EventArgs e)
        {
            List<int> knsLst = new List<int>();
            this.labelTyp.Text = TypTransfer;
            this.rdtOd.Value = dOd;
            this.rdtDo.Value = dDo;
            this.rtbUwagi.Text = Uwagi;
            string tmp;
            int i;

            this.KsiegiDataSource.DataSource = this.Context.KnsKsiegi.Where(a=>a.czyFPP <=2 || a.czyFPP ==4 ||  a.czyFPP == null).ToList();
            this.rgvKsiegi.DataSource = this.KsiegiDataSource; //.Mains;

            try
            {

                if (TypTransfer != "Zwrot 3/4")
                {
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
                else
                {
                    this.rgvKsiegi.Visible = false;
                    this.lbKsiegi.Visible = false;
                }
                if (TypTransfer == "Przypisy" || TypTransfer == "Odpisy" || TypTransfer == "Uiszczenia Grz.Odp." || TypTransfer == "Zwrot 3/4" || TypTransfer == "Przypis opłat")
                {
                    if (this.chNewOnly.Checked)
                    {
                        this.rdtDo.Enabled = false;
                        this.rdtOd.Enabled = false;

                    }
                }
                else
                    this.chNewOnly.Visible = false;  
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania z  pliku konfiguracyjnego " + ex.Message);

            }

     
        }

        private void rbOK_Click(object sender, EventArgs e)
        {
            dOd = this.rdtOd.Value;
            dDo = this.rdtDo.Value;
            Uwagi = this.rtbUwagi.Text;
            List<int> lstkns = new List<int>();
            // save configuration.
            if (KsiegiKnsLst == null)
                KsiegiKnsLst = new List<int>();
            else
                KsiegiKnsLst.Clear();
            newOnly = this.chNewOnly.Checked;

            // For read access you do not need to call OpenExeConfiguraton
            try
            {
                foreach (GridViewRowInfo row in this.rgvKsiegi.Rows)
                {
                    if ((Convert.ToBoolean(row.Cells["taknie"].Value)))
                    {
                        lstkns.Add((Convert.ToInt32(row.Cells["Id_Ksiegi"].Value)));
                        KsiegiKnsLst.Add((Convert.ToInt32(row.Cells["Id_Ksiegi"].Value)));
                    }
                }


                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                foreach (string key in ConfigurationManager.AppSettings)
                {
                    if (key.StartsWith("KnsNal"))
                    {
                        string value = ConfigurationManager.AppSettings[key];
                        int j;
                        int lp = Int32.Parse(key.Substring(6));
                        if (lstkns.Contains(lp))
                        {
                            lstkns.Remove(lp);
                            config.AppSettings.Settings[key].Value = lp.ToString();
                        }
                        else
                            config.AppSettings.Settings[key].Value = "";

                    }

                }

                foreach (int j in lstkns)
                    config.AppSettings.Settings.Add("KnsNal" + j.ToString(), j.ToString());

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu w  pliku konfiguracyjnym " + ex.Message + " sprawdż możliwość zapisu w pliku konfiguracyjnym w folderze instalacyjnym " );

            }


        }

        private void chNewOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chNewOnly.Checked)
            {
                this.rdtDo.Enabled = false;
                this.rdtOd.Enabled = false;

            }
            else
            {
                this.rdtDo.Enabled = true;
                this.rdtOd.Enabled = true;
            
            }

        }

       
    }
}
