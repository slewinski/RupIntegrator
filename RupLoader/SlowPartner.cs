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
    public partial class SlowPartner : Form
    {
        RupIntegratorEntities ruEnt;

        private List<DataRowView> lastRemovedRows = new List<DataRowView>();
      





        public SlowPartner()
        {
            InitializeComponent();

            ruEnt = new RupIntegratorEntities();
            List<RL_Konfig> dbcnf = ruEnt.RL_Konfig.Where(a => a.rodzajDB == 2).ToList();

            this.rgvKuratLst.DataSource = ruEnt.MapPartner.OrderBy(a=>a.Nazwisko).ToList();


            rddlSlowTyp.DataSource = Utils.naleznosci;
            rddlSlowTyp.DisplayMember = "nazwa";
            rddlSlowTyp.ValueMember = "nr";
            rddlSlowTyp.SelectedValue = 0;
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
                if (MessageBox.Show("Czy na pewno chcesz usunąć wybranego partnera ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;
                GridViewDataRowInfo[] rows = new GridViewDataRowInfo[this.rgvKuratLst.SelectedRows.Count];
                this.rgvKuratLst.SelectedRows.CopyTo(rows, 0);

                this.rgvKuratLst.BeginUpdate();

                for (int i = 0; i < rows.Length; i++)
                {
                    this.rgvKuratLst.Rows.Remove(rows[i]);
                }
                MapPartner mp = (MapPartner)rgvKuratLst.SelectedRows[0].DataBoundItem;
                this.ruEnt.MapPartner.DeleteObject(mp);
                this.ruEnt.SaveChanges();
                this.rgvKuratLst.EndUpdate();
            }  
        }
        private int getSlowType()
        {
            if (rddlSlowTyp.SelectedItems.Any())
            {

                typPartner tp = (typPartner)(rddlSlowTyp.SelectedItem.DataBoundItem);
                return tp.nr;
            }

            return 0;
        }

        private void rbAdd_Click(object sender, EventArgs e)
        {
            NewMapPartner nmp = new NewMapPartner();
            nmp.TypPartner = getSlowType();
            nmp.rue = this.ruEnt;
           if ( nmp.ShowDialog() ==  System.Windows.Forms.DialogResult.OK)
            this.rgvKuratLst.DataSource = ruEnt.MapPartner.OrderBy(a => a.Nazwisko).ToList();

        }

        private void rbEdit_Click(object sender, EventArgs e)
        {
            if ( rgvKuratLst.SelectedRows.Any())
            {
                MapPartner mp = rgvKuratLst.CurrentRow.DataBoundItem as MapPartner;
                NewMapPartner nmp = new NewMapPartner();
                nmp.TypPartner =  mp.typSlow; //  Utils.naleznosci.Where(a=>a.nr == mp.typSlow).FirstOrDefault().nr;
                nmp.IdPartner = mp.Id;
                nmp.rue = this.ruEnt;
                if (nmp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    this.rgvKuratLst.DataSource = ruEnt.MapPartner.OrderBy(a => a.Nazwisko).ToList();
            }
        }

        private void rddlSlowTyp_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
           if (e.Position > 0  )
               this.rgvKuratLst.DataSource = ruEnt.MapPartner.Where(a=>a.typSlow == e.Position).OrderBy(a => a.Nazwisko).ToList();
          else
  
            this.rgvKuratLst.DataSource = ruEnt.MapPartner.OrderBy(a => a.Nazwisko).ToList();

        }



    }
}
