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
    public partial class SetPfxPwd : Form
    {
        public SetPfxPwd()
        {
            InitializeComponent();
        }

        private string EncryptPhase = "Application error"; 

        private void rbOK_Click(object sender, EventArgs e)
        {
            
            string pnew = this.tbNewPassword.Text;
            string pnew2 = this.tbRepeatPwd.Text;


            if (pnew != pnew2 || string.IsNullOrWhiteSpace(pnew))
            {
                MessageBox.Show("Nowe hasło różne od powtórzenia lub hasło jest puste");
                this.DialogResult = DialogResult.None;
                return;
            }
           
            try
            {
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {

                    Konfiguracja konf = context.Konfiguracja.FirstOrDefault();
                    konf.PfxPassword = Utils.Encrypt(pnew, EncryptPhase);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu " + ex.Message);
                this.DialogResult = DialogResult.None;
                return;
            }
            

        }

       
    }
}
