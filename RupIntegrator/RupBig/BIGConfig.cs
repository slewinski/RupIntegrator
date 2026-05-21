using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using RupBig.ServiceReferenceBigMain;
using System.Windows.Forms;

namespace RupBig
{
    public class BIGConfig
    {
        private RadGridView  rgvBU;
        private RadGridView rgvU4B;
        private RadGridView rgvU;
        private RadButton rbSaveWSConfig;
        private TextBox tbWSSubmit;
        private TextBox tbUserSubmit;
        private TextBox tbPasswordSubmit;
        private TextBox tbWSCheck;
        private TextBox tbUserCheck;
        private TextBox tbPasswordCheck;
        private TextBox tbSys;
        private RadButton rbBUsersSave;
        private RadButton rbSaveBIGs;
        private RadGridView rgvBIGi;

        private void fillBigs(RupIntegratorEntities context)
        {
            BIG_Big bg = new BIG_Big();
            context.BIG_Big.AddObject(new BIG_Big{BIGID = CredentialsBig_id.KBIG.ToString()});
            context.BIG_Big.AddObject(new BIG_Big{BIGID = CredentialsBig_id.ERIF.ToString()});
            context.BIG_Big.AddObject(new BIG_Big{BIGID = CredentialsBig_id.KIDT.ToString()});
            context.BIG_Big.AddObject(new BIG_Big{BIGID = CredentialsBig_id.KRD.ToString()});
            context.BIG_Big.AddObject(new BIG_Big{BIGID = CredentialsBig_id.IM.ToString()});
            context.BIG_Big.AddObject(new BIG_Big { BIGID = CredentialsBig_id.BNP.ToString() });
            context.SaveChanges();

           
        
        }

        private void setWsConfig()
        {

            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {

                BIG_Konfig konf = context.BIG_Konfig.FirstOrDefault();
                if (konf != null)
                {
                    tbWSSubmit.Text = konf.SubmitEndpoint;
                    tbUserSubmit.Text = konf.SubmitAuthUser;
                    tbPasswordSubmit.Text = Utils.Decrypt(konf.SubmitAuthPasword, "Application error");
                    tbWSCheck.Text = konf.CheckRqEndpoint;
                    tbUserCheck.Text = konf.CheckRqAuthUser;
                    tbPasswordCheck.Text = Utils.Decrypt(konf.CheckRqAuthPass, "Application error");
                    tbSys.Text = konf.SysPrefix;
                }
            
            }
        
        
        }
 



        public void setupConfigPageView(RadGridView rgvBIGI, RadGridView rgvUsers4BIG, RadGridView rgvBUsers, WinBIGMain theWind)
        {
            this.rgvBU = rgvBUsers;
            this.rgvU4B = rgvUsers4BIG;

            rgvBUsers.CellEditorInitialized -= rgvBUsers_CellEditorInitialized;
            rgvBUsers.CellEditorInitialized += new GridViewCellEventHandler(rgvBUsers_CellEditorInitialized);

            rgvBUsers.CellFormatting += new CellFormattingEventHandler(rgvBUsers_CellFormatting);

            rbSaveWSConfig = (RadButton)(theWind.Controls.Find("rbSaveWSConfig", true).FirstOrDefault());
            tbWSSubmit = (TextBox)(theWind.Controls.Find("tbWSSubmit", true).FirstOrDefault());
            tbUserSubmit = (TextBox)(theWind.Controls.Find("tbUserSubmit", true).FirstOrDefault());
            tbPasswordSubmit = (TextBox)(theWind.Controls.Find("tbPasswordSubmit", true).FirstOrDefault());
            tbWSCheck = (TextBox)(theWind.Controls.Find("tbWSCheck", true).FirstOrDefault());
            tbUserCheck = (TextBox)(theWind.Controls.Find("tbUserCheck", true).FirstOrDefault());
            tbPasswordCheck = (TextBox)(theWind.Controls.Find("tbPasswordCheck", true).FirstOrDefault());
            tbSys = (TextBox)(theWind.Controls.Find("tbSys", true).FirstOrDefault());
            rbBUsersSave = (RadButton)(theWind.Controls.Find("rbBUsersSave", true).FirstOrDefault());
            rbSaveBIGs = (RadButton)(theWind.Controls.Find("rbSaveBIGs",true).FirstOrDefault());
            rgvBIGi = (RadGridView)(theWind.Controls.Find("rgvBIGi",true).FirstOrDefault());

            rbSaveWSConfig.Click += new EventHandler(rbSaveWSConfig_Click);
            rbBUsersSave.Click += new EventHandler(rbBUsersSave_Click);
            rbSaveBIGs.Click+=new EventHandler(rbSaveBIGs_Click);
            using (RupIntegratorEntities  context = new RupIntegratorEntities())
            {
               GridViewComboBoxColumn bColumn = (GridViewComboBoxColumn)rgvBUsers.Columns["IdBIG"];


               bColumn.DataSource =  context.BIG_Big.ToList();
               bColumn.ValueMember = "IdBig";
               bColumn.DisplayMember = "BIGID";
           

                 List<BIG_Big> bgl = context.BIG_Big.ToList();
                if (bgl == null || !bgl.Any() )
                {
                    fillBigs(context);
                    bgl = context.BIG_Big.ToList();
                }
                 rgvUsers4BIG.SelectionChanged += new EventHandler(rgvUsers4BIG_SelectionChanged);
                 rgvBIGI.DataSource =  bgl;
                 rgvUsers4BIG.DataSource =( from  u in context.User where u.suspend == false && u.deleted == false  orderby u.Username select u).ToList(); 
             
                
            }
            setWsConfig();
        }

        void rbSaveBIGs_Click(object sender, EventArgs e)
        {

            saveBIGs();
        }

