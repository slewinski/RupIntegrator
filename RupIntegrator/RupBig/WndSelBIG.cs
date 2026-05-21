using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace RupBig
{
    public partial class WndSelBIG : Form
    {
        public List<BIG_Big> lstBIGI { get; set; }
        


        public WndSelBIG()
        {
            InitializeComponent();
        }

        private void WndSelBIG_Load(object sender, EventArgs e)
        {
            if (lstBIGI != null)
                this.rgvBIGs.DataSource = lstBIGI;
        }
    }
}
