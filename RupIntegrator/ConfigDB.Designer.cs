namespace RupIntegrator
{
    partial class ConfigDB
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
            Telerik.WinControls.UI.RadListDataItem radListDataItem1 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem2 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem3 = new Telerik.WinControls.UI.RadListDataItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigDB));
            this.rbCancel = new Telerik.WinControls.UI.RadButton();
            this.rbOK = new Telerik.WinControls.UI.RadButton();
            this.rddlbDostawca = new Telerik.WinControls.UI.RadDropDownList();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbBazaDanych = new System.Windows.Forms.TextBox();
            this.radGroupBox3 = new Telerik.WinControls.UI.RadGroupBox();
            this.rRBSQL = new Telerik.WinControls.UI.RadRadioButton();
            this.rRBWindows = new Telerik.WinControls.UI.RadRadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbUserId = new System.Windows.Forms.TextBox();
            this.tbPwd = new System.Windows.Forms.TextBox();
            this.rbTestConnection = new Telerik.WinControls.UI.RadButton();
            this.label8 = new System.Windows.Forms.Label();
            this.tbAlias = new System.Windows.Forms.TextBox();
            this.tb_sp = new System.Windows.Forms.TextBox();
            this.radGroupBox2 = new Telerik.WinControls.UI.RadGroupBox();
            this.chbIsActive = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbConsKNSId = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbParam = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbNazwaPolaczenia = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rddlbDostawca)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox3)).BeginInit();
            this.radGroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rRBSQL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rRBWindows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbTestConnection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox2)).BeginInit();
            this.radGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbCancel
            // 
            this.rbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rbCancel.Location = new System.Drawing.Point(420, 9);
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.Size = new System.Drawing.Size(110, 24);
            this.rbCancel.TabIndex = 12;
            this.rbCancel.Text = "Anuluj";
            // 
            // rbOK
            // 
            this.rbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbOK.Location = new System.Drawing.Point(304, 9);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(110, 24);
            this.rbOK.TabIndex = 13;
            this.rbOK.Text = "Zapisz";
            this.rbOK.Click += new System.EventHandler(this.rbOK_Click);
            // 
            // rddlbDostawca
            // 
            this.rddlbDostawca.DropDownAnimationEnabled = true;
            this.rddlbDostawca.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            radListDataItem1.Text = "Currenda sp. z o.o.";
            radListDataItem2.Text = "ZETO Świdnica sp z o.o.";
            radListDataItem3.Text = "Orcom ";
            this.rddlbDostawca.Items.Add(radListDataItem1);
            this.rddlbDostawca.Items.Add(radListDataItem2);
            this.rddlbDostawca.Items.Add(radListDataItem3);
            this.rddlbDostawca.Location = new System.Drawing.Point(224, 56);
            this.rddlbDostawca.Name = "rddlbDostawca";
            // 
            // 
            // 
            this.rddlbDostawca.RootElement.StretchVertically = true;
            this.rddlbDostawca.Size = new System.Drawing.Size(284, 20);
            this.rddlbDostawca.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(134, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Dostawca:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Serwer Bazy Danych:";
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(224, 82);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(284, 20);
            this.tbServer.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(119, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Baza Danych:";
            // 
            // tbBazaDanych
            // 
            this.tbBazaDanych.Location = new System.Drawing.Point(224, 133);
            this.tbBazaDanych.Name = "tbBazaDanych";
            this.tbBazaDanych.Size = new System.Drawing.Size(284, 20);
            this.tbBazaDanych.TabIndex = 5;
            // 
            // radGroupBox3
            // 
            this.radGroupBox3.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox3.Controls.Add(this.rRBSQL);
            this.radGroupBox3.Controls.Add(this.rRBWindows);
            this.radGroupBox3.HeaderText = "autentykacja";
            this.radGroupBox3.Location = new System.Drawing.Point(159, 159);
            this.radGroupBox3.Name = "radGroupBox3";
            this.radGroupBox3.Size = new System.Drawing.Size(349, 36);
            this.radGroupBox3.TabIndex = 7;
            this.radGroupBox3.Text = "autentykacja";
            // 
            // rRBSQL
            // 
            this.rRBSQL.Location = new System.Drawing.Point(180, 14);
            this.rRBSQL.Name = "rRBSQL";
            // 
            // 
            // 
            this.rRBSQL.RootElement.StretchHorizontally = true;
            this.rRBSQL.RootElement.StretchVertically = true;
            this.rRBSQL.Size = new System.Drawing.Size(74, 18);
            this.rRBSQL.TabIndex = 8;
            this.rRBSQL.Text = "SQL Server";
            // 
            // rRBWindows
            // 
            this.rRBWindows.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rRBWindows.Location = new System.Drawing.Point(24, 14);
            this.rRBWindows.Name = "rRBWindows";
            // 
            // 
            // 
            this.rRBWindows.RootElement.StretchHorizontally = true;
            this.rRBWindows.RootElement.StretchVertically = true;
            this.rRBWindows.Size = new System.Drawing.Size(66, 18);
            this.rRBWindows.TabIndex = 7;
            this.rRBWindows.Text = "Windows";
            this.rRBWindows.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            this.rRBWindows.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.rRBWindows_ToggleStateChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(119, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Użytkownik";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(150, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Hasło";
            // 
            // tbUserId
            // 
            this.tbUserId.Location = new System.Drawing.Point(224, 208);
            this.tbUserId.Name = "tbUserId";
            this.tbUserId.Size = new System.Drawing.Size(284, 20);
            this.tbUserId.TabIndex = 9;
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(224, 233);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.PasswordChar = '*';
            this.tbPwd.Size = new System.Drawing.Size(284, 20);
            this.tbPwd.TabIndex = 10;
            this.tbPwd.UseSystemPasswordChar = true;
            // 
            // rbTestConnection
            // 
            this.rbTestConnection.Location = new System.Drawing.Point(214, 265);
            this.rbTestConnection.Name = "rbTestConnection";
            this.rbTestConnection.Size = new System.Drawing.Size(110, 24);
            this.rbTestConnection.TabIndex = 12;
            this.rbTestConnection.Text = "Testuj";
            this.rbTestConnection.Click += new System.EventHandler(this.rbTestConnection_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(119, 115);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Alias serwera:";
            // 
            // tbAlias
            // 
            this.tbAlias.Location = new System.Drawing.Point(224, 108);
            this.tbAlias.Name = "tbAlias";
            this.tbAlias.Size = new System.Drawing.Size(284, 20);
            this.tbAlias.TabIndex = 4;
            // 
            // tb_sp
            // 
            this.tb_sp.Location = new System.Drawing.Point(224, 295);
            this.tb_sp.Name = "tb_sp";
            this.tb_sp.Size = new System.Drawing.Size(284, 20);
            this.tb_sp.TabIndex = 21;
            // 
            // radGroupBox2
            // 
            this.radGroupBox2.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox2.Controls.Add(this.chbIsActive);
            this.radGroupBox2.Controls.Add(this.label11);
            this.radGroupBox2.Controls.Add(this.tbConsKNSId);
            this.radGroupBox2.Controls.Add(this.label10);
            this.radGroupBox2.Controls.Add(this.label9);
            this.radGroupBox2.Controls.Add(this.tbParam);
            this.radGroupBox2.Controls.Add(this.label7);
            this.radGroupBox2.Controls.Add(this.tbNazwaPolaczenia);
            this.radGroupBox2.Controls.Add(this.label1);
            this.radGroupBox2.Controls.Add(this.tb_sp);
            this.radGroupBox2.Controls.Add(this.tbAlias);
            this.radGroupBox2.Controls.Add(this.label8);
            this.radGroupBox2.Controls.Add(this.rbTestConnection);
            this.radGroupBox2.Controls.Add(this.tbPwd);
            this.radGroupBox2.Controls.Add(this.tbUserId);
            this.radGroupBox2.Controls.Add(this.label6);
            this.radGroupBox2.Controls.Add(this.label5);
            this.radGroupBox2.Controls.Add(this.radGroupBox3);
            this.radGroupBox2.Controls.Add(this.tbBazaDanych);
            this.radGroupBox2.Controls.Add(this.label4);
            this.radGroupBox2.Controls.Add(this.tbServer);
            this.radGroupBox2.Controls.Add(this.label3);
            this.radGroupBox2.Controls.Add(this.label2);
            this.radGroupBox2.Controls.Add(this.rddlbDostawca);
            this.radGroupBox2.HeaderText = "Paramatry połączenia";
            this.radGroupBox2.Location = new System.Drawing.Point(-3, 39);
            this.radGroupBox2.Name = "radGroupBox2";
            this.radGroupBox2.Size = new System.Drawing.Size(533, 436);
            this.radGroupBox2.TabIndex = 14;
            this.radGroupBox2.Text = "Paramatry połączenia";
            // 
            // chbIsActive
            // 
            this.chbIsActive.AutoSize = true;
            this.chbIsActive.Checked = true;
            this.chbIsActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIsActive.Location = new System.Drawing.Point(224, 383);
            this.chbIsActive.Name = "chbIsActive";
            this.chbIsActive.Size = new System.Drawing.Size(78, 17);
            this.chbIsActive.TabIndex = 30;
            this.chbIsActive.Text = "Aktywne ?";
            this.chbIsActive.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(59, 383);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(157, 13);
            this.label11.TabIndex = 29;
            this.label11.Text = "Czy połączenie jest aktywne ?";
            // 
            // tbConsKNSId
            // 
            this.tbConsKNSId.Location = new System.Drawing.Point(224, 347);
            this.tbConsKNSId.Name = "tbConsKNSId";
            this.tbConsKNSId.Size = new System.Drawing.Size(284, 20);
            this.tbConsKNSId.TabIndex = 28;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 350);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(199, 13);
            this.label10.TabIndex = 27;
            this.label10.Text = "Identyfikator księgi należności CONS:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 324);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(201, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Parametr przekazywany do procedury:";
            // 
            // tbParam
            // 
            this.tbParam.Location = new System.Drawing.Point(224, 321);
            this.tbParam.Name = "tbParam";
            this.tbParam.Size = new System.Drawing.Size(284, 20);
            this.tbParam.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(49, 302);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(167, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Nazwa procedury składowanej:";
            // 
            // tbNazwaPolaczenia
            // 
            this.tbNazwaPolaczenia.Location = new System.Drawing.Point(224, 30);
            this.tbNazwaPolaczenia.Name = "tbNazwaPolaczenia";
            this.tbNazwaPolaczenia.Size = new System.Drawing.Size(284, 20);
            this.tbNazwaPolaczenia.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Przyjazna nazwa połączenia:";
            // 
            // ConfigDB
            // 
            this.AcceptButton = this.rbOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 487);
            this.Controls.Add(this.radGroupBox2);
            this.Controls.Add(this.rbOK);
            this.Controls.Add(this.rbCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigDB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Definicja połączenia z bazą danych systemu merytorycznego";
            this.Load += new System.EventHandler(this.ConfigDB_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rbCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rddlbDostawca)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox3)).EndInit();
            this.radGroupBox3.ResumeLayout(false);
            this.radGroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rRBSQL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rRBWindows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbTestConnection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox2)).EndInit();
            this.radGroupBox2.ResumeLayout(false);
            this.radGroupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadButton rbCancel;
        private Telerik.WinControls.UI.RadButton rbOK;
        private Telerik.WinControls.UI.RadDropDownList rddlbDostawca;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbBazaDanych;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox3;
        private Telerik.WinControls.UI.RadRadioButton rRBSQL;
        private Telerik.WinControls.UI.RadRadioButton rRBWindows;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbUserId;
        private System.Windows.Forms.TextBox tbPwd;
        private Telerik.WinControls.UI.RadButton rbTestConnection;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbAlias;
        private System.Windows.Forms.TextBox tb_sp;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox2;
        private System.Windows.Forms.TextBox tbNazwaPolaczenia;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbParam;
        private System.Windows.Forms.TextBox tbConsKNSId;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chbIsActive;
        private System.Windows.Forms.Label label11;
    }
}