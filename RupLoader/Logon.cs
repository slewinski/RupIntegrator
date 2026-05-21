using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace RupLoader
{
    public partial class Logon : Form
    {
        private string EncryptPhase = "Application error";
        public RupIntegratorEntities Context;
        public User usr;
        public Logon()
        {
            InitializeComponent();
        }

        private void llChangePwd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Zmiana hasła
        }

        private void rbOK_Click(object sender, EventArgs e)
        {
            string pwd = Utils.Encrypt(this.tbPassword.Text,EncryptPhase);
            string user = this.tbUsername.Text;
            UserProfile.Username = this.tbUsername.Text;
            User chkUser = this.Context.User.Where(a => a.Username == user && a.Pssword == pwd && (a.deleted == false ) && a.suspend == false).FirstOrDefault();
            if (chkUser == null)
            {
                MessageBox.Show("Błędna nazwa użytkownika lub hasło ");
                this.DialogResult = DialogResult.None;
                return;

            }
            else

            {
                UserProfile.UserID = chkUser.Id;
                usr = new User();
                usr = chkUser;
                if (chkUser.ChangePwd == true || (chkUser.PwdPeriodChange > 0 && (DateTime.Now - Convert.ToDateTime(chkUser.LastPwdChngDate)).TotalDays > chkUser.PwdPeriodChange))
                {
                    changePwd(Assembly.GetExecutingAssembly().Location);
                    this.DialogResult = DialogResult.None;
                    return;

                }
                else
                { // nomalne logowanie
                    if (cbxSave.Checked)
                    {
                        Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                        try
                        {
                            config.AppSettings.Settings.Remove("UserName");
                        }
                        catch { }
                        try
                        {
                            config.AppSettings.Settings.Remove("UserPwd");
                        }
                        catch { }
                        config.AppSettings.Settings .Add("UserName", chkUser.Username);
                        config.AppSettings.Settings.Add("UserPwd", pwd);
                        config.Save(ConfigurationSaveMode.Full);


                    }
                
                
                }
            }
        }

        private bool changePwd(string username)
        {
            User usr;
            usr = Context.User.Where(a => a.Username == username && (a.deleted == false || a.suspend == false)).FirstOrDefault();
            if (usr == null)
            { 
                MessageBox.Show("Brak użytkownika o podanej nazwie");
                return false;
            }
            ChngPwd chdlg = new ChngPwd();
            chdlg.Context = this.Context;
            chdlg.UserId = usr.Id;
            chdlg.ShowDialog();
            if (chdlg.DialogResult == DialogResult.OK)
                return true;
            else
                return false;
        }

        private void llChangePwd_Click(object sender, EventArgs e)
        {

            if (this.tbUsername.Text.Length > 0)
                this.changePwd(this.tbUsername.Text);

            else
                MessageBox.Show("Podaj nazwę  użytkownika ");
        }

       
    }
}
