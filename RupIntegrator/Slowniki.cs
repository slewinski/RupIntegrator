using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Drawing;
using Telerik.WinControls;
using System.Data.SqlClient;
using Telerik.WinControls.Data;
using System.ComponentModel;
using SapPOHelper;
using Ex2PscdInterface.Ex2PscdPaymentClarificationsQueryOutService;
using Ex2PscdInterface.Ex2PscdGetCaseRegistryTypesOutService;
using System.Diagnostics;
using Ex2PscdInterface.Ex2PscdGetCourtsOutService;
using Ex2PscdInterface.Ex2PscdGetDepartmentsOutService;

namespace KnsMigrator
{
    public partial class MigrForm
    {
        private BindingSource RodzajPrzedmiotuDataSource = new BindingSource();
        private BindingSource SygnMappingDataSource = new BindingSource();
        private BindingSource FaktyDataSource = new BindingSource();
        private BindingSource TransferBindingDataSource = new BindingSource();
        private BindingSource DokumentyBindingDataSource = new BindingSource();
        private BindingSource SAPWplatyDataSource = new BindingSource();
        private BindingSource KodyPocztKonfig = new BindingSource();
        private int TransferId = 0;
        private bool breakiIndicator = false;
        private Imports imp;
        private string mapMode;
        private string EncryptPhase = "Application error";

        private class fldValidStatus
        {

            public string fName { get; set; }      // nazwa pola do walidacji ( kolumny ) ;
            public int    validStatus { get; set; } // status walidacji

            public fldValidStatus(string s, int i)
            {
                fName = s;
                validStatus = i;
            }

        }

        private fldValidStatus[] validArry = new fldValidStatus[] {    new fldValidStatus("typFakt",11),
                                                         new fldValidStatus("SAPImportPonowne",0),
                                                         new fldValidStatus("dlFizPraw",0),
                                                         new fldValidStatus("dlimie",11),
                                                         new fldValidStatus("dlnazwisko",0),
                                                         new fldValidStatus("dlUlica",11),
                                                         new fldValidStatus("dlnrdomu",11),
                                                         new fldValidStatus("dlnrmieszkania",0),
                                                         new fldValidStatus("dlkodpoczt",11),
                                                         new fldValidStatus("dlmiejscowosc",11),
                                                         new fldValidStatus("dlkraj",11),
                                                         new fldValidStatus("dlRBN",11),                                                          //new fldValidStatus("IBAN",1),
                                                         new fldValidStatus("dlNip",0),
                                                         new fldValidStatus("dlpesel",0),
                                                         new fldValidStatus("dlRBN",11),
                                                         //new fldValidStatus("TypKontaUmowy",1),
                                                         new fldValidStatus("Karta",11),
                                                         //new fldValidStatus("RelacjaKonta",1),
                                                         new fldValidStatus("RodzPUmo",11),
                                                         new fldValidStatus("sadorzek",11),
                                                         new fldValidStatus("wydzialsekcja",11),
                                                         new fldValidStatus("repertorium",11),
                                                         new fldValidStatus("Numer",11),
                                                         new fldValidStatus("rok",11),
                                                         new fldValidStatus("RodzSprawy",11),
                                                         new fldValidStatus("ltomow",11),
                                                         new fldValidStatus("Sygnatura",11),
                                                         new fldValidStatus("DataDokumentu",11),
                                                         new fldValidStatus("DataKsiegowania",11),
                                                         //new fldValidStatus("RodzajDokumentu",1),
                                                         //new fldValidStatus("Waluta",1),
                                                         //new fldValidStatus("KluczUzgodnienia",1),
                                                         //new fldValidStatus("JednostakaGospodarcaWłasna",1),
                                                         new fldValidStatus("OperacjaGlowna",11),
                                                         new fldValidStatus("OperacjaCzesciowa",11),
                                                         new fldValidStatus("kwota",11),
                                                         new fldValidStatus("DataPlatnosci",11),
                                                         new fldValidStatus("Stan",3),
                                                         new fldValidStatus("Opis",0),
                                                         new fldValidStatus("DocGuid",11),
                                                         new fldValidStatus("SAPKontoPartnera",4),
                                                         new fldValidStatus("SAPKontoUmowy",4),
                                                         new fldValidStatus("SAPPrzedmiotUmowy",4),
                                                         new fldValidStatus("SAPDocId",0),
                                                         new fldValidStatus("SAPDocIdRef",4),
                                                         new fldValidStatus("SAPRatyId",0),
                                                         new fldValidStatus("RataData1",0),
                                                         new fldValidStatus("RataData2",0),
                                                         new fldValidStatus("RataData3",0),
                                                         new fldValidStatus("RataData4",0),
                                                         new fldValidStatus("RataData5",0),
                                                         new fldValidStatus("RataData6",0),
                                                         new fldValidStatus("RataData7",0),
                                                         new fldValidStatus("RataData8",0),
                                                         new fldValidStatus("RataData9",0),
                                                         new fldValidStatus("RataData10",0),
                                                         new fldValidStatus("RataData11",0),
                                                         new fldValidStatus("RataData12",0),
                                                         new fldValidStatus("RataData13",0),
                                                         new fldValidStatus("RataData14",0),
                                                         new fldValidStatus("RataData15",0),
                                                         new fldValidStatus("RataData16",0),
                                                         new fldValidStatus("RataData17",0),
                                                         new fldValidStatus("RataData18",0),
                                                         new fldValidStatus("RataData19",0),
                                                         new fldValidStatus("RataData20",0),
                                                         new fldValidStatus("RataData21",0),
                                                         new fldValidStatus("RataData22",0),
                                                         new fldValidStatus("RataData23",0),
                                                         new fldValidStatus("RataData24",0),
                                                         new fldValidStatus("RataData25",0),
                                                         new fldValidStatus("RataData26",0),
                                                         new fldValidStatus("RataData27",0),
                                                         new fldValidStatus("RataData28",0),
                                                         new fldValidStatus("RataData29",0),
                                                         new fldValidStatus("RataData30",0),
                                                         new fldValidStatus("RataData31",0),
                                                         new fldValidStatus("RataData32",0),
                                                         new fldValidStatus("RataData33",0),
                                                         new fldValidStatus("RataData34",0),
                                                         new fldValidStatus("RataData35",0),
                                                         new fldValidStatus("RataData36",0),
                                                         new fldValidStatus("RataKwota1",0),
                                                         new fldValidStatus("RataKwota2",0),
                                                         new fldValidStatus("RataKwota3",0),
                                                         new fldValidStatus("RataKwota4",0),
                                                         new fldValidStatus("RataKwota5",0),
                                                         new fldValidStatus("RataKwota6",0),
                                                         new fldValidStatus("RataKwota7",0),
                                                         new fldValidStatus("RataKwota8",0),
                                                         new fldValidStatus("RataKwota9",0),
                                                         new fldValidStatus("RataKwota10",0),
                                                         new fldValidStatus("RataKwota11",0),
                                                         new fldValidStatus("RataKwota12",0),
                                                         new fldValidStatus("RataKwota13",0),
                                                         new fldValidStatus("RataKwota14",0),
                                                         new fldValidStatus("RataKwota15",0),
                                                         new fldValidStatus("RataKwota16",0),
                                                         new fldValidStatus("RataKwota17",0),
                                                         new fldValidStatus("RataKwota18",0),
                                                         new fldValidStatus("RataKwota19",0),
                                                         new fldValidStatus("RataKwota20",0),
                                                         new fldValidStatus("RataKwota21",0),
                                                         new fldValidStatus("RataKwota22",0),
                                                         new fldValidStatus("RataKwota23",0),
                                                         new fldValidStatus("RataKwota23",0),
                                                         new fldValidStatus("RataKwota23",0),
                                                         new fldValidStatus("RataKwota23",0),
                                                         new fldValidStatus("RataKwota24",0),
                                                         new fldValidStatus("RataKwota25",0),
                                                         new fldValidStatus("RataKwota26",0),
                                                         new fldValidStatus("RataKwota27",0),
                                                         new fldValidStatus("RataKwota28",0),
                                                         new fldValidStatus("RataKwota29",0),
                                                         new fldValidStatus("RataKwota30",0),
                                                         new fldValidStatus("RataKwota31",0),
                                                         new fldValidStatus("RataKwota32",0),
                                                         new fldValidStatus("RataKwota33",0),
                                                         new fldValidStatus("RataKwota34",0),
                                                         new fldValidStatus("RataKwota35",0),
                                                         new fldValidStatus("RataKwota36",0) };

        private void InitSygnMapping()
        {

            GridViewComboBoxColumn SadIDColumn = rgvSygnMap.Columns["SrcSad"] as Telerik.WinControls.UI.GridViewComboBoxColumn;
            SadIDColumn.DataSource = thecontext.SAPSad.OrderBy(a => a.miastSad).ToList();
            SadIDColumn.ValueMember = "kod";
            SadIDColumn.DisplayMember = "miastSad";
            SadIDColumn.Width = 150;

            GridViewComboBoxColumn SadIDdestColumn = rgvSygnMap.Columns["DestSad"] as Telerik.WinControls.UI.GridViewComboBoxColumn;
            SadIDdestColumn.DataSource = thecontext.SAPSad.OrderBy(a => a.miastSad).ToList();
            SadIDdestColumn.ValueMember = "kod";
            SadIDdestColumn.DisplayMember = "miastSad";
            SadIDdestColumn.Width = 150;

            this.SygnMappingDataSource.DataSource = thecontext.SygnMap; //.ToList();
            this.rgvSygnMap.DataSource = this.SygnMappingDataSource; //.Mains;
        }

        private void InitKsiegiDictionary()
        {
            this.RodzajPrzedmiotuDataSource.DataSource = thecontext.KnsKsiegi.ToList();
            this.rgvKsiegi.DataSource = this.RodzajPrzedmiotuDataSource; //.Mains;
            Dictionary<int, string> wyklucz = new Dictionary<int, string>();
            wyklucz.Add(4, "Świadczenie/Nawiązka.SP");
            wyklucz.Add(3, "Naprawienie szkody");
            wyklucz.Add(2, "Przepadek korzyści");
            wyklucz.Add(1, "Świadczenie/Nawiązka.FPP");
            wyklucz.Add(0, "KNS");

            GridViewComboBoxColumn wykluczColumn = (GridViewComboBoxColumn)this.rgvKsiegi.Columns["czyFPP"];
            wykluczColumn.DataSource = wyklucz.ToList();
            wykluczColumn.ValueMember = "Key";
            wykluczColumn.DisplayMember = "Value";
            wykluczColumn.FilteringMode = GridViewFilteringMode.DisplayMember;


            Dictionary<int, string> mapKS = new Dictionary<int, string>();
            mapKS.Add(2, "grzywna = świadczenie, koszty = nawiązka");
            mapKS.Add(1, "grzywna = nawiązka, koszty = świadczenie");
            mapKS.Add(0, "nie dotyczy");

            GridViewComboBoxColumn mapKSColumn = (GridViewComboBoxColumn)this.rgvKsiegi.Columns["ksGrzFPPMap"];
            mapKSColumn.DataSource = mapKS.ToList();
            mapKSColumn.ValueMember = "Key";
            mapKSColumn.DisplayMember = "Value";
            mapKSColumn.FilteringMode = GridViewFilteringMode.DisplayMember;



            Dictionary<string, string> taknie = new Dictionary<string, string>();
            taknie.Add("TAK", "TAK");
            taknie.Add("NIE", "NIE");
            

            GridViewComboBoxColumn bigColumn = (GridViewComboBoxColumn)this.rgvKsiegi.Columns["oprKosztFiz"];
            bigColumn.DataSource = taknie.ToList();
            bigColumn.ValueMember = "Key";
            bigColumn.DisplayMember = "Value";
            bigColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

           
            GridViewComboBoxColumn rodzajColumn = new GridViewComboBoxColumn();
            rodzajColumn.Name = "Rodzajprzedmiotu";
            rodzajColumn.HeaderText = "Rodzaj przedmiotu";
            rodzajColumn.DataSource = thecontext.SAPOpisPrzedmiotu.ToList();
            rodzajColumn.ValueMember = "Symbol";
            rodzajColumn.DisplayMember = "Opis";
            rodzajColumn.FieldName = "RodzajPrzedmiotu";
            rodzajColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
            rodzajColumn.Width = 120;
            this.rgvKsiegi.Columns.Insert(rgvKsiegi.ColumnCount, rodzajColumn);

        }



        private void LoadKsiegiDictionary()
        {

        }



        private void InitTransfer()
        {
            this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a=>a.rodzaj<=1000).OrderByDescending(a=>a.Rok).OrderByDescending(a=>a.Miesiac).OrderByDescending(a => a.Id).ToList();

            GridViewComboBoxColumn rodzajColumn = new GridViewComboBoxColumn();
            rodzajColumn.Name = "Rodzaj";
            rodzajColumn.HeaderText = "Rodzaj transferu";
            rodzajColumn.DataSource = thecontext.TypTransferu.ToList();
            rodzajColumn.ValueMember = "kod";
            rodzajColumn.DisplayMember = "opis";
            rodzajColumn.FieldName = "rodzaj";
            rodzajColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
            rodzajColumn.Width = 70;
            this.rgvTransfer.Columns.Insert(1, rodzajColumn);

            GroupDescriptor descriptor2 = new GroupDescriptor();
            descriptor2.GroupNames.Add("Rok", ListSortDirection.Descending);
            GroupDescriptor descriptor3 = new GroupDescriptor();
            descriptor3.GroupNames.Add("Miesiac", ListSortDirection.Descending);
            this.rgvTransfer.GroupDescriptors.Add(descriptor2);
            this.rgvTransfer.GroupDescriptors.Add(descriptor3);

            this.rgvTransfer.DataSource = this.TransferBindingDataSource;


        }

