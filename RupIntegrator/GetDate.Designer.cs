namespace KnsMigrator
{
    partial class GetDate
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
            this.lbPromt = new System.Windows.Forms.Label();
            this.bt_OK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.dtPicker = new Telerik.WinControls.UI.RadDateTimePicker();
            this.cbLeave = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dtPicker)).BeginInit();
            this.SuspendLayout();
            // 
            // lbPromt
            // 
            this.lbPromt.AutoSize = true;
            this.lbPromt.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbPromt.Location = new System.Drawing.Point(28, 52);
            this.lbPromt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbPromt.Name = "lbPromt";
            this.lbPromt.Size = new System.Drawing.Size(457, 24);
            this.lbPromt.TabIndex = 1;
            this.lbPromt.Text = "Podaj dzień, na który przeprowadzić import  słownika";
            this.lbPromt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bt_OK
            // 
            this.bt_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_OK.Location = new System.Drawing.Point(324, 4);
            this.bt_OK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_OK.Name = "bt_OK";
            this.bt_OK.Size = new System.Drawing.Size(100, 28);
            this.bt_OK.TabIndex = 4;
            this.bt_OK.Text = "OK";
            this.bt_OK.UseVisualStyleBackColor = true;
            this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(432, 4);
            this.btCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(100, 28);
            this.btCancel.TabIndex = 5;
            this.btCancel.Text = "Anuluj";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // dtPicker
            // 
            this.dtPicker.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dtPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtPicker.Location = new System.Drawing.Point(204, 92);
            this.dtPicker.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(139, 27);
            this.dtPicker.TabIndex = 6;
            this.dtPicker.TabStop = false;
            this.dtPicker.Value = new System.DateTime(((long)(0)));
            // 
            // cbLeave
            // 
            this.cbLeave.AutoSize = true;
            this.cbLeave.Checked = true;
            this.cbLeave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLeave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cbLeave.Location = new System.Drawing.Point(204, 146);
            this.cbLeave.Margin = new System.Windows.Forms.Padding(4);
            this.cbLeave.Name = "cbLeave";
            this.cbLeave.Size = new System.Drawing.Size(169, 22);
            this.cbLeave.TabIndex = 7;
            this.cbLeave.Text = "Pozostaw bez zmian";
            this.cbLeave.UseVisualStyleBackColor = true;
            this.cbLeave.CheckedChanged += new System.EventHandler(this.cbLeave_CheckedChanged);
            // 
            // GetDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 182);
            this.Controls.Add(this.cbLeave);
            this.Controls.Add(this.dtPicker);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.bt_OK);
            this.Controls.Add(this.lbPromt);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GetDate";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Podaj dzień";
            this.Load += new System.EventHandler(this.GetDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtPicker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbPromt;
        private System.Windows.Forms.Button bt_OK;
        private System.Windows.Forms.Button btCancel;
        private Telerik.WinControls.UI.RadDateTimePicker dtPicker;
        private System.Windows.Forms.CheckBox cbLeave;
    }
}