namespace RupLoader
{
    partial class DokDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DokDetail));
            this.rdtDataDokumentu = new Telerik.WinControls.UI.RadDateTimePicker();
            this.rdtDataKsiegowania = new Telerik.WinControls.UI.RadDateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rmebKwota = new Telerik.WinControls.UI.RadMaskedEditBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbOpGl = new System.Windows.Forms.TextBox();
            this.tbOpCz = new System.Windows.Forms.TextBox();
            this.btKsieguj = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.rdtDataDokumentu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdtDataKsiegowania)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rmebKwota)).BeginInit();
            this.SuspendLayout();
            // 
            // rdtDataDokumentu
            // 
            this.rdtDataDokumentu.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.rdtDataDokumentu.Location = new System.Drawing.Point(104, 51);
            this.rdtDataDokumentu.Name = "rdtDataDokumentu";
            this.rdtDataDokumentu.Size = new System.Drawing.Size(86, 20);
            this.rdtDataDokumentu.TabIndex = 1;
            this.rdtDataDokumentu.TabStop = false;
            this.rdtDataDokumentu.Value = new System.DateTime(((long)(0)));
            // 
            // rdtDataKsiegowania
            // 
            this.rdtDataKsiegowania.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.rdtDataKsiegowania.Location = new System.Drawing.Point(314, 51);
            this.rdtDataKsiegowania.Name = "rdtDataKsiegowania";
            this.rdtDataKsiegowania.Size = new System.Drawing.Size(86, 20);
            this.rdtDataKsiegowania.TabIndex = 2;
            this.rdtDataKsiegowania.TabStop = false;
            this.rdtDataKsiegowania.Value = new System.DateTime(((long)(0)));
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(216, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Data księgowania";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Data dokumentu";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Kwota";
            // 
            // rmebKwota
            // 
            this.rmebKwota.Location = new System.Drawing.Point(106, 96);
            this.rmebKwota.Mask = "C";
            this.rmebKwota.MaskType = Telerik.WinControls.UI.MaskType.Numeric;
            this.rmebKwota.Name = "rmebKwota";
            this.rmebKwota.Size = new System.Drawing.Size(84, 20);
            this.rmebKwota.TabIndex = 6;
            this.rmebKwota.TabStop = false;
            this.rmebKwota.Text = "0,00 zł";
            this.rmebKwota.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Operacja główna";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(205, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Operacja częściowa";
            // 
            // tbOpGl
            // 
            this.tbOpGl.Location = new System.Drawing.Point(106, 139);
            this.tbOpGl.MaxLength = 4;
            this.tbOpGl.Name = "tbOpGl";
            this.tbOpGl.Size = new System.Drawing.Size(41, 20);
            this.tbOpGl.TabIndex = 9;
            // 
            // tbOpCz
            // 
            this.tbOpCz.Location = new System.Drawing.Point(314, 139);
            this.tbOpCz.MaxLength = 4;
            this.tbOpCz.Name = "tbOpCz";
            this.tbOpCz.Size = new System.Drawing.Size(37, 20);
            this.tbOpCz.TabIndex = 10;
            // 
            // btKsieguj
            // 
            this.btKsieguj.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btKsieguj.Location = new System.Drawing.Point(249, 0);
            this.btKsieguj.Name = "btKsieguj";
            this.btKsieguj.Size = new System.Drawing.Size(75, 23);
            this.btKsieguj.TabIndex = 11;
            this.btKsieguj.Text = "Księguj";
            this.btKsieguj.UseVisualStyleBackColor = true;
            this.btKsieguj.Click += new System.EventHandler(this.btKsieguj_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(330, 0);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 12;
            this.btCancel.Text = "Anuluj";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // DokDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 195);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btKsieguj);
            this.Controls.Add(this.tbOpCz);
            this.Controls.Add(this.tbOpGl);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rmebKwota);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rdtDataKsiegowania);
            this.Controls.Add(this.rdtDataDokumentu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DokDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Szczegóły dokumentu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DokDetail_FormClosing);
            this.Load += new System.EventHandler(this.DokDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rdtDataDokumentu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdtDataKsiegowania)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rmebKwota)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadDateTimePicker rdtDataDokumentu;
        private Telerik.WinControls.UI.RadDateTimePicker rdtDataKsiegowania;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private Telerik.WinControls.UI.RadMaskedEditBox rmebKwota;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbOpGl;
        private System.Windows.Forms.TextBox tbOpCz;
        private System.Windows.Forms.Button btKsieguj;
        private System.Windows.Forms.Button btCancel;
    }
}