        void rbBUsersSave_Click(object sender, EventArgs e)
        {
            saveBigUsers();   
        }

        void rbSaveWSConfig_Click(object sender, EventArgs e)
        {


            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {

                BIG_Konfig konf = context.BIG_Konfig.FirstOrDefault();
                if (konf == null)
                {
                    konf = new BIG_Konfig();
                    context.BIG_Konfig.AddObject(konf);
                }
                konf.SubmitEndpoint = tbWSSubmit.Text;
                konf.SubmitAuthUser = tbUserSubmit.Text;
                konf.SubmitAuthPasword = Utils.Encrypt(tbPasswordSubmit.Text, "Application error");
                konf.CheckRqEndpoint = tbWSCheck.Text;
                konf.CheckRqAuthUser = tbUserCheck.Text;
                konf.CheckRqAuthPass = Utils.Encrypt(tbPasswordCheck.Text, "Application error");
                konf.SysPrefix = tbSys.Text;
                context.SaveChanges();

            }
        }

        void rgvBUsers_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            GridViewDataColumn dataColumn = e.CellElement.ColumnInfo as GridViewDataColumn;

            if (dataColumn != null && dataColumn.Name == "BigUserPassword")
            {
                object value = e.CellElement.RowInfo.Cells["BigUserPassword"].Value;
                string text = String.Empty;
                if (value != null)
                {
                    int passwordLen = Convert.ToString(value).Length;
                    text = String.Join("*", new string[passwordLen]);
                }

                e.CellElement.Text = text;
            }
        }

        void rgvBUsers_CellEditorInitialized(object sender, GridViewCellEventArgs e)
        {

            GridViewDataColumn dataColumn = e.Column as GridViewDataColumn;

            if (dataColumn != null)
            {
                RadTextBoxEditor textBoxEditor = this.rgvBU.ActiveEditor as RadTextBoxEditor;

                if (textBoxEditor != null)
                {
                    RadTextBoxEditorElement editorElement = textBoxEditor.EditorElement as RadTextBoxEditorElement;

                    if (dataColumn.Name == "BigUserPassword")
                    {
                        editorElement.PasswordChar = '*';
                    }
                    else
                    {
                        editorElement.PasswordChar = '\0';
                    }
                }
            }


        }


        private void saveBigUsers()
        {

            using (RupIntegratorEntities rue = new RupIntegratorEntities())
            {

                // sprawdzenie czy nastąpiła zmiana
                foreach (GridViewRowInfo row in rgvBU.Rows)
                {
                    BIG_User bu  = (BIG_User) row.DataBoundItem;
                    BIG_User dbBUser = rue.BIG_User.Where(a => a.IdBIG == bu.IdBIG && a.IdUser == bu.IdUser).FirstOrDefault();
                    if (dbBUser == null) continue;
                    string pass = Utils.Encrypt(bu.BigUserPassword, "Application error");
                    if (dbBUser.BigUserPassword == pass && dbBUser.BigUserName == bu.BigUserName)
                        continue;
                    dbBUser.BigUserName = bu.BigUserName;
                    dbBUser.BigUserPassword = pass;
                    dbBUser.BigUserSha256 = Utils.sha256_hash(bu.BigUserPassword); 
                     
                    

                }
                rue.SaveChanges();
            
            
            }
        
        
        
        
        }

        private void saveBIGs()
        {

            using (RupIntegratorEntities rue = new RupIntegratorEntities())
            {

                // sprawdzenie czy nastąpiła zmiana
                foreach (GridViewRowInfo row in rgvBIGi.Rows)
                {
                    BIG_Big bi = (BIG_Big)row.DataBoundItem;
                    BIG_Big bb = rue.BIG_Big.Where(a => a.IdBig == bi.IdBig).FirstOrDefault();
                    if (bb == null) continue;
                    bb.Obsluga = bi.Obsluga;
                    bb.SubscriberId = bi.SubscriberId;

                }
                rue.SaveChanges();


            }




        }

        private void reloadBiGUsers( User u)
        {
            bool anychange = false;
            using (RupIntegratorEntities r = new RupIntegratorEntities())
            {
                if (rgvBU.Rows.Any())
                    saveBigUsers();

                List <BIG_Big> lBIGs  = r.BIG_Big.Where(a=>a.Obsluga == true).ToList();
                if (lBIGs == null || !lBIGs.Any())
                    return;

                foreach (BIG_Big bbin in lBIGs)
                {
                    BIG_User bu  =  r.BIG_User.Where(a => a.IdBIG == bbin.IdBig && a.IdUser == u.Id).FirstOrDefault();
                    if  (bu == null)
                    {
                        bu = new BIG_User();
                        bu.IdBIG = bbin.IdBig;
                        bu.IdUser = u.Id;
                        bu.BigUserName  = "<nazwa użytkownika w BIG>";
                        bu.BigUserPassword = Utils.Encrypt("", "Application error"); 
                        r.BIG_User.AddObject(bu);    
                        anychange  = true;
                    }
                }
                if (anychange)
                    r.SaveChanges();

                  List<BIG_User> buser = r.BIG_User.Where(a=> a.IdUser == u.Id).ToList();
                  foreach (BIG_User b in buser)
                      b.BigUserPassword = Utils.Decrypt(b.BigUserPassword, "Application error");
                  this.rgvBU.DataSource = buser;


            }
        
        }

        void rgvUsers4BIG_SelectionChanged(object sender, EventArgs e)
        {
            User u = ((RadGridView)sender).CurrentRow.DataBoundItem as User;
            if (u == null) return;
           
            this.reloadBiGUsers(u);
        }

        void rgvBUsers_UserAddingRow(object sender, GridViewRowCancelEventArgs e)
        {
            throw new NotImplementedException();
        }

      

     }
}
