namespace MayChamCongMaster
{
    partial class ChangePassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePassword));
            this.label1 = new System.Windows.Forms.Label();
            this.txtMKCu = new System.Windows.Forms.TextBox();
            this.txtMKMoi = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNLMKMoi = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnXacNhan = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nhập mật khẩu cũ: ";
            // 
            // txtMKCu
            // 
            this.txtMKCu.Location = new System.Drawing.Point(136, 28);
            this.txtMKCu.Name = "txtMKCu";
            this.txtMKCu.PasswordChar = '*';
            this.txtMKCu.Size = new System.Drawing.Size(179, 20);
            this.txtMKCu.TabIndex = 1;
            this.txtMKCu.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMKCu_KeyDown);
            // 
            // txtMKMoi
            // 
            this.txtMKMoi.Location = new System.Drawing.Point(136, 67);
            this.txtMKMoi.Name = "txtMKMoi";
            this.txtMKMoi.PasswordChar = '*';
            this.txtMKMoi.Size = new System.Drawing.Size(179, 20);
            this.txtMKMoi.TabIndex = 3;
            this.txtMKMoi.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMKMoi_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nhập mật khẩu mới: ";
            // 
            // txtNLMKMoi
            // 
            this.txtNLMKMoi.Location = new System.Drawing.Point(136, 103);
            this.txtNLMKMoi.Name = "txtNLMKMoi";
            this.txtNLMKMoi.PasswordChar = '*';
            this.txtNLMKMoi.Size = new System.Drawing.Size(179, 20);
            this.txtNLMKMoi.TabIndex = 5;
            this.txtNLMKMoi.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNLMKMoi_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Nhập lại mật khẩu mới: ";
            // 
            // btnXacNhan
            // 
            this.btnXacNhan.Location = new System.Drawing.Point(109, 150);
            this.btnXacNhan.Name = "btnXacNhan";
            this.btnXacNhan.Size = new System.Drawing.Size(101, 23);
            this.btnXacNhan.TabIndex = 6;
            this.btnXacNhan.Text = "Đổi mật khẩu";
            this.btnXacNhan.UseVisualStyleBackColor = true;
            this.btnXacNhan.Click += new System.EventHandler(this.btnXacNhan_Click);
            // 
            // ChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(327, 190);
            this.Controls.Add(this.btnXacNhan);
            this.Controls.Add(this.txtNLMKMoi);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMKMoi);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMKCu);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChangePassword";
            this.Text = "ChangePassword";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMKCu;
        private System.Windows.Forms.TextBox txtMKMoi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNLMKMoi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnXacNhan;
    }
}