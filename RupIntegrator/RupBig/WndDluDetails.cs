using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Telerik.WinControls.UI;

namespace RupBig
{
    public partial class WndDluDetails : Form
    {

        public vw_BIG_Dluznicy dlu { get; set; }


        public WndDluDetails()
        {
            InitializeComponent();
            string appDir = Path.GetDirectoryName(Application.ExecutablePath);


            if (File.Exists(appDir + "\\" + "rgvOperacje.lyt"))
                this.rgvDluznik.LoadLayout(appDir + "\\" + "rgvOperacje.lyt");

            GridViewComboBoxColumn col = (GridViewComboBoxColumn)rgvDluznik.Columns["RA_Country"];
            col.DataSource = Utils.kraje;
            col.ValueMember = "skrot";
            col.DisplayMember = "nazwa";

            col = (GridViewComboBoxColumn)rgvDluznik.Columns["Citizenship"];
            col.DataSource = Utils.kraje;
            col.ValueMember = "skrot";
            col.DisplayMember = "nazwa";

            col = (GridViewComboBoxColumn)rgvDluznik.Columns["LiabilityType"];
            col.DataSource = Utils.naleznosci;
            col.ValueMember = "id";
            col.DisplayMember = "nazwa";

            ExpressionFormattingObject obj = new ExpressionFormattingObject("Cond1", "status = 0", false);
            obj.CellBackColor = Color.LightGray;
            obj.CellForeColor = Color.LightGray;
            this.rgvDluznik.Columns["status"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond2", "status = -1 ", false);
            obj.CellBackColor = Color.Red;
            obj.CellForeColor = Color.Red;
            this.rgvDluznik.Columns["status"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond3", "status = 1 ", false);
            obj.CellBackColor = Color.Green;
            obj.CellForeColor = Color.Green;
            this.rgvDluznik.Columns["status"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond4", "status = -1000 ", false);
            obj.CellBackColor = Color.Orange;
            obj.CellForeColor = Color.Orange;
            this.rgvDluznik.Columns["status"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond5", "status = 1000 ", false);
            obj.CellBackColor = Color.LightGreen;
            obj.CellForeColor = Color.LightGreen;
            this.rgvDluznik.Columns["status"].ConditionalFormattingObjectList.Add(obj);
            this.rgvDluznik.ReadOnly = true;

        }

        private void WndDluDetails_Load(object sender, EventArgs e)
        {
            if (dlu != null)
            {
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                { 
                    string dluId = dlu.DebatorID;

                    List<vw_BIG_Operacje> oprLst = context.vw_BIG_Operacje.Where(a => a.DebatorID == dluId).OrderByDescending(a => a.SentDate).ToList();
                    this.rgvDluznik.DataSource = oprLst;
                 
                
                
                }
            
            
            
            
            }
        }

        private void rgvDluznik_DoubleClick(object sender, EventArgs e)
        {

            if ((sender as RadGridView).CurrentRow != null)
            {
                vw_BIG_Operacje bo = (vw_BIG_Operacje)(sender as RadGridView).CurrentRow.DataBoundItem;
                OperDetails odet = new OperDetails();
                odet.operId = bo.IdBIG_InfoOperation;
                odet.ShowDialog();
            }

        }

           
       
      

        private void rbDetailsOp_Click(object sender, EventArgs e)
        {

            if (this.rgvDluznik != null && this.rgvDluznik.SelectedRows.Count > 0)
            {
                vw_BIG_Operacje bo = (vw_BIG_Operacje)this.rgvDluznik.SelectedRows[0].DataBoundItem;

                OperDetails odet = new OperDetails();
                odet.operId = bo.IdBIG_InfoOperation;
                odet.ShowDialog();
            }
            else
            {
                MessageBox.Show("Wybierz wiersz z listy");
                return;
            }
        }

      
    }
}
