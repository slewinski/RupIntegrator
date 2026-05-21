namespace RupLoader
{
    partial class MapSadKurat
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapSadKurat));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.rbCancel = new Telerik.WinControls.UI.RadButton();
            this.tbSapSad = new System.Windows.Forms.TextBox();
            this.tbSad = new System.Windows.Forms.TextBox();
            this.tbSygn = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rgvSAPSad = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSAPSad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSAPSad.MasterTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rbOK);
            this.splitContainer1.Panel1.Controls.Add(this.rbCancel);
            this.splitContainer1.Panel1.Controls.Add(this.tbSapSad);
            this.splitContainer1.Panel1.Controls.Add(this.tbSad);
            this.splitContainer1.Panel1.Controls.Add(this.tbSygn);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rgvSAPSad);
            this.splitContainer1.Size = new System.Drawing.Size(785, 332);
            this.splitContainer1.SplitterDistance = 101;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 0;
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(616, 3);
            this.rbOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(76, 24);
            this.rbOK.TabIndex = 7;
            this.rbOK.Text = "OK";
            // 
            // rbCancel
            // 
            this.rbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbCancel.Location = new System.Drawing.Point(696, 3);
            this.rbCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.Size = new System.Drawing.Size(78, 24);
            this.rbCancel.TabIndex = 6;
            this.rbCancel.Text = "Anuluj";
            this.rbCancel.Click += new System.EventHandler(this.rbCancel_Click);
            // 
            // tbSapSad
            // 
            this.tbSapSad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbSapSad.ForeColor = System.Drawing.Color.DodgerBlue;
            this.tbSapSad.Location = new System.Drawing.Point(188, 56);
            this.tbSapSad.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbSapSad.Name = "tbSapSad";
            this.tbSapSad.ReadOnly = true;
            this.tbSapSad.Size = new System.Drawing.Size(586, 21);
            this.tbSapSad.TabIndex = 5;
            // 
            // tbSad
            // 
            this.tbSad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbSad.ForeColor = System.Drawing.Color.DodgerBlue;
            this.tbSad.Location = new System.Drawing.Point(188, 35);
            this.tbSad.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbSad.Name = "tbSad";
            this.tbSad.ReadOnly = true;
            this.tbSad.Size = new System.Drawing.Size(586, 21);
            this.tbSad.TabIndex = 4;
            // 
            // tbSygn
            // 
            this.tbSygn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbSygn.ForeColor = System.Drawing.Color.DodgerBlue;
            this.tbSygn.Location = new System.Drawing.Point(188, 8);
            this.tbSygn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbSygn.Name = "tbSygn";
            this.tbSygn.ReadOnly = true;
            this.tbSygn.Size = new System.Drawing.Size(174, 21);
            this.tbSygn.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(59, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Sąd w systemie ZSRK";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(122, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sygnatura";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(9, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sąd w systemie merytorycznym";
            // 
            // rgvSAPSad
            // 
            this.rgvSAPSad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rgvSAPSad.Location = new System.Drawing.Point(0, 0);
            this.rgvSAPSad.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            // 
            // 
            // 
            this.rgvSAPSad.MasterTemplate.AllowAddNewRow = false;
            this.rgvSAPSad.MasterTemplate.AllowColumnReorder = false;
            this.rgvSAPSad.MasterTemplate.AllowDeleteRow = false;
            this.rgvSAPSad.MasterTemplate.AllowDragToGroup = false;
            this.rgvSAPSad.MasterTemplate.AllowEditRow = false;
            this.rgvSAPSad.MasterTemplate.AutoGenerateColumns = false;
            gridViewTextBoxColumn1.FieldName = "kod";
            gridViewTextBoxColumn1.HeaderText = "Kod";
            gridViewTextBoxColumn1.Name = "kod";
            gridViewTextBoxColumn1.Width = 60;
            gridViewTextBoxColumn2.FieldName = "miasto";
            gridViewTextBoxColumn2.HeaderText = "Miasto";
            gridViewTextBoxColumn2.Name = "miasto";
            gridViewTextBoxColumn2.Width = 200;
            gridViewTextBoxColumn3.FieldName = "sad";
            gridViewTextBoxColumn3.HeaderText = "Sąd";
            gridViewTextBoxColumn3.Name = "sad";
            gridViewTextBoxColumn3.Width = 500;
            gridViewTextBoxColumn4.FieldName = "typSad";
            gridViewTextBoxColumn4.HeaderText = "Typ";
            gridViewTextBoxColumn4.Name = "typ";
            this.rgvSAPSad.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4});
            this.rgvSAPSad.MasterTemplate.EnableFiltering = true;
            this.rgvSAPSad.MasterTemplate.EnableGrouping = false;
            this.rgvSAPSad.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.rgvSAPSad.Name = "rgvSAPSad";
            this.rgvSAPSad.Size = new System.Drawing.Size(785, 228);
            this.rgvSAPSad.TabIndex = 0;
            this.rgvSAPSad.Text = "radGridView1";
            this.rgvSAPSad.DoubleClick += new System.EventHandler(this.rgvSAPSad_DoubleClick);
            // 
            // MapSadKurat
            // 
            this.AcceptButton = this.rbOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 332);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MapSadKurat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ustalanie numeru sądu  w ZSRK";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapSadKurat_FormClosing);
            this.Load += new System.EventHandler(this.MapSadKurat_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSAPSad.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSAPSad)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Telerik.WinControls.UI.RadGridView rgvSAPSad;
        private System.Windows.Forms.TextBox tbSapSad;
        private System.Windows.Forms.TextBox tbSad;
        private System.Windows.Forms.TextBox tbSygn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadButton rbOK;
        private Telerik.WinControls.UI.RadButton rbCancel;
    }
}