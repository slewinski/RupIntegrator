namespace RupFinder
{
    partial class RupFinder
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RupFinder));
            this.tbFind = new System.Windows.Forms.TextBox();
            this.btFind = new System.Windows.Forms.Button();
            this.dgVResult = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTextAll = new System.Windows.Forms.TextBox();
            this.cmMarkText = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgVResult)).BeginInit();
            this.SuspendLayout();
            // 
            // tbFind
            // 
            this.tbFind.Location = new System.Drawing.Point(2, 31);
            this.tbFind.Name = "tbFind";
            this.tbFind.Size = new System.Drawing.Size(322, 20);
            this.tbFind.TabIndex = 2;
            // 
            // btFind
            // 
            this.btFind.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btFind.Location = new System.Drawing.Point(330, 29);
            this.btFind.Name = "btFind";
            this.btFind.Size = new System.Drawing.Size(75, 23);
            this.btFind.TabIndex = 6;
            this.btFind.Text = "&Szukaj";
            this.btFind.UseVisualStyleBackColor = true;
            this.btFind.Click += new System.EventHandler(this.btFind_Click);
            // 
            // dgVResult
            // 
            this.dgVResult.AllowUserToAddRows = false;
            this.dgVResult.AllowUserToDeleteRows = false;
            this.dgVResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgVResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgVResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgVResult.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgVResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgVResult.Location = new System.Drawing.Point(0, 76);
            this.dgVResult.Name = "dgVResult";
            this.dgVResult.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgVResult.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgVResult.Size = new System.Drawing.Size(943, 230);
            this.dgVResult.TabIndex = 3;
            this.dgVResult.DoubleClick += new System.EventHandler(this.dgVResult_DoubleClick);
            this.dgVResult.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgVResult_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(313, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Podaj sygnaturę  lub nazwisko dłużnika lub numer karty dłużnika";
            // 
            // tbTextAll
            // 
            this.tbTextAll.ContextMenuStrip = this.cmMarkText;
            this.tbTextAll.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbTextAll.Location = new System.Drawing.Point(423, 1);
            this.tbTextAll.Multiline = true;
            this.tbTextAll.Name = "tbTextAll";
            this.tbTextAll.Size = new System.Drawing.Size(491, 69);
            this.tbTextAll.TabIndex = 1;
            this.tbTextAll.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbTextAll_MouseUp);
            // 
            // cmMarkText
            // 
            this.cmMarkText.Name = "cmMarkText";
            this.cmMarkText.Size = new System.Drawing.Size(153, 26);
          
            // 
            // RupFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 306);
            this.Controls.Add(this.tbTextAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgVResult);
            this.Controls.Add(this.btFind);
            this.Controls.Add(this.tbFind);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RupFinder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RupFinder";
            this.Load += new System.EventHandler(this.RupFinder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgVResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFind;
        private System.Windows.Forms.Button btFind;
        private System.Windows.Forms.DataGridView dgVResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTextAll;
        private System.Windows.Forms.ContextMenuStrip cmMarkText;
      
    }
}

