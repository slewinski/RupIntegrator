namespace RupLoader
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
            Telerik.WinControls.UI.RadListDataItem radListDataItem4 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem5 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem6 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem7 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem8 = new Telerik.WinControls.UI.RadListDataItem();
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
            this.radGroupBox2 = new Telerik.WinControls.UI.RadGroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbSygnTech = new System.Windows.Forms.TextBox();
            this.tbIdZespolu = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tb_sp = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.rddbtypDb = new Telerik.WinControls.UI.RadDropDownList();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbWydzialy = new System.Windows.Forms.TextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.rddbtypDb)).BeginInit();
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
            this.rddlbDostawca.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            radListDataItem1.Text = "Currenda sp. z o.o.";
            radListDataItem2.Text = "ZETO Świdnica sp z o.o.";
            radListDataItem3.Text = "Orcom ";
            radListDataItem4.Text = "Albit sp z o.o.";
            this.rddlbDostawca.Items.Add(radListDataItem1);
            this.rddlbDostawca.Items.Add(radListDataItem2);
            this.rddlbDostawca.Items.Add(radListDataItem3);
            this.rddlbDostawca.Items.Add(radListDataItem4);
            this.rddlbDostawca.Location = new System.Drawing.Point(155, 44);
            this.rddlbDostawca.Name = "rddlbDostawca";
            // 
            // 
            // 
            this.rddlbDostawca.RootElement.StretchVertically = true;
            this.rddlbDostawca.Size = new System.Drawing.Size(217, 20);
            this.rddlbDostawca.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Dostawca:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Serwer Bazy Danych:";
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(155, 70);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(212, 20);
            this.tbServer.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(50, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Baza Danych:";
            // 
            // tbBazaDanych
            // 
            this.tbBazaDanych.Location = new System.Drawing.Point(155, 121);
            this.tbBazaDanych.Name = "tbBazaDanych";
            this.tbBazaDanych.Size = new System.Drawing.Size(212, 20);
            this.tbBazaDanych.TabIndex = 5;
            // 
            // radGroupBox3
            // 
            this.radGroupBox3.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox3.Controls.Add(this.rRBSQL);
            this.radGroupBox3.Controls.Add(this.rRBWindows);
            this.radGroupBox3.HeaderText = "autentykacja";
            this.radGroupBox3.Location = new System.Drawing.Point(18, 146);
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
            this.label5.Location = new System.Drawing.Point(15, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Użytkownik";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 227);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Hasło";
            // 
            // tbUserId
            // 
            this.tbUserId.Location = new System.Drawing.Point(155, 195);
            this.tbUserId.Name = "tbUserId";
            this.tbUserId.Size = new System.Drawing.Size(212, 20);
            this.tbUserId.TabIndex = 9;
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(155, 220);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.PasswordChar = '*';
            this.tbPwd.Size = new System.Drawing.Size(212, 20);
            this.tbPwd.TabIndex = 10;
            this.tbPwd.UseSystemPasswordChar = true;
            // 
            // rbTestConnection
            // 
            this.rbTestConnection.Location = new System.Drawing.Point(145, 252);
            this.rbTestConnection.Name = "rbTestConnection";
            this.rbTestConnection.Size = new System.Drawing.Size(110, 24);
            this.rbTestConnection.TabIndex = 12;
            this.rbTestConnection.Text = "Testuj";
            this.rbTestConnection.Click += new System.EventHandler(this.rbTestConnection_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(50, 103);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Alias serwera:";
            // 
            // tbAlias
            // 
            this.tbAlias.Location = new System.Drawing.Point(155, 96);
            this.tbAlias.Name = "tbAlias";
            this.tbAlias.Size = new System.Drawing.Size(212, 20);
            this.tbAlias.TabIndex = 4;
            // 
            // radGroupBox2
            // 
            this.radGroupBox2.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox2.Controls.Add(this.label13);
            this.radGroupBox2.Controls.Add(this.tbSygnTech);
            this.radGroupBox2.Controls.Add(this.tbIdZespolu);
            this.radGroupBox2.Controls.Add(this.label11);
            this.radGroupBox2.Controls.Add(this.tb_sp);
            this.radGroupBox2.Controls.Add(this.label10);
            this.radGroupBox2.Controls.Add(this.label9);
            this.radGroupBox2.Controls.Add(this.rddbtypDb);
            this.radGroupBox2.Controls.Add(this.label7);
            this.radGroupBox2.Controls.Add(this.label1);
            this.radGroupBox2.Controls.Add(this.tbWydzialy);
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
            this.radGroupBox2.Location = new System.Drawing.Point(49, 39);
            this.radGroupBox2.Name = "radGroupBox2";
            this.radGroupBox2.Size = new System.Drawing.Size(453, 436);
            this.radGroupBox2.TabIndex = 14;
            this.radGroupBox2.Text = "Paramatry połączenia";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(28, 404);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(120, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Sygnatura  techniczna";
            // 
            // tbSygnTech
            // 
            this.tbSygnTech.Location = new System.Drawing.Point(158, 401);
            this.tbSygnTech.Name = "tbSygnTech";
            this.tbSygnTech.Size = new System.Drawing.Size(207, 20);
            this.tbSygnTech.TabIndex = 24;
            // 
            // tbIdZespolu
            // 
            this.tbIdZespolu.Location = new System.Drawing.Point(158, 370);
            this.tbIdZespolu.Name = "tbIdZespolu";
            this.tbIdZespolu.Size = new System.Drawing.Size(207, 20);
            this.tbIdZespolu.TabIndex = 23;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 371);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(135, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Id zespołu kuratorskiego";
            // 
            // tb_sp
            // 
            this.tb_sp.Location = new System.Drawing.Point(158, 331);
            this.tb_sp.Name = "tb_sp";
            this.tb_sp.Size = new System.Drawing.Size(207, 20);
            this.tb_sp.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 334);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(124, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Procedura składowana";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label9.Location = new System.Drawing.Point(5, 304);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(197, 12);
            this.label9.TabIndex = 19;
            this.label9.Text = "(Podać listę oznaczeń wydziałów oddzileonych \";\"";
            // 
            // rddbtypDb
            // 
            radListDataItem5.Text = "Wydział Orzeczniczy";
            radListDataItem6.Text = "KNS ( Wykonawstwo)";
            radListDataItem7.Text = "Kuratorzy dla dorosłych";
            radListDataItem8.Text = "Kuratorzy w spraw rodzinnych i nieletnich";
            this.rddbtypDb.Items.Add(radListDataItem5);
            this.rddbtypDb.Items.Add(radListDataItem6);
            this.rddbtypDb.Items.Add(radListDataItem7);
            this.rddbtypDb.Items.Add(radListDataItem8);
            this.rddbtypDb.Location = new System.Drawing.Point(155, 17);
            this.rddbtypDb.Name = "rddbtypDb";
            this.rddbtypDb.Size = new System.Drawing.Size(217, 20);
            this.rddbtypDb.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(65, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Typ systemu";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 285);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Wydziały obsługiwane";
            // 
            // tbWydzialy
            // 
            this.tbWydzialy.Location = new System.Drawing.Point(158, 282);
            this.tbWydzialy.Name = "tbWydzialy";
            this.tbWydzialy.Size = new System.Drawing.Size(207, 20);
            this.tbWydzialy.TabIndex = 13;
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
            this.Load += new System.EventHandler(this.UserAccount_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.rddbtypDb)).EndInit();
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
        private Telerik.WinControls.UI.RadGroupBox radGroupBox2;
        private Telerik.WinControls.UI.RadDropDownList rddbtypDb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbWydzialy;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tb_sp;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbIdZespolu;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbSygnTech;
    }
}