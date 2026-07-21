using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Export;
using Telerik.WinControls.Export;
using System.IO;
using Telerik.WinControls.UI.Localization;
using BackgroundWorkerDemo;
using SapPOHelper;
using System.Security.Cryptography.X509Certificates;
using MessageSignature;
using Ex2PscdInterface.Ex2PscdContractObjectCreateOutService;

namespace RupLoader
{
    public partial class RyczaltyKuratorskie : Form
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // private SapPIHelper sapPi;

        private BindingSource konfigDataSource = new BindingSource();

        private void setSAPConnectionParams()
        {
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                User usr = context.User.Where(a => a.Id == UserInfo.Id).FirstOrDefault();
                setSAPConnectionParams(usr);
            }
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
        }


        public RyczaltyKuratorskie()
        {

            string appDir = Path.GetDirectoryName(Application.ExecutablePath);
            setSAPConnectionParams();
            InitializeComponent();
            switch  ( RupDatabase.typPartner )
            {
                case 1: this.Text = "Rozrachunki z biegłymi";
                        break;
                case 2: this.Text = "Rekompensaty i ryczłaty ławników";
                    break;

             }
              setSAPConnectionParams();
            log.Debug("Ryczałty kuratorskie uruchomione");

            RadGridLocalizationProvider.CurrentProvider = new PolishRadGridLocalizationProvider();
          
            if (File.Exists(appDir + "\\" + "ryczwidok.lyt"))
                this.rgvView.LoadLayout(appDir + "\\" + "ryczwidok.lyt");
            if (File.Exists(appDir + "\\" + "ryczwynik.lyt"))
                this.rgvResult.LoadLayout(appDir + "\\" + "ryczwynik.lyt");

            if (this.rgvResult.SummaryRowsTop.Count == 0)
            {
                GridViewSummaryItem summaryItemKwota = new GridViewSummaryItem("Kwota", "{0}", GridAggregateFunction.Sum);
                GridViewSummaryItem summaryItemStatus = new GridViewSummaryItem("NumerKuratora", "{0}", GridAggregateFunction.Count);

                GridViewSummaryRowItem summaryRowItem = new GridViewSummaryRowItem(new GridViewSummaryItem[] { summaryItemStatus, summaryItemKwota });

                this.rgvResult.SummaryRowsTop.Add(summaryRowItem);
            }
            //this.rgvResult.SummaryRowsTop.Count

            if (this.rgvView.SummaryRowsTop.Count == 0)
            {
                GridViewSummaryItem summaryItemKwota2 = new GridViewSummaryItem("Kwota", "{0}", GridAggregateFunction.Sum);
                GridViewSummaryItem summaryItemStatus2 = new GridViewSummaryItem("msg", "{0}", GridAggregateFunction.Count);

                GridViewSummaryRowItem summaryRowItem2 = new GridViewSummaryRowItem(new GridViewSummaryItem[] { summaryItemStatus2, summaryItemKwota2 });

                this.rgvView.SummaryRowsTop.Add(summaryRowItem2);
            }

           
        }

        public class cnf
        {
            public bool tn { get; set; }
            public string ERPLogon { get; set; }
            public int id { get; set; }
        }
        private List<cnf> konflst;
        private void RyczaltyKuratorskie_Load(object sender, EventArgs e)
        {
            DateTime prevMon = DateTime.Today.AddDays(-28);
            rdtDo.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            rdtOd.Value = prevMon;
            List<RL_Konfig> rcnf = new List<RL_Konfig>();
            RupIntegratorEntities ent = new RupIntegratorEntities();

            konflst = new List<cnf>();



            rcnf = ent.RL_Konfig.Where(a => a.rodzajDB > 1).ToList();
            foreach (RL_Konfig r in rcnf)
            {
                cnf c = new cnf();
                c.tn = true;
                c.id = r.id;
                c.ERPLogon = r.ERPLogon;
                konflst.Add(c);
            
            }


            rgvConn.DataSource = konflst;

            rdtWazneOd.Value = DateTime.Today;
            rdtWazneDo.Value = DateTime.Today;
            rdtDataPlatnosci.Value  = DateTime.Today;
            rdtWplyw.Value = DateTime.Today;
            rdtZarzadz.Value = DateTime.Today;
            this.cbOkres.Text = DateTime.Today.Month.ToString("D2");
        }


     
        private string combineOptions()
        {
            string option = "";
            if (this.rcbDozory.Checked == true)
                option += "D";
            if (this.rcbNadzor.Checked == true)
                option += "N";
            if (this.rcbWywiad.Checked == true)
                option += "W";
            option += tbInne.Text.ToUpper();
            return option;

        }

        private void rbRun_Click(object sender, EventArgs e)
        {
            string errCode = "";
            RyczaltyService rc = new RyczaltyService();
            string filter = this.combineOptions();
            List<rStruct> dsLst;
            this.labelResult.Text = "Odczyt ryczałtów... ";
            this.labelResult.Refresh();
            foreach (GridViewRowInfo dr in rgvConn.Rows)
            {
                if ((bool)dr.Cells["tn"].Value == true)
                {

                    errCode = rc.GetRyczaltyByDB((int)dr.Cells["Id"].Value, rdtOd.Value , rdtDo.Value, filter, ref this.labelResult);
                    if (!String.IsNullOrWhiteSpace(errCode))
                    {
                        MessageBox.Show(errCode);
                        return;
                    }



                }

            }
            dsLst = rc.GetAallList();
            this.rgvView.DataSource = dsLst;
            this.labelResult.Text = "Odczyt ryczałtów zakończona pomyślnie ";
            this.labelResult.Refresh();

            if (this.rgvView.Columns["status"].ConditionalFormattingObjectList.Count == 0)
            {

                ExpressionFormattingObject obj = new ExpressionFormattingObject("Cond1", "status = 0", false);
                obj.CellBackColor = Color.LightGray;
                obj.CellForeColor = Color.Black;
                this.rgvView.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond2", "status < 0 AND status > -1000", false);
                obj.CellBackColor = Color.Red;
                obj.CellForeColor = Color.Black;
                this.rgvView.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond3", "status > 0 ", false);
                obj.CellBackColor = Color.Green;
                obj.CellForeColor = Color.Black;
                this.rgvView.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond4", "status = -1000 ", false);
                obj.CellBackColor = Color.Yellow;
                obj.CellForeColor = Color.Black;
                this.rgvView.Columns["status"].ConditionalFormattingObjectList.Add(obj);
            }
        }

        private void rbCheck_Click(object sender, EventArgs e)
        {
            List<rStruct> lst = this.rgvView.DataSource as List<rStruct>;
            rStruct therow = null;
             bool czyblad = false;
              string mojeJG;
            

            using (RupIntegratorEntities dbContext = new RupIntegratorEntities())
            {
                this.labelResult.Text = "Weryfikacja sądów orzekających ";
                this.labelResult.Refresh();
                Konfiguracja konf = dbContext.Konfiguracja.FirstOrDefault();
                mojeJG = (String.IsNullOrWhiteSpace(konf.StanowiskoFin) ? konf.JednostkaGospodarcza.Trim() : konf.StanowiskoFin);
            
                foreach (rStruct r in lst)
                {
                    // sprawdzenie czy jest numer sapowy
                    this.labelResult.Text = "Weryfikacja sądów orzekających dla " + r.Sygnatura;
                    this.labelResult.Refresh();
                    therow = r;
                    if (r.IdSadOrzek <= 0) continue;
                    KuratSad ks = dbContext.KuratSad.Where(a => a.dbname == r.SygnDbName && a.srvname == r.SygnSrvName && a.Sad_Id == r.IdSadOrzek).FirstOrDefault();
                    if (ks != null && !String.IsNullOrWhiteSpace(ks.SAPSad_Id))
                        r.SapSad = ks.SAPSad_Id;
                    else
                    {//pobierz sąd
                        MapSadKurat mps = new MapSadKurat();
                        mps.sadKura = r.SadOrzek;
                        mps.sygnatura = r.Sygnatura;
                        if (mps.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (!String.IsNullOrWhiteSpace(mps.sadSAP))
                            {
                                r.SapSad = mps.sadSAP.Substring(0, 4);
                               
                                if (String.IsNullOrWhiteSpace(r.SapSad)) continue;
                                ks = new KuratSad();
                                r.SadOrzek = r.SadOrzek.Trim();
                                ks.Nazwa = r.SadOrzek.Length > 100 ?  r.SadOrzek.Substring(0,99) : r.SadOrzek;
                                ks.Sad_Id = r.IdSadOrzek;
                                ks.SAPSad_Id = r.SapSad;
                                ks.dbname = r.SygnDbName;
                                ks.srvname = r.SygnSrvName;
                                try
                                {
                                    dbContext.KuratSad.AddObject(ks);
                                    dbContext.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));

                                }
                                   
                                // uzupełnie pozostalych 
                                foreach (rStruct y in lst)
                                {
                                    if (String.IsNullOrWhiteSpace(y.SapSad) && y.IdSadOrzek == r.IdSadOrzek)
                                        y.SapSad = r.SapSad;

                                }
                            }
                            else
                                break;

                        }
                        else
                            return; 
                        
                    }


                }
                // weryfikacja kuratorów

                // mapowanie kuratorów

                this.rgvView.MasterTemplate.Refresh();
                this.labelResult.Text = "Weryfikacja partnerów";
                this.labelResult.Refresh();
                try
                {
                    foreach (rStruct r in lst)
                    {
                        therow = r;
                        this.labelResult.Text = "Weryfikacja partnerów: " + r.ImieNazwisko + " " + r.Sygnatura;
                        this.labelResult.Refresh();
                        KuratMap kur = dbContext.KuratMap.Where(a => a.DbId == r.IdKuratora && a.typPartner == RupDatabase.typPartner && a.servername == r.SygnSrvName && a.dbname == r.SygnDbName).FirstOrDefault();
                        if (kur != null)
                        {
                            r.NumerKuratora = kur.SAPId;
                            this.labelResult.Text = "Odczyt numeru osobowego z bazy: " + r.ImieNazwisko + " " + r.Sygnatura + " " + kur.SAPId;
                            this.labelResult.Refresh();

                        }
                        else
                        {
                            KuratNo kn = new KuratNo();
                            kn.kuratName = r.ImieNazwisko;
                            kn.sygnatura = r.Sygnatura;
                            if (kn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                if (!String.IsNullOrWhiteSpace(kn.kuratNo))
                                {
                                    r.NumerKuratora = kn.kuratNo;
                                    KuratMap km = new KuratMap();
                                    km.Nazwa = r.ImieNazwisko;
                                    km.DbId = r.IdKuratora;
                                    km.SAPId = r.NumerKuratora;
                                    km.servername = r.SygnSrvName;
                                    km.dbname = r.SygnDbName;
                                    km.typPartner = RupDatabase.typPartner;
                                    km.czyPSCD = 1;
                                    km.czyVAT = 0; 
                                    dbContext.KuratMap.AddObject(km);
                                    dbContext.SaveChanges();
                                    foreach (rStruct y in lst)
                                    {
                                        if (String.IsNullOrWhiteSpace(y.NumerKuratora) && y.IdKuratora == r.IdKuratora)
                                            y.NumerKuratora = r.NumerKuratora;

                                    }
                                }
                                else
                                    break;

                            }
                            else
                                return;
                        }

                    }

                }
                catch (Exception ex)
                {
                    log.Error("Błąd walidacji partnerów " + therow.Sygnatura, ex);
                    MessageBox.Show(ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "") + therow != null ? " " + therow.Sygnatura : "");
                    return;

                }
                
                this.rgvView.MasterTemplate.Refresh();
                this.labelResult.Text = "";
                this.labelResult.Refresh();

                int step = 0;



                try
                {
                    czyblad = false;   // weyfikacja  sygnatur

                    //startProgressWindow();
                    this.labelResult.Text = "Weryfikacja sygnatur... ";
                    this.labelResult.Refresh();
                    foreach (rStruct r in lst)
                    {
                        int rok;
                        step = 0;
                        int nr;
                        string repOryg;
                        string wydzial, repert;
                        string ans;
                        string outsad = "";
                        therow = r;
                        this.labelResult.Text = "Weryfikacja sygnatury " + r.Sygnatura;
                        this.labelResult.Refresh();

                        if (!String.IsNullOrWhiteSpace(r.SRepertorium) && !String.IsNullOrWhiteSpace(r.SWydzial) && !String.IsNullOrWhiteSpace(r.SNumer) && !String.IsNullOrWhiteSpace(r.SRok) ) continue;

                        if (cbObce.Checked && !String.IsNullOrWhiteSpace(r.SapSad) && !String.IsNullOrWhiteSpace(r.Sygnatura) && r.SapSad != mojeJG)   // jeśli nielusta i obca 
                        {
                            r.Sygnatura = Utils.getTechSygn(r.IdCofDB);
                            if (!String.IsNullOrWhiteSpace(r.Sygnatura) && r.Sygnatura.Length > 5 && r.Sygnatura.Substring(4, 1) == " ")
                                r.Sygnatura = r.Sygnatura.Substring(4).Trim();
                            r.SapSad = mojeJG;
                        }
                       /*
                        else
                            if (cbEmpty.Checked && (String.IsNullOrWhiteSpace(r.Sygnatura) || r.status == -1))
                                outr.Sygnatura = getTechSygn(r.IdCofDB);
                            else
                                outr.Sygnatura = r.SapSad + " " + r.SWydzial + r.SRepertorium + " " + r.SNumer + "/" + (!String.IsNullOrWhiteSpace(r.SRok) && r.SRok.Trim().Length >= 4 ? r.SRok.Substring(2, 2) : "");
                
                        */
                        
                        r.msg = "";
                        r.status = 0;
                        step = 1;
                        ans = Utils.ParseSygn(String.IsNullOrWhiteSpace( r.Sygnatura ) ? "" : r.Sygnatura.ToUpper()  , out wydzial, out repert, out nr, out rok, out repOryg, out outsad,r.SapSad);
                        step = 111;
                        if (!String.IsNullOrWhiteSpace(ans))
                        {
                            r.msg += ans;
                            r.status = -1;
                            czyblad = true;
                        }
                        else
                        {
                            r.SWydzial = wydzial;
                            r.SRepertorium = repert;
                            r.SNumer = nr.ToString();
                            r.SRok = rok.ToString();
                            if (rok <= 0 ) 
                            {
                              r.msg += " Rok sprawy nie może być zerowy ";
                             r.status = -1;
                             czyblad = true;
                            }
                            if (!String.IsNullOrWhiteSpace(outsad)) r.SapSad = outsad;
                            step = 11;
                            SAPRepertorium rep = dbContext.SAPRepertorium.Where(a => a.kod == repert).FirstOrDefault();
                            if (rep == null)
                            {
                                r.msg += "W słowniku brak takiego repertorium - rodzaju przedmiotu";
                                r.status = -1;
                                czyblad = true;
                            }
                            else
                                r.SRodzajPrzedm = rep.SymbolRodzajPrzedmiotu;
                            step = 2;
                            string typsad = "SR";
                            if (String.IsNullOrWhiteSpace(r.SapSad))
                            {
                                r.msg += "Sąd wymaga wyboru ze słownika w systemie kuratorskim";
                                r.status = -1;
                                czyblad = true;
                            }
                            else
                                switch (r.SapSad.Substring(0, 1).ToUpper())
                            {
                                case "5":
                                case "4":
                                    typsad = "SR";
                                    break;
                                case "3":
                                    typsad = "SO";
                                    break;
                                case "2":
                                    typsad = "SA";
                                    break;
                                default:
                                    break;
                            }
                            step = 3;
                            SAPRodzajSprawy sps = dbContext.SAPRodzajSprawy.Where(a => a.repertorium == repert && a.typSad == typsad).FirstOrDefault();
                            step = 5;
                            if (sps == null)
                            {
                                r.msg += "W słowniku brak takiego rodzaju sprawy";
                                r.status = -1;
                                czyblad = true;
                            }
                            else
                                r.SRodzaj = sps.kod;
                        }
                        step = 99;
                        if (r.status != -1) r.status = 1;
                    }

                    if (czyblad == true)
                    {
                        MessageBox.Show("Walidacja sygnatur zakończyła się błędem. Poprawne pozycje zostaną zweryfikowane w ZSRK"); //return;
                    }
                   
                    this.rgvView.MasterTemplate.Refresh();
                }
                
                catch (Exception ex)
                {
                    log.Error("Błąd walidacji sygnatur " + step.ToString() + " " + therow.Sygnatura, ex);
                    MessageBox.Show("Błąd walidacji sygnatur " + step.ToString());
                    MessageBox.Show(ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "") + (therow != null ? " " + therow.Sygnatura : "") );
                    return;

                }

                this.rgvView.MasterTemplate.Refresh();
                Cursor.Current = Cursors.WaitCursor;



        

                Konfiguracja knf = RupDatabase.theContext.Konfiguracja.FirstOrDefault();
                try
                {
              
                    this.labelResult.Text = "Weryfikacja sygnatury w systemie ZSRK..." ;
                    this.labelResult.Refresh();


                    //weryfikacja sygnatur wde SAP'ie i ew założenie;
                    foreach (rStruct r in lst)
                    {
                        
                        Double nop = 0 ;
                        if (!String.IsNullOrWhiteSpace(r.msg) && r.msg.Length > 15 && r.msg.Length < 23 && Double.TryParse(r.msg, out nop)) continue;
                        this.labelResult.Text = "Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura;
                        this.labelResult.Refresh();
                       
                        therow = r;

                        SygnaturaTworzenie sygnqry = Utils.setupSygnStruct(r, knf);
                        if (sygnqry == null) {r.status = -1;  continue; }
                        string Przedmiotumowy = Utils.verifySygnatura(sygnqry);
                        if (!String.IsNullOrWhiteSpace(Przedmiotumowy))
                        {
                            this.labelResult.Text = "Odczyt sygnatury lokalnie " + r.Sygnatura + " OK nr przedmiotu" + Przedmiotumowy;
                            this.labelResult.Refresh();
                            r.msg += Przedmiotumowy;
                            r.status = 1;
                            continue;
                        }

                        ContractObjectCreateResponse anssygn = ZSRKRequestHelper.ZalozSygnature(sygnqry);
                        if (anssygn != null)
                        {
                            if (anssygn.Sygnatura != null)
                            {
                                if (String.IsNullOrWhiteSpace(anssygn.Sygnatura.IDPrzedmiotuUmowy))
                                {
                                    r.msg += "Błąd podczas zakładania/wyszukiwania sygnatury ";
                                    if (anssygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0)
                                    {
                                        r.msg =  anssygn.Komunikaty[0].Komunikat1 + r.msg;
                                    }
                                    r.status = -1;
                                    czyblad = true;

                                }
                                else
                                {
                                    this.labelResult.Text = "Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura + " OK nr przedmiotu" + anssygn.Sygnatura.IDPrzedmiotuUmowy;
                                    this.labelResult.Refresh();
                                    r.msg += anssygn.Sygnatura.IDPrzedmiotuUmowy;
                                    r.status = 1;
                                    Utils.addSygnatura(sygnqry, r.Sygnatura, anssygn.Sygnatura.IDPrzedmiotuUmowy);
                                }



                            }

                            else
                            {
                                if (anssygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0 && anssygn.Komunikaty[0].RodzajKomunikatu == "E")
                                {
                                    r.msg =   anssygn.Komunikaty[0].Komunikat1 + r.msg;
                                    this.labelResult.Text = "Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura + " Błąd " + anssygn.Komunikaty[0].Komunikat1;
                                    this.labelResult.Refresh();
                                    r.status = -1;
                                    czyblad = true;
                                }

                            }

                        }
                        else
                        {
                            if (MessageBox.Show("Błąd działania usługi sieciowej, nie można sprawdzić sygnatury w ZSRK " + r.Sygnatura + ", czy kontynuować sprawdzenie ?", "Zdecyduj", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                            {
                                continue;
                                czyblad = true;
                            }
                            else
                                break;
                                //return;
                            
                        }


                    }
                    if (czyblad == true)
                    {
                        MessageBox.Show("Sprawdzenie sygnatur w ZSRK zakończyła się błędem"); return;
                    }
                
                } // try
                catch (Exception ex)
                {
                    log.Error("Błąd walidacji sygnatur w ZSRK " + therow.Sygnatura, ex);
                    MessageBox.Show(ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "") + (therow != null ?   " " +therow.Sygnatura :""));
                    return;

                }

            }
        }
      

        private void rbExport_Click(object sender, EventArgs e)
        {
            bool sygnreplace = false;
            string mojeJG;
            List<rStruct> lst = this.rgvView.DataSource as List<rStruct>;
            List<outStruct > outlst = new List<outStruct>();
            rStruct ptr = null;
            // parsowanie 
            rgvResult.Rows.Clear();
            using (RupIntegratorEntities dbContext = new RupIntegratorEntities())
            {
                Konfiguracja knf = dbContext.Konfiguracja.FirstOrDefault();
                mojeJG = (String.IsNullOrWhiteSpace(knf.StanowiskoFin) ? knf.JednostkaGospodarcza.Trim() : knf.StanowiskoFin);
                
            
            }

            /*
            if (MessageBox.Show("Czy zamienić sygnatury sądów obcych na techniczne ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            { 
                // 
                sygnreplace = true; 
                
            }
             * */

            try
            {
                foreach (rStruct r in lst)
                {
                    ptr = r;
                    outStruct outr = new outStruct();
                    outr.ImieNazwisko = r.ImieNazwisko;
                    outr.NumerKuratora = r.NumerKuratora;
                    outr.WazneOd = this.rdtWazneOd.Value.ToString("dd.MM.yyyy");
                    outr.WazneDo = this.rdtWazneDo.Value.ToString("dd.MM.yyyy");
                    outr.OkresWymag = this.cbOkres.Text; 
                    if (cbObce.Checked && !String.IsNullOrWhiteSpace(r.SapSad) && !String.IsNullOrWhiteSpace(r.Sygnatura) && r.SapSad != mojeJG)   // jeśli nielusta i obca 
                        outr.Sygnatura = Utils.getTechSygn(r.IdCofDB);
                    else
                        if ( cbEmpty.Checked && (  String.IsNullOrWhiteSpace(r.Sygnatura) || r.status == -1 ) )
                            outr.Sygnatura = Utils.getTechSygn(r.IdCofDB);
                        else
                            outr.Sygnatura = r.SapSad + " " + r.SWydzial + r.SRepertorium + " " + r.SNumer + "/" +  (!String.IsNullOrWhiteSpace(r.SRok) && r.SRok.Trim().Length>=4 ?    r.SRok.Substring(2, 2) : "");
                    int ind = outr.Sygnatura.LastIndexOf('/');
                    string year;
                    if (ind > 0)
                    {
                        year = outr.Sygnatura.Substring(ind + 1).Trim();
                        if (year.Length == 4)
                        {
                            year = year.Substring(2);
                            outr.Sygnatura = outr.Sygnatura.Substring(0, ind +1) + year;
                        
                        }

                    }
                    outr.NrRachunku = r.NrRachunku;
                    outr.DataWplZarz = r.DataWplZarz.Value.ToString("dd.MM.yyyy");
                    outr.DataPlatnosci = this.rdtDataPlatnosci.Value.ToString("dd.MM.yyyy");
                    outr.DataWydZarz = (r.DataWydZarz == null || (r.DataWydZarz != null  && r.DataWydZarz.Value < new DateTime(2010,1,1))?  this.rdtZarzadz.Value.ToString("dd.MM.yyyy") : r.DataWydZarz.Value.ToString("dd.MM.yyyy")) ;
                    outr.DataWplZarz = (r.DataWplZarz == null || (r.DataWplZarz != null && r.DataWplZarz.Value < new DateTime(2010, 1, 1)) ? this.rdtWplyw.Value.ToString("dd.MM.yyyy") : r.DataWplZarz.Value.ToString("dd.MM.yyyy"));
                    outr.PowoDodRozl = r.PowoDodRozl;
                    outr.TypRozl = r.TypRozl;
                    outr.IdListyPlac = r.IdListyPlac;
                    outr.Skladnik = r.Skladnik;
                    outr.Kwota = r.Kwota > 0 ? r.Kwota : 0;
                    outr.ZwKosztDojSkladnik = r.ZwKosztDojSkladnik > 0 ? r.ZwKosztDojSkladnik.ToString("#.##") : "";
                    outr.ZwKosztDojKWSkladnik = r.ZwKosztDojKWSkladnik > 0 ? r.ZwKosztDojKWSkladnik.ToString("#.##") : "";
                    outr.LWywiadow = r.LWywiadow > 0 ? r.LWywiadow.ToString() : "";
                    outr.LNadzorow = r.LNadzorow > 0 ? r.LNadzorow.ToString() : "";
                    outr.WywiadDaneOsob = r.WywiadDaneOsob;
                    outr.Uwagi = r.Uwagi;
                    outr.StatusDokumentu = r.StatusDokumentu;
                    outr.WydatekIncydantalny = r.WydatekIncydantalny > 0 ? r.WydatekIncydantalny.ToString() : "";
                    // Nowe
                    outr.RodzWypl = r.RodzWypl;
                    outr.PotracZaliczki = r.PotracZaliczki;
                    outr.ZwrotKosztKwt2 = r.ZwrotKosztKwt2;
                    outr.ZwrotKosztSkladnik2 = r.ZwrotKosztSkladnik2;
                    outr.ProcDofin = r.ProcDofin;
                    

                    outlst.Add(outr);
                }

                rgvResult.DataSource = outlst;
            }
            catch (Exception ex)
            {
                log.Error("Błąd eksportu do Excela " + ptr.Sygnatura, ex);
                MessageBox.Show(ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "") + " Sygn:" + ptr.Sygnatura);
                return;

            }
        string filename = "";    // FileOpen
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "XLSX (*.xlsx)|*.xlsx";
            
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            if (!saveFileDialog.FileName.Equals(String.Empty))
            {
                filename = saveFileDialog.FileName;
            }
        }
            if ( String.IsNullOrWhiteSpace(filename)) return;
            // export do Excela'
            try
            {
                GridViewSpreadExport spreadExporter = new GridViewSpreadExport(this.rgvResult, SpreadExportFormat.Xlsx);
                spreadExporter.ExportChildRowsGrouped = false;
                spreadExporter.HiddenColumnOption = HiddenOption.DoNotExport;
                spreadExporter.SummariesExportOption = SummariesOption.DoNotExport;

                SpreadExportRenderer exportRenderer = new SpreadExportRenderer();
                spreadExporter.RunExport(filename, exportRenderer);
            }
            catch (Exception ex)
            {
                                log.Error("Błąd eksportu do Excela " + ptr.Sygnatura, ex);
                MessageBox.Show("Bład podczas ekzportu do Ms Excel: " + ex.Message);

            }
        }

        private void tbSaveLayout_Click(object sender, EventArgs e)
        {
            string appDir = Path.GetDirectoryName(Application.ExecutablePath);

            if (MessageBox.Show("Czy chcesz zapisać bieżący układ ?", "Zapis układu", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                
             
                this.rgvView.SaveLayout(appDir + "\\" + "ryczwidok.lyt");
                this.rgvResult.SaveLayout(appDir + "\\" +  "ryczwynik.lyt");
            }




        }

        private void rgvView_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
           
        RadMenuItem customMenuItem = new RadMenuItem();
        customMenuItem.Text = "Sprawdź zaznaczone";
        customMenuItem.Name = "checkAll";

        RadMenuItem customMenuItem1 = new RadMenuItem();
        customMenuItem1.Text = "Sprawdź tylko w ZSRK";
        customMenuItem1.Name = "checkZSRK";
        
            RadMenuSeparatorItem separator = new RadMenuSeparatorItem();
        e.ContextMenu.Items.Add(separator);
        customMenuItem.Click += new EventHandler(customMenuItem_Click);
        customMenuItem1.Click += new EventHandler(customMenuItem_Click);
        e.ContextMenu.Items.Add(customMenuItem);
        e.ContextMenu.Items.Add(customMenuItem1);

        }
        private void customMenuItem_Click(object sender, EventArgs e)
        {
            rStruct therow = null;
            bool czyblad = false;
            if (rgvView.SelectedRows.Count == 0) { MessageBox.Show("Wybierz wiersze do sprawdzenia"); return; }
            Konfiguracja knf = RupDatabase.theContext.Konfiguracja.FirstOrDefault();
            try
            {
                using (RupIntegratorEntities dbContext = new RupIntegratorEntities())
                {
                    if ((sender as RadMenuItem).Name == "checkAll")
                    {
                        foreach (GridViewRowInfo row in rgvView.SelectedRows)
                        {
                            rStruct r = row.DataBoundItem as rStruct;
                            int rok;
                            int nr;
                            string repOryg;
                            string wydzial, repert;
                            string ans;
                            string outsad = "";
                            therow = r;
                            this.labelResult.Text = "Weryfikacja sygnatury " + r.Sygnatura;
                            this.labelResult.Refresh();

                            r.msg = "";
                            r.status = 0;
                            if ( !String.IsNullOrWhiteSpace(r.Sygnatura)) r.Sygnatura = r.Sygnatura.ToUpper();
                            ans = Utils.ParseSygn(r.Sygnatura, out wydzial, out repert, out nr, out rok, out repOryg, out outsad, r.SapSad);
                            if (!String.IsNullOrWhiteSpace(ans))
                            {
                                r.msg += ans;
                                r.status = -1;
                                czyblad = true;
                            }
                            else
                            {
                                r.SWydzial = wydzial;
                                r.SRepertorium = repert;
                                r.SNumer = nr.ToString();
                                r.SRok = rok.ToString();
                                if (!String.IsNullOrWhiteSpace(outsad)) r.SapSad = outsad;
                                SAPRepertorium rep = dbContext.SAPRepertorium.Where(a => a.kod == repert).FirstOrDefault();
                                if (rep == null)
                                {
                                    r.msg += "W słowniku brak takiego repertorium - rodzaju przedmiotu";
                                    r.status = -1;
                                    czyblad = true;
                                }
                                else
                                    r.SRodzajPrzedm = rep.SymbolRodzajPrzedmiotu;
                                string typsad = "SR";
                                switch (r.SapSad.Substring(0, 1).ToUpper())
                                {
                                    case "5":
                                    case "4":
                                        typsad = "SR";
                                        break;
                                    case "3":
                                        typsad = "SO";
                                        break;
                                    case "2":
                                        typsad = "SA";
                                        break;
                                    default:
                                        break;
                                }

                                SAPRodzajSprawy sps = dbContext.SAPRodzajSprawy.Where(a => a.repertorium == repert && a.typSad == typsad).FirstOrDefault();
                                if (sps == null)
                                {
                                    r.msg += "W słowniku brak takiego rodzaju sprawy";
                                    r.status = -1;
                                    czyblad = true;
                                }
                                else
                                    r.SRodzaj = sps.kod;
                            }

                            if (r.status != -1) r.status = 1;


                        }
                        if (czyblad == true)
                        {
                            MessageBox.Show("Walidacja sygnatur zakończyła się błędem"); return;
                        }
                    }
                    
                    this.labelResult.Text = "Weryfikacja sygnatury w systemie ZSRK...";
                    this.labelResult.Refresh();

                    foreach (GridViewRowInfo row in rgvView.SelectedRows)
                    {
                        rStruct r = row.DataBoundItem as rStruct;

                        Double nop = 0;
                        this.labelResult.Text = "Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura;
                        this.labelResult.Refresh();
                        r.status = 0;
                        therow = r;
                        SygnaturaTworzenie sygnqry = Utils.setupSygnStruct(r, knf);


                        string Przedmiotumowy = Utils.verifySygnatura(sygnqry);
                        if (!String.IsNullOrWhiteSpace(Przedmiotumowy))
                        {
                            this.labelResult.Text = "Odczyt sygnatury lokalnie " + r.Sygnatura + " OK nr przedmiotu" + Przedmiotumowy;
                            this.labelResult.Refresh();
                            r.msg += Przedmiotumowy;
                            r.status = 1;
                            continue;
                        }


                        ContractObjectCreateResponse anssygn = ZSRKRequestHelper.ZalozSygnature(sygnqry);
                        if (anssygn != null)
                        {
                            if (anssygn.Sygnatura != null)
                            {
                                if (String.IsNullOrWhiteSpace(anssygn.Sygnatura.IDPrzedmiotuUmowy))
                                {
                                    r.msg += "Błąd podczas zakładania/wyszukiwania sygnatury ";
                                    if (anssygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0 && anssygn.Komunikaty[0].RodzajKomunikatu == "E")
                                    {
                                        r.msg += anssygn.Komunikaty[0].Komunikat1;
                                    }
                                    r.status = -1;
                                    czyblad = true;

                                }
                                else
                                {
                                    this.labelResult.Text = "Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura + " OK nr przedmiotu" + anssygn.Sygnatura.IDPrzedmiotuUmowy;
                                    this.labelResult.Refresh();
                                    r.msg += anssygn.Sygnatura.IDPrzedmiotuUmowy;
                                    r.status = 1;
                                    Utils.addSygnatura(sygnqry, r.Sygnatura, anssygn.Sygnatura.IDPrzedmiotuUmowy);
                                }



                            }

                            else
                            {
                                if (anssygn.Komunikaty != null && anssygn.Komunikaty.GetUpperBound(0) >= 0 && anssygn.Komunikaty[0].RodzajKomunikatu == "E")
                                {
                                    r.msg += "Błąd podczas zakładania/wyszukiwania sygnatury " + anssygn.Komunikaty[0].Komunikat1;
                                    this.labelResult.Text = "Weryfikacja sygnatury w systemie ZSRK: " + r.Sygnatura + " Błąd " + anssygn.Komunikaty[0].Komunikat1;
                                    this.labelResult.Refresh();
                                    r.status = -1;
                                    czyblad = true;
                                }

                            }

                        }
                        else
                        {
                            MessageBox.Show("Błąd działania usługi sieciowej, nie można sprawdzić sygnatury w ZSRK");
                            //return;
                            break;
                        }


                    }
                    if (czyblad == true)
                    {
                        MessageBox.Show("Sprawdzenie sygnatur w ZSRK zakończyła się błędem"); return;
                    }
                    // mapowanie kuratorów

                    this.rgvView.MasterTemplate.Refresh();



                }

            }

            catch (Exception ex)
            {   log.Error("Błąd walidacji sygnatur w ZSRK " + therow.Sygnatura, ex);
                MessageBox.Show(ex.Message + (ex.InnerException != null ? " " + ex.InnerException : "") + therow != null ? " " + therow.Sygnatura : "");
                return;

            } 
             


        }

        private void cbClearFilters_Click(object sender, EventArgs e)
        {
            this.rgvView.FilterDescriptors.Clear(); 
        }

        private void kuratorzyMenuItem_Click(object sender, EventArgs e)
        {
            // edycja kuratorów
            SlowKurat sk = new SlowKurat();
            sk.ShowDialog();

        }

        private void sadyMenuItem_Click(object sender, EventArgs e)
        {
            // edycja mapowania sądów

            SlowSad sk = new SlowSad();
            sk.ShowDialog();
        }

        private void zastepstwaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapSygnatura kf = new MapSygnatura();
            kf.ShowDialog();
        }
    }
}