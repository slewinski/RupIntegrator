namespace RupBig
{
    partial class SetPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetPassword));
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.rbCancel = new Telerik.WinControls.UI.RadButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.tbPass2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbBig = new System.Windows.Forms.Label();
            this.lbUser = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(134, 12);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(82, 24);
            this.rbOK.TabIndex = 0;
            this.rbOK.Text = "OK";
            // 
            // rbCancel
            // 
            this.rbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbCancel.Location = new System.Drawing.Point(222, 12);
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.Size = new System.Drawing.Size(82, 24);
            this.rbCancel.TabIndex = 1;
            this.rbCancel.Text = "Anuluj";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hasło";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Powtórz";
            // 
            // tbPass
            // 
            this.tbPass.Location = new System.Drawing.Point(116, 109);
            this.tbPass.Name = "tbPass";
            this.tbPass.PasswordChar = '*';
            this.tbPass.Size = new System.Drawing.Size(153, 20);
            this.tbPass.TabIndex = 4;
            // 
            // tbPass2
            // 
            this.tbPass2.Location = new System.Drawing.Point(116, 136);
            this.tbPass2.Name = "tbPass2";
            this.tbPass2.PasswordChar = '*';
            this.tbPass2.Size = new System.Drawing.Size(153, 20);
            this.tbPass2.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(83, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "BIG";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(41, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Użytkownik";
            // 
            // lbBig
            // 
            this.lbBig.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbBig.Location = new System.Drawing.Point(119, 49);
            this.lbBig.Name = "lbBig";
            this.lbBig.Size = new System.Drawing.Size(150, 23);
            this.lbBig.TabIndex = 8;
            // 
            // lbUser
            // 
            this.lbUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbUser.Location = new System.Drawing.Point(119, 71);
            this.lbUser.Name = "lbUser";
            this.lbUser.Size = new System.Drawing.Size(160, 23);
            this.lbUser.TabIndex = 9;
            // 
            // SetPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 187);
            this.Controls.Add(this.lbUser);
            this.Controls.Add(this.lbBig);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbPass2);
            this.Controls.Add(this.tbPass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbCancel);
            this.Controls.Add(this.rbOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ustawienie hasła BIG";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetPassword_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton rbOK;
        private Telerik.WinControls.UI.RadButton rbCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label lbBig;
        public System.Windows.Forms.TextBox tbPass;
        public System.Windows.Forms.Label lbUser;
        public System.Windows.Forms.TextBox tbPass2;
    }
}