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
    public partial class WhoWnd : Form
    {

        public int docId { get; set; }

        public WhoWnd()
        {
            InitializeComponent();
        }

        private void WhoWnd_Load(object sender, EventArgs e)
        {
            string i1, s1, s2, s3;
            KnsMigratorEntities theContext = new KnsMigratorEntities();
            if (docId > 0)
            {
                Dokument d = theContext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.id == docId).FirstOrDefault();
                if (d != null)
                {
                    i1 = d.InsertedBy + " " + (d.InsDate != null ? d.InsDate.ToString():"");
                    s1 = "Dokument: " + d.SentBy + " " + (d.SentDate != null ? d.SentDate.ToString() : "");
                    s2 = "Konto umowy: " + d.Sprawa.SentBy + " " + (d.Sprawa.SentDate != null ? d.Sprawa.SentDate.ToString() : "");
                    s3 = "Partner: " + d.Dluznik.SentBy + " " + (d.Dluznik.SentDate != null ? d.Dluznik.SentDate.ToString() : "");
                    lbimport.Text = i1;
                    lbprzekaz1.Text = s1;
                    lbprzekaz2.Text = s2;
                    lbprzekaz3.Text = s3;
            
            
            }
            
            }
        }

    }
}
