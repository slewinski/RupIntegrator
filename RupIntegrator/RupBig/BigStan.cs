using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.EntityClient;
using RupBig.ServiceReferenceBigMain;
using System.Drawing;

namespace RupBig
{
    public class BigStan
    {

        // zarządzanie stanem

        private string errMessage;

        private RadGridView rgvOperacje;
        private RadGridView rgvMyDlu;
        private RadButton rbOdczytOper;
        private RadButton rbSend;
        private RadButton rbCheck;
        private RadButton rbUpdateOper;
        private RadButton rbDelOpr;
        private RadPageView rpvOperacje;
        private RadButton rbDetailsOper;
        private RadButton rbAddInNew;
        private RadButton rbSendAgain;
        private RadButton rbDetailsDlu;
        private Button bt1;

        private RadButton rbPrint;
        private RadGridView rgvDelDlu;
        
        private RadDateTimePicker rdOd;
        private RadDateTimePicker rdDo;

        private RadioButton rbWszy;
        private RadioButton rbBad;
        private RadioButton rbUnconf;

        public BIGDBHelper bigDB { get; set; }

        public string GetErrMessage()
        {

            return errMessage;

        }
        
        public BigStan()
        {

            ;
        
        }

        public BigStan(WinBIGMain theWind)
        {
            try
            {
                rgvOperacje = (RadGridView)(theWind.Controls.Find("rgvOperacje", true).FirstOrDefault());
                rgvMyDlu = (RadGridView)(theWind.Controls.Find("rgvMyDlu", true).FirstOrDefault());
                rbOdczytOper = (RadButton)(theWind.Controls.Find("rbOdczytOper", true).FirstOrDefault());
                rbCheck = (RadButton)(theWind.Controls.Find("rbCheck", true).FirstOrDefault());
                rbUpdateOper = (RadButton)(theWind.Controls.Find("rbUpdateOper",true).FirstOrDefault());
                rbDelOpr = (RadButton)(theWind.Controls.Find("rbDelOpr", true).FirstOrDefault());
                rpvOperacje = (RadPageView)(theWind.Controls.Find("rpvOperacje", true).FirstOrDefault());
                rbDetailsOper = (RadButton)(theWind.Controls.Find("rbDetailsOper", true).FirstOrDefault());
                rbAddInNew  = (RadButton)(theWind.Controls.Find("rbAddInNew",true).FirstOrDefault());
                rbSendAgain = (RadButton)(theWind.Controls.Find("rbSendAgain",true).FirstOrDefault());
                rbDetailsDlu = (RadButton)(theWind.Controls.Find("rbDetailsDlu", true).FirstOrDefault());
                rbPrint = (RadButton)(theWind.Controls.Find("rbPrint", true).FirstOrDefault());
                rgvDelDlu = (RadGridView)(theWind.Controls.Find("rgvDelDlu", true).FirstOrDefault());
                bt1 = (Button)(theWind.Controls.Find("bt1", true).FirstOrDefault());
                
                rdOd = (RadDateTimePicker)(theWind.Controls.Find("rdtpOd", true).FirstOrDefault());
                rdDo = (RadDateTimePicker)(theWind.Controls.Find("rdtpDo", true).FirstOrDefault());

                rbWszy = (RadioButton)(theWind.Controls.Find("rbWszy", true).FirstOrDefault());
                rbBad = (RadioButton)(theWind.Controls.Find("rbBad", true).FirstOrDefault());
                rbUnconf = (RadioButton)(theWind.Controls.Find("rbUnconf", true).FirstOrDefault());

                rbDetailsOper.Click += new EventHandler(rbDetailsOper_Click);
              //  rbSend = (RadButton)(theWind.Controls.Find("rbSend", true).FirstOrDefault());
                rbOdczytOper.Click -= rbOdczytOper_Click;
                rbOdczytOper.Click += new EventHandler(rbOdczytOper_Click);
                rbCheck.Click += new EventHandler(rbCheck_Click);
               // rbSend.Click += new EventHandler(rbSend_Click);
                rbUpdateOper.Click += new EventHandler(rbUpdateOper_Click);
                rbDelOpr.Click += new EventHandler(rbDelOpr_Click);
                rbAddInNew.Click+=new EventHandler(rbAddInNew_Click);
                rbSendAgain.Click += new EventHandler(rbSendAgain_Click);
                rbDetailsDlu.Click += new EventHandler(rbDetailsDlu_Click);
                rbPrint.Click += new EventHandler(rbPrint_Click);
               

                rbWszy.Click+=new EventHandler(rbWszy_Click);
                
                rbBad.Click+=new EventHandler(rbBad_Click);

                rbUnconf.Click += new EventHandler(rbUnconf_Click);
                bt1.Click += new EventHandler(bt1_Click);
                
                rdDo.Value = DateTime.Today.AddDays(1);
                rdOd.Value = DateTime.Today.AddMonths(-1);

                GridViewComboBoxColumn col = (GridViewComboBoxColumn)rgvOperacje.Columns["RA_Country"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvOperacje.Columns["Citizenship"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvOperacje.Columns["LiabilityType"];
                col.DataSource = Utils.naleznosci;
                col.ValueMember = "id";
                col.DisplayMember = "nazwa";

                ExpressionFormattingObject obj = new ExpressionFormattingObject("Cond1", "status = 0", false);
                obj.CellBackColor = Color.LightGray;
                obj.CellForeColor = Color.LightGray;
                this.rgvOperacje.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond2", "status = -1 ", false);
                obj.CellBackColor = Color.Red;
                obj.CellForeColor = Color.Red;
                this.rgvOperacje.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond3", "status = 1 ", false);
                obj.CellBackColor = Color.Green;
                obj.CellForeColor = Color.Green;
                this.rgvOperacje.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond4", "status = -1000 ", false);
                obj.CellBackColor = Color.Orange;
                obj.CellForeColor = Color.Orange;
                this.rgvOperacje.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond5", "status = 1000 ", false);
                obj.CellBackColor = Color.LightGreen;
                obj.CellForeColor = Color.LightGreen;
                this.rgvOperacje.Columns["status"].ConditionalFormattingObjectList.Add(obj);

                // moi dłużnicy
           
                col = (GridViewComboBoxColumn)rgvMyDlu.Columns["RA_Country"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvMyDlu.Columns["Citizenship"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvMyDlu.Columns["LiabilityType"];
                col.DataSource = Utils.naleznosci;
                col.ValueMember = "id";
                col.DisplayMember = "nazwa";

                 obj = new ExpressionFormattingObject("Cond1", "status = 0", false);
                obj.CellBackColor = Color.LightGray;
                obj.CellForeColor = Color.LightGray;
                this.rgvMyDlu.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond2", "status = -1 ", false);
                obj.CellBackColor = Color.Red;
                obj.CellForeColor = Color.Red;
                this.rgvMyDlu.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond3", "status = 1 ", false);
                obj.CellBackColor = Color.Green;
                obj.CellForeColor = Color.Green;
                this.rgvMyDlu.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond4", "status = -1000 ", false);
                obj.CellBackColor = Color.Orange;
                obj.CellForeColor = Color.Orange;
                this.rgvMyDlu.Columns["status"].ConditionalFormattingObjectList.Add(obj);
                obj = new ExpressionFormattingObject("Cond5", "status = 1000 ", false);
                obj.CellBackColor = Color.LightGreen;
                obj.CellForeColor = Color.LightGreen;
                this.rgvMyDlu.Columns["status"].ConditionalFormattingObjectList.Add(obj);

               
                // 

                col = (GridViewComboBoxColumn)rgvDelDlu.Columns["RA_Country"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvDelDlu.Columns["Citizenship"];
                col.DataSource = Utils.kraje;
                col.ValueMember = "skrot";
                col.DisplayMember = "nazwa";

                col = (GridViewComboBoxColumn)rgvDelDlu.Columns["LiabilityType"];
                col.DataSource = Utils.naleznosci;
                col.ValueMember = "id";
                col.DisplayMember = "nazwa";


                GridViewSummaryItem summaryItem = new GridViewSummaryItem();
                summaryItem.Name = "Mark";
                summaryItem.Aggregate = GridAggregateFunction.Count;
                
                GridViewSummaryItem summaryItemSum = new GridViewSummaryItem();
                summaryItemSum.Name = "ArrearsAmount";
                summaryItemSum.Aggregate = GridAggregateFunction.Sum;

                GridViewSummaryRowItem summaryRowItem = new GridViewSummaryRowItem();
                summaryRowItem.Add(summaryItem);
                summaryRowItem.Add(summaryItemSum);
                this.rgvMyDlu.SummaryRowsTop.Add(summaryRowItem);


                this.rgvOperacje.SummaryRowsTop.Add(summaryRowItem);
                
                
                retrieveAllOper();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd obsługi elementów interfeejsu " + ex.Message);

            }
        }

        void bt1_Click(object sender, EventArgs e)
        {
            // obsługa przepisania 

            List<string> lstToDo = new List<string>();
            if (rgvMyDlu.SelectedRows.Count > 0)
            {

                foreach (GridViewRowInfo row in rgvMyDlu.Rows)
                {


                    if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                    {
                        vw_BIG_Dluznicy dlu = (vw_BIG_Dluznicy)rgvMyDlu.SelectedRows[0].DataBoundItem;
                        // wykreślenie dłużnika i załadowanie ponownie.
                        // sprawdzamy gdzie dłużnik jest wpisany

                        //**********

                                                           lstToDo.Add(row.Cells["LiabilityId"].Value.ToString());
                        

                    }
                }

                        if (lstToDo.Any())
                        {
                            SubmitRqHelper shlp = new SubmitRqHelper();

                            List<extraData> extraLst = new List<extraData>();

                            Package mess = shlp.CreateDelRq(lstToDo, ref extraLst);
                            if (mess == null)
                                return;
                            using (RupIntegratorEntities context = new RupIntegratorEntities())
                            {
                                try
                                {
                                    if (shlp.sendPackage(mess))
                                    {


                                        if ((bigDB.savePackage(mess, context, extraLst)) == null)
                                        {
                                            MessageBox.Show(this.errMessage);
                                            return;
                                        }
                                        // 
                                        //context.BIG_Package.AddObject(bp);
                                        context.SaveChanges();
                                        MessageBox.Show("Wysyłka usuwanych pozycji zakończyła się powodzeniem");
                                      
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Błąd podczas wysyłki danych na platformę MS lub zapisu danych " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                                    return;
                                }

                            }
                        }


                        List<GridViewRowInfo> lstToDoAdd = new List<GridViewRowInfo>();
                        List<int> IdOperations = new List<int>();
                
                        int operType = 0;
                        string currPageName = rpvOperacje.SelectedPage.Name;

                       
                            if (currPageName.StartsWith("rpvMyBig"))
                            {
                                operType = 2;
                                foreach (GridViewRowInfo row in rgvMyDlu.Rows)
                                {

                                    if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                                    {
                                        // przepisanie Id operacji do dłużniua
                                        row.Cells["DebatorID"].Value = row.Cells["LiabilityId"].Value;
                                        lstToDoAdd.Add(row);
                                        
                                        IdOperations.Add(Convert.ToInt32(row.Cells["IdBIG_InfoOperation"].Value));
                                        if (operType == 0)
                                            operType = Convert.ToInt32(row.Cells["OperType"].Value);

                                        if (operType != Convert.ToInt32(row.Cells["OperType"].Value))
                                        {
                                     //       MessageBox.Show("Wszystkie operacje muszą być tego samego rodzaju Dodanie/Aktualizacja/Usunięcie ");
                                     //       return;
                                        }
                                    }
                                }
                            }
                            else return;

                        if (!lstToDoAdd.Any())
                        {
                            MessageBox.Show("Nie wybrano żadnych wierszy");
                            return;
                        }

                        using (RupIntegratorEntities context = new RupIntegratorEntities())
                        {
                            // weryfikacja czy można dostac  
                        

                     
                            List<BIG_Big> lstBigBig = new List<BIG_Big>();

                                lstBigBig = context.BIG_Big.Where(a =>  a.Obsluga == true).ToList();
                               
                             

                         


                            {
                                bool result;
                                SubmitRqHelper srq = new SubmitRqHelper();
                                List<extraData> lstextraData = new List<extraData>();
                                Package pack = null;
                                pack = srq.CreateUpdtRqFromDB(lstToDoAdd, ref lstextraData, 1, lstBigBig);
                                // nadpisanie id dłuznika

                                BIG_Package bp;
                                if (pack != null)
                                {
                                    // nadpisanie id dłuznika
                                    if (pack.packageSubmit != null && pack.packageSubmit.operation != null && pack.packageSubmit.operation.Any())
                                    {
                                        foreach (Operation o in pack.packageSubmit.operation)
                                        {
                                            if (o.addInformation != null)
                                            {
                                                o.addInformation.debtor.debtorId = o.addInformation.liability.liabilityId;
                                            
                                            }
                                        }
                                    
                                    
                                    }

                                    result = srq.sendPackage(pack);
                                    if (result)
                                    {


                                        try
                                        {
                                            if ((bp = bigDB.savePackage(pack, context, lstextraData)) == null)
                                            {
                                                MessageBox.Show(this.errMessage);
                                                return;
                                            }
                                            // 
                                            //context.BIG_Package.AddObject(bp);
                                            context.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {

                                            MessageBox.Show("Błąd podczas zapisu pakietu danych " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                                            return;
                                        }
                                    }
                                    MessageBox.Show("Wysyłka zakończyła się powodzeniem");
                                    this.retrieveAllOper();


                                }
                            }
                            // wpisanie do podanych bigów.


                        }


                    }

                    

        }

        void rbUnconf_Click(object sender, EventArgs e)
        {
            retrieveAllOper();
        }

        void rbBad_Click(object sender, EventArgs e)
        {
            retrieveAllOper();
        }

        void rbWszy_Click(object sender, EventArgs e)
        {
            retrieveAllOper();
        }

        void rbPrint_Click(object sender, EventArgs e)
        {
          

            foreach (GridViewRowInfo row in rgvOperacje.Rows)
            {
                    

                if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                {
                    OperDetails odet = new OperDetails();
                    odet.Shown += new EventHandler(odet_Shown); 
                    vw_BIG_Operacje bo = (vw_BIG_Operacje)row.DataBoundItem;

                    odet.operId = bo.IdBIG_InfoOperation;
             
                  //  odet.Retrieve(bo.IdBIG_InfoOperation);
                    odet.Location = new Point(-1000, -1000);
                    odet.Show();
                        
                    
                    

                }
            }

        }

        void odet_Shown(object sender, EventArgs e)
        {
            OperDetails win = (OperDetails)sender;
            win.Print(false);
            win.Close();
            //odet.Print(false);
        }

        void rbDetailsDlu_Click(object sender, EventArgs e)
        {
            if ( rgvMyDlu.SelectedRows.Count> 0 )
            {

                vw_BIG_Dluznicy dlu = (vw_BIG_Dluznicy)rgvMyDlu.SelectedRows[0].DataBoundItem;
                WndDluDetails wnd = new WndDluDetails();
                wnd.dlu = dlu;
                wnd.ShowDialog();
            
            }
            else
            {
                MessageBox.Show("Wybierz dłużnika i zobowiązanie z listy ");
                return;
            
            }

                
        }

        void rbSendAgain_Click(object sender, EventArgs e)
        {
            // 
            List<GridViewRowInfo> lstToDo = new List<GridViewRowInfo>();
            List<int> IdOperations =  new List<int>();
            int OperTyp ;

            int operType = 0;
            string currPageName = rpvOperacje.SelectedPage.Name;

            if (currPageName.StartsWith("rpvOperacjeBig"))
            {
                
                foreach (GridViewRowInfo row in rgvOperacje.Rows)
                {

                    if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                    {
                        lstToDo.Add(row);
                        IdOperations.Add(Convert.ToInt32(row.Cells["IdBIG_InfoOperation"].Value));
                        if ( operType == 0 )
                            operType = Convert.ToInt32(row.Cells["OperType"].Value);
                        if (operType != Convert.ToInt32(row.Cells["OperType"].Value))
                        {
                            MessageBox.Show("Wszystkie operacje muszą być tego samego rodzaju Dodanie/Aktualizacja/Usunięcie ");
                            return;
                        }
                    }
                }
            }
            else
                if (currPageName.StartsWith("rpvMyBig"))
                {
                    operType = 2;
                    foreach (GridViewRowInfo row in rgvMyDlu.Rows)
                    {

                        if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                        {
                            lstToDo.Add(row);
                            IdOperations.Add(Convert.ToInt32(row.Cells["IdBIG_InfoOperation"].Value));
                            if (operType == 0)
                                operType = Convert.ToInt32(row.Cells["OperType"].Value);

                            if (operType != Convert.ToInt32(row.Cells["OperType"].Value))
                            {
                                MessageBox.Show("Wszystkie operacje muszą być tego samego rodzaju Dodanie/Aktualizacja/Usunięcie ");
                                return;
                            }
                        }
                    }
                }
                else return;

            if (!lstToDo.Any())
            {
                MessageBox.Show("Nie wybrano żadnych wierszy");
                return;
            }

            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
            // weryfikacja czy można dostac  
            List<int> listBigId = new List<int>();

            foreach (int o in IdOperations)
            {
                List<BIG_Oper_Status> lstbad = context.BIG_Oper_Status.Where(a => a.IdBIG_InfoOperation == o && a.Status < 0).ToList();
                if ( !listBigId.Any())
                {
                   foreach (BIG_Oper_Status bo in lstbad)
                   {
                        listBigId.Add(bo.IdBIG_Big);
                   }
                
                
                }
                else
                {
                  int i;
                   
                 for ( i = 0 ; i < listBigId.Count; i++)
                   {
                        if (  !lstbad.Select (a=>a.IdBIG_Big).ToList().Contains(listBigId[i]) )
                        {
                            listBigId[i] = 0 ;           
                            
                        }

                   }
                
                }
            }
                listBigId = listBigId.Where(a=>a > 0 ).ToList();
                //
            
                if (!listBigId.Any())
                {
                    MessageBox.Show("Niektóre z zaznaczonych pozycji zostały wysłane poprawnie. Zmień zaznaczenie");
                    return;
                
                }
                List<BIG_Big> lstBigBig = new List<BIG_Big> ();

                using (RupIntegratorEntities cont = new RupIntegratorEntities())
                {

                    lstBigBig = cont.BIG_Big.Where(a => listBigId.Contains(a.IdBig) && a.Obsluga == true).ToList();
                    {

                        WndSelBIG wGetBIG = new WndSelBIG();
                        wGetBIG.lstBIGI = lstBigBig;
                        if (wGetBIG.ShowDialog() != DialogResult.OK)
                            return;

                        lstBigBig = lstBigBig.Where(a => a.Obsluga == true).ToList();
                    }
                    if (MessageBox.Show("Czy wysłać zaznaczonych dłużników do następujących BIG " + String.Join(" ", lstBigBig.Select(a => a.BIGID).ToArray()), "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                        return;


                }



                {
                    bool result;
                    SubmitRqHelper srq = new SubmitRqHelper();
                    List<extraData> lstextraData = new List<extraData>();
                    Package pack = null;
                    pack = srq.CreateUpdtRqFromDB(lstToDo, ref lstextraData, operType, lstBigBig);

                    BIG_Package bp;
                    if (pack != null)
                    {
                        result = srq.sendPackage(pack);
                        if (result)
                        {

                     
                                try
                                {
                                    if ((bp = bigDB.savePackage(pack, context, lstextraData)) == null)
                                    {
                                        MessageBox.Show(this.errMessage);
                                        return;
                                    }
                                    // 
                                    //context.BIG_Package.AddObject(bp);
                                    context.SaveChanges();
                                    MessageBox.Show("Wysyłka zakończyła się powodzeniem");
                                }

                                catch (Exception ex)
                                {

                                    MessageBox.Show("Błąd podczas zapisu pakietu danych " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                                    return;
                                }
                            }
                           
                            this.retrieveAllOper();
                            
                        
                    }
                }
                // wpisanie do podanych bigów.


            }

        }
        /*
        private sendDluToBIGs(List<vw_BIG_Dluznicy> dluLst, List<BIG_Big> bigs)
        {


            foreach ( dlu in  dluLst )
            {
                
                
            
            
            }
        
        
        
        }

        */
        void rbAddInNew_Click(object sender, EventArgs e)
        {
            // dodawanie tylko do nowych BIG 
             List<vw_BIG_Dluznicy > lstToDo = new List<vw_BIG_Dluznicy>();
            List<GridViewRowInfo> rowLst = new List<GridViewRowInfo>();
                  List<BIG_Big> lstBig = null;
            //
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
               lstBig = context.BIG_Big.Where(a => a.Obsluga == true).ToList();
                //

               
                string currPageName = rpvOperacje.SelectedPage.Name;

                if (currPageName.StartsWith("rpvOperacjeBig"))
                {
                    MessageBox.Show("Funkcja dostępna na zakładce Dłużnicy");
                }
                else
                    if (currPageName.StartsWith("rpvMyBig"))
                    {
                        foreach (GridViewRowInfo row in rgvMyDlu.Rows)
                        {

                            if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                            {
                                lstToDo.Add( row.DataBoundItem as vw_BIG_Dluznicy );
                                rowLst.Add(row);
                            }
                        }
                    }
                    else return;


                if (!lstToDo.Any())
                {
                    MessageBox.Show("Nie wybrano żadnego wiersza");
                    return;
                }
                // 
                // Wyznaczenie big, do którego ma trafić dłużnik.
                List<int> idBigs = new List<int>();
                foreach (vw_BIG_Dluznicy dlu in lstToDo)
                {
                    List<int>  stats   =    (from  c in  context.BIG_Oper_Status 
                                                                    where c.IdBIG_InfoOperation == dlu.IdBIG_InfoOperation 
                                                                     && c.Status > 0 select c.IdBIG_Big).Distinct().ToList();
                    if (stats != null && stats.Any())
                    {
                        foreach (int id in stats)
                        {
                            if (!idBigs.Contains(id))
                                idBigs.Add(id);
                        
                        
                        }
                    
                    }

                                                   
                
                }
                //
                foreach (BIG_Big bb in lstBig)
                    if (idBigs.Contains(bb.IdBig))
                        bb.Obsluga  = false ;

                lstBig = lstBig.Where(a => a.Obsluga == true).ToList();
                if (!lstBig.Any())
                {
                    MessageBox.Show("Co najmniej jeden z zaznaczonych dłużników jest we wsystkich aktywnych BIG'ach   ");
                    return;
                }

                WndSelBIG wGetBIG = new WndSelBIG();
                wGetBIG.lstBIGI = lstBig;
                if (wGetBIG.ShowDialog() != DialogResult.OK)
                    return;

                lstBig = lstBig.Where(a => a.Obsluga == true).ToList();

                if (MessageBox.Show("Czy wpisać zaznaczonych dłużników do następujących BIG " + String.Join(" ", lstBig.Select(a => a.BIGID).ToArray()), "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                    return;
            }
                {
                    bool result;
                    SubmitRqHelper srq = new SubmitRqHelper();
                    List<extraData> lstextraData = new List<extraData>();
                    Package pack = null;
                    pack = srq.CreateUpdtRqFromDB(rowLst, ref lstextraData, 1,lstBig);

                    BIG_Package bp;
                    if (pack != null)
                    {
                        result = srq.sendPackage(pack);
                        if (result)
                        {

                            using (RupIntegratorEntities context = new RupIntegratorEntities())
                            {
                                try
                                {
                                    if ((bp = bigDB.savePackage(pack, context, lstextraData)) == null)
                                    {
                                        MessageBox.Show(this.errMessage);
                                        return;
                                    }
                                    // 
                                    //context.BIG_Package.AddObject(bp);
                                    context.SaveChanges();
                                }
                                catch (Exception ex)
                                {

                                    MessageBox.Show("Błąd podczas zapisu pakietu danych " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                                    return;
                                }
                            }
                            MessageBox.Show("Wysyłka zakończyła się powodzeniem");
                            this.retrieveAllOper();
                        }
                    }
                }
                // wpisanie do podanych bigów.



           
        }

        void rbDetailsOper_Click(object sender, EventArgs e)
        {

            if (this.rgvOperacje != null && this.rgvOperacje.SelectedRows.Count > 0 ) 
            {
                vw_BIG_Operacje bo = (vw_BIG_Operacje)this.rgvOperacje.SelectedRows[0].DataBoundItem;

                OperDetails odet = new OperDetails();
                odet.operId = bo.IdBIG_InfoOperation;
                odet.ShowDialog();
             }
            else
            {
                MessageBox.Show("Wybierz wiersz z listy");
                return;
            }

        }

        private void retrieveAllOper()
        {

            Cursor.Current = Cursors.WaitCursor;
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {

                DateTime dOd, dDo;
                int status = 1000;
                dOd = rdOd.Value;
                dDo = rdDo.Value;
                try
                {
                    if (rbBad.Checked)
                    {
                        rgvOperacje.DataSource = context.vw_BIG_Operacje.Where(a => a.SentDate >= dOd && a.SentDate <= dDo && a.status < 0).OrderByDescending(a => a.SentDate).ToList();
                    }
                    else
                    {

                        if (rbUnconf.Checked)
                            rgvOperacje.DataSource = context.vw_BIG_Operacje.Where(a => a.SentDate >= dOd && a.SentDate <= dDo && (a.status == 0 || a.status == 1000)).OrderByDescending(a => a.SentDate).ToList();
                        else
                            rgvOperacje.DataSource = context.vw_BIG_Operacje.Where(a => a.SentDate >= dOd && a.SentDate <= dDo).OrderByDescending(a => a.SentDate).ToList();

                    }

                    rgvMyDlu.DataSource = context.vw_BIG_Dluznicy.OrderByDescending(a => a.SentDate).ToList();

                }
                catch ( Exception ex)
            {
                MessageBox.Show(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message:""));
            
            }
        
                Cursor.Current = Cursors.Default;
                     
            }
            
        }

        void rbUpdateOper_Click(object sender, EventArgs e)
        {
            List<GridViewRowInfo> lstToDo = new List<GridViewRowInfo>();
            List<string> lstToDoDel = new List<string>();

            int operType = 0;
            string currPageName = rpvOperacje.SelectedPage.Name;

            if (currPageName.StartsWith("rpvOperacjeBig"))
            {
                operType = 1;
                foreach (GridViewRowInfo row in rgvOperacje.Rows)
                {

                    if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                    {
                        lstToDo.Add(row);

                    }
                }
            }
            else
                if (currPageName.StartsWith("rpvMyBig"))
                {
                    operType = 2;
                    foreach (GridViewRowInfo row in rgvMyDlu.Rows)
                    {

                        if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                        {
                            lstToDo.Add(row);

                        }
                    }
                }
                else return;

            if (!lstToDo.Any())
                return;
                     
            {
                bool result;
                SubmitRqHelper srq = new SubmitRqHelper();
                List<extraData> lstextraData = new List<extraData>();
                Package pack = null;
                pack = srq.CreateUpdtRqFromDB(lstToDo, ref lstextraData, 2);
               
                BIG_Package bp;
                if (pack != null)
                {
                    result = srq.sendPackage(pack);
                    if (result)
                    {

                        using (RupIntegratorEntities context = new RupIntegratorEntities())
                        {
                            try
                            {
                                if ((bp = bigDB.savePackage(pack, context, lstextraData)) == null)
                                {
                                    MessageBox.Show(this.errMessage);
                                    return;
                                }
                                // 
                                //context.BIG_Package.AddObject(bp);
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                                MessageBox.Show("Błąd podczas zapisu pakietu danych " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                                return;
                            }
                        }
                        MessageBox.Show("Wysyłka zakończyła się powodzeniem");
                        this.retrieveAllOper();
                    }
                }
            }
        }

        void rbDelOpr_Click(object sender, EventArgs e)
        {
        List<string> lstToDo = new List<string>();

   

           
            string currPageName = rpvOperacje.SelectedPage.Name;

            if (currPageName.StartsWith("rpvOperacjeBig"))
            {
                foreach (GridViewRowInfo row in rgvOperacje.Rows)
            {


                if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                {
                    lstToDo.Add(row.Cells["LiabilityId"].Value.ToString());

                }


            }
            }
            else
                if (currPageName.StartsWith("rpvMyBig"))
                {
                    foreach (GridViewRowInfo row in rgvMyDlu.Rows)
                    {


                        if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                        {
                            lstToDo.Add(row.Cells["LiabilityId"].Value.ToString());

                        }


                    }
                }
                else
                {
                    MessageBox.Show("Błędny kontekst wywołania");
                    return;

                }


            if (lstToDo.Any())
            {
                SubmitRqHelper shlp = new SubmitRqHelper();

                 List<extraData> extraLst = new List<extraData>();

                Package  mess = shlp.CreateDelRq(lstToDo, ref extraLst );
                if (mess == null)
                    return;
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    try
                    {
                        if (shlp.sendPackage(mess))
                        {




                            if ((bigDB.savePackage(mess, context, extraLst)) == null)
                            {
                                MessageBox.Show(this.errMessage);
                                return;
                            }
                            // 
                            //context.BIG_Package.AddObject(bp);
                            context.SaveChanges();
                            MessageBox.Show("Wysyłka zakończyła się powodzeniem");
                            this.retrieveAllOper();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas wysyłki danych na platformę MS lub zapisu danych " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                        return;
                    }

                }
                }
            }
             
        

       

        void rbCheck_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            // sprawdzenie czy są zamarkowane jakieś pozycje 

            string currPageName = rpvOperacje.SelectedPage.Name;
            List<string> lstToDo = new List<string>();


            if (currPageName.StartsWith("rpvOperacjeBig"))
            {
                foreach (GridViewRowInfo row in rgvOperacje.Rows)
                {


                    if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                    {
                        string packId = row.Cells["PackageFullId"].Value.ToString();
                        if (!lstToDo.Contains(packId))
                        lstToDo.Add(packId);

                    }


                }
            }
            else
                if (currPageName.StartsWith("rpvMyBig"))
                {
                    foreach (GridViewRowInfo row in  rgvMyDlu.Rows)
                    {


                        if (Convert.ToBoolean(row.Cells["Mark"].Value) == true)
                        {
                            string packId = row.Cells["PackageFullId"].Value.ToString();
                            if (!lstToDo.Contains(packId))
                                lstToDo.Add(packId);

                        }


                    }
                }
                else
                {
                    MessageBox.Show("Błędny kontekst wywołania");
                    return;

                }




            CheckOperationStatus cso = new CheckOperationStatus();
            if (lstToDo.Any())
                cso.CheckStatusByPackageList(lstToDo);
            else
                cso.checkStatusAll();

            retrieveAllOper();
            Cursor.Current = Cursors.Default;
         }

        void rbOdczytOper_Click(object sender, EventArgs e)
        {
            retrieveAllOper();
        }

     


    }
}
