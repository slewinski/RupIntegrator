using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace RupLoader
{
    public partial class UsrManager : Form
    {
        public UsrManager()
        {
            InitializeComponent();
        }
        private void rbAddAccount_Click(object sender, EventArgs e)
        {
            UserAccount acc = new UserAccount();
            //tdl.dOd = DateTime.Today;
            //tdl.dDo = DateTime.Today;
            using (RupIntegratorEntities thecontext = new RupIntegratorEntities())
            {
                acc.Context = thecontext;
                acc.Id = 0;
                acc.ShowDialog();
                if (acc.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    
                    this.rgvUsers.DataSource = thecontext.User.ToList();
                }
            }
        }

        private void rgvUsers_Initialized(object sender, EventArgs e)
        {

            using (RupIntegratorEntities thecontext = new RupIntegratorEntities())
            {
                this.rgvUsers.DataSource = thecontext.User.ToList();
            }
        }

        private void rbManage_Click(object sender, EventArgs e)
        {
            UserAccount acc = new UserAccount();
            //tdl.dOd = DateTime.Today;
            //tdl.dDo = DateTime.Today;
            if (rgvUsers.SelectedRows.Count == 0) return;
            GridViewRowInfo therow = rgvUsers.SelectedRows[0];
            using (RupIntegratorEntities thecontext = new RupIntegratorEntities())
            {
                acc.Context = thecontext;
                acc.Id = Convert.ToInt32(therow.Cells["Id"].Value);
                acc.ShowDialog();
                if (acc.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.rgvUsers.DataSource = thecontext.User.ToList();
                }
            }
        }

        private void rbDeleteAcc_Click(object sender, EventArgs e)
        {
            if (rgvUsers.SelectedRows.Count == 0) return;
            GridViewRowInfo therow = rgvUsers.SelectedRows[0];
            int Id = Convert.ToInt32(therow.Cells["Id"].Value);
            bool isdel = Convert.ToBoolean(therow.Cells["deleted"].Value);
            if (isdel)
            {
                MessageBox.Show("To konto zostało już  usunięte");
                return;
            }
            if (MessageBox.Show("Czy na pewno chcesz usunąć wybrane konto ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                using (RupIntegratorEntities thecontext = new RupIntegratorEntities())
                {
                    User usr =  thecontext.User.Where(a => a.Id == Id).FirstOrDefault();
                    usr.deleted = true;
                    usr.DeleteDate = DateTime.Now;
                    thecontext.SaveChanges();
                    this.rgvUsers.DataSource = thecontext.User.ToList();
                }


            }

        }
    }
}
