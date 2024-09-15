using MayChamCong;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MayChamCongMaster
{
    public partial class Malls : Form
    {
        private string strConnect = ConfigurationManager.ConnectionStrings["ConnectDBSQL"].ConnectionString;
        AutoSize _form_resize;
        public Malls()
        {
            InitializeComponent();

            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            DataTable Dv = null;
            Dv = new Connect().ThongtinMalls();
            gridviewmall.DataSource = Dv;
            gridviewmall.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridviewmall.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //AutoSize
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            SqlDataAdapter MyAdapterConnect;
            DataTable MyTableMallsID = new DataTable();
            MyAdapterConnect = new SqlDataAdapter("SELECT MallsID FROM Malls", con);
            MyAdapterConnect.Fill(MyTableMallsID);
            //string DELETEDEIVCES = "DETELE DEVICES";
            //cmd.CommandText = DELETEDEIVCES;
            //cmd.ExecuteNonQuery();
            try
            {
                for (int i = 0; i < gridviewmall.Rows.Count - 1; i++)
                {
                    string mallid = gridviewmall.Rows[i].Cells[0].Value.ToString();
                    string mallname = gridviewmall.Rows[i].Cells[1].Value.ToString();
                    string malladdress = gridviewmall.Rows[i].Cells[2].Value.ToString();
                    

                    if (mallid == "" || mallname == ""|| malladdress=="")
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Không được để trống");
                        return;
                    }

                    int dem = 0;
                    for (int j = 0; j < MyTableMallsID.Rows.Count; j++)
                    {
                        if (gridviewmall.Rows[i].Cells[0].Value.ToString() == MyTableMallsID.Rows[j][0].ToString())
                        {
                            dem++;
                        }
                    }
                    if (dem == 0)
                    {
                        try
                        {
                            //insert
                            string INSERTMalls = "INSERT INTO Malls (MallsID,MallsName,MallsAddress) VALUES ('" + mallid + "',N'" + mallname + "',N'" + malladdress + "')";
                            cmd.CommandText = INSERTMalls;
                            cmd.ExecuteNonQuery();

                        }
                        catch
                        {
                            MessageBox.Show("Insert failed!", "Error");
                        }
                    }
                    else
                    {
                        try
                        {
                            //update
                            string UPDATEMalls = "UPDATE Malls SET MallsName=N'" + mallname + "',MallsAddress=N'" + malladdress + "' where MallsID='" + mallid + "'";
                            cmd.CommandText = UPDATEMalls;
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("Update failed!", "Error");
                        }
                    }
                }
                MessageBox.Show("Successed", "Thông báo");
            }
            catch
            {

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            try
            {
                for (int i = 0; i < gridviewmall.SelectedRows.Count; i++)
                {
                    string mallsid = gridviewmall.SelectedRows[i].Cells[0].Value.ToString();
                    //delete
                    string DELETEMalls = "DELETE Malls WHERE MallsID = '" + mallsid + "'";
                    cmd.CommandText = DELETEMalls;
                    cmd.ExecuteNonQuery();
                }
                //refresh gridview
                DataTable Dv = null;
                Dv = new Connect().ThongtinMalls();
                gridviewmall.DataSource = Dv;
                MessageBox.Show("Deleted successed!", "Thông báo");
            }
            catch
            { }
        }
    }
}
