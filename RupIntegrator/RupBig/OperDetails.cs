using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Drawing.Printing;

namespace RupBig
{
    public partial class OperDetails : Form
    {
        public int operId { get; set; }
        private BIG_InfoOperation opr;
        private IQueryable<BIG_Oper_Status> opr_status_lst;

        PrintDocument printdoc1 = new PrintDocument();
        PrintPreviewDialog previewdlg = new PrintPreviewDialog();
        Telerik.WinControls.UI.SplitPanel pannel = null;


        public OperDetails()
        {
            InitializeComponent();
            
            printdoc1.PrintPage += new PrintPageEventHandler(printdoc1_PrintPage);
            pannel = splitPanelPrn;
            ExpressionFormattingObject obj = new ExpressionFormattingObject("Cond1", "Status = 0", false);
            obj.CellBackColor = Color.LightGray;
            obj.CellForeColor = Color.LightGray;
            this.rgvOprDet.Columns["Status"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond2", "Status < 0 ", false);
            obj.CellBackColor = Color.Red;
            obj.CellForeColor = Color.Red;
            this.rgvOprDet.Columns["Status"].ConditionalFormattingObjectList.Add(obj);
            obj = new ExpressionFormattingObject("Cond3", "Status > 0 ", false);
            obj.CellBackColor = Color.Green;
            obj.CellForeColor = Color.Green;
        
            this.rgvOprDet.Columns["Status"].ConditionalFormattingObjectList.Add(obj);
        }

        public void Retrieve(int oId)
        {
            if (oId > 0)
            {

                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    opr = context.BIG_InfoOperation.Where(a => a.IdBIG_InfoOperation == operId).FirstOrDefault();
                    opr_status_lst = context.BIG_Oper_Status.Include("BIG_Big").Where(a => a.IdBIG_InfoOperation == oId);

                    GridViewComboBoxColumn stColumn = (GridViewComboBoxColumn)rgvOprDet.Columns["IdBIG_Big"];

                    stColumn.DataSource = context.BIG_Big.ToList();
                    stColumn.ValueMember = "IdBig";
                    stColumn.DisplayMember = "BIGID";


                    tbImie.Text = opr.Forename;
                    tbNazwisko.Text = opr.Surename;
                    tbPesel.Text = opr.Pesel;
                    tbDowod.Text = opr.DocumentNumber;
                    tbCitizen.Text = opr.Citizenship;
                    tbCity.Text = opr.RA_City;
                    tbAdrCountry.Text = opr.RA_Country;
                    tbPostCode.Text = opr.RA_Postcode;
                    tbStreet.Text = opr.RA_Street;
                    tbHouseNumber.Text = opr.RA_HouseNumber;
                    tbLocalNumber.Text = opr.RA_LocalNumber;
                    tbDebtorId.Text = opr.DebatorID;
                    switch (opr.OperType)
                    {
                        case 1:
                            lbtyp.Text = "Dodanie";
                            break;
                        case 2:
                            lbtyp.Text = "Aktualizacja";
                            break;
                        case 3:
                            lbtyp.Text = "Usunięcie";
                            break;
                    }
                    tbPackageId.Text = opr.BIG_Package.PackageFullId;
                    lbSentDate.Text = opr.BIG_Package.SentDate.Value.ToString("yyyy-MM-dd HH:mm");
                    tbLiabilityId.Text = opr.LiabilityId;
                    this.rgvOprDet.DataSource = opr_status_lst.ToList();
                    if (opr.OperType > 2) return;

                    switch ((ServiceReferenceBigMain.liabilityTypeEnum)Enum.Parse(typeof(ServiceReferenceBigMain.liabilityTypeEnum), opr.LiabilityType, false))
                    {
                        case ServiceReferenceBigMain.liabilityTypeEnum.fine:
                            tbLiabilityType.Text = "Grzywna";
                            break;
                        case ServiceReferenceBigMain.liabilityTypeEnum.courtCosts:
                            tbLiabilityType.Text = "Koszty";
                            break;
                        case ServiceReferenceBigMain.liabilityTypeEnum.compensation:
                            tbLiabilityType.Text = "Nawiązka na rzecz S.P.";
                            break;
                        case ServiceReferenceBigMain.liabilityTypeEnum.forfeit:
                            tbLiabilityType.Text = "Przedmiot przepadku";
                            break;
                        case ServiceReferenceBigMain.liabilityTypeEnum.monetaryPenalty:
                            tbLiabilityType.Text = "Kara pieniężna";
                            break;
                        case ServiceReferenceBigMain.liabilityTypeEnum.cashBenefits:
                            tbLiabilityType.Text = "Świadczenie pieniężne";
                            break;
                        case ServiceReferenceBigMain.liabilityTypeEnum.compensatoryDamages:
                            tbLiabilityType.Text = "Naprawienie szkody";
                            break;
                    }

                    tbKwotaNal.Text = opr.LiabilityAmount.ToString();
                    tbSaldoNal.Text = opr.ArrearsAmount.ToString();
                    tbCurrency.Text = opr.Currency;
                    tbDataNal.Text = opr.ArrearsRiseDate.Value.Date.ToString("yyyy-MM-dd");
                    tbDataPraw.Text = opr.IssueDate.Value.Date.ToString("yyyy-MM-dd");
                    tbDataWezw.Text = (opr.PaymentRequestDispatchDate != null ? opr.PaymentRequestDispatchDate.Value.Date.ToString("yyyy-MM-dd") :"");
                    tbSygnatura.Text = opr.Sygnatura;
                    tbSąd.Text = opr.AdjudicatingBody;






                }
            }
        
        }

        private void OperDetails_Load(object sender, EventArgs e)
        {
            this.Retrieve(operId);

            }
        Bitmap MemoryImage;
        public void GetPrintArea(Panel pnl)
        {
            MemoryImage = new Bitmap(pnl.Width, pnl.Height);
            pnl.DrawToBitmap(MemoryImage, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (MemoryImage != null)
            {
                e.Graphics.DrawImage(MemoryImage, 0, 0);
                base.OnPaint(e);
            }
        }
        void printdoc1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Rectangle pagearea = e.PageBounds;
            e.Graphics.DrawImage(MemoryImage, (pagearea.Width / 2) - (this.pannel.Width / 2), this.pannel.Location.Y);
        }
        public void GetPrintArea(Telerik.WinControls.UI.SplitPanel pnl)
        {
            MemoryImage = new Bitmap(pnl.Width, pnl.Height);
            pnl.DrawToBitmap(MemoryImage, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }

        public void Print(bool preview )
        {
       // 1 - preview
            // 0 - print
            GetPrintArea(pannel);
            if (preview)
            {
                previewdlg.Document = printdoc1;
                previewdlg.ShowDialog();
            }
            else
            {
              
                printdoc1.Print();
            }
        }


        private void rbPrint_Click(object sender, EventArgs e)
        {
            Print(true);

        }

    

        }

        
    }

