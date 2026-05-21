namespace KnsMigrator
{
    partial class WhoWnd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WhoWnd));
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbprzekaz1 = new System.Windows.Forms.Label();
            this.lbprzekaz2 = new System.Windows.Forms.Label();
            this.lbprzekaz3 = new System.Windows.Forms.Label();
            this.lbimport = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            this.SuspendLayout();
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(493, 12);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(107, 30);
            this.rbOK.TabIndex = 0;
            this.rbOK.Text = "OK";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Importował:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Przekazał do ZSRK:";
            // 
            // lbprzekaz1
            // 
            this.lbprzekaz1.AutoSize = true;
            this.lbprzekaz1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbprzekaz1.Location = new System.Drawing.Point(154, 145);
            this.lbprzekaz1.Name = "lbprzekaz1";
            this.lbprzekaz1.Size = new System.Drawing.Size(440, 18);
            this.lbprzekaz1.TabIndex = 3;
            this.lbprzekaz1.Text = "                                                                                 " +
    "                           ";
            // 
            // lbprzekaz2
            // 
            this.lbprzekaz2.AutoSize = true;
            this.lbprzekaz2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbprzekaz2.Location = new System.Drawing.Point(154, 176);
            this.lbprzekaz2.Name = "lbprzekaz2";
            this.lbprzekaz2.Size = new System.Drawing.Size(440, 18);
            this.lbprzekaz2.TabIndex = 4;
            this.lbprzekaz2.Text = "                                                                                 " +
    "                           ";
            // 
            // lbprzekaz3
            // 
            this.lbprzekaz3.AutoSize = true;
            this.lbprzekaz3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbprzekaz3.Location = new System.Drawing.Point(154, 208);
            this.lbprzekaz3.Name = "lbprzekaz3";
            this.lbprzekaz3.Size = new System.Drawing.Size(440, 18);
            this.lbprzekaz3.TabIndex = 5;
            this.lbprzekaz3.Text = "                                                                                 " +
    "                           ";
            // 
            // lbimport
            // 
            this.lbimport.AutoSize = true;
            this.lbimport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbimport.Location = new System.Drawing.Point(158, 75);
            this.lbimport.Name = "lbimport";
            this.lbimport.Size = new System.Drawing.Size(440, 18);
            this.lbimport.TabIndex = 6;
            this.lbimport.Text = "                                                                                 " +
    "                           ";
            // 
            // WhoWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 289);
            this.Controls.Add(this.lbimport);
            this.Controls.Add(this.lbprzekaz3);
            this.Controls.Add(this.lbprzekaz2);
            this.Controls.Add(this.lbprzekaz1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WhoWnd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Autor operacji";
            this.Load += new System.EventHandler(this.WhoWnd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton rbOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbprzekaz1;
        private System.Windows.Forms.Label lbprzekaz2;
        private System.Windows.Forms.Label lbprzekaz3;
        private System.Windows.Forms.Label lbimport;
    }
}