namespace RupLoader
{
    partial class MapSygnatura
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
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn1 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn2 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapSygnatura));
            this.radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            this.splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            this.rbAdd = new Telerik.WinControls.UI.RadButton();
            this.rbDell = new Telerik.WinControls.UI.RadButton();
            this.rbClose = new Telerik.WinControls.UI.RadButton();
            this.rbSave = new Telerik.WinControls.UI.RadButton();
            this.splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            this.rgvSygnMap = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).BeginInit();
            this.radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).BeginInit();
            this.splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbDell)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).BeginInit();
            this.splitPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSygnMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSygnMap.MasterTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // radSplitContainer1
            // 
            this.radSplitContainer1.Controls.Add(this.splitPanel1);
            this.radSplitContainer1.Controls.Add(this.splitPanel2);
            this.radSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.radSplitContainer1.Name = "radSplitContainer1";
            this.radSplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // 
            // 
            this.radSplitContainer1.RootElement.MinSize = new System.Drawing.Size(0, 0);
            this.radSplitContainer1.Size = new System.Drawing.Size(776, 417);
            this.radSplitContainer1.TabIndex = 0;
            this.radSplitContainer1.TabStop = false;
            this.radSplitContainer1.Text = "radSplitContainer1";
            // 
            // splitPanel1
            // 
            this.splitPanel1.Controls.Add(this.rbAdd);
            this.splitPanel1.Controls.Add(this.rbDell);
            this.splitPanel1.Controls.Add(this.rbClose);
            this.splitPanel1.Controls.Add(this.rbSave);
            this.splitPanel1.Location = new System.Drawing.Point(0, 0);
            this.splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            this.splitPanel1.RootElement.MinSize = new System.Drawing.Size(0, 0);
            this.splitPanel1.Size = new System.Drawing.Size(776, 40);
            this.splitPanel1.SizeInfo.AbsoluteSize = new System.Drawing.Size(200, 40);
            this.splitPanel1.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Absolute;
            this.splitPanel1.TabIndex = 0;
            this.splitPanel1.TabStop = false;
            this.splitPanel1.Text = "splitPanel1";
            // 
            // rbAdd
            // 
            this.rbAdd.Location = new System.Drawing.Point(355, 8);
            this.rbAdd.Name = "rbAdd";
            this.rbAdd.Size = new System.Drawing.Size(100, 24);
            this.rbAdd.TabIndex = 4;
            this.rbAdd.Text = "Dodaj";
            this.rbAdd.Click += new System.EventHandler(this.rbAdd_Click);
            // 
            // rbDell
            // 
            this.rbDell.Location = new System.Drawing.Point(461, 8);
            this.rbDell.Name = "rbDell";
            this.rbDell.Size = new System.Drawing.Size(100, 24);
            this.rbDell.TabIndex = 2;
            this.rbDell.Text = "Usuń";
            this.rbDell.Click += new System.EventHandler(this.rbDell_Click);
            // 
            // rbClose
            // 
            this.rbClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbClose.Location = new System.Drawing.Point(674, 8);
            this.rbClose.Name = "rbClose";
            this.rbClose.Size = new System.Drawing.Size(96, 24);
            this.rbClose.TabIndex = 1;
            this.rbClose.Text = "Zamknij";
            // 
            // rbSave
            // 
            this.rbSave.Location = new System.Drawing.Point(567, 8);
            this.rbSave.Name = "rbSave";
            this.rbSave.Size = new System.Drawing.Size(101, 24);
            this.rbSave.TabIndex = 0;
            this.rbSave.Text = "Zapisz";
            this.rbSave.Click += new System.EventHandler(this.rbSave_Click);
            // 
            // splitPanel2
            // 
            this.splitPanel2.Controls.Add(this.rgvSygnMap);
            this.splitPanel2.Location = new System.Drawing.Point(0, 44);
            this.splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            this.splitPanel2.RootElement.MinSize = new System.Drawing.Size(0, 0);
            this.splitPanel2.Size = new System.Drawing.Size(776, 373);
            this.splitPanel2.TabIndex = 1;
            this.splitPanel2.TabStop = false;
            this.splitPanel2.Text = "splitPanel2";
            // 
            // rgvSygnMap
            // 
            this.rgvSygnMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rgvSygnMap.Location = new System.Drawing.Point(0, 0);
            // 
            // 
            // 
            this.rgvSygnMap.MasterTemplate.AllowAddNewRow = false;
            gridViewComboBoxColumn1.FieldName = "SrcSad";
            gridViewComboBoxColumn1.HeaderText = "Sąd ( źródło)";
            gridViewComboBoxColumn1.Name = "SrcSad";
            gridViewComboBoxColumn1.Width = 120;
            gridViewTextBoxColumn1.FieldName = "SrcWydz";
            gridViewTextBoxColumn1.HeaderText = "Wydział ( źródło )";
            gridViewTextBoxColumn1.Name = "SrcWydz";
            gridViewTextBoxColumn1.Width = 90;
            gridViewTextBoxColumn2.FieldName = "SrcRep";
            gridViewTextBoxColumn2.HeaderText = "Repertorium ( źródło)";
            gridViewTextBoxColumn2.Name = "SrcRep";
            gridViewTextBoxColumn2.Width = 70;
            gridViewComboBoxColumn2.FieldName = "DestSad";
            gridViewComboBoxColumn2.HeaderText = "Sąd Docelowy";
            gridViewComboBoxColumn2.Name = "DestSad";
            gridViewComboBoxColumn2.Width = 150;
            gridViewTextBoxColumn3.FieldName = "DestWydz";
            gridViewTextBoxColumn3.HeaderText = "Wydział docelowy";
            gridViewTextBoxColumn3.Name = "DestWydz";
            gridViewTextBoxColumn3.Width = 70;
            gridViewTextBoxColumn4.FieldName = "DestRep";
            gridViewTextBoxColumn4.HeaderText = "Repertorium docelowe";
            gridViewTextBoxColumn4.Name = "DestRep";
            gridViewTextBoxColumn4.Width = 70;
            gridViewTextBoxColumn5.FieldName = "DestNr";
            gridViewTextBoxColumn5.HeaderText = "Numer docelowy";
            gridViewTextBoxColumn5.Name = "DestNr";
            gridViewTextBoxColumn5.Width = 70;
            gridViewTextBoxColumn6.FieldName = "DestRok";
            gridViewTextBoxColumn6.HeaderText = "Rok docelowy";
            gridViewTextBoxColumn6.Name = "DestRok";
            gridViewTextBoxColumn6.Width = 70;
            this.rgvSygnMap.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewComboBoxColumn1,
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewComboBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5,
            gridViewTextBoxColumn6});
            this.rgvSygnMap.MasterTemplate.EnableGrouping = false;
            this.rgvSygnMap.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.rgvSygnMap.Name = "rgvSygnMap";
            this.rgvSygnMap.Size = new System.Drawing.Size(776, 373);
            this.rgvSygnMap.TabIndex = 1;
            this.rgvSygnMap.Text = "radGridView1";
            // 
            // MapSygnatura
            // 
            this.AcceptButton = this.rbSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.rbClose;
            this.ClientSize = new System.Drawing.Size(776, 417);
            this.Controls.Add(this.radSplitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MapSygnatura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mapowanie sygnatur";
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).EndInit();
            this.radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).EndInit();
            this.splitPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rbAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbDell)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).EndInit();
            this.splitPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rgvSygnMap.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSygnMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        private Telerik.WinControls.UI.RadButton rbClose;
        private Telerik.WinControls.UI.RadButton rbSave;
        private Telerik.WinControls.UI.RadButton rbDell;
        private Telerik.WinControls.UI.RadButton rbAdd;
        private Telerik.WinControls.UI.RadGridView rgvSygnMap;
    }
}