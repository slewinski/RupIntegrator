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
    public partial class KuratNo : Form
    {

        public string kuratName { get; set; }
        public string kuratNo { get; set; }
        public string sygnatura { get; set; }
        public bool forceBreak { get; set; }
        public KuratNo()
        {
            InitializeComponent();
            forceBreak = false;
        }

        private void KuratNo_Load(object sender, EventArgs e)
        {
            tbSygn.Text = sygnatura;
            tbKurat.Text = kuratName;
        }

        private void rbOK_Click(object sender, EventArgs e)
        {
            kuratNo = tbSAPID.Text;
        }

        private void KuratNo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbSAPID.Text) && !forceBreak)
            {
                MessageBox.Show("Wprowadź numer osobowy kuratora");
                e.Cancel = true;

            }
        }

        private void rbCancel_Click(object sender, EventArgs e)
        {
            forceBreak = true;   
        }

    }




}
