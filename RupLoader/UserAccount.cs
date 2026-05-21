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
    public partial class UserAccount : Form
    {
        public int Id {get;set;}
        public string login { get; set; }
        public string pwd { get; set; }
        public bool chngPwd { get; set; }
        public bool suspended {get;set; }
        public int role {get;set;}
	    public DateTime ChangePwdDate {get;set;}
	    public string FirstName {get;set;}
	    public string LastName {get;set;}
	    public bool deleted  {get;set;} 
	    public DateTime CreationDate {get;set;}
	    public DateTime DeleteDate  {get;set;}
        public RupIntegratorEntities Context { get; set; }
        private string EncryptPhase = "Application error"; 
        private bool LoadUser ()
        {
            if (Id > 0)
            {
                User myuser = this.Context.User.Where(a => a.Id == Id).FirstOrDefault();
                if (myuser == null)
                    return false;
                
                this.tbLogin.Text = myuser.Username;
                this.tbFirstName.Text = myuser.FirstName;
                this.tbLastName.Text = myuser.LastName;
                this.tbPassword.Text = Utils.Decrypt(myuser.Pssword, EncryptPhase);
                this.tbRepeatPwd.Text = this.tbPassword.Text; 
                this.tbPassword.Enabled = false;
                this.tbRepeatPwd.Enabled = false;
                this.radDropDownList1.SelectedIndex = (myuser.role == null ? 0 : Convert.ToInt32(myuser.role));
                this.lbLastPwdChange.Text = myuser.LastPwdChngDate.ToString();
                this.cbSuspend.Checked = Convert.ToBoolean(myuser.suspend);
                this.cbPassChange.Checked = Convert.ToBoolean(myuser.ChangePwd);
                this.tbPeriod.Text = myuser.PwdPeriodChange.ToString() ;
                this.tbMEP.Text = myuser.MEPUser;
                if (myuser.deleted)
                {
                    this.lbDeleted.Text = "Usunięty";
                    this.tbLogin.Enabled = false;
                    this.tbFirstName.Enabled = false;
                    this.tbLastName.Enabled = false;
                    this.tbPassword.Enabled = false;
                    this.tbRepeatPwd.Enabled = false;
                    this.tbPassword.Enabled = false;
                    this.tbRepeatPwd.Enabled = false;
                    this.radDropDownList1.Enabled = false;
                    this.lbLastPwdChange.Enabled = false;
                    this.cbSuspend.Enabled = false;
                    this.cbPassChange.Enabled = false;
                    this.tbPeriod.Enabled = false;
                    this.tbMEP.Enabled = false;
                    this.rbOK.Enabled = false;
                    this.rbResetPwd.Enabled = false;
                }   
            }
            else
            {
                this.radDropDownList1.SelectedIndex = 0;
                this.cbPassChange.Checked = true;
                this.cbSuspend.Checked = false;
                this.tbPeriod.Text = "0";
                
            }
                return true; ;
        
        
        }
        public UserAccount()
        {
            InitializeComponent();
        }

        private void rbOK_Click(object sender, EventArgs e)
        {
            User myuser;
            try
            {
                if (this.tbLogin.Text.Trim().Length == 0 ||
                    this.tbFirstName.Text.Trim().Length == 0 ||
                    this.tbLastName.Text.Trim().Length == 0 ||
                    this.tbPassword.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Pola imię, nazwisko, nazwa użytkownika i hasło nie mogą byc puste");
                    this.DialogResult = DialogResult.None;
                    return;
                }
                if (this.tbPassword.Text.Trim().Length < 6)
                {
                    MessageBox.Show("Hasło nie może być krótsze od 6 znaków");
                    this.DialogResult = DialogResult.None;
                    return;
                }

                if (this.tbPassword.Text.Trim() != this.tbRepeatPwd.Text.Trim())
                {
                    MessageBox.Show("Niezgodność powtórzeń hasła");
                    this.DialogResult = DialogResult.None;
                    return;
                
                }
                if (this.Id > 0) // jeśli edycja
                    myuser = this.Context.User.Where(a => a.Id == Id).FirstOrDefault();
                else
                {   string s = this.tbLogin.Text.Trim();
                    User chkUser = this.Context.User.Where(a => a.Username == s && (a.deleted == false  ) ).FirstOrDefault();
                    if (chkUser != null)
                    {
                        MessageBox.Show("Istnieje konto o takiej nazwie użytkownika");
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                    myuser = new User();
                    myuser.CreationDate = DateTime.Now;
                    myuser.deleted = false;
                }
                myuser.Username = this.tbLogin.Text;
                myuser.FirstName = this.tbFirstName.Text;
                myuser.LastName = this.tbLastName.Text;
                myuser.Pssword = Utils.Encrypt(this.tbPassword.Text, EncryptPhase);
                myuser.PwdPeriodChange = Convert.ToInt32(this.tbPeriod.Text);
                myuser.role = this.radDropDownList1.SelectedIndex;
                myuser.MEPUser = this.tbMEP.Text;
                myuser.suspend = this.cbSuspend.Checked;
                myuser.ChangePwd = this.cbPassChange.Checked;
                if (this.Id > 0)
                    ;
                else
                    this.Context.User.AddObject(myuser);

                Context.SaveChanges();
                this.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Błąd " + ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message:"") );
                this.DialogResult = DialogResult.None;
                
            }
        }

        private void UserAccount_Load(object sender, EventArgs e)
        {
            LoadUser();
        }

        private void tbPeriod_Validating(object sender, CancelEventArgs e)
        {
            int j;
            if  (!Int32.TryParse(tbPeriod.Text,out j))
                    e.Cancel = true;
}

        private static Random random = new Random();
        private  string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void rbResetPwd_Click(object sender, EventArgs e)
        { User  xuser;
        if (this.Id > 0) // jeśli edycja
        {   string pwd = RandomString(6);
            xuser = this.Context.User.Where(a => a.Id == Id).FirstOrDefault();
            xuser.Pssword = Utils.Encrypt(pwd,EncryptPhase);
            Context.SaveChanges();
            MessageBox.Show("Hasło zostało zmienione na:" + pwd, "Zmiana hasła");
            this.Close();


        }
        }
        }


    }

