namespace MayChamCongMaster
{
    partial class Logfile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Logfile));
            this.btnUpdateLogFile = new System.Windows.Forms.Button();
            this.txtboxFileName = new System.Windows.Forms.TextBox();
            this.btnChonFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbTTLogFile = new System.Windows.Forms.ComboBox();
            this.gridviewlogfile = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewlogfile)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUpdateLogFile
            // 
            this.btnUpdateLogFile.Location = new System.Drawing.Point(347, 368);
            this.btnUpdateLogFile.Name = "btnUpdateLogFile";
            this.btnUpdateLogFile.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateLogFile.TabIndex = 2;
            this.btnUpdateLogFile.Text = "Update";
            this.btnUpdateLogFile.UseVisualStyleBackColor = true;
            this.btnUpdateLogFile.Click += new System.EventHandler(this.btnUpdateLogFile_Click);
            // 
            // txtboxFileName
            // 
            this.txtboxFileName.Location = new System.Drawing.Point(93, 370);
            this.txtboxFileName.Name = "txtboxFileName";
            this.txtboxFileName.Size = new System.Drawing.Size(248, 20);
            this.txtboxFileName.TabIndex = 1;
            // 
            // btnChonFile
            // 
            this.btnChonFile.Location = new System.Drawing.Point(12, 368);
            this.btnChonFile.Name = "btnChonFile";
            this.btnChonFile.Size = new System.Drawing.Size(75, 23);
            this.btnChonFile.TabIndex = 0;
            this.btnChonFile.Text = "Chọn file log";
            this.btnChonFile.UseVisualStyleBackColor = true;
            this.btnChonFile.Click += new System.EventHandler(this.btnChonFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Chọn trạng thái: ";
            // 
            // cbbTTLogFile
            // 
            this.cbbTTLogFile.FormattingEnabled = true;
            this.cbbTTLogFile.Items.AddRange(new object[] {
            "Tất cả",
            "Checked",
            "Updated"});
            this.cbbTTLogFile.Location = new System.Drawing.Point(119, 23);
            this.cbbTTLogFile.Name = "cbbTTLogFile";
            this.cbbTTLogFile.Size = new System.Drawing.Size(121, 21);
            this.cbbTTLogFile.TabIndex = 4;
            this.cbbTTLogFile.Text = "Tất cả";
            this.cbbTTLogFile.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // gridviewlogfile
            // 
            this.gridviewlogfile.AllowUserToAddRows = false;
            this.gridviewlogfile.AllowUserToDeleteRows = false;
            this.gridviewlogfile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridviewlogfile.Location = new System.Drawing.Point(12, 50);
            this.gridviewlogfile.Name = "gridviewlogfile";
            this.gridviewlogfile.ReadOnly = true;
            this.gridviewlogfile.Size = new System.Drawing.Size(410, 312);
            this.gridviewlogfile.TabIndex = 5;
            // 
            // Logfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 397);
            this.Controls.Add(this.gridviewlogfile);
            this.Controls.Add(this.cbbTTLogFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnUpdateLogFile);
            this.Controls.Add(this.txtboxFileName);
            this.Controls.Add(this.btnChonFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Logfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XEM THÔNG TIN CHI TIẾT FILE LOG";
            ((System.ComponentModel.ISupportInitialize)(this.gridviewlogfile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnUpdateLogFile;
        private System.Windows.Forms.TextBox txtboxFileName;
        private System.Windows.Forms.Button btnChonFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbTTLogFile;
        private System.Windows.Forms.DataGridView gridviewlogfile;
    }
}