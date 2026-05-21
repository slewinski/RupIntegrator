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
    public partial class PozWyciagFilter : Form
    {
        public string RodzajRachunkuBankowego { get; set; }
        public string StatusRozliczenia { get; set; }
        public string TypPozycji { get; set; }
        public string dOd { get; set; }
        public string dDo { get; set; }



        public PozWyciagFilter()
        {
            InitializeComponent();
        }

        private void radLabel2_Click(object sender, EventArgs e)
        {

        }

        private void rbOK_Click(object sender, EventArgs e)
        {
            if (rbBank.IsChecked)
            {
                TypPozycji = "1";

            }
            else
            {
                TypPozycji = "2";
            }
            RodzajRachunkuBankowego = ((SAPSlownikRozlicz)rddlRachunek.SelectedValue).rodzaj.ToString();
            StatusRozliczenia = tbStatusRozliczenia.Text;
            if (String.IsNullOrWhiteSpace(StatusRozliczenia)) StatusRozliczenia = "1";
            dOd  = rdOD.Value.ToString("yyyyMMdd");
            dDo  = rdDO.Value.ToString("yyyyMMdd");
        }

        private void PozWyciagFilter_Load(object sender, EventArgs e)
        {
            rddlRachunek.SelectedIndex = 0;

            switch ((RunMode.data??"").Replace("/","").Trim().ToUpper())
            {
                case "WY":
                    rddlRachunek.SelectedValue = 2;
                    break;
                case "SZ":
                    rddlRachunek.SelectedValue = 3;
                    break;
                case "DO":
                    rddlRachunek.SelectedValue = 1;
                    break;
                case "FPP":
                    rddlRachunek.SelectedValue = 4;
                    break;
                default:
                    rddlRachunek.SelectedValue = 1;
                    break;
            }
            rdOD.Value = DateTime.Today;
            rdDO.Value = DateTime.Today;
            using (RupIntegratorEntities db = new RupIntegratorEntities())
            {
                List<SAPSlownikRozlicz> lst = db.SAPSlownikRozlicz.Where(a => a.kasabank == 1).OrderBy(a=>a.rodzaj).ToList();
                if (lst != null)
                {
                    rddlRachunek.DataSource = lst;
                    rddlRachunek.DisplayMember = "nazwa";
                    rddlRachunek.DataMember = "rodzaj";
                }
            
            }
        }

        private void rbBank_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            int kasabank = 1;

            if (rbBank.IsChecked)
            {
                kasabank = 1;
            }
            else {
                kasabank = 2;
            }
            using (RupIntegratorEntities db = new RupIntegratorEntities())
            {
                List<SAPSlownikRozlicz> lst = db.SAPSlownikRozlicz.Where(a => a.kasabank == kasabank).OrderBy(a => a.rodzaj).ToList();
                if (lst != null)
                {
                    rddlRachunek.DataSource = lst;
                    rddlRachunek.DisplayMember = "nazwa";
                    rddlRachunek.DataMember = "rodzaj";


                }

            }
            rddlRachunek.SelectedIndex = 0;
        }
    }
}
