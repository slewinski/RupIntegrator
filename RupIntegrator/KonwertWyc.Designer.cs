namespace KnsMigrator
{
    partial class KonwertWyc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KonwertWyc));
            this.label1 = new System.Windows.Forms.Label();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.btSrc = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btDest = new System.Windows.Forms.Button();
            this.btKonwert = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.rmbSaldo = new Telerik.WinControls.UI.RadMaskedEditBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbUtf = new System.Windows.Forms.RadioButton();
            this.rbWin = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.rmbSaldo)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(23, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Zbiór żródłowy";
            // 
            // tbInput
            // 
            this.tbInput.Location = new System.Drawing.Point(133, 51);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(383, 20);
            this.tbInput.TabIndex = 1;
            // 
            // btSrc
            // 
            this.btSrc.Location = new System.Drawing.Point(522, 49);
            this.btSrc.Name = "btSrc";
            this.btSrc.Size = new System.Drawing.Size(28, 23);
            this.btSrc.TabIndex = 2;
            this.btSrc.Text = "...";
            this.btSrc.UseVisualStyleBackColor = true;
            this.btSrc.Click += new System.EventHandler(this.btSrc_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(31, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Data wyciągu";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(0, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Saldo początkowe";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(20, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Zbiór wynikowy";
            // 
            // btDest
            // 
            this.btDest.Location = new System.Drawing.Point(522, 135);
            this.btDest.Name = "btDest";
            this.btDest.Size = new System.Drawing.Size(28, 23);
            this.btDest.TabIndex = 6;
            this.btDest.Text = "...";
            this.btDest.UseVisualStyleBackColor = true;
            this.btDest.Click += new System.EventHandler(this.btDest_Click);
            // 
            // btKonwert
            // 
            this.btKonwert.Location = new System.Drawing.Point(247, 220);
            this.btKonwert.Name = "btKonwert";
            this.btKonwert.Size = new System.Drawing.Size(103, 23);
            this.btKonwert.TabIndex = 7;
            this.btKonwert.Text = "Konwertuj";
            this.btKonwert.UseVisualStyleBackColor = true;
            this.btKonwert.Click += new System.EventHandler(this.btKonwert_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(133, 77);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(125, 20);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(133, 137);
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.Size = new System.Drawing.Size(383, 20);
            this.tbOutput.TabIndex = 5;
            // 
            // rmbSaldo
            // 
            this.rmbSaldo.HideSelection = false;
            this.rmbSaldo.Location = new System.Drawing.Point(133, 103);
            this.rmbSaldo.Mask = "C2";
            this.rmbSaldo.MaskType = Telerik.WinControls.UI.MaskType.Numeric;
            this.rmbSaldo.Name = "rmbSaldo";
            this.rmbSaldo.Size = new System.Drawing.Size(125, 20);
            this.rmbSaldo.TabIndex = 4;
            this.rmbSaldo.TabStop = false;
            this.rmbSaldo.Text = "0,00 zł";
            this.rmbSaldo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbWin);
            this.groupBox1.Controls.Add(this.rbUtf);
            this.groupBox1.Location = new System.Drawing.Point(133, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 39);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "kodowanie";
            // 
            // rbUtf
            // 
            this.rbUtf.AutoSize = true;
            this.rbUtf.Checked = true;
            this.rbUtf.Location = new System.Drawing.Point(73, 16);
            this.rbUtf.Name = "rbUtf";
            this.rbUtf.Size = new System.Drawing.Size(55, 17);
            this.rbUtf.TabIndex = 0;
            this.rbUtf.Text = "UTF-8";
            this.rbUtf.UseVisualStyleBackColor = true;
            // 
            // rbWin
            // 
            this.rbWin.AutoSize = true;
            this.rbWin.Location = new System.Drawing.Point(171, 16);
            this.rbWin.Name = "rbWin";
            this.rbWin.Size = new System.Drawing.Size(99, 17);
            this.rbWin.TabIndex = 1;
            this.rbWin.Text = "Windows -1250";
            this.rbWin.UseVisualStyleBackColor = true;
            // 
            // KonwertWyc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 255);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rmbSaldo);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.btKonwert);
            this.Controls.Add(this.btDest);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btSrc);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "KonwertWyc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Konwerter wyciągu bankowego - emulacja";
            ((System.ComponentModel.ISupportInitialize)(this.rmbSaldo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.Button btSrc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btDest;
        private System.Windows.Forms.Button btKonwert;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TextBox tbOutput;
        private Telerik.WinControls.UI.RadMaskedEditBox rmbSaldo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbWin;
        private System.Windows.Forms.RadioButton rbUtf;
    }
}