using MayChamCong;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Configuration;

namespace MayChamCongMaster
{
    public partial class Login : Form
    {
        private string strConnect = ConfigurationManager.ConnectionStrings["ConnectDBSQL"].ConnectionString;
        AutoSize _form_resize;
        public Login()
        {
            InitializeComponent();
            this.Text = "LOGIN PAGE";
            _form_resize = new AutoSize(this);
            this.Load += _Load;
            this.Resize += _Resize;
        }

        private void _Load(object sender, EventArgs e)
        {
            _form_resize._get_initial_size();
        }

        private void _Resize(object sender, EventArgs e)
        {
            _form_resize._resize();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUserName.Text == "")
                {
                    MessageBox.Show("Tên đăng nhập không được để trống!", "Thông báo");
                    return;
                }
                else if (txtPassword.Text == "")
                {
                    MessageBox.Show("Mật khẩu không được để trống!", "Thông báo");
                    return;
                }
                SqlDataAdapter MyAdapter;
                DataTable MyTable = new DataTable();
                //string strConnect = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                SqlConnection con = new SqlConnection(strConnect);
                MyAdapter = new SqlDataAdapter("select count(*) from userlogin where username = '" + txtUserName.Text + "'and password = '" + toMD5(txtPassword.Text) + "'", con);
                MyAdapter.Fill(MyTable);
                if (MyTable.Rows[0][0].ToString() == "1")
                {
                    MasterPage f = new MasterPage();
                    this.Hide();
                    f.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Tài khoản hoặc mật khẩu không đúng!", "Đăng nhập thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Đăng nhập thất bại!");
            }
        }

        private void MasterPage_Load(object sender, EventArgs e)
        {

        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDangNhap_Click(null, null);
                e.Handled = true;
            }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDangNhap_Click(null, null);
                e.Handled = true;
            }
        }

        //đây là hàm để mã hóa
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

        private void MasterPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Close();
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
