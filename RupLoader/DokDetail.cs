using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace RupLoader
{   
    public partial class DokDetail : Form
    {
        public DateTime dKsiegowania { get; set; }
        public DateTime dDokumentu { get; set; }
        public decimal  kwota { get; set; }
        public string opGl { get; set; }
        public string opCz { get; set; }
        private bool skipValidation = false;
        private void setupVlues()
        {
            if (dKsiegowania > new DateTime(2000, 1, 1))
                this.rdtDataKsiegowania.Value = dKsiegowania;
            else
                this.rdtDataKsiegowania.Value = DateTime.Today;

            if ( dDokumentu > new DateTime(2000,1,1))
                this.rdtDataDokumentu.Value = dDokumentu;
        else
                this.rdtDataDokumentu.Value = dKsiegowania;
            
            if ( kwota != 0 ) 
                this.rmebKwota.Value  = kwota;
            this.tbOpGl.Text = opGl;
            this.tbOpCz.Text = opCz;
            
            }
        private void getValues()
        {
            decimal ddd;
            dDokumentu = this.rdtDataDokumentu.Value;
            dKsiegowania = this.rdtDataKsiegowania.Value  ;
            if (decimal.TryParse(this.rmebKwota.Text, NumberStyles.Currency,NumberFormatInfo.CurrentInfo, out ddd))
            {
                kwota = ddd;
            }
            opGl = this.tbOpGl.Text ;
            opCz = this.tbOpCz.Text ;
            
        }
        public DokDetail()
        {
            InitializeComponent();
            
        }

        private void btKsieguj_Click(object sender, EventArgs e)
        {
            getValues();
        }

        private void DokDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!skipValidation && (kwota == 0 || string.IsNullOrWhiteSpace(opGl) || string.IsNullOrWhiteSpace(opCz)))
            {
                MessageBox.Show("Brak kompletu danych do księgowania");
                e.Cancel = true;
            }
        }

        private void DokDetail_Load(object sender, EventArgs e)
        {
            setupVlues();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            skipValidation = true;
        }

       

    }
}
