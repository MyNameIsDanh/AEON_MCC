namespace MayChamCong
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExportStrCardNumber = new System.Windows.Forms.Button();
            this.lvCard = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnGetStrCardNumber = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.chbEnabled = new System.Windows.Forms.CheckBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnSetStrCardNumber = new System.Windows.Forms.Button();
            this.label89 = new System.Windows.Forms.Label();
            this.txtCardnumber = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.label90 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnClearGLog = new System.Windows.Forms.Button();
            this.btnSaveRecordToDB = new System.Windows.Forms.Button();
            this.btnGetGeneralLogData = new System.Windows.Forms.Button();
            this.lvLogs = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvLogsch1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvLogsch2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvLogsch3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvLogsch4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvLogsch5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvLogsch6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvLogsch7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnrefresh = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.gridviewdevices = new System.Windows.Forms.DataGridView();
            this.btnDongBo = new System.Windows.Forms.Button();
            this.lblState = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbbMall = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePickTo = new System.Windows.Forms.DateTimePicker();
            this.DateTimePickFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExportRecordsData = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewdevices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Location = new System.Drawing.Point(412, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(739, 239);
            this.groupBox1.TabIndex = 80;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download or Upload Card Number";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnExportStrCardNumber);
            this.groupBox2.Controls.Add(this.lvCard);
            this.groupBox2.Controls.Add(this.btnGetStrCardNumber);
            this.groupBox2.Location = new System.Drawing.Point(6, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(460, 207);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Download the Card Number(A property of user information)";
            // 
            // btnExportStrCardNumber
            // 
            this.btnExportStrCardNumber.Location = new System.Drawing.Point(229, 178);
            this.btnExportStrCardNumber.Name = "btnExportStrCardNumber";
            this.btnExportStrCardNumber.Size = new System.Drawing.Size(121, 23);
            this.btnExportStrCardNumber.TabIndex = 46;
            this.btnExportStrCardNumber.Text = "ExportStrCardNumber";
            this.btnExportStrCardNumber.UseVisualStyleBackColor = true;
            this.btnExportStrCardNumber.Click += new System.EventHandler(this.btnExportStrCardNumber_Click);
            // 
            // lvCard
            // 
            this.lvCard.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvCard.GridLines = true;
            this.lvCard.HideSelection = false;
            this.lvCard.Location = new System.Drawing.Point(6, 16);
            this.lvCard.Name = "lvCard";
            this.lvCard.Size = new System.Drawing.Size(448, 156);
            this.lvCard.TabIndex = 45;
            this.lvCard.UseCompatibleStateImageBehavior = false;
            this.lvCard.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "MachineNumber";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "UserID";
            this.columnHeader1.Width = 49;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 91;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Cardnumber";
            this.columnHeader3.Width = 105;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Privilege";
            this.columnHeader4.Width = 53;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Password";
            this.columnHeader5.Width = 86;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Enabled";
            this.columnHeader6.Width = 55;
            // 
            // btnGetStrCardNumber
            // 
            this.btnGetStrCardNumber.Location = new System.Drawing.Point(106, 178);
            this.btnGetStrCardNumber.Name = "btnGetStrCardNumber";
            this.btnGetStrCardNumber.Size = new System.Drawing.Size(117, 23);
            this.btnGetStrCardNumber.TabIndex = 1;
            this.btnGetStrCardNumber.Text = "GetStrCardNumber";
            this.btnGetStrCardNumber.UseVisualStyleBackColor = true;
            this.btnGetStrCardNumber.Click += new System.EventHandler(this.btnGetStrCardNumber_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.txtUserID);
            this.groupBox3.Controls.Add(this.txtName);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.chbEnabled);
            this.groupBox3.Controls.Add(this.txtPassword);
            this.groupBox3.Controls.Add(this.btnSetStrCardNumber);
            this.groupBox3.Controls.Add(this.label89);
            this.groupBox3.Controls.Add(this.txtCardnumber);
            this.groupBox3.Controls.Add(this.label55);
            this.groupBox3.Controls.Add(this.label90);
            this.groupBox3.Location = new System.Drawing.Point(472, 23);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(262, 207);
            this.groupBox3.TabIndex = 44;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " ";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(133, 178);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(123, 23);
            this.button2.TabIndex = 70;
            this.button2.Text = "DeleteCardNumber";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.DeleteCardNumber_Click);
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(83, 28);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(173, 20);
            this.txtUserID.TabIndex = 56;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(83, 54);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(173, 20);
            this.txtName.TabIndex = 57;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(6, 31);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 18);
            this.label16.TabIndex = 63;
            this.label16.Text = "User ID";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(7, 58);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(36, 17);
            this.label15.TabIndex = 64;
            this.label15.Text = "Name";
            // 
            // chbEnabled
            // 
            this.chbEnabled.AutoSize = true;
            this.chbEnabled.Location = new System.Drawing.Point(83, 145);
            this.chbEnabled.Name = "chbEnabled";
            this.chbEnabled.Size = new System.Drawing.Size(15, 14);
            this.chbEnabled.TabIndex = 69;
            this.chbEnabled.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(83, 82);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(173, 20);
            this.txtPassword.TabIndex = 58;
            // 
            // btnSetStrCardNumber
            // 
            this.btnSetStrCardNumber.Location = new System.Drawing.Point(10, 178);
            this.btnSetStrCardNumber.Name = "btnSetStrCardNumber";
            this.btnSetStrCardNumber.Size = new System.Drawing.Size(117, 23);
            this.btnSetStrCardNumber.TabIndex = 0;
            this.btnSetStrCardNumber.Text = "SetStrCardNumber";
            this.btnSetStrCardNumber.UseVisualStyleBackColor = true;
            this.btnSetStrCardNumber.Click += new System.EventHandler(this.btnSetStrCardNumber_Click);
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(7, 145);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(52, 13);
            this.label89.TabIndex = 67;
            this.label89.Text = "Enabled  ";
            // 
            // txtCardnumber
            // 
            this.txtCardnumber.Location = new System.Drawing.Point(83, 111);
            this.txtCardnumber.Name = "txtCardnumber";
            this.txtCardnumber.Size = new System.Drawing.Size(173, 20);
            this.txtCardnumber.TabIndex = 61;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(7, 114);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(66, 13);
            this.label55.TabIndex = 66;
            this.label55.Text = "CardNumber";
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(7, 85);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(53, 13);
            this.label90.TabIndex = 68;
            this.label90.Text = "Password";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btnClearGLog);
            this.groupBox7.Controls.Add(this.btnSaveRecordToDB);
            this.groupBox7.Controls.Add(this.btnGetGeneralLogData);
            this.groupBox7.Controls.Add(this.lvLogs);
            this.groupBox7.ForeColor = System.Drawing.Color.DarkBlue;
            this.groupBox7.Location = new System.Drawing.Point(412, 257);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(532, 279);
            this.groupBox7.TabIndex = 81;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Download or Clear Attendance Records";
            // 
            // btnClearGLog
            // 
            this.btnClearGLog.Location = new System.Drawing.Point(245, 244);
            this.btnClearGLog.Name = "btnClearGLog";
            this.btnClearGLog.Size = new System.Drawing.Size(111, 23);
            this.btnClearGLog.TabIndex = 13;
            this.btnClearGLog.Text = "ClearGLog";
            this.btnClearGLog.UseVisualStyleBackColor = true;
            this.btnClearGLog.Click += new System.EventHandler(this.btnClearGLog_Click);
            // 
            // btnSaveRecordToDB
            // 
            this.btnSaveRecordToDB.Location = new System.Drawing.Point(128, 244);
            this.btnSaveRecordToDB.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveRecordToDB.Name = "btnSaveRecordToDB";
            this.btnSaveRecordToDB.Size = new System.Drawing.Size(111, 23);
            this.btnSaveRecordToDB.TabIndex = 12;
            this.btnSaveRecordToDB.Text = "SaveRecordToDB";
            this.btnSaveRecordToDB.UseVisualStyleBackColor = true;
            this.btnSaveRecordToDB.Click += new System.EventHandler(this.btnSaveRecordToDB_Click);
            // 
            // btnGetGeneralLogData
            // 
            this.btnGetGeneralLogData.Location = new System.Drawing.Point(9, 244);
            this.btnGetGeneralLogData.Name = "btnGetGeneralLogData";
            this.btnGetGeneralLogData.Size = new System.Drawing.Size(111, 23);
            this.btnGetGeneralLogData.TabIndex = 1;
            this.btnGetGeneralLogData.Text = "DownloadAttLogs";
            this.btnGetGeneralLogData.UseVisualStyleBackColor = true;
            this.btnGetGeneralLogData.Click += new System.EventHandler(this.btnGetGeneralLogData_Click);
            // 
            // lvLogs
            // 
            this.lvLogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.lvLogsch1,
            this.lvLogsch2,
            this.lvLogsch3,
            this.lvLogsch4,
            this.lvLogsch5,
            this.lvLogsch6,
            this.lvLogsch7,
            this.columnHeader9});
            this.lvLogs.GridLines = true;
            this.lvLogs.HideSelection = false;
            this.lvLogs.Location = new System.Drawing.Point(9, 15);
            this.lvLogs.Name = "lvLogs";
            this.lvLogs.Size = new System.Drawing.Size(517, 223);
            this.lvLogs.TabIndex = 0;
            this.lvLogs.UseCompatibleStateImageBehavior = false;
            this.lvLogs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "MachineNumber";
            // 
            // lvLogsch1
            // 
            this.lvLogsch1.Text = "Count";
            this.lvLogsch1.Width = 45;
            // 
            // lvLogsch2
            // 
            this.lvLogsch2.Text = "EnrollNumber";
            this.lvLogsch2.Width = 81;
            // 
            // lvLogsch3
            // 
            this.lvLogsch3.Text = "VerifyMode";
            this.lvLogsch3.Width = 67;
            // 
            // lvLogsch4
            // 
            this.lvLogsch4.Text = "InOutMode";
            this.lvLogsch4.Width = 67;
            // 
            // lvLogsch5
            // 
            this.lvLogsch5.Text = "Date";
            this.lvLogsch5.Width = 98;
            // 
            // lvLogsch6
            // 
            this.lvLogsch6.Text = "WorkCode";
            this.lvLogsch6.Width = 66;
            // 
            // lvLogsch7
            // 
            this.lvLogsch7.Text = "Reserved";
            this.lvLogsch7.Width = 59;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "UserCardNo";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 300000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnrefresh);
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.gridviewdevices);
            this.groupBox4.Controls.Add(this.btnDongBo);
            this.groupBox4.Controls.Add(this.lblState);
            this.groupBox4.Controls.Add(this.btnConnect);
            this.groupBox4.Location = new System.Drawing.Point(12, 199);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(394, 337);
            this.groupBox4.TabIndex = 82;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Devices";
            // 
            // btnrefresh
            // 
            this.btnrefresh.Location = new System.Drawing.Point(87, 277);
            this.btnrefresh.Name = "btnrefresh";
            this.btnrefresh.Size = new System.Drawing.Size(89, 23);
            this.btnrefresh.TabIndex = 17;
            this.btnrefresh.Text = "refresh";
            this.btnrefresh.UseVisualStyleBackColor = true;
            this.btnrefresh.Click += new System.EventHandler(this.btnrefresh_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(182, 277);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "SyncTime";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gridviewdevices
            // 
            this.gridviewdevices.AllowUserToAddRows = false;
            this.gridviewdevices.AllowUserToDeleteRows = false;
            this.gridviewdevices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridviewdevices.Location = new System.Drawing.Point(6, 19);
            this.gridviewdevices.Name = "gridviewdevices";
            this.gridviewdevices.ReadOnly = true;
            this.gridviewdevices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridviewdevices.Size = new System.Drawing.Size(382, 249);
            this.gridviewdevices.TabIndex = 15;
            // 
            // btnDongBo
            // 
            this.btnDongBo.Location = new System.Drawing.Point(299, 277);
            this.btnDongBo.Name = "btnDongBo";
            this.btnDongBo.Size = new System.Drawing.Size(89, 23);
            this.btnDongBo.TabIndex = 14;
            this.btnDongBo.Text = "Turn on sync";
            this.btnDongBo.UseVisualStyleBackColor = true;
            this.btnDongBo.Click += new System.EventHandler(this.btnDongBo_Click);
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.ForeColor = System.Drawing.Color.Crimson;
            this.lblState.Location = new System.Drawing.Point(114, 312);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(138, 13);
            this.lblState.TabIndex = 13;
            this.lblState.Text = "Current State:Disconnected";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(6, 277);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 12;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(394, 181);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 83;
            this.pictureBox1.TabStop = false;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 180000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbbMall);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.dateTimePickTo);
            this.groupBox5.Controls.Add(this.DateTimePickFrom);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.btnExportRecordsData);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Location = new System.Drawing.Point(950, 257);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(201, 279);
            this.groupBox5.TabIndex = 84;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Export Data";
            // 
            // cbbMall
            // 
            this.cbbMall.FormattingEnabled = true;
            this.cbbMall.Location = new System.Drawing.Point(78, 72);
            this.cbbMall.Name = "cbbMall";
            this.cbbMall.Size = new System.Drawing.Size(112, 21);
            this.cbbMall.TabIndex = 85;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(14, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 18);
            this.label3.TabIndex = 84;
            this.label3.Text = "Mall: ";
            // 
            // dateTimePickTo
            // 
            this.dateTimePickTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickTo.Location = new System.Drawing.Point(77, 45);
            this.dateTimePickTo.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dateTimePickTo.Name = "dateTimePickTo";
            this.dateTimePickTo.Size = new System.Drawing.Size(113, 20);
            this.dateTimePickTo.TabIndex = 83;
            // 
            // DateTimePickFrom
            // 
            this.DateTimePickFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DateTimePickFrom.Location = new System.Drawing.Point(77, 19);
            this.DateTimePickFrom.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.DateTimePickFrom.Name = "DateTimePickFrom";
            this.DateTimePickFrom.Size = new System.Drawing.Size(113, 20);
            this.DateTimePickFrom.TabIndex = 82;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(14, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 81;
            this.label2.Text = "DateTo: ";
            // 
            // btnExportRecordsData
            // 
            this.btnExportRecordsData.Location = new System.Drawing.Point(79, 108);
            this.btnExportRecordsData.Name = "btnExportRecordsData";
            this.btnExportRecordsData.Size = new System.Drawing.Size(111, 23);
            this.btnExportRecordsData.TabIndex = 79;
            this.btnExportRecordsData.Text = "ExportRecordData";
            this.btnExportRecordsData.UseVisualStyleBackColor = true;
            this.btnExportRecordsData.Click += new System.EventHandler(this.btnExportRecordsData_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(14, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 18);
            this.label1.TabIndex = 80;
            this.label1.Text = "DateFrom: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 548);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewdevices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView lvCard;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button btnGetStrCardNumber;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox chbEnabled;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnSetStrCardNumber;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.TextBox txtCardnumber;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btnGetGeneralLogData;
        private System.Windows.Forms.ListView lvLogs;
        private System.Windows.Forms.ColumnHeader lvLogsch1;
        private System.Windows.Forms.ColumnHeader lvLogsch2;
        private System.Windows.Forms.ColumnHeader lvLogsch3;
        private System.Windows.Forms.ColumnHeader lvLogsch4;
        private System.Windows.Forms.ColumnHeader lvLogsch5;
        private System.Windows.Forms.ColumnHeader lvLogsch6;
        private System.Windows.Forms.ColumnHeader lvLogsch7;
        private System.Windows.Forms.Button btnSaveRecordToDB;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView gridviewdevices;
        private System.Windows.Forms.Button btnDongBo;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnClearGLog;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button btnrefresh;
        private System.Windows.Forms.Button btnExportStrCardNumber;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cbbMall;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePickTo;
        private System.Windows.Forms.DateTimePicker DateTimePickFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExportRecordsData;
        private System.Windows.Forms.Label label1;
    }
}

