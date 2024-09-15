using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace MayChamCongMaster
{

    public partial class ChangePassword : Form
    {
        private string strConnect = ConfigurationManager.ConnectionStrings["ConnectDBSQL"].ConnectionString;
        public ChangePassword()
        {
            InitializeComponent();
            
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            
            if (txtMKCu.Text == "")
            {
                MessageBox.Show("Nhập mật khẩu cũ!", "Thông báo");
                return;
            }
            else if (txtMKMoi.Text == "")
            {
                MessageBox.Show("Nhập mật khẩu mới!", "Thông báo");
                return;
            }
            else if (txtNLMKMoi.Text == "")
            {
                MessageBox.Show("Nhập lại mật khẩu mới!", "Thông báo");
                return;
            }
            else if (txtNLMKMoi.Text != txtMKMoi.Text)
            {
                MessageBox.Show("Nhập sai mật khẩu mới!", "Thông báo");
                return;
            }
            else
            {
                SqlDataAdapter MyAdapter;
                DataTable MyTable = new DataTable();
                //string strConnect = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                SqlConnection con = new SqlConnection(strConnect);
                MyAdapter = new SqlDataAdapter("SELECT password FROM userlogin WHERE username='admin'", con);
                MyAdapter.Fill(MyTable);
                if (MyTable.Rows[0][0].ToString() == toMD5(txtMKCu.Text))
                {
                    //string strConnect = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                    //SqlConnection con = new SqlConnection(strConnect);
                    String sql = "UPDATE userlogin SET password='" + toMD5(txtMKMoi.Text) + "' WHERE username='admin'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo!");
                    txtMKCu.Text = "";
                    txtMKMoi.Text = "";
                    txtNLMKMoi.Text = "";
                }
                else
                {
                    MessageBox.Show("Mật khẩu cũ không đúng!", "Thông báo!");
                }
            }
        }
        public static string toMD5(string pass)
        {
            MD5CryptoServiceProvider myMD5 = new MD5CryptoServiceProvider();
            byte[] myPass = System.Text.Encoding.UTF8.GetBytes(pass);
            myPass = myMD5.ComputeHash(myPass);

            StringBuilder s = new StringBuilder();
            foreach (byte p in myPass)
            {
                s.Append(p.ToString("x").ToLower());
            }
            return s.ToString();
        }

        private void txtMKCu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnXacNhan_Click(null, null);
                e.Handled = true;
            }
        }

        private void txtMKMoi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnXacNhan_Click(null, null);
                e.Handled = true;
            }
        }

        private void txtNLMKMoi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnXacNhan_Click(null, null);
                e.Handled = true;
            }
        }
    }
}
