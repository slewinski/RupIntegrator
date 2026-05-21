using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RupBig
{
    public partial class SetPassword : Form
    {
        public SetPassword()
        {
            InitializeComponent();
        }
         
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void SetPassword_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;
            if (String.IsNullOrWhiteSpace(tbPass.Text))
            {
                MessageBox.Show("Hasło nie może byc puste");
                e.Cancel = true;
                return;
            }
            if (tbPass.Text != tbPass2.Text)
            {
                MessageBox.Show("Hasło nie zostało wprowadzone dwukrotnie identycznie");
                e.Cancel = true;
                return;
            }

        }
        }
    }
