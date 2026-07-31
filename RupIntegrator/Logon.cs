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
    public partial class Logon : Form
    {
        private string EncryptPhase = "Application error";
        public KnsMigratorEntities Context;
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

            //pwd = Utils.Decrypt(this.tbPassword.Text, EncryptPhase);

            UserProfile.Username = this.tbUsername.Text;
            int id = (from u in this.Context.User where u.Username == user && u.Pssword == pwd && u.deleted == false && u.suspend == false select u.Id).FirstOrDefault();
            if (id == 0)
            {
                MessageBox.Show("Błędna nazwa użytkownika lub hasło ");
                this.DialogResult = DialogResult.None;
                return;
            }
            int role = (from u in this.Context.User where u.Username == user && u.Pssword == pwd && u.deleted == false && u.suspend == false select u.role).FirstOrDefault();


            string dbversion = KnsMigrator.MigrForm.getDBVersion();
            while (dbversion != RunMode.dbversion)
            {

                if (role != 1)
                {
                    MessageBox.Show("Wersja bazy danych: \"" + dbversion + "\" jest niezgodna z wersją bazy danych oczekiwaną przez aplikację \"" + RunMode.dbversion + "\".\r\nNależy zalogować się na profilu administratora i wykonać operację przebudowy bazy danych.");
                    Application.Exit();
                    return;
                }
                if (MessageBox.Show("Wersja bazy danych: \"" + dbversion + "\" jest niezgodna z wersją bazy danych systemu \"" + RunMode.dbversion + "\".\r\nSystem wymaga wykonania przebudowy bazy danych. Upewnij się, czy użykownik posiada uprawnienia do przebudowy bazy.\n\r Czy wykonać operację przebudowy bazy danych ? ", "Niezgodność wersji bazy danych", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    Application.Exit();
                    return;
                }
                MigrForm.rebuildDbScript();
                dbversion = KnsMigrator.MigrForm.getDBVersion();
            }

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
                    changePwd(usr.Username);
                    this.DialogResult = DialogResult.None;
                    return;
                
                }
                // sprawdzenie hasła

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
