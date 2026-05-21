namespace RupLoader
{
    partial class RupLoaderMain
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
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RupLoaderMain));
            this.tbTextAll = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btFind = new System.Windows.Forms.Button();
            this.tbFind = new System.Windows.Forms.TextBox();
            this.dgVResult = new Telerik.WinControls.UI.RadGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            this.rmRyczalty = new Telerik.WinControls.UI.RadMenuItem();
            this.rmPredykcja = new Telerik.WinControls.UI.RadMenuItem();
            this.rmKonfiguracja = new Telerik.WinControls.UI.RadMenuItem();
            this.rmKonfig = new Telerik.WinControls.UI.RadMenuItem();
            this.rmKonfigJobs = new Telerik.WinControls.UI.RadMenuItem();
            this.rmKontoMEP = new Telerik.WinControls.UI.RadMenuItem();
            this.rmiAbout = new Telerik.WinControls.UI.RadMenuItem();
            this.btLayout = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rmUserMgr = new Telerik.WinControls.UI.RadMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgVResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgVResult.MasterTemplate)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbTextAll
            // 
            this.tbTextAll.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbTextAll.Location = new System.Drawing.Point(383, 24);
            this.tbTextAll.Multiline = true;
            this.tbTextAll.Name = "tbTextAll";
            this.tbTextAll.Size = new System.Drawing.Size(592, 69);
            this.tbTextAll.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Podaj klucz wyszukiwania";
            // 
            // btFind
            // 
            this.btFind.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btFind.Location = new System.Drawing.Point(250, 23);
            this.btFind.Name = "btFind";
            this.btFind.Size = new System.Drawing.Size(75, 23);
            this.btFind.TabIndex = 11;
            this.btFind.Text = "&Szukaj";
            this.btFind.UseVisualStyleBackColor = true;
            this.btFind.Click += new System.EventHandler(this.btFind_Click);
            // 
            // tbFind
            // 
            this.tbFind.Location = new System.Drawing.Point(3, 52);
            this.tbFind.Name = "tbFind";
            this.tbFind.Size = new System.Drawing.Size(322, 20);
            this.tbFind.TabIndex = 2;
            // 
            // dgVResult
            // 
            this.dgVResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgVResult.EnableCustomFiltering = true;
            this.dgVResult.Location = new System.Drawing.Point(0, 0);
            // 
            // 
            // 
            this.dgVResult.MasterTemplate.AllowAddNewRow = false;
            this.dgVResult.MasterTemplate.AllowCellContextMenu = false;
            this.dgVResult.MasterTemplate.AllowDeleteRow = false;
            this.dgVResult.MasterTemplate.AllowDragToGroup = false;
            this.dgVResult.MasterTemplate.EnableCustomFiltering = true;
            this.dgVResult.MasterTemplate.EnableFiltering = true;
            this.dgVResult.MasterTemplate.EnableGrouping = false;
            this.dgVResult.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.dgVResult.Name = "dgVResult";
            this.dgVResult.Size = new System.Drawing.Size(1113, 329);
            this.dgVResult.TabIndex = 12;
            this.dgVResult.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.dgVResult_ContextMenuOpening);
            this.dgVResult.DoubleClick += new System.EventHandler(this.dgVResult_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.radMenu1);
            this.panel1.Controls.Add(this.btLayout);
            this.panel1.Controls.Add(this.tbTextAll);
            this.panel1.Controls.Add(this.tbFind);
            this.panel1.Controls.Add(this.btFind);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(2, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1113, 96);
            this.panel1.TabIndex = 13;
            // 
            // radMenu1
            // 
            this.radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.rmRyczalty,
            this.rmPredykcja,
            this.rmKonfiguracja,
            this.rmiAbout});
            this.radMenu1.Location = new System.Drawing.Point(0, 0);
            this.radMenu1.Name = "radMenu1";
            this.radMenu1.Size = new System.Drawing.Size(1113, 20);
            this.radMenu1.TabIndex = 13;
            // 
            // rmRyczalty
            // 
            this.rmRyczalty.Name = "rmRyczalty";
            this.rmRyczalty.Text = "Ryczałty/Rozrachunki";
            this.rmRyczalty.Click += new System.EventHandler(this.rmRyczalty_Click);
            // 
            // rmPredykcja
            // 
            this.rmPredykcja.Name = "rmPredykcja";
            this.rmPredykcja.Text = "Predykcja księgowań wyciągów";
            this.rmPredykcja.Click += new System.EventHandler(this.rmPredykcja_Click);
            // 
            // rmKonfiguracja
            // 
            this.rmKonfiguracja.DisplayStyle = Telerik.WinControls.DisplayStyle.ImageAndText;
            this.rmKonfiguracja.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.rmKonfig,
            this.rmKonfigJobs,
            this.rmKontoMEP,
            this.rmUserMgr});
            this.rmKonfiguracja.Name = "rmKonfiguracja";
            this.rmKonfiguracja.Text = "Konfiguracja";
            // 
            // rmKonfig
            // 
            this.rmKonfig.Name = "rmKonfig";
            this.rmKonfig.Text = "Ustawienie konfiguracyjne";
            this.rmKonfig.Click += new System.EventHandler(this.rmKonfig_Click);
            // 
            // rmKonfigJobs
            // 
            this.rmKonfigJobs.Name = "rmKonfigJobs";
            this.rmKonfigJobs.Text = "Zadania w harmonogramie";
            this.rmKonfigJobs.Click += new System.EventHandler(this.rmKonfigJobs_Click);
            // 
            // rmKontoMEP
            // 
            this.rmKontoMEP.Name = "rmKontoMEP";
            this.rmKontoMEP.Text = "Konto MEP";
            this.rmKontoMEP.Click += new System.EventHandler(this.rmKontoMEP_Click);
            // 
            // rmiAbout
            // 
            this.rmiAbout.Name = "rmiAbout";
            this.rmiAbout.Text = "O programie";
            this.rmiAbout.Click += new System.EventHandler(this.rmiAbout_Click);
            // 
            // btLayout
            // 
            this.btLayout.Location = new System.Drawing.Point(3, 71);
            this.btLayout.Name = "btLayout";
            this.btLayout.Size = new System.Drawing.Size(27, 23);
            this.btLayout.TabIndex = 12;
            this.btLayout.Text = "U";
            this.btLayout.UseVisualStyleBackColor = true;
            this.btLayout.Click += new System.EventHandler(this.btLayout_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.dgVResult);
            this.panel2.Location = new System.Drawing.Point(2, 103);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1113, 329);
            this.panel2.TabIndex = 14;
            // 
            // rmUserMgr
            // 
            this.rmUserMgr.Name = "rmUserMgr";
            this.rmUserMgr.Text = "Zarządzanie użytkownikami";
            this.rmUserMgr.Click += new System.EventHandler(this.rmUserMgr_Click);
            // 
            // RupLoaderMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1117, 433);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RupLoaderMain";
            this.Text = "RupLoader";
            this.Load += new System.EventHandler(this.RupLoaderMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgVResult.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgVResult)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbTextAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btFind;
        private System.Windows.Forms.TextBox tbFind;
        private Telerik.WinControls.UI.RadGridView dgVResult;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btLayout;
        private Telerik.WinControls.UI.RadMenuItem rmKonfiguracja;
        private Telerik.WinControls.UI.RadMenu radMenu1;
        private Telerik.WinControls.UI.RadMenuItem rmiAbout;
        private Telerik.WinControls.UI.RadMenuItem rmRyczalty;
        private Telerik.WinControls.UI.RadMenuItem rmKonfig;
        private Telerik.WinControls.UI.RadMenuItem rmKonfigJobs;
        private Telerik.WinControls.UI.RadMenuItem rmPredykcja;
        private Telerik.WinControls.UI.RadMenuItem rmKontoMEP;
        private Telerik.WinControls.UI.RadMenuItem rmUserMgr;
    }
}

