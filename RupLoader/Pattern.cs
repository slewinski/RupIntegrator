using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace RupLoader
{
    public partial class Pattern : Form
    {
        public int Id { get; set; }
        private RL_Schemat rls = null;
        public Pattern()
        {
            InitializeComponent();
        }

        private void Pattern_Load(object sender, EventArgs e)
        {
            if (Id > 0)
            {
                rls = (from c in RupDatabase.theContext.RL_Schemat where c.Id == Id select c).FirstOrDefault();
                if (rls != null)
                {
                    tbWzorzec.Text = rls.wzorzec;
                    tbNazwa.Text = rls.nazwa;
                    tbPriorytet.Text = rls.priority.ToString();
                    tbDetail.Text = rls.detailsPattern;
                    tbNo.Text = rls.NextIfNo.ToString();
                    tbYes.Text = rls.NextIfYes.ToString();
                    rddlMode.SelectedIndex = rls.MatchMode == "R" ? 1 : 0;
                    tbKod.Text = rls.kod;
                }



            }
            else 
            { 
                tbNo.Text = "0";
                tbYes.Text = "0" ;
                tbPriorytet.Text = "0";
            }

        }

        private void rbOK_Click(object sender, EventArgs e)
        {
            if (rls == null) rls = new RL_Schemat();
            try 
                {
                    rls.wzorzec = tbWzorzec.Text ;
                    rls.nazwa = tbNazwa.Text ;
                    rls.priority = Convert.ToInt32(tbPriorytet.Text) ;
                    rls.detailsPattern = tbDetail.Text ;
                    rls.NextIfNo = Convert.ToInt32(tbNo.Text) ;
                    rls.NextIfYes = Convert.ToInt32(tbYes.Text) ;
                    rls.MatchMode =  rddlMode.SelectedIndex == 1 ? "R":"M";
                    rls.kod  = tbKod.Text ;
                    if (this.Id == null || this.Id ==  0) RupDatabase.theContext.RL_Schemat.AddObject(rls);
                    RupDatabase.theContext.SaveChanges();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            catch(Exception ex)
            {
                 MessageBox.Show("Błąd " + ex.Message + " " +  ((ex.InnerException == null) ? "": ex.InnerException.Message));
            
            }
        }

        private void btTest_Click(object sender, EventArgs e)
        {
            tbResult.Text = "";

            if (rddlMode.SelectedIndex == 1)
            {
                tbResult.Text = Regex.Replace(tbSample.Text, tbWzorzec.Text, tbDetail.Text);
            }
            else
            {
                RecognizeService recog = new RecognizeService();

                Regex r = new Regex(recog.interpreter(tbWzorzec.Text));
                Match m = r.Match(tbSample.Text);
            if (m.Success)
            {
                 tbResult.Text = "[Odnaleziony wzorzec:]\n" + m.Value;
                
                Regex r1 = new Regex(tbDetail.Text);
                Match m1 = r1.Match(m.Value);
                if (m1.Success)
                {
                     tbResult.Text += '\n'+"[Klucz wyszukiwania:]\n"+m1.Value;
                }
                else
                    tbResult.Text += "[Nie znaleziono wzortca wyszukiwania]";
            }
            else 
                tbResult.Text = "[Nie znaleziono wzortca w tekście]";
            }
            }
          
        

        
    }
}
