namespace KnsMigrator
{
    partial class ProgresForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgresForm));
            this.lbInfo = new System.Windows.Forms.Label();
            this.rbStop = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.rbStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // lbInfo
            // 
            this.lbInfo.BackColor = System.Drawing.Color.OldLace;
            this.lbInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbInfo.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbInfo.ForeColor = System.Drawing.Color.Blue;
            this.lbInfo.Location = new System.Drawing.Point(48, 36);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(285, 23);
            this.lbInfo.TabIndex = 3;
            this.lbInfo.Text = "Przetwarzanie w toku proszę czekać..";
            // 
            // rbStop
            // 
            this.rbStop.Image = ((System.Drawing.Image)(resources.GetObject("rbStop.Image")));
            this.rbStop.Location = new System.Drawing.Point(339, 20);
            this.rbStop.Name = "rbStop";
            this.rbStop.Size = new System.Drawing.Size(35, 39);
            this.rbStop.TabIndex = 2;
            // 
            // ProgresForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 95);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.rbStop);
            this.Name = "ProgresForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "ProgresForm";
            this.ThemeName = "ControlDefault";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.rbStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbInfo;
        private Telerik.WinControls.UI.RadButton rbStop;
    }
}
