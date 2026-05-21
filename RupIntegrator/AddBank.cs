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
    public partial class AddBank : Form
    {
        public AddBank()
        {
            InitializeComponent();
        }

        public KnsMigratorEntities Context { get; set; }

        private void rbSave_Click(object sender, EventArgs e)
        {
            BankiKonfig b = new BankiKonfig();
            b.Label = this.tbBank.Text.Trim();
            b.Folder = this.tbFolder.Text.Trim();
            b.ExePath = this.tbFolder.Text.Trim();
            try
            {
                Context.BankiKonfig.AddObject(b);
                Context.SaveChanges();
            }
            catch (Exception ex )
            {
                MessageBox.Show(" Błąd zapisu " + ex.Message);
                DialogResult = DialogResult.None;
            }

        }
    }
}
