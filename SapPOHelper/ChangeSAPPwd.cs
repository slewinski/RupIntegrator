using Ex2PscdInterface.Ex2PscdManageAccountOutService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SapPOHelper
{
    public partial class ChangeSAPPwd : Form
    {
        public string NewPassword { get; set; }
        
        public ChangeSAPPwd()
        {
            InitializeComponent();
        }

        private void rbOK_Click(object sender, EventArgs e)
        {
          
            string pnew = this.tbNewPassword.Text;
            string pnew2 = this.tbRepeatPwd.Text;


            if (pnew != pnew2)
            {
                tbMessage.Text ="Nowe hasło różne od powtórzenia" ;
               
                return;
            }
            if (pnew.Trim().Length < 1)
            {
                tbMessage.Text = "Nowe hasło nie może być puste ";
              
                return;
            }
            try
            {
                Komunikat message = ChngSAPPwd.SetNewPassword(pnew);
                if (!(message.RodzajKomunikatu == "S" && message.Komunikat1.ToLower().Contains("zmienio")))
                {
                    this.DialogResult = DialogResult.None;
                    this.tbMessage.Text = message.Komunikat1;
                    return;
                }
                else
                {
                    NewPassword = pnew;

                }
                
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
