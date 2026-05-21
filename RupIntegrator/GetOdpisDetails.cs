using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KnsMigrator
{

    public partial class GetOdpisDetails : Form
    {
            public string opGl {get;set;}
            public DateTime dKsiegowania { get; set; }
            public DateTime dDok { get; set; }
            public string Opis { get; set; }
            public string kluczUzg { get; set; }
            public string opCze { get; set; }

            public bool czyP { get; set; }
            public string opGlP { get; set; }
            public DateTime dKsiegowaniaP { get; set; }
            public DateTime dDokP { get; set; }
            public string OpisP { get; set; }
            public string kluczUzgP { get; set; }
            public bool czyDataP { get; set; }
            public string opCzeP { get; set; }


        public GetOdpisDetails()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            opGl = cbOPGl.Text;
            dKsiegowania = rdtpDataKsie.Value;
            dDok = rdtpDataDok.Value;
            Opis = tbOpis.Text;
            kluczUzg = tbKlucz.Text;
            opCze = tbOpCz.Text;

            opGlP = cbOPGlP.Text;
            dKsiegowaniaP = rdtpDataKsieP.Value;
            dDokP = rdtpDataDokP.Value;
            OpisP = tbOpisP.Text;
            kluczUzgP = tbKluczP.Text;
            czyDataP = cbDataDokP.Checked;
            opCzeP = tbOpCzP.Text;

        }

        private void GetOdpisDetails_Load(object sender, EventArgs e)
        {
            if (czyP)
            {
                gbPrzypisy.Visible = true;
                rdtpDataDok.Value = dDok;
                rdtpDataDokP.Value = dDokP;
                rdtpDataKsie.Value = dKsiegowania;
                rdtpDataKsieP.Value = dKsiegowaniaP;
                
            }
            else
                gbPrzypisy.Visible = false;


        }

      
       
        
      

       
    }
}