        private void InitDokumenty()
        {
            Dictionary<string, string> samoistna = new Dictionary<string, string>();
            samoistna.Add("s", "samoistna grzy.");
            samoistna.Add("", "");



            Dictionary<int, string> wyklucz = new Dictionary<int, string>();
            wyklucz.Add(1, "TAK");
            wyklucz.Add(0, "");

            Dictionary<string, string> typOsoby = new Dictionary<string, string>();
            typOsoby.Add(" ", "fizyczna");
            typOsoby.Add("X", "prawna");

            this.rgvDokumenty.MasterTemplate.AllowAddNewRow = false;
            this.rgvDokumenty.EnableFiltering = true;
            this.rgvDokumenty.MasterTemplate.ShowHeaderCellButtons = true;
            this.rgvDokumenty.MasterTemplate.ShowFilteringRow = false;
            this.rgvDokumenty.TableElement.RowHeaderColumnWidth = 40;
            this.rgvDokumenty.Dock = DockStyle.Fill;
            this.rgvWplaty.Visible = false;
            this.rgvDokumenty.DataSource = this.DokumentyBindingDataSource;
            /*
            GridViewComboBoxColumn rodzajColumn =  (GridViewComboBoxColumn)this.rgvDokumenty.Columns["OperacjaCzesciowa"];
            rodzajColumn.DataSource = thecontext.SAPKodyOpr.ToList();
            rodzajColumn.ValueMember = "kod";
            rodzajColumn.DisplayMember = "nazwa";
            rodzajColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
            */

            GridViewComboBoxColumn rodzajColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["OperacjaCzesciowa"];
            rodzajColumn.DataSource = thecontext.SAPKodyOpr.Select(i =>
            new
            {
                kod = i.kod,
                nazwa = i.nazwa,
                opgl = i.operacjaGlowna
            }).Distinct()//.Where(i => i.opgl == "N010")
            .ToList();
            rodzajColumn.ValueMember = "kod";
            rodzajColumn.DisplayMember = "nazwa";
            rodzajColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn stanColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["Stan"];
            stanColumn.DataSource = thecontext.SAPStanNal.ToList();
            stanColumn.ValueMember = "Kod";
            stanColumn.DisplayMember = "Opis";
            stanColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn samoistnaColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["grzSamoistna"];
            samoistnaColumn.DataSource = samoistna.ToList();
            samoistnaColumn.ValueMember = "Key";
            samoistnaColumn.DisplayMember = "Value";
            samoistnaColumn.FilteringMode = GridViewFilteringMode.DisplayMember;


            GridViewComboBoxColumn sadorzekColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["sadorzek"];
            sadorzekColumn.DataSource = thecontext.SAPSad.OrderBy(a => a.miasto).ToList();
            sadorzekColumn.ValueMember = "kod";
            sadorzekColumn.DisplayMember = "miastSad";
            samoistnaColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn repColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["repertorium"];
            repColumn.DataSource = thecontext.SAPRepertorium.ToList();
            repColumn.ValueMember = "kod";
            repColumn.DisplayMember = "kod";
            repColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn rpumoColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["RodzPUmo"];
            rpumoColumn.DataSource = thecontext.SAPOpisPrzedmiotu.ToList();
            rpumoColumn.ValueMember = "Symbol";
            rpumoColumn.DisplayMember = "Opis";
            rpumoColumn.FilteringMode = GridViewFilteringMode.DisplayMember;


            GridViewComboBoxColumn ksColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["KnsKsiega"];
            ksColumn.DataSource = thecontext.KnsKsiegi.ToList();
            ksColumn.ValueMember = "Id_Ksiegi";
            ksColumn.DisplayMember = "Nazwa";
            ksColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn rsColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["RodzSprawy"];
            rsColumn.DataSource = thecontext.SAPRodzajSprawy.ToList();
            rsColumn.ValueMember = "kod";
            rsColumn.DisplayMember = "opis";
            rsColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn taColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["ltomow"];
            taColumn.DataSource = thecontext.SAPTomyAkt.ToList();
            taColumn.ValueMember = "Kod";
            taColumn.DisplayMember = "Opis";
            taColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn krajColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["dlkraj"];
            krajColumn.DataSource = thecontext.SAPKodKraju.OrderBy(a => a.kraj).ToList();
            krajColumn.ValueMember = "kod";
            krajColumn.DisplayMember = "kraj";
            krajColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn fpColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["dlFizPraw"];
            fpColumn.DataSource = typOsoby.ToList();
            fpColumn.ValueMember = "Key";
            fpColumn.DisplayMember = "Value";
            fpColumn.DataSourceNullValue = "";
            fpColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn rbnColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["dlRBN"];
            rbnColumn.DataSource = thecontext.SAPRBN.ToList();
            rbnColumn.ValueMember = "kod";
            rbnColumn.DisplayMember = "opis";
            rbnColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn wykluczColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["wyklucz"];
            wykluczColumn.DataSource = wyklucz.ToList();
            wykluczColumn.ValueMember = "Key";
            wykluczColumn.DisplayMember = "Value";
            wykluczColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            GridViewComboBoxColumn opgColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["OperacjaGlowna"];
            opgColumn.DataSource = thecontext.SAPKodyOpr.Select(i =>
            new
            {
                operacjaGl = i.operacjaGlowna,
                opis = i.oznaczenieOpGlownej
            }).Distinct().ToList();
            opgColumn.ValueMember = "operacjaGl";
            opgColumn.DisplayMember = "opis";
            opgColumn.FilteringMode = GridViewFilteringMode.DisplayMember;

            ExpressionFormattingObject obj = new ExpressionFormattingObject("Cond1", "SAPImportStatus = 0", false);
            obj.CellBackColor = Color.LightGray;
            obj.CellForeColor = Color.Black;
            this.rgvDokumenty.Columns["SAPImportStatus"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond2", "SAPImportStatus < 0 AND SAPImportStatus > -1000", false);
            obj.CellBackColor = Color.Red;
            obj.CellForeColor = Color.Black;
            this.rgvDokumenty.Columns["SAPImportStatus"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond3", "SAPImportStatus > 0 ", false);
            obj.CellBackColor = Color.Green;
            obj.CellForeColor = Color.Black;
            this.rgvDokumenty.Columns["SAPImportStatus"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond4", "SAPImportStatus = -1000 ", false);
            obj.CellBackColor = Color.Yellow;
            obj.CellForeColor = Color.Black;
            this.rgvDokumenty.Columns["SAPImportStatus"].ConditionalFormattingObjectList.Add(obj);


            GridViewSummaryItem summaryItem = new GridViewSummaryItem("kwota", "{0}", GridAggregateFunction.Sum);
            GridViewSummaryItem countItem = new GridViewSummaryItem("Karta", "{0}", GridAggregateFunction.Count);

            //summaryItem.Name = "kwota";

            //summaryItem.AggregateExpression = "Sum(kwota)";

            GridViewSummaryRowItem summaryRowItem = new GridViewSummaryRowItem();
            summaryRowItem.Add(countItem);
            summaryRowItem.Add(summaryItem);
            this.rgvDokumenty.SummaryRowsTop.Add(summaryRowItem);

        }

        private void InitEkstrakcja()
        {


            Konfiguracja knf = this.thecontext.Konfiguracja.FirstOrDefault();
            switch (knf.typImportSAP)
            { 
                case 2:// rup integrator

                    
                    
                    GridViewTextBoxColumn k1= new GridViewTextBoxColumn();
                    k1.Name = "Mikrorachunek";
                    k1.HeaderText = "Mikrorachunek";
                    this.rgvEkstrakcja.Columns.Insert(48, k1);

                    GridViewTextBoxColumn k2= new GridViewTextBoxColumn();
                    k2.Name = "MPK";
                    k2.HeaderText = "MPK";
                    this.rgvEkstrakcja.Columns.Insert(49, k2);

                    GridViewTextBoxColumn k3= new GridViewTextBoxColumn();
                    k3.Name = "Zlecenie";
                    k3.HeaderText = "Zlecenie";
                    this.rgvEkstrakcja.Columns.Insert(50, k3);

                    GridViewTextBoxColumn k4= new GridViewTextBoxColumn();
                    k4.Name = "Referencja";
                    k4.HeaderText = "Referencja";
                    this.rgvEkstrakcja.Columns.Insert(51, k4);


                    GridViewTextBoxColumn k5= new GridViewTextBoxColumn();
                    k5.Name = "NumerWydzialuiSekcji";
                    k5.HeaderText = "NumerWydziałuiSekcji";
                    this.rgvEkstrakcja.Columns.Insert(52, k5);

                    GridViewTextBoxColumn k6= new GridViewTextBoxColumn();
                    k6.Name = "Repert";
                    k6.HeaderText = "Repert";
                    this.rgvEkstrakcja.Columns.Insert(53, k6);

                    GridViewTextBoxColumn k7= new GridViewTextBoxColumn();
                    k7.Name = "KolejnyNumerSprawy";
                    k7.HeaderText = "KolejnyNumerSprawy";
                    this.rgvEkstrakcja.Columns.Insert(54, k7);

                    GridViewTextBoxColumn k8= new GridViewTextBoxColumn();
                    k8.Name = "rRok";
                    k8.HeaderText = "rRok";
                    this.rgvEkstrakcja.Columns.Insert(55, k8);

                    GridViewTextBoxColumn k9= new GridViewTextBoxColumn();
                    k9.Name = "OznaczeniePrzedmiotuUmowy";
                    k9.HeaderText = "OznaczeniePrzedmiotuUmowy";
                    this.rgvEkstrakcja.Columns.Insert(56, k9);

                    GridViewTextBoxColumn stanKUColumn1= new GridViewTextBoxColumn();
                    stanKUColumn1.Name = "StanowiskoFinansoweKU";
                    stanKUColumn1.HeaderText = "Stanowisko Finansowe KU";
                    stanKUColumn1.FieldName = "StanowiskoFinansoweKU";

                    
                    this.rgvEkstrakcja.Columns.Insert(20, stanKUColumn1);


                    GridViewTextBoxColumn stanPUColumn1= new GridViewTextBoxColumn();
                    stanPUColumn1.Name = "StanowiskoFinansowePU";
                    stanPUColumn1.HeaderText = "Stanowisko Finansowe PU";
                    stanPUColumn1.FieldName = "StanowiskoFinansowePU";
                    this.rgvEkstrakcja.Columns.Insert(23, stanPUColumn1);


                    break;
                case 1: // KNS 2014

                    break;
                default:
                    GridViewTextBoxColumn stanKUColumn= new GridViewTextBoxColumn();
                    stanKUColumn.Name = "StanowiskoFinansoweKU";
                    stanKUColumn.HeaderText = "Stanowisko Finansowe KU";
                    stanKUColumn.FieldName = "StanowiskoFinansoweKU";

                    
                    this.rgvEkstrakcja.Columns.Insert(20, stanKUColumn);


                    GridViewTextBoxColumn stanPUColumn= new GridViewTextBoxColumn();
                    stanPUColumn.Name = "StanowiskoFinansowePU";
                    stanPUColumn.HeaderText = "Stanowisko Finansowe PU";
                    stanPUColumn.FieldName = "StanowiskoFinansowePU";
                    this.rgvEkstrakcja.Columns.Insert(23, stanPUColumn);


                    
                break;
            
            
            
            }

         
        }


        private void InitWplaty()
        {
            this.rgvWplaty.DataSource = this.wplataBindingSource;

        }

        private void setupDokView(int? transferType)
        {
            switch (transferType)
            {
                case 1: // salda
                case 2:
                case 5: //raty
                case 6:
                    setRatyColsInvisible(false);
                    this.rgvDokumenty.Columns["KnsKsiega"].IsVisible = true;
                    this.rgvDokumenty.Columns["sadorzekkns"].IsVisible = true;
                    this.rgvDokumenty.Columns["grzSamoistna"].IsVisible = true;
                    this.rgvDokumenty.Columns["Stan"].IsVisible = true;
                    this.rgvDokumenty.Columns["SAPRatyId"].IsVisible = true;
                    this.rgvDokumenty.Columns["Karta"].HeaderText = "K. dł.";
                    this.rmiDpodst.Text = "Salda/Przypisy";
                    this.rmi_OdpisyWS.Visibility = ElementVisibility.Visible;
                    break;
                case 3:
                    setRatyColsInvisible(false);
                    this.rgvDokumenty.Columns["KnsKsiega"].IsVisible = true;
                    this.rgvDokumenty.Columns["sadorzekkns"].IsVisible = true;
                    this.rgvDokumenty.Columns["grzSamoistna"].IsVisible = true;
                    this.rgvDokumenty.Columns["Stan"].IsVisible = true;
                    this.rgvDokumenty.Columns["SAPRatyId"].IsVisible = true;
                    this.rgvDokumenty.Columns["Karta"].HeaderText = "K. dł.";
                    this.rmiDpodst.Text = "Salda/Przypisy";
                    this.rmi_OdpisyWS.Visibility = ElementVisibility.Visible;
                    break;
                case 7:
                    setRatyColsInvisible(false);
                    this.rgvDokumenty.Columns["KnsKsiega"].IsVisible = false;
                    this.rgvDokumenty.Columns["sadorzekkns"].IsVisible = false;
                    this.rgvDokumenty.Columns["grzSamoistna"].IsVisible = false;
                    this.rgvDokumenty.Columns["Stan"].IsVisible = false;
                    this.rgvDokumenty.Columns["SAPRatyId"].IsVisible = false;
                    this.rgvDokumenty.Columns["Karta"].HeaderText = "Konto um.";
                    this.rmiDpodst.Text = "Zwrot 3/4 opłaty";
                    this.rmi_OdpisyWS.Visibility = ElementVisibility.Hidden;
                    break;
                default:
                    break;
            }

        }

        private void setRatyColsInvisible(bool state)
        {
            int i;
            

            for (i = 1 ; i <=36; i++)
            {
                this.rgvDokumenty.Columns["RataData" + i.ToString()].IsVisible = state;
                this.rgvDokumenty.Columns["RataKwota" + i.ToString()].IsVisible = state;
            }
        }

        private void rgvTransfer_SelectionChanged(object sender, EventArgs e)
        {
            int Id;

            Transfer trns;
            if ((sender as RadGridView).CurrentRow != null)
            {


                Id = Convert.ToInt32((sender as RadGridView).CurrentRow.Cells["Id"].Value);
                if (Id > 0)
                {
                    //rgvTransfer.DataSource = null;
                    TransferId = Id;
                    trns = thecontext.Transfer.Where(a => a.Id == TransferId).FirstOrDefault();

                    if (trns != null)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        switch (trns.rodzaj)
                        {
                            case 1: // salda
                            case 2:
                            case 5: //raty
                            case 6:
                                setupDokView (trns.rodzaj);  
                                GridViewComboBoxColumn rodzajColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["OperacjaCzesciowa"];
                                rodzajColumn.DataSource = null;
                                rodzajColumn.DataSource = thecontext.SAPKodyOpr.Select(i =>
                                new
                                {
                                    kod = i.kod,
                                    nazwa = i.nazwa,
                                    opgl = i.operacjaGlowna
                                }).Where(i => i.opgl == "N010" || i.opgl == "FPP0").Distinct()
                                .ToList();
                                rodzajColumn.ValueMember = "kod";
                                rodzajColumn.DisplayMember = "nazwa";
                                rodzajColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
                                this.DokumentyBindingDataSource.DataSource = thecontext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.Transfer_Id == TransferId).OrderBy(b => b.Sprawa.KdNumer).OrderBy(b => b.Sprawa.KdRok).OrderBy(a => a.Sprawa.KnsKsiega).ToList();
                                rgvDokumenty.Dock = DockStyle.Fill;
                                rgvDokumenty.Visible = true;
                                rgvWplaty.Visible = false;
                                break;
                            case 3: // odpisy
                                setupDokView(trns.rodzaj);  
                                GridViewComboBoxColumn rdColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["OperacjaCzesciowa"];
                                rdColumn.DataSource = null;
                                rdColumn.DataSource = thecontext.SAPKodyOpr.Select(i =>
                                new
                                {
                                    kod = i.kod,
                                    nazwa = i.nazwa,
                                    opgl = i.operacjaGlowna
                                }).Where(i => i.opgl == "N020" || i.opgl == "N030" || i.opgl == "N021" || i.opgl == "N022" || i.opgl == "N023" || i.opgl == "N031" || i.opgl == "FPP0").Distinct()
                                .ToList();
                                rdColumn.ValueMember = "kod";
                                rdColumn.DisplayMember = "nazwa";
                                rdColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
                                this.DokumentyBindingDataSource.DataSource = thecontext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.Transfer_Id == TransferId).OrderBy(b => b.Sprawa.KdNumer).OrderBy(b => b.Sprawa.KdRok).OrderBy(a => a.Sprawa.KnsKsiega).ToList();
                                rgvDokumenty.Dock = DockStyle.Fill;
                                rgvDokumenty.Visible = true;
                                rgvWplaty.Visible = false;
                                break;
                            case 4: // wpłaty
                                rgvDokumenty.Visible = false;
                                rgvWplaty.Dock = DockStyle.Fill;
                                rgvWplaty.Visible = true;
                                this.wplataBindingSource.DataSource = thecontext.Wplata.Where(a => a.Transfer_Id == TransferId).ToList();
                                break;
                            case  7: // zwroty 3/4 opłat
                                setupDokView(trns.rodzaj);  
                                GridViewComboBoxColumn rodzajoprColumn = (GridViewComboBoxColumn)this.rgvDokumenty.Columns["OperacjaCzesciowa"];
                                  rodzajoprColumn.DataSource = null;
                                  rodzajoprColumn.DataSource = thecontext.SAPKodyOpr.Select(i =>
                                new
                                {
                                    kod = i.kod,
                                    nazwa = i.nazwa,
                                    opgl = i.operacjaGlowna
                                }).Where(i => i.opgl == "P020").Distinct()
                                .ToList();
                                  rodzajoprColumn.ValueMember = "kod";
                                  rodzajoprColumn.DisplayMember = "nazwa";
                                  rodzajoprColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
                                this.DokumentyBindingDataSource.DataSource = thecontext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.Transfer_Id == TransferId).OrderBy(b => b.Sprawa.KdNumer).OrderBy(b => b.Sprawa.KdRok).OrderBy(a => a.Sprawa.KnsKsiega).ToList();
                                rgvDokumenty.Dock = DockStyle.Fill;
                                rgvDokumenty.Visible = true;
                                rgvWplaty.Visible = false;
                                break;
                            default:
                                break;
                        }
                        if (validateImports)
                        {
                            validateImports = false;
                             if (this.rgvTransfer.SelectedRows.Count > 0)
                                {

                                    int loopcounter = 0;
                                    foreach (GridViewRowInfo row in this.rgvDokumenty.Rows)
                                    {

                                        this.rlProgress.Text = "Walidacja " + (++loopcounter).ToString();
                                        rlProgress.Refresh();
                                        if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;
                                        string message = ValidateRow(row);
                                        if (message.Length > 0)
                                        {
                                            if (row.Cells["Blad"].Value != null)

                                                row.Cells["Blad"].Value += ";Uzupełnij :" + message.Truncate(200);
                                            else
                                                row.Cells["Blad"].Value = "Uzupełnij :" + message.Truncate(200);

                                        }
                                        else  row.Cells["Blad"].Value = null;
                                        
                                        thecontext.SaveChanges();
                                    }
                                }
                        
                        
                        
                        }

                        this.rgvDokumenty.FilterDescriptors.Clear(); // MasterTemplate.FilterExpressions.Clear();
                        this.rgvDokumenty.FilterDescriptors.Clear(); // MasterTemplate.FilterExpressions.Clear(); 
                        Cursor.Current = Cursors.Default;

                    }
                }

            }
        }



        private void rmiOdpisy_Click(object sender, EventArgs e)
        {
            // import odpisów


            TransferDialog tdl = new TransferDialog();
            tdl.dOd = DateTime.Today;
            tdl.dDo = DateTime.Today;
            this.thecontext.SaveChanges();
            this.thecontext = null;
            this.thecontext = new KnsMigratorEntities();
            tdl.Context = this.thecontext;
            tdl.TypTransfer = "Odpisy";
            tdl.ShowDialog();
            if (tdl.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                imp = new Imports();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.data_od = tdl.dOd;
                imp.theday = tdl.dDo;
                imp.uwagi = tdl.Uwagi;
                imp.newOnly = tdl.newOnly;
                imp.typImport = 3;

                imp.KsiegiKnsLst = tdl.KsiegiKnsLst;
                if (imp.newOnly)
                {
                    if (konfig.StartImportDate == null)
                    {
                        MessageBox.Show("Wprowadź datę rozpoczęcia różnicowego importu danych w konfiguracji");
                        return;
                    }
                    DateTime dt = DateTime.Now;


                    imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
                    imp.theday = DateTime.Now.AddMonths(1);
                    DateTime dod = DateTime.Today.AddMonths(-1);
                    dod = new DateTime(dod.Year, dod.Month, 1);
                    if (dod > imp.data_od)
                        imp.data_od = dod;

                }
                //imp.ImportSaldo();
                Thread thImport = new Thread(imp.ImportOdpis);
                thImport.Start();

                while (!imp.breakIndicator)
                {

                    Thread.Sleep(300);
                    this.rlProgress.Text = imp.progressMsg;
                    this.rlProgress.Refresh();


                }



                //Thread thImportRaty = new Thread(imp.ImportRaty);
                //thImportRaty.Start();
                if (imp.ImportedDocs > 0)
                    this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();
                else
                    MessageBox.Show("Brak dokumentów do importu");
            }

        }

        private void rmiTermWymag_Click(object sender, EventArgs e)
        {
            TransferDialog tdl = new TransferDialog();
            tdl.dOd = DateTime.Today;
            tdl.dDo = DateTime.Today;
            tdl.Context = this.thecontext;
            tdl.TypTransfer = "Terminy Wymagalności";
            tdl.ShowDialog();


            if (tdl.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                imp = new Imports();
                this.thecontext.SaveChanges();
                this.thecontext = null;
                this.thecontext = new KnsMigratorEntities();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.data_od = tdl.dOd;
                imp.theday = tdl.dDo;
                imp.uwagi = tdl.Uwagi;
                imp.newOnly = tdl.newOnly;
                imp.KsiegiKnsLst = tdl.KsiegiKnsLst;
                imp.typImport = 9;
                
                //imp.ImportSaldo();
                Thread thImport = new Thread(imp.ImportTerminWymag);
                thImport.Start();

                while (!imp.breakIndicator)
                {

                    Thread.Sleep(300);
                    this.rlProgress.Text = imp.progressMsg;
                    this.rlProgress.Refresh();
                    
                }

                
            }
        }

       


        private void rmiPrzypisy_clicked(object sender, EventArgs e)
        {
            TransferDialog tdl = new TransferDialog();
            tdl.dOd = DateTime.Today;
            tdl.dDo = DateTime.Today;
            tdl.Context = this.thecontext;
            tdl.TypTransfer = "Przypisy";
            tdl.ShowDialog();
            

            if (tdl.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                imp = new Imports();
                this.thecontext.SaveChanges();
                this.thecontext = null;
                this.thecontext = new KnsMigratorEntities();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.data_od = tdl.dOd;
                imp.theday = tdl.dDo;
                imp.uwagi = tdl.Uwagi;
                imp.newOnly = tdl.newOnly;
                imp.KsiegiKnsLst = tdl.KsiegiKnsLst;
                imp.typImport = 2;
                if (imp.newOnly)
                {
                    if (konfig.StartImportDate == null)
                    {
                        MessageBox.Show("Wprowadź datę rozpoczęcia różnicowego importu danych w konfiguracji");
                        return;
                    }
                    DateTime dt = DateTime.Now;

                    
                    imp.data_od = Convert.ToDateTime(konfig.StartImportDate) ;
                    DateTime dod = DateTime.Today.AddMonths(-1);
                    dod = new DateTime(dod.Year, dod.Month, 1);
                    if (dod > imp.data_od)
                        imp.data_od = dod;

                    imp.theday = DateTime.Now.AddMonths(1);
                    


                }
                //imp.ImportSaldo();
                Thread thImport = new Thread(imp.ImportPrzypis);
                thImport.Start();

                while (!imp.breakIndicator)
                {

                    Thread.Sleep(300);
                    this.rlProgress.Text = imp.progressMsg;
                    this.rlProgress.Refresh();


                }

                //Thread thImportRaty = new Thread(imp.ImportRaty);
                //thImportRaty.Start();
                if (imp.ImportedDocs > 0)
                {
                    validateImports = true;
                    this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();
                }
                else
                    if (imp.ImportedDocs == 0)
                    MessageBox.Show("Brak danych do importu");
                /*
                 foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
                {

                    this.rlProgress.Text = "Walidacja " + (++loopcounter).ToString();
                    rlProgress.Refresh();
                    if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;
                    message =  ValidateRow (row);
                  */
            }
        }

        private void rmiZwroty3_4_Click(object sender, EventArgs e)
        {
            TransferDialog tdl = new TransferDialog();
            tdl.dOd = DateTime.Today;
            tdl.dDo = DateTime.Today;
            tdl.Context = this.thecontext;
            tdl.TypTransfer = "Zwrot 3/4";
            tdl.ShowDialog();


            if (tdl.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                imp = new Imports();
                this.thecontext.SaveChanges();
                this.thecontext = null;
                this.thecontext = new KnsMigratorEntities();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.data_od = tdl.dOd;
                imp.theday = tdl.dDo;
                imp.uwagi = tdl.Uwagi;
                imp.newOnly = tdl.newOnly;
                imp.KsiegiKnsLst = tdl.KsiegiKnsLst;
                imp.typImport = 7;
                if (imp.newOnly)
                {
                    if (konfig.StartImportDate == null)
                    {
                        MessageBox.Show("Wprowadź datę rozpoczęcia różnicowego importu danych w konfiguracji");
                        return;
                    }
                    DateTime dt = DateTime.Now;


                    imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
                    imp.theday = DateTime.Now.AddMonths(1);
                    DateTime dod = DateTime.Today.AddMonths(-1);
                    dod = new DateTime(dod.Year, dod.Month, 1);
                    if (dod > imp.data_od)
                        imp.data_od = dod;

                }
                //imp.ImportSaldo();
                Thread thImport = new Thread(imp.ImportZwrot_3_4);
                thImport.Start();

                while (!imp.breakIndicator)
                {

                    Thread.Sleep(300);
                    this.rlProgress.Text = imp.progressMsg;
                    this.rlProgress.Refresh();


                }

                //Thread thImportRaty = new Thread(imp.ImportRaty);
                //thImportRaty.Start();
                if (imp.ImportedDocs > 0)
                {
                    validateImports = true;
                    this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();
                }
                else
                    if (imp.ImportedDocs == 0)
                        MessageBox.Show("Brak danych do importu");
                /*
                 foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
                {

                    this.rlProgress.Text = "Walidacja " + (++loopcounter).ToString();
                    rlProgress.Refresh();
                    if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;
                    message =  ValidateRow (row);
                  */
            }
        }


        private void rmiUGO_Click(object sender, EventArgs e)
        {
            TransferDialog tdl = new TransferDialog();
            tdl.dOd = DateTime.Today;
            tdl.dDo = DateTime.Today;
            tdl.Context = this.thecontext;
            tdl.TypTransfer = "Uiszczenia Grz.Odp.";
            tdl.ShowDialog();


            if (tdl.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                imp = new Imports();
                this.thecontext.SaveChanges();
                this.thecontext = null;
                this.thecontext = new KnsMigratorEntities();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.data_od = tdl.dOd;
                imp.theday = tdl.dDo;
                imp.uwagi = tdl.Uwagi;
                imp.newOnly = tdl.newOnly;
                imp.KsiegiKnsLst = tdl.KsiegiKnsLst;
                imp.typImport = 6; // uiszczeniegrzywien odpisanych
                if (imp.newOnly)
                {
                    if (konfig.StartImportDate == null)
                    {
                        MessageBox.Show("Wprowadź datę rozpoczęcia różnicowego importu danych w konfiguracji");
                        return;
                    }
                    DateTime dt = DateTime.Now;


                    imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
                    imp.theday = DateTime.Now.AddMonths(1);
                    DateTime dod = DateTime.Today.AddMonths(-1);
                    dod = new DateTime(dod.Year, dod.Month, 1);
                    if (dod > imp.data_od)
                        imp.data_od = dod;

                }
                //imp.ImportSaldo();
                Thread thImport = new Thread(imp.ImportPrzypis);
                thImport.Start();

                while (!imp.breakIndicator)
                {

                    Thread.Sleep(300);
                    this.rlProgress.Text = imp.progressMsg;
                    this.rlProgress.Refresh();


                }

                //Thread thImportRaty = new Thread(imp.ImportRaty);
                //thImportRaty.Start();
                if (imp.ImportedDocs > 0)
                {
                    validateImports = true;
                    this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Id).ToList();
                }
                else
                    if (imp.ImportedDocs == 0)
                        MessageBox.Show("Brak danych do importu");
                /*
                 foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
                {

                    this.rlProgress.Text = "Walidacja " + (++loopcounter).ToString();
                    rlProgress.Refresh();
                    if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;
                    message =  ValidateRow (row);
                  */
            }
        }

        private void rmiSalda_Click(object sender, EventArgs e)
        {  // event - import sald.
            //labelTyp.

            TransferDialog tdl = new TransferDialog();
            tdl.Context = this.thecontext;
            tdl.ShowDialog();
            tdl.TypTransfer = "Salda";
            if (tdl.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (konfig.StartImportDate < tdl.dDo)
                {
                    MessageBox.Show("Nie można zaimportować sald po  ustawionej w konfiguracji dacie importu sald");
                    return ;

                }
                imp = new Imports();
                this.thecontext.SaveChanges();
                this.thecontext = null;
                this.thecontext = new KnsMigratorEntities();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.theday = tdl.dDo;
                imp.uwagi = tdl.Uwagi;
                imp.KsiegiKnsLst = tdl.KsiegiKnsLst;
                imp.typImport = 1;
                //imp.ImportSaldo();
                Thread thImport = new Thread(imp.ImportSaldo);
                thImport.Start();
                while (!imp.breakIndicator)
                {

                    Thread.Sleep(300);
                    this.rlProgress.Text = imp.progressMsg;
                    this.rlProgress.Refresh();


                }
                // Raty
                if (!konfig.skipraty as bool? ?? default(bool))
                {
                    switch (konfig.typKns)
                    {
                        case 0: // currenda
                            imp.ImportRaty();
                            break;
                        case 1: // Zeto
                            imp.ImportRatyHarmonogram();
                            imp.ImportRaty();
                            break;
                        default:
                            break;
                    }
                }

                //Thread thImportRaty = new Thread(imp.ImportRaty);
                //thImportRaty.Start();
                validateImports = true;
                this.TransferBindingDataSource.DataSource =  thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();



            }
        }

        private void rmiKosztyS2017_Click(object sender, EventArgs e)
        {
            // potwierdzenie kosztów
            List<string> lst;
            if (MessageBox.Show("Import raportu z przeniesienia FPP na typ konta umowy wymaga uprzedniego wykonania kopii bezpieczeństwa danych!!!. Jeśli chcesz kontynuować, wybierz przycisk TAK i wskaż loklalizację raportu w formacie csv  ?","Uwaga", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;
            imp = new Imports();
            imp.Context = thecontext;
            imp.Konfig = konfig;
            
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    FileInfo file = new FileInfo(openFileDialog.FileName);

                    if (file.Extension.Equals(".csv"))
                    {
                        imp.fileName = openFileDialog.FileName;
                        // 
                        imp.CreateSchema();
                        // lst = imp.ImportConfirmationKoszty(imp.fileName); 

                        lst = imp.ImportConfirmationFPP(imp.fileName);
                        this.rgvDokumenty.Refresh();
                        if (lst != null && lst.Count > 0)
                        {
                            DispResult dr = new DispResult();
                            dr.SVal =  String.Join(Environment.NewLine, lst);
                            dr.ShowDialog();
                        }
                    }
                }
            }
            Cursor.Current = Cursors.Default;

        }


        private void rmiPotwSaldaSAP_Click(object sender, EventArgs e)
        {
            // trzeba wskazać transfer
            int transferId;
        if (rgvTransfer.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Import potwierdzeń dla wybranego transferu.Czy na pewno chcesz zaimportować potwierdzenia z SAP dla wskazanego transferu ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return ;
                transferId = Convert.ToInt32(rgvTransfer.SelectedRows[0].Cells["Id"].Value);
                int count  = thecontext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.Transfer_Id == transferId).Count();
                if (count == 0)
                { 
                    MessageBox.Show("Brak dokumentów w wybranym transferze");
                     return;
                }   
                Cursor.Current = Cursors.WaitCursor;
                imp = new Imports();
                this.thecontext.SaveChanges();
                this.thecontext = null;
                this.thecontext = new KnsMigratorEntities();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!openFileDialog.FileName.Equals(String.Empty))
                    {
                        FileInfo file = new FileInfo(openFileDialog.FileName);

                        if (file.Extension.Equals(".csv"))
                        {
                            imp.fileName = openFileDialog.FileName;
                            // 
                            imp.CreateSchema();
                         //   imp.ImportConfirmationAll();
                            imp.ImportConfirmationZPSCDDOKS(0, transferId, true);
                            this.rgvDokumenty.Refresh();
                        }
                    }
                }
                Cursor.Current = Cursors.Default;
            }

            else
                MessageBox.Show("Wskaż transfer dla którego chcesz wprowadzić potwierdzenia");

        }

        private void rmiPotwSalda_Click(object sender, EventArgs e)
        {  // import potwierdzeń sald

            imp = new Imports();
            imp.Context = thecontext;
            imp.Konfig = konfig;
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    FileInfo file = new FileInfo(openFileDialog.FileName);

                    if (file.Extension.Equals(".csv"))
                    {
                        imp.fileName = openFileDialog.FileName;
                        // 
                        imp.CreateSchema();
                        imp.ImportConfirmation(0);
                        this.rgvDokumenty.Refresh();
                    }
                }
            }
        }
        /*
        private void rdbBreak_Click(object sender, EventArgs e)
        {
            if (imp != null)
            {
                DialogResult dialresult = MessageBox.Show("Czy chcesz przerwać przetwarzanie ?", "Uwaga", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialresult == DialogResult.Yes)
                    imp.breakIndicator = true;

            }
        }

        */
        private void rbSaveImport_Click(object sender, EventArgs e)
        {
            thecontext.SaveChanges();
        }

        private string ValidateRow(GridViewRowInfo row)
        { ///*********************** dodać walidację sygnatur ze słownikiem  *********************/
            string message ="";
            fldValidStatus fldSave = null;
           try
           {
               foreach (fldValidStatus fld in validArry)
               {
                   int mask;
                   fldSave = fld;
                   switch ((row.Cells["typFakt"].Value).ToString().Trim().ToUpper())
                   {
                       case "GS":
                       case "KS":
                           mask = 1;
                           break;
                       case "GP":
                       case "KP":
                           if ((row.Cells["sprTypKontaUm"].Value as string) == "DO")
                                mask = 8;
                           else
                                mask = 2;
                           break;
                       case "GO":
                       case "KO":
                           mask = 4;
                           break;

                       case "GR":
                       case "KR":
                           mask = 8;
                           break;

                       default: mask = 0;
                           break;

                   }

                   if ((fld.validStatus & mask) > 0)
                   {
                       if (row.Cells[fld.fName].Value == null)
                              message = (message.Length > 0) ? message += "; " + row.Cells[fld.fName].ColumnInfo.HeaderText : row.Cells[fld.fName].ColumnInfo.HeaderText;
                       else
                           if (String.IsNullOrWhiteSpace(row.Cells[fld.fName].Value.ToString().Trim()))
                               message = (message.Length > 0) ? message += "; " + row.Cells[fld.fName].ColumnInfo.HeaderText : row.Cells[fld.fName].ColumnInfo.HeaderText;


                   }
               }
           }
           // Walidacja wiersza 


           catch (Exception ex)
           {
               MessageBox.Show("Błąd podczas wykonywania walidacji " + ex.Message + " " + fldSave.fName);

           }
           
               return message;
           
        }
        
        
        private bool SetEkstrakcjaRow(ref Ekstrakcja ekstr, GridViewRowInfo row, Konfiguracja knf,string typKontaUm = null,string rodzajDok = null )
        {
            string fieldName = "";

            try
            {
                fieldName = "typFakt";
                ekstr.KodOperacji = (row.Cells["typFakt"].Value).ToString().Trim();
                fieldName = "SAPImportPonowne";
                ekstr.SAPImportPonowne = row.Cells["SAPImportPonowne"].Value == null ? "" : (row.Cells["SAPImportPonowne"].Value).ToString().Trim();
                fieldName = "dlFizPraw";
                ekstr.Osoba_fizyczna_Osoba_prawna = (row.Cells["dlFizPraw"].Value).ToString().Trim();
                fieldName = "dlimie";
                ekstr.Imię_Nazwa1 = (row.Cells["dlimie"].Value as string).Trim();
                fieldName = "dlnazwisko";
                ekstr.Nazwisko__Nazwa2 = (row.Cells["dlnazwisko"].Value as string).Trim();
                fieldName = "dlUlica";
                ekstr.Ulica = (row.Cells["dlUlica"].Value as string).Trim();
                fieldName = "dlnrdomu";
                ekstr.Nrdomu = (row.Cells["dlnrdomu"].Value as string).Trim();
                fieldName = "dlnrmieszkania";
                ekstr.Nrmieszkania = row.Cells["dlnrmieszkania"].Value  == null  ? "" : (row.Cells["dlnrmieszkania"].Value as string).Trim();
                fieldName = "dlkodpoczt";
                ekstr.Kodpocztowy = row.Cells["dlkodpoczt"].Value as string;
                fieldName = "dlmiejscowosc";
                ekstr.Miejscowość = (row.Cells["dlmiejscowosc"].Value as string).Trim();
                fieldName = "dlkraj";
                ekstr.Kluczkraju = row.Cells["dlkraj"].Value as string;
                fieldName = "IBAN";
                ekstr.IBAN = row.Cells["dlIban"].Value as string;
                fieldName = "dlNip";
                ekstr.NIP = row.Cells["dlNip"].Value as string;
                fieldName = "dlpesel";
                ekstr.Pesel = row.Cells["dlpesel"].Value as string;
                fieldName = "dlRBN";
                ekstr.KwalifikatordoRBN = row.Cells["dlRBN"].Value as string;
                fieldName = "TypKontaUmowy";
                ekstr.Typkontaumowy = row.Cells["sprTypKontaUm"].Value as string;
                if (String.IsNullOrEmpty(ekstr.Typkontaumowy)) ekstr.Typkontaumowy = typKontaUm == null ? "KN" : typKontaUm; //row.Cells["TypKontaUmowy"].Value as string; // mozę być K1, K2.. K9
                fieldName = "Karta";
                ekstr.Oznaczeniekontaumowy = row.Cells["Karta"].Value as string;
                fieldName = "sprRelacjaKUm";
                ekstr.Relacjakonta = row.Cells["sprRelacjaKUm"].Value as string;
                if (String.IsNullOrEmpty(ekstr.Relacjakonta)) ekstr.Relacjakonta  = "99";//row.Cells["RelacjaKonta"].Value as string;
                ekstr.GrupaJG = ""; //row.Cells["RelacjaKonta"].Value as string;
                ekstr.StandardowaJG = konfig.JednostkaGospodarcza; //""; //
                ekstr.StanowiskoFinansoweKU = konfig.StanowiskoFin;
                ekstr.StanowiskoFianasoweWindyk = konfig.StanowiskoFin;
                ekstr.JeGoWindyk = konfig.JednostkaGospodarcza;
                fieldName = "RodzPUmo";
                ekstr.Rodzajprzedmiotuumowy = row.Cells["RodzPUmo"].Value as string;
                fieldName = "sadorzek";
                ekstr.JednostkaGospodarcza = row.Cells["sadorzek"].Value as string;
                if (ekstr.JednostkaGospodarcza != null)
                {
                    int jego;
                    if (int.TryParse(ekstr.JednostkaGospodarcza, out jego))
                        if (jego > 5000)   // stanowisko finansowe; 
                        {
                            ekstr.StanowiskoFinansowePU = ekstr.JednostkaGospodarcza;
                            string jedngosp = ekstr.JednostkaGospodarcza;
                            SAPSad ss = thecontext.SAPSad.Where(d => d.kod == jedngosp ).FirstOrDefault();
                            ekstr.JednostkaGospodarcza = ss.JEGO;
                         }
                }
                fieldName = "wydzialsekcja";
                ekstr.Nrwydziałuisekcji = row.Cells["wydzialsekcja"].Value as string; // brak załącznika
                fieldName = "repertorium";
                ekstr.Repertorium = row.Cells["repertorium"].Value as string; // brak załącznika
                fieldName = "Numer";
                ekstr.NrSprawy = row.Cells["Numer"].Value.ToString();
                fieldName = "rok";
                ekstr.Rok = row.Cells["rok"].Value.ToString();
                fieldName = "RodzSprawy";
                ekstr.Rodzajsprawy = row.Cells["RodzSprawy"].Value as string;  // brak załącznika
                fieldName = "ltomow";
                ekstr.Ilośćtomów = row.Cells["ltomow"].Value as string;
                fieldName = "sygnatura poprzednia";
                ekstr.SygnaturaPoprzednia = row.Cells["Sygnatura"].Value as string;
                if (ekstr.SygnaturaPoprzednia != null)
                    if (ekstr.SygnaturaPoprzednia.Length > 25)
                        ekstr.SygnaturaPoprzednia = ekstr.SygnaturaPoprzednia.Substring(0, 25);
                fieldName = "DataDokumentu";
                ekstr.Datadokumentu = row.Cells["DataDokumentu"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");     // data orzeczenia orzekajacego ??? czy ma być data przypisu ???
                fieldName = "DataKsiegowania";
                ekstr.Dataksięgowania = row.Cells["DataKsiegowania"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //; Convert.ToDateTime(row.Cells["DataKsiegowania"].Value).ToString("yyyyMMdd"); //Convert.ToDateTime(row.Cells["DataDokumentu"].Value).ToString("yyyyMMdd");
                fieldName = "RodzajDokumentu";
                if (!String.IsNullOrEmpty(row.Cells["RodzajDokumentu"].Value as string))
                    ekstr.Rodzajdokumentu = row.Cells["RodzajDokumentu"].Value as string;
                else
                        ekstr.Rodzajdokumentu = rodzajDok == null ? "NS" : rodzajDok;//row.Cells["RodzajDokumentu"].Value as string;  // stała wartość  "NS"
                fieldName = "Waluta";
                ekstr.Waluta = "PLN";//row.Cells["Waluta"].Value as string;
                fieldName = "KluczUzgodnienia";
                ekstr.Kluczuzgodnienia = "";//row.Cells["KluczUzgodnienia"].Value as string;
                fieldName = "JednostakaGospodarcaWłasna";
                ekstr.JednostkaGospodarcza32 = konfig.JednostkaGospodarcza; //row.Cells["JednostakaGospodarcaWłasna"].Value as string;
                fieldName = "OperacjaGłówna";
                ekstr.Operacjagłówna = row.Cells["OperacjaGlowna"].Value as string;
                fieldName = "OperacjaCzesciowa";
                ekstr.Operacjaczęściowa = row.Cells["OperacjaCzesciowa"].Value as string;
                fieldName = "kwota";
                ekstr.KwotawPLN = Convert.ToDecimal(row.Cells["kwota"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                fieldName = "DataPlatnosci";
                ekstr.Datapłatności = row.Cells["DataPlatnosci"].Value == null ? "" : Convert.ToDateTime(row.Cells["DataPlatnosci"].Value).ToString("yyyyMMdd");
                fieldName = "Stan";
                ekstr.Stannależności = row.Cells["Stan"].Value as string;
                /*
                if (typKontaUm == "KN" && (ekstr.KodOperacji == "GS" || ekstr.KodOperacji == "KS"))
                {
                    string ks;
                    int    nr, rok;
                    Utils.ParseKartaDl(Convert.ToInt32(knf.typKns), ekstr.Oznaczeniekontaumowy, out ks, out nr, out rok);
                    if (rok != null && rok > 0 && rok < (Convert.ToDateTime(row.Cells["DataKsiegowania"].Value)).Year)
                        ekstr.KontoKG = Convert.ToInt64("9100100000");


                }  */  
                if (typKontaUm != "KN") ekstr.KontoKG = null;
                fieldName = "Opis";
                ekstr.Opis = row.Cells["Opis"].Value as string;
                fieldName = "DocGuid";
                ekstr.DocGuid = row.Cells["DocGuid"].Value as System.Guid?;
                fieldName = "SAPKontoPartnera";
                ekstr.NumerPartnera = (row.Cells["SAPKontoPartnera"].Value) as string;
                fieldName = "SAPKontoUmowy";
                ekstr.NumerKontaUmowy = (row.Cells["SAPKontoUmowy"].Value) as string;
                fieldName = "SAPPrzedmiotUmowy";
                ekstr.NumerPrzedmiotuUmowy = (row.Cells["SAPPrzedmiotUmowy"].Value) as string;
                fieldName = "SAPDocId";
                ekstr.NumerDokumentu = (row.Cells["SAPDocId"].Value) as string;
                fieldName = "SAPDocIdRef";
                ekstr.NumerDokumentuReferencyjnego = (row.Cells["SAPDocIdRef"].Value) as string;
                fieldName = "SAPRatyId";
                ekstr.NumerDokumentuPlanRat = (row.Cells["SAPRatyId"].Value) as string;
                fieldName = "RataData1";
                ekstr.RataData1 = row.Cells["RataData1"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData1"].Value).ToString("yyyyMMdd");
                fieldName = "RataData2";
                ekstr.RataData2 = row.Cells["RataData2"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData2"].Value).ToString("yyyyMMdd");
                fieldName = "RataData3";
                ekstr.RataData3 = row.Cells["RataData3"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData3"].Value).ToString("yyyyMMdd");
                fieldName = "RataData4";
                ekstr.RataData4 = row.Cells["RataData4"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData4"].Value).ToString("yyyyMMdd");
                fieldName = "RataData5";
                ekstr.RataData5 = row.Cells["RataData5"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData5"].Value).ToString("yyyyMMdd");
                fieldName = "RataData6";
                ekstr.RataData6 = row.Cells["RataData6"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData6"].Value).ToString("yyyyMMdd");
                fieldName = "RataData7";
                ekstr.RataData7 = row.Cells["RataData7"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData7"].Value).ToString("yyyyMMdd");
                ekstr.RataData8 = row.Cells["RataData8"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData8"].Value).ToString("yyyyMMdd");
                ekstr.RataData9 = row.Cells["RataData9"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData9"].Value).ToString("yyyyMMdd");
                ekstr.RataData10 = row.Cells["RataData10"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData10"].Value).ToString("yyyyMMdd");
                ekstr.RataData11 = row.Cells["RataData11"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData11"].Value).ToString("yyyyMMdd");
                ekstr.RataData12 = row.Cells["RataData12"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData12"].Value).ToString("yyyyMMdd");
                ekstr.RataData13 = row.Cells["RataData13"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData13"].Value).ToString("yyyyMMdd");
                ekstr.RataData14 = row.Cells["RataData14"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData14"].Value).ToString("yyyyMMdd");
                ekstr.RataData15 = row.Cells["RataData15"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData15"].Value).ToString("yyyyMMdd");
                ekstr.RataData16 = row.Cells["RataData16"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData16"].Value).ToString("yyyyMMdd");
                ekstr.RataData17 = row.Cells["RataData17"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData17"].Value).ToString("yyyyMMdd");
                ekstr.RataData18 = row.Cells["RataData18"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData18"].Value).ToString("yyyyMMdd");
                ekstr.RataData19 = row.Cells["RataData19"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData19"].Value).ToString("yyyyMMdd");
                ekstr.RataData20 = row.Cells["RataData20"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData20"].Value).ToString("yyyyMMdd");
                ekstr.RataData21 = row.Cells["RataData21"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData21"].Value).ToString("yyyyMMdd");
                ekstr.RataData22 = row.Cells["RataData22"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData22"].Value).ToString("yyyyMMdd");
                ekstr.RataData23 = row.Cells["RataData23"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData23"].Value).ToString("yyyyMMdd");
                ekstr.RataData24 = row.Cells["RataData24"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData24"].Value).ToString("yyyyMMdd");
                ekstr.RataData25 = row.Cells["RataData25"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData25"].Value).ToString("yyyyMMdd");
                ekstr.RataData26 = row.Cells["RataData26"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData26"].Value).ToString("yyyyMMdd");
                ekstr.RataData27 = row.Cells["RataData27"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData27"].Value).ToString("yyyyMMdd");
                ekstr.RataData28 = row.Cells["RataData28"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData28"].Value).ToString("yyyyMMdd");
                ekstr.RataData29 = row.Cells["RataData29"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData29"].Value).ToString("yyyyMMdd");
                ekstr.RataData30 = row.Cells["RataData30"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData30"].Value).ToString("yyyyMMdd");
                ekstr.RataData31 = row.Cells["RataData31"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData31"].Value).ToString("yyyyMMdd");
                ekstr.RataData32 = row.Cells["RataData32"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData32"].Value).ToString("yyyyMMdd");
                ekstr.RataData33 = row.Cells["RataData33"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData33"].Value).ToString("yyyyMMdd");
                ekstr.RataData34 = row.Cells["RataData34"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData34"].Value).ToString("yyyyMMdd");
                ekstr.RataData35 = row.Cells["RataData35"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData35"].Value).ToString("yyyyMMdd");
                ekstr.RataData36 = row.Cells["RataData36"].Value == null ? "" : Convert.ToDateTime(row.Cells["RataData36"].Value).ToString("yyyyMMdd");

                fieldName = "RataKwota1";
                ekstr.RataKwota1 = Convert.ToDecimal(row.Cells["RataKwota1"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                fieldName = "RataKwota2";
                ekstr.RataKwota2 = Convert.ToDecimal(row.Cells["RataKwota2"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                fieldName = "RataKwota3";
                ekstr.RataKwota3 = Convert.ToDecimal(row.Cells["RataKwota3"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                fieldName = "RataKwota4";
                ekstr.RataKwota4 = Convert.ToDecimal(row.Cells["RataKwota4"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                fieldName = "RataKwota5";
                ekstr.RataKwota5 = Convert.ToDecimal(row.Cells["RataKwota5"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota6 = Convert.ToDecimal(row.Cells["RataKwota6"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota7 = Convert.ToDecimal(row.Cells["RataKwota7"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota8 = Convert.ToDecimal(row.Cells["RataKwota8"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota9 = Convert.ToDecimal(row.Cells["RataKwota9"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota10 = Convert.ToDecimal(row.Cells["RataKwota10"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota11 = Convert.ToDecimal(row.Cells["RataKwota11"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota12 = Convert.ToDecimal(row.Cells["RataKwota12"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota13 = Convert.ToDecimal(row.Cells["RataKwota13"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota14 = Convert.ToDecimal(row.Cells["RataKwota14"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota15 = Convert.ToDecimal(row.Cells["RataKwota15"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota16 = Convert.ToDecimal(row.Cells["RataKwota16"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota17 = Convert.ToDecimal(row.Cells["RataKwota17"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota18 = Convert.ToDecimal(row.Cells["RataKwota18"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota19 = Convert.ToDecimal(row.Cells["RataKwota19"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota20 = Convert.ToDecimal(row.Cells["RataKwota20"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota21 = Convert.ToDecimal(row.Cells["RataKwota21"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota22 = Convert.ToDecimal(row.Cells["RataKwota22"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota23 = Convert.ToDecimal(row.Cells["RataKwota23"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota23 = Convert.ToDecimal(row.Cells["RataKwota23"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota23 = Convert.ToDecimal(row.Cells["RataKwota23"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota23 = Convert.ToDecimal(row.Cells["RataKwota23"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota24 = Convert.ToDecimal(row.Cells["RataKwota24"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota25 = Convert.ToDecimal(row.Cells["RataKwota25"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota26 = Convert.ToDecimal(row.Cells["RataKwota26"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota27 = Convert.ToDecimal(row.Cells["RataKwota27"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota28 = Convert.ToDecimal(row.Cells["RataKwota28"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota29 = Convert.ToDecimal(row.Cells["RataKwota29"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota30 = Convert.ToDecimal(row.Cells["RataKwota30"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota31 = Convert.ToDecimal(row.Cells["RataKwota31"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota32 = Convert.ToDecimal(row.Cells["RataKwota32"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota33 = Convert.ToDecimal(row.Cells["RataKwota33"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota34 = Convert.ToDecimal(row.Cells["RataKwota34"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota35 = Convert.ToDecimal(row.Cells["RataKwota35"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                ekstr.RataKwota36 = Convert.ToDecimal(row.Cells["RataKwota36"].Value).ToString(CultureInfo.GetCultureInfo("en-US"));
                
                if (UserInfo.Id > 0 )
                    ekstr.UserId = UserInfo.Id;
                else 
                    ekstr.UserId = - ExportDetails.IdTransfer;
                ekstr.IsDeleted = false;
                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ekstrakcji dłużnika: " + ekstr.Nazwisko__Nazwa2 + " kolumna " + fieldName + " " + ex.Message);
                return false;
            }


        }

        private bool checkEkstrakcja()
        {
            int ekstrNo;
            try
            {
                if (UserInfo.Id == 0)
                    ekstrNo = thecontext.Ekstrakcja.Count();
                else
                    ekstrNo = thecontext.Ekstrakcja.Where(a => a.UserId == UserInfo.Id).Count();
                if (ekstrNo == 0) return true;
                switch (MessageBox.Show("W zakładce Ekstrakcja znajduje się  " + ekstrNo.ToString() + " zapisów. Czy chcesz je usunąć przed wykonaniem ekstrakcji ? ", "Potwierdź", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                {
                    case DialogResult.Yes:
                        // usuń z zakłądki ekstrakcja
                        Cursor.Current = Cursors.WaitCursor;
                        var query = thecontext.Ekstrakcja.Where(a => a.UserId == UserInfo.Id).ToList();
                        foreach (var q in query)
                        {
                            thecontext.Ekstrakcja.DeleteObject(q);
                        }
                        thecontext.SaveChanges();
                        Cursor.Current = Cursors.Default;
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        return false;

                    default:
                        break;

                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas walidacja: " + ex.Message);
                return false;
            }
        }

        private bool DoEkstrakcja(int mode,  bool extract_only ,bool range = false )
        {
            // mode =1 - silent mode
            int loopcounter = 0;
            int isvalid = 0;
            string message = "";
            int curKsiega;
            bool czyblad = false;
            int typTransfer;
            Konfiguracja knf = thecontext.Konfiguracja.FirstOrDefault();
            // ekstrakcja danych 
            try
            {

                if (range) this.rgvDokumenty.SelectAll(); 
                // walidacja wioerszy
                // dodanie kolumn

                if (this.rgvTransfer.CurrentRow != null)
                    typTransfer = Convert.ToInt32(this.rgvTransfer.CurrentRow.Cells["Rodzaj"].Value);
                else
                    typTransfer = 0;
                foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
                {

                    this.rlProgress.Text = "Walidacja " + (++loopcounter).ToString();
                    rlProgress.Refresh();
                    if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;
                    message = ValidateRow(row);

                    Guid? docguid = row.Cells["DocGuid"].Value as System.Guid?;
                    Ekstrakcja eks = thecontext.Ekstrakcja.Where(a => a.DocGuid == docguid).FirstOrDefault();
                    if (eks != null) message += ";  wiersz o  takim  Id jest na zakładce  Ekstrakcja";
                    if (message.Length > 0)
                    {
                        isvalid++;
                        row.Cells["Blad"].Value = "Uzupełnij :" + message.Truncate(240);

                    }
                    else
                        row.Cells["Blad"].Value = null;


                }
                thecontext.SaveChanges();
                if (isvalid > 0 )
                {
                    if (mode == 0)
                        MessageBox.Show("Wykryto błędy w " + isvalid.ToString() + "  wierszach. Szczegóły zawarte w kolumnie Info ", " Błąd walidacji ", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    else
                        Utils.LogWriter("Wykryto błędy podczas ekstrakcji danych w " + isvalid.ToString() + "  wierszach. Szczegóły zawarte w kolumnie Info ");
                    return false;
                }
                loopcounter = 0;
                // sprawdzenie czy wyczyścić zakładkę ekstrakcja
                if (extract_only)
                {
                    if (!checkEkstrakcja()) return false;
                }
                // odczytanie księgi
                List<KnsKsiegi> knsLst = this.thecontext.KnsKsiegi.ToList();
                KnsKsiegi ksiega;

                foreach (GridViewRowInfo row in this.rgvDokumenty.SelectedRows)
                {


                    this.rlProgress.Text = "Dokument " + (++loopcounter).ToString();
                    rlProgress.Refresh();
                    if (Convert.ToInt16(row.Cells["wyklucz"].Value) == 1) continue;
                    Ekstrakcja ekstr = new Ekstrakcja();
                    curKsiega = Convert.ToInt32(row.Cells["KnsKsiega"].Value);
                    ksiega = knsLst.Where(a => a.Id_Ksiegi == curKsiega).FirstOrDefault();
                    if (ksiega.czyFPP == 1)
                    {
                        if (!SetEkstrakcjaRow(ref ekstr, row, knf, "F1", "FP")) { czyblad = true; break; }
                    } else if (ksiega.czyFPP == 2)
                    {
                        if (!SetEkstrakcjaRow(ref ekstr, row, knf, "KN", "NS")) { czyblad = true; break; }
                    }
                    else if (typTransfer == 7) // zworoty 3/4
                    { if (!SetEkstrakcjaRow(ref ekstr, row, knf, "DO", "DN")) { czyblad = true; break; } }
                    else if (typTransfer <=6 )
                    { if (!SetEkstrakcjaRow(ref ekstr, row, knf, "KN", "NS")) { czyblad = true; break; } }
                    else
                    {if (!SetEkstrakcjaRow(ref ekstr, row, knf)) { czyblad = true; break; }}
                    


                    thecontext.AddToEkstrakcja(ekstr);

                }
                thecontext.SaveChanges();
                this.rgvDokumenty.ClearSelection();
                if (czyblad) return false;
                if (loopcounter > 0 && extract_only)
                {
                    MessageBox.Show(" Dokonano ekstrakcji " + loopcounter.ToString() + "  pozycji \n w  Przejdż na zakładkę <<Ekstrakcja>> aby zapisać dane w zbiorze dyskowym");
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (mode == 0)
                    MessageBox.Show("Błąd podczas ekstrakcji danych w wierszu " + loopcounter.ToString() + "  " + ex.Message + " " + ex.InnerException != null ? ex.InnerException.Message : "");
                else
                    Utils.LogWriter("Błąd podczas ekstrakcji danych w wierszu " + loopcounter.ToString() + "  " + ex.Message + " " + ex.InnerException != null ? ex.InnerException.Message : "");               
                return false;
            }
        
        
        }





        private void rbEkstrkcjaDanych_Click(object sender, EventArgs e)
        {
            DoEkstrakcja(0, true, false);

        }
        
        private void rbImport_Click(object sender, EventArgs e)
        {
            // import danych zakładka Mapowania
            switch (mapMode)
            {
                case "Ksiegi":
                    {
                        Imports imp = new Imports();
                        imp.Konfig = konfig;
                        imp.Context = thecontext;
                        imp.theday = DateTime.Today;
                        imp.ImportKsiega(imp.theday.AddDays(1));
                        this.rgvKsiegi.DataSource = thecontext.KnsKsiegi.OrderBy(a => a.Id_Ksiegi).ToList();
                    }
                    break;
                case "SadWydzialy":
                    GetDate getdtDlg = new GetDate();
                    getdtDlg.ShowDialog();
                    if (getdtDlg.DialogResult != DialogResult.OK) return;
                    {
                        Imports imp = new Imports();
                        imp.Konfig = konfig;
                        imp.Context = thecontext;

                        imp.theday = getdtDlg.theDay.Date;
                        imp.ImportSadWydz(imp.theday.AddDays(1));
                        this.rgvKnsSady.DataSource = thecontext.KnsSad.OrderBy(a => a.Id).ToList();
                    }
                    break;
                case "Komornicy":
                    GetDate getdatDlg = new GetDate();
                    getdatDlg.ShowDialog();
                    if (getdatDlg.DialogResult != DialogResult.OK) return;
                    {
                        Imports imp = new Imports();
                        imp.Konfig = konfig;
                        imp.Context = thecontext;

                        imp.theday = getdatDlg.theDay.Date;
                        imp.ImportKomornik(imp.theday.AddDays(1));
                        this.rgvKomornicy.DataSource = thecontext.KnsKomornik.OrderBy(a => a.Miasto).ToList();
                    }
                    break;
                default:
                    break;
            }

        }



        private void rgvDokumenty_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            string colName;
            string fieldName;

            string[] fNames = { "Dluznik.FizPraw", "ImięNazwa1", "Ulica", "NrDomu", "NrMieszkania", "KodPocztowy", "Miejscowość", "KluczKraju", "IBAN", "KwalifikatorDoRBN", "TypKontaUmowy", "OznaczenieKontaUmowy", "RelacjaKonta", "IdSaduOrzek", "Sygnatura", "JednostkaGospodarcza", "NrWydziałuISekcji", "NrSprawy", "Rok", "DataDokumentu", "DataKsięgowania", "RodzajDokumentu", "Waluta", "KluczUzgodnienia", "JednostakaGospodarcaWłasna", "Czysamoistna", "OperacjaGłówna", "CzęściowoGrzywna", "CzęściowoKoszty", "Grzywna", "Koszty", "DataWymagalności", "RatyKoszty", "RatyGrzywna", "EgzekucjaGrzywny", "EgzekucjaKoszty", "GrzywnyOdroczone", "KosztyOdroczone", "KaraZastępcza", "Sprawa_id", "Ksiega", "intId", "SadKns", "RodzajPrzedmiotuUmowy", "Pesel", "Opis", "Nip", "IlośćTomów", "RodzajSprawy", "Repertorium", "StanNależnościKoszty", "StanNależnościGrzywna" };


            checkboxFlag = true;
            if (!this.rbGroup.IsChecked) return;
            fieldName = rgvDokumenty.CurrentColumn.FieldName;//.FieldName;

            //if (!fNames.Contains(fieldName)) return;
            colName = rgvDokumenty.CurrentColumn.Name;

            foreach (GridViewRowInfo row in rgvDokumenty.SelectedRows)
            {

                row.Cells[colName].Value = e.Value;
                //rgvValid.Rows[row.Index].Cells[colName].Value = e.Value;
            }
        }

        private void rgvDokumenty_CellEditorInitialized(object sender, GridViewCellEventArgs e)
        {
            string opGl;
            if (e.Column.Name == "OperacjaCzesciowa")
            {
                if (this.rgvDokumenty.CurrentRow.Cells["OperacjaGlowna"].Value != DBNull.Value
                    && this.rgvDokumenty.CurrentRow.Cells["OperacjaGlowna"].Value != null)
                {
                    RadDropDownListEditor editor = (RadDropDownListEditor)this.rgvDokumenty.ActiveEditor;
                    RadDropDownListEditorElement editorElement = (RadDropDownListEditorElement)editor.EditorElement;
                    opGl = this.rgvDokumenty.CurrentRow.Cells["OperacjaGlowna"].Value.ToString();
                    editorElement.DataSource = thecontext.SAPKodyOpr.Select(i =>
                                     new
                                     {
                                         kod = i.kod,
                                         nazwa = i.nazwa,
                                         opgl = i.operacjaGlowna
                                     }).Where(i => i.opgl == opGl).Distinct()
                                     .ToList();
                    editorElement.SelectedValue = null;
                    editorElement.SelectedValue = this.rgvDokumenty.CurrentCell.Value;
                }
            }
        }



        private void rmiWplaty_Click(object sender, EventArgs e)
        {

            imp = new Imports();
            imp.Context = thecontext;
            imp.Konfig = konfig;
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    FileInfo file = new FileInfo(openFileDialog.FileName);

                    if (file.Extension.Equals(".csv"))
                    {
                        imp.fileName = openFileDialog.FileName;
                        // 
                        imp.CreateSchema();
                        imp.ImportWplaty();
                        this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();

                    }
                }
            }
        }

        private Image GetImageForType(int status)
        {
            Image img = null;


            return img;
        }

        private void rmiRaty_Click(object sender, EventArgs e)
        {
            TransferDialog tdl = new TransferDialog();
            tdl.dOd = DateTime.Today;
            tdl.dDo = DateTime.Today;
            tdl.Context = this.thecontext;
            tdl.ShowDialog();
            tdl.TypTransfer = "Raty";

            if (tdl.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                imp = new Imports();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.data_od = tdl.dOd;
                imp.typImport = 5;
                imp.theday = tdl.dDo;
                imp.uwagi = tdl.Uwagi;
                imp.KsiegiKnsLst = tdl.KsiegiKnsLst;
                Thread thImport = new Thread(imp.ImportRatRozlicz);
                thImport.Start();

                while (!imp.breakIndicator)
                {

                    Thread.Sleep(300);
                    this.rlProgress.Text = imp.progressMsg;
                    this.rlProgress.Refresh();


                }

                //Thread thImportRaty = new Thread(imp.ImportRaty);
                //thImportRaty.Start();

                this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();
            }
        }

        // Usuwanie 
        private void rbDelTransfer_Click(object sender, EventArgs e)
        {
            // usuwanie danych transferu
            Transfer trn;
            int transferId;
            List<int?> lstspr = new List<int?>();
            List<int?> dlulst = new List<int?>();
            List<int?> doklst = new List<int?>();
            try
            {
                if (rgvTransfer.SelectedRows.Count > 0)
                {
                    if (MessageBox.Show("Czy na pewno chcesz usunąć wskazany transfer wraz z wszystkim związanymi  z nim  dokumentami ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                    {
                        transferId = Convert.ToInt32(rgvTransfer.SelectedRows[0].Cells["Id"].Value);
                        List<Dokument> dokLst = thecontext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.Transfer_Id == transferId).ToList();
                        Cursor.Current = Cursors.WaitCursor;
                        trn = thecontext.Transfer.Where(a => a.Id == transferId).FirstOrDefault();
                        if (trn != null)
                        {
                            foreach (Dokument dok in dokLst)
                            {
                                if (dok.SAPDocId != null)
                                {
                                    if (dok.SAPDocId.Length > 5)
                                    {
                                        MessageBox.Show("Nie można usunąć tego zestawu bo został on przeniesiony do RUP/SAP ");
                                        return;

                                    }
                                }
                            }
                            foreach (Dokument dok in dokLst)
                            {

                                lstspr.Add(dok.Sprawa_Id);
                                dlulst.Add(dok.Dluznik_Id);
                                this.thecontext.DeleteObject(dok);
                            }
                            thecontext.SaveChanges();
                            foreach (int? dlu_id in dlulst)
                            {
                                if (dlu_id > 0)
                                {
                                    Dluznik dl = thecontext.Dluznik.Where(a => a.Id == dlu_id).FirstOrDefault();
                                    if (dl != null)
                                    {
                                        Dokument d = thecontext.Dokument.Where(o => o.Dluznik_Id == dlu_id).FirstOrDefault();
                                        if (d == null)
                                            thecontext.DeleteObject(dl);


                                    }

                                }
                            }
                            // sprawy
                            
                            foreach (int? spr_id in lstspr)
                            {
                                if (spr_id > 0)
                                {
                                    Sprawa spr = thecontext.Sprawa.Where(a => a.Id == spr_id).FirstOrDefault();
                                    if (spr != null)
                                    {
                                        Dokument s = thecontext.Dokument.Where(o => o.Sprawa_Id == spr_id).FirstOrDefault();
                                        if (s == null)
                                            thecontext.Sprawa.DeleteObject(spr);

                                    }

                                }

                            }

                            thecontext.Transfer.DeleteObject(trn);
                            thecontext.SaveChanges();
                            this.TransferBindingDataSource.DataSource =  thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();
                            this.rgvTransfer.DataSource = this.TransferBindingDataSource; // refresh
                        }
                    }
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas próby usuwania transferu " + ex.Message);

            }
        }
        private void rddbImporty_Initialized(object sender, EventArgs e)
        {
            // dodanie dodatkowych pozycji w menu
            RadItem  item;
            item = new RadItem();
            // 
            // rmiSalda
            // 
            /*
            this.rmiSalda.AccessibleDescription = "Sald";
            this.rmiSalda.AccessibleName = "Sald";
            this.rmiSalda.Name = "rmiSalda";
            this.rmiSalda.Tag = "Salda";
            this.rmiSalda.Text = "Sald";
            this.rmiSalda.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.rmiSalda.Click += new System.EventHandler(this.rmiSalda_Click);
            */
            
            
            //item.RadObjectType = 
            //rddbImporty.Items.Add(
        }
        private int checkErrors()
        { 
            int count = 0 ;
           foreach (GridViewRowInfo row in this.rgvDokumenty.Rows)
           {
               if (row.Cells["Blad"] != null && row.Cells["Blad"].Value != null  && !String.IsNullOrEmpty(row.Cells["Blad"].Value.ToString()))
                   count++;
        
           }
            Utils.LogWriter("Znaleziono błędy podczas walidacji ");
           return count;
        }
        public  void ImportSilent()
        {
            

                // usuwanie zbioru - jesli istnieje
            if (File.Exists(RunMode.fileName))
                File.Delete(RunMode.fileName);

                imp = new Imports();
                imp.Context = thecontext;
                imp.Konfig = konfig;
                imp.uwagi = "Przypisy na dzień "+ DateTime.Today.ToShortDateString();
                imp.newOnly = true;
                imp.KsiegiKnsLst = null;
                 if (RunMode.grKsiag > 0)
                {
                imp.KsiegiKnsLst = thecontext.KnsKsiegi.Where(a => a.czymies == RunMode.grKsiag).Select(a => a.Id_Ksiegi.Value).ToList();
                if (imp.KsiegiKnsLst != null && imp.KsiegiKnsLst.Any())
                { int first = imp.KsiegiKnsLst.FirstOrDefault();
                  KnsKsiegi kks =   thecontext.KnsKsiegi.Where(a => a.Id_Ksiegi == first).FirstOrDefault();
                    imp.uwagi += " " + (kks != null ? kks.nazwa : "");
                }
                }
                imp.newOnly = true;
                imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
                imp.theday = DateTime.Now.AddMonths(1);
                imp.typImport = 2;
                if (imp.newOnly)
                {
                if (konfig.StartImportDate == null)
                {
                    Utils.LogWriter("Wprowadź datę rozpoczęcia różnicowego importu danych w konfiguracji");
                    return;
                }
                DateTime dt = DateTime.Now;
                
                imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
                DateTime dod = DateTime.Today.AddMonths(-1);
                dod = new DateTime(dod.Year, dod.Month, 1);
                if (dod > imp.data_od)
                    imp.data_od = dod;

                imp.theday = DateTime.Now.AddMonths(1);



            }


            imp.ImportPrzypis();
                validateImports = true;
                this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();
            if (imp.errorStatus)
            {
                Application.Exit();
                return;
            }
            if (checkErrors() > 0)
            {
                Application.Exit();
                return;
            }
                // Nie było błędów - można wyeksportować zbiór.
                // if (!silentExtract(RunMode.fileName))
               //     Utils.LogWriter("Błąd podczas zapisu zbioru " + RunMode.fileName);
                Application.Exit();
              
        }
        public void ImportOdpisSilent()
        {


            // usuwanie zbioru - jesli istnieje
            if (File.Exists(RunMode.fileName))
                File.Delete(RunMode.fileName);

            imp = new Imports();
            imp.Context = thecontext;
            imp.Konfig = konfig;
            imp.uwagi = "Odpisy na dzień " + DateTime.Today.ToShortDateString();
            imp.newOnly = true;
            imp.KsiegiKnsLst = null;
            if (RunMode.grKsiag > 0)
            {
                imp.KsiegiKnsLst = thecontext.KnsKsiegi.Where(a => a.czymies == RunMode.grKsiag).Select(a => a.Id_Ksiegi.Value).ToList();
                if (imp.KsiegiKnsLst != null && imp.KsiegiKnsLst.Any())
                {
                    int first = imp.KsiegiKnsLst.FirstOrDefault();
                    KnsKsiegi kks = thecontext.KnsKsiegi.Where(a => a.Id_Ksiegi == first).FirstOrDefault();
                    imp.uwagi += " " + (kks != null ? kks.nazwa : "");
                }
            }
            imp.newOnly = true;
            imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
            imp.theday = DateTime.Now.AddMonths(1);
            
            imp.typImport = 3;

            if (imp.newOnly)
            {
                if (konfig.StartImportDate == null)
                {
                    Utils.LogWriter("Wprowadź datę rozpoczęcia różnicowego importu danych w konfiguracji");
                    return;
                }
                DateTime dt = DateTime.Now;

                imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
                DateTime dod = DateTime.Today.AddMonths(-1);
                dod = new DateTime(dod.Year, dod.Month, 1);
                if (dod > imp.data_od)
                    imp.data_od = dod;

                imp.theday = DateTime.Now.AddMonths(1);
            }

            imp.ImportOdpis();
            validateImports = true;
            this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();
            if (imp.errorStatus) Application.Exit();
            if (checkErrors() > 0) Application.Exit();
            // Nie było błędów - można wyeksportować zbiór.
            if (!silentExtract(RunMode.fileName))
                Utils.LogWriter("Błąd podczas zapisu zbioru " + RunMode.fileName);
            Application.Exit();

        }

        public void ImportUGOSilent()
        {


            // usuwanie zbioru - jesli istnieje
            if (File.Exists(RunMode.fileName))
                File.Delete(RunMode.fileName);

            imp = new Imports();
            imp.Context = thecontext;
            imp.Konfig = konfig;
            imp.uwagi = "Przypisy na dzień " + DateTime.Today.ToShortDateString();
            imp.newOnly = true;
            imp.KsiegiKnsLst = null;
            if (RunMode.grKsiag > 0)
            {
                imp.KsiegiKnsLst = thecontext.KnsKsiegi.Where(a => a.czymies == RunMode.grKsiag).Select(a => a.Id_Ksiegi.Value).ToList();
                if (imp.KsiegiKnsLst != null && imp.KsiegiKnsLst.Any())
                {
                    int first = imp.KsiegiKnsLst.FirstOrDefault();
                    imp.uwagi += " " + thecontext.KnsKsiegi.Where(a => a.Id_Ksiegi == first).Select(a => a.nazwa);
                }
            }
            imp.newOnly = true;
            imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
            imp.theday = DateTime.Now.AddMonths(1);
            imp.typImport = 6;

            if (imp.newOnly)
            {
                if (konfig.StartImportDate == null)
                {
                    Utils.LogWriter("Wprowadź datę rozpoczęcia różnicowego importu danych w konfiguracji");
                    return;
                }
                DateTime dt = DateTime.Now;

                imp.data_od = Convert.ToDateTime(konfig.StartImportDate);
                DateTime dod = DateTime.Today.AddMonths(-1);
                dod = new DateTime(dod.Year, dod.Month, 1);
                if (dod > imp.data_od)
                    imp.data_od = dod;

                imp.theday = DateTime.Now.AddMonths(1);
            }

            imp.ImportPrzypis();
            validateImports = true;
            this.TransferBindingDataSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000).OrderByDescending(a => a.Rok).OrderByDescending(a => a.Miesiac).OrderByDescending(a => a.Id).ToList();
            if (imp.errorStatus) Application.Exit();
            if (checkErrors() > 0) Application.Exit();
            // Nie było błędów - można wyeksportować zbiór.
            if (!silentExtract(RunMode.fileName))
                Utils.LogWriter("Błąd podczas zapisu zbioru " + RunMode.fileName);
            Application.Exit();

        }

        public void ExportSilent(int typoper, int mode)
        {
            DateTime d_od = DateTime.Today.AddDays(-3) ;
            // trzy dni wstecz.
            if (typoper > 0)
                this.transferBindingSource.DataSource  = thecontext.Transfer.Where(a => a.rodzaj <= 1000 && a.DataTransferu > d_od && a.rodzaj == typoper).OrderByDescending(a => a.Id).ToList();
            else
                this.transferBindingSource.DataSource = thecontext.Transfer.Where(a => a.rodzaj <= 1000 && a.DataTransferu > d_od ).OrderByDescending(a => a.Id).ToList();
            this.rgvTransfer.DataSource = this.transferBindingSource;
            foreach (GridViewRowInfo row in this.rgvTransfer.Rows)
            {
                int rodzaj  = Convert.ToInt32( row.Cells["Rodzaj"].Value);

                if (rodzaj == 2 || rodzaj == 3 || rodzaj == 6)
                {
                    rgvTransfer.CurrentRow = row;
                    int Id = Convert.ToInt32(row.Cells["Id"].Value);
                    this.DokumentyBindingDataSource.DataSource = thecontext.Dokument.Include("Sprawa").Include("Dluznik").Where(a => a.Transfer_Id == Id).OrderBy(b => b.Sprawa.KdNumer).OrderBy(b => b.Sprawa.KdRok).OrderBy(a => a.Sprawa.KnsKsiega).ToList();
                    this.rgvDokumenty.DataSource = this.DokumentyBindingDataSource;
                    row.IsSelected = true;
                    if (rodzaj == 3)
                        ExportOdpis();
                    else
                        ExportData(mode);
                }
                else
                    continue;

            }
                Application.Exit();

        }

        public void ImportConfirmationsSilent()
        {
            imp = new Imports();
            imp.Context = thecontext;
            imp.Konfig = konfig;
            imp.fileName = RunMode.fileName;
            // 
            imp.CreateSchema();
            imp.ImportConfirmation(1);
            Application.Exit();    
        
        }
        private void ReloadWplaty()
        {
            DateTime dOd, dDo;

            dOd = this.dtWplOd.Value;
            dDo = this.dtWplDo.Value;
            SAPWplatyDataSource.DataSource = this.thecontext.SAPWplaty.Where(a=>a.DataKsiegowania<= dDo && a.DataKsiegowania>= dOd).OrderByDescending(a => a.DataKsiegowania).ThenBy(a => a.PartiaPlatnosci).ThenBy(a => a.NumerPozycjiWPartii).ToList();
            // dodanie podsumowań
            this.rgvValidSaldo.Visible = false;
            this.rgvSAPWplaty.Visible = true;
            this.rgvMasowe.Visible = false;
            this.rgvSAPWplaty.Dock = DockStyle.Fill;
            this.rgvSAPWplaty.DataSource = SAPWplatyDataSource;

        }

        private void fillKNSData(ref SAPWplaty wpl)
        {

            try
            {
                if (!String.IsNullOrWhiteSpace(wpl.NumerKontaUmowy))
                {
                    string dok = wpl.NumerDokumentuRozlicz;
                    string konto = wpl.NumerKontaUmowy;
                    string partner = wpl.NumerPartnera;

                    Sprawa sp = (from m in thecontext.Sprawa
                                 join n in thecontext.Dokument on m.Id equals n.Sprawa_Id
                                 where n.SAPDocId == dok && m.SAPKontoUmowy == konto
                                 select m).FirstOrDefault();
                    if (sp != null)
                    {
                        wpl.KartaDl = sp.Karta;
                        wpl.Sygnatura = sp.Sygnatura;
                        Dluznik d = (from m in thecontext.Dluznik
                                     join n in thecontext.Sprawa on m.Sprawa_Id equals n.Id
                                     where m.SAPKontoPartnera == partner
                                     select m).FirstOrDefault();

                        if (d != null)
                            wpl.Dluznik = d.Imie + " " + d.Nazwisko;


                    }

                    Dokument dSAP = thecontext.Dokument.Where(a => a.SAPDocId == dok).FirstOrDefault();
                    if (dSAP != null)
                    {
                        if (dSAP.typFakt == "KS" || dSAP.typFakt == "KP")
                            wpl.KosztyGrzywna = "koszty"; // koszty
                        else
                            if (dSAP.typFakt == "GS" || dSAP.typFakt == "GP")
                                wpl.KosztyGrzywna = "grzywna"; // koszty
                            else
                                wpl.KosztyGrzywna = "????"; //inne
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + (ex.InnerException != null ? ex.InnerException : null), "Błąd podczas zapisu wpłaty");
                return ;
            }
        
        }

        private bool RozpiszWplatyDb(PaymentClarificationsQueryResponse wplLst, DateTime dt)
        {

            try
            {
                if (wplLst.Wplaty != null && wplLst.Wplaty != null && wplLst.Wplaty.Wplata != null)
                {
                   

                    foreach (var wpl in wplLst.Wplaty.Wplata)
                    {
                        SAPWplaty wplDb = thecontext.SAPWplaty.Where(a => a.PartiaPlatnosci == wpl.PartiaPlatnosci && a.NumerPozycjiWPartii == wpl.NumerPozycjiWPartii).FirstOrDefault();
                        if (wplDb != null)
                        {
                            if (wplDb.DataKsiegowania == dt && wplDb.Kwota == wpl.KwotaCzesciowa && wplDb.NumerDokumentu == wpl.NumerDokumentu1 && wplDb.NumerDokumentuRozlicz == wpl.NumerDokumentu2)
                                ;
                            else
                            {
                                wplDb.DataKsiegowania = dt;
                                wplDb.DataOdczytu = DateTime.Today;
                                wplDb.NumerDokumentu = wpl.NumerDokumentu1;
                                wplDb.NumerDokumentuRozlicz = wpl.NumerDokumentu2;
                                wplDb.NumerKontaUmowy = wpl.NumerKontaUmowy;
                                wplDb.NumerPartnera = wpl.NumerPartnera;
                                wplDb.PrzedmiotUmowy = wpl.PrzedmiotUmowy;
                                wplDb.Kwota = wpl.KwotaCzesciowa;
                                fillKNSData(ref wplDb);
                            }
                        }
                        else
                        {
                            SAPWplaty wplDbx = new SAPWplaty();
                            wplDbx.DataKsiegowania = dt;
                            wplDbx.DataOdczytu = DateTime.Today;
                            wplDbx.NumerDokumentu = wpl.NumerDokumentu1;
                            wplDbx.NumerDokumentuRozlicz = wpl.NumerDokumentu2;
                            wplDbx.NumerKontaUmowy = wpl.NumerKontaUmowy;
                            wplDbx.NumerPartnera = wpl.NumerPartnera;
                            wplDbx.PrzedmiotUmowy = wpl.PrzedmiotUmowy;
                            wplDbx.NumerPozycjiWPartii = wpl.NumerPozycjiWPartii;
                            wplDbx.PartiaPlatnosci = wpl.PartiaPlatnosci;
                            wplDbx.Kwota = wpl.KwotaCzesciowa;
                            fillKNSData(ref wplDbx);
                            thecontext.SAPWplaty.AddObject(wplDbx);
                        }



                    }
                }
                thecontext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + (ex.InnerException != null ? ex.InnerException : null), "Błąd podczas zapisu wpłaty");
                return false;
            }
        
        }

        
        private void  GetWplatyOdDo(DateTime dOd, DateTime dDo)
        {
        PaymentClarificationsQueryResponse wplZaksieg;
        setSAPConnectionParams();
            try
        { // usunięcie wpłat 
                List<SAPWplaty> sapwpl = thecontext.SAPWplaty.Where(a=>a.DataKsiegowania >= dOd && a.DataKsiegowania <= dDo).ToList();
                if (sapwpl != null)
                {
                    foreach (SAPWplaty saw in sapwpl)
                        thecontext.SAPWplaty.DeleteObject(saw);
                }

                thecontext.SaveChanges();


                Cursor.Current = Cursors.WaitCursor;
            while (dOd <= dDo)
            {

                if ((wplZaksieg = ZSRKRequestHelper.PokazWplatyZaksiegowane(dOd, dDo)) != null)
                {
                    if (!RozpiszWplatyDb(wplZaksieg,dOd))
                        break;
                    // rozpisz 
                    

                    dOd = dOd.AddDays(1);
                }
                else
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show( " Błąd komunikacji z ZSRK");
                    return;
                }


            }
            ReloadWplaty();
        }
        catch (Exception ex)
        {
            Cursor.Current = Cursors.Default;
            MessageBox.Show(ex.Message,"Błąd podaczcas odczytu wpłat");
        
        
        }
    
    
        }
        
        

        private bool silentExtract(string filename)
          {
          int Id;
          bool range; 
          // Po0bierz aktualny transfer
          if (this.rgvTransfer.CurrentRow != null)
          {
              Id = Convert.ToInt32(this.rgvTransfer.CurrentRow.Cells["Id"].Value);
             if (Id > 0)
              {

                   range = true; 
                   //rgvTransfer.DataSource = null;
                  ExportDetails.IdTransfer = Id;
                  //trns = thecontext.Transfer.Where(a => a.Id == TransferId).FirstOrDefault();
                  // usunięcie istniejących 

                  Cursor.Current = Cursors.WaitCursor;
                  thecontext.ExecuteStoreCommand("delete  from Ekstrakcja  where UserId = @p0", new SqlParameter { ParameterName = "p0", Value = -Id });

                  if (!DoEkstrakcja(1,false ,range)) { Cursor.Current = Cursors.Default; return false; }

                  EkstrakcjadataSource.DataSource = null;
                  EkstrakcjadataSource.DataSource = thecontext.Ekstrakcja.Where(a => a.UserId == -Id).ToList();
                  rgvEkstrakcja.DataSource = EkstrakcjadataSource;
                  Cursor.Current = Cursors.Default;
                  ExtractToCSV(this.rgvEkstrakcja, true,filename);
                  thecontext.ExecuteStoreCommand("delete  from Ekstrakcja  where UserId = @p0", new SqlParameter { ParameterName = "p0", Value = -Id });
                  return true;
              }
             return false;
          }
          return false;



      }

        private List<SAPRepertorium> genSapRep (int typzastosowanie, int typSad , string repertorium)
        {
         
            List < SAPRepertorium > result = new List<SAPRepertorium> ();
            for (int i = 1; i<8; i++)
            {
                if (typzastosowanie%2 == 1)
                {
                    SAPRepertorium srep = new SAPRepertorium();
                    srep.kod = repertorium;
                    switch (i)
                    {
                        case 1:
                            srep.SymbolRodzajPrzedmiotu = "SROD";
                            break;
                        case 2:
                            srep.SymbolRodzajPrzedmiotu = "SRES";
                            break;
                        case 3:
                            srep.SymbolRodzajPrzedmiotu = "SUBE";
                            break;
                        case 4:
                            srep.SymbolRodzajPrzedmiotu = "SPPR";
                            break;
                        case 5:
                            srep.SymbolRodzajPrzedmiotu = "SKAR";
                            break;
                        case 6:
                            srep.SymbolRodzajPrzedmiotu = "SGOS";
                            break;
                        case 7:
                            srep.SymbolRodzajPrzedmiotu = "SCYW";
                            break;

                    }

                    for (int k = 1; k < 5; k++)
                    {
                        if (typSad % 2 == 1)
                        {
                            switch (k)
                            {
                                case 1:
                                    srep.typSad = "SR";
                                    break;
                                case 2:
                                    srep.typSad = "SO";
                                    break;
                                case 3:
                                    srep.typSad = "SA";
                                    break;
                                case 4:
                                    srep.typSad = "S2";
                                    break;
                                default:
                                    break;

                            }

                            result.Add(srep);
                            string symbol = srep.SymbolRodzajPrzedmiotu;
                            srep = new SAPRepertorium();
                            srep.SymbolRodzajPrzedmiotu = symbol;
                            srep.kod = repertorium;
                            
                        }
                        typSad = typSad / 2;

                    }
                   
                }
                typzastosowanie = typzastosowanie / 2;
                
            }
        
            return result;
        }


        private void ImportRepertorium(bool mojsad = false)
        {
            GetCaseRegistryTypesOutResponse repert;
            setSAPConnectionParams(true);
            try
            { // usunięcie wpłat 
                List<SAPRepertorium> rList = thecontext.SAPRepertorium.ToList();
                if (rList != null)
                {
                    foreach (SAPRepertorium rep in rList)
                        thecontext.SAPRepertorium.DeleteObject(rep);
                }

                thecontext.SaveChanges();

                CaseRegistryTypeData[] repetoria =  ZSRKRequestHelper.ImportujRepertoria();
                
                Cursor.Current = Cursors.WaitCursor;
                List<CaseRegistryTypeData> blackLst = new List<CaseRegistryTypeData>();

                foreach (var rept in repetoria.ToList())
                {   

                    
                    int zastRodz = (rept.ZastosowanieSCYW ? 1 : 0)*64 + (rept.ZastosowanieSGOS ? 1 : 0)*32 + (rept.ZastosowanieSKAR ? 1 : 0)*16 + (rept.ZastosowanieSPPR ? 1 : 0)*8 
                                + (rept.ZastosowanieSUBE ? 1 : 0) *4 + (rept.ZastosowanieSRES ? 1 : 0)*2  + (rept.ZastosowanieSROD ? 1 : 0);

                    int zastSad = (rept.ZastosowanieS2 ? 1 : 0) * 8 + (rept.ZastosowanieSA ? 1 : 0) * 4 + (rept.ZastosowanieSO ? 1 : 0) * 2 + (rept.ZastosowanieSR ? 1 : 0);

                    List<SAPRepertorium> lst = genSapRep(zastRodz, zastSad, rept.Repertorium);
                    if (lst.Count > 0)
                    {
                        foreach (var r in lst)
                        {

                            thecontext.SAPRepertorium.AddObject(r);

                        }
                        thecontext.SaveChanges();                    
                    
                    }

                }
                Cursor.Current = Cursors.Default;
                //ReloadWplaty();
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message, "Błąd podaczas aktualizacji słownika repertoriów");

            }


        }

        private void ImportSady()
        {
            setSAPConnectionParams(true);
            try
            { // usunięcie wpłat 
                List<SAPSad> rList = thecontext.SAPSad.ToList();
               // if (rList != null)
               // {
               //     foreach (SAPSad sad in rList)
               //     {
               //         thecontext.SAPSad.DeleteObject(sad);
               //     }
               // }
               //
               // thecontext.SaveChanges();

                CourtData [] courts = ZSRKRequestHelper.ImportujSady();
                DepartmentData[] departments = ZSRKRequestHelper.ImportujWydzialy();
                Cursor.Current = Cursors.WaitCursor;

                List<SAPSad> ssadLst = new List<SAPSad>();
                foreach (var crt in courts)
                {

                    SAPSad sSad = new SAPSad();
                    sSad.kod = crt.StanowiskoFinansowe;
                    sSad.sad = crt.Opis;
                    List<string> lst = crt.Nazwa.Trim().Split(' ').ToList();
                    int i = 0;
                    string cut = string.Empty;
                    //foreach (string s in lst)
                    //    {
                    //        
                    //        if (i++ < 2) continue;
                    //        cut = (!string.IsNullOrEmpty(cut) ?  cut + " " : cut) + s;
                    //
                    //    }

                    //sSad.miasto = cut.Trim();
                    cut = crt.Nazwa.ToUpper().Trim().Replace("SF W ", "").Replace("SF WE ", "").Replace("SR W ", "").Replace("SR WE ", "").Replace("SO W ", "").Replace("SO WE ", "").Replace("SA W ", "").Replace("SA WE ", "").Replace("SA ", "").Replace("SR ", "").Replace("SO ", "").Replace("SF ", "");
                    sSad.miasto = cut.Trim();
                    sSad.typSad = crt.Nazwa.Trim().Substring(0, 2);
                    sSad.miastSad = sSad.miasto + " " + sSad.sad;
                    if (!crt.StanowiskoFinansowe.StartsWith("5"))
                    {
                        sSad.JEGO = sSad.kod;
                    }
                    else
                    {
                        DepartmentData dep = departments.Where(a => a.StanowiskoFinansowe == crt.StanowiskoFinansowe).FirstOrDefault();
                        if (dep != null)
                            sSad.JEGO = dep.JednostkaGospodarcza;

                    }
                    DateTime tmpDt;
                    if (DateTime.TryParseExact(crt.WazneOd, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDt))
                    {
                        sSad.WazneOd = tmpDt;
                    }
                    else
                    {
                        sSad.WazneOd = new DateTime(2000, 1, 1);

                    }
                    if (DateTime.TryParseExact(crt.WazneDo, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDt))
                    {
                        sSad.WazneDo = tmpDt;
                    }
                    else
                    {
                        sSad.WazneDo = new DateTime(2099, 1, 1);

                    }
                    ssadLst.Add(sSad);
                }
                foreach (var r in ssadLst)
                { 
                    SAPSad ss = thecontext.SAPSad.Where(a=>a.kod == r.kod).FirstOrDefault();
                    if (ss != null)
                    {
                        ss.miasto = r.miasto;
                        ss.miastSad = r.miastSad;
                        ss.sad = r.sad;
                        ss.WazneOd = r.WazneOd;
                        ss.WazneDo = r.WazneDo;
                        ss.typSad = r.typSad;
                        if (!string.IsNullOrWhiteSpace(r.JEGO))
                            ss.JEGO = r.JEGO;
                    }
                    else
                    { 
                        thecontext.SAPSad.AddObject(r);
                    
                    }
                    
                
                }
                thecontext.SaveChanges();
                Cursor.Current = Cursors.Default;
                //ReloadWplaty();
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message, "Błąd podaczas aktualizacji słownika sądów");

            }


        }

        private void verifySAPPwd()
        {
            
            if (UserInfo.role != 1) // jeśli nie admin
            {
                try
                {
                    Konfiguracja knf = this.thecontext.Konfiguracja.FirstOrDefault();
                    if (knf.SAPPwdExpPeriod > 0)
                    {
                        try
                        {
                            setSAPConnectionParams();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            MessageBox.Show("Błąd podczas weryfikacji hasła MEP. Ustawienie parametrów połączenia z ZSRK \r\n" + ex.Message);
                        }
                        bool werStatus = false;
                        try
                        {
                            werStatus = ChngSAPPwd.VerifySAPPwdExpire(knf.SAPPwdExpPeriod.Value);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            MessageBox.Show("Błąd wywołania metody weryfikacji czasu ważności hasła \r\n" + ex.Message);


                        }
                        if (werStatus)
                        {
                            ChangeSAPPwd changeSAPPwd = new ChangeSAPPwd();
                            if (changeSAPPwd.ShowDialog() == DialogResult.OK)
                            {
                                using (KnsMigratorEntities context = new KnsMigratorEntities())
                                {
                                    User usr = context.User.Where(a => a.Id == UserProfile.UserID).FirstOrDefault();
                                    usr.MEPPassword = Utils.Encrypt(changeSAPPwd.NewPassword, EncryptPhase);
                                    context.SaveChanges();
                                    MessageBox.Show("Hasło do ZSRK/MEP zostało zmienione. Używaj go również podczas logowania do systemu ZSRK", " Potwierdzenie zmiany hasła");
                                    UserInfo.MEPPassword = usr.MEPPassword;
                                    setSAPConnectionParams();

                                }



                            }

                        }


                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    MessageBox.Show("Błąd podczas weryfikacji hasła MEP. Sprawdź połączenie z ZSRK \r\n" + ex.Message);


                }

            }

        }

    }
}
