using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RupLoader
{
    public partial class ErrorInfoRL : Form
    {
        public ErrorInfoRL()
        {
            InitializeComponent();
        }
        public string info { get; set; } 

        private void ErrorInfo_Load(object sender, EventArgs e)
        {
            this.tbDiagnostyka.Text = info;
        }
    }
}
