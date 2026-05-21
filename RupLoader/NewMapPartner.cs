using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RupLoader
{
    public partial class NewMapPartner : Form
    {
        public int TypPartner { get; set; }
        public int IdPartner  { get; set; }
        public RupIntegratorEntities rue { get; set; }
        public NewMapPartner()
        {
            InitializeComponent();
            rddlSlowTyp.DataSource = Utils.naleznosci;
            rddlSlowTyp.DisplayMember = "nazwa";
            rddlSlowTyp.ValueMember = "nr";
            rddlFizPraw.SelectedIndex = 0;
        }

        private int getSlowType()
        {
            if (rddlSlowTyp.SelectedItems.Any())
            {

                typPartner tp = (typPartner)(rddlSlowTyp.SelectedItem.DataBoundItem);
                return tp.nr;
            }

            return 0;
        }

        private void NewMapPartner_Load(object sender, EventArgs e)
        {
            if (IdPartner > 0)
            {
                MapPartner mp = rue.MapPartner.Where(a => a.Id == IdPartner).FirstOrDefault();
                if (mp != null)
                    this.setupForm(mp);
                else
                {

                    MessageBox.Show("Brak wybranego partnera");
                    this.Close();
                }

            }
            else
                if (TypPartner > 0)
                {
                    rddlSlowTyp.SelectedValue = TypPartner;

                }
                else
                    rddlSlowTyp.SelectedIndex = 1;

        }
        private void setupStructPartner(ref MapPartner mp)
        {
            Int64 tmp = 0 ;
            mp.dmodyf = DateTime.Now;
            if (Int64.TryParse( this.tbIdDanych.Text,out tmp))
             mp.IdDanych = tmp;
            mp.Imie = tbImie.Text;
            mp.Nazwisko = tbNazwisko.Text;
            mp.SAPPartner = tbSAPPartner.Text;
            mp.NIP = tbNIP.Text;
            mp.Pesel = tbPesel.Text;
            mp.Ulica = tbUlica.Text;
            mp.Miejscowosc = tbMiejsce.Text;
            mp.Kod = tbKod.Text;
            mp.KU_DO = tbKU_DO.Text;
            mp.KU_SZ = tbKU_SZ.Text;
            mp.KU_WY = tbKU_WY.Text;
            mp.nrDom = tbDom.Text;
            mp.typSlow = getSlowType();
            mp.fizpraw = rddlFizPraw.SelectedIndex;

        }

        private void setupForm(MapPartner mp)
        {

            
           
            if ( mp.IdDanych != null && mp.IdDanych > 0 )
                this.tbIdDanych.Text = mp.IdDanych.ToString();

            tbImie.Text = mp.Imie;
            tbNazwisko.Text= mp.Nazwisko  ;
            tbSAPPartner.Text = mp.SAPPartner;
            tbNIP.Text = mp.NIP  ;
            tbPesel.Text = mp.Pesel;
            tbUlica.Text = mp.Ulica;
            tbMiejsce.Text = mp.Miejscowosc;
            tbKod.Text = mp.Kod;
            tbKU_DO.Text = mp.KU_DO;
            tbKU_SZ.Text = mp.KU_SZ;
            tbKU_WY.Text = mp.KU_WY;
            tbDom.Text = mp.nrDom;
            rddlSlowTyp.SelectedValue = mp.typSlow; 
            rddlFizPraw.SelectedIndex  = mp.fizpraw?? 0 ;
        
        }



        private void rbOK_Click(object sender, EventArgs e)
        {
            // OK
            MapPartner mp;
            try
            {
                if (IdPartner > 0)
                {

                  mp = rue.MapPartner.Where(a=>a.Id == IdPartner).FirstOrDefault();
                  if (mp != null)
                  {
                      this.setupStructPartner(ref mp);
                      rue.SaveChanges();
                  }
                  else
                      MessageBox.Show("Błąd odczytu danych partnera");
                }
                else
                {

                    mp = new MapPartner();
                    mp.dcreate = DateTime.Now;
                    this.setupStructPartner(ref mp);
                    rue.MapPartner.AddObject(mp);
                    rue.SaveChanges();
                    this.IdPartner = mp.Id;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd podczas zapisu partnera  " + ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));
                
            }
        }

        
    }
}
