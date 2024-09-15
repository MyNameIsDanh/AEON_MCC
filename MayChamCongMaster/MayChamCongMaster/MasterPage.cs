using MayChamCong;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MayChamCongMaster
{
    
    public partial class MasterPage : Form
    {

        Form1 f1 = new Form1();
        Logfile f2 = new Logfile();
        Records f3 = new Records();
        Devices f4 = new Devices();
        Malls f5 = new Malls();
        ChangePassword f6 = new ChangePassword();
        public MasterPage()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;

        }

        private void quảnLýFilesLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f2 = new Logfile();
            f2.Show();
        }

        private void quảnLýthôngtinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f3 = new Records();
            f3.Show();
        }

        private void devicesManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f1 = new Form1();
            f1.Show();
        }

        private void MasterPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            DialogResult TL;
            TL = MessageBox.Show("Bạn Có Muốn Thoát Không?", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (TL == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                //đóng tất cả form con
                f1.Close();
                f2.Close();
                f3.Close();
                f4.Close();
                f5.Close();
                f6.Close();
                //mở form login
                Login f = new Login();
                f.Show();
            }
        }

        private void devicesChangedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f4 = new Devices();
            f4.Show();
        }

        private void mallsManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f5 = new Malls();
            f5.Show();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f6 = new ChangePassword();
            f6.Show();
        }

    }
}
