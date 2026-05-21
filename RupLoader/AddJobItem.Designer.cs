namespace RupLoader
{
    partial class AddJobItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddJobItem));
            this.rddlJobItem = new Telerik.WinControls.UI.RadDropDownList();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.rbCancel = new Telerik.WinControls.UI.RadButton();
            this.tbArgs = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.rddlJobItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // rddlJobItem
            // 
            this.rddlJobItem.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.rddlJobItem.Location = new System.Drawing.Point(149, 56);
            this.rddlJobItem.Name = "rddlJobItem";
            this.rddlJobItem.Size = new System.Drawing.Size(378, 20);
            this.rddlJobItem.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Zadanie";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Argumenty wywołania";
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(335, 12);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(93, 24);
            this.rbOK.TabIndex = 3;
            this.rbOK.Text = "OK";
            // 
            // rbCancel
            // 
            this.rbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbCancel.Location = new System.Drawing.Point(434, 12);
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.Size = new System.Drawing.Size(93, 24);
            this.rbCancel.TabIndex = 4;
            this.rbCancel.Text = "Anuluj";
            // 
            // tbArgs
            // 
            this.tbArgs.Location = new System.Drawing.Point(149, 93);
            this.tbArgs.Name = "tbArgs";
            this.tbArgs.Size = new System.Drawing.Size(378, 20);
            this.tbArgs.TabIndex = 5;
            // 
            // AddJobItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 141);
            this.Controls.Add(this.tbArgs);
            this.Controls.Add(this.rbCancel);
            this.Controls.Add(this.rbOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rddlJobItem);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddJobItem";
            this.Text = "Nowe zadanie";
            ((System.ComponentModel.ISupportInitialize)(this.rddlJobItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Telerik.WinControls.UI.RadDropDownList rddlJobItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.UI.RadButton rbOK;
        private Telerik.WinControls.UI.RadButton rbCancel;
        public System.Windows.Forms.TextBox tbArgs;
    }
}