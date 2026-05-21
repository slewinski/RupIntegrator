using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Localization;
using Telerik.WinControls.UI;
using System.IO;

namespace RupLoader
{
    public partial class ExtraSearch : Form
    {
        PaymentService psrv;
        
        public string searchKey { get; set; }

        public GridViewRowInfo theRow { get; set; }  


        public ExtraSearch()
        {
            InitializeComponent();
            RadGridLocalizationProvider.CurrentProvider = new PolishRadGridLocalizationProvider();
            psrv = new PaymentService();
                    
        }

        private void ExtraSearch_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(searchKey))
            {
                this.Text += " :" + searchKey; 
                Cursor = Cursors.WaitCursor;
                psrv.DoSearchEx(searchKey, this.rgvSearch,"");
                Cursor = Cursors.Default;
                if (File.Exists("extraSearch.lyt"))
                    this.rgvSearch.LoadLayout("extraSearch.lyt");
            }
        }
        private void rgvSearch_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as RadGridView).CurrentRow != null)
            {
                this.theRow = (sender as RadGridView).CurrentRow;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }

           
        }

        private void rbOK_Click(object sender, EventArgs e)
        {

            if (rgvSearch.CurrentRow != null)
            {
                this.theRow = rgvSearch.CurrentRow;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void bt_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Czy chcesz zapisać układ ?", "Zapis układu tabeli", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                this.rgvSearch.SaveLayout("extraSearch.lyt");
            }
        }

            
           
        


    }
}
