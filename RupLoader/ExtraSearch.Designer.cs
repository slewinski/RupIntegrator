namespace RupLoader
{
    partial class ExtraSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtraSearch));
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.rgvSearch = new Telerik.WinControls.UI.RadGridView();
            this.bt = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bt);
            this.panel1.Controls.Add(this.rbOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1040, 32);
            this.panel1.TabIndex = 0;
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(35, 5);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(110, 24);
            this.rbOK.TabIndex = 0;
            this.rbOK.Text = "Wybierz";
            this.rbOK.Click += new System.EventHandler(this.rbOK_Click);
            // 
            // rgvSearch
            // 
            this.rgvSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rgvSearch.Location = new System.Drawing.Point(0, 32);
            // 
            // rgvSearch
            // 
            this.rgvSearch.MasterTemplate.AllowAddNewRow = false;
            this.rgvSearch.MasterTemplate.AllowDeleteRow = false;
            this.rgvSearch.MasterTemplate.EnableFiltering = true;
            this.rgvSearch.MasterTemplate.EnableGrouping = false;
            this.rgvSearch.Name = "rgvSearch";
            this.rgvSearch.Size = new System.Drawing.Size(1040, 317);
            this.rgvSearch.TabIndex = 1;
            this.rgvSearch.Text = "radGridView1";
            this.rgvSearch.DoubleClick += new System.EventHandler(this.rgvSearch_DoubleClick);
            // 
            // bt
            // 
            this.bt.Location = new System.Drawing.Point(0, 5);
            this.bt.Name = "bt";
            this.bt.Size = new System.Drawing.Size(29, 23);
            this.bt.TabIndex = 1;
            this.bt.Text = "U";
            this.bt.UseVisualStyleBackColor = true;
            this.bt.Click += new System.EventHandler(this.bt_Click);
            // 
            // ExtraSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 349);
            this.Controls.Add(this.rgvSearch);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExtraSearch";
            this.Text = "Wyszukiwanie dodatkowe";
            this.Load += new System.EventHandler(this.ExtraSearch_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvSearch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadGridView rgvSearch;
        private Telerik.WinControls.UI.RadButton rbOK;
        private System.Windows.Forms.Button bt;
    }
}