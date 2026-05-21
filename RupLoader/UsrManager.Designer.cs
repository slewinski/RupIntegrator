
namespace RupLoader
{
    partial class UsrManager
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
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn1 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn2 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn2 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn3 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsrManager));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.rbDeleteAcc = new Telerik.WinControls.UI.RadButton();
            this.rbManage = new Telerik.WinControls.UI.RadButton();
            this.rbAddAccount = new Telerik.WinControls.UI.RadButton();
            this.rgvUsers = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbDeleteAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbManage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbAddAccount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvUsers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvUsers.MasterTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.rbDeleteAcc);
            this.splitContainer2.Panel1.Controls.Add(this.rbManage);
            this.splitContainer2.Panel1.Controls.Add(this.rbAddAccount);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.rgvUsers);
            this.splitContainer2.Size = new System.Drawing.Size(623, 615);
            this.splitContainer2.SplitterDistance = 45;
            this.splitContainer2.TabIndex = 2;
            // 
            // rbDeleteAcc
            // 
            this.rbDeleteAcc.Location = new System.Drawing.Point(182, 8);
            this.rbDeleteAcc.Name = "rbDeleteAcc";
            this.rbDeleteAcc.Size = new System.Drawing.Size(78, 28);
            this.rbDeleteAcc.TabIndex = 2;
            this.rbDeleteAcc.Text = "Usuń";
            this.rbDeleteAcc.Click += new System.EventHandler(this.rbDeleteAcc_Click);
            // 
            // rbManage
            // 
            this.rbManage.Location = new System.Drawing.Point(97, 9);
            this.rbManage.Name = "rbManage";
            this.rbManage.Size = new System.Drawing.Size(78, 27);
            this.rbManage.TabIndex = 1;
            this.rbManage.Text = "Zarządzaj";
            this.rbManage.Click += new System.EventHandler(this.rbManage_Click);
            // 
            // rbAddAccount
            // 
            this.rbAddAccount.Location = new System.Drawing.Point(3, 9);
            this.rbAddAccount.Name = "rbAddAccount";
            this.rbAddAccount.Size = new System.Drawing.Size(88, 27);
            this.rbAddAccount.TabIndex = 0;
            this.rbAddAccount.Text = "Dodaj";
            this.rbAddAccount.Click += new System.EventHandler(this.rbAddAccount_Click);
            // 
            // rgvUsers
            // 
            this.rgvUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rgvUsers.Location = new System.Drawing.Point(0, 0);
            // 
            // 
            // 
            this.rgvUsers.MasterTemplate.AllowAddNewRow = false;
            this.rgvUsers.MasterTemplate.AutoGenerateColumns = false;
            gridViewDecimalColumn1.DataType = typeof(int);
            gridViewDecimalColumn1.FieldName = "Id";
            gridViewDecimalColumn1.HeaderText = "Id";
            gridViewDecimalColumn1.IsAutoGenerated = true;
            gridViewDecimalColumn1.IsVisible = false;
            gridViewDecimalColumn1.Name = "Id";
            gridViewTextBoxColumn1.FieldName = "Username";
            gridViewTextBoxColumn1.HeaderText = "Login";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.Name = "Username";
            gridViewTextBoxColumn1.Width = 200;
            gridViewDecimalColumn2.DataType = typeof(int);
            gridViewDecimalColumn2.FieldName = "role";
            gridViewDecimalColumn2.HeaderText = "role";
            gridViewDecimalColumn2.IsAutoGenerated = true;
            gridViewDecimalColumn2.IsVisible = false;
            gridViewDecimalColumn2.Name = "role";
            gridViewTextBoxColumn2.Expression = "IIF(role =  1, \'Administrator\', IIF(role=3, \'System\' ,\'Użytkownik\' ) )";
            gridViewTextBoxColumn2.HeaderText = "Rola";
            gridViewTextBoxColumn2.Name = "column2";
            gridViewTextBoxColumn2.Width = 140;
            gridViewCheckBoxColumn1.DataType = typeof(System.Nullable<bool>);
            gridViewCheckBoxColumn1.FieldName = "suspend";
            gridViewCheckBoxColumn1.HeaderText = "Zawieszone";
            gridViewCheckBoxColumn1.IsAutoGenerated = true;
            gridViewCheckBoxColumn1.Name = "suspend";
            gridViewCheckBoxColumn1.Width = 90;
            gridViewCheckBoxColumn2.DataType = typeof(System.Nullable<bool>);
            gridViewCheckBoxColumn2.FieldName = "ChangePwd";
            gridViewCheckBoxColumn2.HeaderText = "Zm hasła";
            gridViewCheckBoxColumn2.IsAutoGenerated = true;
            gridViewCheckBoxColumn2.Name = "ChangePwd";
            gridViewCheckBoxColumn2.Width = 90;
            gridViewCheckBoxColumn3.FieldName = "deleted";
            gridViewCheckBoxColumn3.HeaderText = "Usunięte";
            gridViewCheckBoxColumn3.Name = "deleted";
            this.rgvUsers.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewDecimalColumn1,
            gridViewTextBoxColumn1,
            gridViewDecimalColumn2,
            gridViewTextBoxColumn2,
            gridViewCheckBoxColumn1,
            gridViewCheckBoxColumn2,
            gridViewCheckBoxColumn3});
            this.rgvUsers.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.rgvUsers.Name = "rgvUsers";
            this.rgvUsers.ReadOnly = true;
            this.rgvUsers.Size = new System.Drawing.Size(623, 566);
            this.rgvUsers.TabIndex = 0;
            // 
            // UsrManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 615);
            this.Controls.Add(this.splitContainer2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UsrManager";
            this.Text = "Zarządzanie użytkownikami";
            this.Load += new System.EventHandler(this.rgvUsers_Initialized);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rbDeleteAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbManage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbAddAccount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvUsers.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvUsers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private Telerik.WinControls.UI.RadButton rbDeleteAcc;
        private Telerik.WinControls.UI.RadButton rbManage;
        private Telerik.WinControls.UI.RadButton rbAddAccount;
        private Telerik.WinControls.UI.RadGridView rgvUsers;
    }
}