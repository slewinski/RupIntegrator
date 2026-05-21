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
    public partial class ChngPwd : Form
    {
        public int UserId { get; set; }
        public RupIntegratorEntities Context;
        public ChngPwd()
        {
            InitializeComponent();
        }

        private string EncryptPhase = "Application error"; 

        private void rbOK_Click(object sender, EventArgs e)
        {
            string pold = this.tbPassword.Text;
            string pnew = this.tbNewPassword.Text;
            string pnew2 = this.tbRepeatPwd.Text;


            if (pnew != pnew2)
            {
                MessageBox.Show("Nowe hasło różne od powtórzenia");
                this.DialogResult = DialogResult.None;
                return;
            }
            if (pnew.Length < 6 )
            {
                MessageBox.Show("Nowe hasło jest krótsze od 6  znaków");
                this.DialogResult = DialogResult.None;
                return;
            }
            try
            {

            User usr = this.Context.User.Where(a=>a.Id == UserId && a.deleted == false && a.suspend == false).FirstOrDefault();
            if (usr == null)
            { 
                MessageBox.Show("Brak użytkownika o podanej nazwie");
                this.DialogResult = DialogResult.None;
                return;
            }
            if ( Utils.Decrypt(usr.Pssword,EncryptPhase) !=  pold)
            {
                MessageBox.Show("Podano błędne dotychczasowe hasło");
                this.DialogResult = DialogResult.None;
                return;
            }
            usr.Pssword = Utils.Encrypt(pnew, EncryptPhase);
            usr.LastPwdChngDate = DateTime.Now;
            usr.ChangePwd = false;
          
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu");
                this.DialogResult = DialogResult.None;
                return;
            }
            

        }
    }
}
