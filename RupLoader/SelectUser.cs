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
    public partial class SelectUser : Form
    {
        public User SelectedUser { get; set; }

        public SelectUser()
        {
            InitializeComponent();
        }

        private void SelectUser_Load(object sender, EventArgs e)
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                List<User> usrLst  = context.User.Where(a => a.deleted == false && a.suspend == false).ToList();
                this.rgvUsers.DataSource = usrLst;
                
            }
        }

        private void rbOK_Click(object sender, EventArgs e)
        {
            if (rgvUsers.SelectedRows.Count > 0)
            {

                User u = rgvUsers.SelectedRows.FirstOrDefault().DataBoundItem as User;
                SelectedUser = u;
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
