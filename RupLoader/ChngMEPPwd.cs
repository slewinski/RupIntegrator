using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RupLoader
{
    public partial class ChngMEPPwd : Form
    {
        public ChngMEPPwd()
        {
            InitializeComponent();
        }

        private string EncryptPhase = "Application error";

        private void rbOK_Click(object sender, EventArgs e)
        {
            string pold = this.tbLoginMEP.Text;
            string pnew = this.tbNewPassword.Text;
            string pnew2 = this.tbRepeatPwd.Text;


            if (pnew != pnew2)
            {
                MessageBox.Show("Hasło różne od powtórzenia");
                this.DialogResult = DialogResult.None;
                return;
            }
            if (pnew.Length <= 1)
            {
                MessageBox.Show("Hasło jest za krótkie");
                this.DialogResult = DialogResult.None;
                return;
            }
            try
            {
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    User usr = context.User.Where(a => a.Id == UserProfile.UserID && a.deleted == false && a.suspend == false).FirstOrDefault();

                    usr.MEPUser = this.tbLoginMEP.Text.Trim();
                    usr.MEPPassword = Utils.Encrypt(pnew, EncryptPhase);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu");
                this.DialogResult = DialogResult.None;
                return;
            }


        }

        private void ChngMEPPwd_Load(object sender, EventArgs e)
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                User usr = context.User.Where(a => a.Id == UserProfile.UserID && a.deleted == false && a.suspend == false).FirstOrDefault();
                if (usr == null)
                {
                    MessageBox.Show("Brak użytkownika o podanej nazwie");
                    this.DialogResult = DialogResult.None;
                    return;
                }
                this.tbLoginMEP.Text = usr.MEPUser;
                this.tbNewPassword.Text = Utils.Decrypt(usr.MEPPassword, EncryptPhase);
                this.tbRepeatPwd.Text = this.tbNewPassword.Text;
            }
        }
    }
}
