namespace RupLoader
{
    partial class KuratNo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KuratNo));
            this.label1 = new System.Windows.Forms.Label();
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.rbCancel = new Telerik.WinControls.UI.RadButton();
            this.tbSygn = new System.Windows.Forms.TextBox();
            this.tbKurat = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSAPID = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).BeginInit();
            this.rbCancel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(44, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kurator:";
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(260, 3);
            this.rbOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(76, 24);
            this.rbOK.TabIndex = 9;
            this.rbOK.Text = "OK";
            this.rbOK.Click += new System.EventHandler(this.rbOK_Click);
            // 
            // rbCancel
            // 
            this.rbCancel.Controls.Add(this.tbSygn);
            this.rbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbCancel.Location = new System.Drawing.Point(340, 3);
            this.rbCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.Size = new System.Drawing.Size(78, 24);
            this.rbCancel.TabIndex = 8;
            this.rbCancel.Text = "Anuluj";
            this.rbCancel.Click += new System.EventHandler(this.rbCancel_Click);
            // 
            // tbSygn
            // 
            this.tbSygn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbSygn.Location = new System.Drawing.Point(-254, 6);
            this.tbSygn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbSygn.Name = "tbSygn";
            this.tbSygn.ReadOnly = true;
            this.tbSygn.Size = new System.Drawing.Size(228, 21);
            this.tbSygn.TabIndex = 13;
            // 
            // tbKurat
            // 
            this.tbKurat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbKurat.Location = new System.Drawing.Point(108, 48);
            this.tbKurat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbKurat.Name = "tbKurat";
            this.tbKurat.ReadOnly = true;
            this.tbKurat.Size = new System.Drawing.Size(308, 21);
            this.tbKurat.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(-2, 103);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Numer osobowy w ZSRK:";
            // 
            // tbSAPID
            // 
            this.tbSAPID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbSAPID.Location = new System.Drawing.Point(164, 99);
            this.tbSAPID.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbSAPID.Name = "tbSAPID";
            this.tbSAPID.Size = new System.Drawing.Size(116, 21);
            this.tbSAPID.TabIndex = 1;
            // 
            // KuratNo
            // 
            this.AcceptButton = this.rbOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 170);
            this.Controls.Add(this.tbSAPID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbKurat);
            this.Controls.Add(this.rbOK);
            this.Controls.Add(this.rbCancel);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "KuratNo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Numer osobowy Kuratora w ZSRK";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KuratNo_FormClosing);
            this.Load += new System.EventHandler(this.KuratNo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).EndInit();
            this.rbCancel.ResumeLayout(false);
            this.rbCancel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadButton rbOK;
        private Telerik.WinControls.UI.RadButton rbCancel;
        private System.Windows.Forms.TextBox tbKurat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSAPID;
        private System.Windows.Forms.TextBox tbSygn;
    }
}