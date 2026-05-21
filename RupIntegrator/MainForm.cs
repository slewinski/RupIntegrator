using System;
using System.Linq;
using System.Collections.Generic;
using Telerik.WinControls.UI;
using DBModel;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Enumerations;
using CheckBoxInHeader;
using Telerik.WinControls.UI.Localization;

namespace KnsMigrator
{
    public partial class MainForm : RadForm
    {
        public MainForm()
        {
            InitializeComponent();
            RadGridLocalizationProvider.CurrentProvider = new PolishRadGridLocalizationProvider();
            this.AddCheckColumn();

            this.rgvValid.MasterTemplate.AllowAddNewRow = false;
            this.rgvValid.EnableFiltering = true;

            this.rgvValid.ViewCellFormatting += new CellFormattingEventHandler(rgvValid_ViewCellFormatting);
        }

        private KnsMigro thecontext = new KnsMigro();
        private BindingSource dataSource = new BindingSource();

        private void radPageView1_SelectedPageChanged(object sender, EventArgs e)
        {

        }

        private void ReloadVAlidGrid()
        {
            this.thecontext.ClearChanges();

        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.dataSource.DataSource  = thecontext.Mains.ToList() ;
            this.rgvValid.DataSource = this.dataSource; //.Mains;

// kolumny statusów
            GridViewTextBoxColumn validColumn = new GridViewTextBoxColumn();
            
            validColumn.Name = "IsValid";
            validColumn.HeaderText = "";
            validColumn.Width = 30;
            this.rgvValid.Columns.Insert(0, validColumn);




            GridViewComboBoxColumn supplierColumn = new GridViewComboBoxColumn();
            supplierColumn.Name = "FizPraw";
            supplierColumn.HeaderText = "Fiz/Praw";
            supplierColumn.DataSource = thecontext.TypOsobas.ToList();
            supplierColumn.ValueMember = "Typ";
            supplierColumn.DisplayMember = "Opis";
            supplierColumn.FieldName = "OsobaFizycznaOsobaPrawna";
            supplierColumn.Width = 70;
            this.rgvValid.Columns.Insert(2,supplierColumn);
            // Rodzaj przedmiotu umowy

            GridViewComboBoxColumn RodzPUmo = new GridViewComboBoxColumn();
            RodzPUmo.Name = "RodzPUmo";
            RodzPUmo.HeaderText = "Rodz. prz.umowy";
            RodzPUmo.DataSource = thecontext.SAPOpisPrzedmiotus.ToList();
            RodzPUmo.ValueMember = "Symbol";
            RodzPUmo.DisplayMember = "Opis";
            RodzPUmo.FieldName = "RodzajPrzedmiotuUmowy";
            RodzPUmo.Width = 150;
            this.rgvValid.Columns.Insert(26, RodzPUmo);


            // NrWydziałuISekcji
            GridViewComboBoxColumn NrWydzSek = new GridViewComboBoxColumn();
            NrWydzSek.Name = "NrWydzSek";
            NrWydzSek.HeaderText = "Wydział / sekcja";
            NrWydzSek.DataSource = thecontext.SAPWydzSekcjas.ToList();
            NrWydzSek.ValueMember = "Kod";
            NrWydzSek.DisplayMember = "Opis";
            NrWydzSek.FieldName = "NrWydziałuISekcji";
            NrWydzSek.Width = 150;
            this.rgvValid.Columns.Insert(27, NrWydzSek);
            // Repertorium

            GridViewComboBoxColumn Repertorium = new GridViewComboBoxColumn();
            Repertorium.Name = "Repertorium";
            Repertorium.HeaderText = "Repertorium";
            Repertorium.DataSource = thecontext.SAPRepertoria.ToList();
            Repertorium.ValueMember = "Kod";
            Repertorium.DisplayMember = "Symbol";
            Repertorium.FieldName = "Repertorium";
            Repertorium.Width = 150;
            this.rgvValid.Columns.Insert(28, Repertorium);
            // tomy akt
            GridViewComboBoxColumn Tomyakt = new GridViewComboBoxColumn();
            Tomyakt.Name = "Tomyakt";
            Tomyakt.HeaderText = "L. tomów akt";
            Tomyakt.DataSource = thecontext.SAPTomyAkts.ToList();
            Tomyakt.ValueMember = "Kod";
            Tomyakt.DisplayMember = "Opis";
            Tomyakt.FieldName = "IlośćTomów";
            Tomyakt.Width = 150;
            this.rgvValid.Columns.Insert(29, Tomyakt);
            //Stan należności
        
            GridViewComboBoxColumn StanNalGrz = new GridViewComboBoxColumn();
            StanNalGrz.Name = "StanyGrz";
            StanNalGrz.HeaderText = "Stan Nal Grz";
            StanNalGrz.DataSource = thecontext.SAPStanNals.ToList();
            StanNalGrz.ValueMember = "Kod";
            StanNalGrz.DisplayMember = "Opis";
            StanNalGrz.FieldName = "StanNależnościGrzywna";
            StanNalGrz.Width = 150;
            this.rgvValid.Columns.Insert(35, StanNalGrz);
          
            GridViewComboBoxColumn StanNalKs = new GridViewComboBoxColumn();
            StanNalKs.Name = "StanyKs";
            StanNalKs.HeaderText = "Stan Nal Koszty";
            StanNalKs.DataSource = thecontext.SAPStanNals.Where(a => a.Grzywnakoszty == 'a').ToList();
            StanNalKs.ValueMember = "Kod";
            StanNalKs.DisplayMember = "Opis";
            StanNalKs.FieldName = "StanNależnościKoszty";
            StanNalKs.Width = 150;
            this.rgvValid.Columns.Insert(36, StanNalKs);
           
            
            GridViewComboBoxColumn JedGColumn = new GridViewComboBoxColumn();
            JedGColumn.Name = "JGSygn";
            JedGColumn.HeaderText = "Sąd Sygnat.";
            JedGColumn.DataSource = thecontext.SAPSads.ToList();
            JedGColumn.ValueMember = "kod";
            JedGColumn.DisplayMember = "miastSad";
            JedGColumn.FieldName = "JednostkaGospodarcza";
            JedGColumn.Width = 150;
            this.rgvValid.Columns.Insert(19, JedGColumn);

            // CzęściowoGrzywna
            GridViewComboBoxColumn CzescGrz = new GridViewComboBoxColumn();
            CzescGrz.Name = "CzescGrz";
            CzescGrz.HeaderText = "Opr. cz. grzywna";
            CzescGrz.DataSource = thecontext.SAPKodyOprs.Where(c=> c.Grzywnakoszty == 'g').ToList();
            CzescGrz.ValueMember = "Kod";
            CzescGrz.DisplayMember = "Nazwa";
            CzescGrz.FieldName = "CzęściowoGrzywna";
            CzescGrz.Width = 150;
            this.rgvValid.Columns.Insert(27, CzescGrz);
            
            GridViewComboBoxColumn CzescKs = new GridViewComboBoxColumn();
            CzescKs.Name = "CzescKs";
            CzescKs.HeaderText = "Opr. cz. koszty";
            CzescKs.DataSource = thecontext.SAPKodyOprs.Where(c => c.Grzywnakoszty == 'k').ToList();
            CzescKs.ValueMember = "Kod";
            CzescKs.DisplayMember = "Nazwa";
            CzescKs.FieldName = "CzęściowoKoszty";
            CzescKs.Width = 150;
            this.rgvValid.Columns.Insert(28, CzescKs);
            
            //

            SetColors();
            /*
            GridViewComboBoxColumn sadColumn = new GridViewComboBoxColumn();
                        supplierColumn.Name = "SadOrze";
                        supplierColumn.HeaderText = "Sąd Orzek.";
                        supplierColumn.DataSource = thecontext.TypOsobas.ToList();
                        supplierColumn.ValueMember = "Typ";
                        supplierColumn.DisplayMember = "Opis";
                        supplierColumn.FieldName = "OsobaFizycznaOsobaPrawna";
                        supplierColumn.Width = 100;
                        this.rgvValid.Columns.Insert(1, supplierColumn);
              */

        }

