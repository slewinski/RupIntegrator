namespace RupBig
{
    partial class WndSelBIG
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
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WndSelBIG));
            this.label1 = new System.Windows.Forms.Label();
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.rbCancel = new Telerik.WinControls.UI.RadButton();
            this.rgvBIGs = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvBIGs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvBIGs.MasterTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(27, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(299, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Zaznacz BIG\'i do których dodać zobowiązania";
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(180, 12);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(83, 24);
            this.rbOK.TabIndex = 1;
            this.rbOK.Text = "OK";
            // 
            // rbCancel
            // 
            this.rbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbCancel.Location = new System.Drawing.Point(268, 12);
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.Size = new System.Drawing.Size(83, 24);
            this.rbCancel.TabIndex = 2;
            this.rbCancel.Text = "Anuluj";
            // 
            // rgvBIGs
            // 
            this.rgvBIGs.Location = new System.Drawing.Point(3, 69);
            // 
            // 
            // 
            this.rgvBIGs.MasterTemplate.AllowAddNewRow = false;
            this.rgvBIGs.MasterTemplate.AllowColumnReorder = false;
            this.rgvBIGs.MasterTemplate.AllowDeleteRow = false;
            gridViewCheckBoxColumn1.FieldName = "Obsluga";
            gridViewCheckBoxColumn1.HeaderText = "T/N";
            gridViewCheckBoxColumn1.Name = "Obsluga";
            gridViewTextBoxColumn1.FieldName = "BIGID";
            gridViewTextBoxColumn1.HeaderText = "BIG";
            gridViewTextBoxColumn1.Name = "BIGID";
            gridViewTextBoxColumn1.Width = 100;
            this.rgvBIGs.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewCheckBoxColumn1,
            gridViewTextBoxColumn1});
            this.rgvBIGs.MasterTemplate.EnableGrouping = false;
            this.rgvBIGs.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.rgvBIGs.Name = "rgvBIGs";
            this.rgvBIGs.Size = new System.Drawing.Size(348, 149);
            this.rgvBIGs.TabIndex = 3;
            this.rgvBIGs.Text = "radGridView1";
            // 
            // WndSelBIG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 221);
            this.Controls.Add(this.rgvBIGs);
            this.Controls.Add(this.rbCancel);
            this.Controls.Add(this.rbOK);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WndSelBIG";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wskaż BIG\'i";
            this.Load += new System.EventHandler(this.WndSelBIG_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvBIGs.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvBIGs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadButton rbOK;
        private Telerik.WinControls.UI.RadButton rbCancel;
        private Telerik.WinControls.UI.RadGridView rgvBIGs;
    }
}