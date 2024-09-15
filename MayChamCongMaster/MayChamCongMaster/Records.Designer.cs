namespace MayChamCongMaster
{
    partial class Records
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Records));
            this.cbbTTRecords = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gridviewRecords = new System.Windows.Forms.DataGridView();
            this.checkbox1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnExec = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnBoSelectAll = new System.Windows.Forms.Button();
            this.txtTim = new System.Windows.Forms.TextBox();
            this.btnTim = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewRecords)).BeginInit();
            this.SuspendLayout();
            // 
            // cbbTTRecords
            // 
            this.cbbTTRecords.FormattingEnabled = true;
            this.cbbTTRecords.Items.AddRange(new object[] {
            "Tất cả",
            "Error"});
            this.cbbTTRecords.Location = new System.Drawing.Point(141, 15);
            this.cbbTTRecords.Name = "cbbTTRecords";
            this.cbbTTRecords.Size = new System.Drawing.Size(121, 21);
            this.cbbTTRecords.TabIndex = 1;
            this.cbbTTRecords.Text = "Error";
            this.cbbTTRecords.SelectedIndexChanged += new System.EventHandler(this.cbbTTRecords_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Chọn trạng thái records: ";
            // 
            // gridviewRecords
            // 
            this.gridviewRecords.AllowUserToAddRows = false;
            this.gridviewRecords.AllowUserToDeleteRows = false;
            this.gridviewRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridviewRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridviewRecords.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checkbox1});
            this.gridviewRecords.Location = new System.Drawing.Point(12, 42);
            this.gridviewRecords.Name = "gridviewRecords";
            this.gridviewRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridviewRecords.Size = new System.Drawing.Size(828, 397);
            this.gridviewRecords.TabIndex = 3;
            this.gridviewRecords.SelectionChanged += new System.EventHandler(this.gridviewRecords_SelectionChanged);
            // 
            // checkbox1
            // 
            this.checkbox1.HeaderText = "";
            this.checkbox1.Name = "checkbox1";
            this.checkbox1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.checkbox1.Width = 5;
            // 
            // btnExec
            // 
            this.btnExec.Location = new System.Drawing.Point(765, 445);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(75, 23);
            this.btnExec.TabIndex = 4;
            this.btnExec.Text = "Exec";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(12, 445);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 5;
            this.btnSelectAll.Text = "Chọn tất cả";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnBoSelectAll
            // 
            this.btnBoSelectAll.Location = new System.Drawing.Point(93, 445);
            this.btnBoSelectAll.Name = "btnBoSelectAll";
            this.btnBoSelectAll.Size = new System.Drawing.Size(91, 23);
            this.btnBoSelectAll.TabIndex = 6;
            this.btnBoSelectAll.Text = "Bỏ chọn tất cả";
            this.btnBoSelectAll.UseVisualStyleBackColor = true;
            this.btnBoSelectAll.Click += new System.EventHandler(this.btnBoSelectAll_Click);
            // 
            // txtTim
            // 
            this.txtTim.Location = new System.Drawing.Point(602, 15);
            this.txtTim.Name = "txtTim";
            this.txtTim.Size = new System.Drawing.Size(157, 20);
            this.txtTim.TabIndex = 7;
            // 
            // btnTim
            // 
            this.btnTim.Location = new System.Drawing.Point(765, 13);
            this.btnTim.Name = "btnTim";
            this.btnTim.Size = new System.Drawing.Size(75, 23);
            this.btnTim.TabIndex = 8;
            this.btnTim.Text = "Tìm";
            this.btnTim.UseVisualStyleBackColor = true;
            this.btnTim.Click += new System.EventHandler(this.btnTim_Click);
            // 
            // Records
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 482);
            this.Controls.Add(this.btnTim);
            this.Controls.Add(this.txtTim);
            this.Controls.Add(this.gridviewRecords);
            this.Controls.Add(this.btnBoSelectAll);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbbTTRecords);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Records";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "THÔNG TIN CHI TIẾT RECORDS";
            this.Load += new System.EventHandler(this.Records_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridviewRecords)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbbTTRecords;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView gridviewRecords;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkbox1;
        private System.Windows.Forms.Button btnBoSelectAll;
        private System.Windows.Forms.TextBox txtTim;
        private System.Windows.Forms.Button btnTim;
    }
}