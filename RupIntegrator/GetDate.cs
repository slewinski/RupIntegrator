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
     

    public partial class GetDate : Form
    {
        public DateTime theDay{get; set;}
        public bool leaveUnchanged { get; set; }
        public GetDate()
        {
            InitializeComponent();
        }

        private void GetDate_Load(object sender, EventArgs e)
        {
            if (theDay > new DateTime(2000-01-01) )
            {
                this.dtPicker.Value = theDay;
                this.lbPromt.Text = "Podaj datę księgowania";
                this.dtPicker.Enabled = false;
            }
            else
            {
                cbLeave.Visible = false;
                this.dtPicker.Value = DateTime.Today;            
            }

        }

        private void bt_OK_Click(object sender, EventArgs e)
        {
            theDay = this.dtPicker.Value;
            leaveUnchanged = cbLeave.Checked;
        }

        private void cbLeave_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLeave.CheckState == CheckState.Unchecked)
                this.dtPicker.Enabled = true;
            else
                this.dtPicker.Enabled = false;
        }

       
    }
}
