using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.Collections.Generic;
using Telerik.WinControls.UI;
using RupBig.ServiceReferenceBigMain;
using System.IO;

namespace RupBig
{
    public partial class WinBIGMain : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BIGDBHelper bigHlp;
        private BigStan bStan;
        private BIGConfig bCnf = new BIGConfig();

        public WinBIGMain()
        {
            log.Debug("Bigi");
            InitializeComponent();
         
            bigHlp = new BIGDBHelper(this);
          
            bStan = new BigStan(this);
     
            bStan.bigDB = bigHlp;
            rpvConfig.Visible = false;
            rpvOperacje.Visible = false;
            rpvNoweOper.Visible = false;
            rbOdczyt.Visible = false;
            rbSend.Visible = false;
            rbOdczytOper.Visible = false;
            rbDelOpr.Visible = false;
            rbUpdateOper.Visible = false;
            rbCheck.Visible = false;
            rbAddInNew.Visible = false;
            rbSendAgain.Visible = false;
            LoadAllLayouts();
            
        }

      

        private void rlstBIGMain_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
           
            switch (e.Position)
            {
                case 0: //"Nowe"
                    
                    rpvNoweOper.Visible = true;
                    rpvConfig.Visible = false;
                    rpvNoweOper.Dock = DockStyle.Fill;
                    rpvOperacje.Visible = false;
                    rbOdczyt.Visible = true;
                    rbSend.Visible = true;
                    rbOdczytOper.Visible = false;
                    rbDelOpr.Visible = false;
                    rbUpdateOper.Visible = false;
                    rbCheck.Visible = false;
                    rbAddInNew.Visible = false;
                    rbSendAgain.Visible = false;
                    break;
                case 1: //"Operacje":
                    rpvConfig.Visible = false;
                    rpvNoweOper.Visible = false;
                    rpvOperacje.Dock = DockStyle.Fill;
                    rpvOperacje.Visible = true;

                    rbOdczyt.Visible = false;
                    rbSend.Visible = false;

                    rbOdczytOper.Visible = true;
                    rbDelOpr.Visible = true;
                    rbUpdateOper.Visible = true;
                    rbCheck.Visible = true;
                    rbAddInNew.Visible = true;
                    rbSendAgain.Visible = true;
                    break;
                case 2: //"Konfiguracja":
                    rpvNoweOper.Visible = false;
                    rpvOperacje.Visible = false;
                    rpvConfig.Dock = DockStyle.Fill;
                    rpvConfig.Visible = true;
                    rbOdczyt.Visible = false;
                    rbSend.Visible = false;
                    rbOdczytOper.Visible = false;
                    rbDelOpr.Visible = false;
                    rbUpdateOper.Visible = false;
                    rbCheck.Visible = false;
                    rbAddInNew.Visible = false;
                    rbSendAgain.Visible = false;
                    bCnf.setupConfigPageView(rgvBIGi,rgvUsers4BIG, rgvBIGUsers, this);
                    break;
                default: break;
                
            
            }

        }

        private void saveLayout(RadGridView  rgv, string rgvName)
        {
            string appDir = Path.GetDirectoryName(Application.ExecutablePath);

            if (MessageBox.Show("Czy chcesz zapisać bieżący układ ?", "Zapis układu", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                rgv.SaveLayout(appDir + "\\" +   rgvName+ ".lyt");

            }
        
        
        }

        private void LoadAllLayouts()
        {
        string appDir = Path.GetDirectoryName(Application.ExecutablePath);

          
        if (File.Exists(appDir + "\\" + "rgvMyDlu.lyt"))
            this.rgvMyDlu.LoadLayout(appDir + "\\" + "rgvMyDlu.lyt");

        if (File.Exists(appDir + "\\" + "rgvOperacje.lyt"))
            this.rgvOperacje.LoadLayout(appDir + "\\" + "rgvOperacje.lyt");



        if (File.Exists(appDir + "\\" + "rgvDelDlu.lyt"))
            this.rgvDelDlu.LoadLayout(appDir + "\\" + "rgvDelDlu.lyt");


        if (File.Exists(appDir + "\\" + "rgvUpdate.lyt"))
            this.rgvUpdate.LoadLayout(appDir + "\\" + "rgvUpdate.lyt");


        if (File.Exists(appDir + "\\" + "rgvNowe.lyt"))
            this.rgvNowe.LoadLayout(appDir + "\\" + "rgvNowe.lyt");
       


        }


        private void rbSaveLayoutDlu_Click(object sender, EventArgs e)
        {
            saveLayout(this.rgvMyDlu,"rgvMyDlu");
        }

        private void rbSaveLayoutStan_Click(object sender, EventArgs e)
        {
            saveLayout(this.rgvOperacje,"rgvOperacje");
            
        }

        private void rbSaveLayoutDel_Click(object sender, EventArgs e)
        {
            saveLayout(this.rgvDelDlu, "rgvDelDlu"); 
        }

        private void rbSaveLayoutUpdate_Click(object sender, EventArgs e)
        {
            saveLayout(this.rgvUpdate, "rgvUpdate"); 
        }

        private void rbSaveLayoutNowe_Click(object sender, EventArgs e)
        {
            saveLayout(this.rgvNowe, "rgvNowe"); 
        }

        private void rgvBIGUsers_CommandCellClick(object sender, GridViewCellEventArgs e)
        {
            if (e == null) return;
            if (e.RowIndex >= 0)
            {
                string userName;
                string bigName;
                string password;
                userName = e.Row.Cells["BigUserName"].Value.ToString();
                bigName = e.Row.Cells["IdBIG"].Value.ToString();
                password = e.Row.Cells["BigUserPassword"].Value.ToString();

                GridViewComboBoxColumn comboBoxColumn = this.rgvBIGUsers.Columns["IdBIG"] as GridViewComboBoxColumn;
                object value = this.rgvBIGUsers.Rows[e.RowIndex].Cells["IdBIG"].Value;
                string txt = (string)comboBoxColumn.GetLookupValue(value);
                SetPassword spdialog = new SetPassword();
                spdialog.lbBig.Text = txt;
                spdialog.lbUser.Text = userName;
                spdialog.tbPass.Text = password;
                spdialog.tbPass2.Text = password;

                if (spdialog.ShowDialog() == DialogResult.OK)
                {
                    string pass = spdialog.tbPass.Text;
                    e.Row.Cells["BigUserPassword"].Value = pass;
                    e.Row.Cells["BigUserSha256"].Value =   Utils.sha256_hash(pass);
                }   


            }
        }
    }
}
