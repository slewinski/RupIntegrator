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
    public partial class DispResult : Form
    {
        public string SVal { get; set; }

        public DispResult()
        {
            InitializeComponent();

        }

        private void DispResult_Load(object sender, EventArgs e)
        {
            tbResult.Text = SVal;
        }
    }
}
