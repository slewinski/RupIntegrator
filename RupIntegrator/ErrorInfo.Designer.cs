
namespace KnsMigrator
{
    partial class ErrorInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorInfo));
            this.tbDiagnostyka = new System.Windows.Forms.TextBox();
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            this.SuspendLayout();
            // 
            // tbDiagnostyka
            // 
            this.tbDiagnostyka.Location = new System.Drawing.Point(12, 40);
            this.tbDiagnostyka.Multiline = true;
            this.tbDiagnostyka.Name = "tbDiagnostyka";
            this.tbDiagnostyka.Size = new System.Drawing.Size(460, 179);
            this.tbDiagnostyka.TabIndex = 0;
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(392, 11);
            this.rbOK.Margin = new System.Windows.Forms.Padding(2);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(80, 24);
            this.rbOK.TabIndex = 2;
            this.rbOK.Text = "OK";
            // 
            // ErrorInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.rbOK;
            this.ClientSize = new System.Drawing.Size(483, 231);
            this.ControlBox = false;
            this.Controls.Add(this.rbOK);
            this.Controls.Add(this.tbDiagnostyka);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ErrorInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Diagnostyka";
            this.Load += new System.EventHandler(this.ErrorInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDiagnostyka;
        private Telerik.WinControls.UI.RadButton rbOK;
    }
}