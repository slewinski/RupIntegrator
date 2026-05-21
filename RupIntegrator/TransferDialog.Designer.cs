namespace KnsMigrator
{
    partial class TransferDialog
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
            this.components = new System.ComponentModel.Container();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn1 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            this.rbCancel = new Telerik.WinControls.UI.RadButton();
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTyp = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rdtOd = new Telerik.WinControls.UI.RadDateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.rdtDo = new Telerik.WinControls.UI.RadDateTimePicker();
            this.rtbUwagi = new Telerik.WinControls.UI.RadTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbKsiegi = new System.Windows.Forms.Label();
            this.chNewOnly = new System.Windows.Forms.CheckBox();
            this.rgvKsiegi = new Telerik.WinControls.UI.RadGridView();
            this.knsKsiegiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdtOd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdtDo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rtbUwagi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvKsiegi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvKsiegi.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.knsKsiegiBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // rbCancel
            // 
            this.rbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbCancel.Location = new System.Drawing.Point(372, 543);
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.Size = new System.Drawing.Size(110, 24);
            this.rbCancel.TabIndex = 0;
            this.rbCancel.Text = "Anuluj";
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(256, 543);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(110, 24);
            this.rbOK.TabIndex = 1;
            this.rbOK.Text = "OK";
            this.rbOK.Click += new System.EventHandler(this.rbOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(94, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Rodzaj importu";
            // 
            // labelTyp
            // 
            this.labelTyp.AutoSize = true;
            this.labelTyp.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTyp.Location = new System.Drawing.Point(198, 17);
            this.labelTyp.Name = "labelTyp";
            this.labelTyp.Size = new System.Drawing.Size(40, 17);
            this.labelTyp.TabIndex = 3;
            this.labelTyp.Text = "Salda";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(25, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Import za okres od:";
            // 
            // rdtOd
            // 
            this.rdtOd.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rdtOd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.rdtOd.Location = new System.Drawing.Point(154, 53);
            this.rdtOd.Name = "rdtOd";
            this.rdtOd.Size = new System.Drawing.Size(93, 23);
            this.rdtOd.TabIndex = 5;
            this.rdtOd.TabStop = false;
            this.rdtOd.Text = "2014-01-13";
            this.rdtOd.Value = new System.DateTime(2014, 1, 13, 12, 28, 2, 762);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(253, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "do( stan na dzień):";
            // 
            // rdtDo
            // 
            this.rdtDo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rdtDo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.rdtDo.Location = new System.Drawing.Point(375, 53);
            this.rdtDo.Name = "rdtDo";
            this.rdtDo.Size = new System.Drawing.Size(93, 23);
            this.rdtDo.TabIndex = 6;
            this.rdtDo.TabStop = false;
            this.rdtDo.Text = "2014-01-13";
            this.rdtDo.Value = new System.DateTime(2014, 1, 13, 12, 28, 2, 762);
            // 
            // rtbUwagi
            // 
            this.rtbUwagi.Location = new System.Drawing.Point(65, 516);
            this.rtbUwagi.Name = "rtbUwagi";
            this.rtbUwagi.Size = new System.Drawing.Size(403, 20);
            this.rtbUwagi.TabIndex = 7;
            this.rtbUwagi.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(12, 516);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Uwagi:";
            // 
            // lbKsiegi
            // 
            this.lbKsiegi.AutoSize = true;
            this.lbKsiegi.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbKsiegi.Location = new System.Drawing.Point(6, 107);
            this.lbKsiegi.Name = "lbKsiegi";
            this.lbKsiegi.Size = new System.Drawing.Size(169, 17);
            this.lbKsiegi.TabIndex = 10;
            this.lbKsiegi.Text = "Wybierz księgi do importu :";
            // 
            // chNewOnly
            // 
            this.chNewOnly.AutoSize = true;
            this.chNewOnly.Checked = true;
            this.chNewOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chNewOnly.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chNewOnly.Location = new System.Drawing.Point(154, 83);
            this.chNewOnly.Name = "chNewOnly";
            this.chNewOnly.Size = new System.Drawing.Size(139, 21);
            this.chNewOnly.TabIndex = 11;
            this.chNewOnly.Text = "Tylko nowe pozycje";
            this.chNewOnly.UseVisualStyleBackColor = true;
            this.chNewOnly.CheckedChanged += new System.EventHandler(this.chNewOnly_CheckedChanged);
            // 
            // rgvKsiegi
            // 
            this.rgvKsiegi.EnableCustomGrouping = true;
            this.rgvKsiegi.Location = new System.Drawing.Point(-4, 137);
            // 
            // 
            // 
            this.rgvKsiegi.MasterTemplate.AllowAddNewRow = false;
            this.rgvKsiegi.MasterTemplate.AllowColumnReorder = false;
            this.rgvKsiegi.MasterTemplate.AllowDeleteRow = false;
            this.rgvKsiegi.MasterTemplate.AutoGenerateColumns = false;
            gridViewCheckBoxColumn1.HeaderText = "";
            gridViewCheckBoxColumn1.Name = "taknie";
            gridViewDecimalColumn1.FieldName = "Id_Ksiegi";
            gridViewDecimalColumn1.HeaderText = "";
            gridViewDecimalColumn1.IsVisible = false;
            gridViewDecimalColumn1.Name = "Id_Ksiegi";
            gridViewTextBoxColumn1.FieldName = "nazwa";
            gridViewTextBoxColumn1.HeaderText = "Księga należności ";
            gridViewTextBoxColumn1.Name = "Nazwa";
            gridViewTextBoxColumn1.Width = 400;
            this.rgvKsiegi.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewCheckBoxColumn1,
            gridViewDecimalColumn1,
            gridViewTextBoxColumn1});
            this.rgvKsiegi.MasterTemplate.EnableCustomGrouping = true;
            this.rgvKsiegi.MasterTemplate.EnableGrouping = false;
            this.rgvKsiegi.MasterTemplate.MultiSelect = true;
            this.rgvKsiegi.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.rgvKsiegi.Name = "rgvKsiegi";
            this.rgvKsiegi.Size = new System.Drawing.Size(486, 373);
            this.rgvKsiegi.TabIndex = 12;
            this.rgvKsiegi.Text = "radGridView1";
            // 
            // knsKsiegiBindingSource
            // 
            this.knsKsiegiBindingSource.DataSource = typeof(KnsMigrator.KnsKsiegi);
            // 
            // TransferDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 568);
            this.Controls.Add(this.rgvKsiegi);
            this.Controls.Add(this.chNewOnly);
            this.Controls.Add(this.lbKsiegi);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rtbUwagi);
            this.Controls.Add(this.rdtDo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rdtOd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelTyp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbOK);
            this.Controls.Add(this.rbCancel);
            this.Name = "TransferDialog";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import danych z systemu dziedzinowego";
            this.ThemeName = "ControlDefault";
            this.Load += new System.EventHandler(this.TransferDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdtOd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdtDo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rtbUwagi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvKsiegi.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvKsiegi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.knsKsiegiBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton rbCancel;
        private Telerik.WinControls.UI.RadButton rbOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTyp;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.UI.RadDateTimePicker rdtOd;
        private System.Windows.Forms.Label label3;
        private Telerik.WinControls.UI.RadDateTimePicker rdtDo;
        private Telerik.WinControls.UI.RadTextBox rtbUwagi;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.BindingSource knsKsiegiBindingSource;
        private System.Windows.Forms.Label lbKsiegi;
        private System.Windows.Forms.CheckBox chNewOnly;
        private Telerik.WinControls.UI.RadGridView rgvKsiegi;
    }
}
