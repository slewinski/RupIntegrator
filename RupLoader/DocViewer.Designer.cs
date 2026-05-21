namespace RupLoader
{
    partial class DocViewer
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
            this.rtStandard = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtStandard
            // 
            this.rtStandard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtStandard.Location = new System.Drawing.Point(0, 0);
            this.rtStandard.Name = "rtStandard";
            this.rtStandard.Size = new System.Drawing.Size(936, 458);
            this.rtStandard.TabIndex = 1;
            this.rtStandard.Text = "";
            
            // 
            // DocViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 458);
            this.Controls.Add(this.rtStandard);
            this.Name = "DocViewer";
            this.Text = "DocViewer";
            this.Load += new System.EventHandler(this.DocViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtStandard;
    }
}