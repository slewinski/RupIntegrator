using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace RupLoader
{
    public partial class MapSadKurat : Form

        
    {
        public string sadSAP { get; set; }
        public string sadKura { get; set;}
        public string sygnatura { get; set; }
        public bool forceBreak { get; set; }

        public MapSadKurat()
        {
            InitializeComponent();
            forceBreak = false;
            using (RupIntegratorEntities theContext = new RupIntegratorEntities())
            {
                rgvSAPSad.DataSource = theContext.SAPSad.Where(a => a.kod != "MS" && a.kod != null).OrderBy(a => a.miasto).ThenBy(a=>a.typSad);
                
            
            }
            
        }

        private void rgvSAPSad_DoubleClick(object sender, EventArgs e)
        {
            if (rgvSAPSad.CurrentRow == null)
               return;


            sadSAP = tbSapSad.Text = (sender as RadGridView).CurrentRow.Cells["kod"].Value + " " + (sender as RadGridView).CurrentRow.Cells["sad"].Value + " " + (sender as RadGridView).CurrentRow.Cells["miasto"].Value;


        }

        private void MapSadKurat_Load(object sender, EventArgs e)
        {
            tbSygn.Text = sygnatura;
            tbSad.Text = sadKura;
        }

        private void MapSadKurat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(sadSAP) && !forceBreak)
            {
                MessageBox.Show("Wybierz przez dwuklik właściwy sąd z listy ");
                e.Cancel = true;
            
            }
        }

        private void rbCancel_Click(object sender, EventArgs e)
        {
                     
            forceBreak = true;
        }

        

       
    }
}
