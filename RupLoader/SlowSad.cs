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
    public partial class SlowSad : Form
    {
        RupIntegratorEntities ruEnt;

        private List<DataRowView> lastRemovedRows = new List<DataRowView>();

        public SlowSad()
        {
            InitializeComponent();
            ruEnt = new RupIntegratorEntities();
            List<RL_Konfig> dbcnf = ruEnt.RL_Konfig.Where(a => a.rodzajDB == 2).ToList();

            this.rgvKuratLst.DataSource = ruEnt.KuratSad.OrderBy(a=>a.Nazwa).ToList();

        }

        private void rbSave_Click(object sender, EventArgs e)
        {
            if (ruEnt != null)
                ruEnt.SaveChanges();
        }

        private void rbDell_Click(object sender, EventArgs e)
        {
            if (this.rgvKuratLst.SelectedRows.Count > 0)
            {
                GridViewDataRowInfo[] rows = new GridViewDataRowInfo[this.rgvKuratLst.SelectedRows.Count];
                this.rgvKuratLst.SelectedRows.CopyTo(rows, 0);

                this.rgvKuratLst.BeginUpdate();

                for (int i = 0; i < rows.Length; i++)
                {
                    this.rgvKuratLst.Rows.Remove(rows[i]);
                }
                KuratSad km = (KuratSad)rgvKuratLst.SelectedRows[0].DataBoundItem;
                this.ruEnt.KuratSad.DeleteObject(km);
                this.ruEnt.SaveChanges();
                this.rgvKuratLst.EndUpdate();
            }  
        }



    }
}
