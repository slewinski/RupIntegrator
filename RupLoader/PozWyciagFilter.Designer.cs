
namespace RupLoader
{
    partial class PozWyciagFilter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.WinControls.UI.RadListDataItem radListDataItem1 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem2 = new Telerik.WinControls.UI.RadListDataItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PozWyciagFilter));
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.rddlRachunek = new Telerik.WinControls.UI.RadDropDownList();
            this.rbCancel = new Telerik.WinControls.UI.RadButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbKasa = new Telerik.WinControls.UI.RadRadioButton();
            this.rbBank = new Telerik.WinControls.UI.RadRadioButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.rdOD = new Telerik.WinControls.UI.RadDateTimePicker();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.rdDO = new Telerik.WinControls.UI.RadDateTimePicker();
            this.tbStatusRozliczenia = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rddlRachunek)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbKasa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbBank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdOD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdDO)).BeginInit();
            this.SuspendLayout();
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(172, 208);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(88, 24);
            this.rbOK.TabIndex = 0;
            this.rbOK.Text = "OK";
            this.rbOK.Click += new System.EventHandler(this.rbOK_Click);
            // 
            // rddlRachunek
            // 
            this.rddlRachunek.DropDownAnimationEnabled = true;
            this.rddlRachunek.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            radListDataItem1.Text = "Sumy na zlecenie";
            radListDataItem2.Text = "Dochody budżetowe";
            this.rddlRachunek.Items.Add(radListDataItem1);
            this.rddlRachunek.Items.Add(radListDataItem2);
            this.rddlRachunek.Location = new System.Drawing.Point(172, 80);
            this.rddlRachunek.Name = "rddlRachunek";
            this.rddlRachunek.Size = new System.Drawing.Size(190, 20);
            this.rddlRachunek.TabIndex = 1;
            // 
            // rbCancel
            // 
            this.rbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbCancel.Location = new System.Drawing.Point(269, 208);
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.Size = new System.Drawing.Size(88, 24);
            this.rbCancel.TabIndex = 1;
            this.rbCancel.Text = "Anuluj";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbKasa);
            this.groupBox1.Controls.Add(this.rbBank);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(319, 50);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bank/Kasa";
            // 
            // rbKasa
            // 
            this.rbKasa.Location = new System.Drawing.Point(190, 19);
            this.rbKasa.Name = "rbKasa";
            this.rbKasa.Size = new System.Drawing.Size(93, 18);
            this.rbKasa.TabIndex = 0;
            this.rbKasa.TabStop = false;
            this.rbKasa.Text = "Raport kasowy";
            // 
            // rbBank
            // 
            this.rbBank.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rbBank.Location = new System.Drawing.Point(29, 19);
            this.rbBank.Name = "rbBank";
            this.rbBank.Size = new System.Drawing.Size(105, 18);
            this.rbBank.TabIndex = 1;
            this.rbBank.Text = "Wyciąg bankowy";
            this.rbBank.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            this.rbBank.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.rbBank_ToggleStateChanged);
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(12, 80);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(154, 18);
            this.radLabel1.TabIndex = 4;
            this.radLabel1.Text = "Rodzaj Rachunku Bankowego";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(71, 122);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(95, 18);
            this.radLabel2.TabIndex = 5;
            this.radLabel2.Text = "Status Rozliczenia";
            this.radLabel2.Click += new System.EventHandler(this.radLabel2_Click);
            // 
            // rdOD
            // 
            this.rdOD.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.rdOD.Location = new System.Drawing.Point(172, 163);
            this.rdOD.Name = "rdOD";
            this.rdOD.Size = new System.Drawing.Size(81, 20);
            this.rdOD.TabIndex = 6;
            this.rdOD.TabStop = false;
            this.rdOD.Text = "2023-09-26";
            this.rdOD.Value = new System.DateTime(2023, 9, 26, 14, 55, 10, 377);
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(120, 163);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(46, 18);
            this.radLabel3.TabIndex = 6;
            this.radLabel3.Text = "Od dnia";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(259, 163);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(19, 18);
            this.radLabel4.TabIndex = 7;
            this.radLabel4.Text = "do";
            // 
            // rdDO
            // 
            this.rdDO.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.rdDO.Location = new System.Drawing.Point(281, 163);
            this.rdDO.Name = "rdDO";
            this.rdDO.Size = new System.Drawing.Size(81, 20);
            this.rdDO.TabIndex = 7;
            this.rdDO.TabStop = false;
            this.rdDO.Text = "2023-09-26";
            this.rdDO.Value = new System.DateTime(2023, 9, 26, 14, 55, 10, 377);
            // 
            // tbStatusRozliczenia
            // 
            this.tbStatusRozliczenia.Location = new System.Drawing.Point(172, 122);
            this.tbStatusRozliczenia.MaxLength = 2;
            this.tbStatusRozliczenia.Name = "tbStatusRozliczenia";
            this.tbStatusRozliczenia.Size = new System.Drawing.Size(30, 20);
            this.tbStatusRozliczenia.TabIndex = 8;
            this.tbStatusRozliczenia.Text = "1";
            // 
            // PozWyciagFilter
            // 
            this.AcceptButton = this.rbOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.rbCancel;
            this.ClientSize = new System.Drawing.Size(383, 244);
            this.Controls.Add(this.tbStatusRozliczenia);
            this.Controls.Add(this.rdDO);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.rdOD);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rbCancel);
            this.Controls.Add(this.rddlRachunek);
            this.Controls.Add(this.rbOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PozWyciagFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zakres płatności do analizy";
            this.Load += new System.EventHandler(this.PozWyciagFilter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rddlRachunek)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbKasa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbBank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdOD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdDO)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton rbOK;
        private Telerik.WinControls.UI.RadDropDownList rddlRachunek;
        private Telerik.WinControls.UI.RadButton rbCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private Telerik.WinControls.UI.RadRadioButton rbKasa;
        private Telerik.WinControls.UI.RadRadioButton rbBank;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadDateTimePicker rdOD;
        private Telerik.WinControls.UI.RadDateTimePicker rdDO;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private System.Windows.Forms.TextBox tbStatusRozliczenia;
    }
}