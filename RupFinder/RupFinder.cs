using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace RupFinder
{
    public partial class RupFinder : Form
    {
        string firstField = "t1";
        public string inArg = null;
        public string mode   = null;
        public string connStr { get; set; }

        public RupFinder()
        {
            InitializeComponent();
        }


        public void FindSAPIds(string InString)
        {
            string ConnectionString;
            SqlDataReader rdr = null;
            SqlConnection con = null;
            SqlCommand storedProcCommand;

            

           try
            {
                // Open connection to the database
                Cursor.Current = Cursors.WaitCursor;
                if (!String.IsNullOrWhiteSpace(connStr))
                    ConnectionString = connStr;
                else
                    ConnectionString = ConfigurationManager.ConnectionStrings["RupFinder.Properties.Settings.RupDB"].ConnectionString;
                
               con = new SqlConnection(ConnectionString);
                //con.Open();
                storedProcCommand = new SqlCommand("sp_Search", con);
                storedProcCommand.CommandType = CommandType.StoredProcedure;
                storedProcCommand.Parameters.Add("@instring", InString );
                storedProcCommand.CommandTimeout = 600;
                storedProcCommand.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter();    
               
                da.SelectCommand = storedProcCommand;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgVResult.DataSource = dt;
                Cursor.Current = Cursors.Default;
              }
            catch (Exception ex)
            {
                // Print error message
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();
                if (con.State == ConnectionState.Open)
                    con.Close(); 

            };

         


        }

        private void CopyCellsToClipboard(int currentRow)
        {
            int firstCol;
            int i;
            if (dgVResult.RowCount > 0)
            {
                try{
                if (!String.IsNullOrEmpty(inArg))
                {
                    string result = "";
                    List<string> TagIds = inArg.Split(';').ToList();
                    foreach ( string tag in TagIds)
                    {
                        int colno;
                        string strvalue = "";
                        if (int.TryParse(tag, out colno))
                        {
                        // wartosć z ntej kolumny
                            if (dgVResult.Columns.Count >= colno)
                            {
                                strvalue = dgVResult.Rows[currentRow].Cells[colno - 1].Value.ToString();
                                
                            }

                        }
                        else
                        {
                            strvalue = tag;
                            
                        
                        }
                         if (!string.IsNullOrEmpty(result)) result += "\t";
                         result += strvalue;

                    }

                    MessageBox.Show(result);
                    Clipboard.SetDataObject(result,true);
                    
                }
                else
                {
                    dgVResult.ClearSelection();
                    firstCol = dgVResult.Columns[firstField].Index;
                    for (i = 0; i < 4; i++)
                    {
                        dgVResult.Rows[currentRow].Cells[firstCol + i].Selected = true;


                    }
                    Clipboard.SetDataObject(this.dgVResult.GetClipboardContent(), true);  //SelectedCells DataGridView1.GetClipboardContent()
                }
            }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : "") );
                
                }
            }
        }

        private void btFind_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            FindSAPIds(tbFind.Text);
            
        }


        private void dgVResult_DoubleClick(object sender, EventArgs e)
        {
            int row;
            if (dgVResult.SelectedRows.Count == 1)
            {
                
                row = (sender as DataGridView).CurrentRow.Index;            
                CopyCellsToClipboard(row);
                Application.Exit();
            }
        }

        
        private void dgVResult_KeyDown(object sender, KeyEventArgs e)
        {
          
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.Handled = true;
                Clipboard.SetDataObject(this.dgVResult.GetClipboardContent(), true);
                Application.Exit(); 
            }

        }

        private void RupFinder_Load(object sender, EventArgs e)
        {
            int mymode ;

            if (mode != null)
            {
                if (mode[0] == '/')
                {
                    mode = mode.Substring(1);
                    if (int.TryParse(mode, out mymode ))
                    {
                        tbTextAll.Visible = true;
                        tbTextAll.Enabled = true;
                        return;                   
                    
                    }
                
                }
            
            
            
            }
            tbTextAll.Visible = false;
            tbTextAll.Enabled = false;
            
        }

       

        private void tbTextAll_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                string txt = tbTextAll.SelectedText;
                if (!String.IsNullOrEmpty(txt))
                {
                    tbFind.Text = txt;
                    Clipboard.Clear();
                    FindSAPIds(tbFind.Text);

                }

            }
        }

      

       
    }
}
