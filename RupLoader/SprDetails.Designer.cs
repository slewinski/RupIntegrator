namespace RupLoader
{
    partial class SprDetails
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
            this.dgvDokumenty = new System.Windows.Forms.DataGridView();
            this.tbSygnatura = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbDokumenty = new System.Windows.Forms.Label();
            this.rgvStrony = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDokumenty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvStrony)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDokumenty
            // 
            this.dgvDokumenty.AllowUserToAddRows = false;
            this.dgvDokumenty.AllowUserToDeleteRows = false;
            this.dgvDokumenty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDokumenty.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDokumenty.Location = new System.Drawing.Point(4, 352);
            this.dgvDokumenty.Name = "dgvDokumenty";
            this.dgvDokumenty.ReadOnly = true;
            this.dgvDokumenty.Size = new System.Drawing.Size(992, 130);
            this.dgvDokumenty.TabIndex = 1;
            this.dgvDokumenty.DoubleClick += new System.EventHandler(this.dgvDokumenty_DoubleClick);
            // 
            // tbSygnatura
            // 
            this.tbSygnatura.Location = new System.Drawing.Point(106, 39);
            this.tbSygnatura.Name = "tbSygnatura";
            this.tbSygnatura.Size = new System.Drawing.Size(218, 20);
            this.tbSygnatura.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Sygnatura sprawy";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Uczestnicy postępowania";
            // 
            // lbDokumenty
            // 
            this.lbDokumenty.AutoSize = true;
            this.lbDokumenty.Location = new System.Drawing.Point(1, 336);
            this.lbDokumenty.Name = "lbDokumenty";
            this.lbDokumenty.Size = new System.Drawing.Size(61, 13);
            this.lbDokumenty.TabIndex = 5;
            this.lbDokumenty.Text = "Dokumenty";
            // 
            // rgvStrony
            // 
            this.rgvStrony.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.rgvStrony.Location = new System.Drawing.Point(4, 98);
            // 
            // rgvStrony
            // 
            this.rgvStrony.MasterTemplate.AllowAddNewRow = false;
            this.rgvStrony.MasterTemplate.AllowColumnReorder = false;
            this.rgvStrony.MasterTemplate.AllowDeleteRow = false;
            this.rgvStrony.MasterTemplate.AllowEditRow = false;
            this.rgvStrony.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            this.rgvStrony.MasterTemplate.EnableFiltering = true;
            this.rgvStrony.Name = "rgvStrony";
            this.rgvStrony.ReadOnly = true;
            this.rgvStrony.Size = new System.Drawing.Size(992, 235);
            this.rgvStrony.TabIndex = 6;
            // 
            // SprDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 486);
            this.Controls.Add(this.rgvStrony);
            this.Controls.Add(this.lbDokumenty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSygnatura);
            this.Controls.Add(this.dgvDokumenty);
            this.Name = "SprDetails";
            this.Text = "Szczegóły sprawy";
            this.Load += new System.EventHandler(this.SprDetails_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDokumenty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvStrony)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDokumenty;
        private System.Windows.Forms.TextBox tbSygnatura;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbDokumenty;
        private Telerik.WinControls.UI.RadGridView rgvStrony;
    }
}