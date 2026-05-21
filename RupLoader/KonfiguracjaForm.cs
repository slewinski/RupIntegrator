using ConsImport;
using Ex2PscdInterface.Ex2PscdPartnerQueryOutService;
using MessageSignature;
using SapPOHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace RupLoader
{
    public partial class KonfiguracjaForm : Form
    {

        BindingSource KonfigSource = new BindingSource();
        BindingSource KonfigSource1 = new BindingSource();

        public KonfiguracjaForm()
        {
            InitializeComponent();
        }
        private BindingSource KonfiguracjaDS = new BindingSource();
        private BindingSource WzorceDS = new BindingSource();
        private void rbAddAccount_Click(object sender, EventArgs e)
        {
            ConfigDB knf = new ConfigDB();
            if (knf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            { 
                KonfiguracjaDS.DataSource = RupDatabase.theContext.RL_Konfig.ToList();
                rgvConnection.DataSource = KonfiguracjaDS;
            
            }
        }

        private void InitKonfig()
        {
            this.KonfigSource.DataSource = RupDatabase.theContext.SAPSad.Where(a => a.typSad != "SF").ToList().OrderBy(a => a.miasto);
            this.rddlJedGosp.DataSource = this.KonfigSource;
            this.rddlJedGosp.DisplayMember = "miastSad";
            this.rddlJedGosp.ValueMember = "kod";

            this.KonfigSource1.DataSource = RupDatabase.theContext.SAPSad.Where(a => a.typSad == "SF" || a.kod == "").ToList().OrderBy(a => a.miasto);
            this.rddStanFin.DataSource = this.KonfigSource1;
            this.rddStanFin.DisplayMember = "miastSad";
            this.rddStanFin.ValueMember = "kod";


            this.rddlJedGosp.SelectedValue = RupDatabase.theConfig.JednostkaGospodarcza;
            this.rddStanFin.SelectedValue = RupDatabase.theConfig.StanowiskoFin;

         


            if (!String.IsNullOrEmpty(RupDatabase.theConfig.WSpwd))
            {
                this.tbPwdWS.Text = Utils.Decrypt(RupDatabase.theConfig.WSpwd, "Application error");

            }
            this.tbLoginWS.Text = RupDatabase.theConfig.WSLogon;
            this.rbPartner.Checked = RupDatabase.theConfig.czyautoks  == 1 ? true :  false ;
            this.cbKsNiep.Checked = RupDatabase.theConfig.czyautoprzyp == 1 ? true : false;
            this.tbDniHasla.Text = RupDatabase.theConfig.SAPPwdExpPeriod.ToString();

        }

        private void updateKonfig()
        {

            RupDatabase.theConfig.JednostkaGospodarcza = this.rddlJedGosp.SelectedValue == null ? "": this.rddlJedGosp.SelectedValue.ToString();
            RupDatabase.theConfig.StanowiskoFin = this.rddStanFin.SelectedValue == null ? "":  this.rddStanFin.SelectedValue.ToString();




            if (!String.IsNullOrEmpty(this.tbPwdWS.Text))
                RupDatabase.theConfig.WSpwd = Utils.Encrypt(this.tbPwdWS.Text, "Application error");
            else
                RupDatabase.theConfig.WSpwd = string.Empty;

            RupDatabase.theConfig.WSLogon = this.tbLoginWS.Text;
            try {
                RupDatabase.theConfig.SAPPwdExpPeriod = Convert.ToInt32(this.tbDniHasla.Text);

            }
            catch { }
            
           RupDatabase.theConfig.czyautoks =  this.rbPartner.Checked  ? 1 : 0;
           RupDatabase.theConfig.czyautoprzyp =  this.cbKsNiep.Checked  ? 1 : 0;
            foreach (GridViewRowInfo row in this.rgvMethods.Rows)
            {
                ServiceEndpoint se = (ServiceEndpoint)row.DataBoundItem;
                ServiceEndpoint sen = RupDatabase.theContext.ServiceEndpoint.Where(a => a.ServiceName == se.ServiceName).FirstOrDefault();
                if (sen != null)
                {
                    sen.Endpoint = se.Endpoint;
                }

            }
            RupDatabase.theContext.SaveChanges();

        }
        private void Konfiguracja_Load(object sender, EventArgs e)
        {
            KonfiguracjaDS.DataSource = RupDatabase.theContext.RL_Konfig.ToList();
            rgvConnection.DataSource = KonfiguracjaDS;

            WzorceDS.DataSource = RupDatabase.theContext.RL_Schemat.ToList();
            rgvPatterns.DataSource = WzorceDS;
          
             List<ServiceEndpoint> lst = RupDatabase.theContext.ServiceEndpoint.ToList();
             this.rgvMethods.DataSource = lst;

            InitKonfig();
        }

        private void rbManage_Click(object sender, EventArgs e)
        {
            if (rgvConnection.CurrentRow != null && rgvConnection.CurrentRow.Index >= 0 )
            {
            ConfigDB knf = new ConfigDB();
                knf.Id = rgvConnection.CurrentRow.Cells["id"].Value as  int? ?? default(int);
            if (knf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                KonfiguracjaDS.DataSource = RupDatabase.theContext.RL_Konfig.ToList();
                rgvConnection.DataSource = KonfiguracjaDS;

            }
           }
        }

        private void rgvConnection_DoubleClick(object sender, EventArgs e)
        {
            rbManage_Click(sender, e);
        }

        private void rbDeleteAcc_Click(object sender, EventArgs e)
        {
            if (rgvConnection.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Czy na  pewno chcasz usunąć wybrane połączenie ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        int id = rgvConnection.SelectedRows[0].Cells["id"].Value as int? ?? default(int);
                        if (id > 0)
                        {
                            RL_Konfig kn = RupDatabase.theContext.RL_Konfig.Where(a => a.id == id).FirstOrDefault();
                            if (kn != null) 
                            { 
                                RupDatabase.theContext.RL_Konfig.DeleteObject(kn); 
                                RupDatabase.theContext.SaveChanges();
                                KonfiguracjaDS.DataSource = RupDatabase.theContext.RL_Konfig.ToList();
                                rgvConnection.DataSource = KonfiguracjaDS;
                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd " + ex.Message + " " + ((ex.InnerException == null) ? "" : ex.InnerException.Message));
                    
                    }
                
                }
            }
        }

        private void rbAddPattern_Click(object sender, EventArgs e)
        {
            Pattern pt = new Pattern();
            if (pt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.WzorceDS.DataSource = RupDatabase.theContext.RL_Schemat.ToList();
                rgvPatterns.DataSource = this.WzorceDS;
            
            }
        }

        private void rbManagePattern_Click(object sender, EventArgs e)
        {
            if (rgvPatterns.SelectedRows.Count <= 0) return;
            Pattern pt = new Pattern();
            pt.Id = rgvPatterns.SelectedRows[0].Cells["Id"].Value as int? ?? default(int);

            if (pt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.WzorceDS.DataSource = RupDatabase.theContext.RL_Schemat.ToList();
                rgvPatterns.DataSource = this.WzorceDS;

            }
        }

        private void rbZapiszCnf_Click(object sender, EventArgs e)
        {
            updateKonfig();
        }

        private void setSAPConnectionParams(User u)
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

                List<ServiceEndpoint> l = context.ServiceEndpoint.ToList();
                if (l != null)
                    foreach (ServiceEndpoint s in l)
                    {
                        lst.Add(new KeyValuePair<string, string>(s.ServiceName, s.Endpoint));
                    }

                ZSRKRequestHelper.ServiceMapping = lst;
                ZSRKRequestHelper.AuthCert = new X509Certificate2(knf.Pfx, Utils.Decrypt(knf.PfxPassword, "Application error"));
                ZSRKRequestHelper.BasicAuthLogin = knf.WSLogon;
                ZSRKRequestHelper.BasicAuthPassword = knf.WSpwd;
                ZSRKRequestHelper.MEPUser = u.MEPUser;
                ZSRKRequestHelper.MEPPassword = Utils.Decrypt(u.MEPPassword, "Application error");
                ZSRKRequestHelper.ApplicationID = knf.AppName;
                ZSRKRequestHelper.JednostkaGospodarcza = knf.JednostkaGospodarcza;

                SignatureHelper.Password = Utils.Decrypt(u.MEPPassword, "Application error");
                SignatureHelper.SetCert(knf.Cer);

            }


           ;



        }

        private void setSAPConnectionParamsCons(User u)
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();

                List<ServiceEndpoint> l = context.ServiceEndpoint.ToList();
                if (l != null)
                    foreach (ServiceEndpoint s in l)
                    {
                        lst.Add(new KeyValuePair<string, string>(s.ServiceName, s.Endpoint));
                    }

                ConsWebServiceHelper.ServiceMapping = lst;
                ConsWebServiceHelper.AuthCert = new X509Certificate2(knf.Pfx, Utils.Decrypt(knf.PfxPassword, "Application error"));
                ConsWebServiceHelper.BasicAuthLogin = knf.WSLogon;
                ConsWebServiceHelper.BasicAuthPassword = knf.WSpwd;
                ConsWebServiceHelper.MEPUser = u.MEPUser;
                ConsWebServiceHelper.MEPPassword = Utils.Decrypt(u.MEPPassword, "Application error");
                ConsWebServiceHelper.ApplicationID = knf.AppName;
                ConsWebServiceHelper.JednostkaGospodarcza = knf.JednostkaGospodarcza;

                SignatureHelper.Password = Utils.Decrypt(u.MEPPassword, "Application error");
                SignatureHelper.SetCert(knf.Cer);

            }


           ;



        }

        private void rbTestWS_Click(object sender, EventArgs e)
        {

            
            User usr = null;
            SelectUser su = new SelectUser();
            if (su.ShowDialog() == DialogResult.OK)
            {
                usr = su.SelectedUser;
            }
            else return;
            updateKonfig();
            
            setSAPConnectionParams(usr);

            PartnerQuery arg = new PartnerQuery();
            arg.TypPartnera = "1";
            arg.PESEL = "94050395939";
            PartnerQueryRequest queryPartner = new PartnerQueryRequest();
            queryPartner.Partner = arg;
            try
            {
               
                PartnerQueryResponse resp = (PartnerQueryResponse)(ZSRKRequestHelper.CallSAPMethod("PartnerQueryOut", queryPartner));
                if (resp.Komunikaty != null && resp.Komunikaty.ToList().Count > 0)
                {
                    string s = string.Empty;
                    foreach (Ex2PscdInterface.Ex2PscdPartnerQueryOutService.Komunikat k in resp.Komunikaty)
                    {
                        s += "\n\r" + (k.IDKomunikatu + " " + k.NumerKomunikatu + " " + k.Komunikat1 + " " + k.RodzajKomunikatu).Trim();


                    }
                    if (!String.IsNullOrWhiteSpace(s))
                        MessageBox.Show( s, "Połączenie z systemem ZSRK przebiegło pomyślnie ");
                    else
                        MessageBox.Show("OK", "Połączenie z systeme ZSRK przebiegło pomyślnie");


                }
                else
                    MessageBox.Show(ZSRKRequestHelper.GetErrorMessage(), "Błąd podczas połączenia z Ex2PSCD");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ZSRKRequestHelper.GetErrorMessage() + ex.Message, "Błąd podczas próby połączenia z Ex2PSCD " );

            }

          
        }

        private void rbtSaveOther_Click(object sender, EventArgs e)
        {
            updateKonfig();
        }

        private void rbtTestPfx_Click(object sender, EventArgs e)
        {
            try
            {
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    Konfiguracja knf = context.Konfiguracja.FirstOrDefault();
                    var certificate = new X509Certificate2(knf.Pfx, Utils.Decrypt(knf.PfxPassword, GlobalStrings.APP_ERROR));


                }
            }
            catch (CryptographicException ex)
            {
                if ((ex.HResult & 0xFFFF) == 0x56)
                {
                    MessageBox.Show("Błędne hasło do certyfikatu *.pfx lub certyfikat nie został zaimportowany");
                    return;
                };


            }

            MessageBox.Show("Instalacja certyfikatu *.pfx poprawna");

        }

        private void btPfx_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "pfx (*.pfx)|*.pfx";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    byte[] pfx = File.ReadAllBytes(openFileDialog.FileName);
                    if (pfx != null)
                    {
                        using (RupIntegratorEntities context = new RupIntegratorEntities())
                        {
                            Konfiguracja konf = context.Konfiguracja.FirstOrDefault();
                            konf.Pfx = pfx;
                            context.SaveChanges();

                        }

                    }
                }
            }
        }

        private void rbtCer_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "cer (*.cer)|*.cer";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    byte[] pfx = File.ReadAllBytes(openFileDialog.FileName);
                    if (pfx != null)
                    {
                        using (RupIntegratorEntities context = new RupIntegratorEntities())
                        {
                            Konfiguracja konf = context.Konfiguracja.FirstOrDefault();
                            konf.Cer = pfx;
                            context.SaveChanges();

                        }

                    }
                }
            }
        }

     

        private void rbtPfxPassword_Click(object sender, EventArgs e)
        {

            SetPfxPwd chdlg = new SetPfxPwd();
            chdlg.ShowDialog();


        }

    }
}
