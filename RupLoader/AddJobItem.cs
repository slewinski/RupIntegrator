using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
namespace RupLoader
{
    public partial class AddJobItem : Form
    {
        public AddJobItem()
        {
            InitializeComponent();
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                rddlJobItem.DataSource = context.RL_Konfig.Select(x => new { key = x.id, itemValue = x.sp_name + "/" + x.srvName + "/" + x.DbName + "/" + x.ERPLogon }).ToList();
                rddlJobItem.DisplayMember = "itemValue";
                rddlJobItem.ValueMember = "key";
            }
               

        }
    }
}
