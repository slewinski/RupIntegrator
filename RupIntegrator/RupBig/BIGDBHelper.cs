using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.EntityClient;
using RupBig.ServiceReferenceBigMain;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Globalization;

namespace RupBig
{
   
   public  class BIGDBHelper
    {
        private string errMessage;

        private RadGridView rgvNowe;
        private RadButton rbOdczyt;
        private RadButton rbSend;
        private RadButton rbAddByKdl;
        private TextBox tbKdl;
        private RadGridView rgvUpdate;
        private RadGridView rgvDelDlu;
        private RadPageView rpvNoweOper;
       

     

        public string GetErrMessage()
        {

            return errMessage;
        
        }

        public BIGDBHelper()
        {
            ;
        }

        public  BIGDBHelper(WinBIGMain theWind)
        {
            try

            {
                rgvNowe = (RadGridView)(theWind.Controls.Find("rgvNowe", true).FirstOrDefault());
                rbOdczyt = (RadButton)(theWind.Controls.Find("rbOdczyt", true).FirstOrDefault());
                rbSend = (RadButton)(theWind.Controls.Find("rbSend", true).FirstOrDefault());
                rgvUpdate = (RadGridView)(theWind.Controls.Find("rgvUpdate", true).FirstOrDefault());
                rgvDelDlu = (RadGridView)(theWind.Controls.Find("rgvDelDlu", true).FirstOrDefault());
                rgvDelDlu = (RadGridView)(theWind.Controls.Find("rgvDelDlu", true).FirstOrDefault());
                rbAddByKdl = (RadButton)(theWind.Controls.Find("rbAddByKdl", true).FirstOrDefault());
                tbKdl = (TextBox)(theWind.Controls.Find("tbKdl", true).FirstOrDefault());

                rpvNoweOper = (RadPageView)(theWind.Controls.Find("rpvNoweOper", true).FirstOrDefault());
                rbOdczyt.Click += new EventHandler(rbOdczyt_Click);
                rbSend.Click += new EventHandler(rbSend_Click);
                rbAddByKdl.Click += new EventHandler(rbAddByKdl_Click);
                

                GridViewComboBoxColumn col = (GridViewComboBoxColumn)rgvNowe.Columns["Citizen"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvNowe.Columns["Country"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvUpdate.Columns["Citizen"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvUpdate.Columns["Country"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";


                GridViewSummaryItem summaryItem = new GridViewSummaryItem();
                summaryItem.Name = "Mark";
                summaryItem.Aggregate = GridAggregateFunction.Count;

             
                GridViewSummaryRowItem summaryRowItem = new GridViewSummaryRowItem();
                summaryRowItem.Add(summaryItem);
               
                this.rgvNowe.SummaryRowsTop.Add(summaryRowItem);
                this.rgvDelDlu.SummaryRowsTop.Add(summaryRowItem);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd obsługi elementów interfeejsu " + ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message:""));

            }
        }

        void rbAddByKdl_Click(object sender, EventArgs e)
        {
            this.reloadDS();
            string kdl = tbKdl.Text.Trim();
            if (kdl.Length > 0)
            {


                int rowIndex = -1;
                foreach (GridViewRowInfo row in rgvNowe.Rows)
                {
                    if (row.Cells["KartaDl"].Value.ToString().Equals(kdl))
                    {
                        rowIndex = row.Index;
                        break;
                    }
                }
                if (rowIndex >= 0)
                {
                    rgvNowe.Rows[rowIndex].IsCurrent = true;
                    rgvNowe.Rows[rowIndex].IsSelected = true;

                    GridTableElement tableElement = this.rgvNowe.CurrentView as GridTableElement;
                    GridViewRowInfo row = this.rgvNowe.CurrentRow;


                    if (tableElement != null && row != null)
                    {
                        tableElement.ScrollToRow(row);
                    }
                }

            }
        }
        void rbSend_Click(object sender, EventArgs e)
        {
            List<GridViewRowInfo> lstToDo = new List<GridViewRowInfo>();
            List<string> lstToDoDel = new List<string>();

            int operType = 0;
            string currPageName = rpvNoweOper.SelectedPage.Text;

            if (currPageName.StartsWith("Nowe"))
            {
                operType = 1;
                foreach (GridViewRowInfo row in rgvNowe.Rows)
                {

                    if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                    {
                        lstToDo.Add(row);

                    }
                }
            }
            else
                if (currPageName.StartsWith("Zmiany"))
                {
                    operType = 2;
                    foreach (GridViewRowInfo row in rgvUpdate.Rows)
                    {

                        if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                        {
                            lstToDo.Add(row);

                        }
                    }
                }
                else
                    if (currPageName.StartsWith("Wykreśl"))
                    {
                        operType = 3;
                        foreach (GridViewRowInfo row in rgvDelDlu.Rows)
                        {

                            if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                            {
                                lstToDoDel.Add(row.Cells["LiabilityId"].Value.ToString());

                            }
                        }

                    }
                    else return;
            
            if ((lstToDo.Any() && operType < 3) || (lstToDoDel.Any() && operType == 3))
            {
                Utils.LogWriter("Zaznaczono wiersze do wysyłki");
                bool result;
                SubmitRqHelper srq = new SubmitRqHelper();
                List<extraData> lstextraData = new List<extraData>();
                Package pack = null;
                switch (operType)
                { 
                    case 1:
                        pack = srq.CreateInsUpdtRq(lstToDo, ref lstextraData, operType);
                        break;
                    case 2:
                        pack = srq.CreateInsUpdtRq(lstToDo, ref lstextraData, operType);
                        break;
                    case 3:
                        pack = srq.CreateDelRq(lstToDoDel, ref lstextraData);
                        break;
                
                }
               
                BIG_Package bp;
                if (pack != null)
                {
                    Utils.LogWriter("Wysyłka pakietu " + pack.packageSubmit.packageId); 
                    result = srq.sendPackage(pack);
                    Utils.LogWriter("Po wysyłce pakietu " + pack.packageSubmit.packageId);
                    if (result || true )
                    {

                        using (RupIntegratorEntities context = new RupIntegratorEntities())
                        {
                            try
                            {
                                Utils.LogWriter("Zapis pakietu " + pack.packageSubmit.packageId);
                                if ((bp = this.savePackage(pack, context, lstextraData)) == null)
                                {
                                    MessageBox.Show(this.errMessage);
                                    return;
                                }
                                else
                                {
                                    context.SaveChanges();
                                    updateSourceDb(bp, context);
                                }
                              
                                Utils.LogWriter("Zapisano pakiet " + pack.packageSubmit.packageId);
                                // 
                                //context.BIG_Package.AddObject(bp);
                                context.SaveChanges();
                                Utils.LogWriter("Zapis zatwierdzony pakietu " + pack.packageSubmit.packageId);
                            }
                            catch (Exception ex)
                            {

                                MessageBox.Show("Błąd podczas zapisu pakietu danych " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));

                            }
                        }
                        MessageBox.Show("Wysyłka zakończyła się powodzeniem");
                        this.reloadDS();

                    }
                }
                else
                {
                    MessageBox.Show("Podczas tworzenia pakietu wystąpił błąd. Sprawdź zawartość kolumny Komunikat");
                    return;
                    
                }
            }
     
        }

        private void reloadDS()
        {
            Cursor.Current = Cursors.WaitCursor;

            DataTable dt = this.GetAllToBig();
            if (dt == null)
                return;
            // rgvNowe.DataSource = dt;

            DataTable dtIns;
            DataTable dtUpdt;
            List<vw_BIG_Dluznicy> dluLst;

            //rgvNowe.DataSource = dt;// dtIns;
            this.splitTable(dt, out dtUpdt, out dluLst, out dtIns);

            //;
            foreach (DataColumn col in dtIns.Columns)
            {
                col.ReadOnly = false;
            }
            foreach (DataColumn col in dtUpdt.Columns)
            {
                col.ReadOnly = false;
            }
            rgvNowe.MasterTemplate.FilterDescriptors.Clear();
            rgvUpdate.MasterTemplate.FilterDescriptors.Clear();
            rgvDelDlu.MasterTemplate.FilterDescriptors.Clear();
            
 
            rgvNowe.DataSource = dtIns;
            rgvUpdate.DataSource = dtUpdt;
            rgvDelDlu.DataSource = dluLst;


            Cursor.Current = Cursors.Default;
        
        
        }


        void rbOdczyt_Click(object sender, EventArgs e)
        {
            this.reloadDS();
        }



        public BIG_Package savePackage(Package ps, RupIntegratorEntities context , List<extraData> exdLst = null)
        {
            if (ps == null)
                return null;

            try
            {
                int pkgId = Convert.ToInt32(ps.packageSubmit.packageId.Substring(ps.packageSubmit.packageId.LastIndexOf("/") + 1));
                BIG_Package bp = context.BIG_Package.Where(a => a.IdBIG_Package == pkgId).FirstOrDefault();
                if (bp == null)
                {
                    errMessage = "Błędne oznaczenie pakietu";
                    return null;
                }
                bp.PackageFullId = ps.packageSubmit.packageId;
                bp.SentDate = DateTime.Now;



                foreach (Operation op in ps.packageSubmit.operation)
                {
                    BIG_InfoOperation infop = new BIG_InfoOperation();
                    infop.OperationId = op.OperationId;
                    EconomicInformation einf = new EconomicInformation();
                    if (exdLst != null)
                    {
                        infop.KartaDl = exdLst.Where(a => a.dataname == "KartaDl" && a.operId == op.OperationId).Select(a => a.datavalue).FirstOrDefault();  //!!!!!!!!
                        string idks = exdLst.Where(a => a.dataname == "IdKsiega" && a.operId == op.OperationId).Select(a => a.datavalue).FirstOrDefault();  //!!!!!!!!
                        infop.Citizenship = exdLst.Where(a => a.dataname == "Citizen" && a.operId == op.OperationId).Select(a => a.datavalue).FirstOrDefault();  //!!!!!!!!
                        if (!String.IsNullOrWhiteSpace(idks))
                        {
                            infop.IdKsiega = Convert.ToInt32(idks);

                        }
                    }

                    if (op.addInformation != null)
                    {
                        infop.OperType = 1;
                        einf = op.addInformation;
                    }
                    if (op.updateInformation != null)
                    {
                        infop.OperType = 2;
                        einf = op.updateInformation;
                    }
                    if (op.deleteInformation != null)
                    {
                        infop.OperType = 3;
                        infop.DebatorID = op.deleteInformation.debtorId;
                        infop.LiabilityId = op.deleteInformation.liabilityId;
                        BIG_InfoOperation bopSrc = context.BIG_InfoOperation.Where(a => a.LiabilityId == infop.LiabilityId && (a.OperType == 1 || a.OperType == 2)).OrderByDescending(a => a.IdBIG_InfoOperation).FirstOrDefault();
                        if (bopSrc != null)
                        {
                            infop.Forename = bopSrc.Forename;
                            infop.Sygnatura = bopSrc.Sygnatura;
                            infop.Surename = bopSrc.Surename;


                        }

                    }
                    if (op.blockInformation != null)
                    {
                        infop.OperType = 4;
                        infop.DebatorID = op.blockInformation.debtorId;
                        infop.LiabilityId = op.blockInformation.liabilityId;
                        infop.ShareSuspensionFinalDate = op.blockInformation.shareSuspensionFinalDate;
                    }
                    if (op.unblockInformation != null)
                    {
                        infop.OperType = 5;
                        infop.DebatorID = op.unblockInformation.debtorId;
                        infop.LiabilityId = op.unblockInformation.liabilityId;

                    }
                    if (infop.OperType == 1 || infop.OperType == 2)
                    {
                        infop.AdjudicatingBody = einf.liability.legalTitle.enforceableTitle.adjudicatingBody;
                        infop.IssueDate = einf.liability.legalTitle.enforceableTitle.issueDate;
                        infop.Sygnatura = einf.liability.legalTitle.enforceableTitle.enforceableTitleId;

                        infop.ArrearsAmount = einf.liability.arrearsAmount;
                        infop.ArrearsRiseDate = einf.liability.arrearsRiseDate;
                        if (einf.debtor.corespondenceAddress != null)
                        {
                            infop.CA_City = einf.debtor.corespondenceAddress.city;
                            infop.CA_Country = einf.debtor.corespondenceAddress.country.ToString();
                            infop.CA_HouseNumber = einf.debtor.corespondenceAddress.houseNumber;
                            infop.CA_LocalNumber = einf.debtor.corespondenceAddress.localNumber;
                            infop.CA_Postcode = einf.debtor.corespondenceAddress.postcode;
                            infop.CA_Street = einf.debtor.corespondenceAddress.street;
                        }
                        if (einf.debtor.residenceAddress != null)
                        {
                            infop.RA_City = einf.debtor.residenceAddress.city;
                            infop.RA_Country = einf.debtor.residenceAddress.country.ToString();
                            infop.RA_HouseNumber = einf.debtor.residenceAddress.houseNumber;
                            infop.RA_LocalNumber = einf.debtor.residenceAddress.localNumber;
                            infop.RA_Postcode = einf.debtor.residenceAddress.postcode;
                            infop.RA_Street = einf.debtor.residenceAddress.street;
                        }
                        if (einf.debtor.debtorIdentity.foreignCitizenIdentity != null)
                        {
                            if (einf.debtor.debtorIdentity.foreignCitizenIdentity.document != null && einf.debtor.debtorIdentity.foreignCitizenIdentity.document.documentTypeSpecified)
                            {
                                if (string.IsNullOrEmpty(infop.Citizenship))
                                    infop.Citizenship = einf.debtor.residenceAddress.country.ToString();

                                infop.DocumentNumber = einf.debtor.debtorIdentity.foreignCitizenIdentity.document.documentNumber;
                                infop.DocumentType = einf.debtor.debtorIdentity.foreignCitizenIdentity.document.documentType.ToString();
                            }
                        }
                        else
                        {
                            infop.Citizenship = "POL";
                            if (einf.debtor.debtorIdentity.polishCitizenIdentity.document != null && einf.debtor.debtorIdentity.polishCitizenIdentity.document.documentTypeSpecified)
                            {
                                infop.DocumentNumber = einf.debtor.debtorIdentity.polishCitizenIdentity.document.documentNumber;
                                infop.DocumentType = einf.debtor.debtorIdentity.polishCitizenIdentity.document.documentType.ToString();

                            }
                            infop.Pesel = einf.debtor.debtorIdentity.polishCitizenIdentity.pesel;
                        }
                        infop.Forename = einf.debtor.forename;
                        infop.Surename = einf.debtor.surename;
                        infop.DebatorID = einf.debtor.debtorId;

                        infop.Currency = einf.liability.currency.ToString();
                        infop.DataPrzypisu = DateTime.Now;


                        infop.DisputedAmount = (einf.liability.disputedAmountSpecified ? einf.liability.disputedAmount : 0);
                        if (einf.liability.liabilityAmountSpecified)
                        {
                            infop.LiabilityAmount = einf.liability.liabilityAmount;

                        }
                        infop.LiabilityId = einf.liability.liabilityId;
                        infop.LiabilityType = einf.liability.liabilityType.ToString();
                        if (einf.liability.paymentRequestDispatchDateSpecified)
                            infop.PaymentRequestDispatchDate = einf.liability.paymentRequestDispatchDate;
                        if (einf.liability.shareSuspensionFinalDateSpecified)
                            infop.ShareSuspensionFinalDate = einf.liability.shareSuspensionFinalDate;


                        infop.InstitutionDataShareRestricted = false;



                    }


                    bp.BIG_InfoOperation.Add(infop);

                }
                // zapisanie BIG_Status operacji
                foreach (Credentials cr in ps.credentials)
                {


                    string bigName = cr.big_id.ToString();
                    int bigId = context.BIG_Big.Where(a => a.BIGID == bigName).Select(a => a.IdBig).FirstOrDefault();
                    // niopotwierdzony

                    foreach (BIG_InfoOperation bio in bp.BIG_InfoOperation)
                    {
                        BIG_Oper_Status bigstat = new BIG_Oper_Status();
                        bigstat.dProba = bp.SentDate;
                        bigstat.IdBIG_Big = bigId;
                        bigstat.Status = 0;
                        bio.BIG_Oper_Status.Add(bigstat);
                        bp.BIG_Oper_Status.Add(bigstat);
                    }

                    BIG_Package_User bpu = new BIG_Package_User();
                    bpu.IdBIG_User = context.BIG_User.Where(a => a.IdBIG == bigId && a.BigUserName == cr.userId).Select(a => a.IdBigUser).FirstOrDefault();
                    bp.BIG_Package_User.Add(bpu);
                }
                // utworzenie statusów
                // dodanie userów

                return bp;
            }
            catch (Exception ex)
            { 
                MessageBox.Show("Błąd podczas zapisu pakietu w bazie " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message:""));
                return null;
            }
        }



        public bool updateSourceDb(BIG_Package bp, RupIntegratorEntities context)
        {
            
            try
            {
                if (bp == null)
                {
                    errMessage = "Błędny pakiet";
                    return false;
                }


                List<BIG_InfoOperation> lbio = context.BIG_InfoOperation.Where(a => a.IdBIGPackage == bp.IdBIG_Package).ToList();
                if (lbio == null && !lbio.Any())
                    return false;
                SqlConnection con;
                SqlCommand storedProcCommand;
                SqlDataReader rdr;

                    Konfiguracja Konfig = context.Konfiguracja.FirstOrDefault();
                    string kdl = tbKdl.Text.Trim();
                    string ConnectionString = (Konfig.typKns == 2) ? Utils.BuildMyConnectionString(context) : ((EntityConnection)context.Connection).StoreConnection.ConnectionString;  //KnsMigrator.Properties.Settings.Default.KnsMigratorConnectionString;
                    con = new SqlConnection(ConnectionString);
                    con.Open();
                    storedProcCommand = new SqlCommand("sp_KNSToBIGNotify_CR", con);
                    storedProcCommand.CommandType = CommandType.StoredProcedure;
                    string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                    storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                    //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                    storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                    storedProcCommand.Parameters.Add("@dzien", DateTime.Today);
                storedProcCommand.Parameters.Add("@ksiega", 0);
                storedProcCommand.Parameters.Add("@kartadl", "");

                storedProcCommand.Connection = con;
                    storedProcCommand.CommandTimeout = 600;


                   


                foreach (BIG_InfoOperation bio in lbio)
                {
                    if (bio.OperType == 1) // jeśli dodawanie

                    {
                        storedProcCommand.Parameters["@ksiega"].Value =  bio.IdKsiega;
                        storedProcCommand.Parameters["@kartadl"].Value =  bio.KartaDl;
                        rdr = storedProcCommand.ExecuteReader();


                    }


                }

                
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogWriter("Brak procedury aktualizacji lub błąd zapisu");
                return false;
            }
        }


        public void splitTable( DataTable inTable,  out DataTable UpdateTable, out List<vw_BIG_Dluznicy> delLst, out DataTable InsertTable)
       {

           InsertTable = inTable.Clone();
           UpdateTable = inTable.Clone();
           DataRow dtRowToForceAdd;
           bool forceAdd = false;
          delLst = new List<vw_BIG_Dluznicy>();
          List<string> activeLiability = new List<string>();
            DataTable Unchanged = new DataTable();
            try
            {
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {

                     foreach (DataColumn col in inTable.Columns)
                     {
                         col.ReadOnly = false;                    
                     }
    
                    foreach (DataRow dtRow in inTable.Rows)
                    {
                        decimal saldoKoszty = Convert.ToDecimal(dtRow["SaldoKoszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                        decimal saldoGrzywna = Convert.ToDecimal(dtRow["SaldoGrzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                        decimal przypisKoszty = Convert.ToDecimal(dtRow["Koszty"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                        decimal przypisGrzywna = Convert.ToDecimal(dtRow["Grzywna"].ToString().Replace(".", ","), CultureInfo.GetCultureInfo("pl-PL"));
                        string KartaDl = dtRow["KartaDl"].ToString();
                        forceAdd = false;
                        if (tbKdl.Text.Trim().Length > 3 && KartaDl.Trim().ToUpper() == tbKdl.Text.Trim().ToUpper())
                        {
                            dtRowToForceAdd = dtRow;
                            forceAdd = true;
                        }
                        
                        int IdNal = Convert.ToInt32(dtRow["IdNal"].ToString());
                        string liabilityKs = Utils.getLiabilId(KartaDl, "001", 1, IdNal);
                        string liabilityGrz = Utils.getLiabilId(KartaDl, "001", 0, IdNal);
                        if (saldoGrzywna > 0 || saldoKoszty > 0 )
                        {
                            BIG_InfoOperation biog = null;
                            BIG_InfoOperation biok = null;
                            if (KartaDl == "814/2016/W")
                            {
                                ;
                            
                            }
                            
                            if (saldoGrzywna > 0 )
                            {
                                biog = context.BIG_InfoOperation.Where(a => a.LiabilityId == liabilityGrz && (a.OperType == 1 || a.OperType == 2)).OrderByDescending(a => a.IdBIG_InfoOperation).FirstOrDefault();
                                activeLiability.Add(liabilityGrz);

                            }
                            if (saldoKoszty > 0 )
                              {
                               biok = context.BIG_InfoOperation.Where(a => a.LiabilityId == liabilityKs && (a.OperType == 1 || a.OperType == 2)).OrderByDescending(a => a.IdBIG_InfoOperation).FirstOrDefault();
                               activeLiability.Add(liabilityKs);
                              }

                            if (biog == null && biok == null)
                            {

                                InsertTable.ImportRow(dtRow);
                                continue;

                            }
                            else
                                if (forceAdd && MessageBox.Show("Należność z karty " + KartaDl + " już była zarejestrowana w systemie. Czy na pewno chcesz ją dodać ponownie ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {
                                    InsertTable.ImportRow(dtRow);
                                    continue;

                                }
                                else
                                    if (forceAdd)
                                            continue;
                                    

                            if (saldoKoszty > 0 && biok != null && saldoKoszty == biok.ArrearsAmount)
                            {
                            saldoKoszty = 0;
                            dtRow["SaldoKoszty"] = 0 ;
                            }   
                        if (saldoGrzywna > 0 && biog != null && saldoGrzywna == biog.ArrearsAmount)
                            {
                            saldoGrzywna = 0;
                            dtRow["SaldoGrzywna"] = 0 ;
                            }   
                             if (saldoGrzywna > 0 && biog == null)
                            {
                                InsertTable.ImportRow(dtRow);
                                                           
                            }
                            if (saldoKoszty > 0 && biok == null)
                            {
                                InsertTable.ImportRow(dtRow);
                                                            
                            }

                            if ((saldoGrzywna > 0  && biog!= null &&  saldoGrzywna != biog.ArrearsAmount )  ||  (saldoKoszty > 0  && biok!= null &&  saldoKoszty != biok.ArrearsAmount ))
                                    {
                                    UpdateTable.ImportRow(dtRow);
                                    continue;
                                    }


                        }
                        

                    }
                    // pozstają do usunięcia 
                    List<vw_BIG_Dluznicy> dlulst = context.vw_BIG_Dluznicy.ToList();
                    foreach (vw_BIG_Dluznicy dlu in dlulst)
                    {
                        if (!activeLiability.Contains(dlu.LiabilityId))
                        {
                            delLst.Add(dlu);

                        }


                    }

                }
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message +  (ex.InnerException != null ? ex.InnerException.Message :""));
            
            
            
            }

          }
       


        private DataTable GetAllToBig()
        {
            SqlDataReader rdr = null;
            SqlConnection con = null;
            const int counter = 200;
            DataTable dt = new DataTable();
            DataRow currentdtr = null;
            SqlCommand storedProcCommand;
            string knsks = "";

            //  Thread th = new Thread(progressWindow);
            // th.Start();

            try
            {
                // Open connection to the database

                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    Konfiguracja Konfig = context.Konfiguracja.FirstOrDefault();
                    List<KnsKsiegi> KnsLst = context.KnsKsiegi.Where(a => a.rodzajPrzedmiotu == "SKAR" || a.oprKosztFiz == "TAK").ToList();
                    if (KnsLst == null || !KnsLst.Any())
                    {
                        MessageBox.Show("Brak ksiąg należności - użyj opcji mapowania");
                        return null;
                    }
                    foreach (KnsKsiegi k in KnsLst)
                    {
                        if (!string.IsNullOrWhiteSpace(knsks))
                            knsks += ",";
                        knsks += k.Id_Ksiegi.ToString();
                    
                    }
                    string kdl = tbKdl.Text.Trim();
                    string ConnectionString = (Konfig.typKns == 2) ? Utils.BuildMyConnectionString(context) : ((EntityConnection)context.Connection).StoreConnection.ConnectionString;  //KnsMigrator.Properties.Settings.Default.KnsMigratorConnectionString;
                    con = new SqlConnection(ConnectionString);
                    con.Open();
                    storedProcCommand = new SqlCommand("sp_KNSToBIG_CR", con);
                    storedProcCommand.CommandType = CommandType.StoredProcedure;
                    string jg = (Konfig.StanowiskoFin == null) ? Konfig.JednostkaGospodarcza : (Konfig.StanowiskoFin.Trim().Length == 4) ? Konfig.StanowiskoFin : Konfig.JednostkaGospodarcza;
                    storedProcCommand.Parameters.Add("@sourcesrv", (String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias) + (Konfig.typKns == 2 ? "@@" + jg : ""));
                    //storedProcCommand.Parameters.Add("@sourcesrv", String.IsNullOrEmpty(Konfig.srvAlias) ? Konfig.srvName : Konfig.srvAlias);
                    storedProcCommand.Parameters.Add("@dbname", Konfig.DbName);
                    storedProcCommand.Parameters.Add("@dzien",DateTime.Today);
                    storedProcCommand.Parameters.Add("@ksiegi", knsks);
                    storedProcCommand.Parameters.Add("@sygnmask", DBNull.Value);
                    storedProcCommand.Parameters.Add("@kartadl", kdl);

                    storedProcCommand.Connection = con;
                    storedProcCommand.CommandTimeout = 600;


                    rdr = storedProcCommand.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(rdr);
                        return dt;
                    }
                    else
                        return null;



                }


            }
            catch (Exception ex)
            {
                
                errMessage = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                MessageBox.Show(errMessage);
                return null;
            }


        }
    }
}
