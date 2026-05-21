using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Globalization;

namespace KnsMigrator
{
 

    public partial class KonwertWyc : Form
    {
        public KnsMigratorEntities theContext {get; set;}
        private string inputFileName;
        private string outputFileName;
 
        public KonwertWyc()
        {
            InitializeComponent();
        }


        private void btSrc_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "IMP (*.imp)|*.imp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!openFileDialog.FileName.Equals(String.Empty))
                {
                    this.tbInput.Text = openFileDialog.FileName;
                }
            }

        }

        private void btDest_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "EXP (*.exp)|*.exp";
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!saveFileDialog.FileName.Equals(String.Empty))
                {
                    this.tbOutput.Text = saveFileDialog.FileName;
                }
            }
        }

        private void btKonwert_Click(object sender, EventArgs e)
        {
            decimal kwota;
     
            if (String.IsNullOrEmpty(this.tbInput.Text)) { MessageBox.Show("Wybierz zbiór wejściowy"); return; }
            if (String.IsNullOrEmpty(this.tbOutput.Text)) { MessageBox.Show("Wskaż zbiór wyjściowy"); return; }
            if (!File.Exists(this.tbInput.Text)) { MessageBox.Show("Błędna nazwa zbioru wejściowego"); return; }
           
            try
            {
                SqlDataReader rdr_rh = null;
                string ConnectionString = Utils.BuildMyConnectionString(theContext);
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand storedProcCommand = new SqlCommand("przelew_videotel_dlugi", con);
                storedProcCommand.CommandType = CommandType.StoredProcedure;


                storedProcCommand.Parameters.Add("@sourcefile", this.tbInput.Text);
                storedProcCommand.Parameters.Add("@destfile", this.tbOutput.Text);
                storedProcCommand.Parameters.Add("@dataWyc", this.dateTimePicker1.Value.Date);
                kwota = Convert.ToDecimal(rmbSaldo.Value.ToString().Replace("zł","").Trim(),new CultureInfo("pl-PL"));
                //storedProcCommand.Parameters.Add("@ParameterName", SqlDbType.Int, 5);

                storedProcCommand.Parameters.Add("@saldoWyc", kwota);
                storedProcCommand.Connection = con;
                storedProcCommand.CommandTimeout = 600;
                rdr_rh = storedProcCommand.ExecuteReader();
                if (rdr_rh.HasRows)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.tbOutput.Text,false,Encoding.GetEncoding(this.rbUtf.Checked ? 65001:1250)))
                {
                    while (rdr_rh.Read())
                    {
                        file.WriteLine(rdr_rh["linia"]);
                    }
               

                }
                }
                rdr_rh.Close();
                con.Close();
                MessageBox.Show("Konwersja przebiegła pomyślnie. Wynik w zbiorze: " + tbOutput.Text, "Komunikat",MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd poczas przetwarzania zbioru " + ex.Message);
                
            
            }

        }
    }
}