        private void radPageViewMain_SelectedPageChanged(object sender, EventArgs e)
        {
            if (this.radPageViewMain.SelectedPage == this.radPageViewSlowniki)
            {
                this.radPageViewWorkspace.SelectedPage = this.radPageSlowniki;
            }
            if (this.radPageViewMain.SelectedPage == this.radPageViewMapowania)
            {
                this.radPageViewWorkspace.SelectedPage = this.radPageMapowanie;
            }

            if (this.radPageViewMain.SelectedPage == this.radPageViewWalidacja)
            {
                this.radPageViewWorkspace.SelectedPage = this.radPageWalidacja;
                            
            }

            if (this.radPageViewMain.SelectedPage == this.radPageViewEkstrakcja)
            {
                this.radPageViewWorkspace.SelectedPage = this.radPageEkstrakcja;

            }

            if (this.radPageViewMain.SelectedPage == this.radPageViewKonfig)
            {
                this.radPageViewWorkspace.SelectedPage = null;

            }
          
        }

        private void rbSaveValid_Click(object sender, EventArgs e)
        {
            thecontext.SaveChanges();

        }

        private void startWBar()
        {
            this.radWaitingBar1.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.Dash;
            WaitingBarSeparatorElement dash = this.radWaitingBar1.WaitingBarElement.SeparatorElement;
            dash.NumberOfColors = 2;
            dash.BackColor = Color.Orange;
            dash.BackColor2 = Color.Yellow;
            dash.SweepAngle = 45;
            dash.StepWidth = 15;
            dash.SeparatorWidth = 10;
            dash.GradientPercentage = 0.25f;
        }

        private void rbReload_Click(object sender, EventArgs e)
        {
            this.thecontext.ClearChanges();
            //this.
            startWBar();
            this.rgvValid.MasterTemplate.Refresh();
        }


        private void AddCheckColumn()
        {
            CustomCheckBoxColumn checkColumn = new CustomCheckBoxColumn();
            checkColumn.Name = "Wybierz";
            checkColumn.HeaderText = "";
            this.rgvValid.Columns.Insert(0, checkColumn);
        }


        void rgvValid_ViewCellFormatting(object sender, CellFormattingEventArgs e)
        {
            if (e.CellElement is GridFilterCellElement && e.CellElement.ColumnInfo.Name == "Wybierz")
            {
                e.CellElement.Children.Clear();
            }
        }

        private void SetColors()
        {
            ConditionalFormattingObject obj = new ConditionalFormattingObject("GrzCywFiz", ConditionTypes.Equal, "0010", "", false);
            obj.CellBackColor = Color.SkyBlue;
            //obj.CellForeColor = Color.Red;
            obj.TextAlignment = ContentAlignment.MiddleRight;
            this.rgvValid.Columns["CzescGrz"].ConditionalFormattingObjectList.Add(obj);

                    
        }

        private void rgvValid_Click(object sender, EventArgs e)
        {

        }

        private void rbParse_Click(object sender, EventArgs e)
        {
            // parsowanie zawartości
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            MigrForm mf = new MigrForm();

            mf.Show();
        }

       

    }
}
