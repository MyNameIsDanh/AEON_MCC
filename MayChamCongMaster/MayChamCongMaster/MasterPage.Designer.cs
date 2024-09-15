namespace MayChamCongMaster
{
    partial class MasterPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterPage));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.quảnLýMCCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devicesManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devicesChangedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mallsManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quảnLýFilesLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quảnLýthôngtinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quảnLýMCCToolStripMenuItem,
            this.mallsManagementToolStripMenuItem,
            this.quảnLýFilesLogToolStripMenuItem,
            this.quảnLýthôngtinToolStripMenuItem,
            this.accountToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(643, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // quảnLýMCCToolStripMenuItem
            // 
            this.quảnLýMCCToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.devicesManagementToolStripMenuItem,
            this.devicesChangedToolStripMenuItem});
            this.quảnLýMCCToolStripMenuItem.Name = "quảnLýMCCToolStripMenuItem";
            this.quảnLýMCCToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.quảnLýMCCToolStripMenuItem.Text = "Devices";
            // 
            // devicesManagementToolStripMenuItem
            // 
            this.devicesManagementToolStripMenuItem.Name = "devicesManagementToolStripMenuItem";
            this.devicesManagementToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.devicesManagementToolStripMenuItem.Text = "Devices Management";
            this.devicesManagementToolStripMenuItem.Click += new System.EventHandler(this.devicesManagementToolStripMenuItem_Click);
            // 
            // devicesChangedToolStripMenuItem
            // 
            this.devicesChangedToolStripMenuItem.Name = "devicesChangedToolStripMenuItem";
            this.devicesChangedToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.devicesChangedToolStripMenuItem.Text = "Devices Information";
            this.devicesChangedToolStripMenuItem.Click += new System.EventHandler(this.devicesChangedToolStripMenuItem_Click);
            // 
            // mallsManagementToolStripMenuItem
            // 
            this.mallsManagementToolStripMenuItem.Name = "mallsManagementToolStripMenuItem";
            this.mallsManagementToolStripMenuItem.Size = new System.Drawing.Size(113, 20);
            this.mallsManagementToolStripMenuItem.Text = "Malls Information";
            this.mallsManagementToolStripMenuItem.Click += new System.EventHandler(this.mallsManagementToolStripMenuItem_Click);
            // 
            // quảnLýFilesLogToolStripMenuItem
            // 
            this.quảnLýFilesLogToolStripMenuItem.Name = "quảnLýFilesLogToolStripMenuItem";
            this.quảnLýFilesLogToolStripMenuItem.Size = new System.Drawing.Size(128, 20);
            this.quảnLýFilesLogToolStripMenuItem.Text = "FilesLog Information";
            this.quảnLýFilesLogToolStripMenuItem.Click += new System.EventHandler(this.quảnLýFilesLogToolStripMenuItem_Click);
            // 
            // quảnLýthôngtinToolStripMenuItem
            // 
            this.quảnLýthôngtinToolStripMenuItem.Name = "quảnLýthôngtinToolStripMenuItem";
            this.quảnLýthôngtinToolStripMenuItem.Size = new System.Drawing.Size(151, 20);
            this.quảnLýthôngtinToolStripMenuItem.Text = "Record Push Information";
            this.quảnLýthôngtinToolStripMenuItem.Click += new System.EventHandler(this.quảnLýthôngtinToolStripMenuItem_Click);
            // 
            // accountToolStripMenuItem
            // 
            this.accountToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changePasswordToolStripMenuItem});
            this.accountToolStripMenuItem.Name = "accountToolStripMenuItem";
            this.accountToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.accountToolStripMenuItem.Text = "Account";
            // 
            // changePasswordToolStripMenuItem
            // 
            this.changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            this.changePasswordToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.changePasswordToolStripMenuItem.Text = "Change Password";
            this.changePasswordToolStripMenuItem.Click += new System.EventHandler(this.changePasswordToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(619, 411);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // MasterPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 450);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MasterPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MasterPage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MasterPage_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem quảnLýMCCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quảnLýFilesLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quảnLýthôngtinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devicesChangedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devicesManagementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mallsManagementToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem accountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changePasswordToolStripMenuItem;
    }
}