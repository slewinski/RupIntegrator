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
    public partial class MapSygnatura : Form
    {
        RupIntegratorEntities ruEnt;

        private List<DataRowView> lastRemovedRows = new List<DataRowView>();






        public MapSygnatura()
        {
            InitializeComponent();

            ruEnt = new RupIntegratorEntities();
            GridViewComboBoxColumn SadIDColumn = rgvSygnMap.Columns["SrcSad"] as Telerik.WinControls.UI.GridViewComboBoxColumn;
            SadIDColumn.DataSource = ruEnt.SAPSad.OrderBy(a => a.miastSad).ToList();
            SadIDColumn.ValueMember = "kod";
            SadIDColumn.DisplayMember = "miastSad";
            SadIDColumn.Width = 150;

            GridViewComboBoxColumn SadIDdestColumn = rgvSygnMap.Columns["DestSad"] as Telerik.WinControls.UI.GridViewComboBoxColumn;
            SadIDdestColumn.DataSource = ruEnt.SAPSad.OrderBy(a => a.miastSad).ToList();
            SadIDdestColumn.ValueMember = "kod";
            SadIDdestColumn.DisplayMember = "miastSad";
            SadIDdestColumn.Width = 150;
            this.rgvSygnMap.DataSource = ruEnt.SygnMap.OrderByDescending(a=>a.Id).ToList();


           
        }

        private void rbSave_Click(object sender, EventArgs e)
        {
            if (ruEnt != null)
                ruEnt.SaveChanges();
        }

        private void rbDell_Click(object sender, EventArgs e)
        {
          
            if (this.rgvSygnMap.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Czy na pewno chcesz usunąć wybrane  mapowanie ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;
                GridViewDataRowInfo[] rows = new GridViewDataRowInfo[this.rgvSygnMap.SelectedRows.Count];
                this.rgvSygnMap.SelectedRows.CopyTo(rows, 0);

                this.rgvSygnMap.BeginUpdate();

                for (int i = 0; i < rows.Length; i++)
                {
                    this.rgvSygnMap.Rows.Remove(rows[i]);
                }
                SygnMap mp = (SygnMap)rgvSygnMap.SelectedRows[0].DataBoundItem;
                this.ruEnt.SygnMap.DeleteObject(mp);
                this.ruEnt.SaveChanges();
                this.rgvSygnMap.EndUpdate();
            }  
        }
       

        private void rbAdd_Click(object sender, EventArgs e)
        {
           
            SygnMap sg = new SygnMap();
            ruEnt.SygnMap.AddObject(sg);
            ruEnt.SaveChanges();
            this.rgvSygnMap.DataSource =  ruEnt.SygnMap.OrderByDescending(a => a.Id).ToList();

        }

       
        }

      

    
}
